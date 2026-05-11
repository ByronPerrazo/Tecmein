using System.ComponentModel.DataAnnotations;

namespace Entity
{
    public partial class Permiso
    {
        public Permiso()
        {
            RolPermisos = new HashSet<RolPermiso>();
        }

        [Key]
        [StringLength(100)]
        public string IdPermiso { get; set; }

        [Required]
        [StringLength(255)]
        public string Descripcion { get; set; }

        public virtual ICollection<RolPermiso> RolPermisos { get; set; }
    }
}
