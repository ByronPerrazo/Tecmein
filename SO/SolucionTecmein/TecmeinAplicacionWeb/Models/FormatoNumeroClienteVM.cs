
namespace TecmeinWebApp.Models
{
    public class FormatoNumeroClienteVM
    {
        public int SecFormatoNumeroCliente { get; set; }
        public bool UsaFormato { get; set; } = true;
        public string? Formato { get; set; } = "CLI-{YYYY}-";
        public int NumeroInicio { get; set; } = 1;
    }
}
