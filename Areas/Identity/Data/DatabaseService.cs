using ForestChurches.Components.Configuration;
using Microsoft.Data.SqlClient;
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

        public async Task<float> GetLogDatabaseSize()
        {
            FileInfo fileInformation = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "LOGS.db");

            // Returns the database size in MB
            return (fileInformation.Length / 1024f) / 1024f;
        }

        public async Task<float> GetDatabaseSize()
        {
            var environment = _environment.EnvironmentName;
            float databaseSize = 0f;

            string connectionString = environment == "Production"
                ? _configuration.Client.GetSecret("db-connection-live").Value.Value
                : _configuration.Client.GetSecret("db-connection-dev").Value.Value;

            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "SELECT ROUND(SUM(data_length + index_length) / 1024 / 1024, 2) " +
                                          "AS 'Size (MB)' FROM information_schema.tables " +
                                          "WHERE table_schema = 'forestchurches';";

                    var result = await command.ExecuteScalarAsync();
                    if (result != null && float.TryParse(result.ToString(), out float size))
                    {
                        databaseSize = size;
                    }
                }
            }

            return databaseSize;
        }
    }
}
