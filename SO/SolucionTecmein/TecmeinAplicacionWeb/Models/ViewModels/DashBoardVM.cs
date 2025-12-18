

namespace TecmeinAplicacionWeb.Models.ViewModels
{
    public class DashBoardVM
    {
        public int totalVisitasUltimaSemana { get; set; }
        public int totalEquipos { get; set; }
        public string? totalIngresosUltimaSemana { get; set; }
        public int totalMarcas { get; set; }

        public int TotalContratos { get; set; }
        public string? IngresosMensuales { get; set; }
        public int PagosVencidos { get; set; }
        public int NuevosClientes { get; set; }

        public List<MarcasMasVendidasVM> listaMarcasMasVendidasVM { get; set; } = new();
        public List<VisitasUktimaSemanaVM> listaVisitasUktimaSemanaVM { get; set; } = new();
        public Dictionary<string, int> VisitasPorEtapa { get; set; } = new();
        public Dictionary<string, int> ContratosPorMes { get; set; } = new();
        public Dictionary<string, int> TopClientesConMasContratos { get; set; } = new();
    }

    public class MarcasMasVendidasVM { 
        public string Marca { get; set; } = string.Empty;
        public int TotalCantidad { get; set; }
        public MarcasMasVendidasVM()
        {
                
        }
        public MarcasMasVendidasVM(string marca, int totalCantidad)
        {
            Marca = marca;
            TotalCantidad = totalCantidad;
        }
    }
    public class VisitasUktimaSemanaVM
    {
        public string Fecha { get; set; } = string.Empty;
        public int Total { get; set; }
        public VisitasUktimaSemanaVM()
        {
            
        }
        public VisitasUktimaSemanaVM(string fecha, int total)
        {
            Fecha = fecha;
            Total = total;
        }
    }
}
