using BLL.DTOs;
using BLL.DTOs;
using Entity;

namespace BLL.Interfaces
{
    public interface IPreContratoServices
    {
        Task<List<PreContratoDTO>> Lista(int secUsuario);
        Task<PreContratoDTO> Obtener(int secPreContrato, int secUsuario);
        Task<PreContratoDTO> Crear(PreContratoDTO entidad);
        Task<PreContratoDTO> Editar(PreContratoDTO entidad);
        Task<bool> Eliminar(int secPreContrato, int secUsuario);
        Task<PreContratoDTO> ObtenerUltimaVersion(int secCotizacion);
        Task<PreContratoDTO> CrearDesdeCotizacion(int cotizacionId, int secUsuario);
        Task<List<PreContratoDTO>> ObtenerHistorial(int secPreContrato);
        Task<PreContratoDTO> CrearDesdeModal(PreContrato entidad, int usuarioId);
        Task<PreContratoDTO> GuardarBorrador(PreContratoConPagosDTO dto, int usuarioId);
        Task<PreContratoParaEdicionDTO> ObtenerParaEdicion(int secPreContrato, int secUsuario);
        Task<bool> Aprobar(int secPreContrato);
        Task<bool> SubirContratoFinal(int secPreContrato, System.IO.Stream archivoStream, int secUsuario);
        Task<byte[]> ObtenerContenidoDocumento(int secPreContrato, int secUsuario);
    }
}
