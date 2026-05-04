using BLL.DTOs;
using Entity;

namespace BLL.Interfaces
{
    public interface IPreContratoServices
    {
        Task<List<PreContrato>> Lista();
        Task<PreContrato> Obtener(int secPreContrato);
        Task<PreContrato> Crear(PreContrato entidad);
        Task<PreContrato> Editar(PreContrato entidad);
        Task<bool> Eliminar(int secPreContrato);
        Task<PreContrato> ObtenerUltimaVersion(int secCotizacion);
        Task<PreContrato> CrearDesdeCotizacion(int cotizacionId, int secUsuario);
        Task<List<PreContrato>> ObtenerHistorial(int secPreContrato);
        Task<PreContrato> CrearDesdeModal(PreContrato entidad, int usuarioId);
        Task<PreContrato> GuardarBorrador(PreContratoConPagosDTO dto, int usuarioId);
        Task<PreContratoParaEdicionDTO> ObtenerParaEdicion(int secPreContrato);
        Task<bool> Aprobar(int secPreContrato);
        Task<bool> SubirContratoFinal(int secPreContrato, System.IO.Stream archivoStream);
        Task<byte[]> ObtenerContenidoDocumento(int secPreContrato);
    }
}
