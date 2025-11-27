using System;
using System.Collections.Generic;

namespace TecmeinAplicacionWeb.Models.ViewModels
{
    public class HistorialPagoCuotaVM
    {
        public int IdPago { get; set; }
        public int IdCuota { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; }
        public string? ComprobanteUrl { get; set; }
        public string? ComprobanteNombre { get; set; }
        public int RegistradoPorUsuarioId { get; set; }
        public string? RegistradoPorUsuarioNombre { get; set; } // Para mostrar el nombre del usuario
        public bool EstaActivo { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}