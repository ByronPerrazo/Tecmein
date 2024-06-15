using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Implementacion
{

    public class UsuarioServices : IUsuarioServices
    {
        private readonly IGenericRepository<Usuario> _repositorio;
        private readonly IStorageServices _storageServies;
        private readonly IUtilidadesServices _utilidadesServices;
        private readonly ICorreoServices _correoServies;
        private readonly IEmpresaStorageServices _empresaStorageServices;


        public UsuarioServices(IGenericRepository<Usuario> repositorio
                               , IStorageServices storageServies
                               , IUtilidadesServices utilidadesServices
                               , ICorreoServices correoServies
                               , IEmpresaStorageServices empresaStorageServices
            )
        {
            _repositorio = repositorio;
            _storageServies = storageServies;
            _utilidadesServices = utilidadesServices;
            _correoServies = correoServies;
            _empresaStorageServices = empresaStorageServices;
        }

        public async Task<List<Usuario>> Lista()
        {
            var query = await _repositorio.Consultar();
            return query.Include(x => x.SecRolNavigation).ToList();
        }
        public async Task<Usuario> Crear(Usuario entidad, Stream? imagen = null, string nombreImagen = "", string urlPantillaCorreo = "")
        {
            try
            {
                var usuario
                    = await
                        _repositorio
                        .Obtener(x =>
                                 x.Correo == entidad.Correo);

                if (usuario != null)
                    throw new TaskCanceledException("Codigo Usuario/Correo Ya Registrado");

                string claveGenerada = _utilidadesServices.GenerarClave(8);
                entidad.Clave = _utilidadesServices.ConvertirSha256(claveGenerada);
                entidad.NombreFoto = string.IsNullOrEmpty(nombreImagen) ? $"{entidad.Nombre}_img" : nombreImagen;

                var empresaStorage = await _empresaStorageServices.Consultar();
                var almacenamientoEmpresa = empresaStorage.FirstOrDefault(x => x.SecEmpresa == 1);
                if (almacenamientoEmpresa == null)
                    throw new TaskCanceledException($"Error Empresa No ha definido un FTP");

                if (imagen != null)
                {
                    //var imagenTransformada = _utilidadesServices.ConvertToWebPComprimido(NombreFoto, "C:/", "C:/convert/");
                    entidad.UrlFoto = await _storageServies.SubirStorage(imagen,
                                                                         almacenamientoEmpresa.CarpetaUsuario,
                                                                         nombreImagen);
                }

                var usuarioGenerado = await _repositorio.Crear(entidad);

                if (usuarioGenerado.Secuencial == 0)
                    throw new TaskCanceledException($"Error Usuario {entidad.Correo} No se pudo Generar");

                urlPantillaCorreo = await EnviarCorreoConPlantilla(urlPantillaCorreo, usuarioGenerado, almacenamientoEmpresa.SecEmpresaNavigation, false, claveGenerada);


                var userAdquirido = await _repositorio.Consultar(x => x.Correo == usuarioGenerado.Correo);
                usuarioGenerado = userAdquirido.Include(x => x.SecRolNavigation).First();

                return usuarioGenerado;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<string> EnviarCorreoConPlantilla(string urlPlantilla, Usuario usuario, Empresa empresa, bool esRecuperarClave, string claveGenerada = "")
        {
            // Cargar la plantilla HTML desde la URL o archivo local
            string htmlCorreo = await CargarPlantillaHTML(urlPlantilla, usuario, esRecuperarClave, claveGenerada);

            // Asunto del correo
            string asunto = esRecuperarClave
                ? $"Restablecer Contraseña de {empresa.Nombre}"
                : $"Registro en el Sistema de {empresa.Nombre}";

            // Envío del correo con el HTML generado
            bool seEnvio = await _correoServies.EnvioCorreo(usuario.Correo, asunto, htmlCorreo);

            if (!seEnvio)
            {
                throw new TaskCanceledException("Error: Envio de correo fallido.");
            }
            return htmlCorreo;
        }
        private async Task<string> CargarPlantillaHTML(string urlPlantilla, Usuario usuario, bool esRecuperarClave, string claveGenerada)
        {
            // Realizar reemplazo en la URL si es necesario
            if (!string.IsNullOrWhiteSpace(urlPlantilla) && !esRecuperarClave)
            {
                urlPlantilla = urlPlantilla.Replace("[correo]", usuario.Correo).Replace("[clave]", claveGenerada);
            }
            else
            {
                urlPlantilla = urlPlantilla.Replace("[clave]", claveGenerada);
            }

            // Descargar y leer la plantilla HTML
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(urlPlantilla);
                response.EnsureSuccessStatusCode();
                string html = await response.Content.ReadAsStringAsync();
                return html;
            }
        }
        public async Task<bool> CambiarClave(int secuencialUsuario, string ClaveActual, string ClaveNueva)
        {
            try
            {
                var usuario =
                    _repositorio
                    .Obtener(x => x.Secuencial == secuencialUsuario)
                    .Result ??
                     throw new TaskCanceledException("Usuario No Registrado");

                if (!usuario.Clave.Equals(_utilidadesServices.ConvertirSha256(ClaveActual)))
                    throw new TaskCanceledException("Contraseña Incorrecta, Intente Otra Vez");

                usuario.Clave = _utilidadesServices.ConvertirSha256(ClaveNueva);

                return await _repositorio.Editar(usuario);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<Usuario> Editar(Usuario entidad, Stream? Foto = null, string? NombreFoto = "", string cabeceraUrlCorreo = "")
        {
            try
            {
                var usuario
                    = await _repositorio
                            .Consultar();

                if (usuario
                    .Any(x => x.Correo == entidad.Correo &&
                              x.Secuencial != entidad.Secuencial))
                    throw new TaskCanceledException("Correo Ya Registrado");

                var correoModificado
                    = usuario.Any(x =>
                                  x.Secuencial == entidad.Secuencial &&
                                  x.Correo != entidad.Correo);


                var usuarioProcesado = usuario.First(x => x.Secuencial == entidad.Secuencial);

                usuarioProcesado.Nombre = string.IsNullOrEmpty(entidad.Nombre) ? usuarioProcesado.Nombre : entidad.Nombre;
                usuarioProcesado.Correo = string.IsNullOrEmpty(entidad.Correo) ? usuarioProcesado.Correo : entidad.Correo;
                usuarioProcesado.SecRol = entidad.SecRol == 0 ? usuarioProcesado.SecRol : entidad.SecRol;
                usuarioProcesado.Telefono = string.IsNullOrEmpty(entidad.Telefono) ? usuarioProcesado.Telefono : entidad.Telefono;
                usuarioProcesado.EsActivo = entidad.EsActivo;


                var empresaStorage = await _empresaStorageServices.Consultar();
                var almacenamientoEmpresa = empresaStorage.FirstOrDefault(x => x.SecEmpresa == 1);
                if (almacenamientoEmpresa == null)
                    throw new TaskCanceledException($"Error Empresa No ha definido un FTP");


                if (Foto != null)
                {
                    //var imagenTransformada = _utilidadesServices.ConvertToWebPComprimido(NombreFoto, "C:/", "C:/convert/");
                    usuarioProcesado.UrlFoto = await _storageServies.SubirStorage(Foto,
                                                                                  almacenamientoEmpresa.CarpetaUsuario,
                                                                                  NombreFoto);
                }

                var urlPantillaCorreo = cabeceraUrlCorreo;

                if (correoModificado)
                {
                    urlPantillaCorreo += $"/Plantilla/RestablecerClave?clave=[clave]";
                    string claveGenerada = _utilidadesServices.GenerarClave(8);
                    usuarioProcesado.Clave = _utilidadesServices.ConvertirSha256(claveGenerada);
                    urlPantillaCorreo = await EnviarCorreoConPlantilla(urlPantillaCorreo, usuarioProcesado, almacenamientoEmpresa.SecEmpresaNavigation, true, claveGenerada);
                }

                var usuarioGenerado = await _repositorio.Editar(usuarioProcesado);

                var usuarioModificado
                      = await _repositorio
                             .Consultar(x => x.Secuencial == usuarioProcesado.Secuencial);
                usuarioProcesado = usuarioModificado.Include(x => x.SecRolNavigation).First();

                return usuarioProcesado;
            }
            catch (Exception)
            {

                throw;
            }

        }
        public async Task<bool> Eliminar(int secuencialUsuario)
        {
            try
            {
                var seElimino = false;
                var usuario
                    = await _repositorio
                             .Consultar(x => x.Secuencial == secuencialUsuario);

                var user = usuario.FirstOrDefault();
                if (user != null)
                {
                    var usuarioGenerado = await _repositorio.Eliminar(user);
                    seElimino = true;
                }
                return seElimino;
            }
            catch (Exception)
            {
                throw;
            }

        }
        public async Task<bool> GuardarPerfil(Usuario entidad)
        {
            try
            {
                var usuario =
                    await _repositorio
                          .Obtener(x => x.Secuencial == entidad.Secuencial)
                          ?? throw new TaskCanceledException("Usuario no Encontrado");

                usuario.Correo = entidad.Correo;
                usuario.Telefono = entidad.Telefono;

                return await _repositorio.Editar(usuario);
            }
            catch 
            {

                throw;
            }
        }
        public async Task<Usuario> ExistePorSecuencial(int secuencialUsuario)
        {
            var query = await _repositorio.Consultar();
            return query
                .Where(x => x.Secuencial == secuencialUsuario)
                .Include(x => x.SecRolNavigation)
                .FirstOrDefault();
        }
        public async Task<Usuario> OtenerPorCredenciales(string correo, string clave)
         => await _repositorio.Obtener(x =>
                                       x.Correo.Equals(correo) &&
                                       x.Clave.Equals(_utilidadesServices.ConvertirSha256(clave)));
        public async Task<bool> RestablecerClave(string? correoDestino, string urlPantillaCorreo = "")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(correoDestino))
                    return false;

                var usuario =
                    _repositorio
                    .Obtener(x => !(x.Correo == correoDestino))
                    .Result ??
                     throw new TaskCanceledException($"Correo {correoDestino} No Registrado");

                var claveGenerada = _utilidadesServices.GenerarClave(8);

                usuario.Clave = _utilidadesServices.ConvertirSha256(claveGenerada);

                var empresaStorage = await _empresaStorageServices.Consultar();
                var almacenamientoEmpresa = empresaStorage.FirstOrDefault(x => x.SecEmpresa == 1);
                if (almacenamientoEmpresa == null)
                    throw new TaskCanceledException($"Error Empresa No ha definido un FTP");


                urlPantillaCorreo = await EnviarCorreoConPlantilla(urlPantillaCorreo, usuario, almacenamientoEmpresa.SecEmpresaNavigation, true, claveGenerada);

                return await _repositorio.Editar(usuario);

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
