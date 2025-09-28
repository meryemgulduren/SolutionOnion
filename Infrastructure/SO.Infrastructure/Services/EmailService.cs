using Microsoft.Extensions.Options;
using SO.Application.Abstractions.Services;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SO.Infrastructure.Services
{
    public class EmailSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public string From { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendAsync(string toEmail, string subject, string htmlBody)
        {
            try
            {
                using var client = new SmtpClient(_settings.Host, _settings.Port)
                {
                    EnableSsl = _settings.EnableSsl,
                    Credentials = new NetworkCredential(_settings.UserName, _settings.Password),
                    Timeout = 10000 // 10 saniye timeout
                };

                var mail = new MailMessage(_settings.From, toEmail)
                {
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };

                await client.SendMailAsync(mail);
                Console.WriteLine($"Email başarıyla gönderildi: {toEmail}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email gönderimi başarısız: {ex.Message}");
                throw new Exception($"Email gönderilemedi: {ex.Message}");
            }
        }
    }
}
