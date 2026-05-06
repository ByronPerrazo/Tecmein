using Microsoft.AspNetCore.Mvc;
using BLL.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using Entity;
using AutoMapper;
using TecmeinAplicacionWeb.Models.ViewModels;
using Newtonsoft.Json;
using System.Security.Claims;
using TecmeinWebApp.Utilidades.Response;
using TecmeinWebApp.Utilidades.ViewComponents;
using BLL.DTOs;
using System.Linq;

namespace TecmeinAplicacionWeb.Controllers
{
    public class PreContratoController : Controller
    {
        private readonly IPreContratoServices _preContratoService;
        private readonly ICotizacionServices _cotizacionService;
        private readonly IFormaPagoServices _formaPagoService;
        private readonly IPlantillaPreContratoServices _plantillaPreContratoService;
        private readonly IPolizaGarantiaServices _polizaGarantiaService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IPreContratoGeneratorService _preContratoGeneratorService;
        private readonly ITipoDocumentoServices _tipoDocumentoServices; // Inyectado

        public PreContratoController(IPreContratoServices preContratoService, ICotizacionServices cotizacionService, IFormaPagoServices formaPagoService, IPlantillaPreContratoServices plantillaPreContratoService, IPolizaGarantiaServices polizaGarantiaService, IMapper mapper, IConfiguration configuration, IPreContratoGeneratorService preContratoGeneratorService, ITipoDocumentoServices tipoDocumentoServices)
        {
            _preContratoService = preContratoService;
            _cotizacionService = cotizacionService;
            _formaPagoService = formaPagoService;
            _plantillaPreContratoService = plantillaPreContratoService;
            _polizaGarantiaService = polizaGarantiaService;
            _mapper = mapper;
            _configuration = configuration;
            _preContratoGeneratorService = preContratoGeneratorService;
            _tipoDocumentoServices = tipoDocumentoServices;
        }

        [ValidatePermission("VER_MENU")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> Listar()
        {
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int usuarioId = usuarioIdClaim != null ? int.Parse(usuarioIdClaim.Value) : 0;

            var lista = await _preContratoService.Lista(usuarioId);
            List<PreContratoVM> vmLista = _mapper.Map<List<PreContratoVM>>(lista);
            return StatusCode(StatusCodes.Status200OK, new { data = vmLista });
        }

        [HttpPost]
        [ValidatePermission("ACTUALIZAR")]
        public async Task<IActionResult> Aprobar(int id)
        {
            try
            {
                bool resultado = await _preContratoService.Aprobar(id);
                return StatusCode(StatusCodes.Status200OK, new { success = resultado, message = resultado ? "Pre-contrato aprobado correctamente." : "No se pudo aprobar el pre-contrato." });
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = ex.Message });
            }
        }

        [HttpDelete]
        [ValidatePermission("ELIMINAR")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int usuarioId = usuarioIdClaim != null ? int.Parse(usuarioIdClaim.Value) : 0;

            bool resultado = await _preContratoService.Eliminar(id, usuarioId);
            return StatusCode(StatusCodes.Status200OK, new { estado = resultado, mensajes = resultado ? "Pre-contrato eliminado correctamente." : "No se pudo eliminar el pre-contrato." });
        }

        [HttpGet]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> Historial(int id)
        {
            try
            {
                var lista = await _preContratoService.ObtenerHistorial(id);
                List<PreContratoVM> vmLista = _mapper.Map<List<PreContratoVM>>(lista);
                return StatusCode(StatusCodes.Status200OK, new { data = vmLista });
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = ex.Message });
            }
        }

        // Método Editor eliminado (flujo DOCX)

        [HttpGet]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> ListaCotizacionesAprobadas()
        {
            try
            {
                var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                int usuarioId = usuarioIdClaim != null ? int.Parse(usuarioIdClaim.Value) : 0;

                var cotizaciones = await _cotizacionService.Lista(usuarioId);
                var cotizacionesAprobadas = cotizaciones
                    .Where(c => c.EstaActivo == 1 && c.Confirmacion == true)
                    .Select(c => new
                    {
                        value = c.Secuencial,
                        text = $"COT-{c.Secuencial} - {(!string.IsNullOrEmpty(c.NombreObra) ? c.NombreObra : "Sin Nombre de Obra")}"
                    })
                    .ToList();

                return StatusCode(StatusCodes.Status200OK, new { data = cotizacionesAprobadas });
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> ListaFormasPago()
        {
            try
            {
                var lista = await _formaPagoService.Lista();
                var formasPago = lista.Select(fp => new { value = fp.SecFormaPago, text = fp.Descripcion }).ToList();
                return StatusCode(StatusCodes.Status200OK, new { data = formasPago });
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> ListaPlantillas()
        {
            try
            {
                var lista = await _plantillaPreContratoService.Lista();
                var plantillas = lista.Select(p => new { value = p.SecPlantillaPreContrato, text = p.Nombre }).ToList();
                return StatusCode(StatusCodes.Status200OK, new { data = plantillas });
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> ListaParaDropdown()
        {
            try
            {
                var lista = await _polizaGarantiaService.Lista();
                var polizas = lista.Select(p => new { value = p.Secuencial, text = p.Descripcion }).ToList();
                return StatusCode(StatusCodes.Status200OK, new { data = polizas });
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidatePermission("ACTUALIZAR")]
        public async Task<IActionResult> SubirContratoFinal(int secPreContrato, IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
                return Json(new { estado = false, mensajes = "Debe subir un archivo válido." });

            var extension = Path.GetExtension(archivo.FileName).ToLower();
            if (extension != ".docx")
                return Json(new { estado = false, mensajes = "Solo se permiten archivos DOCX." });

            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int usuarioId = usuarioIdClaim != null ? int.Parse(usuarioIdClaim.Value) : 0;

            using (var stream = archivo.OpenReadStream())
            {
                await _preContratoService.SubirContratoFinal(secPreContrato, stream, usuarioId);
            }

            return Json(new { estado = true, mensajes = "Contrato final guardado exitosamente." });
        }

        [HttpGet]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> DescargarContratoFinal(int id)
        {
             try
             {
                 return StatusCode(StatusCodes.Status501NotImplemented, new { mensajes = "Descarga de contrato final no implementada aún." });
             }
             catch(Exception ex)
             {
                 return StatusCode(StatusCodes.Status500InternalServerError, new { estado = false, mensajes = ex.Message });
             }
        }

        [HttpGet]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> DescargarDocumentoActual(int id)
        {
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int usuarioId = usuarioIdClaim != null ? int.Parse(usuarioIdClaim.Value) : 0;

            byte[] documentoGuardado = await _preContratoService.ObtenerContenidoDocumento(id, usuarioId);
            
            if (documentoGuardado != null)
            {
                return File(documentoGuardado, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"PreContrato_Editado_{id}.docx");
            }

            var datosEdicion = await _preContratoService.ObtenerParaEdicion(id, usuarioId);
            var preContratoData = new BLL.DTOs.PreContratoConPagosDTO
            {
                SecCotizacion = datosEdicion.SecCotizacion,
                SecTipoDocumento = datosEdicion.SecTipoDocumento,
                Dias = datosEdicion.Dias,
                TipoDias = datosEdicion.TipoDias,
                PeriodoMantenimiento = datosEdicion.PeriodoMantenimiento,
                AniosGarantia = datosEdicion.AniosGarantia,
                MesesGarantia = datosEdicion.MesesGarantia,
                PolizaGarantia = datosEdicion.PolizaGarantia,
                CompromisosDePago = datosEdicion.CompromisosDePago
            };

            byte[] docxBytes = await _preContratoGeneratorService.GenerarVistaPreviaDocx(preContratoData);
            return File(docxBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"PreContrato_Generado_{id}.docx");
        }

        [HttpPost]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> GenerarPreContratoDocx([FromBody] GenerarVistaPreviaRequest request)
        {
            try
            {
                var preContratoData = new BLL.DTOs.PreContratoConPagosDTO
                {
                    SecCotizacion = request.SecCotizacion,
                    Dias = request.Dias,
                    TipoDias = request.TipoDias,
                    PeriodoMantenimiento = request.PeriodoMantenimiento,
                    AniosGarantia = request.AniosGarantia,
                    MesesGarantia = request.MesesGarantia,
                    PolizaGarantia = request.PolizaGarantia,
                    SecTipoDocumento = request.SecTipoDocumento,
                    CompromisosDePago = request.CompromisosDePago
                };

                byte[] docxBytes = await _preContratoGeneratorService.GenerarVistaPreviaDocx(preContratoData);

                string fileName = $"PreContrato_COT-{request.SecCotizacion}_{DateTime.Now:yyyyMMddHHmmss}.docx";

                return File(docxBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
            }
            catch (Exception ex)
            {
                // Devolvemos un error en formato JSON para que el cliente pueda manejarlo
                return StatusCode(StatusCodes.Status500InternalServerError, new { estado = false, mensajes = $"Error al generar el documento: {ex.Message}" });
            }
        }

        [HttpPost]
        [ValidatePermission("CREAR")]
        public async Task<IActionResult> CrearDesdeModal([FromBody] CrearPreContratoRequest request)
        {
            try
            {
                // Obtener el ID del usuario autenticado
                var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (usuarioIdClaim == null || !int.TryParse(usuarioIdClaim.Value, out int usuarioId))
                {
                    return Json(new { estado = false, mensajes = "Usuario no autenticado o ID de usuario inválido." });
                }

                // Mapear el request a PreContrato
                var entidad = new PreContrato
                {
                    SecCotizacion = request.SecCotizacion,
                    //SecFormaPago = request.SecFormaPago,
                    SecPlantillaPreContrato = request.SecPlantillaPreContrato,
                    //ValorContrato = request.ValorContrato,
                    //ValorAnticipo = request.ValorAnticipo,
                    //FechaAnticipo = string.IsNullOrEmpty(request.FechaAnticipo) ? (DateTime?)null : DateTime.Parse(request.FechaAnticipo),
                    //NumeroCuotas = request.NumeroCuotas,
                    //FechaPrimeraCuota = string.IsNullOrEmpty(request.FechaPrimeraCuota) ? (DateTime?)null : DateTime.Parse(request.FechaPrimeraCuota),
                    Dias = request.Dias,
                    TipoDias = request.TipoDias,
                    PeriodoMantenimiento = request.PeriodoMantenimiento,
                    AniosGarantia = request.AniosGarantia,
                    MesesGarantia = request.MesesGarantia,
                    PolizaGarantia = request.PolizaGarantia
                };

                var preContratoCreado = await _preContratoService.CrearDesdeModal(entidad, usuarioId);
                if (preContratoCreado == null || preContratoCreado.SecPreContrato == 0)
                {
                    return Json(new { estado = false, mensajes = "No se pudo crear el pre-contrato." });
                }

                return Json(new { estado = true, mensajes = "Pre-contrato creado exitosamente." });
            }
            catch (Exception ex)
            {
                return Json(new { estado = false, mensajes = ex.Message });
            }
        }



        // Método CrearDesdeModalConPagos eliminado (viejo modal HTML)

        [HttpPost]
        [ValidatePermission("ACTUALIZAR")]
        public async Task<IActionResult> GuardarBorrador([FromBody] BLL.DTOs.PreContratoConPagosDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var errores = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return Json(new { estado = false, mensajes = $"Error de validación: {errores}" });
            }

            try
            {
                var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (usuarioIdClaim == null || !int.TryParse(usuarioIdClaim.Value, out int usuarioId))
                {
                    return Json(new { estado = false, mensajes = "Usuario no autenticado o ID de usuario inválido." });
                }

                if (dto == null)
                {
                     return Json(new { estado = false, mensajes = "El servidor recibió un objeto nulo. Verifique el formato de los datos." });
                }

                var preContratoGuardado = await _preContratoService.GuardarBorrador(dto, usuarioId);
                if (preContratoGuardado == null || preContratoGuardado.SecPreContrato == 0)
                {
                    return Json(new { estado = false, mensajes = "No se pudo guardar el borrador del pre-contrato." });
                }

                return Json(new { estado = true, mensajes = "Borrador guardado exitosamente." });
            }
            catch (Exception ex)
            {
                return Json(new { estado = false, mensajes = ex.Message });
            }
        }

        [HttpGet]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> DetallesParaEdicion(int id)
        {
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int usuarioId = usuarioIdClaim != null ? int.Parse(usuarioIdClaim.Value) : 0;

            var dto = await _preContratoService.ObtenerParaEdicion(id, usuarioId);
            return Json(new { estado = true, objeto = dto });
        }

        [HttpGet]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> ListaTipoDocumentos()
        {
            try
            {
                var lista = await _tipoDocumentoServices.Lista();
                var tiposDocumento = lista.Where(td => td.EstaActivo == true)
                                          .Select(td => new { value = td.SecTipoDocumento, text = td.Descripcion })
                                          .ToList();
                return StatusCode(StatusCodes.Status200OK, new { data = tiposDocumento });
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = ex.Message });
            }
        }
    }

        // GuardarPreContratoRequest eliminado
}