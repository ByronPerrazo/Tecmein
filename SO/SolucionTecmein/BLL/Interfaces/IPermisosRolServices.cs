using Entity;

namespace BLL.Interfaces
{
    public interface IPermisosRolServices
    {
        Task<List<Permisosrol>> Lista();
        Task<Permisosrol> Crea(Permisosrol entidad);
        Task<Permisosrol?> PermisosRolActivo(int? secRol);
        Task<Permisosrol> Editar(Permisosrol entidad);
        Task<bool> Eliminar(int secuencial);
    }
}
