using ForestChurches.Components.Configuration;
using MySqlConnector;
using System.Configuration;
using static ServiceStack.Diagnostics.Events;
using Configuration = ForestChurches.Components.Configuration.Configuration;

namespace ForestChurches.Areas.Identity.Data
{
    public class DatabaseService : IDatabaseService
    {
        private readonly Configuration _configuration;
        private readonly IHostEnvironment _environment;
        public DatabaseService(Configuration configuration, IHostEnvironment enviroment) 
        {
            _configuration = configuration;
            _environment = enviroment;
        }

        public async Task<string> GetDatabaseSize(string database)
        {
            string connectionStringName = _environment.IsDevelopment() ? "db-connection-dev" : "db-connection-live";
            string connectionString = _configuration.Client.GetSecret(connectionStringName).Value.Value;

            // Will be cleaned up later
            // TESTING
            string query = @"
                SELECT ROUND(SUM(data_length + index_length) / 1024 / 1024, 2) AS 'Size (MB)'
                FROM information_schema.tables
                WHERE table_schema = '" + database + "' GROUP BY table_schema;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    var result = await command.ExecuteScalarAsync();
                    return result != null ? result.ToString() : "0";
                }
            }
        }
    }
}
