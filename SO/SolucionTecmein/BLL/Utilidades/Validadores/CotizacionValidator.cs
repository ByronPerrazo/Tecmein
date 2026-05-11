using BLL.DTOs;
using FluentValidation;

namespace BLL.Utilidades.Validadores
{
    public class CotizacionValidator : AbstractValidator<CotizacionDTO>
    {
        public CotizacionValidator()
        {
            RuleFor(x => x.SecVisita)
                .GreaterThan(0).WithMessage("La visita asociada es obligatoria.");

            RuleFor(x => x.Subtotal)
                .GreaterThanOrEqualTo(0).WithMessage("El subtotal no puede ser negativo.");

            RuleFor(x => x.TotalConImpuestos)
                .GreaterThan(0).WithMessage("El total de la cotización debe ser mayor a cero.");

            RuleFor(x => x.Cotizaciondetalles)
                .NotEmpty().WithMessage("La cotización debe tener al menos un detalle.");

            RuleForEach(x => x.Cotizaciondetalles).SetValidator(new CotizacionDetalleValidator());
        }
    }

    public class CotizacionDetalleValidator : AbstractValidator<CotizaciondetalleDTO>
    {
        public CotizacionDetalleValidator()
        {
            RuleFor(x => x.Cantidad)
                .GreaterThan(0).WithMessage("La cantidad debe ser mayor a cero.");

            RuleFor(x => x.Total)
                .GreaterThan(0).WithMessage("El total del detalle debe ser mayor a cero.");

            RuleFor(x => x.NombreEquipo)
                .NotEmpty().When(x => x.SecEquipoVisita == null)
                .WithMessage("Debe especificar un nombre de equipo si no está asociado a una visita.");
        }
    }
}
