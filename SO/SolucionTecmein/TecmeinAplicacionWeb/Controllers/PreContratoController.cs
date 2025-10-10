using Microsoft.AspNetCore.Mvc;
using BLL.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using Entity;
using AutoMapper;
using TecmeinAplicacionWeb.Models.ViewModels;
using Newtonsoft.Json;
using System.Security.Claims;

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

        public PreContratoController(IPreContratoServices preContratoService, ICotizacionServices cotizacionService, IFormaPagoServices formaPagoService, IPlantillaPreContratoServices plantillaPreContratoService, IPolizaGarantiaServices polizaGarantiaService, IMapper mapper, IConfiguration configuration, IPreContratoGeneratorService preContratoGeneratorService)
        {
            _preContratoService = preContratoService;
            _cotizacionService = cotizacionService;
            _formaPagoService = formaPagoService;
            _plantillaPreContratoService = plantillaPreContratoService;
            _polizaGarantiaService = polizaGarantiaService;
            _mapper = mapper;
            _configuration = configuration;
            _preContratoGeneratorService = preContratoGeneratorService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var lista = await _preContratoService.Lista();
            List<PreContratoVM> vmLista = _mapper.Map<List<PreContratoVM>>(lista);
            return StatusCode(StatusCodes.Status200OK, new { data = vmLista });
        }

        [HttpPost]
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
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                bool resultado = await _preContratoService.Eliminar(id);
                return StatusCode(StatusCodes.Status200OK, new { estado = resultado, mensajes = resultado ? "Pre-contrato eliminado correctamente." : "No se pudo eliminar el pre-contrato." });
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { estado = false, mensajes = ex.Message });
            }
        }

        [HttpGet]
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

        [HttpGet]
        public async Task<IActionResult> ContenidoParrafo(int id)
        {
            try
            {
                var parrafo = await _preContratoService.ObtenerPrimerParrafo(id);
                if (parrafo == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new { estado = false, mensajes = "Contenido no encontrado." });
                }
                return StatusCode(StatusCodes.Status200OK, new { estado = true, objeto = new { contenido = parrafo.Contenido } });
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { estado = false, mensajes = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Editor(int id)
        {
            try
            {
                // Obtener el modelo base (sin párrafos) para el @Model de la vista
                var preContrato = await _preContratoService.Obtener(id);
                if (preContrato == null)
                {
                    // Considerar una página de error amigable
                    return NotFound($"Pre-contrato con ID {id} no encontrado.");
                }
                var vmPreContrato = _mapper.Map<PreContratoVM>(preContrato);

                // Obtener el contenido HTML por separado y pasarlo por ViewBag
                string contenidoHtml = await _preContratoService.ObtenerContenidoHtml(id);
                ViewBag.Contenido = contenidoHtml;

                // Obtener la clave de TinyMCE desde la configuración
                ViewBag.TinyMceApiKey = _configuration["ApiKeys:TinyMCE"];

                return View(vmPreContrato);
            }
            catch (Exception ex)
            {
                // Loggear el error y mostrar una vista de error
                // Log.Error(ex, "Error al cargar el editor para PreContrato ID {id}");
                return View("Error"); // Asumiendo que tienes una vista de error genérica
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListaCotizacionesAprobadas()
        {
            try
            {
                var cotizaciones = await _cotizacionService.Lista();
                var cotizacionesAprobadas = cotizaciones
                    .Where(c => c.EstaActivo == 1 && c.Confirmacion == true && c.SecVisitaNavigation != null)
                    .Select(c => new
                    {
                        value = c.Secuencial,
                        text = $"COT-{c.Secuencial} - {(c.SecVisitaNavigation != null ? c.SecVisitaNavigation.Nombre : "Sin Nombre de Obra")}"
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
        public async Task<IActionResult> GuardarPreContrato([FromBody] GuardarPreContratoRequest request)
        {
            try
            {
                // Obtener el ID del usuario autenticado
                var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (usuarioIdClaim == null || !int.TryParse(usuarioIdClaim.Value, out int usuarioId))
                {
                    return Json(new { estado = false, mensajes = "Usuario no autenticado o ID de usuario inválido." });
                }

                // Llamar al servicio para guardar el pre-contrato con el nuevo contenido
                await _preContratoService.ActualizarContenidoPreContrato(request.SecPreContrato, request.Contenido, usuarioId);

                return Json(new { estado = true, mensajes = "Pre-contrato guardado exitosamente." });
            }
            catch (Exception ex)
            {
                return Json(new { estado = false, mensajes = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GenerarVistaPrevia([FromBody] GenerarVistaPreviaRequest request)
        {
            try
            {
                // Mapear el request a PreContratoGeneratorDTO
                var preContratoData = new BLL.DTOs.PreContratoGeneratorDTO
                {
                    SecCotizacion = request.SecCotizacion,
                    Dias = request.Dias,
                    TipoDias = request.TipoDias,
                    PeriodoMantenimiento = request.PeriodoMantenimiento,
                    AniosGarantia = request.AniosGarantia,
                    MesesGarantia = request.MesesGarantia,
                    PolizaGarantia = request.PolizaGarantia
                };

                string htmlPreview = await _preContratoGeneratorService.GenerarVistaPreviaHtml(preContratoData);

                return Json(new { estado = true, objeto = htmlPreview });
            }
            catch (Exception ex)
            {
                return Json(new { estado = false, mensajes = $"Error al generar la vista previa: {ex.Message}" });
            }
        }

        [HttpPost]
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

                var preContratoCreado = await _preContratoService.CrearDesdeModal(entidad, usuarioId, request.ContenidoHtml);
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
    }

    // ViewModel interno para la solicitud de guardado
    public class GuardarPreContratoRequest
    {
        public int SecPreContrato { get; set; }
        public string Contenido { get; set; }
    }
}