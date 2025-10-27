using System.Collections.Generic;

namespace TecmeinAplicacionWeb.Models.ViewModels
{
    public class GestionRolMenuVM
    {
        public int SecRol { get; set; }
        public string NombreRol { get; set; }
        public List<MenuPermisoVM> Menus { get; set; }
    }

    public class MenuPermisoVM
    {
        public int SecMenu { get; set; }
        public string Descripcion { get; set; }
        public string Icono { get; set; }
        public int? SecMenuPadre { get; set; }
        public bool VerMenu { get; set; }
        public bool Crear { get; set; }
        public bool Leer { get; set; }
        public bool Actualizar { get; set; }
        public bool Eliminar { get; set; }
        public List<MenuPermisoVM> SubMenus { get; set; }
    }
}
