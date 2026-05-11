using System.Net.Mail;

namespace BLL.Interfaces
{
    public interface ISmtpClientWrapper
    {
        Task SendMailAsync(MailMessage mailMessage);
    }
}