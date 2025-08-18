using Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IPreContratoServices
    {
        Task<List<PreContrato>> Lista();
        Task<PreContrato> Obtener(int secPreContrato);
        Task<PreContrato> Crear(PreContrato entidad);
        Task<PreContrato> Editar(PreContrato entidad);
        Task<bool> Eliminar(int secPreContrato);
        Task<string> GenerarDocumentoWord(int secPreContrato);
    }
}
