namespace Entity;

public partial class Contactovisita
{
    public int Secuencial { get; set; }

    public int SecContacto { get; set; }

    public int SecVisita { get; set; }

    public short EstaActivo { get; set; }

    public virtual Contacto? SecContactoNavigation { get; set; }
}
