namespace NalamVazhaWebApi
{
    using System;
    using System.Data;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using NalamVazha.DAL;
    using NalamVazha.Models;

    /// <summary>Builds and sends due pre-admission reminders without using an HTTP endpoint.</summary>
    public sealed class PreAdmissionNotificationService
    {
        internal const string EntityName = "IPDApplicationForm";
        internal const string EntityAction = "PreAdmissionNotification";
        internal const string MailFor = "Pre-Admission Notification";

        private readonly string _connectionString;
        private readonly IEmailService _emailService;
        private readonly ILogger<PreAdmissionNotificationService> _logger;

        public PreAdmissionNotificationService(
            IOptions<ConnectionSettings> connectionSettings,
            IEmailService emailService,
            ILogger<PreAdmissionNotificationService> logger)
        {
            _connectionString = connectionSettings.Value.ConnectionString;
            _emailService = emailService;
            _logger = logger;
        }

        public int SendDueNotifications(DateTime businessDate)
        {
            DataTable candidates = new PreAdmissionNotificationDAL(_connectionString)
                .GetCandidates(businessDate.Date);
            int sentCount = 0;

            foreach (DataRow row in candidates.Rows)
            {
                string applicationId = row["ipdapplicationformid"].ToString();
                string tenantId = row["tenantid"].ToString();
                Guid createdUser = row["createduser"] is Guid id ? id : Guid.Empty;

                try
                {
                    var mailLogs = new MailLogsDAL(_connectionString, "");
                    mailmodel mail = mailLogs.Mailer(EntityName, EntityAction, applicationId, tenantId);
                    if (mail == null || string.IsNullOrWhiteSpace(mail.mailto) ||
                        string.IsNullOrWhiteSpace(mail.mailsubject))
                    {
                        _logger.LogWarning(
                            "Pre-admission notification skipped for IPD {ApplicationId}: template or recipient is missing.",
                            applicationId);
                        continue;
                    }

                    MailBoxModel mailBox = GetMailBox(tenantId);
                    bool sent = mailBox != null && _emailService.Send(
                        mailBox, mail.mailto, "", mail.mailsubject, mail.mailbody, true, null);

                    mailLogs.Create_MailLog(new MailLogsModel
                    {
                        entityname = EntityName,
                        entityid = applicationId,
                        mailfor = MailFor,
                        mailto = mail.mailto,
                        mailsubject = mail.mailsubject,
                        mailbody = mail.mailbody,
                        issent = sent,
                        createduser = createdUser,
                        craftmyapp_actionmethodname = "Create_MailLog"
                    });

                    if (sent) sentCount++;
                    else _logger.LogWarning("Pre-admission email failed for IPD {ApplicationId}.", applicationId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Pre-admission notification failed for IPD {ApplicationId}.", applicationId);
                }
            }

            _logger.LogInformation(
                "Pre-admission notification run completed for {BusinessDate}. Eligible: {Eligible}; sent: {Sent}.",
                businessDate.Date, candidates.Rows.Count, sentCount);
            return sentCount;
        }

        private MailBoxModel GetMailBox(string tenantId)
        {
            DataTable rows = new MailBoxDAL(_connectionString).Mail_Box_List(tenantId);
            if (rows == null || rows.Rows.Count == 0) return null;

            DataRow selected = rows.Rows[0];
            foreach (DataRow row in rows.Rows)
            {
                string applicable = row["applicableservice"]?.ToString() ?? "";
                if (applicable.IndexOf("IPD", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    selected = row;
                    break;
                }
            }

            return new MailBoxModel
            {
                senderdisplayname = selected["senderdisplayname"]?.ToString() ?? "",
                senderemail = selected["senderemail"]?.ToString() ?? "",
                password = selected["password"]?.ToString() ?? "",
                emailhostname = selected["emailhostname"]?.ToString() ?? "",
                portnumber = selected["portnumber"] == DBNull.Value ? 25 : Convert.ToInt32(selected["portnumber"]),
                applicableservice = selected["applicableservice"]?.ToString() ?? ""
            };
        }
    }
}
