namespace BLL.Interfaces
{
    public interface IStorageServices
    {

        Task<string> SubirStorage(Stream? RepositorioExterno, string? CarpetaDestino, string? NombreArchivo);
        Task<bool> EliminarStorage(string? carpetaDestino, string? nombreArchivo);
    }
}
