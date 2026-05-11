using AutoMapper;
using BLL.Interfaces;
using Entity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;
using TecmeinAplicacionWeb.Models.ViewModels;
using TecmeinWebApp.Utilidades.Response;
using TecmeinWebApp.Utilidades.ViewComponents;

namespace TecmeinAplicacionWeb.Controllers
{
    public class MenuController : Controller
    {
        private readonly IMenuServices _menuServices;
        private readonly IMenuDiscoveryService _menuDiscoveryService;
        private readonly IMapper _mapper;

        public MenuController(IMenuServices menuServices, IMapper mapper, IMenuDiscoveryService menuDiscoveryService)
        {
            _menuServices = menuServices;
            _mapper = mapper;
            _menuDiscoveryService = menuDiscoveryService;
        }

        [ValidatePermission("VER_MENU")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidatePermission("ACTUALIZAR")]
        public async Task<IActionResult> SincronizarModulos()
        {
            var gResponse = new GenericResponse<string>();
            try
            {
                var assembly = typeof(MenuController).Assembly;
                var resultado = await _menuDiscoveryService.DiscoverAndRegisterMenusAsync(assembly);
                gResponse.Estado = true;
                gResponse.Objeto = resultado;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        private static readonly List<string> _iconosFontAwesome = new List<string>()
        {
            "fas fa-home",
            "fas fa-user",
            "fas fa-cog",
            "fas fa-chart-area",
            "fas fa-table",
            "fas fa-folder",
            "fas fa-file",
            "fas fa-envelope",
            "fas fa-bell",
            "fas fa-calendar",
            "fas fa-cogs",
            "fas fa-wrench",
            "fas fa-users",
            "fas fa-shopping-cart",
            "fas fa-truck",
            "fas fa-clipboard-list",
            "fas fa-tasks",
            "fas fa-comments",
            "fas fa-map-marker-alt",
            "fas fa-globe",
            "fas fa-chart-bar",
            "fas fa-chart-line",
            "fas fa-chart-pie",
            "fas fa-tag",
            "fas fa-tags",
            "fas fa-barcode",
            "fas fa-qrcode",
            "fas fa-credit-card",
            "fas fa-money-bill-alt",
            "fas fa-dollar-sign",
            "fas fa-euro-sign",
            "fas fa-pound-sign",
            "fas fa-yen-sign",
            "fas fa-rupee-sign",
            "fas fa-shekel-sign",
            "fas fa-lira-sign",
            "fas fa-won-sign",
            "fas fa-file-alt",
            "fas fa-file-excel",
            "fas fa-file-pdf",
            "fas fa-file-word",
            "fas fa-file-image",
            "fas fa-file-csv",
            "fas fa-print",
            "fas fa-download",
            "fas fa-upload",
            "fas fa-plus-circle",
            "fas fa-minus-circle",
            "fas fa-times-circle",
            "fas fa-check-circle",
            "fas fa-info-circle",
            "fas fa-exclamation-triangle",
            "fas fa-question-circle",
            "fas fa-trash",
            "fas fa-edit",
            "fas fa-eye",
            "fas fa-search",
            "fas fa-filter",
            "fas fa-sort",
            "fas fa-sort-up",
            "fas fa-sort-down",
            "fas fa-sync-alt",
            "fas fa-redo",
            "fas fa-undo",
            "fas fa-share",
            "fas fa-share-alt",
            "fas fa-reply",
            "fas fa-reply-all",
            "fas fa-forward",
            "fas fa-backward",
            "fas fa-play",
            "fas fa-pause",
            "fas fa-stop",
            "fas fa-volume-up",
            "fas fa-volume-down",
            "fas fa-volume-mute",
            "fas fa-expand",
            "fas fa-compress",
            "fas fa-arrows-alt",
            "fas fa-external-link-alt",
            "fas fa-external-link-square-alt",
            "fas fa-sign-in-alt",
            "fas fa-sign-out-alt",
            "fas fa-user-plus",
            "fas fa-user-minus",
            "fas fa-user-friends",
            "fas fa-user-tie",
            "fas fa-id-card",
            "fas fa-address-book",
            "fas fa-address-card",
            "fas fa-phone",
            "fas fa-phone-alt",
            "fas fa-fax",
            "fas fa-mobile-alt",
            "fas fa-laptop",
            "fas fa-desktop",
            "fas fa-tablet-alt",
            "fas fa-tv",
            "fas fa-camera",
            "fas fa-video",
            "fas fa-microphone",
            "fas fa-headphones",
            "fas fa-gamepad",
            "fas fa-keyboard",
            "fas fa-mouse",
            "fas fa-print",
            "fas fa-calculator",
            "fas fa-calendar-alt",
            "fas fa-clock",
            "fas fa-hourglass-half",
            "fas fa-history",
            "fas fa-bookmark",
            "fas fa-star",
            "fas fa-heart",
            "fas fa-thumbs-up",
            "fas fa-thumbs-down",
            "fas fa-comment",
            "fas fa-comments",
            "fas fa-quote-left",
            "fas fa-quote-right",
            "fas fa-lightbulb",
            "fas fa-fire",
            "fas fa-tint",
            "fas fa-cloud",
            "fas fa-sun",
            "fas fa-moon",
            "fas fa-snowflake",
            "fas fa-umbrella",
            "fas fa-wind",
            "fas fa-smog",
            "fas fa-cloud-sun",
            "fas fa-cloud-moon",
            "fas fa-cloud-rain",
            "fas fa-bolt",
            "fas fa-gem",
            "fas fa-crown",
            "fas fa-shield-alt",
            "fas fa-lock",
            "fas fa-unlock-alt",
            "fas fa-key",
            "fas fa-thumbtack",
            "fas fa-map-pin",
            "fas fa-location-arrow",
            "fas fa-compass",
            "fas fa-road",
            "fas fa-bus",
            "fas fa-car",
            "fas fa-plane",
            "fas fa-ship",
            "fas fa-subway",
            "fas fa-bicycle",
            "fas fa-walking",
            "fas fa-running",
            "fas fa-swimmer",
            "fas fa-dumbbell",
            "fas fa-heartbeat",
            "fas fa-medkit",
            "fas fa-hospital",
            "fas fa-clinic-medical",
            "fas fa-syringe",
            "fas fa-pills",
            "fas fa-band-aid",
            "fas fa-first-aid",
            "fas fa-microscope",
            "fas fa-flask",
            "fas fa-dna",
            "fas fa-atom",
            "fas fa-brain",
            "fas fa-tooth",
            "fas fa-bone",
            "fas fa-joint",
            "fas fa-lungs",
            "fas fa-hand-holding-heart",
            "fas fa-hand-holding-usd",
            "fas fa-hands-helping",
            "fas fa-people-carry",
            "fas fa-box",
            "fas fa-boxes",
            "fas fa-pallet",
            "fas fa-truck-loading",
            "fas fa-truck-moving",
            "fas fa-warehouse",
            "fas fa-building",
            "fas fa-city",
            "fas fa-industry",
            "fas fa-factory",
            "fas fa-store",
            "fas fa-store-alt",
            "fas fa-cash-register",
            "fas fa-receipt",
            "fas fa-shopping-basket",
            "fas fa-shopping-bag",
            "fas fa-cart-plus",
            "fas fa-cart-arrow-down",
            "fas fa-gift",
            "fas fa-gifts",
            "fas fa-trophy",
            "fas fa-medal",
            "fas fa-award",
            "fas fa-ribbon",
            "fas fa-certificate",
            "fas fa-ticket-alt",
            "fas fa-plane-departure",
            "fas fa-plane-arrival",
            "fas fa-passport",
            "fas fa-suitcase",
            "fas fa-suitcase-rolling",
            "fas fa-globe-americas",
            "fas fa-globe-europe",
            "fas fa-globe-asia",
            "fas fa-mountain",
            "fas fa-tree",
            "fas fa-leaf",
            "fas fa-seedling",
            "fas fa-tractor",
            "fas fa-cow",
            "fas fa-piggy-bank",
            "fas fa-fish",
            "fas fa-dog",
            "fas fa-cat",
            "fas fa-paw",
            "fas fa-feather",
            "fas fa-feather-alt",
            "fas fa-paint-brush",
            "fas fa-palette",
            "fas fa-ruler",
            "fas fa-ruler-combined",
            "fas fa-ruler-horizontal",
            "fas fa-ruler-vertical",
            "fas fa-pencil-alt",
            "fas fa-pen",
            "fas fa-highlighter",
            "fas fa-eraser",
            "fas fa-cut",
            "fas fa-copy",
            "fas fa-paste",
            "fas fa-save",
            "fas fa-file-export",
            "fas fa-file-import",
            "fas fa-folder-open",
            "fas fa-folder-plus",
            "fas fa-folder-minus",
            "fas fa-archive",
            "fas fa-trash-alt",
            "fas fa-recycle",
            "fas fa-sync",
            "fas fa-redo-alt",
            "fas fa-undo-alt",
            "fas fa-history",
            "fas fa-clock",
            "fas fa-hourglass",
            "fas fa-stopwatch",
            "fas fa-timer",
            "fas fa-bell-slash",
            "fas fa-bookmark",
            "fas fa-star-half-alt",
            "fas fa-heart-broken",
            "fas fa-frown",
            "fas fa-meh",
            "fas fa-smile",
            "fas fa-laugh",
            "fas fa-surprise",
            "fas fa-angry",
            "fas fa-sad-tear",
            "fas fa-grimace",
            "fas fa-grin",
            "fas fa-grin-alt",
            "fas fa-grin-beam",
            "fas fa-grin-beam-sweat",
            "fas fa-grin-hearts",
            "fas fa-grin-squint",
            "fas fa-grin-squint-tears",
            "fas fa-grin-stars",
            "fas fa-grin-tears",
            "fas fa-grin-tongue",
            "fas fa-grin-tongue-squint",
            "fas fa-grin-tongue-wink",
            "fas fa-grin-wink",
            "fas fa-kiss",
            "fas fa-kiss-beam",
            "fas fa-kiss-wink-heart",
            "fas fa-flushed",
            "fas fa-dizzy",
            "fas fa-tired",
            "fas fa-sleepy",
            "fas fa-sad-cry",
            "fas fa-angry",
            "fas fa-surprise",
            "fas fa-frown-open",
            "fas fa-laugh-beam",
            "fas fa-laugh-squint",
            "fas fa-laugh-wink",
            "fas fa-smile-beam",
            "fas fa-smile-wink",
            "fas fa-tired",
            "fas fa-dizzy",
            "fas fa-flushed",
            "fas fa-grimace",
            "fas fa-grin",
            "fas fa-grin-alt",
            "fas fa-grin-beam",
            "fas fa-grin-beam-sweat",
            "fas fa-grin-hearts",
            "fas fa-grin-squint",
            "fas fa-grin-squint-tears",
            "fas fa-grin-stars",
            "fas fa-grin-tears",
            "fas fa-grin-tongue",
            "fas fa-grin-tongue-squint",
            "fas fa-grin-tongue-wink",
            "fas fa-grin-wink",
            "fas fa-kiss",
            "fas fa-kiss-beam",
            "fas fa-kiss-wink-heart",
            "fas fa-meh-blank",
            "fas fa-meh-rolling-eyes",
            "fas fa-sad-cry",
            "fas fa-sad-tear",
            "fas fa-smile-wink",
            "fas fa-surprise",
            "fas fa-tired",
            "fas fa-angry",
            "fas fa-dizzy",
            "fas fa-flushed",
            "fas fa-frown",
            "fas fa-frown-open",
            "fas fa-grimace",
            "fas fa-grin",
            "fas fa-grin-alt",
            "fas fa-grin-beam",
            "fas fa-grin-beam-sweat",
            "fas fa-grin-hearts",
            "fas fa-grin-squint",
            "fas fa-grin-squint-tears",
            "fas fa-grin-stars",
            "fas fa-grin-tears",
            "fas fa-grin-tongue",
            "fas fa-grin-tongue-squint",
            "fas fa-grin-tongue-wink",
            "fas fa-grin-wink",
            "fas fa-kiss",
            "fas fa-kiss-beam",
            "fas fa-kiss-wink-heart",
            "fas fa-laugh",
            "fas fa-laugh-beam",
            "fas fa-laugh-squint",
            "fas fa-laugh-wink",
            "fas fa-meh",
            "fas fa-meh-blank",
            "fas fa-meh-rolling-eyes",
            "fas fa-sad-cry",
            "fas fa-sad-tear",
            "fas fa-smile",
            "fas fa-smile-beam",
            "fas fa-smile-wink",
            "fas fa-surprise",
            "fas fa-tired"
        };

        [HttpGet]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> ObtenerParaEditar(int secuencial)
        {
            var gResponse = new GenericResponse<MenuEditarVM>();
            try
            {
                var menu = await _menuServices.ObtenerPorId(secuencial);
                var menusPadre = await _menuServices.ObtenerTodosPadre();

                var viewModel = new MenuEditarVM
                {
                    Menu = _mapper.Map<MenuVM>(menu),
                    ListaMenusPadre = _mapper.Map<List<MenuVM>>(menusPadre),
                    IconosDisponibles = _iconosFontAwesome
                };

                gResponse.Estado = true;
                gResponse.Objeto = viewModel;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
            }

            var jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            return new JsonResult(gResponse, jsonOptions);
        }

        [HttpGet]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> Lista()
        {
            var gResponse = new GenericResponse<List<MenuTableVM>>();
            try
            {
                var menus = await _menuServices.ObtenerTodosParaAdministracion();

                // Crear un diccionario para buscar descripciones de menús por su secuencial
                var menuDescriptions = menus.ToDictionary(m => m.Secuencial, m => m.Descripcion);

                var menuTableVMs = new List<MenuTableVM>();

                foreach (var menu in menus)
                {
                    var menuTableVM = new MenuTableVM
                    {
                        Secuencial = menu.Secuencial,
                        Orden = menu.Orden,
                        Descripcion = menu.Descripcion,
                        Icono = menu.Icono,
                        Controlador = menu.Controlador,
                        PaginaAccion = menu.PaginaAccion,
                        EsActivo = menu.EsActivo,
                        MostrarEnMenu = menu.MostrarEnMenu
                    };

                    if (menu.SecMenuPadre.HasValue && menu.SecMenuPadre.Value != menu.Secuencial)
                    {
                        if (menuDescriptions.TryGetValue(menu.SecMenuPadre.Value, out string parentDescription))
                        {
                            menuTableVM.DescripcionMenuPadre = parentDescription;
                        }
                    }
                    else
                    {
                        menuTableVM.DescripcionMenuPadre = ""; // Menú principal o sin padre
                    }
                    menuTableVMs.Add(menuTableVM);
                }

                gResponse.Estado = true;
                gResponse.Objeto = menuTableVMs;
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
        public async Task<IActionResult> ProcesaGuardarMenu([FromBody] MenuVM menuVM)
        {
            var gResponse = new GenericResponse<MenuVM>();
            try
            {
                if (menuVM.Secuencial != 0 && menuVM.Secuencial == menuVM.SecMenuPadre)
                {
                    gResponse.Estado = false;
                    gResponse.Mensajes = "Un menú no puede ser su propio menú padre.";
                    return StatusCode(StatusCodes.Status400BadRequest, gResponse);
                }

                var menu = _mapper.Map<Menu>(menuVM);
                Menu menuProcesado;

                if (menu.Secuencial == 0)
                {
                    menuProcesado = await _menuServices.Crear(menu);
                }
                else
                {
                    menuProcesado = await _menuServices.Editar(menu);
                }

                gResponse.Estado = true;
                gResponse.Objeto = _mapper.Map<MenuVM>(menuProcesado);
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
                gResponse.Estado = await _menuServices.Eliminar(secuencial);
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpGet]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> ObtenerMenusPadre()
        {
            var gResponse = new GenericResponse<List<MenuVM>>();
            try
            {
                var menus = await _menuServices.ObtenerTodosPadre();
                var menuVMs = _mapper.Map<List<MenuVM>>(menus);
                gResponse.Estado = true;
                gResponse.Objeto = menuVMs;
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