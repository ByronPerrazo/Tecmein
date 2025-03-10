using BLL.Interfaces;
using DAL.Interfaces;
using Entity;

namespace BLL.Implementacion
{
    public class DashBoardServices : IDashBoardServices
    {

        private readonly IGenericRepository<Visita> _visitaRepository;
        private readonly IGenericRepository<Equiposvisita> _equiposRepository;
        private DateTime _fechaSistema;
        private DateTime _fechaInicio;
        public DashBoardServices(
                IGenericRepository<Visita> visitaRepository,
                IGenericRepository<Equiposvisita> equiposRepository)
        {
            _visitaRepository = visitaRepository;
            _fechaSistema = DateTime.Now.Date;
            _fechaInicio = _fechaSistema.Date.AddDays(-360);
            _equiposRepository = equiposRepository;
        }

        public Task<int> TotalVisitasUltimaSemana()
        {
            var numeroVisitasUltimaSemana =
                    _visitaRepository
                    .Consultar(x =>
                           x.FechaRegistro.Value.Date >= _fechaInicio &&
                           x.FechaRegistro.Value.Date <= _fechaSistema.Date)
                    .Result
                    .Count();

            return Task.FromResult(numeroVisitasUltimaSemana);
        }
        public Task<int> TotalEquipos()
        {
            var totalEquipos =
                    _equiposRepository
                    .Consultar(x =>
                               x.EstaActivo == 1)
                    .Result
                    .Sum(x => x.Cantidad).GetValueOrDefault();

            return Task.FromResult(totalEquipos);
        }

        public Task<string> TotalIngresosUltimaSemana()
        {
            var contidadCero = "0.00";
            
            return Task.FromResult(contidadCero);
        }

        public Task<int> TotalMarcas()
        {
            var totalMarcas =
                   _equiposRepository
                   .Consultar(x =>
                              x.EstaActivo == 1)
                   .Result
                   .GroupBy(x => x.Marca)
                   .Count();

            return Task.FromResult(totalMarcas);
        }

        public async Task<Dictionary<string, int>> MarcasMasVendidas()
        {
            var visitasUltimaSemana = await
                    _visitaRepository
                    .Consultar(x => x.FechaRegistro.Value.Date >= _fechaInicio);

            var equipos =
                await _equiposRepository.Consultar(x => x.EstaActivo == 1);

            Dictionary<string, int> equiposPorVisita =
                (from vus in visitasUltimaSemana
                 join equi in equipos on vus.Secuencial equals equi.SecVisita
                 group equi by equi.Marca into grupo
                 select new
                 {
                     Marca = grupo.Key,
                     TotalCantidad = grupo.Sum(e => e.Cantidad)
                 }
                ).ToDictionary(keySelector: r => r.Marca, elementSelector: r => r.TotalCantidad.GetValueOrDefault());


            return equiposPorVisita;
        }

        public async Task<Dictionary<string, int>> VisitasUltimaSemana()
        {
            var visitasUltimaSemana = await
                    _visitaRepository
                    .Consultar(x => x.FechaRegistro.Value.Date >= _fechaInicio);

            Dictionary<string, int> consulta =
            visitasUltimaSemana
                .GroupBy(x => x.FechaRegistro.Value.Date)
                .OrderByDescending(g => g.Key)
                .Select(dv =>
                    new
                    {
                        fecha = dv.Key.ToString("dd/MM/yyyy"),
                        total = dv.Count()
                    })
                .ToDictionary(keySelector: r => r.fecha, elementSelector: r => r.total);

            return consulta;
        }
    }
}
