using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;

namespace BLL.Implementacion
{
    public class ProvinciaServices : IProvinciaServices
    {
        private readonly IGenericRepository<Provincia> _repositorio;
        public ProvinciaServices(IGenericRepository<Provincia> repositorio)
        {
            _repositorio = repositorio;
        }


        public async Task<List<Provincia>> Lista()
        {
            var query = _repositorio.Consultar();
            return await query.Result.ToListAsync();
        }

        public async Task<Provincia?> ProvinciaPorSecuencial(int secuencial)
        {
            return await _repositorio.Obtener(x => x.Secuencial == secuencial);
        }


    }
}
