using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Lab_34
{
    public class EmailSender : IEmailSender
    {
        public EmailSender(IOptions<EmailConfiguration> configuration)
        {
            this.configuration = configuration.Value;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient(configuration.Server)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(configuration.Login, configuration.Password)
            };
            var message = new MailMessage
            {
                From = new MailAddress(configuration.Address),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };
            message.To.Add(email);
            return Task.Factory.StartNew(() => { client.Send(message); });
        }

        private EmailConfiguration configuration;
    }
}
