using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entity;

public partial class Visita : IAuditEntity
{
    [Key]
    public int Secuencial { get; set; }

    public int? SecUsuario { get; set; }

    public int? SecProvincia { get; set; }

    public int? SecCanton { get; set; }

    public int? SecParroquia { get; set; }

    public int IdEtapa { get; set; }

    public int? SecEmpresa { get; set; }

    public string? Nombre { get; set; }

    public string? Direccion { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public DateTime? FechaSiguienteVisita { get; set; }

    public string? GeoUbicacion { get; set; }

    public string? Detalle { get; set; }

    public short? EstaActivo { get; set; }

    public int? SecConstructora { get; set; } // Nuevo campo

    // Implementación IAuditEntity
    public int? SecUsuarioModifica { get; set; }
    public DateTime? FechaModificacion { get; set; }

    public virtual Canton? SecCantonNavigation { get; set; }

    public virtual Parroquia? SecParroquiaNavigation { get; set; }

    public virtual Provincia? SecProvinciaNavigation { get; set; }

    public virtual Usuario SecUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<Contactovisita> Contactovisita { get; set; } = new List<Contactovisita>();

    [ForeignKey("IdEtapa")]
    public virtual Etapa IdEtapaNavigation { get; set; } = null!;

    [ForeignKey("SecEmpresa")]
    public virtual Empresa? SecEmpresaNavigation { get; set; }

    [ForeignKey("SecConstructora")] // Nueva navegación
    public virtual Constructora? SecConstructoraNavigation { get; set; }
}
