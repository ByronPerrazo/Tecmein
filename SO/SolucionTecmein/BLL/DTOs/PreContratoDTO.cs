using System;

namespace BLL.DTOs
{
    public class PreContratoDTO
    {
        public int SecPreContrato { get; set; }
        public int SecCotizacion { get; set; }
        public string? NumeroCotizacion { get; set; }
        public string? NombreCliente { get; set; }
        public decimal? ValorContrato { get; set; }
        public string? Estado { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public int? Version { get; set; }
        public bool? EstaActivo { get; set; }
        public string? NombreObra { get; set; }
        public string? NombreUsuarioCrea { get; set; }
        public int SecPlantillaPreContrato { get; set; }
        public string? CodigoTipoDocumento { get; set; }
        public int? Dias { get; set; }
        public string? TipoDias { get; set; }
        public string? PeriodoMantenimiento { get; set; }
        public int? AniosGarantia { get; set; }
        public int? MesesGarantia { get; set; }
        public string? PolizaGarantia { get; set; }
        public List<CompromisoPagoDTO> PreContratoCompromisoPagos { get; set; } = new List<CompromisoPagoDTO>();
    }
}
