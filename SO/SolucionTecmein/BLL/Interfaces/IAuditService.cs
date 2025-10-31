using Entity;

namespace BLL.Interfaces
{
    public interface IAuditService
    {
        Task RegistrarEventoAsync(string tipoEvento, int? idUsuario, string? nombreUsuario, string detalle, string? direccionIp);
        Task<List<AuditoriaEvento>> GetEventsByPrefixAsync(string prefix);
    }
}
