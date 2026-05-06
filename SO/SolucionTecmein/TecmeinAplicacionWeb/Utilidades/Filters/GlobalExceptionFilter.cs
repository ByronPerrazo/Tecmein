using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TecmeinWebApp.Utilidades.Response;
using System.Net;

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
                Estado = false,
                Mensajes = context.Exception.Message
            };

            // Si es una excepción de negocio (ej. TaskCanceledException o InvalidOperationException), 
            // devolvemos 400 Bad Request, de lo contrario 500 Internal Server Error.
            int statusCode = context.Exception is InvalidOperationException || 
                             context.Exception is TaskCanceledException ||
                             context.Exception is ArgumentException
                             ? (int)HttpStatusCode.BadRequest 
                             : (int)HttpStatusCode.InternalServerError;

            context.Result = new ObjectResult(response)
            {
                StatusCode = statusCode
            };

            context.ExceptionHandled = true;
        }
    }
}
