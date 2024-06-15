using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Newtonsoft.Json;
using TecmeinWebApp.Models.ViewModel;
using TecmeinWebApp.Utilidades.Response;
using BLL.Interfaces;
using Entity;

namespace TecmeinWebApp.Controllers
{
    public class EmpresaController : Controller
    {
        private readonly IMapper _mapper;    
        private readonly IEmpresaServices _empresaServices;

        public EmpresaController(IMapper mapper,IEmpresaServices empresaServices)
        {
            _mapper = mapper;
            _empresaServices = empresaServices;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Obtener()
        {
            var gResponse = new GenericResponse<EmpresaVM>();
            try
            {
                var empresaVM = _mapper.Map<EmpresaVM>(await _empresaServices.Obtener());
                gResponse.Estado = true;
                gResponse.Objeto = empresaVM;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarCambios( [FromForm]IFormFile logo, [FromForm]string modelo )
        {
            var gResponse = new GenericResponse<EmpresaVM>();
            try
            {
                var empresaVM = JsonConvert.DeserializeObject<EmpresaVM>(modelo);

                string nombreLogo = string.Empty;
                Stream logoStream = null;

                if (logo != null) { 
                    string nombreCodificado = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(logo.FileName);
                    nombreLogo= string.Concat( nombreCodificado, extension);
                    logoStream = logo.OpenReadStream();
                
                }
                Empresa empresaEcontrada = await _empresaServices.GuardarCambios(_mapper.Map<Empresa>(empresaVM),logoStream, nombreLogo);

                empresaVM = _mapper.Map<EmpresaVM>(empresaEcontrada);

                gResponse.Estado = true;
                gResponse.Objeto = empresaVM;
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
