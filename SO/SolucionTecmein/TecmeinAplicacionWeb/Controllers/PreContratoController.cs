using AutoMapper;
using BLL.DTOs;
using BLL.Interfaces;
using Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using TecmeinWebApp.Models;
using TecmeinWebApp.Models.ViewModel;
using TecmeinWebApp.Utilidades.Response;

namespace TecmeinWebApp.Controllers
{
    public class PreContratoController : Controller
    {
        private readonly IPreContratoServices _preContratoServices;
        private readonly ICotizacionServices _cotizacionServices;
        private readonly IPlantillaPreContratoServices _plantillaPreContratoServices;
        private readonly IFormaPagoServices _formaPagoServices;
        private readonly ISeguimientoServices _seguimientoServices;
        private readonly IPreContratoGeneratorService _preContratoGeneratorService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public PreContratoController(IPreContratoServices preContratoServices,
                                     ICotizacionServices cotizacionServices,
                                     IPlantillaPreContratoServices plantillaPreContratoServices,
                                     IFormaPagoServices formaPagoServices,
                                     ISeguimientoServices seguimientoServices,
                                     IPreContratoGeneratorService preContratoGeneratorService,
                                     IConfiguration configuration,
                                     IMapper mapper)
        {
            _preContratoServices = preContratoServices;
            _cotizacionServices = cotizacionServices;
            _plantillaPreContratoServices = plantillaPreContratoServices;
            _formaPagoServices = formaPagoServices;
            _seguimientoServices = seguimientoServices;
            _preContratoGeneratorService = preContratoGeneratorService;
            _configuration = configuration;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("PreContrato/Editor/{cotizacionId}")]
        public async Task<IActionResult> Editor(int cotizacionId)
        {
            try
            {
                ViewBag.TinyMceApiKey = _configuration["ApiKeys:TinyMCE"];
                string contenidoHtml;

                var preContrato = await _preContratoServices.ObtenerUltimaVersion(cotizacionId);

                if (preContrato == null)
                {
                    // CASO 1: No existe pre-contrato. Se genera la versión 1 a partir de una plantilla.
                    string userIdString = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
                    if (!int.TryParse(userIdString, out int secUsuario))
                    {
                        return Unauthorized("Sesión de usuario inválida.");
                    }

                    var primeraPlantilla = (await _plantillaPreContratoServices.Lista()).FirstOrDefault();
                    if (primeraPlantilla == null)
                    {
                        throw new Exception("No se encontraron plantillas para generar el pre-contrato inicial.");
                    }

                    var cotizacion = await _cotizacionServices.Detalle(cotizacionId);
                    var dto = new PreContratoGeneratorDTO
                    {
                        SecCotizacion = cotizacionId,
                        SecPlantillaPreContrato = primeraPlantilla.SecPlantillaPreContrato,
                        ValorContrato = (decimal)(cotizacion.Subtotal + cotizacion.ValorImpuestos)
                    };
                    contenidoHtml = await _preContratoGeneratorService.GenerarVistaPreviaHtml(dto);

                    // Se guarda esta primera versión generada.
                    preContrato = await _preContratoServices.GuardarDesdeEditor(cotizacionId, contenidoHtml, secUsuario);
                }
                else
                {
                    // CASO 2: El pre-contrato ya existe. Se carga el contenido HTML guardado en la BD.
                    var parrafo = await _preContratoServices.ObtenerPrimerParrafo(preContrato.SecPreContrato);
                    contenidoHtml = parrafo?.Contenido ?? string.Empty;
                }

                var modelo = _mapper.Map<PreContratoVM>(preContrato);
                ViewBag.Contenido = contenidoHtml;

                return View(modelo);
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            var lista = await _preContratoServices.Lista();
            var listaVM = _mapper.Map<List<PreContratoVM>>(lista);
            return Json(new { data = listaVM });
        }

        [HttpGet]
        public async Task<IActionResult> ListaCotizaciones()
        {
            var lista = await _cotizacionServices.Lista();
            var selectList = lista.Select(c => new SelectListItem
            {
                Value = c.Secuencial.ToString(),
                Text = c.SecVisitaNavigation != null ? c.SecVisitaNavigation.Nombre : c.Secuencial.ToString()
            }).ToList();
            return Json(new { data = selectList });
        }

        [HttpGet]
        public async Task<IActionResult> ListaCotizacionesAprobadas()
        {
            try
            {
                var cotizacionesAprobadas = await _seguimientoServices.ObtenerCotizacionesAprobadasSinPreContrato();
                var selectList = cotizacionesAprobadas.Select(c => new SelectListItem
                {
                    Value = c.Secuencial.ToString(),
                    Text = $"{c.Secuencial} - {c.SecVisitaNavigation?.Nombre ?? "N/A"}"
                }).ToList();
                return Json(new { data = selectList });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { estado = false, mensajes = ex.Message });
            }
        }


        [HttpGet]
        public async Task<IActionResult> ListaPlantillas()
        {
            var lista = await _plantillaPreContratoServices.Lista();
            var selectList = lista.Select(p => new SelectListItem
            {
                Value = p.SecPlantillaPreContrato.ToString(),
                Text = p.Nombre
            }).ToList();
            return Json(new { data = selectList });
        }

        [HttpGet]
        public async Task<IActionResult> ListaFormasPago()
        {
            var lista = await _formaPagoServices.Lista();
            var selectList = lista.Select(f => new SelectListItem
            {
                Value = f.SecFormaPago.ToString(),
                Text = f.Descripcion
            }).ToList();
            return Json(new { data = selectList });
        }

        [HttpGet]
        public async Task<IActionResult> Detalle(int secPreContrato)
        {
            var preContrato = await _preContratoServices.Obtener(secPreContrato);
            var vm = _mapper.Map<PreContratoVM>(preContrato);
            return Json(new { data = vm });
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] PreContratoVM modelo)
        {
            var entidad = _mapper.Map<PreContrato>(modelo);
            var resultado = await _preContratoServices.Crear(entidad);
            return Json(new { data = resultado });
        }

        [HttpPost]
        public async Task<IActionResult> CrearDesdeCotizacion(int cotizacionId)
        {
            try
            {
                string userIdString = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdString, out int secUsuario))
                {
                    return Unauthorized(new { estado = false, mensajes = "Sesión de usuario inválida." });
                }

                var preContrato = await _preContratoServices.CrearDesdeCotizacion(cotizacionId, secUsuario);
                var vm = _mapper.Map<PreContratoVM>(preContrato);
                return Ok(new { estado = true, objeto = vm });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { estado = false, mensajes = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromBody] PreContratoVM modelo)
        {
            var entidad = _mapper.Map<PreContrato>(modelo);
            var resultado = await _preContratoServices.Editar(entidad);
            return Json(new { data = resultado });
        }

        [HttpPost]
        public async Task<IActionResult> GenerarVistaPrevia([FromBody] PreContratoModalVM modelo)
        {
            var gResponse = new GenericResponse<string>();
            try
            {
                var dto = new PreContratoGeneratorDTO
                {
                    SecCotizacion = modelo.SecCotizacion,
                    SecFormaPago = modelo.SecFormaPago,
                    SecPlantillaPreContrato = modelo.SecPlantillaPreContrato,
                    ValorContrato = modelo.ValorContrato,
                    ValorAnticipo = modelo.ValorAnticipo,
                    FechaAnticipo = modelo.FechaAnticipo,
                    NumeroCuotas = modelo.NumeroCuotas,
                    FechaPrimeraCuota = modelo.FechaPrimeraCuota,
                    Dias = modelo.Dias,
                    TipoDias = modelo.TipoDias,
                    PeriodoMantenimiento = modelo.PeriodoMantenimiento,
                    AniosGarantia = modelo.AniosGarantia,
                    MesesGarantia = modelo.MesesGarantia,
                    PolizaGarantia = modelo.PolizaGarantia
                };

                string contenidoHtml = await _preContratoGeneratorService.GenerarVistaPreviaHtml(dto);
                gResponse.Estado = true;
                gResponse.Objeto = contenidoHtml;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
            }
            return Json(gResponse);
        }



        [HttpPost("PreContrato/ObtenerDatosParaPlaceholders")]
        public async Task<IActionResult> ObtenerDatosParaPlaceholders([FromBody] PreContratoModalVM modelo)
        {
            var gResponse = new GenericResponse<PlaceholderDataDTO>();
            try
            {
                var dto = new PreContratoGeneratorDTO
                {
                    SecCotizacion = modelo.SecCotizacion,
                    SecFormaPago = modelo.SecFormaPago,
                    SecPlantillaPreContrato = modelo.SecPlantillaPreContrato,
                    ValorContrato = modelo.ValorContrato,
                    ValorAnticipo = modelo.ValorAnticipo,
                    FechaAnticipo = modelo.FechaAnticipo,
                    NumeroCuotas = modelo.NumeroCuotas,
                    FechaPrimeraCuota = modelo.FechaPrimeraCuota,
                    Dias = modelo.Dias,
                    TipoDias = modelo.TipoDias,
                    PeriodoMantenimiento = modelo.PeriodoMantenimiento,
                    AniosGarantia = modelo.AniosGarantia,
                    MesesGarantia = modelo.MesesGarantia,
                    PolizaGarantia = modelo.PolizaGarantia
                };

                var placeholderData = await _preContratoGeneratorService.ObtenerDatosParaPlaceholders(dto);
                gResponse.Estado = true;
                gResponse.Objeto = placeholderData;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
            }
            return Json(gResponse);
        }

        [HttpPost]
        public async Task<IActionResult> CrearDesdeModal([FromBody] PreContratoModalVM modelo)
        {
            var gResponse = new GenericResponse<PreContratoVM>();
            try
            {
                string userIdString = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdString, out int secUsuario))
                {
                    return Unauthorized(new { estado = false, mensajes = "Sesión de usuario inválida." });
                }

                var entidad = new PreContrato
                {
                    SecCotizacion = modelo.SecCotizacion,
                    SecFormaPago = modelo.SecFormaPago,
                    SecPlantillaPreContrato = modelo.SecPlantillaPreContrato,
                    ValorContrato = modelo.ValorContrato,
                    ValorAnticipo = modelo.ValorAnticipo,
                    FechaAnticipo = modelo.FechaAnticipo,
                    NumeroCuotas = modelo.NumeroCuotas,
                    FechaPrimeraCuota = modelo.FechaPrimeraCuota,
                    Dias = modelo.Dias,
                    TipoDias = modelo.TipoDias,
                    PeriodoMantenimiento = modelo.PeriodoMantenimiento,
                    AniosGarantia = modelo.AniosGarantia,
                    MesesGarantia = modelo.MesesGarantia,
                    PolizaGarantia = modelo.PolizaGarantia
                };
                var preContratoCreado = await _preContratoServices.CrearDesdeModal(entidad, secUsuario);
                var preContratoVM = _mapper.Map<PreContratoVM>(preContratoCreado);

                gResponse.Estado = true;
                gResponse.Objeto = preContratoVM;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
            }
            return Json(gResponse);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarPreContrato([FromBody] GuardarPreContratoVM modelo)
        {
            try
            {
                string userIdString = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdString, out int secUsuario))
                {
                    return Unauthorized(new { estado = false, mensajes = "Sesión de usuario inválida." });
                }

                // Llamada real al nuevo método del servicio
                var preContratoGuardado = await _preContratoServices.GuardarDesdeEditor(modelo.CotizacionId, modelo.Contenido, secUsuario);

                return Ok(new { estado = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { estado = false, mensajes = ex.Message });
            }
        }

        [HttpGet("PreContrato/Historial/{id}")]
        public async Task<IActionResult> Historial(int id)
        {
            try
            {
                var historial = await _preContratoServices.ObtenerHistorial(id);
                var historialVM = _mapper.Map<List<PreContratoVM>>(historial);
                return Ok(new { data = historialVM });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { data = new List<object>(), mensajes = ex.Message });
            }
        }

        [HttpGet("PreContrato/ContenidoParrafo/{id}")]
        public async Task<IActionResult> ContenidoParrafo(int id)
        {
            try
            {
                var parrafo = await _preContratoServices.ObtenerPrimerParrafo(id);
                if (parrafo == null)
                {
                    return NotFound(new { estado = false, mensajes = "No se encontró contenido para esta versión." });
                }
                return Ok(new { estado = true, objeto = new { contenido = parrafo.Contenido } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { estado = false, mensajes = ex.Message });
            }
        }

        [HttpGet("PreContrato/PrevisualizarContenido/{id}")]
        public async Task<IActionResult> PrevisualizarContenido(int id)
        {
            try
            {
                // Cambiado para obtener el contenido guardado, no el regenerado.
                var parrafo = await _preContratoServices.ObtenerPrimerParrafo(id);
                var contenido = parrafo?.Contenido ?? string.Empty;
                return Ok(new { estado = true, objeto = new { contenido = contenido } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { estado = false, mensajes = ex.Message });
            }
        }

        [HttpDelete("PreContrato/Eliminar/{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var gResponse = new GenericResponse<bool>();
            try
            {
                bool resultado = await _preContratoServices.Eliminar(id);
                gResponse.Estado = resultado;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
            }
            return Json(gResponse);
        }
    }
}



