using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entity
{
    public partial class PreContrato
    {
        public PreContrato()
        {
            PreContratoParrafos = new HashSet<PreContratoParrafo>();
            PreContratoCompromisoPagos = new HashSet<PreContratoCompromisoPago>();
        }

        [Key]
        public int SecPreContrato { get; set; } // Renombrado de Secuencial

        public int SecCotizacion { get; set; }
        public int SecPlantillaPreContrato { get; set; }
        public int SecUsuarioCrea { get; set; }
        // Campos de pago y forma de pago movidos a PlanDePago

        public int Version { get; set; }
        [Required]
        public string Estado { get; set; }
        public bool EstaActivo { get; set; }
        public DateTime FechaRegistro { get; set; } // Renombrado de FechaCreacion

        // Campos de negocio añadidos
        public int Dias { get; set; }
        [Required]
        public string TipoDias { get; set; }
        // ValorContrato movido a PlanDePago
        public int AniosGarantia { get; set; }
        public int MesesGarantia { get; set; }
        [Required]
        public string PeriodoMantenimiento { get; set; }
        [Required]
        public string PolizaGarantia { get; set; }
        // ValorAnticipo, FechaAnticipo, NumeroCuotas, FechaPrimeraCuota movidos a PlanDePago


        [ForeignKey("SecCotizacion")]
        public virtual Cotizacion SecCotizacionNavigation { get; set; }

        [ForeignKey("SecPlantillaPreContrato")]
        public virtual PlantillaPreContrato SecPlantillaPreContratoNavigation { get; set; }

        [ForeignKey("SecUsuarioCrea")]
        public virtual Usuario SecUsuarioCreaNavigation { get; set; }

        // [ForeignKey("SecFormaPago")]
        // public virtual FormaPago SecFormaPagoNavigation { get; set; } // Añadido // Removed

        public virtual ICollection<PreContratoParrafo> PreContratoParrafos { get; set; }
        public virtual ICollection<PreContratoCompromisoPago> PreContratoCompromisoPagos { get; set; }
    }
}
