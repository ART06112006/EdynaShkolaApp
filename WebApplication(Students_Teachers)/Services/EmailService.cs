using MailKit.Net.Smtp;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Threading.Tasks;
using Unity;
using WebApplication_Students_Teachers_.Models;

namespace WebApplication_Students_Teachers_.Services
{
    public class EmailService : IIdentityMessageService
    {
        private readonly SmtpSettings _smtpSettings;
        public EmailService()
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            _smtpSettings = configuration.GetSection("SmtpSettings").Get<SmtpSettings>();
        }

        public async Task SendAsync(IdentityMessage message)
        {
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = message.Body;

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_smtpSettings.FromName, _smtpSettings.FromEmail));
            emailMessage.To.Add(new MailboxAddress("", message.Destination));
            emailMessage.Subject = message.Subject;
            emailMessage.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_smtpSettings.UserName, _smtpSettings.Password);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
    }
}