using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IProvinciaServices
    {
        Task<List<Provincia>> Lista();
        Task <Provincia> ProvinciaPorSecuencial(int secuencial);

    }
}
