namespace Entity;

public partial class Parroquia
{
    public int Secuencial { get; set; }

    public int SecCanton { get; set; }

    public string? Codigo { get; set; }

    public string? Nombre { get; set; }

    public short? EstaActivo { get; set; }

    public virtual Canton SecCantonNavigation { get; set; } = null!;

    public virtual ICollection<Visita> Visita { get; set; } = new List<Visita>();
}
