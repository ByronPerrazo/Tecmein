using Entity;

namespace BLL.Interfaces
{
    public interface IRolServices
    {
        Task<List<Rol>> Lista();
        Task<Rol?> RolPorSecuencial(int secuecialRol);
        Task<Rol?> Crear(Rol entidad); // Renombrado de GuardarRol
        Task<Rol?> Editar(Rol entidad); // Mantenido como EditarRol
        Task<bool> EliminarRol(int secuencialRol);
    }
}