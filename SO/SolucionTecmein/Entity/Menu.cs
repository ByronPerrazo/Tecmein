using System;
using System.Collections.Generic;

namespace Entity;

public partial class Menu
{
    public int Secuencial { get; set; }

    public string? Descripcion { get; set; }

    public int? SecMenuPadre { get; set; }

    public string? Icono { get; set; }

    public string? Controlador { get; set; }

    public string? PaginaAccion { get; set; }

    public short? EsActivo { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public virtual ICollection<Menu> InverseSecMenuPadreNavigation { get; set; } = new List<Menu>();

    public virtual ICollection<Rolmenu> Rolmenus { get; set; } = new List<Rolmenu>();

    public virtual Menu? SecMenuPadreNavigation { get; set; }
}
