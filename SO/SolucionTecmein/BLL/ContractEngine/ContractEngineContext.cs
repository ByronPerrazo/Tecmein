using BLL.DTOs;
using Entity;
using System.Collections.Generic;

namespace BLL.ContractEngine
{
    public class ContractEngineContext
    {
        public PreContratoGeneratorDTO Data { get; set; } = null!;
        public Cotizacion Cotizacion { get; set; } = null!;
        
        // Atajos para datos frecuentemente usados
        public Visita? Visita => Cotizacion?.SecVisitaNavigation;
        public Empresa? Empresa => Visita?.SecEmpresaNavigation;
    }
}
