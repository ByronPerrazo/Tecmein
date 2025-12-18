
namespace TecmeinAplicacionWeb.Models.ViewModels
{
    public class ClienteVM
    {
        public int SecCliente { get; set; }
        public int SecConstructora { get; set; }
        public string NumeroCliente { get; set; } = string.Empty;
        public string NombreConstructora { get; set; } = string.Empty;
        public string FechaCreacion { get; set; } = string.Empty;
        public bool EstaActivo { get; set; }
    }
}
