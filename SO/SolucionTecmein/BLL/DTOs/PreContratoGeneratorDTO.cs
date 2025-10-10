using System;

namespace BLL.DTOs
{
    public class PreContratoGeneratorDTO
    {
        public int SecCotizacion { get; set; }
        // Campos relacionados con pagos y plantilla se manejan en el backend o post-contrato
        public int Dias { get; set; }
        public string TipoDias { get; set; }
        public string PeriodoMantenimiento { get; set; }
        public int AniosGarantia { get; set; }
        public int MesesGarantia { get; set; }
        public string PolizaGarantia { get; set; }
    }
}
