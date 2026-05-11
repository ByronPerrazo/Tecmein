using BLL.DTOs;
using Entity;

namespace BLL.Interfaces
{
    public interface ICotizacionServices
    {
        Task<List<CotizacionDTO>> Lista();
        Task<CotizacionDTO> Detalle(int secuencial);
        Task<CotizacionDTO> Crear(CotizacionDTO entidad);
        Task<CotizacionDTO> Editar(CotizacionDTO entidad);
        Task<bool> Eliminar(int secuencial);
        Task<bool> VisitaTieneCotizacionActiva(int visitaId);
        Task<bool> EnviarCorreoProveedor(int idCotizacion);
        Task<bool> EnviarCorreoCliente(int idCotizacion);
        Task<byte[]> GenerarPdfCotizacion(int idCotizacion);
        Task<byte[]> GenerarPdfSolicitudEquipos(int idCotizacion);

    }
}
