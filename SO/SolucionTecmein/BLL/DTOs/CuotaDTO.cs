using System;

namespace BLL.DTOs
{
    public class CuotaDTO
    {
        public int IdCuota { get; set; }
        public int NumeroCuota { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public decimal MontoEsperado { get; set; }
        public string Estado { get; set; }
    }
}
