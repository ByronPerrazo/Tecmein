namespace TecmeinWebApp.Models.ViewModel
{
    public class LoginUsuarioVM
    {
        public string? Correo { get; set; }
        public string? Clave { get; set; }

        public bool MantenerSesionIniciada { get; set; }
    }
}
