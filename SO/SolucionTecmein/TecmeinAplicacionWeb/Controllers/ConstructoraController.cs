using AutoMapper;
using BLL.Interfaces;
using Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TecmeinWebApp.Models.ViewModel;
using TecmeinWebApp.Utilidades.Response;

namespace TecmeinWebApp.Controllers
{
    public class ConstructoraController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IConstructoraServices _constructoraServices;

        public ConstructoraController(IMapper mapper, IConstructoraServices constructoraServices)
        {
            _mapper = mapper;
            _constructoraServices = constructoraServices;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            var listaConstructoraVM
               = _mapper.Map<List<ConstructoraVM>>(await _constructoraServices.Lista());
            return StatusCode(StatusCodes.Status200OK, new { data = listaConstructoraVM });
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerPorSecuencial(int secuencial)
        {
            var gResponse = new GenericResponse<ConstructoraVM>();
            try
            {
                var constructoraVM = _mapper.Map<ConstructoraVM>(await _constructoraServices.ConstructoraPorSecuencial(secuencial));
                gResponse.Estado = true;
                gResponse.Objeto = constructoraVM;
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
            try
            {
                var constructoraVM = JsonConvert.DeserializeObject<ConstructoraVM>(modelo);
                Constructora constructoraCreada = await _constructoraServices.GuardarCambios(_mapper.Map<Constructora>(constructoraVM));
                var gResponse = new GenericResponse<ConstructoraVM>
                {
                    Estado = true,
                    Objeto = _mapper.Map<ConstructoraVM>(constructoraCreada)
                };
                return StatusCode(StatusCodes.Status201Created, gResponse);
            }
            catch (Exception ex)
            {
                var gResponse = new GenericResponse<ConstructoraVM> { Estado = false, Mensajes = ex.Message };
                return StatusCode(StatusCodes.Status400BadRequest, gResponse);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromForm] string modelo)
        {
            try
            {
                ConstructoraVM? constructoraVM = JsonConvert.DeserializeObject<ConstructoraVM>(modelo);
                var constructoraEditada = await _constructoraServices.Editar(_mapper.Map<Constructora>(constructoraVM));
                var gResponse = new GenericResponse<ConstructoraVM>
                {
                    Estado = true,
                    Objeto = _mapper.Map<ConstructoraVM>(constructoraEditada)
                };
                return StatusCode(StatusCodes.Status200OK, gResponse);
            }
            catch (Exception ex)
            {
                var gResponse = new GenericResponse<ConstructoraVM> { Estado = false, Mensajes = ex.Message };
                return StatusCode(StatusCodes.Status400BadRequest, gResponse);
            }
        }

        [HttpDelete]
        [Authorize(Policy = "CanDelete")]
        public async Task<IActionResult> Eliminar(int secuencial)
        {
            try
            {
                bool eliminado = await _constructoraServices.Eliminar(secuencial);
                var gResponse = new GenericResponse<string> { Estado = eliminado };
                return StatusCode(StatusCodes.Status200OK, gResponse);
            }
            catch (Exception ex)
            {
                var gResponse = new GenericResponse<string> { Estado = false, Mensajes = ex.Message };
                return StatusCode(StatusCodes.Status400BadRequest, gResponse);
            }
        }

    }
}
