using AutoMapper;
using BLL.Implementacion;
using BLL.Interfaces;
using Entity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using TecmeinWebApp.Models.ViewModel;
using TecmeinWebApp.Utilidades.Response;

namespace TecmeinWebApp.Controllers
{
    public class VisitaController : Controller
    {
        private readonly IVisitaServices _visitaServices;

        private readonly IProvinciaServices _provinciaServices;
        private readonly ICantonServices _cantonServices;
        private readonly IParroquiaServices _parroquiaServices;

        private readonly IMapper _mapper;
        public VisitaController(IVisitaServices visitaServices,
                                          IProvinciaServices provinciaServices,
                                          ICantonServices cantonServices,
                                          IParroquiaServices parroquiaServices,
                                          IMapper mapper

            )
        {
            _visitaServices = visitaServices;
            _provinciaServices = provinciaServices;
            _cantonServices = cantonServices;
            _parroquiaServices = parroquiaServices;
            _mapper = mapper;
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

        public async Task<IActionResult> Editar([FromForm] string modelo, [FromForm] string modeloVisitaDetalle)
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

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int secuencial)
        {
            var gResponse = new GenericResponse<string>();
            try
            {
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
        public int ObtieneSecuencialUsuario() {

            ClaimsPrincipal claimsUser = HttpContext.User;
            string? secUsuario
                   = claimsUser.Claims
                               .Where(x => x.Type == ClaimTypes.NameIdentifier)
                               .Select(x => x.Value)
                               .SingleOrDefault();

            return (int)(string.IsNullOrEmpty(secUsuario) ? 0 : Convert.ToUInt32(secUsuario));
            
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
