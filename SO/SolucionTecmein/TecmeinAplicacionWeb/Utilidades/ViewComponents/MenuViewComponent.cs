using AutoMapper;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TecmeinAplicacionWeb.Models.ViewModels;

namespace TecmeinWebApp.Utilidades.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly IMenuServices _menuServicio;
        private readonly IMapper _mapper;

        public MenuViewComponent(IMenuServices menuServices, IMapper mapper)
        {
            _mapper = mapper;
            _menuServicio = menuServices;
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

                var todosLosMenus = await _menuServicio.ObtieneMenuTotal();
                var rolMenusPermitidos = await _menuServicio.ObtenerRolMenusPorRol(idRol);

                var idsMenusConPermisoVer = rolMenusPermitidos
                                             .Where(rm => rm.VerMenu)
                                             .Select(rm => rm.SecMenu)
                                             .ToHashSet();

                var idsFinalesParaMostrar = new HashSet<int>(idsMenusConPermisoVer);
                foreach (var idMenu in idsMenusConPermisoVer)
                {
                    var menuActual = todosLosMenus.FirstOrDefault(m => m.Secuencial == idMenu);
                    while (menuActual?.SecMenuPadre.HasValue == true)
                    {
                        int idPadre = menuActual.SecMenuPadre.Value;
                        if (idsFinalesParaMostrar.Contains(idPadre)) break;

                        idsFinalesParaMostrar.Add(idPadre);
                        menuActual = todosLosMenus.FirstOrDefault(m => m.Secuencial == idPadre);
                    }
                }

                var menusParaMostrar = todosLosMenus.Where(m => idsFinalesParaMostrar.Contains(m.Secuencial)).ToList();
                var menusVM = _mapper.Map<List<MenuVM>>(menusParaMostrar);

                var menuLookup = menusVM.ToLookup(m => m.SecMenuPadre);

                foreach (var menuVM in menusVM)
                {
                    menuVM.SubMenu = menuLookup[menuVM.Secuencial].OrderBy(m => m.Orden).ToList();
                }

                listaMenu = menuLookup[null].OrderBy(m => m.Orden).ToList();
            }
            else
            {
                listaMenu = new List<MenuVM>();
            }

            return View(listaMenu);
        }
    }
}