using System;
using System.Collections.Generic;

namespace Entity;

public partial class Empresa
{
    public int Secuencial { get; set; }

    public string? UrlLogo { get; set; }

    public string? NombreLogo { get; set; }

    public string? NumeroDocumento { get; set; }

    public string? Nombre { get; set; }

    public string? Correo { get; set; }

    public string? Direccion { get; set; }

    public string? Telefono { get; set; }

    public decimal? PorcentajeImpuesto { get; set; }

    public string? SimboloMoneda { get; set; }

    public short? EstaActivo { get; set; }

    public virtual Empresacorreo? Empresacorreo { get; set; }

    public virtual Empresastorage? Empresastorage { get; set; }
}
