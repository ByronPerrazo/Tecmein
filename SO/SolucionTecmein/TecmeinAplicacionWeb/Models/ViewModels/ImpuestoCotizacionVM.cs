namespace TecmeinAplicacionWeb.Models.ViewModels
{
    public class ImpuestoCotizacionVM
    {
        public int Id { get; set; }
        public int SecCotizacion { get; set; }
        public int ImpuestoId { get; set; }
        public string NombreImpuesto { get; set; } // To display the name of the tax
        public string TipoImpuestoDescripcion { get; set; }
        public decimal BaseImponible { get; set; }
        public decimal ValorImpuesto { get; set; }
        public bool Exento { get; set; }
        public string? Observaciones { get; set; }
        public string FechaRegistro { get; set; }
    }
}