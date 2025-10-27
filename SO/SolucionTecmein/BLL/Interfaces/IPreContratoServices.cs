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
        Task<string> GenerarDocumentoWord(int secPreContrato);
        Task<PreContrato> ObtenerUltimaVersion(int secCotizacion);
        Task<PreContrato> CrearDesdeCotizacion(int cotizacionId, int secUsuario);
        Task<PreContrato> GuardarDesdeEditor(int cotizacionId, string contenidoHtml, int usuarioId);
        Task<List<PreContrato>> ObtenerHistorial(int secPreContrato);
        Task<PreContratoParrafo> ObtenerPrimerParrafo(int secPreContrato);
        Task<PreContrato> CrearDesdeModal(PreContrato entidad, int usuarioId, string contenidoHtml);
        Task<PreContrato> CrearDesdeModalConPagos(PreContratoConPagosDTO dto, int usuarioId);
        Task<string> GenerarVistaPreviaConPagos(PreContratoConPagosDTO dto);
        Task<PreContrato> GuardarBorrador(PreContratoConPagosDTO dto, int usuarioId);
        Task<PreContratoParaEdicionDTO> ObtenerParaEdicion(int secPreContrato);
        //Task<string> GenerarDocumentoWord(int secPreContrato);
        //Task<bool> ActualizarContenido(int secPreContrato, string contenidoHtml);
        Task<bool> Aprobar(int secPreContrato);
        Task<string> ObtenerContenidoHtml(int secPreContrato);
        Task<PreContrato> ActualizarContenidoPreContrato(int secPreContrato, string contenidoHtml, int usuarioId);
        Task<string> ObtenerContenidoPrevisualizado(int secPreContrato);
        Task<bool> ActualizarContenido(int secPreContrato, string contenidoHtml);
    }
}
