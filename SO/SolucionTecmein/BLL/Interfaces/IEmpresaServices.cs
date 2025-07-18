using Entity;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IEmpresaServices
    {
        Task<List<Empresa>> Lista();
        Task<Empresa> Obtener();
        Task<Empresa> Crear(Empresa entidad, Stream logo = null, string nombreLogo = "");
        Task<Empresa> Editar(Empresa entidad, Stream logo = null, string nombreLogo = "");
        Task<bool> Eliminar(int secuencial);
    }
}
