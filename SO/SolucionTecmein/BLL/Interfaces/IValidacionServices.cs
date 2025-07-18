namespace BLL.Interfaces
{
    public interface IValidacionServices
    {
        void ValidarCorreo(string? correo);
        void ValidarTelefonoEcuador(string? telefono);
        void ValidarNombre(string? nombre, string campo);
    }
}
