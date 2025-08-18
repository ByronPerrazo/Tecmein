using System.ComponentModel.DataAnnotations;

namespace Entity;

public partial class Contacto
{
    [Key]
    public int Secuencial { get; set; }

    public int SecConstructora { get; set; }

    public string? Titulo { get; set; }

    public string? Nombres { get; set; }

    public string? Apellidos { get; set; }

    public string? Telefono { get; set; }

    public string? Correo { get; set; }

    public short? EstaActivo { get; set; }

    public virtual ICollection<Contactovisita> Contactovisita { get; set; } = new List<Contactovisita>();

    public virtual Constructora SecConstructoraNavigation { get; set; } = null!;
}
