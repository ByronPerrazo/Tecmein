using AutoMapper;
using BLL.Interfaces;
using Entity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using TecmeinWebApp.Models.ViewModel;
using TecmeinWebApp.Utilidades.Response;

namespace TecmeinWebApp.Controllers
{
    public class ContactoController : Controller
    {
        private readonly IContactoServices _contactoServices;
        private readonly IConstructoraServices _constructoraServices;
        private readonly IMapper _mapper;

        public ContactoController(IMapper mapper,
                                  IContactoServices contactoServices,
                                  IConstructoraServices constructoraServices)
        {

            _mapper = mapper;
            _contactoServices = contactoServices;
            _constructoraServices = constructoraServices;
        }

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
            return StatusCode(StatusCodes.Status200OK, listaConstructorasVM);
        }

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

                //contactoIngresadoVM..SecUsuario = ObtieneSecuencialUsuario();

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

        public IActionResult Index()
        {
            return View();
        }
    }
}
