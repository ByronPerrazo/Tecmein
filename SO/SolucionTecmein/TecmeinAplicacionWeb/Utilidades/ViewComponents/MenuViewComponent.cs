using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TecmeinWebApp.Models.ViewModel;

using BLL.Interfaces;



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
                string idUsuario =
                   claimsUser
                   .Claims
                   .Where(x => x.Type == ClaimTypes.NameIdentifier)
                   .Select(x => x.Value).SingleOrDefault();

                listaMenu =
                    _mapper
                        .Map<List<MenuVM>>(await _menuServicio
                                                 .ObtieneMenu(int.Parse(idUsuario)));
            }
            else
                listaMenu = new List<MenuVM>();


            return View(listaMenu);

        }


    }
}
