namespace TecmeinAplicacionWeb.Models.ViewModels
{
    public class PlanDePagoVM
    {
        public int IdPlanDePago { get; set; }
        public int IdContrato { get; set; }
        public int SecFormaPago { get; set; }
        public string DescripcionFormaPago { get; set; } = string.Empty; // Para mostrar en la vista
        public decimal ValorContrato { get; set; }
        public decimal ValorAnticipo { get; set; }
        public string? FechaAnticipo { get; set; } // Formato string para la vista
        public int NumeroCuotas { get; set; }
        public string? FechaPrimeraCuota { get; set; } // Formato string para la vista
        public bool EstaActivo { get; set; }
        public string FechaRegistro { get; set; } = string.Empty; // Formato string para la vista
        public List<CuotaVM> Cuotas { get; set; } = new(); // Para la parrilla de cuotas
    }
}