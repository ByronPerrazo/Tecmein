using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entity
{
    public partial class Cuota
    {
        [Key]
        public int IdCuota { get; set; }

        public int IdPlanDePago { get; set; }

        public int NumeroCuota { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal MontoEsperado { get; set; }
        public decimal? MontoPagado { get; set; }

        public DateTime FechaVencimiento { get; set; }

        public string Estado { get; set; } = "Pendiente"; // Ej: Pendiente, Pagada, Vencida

        public DateTime FechaRegistro { get; set; }

        // Propiedad de navegación
        [ForeignKey("IdPlanDePago")]
        public virtual PlanDePago IdPlanDePagoNavigation { get; set; } = null!;
    }
}
