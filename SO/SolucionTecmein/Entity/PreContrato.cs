using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Entity
{
    public partial class PreContrato
    {
        [Key]
        public int SecPreContrato { get; set; }
        public int SecCotizacion { get; set; }
        public int Dias { get; set; }
        public string TipoDias { get; set; } = null!;
        public decimal ValorContrato { get; set; }
        public int AniosGarantia { get; set; }
        public int MesesGarantia { get; set; }
        public string PeriodoMantenimiento { get; set; } = null!;
        public string PolizaGarantia { get; set; } = null!;
        public decimal ValorAnticipo { get; set; }
        public DateTime FechaAnticipo { get; set; }
        public string FormaPago { get; set; } = null!;
        public int NumeroCuotas { get; set; }
        public DateTime FechaPrimeraCuota { get; set; }
        public short EstaActivo { get; set; }
        public DateTime FechaRegistro { get; set; }

        public virtual Cotizacion SecCotizacionNavigation { get; set; } = null!;
        public virtual ICollection<PlantillaPreContratoParrafo> PlantillaPreContratoParrafos { get; set; } = new List<PlantillaPreContratoParrafo>();
    }
}
