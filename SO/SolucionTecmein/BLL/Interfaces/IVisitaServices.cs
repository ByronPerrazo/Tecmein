using Entity;

namespace BLL.Interfaces
{
    public interface IVisitaServices
    {
        Task<Visita> CreaVisita(Visita entidad);
        Task<Visita> ConsultaVisita(int secuencial);
        Task<List<Visita>> ListaVisitas();
        Task<List<Visita>> ListaVisitasPorUsuario(int idUsuario);
        Task<Visita> EditaVisita(Visita entidad);
        Task<bool> Eliminar(int secuencial);
        Task<Visita> ObtenerDetalleVisita(int secuencial);
        Task<List<Visita>> ListaConEquipos();
        Task<bool> CambiarEtapa(int secVisita, string nuevoCodigoEtapa, bool permitirRetroceso = false);

    }
}
