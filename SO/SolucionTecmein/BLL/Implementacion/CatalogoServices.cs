using BLL.Interfaces;
using DAL.Interfaces;
using Entity;

namespace BLL.Implementacion
{
    public class CatalogoServices : ICatalogoServices
    {
        private readonly IGenericRepository<Catalogo> _repositorio;
        private readonly IStorageServices _storageServies;
        private readonly IEmpresaStorageServices _empresaStorageServices;
        private readonly IDatosGlobalesServices _datosGlobalesServices;
        public CatalogoServices(IGenericRepository<Catalogo> repositorio
                               , IStorageServices storageServies
                               , IEmpresaStorageServices empresaStorageServices
                               , IDatosGlobalesServices datosGlobalesServices

            )
        {
            _repositorio = repositorio;
            _storageServies = storageServies;
            _empresaStorageServices = empresaStorageServices;
            _datosGlobalesServices = datosGlobalesServices;
        }

        public async Task<Catalogo> CatalogoPorSecuencial(int secuencialCatalogo)
        {
            var query =
                await _repositorio
                      .Consultar(x => x.Secuencial == secuencialCatalogo);
            return query.FirstOrDefault();
        }

        public Task<List<Catalogo>> CatalogosPorRol(int secuencialRol)
        {
            throw new NotImplementedException();
        }

        public async Task<Catalogo> Crear(Catalogo entidad, Stream? archivo = null, string nombreArchivo = "")
        {
            var catalogoDb
                    = await _repositorio
                            .Obtener(x => x.Nombre
                                         .Equals(entidad.Nombre));
            if (catalogoDb != null)
                throw new TaskCanceledException("El Nombre Ingresado Ya Existe");

            var empresaStorage
                = await _empresaStorageServices
                        .Consultar();

            var almacenamientoEmpresa
                = empresaStorage
                  .FirstOrDefault(x => x.SecEmpresa == _datosGlobalesServices.SecuencialEmpresaPrincipal)
                  ?? throw new TaskCanceledException($"Error Empresa No ha definido un FTP");

            var rutaGuardada =
            await _storageServies
                    .SubirStorage(archivo,
                                  _datosGlobalesServices.PathCatalogos,
                                  nombreArchivo);

            if (rutaGuardada != null)
            {
                entidad.UrlCatalogo = rutaGuardada;
                entidad.NombreArchivo = nombreArchivo;
            }

            else
                throw new TaskCanceledException($"Error No se genera una url para el Archivo");

            var catalogoGenerado = await _repositorio.Crear(entidad);

            if (catalogoGenerado.Secuencial == 0)
                throw new TaskCanceledException($"Error el Catalogo {entidad.Nombre} No se pudo Generar");

            var catalgoAdquirido = await _repositorio.Consultar(x => x.Secuencial == catalogoGenerado.Secuencial);

            catalogoGenerado
                = catalgoAdquirido
                  .First();

            return catalogoGenerado;

        }

        public async Task<Catalogo> Editar(Catalogo entidad, Stream? archivo = null, string nombreArchivo = "")
        {

            var catalogoDb
                    = await _repositorio
                            .Obtener(x => x.Secuencial == entidad.Secuencial);

            if (catalogoDb == null)
                throw new TaskCanceledException("El Catalogo No Existe");

            var empresaStorage
                = await _empresaStorageServices
                        .Consultar();

            var almacenamientoEmpresa
                = empresaStorage
                  .FirstOrDefault(x => x.SecEmpresa == _datosGlobalesServices.SecuencialEmpresaPrincipal)
                  ?? throw new TaskCanceledException($"Error Empresa No ha definido un FTP");

            // Actualizar propiedades
            catalogoDb.Nombre = entidad.Nombre;
            catalogoDb.EstaActivo = entidad.EstaActivo;

            if (archivo != null)
            {
                string rutaGuardada =
                await _storageServies
                        .SubirStorage(archivo,
                                      _datosGlobalesServices.PathCatalogos,
                                      nombreArchivo);

                if (rutaGuardada != null)
                {
                    catalogoDb.UrlCatalogo = rutaGuardada;
                    catalogoDb.NombreArchivo = nombreArchivo;
                }
                else
                {
                    throw new TaskCanceledException($"Error No se genera una url para el Archivo");
                }
            }

            bool editado = await _repositorio.Editar(catalogoDb); // Llamar a Editar, no a Crear

            if (!editado)
                throw new TaskCanceledException($"Error el Catalogo {entidad.Nombre} No se pudo Editar");

            // Volver a obtener la entidad actualizada con propiedades de navegación si es necesario, similar a Crear
            var catalogoAdquirido = await _repositorio.Consultar(x => x.Secuencial == catalogoDb.Secuencial);
            return catalogoAdquirido.First();
        }

        public async Task<bool> Eliminar(int secuencial)
        {
            try
            {
                var seElimino = false;
                var documento
                    = await _repositorio
                             .Consultar(x => x.Secuencial == secuencial);

                var doc = documento.FirstOrDefault();
                if (doc != null)
                {
                    seElimino
                        = await _storageServies
                                .EliminarStorage(_datosGlobalesServices.PathCatalogos,
                                                 doc.NombreArchivo);
                    if (seElimino)
                    {
                        seElimino = await _repositorio.Eliminar(doc);
                    }
                    else
                        throw new TaskCanceledException("No se elmino el archivo del repositorio Web");

                }
                else
                    seElimino = true;

                return seElimino;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Catalogo>> Lista()
        {
            var query = await _repositorio.Consultar();
            return [.. query];
        }
    }
}
