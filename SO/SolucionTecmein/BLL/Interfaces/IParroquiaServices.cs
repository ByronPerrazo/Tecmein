using Entity;

namespace BLL.Interfaces
{
    public interface IParroquiaServices
    {
        Task<List<Parroquia>> Lista();
        Task<List<Parroquia>> ListaPorCanton(int secCanton);

        Task<Parroquia> ParroquiasPorSecuencial(int secuencialProvincia);
    }
}
