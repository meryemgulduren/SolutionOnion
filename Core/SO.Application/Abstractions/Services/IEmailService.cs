using System.Threading.Tasks;

namespace SO.Application.Abstractions.Services
{
    public interface IEmailService
    {
        Task SendAsync(string toEmail, string subject, string htmlBody);
    }
}


