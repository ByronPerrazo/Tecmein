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
    public class PlantillaPreContratoController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IPlantillaPreContratoServices _plantillaServices;

        public PlantillaPreContratoController(IMapper mapper, IPlantillaPreContratoServices plantillaServices)
        {
            _mapper = mapper;
            _plantillaServices = plantillaServices;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            var listaPlantillaVM
               = _mapper.Map<List<PlantillaPreContratoVM>>(await _plantillaServices.Lista());
            return StatusCode(StatusCodes.Status200OK, new { data = listaPlantillaVM });
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] string modelo)
        {
            var gResponse = new GenericResponse<PlantillaPreContratoVM>();
            try
            {
                var plantillaVM = JsonConvert.DeserializeObject<PlantillaPreContratoVM>(modelo);
                PlantillaPreContrato plantillaCreada = await _plantillaServices.Crear(_mapper.Map<PlantillaPreContrato>(plantillaVM));
                gResponse.Estado = true;
                gResponse.Objeto = _mapper.Map<PlantillaPreContratoVM>(plantillaCreada);
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
            var gResponse = new GenericResponse<PlantillaPreContratoVM>();
            try
            {
                PlantillaPreContratoVM? plantillaVM = JsonConvert.DeserializeObject<PlantillaPreContratoVM>(modelo);
                var plantillaEditada = await _plantillaServices.Editar(_mapper.Map<PlantillaPreContrato>(plantillaVM));
                gResponse.Estado = true;
                gResponse.Objeto = _mapper.Map<PlantillaPreContratoVM>(plantillaEditada);
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
                bool eliminado = await _plantillaServices.Eliminar(secuencial);
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
