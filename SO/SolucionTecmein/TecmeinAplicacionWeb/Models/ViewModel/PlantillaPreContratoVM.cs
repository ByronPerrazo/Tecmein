using System;

namespace TecmeinWebApp.Models.ViewModel
{
    public class PlantillaPreContratoVM
    {
        public int SecPlantillaPreContrato { get; set; }
        public string Nombre { get; set; } = null!;
        public string NumeracionInicial { get; set; } = null!;
        public short EstaActivo { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
