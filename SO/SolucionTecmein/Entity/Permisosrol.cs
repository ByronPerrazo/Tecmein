using System.ComponentModel.DataAnnotations;

namespace Entity;

public partial class Permisosrol
{
    [Key]
    public int Secuencial { get; set; }

    public int SecRol { get; set; }

    public int SecUsuarioModifica { get; set; }

    public DateTime FechaRegistro { get; set; }

    public short Consultar { get; set; }

    public short Modificar { get; set; }

    public short Eliminar { get; set; }

    public short Activo { get; set; }
}
