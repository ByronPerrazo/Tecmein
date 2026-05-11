using Entity;

namespace BLL.Interfaces
{
    public interface ITipoImpuestoServices
    {
        Task<List<TipoImpuesto>> Lista();
        Task<TipoImpuesto> Crear(TipoImpuesto entidad);
        Task<TipoImpuesto> Editar(TipoImpuesto entidad);
        Task<bool> Eliminar(int id);
        Task<TipoImpuesto> ObtenerPorId(int id);
    }
}