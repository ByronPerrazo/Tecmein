using System.ComponentModel.DataAnnotations;

namespace Entity;

public partial class Empresastorage
{
    [Key]
    public int SecEmpresa { get; set; }

    public string? Email { get; set; }

    public string? Clave { get; set; }

    public string? Ruta { get; set; }

    public string? ApiKey { get; set; }

    public string? CarpetaUsuario { get; set; }

    public string? CarpetaProducto { get; set; }

    public string? CarpetaLogo { get; set; }

    public short? EstaActivo { get; set; }

    public virtual Empresa SecEmpresaNavigation { get; set; } = null!;
}
