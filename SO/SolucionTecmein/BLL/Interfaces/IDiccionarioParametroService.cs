using Entity;

namespace BLL.Interfaces
{
    public interface IDiccionarioParametroService
    {
        Task<List<DiccionarioParametro>> Lista();
        Task<DiccionarioParametro> Crear(DiccionarioParametro entidad);
        Task<bool> Editar(DiccionarioParametro entidad);
        Task<bool> Eliminar(int secuencial);
        Task<List<DiccionarioParametro>> ListaActivos();
    }
}
