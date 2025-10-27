using DAL.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace BLL.Implementacion
{
    public class AutorizacionService
    {
        private readonly TecmeindbContext _context;
        private readonly IMemoryCache _cache;

        public AutorizacionService(TecmeindbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<bool> TienePermiso(int secRol, string accion, string controllerName)
        {
            var cacheKey = $"Permiso_{secRol}_{controllerName}_{accion}";

            if (_cache.TryGetValue(cacheKey, out bool tienePermisoCache))
            {
                return tienePermisoCache;
            }

            // Buscar el menú que corresponde al controlador
            var menu = await _context.Menus.FirstOrDefaultAsync(m => m.Controlador == controllerName);
            if (menu == null)
            {
                // Si no hay un menú para este controlador, no se concede el permiso
                return false;
            }

            // Buscar la configuración de permisos en la tabla RolMenu
            var rolMenu = await _context.RolMenus
                .FirstOrDefaultAsync(rm => rm.SecRol == secRol && rm.SecMenu == menu.Secuencial);

            if (rolMenu == null)
            {
                // Si no hay una entrada en RolMenu, no hay permisos para este menú.
                return false;
            }

            bool tienePermiso = accion.ToUpper() switch
            {
                "VER_MENU" => rolMenu.VerMenu,
                "CREAR" => rolMenu.Crear,
                "LEER" => rolMenu.Leer,
                "ACTUALIZAR" => rolMenu.Actualizar,
                "ELIMINAR" => rolMenu.Eliminar,
                _ => false, // Si la acción no es una de las 4, se deniega.
            };

            _cache.Set(cacheKey, tienePermiso, TimeSpan.FromMinutes(5)); // Cache por 5 minutos

            return tienePermiso;
        }
    }
}
