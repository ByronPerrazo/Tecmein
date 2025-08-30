using AutoMapper;
using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TecmeinWebApp.Models.ViewModel;

namespace TecmeinWebApp.Utilidades.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly IMenuServices _menuServicio;
        private readonly IGenericRepository<RolMenu> _repositorioRolMenu;
        private readonly IMapper _mapper;

        public MenuViewComponent(IMenuServices menuServices, IMapper mapper, IGenericRepository<RolMenu> repositorioRolMenu)
        {
            _mapper = mapper;
            _menuServicio = menuServices;
            _repositorioRolMenu = repositorioRolMenu;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            ClaimsPrincipal claimsUser = HttpContext.User;
            List<MenuVM> listaMenu;

            if (claimsUser.Identity.IsAuthenticated)
            {
                string idRolString = claimsUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (string.IsNullOrEmpty(idRolString) || !int.TryParse(idRolString, out int idRol))
                {
                    return View(new List<MenuVM>());
                }

                // 1. Obtener todos los menús de la BD una sola vez para optimizar.
                var todosLosMenus = await _menuServicio.ObtieneMenuTotal();
                var todosLosMenusDict = todosLosMenus.ToDictionary(m => m.Secuencial);

                // 2. Obtener los IDs de los menús explícitamente permitidos para el rol.
                var idsMenusPermitidos = (await _repositorioRolMenu.Consultar(rm => rm.SecRol == idRol))
                                             .Select(rm => rm.SecMenu.Value)
                                             .ToHashSet();

                // 3. Calcular la lista final de menús a mostrar, incluyendo los padres.
                var idsFinalesParaMostrar = new HashSet<int>(idsMenusPermitidos);
                foreach (var idMenuPermitido in idsMenusPermitidos)
                {
                    Menu menuActual = todosLosMenusDict.GetValueOrDefault(idMenuPermitido);
                    while (menuActual?.SecMenuPadre.HasValue == true)
                    {
                        int idPadre = menuActual.SecMenuPadre.Value;
                        idsFinalesParaMostrar.Add(idPadre);
                        menuActual = todosLosMenusDict.GetValueOrDefault(idPadre);
                    }
                }

                // 4. Filtrar la lista original con el conjunto completo de IDs (hijos + padres).
                var menusParaMostrar = todosLosMenus.Where(m => idsFinalesParaMostrar.Contains(m.Secuencial)).ToList();

                // 5. Mapear a ViewModel y construir la jerarquía final.
                var menusVM = _mapper.Map<List<MenuVM>>(menusParaMostrar);
                var menuDictVM = menusVM.ToDictionary(m => m.Secuencial);
                var menusRaiz = new List<MenuVM>();

                foreach (var menuVM in menusVM)
                {
                    menuVM.SubMenu = new List<MenuVM>();
                }

                foreach (var menuVM in menusVM)
                {
                    if (menuVM.SecMenuPadre.HasValue && menuDictVM.ContainsKey(menuVM.SecMenuPadre.Value))
                    {
                        menuDictVM[menuVM.SecMenuPadre.Value].SubMenu.Add(menuVM);
                    }
                    else if (!menuVM.SecMenuPadre.HasValue)
                    {
                        menusRaiz.Add(menuVM);
                    }
                }

                listaMenu = menusRaiz;
            }
            else
            {
                listaMenu = new List<MenuVM>();
            }

            return View(listaMenu);
        }
    }
}