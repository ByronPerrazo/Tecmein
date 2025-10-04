using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IAuditService
    {
        Task RegistrarEventoAsync(string tipoEvento, int? idUsuario, string? nombreUsuario, string detalle, string? direccionIp);
    }
}
