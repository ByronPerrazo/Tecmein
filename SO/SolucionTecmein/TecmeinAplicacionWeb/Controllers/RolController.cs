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

                                var permisosActuales = await (await _repositorioRolMenu.Consultar(rm => rm.SecRol == modelo.SecRol)).ToListAsync();

                

                                // Eliminar todos los permisos existentes para este rol

                                foreach (var permiso in permisosActuales)

                                {

                                    await _repositorioRolMenu.Eliminar(permiso);

                                }

                

                                // Crear los nuevos permisos basados en el modelo

                                if (modelo.Menus != null)

                                {

                                    var nuevosPermisos = new List<RolMenu>();

                                    ProcesarMenusRecursivamente(modelo.Menus, modelo.SecRol, nuevosPermisos);

                

                                    foreach (var nuevoPermiso in nuevosPermisos)

                                    {

                                        await _repositorioRolMenu.Crear(nuevoPermiso);

                                    }

                

                                    // Invalidar la caché de permisos para este rol

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

                            }

                            catch (Exception ex)

                            {

                                gResponse.Estado = false;

                                gResponse.Mensajes = ex.Message;

                            }

                            return StatusCode(StatusCodes.Status200OK, gResponse);

                        }

                

                        private void ProcesarMenusRecursivamente(IEnumerable<MenuPermisoVM> menus, int secRol, List<RolMenu> nuevosPermisos)

                        {

                            foreach (var menuVm in menus)

                            {

                                // Añadir el permiso del menú actual si tiene alguna opción seleccionada

                                if (menuVm.VerMenu || menuVm.Crear || menuVm.Leer || menuVm.Actualizar || menuVm.Eliminar)

                                {

                                    nuevosPermisos.Add(new RolMenu

                                    {

                                        SecRol = secRol,

                                        SecMenu = menuVm.SecMenu,

                                        VerMenu = menuVm.VerMenu,

                                        Crear = menuVm.Crear,

                                        Leer = menuVm.Leer,

                                        Actualizar = menuVm.Actualizar,

                                        Eliminar = menuVm.Eliminar

                                    });

                                }

                

                                // Procesar recursivamente los submenús

                                if (menuVm.SubMenus != null && menuVm.SubMenus.Any())

                                {

                                    ProcesarMenusRecursivamente(menuVm.SubMenus, secRol, nuevosPermisos);

                                }

                            }

                        }

                    }

                }

                

        