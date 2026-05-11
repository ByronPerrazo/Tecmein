using System.ComponentModel.DataAnnotations;

namespace Entity
{
    public partial class DiccionarioParametro
    {
        [Key]
        public int Secuencial { get; set; }

        public string Parametro { get; set; }

        public string Descripcion { get; set; }

        public bool EstaActivo { get; set; }
    }
}
