using BLL.Implementacion;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using TecmeinWebApp.Utilidades.Response;


namespace TecmeinWebApp.Utilidades.ViewComponents
{

    public class ValidatePermissionAttribute : Attribute, IAsyncActionFilter
    {
        private readonly string _accion;

        public ValidatePermissionAttribute(string accion)
        {
            _accion = accion;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var user = context.HttpContext.User;
            var secuencialRolValue = user.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(secuencialRolValue) || !int.TryParse(secuencialRolValue, out int secuencialRol))
            {
                context.Result = new JsonResult(new GenericResponse<string>
                {
                    Estado = false,
                    Mensajes = "Acceso denegado. No se pudo obtener la información del rol del usuario."
                })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
                return;
            }

            var controllerName = context.RouteData.Values["controller"]?.ToString();
            if (string.IsNullOrEmpty(controllerName))
            {
                context.Result = new JsonResult(new GenericResponse<string>
                {
                    Estado = false,
                    Mensajes = "Acceso denegado. No se pudo determinar el recurso solicitado."
                })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
                return;
            }

            var autorizacionService = (AutorizacionService)context.HttpContext.RequestServices.GetService(typeof(AutorizacionService));

            // Pasar el rol, la acción Y el nombre del controlador al servicio de autorización
            if (await autorizacionService.TienePermiso(secuencialRol, _accion, controllerName))
            {
                await next(); // Ejecuta la acción del controlador si tiene permiso
            }
            else
            {
                context.Result = new JsonResult(new GenericResponse<string>
                {
                    Estado = false,
                    Mensajes = "Acceso denegado. No tiene permisos para realizar esta acción."
                })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
                return;
            }
        }
    }


}
