namespace Entity
{
    public partial class PlantillaPreContrato
    {
        public PlantillaPreContrato()
        {
            PlantillaPreContratoParrafos = new HashSet<PlantillaPreContratoParrafo>();
        }

        public int SecPlantillaPreContrato { get; set; }
        public string? Nombre { get; set; }
        public string? NumeracionInicial { get; set; } // Corregido a string
        public DateTime? FechaRegistro { get; set; }
        public int? EstaActivo { get; set; }
        public int SecTipoDocumento { get; set; }

        public virtual TipoDocumento SecTipoDocumentoNavigation { get; set; }

        public virtual ICollection<PlantillaPreContratoParrafo> PlantillaPreContratoParrafos { get; set; }
    }
}