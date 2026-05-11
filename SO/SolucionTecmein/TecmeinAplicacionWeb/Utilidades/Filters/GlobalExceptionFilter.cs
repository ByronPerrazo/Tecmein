using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using TecmeinWebApp.Utilidades.Response;

namespace TecmeinWebApp.Utilidades.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "Error no manejado en la aplicación");

            var response = new GenericResponse<string>
            {
                Estado = false
            };

            int statusCode = (int)HttpStatusCode.InternalServerError;

            if (context.Exception is ValidationException valEx)
            {
                statusCode = (int)HttpStatusCode.BadRequest;
                response.Mensajes = string.Join(" | ", valEx.Errors.Select(e => e.ErrorMessage));
            }
            else if (context.Exception is InvalidOperationException ||
                     context.Exception is TaskCanceledException ||
                     context.Exception is ArgumentException ||
                     context.Exception is UnauthorizedAccessException)
            {
                statusCode = context.Exception is UnauthorizedAccessException ? (int)HttpStatusCode.Forbidden : (int)HttpStatusCode.BadRequest;
                response.Mensajes = context.Exception.Message;
            }
            else
            {
                response.Mensajes = "Ocurrió un error inesperado en el servidor. Contacte al administrador.";
                // En desarrollo podrías querer ver el Message original, en producción no.
            }

            context.Result = new ObjectResult(response)
            {
                StatusCode = statusCode
            };

            context.ExceptionHandled = true;
        }
    }
}
