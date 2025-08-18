using Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IFormaPagoServices
    {
        Task<List<FormaPago>> Lista();
        Task<FormaPago> Obtener(int secFormaPago);
        Task<FormaPago> Crear(FormaPago entidad);
        Task<FormaPago> Editar(FormaPago entidad);
        Task<bool> Eliminar(int secFormaPago);
    }
}
