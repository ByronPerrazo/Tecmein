
using Microsoft.AspNetCore.Mvc;
using BLL.Interfaces;
using System.Threading.Tasks;
using Entity;
using TecmeinAplicacionWeb.Models.ViewModels;
using AutoMapper;
using TecmeinWebApp.Utilidades.Response;
using TecmeinWebApp.Utilidades.ViewComponents;
using System.Collections.Generic;

namespace TecmeinAplicacionWeb.Controllers
{
    public class FormatoNumeroClienteController : Controller
    {
        private readonly IFormatoNumeroClienteService _formatoService;
        private readonly IMapper _mapper;

        public FormatoNumeroClienteController(IFormatoNumeroClienteService formatoService, IMapper mapper)
        {
            _formatoService = formatoService;
            _mapper = mapper;
        }

        [ValidatePermission("VER_MENU")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> Lista()
        {
            var gResponse = new GenericResponse<List<FormatoNumeroClienteVM>>();
            try
            {
                var formato = await _formatoService.Obtener();
                var lista = new List<FormatoNumeroClienteVM>();
                if (formato != null)
                {
                    lista.Add(_mapper.Map<FormatoNumeroClienteVM>(formato));
                }
                
                gResponse.Estado = true;
                gResponse.Objeto = lista;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPost]
        [ValidatePermission("ACTUALIZAR")]
        public async Task<IActionResult> Guardar([FromBody] FormatoNumeroClienteVM modelo)
        {
            if (modelo == null)
            {
                return BadRequest(new GenericResponse<FormatoNumeroClienteVM>
                {
                    Estado = false,
                    Mensajes = "No se recibieron datos para guardar."
                });
            }

            if (modelo.LongitudNumero < 3)
            {
                return BadRequest(new GenericResponse<FormatoNumeroClienteVM>
                {
                    Estado = false,
                    Mensajes = "La longitud del número debe ser como mínimo 3."
                });
            }

            var gResponse = new GenericResponse<FormatoNumeroClienteVM>();
            try
            {
                var formato = _mapper.Map<FormatoNumeroCliente>(modelo);
                var formatoGuardado = await _formatoService.Guardar(formato);

                gResponse.Estado = true;
                gResponse.Objeto = _mapper.Map<FormatoNumeroClienteVM>(formatoGuardado);
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }
    }
}
