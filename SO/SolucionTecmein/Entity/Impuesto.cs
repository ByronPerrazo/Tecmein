using System.ComponentModel.DataAnnotations;

namespace Entity;

public partial class Impuesto
{
    [Key]
    public int Id { get; set; }
    public string Codigo { get; set; } = null!;
    public string Descripcion { get; set; } = null!;
    public decimal? Porcentaje { get; set; }
    public decimal? ValorFijo { get; set; }
    public string? CodigoSri { get; set; }
    public int SecTipoImpuesto { get; set; }
    public bool Vigente { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaModificacion { get; set; }

    public virtual TipoImpuesto SecTipoImpuestoNavigation { get; set; } = null!;

    public virtual ICollection<ImpuestoCotizacion> ImpuestoCotizacion { get; set; }
}
