using System.Net.Mail;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface ISmtpClientWrapper
    {
        Task SendMailAsync(MailMessage mailMessage, string host, int port, string userName, string password, bool enableSsl);
    }
}
