using AutoMapper;
using BLL.Interfaces;
using Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TecmeinWebApp.Models.ViewModel;
using TecmeinWebApp.Utilidades.Response;

namespace TecmeinWebApp.Controllers
{
    public class RolController : Controller
    {

        private readonly IRolServices _rolServices;
        private readonly IPermisosRolServices _permisosRolServices;
        private readonly IMenusHijosDesplegables _menusHijosDesplegables;
        private readonly IRolMenuServices _rolMenuServices;
        private readonly IMapper _mapper;

        public RolController(IRolServices rolServices, IMapper mapper, IPermisosRolServices permisosRolServices, IMenusHijosDesplegables menusHijosDesplegables, IRolMenuServices rolMenuServices)
        {
            _rolServices = rolServices;
            _mapper = mapper;
            _permisosRolServices = permisosRolServices;
            _menusHijosDesplegables = menusHijosDesplegables;
            _rolMenuServices = rolMenuServices;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListaRol()
        {
            List<RolVM> listaRolVM
              = _mapper.Map<List<RolVM>>(await _rolServices.Lista());
            return StatusCode(StatusCodes.Status200OK, new { data = listaRolVM });
        }

        [HttpGet]
        public async Task<IActionResult> RolPorSecuencial(int secRol)
        {
            var rol = await _rolServices.RolPorSecuencial(secRol);
            var permisos = await _permisosRolServices.PermisosRolActivo(secRol);

            var rolVM = _mapper.Map<RolVM>(rol);
            rolVM.oPermisosRol = _mapper.Map<PermisosrolVM>(permisos);

            return StatusCode(StatusCodes.Status200OK, rolVM);
        }

        [HttpPost]
        [Authorize(Policy = "CanModify")]
        public async Task<IActionResult> ProcesaGuardarRol([FromForm] string modelo)
        {
            var gResponse = new GenericResponse<RolVM>();
            try
            {
                var rolVM = JsonConvert.DeserializeObject<RolVM>(modelo);

                Rol rolGenerado;
                var permisos = _mapper.Map<Permisosrol>(rolVM.oPermisosRol);

                if (rolVM.Secuencial == 0)
                {
                    rolGenerado = await _rolServices.GuardarRolCompleto(_mapper.Map<Rol>(rolVM), permisos);
                }
                else
                {
                    rolGenerado = await _rolServices.EditarRolCompleto(_mapper.Map<Rol>(rolVM), permisos);
                }

                rolVM = _mapper.Map<RolVM>(rolGenerado);

                gResponse.Estado = true;
                gResponse.Objeto = rolVM;
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
                gResponse.Estado = await _rolServices.EliminarRol(secuencial);
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerMenusPorRol(int secRol)
        {
            var gResponse = new GenericResponse<object>();
            try
            {
                var todosLosMenusHijos = await _menusHijosDesplegables.ObtenerMenusHijos();
                var rolMenusExistentes = await _rolMenuServices.Lista();

                var menusAsignadosIds = rolMenusExistentes
                                        .Where(rm => rm.SecRol == secRol && rm.EsActivo == 1 && rm.SecMenu.HasValue)
                                        .Select(rm => rm.SecMenu.Value)
                                        .ToList();

                var menusDisponibles = todosLosMenusHijos
                                        .Where(m => !menusAsignadosIds.Contains(m.Secuencial))
                                        .Select(m => _mapper.Map<MenuVM>(m))
                                        .ToList();

                var menusSeleccionados = todosLosMenusHijos
                                        .Where(m => menusAsignadosIds.Contains(m.Secuencial))
                                        .Select(m => _mapper.Map<MenuVM>(m))
                                        .ToList();

                gResponse.Estado = true;
                gResponse.Objeto = new
                {
                    menusDisponibles = menusDisponibles,
                    menusSeleccionados = menusSeleccionados
                };
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
            }

            var jsonOptions = new System.Text.Json.JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            };
            return new JsonResult(gResponse, jsonOptions);
        }
    }
}
