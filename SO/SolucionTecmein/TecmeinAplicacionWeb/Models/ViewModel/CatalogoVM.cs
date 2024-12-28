namespace TecmeinWebApp.Models.ViewModel
{
    public class CatalogoVM
    {
        public int Secuencial { get; set; }

        public string? Nombre { get; set; }

        public string? UrlCatalogo { get; set; }

        public DateTime? FechaRegistro { get; set; }

        public ulong? EstaActivo { get; set; }
    }
}
