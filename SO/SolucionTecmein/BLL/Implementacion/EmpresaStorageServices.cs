using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Implementacion
{
    public class EmpresaStorageServices(IGenericRepository<Empresastorage> repositorio) : IEmpresaStorageServices
    {
        private readonly IGenericRepository<Empresastorage> _repositorio = repositorio;

        public Task<Empresastorage> Obtener(int secuencialEmpresa)
            => _repositorio.Obtener(x => x.SecEmpresa == secuencialEmpresa);

        public async Task<List<Empresastorage>> Consultar()
        {
            var query = await _repositorio.Consultar();
            return await query.Include(x => x.SecEmpresaNavigation).ToListAsync();
        }

        public async Task<Empresastorage> ProcesaGuardar(Empresastorage empresaStorage)
        {
            var registroExistente = await _repositorio.Obtener(x => x.SecEmpresa == empresaStorage.SecEmpresa);

            if (registroExistente != null)
            {
                // Editar
                registroExistente.CarpetaLogo = empresaStorage.CarpetaLogo;
                registroExistente.Email = empresaStorage.Email;
                registroExistente.Clave = empresaStorage.Clave;
                registroExistente.Ruta = empresaStorage.Ruta;
                registroExistente.ApiKey = empresaStorage.ApiKey;
                registroExistente.CarpetaUsuario = empresaStorage.CarpetaUsuario;
                registroExistente.CarpetaProducto = empresaStorage.CarpetaProducto;
                bool seEdito = await _repositorio.Editar(registroExistente);
                if (!seEdito)
                    throw new TaskCanceledException("No se pudo actualizar la configuración de almacenamiento.");
                
                return registroExistente;
            }
            else
            {
                // Crear
                return await _repositorio.Crear(empresaStorage);
            }
        }
    }
}
