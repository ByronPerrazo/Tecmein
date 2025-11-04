using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace BLL.Implementacion
{
    public class DashBoardServices : IDashBoardServices
    {
        private readonly IGenericRepository<Visita> _visitaRepository;
        private readonly IGenericRepository<Equiposvisita> _equiposRepository;
        private readonly IGenericRepository<Contrato> _contratoRepository;

        public DashBoardServices(
            IGenericRepository<Visita> visitaRepository,
            IGenericRepository<Equiposvisita> equiposRepository,
            IGenericRepository<Contrato> contratoRepository)
        {
            _visitaRepository = visitaRepository;
            _equiposRepository = equiposRepository;
            _contratoRepository = contratoRepository;
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

        public async Task<Dictionary<string, int>> VisitasPorEtapa()
        {
            IQueryable<Visita> query = await _visitaRepository.Consultar();
            var resultado = query
                .Include(v => v.IdEtapaNavigation)
                .GroupBy(v => v.IdEtapaNavigation.Descripcion)
                .Select(g => new { Etapa = g.Key, Total = g.Count() })
                .ToDictionary(keySelector: r => r.Etapa, elementSelector: r => r.Total);
            return resultado;
        }

        public async Task<Dictionary<string, int>> ContratosPorMes()
        {
            DateTime fechaInicio = DateTime.Now.Date.AddMonths(-12);
            IQueryable<Contrato> query = await _contratoRepository.Consultar(c => c.FechaFirma.Date >= fechaInicio);
            var resultado = query
                .GroupBy(c => new { Year = c.FechaFirma.Year, Month = c.FechaFirma.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new { Mes = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy"), Total = g.Count() })
                .ToDictionary(keySelector: r => r.Mes, elementSelector: r => r.Total);
            return resultado;
        }

        public async Task<Dictionary<string, int>> TopClientesConMasContratos()
        {
            IQueryable<Contrato> query = await _contratoRepository.Consultar();
            var resultado = query
                .Include(c => c.SecClienteNavigation.SecConstructoraNavigation)
                .GroupBy(c => c.SecClienteNavigation.SecConstructoraNavigation.Nombre)
                .Select(g => new { Cliente = g.Key, Total = g.Count() })
                .OrderByDescending(r => r.Total)
                .Take(5)
                .ToDictionary(keySelector: r => r.Cliente, elementSelector: r => r.Total);
            return resultado;
        }
    }
}
