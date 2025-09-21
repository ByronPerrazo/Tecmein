using AutoMapper;
using BLL.Interfaces;
using Entity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TecmeinAplicacionWeb.Models.ViewModels;
using TecmeinWebApp.Utilidades.Response;
using System.Threading.Tasks;

namespace TecmeinWebApp.Controllers
{
    public class FormatoNumeroClienteController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IFormatoNumeroClienteServices _formatoServices;

        public FormatoNumeroClienteController(IMapper mapper, IFormatoNumeroClienteServices formatoServices)
        {
            _mapper = mapper;
            _formatoServices = formatoServices;
        }

        public async Task<IActionResult> Index()
        {
            // Asumimos SecEmpresa = 1 por ahora
            var formato = await _formatoServices.ObtenerPorEmpresa(1);
            var formatoVM = _mapper.Map<FormatoNumeroClienteVm>(formato);
            return View(formatoVM);
        }

        [HttpPost]
        public async Task<IActionResult> Guardar([FromForm] string modelo)
        {
            var gResponse = new GenericResponse<FormatoNumeroClienteVm>();
            try
            {
                var formatoVM = JsonConvert.DeserializeObject<FormatoNumeroClienteVm>(modelo);
                // Asumimos SecEmpresa = 1 por ahora
                formatoVM.SecEmpresa = 1;

                var formato = _mapper.Map<FormatoNumeroCliente>(formatoVM);
                var formatoGuardado = await _formatoServices.Guardar(formato);

                gResponse.Estado = true;
                gResponse.Objeto = _mapper.Map<FormatoNumeroClienteVm>(formatoGuardado);
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
