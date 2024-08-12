using System;
using System.Collections.Generic;

namespace Entity;

public partial class Producto
{
    public int Secuencial { get; set; }

    public int? SecTipoProducto { get; set; }

    public string? Nombre { get; set; }

    public string? Marca { get; set; }

    public string? Sistema { get; set; }

    public string? Capacidad { get; set; }

    public string? Motor { get; set; }

    public int Stock { get; set; }

    public string? UrlImagen { get; set; }

    public string? NombreImagen { get; set; }

    public decimal? Precio { get; set; }

    public string? Descripcion { get; set; }

    public short? EstaActivo { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public virtual TipoProducto? SecTipoProductoNavigation { get; set; }
}
