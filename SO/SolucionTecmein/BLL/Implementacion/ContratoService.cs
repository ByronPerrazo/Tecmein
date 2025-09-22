using BLL.Interfaces;
using DAL.DBContext;
using DAL.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Implementacion
{
    public class ContratoService : IContratoService
    {
        private readonly IGenericRepository<Contrato> _repoContrato;
        private readonly IGenericRepository<Cotizacion> _repoCotizacion;
        private readonly IGenericRepository<PreContrato> _repoPreContrato;
        private readonly IClienteServices _clienteService;
        private readonly IStorageServices _storageService;
        private readonly TecmeindbContext _dbContext;

        public ContratoService(
            IGenericRepository<Contrato> repoContrato,
            IGenericRepository<Cotizacion> repoCotizacion,
            IGenericRepository<PreContrato> repoPreContrato,
            IClienteServices clienteService,
            IStorageServices storageService,
            TecmeindbContext dbContext)
        {
            _repoContrato = repoContrato;
            _repoCotizacion = repoCotizacion;
            _repoPreContrato = repoPreContrato;
            _clienteService = clienteService;
            _storageService = storageService;
            _dbContext = dbContext;
        }

        public async Task<Contrato> Crear(Contrato entidad, Stream archivoStream = null, string nombreArchivo = "")
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // 1. Obtener la cotización y la constructora asociada
                var cotizacion = await _repoCotizacion.Obtener(
                    c => c.Secuencial == entidad.IdCotizacion,
                    incluirPropiedades: "SecVisitaNavigation.Contactovisita.SecContactoNavigation.SecConstructoraNavigation"
                );

                if (cotizacion?.SecVisitaNavigation?.Contactovisita?.FirstOrDefault()?.SecContactoNavigation?.SecConstructoraNavigation == null)
                {
                    throw new TaskCanceledException("No se pudo encontrar la constructora asociada a la cotización.");
                }
                var constructora = cotizacion.SecVisitaNavigation.Contactovisita.First().SecContactoNavigation.SecConstructoraNavigation;

                // 2. Verificar si ya existe un cliente para la constructora
                var clienteExistente = await _clienteService.ObtenerPorIdConstructora(constructora.Secuencial);
                string numeroCliente;

                if (clienteExistente == null)
                {
                    // 3a. Si no existe, crear el nuevo cliente
                    var nuevoCliente = new Cliente
                    {
                        SecConstructora = constructora.Secuencial,
                        FechaCreacion = DateTime.Now,
                        EstaActivo = true
                    };
                    // El método Crear de ClienteService se encarga de generar el NumeroCliente
                    var clienteCreado = await _clienteService.Crear(nuevoCliente);
                    numeroCliente = clienteCreado.NumeroCliente;
                }
                else
                {
                    // 3b. Si ya existe, usar su número de cliente
                    numeroCliente = clienteExistente.NumeroCliente;
                }

                // 4. Subir el archivo del contrato usando el número de cliente para la ruta
                if (archivoStream != null && !string.IsNullOrEmpty(nombreArchivo))
                {
                    string carpetaDestino = $"Contratos/{numeroCliente}";
                    string urlArchivo = await _storageService.SubirStorage(archivoStream, carpetaDestino, nombreArchivo);
                    entidad.NombreArchivo = nombreArchivo;
                    entidad.RutaArchivo = urlArchivo;
                }

                // 5. Crear el contrato en la BD
                Contrato contratoCreado = await _repoContrato.Crear(entidad);
                if (contratoCreado.IdContrato == 0)
                {
                    throw new TaskCanceledException("No se pudo crear el contrato en la base de datos.");
                }

                // 6. Confirmar la transacción
                await transaction.CommitAsync();
                return contratoCreado;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error al crear el contrato: {ex.Message}", ex);
            }
        }

        // ... (Resto de los métodos Editar, Eliminar, Listar, etc. se mantienen igual por ahora)
        public async Task<Contrato> Editar(Contrato entidad, Stream archivoStream = null, string nombreArchivo = "")
        {
            var contratoExistente = await _repoContrato.Obtener(c => c.IdContrato == entidad.IdContrato);
            if (contratoExistente == null)
                throw new TaskCanceledException("El contrato no fue encontrado");

            contratoExistente.FechaFirma = entidad.FechaFirma;
            contratoExistente.EsActivo = entidad.EsActivo;
            
            // La lógica para editar el archivo es compleja y se abordará por separado.

            bool seEdito = await _repoContrato.Editar(contratoExistente);
            if (!seEdito)
                throw new TaskCanceledException("No se pudo editar el contrato");

            return contratoExistente;
        }

        public async Task<bool> Eliminar(int id)
        {
            var contrato = await _repoContrato.Obtener(c => c.IdContrato == id);
            if (contrato == null)
            {
                return false;
            }

            // La lógica para eliminar el archivo asociado también iría aquí.

            return await _repoContrato.Eliminar(contrato);
        }

        public async Task<List<Contrato>> Listar()
        {
            var query = await _repoContrato.Consultar();
            return await query.Include(c => c.IdCotizacionNavigation)
                              .ThenInclude(cot => cot.SecVisitaNavigation)
                              .Include(c => c.IdUsuarioCargaNavigation)
                              .ToListAsync();
        }

        public async Task<Contrato> Obtener(int id)
        {
            return await _repoContrato.Obtener(c => c.IdContrato == id);
        }

        public async Task<List<PreContrato>> ListarPreContratosParaContrato()
        {
            var query = await _repoPreContrato.Consultar(p => p.Estado == "Aprobado" && p.EstaActivo);
            return await query.Include(p => p.SecCotizacionNavigation)
                              .ThenInclude(c => c.SecVisitaNavigation)
                              .ToListAsync();
        }
    }
}