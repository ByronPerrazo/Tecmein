using System.Collections.Generic;

namespace TecmeinAplicacionWeb.Models.ViewModels
{
    public class RolVM
    {
        public int Secuencial { get; set; }
        public string? Descripcion { get; set; }
        public string? FechaRegistroString { get; set; }
        public int EsActivo { get; set; }
        public List<RolPermisoVM> Permisos { get; set; } = new List<RolPermisoVM>();
    }
}
