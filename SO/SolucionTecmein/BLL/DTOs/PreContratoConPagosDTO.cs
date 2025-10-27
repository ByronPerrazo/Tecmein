using System;
using System.Collections.Generic;

namespace BLL.DTOs
{
    public class PreContratoConPagosDTO
    {
        public int SecPreContrato { get; set; } // Añadido para identificar el pre-contrato a actualizar
        public int SecCotizacion { get; set; }
        public int Dias { get; set; }
        public string TipoDias { get; set; }
        public string PeriodoMantenimiento { get; set; }
        public int AniosGarantia { get; set; }
        public int MesesGarantia { get; set; }
        public string PolizaGarantia { get; set; }
        public string ContenidoHtml { get; set; }
        public List<CompromisoPagoDTO> CompromisosDePago { get; set; }

        public PreContratoConPagosDTO()
        {
            CompromisosDePago = new List<CompromisoPagoDTO>();
        }
    }
}
