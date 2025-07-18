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
            // La consulta debe construirse sobre el IQueryable antes de la ejecución.
            IQueryable<Visita> query = await _repositorio.Consultar(x => x.Secuencial == secuencial);
            
            // Aplicar Includes para carga ansiosa (Eager Loading) y AsNoTracking para eficiencia.
            Visita visitaEncontrada = await query.Include(x => x.SecProvinciaNavigation)
                                                 .Include(x => x.SecCantonNavigation)
                                                 .Include(x => x.SecParroquiaNavigation)
                                                 .Include(u => u.SecUsuarioNavigation)
                                                 .AsNoTracking()
                                                 .FirstOrDefaultAsync();

            return visitaEncontrada;
        }

        public async Task<Visita> CreaVisita(Visita entidad)
        {
            try
            {
                Visita visitaCreada = await _repositorio.Crear(entidad);

                if (visitaCreada.Secuencial == 0)
                    throw new TaskCanceledException("No se pudo crear la visita.");

                // No es necesario volver a consultar, la entidad creada ya tiene los datos.
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
                // Obtenemos la visita original del repositorio. El DbContext la rastreará.
                var visitaOriginal = await _repositorio.Obtener(v => v.Secuencial == entidad.Secuencial);
                if (visitaOriginal == null)
                {
                    throw new KeyNotFoundException($"No se encontró la visita con el secuencial {entidad.Secuencial}");
                }

                // Actualizamos solo las propiedades necesarias.
                visitaOriginal.Nombre = entidad.Nombre;
                visitaOriginal.SecProvincia = entidad.SecProvincia;
                visitaOriginal.SecCanton = entidad.SecCanton;
                visitaOriginal.SecParroquia = entidad.SecParroquia;
                visitaOriginal.Direccion = entidad.Direccion;
                visitaOriginal.GeoUbicacion = string.IsNullOrEmpty(entidad.GeoUbicacion) ? "0,0" : entidad.GeoUbicacion.ToString();
                visitaOriginal.EstaActivo = entidad.EstaActivo;
                visitaOriginal.FechaSiguienteVisita = entidad.FechaSiguienteVisita;
                visitaOriginal.Detalle = entidad.Detalle;

                // Guardamos los cambios. EF Core se encarga de generar el UPDATE solo con los campos modificados.
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
            // Aplicar AsNoTracking para consultas de solo lectura mejora el rendimiento.
            var queryIncludes = query.Include(x => x.SecProvinciaNavigation)
                                      .Include(y => y.SecCantonNavigation)
                                      .Include(z => z.SecParroquiaNavigation)
                                      .Include(u => u.SecUsuarioNavigation)
                                      .AsNoTracking();

            return await queryIncludes.ToListAsync();
        }
    }
}
