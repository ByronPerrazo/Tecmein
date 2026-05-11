using Entity;

namespace BLL.Interfaces
{
    public interface IEtapaServices
    {
        Task<List<Etapa>> Lista();
        Task<Etapa> ObtenerPorCodigo(string codigo);
    }
}
