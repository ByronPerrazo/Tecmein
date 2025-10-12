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
    }
}
