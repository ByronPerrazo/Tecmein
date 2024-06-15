using Entity;

namespace TecmeinWebApp.Models.ViewModel
{
    public class RolMenuVM
    {
        public int Secuencial { get; set; }

        public int? SecRol { get; set; }

        public int? SecMenu { get; set; }

        public ulong? EsActivo { get; set; }

        public DateTime? FechaRegistro { get; set; }

        public virtual Menu? SecMenuNavigation { get; set; }

    }
}
