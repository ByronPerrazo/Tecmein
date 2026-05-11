using BLL.Interfaces;
using DAL.Interfaces;
using Entity;

namespace BLL.Implementacion
{
    public class FormaPagoServices : IFormaPagoServices
    {
        private readonly IGenericRepository<FormaPago> _repositorio;

        public FormaPagoServices(IGenericRepository<FormaPago> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<List<FormaPago>> Lista()
        {
            IQueryable<FormaPago> query = await _repositorio.Consultar();
            return query.ToList();
        }

        public async Task<FormaPago> Obtener(int secFormaPago)
        {
            return await _repositorio.Obtener(f => f.SecFormaPago == secFormaPago);
        }

        public async Task<FormaPago> Crear(FormaPago entidad)
        {
            if (entidad == null) throw new ArgumentNullException(nameof(entidad));
            entidad.EstaActivo = 1; // Asignar 1 para activo (short)
            var formaPagoCreada = await _repositorio.Crear(entidad);
            return formaPagoCreada;
        }

        public async Task<FormaPago> Editar(FormaPago entidad)
        {
            if (entidad == null) throw new ArgumentNullException(nameof(entidad));

            var formaPagoExistente = await _repositorio.Obtener(f => f.SecFormaPago == entidad.SecFormaPago);
            if (formaPagoExistente == null)
            {
                throw new Exception("La forma de pago no existe.");
            }

            formaPagoExistente.Descripcion = entidad.Descripcion;
            formaPagoExistente.EstaActivo = entidad.EstaActivo;

            await _repositorio.Editar(formaPagoExistente);
            return formaPagoExistente;
        }

        public async Task<bool> Eliminar(int secFormaPago)
        {
            var formaPago = await _repositorio.Obtener(f => f.SecFormaPago == secFormaPago);
            if (formaPago == null)
            {
                throw new Exception("La forma de pago no existe.");
            }

            return await _repositorio.Eliminar(formaPago);
        }
    }
}
