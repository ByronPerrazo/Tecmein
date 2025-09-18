
using System;

namespace BLL.DTOs
{
    public class PlaceholderDataDTO
    {
        public string ValorContrato { get; set; } = string.Empty;
        public string ValorAnticipo { get; set; } = string.Empty;
        public string FechaAnticipo { get; set; } = string.Empty;
        public string NumeroCuotas { get; set; } = string.Empty;
        public string FechaPrimeraCuota { get; set; } = string.Empty;
        public string DiasEntrega { get; set; } = string.Empty;
        public string TipoDias { get; set; } = string.Empty;
        public string PeriodoMantenimiento { get; set; } = string.Empty;
        public string AniosGarantia { get; set; } = string.Empty;
        public string MesesGarantia { get; set; } = string.Empty;
        public string PolizaGarantia { get; set; } = string.Empty;
        public ClientePlaceholderDTO Cliente { get; set; }
        public CotizacionPlaceholderDTO Cotizacion { get; set; }

        public PlaceholderDataDTO()
        {
            Cliente = new ClientePlaceholderDTO();
            Cotizacion = new CotizacionPlaceholderDTO();
        }
    }

    public class ClientePlaceholderDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Administrador { get; set; } = string.Empty;
    }

    public class CotizacionPlaceholderDTO
    {
        public string Numero { get; set; } = string.Empty;
        public string Fecha { get; set; } = string.Empty;
        public string Total { get; set; } = string.Empty;
    }
}
