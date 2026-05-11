namespace TecmeinAplicacionWeb.Models.ViewModels
{
    public class TipoDocumentoVM
    {
        public int SecTipoDocumento { get; set; }
        public string? Codigo { get; set; }
        public string? Descripcion { get; set; }
        public bool? EstaActivo { get; set; }
        public string? FechaRegistro { get; set; } // Changed to string for display purposes
        public int? SecPlantilla { get; set; }
        public string? NombrePlantilla { get; set; }
    }
}