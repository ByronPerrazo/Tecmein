using Entity;

namespace BLL.Interfaces
{
    public interface IFormatoNumeroClienteServices
    {
        Task<FormatoNumeroCliente> ObtenerPorEmpresa(int secEmpresa);
        Task<FormatoNumeroCliente> Guardar(FormatoNumeroCliente entidad);
    }
}
