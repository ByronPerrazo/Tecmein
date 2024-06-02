using Microsoft.AspNetCore.Mvc;

namespace AplicacionWeb.Controllers
{
    public class AccesoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
