using Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IActivoClienteService
    {
        Task<List<ActivoCliente>> Lista();
        Task<ActivoCliente> Obtener(int idActivoCliente);
        Task<ActivoCliente> Crear(ActivoCliente entidad);
        Task<ActivoCliente> Editar(ActivoCliente entidad);
        Task<bool> Eliminar(int idActivoCliente);
        Task<List<ActivoCliente>> ObtenerPorCliente(int secCliente);
        Task<List<ActivoCliente>> ObtenerPorContratoOrigen(int secContratoOrigen);
    }
}