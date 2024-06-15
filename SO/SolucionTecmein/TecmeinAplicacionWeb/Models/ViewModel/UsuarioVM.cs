using System.ComponentModel.DataAnnotations;

namespace TecmeinWebApp.Models.ViewModel
{
    public class UsuarioVM
    {
        public int Secuencial { get; set; }
        public string? Nombre { get; set; }
        public string? Correo { get; set; }
        public string? Telefono { get; set; }
        public int? SecRol { get; set; }
        public string? NombreRol { get; set; }
        public string? UrlFoto { get; set; }
        public string? NombreFoto { get; set; }
        public string? Clave { get; set; }
        public ulong? EsActivo { get; set; }
        public DateTime? FechaRegistro { get; set; }
    }
}
