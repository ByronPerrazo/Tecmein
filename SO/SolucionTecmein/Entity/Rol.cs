﻿using System;
using System.Collections.Generic;

namespace Entity;

public partial class Rol
{
    public int Secuencial { get; set; }

    public string? Descripcion { get; set; }

    public short? EsActivo { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public virtual ICollection<RolMenu> Rolmenus { get; set; } = new List<RolMenu>();

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
