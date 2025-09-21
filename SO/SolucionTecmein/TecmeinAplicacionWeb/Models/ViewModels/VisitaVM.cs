namespace TecmeinAplicacionWeb.Models.ViewModels
{
    public class VisitaVM
    {
        public int Secuencial { get; set; }

        public string? Nombre { get; set; }

        public int? SecProvincia { get; set; }
        public string? NombreProvincia { get; set; }

        public int? SecCanton { get; set; }
        public string? NombreCanton { get; set; }

        public int? SecParroquia { get; set; }
        public string? NombreParroquia { get; set; }

        public string? Direccion { get; set; }

        public DateTime? FechaRegistro { get; set; }

        public string? GeoUbicacion { get; set; }

        public short? EstaActivo { get; set; }

        public int SecUsuario { get; set; }

        public DateTime? FechaSiguienteVisita { get; set; }

        public string? Detalle { get; set; }

        public int IdEtapa { get; set; }
        public string? DescripcionEtapa { get; set; }

        public int? SecEmpresa { get; set; }
        public string? NombreEmpresa { get; set; }

        //public virtual ICollection<ContactoVisita> Contactovista { get; set; } = new List<ContactoVisita>();

    }
}
