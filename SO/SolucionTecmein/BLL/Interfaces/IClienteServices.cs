using Entity;
using System.Threading.Tasks;
using BLL.DTOs;

namespace BLL.Interfaces
{
    public interface IClienteServices
    {
        Task<Cliente> Crear(Cliente entidad);
        Task<Cliente> Editar(Cliente entidad);
        Task<bool> Eliminar(int secCliente);
        Task<Cliente> ObtenerPorId(int secCliente);
        Task<Cliente> ObtenerPorIdConstructora(int secConstructora);
        Task<string> GenerarSiguienteNumeroCliente();
        Task<List<Cliente>> Listar();




        // Nuevo método para buscar clientes
        Task<List<Cliente>> BuscarClientes(string terminoBusqueda);

        // Nuevo método para obtener contratos por cliente
        Task<List<Contrato>> ObtenerContratosPorCliente(int secCliente);

        Task<Cliente> ObtenerOCrearPorConstructora(int secConstructora);
    }
}
