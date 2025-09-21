using Entity;

namespace TecmeinAplicacionWeb.Models.ViewModels
{
    public class RolMenuVM
    {
        public int Secuencial { get; set; }
        public int? SecRol { get; set; }
        public string? DescripcionRol { get; set; }
        public int? SecMenu { get; set; }
        public string? DescripcionMenu { get; set; }
        public short? EsActivo { get; set; }
        public DateTime? FechaRegistro { get; set; }
    }
}