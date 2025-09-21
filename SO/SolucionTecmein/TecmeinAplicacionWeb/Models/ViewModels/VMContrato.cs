namespace TecmeinAplicacionWeb.Models.ViewModels
{
    public class VMContrato
    {
        public int IdContrato { get; set; }
        public int IdCotizacion { get; set; }
        public string? NombreObra { get; set; } // Para mostrar en la vista
        public string? FechaFirma { get; set; }
        public int IdUsuarioCarga { get; set; }
        public string? NombreUsuarioCarga { get; set; } // Para mostrar
        public string? NombreArchivo { get; set; }
        public int EsActivo { get; set; }
    }
}
