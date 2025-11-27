using System;
using System.Collections.Generic;

namespace BLL.DTOs
{
    public class PlanDePagoDTO
    {
        public int IdPlanDePago { get; set; }
        public int IdContrato { get; set; }
        public int SecFormaPago { get; set; }
        public decimal ValorContrato { get; set; }
        public int NumeroCuotas { get; set; }
        public DateTime? FechaPrimeraCuota { get; set; }
        public decimal? ValorAnticipo { get; set; }
        public DateTime? FechaAnticipo { get; set; }
        public List<CuotaDTO> Cuotas { get; set; } = new List<CuotaDTO>();
        public DateTime FechaRegistro { get; set; }
        public bool EstaActivo { get; set; }


        // Propiedades adicionales para la vista Financiero
        public string? NumeroContrato { get; set; }
        public string? NombreCliente { get; set; }
        public decimal MontoPagado { get; set; }
        public decimal SaldoPendiente { get; set; }
        public string? EstadoPlan { get; set; }
        public decimal MontoPagadoTotal { get; set; }
        public decimal SaldoPendienteTotal { get; set; }
    }
}
