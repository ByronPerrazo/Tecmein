using BLL.Interfaces;
using DAL.Interfaces;
using Entity;

namespace BLL.Implementacion
{
    public class EmpresaServices : IEmpresaServices
    {
        private readonly IGenericRepository<Empresa> _repositorio;
        private readonly IStorageServices _storageService;
        private readonly IEmpresaStorageServices _empresaStorageServices;
        public EmpresaServices(IGenericRepository<Empresa> repositorio,
                               IStorageServices storageService,
                               IEmpresaStorageServices empresaStorageServices)
        {
            _repositorio = repositorio;
            _storageService = storageService;
            _empresaStorageServices = empresaStorageServices;
        }

        public async Task<List<Empresa>> Lista()
        {
            var query = await _repositorio.Consultar();
            return [.. query];
        }
        public async Task<Empresa> Obtener()
        {
            try
            {
                var empresaEncontrada = await _repositorio.Obtener(x => x.Secuencial == 1);
                return empresaEncontrada;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Empresa> GuardarCambios(Empresa entidad, Stream logo = null, string NombreLogo = "")
        {
            try
            {
                if (await _repositorio.Obtener(x => x.Identificacion == entidad.Identificacion) != null)
                {
                    throw new TaskCanceledException($"Error Identificación Empresa Ya Registrada");
                }
                var empresaEncontrada
                    = new Empresa
                    {
                        Secuencial = entidad.Secuencial,
                        Identificacion = entidad.Identificacion,
                        Nombre = entidad.Nombre,
                        Correo = entidad.Correo,
                        Direccion = entidad.Direccion,
                        Telefono = entidad.Telefono,
                        CodigoOperador = entidad.CodigoOperador,
                        EstaActivo = entidad.EstaActivo,
                    };

                empresaEncontrada.NombreLogo
                        = empresaEncontrada.NombreLogo == ""
                        ? NombreLogo
                        : empresaEncontrada.NombreLogo;

                if (logo != null)
                {

                    var empresaStorage = await
                                         _empresaStorageServices
                                         .Consultar();

                    var almacenamientoEmpresa
                        = empresaStorage
                          .FirstOrDefault(x => x.SecEmpresa == 1)
                        ?? throw new TaskCanceledException($"Error Empresa No ha definido un FTP");

                    var urlLogo = await
                                  _storageService
                                  .SubirStorage(logo,
                                                almacenamientoEmpresa.CarpetaLogo,
                                                empresaEncontrada.NombreLogo);

                    empresaEncontrada.UrlLogo = urlLogo;

                }

                await _repositorio.Editar(empresaEncontrada);
                return empresaEncontrada;

            }
            catch
            {
                throw;
            }
        }

        public async Task<Empresa> Editar(Empresa entidad, Stream logo = null, string NombreLogo = "")
        {
            try
            {
                var registroDb = await _repositorio.Obtener(x => x.Secuencial == entidad.Secuencial)
                    ?? throw new TaskCanceledException("Registro No Existe");

                registroDb.Identificacion = entidad.Identificacion;
                registroDb.Nombre = entidad.Nombre;
                registroDb.Correo = entidad.Correo;
                registroDb.Direccion = entidad.Direccion;
                registroDb.Telefono = entidad.Telefono;
                registroDb.CodigoOperador = entidad.CodigoOperador;
                registroDb.EstaActivo = entidad.EstaActivo;

                if (logo != null)
                {
                    var nombreLogoAnterior = registroDb.NombreLogo;
                    var urlLogoAnterior = registroDb.UrlLogo;

                    var empresaStorage = await
                                         _empresaStorageServices
                                         .Consultar();

                    var almacenamientoEmpresa
                        = empresaStorage
                          .FirstOrDefault(x => x.SecEmpresa == 1)
                        ?? throw new TaskCanceledException($"Error Empresa No ha definido un FTP");

                    var urlLogo = await
                                  _storageService
                                  .SubirStorage(logo,
                                                almacenamientoEmpresa.CarpetaLogo,
                                                registroDb.NombreLogo);

                    registroDb.UrlLogo = urlLogo;

                    await _storageService
                            .EliminarStorage(almacenamientoEmpresa.CarpetaLogo,
                                             nombreLogoAnterior);

                }
                var empresaEditada = await _repositorio.Editar(registroDb);
                //return tipoProducto;

                if (empresaEditada)
                    registroDb = await _repositorio.Obtener(x => x.Secuencial == entidad.Secuencial);
                else
                    throw new TaskCanceledException("Error el Registrio no se puede guardar");

                return registroDb;

            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> Eliminar(int secuencial)
        {
            try
            {
                var seElimino = false;
                var empresa
                    = await _repositorio
                             .Consultar(x => x.Secuencial == secuencial);

                var tipo = empresa.FirstOrDefault();
                if (tipo != null)
                {
                    await _repositorio.Eliminar(tipo);
                    seElimino = true;
                }
                return seElimino;
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}
