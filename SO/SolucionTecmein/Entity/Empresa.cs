using System.ComponentModel.DataAnnotations;

namespace Entity;

public partial class Empresa
{
    [Key]
    public int Secuencial { get; set; }

    public string? UrlLogo { get; set; }

    public string? NombreLogo { get; set; }

    public string? Identificacion { get; set; }

    public string? Nombre { get; set; }

    public string? Correo { get; set; }

    public string? Direccion { get; set; }

    public string? Telefono { get; set; }

    public string? CodigoOperador { get; set; }

    public short? EstaActivo { get; set; }

    public virtual Empresacorreo? Empresacorreo { get; set; }

    public virtual Empresastorage? Empresastorage { get; set; }

    public virtual ICollection<FormatoNumeroCliente> FormatosNumeroCliente { get; set; } = new List<FormatoNumeroCliente>();
}
