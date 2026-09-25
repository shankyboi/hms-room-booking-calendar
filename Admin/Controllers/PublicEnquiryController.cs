namespace Admin.Controllers
{
    using System;
    using System.Data;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Threading.Tasks;
    using System.Net.Http.Headers;
    using Microsoft.Extensions.Options;
    using Microsoft.AspNetCore.Http;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.AspNetCore.Hosting;
    using System.Net;
    using FluentValidation.Results;
    using NalamVazha.Models;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.AspNetCore.HttpOverrides;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using System.Threading;
    using System.Globalization;
    using System.Text.Json;
    using Microsoft.AspNetCore.Authorization;

    [AllowAnonymous]
    public class PublicEnquiryController : BaseController
    {
        private IWebHostEnvironment hostingEnv;
        private IOptions<ApiSettings> _balSettings;
        private IOptions<MailSettings> _mailSettings;
        private IOptions<PublicFormSettings> _publicFormSettings;
        private string url = "";
        private string accesskey = "";
        private IHttpContextAccessor _accessor;
        public IConfiguration Configuration { get; }
        private readonly ILogger<PublicEnquiryController> _logger;

        public PublicEnquiryController(
            IConfiguration configuration,
            IHttpContextAccessor accessor,
            IOptions<ApiSettings> ApiSettings,
            IOptions<MailSettings> MailSettings,
            IOptions<PublicFormSettings> PublicFormSettings,
            IWebHostEnvironment env,
            ILogger<PublicEnquiryController> logger) : base(configuration)
        {
            _logger = logger;
            this.hostingEnv = env;
            _balSettings = ApiSettings;
            _mailSettings = MailSettings;
            _publicFormSettings = PublicFormSettings;
            url = _balSettings.Value.apiURL;
            accesskey = _balSettings.Value.accesskey;
            _accessor = accessor;
            Configuration = configuration;
        }

        private HttpClient getPublicHttpClient()
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(url);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            //httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accesskey);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //httpClient.DefaultRequestHeaders.Add("AuthProvider", "Internal");
            return httpClient;
        }

        public IActionResult Enquiry(string tenantid)
        {
            ViewBag.DefaultTenantId = tenantid ?? "";
            return View();
        }

        [HttpPost()]
        public async Task<string> Enquiry(EnquiryFormModel model, IFormCollection collection)
        {
            string strReturnMessage = "";
            string DefaultUserId = _publicFormSettings.Value.DefaultUserId;

            try
            {
                ModelState.Remove("EnquiryFormid");
                ModelState.Remove("createduser");
                ModelState.Remove("craftmyapp_actionmethodname");
                ModelState.Remove("tenantid");
                ModelState.Remove("patientname");
                ModelState.Remove("preferredroomtype");
                ModelState.Remove("enquirytype");
                ModelState.Remove("verifiedby");
                model.craftmyapp_actionmethodname = "Add_Enquiry";
                model.createduser = new Guid(DefaultUserId);
                model.EnquiryFormid = Guid.NewGuid();

                // Sanitize nullable Guid fields to prevent "Value cannot be null (Parameter 'g')" on API deserialization
                if (model.tenantid == null || model.tenantid == Guid.Empty)
                    model.tenantid = null;
                if (model.patientname == null || model.patientname == Guid.Empty)
                    model.patientname = null;
                if (model.preferredroomtype == null || model.preferredroomtype == Guid.Empty)
                    model.preferredroomtype = null;

                // Sanitize medicalinfo child rows
                if (model.medicalinfo != null)
                {
                    foreach (var med in model.medicalinfo)
                    {
                        if (med.medicalcondition == null || med.medicalcondition == Guid.Empty)
                            med.medicalcondition = null;
                    }
                }

                if (ModelState.IsValid)
                {
                    EnquiryFormModelValidator validator = new EnquiryFormModelValidator();
                    ValidationResult results = validator.Validate(model);
                    if (!results.IsValid)
                    {
                        var errorCollection = string.Join(" | ", results.Errors.Select(e => e.ErrorMessage.Replace("{propertyName}", e.PropertyName)));
                        strReturnMessage = errorCollection.ToString();
                        foreach (var failure in results.Errors)
                        {
                            ModelState.AddModelError(failure.PropertyName, failure.ErrorMessage);
                        }
                    }
                    else
                    {
                        using (var publicClient = getPublicHttpClient())
                        {
                            strReturnMessage = await ApiClient.Post_ApiValuesGetString(publicClient, "api/EnquiryForm/Add_Enquiry_Public", model);
                        }
                    }
                }
                else
                {
                    var errorMessages = ModelState.Where(entry => entry.Value.Errors.Any()).SelectMany(entry => entry.Value.Errors.Select(error => $"{entry.Key}: {error.ErrorMessage}"));
                    var errorCollection = string.Join(" | ", errorMessages);
                    strReturnMessage = errorCollection;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred in - PublicEnquiry / Enquiry, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));
                strReturnMessage = ex.Message;
            }

            if (strReturnMessage.Replace("\"", "").Contains("201.1"))
            {
                try
                {
                    // Fetch the created record to get enquirynumber
                    string enquirynumber = "";
                    using (var publicClient = getPublicHttpClient())
                    {
                        var jsonObjEnquiryForm = await ApiClient.Get_ApiValues(publicClient, "api/EnquiryForm/getById_EnquiryForm?EnquiryFormid=" + model.EnquiryFormid + "&loginUserID=" + DefaultUserId);
                        if (jsonObjEnquiryForm.Length > 2)
                        {
                            var createdRecord = JsonConvert.DeserializeObject<EnquiryFormModel>(jsonObjEnquiryForm);
                            enquirynumber = createdRecord.enquirynumber;
                        }
                    }

                    // Send confirmation email to the enquiry submitter using AlertTemplate
                    if (!string.IsNullOrEmpty(model.emailaddress))
                    {
                        try
                        {
                            using (var publicClient = getPublicHttpClient())
                            {
                                // Fetch the AlertTemplate for PublicEnquiryConfirmation
                                var alertTemplateJson = await ApiClient.Get_ApiValues(publicClient,
                                    "api/AlertTemplates/Alert_Templates_List?tenantid=" + Uri.EscapeDataString(model.tenantid?.ToString() ?? "")
                                    + "&entityname=EnquiryForm&entityaction=PublicEnquiryConfirmation&alerttype=Email");

                                if (!string.IsNullOrEmpty(alertTemplateJson) && alertTemplateJson.Length > 2)
                                {
                                    var alertTemplates = JsonConvert.DeserializeObject<DataTable>(alertTemplateJson);
                                    if (alertTemplates != null && alertTemplates.Rows.Count > 0)
                                    {
                                        string alertSubject = alertTemplates.Rows[0]["alertsubject"].ToString();
                                        string alertContent = alertTemplates.Rows[0]["alertcontent"].ToString();
                                        var mailSender = new MailSender();
                                        string tenantName = await mailSender.GetTenantName(publicClient, model.tenantid?.ToString() ?? "");
                                        if (string.IsNullOrWhiteSpace(tenantName))
                                            tenantName = "Healthcare Provider";

                                        // Replace placeholders with actual values from the submitted form
                                        alertSubject = alertSubject
                                            .Replace("{firstname}", model.firstname ?? "")
                                            .Replace("{lastname}", model.lastname ?? "")
                                            .Replace("{enquirynumber}", enquirynumber ?? "")
                                            .Replace("{emailaddress}", model.emailaddress ?? "")
                                            .Replace("{phonenumber}", model.phonenumber ?? "")
                                            .Replace("{gender}", model.gender ?? "")
                                            .Replace("{age}", model.age.ToString())
                                            .Replace("{enquiryreason}", model.enquiryreason ?? "")
                                            .Replace("{enquirydate}", model.enquirydate.ToString("dd/MM/yyyy"))
                                            .Replace("{preferreddateofarrival}", model.preferreddateofarrival?.ToString("dd/MM/yyyy") ?? "")
                                            .Replace("{preferreddateofdeparture}", model.preferreddateofdeparture?.ToString("dd/MM/yyyy") ?? "");

                                        alertContent = alertContent
                                            .Replace("{firstname}", model.firstname ?? "")
                                            .Replace("{lastname}", model.lastname ?? "")
                                            .Replace("{enquirynumber}", enquirynumber ?? "")
                                            .Replace("{emailaddress}", model.emailaddress ?? "")
                                            .Replace("{phonenumber}", model.phonenumber ?? "")
                                            .Replace("{gender}", model.gender ?? "")
                                            .Replace("{age}", model.age.ToString())
                                            .Replace("{enquiryreason}", model.enquiryreason ?? "")
                                            .Replace("{enquirydate}", model.enquirydate.ToString("dd/MM/yyyy"))
                                            .Replace("{preferreddateofarrival}", model.preferreddateofarrival?.ToString("dd/MM/yyyy") ?? "")
                                            .Replace("{preferreddateofdeparture}", model.preferreddateofdeparture?.ToString("dd/MM/yyyy") ?? "");

                                        alertSubject = MailSender.ApplyTenantBranding(alertSubject, tenantName);
                                        alertContent = MailSender.ApplyTenantBranding(alertContent, tenantName);

                                        MailBoxModel mailBox = await mailSender.GetTenantMailBox(publicClient, model.tenantid?.ToString() ?? "", MailSender.ResolveApplicableService("EnquiryForm", "PublicEnquiryConfirmation"));
                                        Mailer objmail = mailBox == null ? null : new Mailer(mailBox);
                                        bool mailSent = objmail != null && objmail.SendMail_TLS(model.emailaddress, alertSubject, alertContent, true, null, true);

                                        // Log the sent email
                                        MailLogsModel objMailLog = new MailLogsModel();
                                        objMailLog.MailLogsid = Guid.NewGuid();
                                        objMailLog.entityname = "EnquiryForm";
                                        objMailLog.entityid = model.EnquiryFormid.ToString();
                                        objMailLog.mailfor = "EnquiryForm / PublicEnquiryConfirmation";
                                        objMailLog.mailto = model.emailaddress;
                                        objMailLog.mailsubject = alertSubject;
                                        objMailLog.mailbody = alertContent;
                                        objMailLog.issent = mailSent;
                                        objMailLog.createduser = new Guid(DefaultUserId);
                                        objMailLog.craftmyapp_actionmethodname = "Create_MailLog";
                                        await ApiClient.Post_ApiValuesGetString(publicClient, "api/MailLogs/Create_MailLog", objMailLog);
                                    }
                                    else
                                    {
                                        _logger.LogWarning("No AlertTemplate found for EnquiryForm/PublicEnquiryConfirmation. Sending default email.");
                                        // Fallback: send a basic confirmation email if no template is configured
                                        var mailSender = new MailSender();
                                        string tenantName = await mailSender.GetTenantName(publicClient, model.tenantid?.ToString() ?? "");
                                        if (string.IsNullOrWhiteSpace(tenantName))
                                            tenantName = "Healthcare Provider";
                                        MailBoxModel mailBox = await mailSender.GetTenantMailBox(publicClient, model.tenantid?.ToString() ?? "", MailSender.ResolveApplicableService("EnquiryForm", "PublicEnquiryConfirmation"));
                                        Mailer objmail = mailBox == null ? null : new Mailer(mailBox);
                                        string emailSubject = "Enquiry Submitted Successfully - " + enquirynumber;
                                        string emailBody = "<p>Dear " + model.firstname + ",</p>"
                                            + "<p>Thank you for submitting your enquiry. Your enquiry number is <strong>" + enquirynumber + "</strong>.</p>"
                                            + "<p>Our team will review your enquiry and get back to you shortly.</p>"
                                            + "<p>" + System.Net.WebUtility.HtmlEncode(tenantName) + "</p>";
                                        if (objmail != null)
                                            objmail.SendMail_TLS(model.emailaddress, emailSubject, emailBody, true, null, true);
                                    }
                                }
                            }
                        }
                        catch (Exception mailEx)
                        {
                            _logger.LogError(mailEx, "Failed to send confirmation email for enquiry: " + model.EnquiryFormid);
                        }
                    }

                    // Send internal notification to staff using AlertTemplate (ReadyForReview)
                    try
                    {
                        using (var publicClient = getPublicHttpClient())
                        {
                            MailSender maillog = new MailSender();
                            bool mailsent = await maillog.sendNotification("EnquiryForm"
                                , "ReadyForReview"
                                , model.EnquiryFormid.ToString()
                                , _mailSettings
                                , DefaultUserId
                                , publicClient
                                , tenantid: model.tenantid?.ToString() ?? "");
                        }
                    }
                    catch (Exception notifEx)
                    {
                        _logger.LogError(notifEx, "Failed to send internal notification for enquiry: " + model.EnquiryFormid);
                    }

                    return "{\"status\":\"Success\",\"enquirynumber\":\"" + enquirynumber + "\"}";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Post-submission processing error for enquiry: " + model.EnquiryFormid);
                    return "{\"status\":\"Success\",\"enquirynumber\":\"\"}";
                }
            }
            else if (strReturnMessage.StartsWith("BadRequest", StringComparison.OrdinalIgnoreCase))
            {
                strReturnMessage = strReturnMessage.Replace("\"", "").Replace("BadRequest :", "");
                return strReturnMessage;
            }
            else
            {
                if (strReturnMessage == "401.1")
                    strReturnMessage = "Authorization Failed";

                return strReturnMessage;
            }
        }

        [HttpGet()]
        public async Task<string> lookup_enquirytype(string tenantid)
        {
            string DefaultUserId = _publicFormSettings.Value.DefaultUserId;
            using (var publicClient = getPublicHttpClient())
            {
                return await ApiClient.Get_ApiValues(publicClient, "api/EnquiryForm/lookup_EnquiryForm_enquirytype?tenantid=" + tenantid + "&loginUserID=" + DefaultUserId);
            }
        }

        [HttpGet()]
        public async Task<string> get_lookups_by_entity(string id)
        {
            using (var publicClient = getPublicHttpClient())
            {
                return await ApiClient.Get_ApiValues(publicClient, "api/lookups/get_lookups_by_entity?id=" + id);
            }
        }

        [HttpGet()]
        public async Task<string> lookup_preferredroomtype(string tenantid)
        {
            string DefaultUserId = _publicFormSettings.Value.DefaultUserId;
            using (var publicClient = getPublicHttpClient())
            {
                return await ApiClient.Get_ApiValues(publicClient, "api/EnquiryForm/lookup_EnquiryForm_preferredroomtype?tenantid=" + tenantid + "&loginUserID=" + DefaultUserId);
            }
        }

        [HttpGet()]
        public async Task<string> lookup_change_enquirytype(string EnquiryTypeid)
        {
            string DefaultUserId = _publicFormSettings.Value.DefaultUserId;
            using (var publicClient = getPublicHttpClient())
            {
                return await ApiClient.Get_ApiValues(publicClient, "api/EnquiryForm/lookup_change_EnquiryForm_enquirytype?EnquiryTypeid=" + EnquiryTypeid + "&loginUserID=" + DefaultUserId);
            }
        }

        [HttpGet()]
        public async Task<string> lookup_medicalcondition(string tenantid, string searchterm = "", string pagesize = "50", string pagenumber = "1")
        {
            using (var publicClient = getPublicHttpClient())
            {
                return await ApiClient.Get_ApiValues(publicClient, "api/MedicalCondition/get_all_MedicalCondition?tenantid=" + tenantid + "&searchterm=" + searchterm + "&pagesize=" + pagesize + "&pagenumber=" + pagenumber);
            }
        }
    }
}
