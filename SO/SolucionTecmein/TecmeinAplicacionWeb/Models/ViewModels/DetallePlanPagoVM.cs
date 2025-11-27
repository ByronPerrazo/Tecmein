using System;
using System.Collections.Generic;

namespace TecmeinAplicacionWeb.Models.ViewModels
{
    public class DetallePlanPagoVM
    {
        public int IdPlanDePago { get; set; }
        public string? NumeroContrato { get; set; }
        public string? NombreCliente { get; set; }
        public string? NombreProyecto { get; set; } // Added
        public decimal ValorContrato { get; set; }
        public decimal ValorAnticipo { get; set; }
        public string? FechaAnticipo { get; set; }
        public int NumeroCuotas { get; set; }
        public string? FechaPrimeraCuota { get; set; }
        public decimal MontoPagadoTotal { get; set; }
        public decimal SaldoPendienteTotal { get; set; }
        public List<CuotaVM>? Cuotas { get; set; }
    }
}