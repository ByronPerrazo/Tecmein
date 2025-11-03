using BLL.DTOs;
using BLL.Interfaces;
using DAL.DBContext;
using DAL.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                // Buscar si ya existe un plan para este contrato
                var planDePagoEntity = await _dbContext.PlanesDePago
                    .Include(p => p.Cuotas)
                    .FirstOrDefaultAsync(p => p.IdContrato == modelo.IdContrato);

                if (planDePagoEntity == null) // No existe, es totalmente nuevo
                {
                    planDePagoEntity = _mapper.Map<PlanDePago>(modelo);
                    planDePagoEntity.FechaRegistro = DateTime.Now;
                    planDePagoEntity.EstaActivo = true;
                    _dbContext.PlanesDePago.Add(planDePagoEntity);
                    await _dbContext.SaveChangesAsync(); // Guardar para obtener el IdPlanDePago
                }
                else // Ya existe, es una actualización
                {
                    // Borrar cuotas antiguas para reemplazarlas
                    if (planDePagoEntity.Cuotas.Any())
                    {
                        _dbContext.Cuotas.RemoveRange(planDePagoEntity.Cuotas);
                    }

                    // Mapear los datos principales del VM al entity existente
                    _mapper.Map(modelo, planDePagoEntity);
                    planDePagoEntity.FechaModificacion = DateTime.Now;
                }

                // Extraer fechas de la lista de cuotas del DTO
                var cuotaAnticipo = modelo.Cuotas.FirstOrDefault(c => c.Tipo == "Anticipo");
                var primeraCuotaRegular = modelo.Cuotas.Where(c => c.Tipo == "Cuota").OrderBy(c => c.NumeroCuota).FirstOrDefault();

                planDePagoEntity.FechaAnticipo = cuotaAnticipo?.FechaVencimiento;
                planDePagoEntity.FechaPrimeraCuota = primeraCuotaRegular?.FechaVencimiento;

                // Crear las nuevas cuotas
                if (modelo.Cuotas != null && modelo.Cuotas.Any())
                {
                    foreach (var cuotaDto in modelo.Cuotas)
                    {
                        var cuotaEntity = _mapper.Map<Cuota>(cuotaDto);
                        cuotaEntity.IdPlanDePago = planDePagoEntity.IdPlanDePago;
                        cuotaEntity.FechaRegistro = DateTime.Now;
                        _dbContext.Cuotas.Add(cuotaEntity);
                    }
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                var dtoResult = _mapper.Map<PlanDePagoDTO>(planDePagoEntity);
                dtoResult.Cuotas = _mapper.Map<List<CuotaDTO>>(planDePagoEntity.Cuotas.OrderBy(c => c.NumeroCuota).ToList());
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