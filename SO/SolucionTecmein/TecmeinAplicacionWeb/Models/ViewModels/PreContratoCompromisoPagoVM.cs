namespace TecmeinAplicacionWeb.Models.ViewModels
{
    public class PreContratoCompromisoPagoVM
    {
        public int Secuencial { get; set; }
        public int SecPreContrato { get; set; }
        public int NumeroCuota { get; set; }
        public decimal Monto { get; set; }
        public string FechaVencimiento { get; set; } // Formato string para la vista
        public string Tipo { get; set; }
    }
}