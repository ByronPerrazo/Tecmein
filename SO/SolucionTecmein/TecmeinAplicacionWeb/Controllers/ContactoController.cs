using AutoMapper;
using BLL.Interfaces;
using Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using TecmeinAplicacionWeb.Models.ViewModels;
using TecmeinWebApp.Utilidades.Response;

namespace TecmeinWebApp.Controllers
{
    [Authorize] // Requerir que el usuario esté autenticado para todo el controlador
    public class ContactoController : Controller
    {
        private readonly IContactoServices _contactoServices;
        private readonly IConstructoraServices _constructoraServices;
        private readonly IMapper _mapper;
        private readonly ILogger<ContactoController> _logger;

        public ContactoController(IMapper mapper,
                                  IContactoServices contactoServices,
                                  IConstructoraServices constructoraServices,
                                  ILogger<ContactoController> logger)
        {

            _mapper = mapper;
            _contactoServices = contactoServices;
            _constructoraServices = constructoraServices;
            _logger = logger;
        }

        // La página principal del módulo de Contactos
        // Se puede ver si se tiene acceso al menú de contactos.
        [Authorize(Policy = "Menu.Ver.13")]
        public IActionResult Index()
        {
            return View();
        }

        // La lista de datos para la tabla
        // Se puede ver si se tiene el permiso específico para ver contactos.
        [Authorize(Policy = "Contacto.Ver")]
        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            var listaContactosVM
               = _mapper.Map<List<ContactoVM>>(await _contactoServices.Lista());
            return StatusCode(StatusCodes.Status200OK, new { data = listaContactosVM });
        }

        [HttpGet]
        public async Task<IActionResult> EmpresaConstructora()
        {
            var listaConstructorasVM
                   = _mapper.Map<List<ConstructoraVM>>(await _constructoraServices.Lista());

            var jsonOptions = new System.Text.Json.JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            };
            return new JsonResult(listaConstructorasVM, jsonOptions);
        }

        [Authorize(Policy = "Contacto.Editar")]
        [HttpGet]
        public async Task<IActionResult> ObtenerParaEditar(int secuencial)
        {
            var gResponse = new GenericResponse<ContactoEditarVM>();
            try
            {
                var contacto = await _contactoServices.ObtenerPorId(secuencial);
                var constructoras = await _constructoraServices.Lista();

                var viewModel = new ContactoEditarVM
                {
                    Contacto = _mapper.Map<ContactoVM>(contacto),
                    ListaConstructoras = _mapper.Map<List<ConstructoraVM>>(constructoras)
                };

                gResponse.Estado = true;
                gResponse.Objeto = viewModel;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
            }

            var jsonOptions = new System.Text.Json.JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            };
            return new JsonResult(gResponse, jsonOptions);
        }

        [Authorize(Policy = "Contacto.Crear")]
        [HttpPost]
        public async Task<IActionResult> CrearContacto([FromForm] string modelo)
        {
            var genericResponse = new GenericResponse<ContactoVM>();
            try
            {
                ContactoVM? contactoIngresadoVM
                    = JsonConvert
                      .DeserializeObject<ContactoVM>(modelo);

                ClaimsPrincipal claimsUser = HttpContext.User;
                string? secUsuario
                       = claimsUser.Claims
                                   .Where(x => x.Type == ClaimTypes.NameIdentifier)
                                   .Select(x => x.Value)
                                   .SingleOrDefault();

                var contactoGenerado
                    = await _contactoServices
                            .Crear(_mapper.Map<Contacto>(contactoIngresadoVM));

                contactoIngresadoVM = _mapper.Map<ContactoVM>(contactoGenerado);

                genericResponse.Estado = true;
                genericResponse.Objeto = contactoIngresadoVM;
            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensajes = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }

        [Authorize(Policy = "Contacto.Editar")]
        [HttpPost]
        public async Task<IActionResult> Editar([FromForm] string modelo, [FromForm] string modeloVisitaDetalle)
        {
            var genericResponse = new GenericResponse<ContactoVM>();
            try
            {
                ContactoVM? contactoVM = JsonConvert.DeserializeObject<ContactoVM>(modelo);

                var visitaObtenida
                    = await _contactoServices
                            .Editar(_mapper.Map<Contacto>(contactoVM));

                contactoVM = _mapper.Map<ContactoVM>(visitaObtenida);

                genericResponse.Estado = true;
                genericResponse.Objeto = contactoVM;
            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensajes = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }

        [Authorize(Policy = "Contacto.Eliminar")]
        [HttpDelete]
        public async Task<IActionResult> Eliminar(int secuencial)
        {
            var gResponse = new GenericResponse<string>();
            try
            {
                gResponse.Estado = await _contactoServices.Eliminar(secuencial);
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
                throw;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerContactoPrincipal(int secuencialVisita)
        {
            GenericResponse<ContactoVM> gResponse = new GenericResponse<ContactoVM>();
            try
            {
                ContactoVM vmContacto = _mapper.Map<ContactoVM>(await _contactoServices.ObtenerContactoPrincipal(secuencialVisita));
                _logger.LogInformation($"Contacto Principal obtenido: {Newtonsoft.Json.JsonConvert.SerializeObject(vmContacto)}");
                gResponse.Estado = true;
                gResponse.Objeto = vmContacto;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }
    }
}