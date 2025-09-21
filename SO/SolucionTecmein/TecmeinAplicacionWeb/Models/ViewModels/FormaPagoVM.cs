namespace TecmeinAplicacionWeb.Models.ViewModels
{
    public class FormaPagoVM
    {
        public int SecFormaPago { get; set; }
        public string Descripcion { get; set; } = null!;
        public short EstaActivo { get; set; }
    }
}
