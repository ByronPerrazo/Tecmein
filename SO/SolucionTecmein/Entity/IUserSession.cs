namespace Entity
{
    public interface IUserSession
    {
        int? SecUsuario { get; }
        int? SecRol { get; }
        bool IsAuthenticated { get; }
    }
}
