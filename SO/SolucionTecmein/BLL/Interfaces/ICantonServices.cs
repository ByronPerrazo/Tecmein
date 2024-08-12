using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface ICantonServices
    {
        Task<List<Canton>> Lista();

        Task<List<Canton>> ListaPorProvincia(int secuencial);

        Task<Canton> CantonPorSecuencial( int secuencial);
    }
}
