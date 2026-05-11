namespace BLL.DTOs
{
    public class CotizacionDTO
    {
        public int Secuencial { get; set; }
        public int SecVisita { get; set; }
        public string NombreObra { get; set; } = string.Empty;
        public string NombreContacto { get; set; } = string.Empty;
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
        public int? SecTipoDocumento { get; set; }
        public string? DescripcionTipoDocumento { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int? SecUsuario { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public int? SecCotizacionOriginal { get; set; }
        public int? SecUsuarioModifica { get; set; }
        public string NombreUsuarioModifica { get; set; } = string.Empty;
        public List<CotizaciondetalleDTO> Cotizaciondetalles { get; set; } = new();
        public List<ImpuestoCotizacionDTO> ImpuestoCotizaciones { get; set; } = new();
    }

    public class CotizaciondetalleDTO
    {
        public int Secuencial { get; set; }
        public int SecCotizacion { get; set; }
        public int? SecEquipoVisita { get; set; }
        public string? NombreEquipo { get; set; }
        public string? Descripcion { get; set; }
        public int Cantidad { get; set; }
        public decimal ValorCompra { get; set; }
        public decimal ValorVenta { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal Total { get; set; }
    }

    public class ImpuestoCotizacionDTO
    {
        public int Secuencial { get; set; }
        public int SecCotizacion { get; set; }
        public int SecImpuesto { get; set; }
        public string? NombreImpuesto { get; set; }
        public decimal Porcentaje { get; set; }
        public decimal Valor { get; set; }
    }
}
