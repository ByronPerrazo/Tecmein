namespace TecmeinAplicacionWeb.Models.ViewModels
{
    public class GestionRolMenuVM
    {
        public int SecRol { get; set; }
        public string NombreRol { get; set; } = string.Empty;
        public List<MenuPermisoVM> Menus { get; set; } = new();
    }

    public class MenuPermisoVM
    {
        public int SecMenu { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string Icono { get; set; } = string.Empty;
        public int? SecMenuPadre { get; set; }
        public bool VerMenu { get; set; }
        public bool Crear { get; set; }
        public bool Leer { get; set; }
        public bool Actualizar { get; set; }
        public bool Eliminar { get; set; }
        public List<MenuPermisoVM> SubMenus { get; set; } = new();
    }
}
