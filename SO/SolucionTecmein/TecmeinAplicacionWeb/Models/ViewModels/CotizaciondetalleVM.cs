namespace TecmeinAplicacionWeb.Models.ViewModels
{
    public class CotizaciondetalleVM
    {
        public int Secuencial { get; set; }
        public int SecCotizacion { get; set; }
        public string DetalleEquipo { get; set; }
        public decimal ValorCompra { get; set; }
        public decimal MargenGanancia { get; set; }
        public decimal Total { get; set; }
        public int Cantidad { get; set; }
        public int EstaActivo { get; set; }
    }
}
