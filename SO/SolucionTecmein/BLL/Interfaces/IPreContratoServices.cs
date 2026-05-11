using BLL.DTOs;
using BLL.DTOs;
using Entity;

namespace BLL.Interfaces
{
    public interface IPreContratoServices
    {
        Task<List<PreContratoDTO>> Lista();
        Task<PreContratoDTO> Obtener(int secPreContrato);
        Task<PreContratoDTO> Crear(PreContratoDTO entidad);
        Task<PreContratoDTO> Editar(PreContratoDTO entidad);
        Task<bool> Eliminar(int secPreContrato);
        Task<PreContratoDTO> ObtenerUltimaVersion(int secCotizacion);
        Task<PreContratoDTO> CrearDesdeCotizacion(int cotizacionId);
        Task<List<PreContratoDTO>> ObtenerHistorial(int secPreContrato);
        Task<PreContratoDTO> CrearDesdeModal(PreContrato entidad);
        Task<PreContratoDTO> GuardarBorrador(PreContratoConPagosDTO dto);
        Task<PreContratoParaEdicionDTO> ObtenerParaEdicion(int secPreContrato);
        Task<bool> Aprobar(int secPreContrato);
        Task<bool> SubirContratoFinal(int secPreContrato, System.IO.Stream archivoStream);
        Task<byte[]> ObtenerContenidoDocumento(int secPreContrato);
    }
}
