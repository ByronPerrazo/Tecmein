namespace Entity
{
    /// <summary>
    /// Interfaz para marcar entidades que requieren aislamiento de datos por usuario.
    /// Las entidades que la implementen serán filtradas automáticamente por el DbContext.
    /// </summary>
    public interface ISecuredEntity
    {
        int? SecUsuarioCrea { get; set; }
    }
}
