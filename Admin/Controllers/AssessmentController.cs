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
				using System.Text.RegularExpressions;
				using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;




	//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/23/2026 16:41:43
				
                
                
                
                
				public class AssessmentController : BaseController
				{	 
					private IWebHostEnvironment hostingEnv;
					private IOptions<ApiSettings> _balSettings;
                    private IOptions<MailSettings> _mailSettings;
					private string  url = "";
					private string  baseUrl = "";
                    private string  adminUrl = "";
                    private string  clientUrl = "";
                    private string  accesskey = "";
					private IHttpContextAccessor _accessor;
                    public IConfiguration Configuration { get; }
                    private readonly ILogger<AssessmentController> _logger;
                    
                    
                    StorageUtil util;
					public AssessmentController(IConfiguration configuration,IHttpContextAccessor accessor,IOptions<ApiSettings> ApiSettings, IOptions<MailSettings> MailSettings, IWebHostEnvironment env, ILogger<AssessmentController> logger):base( configuration)
					{
                        _logger = logger;
						this.hostingEnv = env;
						_balSettings = ApiSettings;
                        _mailSettings = MailSettings;
						url = _balSettings.Value.apiURL;
						baseUrl = _balSettings.Value.baseURL;
                        adminUrl = _balSettings.Value.adminURL;
                        clientUrl = _balSettings.Value.clientURL;
                        accesskey = _balSettings.Value.accesskey;
 
						_accessor = accessor;
                        Configuration = configuration;
                        util = new StorageUtil(configuration);
					}
						 
 
                     public virtual IActionResult audit()
			         {
					        return View();
			         }
	              

					
				
			  public virtual async Task<string> getById_assessmentquestions(string Assessmentid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/Assessment/getById_assessmentquestions?Assessmentid="+Assessmentid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
					 
			  }

				
	          // ── Patient Answers ──────────────────────────────────────────────────────
          public virtual async Task<string> getById_patientanswers(string Assessmentid)
          {
              return await ApiClient.Get_ApiValues(getHttpClient(),
                  "api/Assessment/getById_patientanswers?Assessmentid=" + Assessmentid
                  + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
          }

          [HttpGet]
          public virtual async Task<string> getLatestPreviousPatientAnswers(
              string patientname, string questionnairetemplate, string taskname = "", string opdform = "", string ipdform = "")
          {
              return await ApiClient.Get_ApiValues(getHttpClient(),
                  "api/Assessment/getLatestPreviousPatientAnswers?patientname=" + Uri.EscapeDataString(patientname ?? "")
                  + "&questionnairetemplate=" + Uri.EscapeDataString(questionnairetemplate ?? "")
                  + "&taskname=" + Uri.EscapeDataString(taskname ?? "")
                  + "&opdform=" + Uri.EscapeDataString(opdform ?? "")
                  + "&ipdform=" + Uri.EscapeDataString(ipdform ?? "")
                  + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
          }

          [HttpGet]
          public virtual async Task<string> getLatestAssessmentForPrefill(
              string patientname, string taskname = "", string opdform = "", string ipdform = "")
          {
              return await ApiClient.Get_ApiValues(getHttpClient(),
                  "api/Assessment/getLatestAssessmentForPrefill?patientname=" + Uri.EscapeDataString(patientname ?? "")
                  + "&taskname=" + Uri.EscapeDataString(taskname ?? "")
                  + "&opdform=" + Uri.EscapeDataString(opdform ?? "")
                  + "&ipdform=" + Uri.EscapeDataString(ipdform ?? "")
                  + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
          }

          private async Task SavePatientAnswersIfPresent(AssessmentModel model)
          {
              if (string.IsNullOrWhiteSpace(model.patientanswers_json)) return;
              try
              {
                  var payload = new
                  {
                      assessmentid = model.Assessmentid.ToString(),
                      answers_json = model.patientanswers_json
                  };
                  await ApiClient.Post_ApiValuesGetString(getHttpClient(),
                      "api/Assessment/Save_patientanswers", payload);
              }
              catch (Exception ex)
              {
                  _logger.LogError(ex, "SavePatientAnswersIfPresent failed: " + ex.Message);
              }
          }

		private async Task SendSeniorOPDAssessmentReviewEmail(Guid? opdFormId)
		{
			if (!opdFormId.HasValue || opdFormId.Value == Guid.Empty) return;
			try
			{
				var loginUserId = HttpContext.Session.GetString("NalamVazhaloginUserID") ?? "";
				var client = getHttpClient();
				var json = await ApiClient.Get_ApiValues(client,
					"api/OPDForm/Get_OPD_Online_Doctor_Workflow_Context?OPDFormid=" + opdFormId + "&loginUserID=" + loginUserId);
				var row = string.IsNullOrWhiteSpace(json) ? null : JsonConvert.DeserializeObject<JArray>(json)?.FirstOrDefault();
				if (row == null || !string.Equals(row["verifiedstatus"]?.ToString(), "Assessment Doctor Review Pending", StringComparison.OrdinalIgnoreCase)) return;
				var mode = row["appointmentmode"]?.ToString() ?? "";
				var taskName = row["taskname"]?.ToString() ?? "";
				if (mode.IndexOf("online", StringComparison.OrdinalIgnoreCase) < 0 && taskName.IndexOf("online", StringComparison.OrdinalIgnoreCase) < 0) return;

				var email = row["seniordoctoremail"]?.ToString()?.Trim();
				var healthSeekerEmail = row["patientemail"]?.ToString()?.Trim();
				var meetingLink = row["seniormeetinglink"]?.ToString()?.Trim();
				if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(meetingLink))
				{
					_logger.LogWarning("Senior OPD assessment email skipped for {OPDFormid}: email or screeningmeetinglink missing", opdFormId);
					return;
				}

				string Enc(string value) => System.Net.WebUtility.HtmlEncode(value ?? "");
				var opdNumber = row["bookingreferencenumber"]?.ToString() ?? "-";
				var subject = "OPD Assessment Review Required - " + opdNumber;
				var body = $"<p>Dear {Enc(row["seniordoctorname"]?.ToString())},</p>" +
					"<p>The intern has completed the assessment. Please review the submitted assessment and complete the senior consultation.</p>" +
					$"<p><strong>Patient:</strong> {Enc(row["patientname"]?.ToString())}<br/><strong>OPD Number:</strong> {Enc(opdNumber)}<br/>" +
					$"<strong>Appointment:</strong> {Enc(row["appointmentdate"]?.ToString())} {Enc(row["durationfrom"]?.ToString())} - {Enc(row["durationto"]?.ToString())}</p>" +
					$"<p><strong>Meeting Link:</strong> <a href='{Enc(meetingLink)}'>{Enc(meetingLink)}</a></p>";
				var tenantId = row["tenantid"]?.ToString() ?? "";
				var mailBox = await new MailSender().GetTenantMailBox(client, tenantId, MailSender.ResolveApplicableService("ClinicalAppointment", "Allot_Doctor"));
				if (mailBox == null) return;
				var sent = new Mailer(mailBox).SendMail_TLS(
					email,
					string.IsNullOrWhiteSpace(healthSeekerEmail) ? null : healthSeekerEmail,
					subject, body, true, null, false);
				await ApiClient.Post_ApiValuesGetString(client, "api/MailLogs/Create_MailLog", new MailLogsModel
				{
					entityname = "OPDForm", entityid = opdFormId.ToString(), mailfor = "Senior Assessment Review",
					mailto = email, mailsubject = subject, mailbody = body, issent = sent,
					createduser = Guid.TryParse(loginUserId, out var userId) ? userId : Guid.Empty,
					craftmyapp_actionmethodname = "Create_MailLog"
				});
			}
			catch (Exception ex) { _logger.LogError(ex, "Senior OPD assessment review email failed for {OPDFormid}", opdFormId); }
		}

		  private bool IsDirectAdmissionRequested(IFormCollection collection)
		  {
			  return collection != null
				  && string.Equals(collection["allowdirectadmission"].ToString(), "yes", StringComparison.OrdinalIgnoreCase);
		  }

		  private static DateTime GetIndiaCurrentDateTime()
		  {
			  var utcNow = DateTime.UtcNow;
			  foreach (var timeZoneId in new[] { "India Standard Time", "Asia/Kolkata" })
			  {
				  try
				  {
					  return TimeZoneInfo.ConvertTimeFromUtc(utcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneId));
				  }
				  catch (TimeZoneNotFoundException)
				  {
				  }
				  catch (InvalidTimeZoneException)
				  {
				  }
			  }

			  // India has no daylight-saving transition, so UTC+05:30 is a safe
			  // fallback when the deployment does not contain either time-zone ID.
			  return utcNow.AddMinutes(330);
		  }

		  private static bool IsAssessmentReviewPendingStatus(string bookingStatus)
		  {
			  return string.Equals(bookingStatus?.Trim(), "Assessment Form - Review Pending", StringComparison.OrdinalIgnoreCase)
				  || string.Equals(bookingStatus?.Trim(), "Screening completed by the patient - Doctor Review Pending", StringComparison.OrdinalIgnoreCase)
				  || string.Equals(bookingStatus?.Trim(), "Screening completed by the patient - Intern Doctor Review Pending", StringComparison.OrdinalIgnoreCase);
		  }

		  private static bool IsAssessmentDraftSaveRequested(AssessmentModel model)
		  {
			  var action = model?.eligibleforfinaladmission?.Trim();
			  return string.Equals(action, "patient-saveasdraft", StringComparison.OrdinalIgnoreCase)
				  || string.Equals(action, "doctor-saveasdraft", StringComparison.OrdinalIgnoreCase)
				  || string.Equals(action, "fdesk-saveasdraft", StringComparison.OrdinalIgnoreCase);
		  }

		  private async Task<string> SubmitCompletedIPDAssessmentDraft(
			  AssessmentModel model,
			  Guid? actionUserId)
		  {
			  if (!model.ipdform.HasValue || model.ipdform.Value == Guid.Empty
				  || IsAssessmentDraftSaveRequested(model))
				  return "201.1";

			  var loginUserId = actionUserId?.ToString()
				  ?? HttpContext.Session.GetString("NalamVazhaloginUserID")
				  ?? string.Empty;
			  var client = getHttpClient();
			  var currentStatus = await GetCurrentIPDBookingStatus(client, model.ipdform.Value, loginUserId);
			  if (!string.Equals(currentStatus, "Assessment Form - In Draft", StringComparison.OrdinalIgnoreCase))
				  return "201.1";

			  var statusUpdate = new IPDBookingStatusUpdateModel
			  {
				  IPDApplicationFormid = model.ipdform.Value.ToString(),
				  bookingstatus = "Assessment Form - Review Pending",
				  modifieduser = loginUserId
			  };
			  var response = await ApiClient.Post_ApiValuesGetRawString(
				  client,
				  "api/IPDApplicationForm/Update_IPD_Booking_Status",
				  statusUpdate);
			  var message = (response ?? string.Empty).Replace("\"", string.Empty);
			  if (message.Contains("201.1", StringComparison.OrdinalIgnoreCase))
				  return "201.1";

			  _logger.LogWarning(
				  "Assessment {AssessmentId} was submitted, but IPD {IPDFormId} remained in draft. Response: {Response}",
				  model.Assessmentid,
				  model.ipdform,
				  response);
			  return "Assessment was saved, but the IPD draft status could not be submitted. Please try again.";
		  }

		  private async Task<string> CompleteIPDAssessmentReviewIfPending(
			  AssessmentModel model,
			  Guid? actionUserId)
		  {
			  if (!model.ipdform.HasValue || model.ipdform.Value == Guid.Empty)
				  return "201.1";

			  var role = (HttpContext.Session.GetString("NalamVazhauserrole") ?? string.Empty).Trim();
			  if (!role.Equals("Doctor", StringComparison.OrdinalIgnoreCase)
				  && !role.Equals("Intern Doctor", StringComparison.OrdinalIgnoreCase))
				  return "201.1";

			  var loginUserId = actionUserId?.ToString()
				  ?? HttpContext.Session.GetString("NalamVazhaloginUserID")
				  ?? string.Empty;
			  var client = getHttpClient();
			  var currentStatus = await GetCurrentIPDBookingStatus(client, model.ipdform.Value, loginUserId);
			  if (!IsAssessmentReviewPendingStatus(currentStatus))
				  return "201.1";

			  var statusUpdate = new IPDBookingStatusUpdateModel
			  {
				  IPDApplicationFormid = model.ipdform.Value.ToString(),
				  bookingstatus = "Assessment Form Reviewed",
				  modifieduser = loginUserId
			  };
			  var response = await ApiClient.Post_ApiValuesGetRawString(
				  client,
				  "api/IPDApplicationForm/Update_IPD_Booking_Status",
				  statusUpdate);
			  var message = (response ?? string.Empty).Replace("\"", string.Empty);
			  if (message.Contains("201.1", StringComparison.OrdinalIgnoreCase))
				  return "201.1";

			  _logger.LogWarning(
				  "Assessment {AssessmentId} was saved, but IPD {IPDFormId} remained in review pending. Response: {Response}",
				  model.Assessmentid,
				  model.ipdform,
				  response);
			  return "Assessment was saved, but the IPD review status could not be completed. Please try again.";
		  }

		  private static bool IsAdmissionDecisionStage(string bookingStatus)
		  {
			  // The configured IPD workflow permits Admission Approved only after
			  // the assessment has been reviewed and screening has been scheduled.
			  return string.Equals(bookingStatus?.Trim(), "Screening Scheduled", StringComparison.OrdinalIgnoreCase);
		  }

		  private static bool IsProvisionalConfirmedBookingStatus(string bookingStatus)
		  {
			  var status = bookingStatus?.Trim();
			  return string.Equals(status, "Provisional Confirmed", StringComparison.OrdinalIgnoreCase)
				  // Preserve compatibility with the legacy misspelling still returned by
				  // some booking records.
				  || string.Equals(status, "Provisional Confimed", StringComparison.OrdinalIgnoreCase);
		  }

		  private async Task<string> GetCurrentIPDBookingStatus(HttpClient client, Guid ipdFormId, string loginUserId)
		  {
			  try
			  {
				  var json = await ApiClient.Get_ApiValues(client,
					  "api/IPDApplicationForm/getById_IPDApplicationForm?IPDApplicationFormid=" + ipdFormId
					  + "&loginUserID=" + Uri.EscapeDataString(loginUserId ?? ""));
				  var ipd = string.IsNullOrWhiteSpace(json)
					  ? null
					  : JsonConvert.DeserializeObject<IPDApplicationFormModel>(json);
				  return ipd?.bookingstatus?.Trim() ?? string.Empty;
			  }
			  catch (Exception ex)
			  {
				  _logger.LogWarning(ex, "Unable to read booking status for IPD form {IPDFormId}", ipdFormId);
				  return string.Empty;
			  }
		  }

		  private async Task<Guid?> GetLoggedInPeopleId(string loginUserId)
		  {
			  if (string.IsNullOrWhiteSpace(loginUserId)) return null;

			  try
			  {
				  var json = await ApiClient.Get_ApiValues(
					  getHttpClient(),
					  "api/People/GetPeopleIdByUserId?usersid=" + Uri.EscapeDataString(loginUserId));
				  if (string.IsNullOrWhiteSpace(json)) return null;

				  var trimmed = json.Trim().Trim('"');
				  if (Guid.TryParse(trimmed, out var directPeopleId)) return directPeopleId;

				  var peopleIdText = JObject.Parse(json)["peopleid"]?.ToString();
				  return Guid.TryParse(peopleIdText, out var peopleId) ? peopleId : null;
			  }
			  catch (Exception ex)
			  {
				  _logger.LogWarning(ex, "Unable to resolve People id for assessment user {LoginUserId}", loginUserId);
				  return null;
			  }
		  }

		  private async Task<bool> GetAllowDoctorToAdmitPatients(Guid? tenantid)
		  {
			  try
			  {
				  Guid tenantGuid = tenantid ?? Guid.Empty;
				  if (tenantGuid == Guid.Empty)
				  {
					  Guid.TryParse(HttpContext.Session.GetString("NalamVazhachoosedtenantid")
						  ?? HttpContext.Session.GetString("NalamVazhatenantid"), out tenantGuid);
				  }
				  if (tenantGuid == Guid.Empty) return false;

				  var jsonObjtenant = await ApiClient.Get_ApiValues(getHttpClient(),
					  "api/tenant/getById_tenant?tenantid=" + tenantGuid
					  + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
				  if (string.IsNullOrWhiteSpace(jsonObjtenant) || jsonObjtenant.Length <= 2) return false;

				  var tenant = JsonConvert.DeserializeObject<tenantModel>(jsonObjtenant);
				  return tenant != null && tenant.allowdoctortoadmitpatients;
			  }
			  catch (Exception ex)
			  {
				  _logger.LogError(ex, "GetAllowDoctorToAdmitPatients failed: " + ex.Message);
				  return false;
			  }
		  }

		  private async Task<string> MarkIPDAdmittedByDoctorIfRequested(AssessmentModel model, IFormCollection collection, Guid? modifieduser)
		  {
			  if (!IsDirectAdmissionRequested(collection)) return "201.1";
			  if (HttpContext.Session.GetString("NalamVazhauserrole") != "Doctor") return "Doctor direct admission is allowed only for Doctor login.";
			  if (!string.Equals(model.eligibleforfinaladmission ?? "", "approved", StringComparison.OrdinalIgnoreCase)) return "201.1";
			  if (model.ipdform == null || model.ipdform == Guid.Empty) return "IPD Application Form is required for direct admission.";
			  if (!await GetAllowDoctorToAdmitPatients(model.tenantid)) return "Doctor direct admission is not enabled for this healthcare provider.";

			  var loginUserId = modifieduser.HasValue && modifieduser.Value != Guid.Empty
				  ? modifieduser.Value.ToString()
				  : HttpContext.Session.GetString("NalamVazhaloginUserID");

			  var statusModel = new IPDBookingStatusUpdateModel
			  {
				  IPDApplicationFormid = model.ipdform.ToString(),
				  bookingstatus = "Arrival Confirmed",
				  verifiedstatus = "Direct Admission",
				  modifieduser = loginUserId
			  };

			  var json = await ApiClient.Post_ApiValuesGetRawString(getHttpClient(),
				  "api/IPDApplicationForm/Update_IPD_Booking_Status", statusModel);
			  var msg = (json ?? "").Replace("\"", "");
			  if (!msg.Contains("201.1")) return msg;

			  try
			  {
				  var completeBody = new CompleteIPScreeningForIPDRequestModel { IPDApplicationFormid = model.ipdform.ToString() };
				  var apptJson = await ApiClient.Post_ApiValuesGetRawString(getHttpClient(),
					  "api/OPDForm/Complete_IP_New_For_IPD_Booking", completeBody);
				  var apptMsg = (apptJson ?? "").Replace("\"", "");
				  if (!apptMsg.Contains("201.1"))
					  _logger.LogWarning("Doctor direct admission: booking status updated but IP New appointment completion failed: {Msg}", apptMsg);
			  }
			  catch (Exception ex)
			  {
				  _logger.LogError(ex, "Doctor direct admission: IP New appointment completion failed: " + ex.Message);
			  }

			  return "201.1";
		  }
          // ────────────────────────────────────────────────────────────────────────

		  public virtual async Task<string> prefill_Assessment_assessmentquestions(string questionnairetemplate)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/Assessment/prefill_Assessment_assessmentquestions?questionnairetemplate="+questionnairetemplate+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
					 
			  }

			  private JArray GetAssessmentTemplateArray(string json)
			  {
					if (string.IsNullOrWhiteSpace(json) || json.Trim() == "[]")
						return new JArray();

					try
					{
						var token = JToken.Parse(json);
						if (token.Type == JTokenType.String)
							token = JToken.Parse(token.ToString());
						if (token is JArray array)
							return array;
						if (token is JObject obj)
						{
							foreach (var key in new[] { "Table", "data", "result", "results" })
							{
								if (obj[key] is JArray nested)
									return nested;
							}
						}
					}
					catch
					{
					}
					return new JArray();
			  }

			  private string GetAssessmentTemplateValue(JToken token, params string[] names)
			  {
					foreach (var name in names)
					{
						var value = token?[name];
						if (value != null && value.Type != JTokenType.Null)
							return value.ToString();
					}
					return string.Empty;
			  }

			  private bool GetAssessmentTemplateFlag(JToken token, params string[] names)
			  {
					var value = GetAssessmentTemplateValue(token, names);
					return value.Equals("true", StringComparison.OrdinalIgnoreCase)
						|| value.Equals("t", StringComparison.OrdinalIgnoreCase)
						|| value.Equals("1", StringComparison.OrdinalIgnoreCase)
						|| value.Equals("yes", StringComparison.OrdinalIgnoreCase);
			  }

			  private async Task<JArray> EnsureSelectedAssessmentTemplateOption(AssessmentModel model, JArray templates, string loginUserId)
			  {
					templates ??= new JArray();
					if (model == null || model.questionnairetemplate == Guid.Empty)
						return templates;

					var selectedTemplateId = model.questionnairetemplate.ToString();
					var hasSelectedTemplate = templates.Any(t =>
						GetAssessmentTemplateValue(t, "AssessmentTemplateid", "assessmenttemplateid", "assessmentTemplateId", "questionnairetemplate")
							.Equals(selectedTemplateId, StringComparison.OrdinalIgnoreCase));

					if (hasSelectedTemplate)
						return templates;

					try
					{
						var templateJson = await ApiClient.Get_ApiValues(getHttpClient(),
							"api/AssessmentTemplate/getById_AssessmentTemplate?AssessmentTemplateid=" + selectedTemplateId
							+ "&loginUserID=" + loginUserId);
						if (!string.IsNullOrWhiteSpace(templateJson) && templateJson.Length > 2)
						{
							var template = JObject.Parse(templateJson);
							var templateName = GetAssessmentTemplateValue(template, "templatename", "TemplateName", "questionnairetemplate_master");
							if (!string.IsNullOrWhiteSpace(templateName))
							{
								templates.AddFirst(new JObject
								{
									["AssessmentTemplateid"] = selectedTemplateId,
									["templatename"] = templateName,
									["ispreselected"] = true
								});
							}
						}
					}
					catch (Exception ex)
					{
						_logger.LogWarning(ex, "Unable to append selected assessment template option for template {TemplateId}", selectedTemplateId);
					}

					return templates;
			  }

			  private async Task PrepareAssessmentTemplateOptions(AssessmentModel model)
			  {
					ViewBag.AssessmentTemplateOptionsJson = "[]";
					try
					{
						await PrepareAssessmentTemplateOptionsCore(model);
					}
					catch (Exception ex)
					{
						// Template suggestions are optional. Keep the assessment form usable
						// when the supporting API is unavailable or returns an invalid payload.
						ViewBag.AssessmentTemplateOptionsJson = "[]";
						_logger.LogWarning(ex, "Unable to prepare assessment template options");
					}
			  }

			  private async Task PrepareAssessmentOpdReference(AssessmentModel model)
			  {
					ViewBag.AssessmentOpdReference = "";
					if (model?.opdform == null || model.opdform.Value == Guid.Empty)
						return;

					try
					{
						var loginUserId = HttpContext.Session.GetString("NalamVazhaloginUserID") ?? "";
						var opdJson = await ApiClient.Get_ApiValues(getHttpClient(),
							"api/OPDForm/getById_OPDForm?OPDFormid=" + model.opdform.Value
							+ "&loginUserID=" + Uri.EscapeDataString(loginUserId));
						var opd = string.IsNullOrWhiteSpace(opdJson)
							? null
							: JsonConvert.DeserializeObject<OPDFormModel>(opdJson);
						ViewBag.AssessmentOpdReference = opd?.bookingreferencenumber ?? "";
					}
					catch (Exception ex)
					{
						// The form can still display and submit the stored OPD id when its
						// friendly booking reference is temporarily unavailable.
						_logger.LogWarning(ex, "Unable to prepare OPD reference for assessment form {OPDFormId}", model.opdform);
					}
			  }

			  private async Task PrepareAssessmentTemplateOptionsCore(AssessmentModel model)
			  {
					if (model == null)
						return;

					var loginUserId = HttpContext.Session.GetString("NalamVazhaloginUserID");
					var userRole = HttpContext.Session.GetString("NalamVazhauserrole") ?? "";
					string templatesJson = "";
					var usedScopedTemplateLookup = false;

					if (model.ipdform.HasValue && model.ipdform.Value != Guid.Empty)
					{
						usedScopedTemplateLookup = true;
						templatesJson = await ApiClient.Get_ApiValues(getHttpClient(),
							"api/Assessment/get_Assessment_suggested_templates?ipdapplicationformid=" + model.ipdform.Value
							+ "&userrole=" + System.Net.WebUtility.UrlEncode(userRole)
							+ "&taskname=" + System.Net.WebUtility.UrlEncode(model.taskname ?? "")
							+ "&loginUserID=" + loginUserId);
					}
					else if (model.opdform.HasValue && model.opdform.Value != Guid.Empty)
					{
						usedScopedTemplateLookup = true;
						templatesJson = await ApiClient.Get_ApiValues(getHttpClient(),
							"api/Assessment/get_opd_assessmenttemplates?opdformid=" + model.opdform.Value
							+ "&taskname=" + System.Net.WebUtility.UrlEncode(model.taskname ?? "")
							+ "&loginUserID=" + loginUserId);
					}

					var templates = GetAssessmentTemplateArray(templatesJson);
					if (!usedScopedTemplateLookup && templates.Count == 0 && model.tenantid.HasValue && model.tenantid.Value != Guid.Empty)
					{
						templatesJson = await ApiClient.Get_ApiValues(getHttpClient(),
							"api/Assessment/lookup_Assessment_questionnairetemplate?tenantid=" + model.tenantid.Value
							+ "&loginUserID=" + loginUserId);
						templates = GetAssessmentTemplateArray(templatesJson);
					}

					templates = await EnsureSelectedAssessmentTemplateOption(model, templates, loginUserId);
					ViewBag.AssessmentTemplateOptionsJson = templates.ToString(Formatting.None);
					if (model.questionnairetemplate != Guid.Empty || templates.Count == 0)
						return;

					var selected = templates.FirstOrDefault(t => GetAssessmentTemplateFlag(t, "ispreselected", "IsPreselected", "isPreSelected"))
						?? templates.FirstOrDefault(t => GetAssessmentTemplateFlag(t, "isfallback", "IsFallback"))
						?? templates.FirstOrDefault();

					var selectedId = GetAssessmentTemplateValue(selected, "AssessmentTemplateid", "assessmenttemplateid", "assessmentTemplateId", "questionnairetemplate");
					if (Guid.TryParse(selectedId, out var templateGuid))
						model.questionnairetemplate = templateGuid;
			  }

			  [HttpGet()]
			  public virtual async Task<IActionResult> Add_Assessment(Guid? ipdform, Guid? patientname, Guid? opdform = null, Guid? preferreddoctor = null, string taskname = "", string ipdformnumber = "", bool completedraft = false, bool interndoctorupdate = false, bool allotdoctor = false, bool allot = false, Guid? Assessmentid = null, string isfinal = "", string returnurl = "")
			  {
					// An OPD supports one assessment record. A stale dashboard/link must
					// never initialize a blank form over an assessment that already exists.
					if (opdform.HasValue && opdform.Value != Guid.Empty
						&& (!Assessmentid.HasValue || Assessmentid.Value == Guid.Empty))
					{
						var existingId = await ApiClient.Get_ApiValues(
							getHttpClient(),
							"api/Assessment/getAssessmentByOpdForm?opdformid=" + opdform.Value
							+ "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
						existingId = (existingId ?? string.Empty).Trim().Trim('"');
						if (Guid.TryParse(existingId, out var existingAssessmentId))
						{
							if (string.Equals(HttpContext.Session.GetString("NalamVazhauserrole"), "Health Seeker", StringComparison.OrdinalIgnoreCase))
								return RedirectToAction(nameof(View_Assessment), new { Assessmentid = existingAssessmentId, returnurl });

							return RedirectToAction(nameof(Update_Assessment), new { Assessmentid = existingAssessmentId, returnurl });
						}
					}

					var model = new AssessmentModel();
					ViewBag.AssessmentIpdBookingStatus = string.Empty;

					// Use currently chosen tenant for dropdown prefill (if available)
					var tenantIdStr = HttpContext.Session.GetString("NalamVazhachoosedtenantid");
					if (Guid.TryParse(tenantIdStr, out var tenantId))
					{
						model.tenantid = tenantId;
					}

					model.ipdform = ipdform;
					model.opdform = opdform;
					model.patientname = patientname ?? Guid.Empty;
					model.taskname = taskname ?? string.Empty;
					// Direct IPD and other IPD assessment links do not always include a
					// task name. Resolve that context before loading template options so
					// the configured IP Screening template is returned and preselected.
					if (ipdform.HasValue && ipdform.Value != Guid.Empty
						&& string.IsNullOrWhiteSpace(model.taskname))
					{
						model.taskname = "IP Screening";
					}
					model.assessmentdate = GetIndiaCurrentDateTime();

					// OPD list responses from older database functions can expose the
					// patient's registration number (for example AP-260424-0001) in the
					// patientname field. That value cannot bind to this action's Guid?
					// parameter, so use the OPD record as the authoritative source.
					if ((!patientname.HasValue || patientname.Value == Guid.Empty)
						&& opdform.HasValue && opdform.Value != Guid.Empty)
					{
						try
						{
							var loginUserId = HttpContext.Session.GetString("NalamVazhaloginUserID") ?? "";
							var opdJson = await ApiClient.Get_ApiValues(getHttpClient(),
								"api/OPDForm/getById_OPDForm?OPDFormid=" + opdform.Value
								+ "&loginUserID=" + Uri.EscapeDataString(loginUserId));
							var opd = string.IsNullOrWhiteSpace(opdJson)
								? null
								: JsonConvert.DeserializeObject<OPDFormModel>(opdJson);
							if (opd != null && opd.patientname != Guid.Empty)
							{
								model.patientname = opd.patientname;
								ModelState.Remove(nameof(patientname));
								if ((!model.tenantid.HasValue || model.tenantid.Value == Guid.Empty)
									&& opd.tenantid.HasValue && opd.tenantid.Value != Guid.Empty)
									model.tenantid = opd.tenantid;
								if ((!preferreddoctor.HasValue || preferreddoctor.Value == Guid.Empty)
									&& opd.preferreddoctor.HasValue && opd.preferreddoctor.Value != Guid.Empty)
									preferreddoctor = opd.preferreddoctor;
							}
						}
						catch (Exception ex)
						{
							_logger.LogWarning(ex, "Unable to resolve assessment patient from OPD form {OPDFormId}", opdform);
						}
					}

					if (preferreddoctor.HasValue && preferreddoctor.Value != Guid.Empty)
					{
						model.doctorname = preferreddoctor.Value;
					}
					else if (string.Equals(HttpContext.Session.GetString("NalamVazhauserrole"), "Doctor", StringComparison.OrdinalIgnoreCase)
						|| string.Equals(HttpContext.Session.GetString("NalamVazhauserrole"), "Intern Doctor", StringComparison.OrdinalIgnoreCase))
					{
						model.doctorname = await GetLoggedInPeopleId(
							HttpContext.Session.GetString("NalamVazhaloginUserID") ?? string.Empty);
					}

					// OPD assessment links can contain the human-readable booking number in
					// patientname. Resolve the authoritative patient/doctor from the OPD form
					// so the locked Patient Visit and OPD Form fields can still be prefilled.
					if (opdform.HasValue && opdform.Value != Guid.Empty)
					{
						try
						{
							var loginUserId = HttpContext.Session.GetString("NalamVazhaloginUserID") ?? string.Empty;
							var opdJson = await ApiClient.Get_ApiValues(
								getHttpClient(),
								"api/OPDForm/getById_OPDForm?OPDFormid="
								+ Uri.EscapeDataString(opdform.Value.ToString())
								+ "&loginUserID=" + Uri.EscapeDataString(loginUserId));
							var opd = string.IsNullOrWhiteSpace(opdJson)
								? null
								: JsonConvert.DeserializeObject<OPDFormModel>(opdJson);
							if (opd != null)
							{
								if (opd.patientname != Guid.Empty)
									model.patientname = opd.patientname;
								if ((!preferreddoctor.HasValue || preferreddoctor.Value == Guid.Empty)
									&& opd.preferreddoctor.HasValue && opd.preferreddoctor.Value != Guid.Empty)
									model.doctorname = opd.preferreddoctor.Value;
							}
						}
						catch (Exception ex)
						{
							_logger.LogWarning(ex, "Unable to resolve OPD assessment context for {OPDForm}", opdform.Value);
						}
					}

					// Apply the same authoritative context resolution to every IPD entry
					// point (new assessment, draft completion, intern review, allot doctor
					// and final consultation). This avoids depending on the value/format of
					// patientname supplied by individual dashboard links.
					if (ipdform.HasValue && ipdform.Value != Guid.Empty)
					{
						try
						{
							var loginUserId = HttpContext.Session.GetString("NalamVazhaloginUserID") ?? string.Empty;
							var ipdJson = await ApiClient.Get_ApiValues(
								getHttpClient(),
								"api/IPDApplicationForm/getById_IPDApplicationForm?IPDApplicationFormid="
								+ Uri.EscapeDataString(ipdform.Value.ToString())
								+ "&loginUserID=" + Uri.EscapeDataString(loginUserId));
							var ipd = string.IsNullOrWhiteSpace(ipdJson)
								? null
								: JsonConvert.DeserializeObject<IPDApplicationFormModel>(ipdJson);
							if (ipd != null)
							{
								ViewBag.AssessmentIpdBookingStatus = ipd.bookingstatus?.Trim() ?? string.Empty;
								if (ipd.patientname != Guid.Empty)
									model.patientname = ipd.patientname;
								if (ipd.tenantid.HasValue && ipd.tenantid.Value != Guid.Empty)
									model.tenantid = ipd.tenantid.Value;
							}
						}
						catch (Exception ex)
						{
							_logger.LogWarning(ex, "Unable to resolve IPD assessment context for {IPDForm}", ipdform.Value);
						}
					}

					// Load existing assessment for Complete Draft, Intern Doctor Update, or IPD Final Consultation (isfinal=Y) modes
					var isIpdFinalConsultation = string.Equals(isfinal, "Y", StringComparison.OrdinalIgnoreCase);
					if ((completedraft || interndoctorupdate || isIpdFinalConsultation) && (ipdform.HasValue || Assessmentid.HasValue))
					{
						try
						{
							var loginUserId = HttpContext.Session.GetString("NalamVazhaloginUserID");
							string assessmentIdToLoad = null;

							// When AssessmentId is explicitly provided, use it directly (faster than lookup by ipdform)
							if (Assessmentid.HasValue && Assessmentid.Value != Guid.Empty)
							{
								assessmentIdToLoad = Assessmentid.Value.ToString();
							}
							else if (ipdform.HasValue && ipdform.Value != Guid.Empty)
							{
								var assessmentsJson = await ApiClient.Get_ApiValues(getHttpClient(),
									$"api/Assessment/getAssessments_by_ipdform?ipdapplicationformid={ipdform}&loginUserID={loginUserId}");
								if (!string.IsNullOrWhiteSpace(assessmentsJson) && assessmentsJson.Length > 2)
								{
									var rows = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Data.DataTable>(assessmentsJson);
									if (rows != null && rows.Rows.Count > 0)
										assessmentIdToLoad = rows.Rows[0]["assessmentid"]?.ToString();
								}
							}

							if (!string.IsNullOrWhiteSpace(assessmentIdToLoad))
							{
								var assessmentJson = await ApiClient.Get_ApiValues(getHttpClient(),
									$"api/Assessment/getById_Assessment?Assessmentid={assessmentIdToLoad}&loginUserID={loginUserId}");
								if (!string.IsNullOrWhiteSpace(assessmentJson) && assessmentJson.Length > 2)
								{
									var existing = Newtonsoft.Json.JsonConvert.DeserializeObject<AssessmentModel>(assessmentJson);
									if (existing != null)
									{
										model = existing;
										model.ipdform = ipdform;
										model.opdform = opdform;
										if (!string.IsNullOrWhiteSpace(taskname))
											model.taskname = taskname;
										if (preferreddoctor.HasValue && preferreddoctor.Value != Guid.Empty)
											model.doctorname = preferreddoctor.Value;
									}
								}
							}
						}
						catch (Exception ex)
						{
							_logger.LogError(ex, "Add_Assessment GET: failed to load existing assessment for ipdform " + ipdform);
						}
					}

					// The final-consultation doctor must be a People id, not the login Users id.
					// The field is workflow-owned and hidden in the view, so resolve it here.
					if (isIpdFinalConsultation
						&& (string.Equals(HttpContext.Session.GetString("NalamVazhauserrole"), "Doctor", StringComparison.OrdinalIgnoreCase)
							|| string.Equals(HttpContext.Session.GetString("NalamVazhauserrole"), "Intern Doctor", StringComparison.OrdinalIgnoreCase)))
					{
						var loggedInPeopleId = await GetLoggedInPeopleId(
							HttpContext.Session.GetString("NalamVazhaloginUserID") ?? string.Empty);
						if (loggedInPeopleId.HasValue && loggedInPeopleId.Value != Guid.Empty)
							model.doctorname = loggedInPeopleId.Value;
					}

					// A brand-new IPD/OPD assessment inherits the latest assessment
					// template and notes for the same health seeker. The current
					// booking identifiers, doctor and workflow fields remain unchanged.
					if ((!model.Assessmentid.HasValue || model.Assessmentid.Value == Guid.Empty)
						&& model.patientname != Guid.Empty
						&& !string.Equals(model.taskname, "Feedbackform", StringComparison.OrdinalIgnoreCase)
						&& !string.Equals(model.taskname, "Dischargechecklist", StringComparison.OrdinalIgnoreCase))
					{
						try
						{
							var loginUserId = HttpContext.Session.GetString("NalamVazhaloginUserID") ?? "";
							var prefillJson = await ApiClient.Get_ApiValues(
								getHttpClient(),
								"api/Assessment/getLatestAssessmentForPrefill?patientname="
								+ Uri.EscapeDataString(model.patientname.ToString())
								+ "&taskname=" + Uri.EscapeDataString(model.taskname ?? "")
								+ "&opdform=" + Uri.EscapeDataString(model.opdform?.ToString() ?? "")
								+ "&ipdform=" + Uri.EscapeDataString(model.ipdform?.ToString() ?? "")
								+ "&loginUserID=" + Uri.EscapeDataString(loginUserId));
							var prefillRows = string.IsNullOrWhiteSpace(prefillJson)
								? null
								: JsonConvert.DeserializeObject<DataTable>(prefillJson);
							if (prefillRows?.Rows.Count > 0)
							{
								var row = prefillRows.Rows[0];
								// OPD and IPD assessments continue from the latest assessment for the
								// same patient, task and encounter type. Reusing its template is
								// required because answers are keyed by template-question identifiers.
								if ((model.opdform.HasValue && model.opdform.Value != Guid.Empty
										|| model.ipdform.HasValue && model.ipdform.Value != Guid.Empty)
									&& row.Table.Columns.Contains("questionnairetemplate")
									&& Guid.TryParse(row["questionnairetemplate"]?.ToString(), out var previousTemplateId))
								{
									model.questionnairetemplate = previousTemplateId;
								}
								if (string.IsNullOrWhiteSpace(model.assessmentnotes))
									model.assessmentnotes = row["assessmentnotes"]?.ToString();
								if (string.IsNullOrWhiteSpace(model.taskname)
									&& row.Table.Columns.Contains("taskname"))
									model.taskname = row["taskname"]?.ToString() ?? string.Empty;
								ViewBag.PrefillSourceAssessmentId = row["assessmentid"]?.ToString() ?? "";
							}
						}
						catch (Exception ex)
						{
							// No previous assessment is a valid new-health-seeker scenario.
							_logger.LogWarning(ex, "Unable to prefill latest assessment for patient {Patient}", model.patientname);
						}
					}

					// Used by Add_Assessment view to optionally show appointment booking popup (when launched from IPD list)
					if (ipdform.HasValue && Guid.TryParse(ipdformnumber, out var ipdFormNumberGuid)	&& ipdFormNumberGuid == ipdform.Value)
					{
						ipdformnumber = string.Empty;
					}
					ViewBag.IPDFormNumber = ipdformnumber ?? string.Empty;
					ViewBag.AllotDoctor = allotdoctor;
					ViewBag.Allot = allot;
					ViewBag.AllowDoctorToAdmitPatients = await GetAllowDoctorToAdmitPatients(model.tenantid);
					await PrepareAssessmentOpdReference(model);
					await PrepareAssessmentTemplateOptions(model);

					// Note: questionnaire template will auto-select first in the view
					// when Model.questionnairetemplate == Guid.Empty.
					return View("Add_Assessment", model);
			  }

			  [HttpGet()]
			  public virtual async Task<IActionResult> get_Assessments_for_IPD_Data(string IPDApplicationFormid)
			  {
					try
					{
						var loginUserId = HttpContext.Session.GetString("NalamVazhaloginUserID") ?? "";
						var json = await ApiClient.Get_ApiValues(getHttpClient(),
							$"api/Assessment/get_Assessments_for_IPD?ipdapplicationformid={IPDApplicationFormid}&loginUserID={loginUserId}");
						return Content(string.IsNullOrWhiteSpace(json) ? "[]" : json, "application/json");
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, "get_Assessments_for_IPD_Data error: " + ex.Message);
						return Content("[]", "application/json");
					}
			  }

			  [HttpGet()]
			  public virtual async Task<string> GetAssessmentHistory(
				  string assessmentid = "",
				  string ipdformid = "",
				  string opdformid = "")
			  {
				  var loginUserId = HttpContext.Session.GetString("NalamVazhaloginUserID") ?? "";
				  return await ApiClient.Get_ApiValues(
					  getHttpClient(),
					  "api/Assessment/GetAssessmentHistory?assessmentid=" + Uri.EscapeDataString(assessmentid ?? "")
					  + "&ipdformid=" + Uri.EscapeDataString(ipdformid ?? "")
					  + "&opdformid=" + Uri.EscapeDataString(opdformid ?? "")
					  + "&loginUserID=" + Uri.EscapeDataString(loginUserId));
			  }

			  private void ApplyAssessmentHistoryContext(AssessmentModel model, bool isNew)
			  {
				  var role = HttpContext.Session.GetString("NalamVazhauserrole") ?? "Unknown";
				  var task = model.taskname ?? string.Empty;
				  var isScreening = task.IndexOf("screen", StringComparison.OrdinalIgnoreCase) >= 0
					  || string.Equals(model.eligibleforfinaladmission, "screeningapproved", StringComparison.OrdinalIgnoreCase)
					  || string.Equals(model.eligibleforfinaladmission, "screeningrejected", StringComparison.OrdinalIgnoreCase);
				  var isDraft = (model.eligibleforfinaladmission ?? string.Empty)
					  .IndexOf("saveasdraft", StringComparison.OrdinalIgnoreCase) >= 0;

				  model.actionbyrole = role;
				  var isHealthSeeker = role.Equals("Health Seeker", StringComparison.OrdinalIgnoreCase);
				  model.historyactiontype = isDraft ? "DRAFT_SAVED" : (isNew ? "CREATED" : (isHealthSeeker ? "UPDATED" : "REVIEWED"));
				  if (isScreening)
					  model.workflowstage = model.ipdform.HasValue ? "IPD_SCREENING_REVIEW" : "OPD_SCREENING_REVIEW";
				  else if (role.Equals("Intern Doctor", StringComparison.OrdinalIgnoreCase))
					  model.workflowstage = model.ipdform.HasValue ? "IPD_INTERN_REVIEW" : "OPD_INTERN_REVIEW";
				  else if (role.Equals("Doctor", StringComparison.OrdinalIgnoreCase))
					  model.workflowstage = model.ipdform.HasValue ? "IPD_DOCTOR_REVIEW" : "OPD_DOCTOR_REVIEW";
				  else
					  model.workflowstage = model.ipdform.HasValue ? "IPD_INITIAL_ASSESSMENT" : "OPD_INITIAL_ASSESSMENT";
			  }

			  public virtual async Task<IActionResult> get_AssessmentId_by_IPDForm(Guid ipdform)
			  {
					try
					{
						var loginUserId = HttpContext.Session.GetString("NalamVazhaloginUserID");
						var assessmentsJson = await ApiClient.Get_ApiValues(getHttpClient(),
							$"api/Assessment/getAssessments_by_ipdform?ipdapplicationformid={ipdform}&loginUserID={loginUserId}");
						if (!string.IsNullOrWhiteSpace(assessmentsJson) && assessmentsJson.Length > 2)
						{
							var rows = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Data.DataTable>(assessmentsJson);
							if (rows?.Rows.Count > 0)
							{
								var id = rows.Rows[0]["assessmentid"]?.ToString();
								if (!string.IsNullOrWhiteSpace(id))
									return Json(new { assessmentid = id });
							}
						}
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, "get_AssessmentId_by_IPDForm error for ipdform " + ipdform);
					}
					return Json(new { assessmentid = "" });
			  }

		/// <summary>
		/// Creates ClinicalAppointment after an IPD Assessment is saved.
		/// Inserts: Assessment already done earlier; here we only create ClinicalAppointment.
		/// </summary>
		[HttpPost()]
        public virtual async Task<string> Create_IPD_Clinical_Appointment(
    string tenantid,
    Guid patient,
    Guid practitioner,
    string appointmentdate,
    string durationfrom,
    string durationto,
    string bookingid = "",
    string tasktype = "IP Screening",
    string taskid = "",
    bool createpayablereceivable = false
)
        {
			try
			{
				var loginUserId = HttpContext.Session.GetString("NalamVazhaloginUserID");
				if (string.IsNullOrWhiteSpace(loginUserId))
					return "Session Expired";

				if (!Guid.TryParse(tenantid, out Guid tenantGuid))
					return "Invalid tenantid";

				if (patient == Guid.Empty) return "Patient is required";
				if (practitioner == Guid.Empty) return "Doctor is required";

				if (!DateTime.TryParseExact(appointmentdate,
						new[] { "dd/MM/yyyy", "d/M/yyyy", "dd-MM-yyyy", "d-M-yyyy" },
						CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime apptDate))
					return "Invalid appointment date";

				var indiaNow = GetIndiaCurrentDateTime();
				if (apptDate.Date < indiaNow.Date)
					return "Past appointment dates are not allowed. Please select today or a future date.";

                if (string.IsNullOrWhiteSpace(durationfrom) ||
        string.IsNullOrWhiteSpace(durationto))
                {
                    return "Slot time is required";
                }

				if (!TimeSpan.TryParse(durationfrom, CultureInfo.InvariantCulture, out var slotStart))
					return "Invalid slot start time.";

				if (apptDate.Date.Add(slotStart) <= indiaNow)
					return "This appointment slot has already started. Please select a later slot.";

                var httpClient = getHttpClient();

				// Task type is passed by the caller (IP Screening for Allot Doctor, Consultation for Allot)
				string resolvedTaskTypeName = string.IsNullOrWhiteSpace(tasktype) ? "IP Screening" : tasktype;

				Guid selectedTaskId;
				if (!Guid.TryParse(taskid, out selectedTaskId) || selectedTaskId == Guid.Empty)
				{
					var resolvedTaskId = await ResolvePractitionerTaskId(
						httpClient,
						practitioner,
						resolvedTaskTypeName,
						loginUserId);

					if (!resolvedTaskId.HasValue || resolvedTaskId.Value == Guid.Empty)
						return "The selected doctor does not have an " + resolvedTaskTypeName.Trim() + " clinical task configured.";

					selectedTaskId = resolvedTaskId.Value;
				}

				// IP New appointments created from Pre-Admission Consultation must have a
				// positive doctor fee because this workflow also creates a receivable.
				// Validate before inserting the appointment so a configuration problem
				// cannot leave an appointment/status update without its financial row.
				if (createpayablereceivable
					&& string.Equals(resolvedTaskTypeName.Trim(), "IP New", StringComparison.OrdinalIgnoreCase))
				{
					var feeJson = await ApiClient.Get_ApiValues(
						httpClient,
						"api/OPDForm/Get_IPD_Doctor_Consultation_Fee?Peopleid=" + practitioner
						+ "&tasktype=" + Uri.EscapeDataString(resolvedTaskTypeName)
						+ "&loginUserID=" + Uri.EscapeDataString(loginUserId));

					if (string.IsNullOrWhiteSpace(feeJson))
						return "Unable to verify the IP New consultation fee. Please try again.";

					try
					{
						JToken feeToken = JToken.Parse(feeJson);
						if (feeToken.Type == JTokenType.String)
							feeToken = JToken.Parse(feeToken.Value<string>() ?? "[]");

						var totalFee = feeToken.Type == JTokenType.Array
							? feeToken.Children<JObject>().Sum(row =>
								(row["feesamount"] ?? row["FeesamountResult"] ?? row["Feesamount"])?.Value<decimal?>() ?? 0m)
							: 0m;

						if (totalFee <= 0m)
							return "An IP New consultation fee is not configured for the selected doctor. Configure the fee before booking the appointment.";
					}
					catch (Newtonsoft.Json.JsonException)
					{
						return "Unable to verify the IP New consultation fee. Please try again.";
					}
				}

				var holidayDate = apptDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
				var holidayJson = await ApiClient.Get_ApiValues(
					httpClient,
					"api/HolidayCalendar/Get_Holiday_Blocked_Dates?tenantid=" + tenantGuid
					+ "&taskname=" + Uri.EscapeDataString(resolvedTaskTypeName)
					+ "&datefrom=" + holidayDate
					+ "&dateto=" + holidayDate
					+ "&loginUserID=" + loginUserId);
				if (!string.IsNullOrWhiteSpace(holidayJson) && holidayJson.Trim().Length > 2)
				{
					try
					{
						var blockedDates = JArray.Parse(holidayJson);
						if (blockedDates.Count > 0)
						{
							var firstBlocked = blockedDates[0];
							var holidayName = (firstBlocked["holidayname"] ?? firstBlocked["HolidayName"])?.ToString();
							var allowedCount = (firstBlocked["count"] ?? firstBlocked["Count"])?.ToString();
							return apptDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
								+ " is a Holiday"
								+ (string.IsNullOrWhiteSpace(holidayName) ? "" : " (" + holidayName + ")")
								+ ". " + resolvedTaskTypeName
								+ (allowedCount == "0"
									? " has no available slots on this day (Count: 0)."
									: " is not available on this day.");
						}
					}
					catch
					{
						// If the holiday lookup response is not JSON, continue with normal appointment validation.
					}
				}

                // ── Create Appointment ─────────────────────────
                var model = new ClinicalAppointmentModel
                {
                    craftmyapp_actionmethodname =
        "Add_Clinical_Appointment",

                    tenantid =
        tenantGuid,

                    /*
                     * Keep current readable task value
                     * for your IP workflow logic.
                     *
                     * IP Screening
                     * IP New
                     * IP Rounds
                     * IP Discharge
                     */
                    tasktype =
        resolvedTaskTypeName,

                    /*
                     * IMPORTANT:
                     * ClinicalAppointment.taskname
                     * stores Task master UUID.
                     */
                    taskname =
        selectedTaskId.ToString(),

                    patient =
        patient,

                    practitioner =
        practitioner,

                    actualpractitioner =
        practitioner,

                    appointmentdate =
        apptDate,

                    durationfrom =
        durationfrom,

                    durationto =
        durationto,

                    status =
        "Scheduled",

                    origin =
        "IPD",

                    bookingid =
        string.IsNullOrWhiteSpace(bookingid)
            ? null
            : bookingid
                };

                var result = await ApiClient.Post_ApiValuesGetString(
					httpClient,
					"api/ClinicalAppointment/Add_Clinical_Appointment",
					model);

				if (result != null && result.Contains("201.1"))
				{
					var tenantName = HttpContext.Session.GetString("NalamVazhachoosedtenantname") ?? "";
				_ = SendAllotDoctorMeetingInvite(httpClient, loginUserId, patient, practitioner, apptDate, durationfrom, durationto, bookingid, tenantName);

					if (!string.IsNullOrWhiteSpace(bookingid))
					{
						try
						{
							if (string.Equals(resolvedTaskTypeName, "IP Screening", StringComparison.OrdinalIgnoreCase))
							{
								var statusModel = new IPDBookingStatusUpdateModel
								{
									IPDApplicationFormid = bookingid,
									bookingstatus = "Screening Scheduled",
									modifieduser = loginUserId
								};
								var statusJson = await ApiClient.Post_ApiValuesGetRawString(httpClient,
									"api/IPDApplicationForm/Update_IPD_Booking_Status",
									statusModel);
								var statusMsg = (statusJson ?? "").Replace("\"", "");
								if (!statusMsg.Contains("201.1"))
									_logger?.LogWarning("Create_IPD_Clinical_Appointment: IPD booking status update to Screening Scheduled failed: {Msg}", statusMsg);

								await ApiClient.Post_ApiValuesGetString<object>(httpClient,
									$"api/IPDApplicationForm/Create_Screening_Fee_Receivable?IPDApplicationFormid={bookingid}",
									new object());
							}
							else if (string.Equals(resolvedTaskTypeName, "IP Rounds", StringComparison.OrdinalIgnoreCase)
								|| string.Equals(resolvedTaskTypeName, "IP Discharge", StringComparison.OrdinalIgnoreCase))
							{
								await ApiClient.Post_ApiValuesGetString<object>(httpClient,
									"api/IPDApplicationForm/Create_IPD_Appointment_Fee_Receivable"
									+ $"?IPDApplicationFormid={bookingid}"
									+ $"&practitioner={practitioner}"
									+ $"&tasktype={Uri.EscapeDataString(resolvedTaskTypeName)}",
									new object());
							}
							else if (string.Equals(resolvedTaskTypeName, "IP New", StringComparison.OrdinalIgnoreCase))
							{
								var statusModel = new IPDBookingStatusUpdateModel
								{
									IPDApplicationFormid = bookingid,
									bookingstatus = "Consultation Scheduled",
									modifieduser = loginUserId
								};
								var statusJson = await ApiClient.Post_ApiValuesGetRawString(httpClient,
									"api/IPDApplicationForm/Update_IPD_Booking_Status", statusModel);
								if (string.IsNullOrWhiteSpace(statusJson)
									|| !statusJson.Replace("\"", "").Contains("201.1"))
								{
									_logger?.LogWarning("Create_IPD_Clinical_Appointment: IP New status update failed: {Msg}", statusJson);
									return "Appointment was booked, but the booking status could not be updated: " + statusJson;
								}

								if (createpayablereceivable)
								{
									var receivableResult = await ApiClient.Post_ApiValuesGetString<object>(httpClient,
										"api/IPDApplicationForm/Create_IPD_Appointment_Fee_Receivable"
										+ $"?IPDApplicationFormid={bookingid}"
										+ $"&practitioner={practitioner}"
										+ $"&tasktype={Uri.EscapeDataString(resolvedTaskTypeName)}",
										new object());
									if (string.IsNullOrWhiteSpace(receivableResult)
										|| !receivableResult.Replace("\"", "").Contains("201.1"))
										return "Appointment was booked and status updated, but the consultation receivable could not be created: " + receivableResult;
								}
							}
						}
						catch (Exception rxEx)
						{
							_logger?.LogError(rxEx, "Create_IPD_Clinical_Appointment: appointment fee receivable error: " + rxEx.Message);
							if (string.Equals(resolvedTaskTypeName, "IP New", StringComparison.OrdinalIgnoreCase))
								return "Appointment was booked, but its status or consultation receivable could not be updated: " + rxEx.Message;
						}
					}

					return "Success";
				}

				return result ?? "Failed";
			}
			catch (Exception ex)
			{
				_logger?.LogError(ex, ex.Message);
				return ex.Message;
			}
		}

		private async Task<Guid?> ResolvePractitionerTaskId(
			HttpClient httpClient,
			Guid practitioner,
			string requestedTaskName,
			string loginUserId)
		{
			if (practitioner == Guid.Empty || string.IsNullOrWhiteSpace(requestedTaskName))
				return null;

			var clinicalTaskInfoJson = await ApiClient.Get_ApiValues(
				httpClient,
				"api/People/getById_clinicaltaskinfo?Peopleid=" + practitioner
				+ "&loginUserID=" + Uri.EscapeDataString(loginUserId));

			if (string.IsNullOrWhiteSpace(clinicalTaskInfoJson))
				return null;

			JToken taskInfoToken;
			try
			{
				taskInfoToken = JToken.Parse(clinicalTaskInfoJson);
				while (taskInfoToken.Type == JTokenType.String)
				{
					var nestedJson = taskInfoToken.Value<string>();
					if (string.IsNullOrWhiteSpace(nestedJson))
						return null;
					taskInfoToken = JToken.Parse(nestedJson);
				}
			}
			catch (Newtonsoft.Json.JsonException)
			{
				return null;
			}

			if (!(taskInfoToken is JArray taskInfoRows))
				return null;

			foreach (var row in taskInfoRows.Children<JObject>())
			{
				var taskIdValue = (row["taskname"] ?? row["Taskname"])?.ToString();
				if (!Guid.TryParse(taskIdValue, out var candidateTaskId) || candidateTaskId == Guid.Empty)
					continue;

				var taskJson = await ApiClient.Get_ApiValues(
					httpClient,
					"api/Task/getById_Task?Taskid=" + candidateTaskId
					+ "&loginUserID=" + Uri.EscapeDataString(loginUserId));

				if (string.IsNullOrWhiteSpace(taskJson))
					continue;

				TaskModel candidateTask;
				try
				{
					candidateTask = JsonConvert.DeserializeObject<TaskModel>(taskJson);
				}
				catch (Newtonsoft.Json.JsonException)
				{
					continue;
				}

				if (candidateTask != null
					&& string.Equals(candidateTask.taskname?.Trim(), requestedTaskName.Trim(), StringComparison.OrdinalIgnoreCase))
				{
					return candidateTaskId;
				}
			}

			return null;
		}

		private async Task SendAllotDoctorMeetingInvite(
			HttpClient httpClient,
			string loginUserId,
			Guid patient,
			Guid practitioner,
			DateTime apptDate,
			string durationfrom,
			string durationto,
			string bookingid = "",
			string tenantName = "")
		{
			try
			{
				// patient = usersid (Health Seeker); practitioner = Peopleid (doctor); loginUserId = frontdesk
				var tenantId         = HttpContext.Session.GetString("NalamVazhachoosedtenantid") ?? "";
				var patientTask      = ApiClient.Get_ApiValues(httpClient, $"api/users/getById_users?usersid={patient}&loginUserID={loginUserId}");
				var doctorTask       = ApiClient.Get_ApiValues(httpClient, $"api/People/getById_People?Peopleid={practitioner}&loginUserID={loginUserId}");
				var frontdeskTask    = ApiClient.Get_ApiValues(httpClient, $"api/users/getById_users?usersid={loginUserId}&loginUserID={loginUserId}");
				var templateResponse = ApiClient.GET_ApiValuesGetRespnse(httpClient, "api/AlertTemplates/Alert_Templates_List?tenantid=" + Uri.EscapeDataString(tenantId) + "&entityname=ClinicalAppointment&entityaction=Allot_Doctor");
				await Task.WhenAll(patientTask, doctorTask, frontdeskTask, templateResponse);

				var patientUser   = string.IsNullOrWhiteSpace(patientTask.Result) ? null
					: JsonConvert.DeserializeObject<usersModel>(patientTask.Result);
				var doctorPeople  = string.IsNullOrWhiteSpace(doctorTask.Result) ? null
					: JsonConvert.DeserializeObject<PeopleModel>(doctorTask.Result);
				var frontdeskUser = string.IsNullOrWhiteSpace(frontdeskTask.Result) ? null
					: JsonConvert.DeserializeObject<usersModel>(frontdeskTask.Result);

				var patientEmail   = patientUser?.emailid?.Trim();
				var doctorEmail    = doctorPeople?.emailid?.Trim();
				var meetingLink    = doctorPeople?.screeningmeetinglink?.Trim();
				var patientName    = $"{patientUser?.firstname} {patientUser?.lastname}".Trim();
				var doctorName     = $"{doctorPeople?.firstname} {doctorPeople?.lastname}".Trim();
				var frontdeskEmail = frontdeskUser?.emailid?.Trim();

				var toList = new[] { patientEmail, doctorEmail }
					.Where(e => !string.IsNullOrWhiteSpace(e)).Distinct().ToList();
				if (!toList.Any()) return;

				if (!templateResponse.Result.IsSuccessStatusCode) return;
				var dt = await templateResponse.Result.Content.ReadAsAsync<System.Data.DataTable>();
				if (dt == null || dt.Rows.Count == 0) return;
				if (dt.Rows[0]["alerttype"].ToString() != "Email") return;

				var alertSubject = dt.Rows[0]["alertsubject"].ToString();
				var alertContentTemplate = dt.Rows[0]["alertcontent"].ToString();

				var meetingLinkHtml = !string.IsNullOrWhiteSpace(meetingLink)
					? $"<p style='margin:0 0 16px;'><strong>Screening Meeting Link:</strong> <a href='{meetingLink}' style='color:#04927B;'>{meetingLink}</a></p>"
					: "";

				var alertContent = alertContentTemplate
					.Replace("{tenantname}",      tenantName)
					.Replace("{patientname}",     patientName)
					.Replace("{doctorname}",      doctorName)
					.Replace("{appointmentdate}", apptDate.ToString("dd/MM/yyyy"))
					.Replace("{durationfrom}",    durationfrom)
					.Replace("{durationto}",      durationto)
					.Replace("~meetinglink~",     meetingLinkHtml)
					.Replace("{currentDate}",     DateTime.Now.ToString("dd MMM yyyy"));

				try
				{
					var meetingLinkTemplate = !string.IsNullOrWhiteSpace(meetingLink)
						? "<p style='margin:0 0 16px;'><strong>Screening Meeting Link:</strong> <a href='{screeningmeetinglink}' style='color:#04927B;'>{screeningmeetinglink}</a></p>"
						: "";
					var translatableTemplate = alertContentTemplate.Replace("~meetinglink~", meetingLinkTemplate);

					var openAIOptions = new OpenAIOptions();
					Configuration.GetSection("OpenAI").Bind(openAIOptions);
					if (string.IsNullOrWhiteSpace(openAIOptions.ApiKey)
						|| string.IsNullOrWhiteSpace(openAIOptions.Endpoint)
						|| string.IsNullOrWhiteSpace(openAIOptions.GptModel))
						throw new InvalidOperationException("OpenAI translation settings are incomplete.");

					var translationService = new OpenAIService(
						Microsoft.Extensions.Options.Options.Create(openAIOptions));
					var hindiTask = translationService.TranslateHtmlEmailBody(translatableTemplate, "Hindi");
					var gujaratiTask = translationService.TranslateHtmlEmailBody(translatableTemplate, "Gujarati");
					await Task.WhenAll(hindiTask, gujaratiTask);

					var hindiTemplate = hindiTask.Result;
					var gujaratiTemplate = gujaratiTask.Result;
					if (!HasPreservedEmailTemplate(translatableTemplate, hindiTemplate, @"[\u0900-\u097F]")
						|| !HasPreservedEmailTemplate(translatableTemplate, gujaratiTemplate, @"[\u0A80-\u0AFF]"))
						throw new InvalidOperationException("Translated email did not preserve its HTML or placeholders.");

					var englishSection = ReplaceAllotDoctorEmailPlaceholders(
						translatableTemplate, tenantName, patientName, doctorName, apptDate,
						durationfrom, durationto, meetingLink);
					var hindiSection = ReplaceAllotDoctorEmailPlaceholders(
						hindiTemplate, tenantName, patientName, doctorName, apptDate,
						durationfrom, durationto, meetingLink);
					var gujaratiSection = ReplaceAllotDoctorEmailPlaceholders(
						gujaratiTemplate, tenantName, patientName, doctorName, apptDate,
						durationfrom, durationto, meetingLink);

					alertContent = BuildMultilingualScreeningEmail(
						englishSection, hindiSection, gujaratiSection);
				}
				catch (Exception translationEx)
				{
					_logger?.LogWarning(
						translationEx,
						"Allot Doctor appointment email translation failed; sending the original English body.");
				}

				alertSubject = alertSubject
					.Replace("{appointmentdate}", apptDate.ToString("dd/MM/yyyy"))
					.Replace("{durationfrom}",    durationfrom);

				var toEmails = string.Join(",", toList);
				var mailBox  = await new MailSender().GetTenantMailBox(httpClient, tenantId, MailSender.ResolveApplicableService("ClinicalAppointment", "Allot_Doctor"));
				var mailer   = mailBox == null ? null : new Mailer(mailBox);
				if (mailer == null) return;
				bool mailSent = mailer.SendMail_TLS(
					toEmails:        toEmails,
					ccEmails:        frontdeskEmail,
					subject:         alertSubject,
					body:            alertContent,
					isHtml:          true,
					attachments:     null,
					handleException: false);

				var logModel = new MailLogsModel
				{
					entityname   = "ClinicalAppointment",
					entityid     = bookingid,
					mailfor      = "Allot_Doctor",
					mailto       = toEmails,
					mailsubject  = alertSubject,
					mailbody     = alertContent,
					issent       = mailSent,
					createduser  = Guid.TryParse(loginUserId, out var lguid) ? lguid : Guid.Empty,
					craftmyapp_actionmethodname = "Create_MailLog"
				};
				await ApiClient.Post_ApiValuesGetString(httpClient, "api/MailLogs/Create_MailLog", logModel);
			}
			catch (Exception ex)
			{
				_logger?.LogError(ex, "Meeting invite email failed: " + ex.Message);
			}
		}

		private static string ReplaceAllotDoctorEmailPlaceholders(
			string template,
			string tenantName,
			string patientName,
			string doctorName,
			DateTime appointmentDate,
			string durationFrom,
			string durationTo,
			string meetingLink)
		{
			return (template ?? "")
				.Replace("{tenantname}", tenantName ?? "")
				.Replace("{patientname}", patientName ?? "")
				.Replace("{doctorname}", doctorName ?? "")
				.Replace("{appointmentdate}", appointmentDate.ToString("dd/MM/yyyy"))
				.Replace("{durationfrom}", durationFrom ?? "")
				.Replace("{durationto}", durationTo ?? "")
				.Replace("{screeningmeetinglink}", meetingLink ?? "")
				.Replace("{currentDate}", DateTime.Now.ToString("dd MMM yyyy"));
		}

		private static bool HasPreservedEmailTemplate(
			string sourceTemplate,
			string translatedTemplate,
			string requiredScriptPattern)
		{
			if (string.IsNullOrWhiteSpace(translatedTemplate)
				|| !Regex.IsMatch(translatedTemplate, requiredScriptPattern))
				return false;

			var sourceTags = Regex.Matches(sourceTemplate ?? "", @"<[^>]+>")
				.Cast<Match>().Select(match => match.Value);
			var translatedTags = Regex.Matches(translatedTemplate, @"<[^>]+>")
				.Cast<Match>().Select(match => match.Value);
			if (!sourceTags.SequenceEqual(translatedTags, StringComparer.Ordinal)) return false;

			var placeholderPattern = @"\{[A-Za-z][A-Za-z0-9_]*\}";
			var sourcePlaceholders = Regex.Matches(sourceTemplate ?? "", placeholderPattern)
				.Cast<Match>().Select(match => match.Value).OrderBy(value => value);
			var translatedPlaceholders = Regex.Matches(translatedTemplate, placeholderPattern)
				.Cast<Match>().Select(match => match.Value).OrderBy(value => value);
			return sourcePlaceholders.SequenceEqual(translatedPlaceholders, StringComparer.Ordinal);
		}

		private static string BuildMultilingualScreeningEmail(
			string englishBody,
			string hindiBody,
			string gujaratiBody)
		{
			const string separator = "<hr style='border:0;border-top:1px solid #d9e2e1;margin:28px 0;' />";
			return "<div lang='en'><h2 style='margin:0 0 16px;'>English</h2>" + englishBody + "</div>"
				+ separator
				+ "<div lang='hi'><h2 style='margin:0 0 16px;'>हिन्दी</h2>" + hindiBody + "</div>"
				+ separator
				+ "<div lang='gu'><h2 style='margin:0 0 16px;'>ગુજરાતી</h2>" + gujaratiBody + "</div>";
		}

		/// <summary>
		/// Resolves which ClinicalAppointment tasktype should be used
		/// for a patient based on People_clinicaltaskinfo task names:
		/// IP new -> IP rounds -> IP screening -> IP discharge.
		/// </summary>
		[HttpGet()]
			  public virtual async Task<string> Get_IPD_TaskTypeName(Guid patient)
			  {
					try
					{
						var loginUserId = HttpContext.Session.GetString("NalamVazhaloginUserID");
						if (string.IsNullOrWhiteSpace(loginUserId))
							return "Session Expired";

						if (patient == Guid.Empty)
							return "Patient is required";

						// Fetch patient clinical taskinfo
						var clinicalTaskInfoJson = await ApiClient.Get_ApiValues(
							getHttpClient(),
							"api/People/getById_clinicaltaskinfo?Peopleid=" + patient +
							"&loginUserID=" + loginUserId);

						if (string.IsNullOrWhiteSpace(clinicalTaskInfoJson))
							return "No clinical task info found";

						JArray clinicalTaskInfo;
						try
						{
							clinicalTaskInfoJson = clinicalTaskInfoJson.Trim();
							if (clinicalTaskInfoJson.StartsWith("\"") && clinicalTaskInfoJson.EndsWith("\""))
								clinicalTaskInfoJson = clinicalTaskInfoJson.Trim('\"');
							clinicalTaskInfo = JArray.Parse(clinicalTaskInfoJson);
						}
						catch
						{
							return "Unable to parse clinical task info";
						}

						var taskNamePriority = new List<string> { "IP New", "IP Rounds", "IP Screening", "IP Discharge" };
						var allowedTaskNames = new HashSet<string>(taskNamePriority, StringComparer.OrdinalIgnoreCase);

						TaskTypeModel resolvedTaskType = null;
						string resolvedMatchedTaskName = null;
						int bestPriorityIndex = int.MaxValue;

						var taskCache = new Dictionary<string, TaskModel>(StringComparer.OrdinalIgnoreCase);
						var taskTypeCache = new Dictionary<Guid, TaskTypeModel>();

						foreach (var row in clinicalTaskInfo)
						{
							var tasknameId = row["taskname"]?.ToString() ?? row["Taskname"]?.ToString();
							if (string.IsNullOrWhiteSpace(tasknameId)) continue;

							if (!taskCache.TryGetValue(tasknameId, out var taskModel))
							{
								var taskJson = await ApiClient.Get_ApiValues(
									getHttpClient(),
									"api/Task/getById_Task?Taskid=" + tasknameId +
									"&loginUserID=" + loginUserId);

								if (string.IsNullOrWhiteSpace(taskJson)) continue;

								taskModel = JsonConvert.DeserializeObject<TaskModel>(taskJson);
								if (taskModel == null) continue;
								taskCache[tasknameId] = taskModel;
							}

							if (taskModel == null || string.IsNullOrWhiteSpace(taskModel.taskname)) continue;
							if (!allowedTaskNames.Contains(taskModel.taskname)) continue;

							var idxResolved = taskNamePriority.FindIndex(x => x.Equals(taskModel.taskname, StringComparison.OrdinalIgnoreCase));
							if (idxResolved < 0) continue;

							TaskTypeModel currentTaskType = null;
							if (taskTypeCache.TryGetValue(taskModel.tasktype, out var existing))
							{
								currentTaskType = existing;
							}
							else
							{
								var taskTypeId = taskModel.tasktype;
								if (taskTypeId == Guid.Empty) continue;

								var taskTypeJson = await ApiClient.Get_ApiValues(
									getHttpClient(),
									"api/TaskType/getById_TaskType?TaskTypeid=" + taskTypeId +
									"&loginUserID=" + loginUserId);

								if (string.IsNullOrWhiteSpace(taskTypeJson)) continue;

								var ttModel = JsonConvert.DeserializeObject<TaskTypeModel>(taskTypeJson);
								if (ttModel == null || string.IsNullOrWhiteSpace(ttModel.tasktypename))
									continue;

								taskTypeCache[taskTypeId] = ttModel;
								currentTaskType = ttModel;
							}

							if (currentTaskType != null && idxResolved < bestPriorityIndex)
							{
								resolvedTaskType = currentTaskType;
								resolvedMatchedTaskName = taskModel.taskname;
								bestPriorityIndex = idxResolved;
							}

							if (bestPriorityIndex == 0) break;
						}

						if (resolvedTaskType == null || string.IsNullOrWhiteSpace(resolvedTaskType.tasktypename))
							return "No matching clinical task found for IP new/IP rounds/IP screening/IP discharge";

						return resolvedTaskType.TaskTypeid.ToString();
					}
					catch (Exception ex)
					{
						_logger?.LogError(ex, "Get_IPD_TaskTypeName failed: " + ex.Message);
						return ex.Message;
					}
			  }
			  [HttpPost()]
			public virtual async Task<string> Add_Assessment(AssessmentModel model, IFormCollection collection)
			{
				string strReturnMessage = "";
				
				try
				{
					ModelState.Remove("Assessmentid");
					ModelState.Remove("createduser");
                    ModelState.Remove("craftmyapp_actionmethodname");
                    model.craftmyapp_actionmethodname="Add_Assessment";
					if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
								model.createduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
								else
								return "Session Expired";                    

					if (IsAssessmentDraftSaveRequested(model)
						&& model.ipdform.HasValue && model.ipdform.Value != Guid.Empty)
					{
						var bookingStatus = await GetCurrentIPDBookingStatus(
							getHttpClient(), model.ipdform.Value, model.createduser?.ToString() ?? string.Empty);
						if (!IsProvisionalConfirmedBookingStatus(bookingStatus))
							return "Save as Draft is available only for a Provisional Confirmed booking.";
					}
					
                   
					
			 	    
					 if (ModelState.IsValid)
					 {
							 AssessmentModelValidator validator = new AssessmentModelValidator();
							 ValidationResult results = validator.Validate(model);
							 if (!results.IsValid)
							 {
								 var errorCollection = string.Join(" | ", results.Errors.Select(e => e.ErrorMessage.Replace("{propertyName}",e.PropertyName)));
								 strReturnMessage = errorCollection.ToString();
								 foreach (var failure in results.Errors)
								 {
									ModelState.AddModelError(failure.PropertyName, failure.ErrorMessage);
								 }
							 }
							 else
							 {
					 // If Assessmentid is pre-populated (completedraft / interndoctorupdate mode), update instead of insert
					 if (model.Assessmentid != null && model.Assessmentid != Guid.Empty)
					 {
						 model.modifieduser = new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
						 ApplyAssessmentHistoryContext(model, false);
						 model.craftmyapp_actionmethodname = "Update_Assessment";
						 strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(), "api/Assessment/Update_Assessment", model);
						 goto assessmentSaveResult;
					 }
					 if (model.opdform != null && model.opdform != Guid.Empty)
					 {
						 var existingId = await ApiClient.Get_ApiValues(getHttpClient(),
							 "api/Assessment/getAssessmentByOpdForm?opdformid=" + model.opdform +
							 "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
						 existingId = existingId?.Trim('"') ?? "";
						 if (!string.IsNullOrEmpty(existingId) && Guid.TryParse(existingId, out Guid existingGuid))
						 {
							 model.Assessmentid = existingGuid;
							 model.modifieduser = new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
							 ApplyAssessmentHistoryContext(model, false);
							 model.craftmyapp_actionmethodname = "Update_Assessment";
							 strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(), "api/Assessment/Update_Assessment", model);
							 goto assessmentSaveResult;
						 }
					 }
					 model.Assessmentid = Guid.NewGuid();
					 ApplyAssessmentHistoryContext(model, true);
                                 strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/Assessment/Add_Assessment", model);
					 assessmentSaveResult:;
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
                 
                 _logger.LogError(ex,"An exception occurred in - Assessment / Add_Assessment, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
               
				 strReturnMessage = ex.Message;
			 }
		     ViewData["message"] = strReturnMessage;
			 if(strReturnMessage.Replace("\"", "").Contains("201.1")){
				 await SavePatientAnswersIfPresent(model);
				 var draftSubmissionMessage = await SubmitCompletedIPDAssessmentDraft(model, model.createduser);
				 if (!draftSubmissionMessage.Contains("201.1", StringComparison.OrdinalIgnoreCase))
				 {
					 TempData["message"] = draftSubmissionMessage;
					 return draftSubmissionMessage;
				 }
				 await SendSeniorOPDAssessmentReviewEmail(model.opdform);
				 var reviewStatusMessage = await CompleteIPDAssessmentReviewIfPending(model, model.createduser);
				 if (!reviewStatusMessage.Contains("201.1", StringComparison.OrdinalIgnoreCase))
				 {
					 TempData["message"] = reviewStatusMessage;
					 return reviewStatusMessage;
				 }
				 var directAdmissionMessage = await MarkIPDAdmittedByDoctorIfRequested(model, collection, model.createduser);
				 if (!directAdmissionMessage.Replace("\"", "").Contains("201.1"))
				 {
					 TempData["message"] = directAdmissionMessage;
					 return directAdmissionMessage;
				 }
				 TempData["message"] = "Success";
				return "Success";
			 }
              else if (strReturnMessage.StartsWith("BadRequest", StringComparison.OrdinalIgnoreCase))
			{
				strReturnMessage= strReturnMessage.Replace("\"", "").Replace("BadRequest :","");
				TempData["message"] = strReturnMessage;

				return strReturnMessage;
			}
             else{
				  if(strReturnMessage=="401.1")
				  	 	 strReturnMessage = "Authorization Failed";

				  return strReturnMessage;
			 }
 
		   }

				
			  [HttpGet]
			  public virtual async Task<string> GetAssessmentIdByOPDForm(string opdformid)
			  {
				  if (!Guid.TryParse(opdformid, out _))
					  return string.Empty;

				  var result = await ApiClient.Get_ApiValues(
					  getHttpClient(),
					  "api/Assessment/getAssessmentByOpdForm?opdformid=" + opdformid
					  + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));

				  return result?.Trim('"') ?? string.Empty;
			  }

			  public virtual async Task<IActionResult> Update_Assessment(string Assessmentid)
			  {

                    string redirectTo="";
                    if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
                            DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
                            DataView dv = new DataView(NalamVazharole_JSON);
                            dv.RowFilter = "controllername='Assessment' AND viewname='list'";

                            if(dv.Count  >0){
                                redirectTo = dv[0]["actionmethodname"] as string;
							 
                            }

                            try{
                                     var jsonObjAssessment = await ApiClient.Get_ApiValues(getHttpClient(), "api/Assessment/getById_Assessment?Assessmentid="+Assessmentid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                                if(jsonObjAssessment.Length > 2)
                                {
                                  
                                    var model = JsonConvert.DeserializeObject<AssessmentModel>(jsonObjAssessment);


                
									await PrepareAssessmentOpdReference(model);
									await PrepareAssessmentTemplateOptions(model);
									ViewBag.AssessmentIpdBookingStatus = model.ipdform.HasValue && model.ipdform.Value != Guid.Empty
										? await GetCurrentIPDBookingStatus(
											getHttpClient(),
											model.ipdform.Value,
											HttpContext.Session.GetString("NalamVazhaloginUserID") ?? string.Empty)
										: string.Empty;
									ViewBag.AllowDoctorToAdmitPatients = await GetAllowDoctorToAdmitPatients(model.tenantid);
                                     
                                    return View("Add_Assessment", model);
                                }
                                else
                                {
                    
                                    TempData["message"] = "Data Not Found - Contact Administrator";
                                    return RedirectToAction(redirectTo);
						 
                                }

                            }catch(Exception ex){
                               _logger.LogError(ex,"An exception occurred in - Assessment / Update_Assessment, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                                TempData["errMessage"] = "Error while fetching data - Contact Administrator";
                                return RedirectToAction(redirectTo);
                            }

                    }
                    TempData["errMessage"] = "Session Expired";
                    return RedirectToAction("Logout", "users");
                }	
			  [HttpPost()]
				public virtual async Task<string> Update_Assessment(AssessmentModel model, IFormCollection collection)
				{
					string strReturnMessage = "";
					try
					{
							ModelState.Remove("Assessmentid");
                            ModelState.Remove("craftmyapp_actionmethodname");
                             model.craftmyapp_actionmethodname="Update_Assessment";
							
							
							if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
					model.modifieduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
					else
					return "Session Expired";

				if (IsAssessmentDraftSaveRequested(model)
					&& model.ipdform.HasValue && model.ipdform.Value != Guid.Empty)
				{
					var bookingStatus = await GetCurrentIPDBookingStatus(
						getHttpClient(), model.ipdform.Value, model.modifieduser?.ToString() ?? string.Empty);
					if (!IsProvisionalConfirmedBookingStatus(bookingStatus))
						return "Save as Draft is available only for a Provisional Confirmed booking.";
				}

				var client = getHttpClient();
				var isAdmissionDecisionRequest = string.Equals(model.eligibleforfinaladmission, "screeningapproved", StringComparison.OrdinalIgnoreCase)
					|| string.Equals(model.eligibleforfinaladmission, "screeningrejected", StringComparison.OrdinalIgnoreCase);
				if (isAdmissionDecisionRequest)
				{
					if (!model.ipdform.HasValue || model.ipdform.Value == Guid.Empty)
						return "IPD Application Form is required for an admission decision.";

					var currentBookingStatus = await GetCurrentIPDBookingStatus(
						client,
						model.ipdform.Value,
						model.modifieduser?.ToString() ?? "");

					if (IsAssessmentReviewPendingStatus(currentBookingStatus))
					{
						// Older review links labelled this action as Screening and submitted
						// screeningapproved. Treat it as review completion; the assessment SQL
						// advances the booking to Assessment Form Reviewed.
						model.eligibleforfinaladmission = "approved";
					}
					else if (!IsAdmissionDecisionStage(currentBookingStatus))
					{
						return "Admission approval or rejection is allowed only when screening is scheduled.";
					}
					else
					{
						IPDBookingStatusUpdateModel model_StatusUpdate = new IPDBookingStatusUpdateModel();
						model_StatusUpdate.IPDApplicationFormid = model.ipdform.ToString();
						model_StatusUpdate.bookingstatus = string.Equals(model.eligibleforfinaladmission, "screeningrejected", StringComparison.OrdinalIgnoreCase)
							? "Rejected"
							: "Admission Approved";
						model_StatusUpdate.modifieduser = model.modifieduser.ToString();

						var json = await ApiClient.Post_ApiValuesGetRawString(client,
										"api/IPDApplicationForm/Update_IPD_Booking_Status", model_StatusUpdate);
						var msg = (json ?? "").Replace("\"", "");
						var ok = msg.Contains("201.1");
						string appointmentMsg = null;
						if (ok)
						{
							var completeBody = new CompleteIPScreeningForIPDRequestModel { IPDApplicationFormid = model.ipdform.ToString() };
							var apptJson = await ApiClient.Post_ApiValuesGetRawString(client,
								"api/OPDForm/Complete_IP_Screening_For_IPD_Booking", completeBody);
							appointmentMsg = (apptJson ?? "").Replace("\"", "");
							if (!appointmentMsg.Contains("201.1"))
								_logger.LogWarning("Mark_IPD_Admission_Confirmed: booking status updated but IP Screening appointment completion failed: {Msg}", appointmentMsg);
						}
					}
				}
				if (ModelState.IsValid)
							{
									AssessmentModelValidator validator = new AssessmentModelValidator();
									ValidationResult results = validator.Validate(model);
									if (!results.IsValid)
									{
										var errorCollection = string.Join(" | ", results.Errors.Select(e => e.ErrorMessage.Replace("{propertyName}",e.PropertyName)));
										strReturnMessage = errorCollection.ToString();
										foreach (var failure in results.Errors)
										{
											ModelState.AddModelError(failure.PropertyName, failure.ErrorMessage);
										}
									}
									else
									{
                                        
										
                                        
                                        
										ApplyAssessmentHistoryContext(model, false);
                                        strReturnMessage = await ApiClient.Post_ApiValuesGetString(client,"api/Assessment/Update_Assessment", model);
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
                      _logger.LogError(ex,"An exception occurred in - Assessment / Update_Assessment, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
						strReturnMessage = ex.Message;
					}
					ViewData["message"] = strReturnMessage;
						    if(strReturnMessage.Replace("\"", "")=="201.1"){
							await SavePatientAnswersIfPresent(model);
							var draftSubmissionMessage = await SubmitCompletedIPDAssessmentDraft(model, model.modifieduser);
							if (!draftSubmissionMessage.Contains("201.1", StringComparison.OrdinalIgnoreCase))
							{
								TempData["message"] = draftSubmissionMessage;
								return draftSubmissionMessage;
							}
							await SendSeniorOPDAssessmentReviewEmail(model.opdform);
							var reviewStatusMessage = await CompleteIPDAssessmentReviewIfPending(model, model.modifieduser);
							if (!reviewStatusMessage.Contains("201.1", StringComparison.OrdinalIgnoreCase))
							{
								TempData["message"] = reviewStatusMessage;
								return reviewStatusMessage;
							}
							await CompleteReviewedIPDAppointment(model, collection);
							var directAdmissionMessage = await MarkIPDAdmittedByDoctorIfRequested(model, collection, model.modifieduser);
							if (!directAdmissionMessage.Replace("\"", "").Contains("201.1"))
							{
								TempData["message"] = directAdmissionMessage;
								return directAdmissionMessage;
							}
							TempData["message"] = "Success";
							return "Success";
						}
                        else if (strReturnMessage.StartsWith("BadRequest", StringComparison.OrdinalIgnoreCase))
			{
				strReturnMessage= strReturnMessage.Replace("\"", "").Replace("BadRequest :","");
				TempData["message"] = strReturnMessage;

				return strReturnMessage;
			}
                         else{
							if(strReturnMessage=="401.1")
									strReturnMessage = "Authorization Failed";

							return strReturnMessage;
						}
		
				}

		private async Task CompleteReviewedIPDAppointment(AssessmentModel model, IFormCollection collection)
		{
			if (!model.ipdform.HasValue || collection == null)
				return;

			try
			{
			var appointmentIdText = collection["clinicalappointmentid"].FirstOrDefault();
			if (!Guid.TryParse(appointmentIdText, out var appointmentId))
				return;

			var loginUserId = HttpContext.Session.GetString("NalamVazhaloginUserID");
			var appointmentJson = await ApiClient.Get_ApiValues(
				getHttpClient(),
				"api/ClinicalAppointment/getById_ClinicalAppointment?ClinicalAppointmentid="
				+ appointmentId + "&loginUserID=" + loginUserId);
			var appointment = JsonConvert.DeserializeObject<ClinicalAppointmentModel>(appointmentJson ?? string.Empty);

			var peopleJson = await ApiClient.Get_ApiValues(
				getHttpClient(), "api/People/GetPeopleIdByUserId?usersid=" + loginUserId);
			var loggedInPeopleId = JObject.Parse(peopleJson ?? "{}")["peopleid"]?.ToString();
			var appointmentTaskName = (appointment?.tasktype ?? string.Empty).Trim();
			var isReviewTask = appointmentTaskName.Equals("IP Rounds", StringComparison.OrdinalIgnoreCase)
				|| appointmentTaskName.Equals("IP Discharge", StringComparison.OrdinalIgnoreCase);

			if (appointment == null
				|| !isReviewTask
				|| !string.Equals(appointment.origin, "IPD", StringComparison.OrdinalIgnoreCase)
				|| !string.Equals(appointment.bookingid, model.ipdform.Value.ToString(), StringComparison.OrdinalIgnoreCase)
				|| !string.Equals(appointment.practitioner?.ToString(), loggedInPeopleId, StringComparison.OrdinalIgnoreCase))
			{
				_logger.LogWarning(
					"Rejected assessment appointment completion because appointment {AppointmentId} is not an assigned IP Rounds/IP Discharge appointment.",
					appointmentId);
				return;
			}

			var payload = new { ClinicalAppointmentid = appointmentId, status = "Completed" };
			var result = await ApiClient.Post_ApiValuesGetString(
				getHttpClient(), "api/OPDForm/Update_Appointment_Status", payload);

			if (string.IsNullOrWhiteSpace(result)
				|| (!result.Contains("201.1") && !result.Contains("\"success\":true", StringComparison.OrdinalIgnoreCase)))
			{
				_logger.LogWarning(
					"Assessment saved but IPD clinical appointment {AppointmentId} was not completed. Response: {Response}",
					appointmentId, result);
			}
			}
			catch (Exception ex)
			{
				// The assessment is already saved at this point; a follow-up status
				// failure must not turn that successful save into an error response.
				_logger.LogError(ex, "Unable to complete the reviewed IPD clinical appointment.");
			}
		}
public virtual async Task<IActionResult> Remove_Assessment(string Assessmentid)
			{
				string message = "";
				try
				{
						message = await ApiClient.Get_ApiValues(getHttpClient(), "api/Assessment/Remove_Assessment?Assessmentid="+Assessmentid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
						 if(message.Replace("\"","").Contains("201.1"))
						{
							TempData["message"] = "Success";

						}else{
							TempData["errMessage"] = message.Replace("\"","");
						}
						
				
				
				}
				catch (Exception ex)
				{
                     _logger.LogError(ex,"An exception occurred in - Assessment / Remove_Assessment, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                
					 TempData["errMessage"] = ex.Message;
					 message = ex.Message;
				}

				string redirectTo="";
						if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
					DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
						 DataView dv = new DataView(NalamVazharole_JSON);
						 dv.RowFilter = "controllername='Assessment' AND viewname='list'";

						if(dv.Count  >0){
						    redirectTo = dv[0]["actionmethodname"] as string;
							 
						}

					}
				
				return RedirectToAction(redirectTo);
			}

                                        public virtual async Task<IActionResult> View_Assessment(string Assessmentid)
                                        {
                                            if (string.IsNullOrEmpty(Assessmentid))
                                                return View();
                                            try
                                            {
                                                var json = await ApiClient.Get_ApiValues(getHttpClient(), "api/Assessment/getById_Assessment?Assessmentid=" + Assessmentid + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
                                                if (json.Length > 2)
                                                {
                                                    var model = JsonConvert.DeserializeObject<AssessmentModel>(json);
                                                    return View(model);
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                _logger.LogError(ex, "An exception occurred in - Assessment / View_Assessment, Error Message : " + ex.Message);
                                            }
                                            return View();
                                        }

			        public virtual IActionResult Assessment_List()
			        {
				        return View();
			        }
			        	
			        [HttpGet()]
			        public virtual async Task<string> get_Assessment_List(string tenantid
,string patientname
,string patientvisit
,string assessmentdate_automatonfrom
,string assessmentdate_automatonto
, int? pagesize=100 , int? pagenumber=0,string searchterm="",string sortFieldsJson="")
			        {
				        
				        return await ApiClient.Get_ApiValues(getHttpClient(), "api/Assessment/Assessment_List?tenantid="+tenantid+"&patientname="+patientname+"&patientvisit="+patientvisit+"&assessmentdate_automatonfrom="+assessmentdate_automatonfrom+"&assessmentdate_automatonto="+assessmentdate_automatonto+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID")
+ "&pagesize=" + pagesize + "&pagenumber="+ pagenumber + "&searchterm="+ searchterm + "&sort_fields=" + sortFieldsJson);
			        }

			        [HttpGet()]
			        public virtual async Task<string> get_Patient_Profile_Assessments(string tenantid, string patientname)
			        {
				        return await ApiClient.Get_ApiValues(getHttpClient(), "api/Assessment/Patient_Profile_Assessments?tenantid="+tenantid+"&patientname="+patientname+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			        }
			          
											[HttpGet()]
											public virtual async Task<string> get_all_PatientProfile(string tenantid,string searchterm, string pagesize="1000", string pagenumber="1")
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/PatientProfile/get_all_PatientProfile?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											 
											[HttpGet()]
											public virtual async Task<string> get_all_PatientVisit(string tenantid,string searchterm, string pagesize="1000", string pagenumber="1")
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/PatientVisit/get_all_PatientVisit?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											 
											[HttpGet()]
											public virtual async Task<string> get_all_People(string tenantid,string searchterm, string pagesize="1000", string pagenumber="1")
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/People/get_all_People?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											 
											[HttpGet()]
											public virtual async Task<string> get_all_IPDApplicationForm(string tenantid,string searchterm, string pagesize="1000", string pagenumber="1")
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/IPDApplicationForm/get_all_IPDApplicationForm?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											 
											[HttpGet()]
											public virtual async Task<string> get_all_OPDForm(string tenantid,string searchterm, string pagesize="1000", string pagenumber="1")
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/OPDForm/get_all_OPDForm?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											 
											[HttpGet()]
											public virtual async Task<string> get_all_AssessmentTemplate(string tenantid)
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/AssessmentTemplate/get_all_AssessmentTemplate?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											

				
			  public virtual async Task<string> getById_allinfo_Assessment(string Assessmentid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/Assessment/getById_allinfo_Assessment?Assessmentid="+Assessmentid);
					 
			  }
[HttpGet()]
                    public virtual async Task<string> lookup_Assessment_patientname(String tenantid,string searchterm, int? pagesize, int? pagenumber)
                    {
                        
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/Assessment/lookup_Assessment_patientname?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }
[HttpGet()]
                    public virtual async Task<string> lookup_Assessment_patientvisit(String tenantid,String patientname,string searchterm, int? pagesize, int? pagenumber)
                    {
                        
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/Assessment/lookup_Assessment_patientvisit?tenantid="+tenantid+"&patientname="+patientname+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }
[HttpGet()]
                    public virtual async Task<string> lookup_Assessment_doctorname(String tenantid,string searchterm, int? pagesize, int? pagenumber)
                    {
                        
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/Assessment/lookup_Assessment_doctorname?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }
[HttpGet()]
                    public virtual async Task<string> lookup_Assessment_ipdform(String tenantid,String patientname,string searchterm, int? pagesize, int? pagenumber)
                    {
                        
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/Assessment/lookup_Assessment_ipdform?tenantid="+tenantid+"&patientname="+patientname+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }
[HttpGet()]
                    public virtual async Task<string> lookup_Assessment_opdform(String tenantid,String patientname,String doctorname,string searchterm, int? pagesize, int? pagenumber)
                    {
                        
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/Assessment/lookup_Assessment_opdform?tenantid="+tenantid+"&patientname="+patientname+"&doctorname="+doctorname+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }
[HttpGet()]
			    public virtual async Task<string> lookup_Assessment_questionnairetemplate(String tenantid)
			    {

				    return await ApiClient.Get_ApiValues(getHttpClient(), "api/Assessment/lookup_Assessment_questionnairetemplate?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			    }

[HttpGet()]
			    public virtual async Task<string> get_Assessment_suggested_templates(String ipdapplicationformid, String userrole = "", String taskname = "")
			    {
                    if (string.IsNullOrWhiteSpace(userrole))
                        userrole = HttpContext.Session.GetString("NalamVazhauserrole") ?? "";
				    return await ApiClient.Get_ApiValues(getHttpClient(), "api/Assessment/get_Assessment_suggested_templates?ipdapplicationformid="+ipdapplicationformid+"&userrole="+System.Net.WebUtility.UrlEncode(userrole)+"&taskname="+System.Net.WebUtility.UrlEncode(taskname ?? "")+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			    }

[HttpGet()]
			    public virtual async Task<string> get_opd_assessmenttemplates(String opdformid, String taskname = "")
			    {
				    return await ApiClient.Get_ApiValues(getHttpClient(), "api/Assessment/get_opd_assessmenttemplates?opdformid="+opdformid+"&taskname="+System.Net.WebUtility.UrlEncode(taskname)+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			    }

[HttpGet()]
                        public virtual async Task<string> lookup_Assessment_assessmentquestions_questions(String tenantid)
                        {
                            return await ApiClient.Get_ApiValues(getHttpClient(), "api/Assessment/lookup_Assessment_assessmentquestions_questions?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                        }
[HttpGet()]
                        public virtual async Task<string> lookup_Assessment_assessmentquestions_questioncategory(String tenantid)
                        {
                            return await ApiClient.Get_ApiValues(getHttpClient(), "api/Assessment/lookup_Assessment_assessmentquestions_questioncategory?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                        }
[HttpGet()]
                        public virtual async Task<string> lookup_Assessment_assessmentquestions_questionsub(String tenantid,String questioncategory)
                        {
                            return await ApiClient.Get_ApiValues(getHttpClient(), "api/Assessment/lookup_Assessment_assessmentquestions_questionsub?tenantid="+tenantid+"&questioncategory="+questioncategory+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                        }
[HttpGet()]
                        public virtual async Task<string> lookup_Assessment_assessmentquestions_question(String tenantid,String questioncategory,String questionsub)
                        {
                            return await ApiClient.Get_ApiValues(getHttpClient(), "api/Assessment/lookup_Assessment_assessmentquestions_question?tenantid="+tenantid+"&questioncategory="+questioncategory+"&questionsub="+questionsub+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                        }

[HttpGet()]
			    public virtual async Task<string> lookup_Assessment_question(String tenantid,String questionsubcategory,String questioncategory)
			    {
				    return await ApiClient.Get_ApiValues(getHttpClient(), "api/Assessment/lookup_Assessment_question?tenantid="+tenantid+"&questionsubcategory="+questionsubcategory+"&questioncategory="+questioncategory+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			    } 
											[HttpGet()]
											public virtual async Task<string> get_all_AssessmentQuestion(string tenantid)
											{
											 
											    return await ApiClient.Get_ApiValues(getHttpClient(), "api/AssessmentQuestion/get_all_AssessmentQuestion?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											
[HttpGet()]
			    public virtual async Task<string> lookup_Assessment_questionsub(String tenantid,String questioncategoryname)
			    {
				    return await ApiClient.Get_ApiValues(getHttpClient(), "api/Assessment/lookup_Assessment_questionsub?tenantid="+tenantid+"&questioncategoryname="+questioncategoryname+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			    } 
											[HttpGet()]
											public virtual async Task<string> get_all_QuestionSubCategory(string tenantid)
											{
											 
											    return await ApiClient.Get_ApiValues(getHttpClient(), "api/QuestionSubCategory/get_all_QuestionSubCategory?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											
 
											[HttpGet()]
											public virtual async Task<string> get_all_QuestionCategory(string tenantid)
											{
											 
											    return await ApiClient.Get_ApiValues(getHttpClient(), "api/QuestionCategory/get_all_QuestionCategory?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											


[HttpGet()]
			public virtual async Task<string> lookup_change_assessmentquestions_Assessment_question(string AssessmentQuestionid)
			{
				return await ApiClient.Get_ApiValues(getHttpClient(), "api/Assessment/lookup_change_assessmentquestions_Assessment_question?AssessmentQuestionid="+AssessmentQuestionid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			}


                    
                     
                        

				}


			}
