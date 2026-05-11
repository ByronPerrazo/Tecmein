namespace TecmeinAplicacionWeb.Models.ViewModels
{
    public class PreContratoVM
    {
        public int SecPreContrato { get; set; }
        public int SecCotizacion { get; set; }
        public string? NumeroCotizacion { get; set; }
        public string? NombreCliente { get; set; }
        public decimal? ValorContrato { get; set; }
        public string? Estado { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public int? Version { get; set; }
        public bool? EstaActivo { get; set; }
        public string? NombreObra { get; set; }
        public string? NombreUsuarioCrea { get; set; }
    }
}