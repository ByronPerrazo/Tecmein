using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data;

namespace DAL.Mcp
{
    public interface IMySqlRepository
    {
        Task<List<Dictionary<string, object>>> ExecuteDynamicQueryAsync(string sqlQuery);
    }

    public class MySqlRepository : IMySqlRepository
    {
        private readonly string _connectionString;

        public MySqlRepository(IConfiguration configuration)
        {
            // Usamos la cadena de conexión que ya está definida para el desarrollo local.
            _connectionString = configuration.GetConnectionString("ConexionDB")!;
        }

        public async Task<List<Dictionary<string, object>>> ExecuteDynamicQueryAsync(string sqlQuery)
        {
            var results = new List<Dictionary<string, object>>();
            
            if (string.IsNullOrWhiteSpace(sqlQuery))
            {
                return results;
            }

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new MySqlCommand(sqlQuery, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var columns = new List<string>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            columns.Add(reader.GetName(i));
                        }

                        while (await reader.ReadAsync())
                        {
                            var row = new Dictionary<string, object>();
                            foreach (var col in columns)
                            {
                                row[col] = reader[col];
                            }
                            results.Add(row);
                        }
                    }
                }
            }

            return results;
        }
    }
}
