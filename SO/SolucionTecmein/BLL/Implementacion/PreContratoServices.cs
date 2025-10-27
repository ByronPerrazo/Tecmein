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

        public async Task<string> ObtenerContenidoPrevisualizado(int secPreContrato)
        {
            var preContrato = await Obtener(secPreContrato);
            if (preContrato == null)
            {
                throw new Exception("Pre-contrato no encontrado para previsualización.");
            }

            var dto = new PreContratoGeneratorDTO
            {
                SecCotizacion = preContrato.SecCotizacion,
                Dias = preContrato.Dias,
                TipoDias = preContrato.TipoDias,
                PeriodoMantenimiento = preContrato.PeriodoMantenimiento,
                AniosGarantia = preContrato.AniosGarantia,
                MesesGarantia = preContrato.MesesGarantia,
                PolizaGarantia = preContrato.PolizaGarantia
            };

            return await _preContratoGeneratorService.GenerarVistaPreviaHtml(dto);
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

            // Eliminar párrafos asociados primero
            var parrafos = await _repositorioPreContratoParrafo.Consultar(pp => pp.SecPreContrato == secPreContrato);
            foreach (var parrafo in parrafos)
            {
                await _repositorioPreContratoParrafo.Eliminar(parrafo);
            }

            return await _repositorio.Eliminar(preContrato);
        }

        public async Task<PreContrato> ObtenerUltimaVersion(int secCotizacion)
        {
            var preContratos = await _repositorio.Consultar(p => p.SecCotizacion == secCotizacion);
            return await preContratos.OrderByDescending(p => p.Version).FirstOrDefaultAsync();
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

        public async Task<PreContrato> GuardarDesdeEditor(int cotizacionId, string contenidoHtml, int usuarioId)
        {
            var ultimaVersion = await ObtenerUltimaVersion(cotizacionId);

            // Si existe una versión anterior, comparamos el contenido
            if (ultimaVersion != null)
            {
                var parrafoActual = await ObtenerPrimerParrafo(ultimaVersion.SecPreContrato);
                var contenidoActual = parrafoActual?.Contenido ?? "";

                // Si el contenido no ha cambiado, no hacemos nada y devolvemos la versión existente para evitar data redundante.
                if (string.Equals(contenidoActual, contenidoHtml, StringComparison.Ordinal))
                {
                    return ultimaVersion;
                }

                // Si el contenido cambió, desactivamos la versión anterior.
                ultimaVersion.EstaActivo = false;
                await _repositorio.Editar(ultimaVersion);
            }

            // Creamos el nuevo pre-contrato con la nueva versión, copiando los datos de la anterior.
            var nuevoPreContrato = new PreContrato
            {
                SecCotizacion = cotizacionId,
                SecUsuarioCrea = usuarioId,
                Version = (ultimaVersion?.Version ?? 0) + 1,
                Estado = "Guardado",
                EstaActivo = true,
                FechaRegistro = DateTime.Now,

                // Copiamos los datos de la versión anterior para no perderlos en la nueva versión.
                SecPlantillaPreContrato = ultimaVersion?.SecPlantillaPreContrato ?? 0,
                // SecFormaPago, ValorContrato, ValorAnticipo, FechaAnticipo, NumeroCuotas, FechaPrimeraCuota movidos a PlanDePago
                Dias = ultimaVersion?.Dias ?? 0,
                TipoDias = ultimaVersion?.TipoDias,
                PeriodoMantenimiento = ultimaVersion?.PeriodoMantenimiento,
                AniosGarantia = ultimaVersion?.AniosGarantia ?? 0,
                MesesGarantia = ultimaVersion?.MesesGarantia ?? 0,
                PolizaGarantia = ultimaVersion?.PolizaGarantia
            };

            var preContratoCreado = await _repositorio.Crear(nuevoPreContrato);

            // Creamos el nuevo párrafo con el contenido actualizado
            var nuevoParrafo = new PreContratoParrafo
            {
                SecPreContrato = preContratoCreado.SecPreContrato,
                Contenido = contenidoHtml,
                Orden = 1
            };
            await _repositorioPreContratoParrafo.Crear(nuevoParrafo);

            return preContratoCreado;
        }

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

        public async Task<PreContratoParrafo> ObtenerPrimerParrafo(int secPreContrato)
        {
            var query = await _repositorioPreContratoParrafo.Consultar(p => p.SecPreContrato == secPreContrato);
            return await query.OrderBy(p => p.Orden).FirstOrDefaultAsync();
        }

        public async Task<PreContrato> CrearDesdeModal(PreContrato entidad, int usuarioId, string contenidoHtml)
        {
            // Obtener el TipoDocumento para Pre-Contrato
            var tipoDocumentoPreContrato = await _repositorioTipoDocumento.Obtener(td => td.Codigo == "PRE-CONTRATO");
            if (tipoDocumentoPreContrato == null)
            {
                throw new Exception("No se encontró el tipo de documento 'PRE-CONTRATO'.");
            }

            // Obtener la plantilla por defecto activa para Pre-Contrato
            var plantillaPorDefecto = await _repositorioPlantillaPreContrato.Obtener(
                p => p.SecTipoDocumento == tipoDocumentoPreContrato.SecTipoDocumento && p.EstaActivo == 1); // Assuming EstaActivo is short for bool

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

            // Crear el párrafo inicial con el contenido HTML
            var nuevoParrafo = new PreContratoParrafo
            {
                SecPreContrato = preContratoCreado.SecPreContrato,
                Contenido = contenidoHtml,
                Orden = 1
            };
            await _repositorioPreContratoParrafo.Crear(nuevoParrafo);

            return preContratoCreado;
        }

        public Task<string> GenerarDocumentoWord(int secPreContrato)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ActualizarContenido(int secPreContrato, string contenidoHtml)
        {
            throw new NotImplementedException();
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

        public async Task<string> ObtenerContenidoHtml(int secPreContrato)
        {
            var query = await _repositorio.Consultar(p => p.SecPreContrato == secPreContrato);
            var preContrato = await query.Include(p => p.PreContratoParrafos).FirstOrDefaultAsync();

            if (preContrato == null || preContrato.PreContratoParrafos == null || !preContrato.PreContratoParrafos.Any())
            {
                return string.Empty;
            }

            return string.Join("\n", preContrato.PreContratoParrafos.OrderBy(p => p.Orden).Select(p => p.Contenido));
        }

        public async Task<PreContrato> ActualizarContenidoPreContrato(int secPreContrato, string contenidoHtml, int usuarioId)
        {
            var preContratoActual = await _repositorio.Consultar(p => p.SecPreContrato == secPreContrato && p.EstaActivo == true);
            var ultimaVersion = await preContratoActual.Include(p => p.PreContratoParrafos).FirstOrDefaultAsync();

            if (ultimaVersion == null)
            {
                throw new Exception("Pre-contrato no encontrado o no activo.");
            }

            var contenidoActual = ultimaVersion.PreContratoParrafos != null && ultimaVersion.PreContratoParrafos.Any()
                                ? string.Join("\n", ultimaVersion.PreContratoParrafos.OrderBy(p => p.Orden).Select(p => p.Contenido))
                                : string.Empty;

            // Si el contenido no ha cambiado, no hacemos nada y devolvemos la versión existente.
            if (string.Equals(contenidoActual, contenidoHtml, StringComparison.Ordinal))
            {
                return ultimaVersion;
            }

            // Inactivar la versión actual
            ultimaVersion.EstaActivo = false;
            await _repositorio.Editar(ultimaVersion);

            // Crear una nueva versión del pre-contrato
            var nuevaVersionPreContrato = new PreContrato
            {
                SecCotizacion = ultimaVersion.SecCotizacion,
                SecPlantillaPreContrato = ultimaVersion.SecPlantillaPreContrato,
                SecUsuarioCrea = usuarioId,
                //SecFormaPago = ultimaVersion.SecFormaPago,
                Version = ultimaVersion.Version + 1,
                Estado = "Guardado", // O el estado que corresponda después de editar
                EstaActivo = true,
                FechaRegistro = DateTime.Now,
                Dias = ultimaVersion.Dias,
                TipoDias = ultimaVersion.TipoDias,
                //ValorContrato = ultimaVersion.ValorContrato,
                AniosGarantia = ultimaVersion.AniosGarantia,
                MesesGarantia = ultimaVersion.MesesGarantia,
                PeriodoMantenimiento = ultimaVersion.PeriodoMantenimiento,
                PolizaGarantia = ultimaVersion.PolizaGarantia,
                //ValorAnticipo = ultimaVersion.ValorAnticipo,
                //FechaAnticipo = ultimaVersion.FechaAnticipo,
                //NumeroCuotas = ultimaVersion.NumeroCuotas,
                //FechaPrimeraCuota = ultimaVersion.FechaPrimeraCuota
            };

            var preContratoCreado = await _repositorio.Crear(nuevaVersionPreContrato);

            // Replicar los compromisos de pago de la versión anterior a la nueva
            var compromisosAnteriores = await _repositorioCompromisoPago.Consultar(c => c.SecPreContrato == ultimaVersion.SecPreContrato);
            if (compromisosAnteriores.Any())
            {
                foreach (var compromiso in compromisosAnteriores)
                {
                    var nuevoCompromiso = new PreContratoCompromisoPago
                    {
                        SecPreContrato = preContratoCreado.SecPreContrato,
                        NumeroCuota = compromiso.NumeroCuota,
                        Monto = compromiso.Monto,
                        FechaVencimiento = compromiso.FechaVencimiento,
                        Tipo = compromiso.Tipo,
                        FechaRegistro = DateTime.Now
                    };
                    await _repositorioCompromisoPago.Crear(nuevoCompromiso);
                }
            }

            // Guardar el nuevo párrafo con el contenido actualizado
            var nuevoParrafo = new PreContratoParrafo
            {
                SecPreContrato = preContratoCreado.SecPreContrato,
                Contenido = contenidoHtml,
                Orden = 1
            };
            await _repositorioPreContratoParrafo.Crear(nuevoParrafo);

            return preContratoCreado;
        }

        public async Task<string> GenerarVistaPreviaConPagos(PreContratoConPagosDTO dto)
        {
            var generatorDto = new PreContratoGeneratorDTO
            {
                SecCotizacion = dto.SecCotizacion,
                Dias = dto.Dias,
                TipoDias = dto.TipoDias,
                PeriodoMantenimiento = dto.PeriodoMantenimiento,
                AniosGarantia = dto.AniosGarantia,
                MesesGarantia = dto.MesesGarantia,
                PolizaGarantia = dto.PolizaGarantia
                // Los compromisos de pago no son parte de la vista previa del contrato principal,
                // pero podrían añadirse al HTML si fuera necesario en el futuro.
            };

            return await _preContratoGeneratorService.GenerarVistaPreviaHtml(generatorDto);
        }

        public async Task<PreContrato> CrearDesdeModalConPagos(PreContratoConPagosDTO dto, int usuarioId)
        {
            // 1. Obtener el pre-contrato existente que se está finalizando
            var preContratoExistente = await _repositorio.Obtener(p => p.SecPreContrato == dto.SecPreContrato && p.EstaActivo == true);

            if (preContratoExistente == null)
            {
                throw new Exception("Pre-contrato no encontrado o no activo para finalizar.");
            }

            // 2. Actualizar el estado del pre-contrato a 'Guardado' (o el estado final)
            preContratoExistente.Estado = "Guardado";
            await _repositorio.Editar(preContratoExistente);

            // 3. Actualizar o crear el párrafo con el contenido HTML
            var parrafoExistente = await _repositorioPreContratoParrafo.Obtener(p => p.SecPreContrato == preContratoExistente.SecPreContrato);
            if (parrafoExistente != null)
            {
                parrafoExistente.Contenido = dto.ContenidoHtml;
                await _repositorioPreContratoParrafo.Editar(parrafoExistente);
            }
            else
            {
                var nuevoParrafo = new PreContratoParrafo
                {
                    SecPreContrato = preContratoExistente.SecPreContrato,
                    Contenido = dto.ContenidoHtml,
                    Orden = 1
                };
                await _repositorioPreContratoParrafo.Crear(nuevoParrafo);
            }

            return preContratoExistente;
        }

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
                if (!inactivado) {
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

                // 5. Copiar el párrafo de contenido de la versión anterior a la nueva
                var oldParrafo = await _repositorioPreContratoParrafo.Obtener(p => p.SecPreContrato == oldPreContrato.SecPreContrato);
                if (oldParrafo != null)
                {
                    var newParrafo = new PreContratoParrafo
                    {
                        SecPreContrato = preContratoParaNuevosCompromisos.SecPreContrato,
                        Contenido = oldParrafo.Contenido,
                        Orden = oldParrafo.Orden
                    };
                    await _repositorioPreContratoParrafo.Crear(newParrafo);
                }

                // Los compromisos de pago anteriores no se eliminan, simplemente no se asocian a la nueva versión.
                // Los nuevos compromisos se crearán para la nueva versión.

            }
            else // Si SecPreContrato es 0 o no se proporciona, es una CREACIÓN
            {
                // --- RUTA DE CREACIÓN ---
                var tipoDocumentoPreContrato = await _repositorioTipoDocumento.Obtener(td => td.Codigo == "PRE-CONTRATO");
                if (tipoDocumentoPreContrato == null) throw new Exception("No se encontró el tipo de documento 'PRE-CONTRATO'.");

                var plantillaPorDefecto = await _repositorioPlantillaPreContrato.Obtener(p => p.SecTipoDocumento == tipoDocumentoPreContrato.SecTipoDocumento && p.EstaActivo == 1);
                if (plantillaPorDefecto == null) throw new Exception("No se encontró una plantilla de Pre-Contrato activa por defecto.");

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

            var dto = new PreContratoParaEdicionDTO
            {
                SecPreContrato = preContrato.SecPreContrato,
                SecCotizacion = preContrato.SecCotizacion,
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
    }
}
