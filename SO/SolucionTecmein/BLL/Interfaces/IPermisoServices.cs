using Entity;

namespace BLL.Interfaces
{
    public interface IPermisoServices
    {
        Task<List<Permiso>> Listar();
        Task<Permiso> Crear(Permiso entidad);
        Task<Permiso> Editar(Permiso entidad);
        Task<bool> Eliminar(string idPermiso);
        Task<List<Permiso>> ObtenerPermisosPorRol(int secRol); // New method
    }
}
