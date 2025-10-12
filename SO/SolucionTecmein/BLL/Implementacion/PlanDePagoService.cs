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

                    // Antes de mapear, nos aseguramos de que las cuotas existentes se eliminen
                    if (planDePagoEntity.Cuotas.Any())
                    {
                        _dbContext.Cuotas.RemoveRange(planDePagoEntity.Cuotas);
                        await _dbContext.SaveChangesAsync();
                    }

                    _mapper.Map(modelo, planDePagoEntity);
                    await _repositorioPlanDePago.Editar(planDePagoEntity);
                }

                // *** Lógica de negocio centralizada para generar cuotas ***
                if (modelo.NumeroCuotas > 0)
                {
                    decimal saldoPendiente = planDePagoEntity.ValorContrato - (planDePagoEntity.ValorAnticipo);
                    decimal montoPorCuota = Math.Round(saldoPendiente / planDePagoEntity.NumeroCuotas, 2);

                    for (int i = 0; i < modelo.Cuotas.Count; i++)
                    {
                        var cuotaDto = modelo.Cuotas[i];
                        var cuotaEntity = new Cuota
                        {
                            IdPlanDePago = planDePagoEntity.IdPlanDePago,
                            NumeroCuota = cuotaDto.NumeroCuota,
                            FechaVencimiento = cuotaDto.FechaVencimiento,
                            MontoEsperado = montoPorCuota, // Usar el monto calculado en el backend
                            MontoPagado = 0, // Siempre 0 al crear/regenerar
                            Estado = "Pendiente", // Estado inicial
                            FechaRegistro = DateTime.Now
                        };
                        await _repositorioCuota.Crear(cuotaEntity);
                    }
                }

                await transaction.CommitAsync();
                var dtoResult = _mapper.Map<PlanDePagoDTO>(planDePagoEntity);
                dtoResult.Cuotas = _mapper.Map<List<CuotaDTO>>((await _repositorioCuota.Consultar(c => c.IdPlanDePago == dtoResult.IdPlanDePago)).OrderBy(c => c.NumeroCuota));
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