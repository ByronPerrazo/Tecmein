
using BLL.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IPagoService
    {
        Task<PagoDTO> RegistrarPago(PagoDTO modelo, int idUsuario);
        Task<IEnumerable<PagoDTO>> ListarPorPlanDePago(int idPlanDePago);
    }
}
