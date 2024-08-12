using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var query = await _repositorio.Consultar();
            return query.ToList();
        }

        public Task<Provincia> ProvinciaPorSecuencial(int secuencial)
        {
            return _repositorio.Obtener(x => x.Secuencial == secuencial);
        }
       
        
    }
}
