using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TecmeinWebApp.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using AutoMapper; // AÑADIDO
using TecmeinWebApp.Models.ViewModel;
using Entity; // AÑADIDO

namespace TecmeinWebApp.Controllers;

[Authorize] 
public class HomeController : Controller
{
    private readonly IUsuarioServices _usuarioServices;
    private readonly ILogger<HomeController> _logger;
    private readonly IMapper _mapper; // AÑADIDO

    public HomeController(ILogger<HomeController> logger, IUsuarioServices usuarioServices, IMapper mapper) // CONSTRUCTOR MODIFICADO
    {
        _logger = logger;
        _usuarioServices = usuarioServices;
        _mapper = mapper; // AÑADIDO
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Perfil()
    {
        string idUsuario = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // 1. Obtener la entidad Usuario desde el servicio
        var usuario = await _usuarioServices.ObtenerPorId(int.Parse(idUsuario));

        // 2. Mapear la entidad al ViewModel que la vista espera
        var vmUsuario = _mapper.Map<Usuario>(usuario);

        return View(vmUsuario);
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerUsuario()
    {
        string idUsuario = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var usuario = await _usuarioServices.ObtenerPorId(int.Parse(idUsuario));
        var vmUsuario = _mapper.Map<UsuarioVM>(usuario);

        return StatusCode(StatusCodes.Status200OK, new { estado = true, objeto = vmUsuario });
    }

    public async Task<IActionResult> Salir()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Acceso");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [AllowAnonymous] // Permitir acceso sin autenticación/autorización
    public IActionResult AccessDenied()
    {
        return View();
    }
}