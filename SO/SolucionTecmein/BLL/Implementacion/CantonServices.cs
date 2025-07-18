using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;

namespace BLL.Implementacion
{
    public class CantonServices : ICantonServices
    {
        private readonly IGenericRepository<Canton> _repositorio;
        public CantonServices(IGenericRepository<Canton> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<List<Canton>> Lista()
        {
            var query = await _repositorio.Consultar();
            return query.Include(x => x.SecProvinciaNavigation).ToList();
        }

        public async Task<List<Canton>> ListaPorProvincia(int secProvincia)
        {
            var query = await _repositorio.Consultar(x => x.SecProvincia == secProvincia);
            return query.Include(x => x.SecProvinciaNavigation).ToList();
        }

        public Task<Canton?> CantonPorSecuencial(int secuencial)
        {
            return _repositorio.Obtener(x => x.Secuencial == secuencial);
        }

    }
}
