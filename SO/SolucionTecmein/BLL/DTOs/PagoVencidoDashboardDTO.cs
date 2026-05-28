namespace BLL.DTOs
{
    public class PagoVencidoDashboardDTO
    {
        public string? NumeroContrato { get; set; }
        public string? NombreCliente { get; set; }
        public int NumeroCuota { get; set; }
        public decimal MontoCuota { get; set; }
        public string? FechaVencimiento { get; set; }
        public int DiasVencidos { get; set; }
    }
}
