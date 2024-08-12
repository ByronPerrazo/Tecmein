using System;
using System.Collections.Generic;

namespace Entity;

public partial class Provincia
{
    public int Secuencial { get; set; }

    public string? Codigo { get; set; }

    public string? Nombre { get; set; }

    public short? EstaActivo { get; set; }

    public virtual ICollection<Canton> Cantons { get; set; } = new List<Canton>();

    public virtual ICollection<Visita> Visita { get; set; } = new List<Visita>();
}
