using BLL.DTOs;
using FluentValidation;

namespace BLL.Utilidades.Validadores
{
    public class PreContratoConPagosValidator : AbstractValidator<PreContratoConPagosDTO>
    {
        public PreContratoConPagosValidator()
        {
            RuleFor(x => x.SecCotizacion)
                .GreaterThan(0).WithMessage("La cotización asociada es obligatoria.");

            RuleFor(x => x.Dias)
                .NotNull().GreaterThanOrEqualTo(0).WithMessage("Los días de entrega no pueden ser negativos.");

            RuleFor(x => x.TipoDias)
                .NotEmpty().WithMessage("Debe especificar el tipo de días.");

            RuleFor(x => x.CompromisosDePago)
                .NotEmpty().WithMessage("Debe definir al menos un compromiso de pago.");

            RuleForEach(x => x.CompromisosDePago).SetValidator(new CompromisoPagoValidator());
        }
    }

    public class CompromisoPagoValidator : AbstractValidator<CompromisoPagoDTO>
    {
        public CompromisoPagoValidator()
        {
            RuleFor(x => x.Tipo)
                .NotEmpty().WithMessage("El tipo de pago es obligatorio.");

            RuleFor(x => x.Monto)
                .GreaterThan(0).WithMessage("El monto del compromiso debe ser mayor a cero.");

            RuleFor(x => x.FechaVencimiento)
                .NotEmpty().WithMessage("La fecha de vencimiento es obligatoria.");
        }
    }
}
