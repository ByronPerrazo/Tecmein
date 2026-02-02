using Microsoft.AspNetCore.Mvc;
using BLL.Interfaces;
using System.Threading.Tasks;
using Entity;
using AutoMapper;
using TecmeinAplicacionWeb.Models.ViewModels;
using TecmeinWebApp.Utilidades.ViewComponents;
using TecmeinWebApp.Utilidades.Response;
using System.Collections.Generic;

namespace TecmeinAplicacionWeb.Controllers
{
    public class ActivoClienteController : Controller
    {
        private readonly IActivoClienteService _activoClienteService;
        private readonly IMapper _mapper;

        public ActivoClienteController(IActivoClienteService activoClienteService, IMapper mapper)
        {
            _activoClienteService = activoClienteService;
            _mapper = mapper;
        }

        [ValidatePermission("VER_MENU")] // Asumiendo que hay un permiso para ver activos
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> ListarActivosPorCliente(int secCliente)
        {
            try
            {
                var lista = await _activoClienteService.ObtenerPorCliente(secCliente);
                var vmLista = _mapper.Map<List<ActivoClienteVM>>(lista); // Necesitará ActivoClienteVM
                return StatusCode(StatusCodes.Status200OK, new { data = vmLista });
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> Obtener(int id)
        {
            try
            {
                var activoCliente = await _activoClienteService.Obtener(id);
                if (activoCliente == null)
                {
                    return Json(new { estado = false, mensajes = "Activo de cliente no encontrado." });
                }
                var vm = _mapper.Map<ActivoClienteVM>(activoCliente);
                return Json(new { estado = true, objeto = vm });
            }
            catch (System.Exception ex)
            {
                return Json(new { estado = false, mensajes = ex.Message });
            }
        }

        [HttpPost]
        [ValidatePermission("CREAR")]
        public async Task<IActionResult> Crear([FromBody] ActivoClienteVM vmActivoCliente)
        {
            try
            {
                var activoCliente = _mapper.Map<ActivoCliente>(vmActivoCliente);
                var activoClienteCreado = await _activoClienteService.Crear(activoCliente);
                if (activoClienteCreado.IdActivoCliente == 0)
                {
                    return Json(new { estado = false, mensajes = "No se pudo crear el activo del cliente." });
                }
                var vmCreado = _mapper.Map<ActivoClienteVM>(activoClienteCreado);
                return Json(new { estado = true, objeto = vmCreado, mensajes = "Activo de cliente creado exitosamente." });
            }
            catch (System.Exception ex)
            {
                return Json(new { estado = false, mensajes = ex.Message });
            }
        }

        [HttpPut]
        [ValidatePermission("ACTUALIZAR")]
        public async Task<IActionResult> Editar([FromBody] ActivoClienteVM vmActivoCliente)
        {
            try
            {
                var activoCliente = _mapper.Map<ActivoCliente>(vmActivoCliente);
                var activoClienteEditado = await _activoClienteService.Editar(activoCliente);
                if (activoClienteEditado == null || activoClienteEditado.IdActivoCliente == 0) // O alguna verificación de éxito
                {
                    return Json(new { estado = false, mensajes = "No se pudo editar el activo del cliente." });
                }
                return Json(new { estado = true, mensajes = "Activo de cliente editado exitosamente." });
                return Json(new { estado = true, mensajes = "Activo de cliente editado exitosamente." });
            }
            catch (System.Exception ex)
            {
                return Json(new { estado = false, mensajes = ex.Message });
            }
        }

        [HttpDelete]
        [ValidatePermission("ELIMINAR")]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                bool resultado = await _activoClienteService.Eliminar(id);
                return Json(new { estado = resultado, mensajes = resultado ? "Activo de cliente eliminado exitosamente." : "No se pudo eliminar el activo del cliente." });
            }
            catch (System.Exception ex)
            {
                return Json(new { estado = false, mensajes = ex.Message });
            }
        }
    }
}
