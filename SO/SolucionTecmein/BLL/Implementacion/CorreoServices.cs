using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using System.Net;
using System.Net.Mail;

namespace BLL.Implementacion
{
    public class CorreoServices : ICorreoServices
    {
        private IGenericRepository<Empresacorreo> _repositorio;
        public CorreoServices(IGenericRepository<Empresacorreo> repositorio)
        {
            _repositorio = repositorio;
        }
        public async Task<bool> EnvioCorreo(string Destino, string Asunto, string Mensaje)
        {
            var respuesta = false;
            try
            {
                var empresaCorreo =
                    await _repositorio
                            .Obtener(x =>
                                     x.SecEmpresa == 1);

                var credenciales =
                        new NetworkCredential(empresaCorreo.Email,
                                              empresaCorreo.Clave);

                var correo = new MailMessage()
                {
                    From = new MailAddress(empresaCorreo.Email), // correo de Origen
                    Subject = Asunto,
                    Body = Mensaje,
                    IsBodyHtml = true
                };

                correo.To.Add(Destino);

                var servidorCorreo
                    = new SmtpClient()
                    {
                        Host = empresaCorreo.Host,
                        Port = Convert.ToInt32(empresaCorreo.Puerto),
                        Credentials = credenciales,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        EnableSsl = true
                    };

                servidorCorreo.Send(correo);
                respuesta = true;
            }
            catch (Exception)
            {

                throw;
            }

            return respuesta;
        }
    }
}
