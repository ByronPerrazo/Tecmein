using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TecmeinWebApp.Models.ViewModel
{
    public class PreContratoVM
    {
        public int SecPreContrato { get; set; }
        public int SecCotizacion { get; set; }
        public int SecPlantillaPreContrato { get; set; }
        public int SecUsuarioCrea { get; set; }
        public int? SecFormaPago { get; set; }
        public int Version { get; set; }

        [Required(ErrorMessage = "El Estado es obligatorio")]
        public string Estado { get; set; }

        public bool EstaActivo { get; set; }
        public DateTime? FechaRegistro { get; set; }

        public int Dias { get; set; }

        [Required(ErrorMessage = "El Tipo de Días es obligatorio")]
        public string TipoDias { get; set; }

        public decimal ValorContrato { get; set; }
        public int AniosGarantia { get; set; }
        public int MesesGarantia { get; set; }

        [Required(ErrorMessage = "El Periodo de Mantenimiento es obligatorio")]
        public string PeriodoMantenimiento { get; set; }

        [Required(ErrorMessage = "La Póliza de Garantía es obligatoria")]
        public string PolizaGarantia { get; set; }

        public decimal ValorAnticipo { get; set; }
        public DateTime? FechaAnticipo { get; set; }
        public int NumeroCuotas { get; set; }
        public DateTime? FechaPrimeraCuota { get; set; }

        public string NombreUsuarioCrea { get; set; }
        public string NombreObra { get; set; }

        public List<SelectListItem> Cotizaciones { get; set; }
        public List<SelectListItem> Plantillas { get; set; }
        public List<SelectListItem> FormasPago { get; set; }
    }
}
