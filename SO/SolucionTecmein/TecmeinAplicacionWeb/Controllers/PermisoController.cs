using AutoMapper;
using BLL.Interfaces;
using Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TecmeinAplicacionWeb.Models.ViewModels;
using TecmeinWebApp.Utilidades.Response;
using TecmeinWebApp.Utilidades.ViewComponents;

namespace TecmeinAplicacionWeb.Controllers
{
    public class PermisoController : Controller
    {
        private readonly IPermisoServices _permisoServicio;
        private readonly IMapper _mapper;
        private readonly ILogger<PermisoController> _logger;

        public PermisoController(IPermisoServices permisoServicio, IMapper mapper, ILogger<PermisoController> logger)
        {
            _permisoServicio = permisoServicio;
            _mapper = mapper;
            _logger = logger;
        }

        [ValidatePermission("VER_MENU")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> Lista()
        {
            var gResponse = new GenericResponse<PermisoVM>();
            try
            {
                var listaPermisos = await _permisoServicio.Listar();
                gResponse.Estado = true;
                gResponse.ListaObjeto = _mapper.Map<List<PermisoVM>>(listaPermisos); // Corregido
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar permisos");
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message; // Corregido
            }
            return StatusCode(200, gResponse);
        }

        [HttpPost]
        [ValidatePermission("CREAR")]
        public async Task<IActionResult> Crear([FromBody] PermisoVM modelo)
        {
            var gResponse = new GenericResponse<PermisoVM>();
            try
            {
                var permisoEntidad = _mapper.Map<Permiso>(modelo);
                permisoEntidad = await _permisoServicio.Crear(permisoEntidad);

                gResponse.Estado = true;
                gResponse.Objeto = _mapper.Map<PermisoVM>(permisoEntidad);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear permiso", modelo);
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message; // Corregido
            }
            return StatusCode(200, gResponse);
        }

        [HttpPut]
        [ValidatePermission("ACTUALIZAR")]
        public async Task<IActionResult> Editar([FromBody] PermisoVM modelo)
        {
            var gResponse = new GenericResponse<PermisoVM>();
            try
            {
                var permisoEntidad = _mapper.Map<Permiso>(modelo);
                permisoEntidad = await _permisoServicio.Editar(permisoEntidad);

                gResponse.Estado = true;
                gResponse.Objeto = _mapper.Map<PermisoVM>(permisoEntidad);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al editar permiso", modelo);
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message; // Corregido
            }
            return StatusCode(200, gResponse);
        }

        [HttpDelete]
        [ValidatePermission("ELIMINAR")]
        public async Task<IActionResult> Eliminar(string idPermiso)
        {
            var gResponse = new GenericResponse<object>();
            try
            {
                gResponse.Estado = await _permisoServicio.Eliminar(idPermiso);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permiso", idPermiso);
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message; // Corregido
            }
            return StatusCode(200, gResponse);
        }
    }
}
