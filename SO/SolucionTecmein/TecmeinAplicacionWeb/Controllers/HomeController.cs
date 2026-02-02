using AutoMapper; // AÑADIDO
using BLL.Interfaces;
using Entity; // AÑADIDO
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using TecmeinAplicacionWeb.Models.ViewModels;
using TecmeinAplicacionWeb.Models.ViewModels;
using TecmeinWebApp.Utilidades.ViewComponents;

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

    public IActionResult TestError()
    {
        throw new Exception("Error de prueba generado intencionalmente para verificar el manejo de errores.");
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

    [HttpPost]
    public async Task<IActionResult> CambiarClave([FromBody] VMCambiarClave modelo)
    {
        try
        {
            string idUsuario = User.FindFirstValue(ClaimTypes.NameIdentifier);

            bool claveCambiada = await _usuarioServices.CambiarClave(int.Parse(idUsuario), modelo.claveActual, modelo.claveNueva);

            if (!claveCambiada)
            {
                return StatusCode(StatusCodes.Status200OK, new { estado = false, mensajes = "No se pudo cambiar la clave. Verifique su clave actual." });
            }

            return StatusCode(StatusCodes.Status200OK, new { estado = true, mensajes = "Clave cambiada con éxito." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en CambiarClave");
            return StatusCode(StatusCodes.Status500InternalServerError, new { estado = false, mensajes = "Error interno del servidor." });
        }
    }

    [HttpPost]
    public async Task<IActionResult> GuardarPerfil([FromBody] VMPerfil modelo)
    {
        try
        {
            string idUsuario = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var usuario = new Usuario()
            {
                Secuencial = int.Parse(idUsuario),
                Correo = modelo.correo,
                Telefono = modelo.telefono
            };

            bool perfilGuardado = await _usuarioServices.GuardarPerfil(usuario);

            if (!perfilGuardado)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { estado = false, mensajes = "No se pudo guardar el perfil." });
            }

            return StatusCode(StatusCodes.Status200OK, new { estado = true, mensajes = "Perfil guardado con éxito." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GuardarPerfil");
            return StatusCode(StatusCodes.Status500InternalServerError, new { estado = false, mensajes = "Error interno del servidor." });
        }
    }
}