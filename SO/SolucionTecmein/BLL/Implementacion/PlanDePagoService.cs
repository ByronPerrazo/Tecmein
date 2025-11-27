using AutoMapper;
using BLL.DTOs;
using BLL.Interfaces;
using DAL.DBContext;
using DAL.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;

namespace BLL.Implementacion
{
    public class PlanDePagoService : IPlanDePagoService
    {
        private readonly IGenericRepository<PlanDePago> _repositorioPlanDePago;
        private readonly IGenericRepository<Cuota> _repositorioCuota;
        private readonly TecmeindbContext _dbContext;
        private readonly IMapper _mapper;

        public PlanDePagoService(
            IGenericRepository<PlanDePago> repositorioPlanDePago,
            IGenericRepository<Cuota> repositorioCuota,
            TecmeindbContext dbContext,
            IMapper mapper)
        {
            _repositorioPlanDePago = repositorioPlanDePago;
            _repositorioCuota = repositorioCuota;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<PlanDePagoDTO> ObtenerPorContratoId(int idContrato)
        {
            var planDePago = await _dbContext.PlanesDePago
                .Include(p => p.Cuotas)
                .FirstOrDefaultAsync(p => p.IdContrato == idContrato);

            if (planDePago == null)
            {
                return new PlanDePagoDTO { IdContrato = idContrato, Cuotas = new List<CuotaDTO>() };
            }

            var dto = _mapper.Map<PlanDePagoDTO>(planDePago);
            dto.Cuotas = _mapper.Map<List<CuotaDTO>>(planDePago.Cuotas.OrderBy(c => c.NumeroCuota));
            return dto;
        }

        public async Task<PlanDePagoDTO> Guardar(PlanDePagoDTO modelo)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var planDePagoEntity = await _dbContext.PlanesDePago
                    .Include(p => p.Cuotas)
                    .FirstOrDefaultAsync(p => p.IdContrato == modelo.IdContrato);

                bool isNewPlan = planDePagoEntity == null;

                if (isNewPlan)
                {
                    planDePagoEntity = new PlanDePago { IdContrato = modelo.IdContrato };
                    _dbContext.PlanesDePago.Add(planDePagoEntity);
                }

                // Mapear datos principales del DTO a la entidad
                planDePagoEntity.SecFormaPago = modelo.SecFormaPago;
                planDePagoEntity.ValorContrato = modelo.ValorContrato;
                planDePagoEntity.ValorAnticipo = (decimal)modelo.ValorAnticipo;
                planDePagoEntity.NumeroCuotas = modelo.Cuotas.Count;
                planDePagoEntity.EstaActivo = true;
                planDePagoEntity.FechaRegistro = isNewPlan ? DateTime.Now : planDePagoEntity.FechaRegistro;
                planDePagoEntity.FechaModificacion = isNewPlan ? null : (DateTime?)DateTime.Now;
                planDePagoEntity.FechaAnticipo = modelo.FechaAnticipo;

                // Extraer fechas clave de la lista de cuotas
                var cuotaAnticipo = modelo.Cuotas.FirstOrDefault(c => c.Tipo == "Anticipo");
                var primeraCuotaRegular = modelo.Cuotas.Where(c => c.Tipo != "Anticipo").OrderBy(c => c.FechaVencimiento).FirstOrDefault();

                planDePagoEntity.FechaPrimeraCuota = primeraCuotaRegular?.FechaVencimiento;

                // Lógica de Upsert/Delete para Cuotas
                var cuotasDtoIds = modelo.Cuotas.Select(c => c.IdCuota).Where(id => id > 0).ToList();
                var cuotasExistentes = planDePagoEntity.Cuotas.ToList();
                var cuotasParaEliminar = cuotasExistentes.Where(c => !cuotasDtoIds.Contains(c.IdCuota)).ToList();

                if (cuotasParaEliminar.Any())
                {
                    _dbContext.Cuotas.RemoveRange(cuotasParaEliminar);
                }

                foreach (var cuotaDto in modelo.Cuotas)
                {
                    if (cuotaDto.IdCuota > 0) // Actualizar cuota existente
                    {
                        var cuotaExistente = cuotasExistentes.FirstOrDefault(c => c.IdCuota == cuotaDto.IdCuota);
                        if (cuotaExistente != null)
                        {
                            cuotaExistente.NumeroCuota = cuotaDto.NumeroCuota;
                            cuotaExistente.MontoEsperado = cuotaDto.MontoEsperado;
                            cuotaExistente.FechaVencimiento = cuotaDto.FechaVencimiento;
                            cuotaExistente.Estado = cuotaDto.Estado;
                        }
                    }
                    else // Insertar nueva cuota
                    {
                        var nuevaCuota = new Cuota
                        {
                            NumeroCuota = cuotaDto.NumeroCuota,
                            MontoEsperado = cuotaDto.MontoEsperado,
                            FechaVencimiento = cuotaDto.FechaVencimiento,
                            Estado = cuotaDto.Estado,
                            FechaRegistro = DateTime.Now
                        };
                        planDePagoEntity.Cuotas.Add(nuevaCuota);
                    }
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                var dtoResult = _mapper.Map<PlanDePagoDTO>(planDePagoEntity);
                return dtoResult;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error al guardar el Plan de Pago: {ex.Message}", ex);
            }
        }
        public async Task<IEnumerable<PlanDePagoDTO>> ListarPlanesDePago()
        {
            var planesDePago = await _dbContext.PlanesDePago
                .Include(p => p.IdContratoNavigation)
                    .ThenInclude(c => c.SecClienteNavigation)
                        .ThenInclude(cli => cli.SecConstructoraNavigation)
                .Include(p => p.Pagos) // Para calcular el monto pagado
                .ToListAsync();

            var dtos = new List<PlanDePagoDTO>();
            foreach (var plan in planesDePago)
            {
                var dto = _mapper.Map<PlanDePagoDTO>(plan);
                dto.NumeroContrato = plan.IdContratoNavigation?.IdContrato.ToString() ?? "N/A";
                dto.NombreCliente = plan.IdContratoNavigation?.SecClienteNavigation?.SecConstructoraNavigation?.Nombre ?? "N/A";
                dto.MontoPagado = plan.Pagos.Sum(p => p.Monto);
                dto.SaldoPendiente = plan.ValorContrato - dto.MontoPagado;
                dto.EstadoPlan = dto.SaldoPendiente <= 0 ? "Pagado" : "Pendiente";
                dtos.Add(dto);
            }
            return dtos;
        }

        public async Task<PlanDePagoDTO> ObtenerDetallePlan(int idPlanDePago)
        {
            var planDePago = await _dbContext.PlanesDePago
                .Include(p => p.IdContratoNavigation)
                .Include(p => p.Cuotas)
                .Include(p => p.Pagos)
                .FirstOrDefaultAsync(p => p.IdPlanDePago == idPlanDePago);

            if (planDePago == null)
            {
                return null;
            }

            var dto = _mapper.Map<PlanDePagoDTO>(planDePago);
            dto.Cuotas = _mapper.Map<List<CuotaDTO>>(planDePago.Cuotas.OrderBy(c => c.NumeroCuota).ToList());
            dto.MontoPagadoTotal = planDePago.Pagos.Sum(p => p.Monto);
            dto.SaldoPendienteTotal = planDePago.ValorContrato - dto.MontoPagadoTotal;
            dto.NumeroContrato = planDePago.IdContratoNavigation?.IdContrato.ToString();

            if (planDePago.IdContratoNavigation?.SecClienteNavigation != null)
            {
                dto.NombreCliente = planDePago.IdContratoNavigation.SecClienteNavigation.SecConstructoraNavigation?.Nombre ?? "N/A";
            }
            else
            {
                dto.NombreCliente = "N/A";
            }

            return dto;
        }
    }
}