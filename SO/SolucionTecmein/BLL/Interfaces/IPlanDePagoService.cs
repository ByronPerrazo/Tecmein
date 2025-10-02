using BLL.DTOs;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IPlanDePagoService
    {
        Task<PlanDePagoDTO> ObtenerPorContratoId(int idContrato);
        Task<PlanDePagoDTO> Guardar(PlanDePagoDTO modelo);
    }
}