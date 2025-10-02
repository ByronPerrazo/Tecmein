using AutoMapper;
using BLL.Interfaces;
using Entity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TecmeinAplicacionWeb.Models.ViewModels;
using TecmeinWebApp.Utilidades.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TecmeinWebApp.Controllers
{
    public class FormaPagoController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IFormaPagoServices _formaPagoServices;

        public FormaPagoController(IMapper mapper, IFormaPagoServices formaPagoServices)
        {
            _mapper = mapper;
            _formaPagoServices = formaPagoServices;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListaActivas()
        {
            var gResponse = new GenericResponse<List<FormaPagoVM>>();
            try
            {
                var listaFormaPago = await _formaPagoServices.Lista();
                var listaActivas = listaFormaPago.Where(fp => fp.EstaActivo == 1).ToList();
                var listaVm = _mapper.Map<List<FormaPagoVM>>(listaActivas);
                gResponse.Estado = true;
                gResponse.Objeto = listaVm;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            var gResponse = new GenericResponse<List<FormaPagoVM>>();
            try
            {
                var listaFormaPago = await _formaPagoServices.Lista();
                var listaVm = _mapper.Map<List<FormaPagoVM>>(listaFormaPago);
                gResponse.Estado = true;
                gResponse.Objeto = listaVm;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] string modelo)
        {
            var gResponse = new GenericResponse<FormaPagoVM>();
            try
            {
                var formaPagoVM = JsonConvert.DeserializeObject<FormaPagoVM>(modelo);
                FormaPago formaPagoCreada = await _formaPagoServices.Crear(_mapper.Map<FormaPago>(formaPagoVM));
                gResponse.Estado = true;
                gResponse.Objeto = _mapper.Map<FormaPagoVM>(formaPagoCreada);
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
        public async Task<IActionResult> Editar([FromForm] string modelo)
        {
            var gResponse = new GenericResponse<FormaPagoVM>();
            try
            {
                FormaPagoVM? formaPagoVM = JsonConvert.DeserializeObject<FormaPagoVM>(modelo);
                var formaPagoEditada = await _formaPagoServices.Editar(_mapper.Map<FormaPago>(formaPagoVM));
                gResponse.Estado = true;
                gResponse.Objeto = _mapper.Map<FormaPagoVM>(formaPagoEditada);
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
        public async Task<IActionResult> Eliminar(int secuencial)
        {
            var gResponse = new GenericResponse<string>();
            try
            {
                bool eliminado = await _formaPagoServices.Eliminar(secuencial);
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
        [HttpGet]
        public async Task<IActionResult> ListaParaDropdown()
        {
            try
            {
                var lista = await _formaPagoServices.Lista();
                var formasPago = lista.Where(fp => fp.EstaActivo == 1).Select(fp => new { value = fp.SecFormaPago, text = fp.Descripcion }).ToList();
                
                return StatusCode(StatusCodes.Status200OK, new { data = formasPago });
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = ex.Message });
            }
        }
    }
}
