using Microsoft.Azure.ServiceBus;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distancify.SerilogExtensions;

namespace Distancify.LitiumAddOns.Integrations.AzureServiceBus
{
    public abstract class MessageQueueSender
    {
        protected abstract string ConnectionString { get; }
        protected abstract string QueueName { get; }

        private IQueueClient _queueClient;
        private bool _queueClientInitializationFailed = false;

        protected void Send(string messageBody, Encoding encoding = null)
        {
            Send(new List<string> { messageBody }, encoding);
        }

        protected void Send(IList<string> messageBodies, Encoding encoding = null)
        {
            var client = GetClient();

            if (client is IQueueClient)
            {
                if (encoding == null)
                {
                    encoding = Encoding.UTF8;
                }

                var messages = messageBodies.Select(messageBody => new Message(encoding.GetBytes(messageBody)))
                    .ToList();

                _queueClient.SendAsync(messages);
            }
        }

        private IQueueClient GetClient()
        {
            if (!_queueClientInitializationFailed)
            {
                if (_queueClient is IQueueClient)
                {
                    return _queueClient;
                }
                else if (!string.IsNullOrEmpty(ConnectionString) && ConnectionString != AzureServiceBusConstants.ConnectionStringPlaceHolder)
                {
                    _queueClient = new QueueClient(ConnectionString, QueueName);

                    return _queueClient;
                }
                else
                {
                    this.Log().Fatal("AzureConnectionString not set - listening for updates on Azure ServiceBus queue {QueueName} is disabled", QueueName);
                    _queueClientInitializationFailed = true;
                }
            }
            else
            {
                this.Log().Fatal("Cannot use message queue sender for queue {QueueName}, queue client initialization failed.", QueueName);
            }

            return null;
        }
    }
}