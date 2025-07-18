using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;

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
        public async Task<Menu> ObtenerPorId(int secuencial)
        {
            return await _repositorioMenu.Obtener(m => m.Secuencial == secuencial);
        }

        public async Task<List<Menu>> ObtenerTodosPadre()
        {
            IQueryable<Menu> query = await _repositorioMenu.Consultar(m => m.SecMenuPadre == null || m.SecMenuPadre == m.Secuencial);
            return await query.ToListAsync();
        }

        public async Task<List<Menu>> ObtieneMenu(int secuencialUsuario)
        {
            var usuario = await _repositorioUsuario.Obtener(u => u.Secuencial == secuencialUsuario);
            if (usuario == null || usuario.SecRol == null) return new List<Menu>();

            // 1. Obtener todos los menús activos para referencia y ponerlos en un diccionario para búsqueda O(1).
            var todosLosMenusActivos = await (await _repositorioMenu.Consultar(m => m.EsActivo == 1)).ToListAsync();
            var todosLosMenusDict = todosLosMenusActivos.ToDictionary(m => m.Secuencial);

            // 2. Obtener los Ids de menú a los que el usuario tiene permiso directo.
            var rolMenuQuery = await _repositorioRolMenu.Consultar(rm => rm.SecRol == usuario.SecRol && rm.EsActivo == 1 && rm.SecMenu != null);
            var idsMenusDirectos = await rolMenuQuery.Select(rm => rm.SecMenu.Value).ToListAsync();

            // 3. Construir la lista final de menús a mostrar, incluyendo todos los ancestros para evitar "hijos huérfanos".
            var menusAMostrar = new Dictionary<int, Menu>();
            foreach (var idMenu in idsMenusDirectos)
            {
                var menuActual = todosLosMenusDict.GetValueOrDefault(idMenu);
                // Escalar hacia arriba en el árbol para agregar a los padres.
                while (menuActual != null && !menusAMostrar.ContainsKey(menuActual.Secuencial))
                {
                    menusAMostrar.Add(menuActual.Secuencial, menuActual);
                    menuActual = menuActual.SecMenuPadre.HasValue ? todosLosMenusDict.GetValueOrDefault(menuActual.SecMenuPadre.Value) : null;
                }
            }

            // 4. Organizar la lista plana en una jerarquía (árbol) para la vista.
            var menusFinales = new List<Menu>();
            var menusProcesados = menusAMostrar.Values.ToList();

            foreach (var menu in menusProcesados)
            {
                // Limpiar la navegación para evitar ciclos o datos incorrectos de iteraciones anteriores.
                menu.InverseSecMenuPadreNavigation = new List<Menu>(); 
            }

            foreach (var menu in menusProcesados.OrderBy(m=>m.Secuencial))
            {
                if (menu.SecMenuPadre.HasValue && menusAMostrar.ContainsKey(menu.SecMenuPadre.Value))
                {
                    var padre = menusAMostrar[menu.SecMenuPadre.Value];
                    padre.InverseSecMenuPadreNavigation.Add(menu);
                }
                else
                {
                    // Es un menú raíz.
                    menusFinales.Add(menu);
                }
            }

            return menusFinales;
        }

        public async Task<List<Menu>> ObtieneMenuTotal()
        {
            IQueryable<Menu> query = await _repositorioMenu.Consultar();
            return await query.ToListAsync();
        }

        public async Task<Menu> Crear(Menu entidad)
        {
            try
            {
                return await _repositorioMenu.Crear(entidad);
            }
            catch
            {
                throw;
            }
        }

        public async Task<Menu> Editar(Menu entidad)
        {
            try
            {
                var menuEncontrado = await _repositorioMenu.Obtener(m => m.Secuencial == entidad.Secuencial);

                if (menuEncontrado == null)
                    throw new TaskCanceledException("El menú no existe");

                menuEncontrado.Descripcion = entidad.Descripcion;
                menuEncontrado.SecMenuPadre = entidad.SecMenuPadre;
                menuEncontrado.Icono = entidad.Icono;
                menuEncontrado.Controlador = entidad.Controlador;
                menuEncontrado.PaginaAccion = entidad.PaginaAccion;
                menuEncontrado.EsActivo = entidad.EsActivo;

                bool respuesta = await _repositorioMenu.Editar(menuEncontrado);

                if (!respuesta)
                    throw new TaskCanceledException("No se pudo editar el menú");

                return menuEncontrado;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Eliminar(int secuencial)
        {
            try
            {
                var menuEncontrado = await _repositorioMenu.Obtener(m => m.Secuencial == secuencial);

                if (menuEncontrado == null)
                    throw new TaskCanceledException("El menú no existe");

                bool respuesta = await _repositorioMenu.Eliminar(menuEncontrado);

                if (!respuesta)
                    throw new TaskCanceledException("No se pudo eliminar el menú");

                return respuesta;
            }
            catch
            {
                throw;
            }
        }
    }
}
