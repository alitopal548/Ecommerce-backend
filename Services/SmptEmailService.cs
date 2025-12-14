using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly SmtpClient _smtpClient;
        private readonly string _fromAddress;
        private readonly string _fromName;

        public SmtpEmailService(IConfiguration config)
        {
            // appsettings.json'daki EmailSettings bölümünü oku
            var section = config.GetSection("EmailSettings");
            _fromAddress = section["From"];
            _fromName = section["FromName"];

            // Gmail SMTP ayarları
            _smtpClient = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(
                    section["Username"],
                    section["Password"]),
                EnableSsl = true
            };
        }

        public async Task SendEmailAsync(string to, string subject, string htmlContent)
        {
            var mail = new MailMessage()
            {
                From = new MailAddress(_fromAddress, _fromName),
                Subject = subject,
                Body = htmlContent,
                IsBodyHtml = true
            };
            mail.To.Add(to);

            await _smtpClient.SendMailAsync(mail);
        }
    }

    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string htmlContent);
    }
}
