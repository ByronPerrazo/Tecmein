using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface ICorreoServices
    {

        Task<bool> EnvioCorreo(string Destino, string Asunto, string Mensaje);
    }
}
