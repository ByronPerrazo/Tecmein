
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;


namespace TecmeinWebApp.Utilidades.ViewComponents
{

    public class ValidateUserAttribute : ActionFilterAttribute
    {
        private readonly string _permission;

        public ValidateUserAttribute(string permission = null)
        {
            _permission = permission;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.User;

            // Verificar si el usuario está autenticado
            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult(); // Devuelve 401 Unauthorized
                return;
            }

            // Verificar si el usuario tiene el permiso requerido
            if (_permission != null && !user.HasClaim("Permission", _permission))
            {
                context.Result = new ForbidResult(); // Devuelve 403 Forbidden
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
