namespace BLL.Interfaces
{
    public interface ICorreoServices
    {

        Task<bool> EnvioCorreo(string? Destino, string Asunto, string Mensaje);
    }
}
