using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Entity
{
    public partial class Cliente
    {
        [Key]
        public int SecCliente { get; set; }
        public int SecConstructora { get; set; }
        public string NumeroCliente { get; set; } = null!;
        public DateTime FechaCreacion { get; set; }
        public bool? EstaActivo { get; set; }

        public virtual Constructora SecConstructoraNavigation { get; set; } = null!;
    }
}
