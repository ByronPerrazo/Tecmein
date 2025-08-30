using BLL.Interfaces;
using DAL.Interfaces;
using System.Linq.Expressions;

namespace BLL.Implementacion
{
    public class GenericService<T> : IGenericService<T> where T : class
    {
        private readonly IGenericRepository<T> _repository;

        public GenericService(IGenericRepository<T> repository)
        {
            _repository = repository;
        }

        public async Task<T> Obtener(Expression<Func<T, bool>> filtro)
        {
            return await _repository.Obtener(filtro);
        }

        public async Task<T> Crear(T entidad)
        {
            return await _repository.Crear(entidad);
        }

        public async Task<bool> Editar(T entidad)
        {
            return await _repository.Editar(entidad);
        }

        public async Task<bool> Eliminar(T entidad)
        {
            return await _repository.Eliminar(entidad);
        }

        public async Task<IQueryable<T>> Consultar(Expression<Func<T, bool>>? filtro = null)
        {
            return await _repository.Consultar(filtro);
        }
    }
}
