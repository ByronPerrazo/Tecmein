using Entity;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IFormatoNumeroClienteServices
    {
        Task<FormatoNumeroCliente> ObtenerPorEmpresa(int secEmpresa);
        Task<FormatoNumeroCliente> Guardar(FormatoNumeroCliente entidad);
    }
}
