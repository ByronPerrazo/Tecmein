using Entity;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IClienteServices
    {
        Task<Cliente> Crear(Cliente entidad);
        Task<Cliente> Editar(Cliente entidad);
        Task<bool> Eliminar(int secCliente);
        Task<Cliente> ObtenerPorId(int secCliente);
        Task<Cliente> ObtenerPorIdConstructora(int secConstructora);
    }
}
