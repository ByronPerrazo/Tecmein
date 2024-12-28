using BLL.Interfaces;
using DAL.Interfaces;
using Entity;

namespace BLL.Implementacion
{
    public class TipoProductoServices : ITipoProductoServices
    {
        private IGenericRepository<TipoProducto> _repositorio;
        public TipoProductoServices(IGenericRepository<TipoProducto> repositorio)
        {
            _repositorio = repositorio;
        }


        public async Task<List<TipoProducto>> Lista()
        {
            var query = await _repositorio.Consultar();
            return [.. query];
        }

        public Task<TipoProducto> TipoProductoPorSecuencial(int secuencial)
        {
            return _repositorio.Obtener(x => x.Secuencial == secuencial);
        }
        public async Task<TipoProducto> Crea(TipoProducto entidad)
        {
            try
            {
                var tipoProducto = await _repositorio.Crear(entidad);

                return tipoProducto;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<TipoProducto> Editar(TipoProducto entidad)
        {
            try
            {
                var registroDb = _repositorio.Obtener(x => x.Secuencial == entidad.Secuencial).Result
                    ?? throw new TaskCanceledException("Registro No Existe");

                registroDb.Nombre = entidad.Nombre;
                registroDb.Descripcion = entidad.Descripcion;
                registroDb.EstaActivo = entidad.EstaActivo;

                var tipoProducto = await _repositorio.Editar(registroDb);

                //return tipoProducto;

                if (tipoProducto)
                    registroDb = await _repositorio.Obtener(x => x.Secuencial == entidad.Secuencial);
                else
                    throw new TaskCanceledException("Error el Registrio no se puede guardar");

                return registroDb;

            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> Eliminar(int secuencial)
        {
            try
            {
                var seElimino = false;
                var tipoProducto
                    = await _repositorio
                             .Consultar(x => x.Secuencial == secuencial);

                var tipo = tipoProducto.FirstOrDefault();
                if (tipo != null)
                {
                    var usuarioGenerado = await _repositorio.Eliminar(tipo);
                    seElimino = true;
                }
                return seElimino;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
