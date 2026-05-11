using Microsoft.AspNetCore.Mvc;

namespace TecmeinWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ConfigController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("tinymce-key")]
        public IActionResult GetTinyMceApiKey()
        {
            var apiKey = _configuration["ApiKeys:TinyMCE"];
            if (string.IsNullOrEmpty(apiKey))
            {
                return NotFound(new { message = "API Key for TinyMCE not found in server configuration." });
            }
            return Ok(new { apiKey });
        }
    }
}
