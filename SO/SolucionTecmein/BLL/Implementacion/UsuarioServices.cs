using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;

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
                               , IAuditService auditService // AUDITORÍA
            )
        {
            _repositorio = repositorio;
            _storageServies = storageServies;
            _utilidadesServices = utilidadesServices;
            _correoServies = correoServies;
            _empresaStorageServices = empresaStorageServices;
            _auditService = auditService; // AUDITORÍA
        }

        private readonly IAuditService _auditService; // AUDITORÍA

        public async Task<List<Usuario>> Lista()
        {
            var query = await _repositorio.Consultar();
            return query.Include(x => x.SecRolNavigation).ToList();
        }
        public async Task<Usuario> Crear(Usuario entidad, Stream? imagen = null, string nombreImagen = "", string urlPantillaCorreo = "", int? idUsuarioAuditoria = null, string? nombreUsuarioAuditoria = null, string? direccionIpAuditoria = null)
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

                // Auditoría: Registro de creación de usuario
                await _auditService.RegistrarEventoAsync(
                    "CREACION_USUARIO",
                    idUsuarioAuditoria,
                    nombreUsuarioAuditoria,
                    $"Usuario creado: {usuarioGenerado.Nombre} ({usuarioGenerado.Correo}) con Rol: {usuarioGenerado.SecRolNavigation?.Descripcion}",
                    direccionIpAuditoria
                );

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
                var usuario = await _repositorio.Obtener(x => x.Secuencial == secuencialUsuario);

                if (usuario == null)
                {
                    return false; // Usuario no encontrado
                }

                if (!usuario.Clave.Equals(_utilidadesServices.ConvertirSha256(ClaveActual)))
                {
                    return false; // Contraseña actual incorrecta
                }

                usuario.Clave = _utilidadesServices.ConvertirSha256(ClaveNueva);

                return await _repositorio.Editar(usuario);
            }
            catch (Exception)
            {
                // Loggear la excepción si es necesario
                return false; // Error durante la operación
            }
        }


        public async Task<Usuario> Editar(Usuario entidad, Stream? Foto = null, string? NombreFoto = "", string cabeceraUrlCorreo = "", int? idUsuarioAuditoria = null, string? nombreUsuarioAuditoria = null, string? direccionIpAuditoria = null)
        {
            try
            {
                var usuarioOriginal = await _repositorio.Obtener(x => x.Secuencial == entidad.Secuencial, "SecRolNavigation");

                if (usuarioOriginal == null)
                    throw new TaskCanceledException("Usuario no encontrado");

                var usuarioConMismoCorreo = await _repositorio.Obtener(x => x.Correo == entidad.Correo);

                if (usuarioConMismoCorreo != null && usuarioConMismoCorreo.Secuencial != entidad.Secuencial)
                    throw new TaskCanceledException("Correo Ya Registrado");

                // Detección de cambios para auditoría
                var cambios = new List<string>();
                if (usuarioOriginal.Nombre != entidad.Nombre) cambios.Add($"Nombre: '{usuarioOriginal.Nombre}' -> '{entidad.Nombre}'");
                if (usuarioOriginal.Correo != entidad.Correo) cambios.Add($"Correo: '{usuarioOriginal.Correo}' -> '{entidad.Correo}'");
                if (usuarioOriginal.Telefono != entidad.Telefono) cambios.Add($"Teléfono: '{usuarioOriginal.Telefono}' -> '{entidad.Telefono}'");
                if (usuarioOriginal.SecRol != entidad.SecRol) cambios.Add($"Rol: '{usuarioOriginal.SecRolNavigation?.Descripcion}' -> '{entidad.SecRolNavigation?.Descripcion}'");
                if (usuarioOriginal.EsActivo != entidad.EsActivo) cambios.Add($"Estado: '{(usuarioOriginal.EsActivo == 1 ? "Activo" : "Inactivo")}' -> '{(entidad.EsActivo == 1 ? "Activo" : "Inactivo")}'");

                // Actualizar propiedades
                usuarioOriginal.Nombre = entidad.Nombre;
                usuarioOriginal.Correo = entidad.Correo;
                usuarioOriginal.SecRol = entidad.SecRol;
                usuarioOriginal.Telefono = entidad.Telefono;
                usuarioOriginal.EsActivo = entidad.EsActivo;

                var empresaStorage = await _empresaStorageServices.Consultar();
                var almacenamientoEmpresa = empresaStorage.FirstOrDefault(x => x.SecEmpresa == 1);
                if (almacenamientoEmpresa == null)
                    throw new TaskCanceledException($"Error Empresa No ha definido un FTP");

                if (Foto != null)
                {
                    usuarioOriginal.UrlFoto = await _storageServies.SubirStorage(Foto,
                                                                         almacenamientoEmpresa.CarpetaUsuario,
                                                                         NombreFoto);
                }

                // Lógica de correo para cambio de clave si el correo fue modificado
                if (cambios.Any(c => c.StartsWith("Correo:")))
                {
                    string urlPantillaCorreo = cabeceraUrlCorreo + $"/Plantilla/RestablecerClave?clave=[clave]";
                    string claveGenerada = _utilidadesServices.GenerarClave(8);
                    usuarioOriginal.Clave = _utilidadesServices.ConvertirSha256(claveGenerada);
                    await EnviarCorreoConPlantilla(urlPantillaCorreo, usuarioOriginal, almacenamientoEmpresa.SecEmpresaNavigation, true, claveGenerada);
                    cambios.Add("Clave: Restablecida por cambio de correo");
                }

                bool respuesta = await _repositorio.Editar(usuarioOriginal);

                if (!respuesta)
                    throw new TaskCanceledException("No se pudo editar el usuario");

                // Auditoría: Registro de edición de usuario
                if (cambios.Any())
                {
                    await _auditService.RegistrarEventoAsync(
                        "EDICION_USUARIO",
                        idUsuarioAuditoria,
                        nombreUsuarioAuditoria,
                        $"Usuario {usuarioOriginal.Nombre} (ID: {usuarioOriginal.Secuencial}) editado. Cambios: {string.Join("; ", cambios)}",
                        direccionIpAuditoria
                    );
                }

                var usuarioModificado = await _repositorio.Obtener(x => x.Secuencial == usuarioOriginal.Secuencial, "SecRolNavigation");

                return usuarioModificado;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> Eliminar(int secuencialUsuario, int? idUsuarioAuditoria = null, string? nombreUsuarioAuditoria = null, string? direccionIpAuditoria = null)
        {
            try
            {
                var usuarioAEliminar = await _repositorio.Obtener(u => u.Secuencial == secuencialUsuario);
                if (usuarioAEliminar == null)
                    throw new TaskCanceledException("Usuario no encontrado");

                bool respuesta = await _repositorio.Eliminar(usuarioAEliminar);

                if (respuesta)
                {
                    // Auditoría: Registro de eliminación de usuario
                    await _auditService.RegistrarEventoAsync(
                        "ELIMINACION_USUARIO",
                        idUsuarioAuditoria,
                        nombreUsuarioAuditoria,
                        $"Usuario eliminado: {usuarioAEliminar.Nombre} (ID: {usuarioAEliminar.Secuencial})",
                        direccionIpAuditoria
                    );
                }

                return respuesta;
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
        public async Task<Usuario?> ExistePorSecuencial(int secuencialUsuario)
        {
            var query = await _repositorio.Consultar();

            Usuario? usuario =
                      query.Where(x => x.Secuencial == secuencialUsuario)
                     .Include(x => x.SecRolNavigation)
                     .FirstOrDefault();

            return usuario;
        }
        public async Task<Usuario> ObtenerPorId(int secuencialUsuario)
        {
            return await _repositorio.Obtener(u => u.Secuencial == secuencialUsuario, "SecRolNavigation");
        }

        public async Task<Usuario> ObtenerPorCredenciales(string correo, string clave)
        {
            return await _repositorio.Obtener(x =>
                                               x.Correo.Equals(correo) &&
                                               x.Clave.Equals(_utilidadesServices.ConvertirSha256(clave)));
        }

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
