using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Distancify.SerilogExtensions;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace Distancify.LitiumAddOns.Foundation.Integrations.AzureServiceBus
{
    public abstract class UTF8SerializedObjectMessageQueueListener<T> : MessageQueueListener
    {
        private readonly bool _logMessages;

        protected UTF8SerializedObjectMessageQueueListener(string connectionString, string queueName, string listenerName, int maxConnectionRetries, bool logMessages)
            : base(connectionString, queueName, listenerName, maxConnectionRetries) {
            _logMessages = logMessages;
        }

        protected override Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            if (!token.IsCancellationRequested)
            {
                try
                {
                    var messageBody = Encoding.UTF8.GetString(message.Body);

                    if (_logMessages)
                    {
                        this.Log().Information("{ListenerName} recieved a message: {Message}", _listenerName, messageBody);
                    }

                    var messageObject = JsonConvert.DeserializeObject<T>(messageBody);
                    ProcessMessageObject(messageObject);
                }
                catch (Exception ex)
                {
                    this.Log().Error(ex, "An error occurred when processing a message in {ListenerName}", _listenerName);
                    throw ex;
                }
            }

            return Task.CompletedTask;
        }

        protected abstract void ProcessMessageObject(T messageObject);
    }
}