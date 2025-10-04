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

        public async Task<bool> TienePermiso(int secRol, string permiso)
        {
            var cacheKey = $"Permiso_{secRol}_{permiso}";

            if (_cache.TryGetValue(cacheKey, out bool tienePermiso))
            {
                return tienePermiso;
            }

            tienePermiso = await _context.RolPermisos
                .AnyAsync(rp => rp.SecRol == secRol && rp.IdPermiso == permiso);

            _cache.Set(cacheKey, tienePermiso, TimeSpan.FromMinutes(5)); // Cache por 5 minutos

            return tienePermiso;
        }
    }
}
