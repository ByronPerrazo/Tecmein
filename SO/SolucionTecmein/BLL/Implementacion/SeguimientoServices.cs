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
            var cotizacionIds = new List<int>();
            int? currentId = secCotizacion;

            while (currentId.HasValue && currentId.Value > 0)
            {
                cotizacionIds.Add(currentId.Value);
                var cotizacion = await _repositorioCotizacion.Obtener(c => c.Secuencial == currentId.Value);
                currentId = cotizacion?.SecCotizacionOriginal;
            }

            IQueryable<Seguimiento> query = await _repositorio.Consultar(s => cotizacionIds.Contains(s.SecCotizacion));
            return query.OrderByDescending(s => s.FechaAccion).ThenByDescending(s => s.FechaRegistro).ToList();
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

            // Cambiar la etapa de la Visita a "SEG" al crear cualquier seguimiento
            await _visitaServices.CambiarEtapa(cotizacion.SecVisita, "SEG");

            if (entidad.AceptacionCliente)
            {
                // 1. Cambiar la etapa de la Visita a "PRE" (Pre-Contrato)
                // Esto solo ocurrirá si "PRE" tiene un orden mayor que "SEG"
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

            // Obtener la cotización asociada para cambiar la etapa de la visita
            var cotizacion = await _repositorioCotizacion.Obtener(c => c.Secuencial == seguimientoExistente.SecCotizacion);
            if (cotizacion != null)
            {
                // Cambiar la etapa de la Visita a "SEG" al editar cualquier seguimiento
                await _visitaServices.CambiarEtapa(cotizacion.SecVisita, "SEG");

                // Si la edición establece AceptacionCliente, intentar cambiar a "PRE"
                if (seguimientoExistente.AceptacionCliente)
                {
                    await _visitaServices.CambiarEtapa(cotizacion.SecVisita, "PRE");
                    // Opcional: Actualizar Confirmacion en Cotizacion si es relevante para la edición
                    // cotizacion.Confirmacion = true;
                    // await _repositorioCotizacion.Editar(cotizacion);
                }
            }
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
