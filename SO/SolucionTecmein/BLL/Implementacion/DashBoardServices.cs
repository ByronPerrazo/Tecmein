using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using System.Globalization;

namespace BLL.Implementacion
{
    public class DashBoardServices : IDashBoardServices
    {
        private readonly IGenericRepository<Visita> _visitaRepository;
        private readonly IGenericRepository<Equiposvisita> _equiposRepository;

        public DashBoardServices(
            IGenericRepository<Visita> visitaRepository,
            IGenericRepository<Equiposvisita> equiposRepository)
        {
            _visitaRepository = visitaRepository;
            _equiposRepository = equiposRepository;
        }

        public async Task<int> TotalVisitasUltimaSemana()
        {
            DateTime fechaInicio = DateTime.Now.Date.AddDays(-360);
            IQueryable<Visita> query = await _visitaRepository.Consultar(v => v.FechaRegistro.Value.Date >= fechaInicio);
            int total = query.Count();
            return total;
        }

        public async Task<int> TotalEquipos()
        {
            IQueryable<Equiposvisita> query = await _equiposRepository.Consultar(e => e.EstaActivo == 1);
            int total = query.Sum(e => e.Cantidad).GetValueOrDefault();
            return total;
        }

        public Task<string> TotalIngresosUltimaSemana()
        {
            // TODO: Implementar la lógica para calcular los ingresos.
            return Task.FromResult("0.00");
        }

        public async Task<int> TotalMarcas()
        {
            IQueryable<Equiposvisita> query = await _equiposRepository.Consultar(e => e.EstaActivo == 1);
            int total = query.Select(e => e.Marca).Distinct().Count();
            return total;
        }

        public async Task<Dictionary<string, int>> MarcasMasVendidas()
        {
            IQueryable<Equiposvisita> query = await _equiposRepository.Consultar();

            var resultado = query
                .GroupBy(e => e.Marca)
                .OrderByDescending(g => g.Sum(e => e.Cantidad).GetValueOrDefault())
                .Select(m => new { Marca = m.Key, Total = m.Sum(e => e.Cantidad).GetValueOrDefault() })
                .ToDictionary(keySelector: r => r.Marca, elementSelector: r => r.Total);

            return resultado;
        }

        public async Task<Dictionary<string, int>> VisitasUltimaSemana()
        {
            DateTime fechaInicio = DateTime.Now.Date.AddDays(-360);
            IQueryable<Visita> query = await _visitaRepository.Consultar(v => v.FechaRegistro.Value.Date >= fechaInicio);

            // Se materializa la lista en memoria para evitar problemas de traducción de EF con GroupBy y ToString.
            List<Visita> visitasEnMemoria = query.ToList();

            Dictionary<string, int> resultado = visitasEnMemoria
                .GroupBy(v => v.FechaRegistro.Value.Date)
                .OrderByDescending(g => g.Key)
                .Select(dv => new { Fecha = dv.Key.ToString("dd/MM/yyyy"), Total = dv.Count() })
                .ToDictionary(keySelector: r => r.Fecha, elementSelector: r => r.Total);

            return resultado;
        }
    }
}
