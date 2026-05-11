namespace BLL.Interfaces
{
    public interface IGeneradorDocumentoService
    {
        Task<byte[]> GenerarDocumento(string codigoTipoDocumento, int secPlantilla, object datos);
    }
}