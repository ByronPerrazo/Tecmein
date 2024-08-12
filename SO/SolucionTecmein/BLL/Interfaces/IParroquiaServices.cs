using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IParroquiaServices
    {
        Task<List<Parroquia>> Lista();
        Task<List<Parroquia>> ListaPorCanton(int secCanton);

        Task<Parroquia> ParroquiasPorSecuencial(int secuencialProvincia);
    }
}
