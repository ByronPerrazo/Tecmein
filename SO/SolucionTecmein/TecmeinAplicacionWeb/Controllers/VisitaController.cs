using AutoMapper;
using BLL.Interfaces;
using Entity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using TecmeinAplicacionWeb.Models.ViewModels;
using TecmeinWebApp.Utilidades.Response;
using TecmeinWebApp.Utilidades.ViewComponents;

namespace TecmeinWebApp.Controllers
{
    public class VisitaController : Controller
    {
        private readonly IVisitaServices _visitaServices;
        private readonly IEmpresaServices _empresaServices;
        private readonly IProvinciaServices _provinciaServices;
        private readonly ICantonServices _cantonServices;
        private readonly IParroquiaServices _parroquiaServices;
        private readonly IConstructoraServices _constructoraServices;
        private readonly IContactoServices _contactoServices;
        private readonly IContactoVisitaServices _contactoVistaServices;
        private readonly IEquiposVisitaServices _equiposVisitaServices;
        private readonly IEtapaServices _etapaServices;
        private readonly ILogger<VisitaController> _logger;
        private readonly IPermisoServices _permisoServices; // New field
        private readonly IRolServices _rolServices; // New field

        private readonly IMapper _mapper;
        public VisitaController(IVisitaServices visitaServices,
                                IEmpresaServices empresaServices,
                                IProvinciaServices provinciaServices,
                                ICantonServices cantonServices,
                                IParroquiaServices parroquiaServices,
                                IMapper mapper,
                                IConstructoraServices constructoraServices,
                                IContactoServices contactoServices,
                                IContactoVisitaServices contactoVistaServices,
                                IEquiposVisitaServices equiposVisitaServices,
                                IEtapaServices etapaServices,
                                ILogger<VisitaController> logger,
                                IPermisoServices permisoServices, // New parameter
                                IRolServices rolServices // New parameter
            )
        {
            _visitaServices = visitaServices;
            _empresaServices = empresaServices;
            _provinciaServices = provinciaServices;
            _cantonServices = cantonServices;
            _parroquiaServices = parroquiaServices;
            _mapper = mapper;
            _constructoraServices = constructoraServices;
            _contactoServices = contactoServices;
            _contactoVistaServices = contactoVistaServices;
            _equiposVisitaServices = equiposVisitaServices;
            _etapaServices = etapaServices;
            _logger = logger;
            _permisoServices = permisoServices; // Assign it
            _rolServices = rolServices; // Assign it
        }

        [HttpGet]
        public async Task<IActionResult> Etapas()
        {
            var listaEtapasVM = _mapper.Map<List<EtapaVM>>(await _etapaServices.Lista());
            return StatusCode(StatusCodes.Status200OK, listaEtapasVM);
        }

        [HttpGet]
        public async Task<IActionResult> Contactos()
        {
            var listaContactoVM
                = _mapper.Map<List<ContactoVM>>(await _contactoServices.Lista());
            return StatusCode(StatusCodes.Status200OK, listaContactoVM);
        }

        [HttpGet]
        public async Task<IActionResult> EmpresaConstructora()
        {
            var listaConstructorasVM
                   = _mapper.Map<List<ConstructoraVM>>(await _constructoraServices.Lista());
            return StatusCode(StatusCodes.Status200OK, listaConstructorasVM);
        }

        [HttpGet]
        public async Task<IActionResult> Operadores()
        {
            var listaOperadoresVM
                = _mapper.Map<List<EmpresaVM>>(await _empresaServices.Lista());
            return StatusCode(StatusCodes.Status200OK, listaOperadoresVM);
        }

        [HttpGet]
        public async Task<IActionResult> Provincias()
        {
            var listaProvinciasVM
                = _mapper.Map<List<ProvinciaVM>>(await _provinciaServices.Lista());
            return StatusCode(StatusCodes.Status200OK, listaProvinciasVM);
        }

        [HttpGet]
        public async Task<IActionResult> Cantones()
        {
            var listaCantonesVM
                = _mapper.Map<List<CantonVM>>(await _cantonServices.Lista());
            return StatusCode(StatusCodes.Status200OK, listaCantonesVM);
        }

        [HttpGet]
        public async Task<IActionResult> Parroquias()
        {
            var listaParroquiaVM
                = _mapper.Map<List<ParroquiaVM>>(await _parroquiaServices.Lista());
            return StatusCode(StatusCodes.Status200OK, listaParroquiaVM);
        }

        [HttpGet]
        public async Task<IActionResult> ListaParaCotizacion()
        {
            var listaVisitaVM
                = _mapper.Map<List<VisitaVM>>(await _visitaServices.ListaConEquipos());
            return StatusCode(StatusCodes.Status200OK, new { data = listaVisitaVM });
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<Visita> lista = await _visitaServices.ListaVisitas();

            var listaVisitaVM = _mapper.Map<List<VisitaVM>>(lista);

            foreach (var visitaVM in listaVisitaVM)
            {
                var visitaOriginal = lista.FirstOrDefault(v => v.Secuencial == visitaVM.Secuencial);
                if (visitaOriginal != null)
                {
                    visitaVM.DescripcionEtapa = visitaOriginal.IdEtapaNavigation?.Descripcion;
                    visitaVM.NombreEmpresa = visitaOriginal.SecEmpresaNavigation?.Nombre;
                }
            }

            var jsonResult = JsonConvert.SerializeObject(new { data = listaVisitaVM });
            return Content(jsonResult, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> GetClaims() // Make it async
        {
            ClaimsPrincipal claimsUser = HttpContext.User;
            string idUsuario = claimsUser.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault();
            string rolName = claimsUser.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).SingleOrDefault();

            List<string> permissionNames = new List<string>();
            if (!string.IsNullOrEmpty(rolName))
            {
                // Get all roles and find the one matching the rolName
                List<Rol> allRoles = await _rolServices.Lista();
                Rol? userRole = allRoles.FirstOrDefault(r => r.Descripcion.ToLower() == rolName.ToLower());

                if (userRole != null)
                {
                    List<Permiso> permisos = await _permisoServices.ObtenerPermisosPorRol(userRole.Secuencial);
                    permissionNames = permisos.Select(p => p.IdPermiso).ToList(); // Assuming IdPermiso is the permission name
                }
            }

            return StatusCode(StatusCodes.Status200OK, new { idUsuario, rol = rolName, permisos = permissionNames });
        }

        [HttpPost]
        [ValidatePermission("ACTUALIZAR")]
        public async Task<IActionResult> CambiarEtapa([FromForm] int secVisita, [FromForm] string nuevoCodigoEtapa)
        {
            var gResponse = new GenericResponse<string>();
            try
            {
                gResponse.Estado = await _visitaServices.CambiarEtapa(secVisita, nuevoCodigoEtapa);
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPost]
        [ValidatePermission("CREAR")]
        public async Task<IActionResult> CrearVisita([FromForm] string modelo)
        {
            var genericResponse = new GenericResponse<VisitaVM>();
            try
            {
                VisitaVM? visitaIngresadaVM
                    = JsonConvert
                      .DeserializeObject<VisitaVM>(modelo);

                if (visitaIngresadaVM.FechaSiguienteVisita == DateTime.MinValue)
                {
                    visitaIngresadaVM.FechaSiguienteVisita = null;
                }

                _logger.LogInformation("Creando visita: {@Visita}", visitaIngresadaVM);

                ClaimsPrincipal claimsUser = HttpContext.User;
                string? secUsuario
                       = claimsUser.Claims
                                   .Where(x => x.Type == ClaimTypes.NameIdentifier)
                                   .Select(x => x.Value)
                                   .SingleOrDefault();

                visitaIngresadaVM.SecUsuario = ObtieneSecuencialUsuario();
                visitaIngresadaVM.IdEtapa = 1;
                visitaIngresadaVM.EstaActivo = 1;

                var visitaGenerada
                    = await _visitaServices
                            .CreaVisita(_mapper.Map<Visita>(visitaIngresadaVM));

                visitaIngresadaVM = _mapper.Map<VisitaVM>(visitaGenerada);

                genericResponse.Estado = true;
                genericResponse.Objeto = visitaIngresadaVM;
            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensajes = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }

        [HttpPost]
        [ValidatePermission("ACTUALIZAR")]
        public async Task<IActionResult> EditarVisita([FromForm] string modelo)
        {
            var genericResponse = new GenericResponse<VisitaVM>();
            try
            {
                VisitaVM? visitaVM = JsonConvert.DeserializeObject<VisitaVM>(modelo);

                if (visitaVM.FechaSiguienteVisita == DateTime.MinValue)
                {
                    visitaVM.FechaSiguienteVisita = null;
                }

                var visitaObtenida
                    = await _visitaServices
                            .EditaVisita(_mapper.Map<Visita>(visitaVM));

                visitaVM = _mapper.Map<VisitaVM>(visitaObtenida);

                genericResponse.Estado = true;
                genericResponse.Objeto = visitaVM;
            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensajes = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }


        [HttpPost]
        [ValidatePermission("ACTUALIZAR")]
        public async Task<IActionResult> ProcesoGuardasContactoVisita([FromForm] string modelo)
        {
            var genericResponse = new GenericResponse<ContactoVisitaVM>();
            try
            {
                ContactoVisitaVM? visitaIngresadaVM
                    = JsonConvert
                      .DeserializeObject<ContactoVisitaVM>(modelo);

                if (visitaIngresadaVM != null && visitaIngresadaVM.SecContacto != 0)
                {
                    var visitaGenerada
                        = await _contactoVistaServices
                                .ProcesaGuardarContactoVisita(_mapper.Map<Contactovisita>(visitaIngresadaVM));

                    visitaIngresadaVM = _mapper.Map<ContactoVisitaVM>(visitaGenerada);

                    genericResponse.Estado = true;
                    genericResponse.Objeto = visitaIngresadaVM;

                }
            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensajes = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }

        [HttpDelete]
        [ValidatePermission("ELIMINAR")]
        public async Task<IActionResult> Eliminar(int secuencial)
        {
            var gResponse = new GenericResponse<string>();
            try
            {
                var visitaContacto = await _contactoVistaServices.ContactoVisitaPorVisita(secuencial);

                if (visitaContacto != null)
                    await _contactoVistaServices.EliminarContactoVisita(visitaContacto.Secuencial);

                gResponse.Estado = await _visitaServices.Eliminar(secuencial);
            }
            catch (InvalidOperationException ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = "No se pudo eliminar la visita."; // Generic message for other errors
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpGet]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> VisitaContacto(int secuencialVisita)
        {
            var contactoVisitaVM
                = _mapper.Map<ContactoVisitaVM>(await _contactoVistaServices.ContactoVisitaPorVisita(secuencialVisita));
            return StatusCode(StatusCodes.Status200OK, new { data = contactoVisitaVM });
        }
        public int ObtieneSecuencialUsuario()
        {

            ClaimsPrincipal claimsUser = HttpContext.User;
            string? secUsuario
                   = claimsUser.Claims
                               .Where(x => x.Type == ClaimTypes.NameIdentifier)
                               .Select(x => x.Value)
                               .SingleOrDefault();

            return (int)(string.IsNullOrEmpty(secUsuario) ? 0 : Convert.ToUInt32(secUsuario));

        }

        [HttpGet]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> EquiposDeVisita(int secuencialVisita)
        {
            var resultadoConsulta = await _equiposVisitaServices.ConsultaListaPorVisita(secuencialVisita);
            var listaEquipoVistaVM = _mapper.Map<List<EquiposVisitaVM>>(resultadoConsulta.Equipos);

            return StatusCode(StatusCodes.Status200OK, new { data = listaEquipoVistaVM, cotizacionActivaExiste = resultadoConsulta.CotizacionActivaExiste });
        }

        [HttpPost]
        [ValidatePermission("ACTUALIZAR")]
        public async Task<IActionResult> ProcesoGuardasEquipoVisita([FromForm] string modelo)
        {
            var genericResponse = new GenericResponse<EquiposVisitaVM>();
            try
            {
                EquiposVisitaVM? equipoVisitaIngresadaVM
                    = JsonConvert
                      .DeserializeObject<EquiposVisitaVM>(modelo);

                if (equipoVisitaIngresadaVM != null && equipoVisitaIngresadaVM.SecVisita != 0)
                {
                    var equipoGenerado
                        = await _equiposVisitaServices
                                .ProcesaGuardar(_mapper.Map<Equiposvisita>(equipoVisitaIngresadaVM));


                    equipoVisitaIngresadaVM = _mapper.Map<EquiposVisitaVM>(equipoGenerado);

                    genericResponse.Estado = true;
                    genericResponse.Objeto = equipoVisitaIngresadaVM;

                }
            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensajes = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }

        [HttpDelete]
        [ValidatePermission("Eliminar")]
        public async Task<IActionResult> ProcesoEliminarEquipoVisita(int secuencialEquipoVisita)
        {

            var gResponse = new GenericResponse<string>();
            try
            {
                var equipoVisita = await _equiposVisitaServices.Obtener(secuencialEquipoVisita);
                var respuesta = false;

                if (equipoVisita != null)
                    respuesta = await _equiposVisitaServices.ProcesaEliminar(equipoVisita);

                gResponse.Estado = respuesta;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
                throw;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> ObtenerDetalleVisita(int secuencialVisita)
        {
            var response = new GenericResponse<VisitaDetalleVM>();
            try
            {
                var visita = await _visitaServices.ObtenerDetalleVisita(secuencialVisita);
                if (visita == null)
                {
                    response.Estado = false;
                    response.Mensajes = "Visita no encontrada.";
                    return StatusCode(StatusCodes.Status404NotFound, response);
                }

                var visitaDetalleVM = new VisitaDetalleVM
                {
                    Secuencial = visita.Secuencial,
                    Nombre = visita.Nombre,
                    Direccion = visita.Direccion,
                    NombreProvincia = visita.SecProvinciaNavigation?.Nombre,
                    NombreCanton = visita.SecCantonNavigation?.Nombre,
                    NombreParroquia = visita.SecParroquiaNavigation?.Nombre,
                    NombreUsuario = visita.SecUsuarioNavigation?.Nombre,
                    // Assuming only one Contactovisita and Contacto is relevant for display
                    NombreConstructora = visita.Contactovisita?.FirstOrDefault()?.SecContactoNavigation?.SecConstructoraNavigation?.Nombre,
                    NombreContacto = visita.Contactovisita?.FirstOrDefault()?.SecContactoNavigation?.Nombres,
                    CorreoContacto = visita.Contactovisita?.FirstOrDefault()?.SecContactoNavigation?.Correo,
                    TelefonoContacto = visita.Contactovisita?.FirstOrDefault()?.SecContactoNavigation?.Telefono
                };

                response.Estado = true;
                response.Objeto = visitaDetalleVM;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener detalle de visita.");
                response.Estado = false;
                response.Mensajes = "Error interno del servidor al obtener detalle de visita.";
            }
            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpPost]
        [ValidatePermission("ACTUALIZAR")]
        public async Task<IActionResult> SincronizarEquipos([FromQuery] int secuencialVisita)
        {
            var gResponse = new GenericResponse<bool>();
            try
            {
                gResponse.Estado = await _equiposVisitaServices.SincronizarEquiposConCotizacionActiva(secuencialVisita);
                gResponse.Mensajes = gResponse.Estado ? "Equipos sincronizados exitosamente." : "No se encontró una cotización activa para sincronizar.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al sincronizar equipos con cotización activa.");
                gResponse.Estado = false;
                gResponse.Mensajes = $"Error al sincronizar equipos: {ex.Message}";
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }
    }

}