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
    [Authorize]
    public class TipoProductoController : Controller
    {
        private readonly ITipoProductoServices _tipoProductoServices;
        private readonly IMapper _mapper;
        public TipoProductoController(ITipoProductoServices tipoProductoServices, 
                                      IMapper mapper)
        {
            _tipoProductoServices = tipoProductoServices;
            _mapper = mapper;   
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            var listaTiposProductoVM
                = _mapper.Map<List<TipoProductoVM>>(await _tipoProductoServices.Lista());

            return StatusCode(StatusCodes.Status200OK, new { data = listaTiposProductoVM });
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] string modelo)
        {
            var genericResponse = new GenericResponse<TipoProductoVM>();
            try
            {
                var tipoProductoVM 
                    = JsonConvert
                      .DeserializeObject<TipoProductoVM>(modelo);

                var _tipoCreado = await _tipoProductoServices.Crea(_mapper.Map<TipoProducto>(tipoProductoVM));
                tipoProductoVM = _mapper.Map<TipoProductoVM>(_tipoCreado);

                genericResponse.Estado = true;
                genericResponse.Objeto = tipoProductoVM;
            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensajes = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }

        [HttpPost]
        public async Task<IActionResult> Editar([FromForm] string modelo)
        {
            var genericResponse = new GenericResponse<TipoProductoVM>();
            try
            {
                TipoProductoVM? tipoProductoVM = JsonConvert.DeserializeObject<TipoProductoVM>(modelo);

                var tipoProdEdit = await _tipoProductoServices.Editar(_mapper.Map<TipoProducto>(tipoProductoVM));
                    tipoProductoVM = _mapper.Map<TipoProductoVM>(tipoProdEdit);

                genericResponse.Estado = true;
                genericResponse.Objeto = tipoProductoVM;
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
                gResponse.Estado = await _tipoProductoServices.Eliminar(secuencial);
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
