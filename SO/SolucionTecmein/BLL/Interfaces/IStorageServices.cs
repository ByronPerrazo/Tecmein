using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IStorageServices
    {

        Task<string> SubirStorage(Stream? RepositorioExterno, string? CarpetaDestino, string? NombreArchivo);
        Task<bool> EliminarStorage(string? carpetaDestino, string? nombreArchivo);
    }
}
