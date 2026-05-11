using Entity;

namespace BLL.Interfaces
{
    public interface IMenusHijosDesplegables
    {
        Task<List<Menu>> ObtenerMenusHijos();
    }
}
