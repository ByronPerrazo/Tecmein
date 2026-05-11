using BLL.DTOs;

namespace BLL.Interfaces
{
    public interface IPreContratoGeneratorService
    {
        Task<byte[]> GenerarVistaPreviaDocx(PreContratoGeneratorDTO preContratoData);
        Task<PlaceholderDataDTO> ObtenerDatosParaPlaceholders(PreContratoGeneratorDTO preContratoData);
    }
}
