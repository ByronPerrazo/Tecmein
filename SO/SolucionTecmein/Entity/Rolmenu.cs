using System.ComponentModel.DataAnnotations.Schema;

namespace Entity;

// Esta entidad ahora representa la matriz de permisos CRUD para un Rol en un Menú específico.
public partial class RolMenu
{
    // La clave primaria compuesta (SecRol, SecMenu) se configurará en el DbContext usando Fluent API.
    public int SecRol { get; set; }
    public int SecMenu { get; set; }

    // Visibilidad
    public bool VerMenu { get; set; }

    // Permisos CRUD
    public bool Crear { get; set; }
    public bool Leer { get; set; }
    public bool Actualizar { get; set; }
    public bool Eliminar { get; set; }


    [ForeignKey(nameof(SecMenu))]
    public virtual Menu SecMenuNavigation { get; set; }

    [ForeignKey(nameof(SecRol))]
    public virtual Rol SecRolNavigation { get; set; }
}
