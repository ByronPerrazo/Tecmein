using System;
using System.Collections.Generic;

namespace Entity
{
    public partial class PlantillaPreContratoParrafo
    {
        public int SecPlantillaPreContratoParrafo { get; set; }
        public int SecPlantillaPreContrato { get; set; }
        public int Orden { get; set; }
        public byte[]? Contenido { get; set; }
        public bool? EstaActivo { get; set; }

        public virtual PlantillaPreContrato? SecPlantillaPreContratoNavigation { get; set; }
    }
}