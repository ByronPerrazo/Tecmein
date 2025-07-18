using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;

namespace BLL.Implementacion
{
    public class RolServices : IRolServices
    {

        private IGenericRepository<Rol> _repositorio;
        private IUsuarioServices _usuarioServices;
        private IPermisosRolServices _permisosRolServices;
        public RolServices(IGenericRepository<Rol> repositorio, IUsuarioServices usuarioServices, IPermisosRolServices permisosRolServices)
        {
            _repositorio = repositorio;
            _usuarioServices = usuarioServices;
            _permisosRolServices = permisosRolServices;
        }

        public async Task<List<Rol>> Lista()
        {
            IQueryable<Rol> query = _repositorio.Consultar().Result;
            return await query.ToListAsync();
        }

        public async Task<Rol?> RolPorSecuencial(int secuecialRol)
        {
            return await _repositorio
                         .Obtener(x => x.Secuencial == secuecialRol);
        }

        public async Task<Rol?> GuardarRol(Rol entidad)
        {
            try
            {
                var registroGuardado = await _repositorio.Crear(entidad);
                return registroGuardado;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Rol?> EditarRol(Rol entidad)
        {
            try
            {
                var registroBase =
                    await _repositorio
                          .Obtener(x => x.Secuencial == entidad.Secuencial);

                if (registroBase == null)
                    throw new TaskCanceledException("El rol no existe");

                registroBase.Descripcion = entidad.Descripcion;
                registroBase.EsActivo = entidad.EsActivo;

                if (await _repositorio.Editar(registroBase))
                {
                    return registroBase;
                }
                else
                {
                    throw new TaskCanceledException("No se pudo editar el rol");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> EliminarRol(int secuencialRol)
        {
            try
            {
                var usuariosConRol = await
                    _usuarioServices
                    .Lista();

                var numeroUsuarios =
                    usuariosConRol
                    .Count(x =>
                           x.SecRol == secuencialRol &&
                           x.EsActivo == 1);

                if (numeroUsuarios > 0)
                    throw new TaskCanceledException("No se puede eliminar el rol porque tiene usuarios asociados");

                var rol
                    = await _repositorio
                             .Obtener(x => x.Secuencial == secuencialRol);

                if (rol == null)
                    throw new TaskCanceledException("El rol no existe");

                return await _repositorio.Eliminar(rol);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Rol> GuardarRolCompleto(Rol entidad, Permisosrol permisos)
        {
            var rolGenerado = await GuardarRol(entidad);
            if (rolGenerado != null)
            {
                permisos.SecRol = rolGenerado.Secuencial;
                await _permisosRolServices.Crea(permisos);
            }
            return rolGenerado;
        }

        public async Task<Rol?> EditarRolCompleto(Rol entidad, Permisosrol permisos)
        {
            var rolEditado = await EditarRol(entidad);
            if (rolEditado != null)
            {
                var permisosExistente = await _permisosRolServices.PermisosRolActivo(rolEditado.Secuencial);
                if (permisosExistente != null)
                {
                    permisosExistente.Consultar = permisos.Consultar;
                    permisosExistente.Modificar = permisos.Modificar;
                    permisosExistente.Eliminar = permisos.Eliminar;
                    await _permisosRolServices.Editar(permisosExistente);
                }
                else
                {
                    permisos.SecRol = rolEditado.Secuencial;
                    await _permisosRolServices.Crea(permisos);
                }
            }
            return rolEditado;
        }

    }
}
