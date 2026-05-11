namespace BLL.DTOs
{
    public class CuotaDTO
    {
        public int IdCuota { get; set; }
        public int NumeroCuota { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public DateTime FechaVencimiento { get; set; }
        public decimal MontoEsperado { get; set; }
        public decimal? MontoPagado { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}
