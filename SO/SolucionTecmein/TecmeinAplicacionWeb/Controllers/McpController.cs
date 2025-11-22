using BLL.Mcp;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
        public async Task<IActionResult> Query([FromBody] McpRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.NaturalLanguageQuery))
            {
                return BadRequest("La consulta en lenguaje natural no puede estar vacía.");
            }

            var result = await _mcpService.ProcessNaturalLanguageQueryAsync(request.NaturalLanguageQuery);

            // Si el resultado contiene una propiedad 'error', podría ser un error del servicio.
            // Esto es una forma simple de manejarlo.
            if (result.GetType().GetProperty("error") != null)
            {
                return StatusCode(500, result);
            }

            return Ok(result);
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
