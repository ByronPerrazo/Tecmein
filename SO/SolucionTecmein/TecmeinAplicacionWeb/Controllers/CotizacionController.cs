using AutoMapper;
using BLL.Interfaces;
using Entity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;
using TecmeinAplicacionWeb.Models.ViewModels;
using TecmeinWebApp.Utilidades.Response;
using TecmeinWebApp.Utilidades.ViewComponents;
using BLL.DTOs;

namespace TecmeinWebApp.Controllers
{
    public class CotizacionController : Controller
    {
        private readonly ICotizacionServices _cotizacionServices;
        private readonly IMapper _mapper;
        private readonly IAuditService _auditService;
        private readonly IUsuarioServices _usuarioServices;

        public CotizacionController(ICotizacionServices cotizacionServices, IMapper mapper, IAuditService auditService, IUsuarioServices usuarioServices)
        {
            _cotizacionServices = cotizacionServices;
            _mapper = mapper;
            _auditService = auditService;
            _usuarioServices = usuarioServices;
        }

        [ValidatePermission("LEER")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> Lista()
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
            int idUsuario = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;

            var lista = await _cotizacionServices.Lista(idUsuario);
            var listaVM = _mapper.Map<List<CotizacionVM>>(lista);

            return StatusCode(StatusCodes.Status200OK, new { data = listaVM });
        }

        [HttpGet]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> Detalle(int id)
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
            int idUsuario = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;

            var cotizacion = await _cotizacionServices.Detalle(id, idUsuario);
            var cotizacionVM = _mapper.Map<CotizacionVM>(cotizacion);
            return StatusCode(StatusCodes.Status200OK, cotizacionVM);
        }

        [HttpGet]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> VerificarVisita(int visitaId)
        {
            bool tieneCotizacion = await _cotizacionServices.VisitaTieneCotizacionActiva(visitaId);
            return StatusCode(StatusCodes.Status200OK, new { valor = tieneCotizacion });
        }

        [HttpGet]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> GenerarPDF(int idCotizacion)
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
            int idUsuario = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;

            byte[] pdfBytes = await _cotizacionServices.GenerarPdfCotizacion(idCotizacion, idUsuario);
            string fileName = $"Cotizacion_{idCotizacion}.pdf";

            return File(pdfBytes, "application/pdf", fileName);
        }

        [HttpGet]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> GenerarPDFSolicitud(int idCotizacion)
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
            int idUsuario = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;

            byte[] pdfBytes = await _cotizacionServices.GenerarPdfSolicitudEquipos(idCotizacion, idUsuario);
            string fileName = $"Solicitud_Equipos_{idCotizacion}.pdf";

            return File(pdfBytes, "application/pdf", fileName);
        }

        [HttpPost]
        [ValidatePermission("CREAR")]
        public async Task<IActionResult> Crear([FromForm] string modelo)
        {
            var response = new GenericResponse<CotizacionVM>();
            try
            {
                var cotizacionVM = JsonConvert.DeserializeObject<CotizacionVM>(modelo);

                var claims = HttpContext.User.Claims;
                var userIdClaim = claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    response.Estado = false;
                    response.Mensajes = "No se pudo obtener el usuario para la creación de la cotización.";
                    return StatusCode(StatusCodes.Status401Unauthorized, response);
                }

                int idUsuario = int.Parse(userIdClaim.Value);

                var cotizacionDTO = _mapper.Map<CotizacionDTO>(cotizacionVM);
                var cotizacionCreada = await _cotizacionServices.Crear(cotizacionDTO, idUsuario);
                var cotizacionCreadaVM = _mapper.Map<CotizacionVM>(cotizacionCreada);

                response.Estado = true;
                response.Objeto = cotizacionCreadaVM;
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensajes = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpPost]
        [ValidatePermission("ACTUALIZAR")]
        public async Task<IActionResult> Editar([FromForm] string modelo)
        {
            var response = new GenericResponse<CotizacionVM>();
            try
            {
                var cotizacionVM = JsonConvert.DeserializeObject<CotizacionVM>(modelo);

                var claims = HttpContext.User.Claims;
                var userIdClaim = claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    response.Estado = false;
                    response.Mensajes = "No se pudo obtener el usuario para la edición de la cotización.";
                    return StatusCode(StatusCodes.Status401Unauthorized, response);
                }

                int idUsuario = int.Parse(userIdClaim.Value);

                var cotizacionDTO = _mapper.Map<CotizacionDTO>(cotizacionVM);
                var cotizacionEditada = await _cotizacionServices.Editar(cotizacionDTO, idUsuario);
                var cotizacionEditadaVM = _mapper.Map<CotizacionVM>(cotizacionEditada);

                response.Estado = true;
                response.Objeto = cotizacionEditadaVM;
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensajes = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpDelete]
        [ValidatePermission("ELIMINAR")]
        public async Task<IActionResult> Eliminar(int secuencial)
        {
            var response = new GenericResponse<string>();
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
            int idUsuario = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;

            response.Estado = await _cotizacionServices.Eliminar(secuencial, idUsuario);
            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpGet]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> HistorialCambios(int idCotizacion)
        {
            // This assumes a method like GetEventsByPrefixAsync exists in IAuditService
            // and that the event type is stored with a prefix like "COTIZACION_123_"
            var eventPrefix = $"COTIZACION_{idCotizacion}";
            var eventos = await _auditService.GetEventsByPrefixAsync(eventPrefix);

            // Further filter for detail-related changes if the service returns broad results
            var eventosDetalle = eventos.Where(e => e.TipoEvento.Contains("_DETALLE_")).ToList();

            var eventosVM = _mapper.Map<List<AuditoriaEventoVM>>(eventosDetalle);

            return StatusCode(StatusCodes.Status200OK, new { data = eventosVM });
        }


    }
}

