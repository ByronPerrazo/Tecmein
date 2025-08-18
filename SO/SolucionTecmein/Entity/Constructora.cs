using System.ComponentModel.DataAnnotations;

namespace Entity;

public partial class Constructora
{
    [Key]
    public int Secuencial { get; set; }

    public string? Nombre { get; set; }

    public string? Direccion { get; set; }

    public string? Telefono { get; set; }

    public string? Correo { get; set; }

    public string? Atencion { get; set; }

    public string? Administrador { get; set; }

    public string? TelefonoAdministrador { get; set; }

    public string? CorreoAdministrador { get; set; }

    public short? EstaActivo { get; set; }

    public virtual ICollection<Contacto> Contactos { get; set; } = new List<Contacto>();

    public virtual Cliente Cliente { get; set; }
}
