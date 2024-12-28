using Entity;

namespace BLL.Interfaces
{
    public interface IConstructoraServices
    {
        Task<List<Constructora>> Lista();
        Task<Constructora> ConstructoraPorSecuencial(int secuencial);
        Task<Constructora> GuardarCambios(Constructora entidad);
        Task<Constructora> Editar(Constructora entidad);
        Task<bool> Eliminar(int secuencial);
    }
}
