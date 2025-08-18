using System.ComponentModel.DataAnnotations;

namespace Entity;

public partial class ImpuestoCotizacion
{
    [Key]
    public int Id { get; set; }
    public int SecCotizacion { get; set; }
    public int ImpuestoId { get; set; }
    public decimal BaseImponible { get; set; }
    public decimal ValorImpuesto { get; set; }
    public bool Exento { get; set; }
    public string? Observaciones { get; set; }
    public DateTime FechaRegistro { get; set; }

    public virtual Cotizacion SecCotizacionNavigation { get; set; } = null!;
    public virtual Impuesto ImpuestoNavigation { get; set; } = null!;
}
