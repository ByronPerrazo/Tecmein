using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Entity;

public partial class Contrato
{
    [Key]
    public int IdContrato { get; set; }

    public int? IdCotizacion { get; set; }

    public DateTime FechaFirma { get; set; }

    public int IdUsuarioCarga { get; set; }

    public string NombreArchivo { get; set; } = null!;

    public string RutaArchivo { get; set; } = null!;

    public DateTime? FechaCreacion { get; set; }

    public bool? EsActivo { get; set; }

    public int SecCliente { get; set; } // Propiedad para la FK a Cliente

    public int? SecTipoDocumento { get; set; } // Propiedad para la FK a TipoDocumento

    public virtual Cotizacion IdCotizacionNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioCargaNavigation { get; set; } = null!;

    public virtual Cliente SecClienteNavigation { get; set; } = null!; // Propiedad de navegación

    public virtual TipoDocumento? SecTipoDocumentoNavigation { get; set; } // Propiedad de navegación

    public virtual PlanDePago? PlanDePagoNavigation { get; set; }
}