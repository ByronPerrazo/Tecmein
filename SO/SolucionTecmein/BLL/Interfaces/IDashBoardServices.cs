using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IDashBoardServices
    {
        Task<int> TotalVisitasUltimaSemana();
        Task<string> TotalIngresosUltimaSemana();
        Task<int> TotalEquipos();
        Task<int> TotalMarcas();
        Task<Dictionary<string, int>> VisitasUltimaSemana();
        Task<Dictionary<string, int>> MarcasMasVendidas();

    }
}
