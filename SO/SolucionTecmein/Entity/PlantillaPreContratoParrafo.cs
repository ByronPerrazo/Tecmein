using System;
using System.Collections.Generic;

namespace Entity
{
    public partial class PlantillaPreContratoParrafo
    {
        public int SecPlantillaPreContratoParrafo { get; set; }
        public int? SecPlantillaPreContrato { get; set; }
        public int? Orden { get; set; }
        public string? Contenido { get; set; }
        public int? EstaActivo { get; set; }

        public virtual PlantillaPreContrato? SecPlantillaPreContratoNavigation { get; set; }
    }
}