namespace NalamVazhaWebApi
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    /// <summary>Runs the pre-admission notification service once each day.</summary>
    public sealed class PreAdmissionNotificationJob : BackgroundService
    {
        private readonly PreAdmissionNotificationService _service;
        private readonly ILogger<PreAdmissionNotificationJob> _logger;

        public PreAdmissionNotificationJob(
            PreAdmissionNotificationService service,
            ILogger<PreAdmissionNotificationJob> logger)
        {
            _service = service;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Pre-admission notification job started.");

            // Process once on startup. MailLogs makes this safe across restarts.
            Run(DateTime.Today);

            while (!stoppingToken.IsCancellationRequested)
            {
                DateTime now = DateTime.Now;
                DateTime nextRun = now.Date.AddDays(1).AddHours(6);
                try
                {
                    await Task.Delay(nextRun - now, stoppingToken);
                    Run(DateTime.Today);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Pre-admission notification job failed.");
                }
            }
        }

        private void Run(DateTime businessDate)
        {
            try { _service.SendDueNotifications(businessDate); }
            catch (Exception ex) { _logger.LogError(ex, "Pre-admission notification run failed."); }
        }
    }
}
