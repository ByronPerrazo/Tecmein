using BLL.DTOs;

namespace BLL.Interfaces
{
    public interface IPlanDePagoService
    {
        Task<PlanDePagoDTO> ObtenerPorContratoId(int idContrato);
        Task<PlanDePagoDTO> Guardar(PlanDePagoDTO modelo);
        Task<IEnumerable<PlanDePagoDTO>> ListarPlanesDePago(); // Añadido
        Task<PlanDePagoDTO> ObtenerDetallePlan(int idPlanDePago); // Añadido
    }
}