using System.ComponentModel.DataAnnotations;

namespace TecmeinAplicacionWeb.Models.ViewModels
{
    public class PolizaGarantiaVM
    {
        public int Secuencial { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        public string Descripcion { get; set; }

        public bool EstaActivo { get; set; }
    }
}