using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TecmeinWebApp.Models.ViewModel
{
    public class RolPermisosGestionVM
    {
        public int SecRol { get; set; }
        public string NombreRol { get; set; }
        public List<PermisoVM> Permisos { get; set; }
    }

    public class PermisoVM
    {
        public string IdPermiso { get; set; }
        public string Descripcion { get; set; }
        public bool Asignado { get; set; }
    }
}
