using DAL.Mcp;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BLL.Mcp
{
    public interface IMcpService
    {
        Task<object> ProcessNaturalLanguageQueryAsync(string naturalLanguageQuery);
    }

    public class McpService : IMcpService
    {
        private readonly IMySqlRepository _mySqlRepository;
        // Inyectaremos un HttpClientFactory para hacer llamadas a la API del LLM.
        private readonly IHttpClientFactory _httpClientFactory;

        public McpService(IMySqlRepository mySqlRepository, IHttpClientFactory httpClientFactory)
        {
            _mySqlRepository = mySqlRepository;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<object> ProcessNaturalLanguageQueryAsync(string naturalLanguageQuery)
        {
            try
            {
                // 1. Obtener el esquema de la base de datos para dar contexto al LLM.
                var dbSchema = await GetDatabaseSchemaAsync();

                // 2. Construir el prompt para el LLM.
                var prompt = BuildPrompt(dbSchema, naturalLanguageQuery);

                // 3. Llamar al LLM para traducir el lenguaje natural a SQL.
                // Esta es una simulación. Aquí iría la llamada real a la API de Gemini u otro LLM.
                var generatedSql = await TranslateNaturalLanguageToSqlAsync(prompt);

                // 4. Medida de seguridad básica: validar la consulta generada.
                if (!IsQuerySafe(generatedSql))
                {
                    throw new InvalidOperationException("La consulta generada no es segura.");
                }

                // 5. Ejecutar la consulta SQL en la base de datos.
                var result = await _mySqlRepository.ExecuteDynamicQueryAsync(generatedSql);

                return result;
            }
            catch (Exception ex)
            {
                // Manejo de errores apropiado. Por ahora, devolvemos el mensaje.
                return new { error = ex.Message };
            }
        }

        private async Task<string> GetDatabaseSchemaAsync()
        {
            // Consulta para obtener nombres de tablas y columnas de la base de datos actual.
            var schemaQuery = @"
                SELECT table_name, column_name, data_type 
                FROM information_schema.columns 
                WHERE table_schema = DATABASE() 
                ORDER BY table_name, ordinal_position;";
            
            var schemaData = await _mySqlRepository.ExecuteDynamicQueryAsync(schemaQuery);

            var schemaBuilder = new StringBuilder();
            string? currentTable = null;

            foreach (var row in schemaData)
            {
                var tableName = row["table_name"].ToString();
                if (tableName != currentTable)
                {
                    if (currentTable != null)
                    {
                        schemaBuilder.AppendLine(")");
                    }
                    schemaBuilder.AppendLine($"Tabla: {tableName} (");
                    currentTable = tableName;
                }
                schemaBuilder.AppendLine($"  {row["column_name"]} {row["data_type"]}");
            }
            if (currentTable != null)
            {
                schemaBuilder.AppendLine(")");
            }

            return schemaBuilder.ToString();
        }

        private string BuildPrompt(string schema, string question)
        {
            return $@"
                Eres un asistente experto en MySQL. Tu tarea es generar una única consulta SQL basada en el esquema de la base de datos y la pregunta del usuario.

                **Reglas estrictas:**
                1. SOLO debes devolver el código SQL.
                2. No incluyas explicaciones, comentarios, ni la palabra 'SQL'.
                3. La consulta debe ser de solo lectura (SELECT). No se permiten operaciones de modificación (INSERT, UPDATE, DELETE, DROP, etc.).
                4. Analiza el esquema para entender las relaciones entre las tablas.

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
            // --- SIMULACIÓN ---
            // En una implementación real, aquí se haría la llamada HTTP al servicio del LLM.
            // Por ejemplo, a la API de Gemini.
            // Por ahora, devolvemos una consulta de prueba.
            
            // Ejemplo de llamada real (conceptual):
            // var client = _httpClientFactory.CreateClient("GeminiApiClient");
            // var requestBody = new { contents = new[] { new { parts = new[] { new { text = prompt } } } } };
            // var response = await client.PostAsJsonAsync("v1beta/models/gemini-pro:generateContent", requestBody);
            // response.EnsureSuccessStatusCode();
            // var geminiResponse = await response.Content.ReadFromJsonAsync<GeminiResponse>();
            // var sql = geminiResponse.Candidates[0].Content.Parts[0].Text;
            // return sql;

            // Para la prueba, vamos a simular que el usuario preguntó "cuantos clientes hay"
            if (prompt.Contains("cuantos clientes hay"))
            {
                return "SELECT COUNT(*) AS TotalClientes FROM cliente;";
            }
            return "SELECT 'Consulta no implementada en simulación' AS Message;";
        }
        
        private bool IsQuerySafe(string sqlQuery)
        {
            // Medida de seguridad muy básica. En un sistema real, esto debería ser mucho más robusto.
            // Por ejemplo, usar un parser de SQL para analizar la consulta.
            var lowerQuery = sqlQuery.Trim().ToLower();
            if (!lowerQuery.StartsWith("select"))
            {
                return false;
            }

            string[] forbiddenKeywords = { "insert", "update", "delete", "drop", "alter", "truncate", "exec" };
            foreach (var keyword in forbiddenKeywords)
            {
                if (lowerQuery.Contains(keyword))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
