using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data;
using Microsoft.Extensions.Logging; // Added

namespace DAL.Mcp
{
    public interface IMySqlRepository
    {
        Task<List<Dictionary<string, object>>> ExecuteDynamicQueryAsync(string sqlQuery);
    }

    public class MySqlRepository : IMySqlRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<MySqlRepository> _logger; // Added

        public MySqlRepository(IConfiguration configuration, ILogger<MySqlRepository> logger) // Modified
        {
            _connectionString = configuration.GetConnectionString("ConexionDB")!;
            _logger = logger; // Added
            _logger.LogInformation("MySqlRepository initialized with connection string: {ConnectionString}", _connectionString); // Added
        }

        public async Task<List<Dictionary<string, object>>> ExecuteDynamicQueryAsync(string sqlQuery)
        {
            var results = new List<Dictionary<string, object>>();

            if (string.IsNullOrWhiteSpace(sqlQuery))
            {
                _logger.LogWarning("Attempted to execute an empty or whitespace SQL query."); // Added
                return results;
            }

            _logger.LogDebug("Executing SQL query: {SqlQuery}", sqlQuery); // Added

            using (var connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    _logger.LogDebug("Database connection opened successfully."); // Added

                    using (var command = new MySqlCommand(sqlQuery, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            var columns = new List<string>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                columns.Add(reader.GetName(i));
                            }
                            _logger.LogDebug("Columns retrieved: {Columns}", string.Join(", ", columns)); // Added

                            while (await reader.ReadAsync())
                            {
                                var row = new Dictionary<string, object>();
                                foreach (var col in columns)
                                {
                                    row[col] = reader[col];
                                }
                                results.Add(row);
                            }
                            _logger.LogDebug("Query returned {RowCount} rows.", results.Count); // Added
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error executing dynamic SQL query: {SqlQuery}", sqlQuery); // Added
                    throw; // Re-throw the exception after logging
                }
            }

            return results;
        }
    }
}
