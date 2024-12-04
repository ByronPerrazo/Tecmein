using Entity;

namespace BLL.Interfaces
{
    public interface IContactoVisitaServices
    {
        Task<List<Contactovisita>> ListaContactoVisita();
        Task<List<Contactovisita>> ListaPorContacto(int secContacto);
        Task<Contactovisita?> ProcesaGuardarContactoVisita(Contactovisita entidad);
        Task<Contactovisita?> ContactoVisitaPorVisita(int secVisita);
        Task<Contactovisita> CrearContactoVisita(Contactovisita entidad);
        Task<Contactovisita> EditarContactoVisita(Contactovisita entidad);
        Task<bool> EliminarContactoVisita(int secuencial);

    }
}
