using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;

namespace BLL.Implementacion
{
    public class ParroquiaServices : IParroquiaServices
    {
        private readonly IGenericRepository<Parroquia> _repositorio;
        public ParroquiaServices(IGenericRepository<Parroquia> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<List<Parroquia>> Lista()
        {
            var query = await _repositorio.Consultar();
            return [.. query.Include(x => x.SecCantonNavigation)
                            .ThenInclude(y=>y.SecProvinciaNavigation)];
        }

        public async Task<List<Parroquia>> ListaPorCanton(int secCanton)
        {
            var query = await _repositorio.Consultar(x => x.SecCanton == secCanton);
            return [.. query.Include(x => x.SecCantonNavigation)
                            .ThenInclude(y=>y.SecProvinciaNavigation)];
        }

        public async Task<Parroquia> ParroquiasPorSecuencial(int secuencial)
        {
            var parroquia =
                await _repositorio
                      .Consultar(x => x.Secuencial == secuencial);
            parroquia.Include(x => x.SecCantonNavigation)
                     .ThenInclude(y => y.SecProvinciaNavigation);

            return (Parroquia)parroquia;
        }
    }
}
