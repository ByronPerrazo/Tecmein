using AutoMapper;
using BLL.Interfaces;
using Entity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TecmeinAplicacionWeb.Models.ViewModels;
using TecmeinWebApp.Utilidades.Response;
using TecmeinWebApp.Utilidades.ViewComponents;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TecmeinWebApp.Controllers
{
    public class SeguimientoController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ISeguimientoServices _seguimientoServices;

        public SeguimientoController(IMapper mapper, ISeguimientoServices seguimientoServices)
        {
            _mapper = mapper;
            _seguimientoServices = seguimientoServices;
        }

        [ValidatePermission("VER_MENU")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> Lista(int secCotizacion)
        {
            var listaSeguimientoVM
               = _mapper.Map<List<SeguimientoVM>>(await _seguimientoServices.Lista(secCotizacion));
            return StatusCode(StatusCodes.Status200OK, new { data = listaSeguimientoVM });
        }

        [HttpPost]
        [ValidatePermission("CREAR")]
        public async Task<IActionResult> Crear([FromForm] string modelo)
        {
            var gResponse = new GenericResponse<SeguimientoVM>();
            try
            {
                var seguimientoVM = JsonConvert.DeserializeObject<SeguimientoVM>(modelo);
                Seguimiento seguimientoCreado = await _seguimientoServices.Crear(_mapper.Map<Seguimiento>(seguimientoVM));
                gResponse.Estado = true;
                gResponse.Objeto = _mapper.Map<SeguimientoVM>(seguimientoCreado);
                return StatusCode(StatusCodes.Status201Created, gResponse);
            }
            catch (System.Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
                return StatusCode(StatusCodes.Status400BadRequest, gResponse);
            }
        }

        [HttpPut]
        [ValidatePermission("ACTUALIZAR")]
        public async Task<IActionResult> Editar([FromForm] string modelo)
        {
            var gResponse = new GenericResponse<SeguimientoVM>();
            try
            {
                SeguimientoVM? seguimientoVM = JsonConvert.DeserializeObject<SeguimientoVM>(modelo);
                var seguimientoEditado = await _seguimientoServices.Editar(_mapper.Map<Seguimiento>(seguimientoVM));
                gResponse.Estado = true;
                gResponse.Objeto = _mapper.Map<SeguimientoVM>(seguimientoEditado);
                return StatusCode(StatusCodes.Status200OK, gResponse);
            }
            catch (System.Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
                return StatusCode(StatusCodes.Status400BadRequest, gResponse);
            }
        }

        [HttpDelete]
        [ValidatePermission("ELIMINAR")]
        public async Task<IActionResult> Eliminar(int secuencial)
        {
            var gResponse = new GenericResponse<string>();
            try
            {
                bool eliminado = await _seguimientoServices.Eliminar(secuencial);
                gResponse.Estado = eliminado;
                return StatusCode(StatusCodes.Status200OK, gResponse);
            }
            catch (System.Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
                return StatusCode(StatusCodes.Status400BadRequest, gResponse);
            }
        }
    }
}
