using Entity;

namespace BLL.Interfaces
{
    public interface ISeguimientoServices
    {
        Task<List<Seguimiento>> Lista(int secCotizacion);
        Task<Seguimiento> Crear(Seguimiento entidad);
        Task<Seguimiento> Editar(Seguimiento entidad);
        Task<bool> Eliminar(int secSeguimiento);
        Task<List<Cotizacion>> ObtenerCotizacionesAprobadasSinPreContrato();
    }
}
