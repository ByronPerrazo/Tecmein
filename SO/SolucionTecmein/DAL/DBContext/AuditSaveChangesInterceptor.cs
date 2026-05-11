using Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DAL.DBContext
{
    public class AuditSaveChangesInterceptor : SaveChangesInterceptor
    {
        private readonly IUserSession _userSession;

        public AuditSaveChangesInterceptor(IUserSession userSession)
        {
            _userSession = userSession;
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            UpdateAuditFields(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            UpdateAuditFields(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void UpdateAuditFields(DbContext? context)
        {
            if (context == null) return;

            var entries = context.ChangeTracker.Entries<IAuditEntity>();
            var now = DateTime.Now; // Usamos la hora local del servidor o UTC según prefiera el sistema
            var userId = _userSession?.SecUsuario;

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.FechaRegistro ??= now;
                    entry.Entity.SecUsuario ??= userId;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.FechaModificacion = now;
                    entry.Entity.SecUsuarioModifica = userId;

                    // Aseguramos que no se sobrescriban los campos de creación
                    entry.Property(x => x.FechaRegistro).IsModified = false;
                    entry.Property(x => x.SecUsuario).IsModified = false;
                }
            }
        }
    }
}
