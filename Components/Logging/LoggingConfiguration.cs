using Microsoft.AspNetCore.SignalR;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.AspNetCore.App.SignalR.Extensions;
using System.Globalization;
using System.Security.Policy;

namespace ForestChurches.Components.Logging
{
    public static class LoggingConfiguration
    {
        internal static void ConfigureLogging(IServiceCollection services)
        {
            services.AddSerilog(
                (services, loggingConfiguration) =>
                    loggingConfiguration
                        .MinimumLevel.Information()
                        .WriteTo.SignalR<LoggingHub>(
                            services,
                            (context, message, logEvent) =>
                                context.Clients.All.SendAsync("RecieveEvent", message, logEvent)
                            )
                        .WriteTo.Console(
                            restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Verbose,
                            formatProvider: CultureInfo.InvariantCulture
                        )
                        .WriteTo.SQLite(
                            AppDomain.CurrentDomain.BaseDirectory + "LOGS.db",
                            "system_logs",
                            restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug,
                            formatProvider: CultureInfo.InvariantCulture
                        )
                );
        }
    }
    public class LoggerSinkConfiguration
    {
        public LoggerConfiguration SQLite(string connectionString, string tableName = "Logs", LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose, IFormatProvider? formatProvider = null, bool storeTimestampInUtc = false, TimeSpan? batchPostingLimit = null, TimeSpan? period = null, LoggingLevelSwitch? levelSwitch = null, uint maxRowCount = 0, uint maxDatabaseSize = 0, bool autoCreateSqlTable = true)
        {
            formatProvider ??= CultureInfo.InvariantCulture;
            // Existing implementation
            return new LoggerConfiguration(); // Ensure a LoggerConfiguration object is returned
        }
    }

}