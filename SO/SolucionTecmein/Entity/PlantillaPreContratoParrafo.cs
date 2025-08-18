using System;
using System.ComponentModel.DataAnnotations;

namespace Entity
{
    public partial class PlantillaPreContratoParrafo
    {
        [Key]
        public int SecPlantillaPreContratoParrafo { get; set; }
        public int SecPlantillaPreContrato { get; set; }
        public int Orden { get; set; }
        public string Contenido { get; set; } = null!;
        public short EstaActivo { get; set; }

        public virtual PlantillaPreContrato SecPlantillaPreContratoNavigation { get; set; } = null!;
    }
}
