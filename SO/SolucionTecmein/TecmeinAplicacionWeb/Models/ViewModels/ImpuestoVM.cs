namespace TecmeinAplicacionWeb.Models.ViewModels
{
    public class ImpuestoVM
    {
        public int Id { get; set; }
        public string? Codigo { get; set; }
        public string? Descripcion { get; set; }
        public decimal? Porcentaje { get; set; }
        public decimal? ValorFijo { get; set; }
        public string? CodigoSri { get; set; }
        public int SecTipoImpuesto { get; set; }
        public string? NombreTipoImpuesto { get; set; }
        public bool EsIva { get; set; }
        public bool EsImportacion { get; set; }
        public bool Vigente { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }
}