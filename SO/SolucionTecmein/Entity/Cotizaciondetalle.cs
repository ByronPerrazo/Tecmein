
using System.ComponentModel.DataAnnotations;

namespace Entity;

public partial class Cotizaciondetalle
{
    [Key]
    public int Secuencial { get; set; }
    public int SecCotizacion { get; set; }
    public int? SecEquipoVisita { get; set; } // Referencia al EquipoVisita original
    public string? DetalleEquipo { get; set; } // Asumo que el detalle del equipo será un texto por ahora.
    public decimal ValorCompra { get; set; }
    public decimal MargenGanancia { get; set; }
    public decimal Total { get; set; }
    public decimal? Subtotal { get; set; }
    public decimal? Impuestos { get; set; }
    public int Cantidad { get; set; }
    public short? EstaActivo { get; set; }
    public DateTime? FechaRegistro { get; set; }

    public virtual Cotizacion SecCotizacionNavigation { get; set; } = null!;
}
