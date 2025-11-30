using AutoMapper;
using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using TecmeinAplicacionWeb.Models.ViewModels;
using TecmeinWebApp.Utilidades.Response;
using TecmeinWebApp.Utilidades.ViewComponents;

namespace TecmeinWebApp.Controllers
{
    [Authorize(Policy = "Roles.Administrar")]
    public class RolController : Controller
    {
        private readonly IGenericRepository<RolPermiso> _repositorioRolPermiso;
        private readonly IGenericRepository<RolMenu> _repositorioRolMenu;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Permiso> _repositorioPermiso;
        private readonly IMemoryCache _cache; // Inyectar IMemoryCache
        private IRolServices _rolServices;
        private readonly IMenuServices _menuServices;

        public RolController(IRolServices rolServices,
                             IMenuServices menuServices,
                             IMapper mapper,
                             IGenericRepository<Permiso> repositorioPermiso,
                             IGenericRepository<RolPermiso> repositorioRolPermiso,
                             IGenericRepository<RolMenu> repositorioRolMenu,
                             IMemoryCache cache) // Añadir IMemoryCache al constructor
        {
            _rolServices = rolServices;
            _menuServices = menuServices;
            _mapper = mapper;
            _repositorioPermiso = repositorioPermiso;
            _repositorioRolPermiso = repositorioRolPermiso;
            _repositorioRolMenu = repositorioRolMenu;
            _cache = cache; // Asignar IMemoryCache
        }

        [ValidatePermission("VER_MENU")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [ValidatePermission("LEER")]
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
        [ValidatePermission("LEER")]
        public async Task<IActionResult> RolPorSecuencial(int secRol)
        {
            var rol = await _rolServices.RolPorSecuencial(secRol);
            var rolVM = _mapper.Map<RolVM>(rol);
            return StatusCode(StatusCodes.Status200OK, rolVM);
        }

        [HttpPost]
        [ValidatePermission("ACTUALIZAR")]
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
        [ValidatePermission("ELIMINAR")]
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
        [ValidatePermission("LEER")]
        public async Task<IActionResult> GestionarPermisos(int secRol)                            
        {
            var rol = await _rolServices.RolPorSecuencial(secRol);

            if (rol == null)
            {
                TempData["ErrorMessage"] = "El rol especificado no existe.";
                return RedirectToAction("Index");
            }
            var todosLosMenus = await _menuServices.ObtenerTodosLosMenusParaGestion();
            var permisosAsignados = (await _repositorioRolMenu.Consultar(rm => rm.SecRol == secRol)).ToList();

            // 1. Crear una lista plana de VMs para todos los menús
            var menuVms = todosLosMenus.Select(menu => new MenuPermisoVM
            {
                SecMenu = menu.Secuencial,
                Descripcion = menu.Descripcion,
                Icono = menu.Icono,
                SecMenuPadre = menu.SecMenuPadre,
                VerMenu = permisosAsignados.FirstOrDefault(p => p.SecMenu == menu.Secuencial)?.VerMenu ?? false,
                Crear = permisosAsignados.FirstOrDefault(p => p.SecMenu == menu.Secuencial)?.Crear ?? false,
                Leer = permisosAsignados.FirstOrDefault(p => p.SecMenu == menu.Secuencial)?.Leer ?? false,
                Actualizar = permisosAsignados.FirstOrDefault(p => p.SecMenu == menu.Secuencial)?.Actualizar ?? false,
                Eliminar = permisosAsignados.FirstOrDefault(p => p.SecMenu == menu.Secuencial)?.Eliminar ?? false,
                SubMenus = new List<MenuPermisoVM>() // Inicializar submenús
            }).ToList();

            // 2. Construir la jerarquía de dos niveles que la vista espera
            var menuLookup = menuVms.ToDictionary(m => m.SecMenu);
            var menuTree = new List<MenuPermisoVM>();

            foreach (var menuVm in menuVms)
            {
                if (menuVm.SecMenuPadre.HasValue && menuLookup.ContainsKey(menuVm.SecMenuPadre.Value))
                {
                    menuLookup[menuVm.SecMenuPadre.Value].SubMenus.Add(menuVm);
                }
                else
                {
                    menuTree.Add(menuVm); // Menú padre o huérfano
                }
            }

            var vm = new GestionRolMenuVM
            {
                SecRol = rol.Secuencial,
                NombreRol = rol.Descripcion,
                Menus = menuTree.OrderBy(m => m.Descripcion).ToList() // Ordenar menús padre
            };

            return View(vm);
        }

        [HttpPost]
        [ValidatePermission("ACTUALIZAR")]
        public async Task<IActionResult> GuardarPermisos([FromBody] GestionRolMenuVM modelo)
        {
            var gResponse = new GenericResponse<bool>();
            try
            {
                var permisosActuales = await (await _repositorioRolMenu.Consultar(rm => rm.SecRol == modelo.SecRol))
                                        .ToDictionaryAsync(rm => rm.SecMenu);

                var menusRecibidos = new List<MenuPermisoVM>();
                void AplanarMenus(IEnumerable<MenuPermisoVM> menus)
                {
                    if (menus == null) return;
                    foreach (var menu in menus)
                    {
                        menusRecibidos.Add(menu);
                        if (menu.SubMenus != null && menu.SubMenus.Any())
                        {
                            AplanarMenus(menu.SubMenus);
                        }
                    }
                }
                AplanarMenus(modelo.Menus);

                foreach (var menuVm in menusRecibidos)
                {
                    var tieneAlgunPermiso = menuVm.VerMenu || menuVm.Crear || menuVm.Leer || menuVm.Actualizar || menuVm.Eliminar;

                    if (permisosActuales.TryGetValue(menuVm.SecMenu, out var permisoExistente))
                    {
                        // The permission exists in the DB
                        if (tieneAlgunPermiso)
                        {
                            // Update it
                            permisoExistente.VerMenu = menuVm.VerMenu;
                            permisoExistente.Crear = menuVm.Crear;
                            permisoExistente.Leer = menuVm.Leer;
                            permisoExistente.Actualizar = menuVm.Actualizar;
                            permisoExistente.Eliminar = menuVm.Eliminar;
                            await _repositorioRolMenu.Editar(permisoExistente);
                        }
                        else
                        {
                            // All permissions were unchecked, so delete it
                            await _repositorioRolMenu.Eliminar(permisoExistente);
                        }
                    }
                    else if (tieneAlgunPermiso)
                    {
                        // The permission does not exist in the DB, but it has been assigned in the UI
                        // Create it
                        var nuevoPermiso = new RolMenu
                        {
                            SecRol = modelo.SecRol,
                            SecMenu = menuVm.SecMenu,
                            VerMenu = menuVm.VerMenu,
                            Crear = menuVm.Crear,
                            Leer = menuVm.Leer,
                            Actualizar = menuVm.Actualizar,
                            Eliminar = menuVm.Eliminar
                        };
                        await _repositorioRolMenu.Crear(nuevoPermiso);
                    }
                }
                
                // Invalidate permission cache for this role
                var acciones = new[] { "CREAR", "LEER", "ACTUALIZAR", "ELIMINAR", "VER_MENU" };
                var todosLosMenus = await _menuServices.ObtieneMenuTotal();
                foreach (var menu in todosLosMenus)
                {
                    if (!string.IsNullOrEmpty(menu.Controlador))
                    {
                        foreach (var accion in acciones)
                        {
                            var cacheKey = $"Permiso_{modelo.SecRol}_{menu.Controlador}_{accion}";
                            _cache.Remove(cacheKey);
                        }
                    }
                }
                var cacheMenuKey = $"Menu_{modelo.SecRol}";
                _cache.Remove(cacheMenuKey);

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