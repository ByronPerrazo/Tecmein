using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TecmeinAplicacionWeb.Models.ViewModels;

namespace TecmeinWebApp.Controllers
{
    [AllowAnonymous]
    public class AccesoController : Controller
    {
        private readonly IUsuarioServices _usuarioServices;
        private readonly IGenericRepository<RolPermiso> _repositorioRolPermiso;
        private readonly IGenericRepository<RolMenu> _repositorioRolMenu; // NUEVO
                private readonly IMenuServices _menuServices; // NUEVO
                private readonly IAuditService _auditService; // AUDITORÍA
        
                public AccesoController(IUsuarioServices usuarioServices, 
                                        IGenericRepository<RolPermiso> repositorioRolPermiso,
                                        IGenericRepository<RolMenu> repositorioRolMenu, // NUEVO
                                        IMenuServices menuServices, // NUEVO
                                        IAuditService auditService) // AUDITORÍA
                {
                    _usuarioServices = usuarioServices;
                    _repositorioRolPermiso = repositorioRolPermiso;
                    _repositorioRolMenu = repositorioRolMenu; // NUEVO
                    _menuServices = menuServices; // NUEVO
                    _auditService = auditService; // AUDITORÍA
                }
        public IActionResult Login()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginUsuarioVM modelo)
        {
            var usuarioDetectado = await _usuarioServices.ObtenerPorCredenciales(modelo.Correo, modelo.Clave);
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "IP no disponible";

            if (usuarioDetectado == null)
            {
                await _auditService.RegistrarEventoAsync("LOGIN_FALLIDO", null, null, $"Intento de login con correo: {modelo.Correo}", ip);
                ViewData["Mensaje"] = "Credenciales no registradas";
                return View();
            }

            await _auditService.RegistrarEventoAsync("LOGIN_EXITOSO", usuarioDetectado.Secuencial, usuarioDetectado.Nombre, "Inicio de sesión correcto.", ip);

            var claims = new List<Claim>()
            {
                new(ClaimTypes.Name, usuarioDetectado.Nombre),
                new(ClaimTypes.NameIdentifier, usuarioDetectado.Secuencial.ToString()),
                new(ClaimTypes.Role, usuarioDetectado.SecRol.ToString()),
                new("UrlFoto", usuarioDetectado.UrlFoto)
            };

            // --- INICIO LÓGICA DE PERMISOS REFACTORIZADA ---
            var rolId = usuarioDetectado.SecRol.Value;

            // 1. Obtener permisos genéricos y específicos del rol (CREATE, READ, Roles.Administrar, etc.)
            var permisosDirectos = (await _repositorioRolPermiso.Consultar(p => p.SecRol == rolId))
                                       .Select(p => p.IdPermiso).ToList();

            // 2. Obtener los menús asignados al rol
            var idsMenusAsignados = (await _repositorioRolMenu.Consultar(rm => rm.SecRol == rolId))
                                        .Select(rm => rm.SecMenu.Value).ToHashSet();

            var menusAsignados = await _menuServices.ObtieneMenusPorIdsAsync(idsMenusAsignados);

            // 3. Añadir permisos directos/específicos como claims
            foreach (var permiso in permisosDirectos)
            {
                claims.Add(new Claim("Permission", permiso));
            }

            // 4. Añadir permisos concatenados (MENU_ACCION) para las políticas
            var permisosGenericos = new[] { "CREATE", "READ", "UPDATE", "DELETE" };
            foreach (var menu in menusAsignados)
            {
                if (!string.IsNullOrEmpty(menu.Controlador))
                {
                    foreach (var permiso in permisosDirectos.Intersect(permisosGenericos))
                    {
                        claims.Add(new Claim("Permission", $"{menu.Controlador.ToUpper()}_{permiso}"));
                    }
                }
            }
            // --- FIN LÓGICA DE PERMISOS ---

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
                IsPersistent = modelo.MantenerSesionIniciada,
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult RestablecerClave() => View();

        [HttpPost]
        public async Task<IActionResult> RestablecerClave(LoginUsuarioVM modelo)
        {
            try
            {
                string urlPlatillaCorreo = $"{Request.Scheme}://{Request.Host}/Plantilla/RestablecerClave?clave=[clave]";
                bool resultado = await _usuarioServices.RestablecerClave(modelo.Correo, urlPlatillaCorreo);
                if (resultado)
                {
                    ViewData["Mensaje"] = "Su Contraseña Fue Restablecida, Los Datos de Acceso fueron enviados al correo ingresado";
                }
                else
                {
                    ViewData["MensajeError"] = "Lo sentimos el correo Ingresado no tenemos registrado";
                }
            }
            catch (Exception ex)
            {
                ViewData["MensajeError"] = ex.Message;
            }
            return View();
        }
    }
}