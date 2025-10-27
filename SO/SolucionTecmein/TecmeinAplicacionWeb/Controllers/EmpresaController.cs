using AutoMapper;
using BLL.Interfaces;
using Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TecmeinAplicacionWeb.Models.ViewModels;
using TecmeinWebApp.Utilidades.Response;
using TecmeinWebApp.Utilidades.ViewComponents;

namespace TecmeinWebApp.Controllers
{
    public class EmpresaController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IEmpresaServices _empresaServices;

        public EmpresaController(IMapper mapper, IEmpresaServices empresaServices)
        {
            _mapper = mapper;
            _empresaServices = empresaServices;
        }

        [ValidatePermission("VER_MENU")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> Lista()
        {
            var listaEmpresaVM
               = _mapper.Map<List<EmpresaVM>>(await _empresaServices.Lista());
            return StatusCode(StatusCodes.Status200OK, new { data = listaEmpresaVM });
        }

        [HttpGet]
        [ValidatePermission("LEER")]
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
        [ValidatePermission("ACTUALIZAR")]
        public async Task<IActionResult> GuardarCambios([FromForm] IFormFile logo, [FromForm] string modelo)
        {
            var gResponse = new GenericResponse<EmpresaVM>();
            try
            {
                var empresaVM = JsonConvert.DeserializeObject<EmpresaVM>(modelo);

                string nombreLogo = string.Empty;
                Stream? logoStream = null;

                if (logo != null)
                {
                    string nombreCodificado = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(logo.FileName);
                    nombreLogo = string.Concat(nombreCodificado, extension);
                    logoStream = logo.OpenReadStream();

                }
                empresaVM.EstaActivo = 1;
                Empresa empresaEcontrada = await _empresaServices.Crear(_mapper.Map<Empresa>(empresaVM), logoStream, nombreLogo);

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

        [HttpPut]
        [ValidatePermission("ACTUALIZAR")]
        public async Task<IActionResult> Editar([FromForm] IFormFile logo, [FromForm] string modelo)
        {
            var genericResponse = new GenericResponse<EmpresaVM>();
            try
            {
                EmpresaVM? tipoProductoVM = JsonConvert.DeserializeObject<EmpresaVM>(modelo);

                var tipoProdEdit = await _empresaServices.Editar(_mapper.Map<Empresa>(tipoProductoVM));
                tipoProductoVM = _mapper.Map<EmpresaVM>(tipoProdEdit);

                genericResponse.Estado = true;
                genericResponse.Objeto = tipoProductoVM;
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
                gResponse.Estado = await _empresaServices.Eliminar(secuencial);
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
                throw;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

    }
}
