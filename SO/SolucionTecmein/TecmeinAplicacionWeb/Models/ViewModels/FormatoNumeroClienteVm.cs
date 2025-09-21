namespace TecmeinAplicacionWeb.Models.ViewModels
{
    public class FormatoNumeroClienteVm
    {
        public int SecFormatoNumeroCliente { get; set; }
        public int SecEmpresa { get; set; }
        public bool UsaFormato { get; set; }
        public string? Formato { get; set; }
        public int NumeroInicio { get; set; }
    }
}
