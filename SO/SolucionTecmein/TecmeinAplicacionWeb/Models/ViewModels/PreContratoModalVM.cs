namespace TecmeinAplicacionWeb.Models.ViewModels
{
    public class PreContratoModalVM
    {
        public int SecCotizacion { get; set; }
        public int? SecFormaPago { get; set; }
        public int SecPlantillaPreContrato { get; set; }
        public decimal ValorContrato { get; set; }
        public decimal ValorAnticipo { get; set; }
        public DateTime? FechaAnticipo { get; set; }
        public int NumeroCuotas { get; set; }
        public DateTime? FechaPrimeraCuota { get; set; }
        public int Dias { get; set; }
        public string TipoDias { get; set; } = string.Empty;
        public string PeriodoMantenimiento { get; set; } = string.Empty;
        public int AniosGarantia { get; set; }
        public int MesesGarantia { get; set; }
        public string PolizaGarantia { get; set; } = string.Empty;
    }
}
