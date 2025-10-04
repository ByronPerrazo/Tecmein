using System;

namespace Entity
{
    public class AuditoriaEvento
    {
        public long Id { get; set; }
        public DateTime FechaHora { get; set; }
        public int? IdUsuario { get; set; }
        public string? NombreUsuario { get; set; }
        public string TipoEvento { get; set; }
        public string Detalle { get; set; }
        public string? DireccionIp { get; set; }
    }
}
