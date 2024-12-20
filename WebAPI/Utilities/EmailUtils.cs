﻿using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;

namespace WebAPI.Utilities

{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;


        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {

            Task.Run(async () =>
            {
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
                emailMessage.To.Add(new MailboxAddress("", email));
                emailMessage.Subject = subject;
                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage };

                using (var client = new SmtpClient())
                {
                    try
                    {
                        await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, true);
                        await client.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.Password);
                        await client.SendAsync(emailMessage);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error sending email: {ex.Message}");
                    }
                    finally
                    {
                        await client.DisconnectAsync(true);
                    }
                }
            });
            return Task.CompletedTask;
        }

        // Email settings for the email sender
        public class EmailSettings
        {
            public string SmtpServer { get; set; }
            public int SmtpPort { get; set; }
            public string SenderName { get; set; }
            public string SenderEmail { get; set; }
            public string Password { get; set; }
        }
    }
}
