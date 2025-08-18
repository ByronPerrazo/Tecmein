using System;

namespace TecmeinWebApp.Models.ViewModel
{
    public class SeguimientoVM
    {
        public int SecSeguimiento { get; set; }
        public int SecCotizacion { get; set; }
        public string Accion { get; set; } = null!;
        public string Detalle { get; set; } = null!;
        public DateTime FechaAccion { get; set; }
        public bool AceptacionCliente { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
