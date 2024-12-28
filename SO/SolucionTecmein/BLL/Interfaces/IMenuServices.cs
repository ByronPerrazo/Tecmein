using Entity;

namespace BLL.Interfaces
{
    public interface IMenuServices
    {
        Task<List<Menu>> ObtieneMenu(int secuencialUsuario);
    }
}
