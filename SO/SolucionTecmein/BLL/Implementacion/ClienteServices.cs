using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Implementacion
{
    public class ClienteServices : IClienteServices
    {
        private readonly IGenericRepository<Cliente> _repositorio;
        private readonly IGenericRepository<FormatoNumeroCliente> _repositorioFormato;
        private readonly IConstructoraServices _constructoraServices;

        public ClienteServices(
            IGenericRepository<Cliente> repositorio, 
            IGenericRepository<FormatoNumeroCliente> repositorioFormato,
            IConstructoraServices constructoraServices)
        {
            _repositorio = repositorio;
            _repositorioFormato = repositorioFormato;
            _constructoraServices = constructoraServices;
        }

        public async Task<string> GenerarSiguienteNumeroCliente()
        {
            var formatoConfig = await _repositorioFormato.Obtener(f => f.UsaFormato);
            if (formatoConfig == null)
            {
                throw new Exception("La numeración automática de clientes no está configurada o está inactiva.");
            }

            string formato = formatoConfig.Formato ?? "";
            string prefijo = formato.Replace("{YYYY}", DateTime.Now.Year.ToString());

            int ultimoNumero = 0;
            var query = await _repositorio.Consultar(c => c.NumeroCliente.StartsWith(prefijo));
            var clientesConPrefijo = await query.ToListAsync();

            if (clientesConPrefijo.Any())
            {
                ultimoNumero = clientesConPrefijo
                    .Select(c => int.TryParse(c.NumeroCliente.Substring(prefijo.Length), out int num) ? num : 0)
                    .Max();
            }

            int proximoNumero = (ultimoNumero == 0) ? formatoConfig.NumeroInicio : ultimoNumero + 1;

            string numeroFormateado = proximoNumero.ToString().PadLeft(formatoConfig.LongitudNumero, '0');

            return $"{prefijo}{numeroFormateado}";
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

            entidad.NumeroCliente = await GenerarSiguienteNumeroCliente();
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

            // No se debería poder cambiar el número de cliente una vez creado
            // clienteExistente.NumeroCliente = entidad.NumeroCliente;
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