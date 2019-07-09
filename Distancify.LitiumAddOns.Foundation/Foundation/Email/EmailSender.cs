using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using Distancify.SerilogExtensions;
using Litium.Foundation.Configuration;
using Litium.ServiceBus;

namespace Distancify.LitiumAddOns.Foundation.Foundation.Email
{
    public class EmailSender : IEmailSender
    {
        private const string QUEUE_NAME = "EmailSenderEmail";

        private readonly EmailQueueProcessor _emailQueueProcessor;
        private readonly ServiceBusQueue<ValidEmail> _queue;

        public EmailSender(ServiceBusFactory serviceBusFactory)
        {
            var host = GeneralConfig.Instance.SMTPServer;
            var enableSsl = GeneralConfig.Instance.EnableSslForSMTP;
            var username = GeneralConfig.Instance.SMTPUsername;
            var password = GeneralConfig.Instance.SMTPPassword;

            _emailQueueProcessor = new EmailQueueProcessor(host, enableSsl, username, password);
            _queue = serviceBusFactory.CreateQueue(new ServiceBusOptions<ValidEmail>(QUEUE_NAME, _emailQueueProcessor.Process));
        }

        public void Send(ValidEmail email)
        {
            _queue.Send(email);
        }

        private class EmailQueueProcessor
        {
            private const SmtpDeliveryMethod DELIVERY_METHOD = SmtpDeliveryMethod.Network;
            private const bool USE_DEFAULT_CREDENTIALS = false;//In order to allow mail to be sent to the server anonymously
            private SmtpClient SmtpClient;

            public EmailQueueProcessor(string host, bool enableSsl, string username, string password)
            {
                if (string.IsNullOrEmpty(host))
                {
                    throw new ConfigurationErrorsException("Host is not set.");
                }

                SmtpClient = new SmtpClient()
                {
                    Host = host,
                    //Port = 465,//Uncomment if you need to use mailtrap or such for testing and port 25 is blocked.
                    EnableSsl = enableSsl,
                    UseDefaultCredentials = USE_DEFAULT_CREDENTIALS,
                    DeliveryMethod = DELIVERY_METHOD
                };

                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    SmtpClient.Credentials = new NetworkCredential(username, password);
                }
            }

            public void Process(ServiceBusMessage<ValidEmail> serviceBusMessage)
            {
                try
                {
                    var email = serviceBusMessage.Message;

                    using (var mailMessage = email.ToMailMessage())
                    {
                        SmtpClient.Send(mailMessage);
                    }
                }
                catch (Exception ex)
                {
                    var message = "Exception occurred when attempting to send e-mail.";

                    if (ex is ConfigurationErrorsException || ex is SmtpException)
                    {
                        this.Log().Fatal(ex, message);
                    }
                    else
                    {
                        this.Log().Error(ex, message);
                    }
                }
            }
        }
    }
}