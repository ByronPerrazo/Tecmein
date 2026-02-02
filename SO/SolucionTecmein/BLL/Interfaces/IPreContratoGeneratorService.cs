using BLL.DTOs;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IPreContratoGeneratorService
    {
        Task<byte[]> GenerarVistaPreviaDocx(PreContratoGeneratorDTO preContratoData);
        Task<PlaceholderDataDTO> ObtenerDatosParaPlaceholders(PreContratoGeneratorDTO preContratoData);
    }
}
