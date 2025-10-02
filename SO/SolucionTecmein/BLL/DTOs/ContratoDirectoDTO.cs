using System;
using System.IO;

namespace BLL.DTOs
{
    public class ContratoDirectoDTO
    {
        public int SecCliente { get; set; }
        public DateTime FechaFirma { get; set; }
        public int IdUsuarioCarga { get; set; }
        public Stream? ArchivoStream { get; set; }
        public string? NombreArchivo { get; set; }

        // Datos opcionales para PlanDePago
        public bool GenerarPlanDePago { get; set; }
        public int? SecFormaPago { get; set; }
        public decimal? ValorContrato { get; set; }
        public decimal? ValorAnticipo { get; set; }
        public DateTime? FechaAnticipo { get; set; }
        public int? NumeroCuotas { get; set; }
        public DateTime? FechaPrimeraCuota { get; set; }
    }
}
