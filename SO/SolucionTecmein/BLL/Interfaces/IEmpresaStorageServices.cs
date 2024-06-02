using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IEmpresaStorageServices
    {
        Task<Empresastorage> Obtener(int secuencialEmpresa);
        Task<List<Empresastorage>> Consultar();
        Task<Empresastorage> ProcesaGuardar(Empresastorage empresaStorage);

    }
}
