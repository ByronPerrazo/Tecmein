namespace TecmeinAplicacionWeb.Models.ViewModels
{
    public class GestionRolPermisoVM
    {
        public int SecRol { get; set; }
        public string? NombreRol { get; set; }
        public List<MenuVM> TodosLosMenus { get; set; } = new List<MenuVM>();
        public List<PermisoVM> TodosLosPermisos { get; set; } = new List<PermisoVM>();
        public HashSet<int> MenusAsignados { get; set; } = new HashSet<int>();
        public HashSet<string> PermisosAsignados { get; set; } = new HashSet<string>();
    }
}
