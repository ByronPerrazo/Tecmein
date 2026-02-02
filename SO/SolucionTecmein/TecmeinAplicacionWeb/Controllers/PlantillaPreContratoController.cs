using Microsoft.AspNetCore.Mvc;
using BLL.Interfaces;
using Entity;
using System.Threading.Tasks;
using System.Collections.Generic;
using TecmeinAplicacionWeb.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using AutoMapper;
using TecmeinWebApp.Utilidades.Response;
using TecmeinWebApp.Utilidades.ViewComponents;
using DAL.Interfaces; // Añadido para IGenericRepository

namespace TecmeinAplicacionWeb.Controllers
{
    public class PlantillaPreContratoController : Controller
    {
        private readonly IPlantillaPreContratoServices _plantillaPreContratoServices;
        private readonly IPlantillaPreContratoParrafoServices _plantillaPreContratoParrafoServices;
        private readonly ITipoDocumentoServices _tipoDocumentoServices;
        private readonly IPreContratoServices _preContratoServices;
        private readonly IGenericRepository<PreContratoCompromisoPago> _repositorioCompromisos; // Añadido
        private readonly IMapper _mapper;
        private readonly IGeneradorDocumentoService _generadorDocumentoService;

        public PlantillaPreContratoController(
            IPlantillaPreContratoServices plantillaPreContratoServices,
            IPlantillaPreContratoParrafoServices plantillaPreContratoParrafoServices,
            ITipoDocumentoServices tipoDocumentoServices,
            IPreContratoServices preContratoServices,
            IGenericRepository<PreContratoCompromisoPago> repositorioCompromisos, // Añadido
            IMapper mapper,
            IGeneradorDocumentoService generadorDocumentoService)
        {
            _plantillaPreContratoServices = plantillaPreContratoServices;
            _plantillaPreContratoParrafoServices = plantillaPreContratoParrafoServices;
            _tipoDocumentoServices = tipoDocumentoServices;
            _preContratoServices = preContratoServices;
            _repositorioCompromisos = repositorioCompromisos; // Añadido
            _mapper = mapper;
            _generadorDocumentoService = generadorDocumentoService;
        }

        [ValidatePermission("VER_MENU")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [ValidatePermission("LEER")]
        public async Task<JsonResult> Lista()
        {
            List<PlantillaPreContrato> listaEntidades = await _plantillaPreContratoServices.Lista();
            List<PlantillaPreContratoVM> listaVM = _mapper.Map<List<PlantillaPreContratoVM>>(listaEntidades);
            return Json(new { data = listaVM });
        }

        [HttpGet]
        [ValidatePermission("LEER")]
        public async Task<JsonResult> ListaTiposDocumento()
        {
            List<TipoDocumento> listaTipos = await _tipoDocumentoServices.Lista();
            List<SelectListItem> selectList = listaTipos.Select(td => new SelectListItem()
            {
                Text = td.Descripcion,
                Value = td.SecTipoDocumento.ToString()
            }).ToList();
            return Json(new { data = selectList });
        }

        [HttpPost]
        [ValidatePermission("CREAR")]
        public async Task<JsonResult> Crear([FromBody] PlantillaPreContrato entidad)
        {
            bool resultado = true;
            try
            {
                PlantillaPreContrato plantilla_creada = await _plantillaPreContratoServices.Crear(entidad);
                if (plantilla_creada.SecPlantillaPreContrato == 0)
                {
                    resultado = false;
                }
            }
            catch
            {
                resultado = false;
            }
            return Json(new { resultado = resultado });
        }

        [HttpPut]
        [ValidatePermission("ACTUALIZAR")]
        public async Task<JsonResult> Editar([FromBody] PlantillaPreContrato entidad)
        {
            bool resultado = true;
            try
            {
                PlantillaPreContrato plantilla_editada = await _plantillaPreContratoServices.Editar(entidad);
                if (plantilla_editada.SecPlantillaPreContrato == 0)
                {
                    resultado = false;
                }
            }
            catch
            {
                resultado = false;
            }
            return Json(new { resultado = resultado });
        }

        [HttpDelete]
        [ValidatePermission("ELIMINAR")]
        public async Task<JsonResult> Eliminar(int SecPlantillaPreContrato)
        {
            bool resultado = true;
            try
            {
                resultado = await _plantillaPreContratoServices.Eliminar(SecPlantillaPreContrato);
            }
            catch
            {
                resultado = false;
            }
            return Json(new { resultado = resultado });
        }

        [ValidatePermission("LEER")]
        public IActionResult Parrafos(int id)
        {
            ViewBag.IdPlantilla = id;
            return View();
        }

        [HttpGet]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> GenerarDocumento(int secPreContrato)
        {
            try
            {
                PreContrato preContrato = await _preContratoServices.Obtener(secPreContrato);
                if (preContrato == null)
                {
                    return NotFound($"PreContrato con ID {secPreContrato} no encontrado.");
                }

                // Obtener compromisos de pago
                var compromisos = (await _repositorioCompromisos.Consultar(c => c.SecPreContrato == secPreContrato))
                                                                 .OrderBy(c => c.NumeroCuota)
                                                                 .ToList();

                var datos = new
                {
                    // Datos básicos y de navegación
                    SecPreContrato = preContrato.SecPreContrato,
                    FechaRegistro = preContrato.FechaRegistro.ToShortDateString(),
                    NombreObra = preContrato.SecCotizacionNavigation?.SecVisitaNavigation?.Nombre,
                    NombreCliente = preContrato.SecCotizacionNavigation?.SecVisitaNavigation?.SecEmpresaNavigation?.Nombre,
                    NombreUsuario = preContrato.SecUsuarioCreaNavigation?.Nombre,

                    // Campos de negocio del PreContrato
                    DiasDeEntrega = $"{preContrato.Dias} {preContrato.TipoDias}",
                    AniosGarantia = preContrato.AniosGarantia,
                    MesesGarantia = preContrato.MesesGarantia,
                    PeriodoMantenimiento = preContrato.PeriodoMantenimiento,
                    PolizaGarantia = preContrato.PolizaGarantia,

                    // Lista para la tabla de pagos
                    CompromisosDePago = compromisos
                };

                string codigoTipoDocumento = preContrato.SecPlantillaPreContratoNavigation.SecTipoDocumentoNavigation.Codigo;
                byte[] documentBytes = await _generadorDocumentoService.GenerarDocumento(codigoTipoDocumento, preContrato.SecPlantillaPreContrato, datos);

                return File(documentBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"precontrato_{secPreContrato}.docx");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al generar documento: {ex.Message}");
                return StatusCode(500, $"Error al generar el documento: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CargarParrafosDesdeWord(IFormFile archivoWord, int secPlantillaPreContrato)
        {
            try
            {
                if (archivoWord == null || archivoWord.Length == 0)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { estado = false, mensajes = "No se ha subido ningún archivo o el archivo está vacío." });
                }

                if (secPlantillaPreContrato == 0)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { estado = false, mensajes = "ID de Plantilla no proporcionado." });
                }

                using (var stream = archivoWord.OpenReadStream())
                {
                    var resultado = await _plantillaPreContratoServices.CargarParrafosDesdeWordAsync(secPlantillaPreContrato, stream);

                    if (resultado.Exito)
                    {
                        return StatusCode(StatusCodes.Status200OK, new { estado = true, mensajes = "Párrafos cargados.", advertencias = resultado.Advertencias });
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, new { estado = false, mensajes = "No se pudieron cargar los párrafos desde el archivo Word." });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { estado = false, mensajes = $"Error al procesar el archivo Word: {ex.Message}" });
            }
        }


    }
}