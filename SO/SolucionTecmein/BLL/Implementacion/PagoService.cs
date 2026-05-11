using AutoMapper;
using BLL.DTOs;
using BLL.Interfaces;
using DAL.DBContext;
using DAL.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;

namespace BLL.Implementacion
{
    public class PagoService : IPagoService
    {
        private readonly IGenericRepository<Pago> _repositorioPago;
        private readonly IGenericRepository<Cuota> _repositorioCuota;
        private readonly TecmeindbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public PagoService(
            IGenericRepository<Pago> repositorioPago,
            IGenericRepository<Cuota> repositorioCuota,
            TecmeindbContext dbContext,
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _repositorioPago = repositorioPago;
            _repositorioCuota = repositorioCuota;
            _dbContext = dbContext;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<PlanPagoDashboardDTO>> ObtenerPlanesDePagoParaDashboard()
        {
            var planesQuery = _dbContext.PlanesDePago
                                .Include(p => p.IdContratoNavigation)
                                    .ThenInclude(c => c.IdCotizacionNavigation)
                                        .ThenInclude(cot => cot.SecVisitaNavigation) // Include Visita for project name
                                .Include(p => p.IdContratoNavigation)
                                    .ThenInclude(c => c.SecClienteNavigation)
                                        .ThenInclude(cl => cl.SecConstructoraNavigation)
                                .Include(p => p.Pagos)
                                .Where(p => p.EstaActivo == true);

            var planes = await planesQuery.ToListAsync();
            var planesDTO = new List<PlanPagoDashboardDTO>();

            foreach (var plan in planes)
            {
                var montoPagado = plan.Pagos?.Sum(p => p.Monto) ?? 0;
                var saldoPendiente = plan.ValorContrato - montoPagado;
                var estado = "Activo"; // Lógica más compleja para definir el estado (Pendiente, En Mora, Pagado)

                planesDTO.Add(new PlanPagoDashboardDTO
                {
                    IdPlanDePago = plan.IdPlanDePago,
                    NumeroContrato = plan.IdContratoNavigation?.IdContrato.ToString() ?? "N/A",
                    NombreCliente = plan.IdContratoNavigation?.SecClienteNavigation?.SecConstructoraNavigation?.Nombre ?? "N/A",
                    NombreProyecto = plan.IdContratoNavigation?.IdCotizacionNavigation?.SecVisitaNavigation?.Nombre ?? "N/A", // Populate project name
                    ValorTotalContrato = plan.ValorContrato,
                    MontoPagado = montoPagado,
                    SaldoPendiente = saldoPendiente,
                    EstadoPlan = estado
                });
            }
            return planesDTO;
        }

        public async Task<DetallePlanPagoDTO> ObtenerDetallePlanDePago(int idPlanDePago)
        {
            var plan = await _dbContext.PlanesDePago
                                .Include(p => p.IdContratoNavigation)
                                    .ThenInclude(c => c.IdCotizacionNavigation)
                                        .ThenInclude(cot => cot.SecVisitaNavigation) // Include Visita for project name
                                .Include(p => p.IdContratoNavigation)
                                    .ThenInclude(c => c.SecClienteNavigation)
                                        .ThenInclude(cl => cl.SecConstructoraNavigation)
                                .Include(p => p.Cuotas)
                                .Include(p => p.Pagos)
                                .FirstOrDefaultAsync(p => p.IdPlanDePago == idPlanDePago && p.EstaActivo == true);

            if (plan == null) return null;

            var montoPagadoTotal = plan.Pagos?.Sum(p => p.Monto) ?? 0;
            var saldoPendienteTotal = plan.ValorContrato - montoPagadoTotal;

            // Calculate SaldoVencidoTotal
            var saldoVencidoTotal = plan.Cuotas
                                        .Where(c => c.FechaVencimiento < DateTime.Now && c.Estado != "Pagada")
                                        .Sum(c => c.MontoEsperado - (c.MontoPagado ?? 0)); // Sum of outstanding balance for overdue

            var detalleDTO = new DetallePlanPagoDTO
            {
                IdPlanDePago = plan.IdPlanDePago,
                NumeroContrato = plan.IdContratoNavigation?.IdContrato.ToString() ?? "N/A",
                NombreCliente = plan.IdContratoNavigation?.SecClienteNavigation?.SecConstructoraNavigation?.Nombre ?? "N/A",
                NombreProyecto = plan.IdContratoNavigation?.IdCotizacionNavigation?.SecVisitaNavigation?.Nombre ?? "N/A", // Populate project name
                ValorContrato = plan.ValorContrato,
                ValorAnticipo = plan.ValorAnticipo,
                FechaAnticipo = plan.FechaAnticipo,
                NumeroCuotas = plan.NumeroCuotas,
                FechaPrimeraCuota = plan.FechaPrimeraCuota,
                MontoPagadoTotal = montoPagadoTotal,
                SaldoVencidoTotal = saldoVencidoTotal,
                SaldoPendienteTotal = saldoPendienteTotal,
                Cuotas = _mapper.Map<List<CuotaDTO>>(plan.Cuotas)
            };

            return detalleDTO;
        }

        public async Task<PagoDTO> RegistrarPago(PagoDTO pagoDTO)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var planDePago = await _dbContext.PlanesDePago
                                            .FirstOrDefaultAsync(p => p.IdPlanDePago == pagoDTO.IdPlanDePago);

                if (planDePago == null) throw new TaskCanceledException("Plan de pago no encontrado.");

                var nuevoPago = _mapper.Map<Pago>(pagoDTO);
                nuevoPago.FechaRegistro = DateTime.Now;
                nuevoPago.EstaActivo = true;

                var pagoCreado = await _repositorioPago.Crear(nuevoPago);
                if (pagoCreado.IdPago == 0) throw new TaskCanceledException("No se pudo registrar el pago.");

                decimal montoRestante = pagoCreado.Monto;
                var cuotasPendientes = await _dbContext.Cuotas
                                            .Where(c => c.IdPlanDePago == pagoDTO.IdPlanDePago && c.Estado != "Pagada")
                                            .OrderBy(c => c.FechaVencimiento)
                                            .ToListAsync();

                foreach (var cuota in cuotasPendientes)
                {
                    if (montoRestante <= 0) break;

                    decimal saldoDeCuota = cuota.MontoEsperado - (cuota.MontoPagado ?? 0);
                    decimal montoAAplicar = Math.Min(montoRestante, saldoDeCuota);

                    cuota.MontoPagado = (cuota.MontoPagado ?? 0) + montoAAplicar;
                    montoRestante -= montoAAplicar;

                    if (cuota.MontoPagado >= cuota.MontoEsperado)
                    {
                        cuota.Estado = "Pagada";
                    }
                    else
                    {
                        cuota.Estado = "Parcialmente Pagada";
                    }
                    await _repositorioCuota.Editar(cuota);
                }
                await _unitOfWork.CommitTransactionAsync();
                return _mapper.Map<PagoDTO>(pagoCreado);
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<IEnumerable<PagoDTO>> ListarPorPlanDePago(int idPlanDePago)
        {
            var pagos = await _repositorioPago.Consultar(p => p.IdPlanDePago == idPlanDePago);
            return _mapper.Map<IEnumerable<PagoDTO>>(pagos.ToList());
        }

        public async Task<List<HistorialPagoCuotaDTO>> ObtenerHistorialPagosPorCuota(int idCuota)
        {
            var cuota = await _dbContext.Cuotas.FirstOrDefaultAsync(c => c.IdCuota == idCuota);

            if (cuota == null) throw new KeyNotFoundException("Cuota no encontrada.");

            var pagos = await _dbContext.Pagos
                                .Include(p => p.RegistradoPorUsuario)
                                .Where(p => p.IdPlanDePago == cuota.IdPlanDePago && p.EstaActivo == true)
                                .OrderByDescending(p => p.FechaRegistro)
                                .ToListAsync();

            var historialDTOs = pagos.Select(p => new HistorialPagoCuotaDTO
            {
                IdPago = p.IdPago,
                IdCuota = cuota.IdCuota, // Asignar el idCuota de la cuota actual para contexto
                Monto = p.Monto,
                FechaPago = p.FechaPago,
                ComprobanteUrl = p.ComprobanteUrl,
                ComprobanteNombre = p.ComprobanteNombre,
                RegistradoPorUsuarioId = p.RegistradoPorUsuarioId, // Corregido: ya no es nullable
                RegistradoPorUsuarioNombre = p.RegistradoPorUsuario?.Nombre ?? "Desconocido",
                EstaActivo = p.EstaActivo,
                FechaRegistro = p.FechaRegistro
            }).ToList();

            return historialDTOs;
        }
    }
}
