using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore; // Añadido para ToListAsync
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq; // Añadido para IQueryable
using System.Threading.Tasks;

namespace BLL.Implementacion
{
    public class PermisoServices : IPermisoServices
    {
        private readonly IGenericRepository<Permiso> _repositorio;
        private readonly ILogger<PermisoServices> _logger;

        public PermisoServices(IGenericRepository<Permiso> repositorio, ILogger<PermisoServices> logger)
        {
            _repositorio = repositorio;
            _logger = logger;
        }

        public async Task<List<Permiso>> Listar()
        {
            try
            {
                IQueryable<Permiso> query = await _repositorio.Consultar();
                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar los permisos.");
                throw;
            }
        }

        public async Task<Permiso> Crear(Permiso entidad)
        {
            if (entidad == null)
                throw new ArgumentNullException(nameof(entidad));
            
            try
            {
                var permisoExistente = await _repositorio.Obtener(p => p.IdPermiso == entidad.IdPermiso);
                if (permisoExistente != null)
                {
                    throw new InvalidOperationException($"El permiso con ID '{entidad.IdPermiso}' ya existe.");
                }

                return await _repositorio.Crear(entidad);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el permiso.");
                throw;
            }
        }

        public async Task<Permiso> Editar(Permiso entidad)
        {
            if (entidad == null)
                throw new ArgumentNullException(nameof(entidad));

            try
            {
                var permisoExistente = await _repositorio.Obtener(p => p.IdPermiso == entidad.IdPermiso);
                if (permisoExistente == null)
                {
                    throw new InvalidOperationException($"No se encontró el permiso con ID '{entidad.IdPermiso}'.");
                }
                
                permisoExistente.Descripcion = entidad.Descripcion;
                
                bool resultado = await _repositorio.Editar(permisoExistente);
                if (!resultado)
                {
                    throw new Exception("No se pudo editar el permiso.");
                }
                
                return permisoExistente;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al editar el permiso.");
                throw;
            }
        }

        public async Task<bool> Eliminar(string idPermiso)
        {
            try
            {
                var permiso = await _repositorio.Obtener(p => p.IdPermiso == idPermiso);
                if (permiso == null)
                {
                    throw new InvalidOperationException($"No se encontró el permiso con ID '{idPermiso}'.");
                }

                return await _repositorio.Eliminar(permiso);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el permiso.");
                throw;
            }
        }
    }
}
