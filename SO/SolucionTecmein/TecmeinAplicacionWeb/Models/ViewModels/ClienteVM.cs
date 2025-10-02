
namespace TecmeinAplicacionWeb.Models.ViewModels
{
    public class ClienteVM
    {
        public int SecCliente { get; set; }
        public int SecConstructora { get; set; }
        public string NumeroCliente { get; set; }
        public string NombreConstructora { get; set; }
        public string FechaCreacion { get; set; }
        public bool EstaActivo { get; set; }
    }
}
