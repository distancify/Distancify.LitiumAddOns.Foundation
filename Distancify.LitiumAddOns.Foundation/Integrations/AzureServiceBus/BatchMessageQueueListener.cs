using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using Distancify.SerilogExtensions;
using Litium.Owin.Lifecycle;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;


namespace Distancify.LitiumAddOns.Integrations.AzureServiceBus
{
    public abstract class BatchMessageQueueListener : IStartupTask
    {
        private MessageReceiver _messageReceiver;
        private BatchBlock<Message> _messageBatcher;
        private ActionBlock<Message[]> _messageProcessor;
        private Timer _triggerBatchTimer;

        protected abstract string ConnectionString { get; }
        protected abstract string QueueName { get; }
        protected abstract int NumberOfMessagesToBatchBeforeProcessing { get; }
        protected abstract int AutoTriggerBatchPeriodMs { get; }
        protected abstract int RetryMinimumBackoffMs { get; }
        protected abstract int RetryMaximumBackoffMs { get; }
        protected abstract int RetryMaximumCount { get; }
        protected abstract int PotentialUnhandledMessagesToReceiveAtATime { get; }

        public void Start()
        {
            if (string.IsNullOrEmpty(ConnectionString) || ConnectionString == AzureServiceBusConstants.ConnectionStringPlaceHolder)
            {
                this.Log().Fatal("AzureConnectionString not set - listening for updates on Azure ServiceBus queue {QueueName} is disabled", QueueName);
                return;
            }

            _messageBatcher = new BatchBlock<Message>(NumberOfMessagesToBatchBeforeProcessing);
            _messageProcessor = new ActionBlock<Message[]>(async messages => await ProcessMessages(messages));
            _messageBatcher.LinkTo(_messageProcessor);
            _messageBatcher.Completion.ContinueWith(t => _messageProcessor.Complete());
            _triggerBatchTimer = new Timer(new TimerCallback(TriggerBatch), null, 0, AutoTriggerBatchPeriodMs);

            try
            {
                var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
                {
                    MaxConcurrentCalls = 1,
                    AutoComplete = false,
                    MaxAutoRenewDuration = TimeSpan.FromMilliseconds(AutoTriggerBatchPeriodMs * 2)
                };

                var retryPolicy = new RetryExponential(TimeSpan.FromMilliseconds(RetryMinimumBackoffMs),
                                                        TimeSpan.FromMilliseconds(RetryMaximumBackoffMs),
                                                        RetryMaximumCount);
                _messageReceiver = new MessageReceiver(ConnectionString, QueueName, ReceiveMode.PeekLock, retryPolicy);
                _messageReceiver.RegisterMessageHandler(HandleMessage, messageHandlerOptions);

                Task.Factory.StartNew(async () => await ReceivePotentialUnhandledMessages());
            }
            catch (Exception ex)
            {
                this.Log().Error(ex, "Unable to initialize Azure Service Bus connection for queue {QueueName}", QueueName);
            }
        }

        private Task HandleMessage(Message message, CancellationToken token)
        {
            this.Log().Debug("Received message with ID {MessageId} for queue {QueueName}", message.MessageId, QueueName);

            if (!token.IsCancellationRequested)
            {
                _messageBatcher.SendAsync(message);
            }

            return Task.CompletedTask;
        }

        private void TriggerBatch(object state)
        {
            _messageBatcher.TriggerBatch();
        }

        private async Task ReceivePotentialUnhandledMessages()
        {
            this.Log().Debug("Starting to receive messages that were not handled from the previous startup for queue {QueueName}", QueueName);

            var messages = await _messageReceiver.ReceiveAsync(PotentialUnhandledMessagesToReceiveAtATime);

            if (messages != null && messages.Any())
            {
                this.Log().Debug("Receieved {Count} messages that were not handled from the previous startup for queue {QueueName}", messages.Count, QueueName);

                foreach (var message in messages)
                {
                    await _messageBatcher.SendAsync(message);
                }

                await ReceivePotentialUnhandledMessages();
            }
            else
            {
                this.Log().Debug("Receieved {Count} messages that were not handled from the previous startup for queue {QueueName}", 0, QueueName);
            }
        }

        private async Task ProcessMessages(Message[] messages)
        {
            var processingOutcome = TryProcessMessages(messages.ToList())
                .Select((processedSuccessfully, index) => new MessageAndProcessingOutcome
                {
                    Message = messages[index],
                    ProcessedSuccessfully = processedSuccessfully
                });

            var messagesThatWereProcessedSucessfully = processingOutcome
                .Where(e => e.ProcessedSuccessfully)
                .Select(e => e.Message).ToList();

            await CompleteMessages(messagesThatWereProcessedSucessfully);

            var messagesThatFailedToProcess = processingOutcome
                .Where(e => !e.ProcessedSuccessfully)
                .Select(e => e.Message).ToList();

            await AbandonMessages(messagesThatFailedToProcess);
        }

        private async Task CompleteMessages(List<Message> messages)
        {
            try
            {
                this.Log().Debug("Completing {Count} messages that were processed successfully for queue {QueueName}", messages.Count, QueueName);

                foreach (var message in messages)
                {
                    await _messageReceiver.CompleteAsync(message.SystemProperties.LockToken);
                }
            }
            catch (Exception ex)
            {
                this.Log().Error(ex, "Unable to complete Azure Service Bus messages for queue {QueueName}", QueueName);
            }
        }

        private async Task AbandonMessages(List<Message> messages)
        {
            try
            {
                this.Log().Debug("Abandoning {Count} messages that failed to process for queue {QueueName}", messages.Count, QueueName);

                foreach (var message in messages)
                {
                    await _messageReceiver.AbandonAsync(message.SystemProperties.LockToken);
                }
            }
            catch (Exception ex)
            {
                this.Log().Error(ex, "Unable to complete Azure Service Bus messages for queue {QueueName}", QueueName);
            }
        }

        private class MessageAndProcessingOutcome
        {
            public Message Message { get; set; }
            public bool ProcessedSuccessfully { get; set; }
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs e)
        {
            var context = e.ExceptionReceivedContext;

            this.Log().ForContext("Endpoint", context.Endpoint)
                .ForContext("EntityPath", context.EntityPath)
                .ForContext("ExecutingAction", context.Action)
                .Error(e.Exception, "An exception occurred during message handling from Azure ServiceBus queue {QueueName}", QueueName);

            return Task.CompletedTask;
        }

        private List<bool> TryProcessMessages(List<Message> messages)
        {
            this.Log().Debug("Trying to process {Count} messages for queue {QueueName}", messages.Count, QueueName);

            try
            {
                return ProcessMessages(messages);
            }
            catch (Exception ex)
            {
                this.Log().Error(ex, "Unable to process messages from Azure ServiceBus queue {QueueName}", QueueName);
                return messages.Select(message => false).ToList();
            }
        }

        protected abstract List<bool> ProcessMessages(List<Message> messages);
    }
}