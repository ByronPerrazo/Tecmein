using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IEquiposVisitaServices
    {
        Task<Equiposvisita> Obtener(int secuencial);
        Task<List<Equiposvisita>> Consultar( int secuencialVisita);
        Task<Equiposvisita> ProcesaGuardar(Equiposvisita equiposvisita);
    }
}
