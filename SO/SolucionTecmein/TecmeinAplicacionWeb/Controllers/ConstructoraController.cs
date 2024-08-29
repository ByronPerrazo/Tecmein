using AutoMapper;
using BLL.Implementacion;
using BLL.Interfaces;
using Entity;
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
            var gResponse = new GenericResponse<ConstructoraVM>();
            try
            {
                var constructoraVM = JsonConvert.DeserializeObject<ConstructoraVM>(modelo);



                Constructora empresaEcontrada = await _constructoraServices.GuardarCambios(_mapper.Map<Constructora>(constructoraVM));

                constructoraVM = _mapper.Map<ConstructoraVM>(empresaEcontrada);

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

        [HttpPut]
        public async Task<IActionResult> Editar( [FromForm] string modelo)
        {
            var genericResponse = new GenericResponse<ConstructoraVM>();
            try
            {
                ConstructoraVM? constructoraVM = JsonConvert.DeserializeObject<ConstructoraVM>(modelo);

                var tipoProdEdit = await _constructoraServices.Editar(_mapper.Map<Constructora>(constructoraVM));
                constructoraVM = _mapper.Map<ConstructoraVM>(tipoProdEdit);

                genericResponse.Estado = true;
                genericResponse.Objeto = constructoraVM;
            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensajes = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int secuencial)
        {
            var gResponse = new GenericResponse<string>();
            try
            {
                gResponse.Estado = await _constructoraServices.Eliminar(secuencial);
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
                throw;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

    }
}
