using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entity
{
    [Table("tipodocumento")]
    public partial class TipoDocumento
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SecTipoDocumento { get; set; }

        [Required]
        [StringLength(50)]
        public string Codigo { get; set; }

        [Required]
        [StringLength(255)]
        public string Descripcion { get; set; }

        public bool? EstaActivo { get; set; }

        public DateTime? FechaRegistro { get; set; }

        public int? SecPlantilla { get; set; }

        [ForeignKey("SecPlantilla")]
        public virtual PlantillaPreContrato? SecPlantillaNavigation { get; set; }
    }
}
