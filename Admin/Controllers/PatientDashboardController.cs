namespace Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json.Linq;
   

    /// <summary>
    /// Health Seeker (patient-facing) dashboard – no left menu, separate layout.
    /// Shows the current patient's own IPDs, OPDs, and payments.
    /// </summary>
    public class PatientDashboardController : BaseController
    {
        public PatientDashboardController(IConfiguration configuration) : base(configuration) { }

        private string TenantId    => HttpContext.Session.GetString("NalamVazhachoosedtenantid") ?? "";
        private string LoginUserId => HttpContext.Session.GetString("NalamVazhaloginUserID") ?? "";
        private string LoginStr    => "&loginUserID=" + LoginUserId;

        // ── Dashboard home ────────────────────────────────────────────────────
        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(LoginUserId))
                return RedirectToAction("Login", "users");

            if (!HasRoleAuthorization("PatientDashboard", "Index") &&
                !HasUserRole("Health Seeker"))
                return RedirectToAction("RoleAuthorizationFailed", "users");

            return View();
        }

        // ── My IPDs (filtered to this patient's PatientProfileid) ─────────────
        [HttpGet]
        public virtual async Task<string> GetMyIPDs(
        string bookingstatus = "",
        string createddate_automatonfrom = "",
        string createddate_automatonto = "",
        int pagesize = 200,
        int pagenumber = 0,
        string searchterm = "",
        string bookingnumber = "", string workflowstatus = "", string financialstatus = "", string paymentmethod = "")
        {
            return await ApiClient.Get_ApiValues(
                getHttpClient(),
                "api/IPDApplicationForm/Added_IPD_Application_Form" +
                "?tenantid=" + TenantId +
                "&patientname=" + LoginUserId +
                "&bookingstatus=" + bookingstatus +
                "&createddate_automatonfrom=" + createddate_automatonfrom +
                "&createddate_automatonto=" + createddate_automatonto +
                "&pagesize=" + pagesize +
                "&pagenumber=" + pagenumber +
                "&searchterm=" + searchterm +
                "&bookingnumber=" + System.Net.WebUtility.UrlEncode(bookingnumber) +
                "&workflowstatus=" + System.Net.WebUtility.UrlEncode(workflowstatus) +
                "&financialstatus=" + System.Net.WebUtility.UrlEncode(financialstatus) +
                "&paymentmethod=" + System.Net.WebUtility.UrlEncode(paymentmethod) +
                LoginStr
            );
        }

        // ── My OPDs (filtered to this patient) ───────────────────────────────
        [HttpGet]
        public virtual async Task<string> GetMyOPDs(
       string createddate_automatonfrom = "",
       string createddate_automatonto = "",
       int pagesize = 200,
       int pagenumber = 0,
       string searchterm = "",
       string bookingnumber = "", string workflowstatus = "", string financialstatus = "", string paymentmethod = "")
        {
            return await ApiClient.Get_ApiValues(getHttpClient(),
                "api/OPDForm/Added_OPD_Form" +
                "?tenantid=" + TenantId +
                "&patientname=" + LoginUserId +
                "&createddate_automatonfrom=" + createddate_automatonfrom +
                "&createddate_automatonto=" + createddate_automatonto +
                "&verifiedstatus=" +
                "&pagesize=" + pagesize +
                "&pagenumber=" + pagenumber +
                "&searchterm=" + searchterm +
                "&bookingnumber=" + System.Net.WebUtility.UrlEncode(bookingnumber) +
                "&workflowstatus=" + System.Net.WebUtility.UrlEncode(workflowstatus) +
                "&financialstatus=" + System.Net.WebUtility.UrlEncode(financialstatus) +
                "&paymentmethod=" + System.Net.WebUtility.UrlEncode(paymentmethod) +
                LoginStr);
        }

        // ── My Payments ───────────────────────────────────────────────────────
        [HttpGet]
        public virtual async Task<IActionResult> GetOPDBillingStatus(string OPDFormid)
        {
            if (string.IsNullOrWhiteSpace(OPDFormid))
                return Json(new { hasFee = false, balance = 0m, totalPaid = 0m });

            var json = await ApiClient.Get_ApiValues(getHttpClient(),
                "api/OPDForm/OPD_Consultation_Fee?OPDFormid=" + OPDFormid + LoginStr);

            if (string.IsNullOrWhiteSpace(json) || json.Length <= 2)
                return Json(new { hasFee = false, balance = 0m, totalPaid = 0m });

            JArray rows;
            try { rows = JArray.Parse(json); }
            catch { return Json(new { hasFee = false, balance = 0m, totalPaid = 0m }); }
            decimal totalAmount = 0m;
            decimal totalPaid = 0m;
            foreach (var row in rows)
            {
                if (decimal.TryParse(row?["amount"]?.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var amount))
                    totalAmount += amount;
                if (decimal.TryParse(row?["paidamount"]?.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var paid))
                    totalPaid += paid;
            }

            return Json(new
            {
                hasFee = totalAmount > 0,
                balance = Math.Max(0m, totalAmount - totalPaid),
                totalPaid
            });
        }

        [HttpGet]
        public virtual async Task<IActionResult> GetIPDBillingStatus(string IPDApplicationFormid)
        {
            if (string.IsNullOrWhiteSpace(IPDApplicationFormid)
                || !Guid.TryParse(IPDApplicationFormid, out _)
                || string.IsNullOrWhiteSpace(LoginUserId))
                return Json(new { hasBalance = false, balance = 0m });

            var client = getHttpClient();

            // Receivables are linked to PatientProfileid, which is not guaranteed to be
            // the same as the authenticated usersid (for example, Front Desk-created IPDs).
            // Resolve the patient from this IPD before querying its pending receivables.
            var ipdJson = await ApiClient.Get_ApiValues(client,
                "api/IPDApplicationForm/Get_IPD_Payment_Details" +
                "?IPDApplicationFormid=" + System.Net.WebUtility.UrlEncode(IPDApplicationFormid) +
                LoginStr);

            NalamVazha.Models.IPDPaymentDetailsModel ipdPayment;
            try
            {
                ipdPayment = Newtonsoft.Json.JsonConvert.DeserializeObject<NalamVazha.Models.IPDPaymentDetailsModel>(ipdJson ?? "");
            }
            catch
            {
                return Json(new { hasBalance = false, balance = 0m });
            }

            if (ipdPayment == null || string.IsNullOrWhiteSpace(ipdPayment.patientname))
                return Json(new { hasBalance = false, balance = 0m });

            // Health Seekers may only query billing status for their own booking.
            if (!string.Equals(ipdPayment.patientname, LoginUserId, StringComparison.OrdinalIgnoreCase))
                return Json(new { hasBalance = false, balance = 0m });

            var json = await ApiClient.Get_ApiValues(client,
                "api/BillingPayment/Get_Unified_Pending_Receivables" +
                "?PatientID=" + System.Net.WebUtility.UrlEncode(ipdPayment.patientname) +
                "&IPDNo=" + System.Net.WebUtility.UrlEncode(IPDApplicationFormid) +
                "&Type=IPD" +
                LoginStr);

            if (string.IsNullOrWhiteSpace(json) || json.Length <= 2)
                return Json(new { hasBalance = false, balance = 0m });

            JArray rows;
            try
            {
                var token = JToken.Parse(json);
                rows = token as JArray
                    ?? token["detail"] as JArray
                    ?? token["Detail"] as JArray
                    ?? new JArray();
            }
            catch { return Json(new { hasBalance = false, balance = 0m }); }

            decimal balance = 0m;
            foreach (var row in rows)
            {
                var balanceValue = row?.Children<JProperty>()
                    .FirstOrDefault(property => property.Name.Equals("balance", StringComparison.OrdinalIgnoreCase))
                    ?.Value?.ToString();

                if (decimal.TryParse(balanceValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var rowBalance)
                    || decimal.TryParse(balanceValue, NumberStyles.Any, CultureInfo.CurrentCulture, out rowBalance))
                    balance += rowBalance;
            }

            balance = Math.Max(0m, balance);
            return Json(new { hasBalance = balance > 0.009m, balance });
        }

        [HttpGet]
        public virtual async Task<string> GetMyPayments(
       string paymentdate_automatonfrom = "",
       string paymentdate_automatonto = "",
       int pagesize = 200,
       int pagenumber = 0)
        {
            return await ApiClient.Get_ApiValues(
                getHttpClient(),
                "api/BillingPayment/Get_HealthSeeker_Payments" +
                "?tenantid=" + TenantId +
                "&paymentdate_automatonfrom=" + paymentdate_automatonfrom +
                "&paymentdate_automatonto=" + paymentdate_automatonto +
                "&patientid=" + LoginUserId +
                "&pagesize=" + pagesize +
                "&pagenumber=" + pagenumber +
                LoginStr
            );
        }
        // ── My Profile (for avatar / name display) ────────────────────────────
        [HttpGet]
        public virtual async Task<string> GetMyProfile()
        {
            return await ApiClient.Get_ApiValues(getHttpClient(),
                "api/PatientProfile/get_Patient_Profiles" +
                "?tenantid=" + TenantId +
                "&searchterm=" + LoginUserId +
                "&pagesize=1&pagenumber=0" +
                LoginStr);
        }

        // ── My Assessments (to check which IPD/OPD forms already have one) ────
        [HttpGet]
        public virtual async Task<string> GetMyAssessments(
            int pagesize = 500,
            int pagenumber = 0)
        {
            return await ApiClient.Get_ApiValues(getHttpClient(),
                "api/Assessment/Assessment_List" +
                "?tenantid=" + TenantId +
                "&patientname=" + LoginUserId +
                "&patientvisit=" +
                "&assessmentdate_automatonfrom=" +
                "&assessmentdate_automatonto=" +
                "&pagesize=" + pagesize +
                "&pagenumber=" + pagenumber +
                LoginStr);
        }

        // Assessment_List is a reporting projection and older database versions do
        // not consistently expose the OPD form id in it. Resolve the state from the
        // authoritative OPD lookup so the dashboard cannot offer a second, blank
        // assessment after the first one has already been saved.
        [HttpGet]
        public virtual async Task<IActionResult> GetMyOPDAssessmentStates(string opdformids = "")
        {
            var requestedIds = (opdformids ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(value => Guid.TryParse(value, out var id) ? id : Guid.Empty)
                .Where(id => id != Guid.Empty)
                .Distinct()
                .Take(200)
                .ToArray();

            if (requestedIds.Length == 0)
                return Json(Array.Empty<object>());

            var ownedIds = new HashSet<Guid>();
            var ownedStatuses = new Dictionary<Guid, string>();
            try
            {
                var myOpdsJson = await GetMyOPDs(pagesize: 1000, pagenumber: 0);
                var parsed = JToken.Parse(string.IsNullOrWhiteSpace(myOpdsJson) ? "[]" : myOpdsJson);
                var rows = parsed as JArray ?? parsed["detail"] as JArray ?? parsed["data"] as JArray ?? new JArray();
                foreach (var row in rows)
                {
                    var idText = (row["opdformid"] ?? row["OPDFormid"] ?? row["OPDFormId"])?.ToString();
                    if (Guid.TryParse(idText, out var id))
                    {
                        ownedIds.Add(id);
                        ownedStatuses[id] = (row["verifiedstatus"] ?? row["VerifiedStatus"])?.ToString()?.Trim() ?? string.Empty;
                    }
                }
            }
            catch (Newtonsoft.Json.JsonException)
            {
                return Json(Array.Empty<object>());
            }

            var lookupTasks = requestedIds
                .Where(ownedIds.Contains)
                .Select(async opdFormId =>
                {
                    var result = await ApiClient.Get_ApiValues(
                        getHttpClient(),
                        "api/Assessment/getAssessmentByOpdForm?opdformid=" + opdFormId
                        + "&loginUserID=" + LoginUserId);
                    var assessmentId = (result ?? string.Empty).Trim().Trim('"');
					var verifiedStatus = ownedStatuses.TryGetValue(opdFormId, out var status)
						? status : string.Empty;
					var isDraft = string.Equals(verifiedStatus, "Assessment Form - In Draft", StringComparison.OrdinalIgnoreCase);
					var isSubmitted = string.Equals(verifiedStatus, "Assessment Intern Review Pending", StringComparison.OrdinalIgnoreCase)
						|| string.Equals(verifiedStatus, "Assessment Doctor Review Pending", StringComparison.OrdinalIgnoreCase)
						|| string.Equals(verifiedStatus, "OPD Completed", StringComparison.OrdinalIgnoreCase);
                    return new
                    {
                        opdformid = opdFormId.ToString(),
						assessmentid = Guid.TryParse(assessmentId, out _) ? assessmentId : string.Empty,
						isDraft,
						submitted = isSubmitted
                    };
                });

            return Json(await Task.WhenAll(lookupTasks));
        }

        [HttpGet]
        public virtual async Task<IActionResult> GetMyFeedbackState(string ipdapplicationformid)
        {
            if (!Guid.TryParse(ipdapplicationformid, out var ipdId))
                return Json(new { submitted = false, assessmentId = "", isDraft = false });

            // Use the persisted task name as the source of truth. Assessment_List is
            // intended for the dashboard and does not consistently return taskname.
            var taskStateJson = await ApiClient.Get_ApiValues(getHttpClient(),
                "api/Assessment/Get_IPD_Assessment_Task_State?ipdapplicationformid=" + ipdId +
                "&loginUserID=" + LoginUserId);
            try
            {
                var parsed = JToken.Parse(string.IsNullOrWhiteSpace(taskStateJson) ? "[]" : taskStateJson);
                var rows = parsed as JArray ?? parsed["detail"] as JArray ?? parsed["data"] as JArray ?? new JArray();
                foreach (var row in rows)
                {
                    var taskName = (row["taskname"] ?? row["TaskName"])?.ToString()?.Trim();
                    if (!string.Equals(taskName, "Feedbackform", StringComparison.OrdinalIgnoreCase)) continue;

                    var assessmentId = (row["assessmentid"] ?? row["Assessmentid"])?.ToString() ?? "";
                    return Json(new { submitted = true, assessmentId, isDraft = false });
                }
            }
            catch (Newtonsoft.Json.JsonException) { }

            var assessmentsJson = await GetMyAssessments();
            try
            {
                var parsed = JToken.Parse(string.IsNullOrWhiteSpace(assessmentsJson) ? "[]" : assessmentsJson);
                var rows = parsed as JArray ?? parsed["detail"] as JArray ?? parsed["data"] as JArray ?? new JArray();
                foreach (var row in rows)
                {
                    var rowIpd = (row["ipdform"] ?? row["IPDForm"])?.ToString();
                    if (!string.Equals(rowIpd, ipdId.ToString(), StringComparison.OrdinalIgnoreCase)) continue;

                    var assessmentId = (row["Assessmentid"] ?? row["assessmentid"])?.ToString() ?? "";
                    var savedTask = (row["taskname"] ?? row["TaskName"])?.ToString()?.Trim();
                    var eligibility = (row["eligibleforfinaladmission"] ?? row["Eligibleforfinaladmission"])?.ToString()?.Trim();
                    if (string.Equals(savedTask, "Feedbackform", StringComparison.OrdinalIgnoreCase))
                    {
                        var submitted = string.Equals(eligibility, "patient", StringComparison.OrdinalIgnoreCase);
                        return Json(new { submitted, assessmentId, isDraft = !submitted });
                    }

                    var templateId = (row["questionnairetemplate"] ?? row["QuestionnaireTemplate"])?.ToString();
                    if (!Guid.TryParse(templateId, out _)) continue;
                    var templateJson = await ApiClient.Get_ApiValues(getHttpClient(),
                        "api/AssessmentTemplate/getById_AssessmentTemplate?AssessmentTemplateid=" + templateId +
                        "&loginUserID=" + LoginUserId);
                    var template = JObject.Parse(templateJson);
                    var templateTask = (template["taskname"] ?? template["TaskName"])?.ToString()?.Trim();
                    if (!string.Equals(templateTask, "Feedbackform", StringComparison.OrdinalIgnoreCase)) continue;

                    var submittedFromTemplate = string.Equals(eligibility, "patient", StringComparison.OrdinalIgnoreCase);
                    return Json(new { submitted = submittedFromTemplate, assessmentId, isDraft = !submittedFromTemplate });
                }
            }
            catch (Newtonsoft.Json.JsonException) { }

            return Json(new { submitted = false, assessmentId = "", isDraft = false });
        }

        // The IPD id is supplied from a row already restricted to the logged-in patient.
        [HttpGet]
        public virtual async Task<string> GetMyRoomAllocations(string ipdapplicationformid)
        {
            if (!Guid.TryParse(ipdapplicationformid, out _))
                return "[]";

            var myIpdsJson = await GetMyIPDs(pagesize: 1000, pagenumber: 0);
            try
            {
                var parsed = JToken.Parse(string.IsNullOrWhiteSpace(myIpdsJson) ? "[]" : myIpdsJson);
                var rows = parsed as JArray ?? parsed["detail"] as JArray ?? parsed["data"] as JArray ?? new JArray();
                var ownsIpd = false;
                foreach (var row in rows)
                {
                    var rowId = (row["ipdapplicationformid"] ?? row["IPDApplicationFormid"])?.ToString();
                    if (string.Equals(rowId, ipdapplicationformid, StringComparison.OrdinalIgnoreCase))
                    {
                        ownsIpd = true;
                        break;
                    }
                }
                if (!ownsIpd) return "[]";
            }
            catch (Newtonsoft.Json.JsonException)
            {
                return "[]";
            }

            return await ApiClient.Get_ApiValues(getHttpClient(),
                "api/RoomAllocation/Room_Allocation_List" +
                "?tenantid=" + TenantId +
                "&ipdno=" + System.Net.WebUtility.UrlEncode(ipdapplicationformid) +
                "&block=&building=&floor=&room=" +
                "&pagesize=100&pagenumber=0&searchterm=&sort_fields=" +
                LoginStr);
        }

        [HttpGet]
        public virtual async Task<string> GetAssessmentSuggestedTemplates(
            string ipdapplicationformid,
            string userrole = "Health Seeker",
            string taskname = "")
        {
            return await ApiClient.Get_ApiValues(getHttpClient(),
                "api/Assessment/get_Assessment_suggested_templates" +
                "?ipdapplicationformid=" + ipdapplicationformid +
                "&userrole=" + System.Net.WebUtility.UrlEncode(userrole ?? "") +
                "&taskname=" + System.Net.WebUtility.UrlEncode(taskname ?? "") +
                LoginStr);
        }
    }
}
