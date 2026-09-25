namespace NalamVazhaWebApi
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Npgsql;

    /// <summary>
    /// Background service that runs once a day at midnight.
    /// Updates age in patientprofile and people tables for records
    /// whose birthday (month + day) matches today's date.
    /// </summary>
    public class AgeUpdateService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<AgeUpdateService> _logger;

        public AgeUpdateService(IServiceProvider services, ILogger<AgeUpdateService> logger)
        {
            _services = services;
            _logger   = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("AgeUpdateService started.");
            while (!stoppingToken.IsCancellationRequested)
            {
                // Wait until next midnight before running
                var now         = DateTime.Now;
                var nextMidnight = now.Date.AddDays(1);
                var delay        = nextMidnight - now;
                await Task.Delay(delay, stoppingToken);

                try
                {
                    await UpdateAgesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "AgeUpdateService: unhandled error during age update.");
                }
            }
        }

        private async Task UpdateAgesAsync()
        {
            using var scope = _services.CreateScope();
            var connectionSettings = scope.ServiceProvider.GetRequiredService<IOptions<ConnectionSettings>>();
            string connStr         = connectionSettings.Value.ConnectionString;

            try
            {
                await using var conn = new NpgsqlConnection(connStr);
                await conn.OpenAsync();

                await using var cmd = new NpgsqlCommand("SELECT \"Update_Age_On_Birthday\"()", conn);
                await cmd.ExecuteNonQueryAsync();

                _logger.LogInformation("AgeUpdateService: age update completed on {Date}.",
                    DateTime.Today.ToString("yyyy-MM-dd"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AgeUpdateService: failed to execute Update_Age_On_Birthday.");
            }
        }
    }
}
