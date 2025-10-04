using DAL.DBContext;
using Microsoft.EntityFrameworkCore;

namespace BLL.Implementacion
{
    public class AutorizacionService
    {
        private readonly TecmeindbContext _context;

        public AutorizacionService(TecmeindbContext context)
        {
            _context = context;
        }

        public async Task<bool> TienePermiso(int secRol, string permiso)
        {
            return await _context.RolPermisos
                .AnyAsync(rp => rp.SecRol == secRol && rp.IdPermiso == permiso);
        }
    }
}
