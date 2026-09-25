namespace Admin.Controllers
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Net.Mail;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;
	using Microsoft.Extensions.Logging;
    using Newtonsoft.Json.Linq;

    public class FrontDeskController : BaseController
    {
        private readonly IOptions<MailSettings> _mailSettings;
        private readonly IConfiguration _configuration;
		private readonly ILogger<FrontDeskController> _logger;

        public FrontDeskController(IConfiguration configuration, IOptions<MailSettings> mailSettings, ILogger<FrontDeskController> logger) : base(configuration)
        {
            _configuration = configuration;
            _mailSettings = mailSettings;
			_logger = logger;
        }

        public IActionResult Dashboard()
        {
            if (!HasRoleAuthorization("FrontDesk", "Dashboard") &&
                !HasUserRole("Frontdesk Admin"))
                return RedirectToAction("RoleAuthorizationFailed", "users");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendPaymentReminder([FromBody] PaymentReminderRequest request)
        {
            if (!HasUserRole("Frontdesk Admin"))
                return Json(new { success = false, message = "Only Frontdesk users can send payment reminders." });

            var loginUserId = HttpContext.Session.GetString("NalamVazhaloginUserID");
            var tenantId = HttpContext.Session.GetString("NalamVazhachoosedtenantid") ?? "";
            if (string.IsNullOrWhiteSpace(loginUserId))
                return Json(new { success = false, message = "Your session has expired. Please sign in again." });
            if (request == null || request.IPDApplicationFormid == Guid.Empty)
                return Json(new { success = false, message = "A valid IPD booking is required." });

            try
            {
                var client = getHttpClient();
                var reminderContext = await LoadPaymentReminderContext(
                    client, request.IPDApplicationFormid, tenantId, loginUserId);

                if (reminderContext == null)
                    return Json(new { success = false, message = "The IPD booking could not be found." });
                if (!reminderContext.IsEligibleStatus)
                    return Json(new { success = false, message = "Payment reminders are available only for a provisional room hold or an occupied room." });
                if (reminderContext.PendingAmount <= 0)
                    return Json(new { success = false, message = "This booking no longer has a pending payment." });
                if (!IsValidEmail(reminderContext.PatientEmail))
                    return Json(new { success = false, message = "The health seeker does not have a valid email address." });

                var cooldownHours = Math.Max(1, _configuration.GetValue<int?>("PaymentReminderSettings:CooldownHours") ?? 24);
                var mailFor = "IPDApplicationForm / PaymentReminder";
                var logJson = await ApiClient.Get_ApiValues(client,
                    "api/MailLogs/MailLogs_List?entityname=IPDApplicationForm&mailfor=" + Uri.EscapeDataString(mailFor) +
                    "&pagesize=1000&pagenumber=0&searchterm=" + Uri.EscapeDataString(reminderContext.PatientEmail) +
                    "&loginUserID=" + Uri.EscapeDataString(loginUserId));
				if (!request.ForceSend && HasRecentSuccessfulReminder(
					logJson, request.IPDApplicationFormid, reminderContext.PatientEmail, cooldownHours))
					return Json(new
					{
						success = false,
						duplicate = true,
						message = $"Reminder has already been sent to this HS in the last {cooldownHours} hours, are you sure you still want to send another reminder?"
					});

                // The dashboard performs this preflight check on the initial click so
                // a recent-reminder warning is shown before the normal send dialog.
                if (request.CheckOnly)
                    return Json(new { success = true, checkOnly = true });

                var effectiveTenantId = !string.IsNullOrWhiteSpace(reminderContext.TenantId)
                    ? reminderContext.TenantId
                    : tenantId;
                var sent = await new MailSender().sendNotification(
                    "IPDApplicationForm", "PaymentReminder", request.IPDApplicationFormid.ToString(),
                    _mailSettings, loginUserId, client, _configuration.GetSection("ApiSettings:baseURL").Value ?? "", effectiveTenantId);

                if (sent)
                    return Json(new { success = true, message = "Payment reminder sent successfully." });

                return Json(new { success = false, message = "The reminder could not be sent. Check the patient email and payment-reminder template." });
            }
			catch (Exception ex)
            {
				_logger.LogError(ex, "Payment reminder failed for IPD {IPDApplicationFormId}.", request?.IPDApplicationFormid);
                return Json(new { success = false, message = "The reminder could not be sent. Please try again." });
            }
        }

        private static bool IsValidEmail(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            try { _ = new MailAddress(value); return true; }
            catch { return false; }
        }

        private static async Task<PaymentReminderContext> LoadPaymentReminderContext(
            HttpClient client, Guid ipdId, string tenantId, string loginUserId)
        {
            var bookingJson = await ApiClient.Get_ApiValues(client,
                "api/IPDApplicationForm/Get_IPD_Payment_Details?IPDApplicationFormid=" +
                Uri.EscapeDataString(ipdId.ToString()));
            var booking = FirstObject(bookingJson);
            if (booking == null ||
                !string.Equals(ReadString(booking, "IPDApplicationFormid"), ipdId.ToString(), StringComparison.OrdinalIgnoreCase))
                return null;

            var bookingTenantId = ReadString(booking, "tenantid");
            if (!string.IsNullOrWhiteSpace(tenantId) && !string.IsNullOrWhiteSpace(bookingTenantId) &&
                !string.Equals(tenantId, bookingTenantId, StringComparison.OrdinalIgnoreCase))
                return null;

            var patientId = ReadString(booking, "patientname");
            if (!Guid.TryParse(patientId, out var patientGuid)) return null;

            var patientJson = await ApiClient.Get_ApiValues(client,
                "api/PatientProfile/getById_PatientProfile?PatientProfileid=" +
                Uri.EscapeDataString(patientGuid.ToString()) +
                "&loginUserID=" + Uri.EscapeDataString(loginUserId ?? ""));
            var patient = FirstObject(patientJson);

            var pendingJson = await ApiClient.Get_ApiValues(client,
                "api/BillingPayment/Get_Unified_Pending_Receivables?PatientID=" +
                Uri.EscapeDataString(patientGuid.ToString()) +
                "&IPDNo=" + Uri.EscapeDataString(ipdId.ToString()) +
                "&Type=IPD&loginUserID=" + Uri.EscapeDataString(loginUserId ?? ""));

            var bookingStatus = ReadString(booking, "bookingstatus");
            var hasAllocatedRoom = HasAllocatedRoom(booking);
			var isEligibleStatus =
				string.Equals(bookingStatus, "Provisional Booking", StringComparison.OrdinalIgnoreCase) ||
				(hasAllocatedRoom && string.Equals(bookingStatus, "Admitted", StringComparison.OrdinalIgnoreCase));

            return new PaymentReminderContext
            {
                TenantId = bookingTenantId,
                IsEligibleStatus = isEligibleStatus,
                PatientEmail = ReadString(patient, "emailaddress"),
                PendingAmount = SumPendingBalance(pendingJson)
            };
        }

        private static JObject FirstObject(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return null;
            var token = JToken.Parse(json);
            if (token.Type == JTokenType.String)
                token = JToken.Parse(token.Value<string>() ?? "[]");
            if (token is JArray array) return array.OfType<JObject>().FirstOrDefault();
            if (token is JObject obj) return obj;
            return null;
        }

        private static JToken ReadToken(JObject row, string name)
        {
            return row?.Properties()
                .FirstOrDefault(property => string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase))
                ?.Value;
        }

        private static string ReadString(JObject row, string name)
        {
            return ReadToken(row, name)?.ToString()?.Trim() ?? "";
        }

        private static bool HasAllocatedRoom(JObject booking)
        {
            var rooms = ReadToken(booking, "automaton_IPDApplicationForm_room");
            if (rooms == null || rooms.Type == JTokenType.Null)
                rooms = ReadToken(booking, "blocked_room_details_json");
            if (rooms == null || rooms.Type == JTokenType.Null) return false;
            if (rooms.Type == JTokenType.String)
            {
                var raw = rooms.Value<string>();
                if (string.IsNullOrWhiteSpace(raw)) return false;
                try { rooms = JToken.Parse(raw); }
                catch { return false; }
            }
            return rooms is JArray roomArray && roomArray.OfType<JObject>().Any();
        }

        private static decimal SumPendingBalance(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return 0;
            var token = JToken.Parse(json);
            if (token.Type == JTokenType.String)
                token = JToken.Parse(token.Value<string>() ?? "[]");
            var rows = token as JArray;
            if (rows == null) return 0;

            var total = rows.OfType<JObject>().Sum(row =>
            {
                var value = ReadString(row, "balance");
                return decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var balance)
                    ? balance
                    : 0;
            });
            return Math.Max(0, total);
        }

        private static bool HasRecentSuccessfulReminder(
            string json, Guid ipdId, string patientEmail, int cooldownHours)
        {
            if (string.IsNullOrWhiteSpace(json)) return false;
            try
            {
                var cutoff = DateTime.Now.AddHours(-cooldownHours);
                var root = JToken.Parse(json) as JContainer;
                if (root == null) return false;

                return root.DescendantsAndSelf().OfType<JObject>().Any(row =>
                {
                    DateTime created;
                    var sentValue = ReadString(row, "issent");
                    var wasSent = string.Equals(sentValue, "true", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(sentValue, "yes", StringComparison.OrdinalIgnoreCase) || sentValue == "1";
                    var sameHealthSeeker = string.Equals(
                        ReadString(row, "mailto"), patientEmail?.Trim(), StringComparison.OrdinalIgnoreCase);
                    var sameBooking = string.Equals(
                        ReadString(row, "entityid"), ipdId.ToString(), StringComparison.OrdinalIgnoreCase);
                    return (sameHealthSeeker || sameBooking) &&
                        string.Equals((string)row["mailfor"], "IPDApplicationForm / PaymentReminder", StringComparison.OrdinalIgnoreCase) &&
                        wasSent &&
                        DateTime.TryParse((string)row["createddate"], out created) &&
                        created >= cutoff;
                });
            }
            catch { return false; }
        }

        public sealed class PaymentReminderRequest
        {
            public Guid IPDApplicationFormid { get; set; }
			public bool ForceSend { get; set; }
			public bool CheckOnly { get; set; }
        }

        private sealed class PaymentReminderContext
        {
            public string TenantId { get; set; }
            public bool IsEligibleStatus { get; set; }
            public string PatientEmail { get; set; }
            public decimal PendingAmount { get; set; }
        }
    }
}
