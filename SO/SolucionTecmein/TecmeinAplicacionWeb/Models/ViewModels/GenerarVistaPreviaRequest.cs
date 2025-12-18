namespace TecmeinAplicacionWeb.Models.ViewModels
{
    public class GenerarVistaPreviaRequest
    {
        public int SecCotizacion { get; set; }
        public int SecFormaPago { get; set; }
        public int SecPlantillaPreContrato { get; set; }
        public decimal ValorContrato { get; set; }
        public decimal ValorAnticipo { get; set; }
        public string FechaAnticipo { get; set; } = string.Empty;
        public int NumeroCuotas { get; set; }
        public string FechaPrimeraCuota { get; set; } = string.Empty;
        public int Dias { get; set; }
        public string TipoDias { get; set; } = string.Empty;
        public string PeriodoMantenimiento { get; set; } = string.Empty;
        public int AniosGarantia { get; set; }
        public int MesesGarantia { get; set; }
        public string PolizaGarantia { get; set; } = string.Empty;
    }
}
