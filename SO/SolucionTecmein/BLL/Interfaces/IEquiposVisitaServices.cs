using BLL.Implementacion;
using Entity;

namespace BLL.Interfaces
{
    public interface IEquiposVisitaServices
    {
        Task<Equiposvisita> Obtener(int secuencial);
        Task<EquiposVisitaConEstadoCotizacion> ConsultaListaPorVisita(int secuencialVisita);
        Task<Equiposvisita> ProcesaGuardar(Equiposvisita equiposvisita);
        Task<bool> ProcesaEliminar(Equiposvisita equiposvisita);
        Task<bool> SincronizarEquiposConCotizacionActiva(int secVisita); // New method
    }
}
