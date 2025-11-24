namespace BLL.Models.ViewModels
{
    public class RegistrarPagoVM
    {
        public int IdPlanDePago { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; }
        public string? ComprobanteUrl { get; set; }
        public string? ComprobanteNombre { get; set; }
        public int RegistradoPorUsuarioId { get; set; } // Se asignará desde el controlador/claims
    }
}
