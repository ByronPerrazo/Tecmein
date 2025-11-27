namespace BLL.DTOs
{
    public class PlanPagoDashboardDTO
    {
        public int IdPlanDePago { get; set; }
        public string? NumeroContrato { get; set; }
        public string? NombreCliente { get; set; }
        public string? NombreProyecto { get; set; } // Added
        public decimal ValorTotalContrato { get; set; }
        public decimal MontoPagado { get; set; }
        public decimal SaldoPendiente { get; set; }
        public string? EstadoPlan { get; set; }
    }
}