using AutoMapper;
using BLL.Interfaces;
using Entity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using TecmeinWebApp.Models.ViewModel;
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
                                IEquiposVisitaServices equiposVisitaServices
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
        public async Task<IActionResult> Lista()
        {
            var listaVisitaVM
                = _mapper.Map<List<VisitaVM>>(await _visitaServices.ListaVisitas());
            return StatusCode(StatusCodes.Status200OK, new { data = listaVisitaVM });
        }

        [HttpPost]
        public async Task<IActionResult> CrearVisita([FromForm] string modelo)
        {
            var genericResponse = new GenericResponse<VisitaVM>();
            try
            {
                VisitaVM? visitaIngresadaVM
                    = JsonConvert
                      .DeserializeObject<VisitaVM>(modelo);

                ClaimsPrincipal claimsUser = HttpContext.User;
                string? secUsuario
                       = claimsUser.Claims
                                   .Where(x => x.Type == ClaimTypes.NameIdentifier)
                                   .Select(x => x.Value)
                                   .SingleOrDefault();

                visitaIngresadaVM.SecUsuario = ObtieneSecuencialUsuario();

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
        public async Task<IActionResult> EditarVisita([FromForm] string modelo)
        {
            var genericResponse = new GenericResponse<VisitaVM>();
            try
            {
                VisitaVM? visitaVM = JsonConvert.DeserializeObject<VisitaVM>(modelo);

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
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
                throw;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpGet]
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
        public async Task<IActionResult> EquiposDeVisita(int secuencialVisita)
        {
            var listaEquipoVistaVM
                = _mapper.Map<List<EquiposVisitaVM>>(await _equiposVisitaServices.ConsultaListaPorVisita(secuencialVisita));
            return StatusCode(StatusCodes.Status200OK, listaEquipoVistaVM);
        }

        [HttpPost]
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
        //[ValidateUser("DeleteUser")]
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
    }
}
