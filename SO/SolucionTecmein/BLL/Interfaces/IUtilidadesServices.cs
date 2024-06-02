//using ImageMagick;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IUtilidadesServices
    {
        string GenerarClave(int longitud);
        string ConvertirSha256(string texto);
        //Stream ConvertToWebPComprimido(string? nombreArchivo, string? inputImagePath, string? outputImagePath);


    }
}
