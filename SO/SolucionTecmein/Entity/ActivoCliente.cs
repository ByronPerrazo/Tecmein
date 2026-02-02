using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entity;

[Table("activocliente")]
public partial class ActivoCliente
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdActivoCliente { get; set; }

    public int SecCliente { get; set; }

    public int? SecEquipo { get; set; } // Puede ser nulo si el equipo no está en el catálogo

    [Required]
    [StringLength(500)]
    public string Descripcion { get; set; } = null!;

    public DateTime FechaInstalacion { get; set; }

    public int? SecContratoOrigen { get; set; } // Puede ser nulo si no proviene de un contrato de venta específico

    // Propiedades de navegación
    public virtual Cliente SecClienteNavigation { get; set; } = null!;
    public virtual Contrato? SecContratoOrigenNavigation { get; set; } // Puede ser nulo
    // public virtual Equipo? SecEquipoNavigation { get; set; } // Descomentar cuando se defina la entidad Equipo
}
