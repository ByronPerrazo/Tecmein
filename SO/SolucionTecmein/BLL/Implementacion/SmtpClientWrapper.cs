using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Azure.Identity;
using BLL.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Users.Item.SendMail;

namespace BLL.Implementacion
{
    public class SmtpClientWrapper : ISmtpClientWrapper
    {
        private readonly IConfiguration _configuration;

        public SmtpClientWrapper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendMailAsync(MailMessage mailMessage)
        {
            var tenantId = _configuration["MailGraph:TenantId"];
            var clientId = _configuration["MailGraph:ClientId"];
            var clientSecret = _configuration["MailGraph:ClientSecret"];
            var fromAddress = mailMessage.From.Address;

            if (string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                throw new InvalidOperationException("La configuración de MailGraph en appsettings.json no es válida.");
            }

            var clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);
            var graphServiceClient = new GraphServiceClient(clientSecretCredential);

            var graphMessage = new Message
            {
                Subject = mailMessage.Subject,
                Body = new ItemBody
                {
                    ContentType = mailMessage.IsBodyHtml ? BodyType.Html : BodyType.Text,
                    Content = mailMessage.Body
                },
                ToRecipients = mailMessage.To.Select(r => new Recipient { EmailAddress = new EmailAddress { Address = r.Address } }).ToList()
            };

            var sendMailBody = new SendMailPostRequestBody
            {
                Message = graphMessage,
                SaveToSentItems = true
            };

            await graphServiceClient.Users[fromAddress]
                .SendMail
                .PostAsync(sendMailBody);
        }
    }
}