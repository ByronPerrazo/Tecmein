using System;
using System.Collections.Generic;

namespace BLL.DTOs
{
    public class DetallePlanPagoDTO
    {
        public int IdPlanDePago { get; set; }
        public string? NumeroContrato { get; set; }
        public string? NombreCliente { get; set; }
        public string? NombreProyecto { get; set; } // Added
        public decimal ValorContrato { get; set; }
        public decimal ValorAnticipo { get; set; }
        public DateTime? FechaAnticipo { get; set; }
        public int NumeroCuotas { get; set; }
        public DateTime? FechaPrimeraCuota { get; set; }
        public decimal MontoPagadoTotal { get; set; }
        public decimal SaldoVencidoTotal { get; set; }
        public decimal SaldoPendienteTotal { get; set; }
        public List<CuotaDTO>? Cuotas { get; set; }
    }
}