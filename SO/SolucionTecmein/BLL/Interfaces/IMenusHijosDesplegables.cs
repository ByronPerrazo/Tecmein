using Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IMenusHijosDesplegables
    {
        Task<List<Menu>> ObtenerMenusHijos();
    }
}
