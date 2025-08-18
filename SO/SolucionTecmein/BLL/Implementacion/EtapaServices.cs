using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Implementacion
{
    public class EtapaServices : IEtapaServices
    {
        private readonly IGenericRepository<Etapa> _repositorio;

        public EtapaServices(IGenericRepository<Etapa> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<List<Etapa>> Lista()
        {
            var query = await _repositorio.Consultar();
            return await query.Where(e => e.EstaActivo).OrderBy(e => e.Orden).ToListAsync();
        }

        public async Task<Etapa> ObtenerPorCodigo(string codigo)
        {
            IQueryable<Etapa> query = await _repositorio.Consultar(e => e.Codigo == codigo && e.EstaActivo);
            return await query.FirstOrDefaultAsync();
        }
    }
}
