namespace NalamVazhaWebApi
{
    using System;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options; // for IOptions<ConnectionSettings>
    using Npgsql;
    using NalamVazha.DAL;
    using System.Collections.Generic;

	/// <summary>
	/// Background service that runs every 15 minutes. Calls Check_ProcessSLA_Breach()
	/// to find IPD applications whose current booking-status has been in that state
	/// for at least the configured ProcessSLA threshold (slaindays * 24 + slainhrs).
	/// Sends a single notification per status epoch (the NOT EXISTS guard in the SP
	/// prevents duplicate sends). Logs every send attempt to MailLogs with
	/// mailfor = "SLA Breach / {status}".
	///
	/// To enable: register in Startup.cs ->
	///     services.AddHostedService&lt;IPDSLANotificationService&gt;();
	/// </summary>
	public class IPDSLANotificationService : BackgroundService
    {
        private static readonly TimeSpan CheckInterval = TimeSpan.FromMinutes(15);

        private readonly IServiceProvider _services;
        private readonly ILogger<IPDSLANotificationService> _logger;

        public IPDSLANotificationService(IServiceProvider services, ILogger<IPDSLANotificationService> logger)
        {
            _services = services;
            _logger   = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("IPDSLANotificationService started.");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAndNotifyAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "IPDSLANotificationService: unhandled error during check.");
                }
                await Task.Delay(CheckInterval, stoppingToken);
            }
        }

        private async Task CheckAndNotifyAsync()
        {
            using var scope = _services.CreateScope();
            var connectionSettings = scope.ServiceProvider.GetRequiredService<IOptions<ConnectionSettings>>();
            string connStr         = connectionSettings.Value.ConnectionString;

            // Process each tenant individually so tenantid is always passed as a real value
            var tenantIds = GetAllActiveTenantIds(connStr);
            if (tenantIds.Count == 0) return;

            var notifier = new IPDNotificationHelper(connStr, _logger);

            foreach (var tenantId in tenantIds)
            {
                DataTable breaches = GetSLABreaches(connStr, tenantId);
                if (breaches == null || breaches.Rows.Count == 0) continue;

                _logger.LogInformation("IPDSLANotificationService: {Count} SLA breach(es) found for tenant {TenantId}.", breaches.Rows.Count, tenantId);

                foreach (DataRow row in breaches.Rows)
                {
                    string ipdId  = row["IPDApplicationFormid"]?.ToString() ?? "";
                    string status = row["bookingstatus"]?.ToString() ?? "";
                    string mailto = row["mailto"]?.ToString() ?? "";
                    if (string.IsNullOrWhiteSpace(ipdId) || string.IsNullOrWhiteSpace(status)) continue;

                    DateTime? statusDate = GetStatusDate(row);
                    if (HasSuccessfulSLABreachMailLog(connStr, ipdId, status, statusDate))
                    {
                        _logger.LogInformation(
                            "IPDSLANotificationService: SLA breach notification already sent for IPD {IPDId} / {Status}; skipping.",
                            ipdId,
                            status);
                        continue;
                    }

                    // Send using AlertTemplate (tenant-customised content + tenant MailBox SMTP)
                    // IPDNotificationHelper.SendIPDNotification writes the MailLog entry
                    // (mailfor = "SLA Breach / {status}") which is what Check_ProcessSLA_Breach
                    // uses as the dedup guard — no separate marker needed here.
                    notifier.SendIPDNotification(ipdId, status, tenantId.ToString(), mailto);

                    await Task.Delay(300); // brief pause between sends
                }
            }
        }

        // ── Helpers ────────────────────────────────────────────────────────────

        /// <summary>
        /// Returns all distinct tenantids that have at least one active (non-deleted)
        /// IPD application, ensuring Check_ProcessSLA_Breach is always called with a
        /// real tenant value rather than NULL.
        /// Delegates to the Get_IPD_Active_TenantIds stored procedure.
        /// </summary>
        private List<Guid> GetAllActiveTenantIds(string connStr)
        {
            var ids = new List<Guid>();
            try
            {
                using var conn = new NpgsqlConnection(connStr);
                conn.Open();
                using var cmd = new NpgsqlCommand("SELECT * FROM \"Get_IPD_Active_TenantIds\"()", conn);
                cmd.CommandType = CommandType.Text;
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                        ids.Add(reader.GetGuid(0));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "IPDSLANotificationService: failed to enumerate active tenant IDs.");
            }
            return ids;
        }

        /// <summary>
        /// Calls Check_ProcessSLA_Breach with the supplied tenantid (never NULL).
        /// </summary>
        private DataTable GetSLABreaches(string connStr, Guid tenantId)
        {
            var dt = new DataTable();
            try
            {
                using var conn = new NpgsqlConnection(connStr);
                conn.Open();
                using var cmd = new NpgsqlCommand("SELECT * FROM \"Check_ProcessSLA_Breach\"(@tenantid)", conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@tenantid", NpgsqlTypes.NpgsqlDbType.Uuid, tenantId);
                using var adapter = new NpgsqlDataAdapter(cmd);
                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "IPDSLANotificationService: failed to query SLA breaches for tenant {TenantId}.", tenantId);
            }
            return dt;
        }

        private bool HasSuccessfulSLABreachMailLog(string connStr, string ipdId, string bookingStatus, DateTime? statusDate)
        {
            string actionName = IPDBookingStatusActionMap.ToActionName(bookingStatus);
            try
            {
                using var conn = new NpgsqlConnection(connStr);
                conn.Open();
                using var cmd = new NpgsqlCommand(
                    "SELECT already_sent FROM \"Check_IPD_SLA_Breach_MailLog\"(@ipdId,@actionMailFor,@statusMailFor,@statusDate)",
                    conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ipdId", NpgsqlTypes.NpgsqlDbType.Varchar, ipdId);
                cmd.Parameters.AddWithValue("@actionMailFor", NpgsqlTypes.NpgsqlDbType.Varchar, "SLA Breach / " + actionName);
                cmd.Parameters.AddWithValue("@statusMailFor", NpgsqlTypes.NpgsqlDbType.Varchar, "SLA Breach / " + bookingStatus);
                cmd.Parameters.AddWithValue("@statusDate", NpgsqlTypes.NpgsqlDbType.Timestamp, statusDate.HasValue ? (object)statusDate.Value : DBNull.Value);

                return Convert.ToBoolean(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "IPDSLANotificationService: failed to check existing SLA breach mail log for IPD {IPDId} / {Status}.", ipdId, bookingStatus);
                return false;
            }
        }

        private static DateTime? GetStatusDate(DataRow row)
        {
            string[] candidateColumns =
            {
                "statuschangeddate",
                "status_changed_date",
                "statuschangedat",
                "status_changed_at",
                "bookingstatusdate",
                "booking_status_date",
                "modifieddate",
                "createddate"
            };

            foreach (string column in candidateColumns)
            {
                DataColumn matchedColumn = null;
                foreach (DataColumn dataColumn in row.Table.Columns)
                {
                    if (string.Equals(dataColumn.ColumnName, column, StringComparison.OrdinalIgnoreCase))
                    {
                        matchedColumn = dataColumn;
                        break;
                    }
                }

                if (matchedColumn == null || row[matchedColumn] == DBNull.Value) continue;
                if (row[matchedColumn] is DateTime date) return date;
                if (DateTime.TryParse(row[matchedColumn]?.ToString(), out date)) return date;
            }

            return null;
        }

    }
}
