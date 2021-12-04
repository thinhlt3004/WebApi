using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Mail
{
    //Arguments for Sending a Mail 
    public class MailSettings
    {
        public string Mail { set; get; }
        public string DisplayName { set; get; }
        public string Password { set; get; }
        public string Host { set; get; }
        public int Port { set; get; }
    }

    public class SendMailService : IEmailSender
    {
        MailSettings mailSettings;
        public SendMailService(IOptions<MailSettings> mailSettings)
        {
            this.mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new MimeMessage();
            message.Sender = new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail);
            // Sender
            message.From.Add(new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail));
            // Receiver
            message.To.Add(MailboxAddress.Parse(email));
            // Handle Body of Content
            message.Subject = subject;
            // Handle Body
            var builder = new BodyBuilder();
            builder.HtmlBody = htmlMessage;
            message.Body = builder.ToMessageBody();

            // Using MailKit to send mail
            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            try
            {
                // Connection to Mail Server
                smtp.Connect(mailSettings.Host, mailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                // Authenticate Mail and Password App
                smtp.Authenticate(mailSettings.Mail, mailSettings.Password);
                // Send Mail
                await smtp.SendAsync(message);
            }
            catch (Exception ex)
            {
                // Handle Error
            }
            finally
            {
                //Close Connection
                smtp.Disconnect(true);
            }

        }
    }
}
