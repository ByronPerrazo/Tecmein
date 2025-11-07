using AutoMapper;
using BLL.Interfaces;
using Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims; // AÑADIDO
using TecmeinAplicacionWeb.Models.ViewModels; // Añadido para encontrar RolVM
using TecmeinAplicacionWeb.Models.ViewModels;
using TecmeinWebApp.Utilidades.Response;
using TecmeinWebApp.Utilidades.ViewComponents;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TecmeinWebApp.Controllers
{
    [Authorize]
    public class UsuarioController : Controller
    {
        private readonly IUsuarioServices _usuarioServices;
        private readonly IRolServices _rolServices;
        private readonly IMapper _mapper;

        public UsuarioController(

                                  IUsuarioServices usuarioServices,
                                  IRolServices rolServices,
                                  IMapper mapper,
                                  IAuditService auditService // AUDITORÍA
            )
        {
            _usuarioServices = usuarioServices;
            _rolServices = rolServices;
            _mapper = mapper;
            _auditService = auditService; // AUDITORÍA
        }

        private readonly IAuditService _auditService; // AUDITORÍA

        [ValidatePermission("VER_MENU")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> ListaRol()
        {
            List<RolVM> listaPerfilesVM
                = _mapper.Map<List<RolVM>>(await _rolServices.Lista());
            return StatusCode(StatusCodes.Status200OK, listaPerfilesVM);

        }


        [HttpGet]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> Lista()
        {
            var usuarioListaVM = _mapper.Map<List<UsuarioVM>>(await _usuarioServices.Lista());
            return StatusCode(StatusCodes.Status200OK, new { data = usuarioListaVM });
        }

        [HttpGet]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> ExisteUsuario(int secuencialUsuario)
        {
            var existe = _mapper.Map<UsuarioVM>(await _usuarioServices.ExistePorSecuencial(secuencialUsuario));
            return StatusCode(StatusCodes.Status200OK, existe);
        }


        [HttpGet]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> ObtenerParaEditar(int secuencialUsuario)
        {
            var gResponse = new GenericResponse<UsuarioEditarVM>();
            try
            {
                var usuario = await _usuarioServices.ObtenerPorId(secuencialUsuario);
                var roles = await _rolServices.Lista();

                var viewModel = new UsuarioEditarVM
                {
                    Usuario = _mapper.Map<UsuarioVM>(usuario),
                    ListaRoles = _mapper.Map<List<RolVM>>(roles)
                };

                gResponse.Estado = true;
                gResponse.Objeto = viewModel;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
            }
            var jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            return new JsonResult(gResponse, jsonOptions);
        }


        [HttpPost]
        [ValidatePermission("CREAR")]
        public async Task<IActionResult> Crear([FromForm] IFormFile imagen, [FromForm] string modelo)
        {
            var genericResponse = new GenericResponse<UsuarioVM>();
            try
            {
                UsuarioVM? usuariosVM = JsonConvert.DeserializeObject<UsuarioVM>(modelo);

                string nombreFoto = string.Empty;
                Stream? imagenStream = null;

                if (imagen != null)
                {
                    Console.WriteLine($"Imagen recibida: {imagen.FileName}, Tamaño: {imagen.Length} bytes");
                    string nombreCodificado = $"{usuariosVM.Secuencial.ToString()}-{Guid.NewGuid().ToString("N").Substring(0, 8)}";
                    string extension = Path.GetExtension(imagen.FileName);
                    nombreFoto = string.Concat(nombreCodificado, extension);
                    imagenStream = imagen.OpenReadStream();
                }
                else
                {
                    Console.WriteLine("No se recibió ninguna imagen para el nuevo usuario.");
                }

                var urlPantallaCorreo = $"{this.Request.Scheme}://" +
                                        $"{this.Request.Host}" +
                                        $"/Plantilla/EnviarClave?correo=[correo]&clave=[clave]";

                // Datos de auditoría
                var idUsuarioAuditoria = User.FindFirstValue(ClaimTypes.NameIdentifier) != null ? int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)) : (int?)null;
                var nombreUsuarioAuditoria = User.FindFirstValue(ClaimTypes.Name);
                var direccionIpAuditoria = HttpContext.Connection.RemoteIpAddress?.ToString();

                var usurioGenerado = await _usuarioServices.Crear(
                    _mapper.Map<Usuario>(usuariosVM),
                    imagenStream,
                    nombreFoto,
                    urlPantallaCorreo,
                    idUsuarioAuditoria,
                    nombreUsuarioAuditoria,
                    direccionIpAuditoria
                );
                usuariosVM = _mapper.Map<UsuarioVM>(usurioGenerado);

                genericResponse.Estado = true;
                genericResponse.Objeto = usuariosVM;
            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensajes = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }


        [HttpPut]
        [ValidatePermission("ACTUALIZAR")]
        public async Task<IActionResult> Editar([FromForm] IFormFile Foto, [FromForm] string modelo, string cabeceraUrlCorreo = "")
        {
            var genericResponse = new GenericResponse<UsuarioVM>();
            try
            {
                UsuarioVM? usuariosVM = JsonConvert.DeserializeObject<UsuarioVM>(modelo);

                string nombreFoto = string.Empty;
                Stream? imagenStream = null;

                if (Foto != null)
                {
                    string nombreCodificado = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(Foto.FileName);
                    nombreFoto = string.Concat(nombreCodificado, extension);
                    imagenStream = Foto.OpenReadStream();
                }
                var cabecera = $"{this.Request.Scheme}://{this.Request.Host}";

                // Datos de auditoría
                var idUsuarioAuditoria = User.FindFirstValue(ClaimTypes.NameIdentifier) != null ? int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)) : (int?)null;
                var nombreUsuarioAuditoria = User.FindFirstValue(ClaimTypes.Name);
                var direccionIpAuditoria = HttpContext.Connection.RemoteIpAddress?.ToString();

                Usuario usuarioEditado = await _usuarioServices.Editar(
                    _mapper.Map<Usuario>(usuariosVM),
                    imagenStream,
                    nombreFoto,
                    cabecera,
                    idUsuarioAuditoria,
                    nombreUsuarioAuditoria,
                    direccionIpAuditoria
                );
                usuariosVM = _mapper.Map<UsuarioVM>(usuarioEditado);
                genericResponse.Estado = true;
                genericResponse.Objeto = usuariosVM;
            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensajes = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }

        [HttpDelete]
        [ValidatePermission("ELIMINAR")]
        public async Task<IActionResult> Eliminar(int secuencialUsuario)
        {
            var gResponse = new GenericResponse<string>();
            try
            {
                // Datos de auditoría
                var idUsuarioAuditoria = User.FindFirstValue(ClaimTypes.NameIdentifier) != null ? int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)) : (int?)null;
                var nombreUsuarioAuditoria = User.FindFirstValue(ClaimTypes.Name);
                var direccionIpAuditoria = HttpContext.Connection.RemoteIpAddress?.ToString();

                gResponse.Estado = await _usuarioServices.Eliminar(secuencialUsuario, idUsuarioAuditoria, nombreUsuarioAuditoria, direccionIpAuditoria);
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
                throw;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }


    }
}
