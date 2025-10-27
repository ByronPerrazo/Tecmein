using BLL.DTOs;
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
        private readonly IGenericRepository<Contrato> _repositorioContrato;
        private readonly IGenericRepository<Cotizacion> _repositorioCotizacion;
        private readonly IGenericRepository<Visita> _repositorioVisita;
        private readonly IClienteServices _clienteServices;
        private readonly IStorageServices _storageServices;
        private readonly IGenericRepository<PreContrato> _repositorioPreContrato;
        private readonly IGenericRepository<Etapa> _repositorioEtapa;
        private readonly TecmeindbContext _dbContext;
        private readonly IGenericRepository<PlanDePago> _repositorioPlanDePago;
        private readonly IGenericRepository<Cuota> _repositorioCuota;
        private readonly IGenericRepository<PreContratoCompromisoPago> _repositorioCompromisoPago;

        public ContratoService(
            IGenericRepository<Contrato> repositorioContrato,
            IGenericRepository<Cotizacion> repositorioCotizacion,
            IGenericRepository<Visita> repositorioVisita,
            IClienteServices clienteServices,
            IStorageServices storageServices,
            IGenericRepository<PreContrato> repositorioPreContrato,
            IGenericRepository<Etapa> repositorioEtapa,
            TecmeindbContext dbContext,
            IGenericRepository<PlanDePago> repositorioPlanDePago,
            IGenericRepository<Cuota> repositorioCuota,
            IGenericRepository<PreContratoCompromisoPago> repositorioCompromisoPago)
        {
            _repositorioContrato = repositorioContrato;
            _repositorioCotizacion = repositorioCotizacion;
            _repositorioVisita = repositorioVisita;
            _clienteServices = clienteServices;
            _storageServices = storageServices;
            _repositorioPreContrato = repositorioPreContrato;
            _repositorioEtapa = repositorioEtapa;
            _dbContext = dbContext;
            _repositorioPlanDePago = repositorioPlanDePago;
            _repositorioCuota = repositorioCuota;
            _repositorioCompromisoPago = repositorioCompromisoPago;
        }

        public async Task<Contrato> Crear(ContratoCreacionDTO dto)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                int idCotizacionFinal;
                int secClienteFinal;

                if (dto.IdCotizacion.HasValue && dto.IdCotizacion > 0)
                {
                    var cotizacion = await _dbContext.Cotizacion.Include(c => c.SecVisitaNavigation).FirstOrDefaultAsync(c => c.Secuencial == dto.IdCotizacion.Value);
                    if (cotizacion == null) throw new Exception("La cotización especificada no fue encontrada.");
                    if (cotizacion.SecVisitaNavigation?.SecConstructora == null) throw new Exception("La visita de la cotización debe tener una constructora.");

                    var cliente = await _clienteServices.ObtenerOCrearPorConstructora(cotizacion.SecVisitaNavigation.SecConstructora.Value);
                    if (cliente == null) throw new Exception("No se pudo obtener o crear el cliente.");

                    idCotizacionFinal = cotizacion.Secuencial;
                    secClienteFinal = cliente.SecCliente;


                }
                else if (dto.SecCliente.HasValue && dto.SecCliente > 0)
                {
                    var cliente = await _dbContext.Clientes.FindAsync(dto.SecCliente.Value);
                    if (cliente == null) throw new Exception("Cliente no encontrado.");

                    var etapa = await _repositorioEtapa.Obtener(e => e.Codigo == "HIST") ?? await CrearEtapaHistorico();
                    var visitaDummy = await CrearVisitaDummy(dto.IdUsuarioCarga, cliente, etapa.Id, dto.NombreProyecto);
                    var cotizacionDummy = await CrearCotizacionDummy(visitaDummy.Secuencial, dto.IdUsuarioCarga);

                    idCotizacionFinal = cotizacionDummy.Secuencial;
                    secClienteFinal = cliente.SecCliente;
                }
                else
                {
                    throw new Exception("Datos insuficientes. Se requiere un IdCotizacion o un SecCliente.");
                }

                var contrato = new Contrato
                {
                    IdCotizacion = idCotizacionFinal,
                    SecCliente = secClienteFinal,
                    FechaFirma = dto.FechaFirma,
                    IdUsuarioCarga = dto.IdUsuarioCarga,
                    NombreArchivo = dto.NombreArchivo,
                    RutaArchivo = "",
                    FechaCreacion = DateTime.Now,
                    EsActivo = true
                };

                var contratoCreado = await _repositorioContrato.Crear(contrato);
                if (contratoCreado.IdContrato == 0) throw new Exception("No se pudo crear el registro del contrato.");

                // <<< START: NEW LOGIC >>>
                var preContrato = await _repositorioPreContrato.Obtener(p => p.SecCotizacion == idCotizacionFinal && p.Estado == "Aprobado");
                if (preContrato != null)
                {
                    var compromisos = await _repositorioCompromisoPago.Consultar(c => c.SecPreContrato == preContrato.SecPreContrato);
                    if (compromisos.Any())
                    {
                        var anticipo = compromisos.FirstOrDefault(c => c.Tipo == "Anticipo");
                        var cuotas = compromisos.Where(c => c.Tipo == "Cuota").OrderBy(c => c.NumeroCuota).ToList();

                        var nuevoPlanDePago = new PlanDePago
                        {
                            IdContrato = contratoCreado.IdContrato,
                            SecFormaPago = 1, // TODO: Hacer que la forma de pago sea dinámica o venga del pre-contrato.
                            ValorContrato = compromisos.Sum(c => c.Monto),
                            ValorAnticipo = anticipo?.Monto ?? 0,
                            FechaAnticipo = anticipo?.FechaVencimiento,
                            NumeroCuotas = cuotas.Count(),
                            FechaPrimeraCuota = cuotas.FirstOrDefault()?.FechaVencimiento,
                            EstaActivo = true,
                            FechaRegistro = DateTime.Now
                        };

                        var planDePagoCreado = await _repositorioPlanDePago.Crear(nuevoPlanDePago);

                        foreach (var c in cuotas)
                        {
                            var nuevaCuota = new Cuota
                            {
                                IdPlanDePago = planDePagoCreado.IdPlanDePago,
                                NumeroCuota = c.NumeroCuota,
                                MontoEsperado = c.Monto,
                                FechaVencimiento = c.FechaVencimiento,
                                Estado = "Pendiente",
                                FechaRegistro = DateTime.Now
                            };
                            await _repositorioCuota.Crear(nuevaCuota);
                        }
                    }

                    preContrato.Estado = "Procesado";
                    preContrato.EstaActivo = false;
                }
                // <<< END: NEW LOGIC >>>

                if (dto.ArchivoStream != null && !string.IsNullOrEmpty(dto.NombreArchivo))
                {
                    string numeroCliente = (await _dbContext.Clientes.FindAsync(secClienteFinal)).NumeroCliente;
                    string carpetaDestino = $"Contratos/{numeroCliente}/{contratoCreado.IdContrato}";
                    contratoCreado.RutaArchivo = await _storageServices.SubirStorage(dto.ArchivoStream, carpetaDestino, dto.NombreArchivo);
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return contratoCreado;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error al crear el contrato: {ex.Message}", ex);
            }
        }
        
        public async Task<Contrato> Editar(Contrato entidad, string nombreProyecto, Stream archivoStream, string nombreArchivo)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var contratoExistente = await _repositorioContrato.Obtener(c => c.IdContrato == entidad.IdContrato);
                if (contratoExistente == null) throw new Exception("El contrato no fue encontrado.");

                var cotizacion = await _dbContext.Cotizacion.Include(c => c.SecVisitaNavigation).FirstOrDefaultAsync(c => c.Secuencial == contratoExistente.IdCotizacion);
                if (cotizacion?.SecVisitaNavigation != null && !string.IsNullOrEmpty(nombreProyecto))
                {
                    cotizacion.SecVisitaNavigation.Nombre = nombreProyecto;
                }

                contratoExistente.FechaFirma = entidad.FechaFirma;
                contratoExistente.EsActivo = entidad.EsActivo;

                if (archivoStream != null && !string.IsNullOrEmpty(nombreArchivo))
                {
                    var cliente = await _dbContext.Clientes.FindAsync(contratoExistente.SecCliente);
                    string carpetaDestino = $"Contratos/{cliente.NumeroCliente}/{contratoExistente.IdContrato}";
                    if (!string.IsNullOrEmpty(contratoExistente.RutaArchivo))
                    {
                        await _storageServices.EliminarStorage(contratoExistente.RutaArchivo, contratoExistente.NombreArchivo);
                    }
                    contratoExistente.RutaArchivo = await _storageServices.SubirStorage(archivoStream, carpetaDestino, nombreArchivo);
                    contratoExistente.NombreArchivo = nombreArchivo;
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return contratoExistente;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error al editar el contrato: {ex.Message}", ex);
            }
        }

        private async Task<Etapa> CrearEtapaHistorico()
        {
            var etapa = new Etapa { Codigo = "HIST", Descripcion = "Histórico", Orden = 99, EstaActivo = false };
            return await _repositorioEtapa.Crear(etapa);
        }

        private async Task<Visita> CrearVisitaDummy(int idUsuario, Cliente cliente, int idEtapa, string nombreProyecto)
        {
            var visita = new Visita
            {
                SecUsuario = idUsuario,
                IdEtapa = idEtapa,
                Nombre = nombreProyecto,
                Detalle = "Registro automático para contrato directo",
                EstaActivo = 0,
                FechaRegistro = DateTime.Now,
                Direccion = "N/A",
                GeoUbicacion = "N/A",
                FechaSiguienteVisita = DateTime.Now,
                SecConstructora = cliente.SecConstructora
            };
            return await _repositorioVisita.Crear(visita);
        }

        private async Task<Cotizacion> CrearCotizacionDummy(int idVisita, int idUsuario)
        {
            var cotizacion = new Cotizacion
            {
                SecVisita = idVisita,
                Subtotal = 0,
                EstaActivo = 0,
                FechaRegistro = DateTime.Now,
                SecUsuario = idUsuario
            };
            return await _repositorioCotizacion.Crear(cotizacion);
        }

        public async Task<List<Contrato>> Listar()
        {
            var query = await _repositorioContrato.Consultar(c => c.EsActivo == true);
            return await query.Include(c => c.IdCotizacionNavigation).ThenInclude(cot => cot.SecVisitaNavigation)
                              .Include(c => c.IdUsuarioCargaNavigation)
                              .Include(c => c.SecClienteNavigation)
                              .ToListAsync();
        }

        public async Task<Contrato> Obtener(int id)
        {
            return await _repositorioContrato.Obtener(c => c.IdContrato == id);
        }

        public async Task<Contrato> ObtenerParaEdicion(int idContrato)
        {
            var query = await _repositorioContrato.Consultar(c => c.IdContrato == idContrato);
            return await query
                .Include(c => c.SecClienteNavigation)
                .Include(c => c.IdCotizacionNavigation).ThenInclude(cot => cot.SecVisitaNavigation)
                .Include(c => c.PlanDePagoNavigation).ThenInclude(pdp => pdp.Cuotas)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> Eliminar(int id)
        {
            var contrato = await _repositorioContrato.Obtener(c => c.IdContrato == id);
            if (contrato == null) 
            {
                throw new TaskCanceledException("Contrato no encontrado");
            }

            contrato.EsActivo = false;
            bool response = await _repositorioContrato.Editar(contrato);
            return response;
        }

        public async Task<List<PreContrato>> ListarPreContratosParaContrato()
        {
            var query = await _repositorioPreContrato.Consultar(p => p.Estado == "Aprobado" && p.EstaActivo);
            return await query.Include(p => p.SecCotizacionNavigation)
                              .ThenInclude(c => c.SecVisitaNavigation)
                              .ToListAsync();
        }

        public async Task<List<Contrato>> ObtenerContratosPorCliente(int secCliente)
        {
            try
            {
                var query = await _repositorioContrato.Consultar(c => c.SecCliente == secCliente);
                return await query.Include(c => c.IdCotizacionNavigation)
                                  .ThenInclude(cot => cot.SecVisitaNavigation)
                                  .ThenInclude(v => v.IdEtapaNavigation)
                                  .Include(c => c.SecClienteNavigation)
                                  .Include(c => c.PlanDePagoNavigation)
                                  .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener contratos por cliente: {ex.Message}", ex);
            }
        }
    }
}