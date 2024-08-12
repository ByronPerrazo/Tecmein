using System;
using System.Collections.Generic;

namespace Entity;

public partial class Contacto
{
    public int Secuencial { get; set; }

    public string? Nombre { get; set; }

    public string? Telefono { get; set; }

    public string? Correo { get; set; }

    public string? Titulo { get; set; }

    public short? EstaActivo { get; set; }

    public virtual ICollection<ContactoVisita> Contactovista { get; set; } = new List<ContactoVisita>();
}
