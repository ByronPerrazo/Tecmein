using System;
using System.Collections.Generic;

namespace Entity;

public partial class Rolmenu
{
    public int Secuencial { get; set; }

    public int? SecRol { get; set; }

    public int? SecMenu { get; set; }

    public short? EsActivo { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public virtual Menu? SecMenuNavigation { get; set; }

    public virtual Rol? SecRolNavigation { get; set; }
}
