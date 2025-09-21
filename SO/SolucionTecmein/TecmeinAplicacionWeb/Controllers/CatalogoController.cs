using AutoMapper;
using BLL.Interfaces;
using Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TecmeinAplicacionWeb.Models.ViewModels;
using TecmeinWebApp.Utilidades.Response;

namespace TecmeinWebApp.Controllers
{
    [Authorize]
    public class CatalogoController : Controller
    {

        private readonly ICatalogoServices _catalogoServices;
        //private readonly IRolServices _rolServices;
        private readonly IMapper _mapper;

        public CatalogoController(

                                  ICatalogoServices catalogoServices,
                                  //IRolServices rolServices,
                                  IMapper mapper
            )
        {
            _catalogoServices = catalogoServices;
            //_rolServices = rolServices;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            var listaCatalogoVM
               = _mapper.Map<List<CatalogoVM>>(await _catalogoServices.Lista());
            return StatusCode(StatusCodes.Status200OK, new { data = listaCatalogoVM });
        }

        [HttpGet]
        public async Task<IActionResult> CatalogoPorSecuencial(int secuencial)
        {
            var catalogo = _mapper.Map<CatalogoVM>(await _catalogoServices.CatalogoPorSecuencial(secuencial));
            return StatusCode(StatusCodes.Status200OK, catalogo);
        }

        [HttpPost]
        [Authorize(Policy = "CanModify")]
        public async Task<IActionResult> CrearCatalogo([FromForm] IFormFile archivoPDF, [FromForm] string modelo)
        {
            var genericResponse = new GenericResponse<CatalogoVM>();
            try
            {
                CatalogoVM? catalogoVM = JsonConvert.DeserializeObject<CatalogoVM>(modelo);

                string nombreArchivo = string.Empty;
                Stream? documentoStream = null;

                if (archivoPDF != null)
                {
                    string nombreCodificado = catalogoVM.Nombre;
                    string extension = Path.GetExtension(archivoPDF.FileName);
                    nombreArchivo = string.Concat(nombreCodificado, extension);
                    documentoStream = archivoPDF.OpenReadStream();
                }

                var usurioGenerado = await _catalogoServices.Crear(_mapper.Map<Catalogo>(catalogoVM), documentoStream, nombreArchivo);
                catalogoVM = _mapper.Map<CatalogoVM>(usurioGenerado);

                genericResponse.Estado = true;
                genericResponse.Objeto = catalogoVM;


            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensajes = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, genericResponse);

        }

        [HttpDelete]
        [Authorize(Policy = "CanDelete")]
        public async Task<IActionResult> Eliminar(int secuencial)
        {
            var gResponse = new GenericResponse<string>();
            try
            {
                gResponse.Estado = await _catalogoServices.Eliminar(secuencial);
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
