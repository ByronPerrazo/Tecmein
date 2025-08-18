using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using System; 
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Implementacion
{
    public class ClienteServices : IClienteServices
    {
        private readonly IGenericRepository<Cliente> _repositorio;
        private readonly IConstructoraServices _constructoraServices;

        public ClienteServices(IGenericRepository<Cliente> repositorio, IConstructoraServices constructoraServices)
        {
            _repositorio = repositorio;
            _constructoraServices = constructoraServices;
        }

        public async Task<Cliente> Crear(Cliente entidad)
        {
            if (entidad == null) throw new ArgumentNullException(nameof(entidad));

            var clienteExistente = await _repositorio.Obtener(c => c.SecConstructora == entidad.SecConstructora);
            if (clienteExistente != null)
            {
                throw new Exception("La constructora ya es un cliente.");
            }

            var constructora = await _constructoraServices.ConstructoraPorSecuencial(entidad.SecConstructora);
            if (constructora == null)
            {
                throw new Exception("La constructora especificada no existe.");
            }

            // Lógica para generar NumeroCliente (simplificada por ahora)
            entidad.NumeroCliente = $"CLI-{constructora.Secuencial}";
            entidad.FechaCreacion = DateTime.Now;
            entidad.EstaActivo = true;

            await _repositorio.Crear(entidad);
            return entidad;
        }

        public async Task<Cliente> Editar(Cliente entidad)
        {
            if (entidad == null) throw new ArgumentNullException(nameof(entidad));

            var clienteExistente = await _repositorio.Obtener(c => c.SecCliente == entidad.SecCliente);
            if (clienteExistente == null)
            {
                throw new Exception("El cliente no existe.");
            }

            clienteExistente.NumeroCliente = entidad.NumeroCliente;
            clienteExistente.EstaActivo = entidad.EstaActivo;

            await _repositorio.Editar(clienteExistente);
            return clienteExistente;
        }

        public async Task<bool> Eliminar(int secCliente)
        {
            var cliente = await _repositorio.Obtener(c => c.SecCliente == secCliente);
            if (cliente == null)
            {
                throw new Exception("El cliente no existe.");
            }

            return await _repositorio.Eliminar(cliente);
        }

        public async Task<Cliente> ObtenerPorId(int secCliente)
        {
            return await _repositorio.Obtener(c => c.SecCliente == secCliente);
        }

        public async Task<Cliente> ObtenerPorIdConstructora(int secConstructora)
        {
            return await _repositorio.Obtener(c => c.SecConstructora == secConstructora);
        }
    }
}
