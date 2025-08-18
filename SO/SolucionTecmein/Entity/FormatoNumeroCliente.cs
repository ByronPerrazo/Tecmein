using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Entity
{
    public partial class FormatoNumeroCliente
    {
        [Key]
        public int SecFormatoNumeroCliente { get; set; }
        public int SecEmpresa { get; set; }
        public bool UsaFormato { get; set; }
        public string? Formato { get; set; }
        public int NumeroInicio { get; set; }

        public virtual Empresa SecEmpresaNavigation { get; set; } = null!;
    }
}
