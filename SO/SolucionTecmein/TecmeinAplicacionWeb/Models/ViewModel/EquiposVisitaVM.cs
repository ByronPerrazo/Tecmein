namespace TecmeinWebApp.Models.ViewModel
{
    public class EquiposVisitaVM
    {
        public int Secuencial { get; set; }

        public int SecVisita { get; set; }

        public string TipoEquipo { get; set; } = null!;

        public string Sistema { get; set; } = null!;

        public string Marca { get; set; } = null!;

        public int Capacidad { get; set; }

        public decimal? Velocidad { get; set; }

        public string? SalaMaquinas { get; set; }

        public string? SalaControl { get; set; }

        public int? NumeroPersonas { get; set; }

        public int? NumeroParadas { get; set; }

        public string? NombresParadas { get; set; }

        public string? Embarque { get; set; }

        public string? TipoDucto { get; set; }

        public string? MedidasAfducto { get; set; }

        public string? TipoMotor { get; set; }

        public int? Foso { get; set; }

        public int? Recorrido { get; set; }

        public int? IngresosFrontales { get; set; }

        public int? IngresosPosteriores { get; set; }

        public int? SobreRecorrido { get; set; }

        public int? DimencionEntrada { get; set; }

        public int? AlturaEntrePisos { get; set; }

        public string? MaterialPuertas { get; set; }

        public string? Energia { get; set; }

        public int? Cantidad { get; set; }

        public short EstaActivo { get; set; }
    }
}
