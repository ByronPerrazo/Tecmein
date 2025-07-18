using System;
using System.Collections.Generic;

namespace Entity;

public partial class Permisosrol
{
    public int Secuencial { get; set; }

    public int SecRol { get; set; }

    public int SecUsuarioModifica { get; set; }

    public DateTime FechaRegistro { get; set; }

    public short Consultar { get; set; }

    public short Modificar { get; set; }

    public short Eliminar { get; set; }

    public short Activo { get; set; }
}
