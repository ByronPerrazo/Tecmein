using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entity
{
    public class PreContratoCompromisoPago
    {
        [Key]
        public int Secuencial { get; set; } // Corregido según la convención

        public int SecPreContrato { get; set; }

        public int NumeroCuota { get; set; } // 0 para el anticipo, 1 en adelante para cuotas

        [Column(TypeName = "decimal(18,2)")]
        public decimal Monto { get; set; }

        public DateTime FechaVencimiento { get; set; }
        
        public string Tipo { get; set; } // "Anticipo" o "Cuota"

        public DateTime FechaRegistro { get; set; } = DateTime.Now;


        [ForeignKey("SecPreContrato")]
        public virtual PreContrato PreContratoNavigation { get; set; }
    }
}
