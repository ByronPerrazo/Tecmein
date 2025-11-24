using AutoMapper;
using BLL.Interfaces;
using BLL.Models.ViewModels;
using DAL.DBContext;
using DAL.Implementacion;
using DAL.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;

namespace BLL.Implementacion
{
    public class PagoServices : IPagoServices
    {
        private readonly IGenericRepository<Pago> _repositorioPago;
        private readonly IGenericRepository<Cuota> _repositorioCuota; // Cuota es el nombre de la entidad para Cuota
        private readonly IGenericRepository<PlanDePago> _repositorioPlanDePago;
        private readonly IMapper _mapper;
        private readonly TecmeindbContext _dbContext; // Para consultas más complejas

        public PagoServices(
            IGenericRepository<Pago> repositorioPago,
            IGenericRepository<Cuota> repositorioCuota,
            IGenericRepository<PlanDePago> repositorioPlanDePago,
            IMapper mapper,
            TecmeindbContext dbContext
        )
        {
            _repositorioPago = repositorioPago;
            _repositorioCuota = repositorioCuota;
            _repositorioPlanDePago = repositorioPlanDePago;
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<List<PlanPagoDashboardVM>> ObtenerPlanesDePagoParaDashboard()
        {
            var planesQuery = _dbContext.PlanesDePago
                                .Include(p => p.IdContratoNavigation)
                                    .ThenInclude(c => c.SecClienteNavigation)
                                        .ThenInclude(cl => cl.SecConstructoraNavigation)
                                .Include(p => p.Cuotas) // Asume que la colección se llama Cuota en la entidad
                                .Include(p => p.Pagos)
                                .Where(p => p.EstaActivo == true);

            var planes = await planesQuery.ToListAsync();

            var planesVM = new List<PlanPagoDashboardVM>();

            foreach (var plan in planes)
            {
                var montoPagado = plan.Pagos?.Sum(p => p.Monto) ?? 0;
                var saldoPendiente = plan.ValorContrato - montoPagado;
                var estado = "Activo"; // Lógica más compleja para definir el estado (Pendiente, En Mora, Pagado)

                planesVM.Add(new PlanPagoDashboardVM
                {
                    IdPlanDePago = plan.IdPlanDePago,
                    NumeroContrato = plan.IdContratoNavigation?.IdContrato.ToString() ?? "N/A", // Necesita un formato de número de contrato
                    NombreCliente = plan.IdContratoNavigation?.SecClienteNavigation?.SecConstructoraNavigation?.Nombre ?? "N/A",
                    ValorTotalContrato = plan.ValorContrato,
                    MontoPagado = montoPagado,
                    SaldoPendiente = saldoPendiente,
                    EstadoPlan = estado
                });
            }

            return planesVM;
        }


        public async Task<DetallePlanPagoVM> ObtenerDetallePlanDePago(int idPlanDePago)
        {
            var plan = await _dbContext.PlanesDePago
                                .Include(p => p.IdContratoNavigation)
                                    .ThenInclude(c => c.SecClienteNavigation)
                                        .ThenInclude(cl => cl.SecConstructoraNavigation)
                                .Include(p => p.Cuotas)
                                .Include(p => p.Pagos)
                                .FirstOrDefaultAsync(p => p.IdPlanDePago == idPlanDePago && p.EstaActivo == true);

            if (plan == null) return null;

            var montoPagadoTotal = plan.Pagos?.Sum(p => p.Monto) ?? 0;
            var saldoPendienteTotal = plan.ValorContrato - montoPagadoTotal;

            var cuotasVM = plan.Cuotas.Select(c => new CuotaVM
            {
                IdCuota = c.IdCuota,
                NumeroCuota = c.NumeroCuota,
                MontoEsperado = c.MontoEsperado,
                FechaVencimiento = c.FechaVencimiento,
                Estado = c.Estado,
                MontoPagado = c.MontoPagado // Asume que Cuota tiene MontoPagado
            }).ToList();

            var detalleVM = new DetallePlanPagoVM
            {
                IdPlanDePago = plan.IdPlanDePago,
                NumeroContrato = plan.IdContratoNavigation?.IdContrato.ToString() ?? "N/A",
                NombreCliente = plan.IdContratoNavigation?.SecClienteNavigation?.SecConstructoraNavigation?.Nombre ?? "N/A",
                ValorContrato = plan.ValorContrato,
                ValorAnticipo = plan.ValorAnticipo,
                FechaAnticipo = plan.FechaAnticipo,
                NumeroCuotas = plan.NumeroCuotas,
                FechaPrimeraCuota = plan.FechaPrimeraCuota,
                MontoPagadoTotal = montoPagadoTotal,
                SaldoPendienteTotal = saldoPendienteTotal,
                Cuotas = cuotasVM
            };

            return detalleVM;
        }


        public async Task<bool> RegistrarPago(RegistrarPagoVM pagoVM)
        {
            try
            {
                var planDePago = await _repositorioPlanDePago.Obtener(p => p.IdPlanDePago == pagoVM.IdPlanDePago);
                if (planDePago == null) throw new TaskCanceledException("Plan de pago no encontrado.");

                var nuevoPago = _mapper.Map<Pago>(pagoVM);
                nuevoPago.FechaRegistro = DateTime.Now;
                nuevoPago.EstaActivo = true; // Por defecto activo
                // Asegúrate de asignar RegistradoPorUsuarioId desde los Claims del usuario logueado en el controlador

                var pagoCreado = await _repositorioPago.Crear(nuevoPago);
                if (pagoCreado.IdPago == 0) throw new TaskCanceledException("No se pudo registrar el pago.");

                // Lógica para actualizar las cuotas
                decimal montoRestante = pagoCreado.Monto;
                var cuotasPendientes = await _dbContext.Cuotas
                                            .Where(c => c.IdPlanDePago == pagoVM.IdPlanDePago && c.Estado == "Pendiente")
                                            .OrderBy(c => c.FechaVencimiento)
                                            .ToListAsync();

                foreach (var cuota in cuotasPendientes)
                {
                    if (montoRestante <= 0) break;

                    decimal montoCuotaPendiente = cuota.MontoEsperado - (cuota.MontoPagado ?? 0);

                    if (montoRestante >= montoCuotaPendiente)
                    {
                        cuota.MontoPagado = cuota.MontoEsperado;
                        cuota.Estado = "Pagado";
                        montoRestante -= montoCuotaPendiente;
                    }
                    else
                    {
                        cuota.MontoPagado = (cuota.MontoPagado ?? 0) + montoRestante;
                        // Si se paga parcialmente, el estado puede seguir siendo "Pendiente" o "Parcialmente Pagado"
                        // Por simplicidad, lo dejaremos en "Pendiente" hasta que se complete
                        montoRestante = 0;
                    }
                    await _repositorioCuota.Editar(cuota); // Asume que Editar actualiza la entidad
                }

                // Considerar también actualizar el estado del PlanDePago si todas las cuotas están pagadas.

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception (using a proper logger like Serilog or NLog)
                Console.WriteLine($"Error al registrar pago: {ex.Message}");
                throw;
            }
        }
    }
}
