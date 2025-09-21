namespace TecmeinAplicacionWeb.Models.ViewModels
{
    public class LoginUsuarioVM
    {
        public string? Correo { get; set; }
        public string? Clave { get; set; }

        public bool MantenerSesionIniciada { get; set; }
    }
}
