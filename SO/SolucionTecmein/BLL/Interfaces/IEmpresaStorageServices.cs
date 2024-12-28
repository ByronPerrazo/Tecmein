using Entity;

namespace BLL.Interfaces
{
    public interface IEmpresaStorageServices
    {
        Task<Empresastorage> Obtener(int secuencialEmpresa);
        Task<List<Empresastorage>> Consultar();
        Task<Empresastorage> ProcesaGuardar(Empresastorage empresaStorage);

    }
}
