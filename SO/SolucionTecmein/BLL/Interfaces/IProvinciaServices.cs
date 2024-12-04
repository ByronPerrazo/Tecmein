using Entity;

namespace BLL.Interfaces
{
    public interface IProvinciaServices
    {
        Task<List<Provincia>> Lista();
        Task<Provincia> ProvinciaPorSecuencial(int secuencial);

    }
}
