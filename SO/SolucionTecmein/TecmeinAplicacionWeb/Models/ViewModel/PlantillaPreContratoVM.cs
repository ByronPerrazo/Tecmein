using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TecmeinWebApp.Models.ViewModel
{
    public class PlantillaPreContratoVM
    {
        public int SecPlantillaPreContrato { get; set; }
        public string? Nombre { get; set; }
        public string? NumeracionInicial { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public int? EstaActivo { get; set; }
        public int SecTipoDocumento { get; set; }
        public string? DescripcionTipoDocumento { get; set; } // Nueva propiedad para la descripción
    }
}
