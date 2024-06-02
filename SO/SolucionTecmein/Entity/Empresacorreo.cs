using System;
using System.Collections.Generic;

namespace Entity;

public partial class Empresacorreo
{
    public int SecEmpresa { get; set; }

    public string? Email { get; set; }

    public string? Clave { get; set; }

    public string? Alias { get; set; }

    public string? Host { get; set; }

    public int? Puerto { get; set; }

    public short? EstaActivo { get; set; }

    public virtual Empresa SecEmpresaNavigation { get; set; } = null!;
}
