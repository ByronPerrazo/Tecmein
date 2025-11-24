using BLL.Interfaces;
using DAL.Interfaces;
using Entity; // Asumo que la entidad PlanDePago está aquí
using AutoMapper; // Si se necesita mapear ViewModels

namespace BLL.Implementacion
{
    public class PlanDePagoServices : IPlanDePagoServices
    {
        private readonly IGenericRepository<PlanDePago> _repositorioPlanDePago;
        private readonly IMapper _mapper; // Inyectar si se usa AutoMapper

        public PlanDePagoServices(IGenericRepository<PlanDePago> repositorioPlanDePago, IMapper mapper)
        {
            _repositorioPlanDePago = repositorioPlanDePago;
            _mapper = mapper;
        }

        // Implementación de métodos específicos para la gestión de PlanDePago
        // (Crear, Editar, Eliminar PlanDePago, etc.)
        // La lógica para el dashboard de cobranzas está en PagoServices
    }
}
