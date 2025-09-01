using System;

namespace TecmeinWebApp.Models.ViewModel
{
    public class PlantillaPreContratoParrafoVM
    {
        public int SecPlantillaPreContratoParrafo { get; set; }
        public int SecPlantillaPreContrato { get; set; }
        public int Orden { get; set; }
        public string Contenido { get; set; } = null!;
        public bool EstaActivo { get; set; }
    }
}
