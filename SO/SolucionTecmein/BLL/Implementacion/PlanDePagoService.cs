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
    }
}