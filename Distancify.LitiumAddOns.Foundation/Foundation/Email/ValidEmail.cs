﻿using System;
using System.Net.Mail;

namespace Distancify.LitiumAddOns.Foundation.Foundation.Email
{
    public class ValidEmail
    {
        public readonly string From;
        public readonly string To;
        public readonly string Subject;
        public readonly string Body;
        public readonly bool IsBodyHtml;

        public ValidEmail(string from, string to, string subject, string body, bool isBodyHtml)
        {
            if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to) || string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(body))
            {
                throw new ArgumentException("\"from\", \"to\", \"subject\" and \"body\" must all be non-empty strings.");
            }

            From = from;
            To = to;
            Subject = subject;
            Body = body;
            IsBodyHtml = isBodyHtml;
        }

        public MailMessage ToMailMessage()
        {
            var mailMessage = new MailMessage(From, To, Subject, Body)
            {
                IsBodyHtml = IsBodyHtml
            };

            return mailMessage;
        }
    }
}
