using System;
using System.ComponentModel.DataAnnotations;

namespace Entity
{
    public partial class FormaPago
    {
        [Key]
        public int SecFormaPago { get; set; }
        public string Descripcion { get; set; } = null!;
        public short EstaActivo { get; set; }
    }
}
