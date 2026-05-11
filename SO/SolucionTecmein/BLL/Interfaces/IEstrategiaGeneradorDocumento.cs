namespace BLL.Interfaces
{
    public interface IEstrategiaGeneradorDocumento
    {
        string CodigoTipoDocumento { get; }
        Task<byte[]> Generar(int secPlantilla, object datos);
    }
}
