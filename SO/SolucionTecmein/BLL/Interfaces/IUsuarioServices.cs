using Entity;

namespace BLL.Interfaces
{
    public interface IUsuarioServices
    {
        Task<List<Usuario>> Lista();
        Task<Usuario> ObtenerPorId(int secuencialUsuario);
        Task<Usuario> ObtenerPorCredenciales(string correo, string clave);
        Task<Usuario> Crear(Usuario entidad, Stream Foto = null, string NombreFoto = "", string UrlPlantillaCorreo = "");
        Task<string> EnviarCorreoConPlantilla(string urlPlantilla, Usuario usuario, Empresa empresa, bool esRecuperarClave, string claveGenerada = "");
        Task<bool> CambiarClave(int secuencialUsuario, string ClaveActual, string ClaveNueva);
        Task<Usuario> Editar(Usuario entidad, Stream? Foto = null, string? NombreFoto = "", string cabeceraUrlCorreo = "");
        Task<bool> Eliminar(int secuencialUsuario);
        Task<bool> GuardarPerfil(Usuario entidad);
        Task<Usuario?> ExistePorSecuencial(int secuencialUsuario);
        Task<bool> RestablecerClave(string? correoDestino, string urlPantillaCorreo = "");

    }
}
