using System;
using System.ComponentModel.DataAnnotations;

namespace Entity
{
    public partial class PolizaGarantia
    {
        [Key]
        public int Secuencial { get; set; }

        [Required]
        public string Descripcion { get; set; }

        public bool EstaActivo { get; set; }
    }
}