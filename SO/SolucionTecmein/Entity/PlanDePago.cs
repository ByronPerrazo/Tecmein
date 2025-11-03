
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entity
{
    public partial class PlanDePago
    {
        public PlanDePago()
        {
            Pagos = new HashSet<Pago>();
            Cuotas = new HashSet<Cuota>();
        }

        [Key]
        public int IdPlanDePago { get; set; }

        public int IdContrato { get; set; }

        public int SecFormaPago { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorContrato { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorAnticipo { get; set; }

        public DateTime? FechaAnticipo { get; set; }

        public int NumeroCuotas { get; set; }

        public DateTime? FechaPrimeraCuota { get; set; }

        public bool EstaActivo { get; set; }

    public DateTime FechaRegistro { get; set; }
    public DateTime? FechaModificacion { get; set; }

    public virtual Contrato IdContratoNavigation { get; set; } = null!;

        [ForeignKey("SecFormaPago")]
        public virtual FormaPago SecFormaPagoNavigation { get; set; } = null!;

        public virtual ICollection<Pago> Pagos { get; set; }

        public virtual ICollection<Cuota> Cuotas { get; set; }
    }
}
