using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TecmeinWebApp.Utilidades.Response;
using TecmeinWebApp.Utilidades.ViewComponents;

namespace TecmeinWebApp.Controllers
{
    [Authorize]
    public class PlantillaController : Controller
    {
        [ValidatePermission("LEER")]
        public IActionResult EnviarClave(string correo, string clave)
        {
            ViewData["Correo"] = correo;
            ViewData["Clave"] = clave;
            ViewData["Url"] = $"{this.Request.Scheme}://{this.Request.Host}";
            return View();
        }
        [ValidatePermission("LEER")]
        public IActionResult RestablecerClave(string clave)
        {
            ViewData["Clave"] = clave;
            return View();
        }
    }
}
