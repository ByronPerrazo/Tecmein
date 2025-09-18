using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entity
{
    [Table("precontratoparrafo")] // Add this attribute
    public partial class PreContratoParrafo
    {
        [Key]
        public int Secuencial { get; set; }

        public int SecPreContrato { get; set; }

        [Column(TypeName = "TEXT")]
        public string Contenido { get; set; }

        public int Orden { get; set; }

        [ForeignKey("SecPreContrato")]
        public virtual PreContrato SecPreContratoNavigation { get; set; }
    }
}
