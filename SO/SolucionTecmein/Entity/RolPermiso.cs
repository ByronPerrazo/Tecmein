using System.ComponentModel.DataAnnotations.Schema;

namespace Entity
{
    public partial class RolPermiso
    {
        // Clave primaria compuesta configurada en el DbContext
        public int SecRol { get; set; }
        public string IdPermiso { get; set; }

        [ForeignKey(nameof(SecRol))]
        public virtual Rol Rol { get; set; }

        [ForeignKey(nameof(IdPermiso))]
        public virtual Permiso Permiso { get; set; }
    }
}
