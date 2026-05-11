using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // New using statement

namespace Entity;

public partial class Cotizacion : IAuditEntity
{
    [Key]
    public int Secuencial { get; set; }
    public int SecVisita { get; set; }
    public bool EnviadoProveedor { get; set; }
    public bool EnviadoCliente { get; set; }
    public bool Confirmacion { get; set; }
    public decimal Subtotal { get; set; }
    public decimal ValorImpuestos { get; set; }
    public decimal TotalConImpuestos { get; set; }
    public decimal ValorIVA { get; set; }
    public decimal ValorImportacion { get; set; }
    public short? EstaActivo { get; set; }
    public string? TipoContrato { get; set; }
    public DateTime? FechaRegistro { get; set; }
    public DateTime? FechaModificacion { get; set; }
    public int? SecUsuario { get; set; }

    public int? SecTipoDocumento { get; set; } // Nuevo campo foráneo
    [ForeignKey("SecTipoDocumento")]
    public virtual TipoDocumento? SecTipoDocumentoNavigation { get; set; }

    [ForeignKey("SecUsuario")]
    public virtual Usuario SecUsuarioNavigation { get; set; }

    public int? SecCotizacionOriginal { get; set; }
    public int? SecUsuarioModifica { get; set; }

    [ForeignKey("SecCotizacionOriginal")]
    public virtual Cotizacion SecCotizacionOriginalNavigation { get; set; }

    [ForeignKey("SecUsuarioModifica")]
    public virtual Usuario SecUsuarioModificaNavigation { get; set; }

    [ForeignKey("SecVisita")] // New attribute
    public virtual Visita SecVisitaNavigation { get; set; } = null!;
    public virtual ICollection<Cotizaciondetalle> Cotizaciondetalles { get; set; } = new List<Cotizaciondetalle>();
    public virtual ICollection<Seguimiento> Seguimientos { get; set; } = new List<Seguimiento>();
    public virtual ICollection<ImpuestoCotizacion> ImpuestoCotizaciones { get; set; } = new List<ImpuestoCotizacion>();
}
