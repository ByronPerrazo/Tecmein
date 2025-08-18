using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Entity
{
    public partial class PlantillaPreContrato
    {
        [Key]
        public int SecPlantillaPreContrato { get; set; }
        public string Nombre { get; set; } = null!;
        public string NumeracionInicial { get; set; } = null!;
        public short EstaActivo { get; set; }
        public DateTime FechaRegistro { get; set; }

        public virtual ICollection<PlantillaPreContratoParrafo> PlantillaPreContratoParrafos { get; set; } = new List<PlantillaPreContratoParrafo>();
    }
}
