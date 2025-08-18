using System.ComponentModel.DataAnnotations;

namespace Entity;

public partial class Etapa
{
    [Key]
    public int Id { get; set; }

    public string Codigo { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public int Orden { get; set; }

    public bool EstaActivo { get; set; }
}
