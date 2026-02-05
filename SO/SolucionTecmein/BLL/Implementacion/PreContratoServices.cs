using BLL.DTOs;
using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;

namespace BLL.Implementacion
{
    public class PreContratoServices : IPreContratoServices
    {
        private readonly IGenericRepository<PreContrato> _repositorio;
        private readonly ICotizacionServices _cotizacionServices;
        private readonly IGenericRepository<PreContratoParrafo> _repositorioPreContratoParrafo;
        private readonly IPreContratoGeneratorService _preContratoGeneratorService;
        private readonly IVisitaServices _visitaServices; // Added
        private readonly IGenericRepository<TipoDocumento> _repositorioTipoDocumento; // Added
        private readonly IGenericRepository<PlantillaPreContrato> _repositorioPlantillaPreContrato; // Added
        private readonly IGenericRepository<PreContratoCompromisoPago> _repositorioCompromisoPago;

        public PreContratoServices(
            IGenericRepository<PreContrato> repositorio,
            ICotizacionServices cotizacionServices,
            IGenericRepository<PreContratoParrafo> repositorioPreContratoParrafo,
            IPreContratoGeneratorService preContratoGeneratorService,
            IVisitaServices visitaServices,
            IGenericRepository<TipoDocumento> repositorioTipoDocumento, // Added
            IGenericRepository<PlantillaPreContrato> repositorioPlantillaPreContrato, // Added
            IGenericRepository<PreContratoCompromisoPago> repositorioCompromisoPago)
        {
            _repositorio = repositorio;
            _cotizacionServices = cotizacionServices;
            _repositorioPreContratoParrafo = repositorioPreContratoParrafo;
            _preContratoGeneratorService = preContratoGeneratorService;
            _visitaServices = visitaServices; // Added
            _repositorioTipoDocumento = repositorioTipoDocumento; // Added
            _repositorioPlantillaPreContrato = repositorioPlantillaPreContrato; // Added
            _repositorioCompromisoPago = repositorioCompromisoPago;
        }

        public async Task<List<PreContrato>> Lista()
        {
            IQueryable<PreContrato> query = await _repositorio.Consultar(p => p.EstaActivo == true);
            return await query.Include(p => p.SecCotizacionNavigation)
                                .ThenInclude(c => c.SecVisitaNavigation)
                                    .ThenInclude(v => v.Contactovisita)
                                        .ThenInclude(cv => cv.SecContactoNavigation)
                              .Include(p => p.SecUsuarioCreaNavigation)
                              .ToListAsync();
        }

        public async Task<PreContrato> Obtener(int secPreContrato)
        {
            return await _repositorio.Obtener(p => p.SecPreContrato == secPreContrato);
        }



        public async Task<PreContrato> Crear(PreContrato entidad)
        {
            if (entidad == null) throw new ArgumentNullException(nameof(entidad));

            ValidatePreContratoFields(entidad);

            var cotizacion = await _cotizacionServices.Detalle(entidad.SecCotizacion);
            if (cotizacion == null)
            {
                throw new Exception("La cotización especificada no existe.");
            }

            entidad.FechaRegistro = DateTime.Now;
            entidad.EstaActivo = true;
            entidad.Version = 1;
            entidad.Estado = "Borrador";

            var preContratoCreado = await _repositorio.Crear(entidad);
            if (preContratoCreado.SecPreContrato == 0)
            {
                throw new Exception("No se pudo crear el pre-contrato.");
            }

            // Cambiar etapa de la visita a "PRE" después de crear el pre-contrato
            await _visitaServices.CambiarEtapa(cotizacion.SecVisita, "PRE");

            return preContratoCreado;
        }

        public async Task<PreContrato> Editar(PreContrato entidad)
        {
            if (entidad == null) throw new ArgumentNullException(nameof(entidad));

            ValidatePreContratoFields(entidad);

            var preContratoExistente = await _repositorio.Obtener(p => p.SecPreContrato == entidad.SecPreContrato);
            if (preContratoExistente == null)
            {
                throw new Exception("El pre-contrato no existe.");
            }

            preContratoExistente.Dias = entidad.Dias;
            preContratoExistente.TipoDias = entidad.TipoDias;
            // ValorContrato movido a PlanDePago
            preContratoExistente.AniosGarantia = entidad.AniosGarantia;
            preContratoExistente.MesesGarantia = entidad.MesesGarantia;
            preContratoExistente.PeriodoMantenimiento = entidad.PeriodoMantenimiento;
            preContratoExistente.PolizaGarantia = entidad.PolizaGarantia;
            // ValorAnticipo, FechaAnticipo, SecFormaPago, NumeroCuotas, FechaPrimeraCuota movidos a PlanDePago
            preContratoExistente.EstaActivo = entidad.EstaActivo;
            preContratoExistente.SecPlantillaPreContrato = entidad.SecPlantillaPreContrato;


            await _repositorio.Editar(preContratoExistente);
            return preContratoExistente;
        }

        private void ValidatePreContratoFields(PreContrato entidad)
        {
            if (entidad.Dias < 0)
            {
                throw new InvalidOperationException("El número de días no puede ser negativo.");
            }
            if (entidad.MesesGarantia < 0)
            {
                throw new InvalidOperationException("El número de meses de garantía no puede ser negativo.");
            }
            if (entidad.AniosGarantia < 0)
            {
                throw new InvalidOperationException("El número de años de garantía no puede ser negativo.");
            }
        }

        public async Task<bool> Eliminar(int secPreContrato)
        {
            var preContrato = await _repositorio.Obtener(p => p.SecPreContrato == secPreContrato);
            if (preContrato == null)
            {
                throw new Exception("El pre-contrato no existe.");
            }

            var resultado = await _repositorio.Eliminar(preContrato);
            if (resultado)
            {
                // Rollback: Si se elimina el pre-contrato, regresar la visita a etapa "COT"
                // Pasamos permitirRetroceso = true para saltar la validación de orden.
                await _visitaServices.CambiarEtapa(preContrato.SecCotizacionNavigation.SecVisita, "COT", permitirRetroceso: true);
            }
            return resultado;
        }

        public async Task<PreContrato> ObtenerUltimaVersion(int secCotizacion)
        {
            var preContratos = await _repositorio.Consultar(p => p.SecCotizacion == secCotizacion);
            return await preContratos.Include(p => p.PreContratoCompromisoPagos) // Incluir los compromisos de pago
                                     .OrderByDescending(p => p.Version)
                                     .FirstOrDefaultAsync();
        }

        public async Task<PreContrato> CrearDesdeCotizacion(int cotizacionId, int secUsuario)
        {
            var preContratoExistente = await _repositorio.Obtener(p => p.SecCotizacion == cotizacionId && p.EstaActivo == true);
            if (preContratoExistente != null)
            {
                return preContratoExistente;
            }

            var cotizacion = await _cotizacionServices.Detalle(cotizacionId);
            if (cotizacion == null)
            {
                throw new Exception("La cotización especificada no existe.");
            }

            var nuevoPreContrato = new PreContrato
            {
                SecCotizacion = cotizacionId,
                SecUsuarioCrea = secUsuario,
                Version = 1,
                Estado = "Borrador",
                EstaActivo = true,
                FechaRegistro = DateTime.Now,
                Dias = 0,
                TipoDias = "N/A",
                // ValorContrato movido a PlanDePago
                AniosGarantia = 0,
                MesesGarantia = 0,
                PeriodoMantenimiento = "N/A",
                PolizaGarantia = "N/A",
                // ValorAnticipo, NumeroCuotas movidos a PlanDePago
            };

            var preContratoCreado = await _repositorio.Crear(nuevoPreContrato);
            return preContratoCreado;
        }

        // Método GuardarDesdeEditor eliminado por migración a flujo DOCX

        public async Task<List<PreContrato>> ObtenerHistorial(int secPreContrato)
        {
            var preContratoActual = await _repositorio.Obtener(p => p.SecPreContrato == secPreContrato);
            if (preContratoActual == null)
            {
                return new List<PreContrato>();
            }

            var query = await _repositorio.Consultar(p => p.SecCotizacion == preContratoActual.SecCotizacion);

            return await query.Include(p => p.SecUsuarioCreaNavigation)
                                .OrderByDescending(p => p.FechaRegistro)
                                .ToListAsync();
        }

        // Método ObtenerPrimerParrafo eliminado

        public async Task<PreContrato> CrearDesdeModal(PreContrato entidad, int usuarioId /*, string contenidoHtml REMOVED */)
        {
            // Obtener el TipoDocumento para Pre-Contrato
            var tipoDocumentoPreContrato = await _repositorioTipoDocumento.Obtener(td => td.Codigo == "PRE-CONTRATO");
            if (tipoDocumentoPreContrato == null)
            {
                throw new Exception("No se encontró el tipo de documento 'PRE-CONTRATO'.");
            }

            // Obtener la plantilla: Prioridad 1 = Vinculada explícitamente, Prioridad 2 = Relación antigua
            PlantillaPreContrato plantillaPorDefecto = null;
            if (tipoDocumentoPreContrato.SecPlantilla.HasValue)
            {
                plantillaPorDefecto = await _repositorioPlantillaPreContrato.Obtener(p => p.SecPlantillaPreContrato == tipoDocumentoPreContrato.SecPlantilla && p.EstaActivo == 1);
            }

            if (plantillaPorDefecto == null)
            {
               plantillaPorDefecto = await _repositorioPlantillaPreContrato.Obtener(
                p => p.SecTipoDocumento == tipoDocumentoPreContrato.SecTipoDocumento && p.EstaActivo == 1);
            }
            
            if (plantillaPorDefecto == null)
            {
                throw new Exception("No se encontró una plantilla de Pre-Contrato activa por defecto. Por favor, configure una.");
            }

            entidad.SecPlantillaPreContrato = plantillaPorDefecto.SecPlantillaPreContrato; // Asignar la plantilla encontrada
            entidad.SecUsuarioCrea = usuarioId;
            entidad.Version = 1; // Siempre 1 para la creación inicial
            entidad.Estado = "Borrador"; // O el estado inicial que corresponda
            entidad.EstaActivo = true;
            entidad.FechaRegistro = DateTime.Now; // Asegurar que FechaCreacion se establezca

            // Los campos de pago y fechas se manejan en la entidad PlanDePago, no en PreContrato.
            // Por lo tanto, no se asignan valores aquí.

            var preContratoCreado = await _repositorio.Crear(entidad);
            if (preContratoCreado == null || preContratoCreado.SecPreContrato == 0)
            {
                throw new Exception("No se pudo crear el pre-contrato desde el modal.");
            }

            return preContratoCreado;
        }



        public async Task<bool> Aprobar(int secPreContrato)
        {
            var preContrato = await _repositorio.Obtener(p => p.SecPreContrato == secPreContrato);
            if (preContrato == null)
            {
                throw new Exception("El pre-contrato no existe.");
            }

            preContrato.Estado = "Aprobado";
            return await _repositorio.Editar(preContrato);
        }

        // Métodos de manejo de contenido HTML eliminados (ObtenerContenidoHtml, ActualizarContenidoPreContrato, CrearDesdeModalConPagos)

        public async Task<PreContrato> GuardarBorrador(PreContratoConPagosDTO dto, int usuarioId)
        {
            PreContrato preContratoParaNuevosCompromisos;

            if (dto.SecPreContrato > 0) // Si se proporciona SecPreContrato, es una ACTUALIZACIÓN (con versionado)
            {
                // 1. Obtener la versión activa actual del pre-contrato
                var oldPreContrato = await _repositorio.Obtener(p => p.SecPreContrato == dto.SecPreContrato && p.EstaActivo == true);

                if (oldPreContrato == null)
                {
                    throw new Exception("Pre-contrato no encontrado o no activo para actualizar.");
                }

                // 2. Marcar la versión anterior como inactiva y PERSISTIR EL CAMBIO
                oldPreContrato.EstaActivo = false;
                bool inactivado = await _repositorio.Editar(oldPreContrato);
                if (!inactivado)
                {
                    throw new Exception("No se pudo inactivar la versión anterior del pre-contrato.");
                }

                // 3. Crear una nueva entidad PreContrato (nueva versión)
                // Obtener la versión más alta existente para esta cotización
                var maxVersion = await _repositorio
                                       .Consultar(p => p.SecCotizacion == oldPreContrato.SecCotizacion)
                                       .Result
                                       .MaxAsync(p => (int?)p.Version) ?? 0;

                var newPreContrato = new PreContrato
                {
                    SecCotizacion = oldPreContrato.SecCotizacion,
                    SecPlantillaPreContrato = oldPreContrato.SecPlantillaPreContrato,
                    SecUsuarioCrea = usuarioId, // El usuario que guarda la nueva versión
                    Version = maxVersion + 1,
                    Estado = "Borrador", // Sigue siendo borrador
                    EstaActivo = true,
                    FechaRegistro = DateTime.Now,

                    // Copiar datos de la versión anterior
                    Dias = oldPreContrato.Dias,
                    TipoDias = oldPreContrato.TipoDias,
                    PeriodoMantenimiento = oldPreContrato.PeriodoMantenimiento,
                    AniosGarantia = oldPreContrato.AniosGarantia,
                    MesesGarantia = oldPreContrato.MesesGarantia,
                    PolizaGarantia = oldPreContrato.PolizaGarantia
                };

                // 4. Actualizar la nueva entidad con los datos del DTO
                newPreContrato.Dias = dto.Dias;
                newPreContrato.TipoDias = dto.TipoDias;
                newPreContrato.PeriodoMantenimiento = dto.PeriodoMantenimiento;
                newPreContrato.AniosGarantia = dto.AniosGarantia;
                newPreContrato.MesesGarantia = dto.MesesGarantia;
                newPreContrato.PolizaGarantia = dto.PolizaGarantia;

                preContratoParaNuevosCompromisos = await _repositorio.Crear(newPreContrato);
                if (preContratoParaNuevosCompromisos == null || preContratoParaNuevosCompromisos.SecPreContrato == 0)
                {
                    throw new Exception("No se pudo crear la nueva versión del borrador del pre-contrato.");
                }

                // El contenido (párrafo) ya no se copia porque es generado dinámicamente en DOCX.

                // Los compromisos de pago anteriores no se eliminan, simplemente no se asocian a la nueva versión.
                // Los nuevos compromisos se crearán para la nueva versión.

            }
            else // Si SecPreContrato es 0 o no se proporciona, es una CREACIÓN
            {
                // --- RUTA DE CREACIÓN ---
                // Obtener el TipoDocumento basándose en el SecTipoDocumento proporcionado en el DTO
                var tipoDocumentoPreContrato = await _repositorioTipoDocumento.Obtener(td => td.SecTipoDocumento == dto.SecTipoDocumento && td.EstaActivo == true);
                if (tipoDocumentoPreContrato == null) throw new Exception($"No se encontró el tipo de documento con SecTipoDocumento {dto.SecTipoDocumento}.");

                // Estrategia de Selección de Plantilla:
                // 1. Si el TipoDocumento tiene una Plantilla explícita (SecPlantilla), usar esa.
                // 2. Si no, buscar una plantilla que apunte a ese TipoDocumento (Legacy).
                PlantillaPreContrato plantillaPorDefecto = null;

                if (tipoDocumentoPreContrato.SecPlantilla.HasValue)
                {
                    plantillaPorDefecto = await _repositorioPlantillaPreContrato.Obtener(p => p.SecPlantillaPreContrato == tipoDocumentoPreContrato.SecPlantilla && p.EstaActivo == 1);
                }

                if (plantillaPorDefecto == null)
                {
                    plantillaPorDefecto = await _repositorioPlantillaPreContrato.Obtener(p => p.SecTipoDocumento == tipoDocumentoPreContrato.SecTipoDocumento && p.EstaActivo == 1);
                }

                if (plantillaPorDefecto == null) throw new Exception($"No se encontró una plantilla de Pre-Contrato activa por defecto para el tipo de documento {tipoDocumentoPreContrato.Descripcion}.");

                var nuevoPreContrato = new PreContrato
                {
                    SecCotizacion = dto.SecCotizacion,
                    Dias = dto.Dias,
                    TipoDias = dto.TipoDias,
                    PeriodoMantenimiento = dto.PeriodoMantenimiento,
                    AniosGarantia = dto.AniosGarantia,
                    MesesGarantia = dto.MesesGarantia,
                    PolizaGarantia = dto.PolizaGarantia,
                    SecPlantillaPreContrato = plantillaPorDefecto.SecPlantillaPreContrato,
                    SecUsuarioCrea = usuarioId,
                    Version = 1,
                    Estado = "Borrador",
                    EstaActivo = true,
                    FechaRegistro = DateTime.Now
                };

                preContratoParaNuevosCompromisos = await _repositorio.Crear(nuevoPreContrato);
                if (preContratoParaNuevosCompromisos == null || preContratoParaNuevosCompromisos.SecPreContrato == 0)
                {
                    throw new Exception("No se pudo crear el borrador del pre-contrato.");
                }
            }

            // --- LÓGICA COMÚN: Crear los nuevos compromisos de pago para la nueva versión ---
            if (dto.CompromisosDePago != null && dto.CompromisosDePago.Any())
            {
                int numeroCuota = 1;
                foreach (var compromisoDto in dto.CompromisosDePago)
                {
                    var nuevoCompromiso = new PreContratoCompromisoPago
                    {
                        SecPreContrato = preContratoParaNuevosCompromisos.SecPreContrato,
                        NumeroCuota = compromisoDto.Tipo == "Anticipo" ? 0 : numeroCuota++,
                        Monto = compromisoDto.Monto,
                        FechaVencimiento = compromisoDto.FechaVencimiento,
                        Tipo = compromisoDto.Tipo,
                        FechaRegistro = DateTime.Now
                    };
                    await _repositorioCompromisoPago.Crear(nuevoCompromiso);
                }
            }

            return preContratoParaNuevosCompromisos;
        }

        public async Task<PreContratoParaEdicionDTO> ObtenerParaEdicion(int secPreContrato)
        {
            var query = await _repositorio.Consultar(p => p.SecPreContrato == secPreContrato);
            var preContrato = await query.Include(p => p.PreContratoCompromisoPagos).FirstOrDefaultAsync();

            if (preContrato == null)
            {
                throw new Exception("No se encontró el pre-contrato solicitado para edición.");
            }

            // Obtener el SecTipoDocumento de la plantilla asociada
            int secTipoDocumento = 0;
            if (preContrato.SecPlantillaPreContrato > 0)
            {
                var plantilla = await _repositorioPlantillaPreContrato.Obtener(p => p.SecPlantillaPreContrato == preContrato.SecPlantillaPreContrato);
                if (plantilla != null)
                {
                    secTipoDocumento = plantilla.SecTipoDocumento;
                }
            }

            var dto = new PreContratoParaEdicionDTO
            {
                SecPreContrato = preContrato.SecPreContrato,
                SecCotizacion = preContrato.SecCotizacion,
                SecTipoDocumento = secTipoDocumento, // Asignar el SecTipoDocumento
                Dias = preContrato.Dias,
                TipoDias = preContrato.TipoDias,
                PeriodoMantenimiento = preContrato.PeriodoMantenimiento,
                AniosGarantia = preContrato.AniosGarantia,
                MesesGarantia = preContrato.MesesGarantia,
                PolizaGarantia = preContrato.PolizaGarantia,
                CompromisosDePago = preContrato.PreContratoCompromisoPagos.Select(c => new CompromisoPagoDTO
                {
                    Tipo = c.Tipo,
                    Monto = c.Monto,
                    FechaVencimiento = c.FechaVencimiento
                }).ToList()
            };

            return dto;
        }
        public async Task<bool> SubirContratoFinal(int secPreContrato, Stream archivoStream)
        {
            var preContrato = await _repositorio.Consultar(p => p.SecPreContrato == secPreContrato);
            var entidad = await preContrato.Include(p => p.PreContratoParrafos).FirstOrDefaultAsync();

            if (entidad == null) throw new Exception("Pre-contrato no encontrado.");

            using (var memoryStream = new MemoryStream())
            {
                await archivoStream.CopyToAsync(memoryStream);
                var bytes = memoryStream.ToArray();
                var base64 = Convert.ToBase64String(bytes);
                var contenido = "BASE64DOCX:" + base64;

                var parrafo = entidad.PreContratoParrafos.FirstOrDefault();
                if (parrafo != null)
                {
                    parrafo.Contenido = contenido;
                    await _repositorioPreContratoParrafo.Editar(parrafo);
                }
                else
                {
                    var nuevoParrafo = new PreContratoParrafo
                    {
                        SecPreContrato = secPreContrato,
                        Contenido = contenido,
                        Orden = 1
                    };
                    await _repositorioPreContratoParrafo.Crear(nuevoParrafo);
                }
            }
            return true;
        }
    }
}
