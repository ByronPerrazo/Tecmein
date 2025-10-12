
using System;

namespace BLL.DTOs
{
    public class PagoDTO
    {
        public int IdPago { get; set; }
        public int IdPlanDePago { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; }
        public string? ComprobanteUrl { get; set; }
        public string? ComprobanteNombre { get; set; }
        public int? RegistradoPorUsuarioId { get; set; }
        public bool EstaActivo { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
