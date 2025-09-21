using AutoMapper;
using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TecmeinAplicacionWeb.Models.ViewModels; 
using TecmeinWebApp.Utilidades.Response;

namespace TecmeinWebApp.Controllers
{
    [Authorize(Policy = "Roles.Administrar")]
    public class RolController : Controller
    {
        private readonly IRolServices _rolServices;
        private readonly IMenuServices _menuServices;
        private readonly IGenericRepository<Permiso> _repositorioPermiso;
        private readonly IGenericRepository<RolPermiso> _repositorioRolPermiso;
        private readonly IGenericRepository<RolMenu> _repositorioRolMenu;
        private readonly IMapper _mapper;

        public RolController(IRolServices rolServices,
                             IMenuServices menuServices,
                             IMapper mapper,
                             IGenericRepository<Permiso> repositorioPermiso,
                             IGenericRepository<RolPermiso> repositorioRolPermiso,
                             IGenericRepository<RolMenu> repositorioRolMenu)
        {
            _rolServices = rolServices;
            _menuServices = menuServices;
            _mapper = mapper;
            _repositorioPermiso = repositorioPermiso;
            _repositorioRolPermiso = repositorioRolPermiso;
            _repositorioRolMenu = repositorioRolMenu;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListaRol()
        {
            List<RolVM> listaRolVM = _mapper.Map<List<RolVM>>(await _rolServices.Lista());
            var settings = new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var jsonResult = JsonConvert.SerializeObject(new { data = listaRolVM }, Formatting.None, settings);
            return Content(jsonResult, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> RolPorSecuencial(int secRol)
        {
            var rol = await _rolServices.RolPorSecuencial(secRol);
            var rolVM = _mapper.Map<RolVM>(rol);
            return StatusCode(StatusCodes.Status200OK, rolVM);
        }

        [HttpPost]
        public async Task<IActionResult> ProcesaGuardarRol([FromForm] string modelo)
        {
            var gResponse = new GenericResponse<RolVM>();
            try
            {
                var rolVM = JsonConvert.DeserializeObject<RolVM>(modelo);
                Rol rolGenerado = (rolVM.Secuencial == 0)
                    ? await _rolServices.Crear(_mapper.Map<Rol>(rolVM))
                    : await _rolServices.Editar(_mapper.Map<Rol>(rolVM));
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

        // ===================================================================
        // GESTIÓN DE PERMISOS REFACTORIZADA
        // ===================================================================

        [HttpGet]
        public async Task<IActionResult> GestionarPermisos(int secRol)
        {
            var rol = await _rolServices.RolPorSecuencial(secRol);
            if (rol == null) return NotFound();

            var todosLosMenus = await _menuServices.ObtieneMenuTotal();
            var todosLosPermisos = await (await _repositorioPermiso.Consultar()).ToListAsync();

            var menusAsignados = (await _repositorioRolMenu.Consultar(rm => rm.SecRol == secRol))
                                     .Select(rm => rm.SecMenu.Value)
                                     .ToHashSet();

            var permisosAsignados = (await _repositorioRolPermiso.Consultar(rp => rp.SecRol == secRol))
                                        .Select(rp => rp.IdPermiso)
                                        .ToHashSet();

            var vm = new GestionRolPermisoVM
            {
                SecRol = rol.Secuencial,
                NombreRol = rol.Descripcion,
                TodosLosMenus = _mapper.Map<List<MenuVM>>(todosLosMenus),
                TodosLosPermisos = _mapper.Map<List<PermisoVM>>(todosLosPermisos),
                MenusAsignados = menusAsignados,
                PermisosAsignados = permisosAsignados
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarPermisos([FromBody] GestionRolPermisoVM modelo)
        {
            var gResponse = new GenericResponse<bool>();
            try
            {
                var menusActuales = await (await _repositorioRolMenu.Consultar(rm => rm.SecRol == modelo.SecRol)).ToListAsync();
                foreach (var menu in menusActuales) await _repositorioRolMenu.Eliminar(menu);

                var permisosActuales = await (await _repositorioRolPermiso.Consultar(rp => rp.SecRol == modelo.SecRol)).ToListAsync();
                foreach (var permiso in permisosActuales) await _repositorioRolPermiso.Eliminar(permiso);

                if (modelo.MenusAsignados != null)
                {
                    foreach (var menuId in modelo.MenusAsignados)
                    {
                        await _repositorioRolMenu.Crear(new RolMenu { SecRol = modelo.SecRol, SecMenu = menuId, EsActivo = 1 });
                    }
                }

                if (modelo.PermisosAsignados != null)
                {
                    foreach (var permisoId in modelo.PermisosAsignados)
                    {
                        await _repositorioRolPermiso.Crear(new RolPermiso { SecRol = modelo.SecRol, IdPermiso = permisoId });
                    }
                }

                gResponse.Estado = true;
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