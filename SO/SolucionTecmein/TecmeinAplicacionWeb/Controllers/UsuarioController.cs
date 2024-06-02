using AplicacionWeb.Models.ViewModel;
using AplicacionWeb.Utilidades.Response;
using AutoMapper;
using BLL.Interfaces;
using Entity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AplicacionWeb.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IUsuarioServices _usuarioServices;
        private readonly IRolServices _rolServices;
        private readonly IMapper _mapper;

        public UsuarioController(

                                  IUsuarioServices usuarioServices,
                                  IRolServices rolServices,
                                  IMapper mapper
            )
        {
            _usuarioServices = usuarioServices;
            _rolServices = rolServices;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListaRol()
        {
            List<RolVM> listaPerfilesVM
                = _mapper.Map<List<RolVM>>(await _rolServices.Lista());
            return StatusCode(StatusCodes.Status200OK, listaPerfilesVM);

        }


        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            var usuarioListaVM = _mapper.Map<List<UsuarioVM>>(await _usuarioServices.Lista());
            return StatusCode(StatusCodes.Status200OK, new { data = usuarioListaVM });
        }

        [HttpGet]
        public async Task<IActionResult> ExisteUsuario(int secuencialUsuario)
        {
            var existe = _mapper.Map<UsuarioVM>(await _usuarioServices.ExistePorSecuencial(secuencialUsuario));
            return StatusCode(StatusCodes.Status200OK, existe);
        }


        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] IFormFile imagen, [FromForm] string modelo)
        {

            var genericResponse = new GenericResponse<UsuarioVM>();
            try
            {
                UsuarioVM? usuariosVM = JsonConvert.DeserializeObject<UsuarioVM>(modelo);

                string nombreFoto = string.Empty;
                Stream? imagenStream = null;

                if (imagen != null)
                {
                    string nombreCodificado = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(imagen.FileName);
                    nombreFoto = string.Concat(nombreCodificado, extension);
                    imagenStream = imagen.OpenReadStream();
                }

                var urlPantallaCorreo = $"{this.Request.Scheme}://" +
                                        $"{this.Request.Host}" +
                                        $"/Plantilla/EnviarClave?correo=[correo]&clave=[clave]";
                var usurioGenerado = await _usuarioServices.Crear(_mapper.Map<Usuario>(usuariosVM), imagenStream, nombreFoto, urlPantallaCorreo);
                usuariosVM = _mapper.Map<UsuarioVM>(usurioGenerado);

                genericResponse.Estado = true;
                genericResponse.Objeto = usuariosVM;


            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensajes = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }


        [HttpPut]
        public async Task<IActionResult> Editar([FromForm] IFormFile imagen, [FromForm] string modelo)
        {

            var genericResponse = new GenericResponse<UsuarioVM>();
            try
            {
                UsuarioVM? usuariosVM = JsonConvert.DeserializeObject<UsuarioVM>(modelo);

                string nombreFoto = string.Empty;
                Stream? imagenStream = null;

                if (imagen != null)
                {

                    string nombreCodificado = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(imagen.FileName);
                    nombreFoto = string.Concat(nombreCodificado, extension);
                    imagenStream = imagen.OpenReadStream();
                }

                Usuario usuarioEditado = await _usuarioServices.Editar(_mapper.Map<Usuario>(usuariosVM), imagenStream, nombreFoto);
                usuariosVM = _mapper.Map<UsuarioVM>(usuarioEditado);
                genericResponse.Estado = true;
                genericResponse.Objeto = usuariosVM;


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
                gResponse.Estado = await _usuarioServices.Eliminar(secuencialUsuario);
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
