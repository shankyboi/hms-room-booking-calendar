using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Admin.Controllers
{
    public class DoctorDashboardController : BaseController
    {
		public DoctorDashboardController(IConfiguration configuration) : base(configuration) { }


        [HttpGet]
        public IActionResult Index()
        {
            if (!HasRoleAuthorization("DoctorDashboard", "Index") &&
                !HasUserRole("Doctor", "Intern Doctor"))
                return RedirectToAction("RoleAuthorizationFailed", "users");

            ViewBag.LoginUserId  = HttpContext.Session.GetString("NalamVazhaloginUserID") ?? "";
            ViewBag.TenantId     = HttpContext.Session.GetString("NalamVazhachoosedtenantid") ?? "";
            ViewBag.UserRole     = HttpContext.Session.GetString("NalamVazhauserrole") ?? "";
            ViewBag.UserName     = HttpContext.Session.GetString("NalamVazhaloggedinusername") ?? "";
            return View();
        }

        /* ── Proxy: get appointments ─────────────────────────────────────── */
        [HttpGet]
        public virtual async Task<string> GetAppointments(
            string tenantid,
            string practitioner     = "",
            string patient          = "",
            string status           = "",
            string appointmentdate_automatonfrom = "",
            string appointmentdate_automatonto   = "",
            int    pagesize  = 500,
            int    pagenumber = 0)
        {
            var loginId = HttpContext.Session.GetString("NalamVazhaloginUserID") ?? "";
            return await ApiClient.Get_ApiValues(getHttpClient(),
                $"api/ClinicalAppointment/get_Clinical_Appointment_List" +
                $"?tenantid={tenantid}" +
                $"&practitioner={practitioner}" +
                $"&patient={patient}" +
                $"&status={status}" +
                $"&appointmentdate_automatonfrom={appointmentdate_automatonfrom}" +
                $"&appointmentdate_automatonto={appointmentdate_automatonto}" +
                $"&pagesize={pagesize}&pagenumber={pagenumber}" +
                $"&loginUserID={loginId}");
        }

        /* ── Proxy: update appointment status ────────────────────────────── */
        [HttpPost]
        public virtual async Task<IActionResult> UpdateStatus([FromBody] AppointmentStatusRequest dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.clinicalappointmentid))
                return BadRequest(new { error = "Appointment ID required" });

            if (!Guid.TryParse(dto.clinicalappointmentid, out var apptId))
                return BadRequest(new { error = "Invalid appointment ID" });

            var payload = new { ClinicalAppointmentid = apptId, status = dto.status };
            var result  = await ApiClient.Post_ApiValuesGetString(getHttpClient(),
                            "api/OPDForm/Update_Appointment_Status", payload);

            return Content(result, "application/json");
        }

        /* ── Proxy: lookup practitioner by search term ───────────────────── */
        [HttpGet]
        public virtual async Task<string> LookupPractitioner(string tenantid, string searchterm = "")
        {
            var loginId = HttpContext.Session.GetString("NalamVazhaloginUserID") ?? "";
            return await ApiClient.Get_ApiValues(getHttpClient(),
                $"api/ClinicalAppointment/lookup_ClinicalAppointment_practitioner" +
                $"?tenantid={tenantid}&searchterm={searchterm}&pagesize=50&pagenumber=0" +
                $"&loginUserID={loginId}");
        }

        public class AppointmentStatusRequest
        {
            public string clinicalappointmentid { get; set; }
            public string status { get; set; }
        }
    }
}
