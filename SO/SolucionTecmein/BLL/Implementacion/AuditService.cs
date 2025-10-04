using BLL.Interfaces;
using DAL.DBContext;
using Entity;
using System;
using System.Threading.Tasks;

namespace BLL.Implementacion
{
    public class AuditService : IAuditService
    {
        private readonly TecmeindbContext _context;

        public AuditService(TecmeindbContext context)
        {
            _context = context;
        }

        public async Task RegistrarEventoAsync(string tipoEvento, int? idUsuario, string? nombreUsuario, string detalle, string? direccionIp)
        {
            var evento = new AuditoriaEvento
            {
                FechaHora = DateTime.UtcNow,
                IdUsuario = idUsuario,
                NombreUsuario = nombreUsuario,
                TipoEvento = tipoEvento,
                Detalle = detalle,
                DireccionIp = direccionIp
            };

            _context.AuditoriaEventos.Add(evento);
            await _context.SaveChangesAsync();
        }
    }
}
