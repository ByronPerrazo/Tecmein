namespace AplicacionWeb.Models.ViewModel
{
    public class LoginUsuarioVM
    {
        public string? Email { get; set; }
        public string? Password { get; set; }

        public bool MantenerSesionIniciada { get; set; }
    }
}
