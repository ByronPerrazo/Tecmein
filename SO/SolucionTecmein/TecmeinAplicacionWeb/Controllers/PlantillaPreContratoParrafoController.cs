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
    public class PlantillaPreContratoParrafoController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IPlantillaPreContratoParrafoServices _parrafoServices;

        public PlantillaPreContratoParrafoController(IMapper mapper, IPlantillaPreContratoParrafoServices parrafoServices)
        {
            _mapper = mapper;
            _parrafoServices = parrafoServices;
        }

        [HttpGet]
        public async Task<IActionResult> Lista(int secPlantillaPreContrato)
        {
            var listaParrafoVM
               = _mapper.Map<List<PlantillaPreContratoParrafoVM>>(await _parrafoServices.Lista(secPlantillaPreContrato));
            return StatusCode(StatusCodes.Status200OK, new { data = listaParrafoVM });
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] string modelo)
        {
            var gResponse = new GenericResponse<PlantillaPreContratoParrafoVM>();
            try
            {
                var parrafoVM = JsonConvert.DeserializeObject<PlantillaPreContratoParrafoVM>(modelo);
                PlantillaPreContratoParrafo parrafoCreado = await _parrafoServices.Crear(_mapper.Map<PlantillaPreContratoParrafo>(parrafoVM));
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
        public async Task<IActionResult> Editar([FromForm] string modelo)
        {
            var gResponse = new GenericResponse<PlantillaPreContratoParrafoVM>();
            try
            {
                PlantillaPreContratoParrafoVM? parrafoVM = JsonConvert.DeserializeObject<PlantillaPreContratoParrafoVM>(modelo);
                var parrafoEditado = await _parrafoServices.Editar(_mapper.Map<PlantillaPreContratoParrafo>(parrafoVM));
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
        public async Task<IActionResult> Eliminar(int secuencial)
        {
            var gResponse = new GenericResponse<string>();
            try
            {
                bool eliminado = await _parrafoServices.Eliminar(secuencial);
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
