using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;

namespace BLL.Implementacion
{
    public class RolMenuServices : IRolMenuServices
    {
        private readonly IGenericRepository<RolMenu> _repository;

        public RolMenuServices(IGenericRepository<RolMenu> repository)
        {
            _repository = repository;
        }

        public async Task<List<RolMenu>> Lista()
        {
            IQueryable<RolMenu> query = _repository.Consultar().Result;
            return await query.Include(r => r.SecRolNavigation).Include(m => m.SecMenuNavigation).ToListAsync();
        }

        public async Task<RolMenu> ObtenerPorId(int idRolMenu)
        {
            return await _repository.Obtener(rm => rm.Secuencial == idRolMenu);
        }

        public async Task<RolMenu> Crear(RolMenu entidad)
        {
            return await _repository.Crear(entidad);
        }

        public async Task<RolMenu> Editar(RolMenu entidad)
        {
            await _repository.Editar(entidad);
            return entidad;
        }

        public async Task<bool> Eliminar(int idRolMenu)
        {
            var rolMenu = await _repository.Obtener(rm => rm.Secuencial == idRolMenu);
            if (rolMenu == null)
                return false;

            return await _repository.Eliminar(rolMenu);
        }
    }
}
