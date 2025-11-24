namespace BLL.Models.ViewModels
{
    public class CuotaVM
    {
        public int IdCuota { get; set; }
        public int NumeroCuota { get; set; }
        public decimal MontoEsperado { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string? Estado { get; set; }
        public decimal? MontoPagado { get; set; }
    }
}
