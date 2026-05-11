using System.ComponentModel.DataAnnotations;

namespace Entity;

public partial class TipoImpuesto
{
    [Key]
    public int Secuencial { get; set; }
    public string Nombre { get; set; } = null!;
    public bool EsIva { get; set; }
    public bool EsImportacion { get; set; }
    public bool EstaActivo { get; set; }
    public int Prioridad { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaModificacion { get; set; }
}