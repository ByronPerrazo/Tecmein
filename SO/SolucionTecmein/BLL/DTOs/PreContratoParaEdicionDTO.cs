using System;
using System.Collections.Generic;

namespace BLL.DTOs
{
    public class PreContratoParaEdicionDTO
    {
        public int SecPreContrato { get; set; }
        public int SecCotizacion { get; set; }
        public int SecTipoDocumento { get; set; } // Nuevo campo para el tipo de documento del pre-contrato
        public int Dias { get; set; }
        public string TipoDias { get; set; }
        public string PeriodoMantenimiento { get; set; }
        public int AniosGarantia { get; set; }
        public int MesesGarantia { get; set; }
        public string PolizaGarantia { get; set; }

        public List<CompromisoPagoDTO> CompromisosDePago { get; set; }

        public PreContratoParaEdicionDTO()
        {
            CompromisosDePago = new List<CompromisoPagoDTO>();
        }
    }
}
