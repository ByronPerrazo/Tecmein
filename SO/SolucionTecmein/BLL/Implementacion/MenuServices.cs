using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;

namespace BLL.Implementacion
{
    public class MenuServices : IMenuServices
    {
        private readonly IGenericRepository<Menu> _repositorioMenu;
        private readonly IGenericRepository<RolPermiso> _repositorioRolPermiso; // CAMBIO: Nueva dependencia
        private readonly IGenericRepository<Usuario> _repositorioUsuario;
        private readonly IGenericRepository<RolMenu> _repositorioRolMenu;


        public MenuServices(IGenericRepository<Menu> repositorioMenu,
                            IGenericRepository<RolPermiso> repositorioRolPermiso, // CAMBIO: Nueva dependencia
                            IGenericRepository<Usuario> repositorioUsuario,
                            IGenericRepository<RolMenu> repositorioRolMenu)
        {
            _repositorioMenu = repositorioMenu;
            _repositorioRolPermiso = repositorioRolPermiso; // CAMBIO
            _repositorioUsuario = repositorioUsuario;
            _repositorioRolMenu = repositorioRolMenu;
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

            // =================== INICIO DE LA LÓGICA CORREGIDA ===================

            // 1. Obtener todos los permisos de visualización de menú para el rol del usuario.
            var permisosQuery = await _repositorioRolPermiso.Consultar(p => p.SecRol == usuario.SecRol && p.IdPermiso.EndsWith("_VIEWMENU"));
            
            // 2. Extraer la raíz del permiso (ej: "USUARIO" de "USUARIO_VIEWMENU") y guardarla en un HashSet para búsqueda eficiente.
            var permisosDeVisualizacion = (await permisosQuery.Select(p => p.IdPermiso).ToListAsync())
                .Select(p => p.Replace("_VIEWMENU", "").ToUpper())
                .ToHashSet();

            // 3. Obtener todos los menús activos para referencia.
            var todosLosMenusActivos = await (await _repositorioMenu.Consultar(m => m.EsActivo == 1)).ToListAsync();
            var todosLosMenusDict = todosLosMenusActivos.ToDictionary(m => m.Secuencial);

            // 4. Encontrar los menús que coinciden con los permisos del usuario.
            //    Se compara la raíz del permiso (ej: "USUARIO") con el nombre del controlador del menú.
            var menusConPermisoDirecto = todosLosMenusActivos
                .Where(m => !string.IsNullOrEmpty(m.Controlador) && permisosDeVisualizacion.Contains(m.Controlador.ToUpper()))
                .ToList();
            
            var idsMenusDirectos = menusConPermisoDirecto.Select(m => m.Secuencial).ToList();

            // =================== FIN DE LA LÓGICA CORREGIDA ===================


            // 5. Construir la lista final de menús a mostrar, incluyendo todos los ancestros para evitar "hijos huérfanos".
            //    (Esta lógica de negocio se mantiene intacta)
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

            // 6. Organizar la lista plana en una jerarquía (árbol) para la vista.
            //    (Esta lógica de negocio se mantiene intacta)
            var menusFinales = new List<Menu>();
            var menusProcesados = menusAMostrar.Values.ToList();

            foreach (var menu in menusProcesados)
            {
                // Limpiar la navegación para evitar ciclos o datos incorrectos de iteraciones anteriores.
                menu.InverseSecMenuPadreNavigation = new List<Menu>();
            }

            foreach (var menu in menusProcesados.OrderBy(m => m.Secuencial))
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
            IQueryable<Menu> query = await _repositorioMenu.Consultar(m => m.EsActivo == 1);
            return await query.ToListAsync();
        }

        public async Task<List<Menu>> ObtenerTodosParaAdministracion()
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

                // Find and delete related RolMenu entries
                var rolMenusAsociadosQuery = await _repositorioRolMenu.Consultar(rm => rm.SecMenu == secuencial);
                var rolMenusAsociados = await rolMenusAsociadosQuery.ToListAsync();

                foreach (var rolMenu in rolMenusAsociados)
                {
                    await _repositorioRolMenu.Eliminar(rolMenu);
                }

                // Now delete the menu
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
