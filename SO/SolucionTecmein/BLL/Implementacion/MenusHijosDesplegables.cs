using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Implementacion
{
    public class MenusHijosDesplegables : IMenusHijosDesplegables
    {
        private readonly IGenericRepository<Menu> _repositorioMenu;

        public MenusHijosDesplegables(IGenericRepository<Menu> repositorioMenu)
        {
            _repositorioMenu = repositorioMenu;
        }

        public async Task<List<Menu>> ObtenerMenusHijos()
        {
            // Obtener todos los menús activos
            var todosLosMenusActivos = await _repositorioMenu.Consultar(m => m.EsActivo == 1);

            // Obtener los secuenciales de todos los menús que son padres (tienen hijos activos)
            var secuencialesDePadres = todosLosMenusActivos
                                            .Select(m => m.Secuencial)
                                            .Where(menuId => todosLosMenusActivos.Any(hijo => hijo.SecMenuPadre == menuId))
                                            .Distinct();

            // Filtrar los menús que no son padres de ningún otro menú activo
            var menusHijosDeUltimoNivel = await todosLosMenusActivos
                                            .Where(m => !secuencialesDePadres.Contains(m.Secuencial))
                                            .ToListAsync();

            return menusHijosDeUltimoNivel;
        }
    }
}
