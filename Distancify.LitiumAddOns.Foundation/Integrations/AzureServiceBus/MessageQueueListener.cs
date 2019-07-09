using System;
using System.Threading;
using System.Threading.Tasks;
using Distancify.SerilogExtensions;
using Litium.Owin.Lifecycle;
using Microsoft.Azure.ServiceBus;
using Polly;

namespace Distancify.LitiumAddOns.Foundation.Integrations.AzureServiceBus
{
    public abstract class MessageQueueListener : IStartupTask
    {
        private readonly int _maxConnectionRetries;
        private readonly string _connectionString;
        private readonly string _queueName;
        protected readonly string _listenerName;

        private QueueClient _azureQueueClient;

        protected MessageQueueListener(string connectionString, string queueName, string listenerName, int maxConnectionRetries)
        {
            _connectionString = connectionString;
            _queueName = queueName;
            _maxConnectionRetries = maxConnectionRetries;
            _listenerName = listenerName;
        }

        public void Start()
        {
            if (string.IsNullOrEmpty(_connectionString) || _connectionString == AzureServiceBusConstants.ConnectionStringPlaceHolder)
            {
                this.Log().Warning("Connection string not set - {ListenerName} is disabled", _listenerName);
                return;
            }
            if (string.IsNullOrEmpty(_queueName))
            {
                this.Log().Warning("Queue name not set - {ListenerName} is disabled", _listenerName);
                return;
            }

            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = true
            };

            var connectResult = Policy.Handle<Exception>()
                    .WaitAndRetry(_maxConnectionRetries, (retryCount) => TimeSpan.FromSeconds(retryCount))
                    .ExecuteAndCapture(() =>
                    {
                        _azureQueueClient = new QueueClient(_connectionString, _queueName);
                        _azureQueueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
                    });

            if (connectResult.Outcome == OutcomeType.Failure)
            {
                this.Log().Error(connectResult.FinalException, "Unable to initialize Azure Service Bus connection for {ListenerName}", _listenerName);
            }
        }

        protected abstract Task ProcessMessagesAsync(Message message, CancellationToken token);

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs e)
        {
            var context = e.ExceptionReceivedContext;

            this.Log().ForContext("Endpoint", context.Endpoint)
               .ForContext("EntityPath", context.EntityPath)
               .ForContext("ExecutingAction", context.Action)
               .Error(e.Exception, "Exception during Azure Service Bus message handling for queue {Queue}", _queueName);

            return Task.CompletedTask;
        }
    }
}