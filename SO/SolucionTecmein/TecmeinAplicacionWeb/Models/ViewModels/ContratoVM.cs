namespace TecmeinAplicacionWeb.Models.ViewModels
{
    public class ContratoVM
    {
        public int IdContrato { get; set; }
        public int? IdCotizacion { get; set; }
        public int? SecCliente { get; set; } // Añadido
        public string? NombreObra { get; set; } // Para mostrar en la vista
        public string? FechaFirma { get; set; }
        public int IdUsuarioCarga { get; set; }
        public string? NombreUsuarioCarga { get; set; } // Para mostrar
        public string? NombreArchivo { get; set; }
        public string? RutaArchivo { get; set; } // Propiedad que faltaba
        public string? NombreProyecto { get; set; } // Añadido para el nombre de la obra en contratos directos
        public bool? EsActivo { get; set; }
    }
}
