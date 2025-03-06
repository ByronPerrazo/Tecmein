using Entity;

namespace BLL.Interfaces
{
    public interface IEquiposVisitaServices
    {
        Task<Equiposvisita> Obtener(int secuencial);
        Task<List<Equiposvisita>> ConsultaListaPorVisita(int secuencialVisita);
        Task<Equiposvisita> ProcesaGuardar(Equiposvisita equiposvisita);
    }
}
