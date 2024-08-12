using System;
using System.Collections.Generic;

namespace Entity;

public partial class Visita
{
    public int Secuencial { get; set; }

    public int SecUsuario { get; set; }

    public int? SecProvincia { get; set; }

    public int? SecCanton { get; set; }

    public int? SecParroquia { get; set; }

    public string? Nombre { get; set; }

    public string? Direccion { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public DateTime? FechaSiguienteVisita { get; set; }

    public string? GeoUbicacion { get; set; }

    public string? Detalle { get; set; }

    public short? EstaActivo { get; set; }

    public virtual ICollection<ContactoVisita> Contactovista { get; set; } = new List<ContactoVisita>();

    public virtual Canton? SecCantonNavigation { get; set; }

    public virtual Parroquia? SecParroquiaNavigation { get; set; }

    public virtual Provincia? SecProvinciaNavigation { get; set; }

    public virtual Usuario SecUsuarioNavigation { get; set; } = null!;
}
