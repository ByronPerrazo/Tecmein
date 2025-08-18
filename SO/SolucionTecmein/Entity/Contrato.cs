using System;
using System.ComponentModel.DataAnnotations;

namespace Entity
{
    public partial class Contrato
    {
        [Key]
        public int SecContrato { get; set; }
        public int SecPreContrato { get; set; }
        public DateTime FechaFirma { get; set; }
        public bool EstaFirmado { get; set; }
        public string? UrlDocumento { get; set; }
        public string? NombreDocumento { get; set; }
        public short EstaActivo { get; set; }
        public DateTime FechaRegistro { get; set; }

        public virtual PreContrato SecPreContratoNavigation { get; set; } = null!;
    }
}
