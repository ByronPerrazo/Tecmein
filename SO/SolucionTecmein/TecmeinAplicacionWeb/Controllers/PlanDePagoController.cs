using AutoMapper;
using BLL.DTOs;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using TecmeinWebApp.Utilidades.Response;
using TecmeinWebApp.Utilidades.ViewComponents;

namespace TecmeinAplicacionWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanDePagoController : ControllerBase
    {
        private readonly IPlanDePagoService _planDePagoService;
        private readonly IMapper _mapper;

        public PlanDePagoController(IPlanDePagoService planDePagoService, IMapper mapper)
        {
            _planDePagoService = planDePagoService;
            _mapper = mapper;
        }

        [HttpGet("ObtenerPorContratoId/{idContrato}")]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> ObtenerPorContratoId(int idContrato)
        {
            var gResponse = new GenericResponse<PlanDePagoDTO>();
            try
            {
                gResponse.Estado = true;
                gResponse.Objeto = await _planDePagoService.ObtenerPorContratoId(idContrato);
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPost("Guardar")]
        [ValidatePermission("ACTUALIZAR")]
        public async Task<IActionResult> Guardar([FromBody] PlanDePagoDTO modelo)
        {
            var gResponse = new GenericResponse<PlanDePagoDTO>();
            try
            {
                gResponse.Estado = true;
                gResponse.Objeto = await _planDePagoService.Guardar(modelo);
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