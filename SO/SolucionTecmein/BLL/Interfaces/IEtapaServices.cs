using Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IEtapaServices
    {
        Task<List<Etapa>> Lista();
        Task<Etapa> ObtenerPorCodigo(string codigo);
    }
}
