using Entity;

namespace BLL.Interfaces
{
    public interface IRolServices
    {
        Task<List<Rol>> Lista();
        Task<Rol?> RolPorSecuencial(int secuecialRol);
        Task<Rol?> GuardarRol(Rol entidad);
        Task<Rol?> EditarRol(Rol entidad);
        Task<Rol> GuardarRolCompleto(Rol entidad, Permisosrol permisos);
        Task<Rol?> EditarRolCompleto(Rol entidad, Permisosrol permisos);
        Task<bool> EliminarRol(int secuencialRol);
    }
}
