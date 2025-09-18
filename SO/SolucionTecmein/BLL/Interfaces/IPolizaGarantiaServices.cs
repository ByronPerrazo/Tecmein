using Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IPolizaGarantiaServices
    {
        Task<List<PolizaGarantia>> Lista();
        Task<PolizaGarantia> Crear(PolizaGarantia entidad);
        Task<PolizaGarantia> Editar(PolizaGarantia entidad);
        Task<bool> Eliminar(int secuencial);
    }
}