using BLL.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BLL.Implementacion
{
    public class LocalStorageService : IStorageServices
    {
        private readonly string _basePath;

        public LocalStorageService()
        {
            // Usamos una ruta absoluta fuera de wwwroot para evitar restricciones de IIS
            _basePath = @"C:\TecmeinFiles";
            
            if (!Directory.Exists(_basePath))
            {
                Directory.CreateDirectory(_basePath);
            }
        }

        public async Task<string> SubirStorage(Stream? RepositorioExterno, string? CarpetaDestino, string? NombreArchivo)
        {
            if (RepositorioExterno == null || string.IsNullOrEmpty(NombreArchivo)) return string.Empty;

            try
            {
                string carpetaRuta = Path.Combine(_basePath, CarpetaDestino ?? "");
                if (!Directory.Exists(carpetaRuta))
                {
                    Directory.CreateDirectory(carpetaRuta);
                }

                string rutaCompleta = Path.Combine(carpetaRuta, NombreArchivo);
                
                using (var fileStream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    await RepositorioExterno.CopyToAsync(fileStream);
                }

                // Retornamos la ruta relativa para ser guardada en la BD
                return Path.Combine("uploads", CarpetaDestino ?? "", NombreArchivo).Replace("\\", "/");
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public async Task<bool> EliminarStorage(string? carpetaDestino, string? nombreArchivo)
        {
            try
            {
                string rutaCompleta = Path.Combine(_basePath, carpetaDestino ?? "", nombreArchivo ?? "");
                if (File.Exists(rutaCompleta))
                {
                    File.Delete(rutaCompleta);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
