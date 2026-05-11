using BLL.Interfaces;
using DAL.Interfaces;
using Entity;

namespace BLL.Implementacion
{
    public class DiccionarioParametroService : IDiccionarioParametroService
    {
        private readonly IGenericRepository<DiccionarioParametro> _repositorio;

        public DiccionarioParametroService(IGenericRepository<DiccionarioParametro> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<List<DiccionarioParametro>> Lista()
        {
            IQueryable<DiccionarioParametro> query = await _repositorio.Consultar();
            return query.ToList();
        }

        public async Task<DiccionarioParametro> Crear(DiccionarioParametro entidad)
        {
            try
            {
                // Validar que el parámetro no esté vacío
                if (string.IsNullOrWhiteSpace(entidad.Parametro))
                    throw new TaskCanceledException("El parámetro no puede estar vacío.");

                // Validar que el parámetro no exista ya
                var parametroExistente = (await _repositorio.Consultar(p => p.Parametro == entidad.Parametro)).FirstOrDefault();
                if (parametroExistente != null)
                    throw new TaskCanceledException("Ya existe un parámetro con ese nombre.");

                DiccionarioParametro entidadCreada = await _repositorio.Crear(entidad);
                if (entidadCreada.Secuencial == 0)
                    throw new TaskCanceledException("No se pudo crear el parámetro.");

                return entidadCreada;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Editar(DiccionarioParametro entidad)
        {
            try
            {
                var parametroEncontrado = await _repositorio.Obtener(p => p.Secuencial == entidad.Secuencial);
                if (parametroEncontrado == null)
                    throw new TaskCanceledException("El parámetro no fue encontrado.");

                parametroEncontrado.Parametro = entidad.Parametro;
                parametroEncontrado.Descripcion = entidad.Descripcion;
                parametroEncontrado.EstaActivo = entidad.EstaActivo;

                bool respuesta = await _repositorio.Editar(parametroEncontrado);
                if (!respuesta)
                    throw new TaskCanceledException("No se pudo editar el parámetro.");

                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Eliminar(int secuencial)
        {
            try
            {
                var parametroEncontrado = await _repositorio.Obtener(p => p.Secuencial == secuencial);
                if (parametroEncontrado == null)
                    throw new TaskCanceledException("El parámetro no fue encontrado.");

                bool respuesta = await _repositorio.Eliminar(parametroEncontrado);
                if (!respuesta)
                    throw new TaskCanceledException("No se pudo eliminar el parámetro.");

                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<DiccionarioParametro>> ListaActivos()
        {
            IQueryable<DiccionarioParametro> query = await _repositorio.Consultar(p => p.EstaActivo == true);
            return query.ToList();
        }
    }
}
