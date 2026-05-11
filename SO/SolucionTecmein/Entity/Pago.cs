using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entity
{
    public partial class Pago
    {
        [Key]
        public int IdPago { get; set; }

        public int IdPlanDePago { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Monto { get; set; }

        public DateTime FechaPago { get; set; }

        public string? ComprobanteUrl { get; set; }

        public string? ComprobanteNombre { get; set; }

        public int RegistradoPorUsuarioId { get; set; }

        public bool EstaActivo { get; set; }

        public DateTime FechaRegistro { get; set; }

        [ForeignKey("IdPlanDePago")]
        public virtual PlanDePago IdPlanDePagoNavigation { get; set; } = null!;

        [ForeignKey("RegistradoPorUsuarioId")]
        public virtual Usuario RegistradoPorUsuario { get; set; } = null!;
    }
}
