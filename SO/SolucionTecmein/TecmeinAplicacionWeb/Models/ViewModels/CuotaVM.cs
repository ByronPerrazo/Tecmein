namespace TecmeinAplicacionWeb.Models.ViewModels
{
    public class CuotaVM
    {
        public int IdCuota { get; set; }
        public int IdPlanDePago { get; set; }
        public int NumeroCuota { get; set; }
        public decimal MontoEsperado { get; set; }
        public string? FechaVencimiento { get; set; }
        public string? Estado { get; set; }
        public decimal? MontoPagado { get; set; } // Added from BLL version
        public string? FechaRegistro { get; set; }
    }
}
