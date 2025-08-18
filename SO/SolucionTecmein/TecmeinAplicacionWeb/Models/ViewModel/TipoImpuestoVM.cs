namespace TecmeinWebApp.Models.ViewModel
{
    public class TipoImpuestoVM
    {
        public int Secuencial { get; set; }
        public string? Nombre { get; set; }
        public bool EsIva { get; set; }
        public bool EsImportacion { get; set; }
        public bool EstaActivo { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }
}