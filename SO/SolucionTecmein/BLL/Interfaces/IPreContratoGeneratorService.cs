using BLL.DTOs;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IPreContratoGeneratorService
    {
        Task<string> GenerarVistaPreviaHtml(PreContratoGeneratorDTO preContratoData);
        Task<PlaceholderDataDTO> ObtenerDatosParaPlaceholders(PreContratoGeneratorDTO preContratoData);
    }
}
