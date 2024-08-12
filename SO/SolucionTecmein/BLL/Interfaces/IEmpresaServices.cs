using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity;

namespace BLL.Interfaces
{
    public interface IEmpresaServices
    {
        Task<List<Empresa>> Lista();
        Task<Empresa> Obtener();
        Task<Empresa> GuardarCambios(Empresa entidad, Stream logo = null, string NombreLogo = "");
        Task<Empresa> Editar(Empresa entidad, Stream logo = null, string NombreLogo = "");
        Task<bool> Eliminar(int secuencial);
    }
}
