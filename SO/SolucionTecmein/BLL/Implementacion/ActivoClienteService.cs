using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;

namespace BLL.Implementacion
{
    public class ActivoClienteService : IActivoClienteService
    {
        private readonly IGenericRepository<ActivoCliente> _repositorio;

        public ActivoClienteService(IGenericRepository<ActivoCliente> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<List<ActivoCliente>> Lista()
        {
            IQueryable<ActivoCliente> query = await _repositorio.Consultar();
            return await query.Include(ac => ac.SecClienteNavigation)
                              .Include(ac => ac.SecContratoOrigenNavigation)
                              .ToListAsync();
        }

        public async Task<ActivoCliente> Obtener(int idActivoCliente)
        {
            return await _repositorio.Obtener(ac => ac.IdActivoCliente == idActivoCliente);
        }

        public async Task<ActivoCliente> Crear(ActivoCliente entidad)
        {
            try
            {
                // Aquí podrías añadir validaciones adicionales si fueran necesarias
                ActivoCliente activoCliente_creado = await _repositorio.Crear(entidad);
                if (activoCliente_creado.IdActivoCliente == 0)
                    throw new Exception("No se pudo crear el activo del cliente");

                return activoCliente_creado;
            }
            catch
            {
                throw;
            }
        }

        public async Task<ActivoCliente> Editar(ActivoCliente entidad)
        {
            try
            {
                ActivoCliente activoCliente_encontrado = await _repositorio.Obtener(ac => ac.IdActivoCliente == entidad.IdActivoCliente);
                if (activoCliente_encontrado == null)
                    throw new Exception("El activo del cliente no existe");

                activoCliente_encontrado.SecCliente = entidad.SecCliente;
                activoCliente_encontrado.SecEquipo = entidad.SecEquipo;
                activoCliente_encontrado.Descripcion = entidad.Descripcion;
                activoCliente_encontrado.FechaInstalacion = entidad.FechaInstalacion;
                activoCliente_encontrado.SecContratoOrigen = entidad.SecContratoOrigen;

                bool respuesta = await _repositorio.Editar(activoCliente_encontrado);
                if (!respuesta)
                    throw new Exception("No se pudo modificar el activo del cliente");

                return activoCliente_encontrado;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Eliminar(int idActivoCliente)
        {
            try
            {
                ActivoCliente activoCliente_encontrado = await _repositorio.Obtener(ac => ac.IdActivoCliente == idActivoCliente);
                if (activoCliente_encontrado == null)
                    throw new Exception("El activo del cliente no existe");

                bool respuesta = await _repositorio.Eliminar(activoCliente_encontrado);
                return respuesta;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<ActivoCliente>> ObtenerPorCliente(int secCliente)
        {
            IQueryable<ActivoCliente> query = await _repositorio.Consultar(ac => ac.SecCliente == secCliente);
            return await query.Include(ac => ac.SecClienteNavigation)
                              .Include(ac => ac.SecContratoOrigenNavigation)
                              .ToListAsync();
        }

        public async Task<List<ActivoCliente>> ObtenerPorContratoOrigen(int secContratoOrigen)
        {
            IQueryable<ActivoCliente> query = await _repositorio.Consultar(ac => ac.SecContratoOrigen == secContratoOrigen);
            return await query.Include(ac => ac.SecClienteNavigation)
                              .Include(ac => ac.SecContratoOrigenNavigation)
                              .ToListAsync();
        }
    }
}
