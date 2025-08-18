using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Implementacion
{
    public class SeguimientoServices : ISeguimientoServices
    {
        private readonly IGenericRepository<Seguimiento> _repositorio;
        private readonly ICotizacionServices _cotizacionServices;
        private readonly IVisitaServices _visitaServices;
        private readonly IGenericRepository<Cotizacion> _repositorioCotizacion;

        public SeguimientoServices(IGenericRepository<Seguimiento> repositorio, ICotizacionServices cotizacionServices, IVisitaServices visitaServices, IGenericRepository<Cotizacion> repositorioCotizacion)
        {
            _repositorio = repositorio;
            _cotizacionServices = cotizacionServices;
            _visitaServices = visitaServices;
            _repositorioCotizacion = repositorioCotizacion;
        }

        public async Task<List<Seguimiento>> Lista(int secCotizacion)
        {
            IQueryable<Seguimiento> query = await _repositorio.Consultar(s => s.SecCotizacion == secCotizacion);
            return query.ToList();
        }

        public async Task<Seguimiento> Crear(Seguimiento entidad)
        {
            if (entidad == null) throw new ArgumentNullException(nameof(entidad));

            var cotizacion = await _repositorioCotizacion.Obtener(c => c.Secuencial == entidad.SecCotizacion); // Get tracked Cotizacion
            if (cotizacion == null)
            {
                throw new Exception("La cotización especificada no existe.");
            }

            entidad.FechaRegistro = DateTime.Now;
            var seguimientoCreado = await _repositorio.Crear(entidad);

            if (entidad.AceptacionCliente)
            {
                // 1. Cambiar la etapa de la Visita a "PRE" (Pre-Contrato)
                await _visitaServices.CambiarEtapa(cotizacion.SecVisita, "PRE");

                // 2. Actualizar el campo Confirmacion en la Cotizacion
                cotizacion.Confirmacion = true;
                await _repositorioCotizacion.Editar(cotizacion);
            }

            return seguimientoCreado;
        }

        public async Task<Seguimiento> Editar(Seguimiento entidad)
        {
            if (entidad == null) throw new ArgumentNullException(nameof(entidad));

            var seguimientoExistente = await _repositorio.Obtener(s => s.SecSeguimiento == entidad.SecSeguimiento);
            if (seguimientoExistente == null)
            {
                throw new Exception("El seguimiento no existe.");
            }

            seguimientoExistente.Accion = entidad.Accion;
            seguimientoExistente.Detalle = entidad.Detalle;
            seguimientoExistente.FechaAccion = entidad.FechaAccion;
            seguimientoExistente.AceptacionCliente = entidad.AceptacionCliente;

            await _repositorio.Editar(seguimientoExistente);
            return seguimientoExistente;
        }

        public async Task<bool> Eliminar(int secSeguimiento)
        {
            var seguimiento = await _repositorio.Obtener(s => s.SecSeguimiento == secSeguimiento);
            if (seguimiento == null)
            {
                throw new Exception("El seguimiento no existe.");
            }

            return await _repositorio.Eliminar(seguimiento);
        }
    }
}
