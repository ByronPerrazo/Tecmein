using Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IImpuestoServices
    {
        Task<List<Impuesto>> Lista();
        Task<Impuesto> Crear(Impuesto entidad);
        Task<Impuesto> Editar(Impuesto entidad);
        Task<bool> Eliminar(int id);
        Task<Impuesto> ObtenerPorId(int id);
    }
}
