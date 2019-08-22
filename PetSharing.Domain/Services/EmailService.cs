using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PetSharing.Domain.Services
{
    public class EmailService
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Администрация сайта", "petsharing.world@outlook.com"));
            emailMessage.To.Add(new MailboxAddress("Dear User", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var smtp = new SmtpClient())
            {
                await smtp.ConnectAsync("smtp.office365.com", 587, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync("petsharing.world@outlook.com", "RUready4SEX");
                await smtp.SendAsync(emailMessage);
                await smtp.DisconnectAsync(true);
            }
        }
    }
}
