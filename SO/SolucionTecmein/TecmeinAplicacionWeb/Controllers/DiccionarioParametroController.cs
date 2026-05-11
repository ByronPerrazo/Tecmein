using BLL.Interfaces;
using Entity;
using Microsoft.AspNetCore.Mvc;
using TecmeinWebApp.Utilidades.ViewComponents;

namespace TecmeinAplicacionWeb.Controllers
{
    // [Authorize(Policy = "DiccionarioParametro.Admin")]
    public class DiccionarioParametroController : Controller
    {
        private readonly IDiccionarioParametroService _diccionarioParametroServicio;

        public DiccionarioParametroController(IDiccionarioParametroService diccionarioParametroServicio)
        {
            _diccionarioParametroServicio = diccionarioParametroServicio;
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
            try
            {
                var lista = await _diccionarioParametroServicio.Lista();
                // Devolvemos los datos en un formato que el frontend espera, evitando la serialización con referencias.
                return StatusCode(StatusCodes.Status200OK, new { data = lista });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = ex.Message });
            }
        }

        [HttpPost]
        [ValidatePermission("CREAR")]
        public async Task<IActionResult> Crear([FromBody] DiccionarioParametro entidad)
        {
            try
            {
                var entidadCreada = await _diccionarioParametroServicio.Crear(entidad);
                return StatusCode(StatusCodes.Status200OK, new { estado = true, objeto = entidadCreada });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status200OK, new { estado = false, mensaje = ex.Message });
            }
        }

        [HttpPut]
        [ValidatePermission("ACTUALIZAR")]
        public async Task<IActionResult> Editar([FromBody] DiccionarioParametro entidad)
        {
            try
            {
                var resultado = await _diccionarioParametroServicio.Editar(entidad);
                return StatusCode(StatusCodes.Status200OK, new { estado = resultado });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status200OK, new { estado = false, mensaje = ex.Message });
            }
        }

        [HttpDelete]
        [ValidatePermission("ELIMINAR")]
        public async Task<IActionResult> Eliminar(int secuencial)
        {
            try
            {
                var resultado = await _diccionarioParametroServicio.Eliminar(secuencial);
                return StatusCode(StatusCodes.Status200OK, new { estado = resultado });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status200OK, new { estado = false, mensaje = ex.Message });
            }
        }

        [HttpGet]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> ListaActivos()
        {
            try
            {
                var lista = await _diccionarioParametroServicio.ListaActivos();
                return StatusCode(StatusCodes.Status200OK, new { data = lista });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = ex.Message });
            }
        }
    }
}