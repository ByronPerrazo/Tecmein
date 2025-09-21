using AutoMapper;
using BLL.Interfaces;
using Entity;
using Microsoft.AspNetCore.Mvc;
using TecmeinAplicacionWeb.Models.ViewModels;
using TecmeinWebApp.Utilidades.Response;

namespace TecmeinWebApp.Controllers
{
    public class PolizaGarantiaController : Controller
    {
        private readonly IPolizaGarantiaServices _polizaGarantiaServices;
        private readonly IMapper _mapper;

        public PolizaGarantiaController(IPolizaGarantiaServices polizaGarantiaServices, IMapper mapper)
        {
            _polizaGarantiaServices = polizaGarantiaServices;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            try
            {
                var lista = await _polizaGarantiaServices.Lista();
                var listaVM = _mapper.Map<List<PolizaGarantiaVM>>(lista);
                return StatusCode(StatusCodes.Status200OK, new { data = listaVM });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { data = new List<PolizaGarantiaVM>(), mensajes = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] PolizaGarantiaVM modelo)
        {
            var response = new GenericResponse<PolizaGarantiaVM>();
            try
            {
                var poliza = _mapper.Map<PolizaGarantia>(modelo);
                var polizaCreada = await _polizaGarantiaServices.Crear(poliza);
                response.Estado = true;
                response.Objeto = _mapper.Map<PolizaGarantiaVM>(polizaCreada);
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensajes = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromBody] PolizaGarantiaVM modelo)
        {
            var response = new GenericResponse<PolizaGarantiaVM>();
            try
            {
                var poliza = _mapper.Map<PolizaGarantia>(modelo);
                var polizaEditada = await _polizaGarantiaServices.Editar(poliza);
                response.Estado = true;
                response.Objeto = _mapper.Map<PolizaGarantiaVM>(polizaEditada);
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensajes = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int id)
        {
            var response = new GenericResponse<string>();
            try
            {
                response.Estado = await _polizaGarantiaServices.Eliminar(id);
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensajes = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpGet]
        public async Task<IActionResult> ListaParaDropdown()
        {
            try
            {
                var lista = await _polizaGarantiaServices.Lista();
                var listaFormateada = lista.Select(p => new { value = p.Secuencial, text = p.Descripcion }).ToList();
                return StatusCode(StatusCodes.Status200OK, new { data = listaFormateada });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { data = new List<object>(), mensajes = ex.Message });
            }
        }
    }
}