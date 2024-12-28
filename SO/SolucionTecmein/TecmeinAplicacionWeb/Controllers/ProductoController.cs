using AutoMapper;
using BLL.Interfaces;
using Entity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TecmeinWebApp.Models.ViewModel;
using TecmeinWebApp.Utilidades.Response;

namespace TecmeinWebApp.Controllers
{
    public class ProductoController : Controller
    {

        private readonly IProductoServices _productoServices;
        private readonly ITipoProductoServices _tipoProductoServices;
        private readonly IMapper _mapper;

        public ProductoController(IProductoServices prodServices,
                                  ITipoProductoServices tipoProductoServices,
                                  IMapper mapper)
        {
            _productoServices = prodServices;
            _tipoProductoServices = tipoProductoServices;
            _mapper = mapper;
        }


        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListaTipoProducto()
        {
            List<TipoProductoVM> listaTipoProductoVM
              = _mapper.Map<List<TipoProductoVM>>(await _tipoProductoServices.Lista());
            return StatusCode(StatusCodes.Status200OK, listaTipoProductoVM);
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            var usuarioListaVM = _mapper.Map<List<ProductoVM>>(await _productoServices.Lista());
            return StatusCode(StatusCodes.Status200OK, new { data = usuarioListaVM });
        }

        [HttpGet]
        public async Task<IActionResult> ExisteProducto(int secuencial)
        {
            var existe = _mapper.Map<ProductoVM>(await _productoServices.OtenerPorSecuencial(secuencial));
            return StatusCode(StatusCodes.Status200OK, existe);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] IFormFile imagen, [FromForm] string modelo)
        {

            var genericResponse = new GenericResponse<ProductoVM>();
            try
            {
                ProductoVM? productoVM = JsonConvert.DeserializeObject<ProductoVM>(modelo);

                string nombreFoto = string.Empty;
                Stream? imagenStream = null;

                if (imagen != null)
                {
                    string nombreCodificado = $"{productoVM.Secuencial.ToString()}-{Guid.NewGuid().ToString("N").Substring(0, 8)}";
                    string extension = Path.GetExtension(imagen.FileName);
                    nombreFoto = string.Concat(nombreCodificado, extension);
                    imagenStream = imagen.OpenReadStream();
                }

                var usurioGenerado = await _productoServices.Crear(_mapper.Map<Producto>(productoVM), imagenStream, nombreFoto);
                productoVM = _mapper.Map<ProductoVM>(usurioGenerado);

                genericResponse.Estado = true;
                genericResponse.Objeto = productoVM;

            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensajes = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }


        [HttpPut]
        public async Task<IActionResult> Editar([FromForm] IFormFile Foto, [FromForm] string modelo)
        {

            var genericResponse = new GenericResponse<ProductoVM>();
            try
            {
                ProductoVM? productoVM = JsonConvert.DeserializeObject<ProductoVM>(modelo);

                string nombreFoto = string.Empty;
                Stream? imagenStream = null;

                if (Foto != null)
                {
                    string nombreCodificado = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(Foto.FileName);
                    nombreFoto = string.Concat(nombreCodificado, extension);
                    imagenStream = Foto.OpenReadStream();
                }

                Producto usuarioEditado = await _productoServices.Editar(_mapper.Map<Producto>(productoVM), imagenStream, nombreFoto);
                productoVM = _mapper.Map<ProductoVM>(usuarioEditado);
                genericResponse.Estado = true;
                genericResponse.Objeto = productoVM;

            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensajes = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int secuencialUsuario)
        {
            var gResponse = new GenericResponse<string>();
            try
            {
                gResponse.Estado = await _productoServices.Eliminar(secuencialUsuario);
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
