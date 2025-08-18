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
    public class TipoImpuestoServices : ITipoImpuestoServices
    {
        private readonly IGenericRepository<TipoImpuesto> _repositorio;

        public TipoImpuestoServices(IGenericRepository<TipoImpuesto> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<List<TipoImpuesto>> Lista()
        {
            var query = await _repositorio.Consultar(t => t.EstaActivo == true);
            return await query.ToListAsync();
        }

        public async Task<TipoImpuesto> Crear(TipoImpuesto entidad)
        {
            try
            {
                entidad.FechaCreacion = DateTime.Now;
                TipoImpuesto tipoImpuestoCreado = await _repositorio.Crear(entidad);
                if (tipoImpuestoCreado.Secuencial == 0)
                    throw new TaskCanceledException("No se pudo crear el tipo de impuesto.");

                return tipoImpuestoCreado;
            }
            catch
            {
                throw;
            }
        }

        public async Task<TipoImpuesto> Editar(TipoImpuesto entidad)
        {
            try
            {
                var tipoImpuestoOriginal = await _repositorio.Obtener(t => t.Secuencial == entidad.Secuencial);
                if (tipoImpuestoOriginal == null)
                    throw new KeyNotFoundException($"No se encontró el tipo de impuesto con el secuencial {entidad.Secuencial}");

                tipoImpuestoOriginal.Nombre = entidad.Nombre;
                tipoImpuestoOriginal.EsIva = entidad.EsIva;
                tipoImpuestoOriginal.EsImportacion = entidad.EsImportacion;
                tipoImpuestoOriginal.EstaActivo = entidad.EstaActivo;
                tipoImpuestoOriginal.FechaModificacion = DateTime.Now;

                bool seEdito = await _repositorio.Editar(tipoImpuestoOriginal);
                if (!seEdito)
                    throw new Exception("No se pudo editar el tipo de impuesto.");

                return tipoImpuestoOriginal;
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
                var tipoImpuesto = await _repositorio.Obtener(t => t.Secuencial == id);
                if (tipoImpuesto == null)
                {
                    return false;
                }
                tipoImpuesto.EstaActivo = false;
                bool resultado = await _repositorio.Editar(tipoImpuesto);
                return resultado;
            }
            catch
            {
                throw;
            }
        }

        public async Task<TipoImpuesto> ObtenerPorId(int id)
        {
            IQueryable<TipoImpuesto> query = await _repositorio.Consultar(t => t.Secuencial == id);
            var tipoImpuesto = await query.FirstOrDefaultAsync();
            return tipoImpuesto;
        }
    }
}