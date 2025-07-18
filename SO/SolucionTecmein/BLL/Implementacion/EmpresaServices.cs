using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            IQueryable<Empresa> query = await _repositorio.Consultar();
            return query.ToList();
        }

        public async Task<Empresa> Obtener()
        {
            return await _repositorio.Obtener(x => x.Secuencial == 1);
        }

        public async Task<Empresa> Crear(Empresa entidad, Stream logo = null, string nombreLogo = "")
        {
            if (await _repositorio.Obtener(x => x.Identificacion == entidad.Identificacion) != null)
                throw new TaskCanceledException("Ya existe una empresa con esa identificación.");

            if (logo != null)
            {
                var (url, nombre) = await SubirLogo(logo, nombreLogo);
                entidad.UrlLogo = url;
                entidad.NombreLogo = nombre;
            }

            return await _repositorio.Crear(entidad);
        }

        public async Task<Empresa> Editar(Empresa entidad, Stream logo = null, string nombreLogo = "")
        {
            var empresaExistente = await _repositorio.Obtener(x => x.Secuencial == entidad.Secuencial)
                                   ?? throw new TaskCanceledException("La empresa no existe.");

            empresaExistente.Identificacion = entidad.Identificacion;
            empresaExistente.Nombre = entidad.Nombre;
            empresaExistente.Correo = entidad.Correo;
            empresaExistente.Direccion = entidad.Direccion;
            empresaExistente.Telefono = entidad.Telefono;
            empresaExistente.CodigoOperador = entidad.CodigoOperador;
            empresaExistente.EstaActivo = entidad.EstaActivo;

            if (logo != null)
            {
                if (!string.IsNullOrEmpty(empresaExistente.NombreLogo))
                {
                    var config = await ObtenerConfiguracionStorage();
                    await _storageService.EliminarStorage(config.CarpetaLogo, empresaExistente.NombreLogo);
                }

                var (url, nombre) = await SubirLogo(logo, nombreLogo, empresaExistente.NombreLogo);
                empresaExistente.UrlLogo = url;
                empresaExistente.NombreLogo = nombre;
            }

            bool seEdito = await _repositorio.Editar(empresaExistente);
            if (!seEdito)
                throw new TaskCanceledException("No se pudo editar la empresa.");

            return empresaExistente;
        }

        public async Task<bool> Eliminar(int secuencial)
        {
            var empresa = await _repositorio.Obtener(x => x.Secuencial == secuencial);
            if (empresa == null) return false;

            // Opcional: Eliminar logo del storage si existe
            if (!string.IsNullOrEmpty(empresa.NombreLogo))
            {
                try
                {
                    var config = await ObtenerConfiguracionStorage();
                    await _storageService.EliminarStorage(config.CarpetaLogo, empresa.NombreLogo);
                }
                catch (TaskCanceledException) { /* Ignorar si el FTP no está configurado */ }
            }

            return await _repositorio.Eliminar(empresa);
        }

        private async Task<(string Url, string Nombre)> SubirLogo(Stream logo, string nombreLogo, string nombreAnterior = null)
        {
            var config = await ObtenerConfiguracionStorage();
            string nombreParaGuardar = !string.IsNullOrEmpty(nombreLogo) ? nombreLogo : nombreAnterior ?? System.Guid.NewGuid().ToString("N");
            string url = await _storageService.SubirStorage(logo, config.CarpetaLogo, nombreParaGuardar);
            return (url, nombreParaGuardar);
        }

        private async Task<Empresastorage> ObtenerConfiguracionStorage()
        {
            var empresaStorage = await _empresaStorageServices.Consultar();
            return empresaStorage.FirstOrDefault(x => x.SecEmpresa == 1)
                   ?? throw new TaskCanceledException("La configuración de almacenamiento (FTP) para la empresa no está definida.");
        }
    }
}

