using System.ComponentModel.DataAnnotations;

namespace Entity
{
    public partial class Seguimiento
    {
        [Key]
        public int SecSeguimiento { get; set; }
        public int SecCotizacion { get; set; }
        public string Accion { get; set; } = null!;
        public string Detalle { get; set; } = null!;
        public DateTime FechaAccion { get; set; }
        public bool AceptacionCliente { get; set; }
        public DateTime FechaRegistro { get; set; }

        public virtual Cotizacion SecCotizacionNavigation { get; set; } = null!;
    }
}
