using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IUsuarioServices
    {
        Task<List<Usuario>> Lista();
        Task<Usuario> Crear(Usuario entidad, Stream? imagen = null, string nombreImagen = "", string urlPantillaCorreo = "");
        Task<string> EnviarCorreoConPlantilla(string urlPlantilla, Usuario usuario, Empresa empresa, bool esRecuperarClave, string claveGenerada = "");
        Task<bool> CambiarClave(string correo, string ClaveActual, string ClaveNueva);
        Task<Usuario> Editar(Usuario entidad, Stream? imagen = null, string? nombreImagen = "");
        Task<bool> Eliminar(int secuencialUsuario);
        Task<bool> GuardarRol(Usuario entidad);
        Task<Usuario> ExistePorSecuencial(int secuencialUsuario);
        Task<Usuario> OtenerPorCredenciales(string correo, string clave);
        Task<bool> RestablecerClave(string? correoDestino, string urlPantillaCorreo = "");

    }
}
