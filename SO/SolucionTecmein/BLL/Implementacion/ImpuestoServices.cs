using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Implementacion
{
    public class ImpuestoServices : IImpuestoServices
    {
        private readonly IGenericRepository<Impuesto> _repositorio;

        public ImpuestoServices(IGenericRepository<Impuesto> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<List<Impuesto>> Lista()
        {
            var query = await _repositorio.Consultar();
            return await query.Include(i => i.SecTipoImpuestoNavigation).ToListAsync();
        }

        public async Task<List<Impuesto>> ListaActivos()
        {
            var query = await _repositorio.Consultar(i => i.Vigente == true);
            return await query.Include(i => i.SecTipoImpuestoNavigation).ToListAsync();
        }

        public async Task<Impuesto> Crear(Impuesto entidad)
        {
            try
            {
                entidad.FechaCreacion = DateTime.Now;
                Impuesto impuestoCreado = await _repositorio.Crear(entidad);
                if (impuestoCreado.Id == 0)
                    throw new TaskCanceledException("No se pudo crear el impuesto.");

                return impuestoCreado;
            }
            catch (DbUpdateException ex)
            {
                // Check for duplicate entry error (specific to MySQL or generic unique constraint violation)
                if (ex.InnerException != null && ex.InnerException.Message.Contains("Duplicate entry"))
                {
                    throw new InvalidOperationException("El código de impuesto ya existe. Por favor, ingrese un código único.", ex);
                }
                throw; // Re-throw other DbUpdateExceptions
            }
            catch (Exception)
            {
                throw; // Re-throw any other generic exceptions
            }
        }

        public async Task<Impuesto> Editar(Impuesto entidad)
        {
            try
            {
                var impuestoOriginal = await _repositorio.Obtener(i => i.Id == entidad.Id);
                if (impuestoOriginal == null)
                    throw new KeyNotFoundException($"No se encontró el impuesto con el ID {entidad.Id}");

                impuestoOriginal.Codigo = entidad.Codigo;
                impuestoOriginal.Descripcion = entidad.Descripcion;
                impuestoOriginal.Porcentaje = entidad.Porcentaje;
                impuestoOriginal.ValorFijo = entidad.ValorFijo;
                impuestoOriginal.CodigoSri = entidad.CodigoSri;
                impuestoOriginal.SecTipoImpuesto = entidad.SecTipoImpuesto;
                impuestoOriginal.Vigente = entidad.Vigente;
                impuestoOriginal.FechaModificacion = DateTime.Now;

                bool seEdito = await _repositorio.Editar(impuestoOriginal);
                if (!seEdito)
                    throw new Exception("No se pudo editar el impuesto.");

                return impuestoOriginal;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Eliminar(int id)
        {
            try
            {
                var impuesto = await _repositorio.Obtener(i => i.Id == id);
                if (impuesto == null)
                {
                    return false;
                }
                // Consider soft delete if applicable, for now, setting Vigente to false
                impuesto.Vigente = false;
                bool resultado = await _repositorio.Editar(impuesto);
                return resultado;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Impuesto> ObtenerPorId(int id)
        {
            IQueryable<Impuesto> query = await _repositorio.Consultar(i => i.Id == id);
            var impuesto = await query.Include(i => i.SecTipoImpuestoNavigation).FirstOrDefaultAsync();
            return impuesto;
        }
    }
}
