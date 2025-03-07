using BLL.Interfaces;
using DAL.Interfaces;
using Entity;

namespace BLL.Implementacion
{
    public class EquiposVisitaServices : IEquiposVisitaServices
    {

        private readonly IGenericRepository<Visita> _repositorioVisita;
        private readonly IGenericRepository<Equiposvisita> _repositorioEquiposVisita;


        public EquiposVisitaServices(IGenericRepository<Visita> repositorioVisita,
                            IGenericRepository<Equiposvisita> repositorioEquiposVisita)
        {
            _repositorioVisita = repositorioVisita;
            _repositorioEquiposVisita = repositorioEquiposVisita;

        }
        
        public async Task<List<Equiposvisita>> ConsultaListaPorVisita(int secuencialVisita)
        {
            var query = await _repositorioEquiposVisita
                              .Consultar(x =>
                                         x.SecVisita == secuencialVisita &&
                                         x.EstaActivo == 1);
            return [.. query];
        }

        public async Task<Equiposvisita> Obtener(int secuencial)
        {
            var query = await _repositorioEquiposVisita.Obtener(x => x.Secuencial == secuencial);
            return query;
        }

        public async Task<Equiposvisita> ProcesaGuardar(Equiposvisita equiposvisita) 
            => await _repositorioEquiposVisita.Crear(equiposvisita);

        public async Task<bool> ProcesaEliminar(Equiposvisita equiposvisita) 
            => await _repositorioEquiposVisita.Eliminar(equiposvisita);
        
    }
}
