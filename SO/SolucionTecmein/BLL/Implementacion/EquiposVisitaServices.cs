using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Implementacion
{
    public class EquiposVisitaServices : IEquiposVisitaServices
    {
        private readonly IGenericRepository<Equiposvisita> _repositorioEquiposVisita;
        private readonly IVisitaServices _visitaServices;
        private readonly IValidacionServices _validacionServices;

        public EquiposVisitaServices(IGenericRepository<Equiposvisita> repositorioEquiposVisita,
                                     IVisitaServices visitaServices,
                                     IValidacionServices validacionServices)
        {
            _repositorioEquiposVisita = repositorioEquiposVisita;
            _visitaServices = visitaServices;
            _validacionServices = validacionServices;
        }
        
        public async Task<List<Equiposvisita>> ConsultaListaPorVisita(int secuencialVisita)
        {
            var query = await _repositorioEquiposVisita
                              .Consultar(x =>
                                         x.SecVisita == secuencialVisita &&
                                         x.EstaActivo == 1);
            return query.ToList();
        }

        public async Task<Equiposvisita> Obtener(int secuencial)
        {
            var query = await _repositorioEquiposVisita.Obtener(x => x.Secuencial == secuencial);
            return query;
        }

        public async Task<Equiposvisita> ProcesaGuardar(Equiposvisita equiposvisita)
        {
            await ValidarEquipoVisita(equiposvisita);
            return await _repositorioEquiposVisita.Crear(equiposvisita);
        }

        public async Task<bool> ProcesaEliminar(Equiposvisita equiposvisita)
        {
            var equipoExistente = await _repositorioEquiposVisita.Obtener(x => x.Secuencial == equiposvisita.Secuencial);
            if (equipoExistente == null)
            {
                return false; // O lanzar una excepción si se prefiere
            }
            return await _repositorioEquiposVisita.Eliminar(equipoExistente);
        }

        private async Task ValidarEquipoVisita(Equiposvisita equipo)
        {
            // Validar SecVisita
            if (equipo.SecVisita <= 0)
            {
                throw new TaskCanceledException("La visita asociada al equipo es obligatoria.");
            }
            var visitaExistente = await _visitaServices.ConsultaVisita(equipo.SecVisita);
            if (visitaExistente == null)
            {
                throw new TaskCanceledException($"La visita con secuencial {equipo.SecVisita} no existe.");
            }

            // Validar Marca
            _validacionServices.ValidarNombre(equipo.Marca, "marca del equipo");

            // Validar Cantidad
            if (equipo.Cantidad == null || equipo.Cantidad <= 0)
            {
                throw new TaskCanceledException("La cantidad del equipo debe ser mayor que cero.");
            }
        }
    }
}
