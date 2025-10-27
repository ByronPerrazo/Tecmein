
using BLL.DTOs;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TecmeinWebApp.Utilidades.Response;
using TecmeinWebApp.Utilidades.ViewComponents;

namespace TecmeinAplicacionWeb.Controllers
{
    [Route("api/Pago")]
    [ApiController]
    public class PagoController : ControllerBase
    {
        private readonly IPagoService _pagoService;

        public PagoController(IPagoService pagoService)
        {
            _pagoService = pagoService;
        }

        [HttpPost("Registrar")]
        [ValidatePermission("CREAR")]
        public async Task<IActionResult> Registrar([FromBody] PagoDTO modelo)
        {
            var gResponse = new GenericResponse<PagoDTO>();
            try
            {
                var claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
                var idUsuarioClaim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                if (idUsuarioClaim == null)
                {
                    gResponse.Estado = false;
                    gResponse.Mensajes = "No se pudo obtener el usuario de la sesión.";
                    return StatusCode(401, gResponse);
                }

                var idUsuario = int.Parse(idUsuarioClaim.Value);

                gResponse.Objeto = await _pagoService.RegistrarPago(modelo, idUsuario);
                gResponse.Estado = true;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
            }
            return StatusCode(200, gResponse);
        }

        [HttpGet("ListarPorPlan/{idPlanDePago}")]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> ListarPorPlanDePago(int idPlanDePago)
        {
            var gResponse = new GenericResponse<IEnumerable<PagoDTO>>();
            try
            {
                gResponse.Objeto = await _pagoService.ListarPorPlanDePago(idPlanDePago);
                gResponse.Estado = true;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
            }
            return StatusCode(200, gResponse);
        }
    }
}
