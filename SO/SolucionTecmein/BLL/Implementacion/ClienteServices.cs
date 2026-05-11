using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;

namespace BLL.Implementacion
{
    public class ClienteServices : IClienteServices
    {
        private readonly IGenericRepository<Cliente> _repositorioCliente;
        private readonly IGenericRepository<FormatoNumeroCliente> _repositorioFormato;
        private readonly IGenericRepository<Visita> _repositorioVisita;
        private readonly IGenericRepository<Cotizacion> _repositorioCotizacion;
        private readonly IGenericRepository<Contrato> _repositorioContrato;
        private readonly IGenericRepository<PlanDePago> _repositorioPlanDePago;
        private readonly IGenericRepository<Etapa> _repositorioEtapa;
        private readonly IGenericRepository<FormaPago> _repositorioFormaPago;
        private readonly IConstructoraServices _constructoraServices;
        private readonly IStorageServices _storageServices;
        private readonly IGenericRepository<Cuota> _repositorioCuota;
        private readonly IUnitOfWork _unitOfWork;

        public ClienteServices(
            IGenericRepository<Cliente> repositorioCliente,
            IGenericRepository<FormatoNumeroCliente> repositorioFormato,
            IGenericRepository<Visita> repositorioVisita,
            IGenericRepository<Cotizacion> repositorioCotizacion,
            IGenericRepository<Contrato> repositorioContrato,
            IGenericRepository<PlanDePago> repositorioPlanDePago,
            IGenericRepository<Etapa> repositorioEtapa,
            IGenericRepository<FormaPago> repositorioFormaPago,
            IConstructoraServices constructoraServices,
            IStorageServices storageServices,
            IGenericRepository<Cuota> repositorioCuota,
            IUnitOfWork unitOfWork)
        {
            _repositorioCliente = repositorioCliente;
            _repositorioFormato = repositorioFormato;
            _repositorioVisita = repositorioVisita;
            _repositorioCotizacion = repositorioCotizacion;
            _repositorioContrato = repositorioContrato;
            _repositorioPlanDePago = repositorioPlanDePago;
            _repositorioEtapa = repositorioEtapa;
            _repositorioFormaPago = repositorioFormaPago;
            _constructoraServices = constructoraServices;
            _storageServices = storageServices;
            _repositorioCuota = repositorioCuota;
            _unitOfWork = unitOfWork;
        }

        private async Task<string> FormatearNumeroCliente(int numero)
        {
            var formatoConfig = await _repositorioFormato.Obtener(f => f.UsaFormato);
            if (formatoConfig == null)
            {
                throw new Exception("La numeración automática de clientes no está configurada o está inactiva.");
            }

            string formato = formatoConfig.Formato ?? "";
            string prefijo = formato.Replace("{YYYY}", DateTime.Now.Year.ToString());
            string numeroFormateado = numero.ToString().PadLeft(formatoConfig.LongitudNumero, '0');

            return $"{prefijo}{numeroFormateado}";
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
            var query = await _repositorioCliente.Consultar(c => c.NumeroCliente.StartsWith(prefijo));
            var clientesConPrefijo = await query.ToListAsync();

            if (clientesConPrefijo.Any())
            {
                ultimoNumero = clientesConPrefijo
                    .Select(c => int.TryParse(c.NumeroCliente.Substring(prefijo.Length), out int num) ? num : 0)
                    .Max();
            }

            int proximoNumero = (ultimoNumero == 0) ? formatoConfig.NumeroInicio : ultimoNumero + 1;

            return await FormatearNumeroCliente(proximoNumero);
        }

        public async Task<Cliente> Crear(Cliente entidad)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                if (entidad == null) throw new ArgumentNullException(nameof(entidad));

                var clienteExistente = await _repositorioCliente.Obtener(c => c.SecConstructora == entidad.SecConstructora);
                if (clienteExistente != null)
                {
                    throw new Exception("La constructora ya es un cliente.");
                }

                var constructora = await _constructoraServices.ConstructoraPorSecuencial(entidad.SecConstructora);
                if (constructora == null)
                {
                    throw new Exception("La constructora especificada no existe.");
                }

                if (!string.IsNullOrEmpty(entidad.NumeroCliente))
                {
                    if (int.TryParse(entidad.NumeroCliente, out int numeroManual))
                    {
                        entidad.NumeroCliente = await FormatearNumeroCliente(numeroManual);
                    }
                    else
                    {
                        throw new Exception("El valor ingresado para 'Número Cliente' no es un número válido.");
                    }
                }
                else
                {
                    entidad.NumeroCliente = await GenerarSiguienteNumeroCliente();
                }

                entidad.FechaCreacion = DateTime.Now;
                entidad.EstaActivo = true;

                await _repositorioCliente.Crear(entidad);
                await _unitOfWork.CommitTransactionAsync();
                return entidad;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<Cliente> Editar(Cliente entidad)
        {
            if (entidad == null) throw new ArgumentNullException(nameof(entidad));

            var clienteExistente = await _repositorioCliente.Obtener(c => c.SecCliente == entidad.SecCliente);
            if (clienteExistente == null)
            {
                throw new Exception("El cliente no existe.");
            }

            clienteExistente.EstaActivo = entidad.EstaActivo;

            await _repositorioCliente.Editar(clienteExistente);
            return clienteExistente;
        }

        public async Task<bool> Eliminar(int secCliente)
        {
            var cliente = await _repositorioCliente.Obtener(c => c.SecCliente == secCliente);
            if (cliente == null)
            {
                throw new Exception("El cliente no existe.");
            }

            return await _repositorioCliente.Eliminar(cliente);
        }

        public async Task<Cliente> ObtenerPorId(int secCliente)
        {
            return await _repositorioCliente.Obtener(c => c.SecCliente == secCliente);
        }

        public async Task<Cliente> ObtenerPorIdConstructora(int secConstructora)
        {
            return await _repositorioCliente.Obtener(c => c.SecConstructora == secConstructora);
        }

        public async Task<List<Cliente>> Listar()
        {
            IQueryable<Cliente> query = await _repositorioCliente.Consultar();
            return await query.Include(c => c.SecConstructoraNavigation).ToListAsync();
        }

        public async Task<List<Cliente>> BuscarClientes(string terminoBusqueda)
        {
            try
            {
                IQueryable<Cliente> query = await _repositorioCliente.Consultar(
                    includeProperties: "SecConstructoraNavigation"
                );

                if (!string.IsNullOrWhiteSpace(terminoBusqueda))
                {
                    query = query.Where(c =>
                        c.NumeroCliente.Contains(terminoBusqueda) ||
                        (c.SecConstructoraNavigation != null && c.SecConstructoraNavigation.Nombre.Contains(terminoBusqueda))
                    );
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al buscar clientes: {ex.Message}", ex);
            }
        }

        public async Task<List<Contrato>> ObtenerContratosPorCliente(int secCliente)
        {
            try
            {
                IQueryable<Contrato> query = await _repositorioContrato.Consultar(c => c.SecCliente == secCliente);
                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener contratos por cliente: {ex.Message}", ex);
            }
        }

        public async Task<Cliente> ObtenerOCrearPorConstructora(int secConstructora)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var clienteExistente = await ObtenerPorIdConstructora(secConstructora);
                if (clienteExistente != null)
                {
                    // Si ya existe, no necesitamos la transacción. Pero como ya se inició, la cerramos limpiamente.
                    await _unitOfWork.CommitTransactionAsync();
                    return clienteExistente;
                }

                var nuevoCliente = new Cliente
                {
                    SecConstructora = secConstructora,
                    NumeroCliente = await GenerarSiguienteNumeroCliente(),
                    FechaCreacion = DateTime.Now,
                    EstaActivo = true
                };

                var clienteCreado = await _repositorioCliente.Crear(nuevoCliente);
                await _unitOfWork.CommitTransactionAsync();
                return clienteCreado;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}