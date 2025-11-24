namespace BLL.Models.ViewModels
{
    public class DetallePlanPagoVM
    {
        public int IdPlanDePago { get; set; }
        public string? NumeroContrato { get; set; }
        public string? NombreCliente { get; set; }
        public decimal ValorContrato { get; set; }
        public decimal ValorAnticipo { get; set; }
        public DateTime? FechaAnticipo { get; set; }
        public int NumeroCuotas { get; set; }
        public DateTime? FechaPrimeraCuota { get; set; }
        public decimal MontoPagadoTotal { get; set; }
        public decimal SaldoPendienteTotal { get; set; }
        public List<CuotaVM>? Cuotas { get; set; }
    }
}
