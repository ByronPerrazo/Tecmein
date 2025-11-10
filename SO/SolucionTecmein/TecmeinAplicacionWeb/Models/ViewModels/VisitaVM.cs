using System.ComponentModel.DataAnnotations;

namespace TecmeinAplicacionWeb.Models.ViewModels
{
    public class VisitaVM
    {
        public int Secuencial { get; set; }

        [Required(ErrorMessage = "El nombre de la obra es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres")]
        public string? Nombre { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una provincia")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una provincia")]
        public int? SecProvincia { get; set; }
        public string? NombreProvincia { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un cantón")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un cantón")]
        public int? SecCanton { get; set; }
        public string? NombreCanton { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una parroquia")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una parroquia")]
        public int? SecParroquia { get; set; }
        public string? NombreParroquia { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria")]
        public string? Direccion { get; set; }

        public DateTime? FechaRegistro { get; set; }

        public string? GeoUbicacion { get; set; }

        public short? EstaActivo { get; set; }

        public int SecUsuario { get; set; }

        public DateTime? FechaSiguienteVisita { get; set; }

        public string? Detalle { get; set; }

        public int IdEtapa { get; set; }
        public string? DescripcionEtapa { get; set; }
        public string? CodigoEtapa { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un operador")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un operador")]
        public int? SecEmpresa { get; set; }
        public string? NombreEmpresa { get; set; }

        public int? SecConstructora { get; set; }
        public string? NombreConstructora { get; set; }

        //public virtual ICollection<ContactoVisita> Contactovista { get; set; } = new List<ContactoVisita>();

    }
}
