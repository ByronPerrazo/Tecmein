using System;
using System.ComponentModel.DataAnnotations;

namespace Entity
{
    public partial class PlantillaCorreo
    {
        [Key]
        public int SecPlantillaCorreo { get; set; }
        public string Nombre { get; set; } = null!;
        public string Asunto { get; set; } = null!;
        public string ContenidoHTML { get; set; } = null!;
        public short EstaActivo { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
