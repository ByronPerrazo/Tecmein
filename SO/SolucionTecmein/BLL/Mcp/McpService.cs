using DAL.Mcp;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging; 

namespace BLL.Mcp
{
    public interface IMcpService
    {
        Task<object> ProcessNaturalLanguageQueryAsync(string naturalLanguageQuery);
    }

    public class McpService : IMcpService
    {
        private readonly IMySqlRepository _mySqlRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly GeminiSettings _geminiSettings;
        private readonly ILogger<McpService> _logger;

        public McpService(IMySqlRepository mySqlRepository, IHttpClientFactory httpClientFactory, IOptions<GeminiSettings> geminiSettings, ILogger<McpService> logger)
        {
            _mySqlRepository = mySqlRepository;
            _httpClientFactory = httpClientFactory;
            _geminiSettings = geminiSettings.Value;
            _logger = logger;
        }

        public async Task<object> ProcessNaturalLanguageQueryAsync(string naturalLanguageQuery)
        {
            try
            {
                var dbSchema = await GetDatabaseSchemaAsync();
                var prompt = BuildPrompt(dbSchema, naturalLanguageQuery);
                var generatedSql = await TranslateNaturalLanguageToSqlAsync(prompt);

                if (!IsQuerySafe(generatedSql))
                {
                    _logger.LogWarning("Unsafe SQL query generated: {GeneratedSql}", generatedSql);
                    throw new InvalidOperationException("La consulta generada no es segura.");
                }

                var result = await _mySqlRepository.ExecuteDynamicQueryAsync(generatedSql);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing natural language query: {Query}", naturalLanguageQuery);
                return new { error = ex.Message };
            }
        }

        private async Task<string> GetDatabaseSchemaAsync()
        {
            try
            {
                var schemaQuery = @"
                    SELECT table_name, column_name, data_type 
                    FROM information_schema.columns 
                    WHERE table_schema = DATABASE() 
                    ORDER BY table_name, ordinal_position;";
                
                var schemaData = await _mySqlRepository.ExecuteDynamicQueryAsync(schemaQuery);

                if (!schemaData.Any())
                {
                    _logger.LogWarning("GetDatabaseSchemaAsync: No schema data returned by the query.");
                    return string.Empty; 
                }

                var schemaBuilder = new StringBuilder();
                string? currentTable = null;

                _logger.LogDebug("GetDatabaseSchemaAsync: Keys in first row of schemaData: {Keys}", string.Join(", ", schemaData.First().Keys));

                foreach (var row in schemaData)
                {
                    if (!row.TryGetValue("TABLE_NAME", out object? tableNameObj) || tableNameObj == null)
                    {
                        _logger.LogError("GetDatabaseSchemaAsync: 'TABLE_NAME' key not found or is null in a row. Available keys: {Keys}", string.Join(", ", row.Keys));
                        continue; 
                    }
                    var tableName = tableNameObj.ToString();

                    if (tableName != currentTable)
                    {
                        if (currentTable != null)
                        {
                            schemaBuilder.AppendLine(")");
                        }
                        schemaBuilder.AppendLine($"Tabla: {tableName} (");
                        currentTable = tableName;
                    }

                    if (!row.TryGetValue("COLUMN_NAME", out object? columnNameObj) || columnNameObj == null)
                    {
                        _logger.LogWarning("GetDatabaseSchemaAsync: 'COLUMN_NAME' key not found or is null for table {TableName}. Skipping.", tableName);
                        continue;
                    }
                    if (!row.TryGetValue("DATA_TYPE", out object? dataTypeObj) || dataTypeObj == null)
                    {
                        _logger.LogWarning("GetDatabaseSchemaAsync: 'DATA_TYPE' key not found or is null for table {TableName}, column {ColumnName}. Skipping.", tableName, columnNameObj.ToString());
                        continue;
                    }

                    schemaBuilder.AppendLine($"  {columnNameObj} {dataTypeObj}");
                }
                if (currentTable != null)
                {
                    schemaBuilder.AppendLine(")");
                }

                return schemaBuilder.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting database schema.");
                throw; 
            }
        }

        private string BuildPrompt(string schema, string question)
        {
            return $@"
                Eres un asistente experto en MySQL. Tu tarea es generar una única consulta SQL basada en el esquema de la base de datos y la pregunta del usuario.

                **Reglas estrictas:**
                1. SOLO debes devolver el código SQL.
                2. No incluyas explicaciones, comentarios, ni la palabra 'SQL'.
                3. La consulta debe ser de solo lectura (SELECT, WITH). No se permiten operaciones de modificación (INSERT, UPDATE, DELETE, DROP, ALTER, TRUNCATE, EXEC, CREATE, GRANT, REVOKE).
                4. Analiza el esquema para entender las relaciones entre las tablas.
                5. **RESPETA LA CAPITALIZACIÓN EXACTA de las tablas y columnas tal como se proporcionan en el esquema.**
                6. Al realizar comparaciones con columnas numéricas que puedan contener valores NULL, como 'MontoPagado', usa la función COALESCE(columna, 0) para tratar los NULL como 0. Por ejemplo: COALESCE(T1.MontoPagado, 0) < T1.MontoEsperado.

                **Esquema de la Base de Datos:**
                ---
                {schema}
                ---

                **Pregunta del Usuario:**
                ---
                {question}
                ---

                **Consulta SQL Generada:**
            ";
        }

        private async Task<string> TranslateNaturalLanguageToSqlAsync(string prompt)
        {
            var client = _httpClientFactory.CreateClient();
            var requestUrl = $"{_geminiSettings.BaseUrl}v1beta/models/{_geminiSettings.Model}:generateContent?key={_geminiSettings.ApiKey}";
            _logger.LogDebug("Gemini API Request URL: {RequestUrl}", requestUrl);

            var requestBody = new GeminiRequestBody
            {
                Contents = new List<Content>
                {
                    new Content
                    {
                        Parts = new List<Part>
                        {
                            new Part { Text = prompt }
                        }
                    }
                }
            };
            _logger.LogDebug("Gemini API Request Body: {RequestBody}", JsonSerializer.Serialize(requestBody));

            var response = await client.PostAsJsonAsync(requestUrl, requestBody);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Gemini API Error Response: Status Code {StatusCode}, Content: {ErrorContent}", response.StatusCode, errorContent);
                response.EnsureSuccessStatusCode(); // This will throw the HttpRequestException
            }

            var geminiResponse = await response.Content.ReadFromJsonAsync<GeminiResponse>();

            if (geminiResponse?.Candidates?.Count > 0 && geminiResponse.Candidates[0].Content?.Parts?.Count > 0)
            {
                var geminiText = geminiResponse.Candidates[0].Content.Parts[0].Text;
                // Extraer la consulta SQL del bloque de código Markdown si existe, de forma más flexible.
                var sqlMatch = System.Text.RegularExpressions.Regex.Match(geminiText, @"```(?:sql)?\s*(.*?)\s*```", System.Text.RegularExpressions.RegexOptions.Singleline);
                if (sqlMatch.Success && sqlMatch.Groups.Count > 1)
                {
                    return sqlMatch.Groups[1].Value.Trim();
                }
                // Si no se encuentra el bloque de código Markdown, asumimos que el texto es directamente la consulta SQL.
                _logger.LogWarning("Could not extract SQL from Markdown block. Returning raw Gemini text. Text: {GeminiText}", geminiText);
                return geminiText.Trim();
            }

            throw new Exception("No se pudo obtener una respuesta SQL del modelo Gemini.");
        }
        
        private bool IsQuerySafe(string sqlQuery)
        {
            var lowerQuery = sqlQuery.Trim().ToLower();
            // Allow SELECT or WITH for CTEs, but ensure no DML/DDL operations
            if (!lowerQuery.StartsWith("select") && !lowerQuery.StartsWith("with"))
            {
                return false; // Must start with SELECT or WITH
            }

            string[] forbiddenKeywords = { "\binsert\b", "\bupdate\b", "\bdelete\b", "\bdrop\b", "\balter\b", "\btruncate\b", "\bexec\b", "\bcreate\b", "\bgrant\b", "\brevoke\b" }; // Using word boundaries
            foreach (var keyword in forbiddenKeywords)
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(lowerQuery, keyword))
                {
                    _logger.LogWarning("Unsafe keyword detected: {Keyword} in query: {Query}", keyword, sqlQuery); // Added log
                    return false;
                }
            }

            return true;
        }
    }
}
