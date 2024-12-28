namespace TecmeinWebApp.Models.ViewModel
{
    public class EmpresaVM
    {
        public int Secuencial { get; set; }
        public string? UrlLogo { get; set; }
        public string? NombreLogo { get; set; }
        public string? Identificacion { get; set; }
        public string? Nombre { get; set; }
        public string? Correo { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public string? CodigoOperador { get; set; }
        public short? EstaActivo { get; set; } = 0;
    }
}
