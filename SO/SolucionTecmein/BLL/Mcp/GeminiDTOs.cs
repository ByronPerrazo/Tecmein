using System.Text.Json.Serialization;

namespace BLL.Mcp
{
    // Request DTOs
    public class GeminiRequestBody
    {
        [JsonPropertyName("contents")]
        public List<Content> Contents { get; set; } = new List<Content>();
    }

    public class Content
    {
        [JsonPropertyName("parts")]
        public List<Part> Parts { get; set; } = new List<Part>();
    }

    public class Part
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
    }

    // Response DTOs
    public class GeminiResponse
    {
        [JsonPropertyName("candidates")]
        public List<Candidate> Candidates { get; set; } = new List<Candidate>();
        [JsonPropertyName("promptFeedback")]
        public PromptFeedback? PromptFeedback { get; set; }
    }

    public class Candidate
    {
        [JsonPropertyName("content")]
        public Content? Content { get; set; }
        [JsonPropertyName("finishReason")]
        public string? FinishReason { get; set; }
        [JsonPropertyName("safetyRatings")]
        public List<SafetyRating>? SafetyRatings { get; set; }
    }

    public class PromptFeedback
    {
        [JsonPropertyName("safetyRatings")]
        public List<SafetyRating>? SafetyRatings { get; set; }
    }

    public class SafetyRating
    {
        [JsonPropertyName("category")]
        public string? Category { get; set; }
        [JsonPropertyName("probability")]
        public string? Probability { get; set; }
    }

    public class McpResponseDTO
    {
        public string Respuesta { get; set; } = string.Empty;
    }
}