
using AutoMapper;
using BLL.DTOs;
using BLL.Interfaces;
using DAL.DBContext;
using DAL.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Implementacion
{
    public class PagoService : IPagoService
    {
        private readonly IGenericRepository<Pago> _repositorioPago;
        private readonly IGenericRepository<Cuota> _repositorioCuota;
        private readonly TecmeindbContext _dbContext;
        private readonly IMapper _mapper;

        public PagoService(
            IGenericRepository<Pago> repositorioPago,
            IGenericRepository<Cuota> repositorioCuota,
            TecmeindbContext dbContext, 
            IMapper mapper)
        {
            _repositorioPago = repositorioPago;
            _repositorioCuota = repositorioCuota;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<PagoDTO> RegistrarPago(PagoDTO modelo, int idUsuario)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // 1. Crear y guardar la entidad Pago
                var pagoEntity = _mapper.Map<Pago>(modelo);
                pagoEntity.FechaRegistro = DateTime.Now;
                pagoEntity.EstaActivo = true;
                pagoEntity.RegistradoPorUsuarioId = idUsuario;

                var pagoCreado = await _repositorioPago.Crear(pagoEntity);
                if (pagoCreado == null) throw new Exception("No se pudo registrar el pago.");

                // 2. Obtener cuotas pendientes
                var cuotasPendientes = await _dbContext.Cuotas
                    .Where(c => c.IdPlanDePago == modelo.IdPlanDePago && c.Estado != "Pagada")
                    .OrderBy(c => c.NumeroCuota)
                    .ToListAsync();

                decimal montoRestanteDelPago = pagoCreado.Monto;

                // 3. Distribuir el pago entre las cuotas
                foreach (var cuota in cuotasPendientes)
                {
                    if (montoRestanteDelPago <= 0) break;

                    decimal saldoDeCuota = cuota.MontoEsperado - (cuota.MontoPagado ?? 0);
                    decimal montoAAplicar = Math.Min(montoRestanteDelPago, saldoDeCuota);

                    cuota.MontoPagado = (cuota.MontoPagado ?? 0) + montoAAplicar;
                    montoRestanteDelPago -= montoAAplicar;

                    // 4. Actualizar estado de la cuota
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

                await transaction.CommitAsync();
                return _mapper.Map<PagoDTO>(pagoCreado);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error al registrar el pago: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<PagoDTO>> ListarPorPlanDePago(int idPlanDePago)
        {
            try
            {
                var pagos = await _repositorioPago.Consultar(p => p.IdPlanDePago == idPlanDePago);
                return _mapper.Map<IEnumerable<PagoDTO>>(pagos.ToList());
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al listar pagos por plan: {ex.Message}", ex);
            }
        }
    }
}
