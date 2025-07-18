using Entity;

namespace BLL.Interfaces
{
    public interface IContactoServices
    {
        Task<Contacto> ObtenerPorId(int secuencial);
        Task<List<Contacto>> Lista();
        Task<List<Contacto>> ListaPorConstructora(int secConstructora);
        Task<Contacto> ContactoPorSecuencial(int secuencial);
        Task<Contacto> ObtenerContactoPrincipal(int secuencialVisita);
        Task<Contacto> Crear(Contacto entidad);
        Task<Contacto> Editar(Contacto entidad);
        Task<bool> Eliminar(int secuencial);
    }
}
