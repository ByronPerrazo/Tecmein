//using ImageMagick;
namespace BLL.Interfaces
{
    public interface IUtilidadesServices
    {
        string GenerarClave(int longitud);
        string ConvertirSha256(string texto);
        //Stream ConvertToWebPComprimido(string? nombreArchivo, string? inputImagePath, string? outputImagePath);


    }
}
