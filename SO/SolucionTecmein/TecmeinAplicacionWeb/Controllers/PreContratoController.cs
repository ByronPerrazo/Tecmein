using AutoMapper;
using BLL.Interfaces;
using Entity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TecmeinWebApp.Models.ViewModel;
using TecmeinWebApp.Utilidades.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TecmeinWebApp.Controllers
{
    public class PreContratoController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IPreContratoServices _preContratoServices;

        public PreContratoController(IMapper mapper, IPreContratoServices preContratoServices)
        {
            _mapper = mapper;
            _preContratoServices = preContratoServices;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            var listaPreContratoVM
               = _mapper.Map<List<PreContratoVM>>(await _preContratoServices.Lista());
            return StatusCode(StatusCodes.Status200OK, new { data = listaPreContratoVM });
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] string modelo)
        {
            var gResponse = new GenericResponse<PreContratoVM>();
            try
            {
                var preContratoVM = JsonConvert.DeserializeObject<PreContratoVM>(modelo);
                PreContrato preContratoCreado = await _preContratoServices.Crear(_mapper.Map<PreContrato>(preContratoVM));
                gResponse.Estado = true;
                gResponse.Objeto = _mapper.Map<PreContratoVM>(preContratoCreado);
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
            var gResponse = new GenericResponse<PreContratoVM>();
            try
            {
                PreContratoVM? preContratoVM = JsonConvert.DeserializeObject<PreContratoVM>(modelo);
                var preContratoEditado = await _preContratoServices.Editar(_mapper.Map<PreContrato>(preContratoVM));
                gResponse.Estado = true;
                gResponse.Objeto = _mapper.Map<PreContratoVM>(preContratoEditado);
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
                bool eliminado = await _preContratoServices.Eliminar(secuencial);
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
        public async Task<IActionResult> GenerarDocumento(int secuencial)
        {
            var gResponse = new GenericResponse<string>();
            try
            {
                string filePath = await _preContratoServices.GenerarDocumentoWord(secuencial);
                gResponse.Estado = true;
                gResponse.Objeto = filePath;
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
