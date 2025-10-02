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
                PlanDePago planDePagoEntity;

                if (modelo.IdPlanDePago == 0) // Nuevo Plan de Pago
                {
                    planDePagoEntity = _mapper.Map<PlanDePago>(modelo);
                    planDePagoEntity.FechaRegistro = DateTime.Now;
                    planDePagoEntity.EstaActivo = true;
                    planDePagoEntity = await _repositorioPlanDePago.Crear(planDePagoEntity);
                }
                else // Actualizar Plan de Pago existente
                {
                    planDePagoEntity = await _dbContext.PlanesDePago
                        .Include(p => p.Cuotas)
                        .FirstOrDefaultAsync(p => p.IdPlanDePago == modelo.IdPlanDePago);

                    if (planDePagoEntity == null) throw new Exception("Plan de Pago no encontrado.");

                    _mapper.Map(modelo, planDePagoEntity); // Actualizar propiedades
                    await _repositorioPlanDePago.Editar(planDePagoEntity);

                    // Eliminar cuotas antiguas
                    _dbContext.Cuotas.RemoveRange(planDePagoEntity.Cuotas);
                    await _dbContext.SaveChangesAsync(); // Guardar cambios para eliminar cuotas
                }

                // Crear nuevas cuotas
                foreach (var cuotaDto in modelo.Cuotas)
                {
                    var cuotaEntity = _mapper.Map<Cuota>(cuotaDto);
                    cuotaEntity.IdPlanDePago = planDePagoEntity.IdPlanDePago;
                    cuotaEntity.FechaRegistro = DateTime.Now;
                    await _repositorioCuota.Crear(cuotaEntity);
                }

                await transaction.CommitAsync();
                return _mapper.Map<PlanDePagoDTO>(planDePagoEntity);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error al guardar el Plan de Pago: {ex.Message}", ex);
            }
        }
    }
}