namespace TecmeinAplicacionWeb.Models.ViewModels
{
    public class RolPermisoVM
    {
        public int IdRolPermiso { get; set; } // Nombre estandarizado
        public int IdRol { get; set; }
        public string IdPermiso { get; set; } = null!;

        // Propiedades adicionales que pueden ser útiles
        public string? NombreRol { get; set; }
        public string? DescripcionPermiso { get; set; }
    }
}
