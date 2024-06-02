using System;
using System.Collections.Generic;

namespace Entity;

public partial class Usuario
{
    public int Secuencial { get; set; }

    public string? Nombre { get; set; }

    public string? Correo { get; set; }

    public string? Telefono { get; set; }

    public int? SecRol { get; set; }

    public string? UrlFoto { get; set; }

    public string? NombreFoto { get; set; }

    public string? Clave { get; set; }

    public short? EsActivo { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public virtual Rol? SecRolNavigation { get; set; }
}
