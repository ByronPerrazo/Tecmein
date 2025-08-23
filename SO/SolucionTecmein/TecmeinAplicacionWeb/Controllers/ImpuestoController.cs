using AutoMapper;
using BLL.Interfaces;
using Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TecmeinWebApp.Models.ViewModel;
using TecmeinWebApp.Utilidades.Response;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TecmeinWebApp.Controllers
{
    [Authorize]
    public class ImpuestoController : Controller
    {
        private readonly IImpuestoServices _impuestoServices;
        private readonly ITipoImpuestoServices _tipoImpuestoServices;
        private readonly IMapper _mapper;

        public ImpuestoController(IImpuestoServices impuestoServices, ITipoImpuestoServices tipoImpuestoServices, IMapper mapper)
        {
            _impuestoServices = impuestoServices;
            _tipoImpuestoServices = tipoImpuestoServices;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            var impuestoLista = await _impuestoServices.Lista();
            var impuestoListaVM = _mapper.Map<List<ImpuestoVM>>(impuestoLista);

            var jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            return new JsonResult(new { data = impuestoListaVM }, jsonOptions);
        }

        [HttpGet]
        public async Task<IActionResult> ListaTipoImpuesto()
        {
            var listaTipoImpuestoVM = _mapper.Map<List<TipoImpuestoVM>>(await _tipoImpuestoServices.Lista());
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            return new JsonResult(listaTipoImpuestoVM, jsonOptions);
        }

        [HttpGet]
        public async Task<IActionResult> ListaActivos()
        {
            var impuestoLista = await _impuestoServices.ListaActivos();
            var impuestoListaVM = _mapper.Map<List<ImpuestoVM>>(impuestoLista);

            var jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            return new JsonResult(new { data = impuestoListaVM }, jsonOptions);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] ImpuestoVM modelo)
        {
            var genericResponse = new GenericResponse<ImpuestoVM>();
            try
            {
                Impuesto impuestoCreado = await _impuestoServices.Crear(_mapper.Map<Impuesto>(modelo));
                modelo = _mapper.Map<ImpuestoVM>(impuestoCreado);

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
        public async Task<IActionResult> Editar([FromBody] ImpuestoVM modelo)
        {
            var genericResponse = new GenericResponse<ImpuestoVM>();
            try
            {
                Impuesto impuestoEditado = await _impuestoServices.Editar(_mapper.Map<Impuesto>(modelo));
                modelo = _mapper.Map<ImpuestoVM>(impuestoEditado);

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
        public async Task<IActionResult> Eliminar(int id)
        {
            var gResponse = new GenericResponse<string>();
            try
            {
                gResponse.Estado = await _impuestoServices.Eliminar(id);
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