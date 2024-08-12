using System;
using System.Collections.Generic;

namespace Entity;

public partial class TipoProducto
{
    public int Secuencial { get; set; }

    public string? Nombre { get; set; }

    public string? Descripcion { get; set; }

    public short? EstaActivo { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
