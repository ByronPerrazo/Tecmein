using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    public class ContratoHistoricoRegistroDTO
    {
        [Required(ErrorMessage = "El ID del cliente es obligatorio.")]
        public int SecCliente { get; set; }

        [Required(ErrorMessage = "La fecha de firma es obligatoria.")]
        public string FechaFirma { get; set; }

        // Propiedades para el PlanDePago
        public int SecFormaPago { get; set; }

        [Required(ErrorMessage = "El valor del contrato es obligatorio.")]
        public decimal ValorContrato { get; set; }

        public decimal ValorAnticipo { get; set; }

        public string? FechaAnticipo { get; set; }

        public int NumeroCuotas { get; set; }

        public string? FechaPrimeraCuota { get; set; }
    }
}
