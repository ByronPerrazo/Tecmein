namespace Entity;

public partial class Canton
{
    public int Secuencial { get; set; }

    public int SecProvincia { get; set; }

    public string? Codigo { get; set; }

    public string? Nombre { get; set; }

    public short? EstaActivo { get; set; }

    public virtual ICollection<Parroquia> Parroquia { get; set; } = new List<Parroquia>();

    public virtual Provincia SecProvinciaNavigation { get; set; } = null!;

    public virtual ICollection<Visita> Visita { get; set; } = new List<Visita>();
}
