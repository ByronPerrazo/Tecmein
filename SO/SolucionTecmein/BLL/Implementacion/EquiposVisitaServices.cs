using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Implementacion
{
    public class EquiposVisitaServices : IEquiposVisitaServices
    {
        private readonly IGenericRepository<Equiposvisita> _repositorioEquiposVisita;
        private readonly IVisitaServices _visitaServices;
        private readonly IValidacionServices _validacionServices;
        private readonly IGenericRepository<Cotizacion> _repositorioCotizacion; // New
        private readonly IGenericRepository<Cotizaciondetalle> _repositorioCotizaciondetalle; // New

        public EquiposVisitaServices(IGenericRepository<Equiposvisita> repositorioEquiposVisita,
                                     IVisitaServices visitaServices,
                                     IValidacionServices validacionServices,
                                     IGenericRepository<Cotizacion> repositorioCotizacion, // New
                                     IGenericRepository<Cotizaciondetalle> repositorioCotizaciondetalle) // New
        {
            _repositorioEquiposVisita = repositorioEquiposVisita;
            _visitaServices = visitaServices;
            _validacionServices = validacionServices;
            _repositorioCotizacion = repositorioCotizacion; // New
            _repositorioCotizaciondetalle = repositorioCotizaciondetalle; // New
        }

        public async Task<EquiposVisitaConEstadoCotizacion> ConsultaListaPorVisita(int secuencialVisita)
        {
            var equipos = await _repositorioEquiposVisita
                              .Consultar(x =>
                                         x.SecVisita == secuencialVisita &&
                                         x.EstaActivo == 1);

            var cotizacionActiva = await _repositorioCotizacion.Obtener(c =>
                c.SecVisita == secuencialVisita && c.EstaActivo.HasValue && c.EstaActivo.Value == 1);

            return new EquiposVisitaConEstadoCotizacion
            {
                Equipos = equipos.ToList(),
                CotizacionActivaExiste = (cotizacionActiva != null)
            };
        }

        public async Task<Equiposvisita> Obtener(int secuencial)
        {
            var query = await _repositorioEquiposVisita.Obtener(x => x.Secuencial == secuencial);
            return query;
        }

        public async Task<Equiposvisita> ProcesaGuardar(Equiposvisita equiposvisita)
        {
            var visita = await _visitaServices.ConsultaVisita(equiposvisita.SecVisita);
            if (visita.IdEtapaNavigation.Codigo == "PRE" || visita.IdEtapaNavigation.Codigo == "SEG" || visita.IdEtapaNavigation.Codigo == "COT")
            {
                throw new InvalidOperationException("No se puede agregar un equipo a una visita que ya está en etapa de cotización, pre-contrato o seguimiento.");
            }

            await ValidarEquipoVisita(equiposvisita);
            var equipoVisitaGuardado = await _repositorioEquiposVisita.Crear(equiposvisita);

            // Sincronizar con Cotizaciondetalle si existe una cotización activa para la visita
            var cotizacionActiva = await _repositorioCotizacion.Obtener(c =>
                c.SecVisita == equipoVisitaGuardado.SecVisita && c.EstaActivo.HasValue && c.EstaActivo.Value == 1);

            if (cotizacionActiva != null)
            {
                var cotizaciondetalle = new Cotizaciondetalle
                {
                    SecCotizacion = cotizacionActiva.Secuencial,
                    SecEquipoVisita = equipoVisitaGuardado.Secuencial, // Link to Equiposvisita
                    DetalleEquipo = $"Sistema:{equipoVisitaGuardado.Sistema} -" +
                                        $" Tipo Eq:{equipoVisitaGuardado.TipoEquipo} -" +
                                        $" Marca:{equipoVisitaGuardado.Marca} -" +
                                        $" Capacidad:{equipoVisitaGuardado.Capacidad} -" +
                                        $" Velocidad:{equipoVisitaGuardado.Velocidad} -" +
                                        $" Sala Maq:{equipoVisitaGuardado.SalaMaquinas} -" +
                                        $" Motor:{equipoVisitaGuardado.TipoMotor} -" +
                                        $" Embarque:{equipoVisitaGuardado.Embarque} -" +
                                        $" Ducto:{equipoVisitaGuardado.TipoDucto} -" +
                                        $" MedidasAF:{equipoVisitaGuardado.MedidasAfducto} -" +
                                        $" Foso:{equipoVisitaGuardado.Foso} -" +
                                        $" Recorrido:{equipoVisitaGuardado.Recorrido} -" +
                                        $" Sbr. Recorrido:{equipoVisitaGuardado.SobreRecorrido} -" +
                                        $" Ing. Frontales:{equipoVisitaGuardado.IngresosFrontales} -" +
                                        $" Ing. Posteriores:{equipoVisitaGuardado.IngresosPosteriores} -" +
                                        $" Dime Entrada:{equipoVisitaGuardado.DimencionEntrada} -" +
                                        $" Alt Entre Pisos:{equipoVisitaGuardado.AlturaEntrePisos} -" +
                                        $" Energia:{equipoVisitaGuardado.Energia} -" +
                                        $" Puertas:{equipoVisitaGuardado.MaterialPuertas} -" +
                                        $" Num. Paradas:{equipoVisitaGuardado.NumeroParadas} -" +
                                        $" Nomb. Paradas:{equipoVisitaGuardado.NombresParadas} -" +
                                        $" Num Personas:{equipoVisitaGuardado.NumeroPersonas}",
                    Cantidad = equipoVisitaGuardado.Cantidad ?? 0, // Use 0 if null
                    ValorCompra = 0, // Default value
                    MargenGanancia = 0, // Default value
                    Total = 0, // Will be calculated in frontend or when saving Cotizacion
                    EstaActivo = 1,
                    FechaRegistro = DateTime.Now
                };
                await _repositorioCotizaciondetalle.Crear(cotizaciondetalle);
            }

            return equipoVisitaGuardado;
        }

        public async Task<bool> ProcesaEliminar(Equiposvisita equiposvisita)
        {
            var equipoExistente = await _repositorioEquiposVisita.Obtener(x => x.Secuencial == equiposvisita.Secuencial);
            if (equipoExistente == null)
            {
                return false; // O lanzar una excepción si se prefiere
            }

            var visita = await _visitaServices.ConsultaVisita(equipoExistente.SecVisita);
            if (visita.IdEtapaNavigation.Codigo == "PRE" || visita.IdEtapaNavigation.Codigo == "SEG" || visita.IdEtapaNavigation.Codigo == "COT")
            {
                throw new InvalidOperationException("No se puede eliminar un equipo de una visita que ya está en etapa de cotización, pre-contrato o seguimiento.");
            }

            // Sincronizar con Cotizaciondetalle si existe una cotización activa para la visita
            var cotizacionActiva = await _repositorioCotizacion.Obtener(c =>
                c.SecVisita == equipoExistente.SecVisita && c.EstaActivo.HasValue && c.EstaActivo.Value == 1);

            if (cotizacionActiva != null)
            {
                var cotizaciondetalleAEliminar = await _repositorioCotizaciondetalle.Obtener(cd =>
                    cd.SecCotizacion == cotizacionActiva.Secuencial &&
                    cd.SecEquipoVisita == equipoExistente.Secuencial); // Use the new FK

                if (cotizaciondetalleAEliminar != null)
                {
                    await _repositorioCotizaciondetalle.Eliminar(cotizaciondetalleAEliminar);
                }
            }

            return await _repositorioEquiposVisita.Eliminar(equipoExistente);
        }

        private async Task ValidarEquipoVisita(Equiposvisita equipo)
        {
            // Validar SecVisita
            if (equipo.SecVisita <= 0)
            {
                throw new TaskCanceledException("La visita asociada al equipo es obligatoria.");
            }
            var visitaExistente = await _visitaServices.ConsultaVisita(equipo.SecVisita);
            if (visitaExistente == null)
            {
                throw new TaskCanceledException($"La visita con secuencial {equipo.SecVisita} no existe.");
            }

            // Validar Marca
            _validacionServices.ValidarNombre(equipo.Marca, "marca del equipo");

            // Validar Cantidad
            if (equipo.Cantidad == null || equipo.Cantidad <= 0)
            {
                throw new TaskCanceledException("La cantidad del equipo debe ser mayor que cero.");
            }
        }

        public async Task<bool> SincronizarEquiposConCotizacionActiva(int secVisita)
        {
            var cotizacionActiva = await _repositorioCotizacion.Obtener(c =>
                c.SecVisita == secVisita && c.EstaActivo.HasValue && c.EstaActivo.Value == 1);

            if (cotizacionActiva == null)
            {
                // No hay cotización activa para sincronizar
                return false;
            }

            var equiposVisita = (await _repositorioEquiposVisita.Consultar(ev => ev.SecVisita == secVisita && ev.EstaActivo == 1)).ToList();
            var cotizacionDetallesExistentes = (await _repositorioCotizaciondetalle.Consultar(cd => cd.SecCotizacion == cotizacionActiva.Secuencial)).ToList();

            var existingSecEquipoVisitaIds = new HashSet<int?>(cotizacionDetallesExistentes.Where(cd => cd.SecEquipoVisita.HasValue).Select(cd => cd.SecEquipoVisita));

            foreach (var equipoVisita in equiposVisita)
            {
                // Solo añadir si no existe un Cotizaciondetalle vinculado a este SecEquipoVisita
                if (!existingSecEquipoVisitaIds.Contains(equipoVisita.Secuencial))
                {
                    var cotizaciondetalle = new Cotizaciondetalle
                    {
                        SecCotizacion = cotizacionActiva.Secuencial,
                        SecEquipoVisita = equipoVisita.Secuencial, // Link to Equiposvisita
                        DetalleEquipo = $"Sistema:{equipoVisita.Sistema} -" +
                                        $" Tipo Eq:{equipoVisita.TipoEquipo} -" +
                                        $" Marca:{equipoVisita.Marca} -" +
                                        $" Capacidad:{equipoVisita.Capacidad} -" +
                                        $" Velocidad:{equipoVisita.Velocidad} -" +
                                        $" Sala Maq:{equipoVisita.SalaMaquinas} -" +
                                        $" Motor:{equipoVisita.TipoMotor} -" +
                                        $" Embarque:{equipoVisita.Embarque} -" +
                                        $" Ducto:{equipoVisita.TipoDucto} -" +
                                        $" MedidasAF:{equipoVisita.MedidasAfducto} -" +
                                        $" Foso:{equipoVisita.Foso} -" +
                                        $" Recorrido:{equipoVisita.Recorrido} -" +
                                        $" Sbr. Recorrido:{equipoVisita.SobreRecorrido} -" +
                                        $" Ing. Frontales:{equipoVisita.IngresosFrontales} -" +
                                        $" Ing. Posteriores:{equipoVisita.IngresosPosteriores} -" +
                                        $" Dime Entrada:{equipoVisita.DimencionEntrada} -" +
                                        $" Alt Entre Pisos:{equipoVisita.AlturaEntrePisos} -" +
                                        $" Energia:{equipoVisita.Energia} -" +
                                        $" Puertas:{equipoVisita.MaterialPuertas} -" +
                                        $" Num. Paradas:{equipoVisita.NumeroParadas} -" +
                                        $" Nomb. Paradas:{equipoVisita.NombresParadas} -" +
                                        $" Num Personas:{equipoVisita.NumeroPersonas}",
                        Cantidad = equipoVisita.Cantidad ?? 0, // Use 0 if null
                        ValorCompra = 0, // Default value
                        MargenGanancia = 0, // Default value
                        Total = 0, // Will be calculated in frontend or when saving Cotizacion
                        EstaActivo = 1,
                        FechaRegistro = DateTime.Now
                    };
                    await _repositorioCotizaciondetalle.Crear(cotizaciondetalle);
                }
            }

            return true;
        }
    }
}