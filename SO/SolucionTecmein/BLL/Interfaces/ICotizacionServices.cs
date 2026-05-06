using BLL.DTOs;
using Entity;

namespace BLL.Interfaces
{
    public interface ICotizacionServices
    {
        Task<List<CotizacionDTO>> Lista(int secUsuario);
        Task<CotizacionDTO> Detalle(int secuencial, int secUsuario);
        Task<CotizacionDTO> Crear(CotizacionDTO entidad, int secUsuario);
        Task<CotizacionDTO> Editar(CotizacionDTO entidad, int secUsuarioActual);
        Task<bool> Eliminar(int secuencial, int secUsuario);
        Task<bool> VisitaTieneCotizacionActiva(int visitaId);
        Task<bool> EnviarCorreoProveedor(int idCotizacion);
        Task<bool> EnviarCorreoCliente(int idCotizacion);
        Task<byte[]> GenerarPdfCotizacion(int idCotizacion, int secUsuario);
        Task<byte[]> GenerarPdfSolicitudEquipos(int idCotizacion, int secUsuario);

    }
}
