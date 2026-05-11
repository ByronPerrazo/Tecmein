namespace BLL.DTOs
{
    public class HistorialPagoCuotaDTO
    {
        public int IdPago { get; set; }
        public int IdCuota { get; set; } // Aunque Pago no está directamente enlazado a Cuota, es útil para el contexto del historial
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; }
        public string? ComprobanteUrl { get; set; }
        public string? ComprobanteNombre { get; set; }
        public int? RegistradoPorUsuarioId { get; set; }
        public string? RegistradoPorUsuarioNombre { get; set; } // Para mostrar el nombre del usuario que registró el pago
        public bool EstaActivo { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}