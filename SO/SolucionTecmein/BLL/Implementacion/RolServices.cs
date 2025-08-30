using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace BLL.Implementacion
{
    public class RolServices : IRolServices
    {

        private IGenericRepository<Rol> _repositorio;
        private IUsuarioServices _usuarioServices;
        // private IPermisosRolServices _permisosRolServices; // ELIMINADO

        public RolServices(IGenericRepository<Rol> repositorio, IUsuarioServices usuarioServices /*, IPermisosRolServices permisosRolServices */)
        {
            _repositorio = repositorio;
            _usuarioServices = usuarioServices;
            // _permisosRolServices = permisosRolServices; // ELIMINADO
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

        // Renombrado de GuardarRol a Crear
        public async Task<Rol?> Crear(Rol entidad)
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

        // Renombrado de EditarRol a Editar
        public async Task<Rol?> Editar(Rol entidad)
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

        // Métodos GuardarRolCompleto y EditarRolCompleto ELIMINADOS
    }
}
