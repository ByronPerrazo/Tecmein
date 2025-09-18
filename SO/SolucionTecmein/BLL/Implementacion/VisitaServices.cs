using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;

namespace BLL.Implementacion
{
    public class VisitaServices : IVisitaServices
    {
        private readonly IGenericRepository<Visita> _repositorio;
        private readonly IGenericRepository<Equiposvisita> _repositorioEquipos;
        private readonly IEtapaServices _etapaServices;

        public VisitaServices(
            IGenericRepository<Visita> repositorio, 
            IGenericRepository<Equiposvisita> repositorioEquipos, 
            IEtapaServices etapaServices
            )
        {
            _repositorio = repositorio;
            _repositorioEquipos = repositorioEquipos;
            _etapaServices = etapaServices;
        }

        public async Task<Visita> ConsultaVisita(int secuencial)
        {
            IQueryable<Visita> query = await _repositorio.Consultar(x => x.Secuencial == secuencial);
            
            Visita visitaEncontrada = await query.Include(x => x.SecProvinciaNavigation)
                                                 .Include(x => x.SecCantonNavigation)
                                                 .Include(x => x.SecParroquiaNavigation)
                                                 .Include(u => u.SecUsuarioNavigation)
                                                 .Include(e => e.IdEtapaNavigation) // <-- Added
                                                 .Include(em => em.SecEmpresaNavigation) // <-- Added
                                                 .AsNoTracking()
                                                 .FirstOrDefaultAsync();

            return visitaEncontrada;
        }

        public async Task<Visita> CreaVisita(Visita entidad)
        {
            try
            {
                var etapaInicial = await _etapaServices.ObtenerPorCodigo("VIS");
                if (etapaInicial == null) throw new TaskCanceledException("No se encontró la etapa inicial 'VIS'.");

                entidad.IdEtapa = etapaInicial.Id;

                Visita visitaCreada = await _repositorio.Crear(entidad);

                if (visitaCreada.Secuencial == 0)
                    throw new TaskCanceledException("No se pudo crear la visita.");

                return visitaCreada;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Visita> EditaVisita(Visita entidad)
        {
            try
            {
                var visitaOriginal = await _repositorio.Obtener(v => v.Secuencial == entidad.Secuencial, "IdEtapaNavigation");
                if (visitaOriginal == null)
                {
                    throw new KeyNotFoundException($"No se encontró la visita con el secuencial {entidad.Secuencial}");
                }

                if (visitaOriginal.IdEtapaNavigation.Codigo == "PRE" || visitaOriginal.IdEtapaNavigation.Codigo == "SEG" || visitaOriginal.IdEtapaNavigation.Codigo == "COT")
                {
                    throw new InvalidOperationException("No se puede editar una visita que ya está en etapa de cotización, pre-contrato o seguimiento.");
                }

                visitaOriginal.Nombre = entidad.Nombre;
                visitaOriginal.SecProvincia = entidad.SecProvincia;
                visitaOriginal.SecCanton = entidad.SecCanton;
                visitaOriginal.SecParroquia = entidad.SecParroquia;
                visitaOriginal.Direccion = entidad.Direccion;
                visitaOriginal.GeoUbicacion = string.IsNullOrEmpty(entidad.GeoUbicacion) ? "0,0" : entidad.GeoUbicacion.ToString();
                visitaOriginal.EstaActivo = entidad.EstaActivo;
                visitaOriginal.FechaSiguienteVisita = entidad.FechaSiguienteVisita;
                visitaOriginal.Detalle = entidad.Detalle;

                bool seEdito = await _repositorio.Editar(visitaOriginal);
                if (!seEdito)
                {
                    throw new Exception("No se pudo editar la visita.");
                }

                return visitaOriginal;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Eliminar(int secuencial)
        {
            try
            {
                var seElimino = false;
                var tipoProducto
                    = await _repositorio
                             .Consultar(x => x.Secuencial == secuencial);

                var tipo = tipoProducto.FirstOrDefault();
                if (tipo != null)
                {
                    var usuarioGenerado = await _repositorio.Eliminar(tipo);
                    seElimino = true;
                }
                return seElimino;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<Visita>> ListaVisitas()
        {
            var query = await _repositorio.Consultar();
            var queryIncludes = query.Include(x => x.SecProvinciaNavigation)
                                      .Include(y => y.SecCantonNavigation)
                                      .Include(z => z.SecParroquiaNavigation)
                                      .Include(u => u.SecUsuarioNavigation)
                                      .Include(e => e.IdEtapaNavigation) // <-- Added
                                      .Include(em => em.SecEmpresaNavigation) // <-- Added
                                      .AsNoTracking();

            return await queryIncludes.ToListAsync();
        }

        public async Task<List<Visita>> ListaVisitasPorUsuario(int idUsuario)
        {
            var query = await _repositorio.Consultar(v => v.SecUsuario == idUsuario);
            var queryIncludes = query.Include(x => x.SecProvinciaNavigation)
                                      .Include(y => y.SecCantonNavigation)
                                      .Include(z => z.SecParroquiaNavigation)
                                      .Include(u => u.SecUsuarioNavigation)
                                      .Include(e => e.IdEtapaNavigation) // <-- Added
                                      .Include(em => em.SecEmpresaNavigation) // <-- Added
                                      .AsNoTracking();

            return await queryIncludes.ToListAsync();
        }

        public async Task<Visita> ObtenerDetalleVisita(int secuencial)
        {
            IQueryable<Visita> query = await _repositorio.Consultar(v => v.Secuencial == secuencial);

            var visitaDetalle = await query
                .Include(v => v.SecProvinciaNavigation)
                .Include(v => v.SecCantonNavigation)
                .Include(v => v.SecParroquiaNavigation)
                .Include(v => v.SecUsuarioNavigation)
                .Include(v => v.IdEtapaNavigation) // <-- Added
                .Include(v => v.SecEmpresaNavigation) // <-- Added
                .Include(v => v.Contactovisita)
                    .ThenInclude(cv => cv.SecContactoNavigation)
                        .ThenInclude(c => c.SecConstructoraNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return visitaDetalle;
        }

        public async Task<List<Visita>> ListaConEquipos()
        {
            IQueryable<Equiposvisita> equiposQuery = await _repositorioEquipos.Consultar();
            List<int> visitaIdsConEquipos = await equiposQuery.Select(e => e.SecVisita).Distinct().ToListAsync();

            IQueryable<Visita> visitasQuery = await _repositorio.Consultar(v => visitaIdsConEquipos.Contains(v.Secuencial));

            return await visitasQuery.AsNoTracking().ToListAsync();
        }

        public async Task<bool> CambiarEtapa(int secVisita, string nuevoCodigoEtapa)
        {
            try
            {
                var visita = await _repositorio.Obtener(v => v.Secuencial == secVisita, "IdEtapaNavigation");
                if (visita == null) throw new KeyNotFoundException("Visita no encontrada.");

                var etapaActual = await _etapaServices.ObtenerPorCodigo(visita.IdEtapaNavigation.Codigo);
                var nuevaEtapa = await _etapaServices.ObtenerPorCodigo(nuevoCodigoEtapa);

                if (nuevaEtapa == null) throw new KeyNotFoundException("La nueva etapa no es válida.");

                // Regla de negocio: Si la etapa actual es "SEG", no se puede retroceder a "COT".
                if (etapaActual.Codigo == "SEG" && nuevoCodigoEtapa == "COT")
                {
                    // No se hace nada, se mantiene en SEG
                    return true; // Se considera exitoso porque no se necesita cambiar
                }

                // Regla de negocio general: No se puede retroceder en el flujo de etapas (basado en orden).
                if (nuevaEtapa.Orden < etapaActual.Orden)
                {
                    throw new InvalidOperationException("No se puede retroceder a una etapa anterior.");
                }

                visita.IdEtapa = nuevaEtapa.Id;
                bool resultado = await _repositorio.Editar(visita);
                return resultado;
            }
            catch
            {
                throw;
            }
        }
    }
}
