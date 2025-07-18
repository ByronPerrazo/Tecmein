using Entity;

namespace BLL.Interfaces
{
    public interface IRolMenuServices
    {
        Task<List<RolMenu>> Lista();
        Task<RolMenu> ObtenerPorId(int idRolMenu);
        Task<RolMenu> Crear(RolMenu entidad);
        Task<RolMenu> Editar(RolMenu entidad);
        Task<bool> Eliminar(int idRolMenu);
    }
}
