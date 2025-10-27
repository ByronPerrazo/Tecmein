using AutoMapper;
using BLL.Interfaces;
using Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TecmeinAplicacionWeb.Models.ViewModels;
using TecmeinWebApp.Utilidades.Response;
using TecmeinWebApp.Utilidades.ViewComponents;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TecmeinWebApp.Controllers
{
    [Authorize]
    public class TipoImpuestoController : Controller
    {
        private readonly ITipoImpuestoServices _tipoImpuestoServices;
        private readonly IMapper _mapper;

        public TipoImpuestoController(ITipoImpuestoServices tipoImpuestoServices, IMapper mapper)
        {
            _tipoImpuestoServices = tipoImpuestoServices;
            _mapper = mapper;
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
            var tipoImpuestoListaVM = _mapper.Map<List<TipoImpuestoVM>>(await _tipoImpuestoServices.Lista());
            var jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            return new JsonResult(new { data = tipoImpuestoListaVM }, jsonOptions);
        }

        [HttpPost]
        [ValidatePermission("CREAR")]
        public async Task<IActionResult> Crear([FromBody] TipoImpuestoVM modelo)
        {
            var genericResponse = new GenericResponse<TipoImpuestoVM>();
            try
            {
                TipoImpuesto tipoImpuestoCreado = await _tipoImpuestoServices.Crear(_mapper.Map<TipoImpuesto>(modelo));
                modelo = _mapper.Map<TipoImpuestoVM>(tipoImpuestoCreado);

                genericResponse.Estado = true;
                genericResponse.Objeto = modelo;
            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensajes = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }

        [HttpPut]
        [ValidatePermission("ACTUALIZAR")]
        public async Task<IActionResult> Editar([FromBody] TipoImpuestoVM modelo)
        {
            var genericResponse = new GenericResponse<TipoImpuestoVM>();
            try
            {
                TipoImpuesto tipoImpuestoEditado = await _tipoImpuestoServices.Editar(_mapper.Map<TipoImpuesto>(modelo));
                modelo = _mapper.Map<TipoImpuestoVM>(tipoImpuestoEditado);

                genericResponse.Estado = true;
                genericResponse.Objeto = modelo;
            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensajes = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }

        [HttpDelete]
        [ValidatePermission("ELIMINAR")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var gResponse = new GenericResponse<string>();
            try
            {
                gResponse.Estado = await _tipoImpuestoServices.Eliminar(id);
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }
    }
}