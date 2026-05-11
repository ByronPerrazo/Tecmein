
using Entity;

namespace BLL.Interfaces
{
    public interface IFormatoNumeroClienteService
    {
        Task<FormatoNumeroCliente> Obtener();
        Task<FormatoNumeroCliente> Guardar(FormatoNumeroCliente entidad);
    }
}
