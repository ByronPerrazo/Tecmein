using BLL.Interfaces;
//using ImageMagick;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
//using static System.Net.Mime.MediaTypeNames;

namespace BLL.Implementacion
{
    public class UtilidadesServices : IUtilidadesServices
    {
        public string GenerarClave(int longitud)
        {
           const string caracteresPermitidos = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            var claveAleatoria = new StringBuilder();

            var rnd = new Random();
            while (0 < longitud--)
            {
                claveAleatoria.Append(caracteresPermitidos[rnd.Next(caracteresPermitidos.Length)]);
            }

            return claveAleatoria.ToString();
        }

        public string ConvertirSha256(string texto)
        {
            var sb = new StringBuilder();
            var enco = Encoding.UTF8;
            byte[] result = SHA256.HashData(enco.GetBytes(texto));
            foreach (var item in result)
            {
                sb.Append(item.ToString("x2"));
            }
            return sb.ToString();
        }

        public Stream ConvertToWebPComprimido(string? nombreArchivo, string? inputImagePath, string? outputImagePath)
        {
            try
            {
                ProcessStartInfo startInfo = new()
                {
                    FileName = nombreArchivo,
                    Arguments = $"{inputImagePath} -o {outputImagePath}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using Process? process = Process.Start(startInfo);
                using StreamReader reader = process.StandardOutput;
                string result = reader.ReadToEnd();

                //using MagickImage image = new(inputImagePath);
                //image.Quality = 75; // Adjust the quality as needed

                using MemoryStream stream = new();
                //image.Write(stream);
                return stream;
            }
            catch (Exception)
            {
                throw;
            }

        }

    }
}
