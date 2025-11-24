namespace BLL.Models.ViewModels
{
    public class PlanPagoDashboardVM
    {
        public int IdPlanDePago { get; set; }
        public string? NumeroContrato { get; set; }
        public string? NombreCliente { get; set; }
        public decimal ValorTotalContrato { get; set; }
        public decimal MontoPagado { get; set; }
        public decimal SaldoPendiente { get; set; }
        public string? EstadoPlan { get; set; }
        // Otros campos relevantes para el dashboard si son necesarios
    }
}
