using Entity;

namespace BLL.Interfaces
{
    public interface IAutorizacionService
    {
        Task<bool> TienePermiso(int secRol, string accion);
    }
}
