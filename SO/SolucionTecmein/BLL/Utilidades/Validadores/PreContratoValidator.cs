using BLL.DTOs;
using FluentValidation;

namespace BLL.Utilidades.Validadores
{
    public class PreContratoValidator : AbstractValidator<PreContratoDTO>
    {
        public PreContratoValidator()
        {
            RuleFor(x => x.SecCotizacion)
                .GreaterThan(0).WithMessage("La cotización asociada es obligatoria.");

            RuleFor(x => x.SecPlantillaPreContrato)
                .GreaterThan(0).WithMessage("Debe seleccionar una plantilla de contrato.");

            RuleFor(x => x.Estado)
                .NotEmpty().WithMessage("El estado del pre-contrato es obligatorio.");

            RuleFor(x => x.Dias)
                .NotNull().GreaterThan(0).WithMessage("Los días de entrega deben ser mayores a cero.");

            RuleFor(x => x.TipoDias)
                .NotEmpty().WithMessage("Debe especificar si los días son Calendario o Laborables.");

            RuleFor(x => x.PeriodoMantenimiento)
                .NotEmpty().WithMessage("El periodo de mantenimiento es obligatorio.");

            RuleFor(x => x.PolizaGarantia)
                .NotEmpty().WithMessage("Debe especificar el tipo de póliza de garantía.");

            RuleFor(x => x.PreContratoCompromisoPagos)
                .NotEmpty().WithMessage("Debe definir al menos un compromiso de pago.");
        }
    }
}
