using BLL.Interfaces;
using Entity;
using DAL.Interfaces;

namespace BLL.Implementacion
{
    public class RolServices : IRolServices
    {

        private IGenericRepository<Rol> _repositorio;
        public RolServices(IGenericRepository<Rol> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<List<Rol>> Lista()
        {
            IQueryable<Rol> query = await _repositorio.Consultar();
            return [.. query];
        }
    }
}
