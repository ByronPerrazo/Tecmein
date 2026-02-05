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
        private readonly ITipoDocumentoServices _tipoDocumentoServices;
        private readonly IActivoClienteService _activoClienteService; // Inyectado

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
            IGenericRepository<PreContratoCompromisoPago> repositorioCompromisoPago,
            ITipoDocumentoServices tipoDocumentoServices,
            IActivoClienteService activoClienteService) // Inyectado
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
            _tipoDocumentoServices = tipoDocumentoServices;
            _activoClienteService = activoClienteService; // Asignado
        }

        public async Task<Contrato> Crear(ContratoCreacionDTO dto)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                int idCotizacionFinal = 0;
                int secClienteFinal = 0;

                // Determinar el cliente y la cotización a asociar
                if (dto.IdCotizacion.HasValue && dto.IdCotizacion > 0)
                {
                    var cotizacion = await _dbContext.Cotizacion
                        .Include(c => c.SecVisitaNavigation)
                            .ThenInclude(v => v.SecConstructoraNavigation)
                        .Include(c => c.SecVisitaNavigation)
                            .ThenInclude(v => v.Contactovisita)
                                .ThenInclude(cv => cv.SecContactoNavigation)
                        .FirstOrDefaultAsync(c => c.Secuencial == dto.IdCotizacion.Value);

                    if (cotizacion == null) throw new Exception("La cotización especificada no fue encontrada.");
                    
                    int? secConstructora = cotizacion.SecVisitaNavigation?.SecConstructora;

                    // Lógica de respaldo: Si SecConstructora es nulo, buscar a través de ContactoVisita
                    if (secConstructora == null)
                    {
                        var contactoVisita = cotizacion.SecVisitaNavigation?.Contactovisita?.FirstOrDefault();
                        if (contactoVisita?.SecContactoNavigation?.SecConstructora != null)
                        {
                            secConstructora = contactoVisita.SecContactoNavigation.SecConstructora;
                            
                            // Opcional: Actualizar el campo en Visita para futuras referencias
                            // cotizacion.SecVisitaNavigation.SecConstructora = secConstructora;
                        }
                    }

                    if (secConstructora == null) throw new Exception("La visita de la cotización debe tener una constructora asociada.");

                    var cliente = await _clienteServices.ObtenerOCrearPorConstructora(secConstructora.Value);
                    if (cliente == null) throw new Exception("No se pudo obtener o crear el cliente a partir de la constructora.");

                    // Actualizar el nombre de la obra en la visita si se proporciona
                    if (cotizacion.SecVisitaNavigation != null && !string.IsNullOrEmpty(dto.NombreProyecto))
                    {
                        cotizacion.SecVisitaNavigation.Nombre = dto.NombreProyecto;
                    }

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
                    throw new Exception("Datos insuficientes. Se requiere una IdCotizacion o un SecCliente.");
                }

                // Recuperar SecTipoDocumento desde PreContrato si no viene en el DTO
                int? secTipoDocumentoFinal = dto.SecTipoDocumento > 0 ? dto.SecTipoDocumento : (int?)null;
                
                if ((secTipoDocumentoFinal == null || secTipoDocumentoFinal == 0) && idCotizacionFinal > 0)
                {
                    var preContratoOrigen = await _repositorioPreContrato.Consultar(p => p.SecCotizacion == idCotizacionFinal && p.Estado == "Aprobado");
                    var preContratoConPlantilla = await preContratoOrigen.Include(p => p.SecPlantillaPreContratoNavigation).FirstOrDefaultAsync();
                    
                    if (preContratoConPlantilla != null && preContratoConPlantilla.SecPlantillaPreContratoNavigation != null)
                    {
                        secTipoDocumentoFinal = preContratoConPlantilla.SecPlantillaPreContratoNavigation.SecTipoDocumento;
                    }
                }

                // Crear la entidad Contrato
                Contrato contrato = new Contrato
                {
                    IdCotizacion = idCotizacionFinal,
                    SecCliente = secClienteFinal,
                    FechaFirma = dto.FechaFirma,
                    IdUsuarioCarga = dto.IdUsuarioCarga,
                    SecTipoDocumento = secTipoDocumentoFinal, // Usar valor calculado
                    NombreArchivo = dto.NombreArchivo,
                    RutaArchivo = "",
                    FechaCreacion = DateTime.Now,
                    EsActivo = true
                };

                var contratoCreado = await _repositorioContrato.Crear(contrato);
                if (contratoCreado.IdContrato == 0) throw new Exception("No se pudo crear el registro del contrato.");

                // Obtener el tipo de documento del contrato creado
                var tipoContratoCreado = await _tipoDocumentoServices.Obtener(contratoCreado.SecTipoDocumento.GetValueOrDefault());

                // --- Lógica para generación automática de Contrato de Garantía si el contrato es de VENTA ---
                if (tipoContratoCreado != null && tipoContratoCreado.Codigo == "VENTA")
                {
                    var tipoGarantia = await _tipoDocumentoServices.ObtenerPorCodigo("GARANTIA");
                    if (tipoGarantia == null) throw new Exception("Tipo de documento 'GARANTIA' no configurado.");

                    // TODO: Aquí se debería obtener la lista de ActivoCliente asociados a esta venta
                    // Por ahora, creamos un contrato de garantía general si no hay activos específicos aún.

                    var contratoGarantia = new Contrato
                    {
                        IdCotizacion = null, // Contrato de garantía no directamente asociado a una cotización, o se crea una dummy si es necesario
                        SecCliente = secClienteFinal,
                        FechaFirma = contratoCreado.FechaFirma, // O una fecha de inicio de garantía específica
                        IdUsuarioCarga = contratoCreado.IdUsuarioCarga,
                        SecTipoDocumento = tipoGarantia.SecTipoDocumento,
                        NombreArchivo = $"Garantía_{contratoCreado.IdContrato}.pdf", // Nombre genérico
                        RutaArchivo = "", // Se actualizará al subir el archivo (si aplica)
                        FechaCreacion = DateTime.Now,
                        EsActivo = true
                    };
                    await _repositorioContrato.Crear(contratoGarantia);

                    // Generar ActivoCliente aquí, vinculando al contrato de venta original (contratoCreado.IdContrato)
                    // y al contrato de garantía (contratoGarantia.IdContrato).
                    if (contratoCreado.IdCotizacion.HasValue && contratoCreado.IdCotizacion > 0)
                    {
                        var cotizacionOriginal = await _repositorioCotizacion.Consultar(c => c.Secuencial == contratoCreado.IdCotizacion.Value);
                        var detallesCotizacion = await cotizacionOriginal.Include(c => c.Cotizaciondetalles).SelectMany(c => c.Cotizaciondetalles).ToListAsync();

                        foreach (var detalle in detallesCotizacion)
                        {
                            var activoCliente = new ActivoCliente
                            {
                                SecCliente = secClienteFinal,
                                SecEquipo = null, // TODO: Si hay una entidad Equipo, vincular aquí
                                Descripcion = detalle.DetalleEquipo,
                                FechaInstalacion = contratoCreado.FechaFirma, // Fecha de instalación podría ser la de firma del contrato
                                SecContratoOrigen = contratoCreado.IdContrato
                            };
                            await _activoClienteService.Crear(activoCliente);
                        }
                    }
                }

                // --- LÓGICA PARA CREAR PLAN DE PAGO AUTOMÁTICAMENTE ---
                var preContrato = await _repositorioPreContrato.Obtener(p => p.SecCotizacion == idCotizacionFinal && p.Estado == "Aprobado");
                if (preContrato != null)
                {
                    var compromisos = await _repositorioCompromisoPago.Consultar(c => c.SecPreContrato == preContrato.SecPreContrato);
                    var listaCompromisos = await compromisos.ToListAsync();

                    if (listaCompromisos.Any())
                    {
                        var anticipo = listaCompromisos.FirstOrDefault(c => c.Tipo == "Anticipo");
                        var cuotas = listaCompromisos.Where(c => c.Tipo == "Cuota").OrderBy(c => c.NumeroCuota).ToList();

                        var nuevoPlanDePago = new PlanDePago
                        {
                            IdContrato = contratoCreado.IdContrato,
                            SecFormaPago = 1, // TODO: La forma de pago debe ser dinámica.
                            ValorContrato = listaCompromisos.Sum(c => c.Monto),
                            ValorAnticipo = anticipo?.Monto ?? 0,
                            FechaAnticipo = anticipo?.FechaVencimiento,
                            NumeroCuotas = cuotas.Count,
                            FechaPrimeraCuota = cuotas.FirstOrDefault()?.FechaVencimiento,
                            EstaActivo = true,
                            FechaRegistro = DateTime.Now
                        };

                        var planDePagoCreado = await _repositorioPlanDePago.Crear(nuevoPlanDePago);

                        foreach (var compromisoCuota in cuotas)
                        {
                            var nuevaCuota = new Cuota
                            {
                                IdPlanDePago = planDePagoCreado.IdPlanDePago,
                                NumeroCuota = compromisoCuota.NumeroCuota,
                                MontoEsperado = compromisoCuota.Monto,
                                FechaVencimiento = compromisoCuota.FechaVencimiento,
                                Estado = "Pendiente",
                                FechaRegistro = DateTime.Now
                            };
                            await _repositorioCuota.Crear(nuevaCuota);
                        }
                    }

                    // Actualizar el estado del pre-contrato para que no se vuelva a usar
                    preContrato.Estado = "Procesado";
                    preContrato.EstaActivo = false;
                    await _repositorioPreContrato.Editar(preContrato);
                }
                // --- FIN LÓGICA PLAN DE PAGO ---

                // Subir el archivo a Firebase
                if (dto.ArchivoStream != null && !string.IsNullOrEmpty(dto.NombreArchivo))
                {
                    string numeroCliente = (await _dbContext.Clientes.FindAsync(secClienteFinal)).NumeroCliente;
                    string carpetaDestino = $"Contratos/{numeroCliente}/{contratoCreado.IdContrato}";
                    contratoCreado.RutaArchivo = await _storageServices.SubirStorage(dto.ArchivoStream, carpetaDestino, dto.NombreArchivo);
                    await _repositorioContrato.Editar(contratoCreado); // Guardar la ruta del archivo
                }

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
                FechaSiguienteVisita = null,
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
                              .Include(c => c.SecClienteNavigation).ThenInclude(cli => cli.SecConstructoraNavigation)
                              .Include(c => c.SecTipoDocumentoNavigation) // <-- Añadido
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
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var contrato = await _repositorioContrato.Obtener(c => c.IdContrato == id);
                if (contrato == null)
                {
                    throw new KeyNotFoundException("Contrato no encontrado.");
                }

                // Delete file from Firebase Storage
                if (!string.IsNullOrEmpty(contrato.NombreArchivo))
                {
                    var cliente = await _dbContext.Clientes.FindAsync(contrato.SecCliente);
                    if (cliente != null)
                    {
                        string carpetaDestino = $"Contratos/{cliente.NumeroCliente}/{contrato.IdContrato}";
                        await _storageServices.EliminarStorage(carpetaDestino, contrato.NombreArchivo);
                    }
                }

                // Revert PreContrato status if it exists
                if (contrato.IdCotizacion != 0)
                {
                    var preContrato = await _repositorioPreContrato.Obtener(p => p.SecCotizacion == contrato.IdCotizacion && p.Estado == "Procesado");
                    if (preContrato != null)
                    {
                        preContrato.Estado = "Aprobado";
                        preContrato.EstaActivo = true;
                        await _repositorioPreContrato.Editar(preContrato);
                    }
                }

                // Soft delete the Contrato
                contrato.EsActivo = false;
                bool resultado = await _repositorioContrato.Editar(contrato);

                if (resultado)
                {
                    await transaction.CommitAsync();
                    return true;
                }
                else
                {
                    await transaction.RollbackAsync();
                    throw new Exception("No se pudo actualizar el estado del contrato a inactivo.");
                }
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<PreContrato>> ListarPreContratosParaContrato()
        {
            var query = await _repositorioPreContrato.Consultar(p => p.Estado == "Aprobado" && p.EstaActivo);
            return await query.Include(p => p.SecCotizacionNavigation)
                              .ThenInclude(c => c.SecVisitaNavigation)
                                .ThenInclude(v => v.Contactovisita)
                                    .ThenInclude(cv => cv.SecContactoNavigation)
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