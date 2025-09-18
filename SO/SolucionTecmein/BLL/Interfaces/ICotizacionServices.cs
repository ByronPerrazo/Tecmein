using Entity;

namespace BLL.Interfaces
{
    public interface ICotizacionServices
    {
        Task<List<Cotizacion>> Lista();
        Task<Cotizacion> Detalle(int secuencial);
        Task<Cotizacion> Crear(Cotizacion entidad, int secUsuario);
        Task<Cotizacion> Editar(Cotizacion entidad, int secUsuarioActual);
        Task<bool> Eliminar(int secuencial);
        Task<bool> VisitaTieneCotizacionActiva(int visitaId);
        Task<bool> EnviarCorreoProveedor(int idCotizacion);
        Task<bool> EnviarCorreoCliente(int idCotizacion);
        Task<byte[]> GenerarPdfCotizacion(int idCotizacion);
        Task<byte[]> GenerarPdfSolicitudEquipos(int idCotizacion);
        
    }
}
