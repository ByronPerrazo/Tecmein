using AutoMapper;
using BLL.Interfaces;
using Entity;
using Microsoft.AspNetCore.Mvc;
using TecmeinWebApp.Models.ViewModel;
using TecmeinWebApp.Utilidades.Response;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http; // Required for StatusCode

namespace TecmeinWebApp.Controllers
{
    public class PlantillaPreContratoParrafoController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IPlantillaPreContratoParrafoServices _parrafoServices;

        public PlantillaPreContratoParrafoController(IMapper mapper, IPlantillaPreContratoParrafoServices parrafoServices)
        {
            _mapper = mapper;
            _parrafoServices = parrafoServices;
        }

        public IActionResult Parrafos(int id)
        {
            ViewBag.IdPlantilla = id;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Lista(int secPlantillaPreContrato)
        {
            var listaParrafos = await _parrafoServices.Lista(secPlantillaPreContrato);
            var listaParrafoVM = _mapper.Map<List<PlantillaPreContratoParrafoVM>>(listaParrafos);
            return Ok(new { data = listaParrafoVM });
        }

        [HttpGet]
        public async Task<IActionResult> Obtener(int secPlantillaPreContratoParrafo)
        {
            var gResponse = new GenericResponse<PlantillaPreContratoParrafoVM>();
            try
            {
                var parrafo = await _parrafoServices.Obtener(secPlantillaPreContratoParrafo);
                gResponse.Estado = true;
                gResponse.Objeto = _mapper.Map<PlantillaPreContratoParrafoVM>(parrafo);
                return StatusCode(StatusCodes.Status200OK, gResponse);
            }
            catch (System.Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
                return StatusCode(StatusCodes.Status400BadRequest, gResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] PlantillaPreContratoParrafoVM modelo)
        {
            var gResponse = new GenericResponse<PlantillaPreContratoParrafoVM>();
            try
            {
                var parrafoEntidad = _mapper.Map<PlantillaPreContratoParrafo>(modelo);
                var parrafoCreado = await _parrafoServices.Crear(parrafoEntidad);
                gResponse.Estado = true;
                gResponse.Objeto = _mapper.Map<PlantillaPreContratoParrafoVM>(parrafoCreado);
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
        public async Task<IActionResult> Editar([FromBody] PlantillaPreContratoParrafoVM modelo)
        {
            var gResponse = new GenericResponse<PlantillaPreContratoParrafoVM>();
            try
            {
                var parrafoEntidad = _mapper.Map<PlantillaPreContratoParrafo>(modelo);
                var parrafoEditado = await _parrafoServices.Editar(parrafoEntidad);
                gResponse.Estado = true;
                gResponse.Objeto = _mapper.Map<PlantillaPreContratoParrafoVM>(parrafoEditado);
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
        public async Task<IActionResult> Eliminar(int secPlantillaPreContratoParrafo)
        {
            var gResponse = new GenericResponse<string>();
            try
            {
                bool eliminado = await _parrafoServices.Eliminar(secPlantillaPreContratoParrafo);
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