using BLL.Implementacion;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;


namespace TecmeinWebApp.Utilidades.ViewComponents
{

    public class ValidatePermissionAttribute : ActionFilterAttribute
    {
        private readonly string _accion;

        public ValidatePermissionAttribute(string accion)
        {
            _accion = accion;
        }

        public override async void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.User;
            var secuencialRol = user.FindFirst(ClaimTypes.Role)?.Value; // Obtener el rol del usuario

            if (string.IsNullOrEmpty(secuencialRol))
            {
                context.Result = new ForbidResult(); // Devuelve 403 Forbidden
                return;
            }

            var autorizacionService =
                     (AutorizacionService)context
                    .HttpContext
                    .RequestServices
                    .GetService(typeof(AutorizacionService));

            if (await autorizacionService.TienePermiso(Convert.ToInt32(secuencialRol), _accion))
            {
                base.OnActionExecuting(context);
            }
            else
            {
                context.Result = new ForbidResult(); // Devuelve 403 Forbidden
                return;
            }
        }
    }

}
