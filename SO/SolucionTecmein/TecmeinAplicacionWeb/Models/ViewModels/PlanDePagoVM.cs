using System;
using System.Collections.Generic;

namespace TecmeinAplicacionWeb.Models.ViewModels
{
    public class PlanDePagoVM
    {
        public int IdPlanDePago { get; set; }
        public int IdContrato { get; set; }
        public int SecFormaPago { get; set; }
        public string? DescripcionFormaPago { get; set; }
        public decimal ValorContrato { get; set; }
        public decimal ValorAnticipo { get; set; }
        public DateTime? FechaAnticipo { get; set; }
        public int NumeroCuotas { get; set; }
        public DateTime? FechaPrimeraCuota { get; set; }
        public bool EstaActivo { get; set; }
        public string? FechaRegistro { get; set; }

        public List<CuotaVM>? Cuotas { get; set; }
    }
}
