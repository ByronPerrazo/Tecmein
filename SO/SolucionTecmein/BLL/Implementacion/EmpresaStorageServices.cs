using BLL.Interfaces;
using DAL.Implementacion;
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
    public class EmpresaStorageServices(IGenericRepository<Empresastorage> repositorio) : IEmpresaStorageServices
    {
        private readonly IGenericRepository<Empresastorage> _repositorio = repositorio;

        public Task<Empresastorage> Obtener(int secuencialEmpresa)
            => _repositorio.Obtener(x => x.SecEmpresa == secuencialEmpresa);

        public async Task<List<Empresastorage>> Consultar()
        {
            var query = await _repositorio.Consultar();
           return [.. query.Include(x => x.SecEmpresaNavigation)];
        }
        public async Task<Empresastorage> ProcesaGuardar(Empresastorage empresaStorage)
        {
            try
            {
                var empresaRegistro 
                    =_repositorio
                        .Obtener(x => x.SecEmpresa == empresaStorage.SecEmpresa)
                        .Result;

                if (empresaRegistro != null)
                    await _repositorio.Editar(empresaRegistro);
                else
                    empresaRegistro = _repositorio.Crear(empresaStorage).Result;

                return empresaRegistro;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
