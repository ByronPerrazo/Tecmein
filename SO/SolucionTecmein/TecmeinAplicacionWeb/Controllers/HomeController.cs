using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TecmeinWebApp.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using TecmeinWebApp.Models.ViewModel;
using TecmeinWebApp.Utilidades.Response;
using BLL.Interfaces;
using Entity;

namespace TecmeinWebApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        private readonly IMapper _mapper;
        private readonly IUsuarioServices _usuarioServicio;

        public HomeController(IUsuarioServices usuarioServicio, IMapper mapper)
        {
            _usuarioServicio = usuarioServicio;
            _mapper = mapper;


        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Perfil()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerUsuario()
        {
           var response = new GenericResponse<UsuarioVM>();
            try
            {
                ClaimsPrincipal claimsUser = HttpContext.User;
                string idUsuario = 
                       claimsUser
                       .Claims
                       .Where(x=> x.Type == ClaimTypes.NameIdentifier)
                       .Select(x=> x.Value).SingleOrDefault();

                var usuario = 
                    _mapper.Map<UsuarioVM>(await _usuarioServicio.ExistePorSecuencial( int.Parse(idUsuario)));

                response.Estado = true;
                response.Objeto = usuario;
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensajes = ex.Message;
               
            }
            return StatusCode(StatusCodes.Status200OK,response);
        }
        [HttpPost]
        public async Task<IActionResult> GuardarPerfil([FromBody] UsuarioVM modelo )
        {
            var response = new GenericResponse<UsuarioVM>();
            try
            {
                ClaimsPrincipal claimsUser = HttpContext.User;
                string idUsuario =
                       claimsUser
                       .Claims
                       .Where(x => x.Type == ClaimTypes.NameIdentifier)
                       .Select(x => x.Value).SingleOrDefault();

                var entiadadUsuario = _mapper.Map<Usuario>(modelo);
                    entiadadUsuario.Secuencial = int.Parse(idUsuario);
                
                bool resultado = await _usuarioServicio.GuardarPerfil(entiadadUsuario);

                response.Estado = resultado;
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensajes = ex.Message;

            }
            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpPost]
        public async Task<IActionResult> CambiarClave([FromBody] CambiarClaveVM modelo)
        {
            var response = new GenericResponse<bool>();
            try
            {
                ClaimsPrincipal claimsUser = HttpContext.User;
                string idUsuario =
                       claimsUser
                       .Claims
                       .Where(x => x.Type == ClaimTypes.NameIdentifier)
                       .Select(x => x.Value).SingleOrDefault();

                bool resultado = 
                    await _usuarioServicio
                           .CambiarClave( int.Parse(idUsuario),
                                          modelo.claveActual, 
                                          modelo.claveNueva );

                response.Estado = resultado;
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensajes = ex.Message;

            }
            return StatusCode(StatusCodes.Status200OK, response);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Salir()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login","Acceso");
        }

    }
}
