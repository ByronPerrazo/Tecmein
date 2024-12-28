namespace Entity;

public partial class Catalogo
{
    public int Secuencial { get; set; }

    public string Nombre { get; set; } = null!;

    public string NombreArchivo { get; set; } = null!;

    public string? UrlCatalogo { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public short? EstaActivo { get; set; }
}
