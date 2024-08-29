using System;
using System.Collections.Generic;

namespace Entity;

public partial class Constructora
{
    public int Secuencial { get; set; }

    public string? Nombre { get; set; }

    public string? Direccion { get; set; }

    public string? Telefono { get; set; }

    public string? Correo { get; set; }

    public string? Atencion { get; set; }

    public string? Administrador { get; set; }

    public string? TelefonoAdministrador { get; set; }

    public string? CorreoAdministrador { get; set; }

    public short? EstaActivo { get; set; }
}
