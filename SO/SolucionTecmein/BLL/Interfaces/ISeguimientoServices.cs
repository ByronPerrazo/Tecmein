using Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

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
