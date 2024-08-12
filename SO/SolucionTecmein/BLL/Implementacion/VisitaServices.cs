using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;

namespace BLL.Implementacion
{
    public class VisitaServices : IVisitaServices
    {
        public readonly IGenericRepository<Visita> _repositorio;
        public VisitaServices(IGenericRepository<Visita> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<Visita> ConsultaVisita(int secuencial)
        {
            var visita = await _repositorio.Consultar(x=>x.Secuencial == secuencial);
                visita
                      .Include(x => x.SecProvinciaNavigation)
                      .Include(x => x.SecCantonNavigation)
                      .Include(x => x.SecParroquiaNavigation)
                      .Include(u => u.SecUsuarioNavigation)
                      .FirstOrDefault();

             return visita.First();
        }

        public async Task<Visita> CreaVisita(Visita entidad)
        {
            Visita visitaProceso;
            try
            {
                visitaProceso = new Visita();
                visitaProceso = await _repositorio.Crear(entidad);

                if (visitaProceso.Secuencial == 0)
                    throw new TaskCanceledException($"Error Visita{ entidad.Nombre } No se Guarda");

                var vistaCreada = await ConsultaVisita(visitaProceso.Secuencial);
            }
            catch (Exception)
            {
                throw;
            }
            return visitaProceso;
        }

        public async Task<Visita> EditaVisita(Visita entidad)
        {
            var resultado = new Visita();
            try
            {
                var visitaProceso
                        = await ConsultaVisita(entidad.Secuencial);

                visitaProceso.Nombre = entidad.Nombre;
                visitaProceso.SecProvincia = entidad.SecProvincia;
                visitaProceso.SecCanton = entidad.SecCanton;
                visitaProceso.SecParroquia = entidad.SecParroquia;
                visitaProceso.Direccion = entidad.Direccion;
                visitaProceso.GeoUbicacion = string.IsNullOrEmpty(entidad.GeoUbicacion) ? "0,0": entidad.GeoUbicacion.ToString();
                visitaProceso.EstaActivo = entidad.EstaActivo;
                visitaProceso.FechaRegistro = DateTime.Now;
                visitaProceso.FechaSiguienteVisita = entidad.FechaSiguienteVisita;
                visitaProceso.Detalle = entidad.Detalle;

                await _repositorio.Editar(visitaProceso);
              

                var visitaModificada
                      = await _repositorio
                             .Consultar(x => x.Secuencial == entidad.Secuencial);

                var visitaProcesada 
                    = visitaModificada
                    .Include(x => x.SecProvinciaNavigation)
                    .Include(y => y.SecCantonNavigation)
                    .Include(z => z.SecParroquiaNavigation)
                    .Include(u => u.SecUsuarioNavigation)
                    .First();

                return visitaProcesada;

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
            var queryIncludes =  query.Include(x => x.SecProvinciaNavigation)
                                      .Include(y => y.SecCantonNavigation)
                                      .Include(z => z.SecParroquiaNavigation)
                                      .Include(u => u.SecUsuarioNavigation)
                                      .ToList();
            return queryIncludes;
        }
    }
}
