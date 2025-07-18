using Entity;

namespace BLL.Interfaces
{
    public interface ICantonServices
    {
        Task<List<Canton>> Lista();

        Task<List<Canton>> ListaPorProvincia(int secuencial);

        Task<Canton?> CantonPorSecuencial(int secuencial);
    }
}
