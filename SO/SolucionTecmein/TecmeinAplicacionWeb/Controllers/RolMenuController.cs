using AutoMapper;
using BLL.Interfaces;
using Entity;
using Microsoft.AspNetCore.Mvc;
using TecmeinAplicacionWeb.Models.ViewModels;
using TecmeinWebApp.Utilidades.Response;

namespace TecmeinWebApp.Controllers
{
    public class RolMenuController : Controller
    {
        private readonly IRolMenuServices _rolMenuServices;
        private readonly IRolServices _rolServices;
        private readonly IMenuServices _menuServices;
        private readonly IMenusHijosDesplegables _menusHijosDesplegables;
        private readonly IMapper _mapper;

        public RolMenuController(IRolMenuServices rolMenuServices, IRolServices rolServices, IMenuServices menuServices, IMenusHijosDesplegables menusHijosDesplegables, IMapper mapper)
        {
            _rolMenuServices = rolMenuServices;
            _rolServices = rolServices;
            _menuServices = menuServices;
            _menusHijosDesplegables = menusHijosDesplegables;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListaRolMenu()
        {
            List<RolMenuVM> listaRolMenuVM = new List<RolMenuVM>();
            try
            {
                var listaRolMenu = await _rolMenuServices.Lista();
                foreach (var rm in listaRolMenu)
                {
                    Console.WriteLine($"RolMenu: {rm.Secuencial}, Rol: {rm.SecRolNavigation?.Descripcion}, Menu: {rm.SecMenuNavigation?.Descripcion}");
                }
                listaRolMenuVM = _mapper.Map<List<RolMenuVM>>(listaRolMenu);
                return StatusCode(StatusCodes.Status200OK, new { data = listaRolMenuVM });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener lista de RolMenu: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ProcesaGuardarRolMenu([FromBody] RolMenuVM rolMenuVM)
        {
            var gResponse = new GenericResponse<RolMenuVM>();
            try
            {
                RolMenu rolMenuGenerado;

                if (rolMenuVM.Secuencial == 0)
                {
                    rolMenuGenerado = await _rolMenuServices.Crear(_mapper.Map<RolMenu>(rolMenuVM));
                }
                else
                {
                    rolMenuGenerado = await _rolMenuServices.Editar(_mapper.Map<RolMenu>(rolMenuVM));
                }

                rolMenuVM = _mapper.Map<RolMenuVM>(rolMenuGenerado);

                gResponse.Estado = true;
                gResponse.Objeto = rolMenuVM;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int secuencial)
        {
            var gResponse = new GenericResponse<string>();
            try
            {
                gResponse.Estado = await _rolMenuServices.Eliminar(secuencial);
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerRoles()
        {
            var roles = await _rolServices.Lista();
            return StatusCode(StatusCodes.Status200OK, roles);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerMenusHijos()
        {
            var menus = await _menusHijosDesplegables.ObtenerMenusHijos();
            var menusVM = _mapper.Map<List<MenuVM>>(menus);
            var jsonOptions = new System.Text.Json.JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            };
            return new JsonResult(menusVM, jsonOptions);
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarAsignacionMenus([FromBody] ActualizarMenusRolRequest request)
        {
            var gResponse = new GenericResponse<bool>();
            try
            {
                // Eliminar asignaciones existentes para el rol
                var rolMenusExistentes = await _rolMenuServices.Lista();
                var menusAEliminar = rolMenusExistentes.Where(rm => rm.SecRol == request.SecRol).ToList();
                foreach (var rm in menusAEliminar)
                {
                    await _rolMenuServices.Eliminar(rm.Secuencial);
                }

                // Crear nuevas asignaciones
                foreach (var secMenu in request.SecMenusSeleccionados)
                {
                    var nuevoRolMenu = new RolMenu
                    {
                        SecRol = request.SecRol,
                        SecMenu = secMenu,
                        EsActivo = 1 // Asignar como activo por defecto
                    };
                    await _rolMenuServices.Crear(nuevoRolMenu);
                }

                gResponse.Estado = true;
                gResponse.Objeto = true;
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

public class ActualizarMenusRolRequest
{
    public int SecRol { get; set; }
    public List<int> SecMenusSeleccionados { get; set; }
}
