using BLL.Mcp;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using TecmeinWebApp.Utilidades.Response;
using TecmeinWebApp.Utilidades.ViewComponents;

namespace TecmeinWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class McpController : ControllerBase
    {
        private readonly IMcpService _mcpService;

        public McpController(IMcpService mcpService)
        {
            _mcpService = mcpService;
        }

        [HttpPost("query")]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> Query([FromBody] McpRequest request)
        {
            var gResponse = new GenericResponse<McpResponseDTO>();
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.NaturalLanguageQuery))
                {
                    gResponse.Estado = false;
                    gResponse.Mensajes = "La consulta en lenguaje natural no puede estar vacía.";
                    return BadRequest(gResponse);
                }

                var serviceResult = await _mcpService.ProcessNaturalLanguageQueryAsync(request.NaturalLanguageQuery);

                // Check if the serviceResult is an error object
                if (serviceResult.GetType().GetProperty("error") != null) // Check if anonymous type with error property
                {
                    var errorProperty = serviceResult.GetType().GetProperty("error");
                    var errorMessage = errorProperty?.GetValue(serviceResult)?.ToString();

                    gResponse.Estado = false;
                    gResponse.Mensajes = errorMessage ?? "Error desconocido del servicio MCP.";
                    return StatusCode(500, gResponse);
                }

                // Assuming serviceResult is IEnumerable<IDictionary<string, object>> for successful queries
                if (serviceResult is IEnumerable<IDictionary<string, object>> queryResults)
                {
                    gResponse.Objeto = new McpResponseDTO { Respuesta = FormatQueryResults(queryResults) };
                    gResponse.Estado = true;
                }
                else
                {
                    // Handle other unexpected result types from service
                    gResponse.Estado = false;
                    gResponse.Mensajes = "Formato de respuesta inesperado del servicio MCP.";
                    return StatusCode(500, gResponse);
                }
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
                return StatusCode(500, gResponse);
            }
            return StatusCode(200, gResponse);
        }

        private string FormatQueryResults(IEnumerable<IDictionary<string, object>> results)
        {
            if (results == null || !results.Any())
            {
                return "No se encontraron resultados para su consulta.";
            }

            StringBuilder htmlTable = new StringBuilder();
            htmlTable.Append("<table class=\"table table-bordered table-striped\">");

            // Headers
            htmlTable.Append("<thead><tr>");
            var firstRow = results.First();
            foreach (var key in firstRow.Keys)
            {
                htmlTable.Append($"<th>{key}</th>");
            }
            htmlTable.Append("</tr></thead>");

            // Body
            htmlTable.Append("<tbody>");
            foreach (var row in results)
            {
                htmlTable.Append("<tr>");
                foreach (var value in row.Values)
                {
                    htmlTable.Append($"<td>{value?.ToString() ?? ""}</td>");
                }
                htmlTable.Append("</tr>");
            }
            htmlTable.Append("</tbody>");
            htmlTable.Append("</table>");

            return htmlTable.ToString();
        }
    }

    /// <summary>
    /// Representa el cuerpo de la petición para el endpoint del MCP.
    /// </summary>
    public class McpRequest
    {
        public string? NaturalLanguageQuery { get; set; }
    }
}
