using Microsoft.AspNetCore.Mvc;

namespace TecmeinWebApp.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
