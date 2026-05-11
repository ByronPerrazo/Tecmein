namespace Entity
{
    public interface IAuditEntity
    {
        // Se usan nombres genéricos que puedan mapear a los existentes o nuevos
        // En EF Core podemos mapear propiedades de interfaz a columnas específicas
        DateTime? FechaRegistro { get; set; }
        int? SecUsuario { get; set; }
        DateTime? FechaModificacion { get; set; }
        int? SecUsuarioModifica { get; set; }
    }
}
