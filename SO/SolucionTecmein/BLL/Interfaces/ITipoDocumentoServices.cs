using Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface ITipoDocumentoServices
    {
        Task<List<TipoDocumento>> Lista();
        Task<TipoDocumento> Crear(TipoDocumento entidad);
        Task<TipoDocumento> Editar(TipoDocumento entidad);
        Task<bool> Eliminar(int secTipoDocumento);
        Task<TipoDocumento> Obtener(int secTipoDocumento);
    }
}
