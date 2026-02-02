using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TecmeinWebApp.Utilidades.Response;
using TecmeinWebApp.Utilidades.ViewComponents;

namespace TecmeinWebApp.Controllers
{
    public class PlantillaController : Controller
    {

        [AllowAnonymous]
        public IActionResult EnviarClave(string correo, string clave)
        {
            ViewData["Correo"] = correo;
            ViewData["Clave"] = clave;
            ViewData["Url"] = $"{this.Request.Scheme}://{this.Request.Host}";
            return View();
        }

        [AllowAnonymous]
        public IActionResult RestablecerClave(string clave)
        {
            ViewData["Clave"] = clave;
            return View();
        }
    }
}
