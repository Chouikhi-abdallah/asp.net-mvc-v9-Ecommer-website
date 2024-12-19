using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Proj.Services
{
    public class MailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPassword;
        private readonly string _smtpFrom;

        public MailService(IConfiguration configuration)
        {
            // Read settings from the configuration file (settings.json)
            var emailSettings = configuration.GetSection("EmailSettings");
            _smtpServer = emailSettings["Host"];
            _smtpPort = int.Parse(emailSettings["Port"]);
            _smtpUser = emailSettings["Username"];
            _smtpPassword = emailSettings["Password"];
            _smtpFrom = emailSettings["FromEmail"];
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpUser),
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };
            
            mailMessage.To.Add(toEmail);

            using (var smtpClient = new SmtpClient(_smtpServer))
            {
                smtpClient.Port = _smtpPort;
                smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPassword);
                smtpClient.EnableSsl = true;

                await smtpClient.SendMailAsync(mailMessage);
            }
        }
    }
}