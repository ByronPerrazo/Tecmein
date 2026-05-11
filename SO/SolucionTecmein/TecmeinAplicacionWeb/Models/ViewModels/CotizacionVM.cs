namespace TecmeinAplicacionWeb.Models.ViewModels
{
    public class CotizacionVM
    {
        public int Secuencial { get; set; }
        public int SecVisita { get; set; }
        public string NombreObra { get; set; } = string.Empty; // Para mostrar en la lista
        public string NombreContacto { get; set; } = string.Empty; // Para mostrar en la lista
        public bool EnviadoProveedor { get; set; }
        public bool EnviadoCliente { get; set; }
        public bool Confirmacion { get; set; }
        public decimal Subtotal { get; set; }
        public decimal ValorImpuestos { get; set; }
        public decimal TotalConImpuestos { get; set; }
        public decimal ValorIVA { get; set; }
        public decimal ValorImportacion { get; set; }
        public int EstaActivo { get; set; }
        public string? TipoContrato { get; set; }
        public int? SecTipoDocumento { get; set; } // Nuevo campo
        public string? DescripcionTipoDocumento { get; set; } // Para mostrar nombre del tipo
        public string FechaRegistro { get; set; } = string.Empty;
        public string FechaModificacion { get; set; } = string.Empty;
        public int? SecUsuario { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public int? SecCotizacionOriginal { get; set; }
        public int? SecUsuarioModifica { get; set; }
        public string NombreUsuarioModifica { get; set; } = string.Empty;
        public List<CotizaciondetalleVM> Cotizaciondetalles { get; set; } = new();
        public List<ImpuestoCotizacionVM> ImpuestoCotizaciones { get; set; } = new();
    }
}
