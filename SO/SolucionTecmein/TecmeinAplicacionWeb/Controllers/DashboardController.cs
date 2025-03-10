using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using TecmeinWebApp.Models.ViewModel;
using TecmeinWebApp.Utilidades.Response;

namespace TecmeinWebApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashBoardServices _dashBoarServicio;
        public DashboardController(IDashBoardServices dashBoarServicio)
        {
            _dashBoarServicio = dashBoarServicio;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerResumen()
        {

            var gResponse = new GenericResponse<DashBoardVM>();
            try
            {
                var dashBoard = new DashBoardVM();

                dashBoard.totalMarcas = await _dashBoarServicio.TotalMarcas();
                dashBoard.totalVisitasUltimaSemana = await _dashBoarServicio.TotalVisitasUltimaSemana();
                dashBoard.totalIngresosUltimaSemana = await _dashBoarServicio.TotalIngresosUltimaSemana();
                dashBoard.totalEquipos = await _dashBoarServicio.TotalEquipos();

                var listaMarcasMasVendidas = new List<MarcasMasVendidasVM>();
                var listaVisitasUltimaSemana = new List<VisitasUktimaSemanaVM>();

                foreach (KeyValuePair<string, int> item in await _dashBoarServicio.MarcasMasVendidas())
                {
                    listaMarcasMasVendidas
                            .Add(new MarcasMasVendidasVM()
                            {
                                Marca = item.Key,
                                TotalCantidad = item.Value
                            });
                }

                foreach (KeyValuePair<string, int> item in await _dashBoarServicio.VisitasUltimaSemana())
                {
                    listaVisitasUltimaSemana
                            .Add(new VisitasUktimaSemanaVM()
                            {
                                Fecha = item.Key,
                                Total = item.Value
                            });
                }

                dashBoard.listaMarcasMasVendidasVM = listaMarcasMasVendidas;
                dashBoard.listaVisitasUktimaSemanaVM = listaVisitasUltimaSemana;

                gResponse.Estado = true;
                gResponse.Objeto = dashBoard;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }
    }
}
