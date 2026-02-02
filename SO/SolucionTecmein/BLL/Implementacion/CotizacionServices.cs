using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using System;
using System.Text;
using BLL.Utilidades.PDF;


namespace BLL.Implementacion
{
    public class CotizacionServices : ICotizacionServices
    {
        private readonly IGenericRepository<Cotizacion> _repositorio;
        private readonly IGenericRepository<Cotizaciondetalle> _repositorioDetalle;
        private readonly IGenericRepository<ImpuestoCotizacion> _repositorioImpuestoCotizacion;
        private readonly IImpuestoServices _impuestoServices;
        private readonly ITipoImpuestoServices _tipoImpuestoServices;
        private readonly IAuditService _auditService;
        private readonly IUsuarioServices _usuarioServices;

        public CotizacionServices(
            IGenericRepository<Cotizacion> repositorio,
            IGenericRepository<Cotizaciondetalle> repositorioDetalle,
            IGenericRepository<ImpuestoCotizacion> repositorioImpuestoCotizacion,
            IImpuestoServices impuestoServices,
            IVisitaServices visitaServices,
            ITipoImpuestoServices tipoImpuestoServices,
            IEquiposVisitaServices equiposVisitaServices,
            IAuditService auditService, // Added
            IUsuarioServices usuarioServices // Added
            )
        {
            _repositorio = repositorio;
            _repositorioDetalle = repositorioDetalle;
            _repositorioImpuestoCotizacion = repositorioImpuestoCotizacion;
            _impuestoServices = impuestoServices;
            _visitaServices = visitaServices;
            _equiposVisitaServices = equiposVisitaServices;
            _auditService = auditService;
            _usuarioServices = usuarioServices;
        }

        private readonly IVisitaServices _visitaServices;
        private readonly IEquiposVisitaServices _equiposVisitaServices;

        public async Task<List<Cotizacion>> Lista()
        {
            var query = await _repositorio.Consultar(c => c.EstaActivo == 1); // Filter by EstaActivo
            return await query
                .Include(c => c.SecVisitaNavigation)
                    .ThenInclude(v => v.Contactovisita)
                        .ThenInclude(cv => cv.SecContactoNavigation)
                .Include(c => c.SecUsuarioNavigation)
                .Include(c => c.SecUsuarioModificaNavigation)
                .AsNoTracking()
                .ToListAsync();
        }



        public async Task<Cotizacion> Detalle(int secuencial)
        {
            IQueryable<Cotizacion> query = await _repositorio.Consultar(c => c.Secuencial == secuencial);
            var cotizacion = await query
                .Include(c => c.SecVisitaNavigation)
                    .ThenInclude(v => v.SecEmpresaNavigation)
                .Include(c => c.SecVisitaNavigation)
                    .ThenInclude(v => v.Contactovisita)
                        .ThenInclude(cv => cv.SecContactoNavigation)
                            .ThenInclude(co => co.SecConstructoraNavigation)
                .Include(c => c.SecVisitaNavigation)
                    .ThenInclude(v => v.SecProvinciaNavigation)
                .Include(c => c.SecVisitaNavigation)
                    .ThenInclude(v => v.SecCantonNavigation)
                .Include(c => c.SecUsuarioNavigation)
                .Include(c => c.SecUsuarioModificaNavigation)
                .Include(c => c.Cotizaciondetalles)
                .Include(c => c.ImpuestoCotizaciones)
                    .ThenInclude(ic => ic.ImpuestoNavigation)
                        .ThenInclude(i => i.SecTipoImpuestoNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return cotizacion;
        }

        public async Task<Cotizacion> Crear(Cotizacion entidad, int secUsuario)
        {
            try
            {
                // Validar si ya existe una cotización activa para la visita
                var cotizacionExistente = await _repositorio.Obtener(c => c.SecVisita == entidad.SecVisita && c.EstaActivo == 1);
                if (cotizacionExistente != null)
                {
                    throw new TaskCanceledException("La visita seleccionada ya tiene una cotización activa.");
                }

                entidad.SecUsuario = secUsuario;
                entidad.FechaRegistro = DateTime.Now;
                decimal subtotalCalculado = 0;
                foreach (var detalle in entidad.Cotizaciondetalles)
                {
                    detalle.Total = detalle.ValorCompra * detalle.Cantidad * (1 + detalle.MargenGanancia / 100);
                    detalle.FechaRegistro = DateTime.Now;
                    subtotalCalculado += detalle.Total;
                }
                entidad.Subtotal = subtotalCalculado;

                // Calculate specific taxes (IVA 15% and Importacion)
                decimal valorIVACalculado = 0;
                decimal valorImportacionCalculado = 0;

                entidad.ImpuestoCotizaciones.Clear();

                var impuestosActivos = await _impuestoServices.Lista();

                var ivaImpuesto = impuestosActivos.FirstOrDefault(i => i.Vigente && i.SecTipoImpuestoNavigation.EsIva);
                if (ivaImpuesto != null && ivaImpuesto.Porcentaje.HasValue)
                {
                    valorIVACalculado = entidad.Subtotal * (ivaImpuesto.Porcentaje.Value / 100m);
                    entidad.ImpuestoCotizaciones.Add(new ImpuestoCotizacion
                    {
                        ImpuestoId = ivaImpuesto.Id,
                        BaseImponible = entidad.Subtotal,
                        ValorImpuesto = valorIVACalculado,
                        Exento = false,
                        FechaRegistro = DateTime.Now
                    });
                }

                var importacionImpuesto = impuestosActivos.FirstOrDefault(i => i.Vigente && i.SecTipoImpuestoNavigation.EsImportacion);
                if (importacionImpuesto != null)
                {
                    if (importacionImpuesto.Porcentaje.HasValue)
                    {
                        valorImportacionCalculado = entidad.Subtotal * (importacionImpuesto.Porcentaje.Value / 100m);
                    }
                    else if (importacionImpuesto.ValorFijo.HasValue)
                    {
                        valorImportacionCalculado = importacionImpuesto.ValorFijo.Value;
                    }

                    if (valorImportacionCalculado > 0)
                    {
                        entidad.ImpuestoCotizaciones.Add(new ImpuestoCotizacion
                        {
                            ImpuestoId = importacionImpuesto.Id,
                            BaseImponible = entidad.Subtotal,
                            ValorImpuesto = valorImportacionCalculado,
                            Exento = false,
                            FechaRegistro = DateTime.Now
                        });
                    }
                }

                entidad.ValorIVA = valorIVACalculado;
                entidad.ValorImportacion = valorImportacionCalculado;
                entidad.ValorImpuestos = valorIVACalculado + valorImportacionCalculado;
                entidad.TotalConImpuestos = entidad.Subtotal + entidad.ValorImpuestos;

                Cotizacion cotizacionCreada = await _repositorio.Crear(entidad);
                if (cotizacionCreada.Secuencial == 0)
                    throw new TaskCanceledException("No se pudo crear la cotización.");

                // Cambiar etapa de la visita a "COT" si la cotización tiene detalles
                if (cotizacionCreada.Cotizaciondetalles.Any())
                {
                    await _visitaServices.CambiarEtapa(cotizacionCreada.SecVisita, "COT");
                }

                return cotizacionCreada;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Cotizacion> Editar(Cotizacion entidad, int secUsuarioActual)
        {
            try
            {
                var visita = await _visitaServices.ConsultaVisita(entidad.SecVisita);
                if (visita.IdEtapaNavigation.Codigo == "PRE" || visita.IdEtapaNavigation.Codigo == "CON")
                {
                    throw new InvalidOperationException("No se puede editar una cotización de una visita que ya ha avanzado a la etapa de pre-contrato o contrato.");
                }

                // Obtener la cotización actual de la base de datos
                var cotizacionActual = await _repositorio.Obtener(c => c.Secuencial == entidad.Secuencial, "Cotizaciondetalles,ImpuestoCotizaciones");

                if (cotizacionActual == null)
                    throw new KeyNotFoundException($"No se encontró la cotización con el secuencial {entidad.Secuencial}");

                // --- AUDIT LOGIC START ---
                var usuario = await _usuarioServices.ObtenerPorId(secUsuarioActual);
                var nombreUsuario = usuario?.Nombre ?? "Sistema";

                var oldDetailsList = cotizacionActual.Cotizaciondetalles.ToList();
                var newDetailsList = entidad.Cotizaciondetalles.ToList();

                // Find updated and removed items
                foreach (var oldItem in oldDetailsList)
                {
                    var newItem = newDetailsList.FirstOrDefault(d => d.SecEquipoVisita != null && d.SecEquipoVisita == oldItem.SecEquipoVisita);

                    if (newItem != null) // Item found, check for updates
                    {
                        var sbChanges = new StringBuilder();
                        if (oldItem.Cantidad != newItem.Cantidad) sbChanges.Append($"Cantidad: '{oldItem.Cantidad}' -> '{newItem.Cantidad}'. ");
                        if (oldItem.ValorCompra != newItem.ValorCompra) sbChanges.Append($"Valor Compra: '{oldItem.ValorCompra:C}' -> '{newItem.ValorCompra:C}'. ");
                        if (oldItem.MargenGanancia != newItem.MargenGanancia) sbChanges.Append($"Margen: '{oldItem.MargenGanancia}%' -> '{newItem.MargenGanancia}%'. ");

                        if (sbChanges.Length > 0)
                        {
                            await _auditService.RegistrarEventoAsync(
                                $"COTIZACION_{cotizacionActual.Secuencial}_DETALLE_UPDATE",
                                secUsuarioActual, nombreUsuario,
                                $"Item '{oldItem.DetalleEquipo}': {sbChanges.ToString()}", null);
                        }
                    }
                    else // Item not found in new list, so it was removed
                    {
                        await _auditService.RegistrarEventoAsync(
                            $"COTIZACION_{cotizacionActual.Secuencial}_DETALLE_DELETE",
                            secUsuarioActual, nombreUsuario,
                            $"Item eliminado: '{oldItem.DetalleEquipo}'.", null);
                    }
                }

                // Find added items
                foreach (var newItem in newDetailsList)
                {
                    if (!oldDetailsList.Any(d => d.SecEquipoVisita != null && d.SecEquipoVisita == newItem.SecEquipoVisita))
                    {
                        await _auditService.RegistrarEventoAsync(
                            $"COTIZACION_{cotizacionActual.Secuencial}_DETALLE_CREATE",
                            secUsuarioActual, nombreUsuario,
                            $"Item añadido: '{newItem.DetalleEquipo}' (Cant: {newItem.Cantidad}, Valor: {newItem.ValorCompra:C}, Margen: {newItem.MargenGanancia}%).", null);
                    }
                }
                // --- AUDIT LOGIC END ---

                // Determinar si se debe crear una nueva versión o actualizar el registro existente
                bool crearNuevaVersion = cotizacionActual.EnviadoCliente; // Si ya fue enviada al cliente, se crea una nueva versión

                Cotizacion cotizacionAfectada;

                if (crearNuevaVersion)
                {
                    // 1. Inactivar la cotización original (actual)
                    cotizacionActual.EstaActivo = 0;
                    cotizacionActual.FechaModificacion = DateTime.Now;
                    bool seInactivo = await _repositorio.Editar(cotizacionActual);
                    if (!seInactivo)
                        throw new Exception("No se pudo inactivar la cotización original.");

                    // 2. Crear una nueva versión de la cotización
                    cotizacionAfectada = new Cotizacion
                    {
                        Secuencial = 0, // Para que EF la inserte como nueva
                        SecVisita = entidad.SecVisita,
                        EnviadoProveedor = entidad.EnviadoProveedor,
                        EnviadoCliente = entidad.EnviadoCliente,
                        Confirmacion = entidad.Confirmacion,
                        EstaActivo = 1, // La nueva versión siempre está activa
                        FechaRegistro = DateTime.Now,
                        FechaModificacion = DateTime.Now,
                        SecUsuario = cotizacionActual.SecUsuario, // Creador original
                        SecUsuarioModifica = secUsuarioActual, // Último editor
                        SecCotizacionOriginal = cotizacionActual.SecCotizacionOriginal ?? cotizacionActual.Secuencial // Enlazar a la versión original
                    };
                }
                else
                {
                    // Actualizar el registro existente (modo borrador)
                    cotizacionAfectada = cotizacionActual;
                    cotizacionAfectada.SecVisita = entidad.SecVisita;
                    cotizacionAfectada.EnviadoProveedor = entidad.EnviadoProveedor;
                    cotizacionAfectada.EnviadoCliente = entidad.EnviadoCliente;
                    cotizacionAfectada.Confirmacion = entidad.Confirmacion;
                    cotizacionAfectada.FechaModificacion = DateTime.Now;
                    cotizacionAfectada.SecUsuarioModifica = secUsuarioActual;
                    // Limpiar detalles e impuestos existentes para reemplazarlos
                    cotizacionAfectada.Cotizaciondetalles.Clear();
                    cotizacionAfectada.ImpuestoCotizaciones.Clear();
                }

                // Recalcular detalles y totales
                decimal subtotalCalculado = 0;
                foreach (var detalleFromFrontend in entidad.Cotizaciondetalles)
                {
                    var newCotizacionDetalle = new Cotizaciondetalle
                    {
                        Secuencial = 0, // For EF to insert as new (or update if existing and not versioning)
                        SecCotizacion = 0, // Will be assigned by EF
                        SecEquipoVisita = detalleFromFrontend.SecEquipoVisita, // Preserve link
                        DetalleEquipo = detalleFromFrontend.DetalleEquipo,
                        ValorCompra = detalleFromFrontend.ValorCompra,
                        MargenGanancia = detalleFromFrontend.MargenGanancia,
                        Cantidad = detalleFromFrontend.Cantidad,
                        EstaActivo = detalleFromFrontend.EstaActivo,
                        FechaRegistro = DateTime.Now,
                        Total = detalleFromFrontend.ValorCompra * detalleFromFrontend.Cantidad * (1 + detalleFromFrontend.MargenGanancia / 100)
                    };
                    cotizacionAfectada.Cotizaciondetalles.Add(newCotizacionDetalle);
                    subtotalCalculado += newCotizacionDetalle.Total;
                }
                cotizacionAfectada.Subtotal = subtotalCalculado;

                // Recalcular impuestos
                decimal valorIVACalculado = 0;
                decimal valorImportacionCalculado = 0;
                var impuestosActivos = await _impuestoServices.Lista();

                var ivaImpuesto = impuestosActivos.FirstOrDefault(i => i.Vigente && i.SecTipoImpuestoNavigation.EsIva);
                if (ivaImpuesto != null && ivaImpuesto.Porcentaje.HasValue)
                {
                    valorIVACalculado = cotizacionAfectada.Subtotal * (ivaImpuesto.Porcentaje.Value / 100m);
                    cotizacionAfectada.ImpuestoCotizaciones.Add(new ImpuestoCotizacion
                    {
                        ImpuestoId = ivaImpuesto.Id,
                        BaseImponible = cotizacionAfectada.Subtotal,
                        ValorImpuesto = valorIVACalculado,
                        Exento = false,
                        FechaRegistro = DateTime.Now
                    });
                }

                var importacionImpuesto = impuestosActivos.FirstOrDefault(i => i.Vigente && i.SecTipoImpuestoNavigation.EsImportacion);
                if (importacionImpuesto != null)
                {
                    if (importacionImpuesto.Porcentaje.HasValue)
                    {
                        valorImportacionCalculado = cotizacionAfectada.Subtotal * (importacionImpuesto.Porcentaje.Value / 100m);
                    }
                    else if (importacionImpuesto.ValorFijo.HasValue)
                    {
                        valorImportacionCalculado = importacionImpuesto.ValorFijo.Value; // Corrected typo
                    }

                    if (valorImportacionCalculado > 0)
                    {
                        cotizacionAfectada.ImpuestoCotizaciones.Add(new ImpuestoCotizacion
                        {
                            ImpuestoId = importacionImpuesto.Id,
                            BaseImponible = cotizacionAfectada.Subtotal,
                            ValorImpuesto = valorImportacionCalculado,
                            Exento = false,
                            FechaRegistro = DateTime.Now
                        });
                    }
                }

                cotizacionAfectada.ValorIVA = valorIVACalculado;
                cotizacionAfectada.ValorImportacion = valorImportacionCalculado;
                cotizacionAfectada.ValorImpuestos = valorIVACalculado + valorImportacionCalculado;
                cotizacionAfectada.TotalConImpuestos = cotizacionAfectada.Subtotal + cotizacionAfectada.ValorImpuestos;

                Cotizacion cotizacionResult;
                if (crearNuevaVersion)
                {
                    cotizacionResult = await _repositorio.Crear(cotizacionAfectada);
                    if (cotizacionResult.Secuencial == 0)
                        throw new Exception("No se pudo crear la nueva versión de la cotización.");
                }
                else
                {
                    bool seEdito = await _repositorio.Editar(cotizacionAfectada);
                    if (!seEdito)
                        throw new Exception("No se pudo actualizar la cotización.");
                    cotizacionResult = cotizacionAfectada; // Devolver la misma entidad actualizada
                }

                // Cambiar etapa de la visita a "COT" si la cotización tiene detalles
                if (cotizacionAfectada.Cotizaciondetalles.Any())
                {
                    await _visitaServices.CambiarEtapa(cotizacionAfectada.SecVisita, "COT");
                }

                return cotizacionResult;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Eliminar(int secuencial)
        {
            try
            {
                var cotizacion = await _repositorio.Obtener(c => c.Secuencial == secuencial);
                if (cotizacion == null)
                {
                    return false;
                }

                var visita = await _visitaServices.ConsultaVisita(cotizacion.SecVisita);
                if (visita.IdEtapaNavigation.Codigo == "PRE" || visita.IdEtapaNavigation.Codigo == "SEG")
                {
                    throw new InvalidOperationException("No se puede eliminar una cotización de una visita que ya está en etapa de pre-contrato o seguimiento.");
                }

                cotizacion.EstaActivo = 0;
                bool resultado = await _repositorio.Editar(cotizacion);
                return resultado;
            }
            catch
            {
                throw;
            }
        }

        public Task<bool> EnviarCorreoCliente(int idCotizacion)
        {
            // Lógica para enviar correo al cliente.
            // Se implementará en futuras fases.
            Console.WriteLine($"Simulando envío de correo al cliente para la cotización {idCotizacion}");
            return Task.FromResult(true);
        }

        public Task<bool> EnviarCorreoProveedor(int idCotizacion)
        {
            // Lógica para enviar correo al proveedor.
            // Se implementará en futuras fases.
            Console.WriteLine($"Simulando envío de correo al proveedor para la cotización {idCotizacion}");
            return Task.FromResult(true);
        }

        public async Task<bool> VisitaTieneCotizacionActiva(int visitaId)
        {
            var cotizacionExistente = await _repositorio.Obtener(c => c.SecVisita == visitaId && c.EstaActivo == 1);
            return cotizacionExistente != null;
        }

        public async Task<byte[]> GenerarPdfCotizacion(int idCotizacion)
        {
            try
            {
                var query = await _repositorio.Consultar(c => c.Secuencial == idCotizacion);

                var cotizacion = await query
                    .Include(c => c.SecVisitaNavigation)
                        .ThenInclude(v => v.SecEmpresaNavigation)
                    .Include(c => c.Cotizaciondetalles)
                    .Include(c => c.ImpuestoCotizaciones)
                        .ThenInclude(ic => ic.ImpuestoNavigation)
                            .ThenInclude(i => i.SecTipoImpuestoNavigation) // Added for TipoImpuesto
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (cotizacion == null)
                {
                    throw new KeyNotFoundException($"Cotización con ID {idCotizacion} no encontrada.");
                }

                // Validar que todos los detalles de la cotización tengan un ValorCompra mayor a 0
                if (cotizacion.Cotizaciondetalles.Any(d => d.ValorCompra == 0))
                {
                    throw new InvalidOperationException("No se puede generar el PDF para el cliente porque uno o más equipos no tienen un valor de compra asignado.");
                }

                // Validar que la cotización haya sido enviada al proveedor
                if (!cotizacion.EnviadoProveedor)
                {
                    throw new InvalidOperationException("No se puede generar el PDF para el cliente porque la cotización aún no ha sido enviada al proveedor.");
                }

                var document = new CotizacionDocument(cotizacion);
                byte[] pdfBytes = document.GeneratePdf();
                return pdfBytes;
            }
            catch
            {
                throw;
            }
        }

        public async Task<byte[]> GenerarPdfSolicitudEquipos(int idCotizacion)
        {
            try
            {
                var query = await _repositorio.Consultar(c => c.Secuencial == idCotizacion);

                var cotizacion = await query
                    .Include(c => c.SecVisitaNavigation)
                    .Include(c => c.Cotizaciondetalles)
                    .Include(c => c.ImpuestoCotizaciones)
                        .ThenInclude(ic => ic.ImpuestoNavigation)
                            .ThenInclude(i => i.SecTipoImpuestoNavigation) // Added for TipoImpuesto
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (cotizacion == null)
                {
                    throw new KeyNotFoundException($"Cotización con ID {idCotizacion} no encontrada.");
                }

                var document = new SolicitudEquiposDocument(cotizacion);
                byte[] pdfBytes = document.GeneratePdf();
                return pdfBytes;
            }
            catch
            {
                throw;
            }
        }
    }
}
