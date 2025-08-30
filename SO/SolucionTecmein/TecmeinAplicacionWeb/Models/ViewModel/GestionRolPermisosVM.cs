using Entity;

namespace TecmeinWebApp.Models.ViewModel
{
    public class GestionRolPermisosVM
    {
        public int SecRol { get; set; }
        public string? NombreRol { get; set; } 

        // Lista completa de menús y permisos para dibujar la UI
        public List<MenuVM>? TodosLosMenus { get; set; }
        public List<PermisoVM>? TodosLosPermisos { get; set; }

        // Ids de lo que el rol ya tiene asignado, para marcar los checkboxes
        public HashSet<int>? MenusAsignados { get; set; }
        public HashSet<string>? PermisosAsignados { get; set; }

        public GestionRolPermisosVM()
        {
            TodosLosMenus = new List<MenuVM>();
            TodosLosPermisos = new List<PermisoVM>();
            MenusAsignados = new HashSet<int>();
            PermisosAsignados = new HashSet<string>();
        }
    }
}
