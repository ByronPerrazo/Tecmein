using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Implementacion
{
    public class MenuServices : IMenuServices
    {
        private readonly IGenericRepository<Menu> _repositorioMenu;
        private readonly IGenericRepository<RolMenu> _repositorioRolMenu;
        private readonly IGenericRepository<Usuario> _repositorioUsuario;

        public MenuServices(IGenericRepository<Menu> repositorioMenu,
                            IGenericRepository<RolMenu> repositorioRolMenu,
                            IGenericRepository<Usuario> repositorioUsuario)
        {
            _repositorioMenu = repositorioMenu;
            _repositorioRolMenu = repositorioRolMenu;
            _repositorioUsuario = repositorioUsuario;
        }
        public async Task<List<Menu>> ObtieneMenu(int secuencialUsuario)
        {

            IQueryable<Menu> listaMenu = await _repositorioMenu.Consultar(x => x.EsActivo == 1);
            IQueryable<RolMenu> listaRolMenu = await _repositorioRolMenu.Consultar(x => x.EsActivo == 1);
            IQueryable<Usuario> usuarioMenu = await _repositorioUsuario.Consultar(x => x.Secuencial == secuencialUsuario && x.EsActivo == 1);

            var menuPadre = ( from um in usuarioMenu join rm in  listaRolMenu on  um.SecRol equals rm.SecRol
                              join me in listaMenu on rm.SecMenu equals me.Secuencial
                              join mPadre in listaMenu on me.SecMenuPadre equals mPadre.Secuencial
                              select mPadre)
                              .Distinct()
                              .AsQueryable();

            var menuHijos = (from um in usuarioMenu
                             join rm in listaRolMenu on um.SecRol equals rm.SecRol
                             join me in listaMenu on rm.SecMenu equals me.Secuencial
                             where me.Secuencial != me.SecMenuPadre
                             select me)
                              .Distinct()
                              .AsQueryable();

            var menuUsuario = (from mPadre in menuPadre
                               select new Menu()
                               {
                                   Descripcion = mPadre.Descripcion,
                                   Icono = mPadre.Icono,
                                   Controlador = mPadre.Controlador,
                                   PaginaAccion = mPadre.PaginaAccion,
                                   InverseSecMenuPadreNavigation 
                                        = (from mhijo in menuHijos 
                                           where mhijo.SecMenuPadre == mPadre.SecMenuPadre 
                                           select mhijo).ToList()
                               }
                               ).ToList();

            return menuUsuario;
        }
    }
}
