using Microsoft.AspNetCore.Mvc;
using BLL.Interfaces;
using TecmeinWebApp.Utilidades.Response;
using TecmeinWebApp.Utilidades.ViewComponents; // Para ValidatePermissionAttribute
using BLL.Models.ViewModels; // Added for ViewModels
using System.Security.Claims; // Added for ClaimTypes

namespace TecmeinWebApp.Controllers
{
    public class FinancieroController : Controller
    {
        private readonly IPlanDePagoServices _planDePagoServices;
        private readonly IPagoServices _pagoServices;

        public FinancieroController(IPlanDePagoServices planDePagoServices, IPagoServices pagoServices)
        {
            _planDePagoServices = planDePagoServices;
            _pagoServices = pagoServices;
        }

        [ValidatePermission("VER_MENU")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> ListaPlanesPago()
        {
            var rsp = new GenericResponse<List<PlanPagoDashboardVM>>();
            try
            {
                rsp.Objeto = await _pagoServices.ObtenerPlanesDePagoParaDashboard();
                rsp.Estado = true;
            }
            catch (Exception ex)
            {
                rsp.Estado = false;
                rsp.Mensajes = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, rsp);
        }

        [HttpGet] // Added Get for detail
        [ValidatePermission("LEER")]
        public async Task<IActionResult> ObtenerDetallePlanDePago(int idPlanDePago)
        {
            var rsp = new GenericResponse<DetallePlanPagoVM>();
            try
            {
                rsp.Objeto = await _pagoServices.ObtenerDetallePlanDePago(idPlanDePago);
                rsp.Estado = true;
            }
            catch (Exception ex)
            {
                rsp.Estado = false;
                rsp.Mensajes = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, rsp);
        }

        [HttpPost]
        [ValidatePermission("CREAR")]
        public async Task<IActionResult> RegistrarPago([FromBody] RegistrarPagoVM pagoVM)
        {
            var rsp = new GenericResponse<string>();
            try
            {
                // Obtener el ID del usuario logueado desde los claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int registradoPorUsuarioId))
                {
                    rsp.Estado = false;
                    rsp.Mensajes = "No se pudo identificar al usuario que registra el pago.";
                    return StatusCode(StatusCodes.Status401Unauthorized, rsp);
                }
                pagoVM.RegistradoPorUsuarioId = registradoPorUsuarioId;

                bool resultado = await _pagoServices.RegistrarPago(pagoVM);
                if (resultado)
                {
                    rsp.Estado = true;
                    rsp.Objeto = "Pago registrado exitosamente.";
                }
                else
                {
                    rsp.Estado = false;
                    rsp.Mensajes = "No se pudo registrar el pago.";
                }
            }
            catch (Exception ex)
            {
                rsp.Estado = false;
                rsp.Mensajes = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, rsp);
        }
    }
}
