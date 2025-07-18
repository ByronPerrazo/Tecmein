using Entity;

namespace BLL.Interfaces
{
    public interface IMenuServices
    {
        Task<Menu> ObtenerPorId(int secuencial);
        Task<List<Menu>> ObtenerTodosPadre();
        Task<List<Menu>> ObtieneMenu(int secuencialUsuario);
        Task<List<Menu>> ObtieneMenuTotal();
        Task<Menu> Crear(Menu entidad);
        Task<Menu> Editar(Menu entidad);
        Task<bool> Eliminar(int secuencial);
    }
}