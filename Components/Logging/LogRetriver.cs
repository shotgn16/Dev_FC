using ForestChurches.Components.Pageination;
using Serilog;
using System.Data.SQLite;

namespace ForestChurches.Components.Logging
{
    public class LogRetriver
    {
        private System.Timers.Timer _timer;
        private SQLiteConnection _connection;
        public List<Models.LogEntry> Logs { get; set; }
        private bool _isRunning;
        private bool _liveLoggingEnabled = true;

        public LogRetriver()
        {
            Logs = new List<Models.LogEntry>();

            _connection = new SQLiteConnection("Data Source=" + AppDomain.CurrentDomain.BaseDirectory + "\\logs.db;Version=3;");
            _connection.Open();

            _timer = new System.Timers.Timer(5 * 1000); // 5 seconds
            _timer.Elapsed += async (sender, e) => await TimerElapsed();
            _timer.Start();
        }

        private async Task TimerElapsed()
        {
            if (_isRunning) return;
            _isRunning = true;

            _timer.Stop(); 
            await LogsOnCall(1, 100); 
            _timer.Start(); 

            _isRunning = false;
        }

        public async Task<PaginatedList<Models.LogEntry>> LogsOnCall(int pageNumber, int pageSize)
        {
            var logs = new List<Models.LogEntry>();

            using (SQLiteCommand command = _connection.CreateCommand())
            {
                command.CommandText = $"SELECT * FROM 'system_logs' ORDER BY Id DESC LIMIT {pageSize} OFFSET {(pageNumber - 1) * pageSize}";
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        logs.Add(new Models.LogEntry
                        {
                            Timestamp = reader.GetString(reader.GetOrdinal("Timestamp")),
                            Level = reader.GetString(reader.GetOrdinal("Level")),
                            Exception = reader.GetString(reader.GetOrdinal("Exception")),
                            Message = reader.GetString(reader.GetOrdinal("RenderedMessage"))
                        });
                    }
                }
            }

            var totalLogs = await GetTotalLogsCountAsync();
            return new PaginatedList<Models.LogEntry>(logs, totalLogs, pageNumber, pageSize);
        }

        private async Task<int> GetTotalLogsCountAsync()
        {
            using (SQLiteCommand command = _connection.CreateCommand())
            {
                command.CommandText = "SELECT COUNT(*) FROM 'system_logs'";
                return Convert.ToInt32(await command.ExecuteScalarAsync());
            }
        }

    }
}
