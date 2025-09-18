using AutoMapper;
using BLL.Interfaces;
using Entity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;
using TecmeinWebApp.Models.ViewModel;
using TecmeinWebApp.Utilidades.Response;

namespace TecmeinWebApp.Controllers
{
    public class CotizacionController : Controller
    {
        private readonly ICotizacionServices _cotizacionServices;
        private readonly IMapper _mapper;

        public CotizacionController(ICotizacionServices cotizacionServices, IMapper mapper)
        {
            _cotizacionServices = cotizacionServices;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            var lista = await _cotizacionServices.Lista();
            var listaVM = _mapper.Map<List<CotizacionVM>>(lista);

            foreach (var cotizacionVM in listaVM)
            {
                var cotizacionOriginal = lista.FirstOrDefault(c => c.Secuencial == cotizacionVM.Secuencial);
                if (cotizacionOriginal != null && cotizacionOriginal.SecVisitaNavigation != null && cotizacionOriginal.SecVisitaNavigation.Contactovisita.Any())
                {
                    var contactoPrincipal = cotizacionOriginal.SecVisitaNavigation.Contactovisita
                        .FirstOrDefault(cv => cv.EstaActivo == 1 && cv.SecContactoNavigation != null)?.SecContactoNavigation;

                    cotizacionVM.NombreContacto = contactoPrincipal != null ? $"{contactoPrincipal.Nombres} {contactoPrincipal.Apellidos}" : "Sin Contacto";
                }
                else
                {
                    cotizacionVM.NombreContacto = "Sin Contacto";
                }
            }

            return StatusCode(StatusCodes.Status200OK, new { data = listaVM });
        }

        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            var cotizacion = await _cotizacionServices.Detalle(id);
            var cotizacionVM = _mapper.Map<CotizacionVM>(cotizacion);
            return StatusCode(StatusCodes.Status200OK, cotizacionVM);
        }

        [HttpGet]
        public async Task<IActionResult> VerificarVisita(int visitaId)
        {
            bool tieneCotizacion = await _cotizacionServices.VisitaTieneCotizacionActiva(visitaId);
            return StatusCode(StatusCodes.Status200OK, new { valor = tieneCotizacion });
        }

        [HttpGet]
        public async Task<IActionResult> GenerarPDF(int idCotizacion)
        {
            try
            {
                byte[] pdfBytes = await _cotizacionServices.GenerarPdfCotizacion(idCotizacion);
                string fileName = $"Cotizacion_{idCotizacion}.pdf";

                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocurrió un error al generar el PDF. {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GenerarPDFSolicitud(int idCotizacion)
        {
            try
            {
                byte[] pdfBytes = await _cotizacionServices.GenerarPdfSolicitudEquipos(idCotizacion);
                string fileName = $"Solicitud_Equipos_{idCotizacion}.pdf";

                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocurrió un error al generar el PDF de solicitud. {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] string modelo)
        {
            var response = new GenericResponse<CotizacionVM>();
            try
            {
                var cotizacionVM = JsonConvert.DeserializeObject<CotizacionVM>(modelo);
                var cotizacion = _mapper.Map<Cotizacion>(cotizacionVM);

                var claims = HttpContext.User.Claims;
                var userIdClaim = claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    response.Estado = false;
                    response.Mensajes = "No se pudo obtener el usuario para la creación de la cotización.";
                    return StatusCode(StatusCodes.Status401Unauthorized, response);
                }

                int idUsuario = int.Parse(userIdClaim.Value);

                var cotizacionCreada = await _cotizacionServices.Crear(cotizacion, idUsuario);
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

        [HttpPut]
        public async Task<IActionResult> Editar([FromForm] string modelo)
        {
            var response = new GenericResponse<CotizacionVM>();
            try
            {
                var cotizacionVM = JsonConvert.DeserializeObject<CotizacionVM>(modelo);
                var cotizacion = _mapper.Map<Cotizacion>(cotizacionVM);

                var claims = HttpContext.User.Claims;
                var userIdClaim = claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    response.Estado = false;
                    response.Mensajes = "No se pudo obtener el usuario para la edición de la cotización.";
                    return StatusCode(StatusCodes.Status401Unauthorized, response);
                }

                int idUsuario = int.Parse(userIdClaim.Value);

                var cotizacionEditada = await _cotizacionServices.Editar(cotizacion, idUsuario);
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
        public async Task<IActionResult> Eliminar(int id)
        {
            var response = new GenericResponse<string>();
            try
            {
                response.Estado = await _cotizacionServices.Eliminar(id);
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensajes = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, response);
        }

                
    }
}

