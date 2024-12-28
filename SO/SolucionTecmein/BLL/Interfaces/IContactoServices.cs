using Entity;

namespace BLL.Interfaces
{
    public interface IContactoServices
    {
        Task<List<Contacto>> Lista();
        Task<List<Contacto>> ListaPorConstructora(int secConstructora);
        Task<Contacto> ContactoPorSecuencial(int secuencial);
        Task<Contacto> Crear(Contacto entidad);
        Task<Contacto> Editar(Contacto entidad);
        Task<bool> Eliminar(int secuencial);
    }
}
