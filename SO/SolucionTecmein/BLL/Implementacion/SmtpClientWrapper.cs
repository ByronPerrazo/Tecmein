using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using BLL.Interfaces;

namespace BLL.Implementacion
{
    public class SmtpClientWrapper : ISmtpClientWrapper
    {
        public async Task SendMailAsync(MailMessage mailMessage, string host, int port, string userName, string password, bool enableSsl)
        {
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.Host = host;
                smtpClient.Port = port;
                smtpClient.Credentials = new NetworkCredential(userName, password);
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.EnableSsl = enableSsl;

                await smtpClient.SendMailAsync(mailMessage);
            }
        }
    }
}
