namespace NalamVazhaWebApi
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Npgsql;
    using NpgsqlTypes;

    /// <summary>Creates and closes pending IPD workflow tasks once per day.</summary>
    public sealed class OverdueIpdReviewTaskService : BackgroundService
    {
        private readonly IOptions<ConnectionSettings> _connectionSettings;
        private readonly ILogger<OverdueIpdReviewTaskService> _logger;

        public OverdueIpdReviewTaskService(
            IOptions<ConnectionSettings> connectionSettings,
            ILogger<OverdueIpdReviewTaskService> logger)
        {
            _connectionSettings = connectionSettings;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("IPD workflow daily-task service started.");
            OverdueIpdReviewTaskOptions options = OverdueIpdReviewTaskOptions.FromEnvironment();

            if (!options.Enabled)
            {
                _logger.LogInformation("OverdueIpdReviewTaskService is disabled.");
                return;
            }

            if (options.RunOnStartup)
            {
                try
                {
                    _logger.LogInformation("Running IPD workflow daily-task startup test.");
                    await CreateTasksAsync(options, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "IPD workflow daily-task startup test failed.");
                }
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                DateTime now = DateTime.Now;
                DateTime nextRun = now.Date.AddHours(options.RunHour).AddMinutes(options.RunMinute);
                if (nextRun <= now) nextRun = nextRun.AddDays(1);

                try
                {
                    await Task.Delay(nextRun - now, stoppingToken);
                    await CreateTasksAsync(options, stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "IPD workflow daily-task service failed.");
                }
            }
        }

        internal async Task CreateTasksAsync(OverdueIpdReviewTaskOptions options, CancellationToken cancellationToken)
        {
            Guid? systemUserId = Guid.TryParse(options.SystemUserId, out Guid parsedSystemUserId)
                && parsedSystemUserId != Guid.Empty
                    ? parsedSystemUserId
                    : null;

            string templateNames = options.GetTemplateNamesJson();
            bool mainSucceeded = await ExecuteWorkflowAsync(
                "Process_IPD_Workflow_Daily_Tasks", "IPD workflow", templateNames, systemUserId, cancellationToken);
            bool postSucceeded = await ExecuteWorkflowAsync(
                "Process_Post_IPD_OPD_Workflow_Daily_Tasks", "Post-IPD/OPD workflow", templateNames, systemUserId, cancellationToken);

            if (!mainSucceeded && !postSucceeded)
                throw new InvalidOperationException("Both DailyTask workflow procedures failed. See the preceding errors.");
        }

        private async Task<bool> ExecuteWorkflowAsync(
            string functionName,
            string workflowName,
            string templateNames,
            Guid? systemUserId,
            CancellationToken cancellationToken)
        {
            try
            {
                await using var connection = new NpgsqlConnection(_connectionSettings.Value.ConnectionString);
                await connection.OpenAsync(cancellationToken);
                await using var command = new NpgsqlCommand(
                    $"SELECT * FROM \"{functionName}\"(@businessDate,@templateNames,@systemUserId)", connection);
                command.Parameters.AddWithValue("businessDate", NpgsqlDbType.Date, DateTime.Today);
                command.Parameters.AddWithValue("templateNames", NpgsqlDbType.Jsonb, templateNames);
                command.Parameters.AddWithValue("systemUserId", NpgsqlDbType.Uuid, (object)systemUserId ?? DBNull.Value);

                await using var reader = await command.ExecuteReaderAsync(cancellationToken);
                if (!await reader.ReadAsync(cancellationToken))
                    throw new InvalidOperationException(workflowName + " returned no result.");

                _logger.LogInformation(
                    "{Workflow} DailyTask run completed. Records checked: {Checked}; tasks created: {Created}; " +
                    "tasks skipped: {Skipped}; tasks closed: {Closed}; errors: {Errors}.",
                    workflowName, reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3), reader.GetInt32(4));
                return true;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Workflow} DailyTask procedure failed: {Error}", workflowName, ex.Message);
                return false;
            }
        }
    }
}
