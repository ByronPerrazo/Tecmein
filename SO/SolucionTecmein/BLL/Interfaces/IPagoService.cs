using BLL.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IPagoService
    {
        Task<List<PlanPagoDashboardDTO>> ObtenerPlanesDePagoParaDashboard();
        Task<DetallePlanPagoDTO> ObtenerDetallePlanDePago(int idPlanDePago);
        Task<PagoDTO> RegistrarPago(PagoDTO pagoDTO); // Changed to accept and return DTO
        Task<IEnumerable<PagoDTO>> ListarPorPlanDePago(int idPlanDePago);
        Task<List<HistorialPagoCuotaDTO>> ObtenerHistorialPagosPorCuota(int idCuota);
    }
}
