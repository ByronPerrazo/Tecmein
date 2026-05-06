using System;

namespace BLL.DTOs
{
    public class CompromisoPagoDTO
    {
        public string Tipo { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public DateTime? FechaVencimiento { get; set; }
    }
}
