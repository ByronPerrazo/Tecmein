using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                var empresaEncontrada = await _repositorio.Obtener(x => x.Secuencial == 1);
                    empresaEncontrada.NumeroDocumento = entidad.NumeroDocumento;
                    empresaEncontrada.Nombre = entidad.Nombre;
                    empresaEncontrada.Correo = entidad.Correo;
                    empresaEncontrada.Direccion = entidad.Direccion;
                    empresaEncontrada.Telefono = entidad.Telefono;
                    empresaEncontrada.PorcentajeImpuesto = entidad.PorcentajeImpuesto;
                    empresaEncontrada.SimboloMoneda = entidad.SimboloMoneda;

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


    }
}
