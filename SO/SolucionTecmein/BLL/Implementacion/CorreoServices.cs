using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using System.Net;
using System.Net.Mail;

namespace BLL.Implementacion
{
    public class CorreoServices : ICorreoServices
    {
        private readonly IGenericRepository<Empresacorreo> _repositorio;
        private readonly ISmtpClientWrapper _smtpClientWrapper;

        public CorreoServices(IGenericRepository<Empresacorreo> repositorio, ISmtpClientWrapper smtpClientWrapper)
        {
            _repositorio = repositorio;
            _smtpClientWrapper = smtpClientWrapper;
        }

        public async Task<bool> EnvioCorreo(string Destino, string Asunto, string Mensaje)
        {
            var empresaCorreo = await _repositorio.Obtener(x => x.SecEmpresa == 1)
                                ?? throw new InvalidOperationException("Configuración de correo empresarial no encontrada.");

            var correoEmpresarial = empresaCorreo.Email;

            var mailMessage = new MailMessage()
            {
                From = new MailAddress(address: correoEmpresarial),
                Subject = Asunto,
                Body = Mensaje,
                IsBodyHtml = true
            };

            mailMessage.To.Add(Destino);

            await _smtpClientWrapper.SendMailAsync(
                mailMessage,
                empresaCorreo.Host,
                Convert.ToInt32(empresaCorreo.Puerto),
                correoEmpresarial,
                empresaCorreo.Clave,
                true // EnableSsl
            );

            return true;
        }
    }
}
