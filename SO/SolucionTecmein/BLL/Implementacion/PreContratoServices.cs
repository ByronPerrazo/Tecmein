using BLL.DTOs;
using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace BLL.Implementacion
{
    public class PreContratoServices : IPreContratoServices
    {
        private readonly IGenericRepository<PreContrato> _repositorio;
        private readonly ICotizacionServices _cotizacionServices;
        private readonly IGenericRepository<PreContratoParrafo> _repositorioPreContratoParrafo;
        private readonly IPreContratoGeneratorService _preContratoGeneratorService;
        private readonly IGenericRepository<TipoDocumento> _repositorioTipoDocumento; 
        private readonly IGenericRepository<PlantillaPreContrato> _repositorioPlantillaPreContrato; 
        private readonly IGenericRepository<PreContratoCompromisoPago> _repositorioCompromisoPago;
        private readonly IUsuarioServices _usuarioServices;
        private readonly IMapper _mapper;
        private readonly IVisitaServices _visitaServices;
        private readonly IStorageServices _storageService;
        private readonly IUnitOfWork _unitOfWork;

        public PreContratoServices(
            IGenericRepository<PreContrato> repositorio,
            ICotizacionServices cotizacionServices,
            IGenericRepository<PreContratoParrafo> repositorioPreContratoParrafo,
            IPreContratoGeneratorService preContratoGeneratorService,
            IVisitaServices visitaServices,
            IGenericRepository<TipoDocumento> repositorioTipoDocumento, 
            IGenericRepository<PlantillaPreContrato> repositorioPlantillaPreContrato, 
            IGenericRepository<PreContratoCompromisoPago> repositorioCompromisoPago,
            IUsuarioServices usuarioServices,
            IMapper mapper,
            IStorageServices storageService,
            IUnitOfWork unitOfWork)
        {
            _repositorio = repositorio;
            _cotizacionServices = cotizacionServices;
            _repositorioPreContratoParrafo = repositorioPreContratoParrafo;
            _preContratoGeneratorService = preContratoGeneratorService;
            _visitaServices = visitaServices;
            _repositorioTipoDocumento = repositorioTipoDocumento;
            _repositorioPlantillaPreContrato = repositorioPlantillaPreContrato;
            _repositorioCompromisoPago = repositorioCompromisoPago;
            _usuarioServices = usuarioServices;
            _mapper = mapper;
            _storageService = storageService;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<PreContratoDTO>> Lista(int secUsuario)
        {
            var usuario = await _usuarioServices.ObtenerPorId(secUsuario);
            bool esAdmin = usuario?.SecRol == 1;

            IQueryable<PreContrato> query = await _repositorio.Consultar(p => p.EstaActivo == true);

            // Filtrado de Seguridad (Punto 1)
            if (!esAdmin)
            {
                query = query.Where(p => p.SecUsuarioCrea == secUsuario);
            }

            var lista = await query.Include(p => p.SecCotizacionNavigation)
                                    .ThenInclude(c => c.SecVisitaNavigation)
                                        .ThenInclude(v => v.SecEmpresaNavigation) // Incluir Empresa
                                 .Include(p => p.SecCotizacionNavigation)
                                    .ThenInclude(c => c.SecVisitaNavigation)
                                        .ThenInclude(v => v.Contactovisita)
                                            .ThenInclude(cv => cv.SecContactoNavigation)
                               .Include(p => p.SecUsuarioCreaNavigation)
                               .ToListAsync();

            return _mapper.Map<List<PreContratoDTO>>(lista);
        }

        public async Task<PreContratoDTO> Obtener(int secPreContrato, int secUsuario)
        {
            var preContrato = await _repositorio.Obtener(p => p.SecPreContrato == secPreContrato);
            if (preContrato == null) return null;

            var usuario = await _usuarioServices.ObtenerPorId(secUsuario);
            if (usuario?.SecRol != 1 && preContrato.SecUsuarioCrea != secUsuario)
            {
                throw new UnauthorizedAccessException("No tiene permisos para acceder a este pre-contrato.");
            }

            return _mapper.Map<PreContratoDTO>(preContrato);
        }



        public async Task<PreContratoDTO> Crear(PreContratoDTO entidadDTO)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var entidad = _mapper.Map<PreContrato>(entidadDTO);
                if (entidad == null) throw new ArgumentNullException(nameof(entidad));

                ValidatePreContratoFields(entidad);

                // Note: ICotizacionServices.Detalle now requires secUsuario.
                var cotizacion = await _cotizacionServices.Detalle(entidad.SecCotizacion, entidad.SecUsuarioCrea);
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

                // Cambiar etapa de la visita a "PRE" (Atómico)
                await _visitaServices.CambiarEtapa(cotizacion.SecVisita, "PRE");

                await _unitOfWork.CommitTransactionAsync();
                return _mapper.Map<PreContratoDTO>(preContratoCreado);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<PreContratoDTO> Editar(PreContratoDTO entidadDTO)
        {
            var entidad = _mapper.Map<PreContrato>(entidadDTO);
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
            return _mapper.Map<PreContratoDTO>(preContratoExistente);
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

        public async Task<bool> Eliminar(int secPreContrato, int secUsuario)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var preContrato = await _repositorio.Obtener(p => p.SecPreContrato == secPreContrato, "SecCotizacionNavigation");
                if (preContrato == null)
                {
                    throw new Exception("El pre-contrato no existe.");
                }

                var usuario = await _usuarioServices.ObtenerPorId(secUsuario);
                if (usuario?.SecRol != 1 && preContrato.SecUsuarioCrea != secUsuario)
                {
                    throw new UnauthorizedAccessException("No tiene permisos para eliminar este pre-contrato.");
                }

                var secVisita = preContrato.SecCotizacionNavigation.SecVisita;
                var resultado = await _repositorio.Eliminar(preContrato);
                if (resultado)
                {
                    // Regresar la visita a etapa "COT" (Atómico)
                    await _visitaServices.CambiarEtapa(secVisita, "COT", permitirRetroceso: true);
                }

                await _unitOfWork.CommitTransactionAsync();
                return resultado;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<PreContratoDTO> ObtenerUltimaVersion(int secCotizacion)
        {
            var preContratos = await _repositorio.Consultar(p => p.SecCotizacion == secCotizacion);
            var preContrato = await preContratos
                                     .Include(p => p.SecUsuarioCreaNavigation)
                                     .Include(p => p.SecCotizacionNavigation)
                                         .ThenInclude(c => c.SecVisitaNavigation)
                                     .Include(p => p.SecPlantillaPreContratoNavigation)
                                         .ThenInclude(pl => pl.SecTipoDocumentoNavigation)
                                     .Include(p => p.PreContratoCompromisoPagos)
                                     .OrderByDescending(p => p.Version)
                                     .FirstOrDefaultAsync();
            return _mapper.Map<PreContratoDTO>(preContrato);
        }

        public async Task<PreContratoDTO> CrearDesdeCotizacion(int cotizacionId, int secUsuario)
        {
            var preContratoExistente = await _repositorio.Obtener(p => p.SecCotizacion == cotizacionId && p.EstaActivo == true);
            if (preContratoExistente != null)
            {
                return _mapper.Map<PreContratoDTO>(preContratoExistente);
            }

            var cotizacion = await _cotizacionServices.Detalle(cotizacionId, secUsuario);
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
                AniosGarantia = 0,
                MesesGarantia = 0,
                PeriodoMantenimiento = "N/A",
                PolizaGarantia = "N/A",
            };

            var preContratoCreado = await _repositorio.Crear(nuevoPreContrato);
            return _mapper.Map<PreContratoDTO>(preContratoCreado);
        }

        // Método GuardarDesdeEditor eliminado por migración a flujo DOCX

        public async Task<List<PreContratoDTO>> ObtenerHistorial(int secPreContrato)
        {
            var preContratoActual = await _repositorio.Obtener(p => p.SecPreContrato == secPreContrato);
            if (preContratoActual == null)
            {
                return new List<PreContratoDTO>();
            }

            var query = await _repositorio.Consultar(p => p.SecCotizacion == preContratoActual.SecCotizacion);

            var lista = await query.Include(p => p.SecUsuarioCreaNavigation)
                                .OrderByDescending(p => p.FechaRegistro)
                                .ToListAsync();

            return _mapper.Map<List<PreContratoDTO>>(lista);
        }

        // Método ObtenerPrimerParrafo eliminado

        public async Task<PreContratoDTO> CrearDesdeModal(PreContrato entidad, int usuarioId)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
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

                await _unitOfWork.CommitTransactionAsync();
                return _mapper.Map<PreContratoDTO>(preContratoCreado);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
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

        public async Task<PreContratoDTO> GuardarBorrador(PreContratoConPagosDTO dto, int usuarioId)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto), "Los datos del pre-contrato son nulos o inválidos.");

            await _unitOfWork.BeginTransactionAsync();
            try
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
                newPreContrato.Dias = dto.Dias ?? 0;
                newPreContrato.TipoDias = dto.TipoDias;
                newPreContrato.PeriodoMantenimiento = dto.PeriodoMantenimiento;
                newPreContrato.AniosGarantia = dto.AniosGarantia ?? 0;
                newPreContrato.MesesGarantia = dto.MesesGarantia ?? 0;
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
                    Dias = dto.Dias ?? 0,
                    TipoDias = dto.TipoDias,
                    PeriodoMantenimiento = dto.PeriodoMantenimiento,
                    AniosGarantia = dto.AniosGarantia ?? 0,
                    MesesGarantia = dto.MesesGarantia ?? 0,
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
                        FechaVencimiento = compromisoDto.FechaVencimiento ?? DateTime.Now,
                        Tipo = compromisoDto.Tipo,
                        FechaRegistro = DateTime.Now
                    };
                    await _repositorioCompromisoPago.Crear(nuevoCompromiso);
                }
            }

            await _unitOfWork.CommitTransactionAsync();
            return _mapper.Map<PreContratoDTO>(preContratoParaNuevosCompromisos);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

        public async Task<PreContratoParaEdicionDTO> ObtenerParaEdicion(int secPreContrato, int secUsuario)
        {
            var query = await _repositorio.Consultar(p => p.SecPreContrato == secPreContrato);
            var preContrato = await query.Include(p => p.PreContratoCompromisoPagos).FirstOrDefaultAsync();

            if (preContrato == null)
            {
                throw new Exception("No se encontró el pre-contrato solicitado para edición.");
            }

            var usuario = await _usuarioServices.ObtenerPorId(secUsuario);
            if (usuario?.SecRol != 1 && preContrato.SecUsuarioCrea != secUsuario)
            {
                throw new UnauthorizedAccessException("No tiene permisos para editar este pre-contrato.");
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
        public async Task<bool> SubirContratoFinal(int secPreContrato, Stream archivoStream, int secUsuario)
        {
            var preContratoQuery = await _repositorio.Consultar(p => p.SecPreContrato == secPreContrato);
            var entidad = await preContratoQuery.Include(p => p.PreContratoParrafos).FirstOrDefaultAsync();

            if (entidad == null) throw new Exception("Pre-contrato no encontrado.");

            var usuario = await _usuarioServices.ObtenerPorId(secUsuario);
            if (usuario?.SecRol != 1 && entidad.SecUsuarioCrea != secUsuario)
            {
                throw new UnauthorizedAccessException("No tiene permisos para subir archivos a este pre-contrato.");
            }

            string nombreArchivo = $"Contrato_{secPreContrato}_{DateTime.Now:yyyyMMddHHmmss}.docx";
            string rutaRelativa = await _storageService.SubirStorage(archivoStream, "Contratos", nombreArchivo);

            if (string.IsNullOrEmpty(rutaRelativa)) throw new Exception("No se pudo guardar el archivo en el almacenamiento local.");

            var parrafo = entidad.PreContratoParrafos.FirstOrDefault();
            if (parrafo != null)
            {
                parrafo.Contenido = "FILEPATH:" + rutaRelativa;
                await _repositorioPreContratoParrafo.Editar(parrafo);
            }
            else
            {
                var nuevoParrafo = new PreContratoParrafo
                {
                    SecPreContrato = secPreContrato,
                    Contenido = "FILEPATH:" + rutaRelativa,
                    Orden = 1
                };
                await _repositorioPreContratoParrafo.Crear(nuevoParrafo);
            }
            return true;
        }

        public async Task<byte[]> ObtenerContenidoDocumento(int secPreContrato, int secUsuario)
        {
            var preContrato = await _repositorio.Obtener(p => p.SecPreContrato == secPreContrato);
            if (preContrato == null) return null;

            var usuario = await _usuarioServices.ObtenerPorId(secUsuario);
            if (usuario?.SecRol != 1 && preContrato.SecUsuarioCrea != secUsuario)
            {
                throw new UnauthorizedAccessException("No tiene permisos para descargar este documento.");
            }

            var parrafo = await _repositorioPreContratoParrafo.Obtener(p => p.SecPreContrato == secPreContrato);
            if (parrafo == null || string.IsNullOrEmpty(parrafo.Contenido))
            {
                return null;
            }

            if (parrafo.Contenido.StartsWith("FILEPATH:"))
            {
                string rutaRelativa = parrafo.Contenido.Substring("FILEPATH:".Length);
                // Construimos la ruta absoluta basándonos en C:\TecmeinFiles (hardcoded por ahora en LocalStorageService)
                string rutaAbsoluta = Path.Combine(@"C:\TecmeinFiles", rutaRelativa.Replace("/", "\\"));
                
                if (File.Exists(rutaAbsoluta))
                {
                    return await File.ReadAllBytesAsync(rutaAbsoluta);
                }
            }

            if (parrafo.Contenido.StartsWith("BASE64DOCX:"))
            {
                var base64 = parrafo.Contenido.Substring("BASE64DOCX:".Length);
                return Convert.FromBase64String(base64);
            }

            return null;
        }
    }
}
