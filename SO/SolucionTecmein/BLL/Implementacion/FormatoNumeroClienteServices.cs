using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Implementacion
{
    public class FormatoNumeroClienteServices : IFormatoNumeroClienteServices
    {
        private readonly IGenericRepository<FormatoNumeroCliente> _repositorio;

        public FormatoNumeroClienteServices(IGenericRepository<FormatoNumeroCliente> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<FormatoNumeroCliente> ObtenerPorEmpresa(int secEmpresa)
        {
            return await _repositorio.Obtener(f => f.SecEmpresa == secEmpresa);
        }

        public async Task<FormatoNumeroCliente> Guardar(FormatoNumeroCliente entidad)
        {
            if (entidad == null) throw new ArgumentNullException(nameof(entidad));

            var formatoExistente = await _repositorio.Obtener(f => f.SecEmpresa == entidad.SecEmpresa);

            if (formatoExistente == null)
            {
                // Crear
                return await _repositorio.Crear(entidad);
            }
            else
            {
                // Editar
                formatoExistente.UsaFormato = entidad.UsaFormato;
                formatoExistente.Formato = entidad.Formato;
                formatoExistente.NumeroInicio = entidad.NumeroInicio;
                await _repositorio.Editar(formatoExistente);
                return formatoExistente;
            }
        }
    }
}
