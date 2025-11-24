using BLL.Models.ViewModels; // Se creará en el siguiente paso

namespace BLL.Interfaces
{
    public interface IPagoServices
    {
        Task<List<PlanPagoDashboardVM>> ObtenerPlanesDePagoParaDashboard();
        Task<DetallePlanPagoVM> ObtenerDetallePlanDePago(int idPlanDePago);
        Task<bool> RegistrarPago(RegistrarPagoVM pagoVM);
    }
}
