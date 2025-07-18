using BLL.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TecmeinWebApp.Models.ViewModel;

namespace TecmeinWebApp.Controllers
{
    public class AccesoController : Controller
    {
        private readonly IUsuarioServices _usuarioServices;
        private readonly IPermisosRolServices _permisosRolServices;

        public AccesoController(IUsuarioServices usuarioServices, IPermisosRolServices permisosRolServices)
        {
            _usuarioServices = usuarioServices;
            _permisosRolServices = permisosRolServices;
        }


        public IActionResult Login()
        {
            ClaimsPrincipal claimUser = HttpContext.User;

            if (claimUser.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
        public IActionResult RestablecerClave()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginUsuarioVM modelo)
        {
            var usuarioDetectado = await _usuarioServices.ObtenerPorCredenciales(modelo.Correo, modelo.Clave);
            if (usuarioDetectado == null)
            {
                ViewData["Mensaje"] = "Credenciales no registradas";
                return View();
            }
            ViewData["Mensaje"] = null;

            var claims = new List<Claim>() {
            new(ClaimTypes.Name, usuarioDetectado.Nombre),
            new(ClaimTypes.NameIdentifier, usuarioDetectado.Secuencial.ToString()),
            new(ClaimTypes.Role, usuarioDetectado.SecRol.ToString()),
            new("UrlFoto", usuarioDetectado.UrlFoto)
            };

            var permisosRol = await _permisosRolServices.PermisosRolActivo(usuarioDetectado.SecRol);

            if (permisosRol != null)
            {
                claims.Add(new Claim("CanConsult", (permisosRol.Consultar == 1).ToString()));
                claims.Add(new Claim("CanModify", (permisosRol.Modificar == 1).ToString()));
                claims.Add(new Claim("CanDelete", (permisosRol.Eliminar == 1).ToString()));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
                IsPersistent = modelo.MantenerSesionIniciada,
            };

            await HttpContext
                    .SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                 new ClaimsPrincipal(claimsIdentity),
                                 properties);

            return RedirectToAction("Index", "DashBoard");
        }

        [HttpPost]
        public async Task<IActionResult> RestablecerClave(LoginUsuarioVM modelo)
        {
            try
            {
                string urlPlatillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/RestablecerClave?clave=[clave]";
                bool resultado = await _usuarioServices.RestablecerClave(modelo.Correo, urlPlatillaCorreo);
                if (resultado)
                {
                    ViewData["Mensaje"] = "Su Contraseña Fue Restablecida, Los Datos de Acceso fueron enviados al correo ingresado";
                    ViewData["MensajeError"] = null;
                    return View();
                }
                else
                {
                    ViewData["Mensaje"] = null;
                    ViewData["MensajeError"] = "Lo sentimos el correo Ingresado no tenemos registrado";


                }
                ViewData["Mensaje"] = null;
            }
            catch (Exception ex)
            {
                ViewData["Mensaje"] = null;
                ViewData["MensajeError"] = ex.Message;
            }
            return View();
        }
    }

}
