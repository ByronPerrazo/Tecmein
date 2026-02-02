using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BLL.Implementacion
{
    public class SeguimientoServices : ISeguimientoServices
    {
        private readonly IGenericRepository<Seguimiento> _repositorio;
        private readonly ICotizacionServices _cotizacionServices;
        private readonly IVisitaServices _visitaServices;
        private readonly IGenericRepository<Cotizacion> _repositorioCotizacion;
        private readonly IGenericRepository<PreContrato> _repositorioPreContrato; // Added

        public SeguimientoServices(IGenericRepository<Seguimiento> repositorio,
                                 ICotizacionServices cotizacionServices,
                                 IVisitaServices visitaServices,
                                 IGenericRepository<Cotizacion> repositorioCotizacion,
                                 IGenericRepository<PreContrato> repositorioPreContrato) // Added
        {
            _repositorio = repositorio;
            _cotizacionServices = cotizacionServices;
            _visitaServices = visitaServices;
            _repositorioCotizacion = repositorioCotizacion;
            _repositorioPreContrato = repositorioPreContrato; // Added
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
            return await query.OrderByDescending(s => s.FechaAccion).ThenByDescending(s => s.FechaRegistro).ToListAsync();
        }

        public async Task<Seguimiento> Crear(Seguimiento entidad)
        {
            if (entidad == null) throw new ArgumentNullException(nameof(entidad));

            var cotizacion = await _repositorioCotizacion.Obtener(c => c.Secuencial == entidad.SecCotizacion);
            if (cotizacion == null)
            {
                throw new Exception("La cotización especificada no existe.");
            }

            entidad.FechaRegistro = DateTime.Now;
            var seguimientoCreado = await _repositorio.Crear(entidad);

            await _visitaServices.CambiarEtapa(cotizacion.SecVisita, "SEG");

            if (entidad.AceptacionCliente)
            {
                await _visitaServices.CambiarEtapa(cotizacion.SecVisita, "ACE");

                cotizacion.Confirmacion = true;
                await _repositorioCotizacion.Editar(cotizacion);

                // Se comenta la creación automática para moverla a un proceso manual desde la pantalla de Pre-Contratos.
                /*
                var nuevoPreContrato = new PreContrato
                {
                    SecCotizacion = cotizacion.Secuencial,
                    SecPlantillaPreContrato = 1, 
                    SecUsuarioCrea = cotizacion.SecUsuario ?? 1, 
                    Version = 1,
                    Estado = "Borrador",
                    EstaActivo = true,
                    FechaRegistro = DateTime.Now
                };
                await _preContratoServices.Crear(nuevoPreContrato);
                */
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

            if (seguimientoExistente.AceptacionCliente)
            {
                var cotizacion = await _repositorioCotizacion.Obtener(c => c.Secuencial == seguimientoExistente.SecCotizacion);
                if (cotizacion != null)
                {
                    cotizacion.Confirmacion = true;
                    await _repositorioCotizacion.Editar(cotizacion);
                    await _visitaServices.CambiarEtapa(cotizacion.SecVisita, "ACE");
                }
            }

            return seguimientoExistente;
        }

        public async Task<bool> Eliminar(int secuencial)
        {
            try
            {
                var seguimiento = await _repositorio.Obtener(s => s.SecSeguimiento == secuencial);
                if (seguimiento == null)
                {
                    return false;
                }
                bool resultado = await _repositorio.Eliminar(seguimiento);
                return resultado;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<Cotizacion>> ObtenerCotizacionesAprobadasSinPreContrato()
        {
            var cotizacionesAprobadasQuery = await _repositorioCotizacion.Consultar(c => c.Confirmacion == true && c.EstaActivo == 1);
            var cotizacionesAprobadas = await cotizacionesAprobadasQuery.Include(c => c.SecVisitaNavigation).ToListAsync();

            var cotizacionesConPreContratoQuery = await _repositorioPreContrato.Consultar(p => p.EstaActivo);
            var cotizacionesConPreContratoIds = await cotizacionesConPreContratoQuery.Select(p => p.SecCotizacion).Distinct().ToListAsync();

            var cotizacionesSinPreContrato = cotizacionesAprobadas.Where(c => !cotizacionesConPreContratoIds.Contains(c.Secuencial));

            var cotizacionesFinales = cotizacionesSinPreContrato
                .GroupBy(c => c.SecVisita)
                .Select(g => g.OrderByDescending(c => c.FechaRegistro).First())
                .ToList();

            return cotizacionesFinales;
        }
    }
}
