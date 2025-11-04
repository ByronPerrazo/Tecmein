

namespace TecmeinAplicacionWeb.Models.ViewModels
{
    public class DashBoardVM
    {
        public int totalVisitasUltimaSemana { get; set; }
        public int totalEquipos { get; set; }
        public string? totalIngresosUltimaSemana { get; set; }
        public int totalMarcas { get; set; }

        public List<MarcasMasVendidasVM> listaMarcasMasVendidasVM { get; set; }
        public List<VisitasUktimaSemanaVM> listaVisitasUktimaSemanaVM { get; set; }
        public Dictionary<string, int> VisitasPorEtapa { get; set; }
        public Dictionary<string, int> ContratosPorMes { get; set; }
        public Dictionary<string, int> TopClientesConMasContratos { get; set; }
    }

    public class MarcasMasVendidasVM { 
        public string Marca { get; set; }
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
        public string Fecha { get; set; }
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
