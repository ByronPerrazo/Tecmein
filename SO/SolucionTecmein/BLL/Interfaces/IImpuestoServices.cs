using Entity;

namespace BLL.Interfaces
{
    public interface IImpuestoServices
    {
        Task<List<Impuesto>> Lista();
        Task<List<Impuesto>> ListaActivos();
        Task<Impuesto> Crear(Impuesto entidad);
        Task<Impuesto> Editar(Impuesto entidad);
        Task<bool> Eliminar(int id);
        Task<Impuesto> ObtenerPorId(int id);
    }
}
