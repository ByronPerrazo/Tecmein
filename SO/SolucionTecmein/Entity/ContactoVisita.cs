using System;
using System.Collections.Generic;

namespace Entity;

public partial class ContactoVisita
{
    public int Secuencial { get; set; }

    public int? SecContacto { get; set; }

    public int? SecVisita { get; set; }

    public short? EstaActivo { get; set; }

    public virtual Contacto? SecContactoNavigation { get; set; }

    public virtual Visita? SecVisitaNavigation { get; set; }
}
