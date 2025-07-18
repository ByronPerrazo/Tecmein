using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;

namespace BLL.Implementacion
{
    public class PermisosRolServices : IPermisosRolServices
    {

        private IGenericRepository<Permisosrol> _repositorio;
        private IGenericRepository<Rol> _repositorioRol;
        public PermisosRolServices(IGenericRepository<Permisosrol> repositorio, IGenericRepository<Rol> repositorioRol)
        {
            _repositorio = repositorio;
            _repositorioRol = repositorioRol;
        }

        public async Task<Permisosrol> Crea(Permisosrol entidad)
        {
            try
            {
                var permisoExistente = await _repositorio.Obtener(p => p.SecRol == entidad.SecRol);

                if (permisoExistente != null)
                {
                    permisoExistente.Consultar = entidad.Consultar;
                    permisoExistente.Modificar = entidad.Modificar;
                    permisoExistente.Eliminar = entidad.Eliminar;
                    permisoExistente.Activo = 1;
                    await _repositorio.Editar(permisoExistente);
                    return permisoExistente;
                }
                else
                {
                    entidad.Activo = 1;
                    var permisoCreado = await _repositorio.Crear(entidad);
                    return permisoCreado;
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<Permisosrol> Editar(Permisosrol entidad)
        {
            try
            {
                var permisoExistente = await _repositorio.Obtener(p => p.SecRol == entidad.SecRol && p.Activo == 1);

                if (permisoExistente != null)
                {
                    permisoExistente.Consultar = entidad.Consultar;
                    permisoExistente.Modificar = entidad.Modificar;
                    permisoExistente.Eliminar = entidad.Eliminar;
                    await _repositorio.Editar(permisoExistente);
                    return permisoExistente;
                }
                else
                {
                    // Si no se encuentra un permiso activo, se crea uno nuevo.
                    entidad.Activo = 1;
                    var permisoCreado = await _repositorio.Crear(entidad);
                    return permisoCreado;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> Eliminar(int secuencial)
        {
            try
            {
                var seElimino = false;
                var permisosrols
                    = await _repositorio
                             .Consultar(x => x.Secuencial == secuencial).Result.FirstOrDefaultAsync();

                var permiso = permisosrols;
                if (permiso != null)
                {
                    seElimino = await _repositorio.Eliminar(permiso);

                }
                return seElimino;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Permisosrol>> Lista()
            => await _repositorio.Consultar().Result.ToListAsync();

        public async Task<Permisosrol?> PermisosRolActivo(int? secRol) 
            => await _repositorio
                     .Obtener(x =>
                              x.SecRol == secRol &&
                              x.Activo == 1);

           
    }
}
