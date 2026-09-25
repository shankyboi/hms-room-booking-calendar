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
				using System.Text;
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
				using iText.Html2pdf;
				using Newtonsoft.Json.Linq;
    using DinkToPdf.Contracts;
    using DinkToPdf;
    using Microsoft.AspNetCore.Mvc.Razor;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using NalamVazha.Models.Template;

  

    //This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:02





    public class IPDApplicationFormController : BaseController
				{	 
					private const string FeedbackTaskType = "Feedbackform";
					private const string DischargeChecklistTaskType = "Dischargechecklist";

					private static string NormalizeTaskName(string taskName)
					{
						return new string((taskName ?? "").Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant).ToArray());
					}

					private static bool IsDischargeChecklistTask(string taskName)
					{
						return NormalizeTaskName(taskName).Contains(NormalizeTaskName(DischargeChecklistTaskType));
					}

					private static bool IsCompletedDischargeAssessment(string taskName, string eligibility)
					{
						if (string.Equals(taskName?.Trim(), FeedbackTaskType, StringComparison.OrdinalIgnoreCase))
							return string.Equals(eligibility?.Trim(), "patient", StringComparison.OrdinalIgnoreCase);

						if (IsDischargeChecklistTask(taskName))
							return string.Equals(eligibility?.Trim(), "approved", StringComparison.OrdinalIgnoreCase);

						return false;
					}

					private sealed class DischargeWorkflowState
					{
						public bool FeedbackSubmitted { get; set; }
						public bool ChecklistCompleted { get; set; }
						public bool HasChecklistTemplate { get; set; }
						public string ChecklistAssessmentId { get; set; } = "";
					}
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
                    private readonly ILogger<IPDApplicationFormController> _logger;
        private readonly IConverter _converter;
        private readonly IRazorViewEngine _razorViewEngine;
        StorageUtil util;
					public IPDApplicationFormController(IConfiguration configuration,IHttpContextAccessor accessor,IOptions<ApiSettings> ApiSettings, IOptions<MailSettings> MailSettings, IWebHostEnvironment env, ILogger<IPDApplicationFormController> logger, IRazorViewEngine razorViewEngine, IConverter converter) :base( configuration)
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
            _razorViewEngine = razorViewEngine;
            _accessor = accessor;
            _converter = converter;
            Configuration = configuration;
                        util = new StorageUtil(configuration);
					}

					private async Task<string> ResolveAssessmentTaskName(JToken row, string loginUserId)
					{
						var taskName = (row["taskname"] ?? row["TaskName"])?.ToString()?.Trim();
						var templateId = (row["questionnairetemplate"] ?? row["QuestionnaireTemplate"])?.ToString();
						if (!Guid.TryParse(templateId, out _)) return taskName ?? "";
						var json = await ApiClient.Get_ApiValues(getHttpClient(),
							"api/AssessmentTemplate/getById_AssessmentTemplate?AssessmentTemplateid=" + templateId +
							"&loginUserID=" + loginUserId);
						try
						{
							var template = JObject.Parse(json);
							return (template["taskname"] ?? template["TaskName"])?.ToString()?.Trim() ?? taskName ?? "";
						}
						catch (Newtonsoft.Json.JsonException) { return taskName ?? ""; }
					}

					private async Task<DischargeWorkflowState> GetDischargeWorkflowState(string ipdFormId, string loginUserId)
					{
						var state = new DischargeWorkflowState();
						var ipdJson = await ApiClient.Get_ApiValues(getHttpClient(),
							"api/IPDApplicationForm/getById_IPDApplicationForm?IPDApplicationFormid=" + ipdFormId +
							"&loginUserID=" + loginUserId);
						if (string.IsNullOrWhiteSpace(ipdJson) || ipdJson.Length <= 2) return state;
						var ipd = JsonConvert.DeserializeObject<IPDApplicationFormModel>(ipdJson);
						if (ipd == null || ipd.patientname == Guid.Empty) return state;

						// Read taskname directly from Assessment. The generic Assessment_List
						// projection does not reliably include it, which left completed feedback
						// showing as "Awaiting Health Seeker Feedback".
						var taskStateJson = await ApiClient.Get_ApiValues(getHttpClient(),
							"api/Assessment/Get_IPD_Assessment_Task_State?ipdapplicationformid=" + ipdFormId +
							"&loginUserID=" + loginUserId);
						try
						{
							var parsedTaskState = JToken.Parse(string.IsNullOrWhiteSpace(taskStateJson) ? "[]" : taskStateJson);
							var taskRows = parsedTaskState as JArray ?? parsedTaskState["detail"] as JArray ?? parsedTaskState["data"] as JArray ?? new JArray();
							foreach (var taskRow in taskRows)
							{
								var persistedTask = (taskRow["taskname"] ?? taskRow["TaskName"])?.ToString()?.Trim();
								if (string.Equals(persistedTask, FeedbackTaskType, StringComparison.OrdinalIgnoreCase))
									state.FeedbackSubmitted = true;
								else if (IsDischargeChecklistTask(persistedTask))
								{
									state.ChecklistAssessmentId = (taskRow["assessmentid"] ?? taskRow["Assessmentid"])?.ToString() ?? "";
									state.ChecklistCompleted = true;
								}
							}
						}
						catch (Newtonsoft.Json.JsonException) { }

						var assessmentsJson = await ApiClient.Get_ApiValues(getHttpClient(),
							"api/Assessment/Assessment_List?tenantid=" + (ipd.tenantid?.ToString() ?? "") +
							"&patientname=" + ipd.patientname +
							"&patientvisit=&assessmentdate_automatonfrom=&assessmentdate_automatonto=" +
							"&pagesize=500&pagenumber=0&searchterm=&loginUserID=" + loginUserId);
						try
						{
							var parsed = JToken.Parse(string.IsNullOrWhiteSpace(assessmentsJson) ? "[]" : assessmentsJson);
							var rows = parsed as JArray ?? parsed["detail"] as JArray ?? parsed["data"] as JArray ?? new JArray();
							foreach (var row in rows)
							{
								var rowIpd = (row["ipdform"] ?? row["IPDForm"])?.ToString();
								if (!string.Equals(rowIpd, ipdFormId, StringComparison.OrdinalIgnoreCase)) continue;
								var taskName = await ResolveAssessmentTaskName(row, loginUserId);
								var eligibility = (row["eligibleforfinaladmission"] ?? row["Eligibleforfinaladmission"])?.ToString();
								if (string.Equals(taskName, FeedbackTaskType, StringComparison.OrdinalIgnoreCase)
									&& IsCompletedDischargeAssessment(taskName, eligibility))
									state.FeedbackSubmitted = true;
								if (IsDischargeChecklistTask(taskName))
								{
									state.ChecklistAssessmentId = (row["Assessmentid"] ?? row["assessmentid"])?.ToString() ?? "";
									state.ChecklistCompleted = state.ChecklistCompleted
										|| IsCompletedDischargeAssessment(taskName, eligibility);
								}
							}
						}
						catch (Newtonsoft.Json.JsonException) { }

						var templatesJson = await ApiClient.Get_ApiValues(getHttpClient(),
							"api/Assessment/get_Assessment_suggested_templates?ipdapplicationformid=" + ipdFormId +
							"&userrole=" + Uri.EscapeDataString("Frontdesk Admin") +
							"&taskname=" + Uri.EscapeDataString(DischargeChecklistTaskType) +
							"&loginUserID=" + loginUserId);
						try
						{
							var templates = JToken.Parse(string.IsNullOrWhiteSpace(templatesJson) ? "[]" : templatesJson);
							state.HasChecklistTemplate = (templates as JArray ?? templates["detail"] as JArray ?? templates["data"] as JArray)?.Count > 0;
						}
						catch (Newtonsoft.Json.JsonException) { }
						return state;
					}
						 
 
                     public virtual IActionResult audit()
			         {
					        return View();
			         }
	              

					
				
			  public virtual async Task<string> getById_preferreddatesofadmission(string IPDApplicationFormid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/IPDApplicationForm/getById_preferreddatesofadmission?IPDApplicationFormid="+IPDApplicationFormid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
					 
			  }

			  public virtual async Task<string> getById_attendantpreferreddates(string IPDApplicationFormid)
			  {
				return await ApiClient.Get_ApiValues(getHttpClient(), "api/IPDApplicationForm/getById_attendantpreferreddates?IPDApplicationFormid="+IPDApplicationFormid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			  }

			  public virtual async Task<string> getById_attendantroompreference(string IPDApplicationFormid)
			  {
				return await ApiClient.Get_ApiValues(getHttpClient(), "api/IPDApplicationForm/getById_attendantroompreference?IPDApplicationFormid="+IPDApplicationFormid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			  }


				
			  public virtual async Task<string> getById_medicalinfo(string IPDApplicationFormid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/IPDApplicationForm/getById_medicalinfo?IPDApplicationFormid="+IPDApplicationFormid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
					 
			  }


				
			  public virtual async Task<string> getById_medicationinfo(string IPDApplicationFormid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/IPDApplicationForm/getById_medicationinfo?IPDApplicationFormid="+IPDApplicationFormid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
					 
			  }


				
			  public virtual async Task<string> getById_medicalrecords(string IPDApplicationFormid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/IPDApplicationForm/getById_medicalrecords?IPDApplicationFormid="+IPDApplicationFormid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
					 
			  }


				
			  public virtual async Task<string> getById_attendantinfo(string IPDApplicationFormid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/IPDApplicationForm/getById_attendantinfo?IPDApplicationFormid="+IPDApplicationFormid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
					 
			  }


				
			  public virtual async Task<string> getById_roompreference(string IPDApplicationFormid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/IPDApplicationForm/getById_roompreference?IPDApplicationFormid="+IPDApplicationFormid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
					 
			  }


				
			  public virtual async Task<string> getById_room(string IPDApplicationFormid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/IPDApplicationForm/getById_room?IPDApplicationFormid="+IPDApplicationFormid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
					 
			  }


			  public virtual IActionResult Add_IPD_Application_Form()
			  {
					return View();
			  }

		[HttpGet]
		public virtual async Task<string> Get_Booking_Emergency_Contacts(string IPDApplicationFormid)
		{
			return await ApiClient.Get_ApiValues(getHttpClient(), "api/IPDApplicationForm/Get_Booking_Emergency_Contacts?IPDApplicationFormid=" + IPDApplicationFormid);
		}
			  [HttpPost()]
			public virtual async Task<string> Add_IPD_Application_Form(IPDApplicationFormModel model, IFormCollection collection)
			{
				string strReturnMessage = "";
				
				try
				{
					ModelState.Remove("IPDApplicationFormid");
					ModelState.Remove("createduser");
                    ModelState.Remove("craftmyapp_actionmethodname");
                    model.craftmyapp_actionmethodname="Add_IPD_Application_Form";
					if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
								model.createduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
								else
								return "Session Expired";                    
                    var userRole = HttpContext.Session.GetString("NalamVazhauserrole");
                    if (string.Equals(userRole, "Frontdesk Admin", StringComparison.OrdinalIgnoreCase))
                        model.verifiedstatus = "Direct Admission";
					ModelState.Remove("uploadpassportcopy");
ModelState.Remove("uploadvisacopy");
ModelState.Remove("consentfile");
                    var isDraft = string.Equals(model.bookingstatus, "Draft", StringComparison.OrdinalIgnoreCase);
                    if (isDraft)
                        ModelState.Clear();

                   
					
			 	    
					 if (ModelState.IsValid)
					 {
							 IPDApplicationFormModelValidator validator = new IPDApplicationFormModelValidator();
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
								 model.IPDApplicationFormid =Guid.NewGuid(); 
                                 model.uploadpassportcopy = collection["uploadpassportcopy_existing"];
model.uploadvisacopy = collection["uploadvisacopy_existing"];
model.consentfile = collection["consentfile_existing"];

                                  var files = Request.Form.Files;
foreach (var file in files) 
{
var filename = ContentDispositionHeaderValue
.Parse(file.ContentDisposition)
.FileName
.Trim('"'); 
string fileExtention = "." + filename.Split('.').Last(); 
Random rnd = new Random();
string uploadFileName = System.Text.RegularExpressions.Regex.Replace(filename.Split('.').First(), @"[^0-9a-zA-Z_.]+", "").Replace(" ", String.Empty)+"_"+"IPDApplicationForm_" +rnd.Next(1, 10000).ToString() + DateTime.Now.ToString("ddMMyyHHmmss")+ fileExtention;
if (fileExtention != ".")
{
Stream stream = file.OpenReadStream();
string fileURL=await util.fileSystem.UploadFileAsync(stream, Configuration.GetSection("AzureBlobStorageSetttings:folderName").Value, uploadFileName);
string row_id_medicalrecords_medicalrecordfile = file.Name.Replace("medicalrecords_medicalrecordfile_cma_uploader", "");
if (file.Name == "medicalrecords_medicalrecordfile_cma_uploader"+row_id_medicalrecords_medicalrecordfile)
{
var dependent_model = (from x in model.medicalrecords.OfType<IPDApplicationForm_medicalrecordsModel>() where x.cma_client_row_id == row_id_medicalrecords_medicalrecordfile select x).FirstOrDefault();
                                        dependent_model.medicalrecordfile += "|" + uploadFileName + "|";
}
if (file.Name == "uploadpassportcopy")
{
uploadFileName = "|" + fileURL +"|";
model.uploadpassportcopy +=  uploadFileName ;
}
if (file.Name == "uploadvisacopy")
{
uploadFileName = "|" + fileURL +"|";
model.uploadvisacopy +=  uploadFileName ;
}
if (file.Name == "consentfile")
{
uploadFileName = "|" + fileURL +"|";
model.consentfile +=  uploadFileName ;
}
}
}

								 
                                 strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/IPDApplicationForm/Add_IPD_Application_Form", model);
                                    
								 
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
                 
                 _logger.LogError(ex,"An exception occurred in - IPDApplicationForm / Add_IPD_Application_Form, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
               
				 strReturnMessage = ex.Message;
			 }
		     ViewData["message"] = strReturnMessage;
			 if(strReturnMessage.Replace("\"", "").Contains("201.1")){
				 TempData["message"] = "Success";
				 MailSender maillog = new MailSender();
                    bool mailsent = await maillog.sendNotification("IPDApplicationForm"
                    , "ReadyForReview"
                    , model.IPDApplicationFormid.ToString()
                    , _mailSettings
                    , model.createduser.ToString()
                    , client
                    , tenantid: model.tenantid?.ToString() ?? "");
                
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

		// ── Room Allotment endpoints (used from Approved Detail page) ──

		[HttpGet()]
		public virtual async Task<string> get_Available_Rooms(string tenantid, string roomtype = "", string occupancystatus = "", string nextdaycheckin = "", string nextdaycheckout = "")
		{
			// Fetch rooms using strict availability check from DB (Get_Room_List_Available)
			// nextdaycheckin/nextdaycheckout carry from/to dates for availability search
			return await ApiClient.Get_ApiValues(getHttpClient(),
				"api/Room/Get_Room_List_Available?tenantid=" + tenantid
				+ "&roomtype=" + (string.IsNullOrEmpty(roomtype) ? "" : Uri.EscapeDataString(roomtype))
				+ "&fromdate=" + (string.IsNullOrEmpty(nextdaycheckin) ? "" : Uri.EscapeDataString(nextdaycheckin))
				+ "&todate=" + (string.IsNullOrEmpty(nextdaycheckout) ? "" : Uri.EscapeDataString(nextdaycheckout))
				+ "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
		}

		[HttpGet()]
		public virtual async Task<string> get_all_RoomType_Details(string tenantid)
		{
			return await ApiClient.Get_ApiValues(getHttpClient(),
				"api/RoomType/get_all_RoomType?tenantid=" + tenantid
				+ "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
		}

		[HttpGet()]
		public virtual IActionResult Direct_IPD_Admission(string calendarSelectionToken = "", string embedded = "")
		{
			var calendarSelectionJson = "";
			if (!string.IsNullOrWhiteSpace(calendarSelectionToken))
			{
				calendarSelectionJson = HttpContext.Session.GetString(
					"CalendarDirectIPDSelection:" + calendarSelectionToken) ?? "";
			}
			ViewData["CalendarDirectIPDSelectionJson"] = calendarSelectionJson;
			ViewData["CalendarDirectIPDEmbedded"] = string.Equals(embedded, "Y", StringComparison.OrdinalIgnoreCase);
			ViewData["CalendarDirectIPDSelectionToken"] = calendarSelectionToken ?? "";
			return View(new IPDApplicationFormModel
			{
				bookingstatus = "Arrival Confirmed",
				verifiedstatus = "Direct Admission",
				agreefortermsandconditions = true
			});
		}

		[HttpPost()]
		public virtual IActionResult Prepare_Calendar_Direct_IPD([FromBody] CalendarDirectIPDSelectionModel selection)
		{
			var loginUserId = HttpContext.Session.GetString("NalamVazhaloginUserID");
			if (string.IsNullOrWhiteSpace(loginUserId))
				return Unauthorized("Session Expired");

			if (selection?.rooms == null || selection.rooms.Count == 0)
				return BadRequest("Select at least one patient room before continuing.");

			var token = Guid.NewGuid().ToString("N");
			HttpContext.Session.SetString(
				"CalendarDirectIPDSelection:" + token,
				JsonConvert.SerializeObject(selection.rooms));

			return Json(new
			{
				success = true,
				token,
				url = Url.Action("Direct_IPD_Admission", "IPDApplicationForm", new
				{
					calendarSelectionToken = token,
					embedded = "Y"
				})
			});
		}

        [HttpPost()]
        public virtual async Task<string> Direct_IPD_Admission(IPDApplicationFormModel model, IFormCollection collection)
        {
            try
            {
                var loginUserId = HttpContext.Session.GetString("NalamVazhaloginUserID");
                if (string.IsNullOrEmpty(loginUserId))
                    return "Session Expired";

                var requestedBookingStatus = (collection["bookingstatus"].ToString() ?? "").Trim();
                var isDraft = requestedBookingStatus.Equals("Draft", StringComparison.OrdinalIgnoreCase);
                var signatureCleared = (collection["signaturecleared"].ToString() ?? "").Trim()
                    .Equals("true", StringComparison.OrdinalIgnoreCase);

                var patientRoomTypeId = (collection["patientRoomType"].ToString() ?? "").Trim();
                var patientRoomId = (collection["patientRoom"].ToString() ?? "").Trim();
                var attendantRoomTypeId = (collection["attendantRoomType"].ToString() ?? "").Trim();
                var attendantRoomId = (collection["attendantRoom"].ToString() ?? "").Trim();
                var admitDateText = (collection["admitDate"].ToString() ?? "").Trim();
                var toDateText = (collection["toDate"].ToString() ?? "").Trim();
                var calendarRoomSelectionsJson = (collection["calendarRoomSelections"].ToString() ?? "").Trim();
                var packageIdText =
    (collection["packagename"].ToString() ?? "").Trim();

                if (Guid.TryParse(packageIdText, out var packageId))
                {
                    model.packagename = packageId;
                }
                else
                {
                    model.packagename = null;
                }
                if (model.patientname == Guid.Empty)
                    return "Please select an existing patient.";

                if (model.tenantid == null || model.tenantid == Guid.Empty)
                {
                    var sessionTenantId = HttpContext.Session.GetString("NalamVazhachoosedtenantid")
                        ?? HttpContext.Session.GetString("NalamVazhatenantid");

                    if (!string.IsNullOrWhiteSpace(sessionTenantId) && Guid.TryParse(sessionTenantId, out var parsedTenantId))
                        model.tenantid = parsedTenantId;
                }

                if (model.tenantid == null || model.tenantid == Guid.Empty)
                    return "Please select Tenant.";

                var calendarRoomResolution = await ResolveCalendarDirectAdmissionRooms(
                    calendarRoomSelectionsJson,
                    model.tenantid.Value,
                    loginUserId);
                if (!string.IsNullOrWhiteSpace(calendarRoomResolution.Error))
                    return calendarRoomResolution.Error;
                var calendarDirectRooms = calendarRoomResolution.Rooms;
                var calendarPatientRooms = calendarDirectRooms
                    .Where(room => string.Equals(room.Role, "Patient", StringComparison.OrdinalIgnoreCase))
                    .OrderBy(room => room.FromDate)
                    .ToList();
                var calendarAttendantRooms = calendarDirectRooms
                    .Where(room => string.Equals(room.Role, "Attendant", StringComparison.OrdinalIgnoreCase))
                    .OrderBy(room => room.FromDate)
                    .ToList();
                var hasCalendarPatientRooms = calendarPatientRooms.Count > 0;

                Guid parsedPatientRoomTypeId = Guid.Empty;
                Guid parsedPatientRoomId = Guid.Empty;
                Guid parsedAttendantRoomTypeId = Guid.Empty;
                Guid parsedAttendantRoomId = Guid.Empty;

                DateTime admitDate = DateTime.Today;
                DateTime toDate = DateTime.Today;

                var hasAttendantRoomInput = calendarAttendantRooms.Count > 0
                    || !string.IsNullOrWhiteSpace(attendantRoomTypeId)
                    || !string.IsNullOrWhiteSpace(attendantRoomId);

                var hasPatientRoomType = Guid.TryParse(patientRoomTypeId, out parsedPatientRoomTypeId);
                var hasPatientRoom = Guid.TryParse(patientRoomId, out parsedPatientRoomId);
                var hasCompletePatientRoom = hasPatientRoomType && hasPatientRoom;
                var hasAttendantRoomType = Guid.TryParse(attendantRoomTypeId, out parsedAttendantRoomTypeId);
                var hasAttendantRoom = Guid.TryParse(attendantRoomId, out parsedAttendantRoomId);
                var hasCompleteAttendantRoom = hasAttendantRoomType && hasAttendantRoom;
                var hasValidAdmitDate = TryParseDirectAdmissionDate(admitDateText, out admitDate);
                var hasValidToDate = TryParseDirectAdmissionDate(toDateText, out toDate);

                if (hasCalendarPatientRooms)
                {
                    var firstPatientRoom = calendarPatientRooms[0];
                    parsedPatientRoomTypeId = firstPatientRoom.RoomTypeId;
                    parsedPatientRoomId = firstPatientRoom.RoomId;
                    hasPatientRoomType = true;
                    hasPatientRoom = true;
                    hasCompletePatientRoom = true;
                    admitDate = calendarPatientRooms.Min(room => room.FromDate).Date;
                    toDate = calendarPatientRooms.Max(room => room.ToDate).Date;
                    hasValidAdmitDate = true;
                    hasValidToDate = true;
                }

                if (calendarAttendantRooms.Count > 0)
                {
                    parsedAttendantRoomTypeId = calendarAttendantRooms[0].RoomTypeId;
                    parsedAttendantRoomId = calendarAttendantRooms[0].RoomId;
                    hasAttendantRoomType = true;
                    hasAttendantRoom = true;
                    hasCompleteAttendantRoom = true;
                }
                var hasCompleteDateRange = hasValidAdmitDate && hasValidToDate && toDate.Date >= admitDate.Date;

                // Drafts preserve complete optional selections but never require or validate them.
                // All business constraints are enforced only when finalising Direct Admission.
                if (!isDraft && !hasPatientRoomType)
                    return "Please select Patient Room Type.";

                if (!isDraft && !hasPatientRoom)
                    return "Please select an available Patient Room.";

                if (!isDraft && model.attendantinfo != null && model.attendantinfo.Any()
                    && !hasCompleteAttendantRoom)
                    return "Please select an available Attendant Room.";

                if (!isDraft && hasAttendantRoomInput)
                {
                    if (!hasAttendantRoomType)
                        return "Please select Attendant Room Type.";

                    if (!hasAttendantRoom)
                        return "Please select an available Attendant Room.";

                    if (!hasCalendarPatientRooms && parsedAttendantRoomId == parsedPatientRoomId)
                        return "Patient Room and Attendant Room should be different rooms.";
                }

                if (!isDraft && !hasValidAdmitDate)
                    return "Please select a valid Admit Date.";

                if (!isDraft && !hasValidToDate)
                    return "Please select a valid To Date.";

                if (!isDraft && toDate.Date < admitDate.Date)
                    return "To Date should be on or after Admit Date.";

                if (!isDraft && admitDate.Date < DateTime.Today)
                    return "Admit Date should be today or a future date.";

                if (!isDraft && toDate.Date < DateTime.Today)
                    return "Past dates are not allowed.";

                if (!isDraft)
                {
                    var patientSegmentsForValidation = hasCalendarPatientRooms
                        ? calendarPatientRooms
                        : new List<DirectAdmissionRoomSelection>
                        {
                            new DirectAdmissionRoomSelection
                            {
                                Role = "Patient",
                                RoomTypeId = parsedPatientRoomTypeId,
                                RoomId = parsedPatientRoomId,
                                FromDate = admitDate.Date,
                                ToDate = toDate.Date
                            }
                        };
                    // A calendar split stay is one continuous admission divided across multiple
                    // rooms. Individual segments are allowed to be shorter than a room type's
                    // minimum stay; all other date rules remain enforced for every segment.
                    var isCalendarSplitStay = hasCalendarPatientRooms && patientSegmentsForValidation.Count > 1;

                    foreach (var patientSegment in patientSegmentsForValidation)
                    {
                        var roomTypeDateRuleMessage = await ValidateDirectAdmissionDateRules(
                            patientSegment.RoomTypeId,
                            patientSegment.FromDate,
                            patientSegment.ToDate,
                            loginUserId,
                            ignoreMinimumBookingDays: isCalendarSplitStay);
                        if (!string.IsNullOrWhiteSpace(roomTypeDateRuleMessage))
                            return roomTypeDateRuleMessage;

                        var patientRoomGenderMismatch = await GetDirectRoomTypeGenderMismatch(
                            patientSegment.RoomTypeId,
                            model.gender,
                            "Patient",
                            loginUserId);
                        if (!string.IsNullOrWhiteSpace(patientRoomGenderMismatch))
                            return patientRoomGenderMismatch;
                    }

                    var attendantGender = model.attendantinfo?.FirstOrDefault()?.gender;
                    var attendantSegmentsForValidation = calendarAttendantRooms.Count > 0
                        ? calendarAttendantRooms
                        : hasCompleteAttendantRoom
                            ? new List<DirectAdmissionRoomSelection>
                            {
                                new DirectAdmissionRoomSelection
                                {
                                    Role = "Attendant",
                                    RoomTypeId = parsedAttendantRoomTypeId,
                                    RoomId = parsedAttendantRoomId,
                                    FromDate = admitDate.Date,
                                    ToDate = toDate.Date
                                }
                            }
                            : new List<DirectAdmissionRoomSelection>();

                    foreach (var attendantSegment in attendantSegmentsForValidation)
                    {
                        var attendantRoomGenderMismatch = await GetDirectRoomTypeGenderMismatch(
                            attendantSegment.RoomTypeId,
                            attendantGender,
                            "Attendant",
                            loginUserId);
                        if (!string.IsNullOrWhiteSpace(attendantRoomGenderMismatch))
                            return attendantRoomGenderMismatch;
                    }
                }

                var existingIdText = (collection["IPDApplicationFormid"].ToString() ?? "").Trim();
                if (string.IsNullOrWhiteSpace(existingIdText) || !Guid.TryParse(existingIdText, out var existingId) || existingId == Guid.Empty)
                {
                    model.IPDApplicationFormid = Guid.NewGuid();
                    model.craftmyapp_actionmethodname = "Add_IPD_Application_Form";
                }
                else
                {
                    model.IPDApplicationFormid = existingId;
                    model.craftmyapp_actionmethodname = "Update_IPD_Application_Form";
                }

                // Server-side re-check: the frontend already warns and redirects when a patient
                // has an active IPD, but that check only runs when the patient is selected on the
                // form. If the user hits the browser Back button after being redirected, the form
                // re-appears from cache with the patient still populated and the check never
                // re-fires, letting a duplicate IPD through. Re-validate here so it can't be
                // bypassed by Back/bfcache (or by skipping the client-side check entirely).
                if (!isDraft)
                {
                    var activeIpdConflictMessage = await GetActiveIpdConflictMessage(model.patientname, model.IPDApplicationFormid, loginUserId);
                    if (!string.IsNullOrWhiteSpace(activeIpdConflictMessage))
                        return activeIpdConflictMessage;
                }

                model.createduser = new Guid(loginUserId);


                if (isDraft)
                {
                    model.bookingstatus = "Draft";
                    model.verifiedstatus = "Direct Admission";
                }
                else
                {
					model.bookingstatus = "Arrival Confirmed";
                    model.verifiedstatus = "Direct Admission";
                }

                model.agreefortermsandconditions = true;
                model.plannedadmissionstartdate = hasValidAdmitDate ? admitDate.Date : null;
                model.uploadpassportcopy = collection["uploadpassportcopy_existing"];
                model.uploadvisacopy = collection["uploadvisacopy_existing"];

                if (!isDraft && model.consentform == Guid.Empty)
                {
                    var defaultConsentId = await ResolveDefaultDirectAdmissionConsent(model.tenantid);
                    if (defaultConsentId.HasValue)
                        model.consentform = defaultConsentId.Value;
                    else
                        return "Please configure at least one consent form for this tenant before direct admission.";
                }

                decimal packageCost = 0;
                int packageDays = 0;
                var packageRoomCoverage = new Dictionary<Guid, decimal>();

                if (!isDraft && model.packagename.HasValue && model.packagename.Value != Guid.Empty)
                {
                    var packageJson = await ApiClient.Get_ApiValues(getHttpClient(),
                        "api/TreatmentPackage/getById_TreatmentPackage?TreatmentPackageid=" + model.packagename.Value
                        + "&loginUserID=" + loginUserId);

                    var packageToken = ParseFirstJsonToken(packageJson);
                    decimal.TryParse((packageToken?["packagecost"] ?? packageToken?["Packagecost"])?.ToString(), out packageCost);
                    int.TryParse((packageToken?["noofdays"] ?? packageToken?["Noofdays"]
                        ?? packageToken?["packagedays"] ?? packageToken?["Packagedays"]
                        ?? packageToken?["numberofdays"] ?? packageToken?["Numberofdays"])?.ToString(), out packageDays);

                    var packageRoomTypesJson = await ApiClient.Get_ApiValues(getHttpClient(),
                        "api/TreatmentPackage/getById_roomtypes?TreatmentPackageid=" + model.packagename.Value
                        + "&loginUserID=" + loginUserId);

                    if (!string.IsNullOrWhiteSpace(packageRoomTypesJson) && packageRoomTypesJson.Length > 2)
                    {
                        var packageRoomTypes = JArray.Parse(packageRoomTypesJson);

                        foreach (var row in packageRoomTypes)
                        {
                            var roomTypeText = (row["roomtype"] ?? row["RoomType"] ?? row["roomtypeid"] ?? row["RoomTypeid"])?.ToString();

                            if (!Guid.TryParse(roomTypeText, out var pkgRoomTypeId))
                                continue;

                            decimal.TryParse((row["percentagecovered"] ?? row["Percentagecovered"])?.ToString(), out var coveragePercent);
                            packageRoomCoverage[pkgRoomTypeId] = Math.Max(0, Math.Min(100, coveragePercent));
                        }
                    }
                }

                // Save room preference for both Draft and Direct Admission
                var directRooms = hasCalendarPatientRooms
                    ? calendarDirectRooms.ToList()
                    : new List<DirectAdmissionRoomSelection>();

                if (!hasCalendarPatientRooms && hasCompletePatientRoom)
                {
                    directRooms.Add(new DirectAdmissionRoomSelection
                    {
                        Role = "Patient",
                        RoomTypeId = parsedPatientRoomTypeId,
                        RoomId = parsedPatientRoomId,
                        FromDate = admitDate.Date,
                        ToDate = toDate.Date
                    });
                }

                if (!hasCalendarPatientRooms && hasCompleteAttendantRoom)
                {
                    directRooms.Add(new DirectAdmissionRoomSelection
                    {
                        Role = "Attendant",
                        RoomTypeId = parsedAttendantRoomTypeId,
                        RoomId = parsedAttendantRoomId,
                        FromDate = admitDate.Date,
                        ToDate = toDate.Date
                    });
                }

                var actionName = model.craftmyapp_actionmethodname;

                model.roompreference = directRooms
                    .Where(room => string.Equals(room.Role, "Patient", StringComparison.OrdinalIgnoreCase))
                    .GroupBy(room => room.RoomTypeId)
                    .Select(group => group.First())
                    .Select((room, index) => new IPDApplicationForm_roompreferenceModel
                    {
                        roomtype = room.RoomTypeId,
                        record_order = index + 1,
                        cma_client_row_id = "direct-" + room.Role.ToLowerInvariant() + "|" + room.RoomId,
                        IPDApplicationForm_roompreferenceid = Guid.Empty,
                        craftmyapp_actionmethodname = actionName  // ← not hardcoded
                    })
                    .ToList();

                model.attendantroompreference = directRooms
                    .Where(room => string.Equals(room.Role, "Attendant", StringComparison.OrdinalIgnoreCase))
                    .GroupBy(room => room.RoomTypeId)
                    .Select(group => group.First())
                    .Select((room, index) => new IPDApplicationForm_attendantroompreferenceModel
                    {
                        roomtypeatt = room.RoomTypeId,
                        record_order = index + 1,
                        cma_client_row_id = "direct-attendant|" + room.RoomId,
                        IPDApplicationForm_attendantroompreferenceid = Guid.Empty,
                        craftmyapp_actionmethodname = actionName
                    })
                    .ToList();

                model.attendantpreferreddates = directRooms
                    .Where(room => string.Equals(room.Role, "Attendant", StringComparison.OrdinalIgnoreCase))
                    .Select((room, index) => new IPDApplicationForm_attendantpreferreddatesModel
                    {
                        dateofarrivalatt = room.FromDate,
                        dateofdepartureatt = room.ToDate,
                        daysofstayatt = (int)(room.ToDate - room.FromDate).TotalDays + 1,
                        record_order = index + 1,
                        cma_client_row_id = "direct-attendant|" + room.RoomId,
                        IPDApplicationForm_attendantpreferreddatesid = Guid.Empty,
                        craftmyapp_actionmethodname = actionName
                    })
                    .ToList();

                model.preferreddatesofadmission = hasCompleteDateRange
                    ? new List<IPDApplicationForm_preferreddatesofadmissionModel>
                    {
                        new IPDApplicationForm_preferreddatesofadmissionModel
                        {
                            dateofarrival = admitDate.Date,
                            dateofdeparture = toDate.Date,
                            daysofstay = (int)(toDate.Date - admitDate.Date).TotalDays + 1,
                            record_order = 1,
                            cma_client_row_id = "direct",
                            IPDApplicationForm_preferreddatesofadmissionid = Guid.Empty,
                            craftmyapp_actionmethodname = actionName
                        }
                    }
                    : new List<IPDApplicationForm_preferreddatesofadmissionModel>();

                // Always send the selected rooms to the parent Add/Update API. Previously this
                // collection was populated only for Draft saves, so finalising an existing draft
                // called Update_IPD_Application_Form without the patient room.
                var existingRoomIdsByRole = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
                if (model.craftmyapp_actionmethodname == "Update_IPD_Application_Form")
                {
                    var existingRoomsJson = await ApiClient.Get_ApiValues(getHttpClient(),
                        "api/IPDApplicationForm/getById_room?IPDApplicationFormid="
                        + model.IPDApplicationFormid
                        + "&loginUserID=" + loginUserId);

                    if (!string.IsNullOrWhiteSpace(existingRoomsJson) && existingRoomsJson.Length > 2)
                    {
                        var existingRooms = JsonConvert.DeserializeObject<List<IPDApplicationForm_roomModel>>(existingRoomsJson)
                            ?? new List<IPDApplicationForm_roomModel>();
                        foreach (var existingRoom in existingRooms)
                        {
                            if (!string.IsNullOrWhiteSpace(existingRoom.allottedto)
                                && existingRoom.IPDApplicationForm_roomid.HasValue
                                && existingRoom.IPDApplicationForm_roomid.Value != Guid.Empty)
                                existingRoomIdsByRole[existingRoom.allottedto.Trim()] = existingRoom.IPDApplicationForm_roomid.Value;
                        }
                    }
                }

                model.room = directRooms
                    .Select((room, index) => new IPDApplicationForm_roomModel
                    {
                        allottedto = room.Role,
                        roomnumber = room.RoomId,
                        roomtype = room.RoomTypeId,
                        fromdate = room.FromDate,
                        todate = room.ToDate,
                        record_order = index + 1,
                        cma_client_row_id = "direct-" + room.Role.ToLowerInvariant() + "|" + room.RoomId,
                        IPDApplicationForm_roomid = existingRoomIdsByRole.TryGetValue(room.Role, out var existingRoomId)
                            ? existingRoomId
                            : Guid.Empty,
                        craftmyapp_actionmethodname = actionName
                    })
                    .ToList();

                // Only Direct Admission checks room conflict. Draft should not block/save occupancy.
                if (!isDraft)
                {
                    foreach (var directRoom in directRooms)
                    {
                        var conflict = await GetDirectRoomAvailabilityConflictMessage(
                            directRoom.RoomId,
                            directRoom.RoomTypeId,
                            directRoom.FromDate,
                            directRoom.ToDate,
                            model.tenantid,
                            loginUserId);

                        if (!string.IsNullOrWhiteSpace(conflict))
                            return directRoom.Role + " room: " + conflict;
                    }
                }

	                foreach (var medicalRecordFile in Request.Form.Files.Where(f =>
	                    f.Name.StartsWith("medicalrecords_medicalrecordfile_cma_uploader", StringComparison.Ordinal)))
	                {
	                    if (!IsAllowedDirectMedicalRecordFile(medicalRecordFile))
	                        return "Only PDF, Word document, and image files are allowed for Medical Records.";
	                }

	                foreach (var file in Request.Form.Files)
                {
                    var filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    string fileExtention = "." + filename.Split('.').Last();

                    if (fileExtention == ".")
                        continue;

                    Random rnd = new Random();

                    string uploadFileName = System.Text.RegularExpressions.Regex.Replace(filename.Split('.').First(), @"[^0-9a-zA-Z_.]+", "").Replace(" ", String.Empty)
                        + "_" + "IPDApplicationForm_" + rnd.Next(1, 10000).ToString()
                        + DateTime.Now.ToString("ddMMyyHHmmss") + fileExtention;

                    using Stream stream = file.OpenReadStream();

                    string fileURL = await util.fileSystem.UploadFileAsync(
                        stream,
                        Configuration.GetSection("AzureBlobStorageSetttings:folderName").Value,
                        uploadFileName
                    );

                    string rowId = file.Name.Replace("medicalrecords_medicalrecordfile_cma_uploader", "");

                    if (file.Name == "medicalrecords_medicalrecordfile_cma_uploader" + rowId && model.medicalrecords != null)
                    {
                        var dependentModel = model.medicalrecords
                            .OfType<IPDApplicationForm_medicalrecordsModel>()
                            .FirstOrDefault(x => x.cma_client_row_id == rowId);

                        if (dependentModel != null)
                            dependentModel.medicalrecordfile += "|" + fileURL + "|";
                    }

                    if (file.Name == "uploadpassportcopy")
                        model.uploadpassportcopy += "|" + fileURL + "|";

                    if (file.Name == "uploadvisacopy")
                        model.uploadvisacopy += "|" + fileURL + "|";
                }

                if (model.craftmyapp_actionmethodname == "Update_IPD_Application_Form")
                {
                    var existingJson = await ApiClient.Get_ApiValues(getHttpClient(),
                        "api/IPDApplicationForm/getById_IPDApplicationForm?IPDApplicationFormid="
                        + model.IPDApplicationFormid
                        + "&loginUserID=" + loginUserId);

                    if (!string.IsNullOrWhiteSpace(existingJson) && existingJson.Length > 2)
                    {
                        var existingModel = JsonConvert.DeserializeObject<IPDApplicationFormModel>(existingJson);
                        if (existingModel != null)
                        {
                            model.bookingreferencenumber = existingModel.bookingreferencenumber;

                            if (!signatureCleared
                                && string.IsNullOrWhiteSpace(model.signature)
                                && !string.IsNullOrWhiteSpace(existingModel.signature))
                                model.signature = existingModel.signature;
                        }
                    }
                }

                var apiEndpoint = model.craftmyapp_actionmethodname == "Update_IPD_Application_Form"
       ? "api/IPDApplicationForm/Update_IPD_Application_Form"
       : "api/IPDApplicationForm/Add_IPD_Application_Form";

                var addResult = await ApiClient.Post_ApiValuesGetString(
                    getHttpClient(),
                    apiEndpoint,
                    model
                );
                var addMessage = (addResult ?? "").Replace("\"", "");

                if (!addMessage.Contains("201.1"))
                    return addResult;

                // Draft stops here.
                // It saves main IPD form + room preference + preferred dates + any entered dependent rows.
                // It does not allot room, create occupancy, create receivables, or update booking status.
                if (isDraft)
                {
                    TempData["message"] = "Draft saved successfully";
                    return "Success";
                }

                var savedIpdJson = await ApiClient.Get_ApiValues(getHttpClient(),
                    "api/IPDApplicationForm/getById_IPDApplicationForm?IPDApplicationFormid="
                    + model.IPDApplicationFormid
                    + "&loginUserID=" + loginUserId);

                if (!string.IsNullOrWhiteSpace(savedIpdJson) && savedIpdJson.Length > 2)
                {
                    var savedModel = JsonConvert.DeserializeObject<IPDApplicationFormModel>(savedIpdJson);

                    if (savedModel != null)
                    {
                        model.bookingreferencenumber = savedModel.bookingreferencenumber;
                        model.tenantid = savedModel.tenantid ?? model.tenantid;
                    }
                }

                // Allot_Room persists both the room allocation and its per-day occupancy rows in
                // one database transaction. Do not insert occupancy again from this request.
                var allotResult = await SaveDirectAdmissionRoomAllotment(model, directRooms, admitDate.Date, toDate.Date, packageRoomCoverage, loginUserId);
                if (allotResult != "Success")
                    return allotResult;

                var bookingDepositCreatedForRole = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var remainingPackagePatientDays = packageDays > 0 ? packageDays : int.MaxValue;
                foreach (var directRoom in directRooms
                    .OrderBy(room => string.Equals(room.Role, "Attendant", StringComparison.OrdinalIgnoreCase) ? 1 : 0)
                    .ThenBy(room => room.FromDate))
                {
                    var createBookingDeposit = bookingDepositCreatedForRole.Add(directRoom.Role ?? "Patient");
                    var segmentDays = (int)(directRoom.ToDate.Date - directRoom.FromDate.Date).TotalDays + 1;
                    var packageCoveredDays = 0;
                    if (!string.Equals(directRoom.Role, "Attendant", StringComparison.OrdinalIgnoreCase)
                        && packageRoomCoverage.ContainsKey(directRoom.RoomTypeId)
                        && remainingPackagePatientDays > 0)
                    {
                        packageCoveredDays = Math.Min(segmentDays, remainingPackagePatientDays);
                        if (remainingPackagePatientDays != int.MaxValue)
                            remainingPackagePatientDays -= packageCoveredDays;
                    }
                    var receivableResult = await CreateDirectAdmissionRoomReceivables(
                        model,
                        directRoom,
                        admitDate.Date,
                        toDate.Date,
                        packageRoomCoverage,
                        loginUserId,
                        createBookingDeposit,
                        packageCoveredDays);

                    if (receivableResult != "Success")
                        return receivableResult;
                }

                if (packageCost > 0)
                {
                    var packageReceivableResult = await CreateDirectAdmissionPackageReceivable(model, packageCost, admitDate.Date, toDate.Date, loginUserId);

                    if (packageReceivableResult != "Success")
                        return packageReceivableResult;
                }

                var statusModel = new IPDBookingStatusUpdateModel
                {
                    IPDApplicationFormid = model.IPDApplicationFormid.ToString(),
                    bookingstatus = "Arrival Confirmed", // Use "Confirmed" if "Admitted" is not in lookup
                    verifiedstatus = "Direct Admission",
                    skiproomreceivables = true,
                    modifieduser = loginUserId
                };

                var statusResult = await ApiClient.Post_ApiValuesGetRawString(
                    getHttpClient(),
                    "api/IPDApplicationForm/Update_IPD_Booking_Status",
                    statusModel
                );

                var statusMessage = (statusResult ?? "").Replace("\"", "");

                if (!statusMessage.Contains("201.1"))
                    return statusResult;

                try
                {
                    // Existing notification data exposes the selected preferred arrival as
                    // {dateofarrival}, so planned confirmations include the future start date.
                    var notificationAction = admitDate.Date > DateTime.Today
                        ? "ProvisionalBooking"
                        : "Admitted";
                    var maillog = new MailSender();
                    using var notificationClient = getHttpClient();
                    var notificationSent = await maillog.sendNotification(
                        "IPDApplicationForm",
                        notificationAction,
                        model.IPDApplicationFormid.ToString(),
                        _mailSettings,
                        loginUserId,
                        notificationClient,
						baseUrl,
                        tenantid: model.tenantid?.ToString() ?? "");

                    if (!notificationSent)
                    {
                        _logger.LogWarning(
                            "Direct IPD admission notification was not sent for IPD {IPDApplicationFormId} and selected patient {PatientProfileId}.",
                            model.IPDApplicationFormid,
                            model.patientname);
                    }
                }
                catch (Exception notificationException)
                {
                    // Admission must remain successful even when notification delivery fails.
                    _logger.LogError(notificationException, "Direct IPD admission notification failed.");
                }

                TempData["message"] = "Success";
                return "Success";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred in - IPDApplicationForm / Direct_IPD_Admission");
                return ex.Message;
            }
        }
        private static bool TryParseDirectAdmissionDate(string value, out DateTime admitDate)
		{
			return DateTime.TryParseExact(value, new[] { "yyyy-MM-dd", "dd/MM/yyyy", "dd/MM/yyyy HH:mm", "yyyy-MM-ddTHH:mm" },
				CultureInfo.InvariantCulture, DateTimeStyles.None, out admitDate)
				|| DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out admitDate);
		}

		private async Task<string> ValidateDirectAdmissionDateRules(
			Guid roomTypeId,
			DateTime admitDate,
			DateTime toDate,
			string loginUserId,
			bool ignoreMinimumBookingDays = false)
		{
			var roomTypeJson = await ApiClient.Get_ApiValues(getHttpClient(),
				"api/RoomType/getById_RoomType?RoomTypeid=" + roomTypeId
				+ "&loginUserID=" + loginUserId);
			var roomTypeToken = ParseFirstJsonToken(roomTypeJson);
			if (roomTypeToken == null)
				return "Selected room type details could not be loaded.";

			var minBookingDays = ParsePositiveDirectAdmissionInt(roomTypeToken["minbookingdays"] ?? roomTypeToken["Minbookingdays"]);
			var maxBookingDays = ParsePositiveDirectAdmissionInt(roomTypeToken["maxbookingdays"] ?? roomTypeToken["Maxbookingdays"]);
			// Previous maximum used futurebookinglimit. prebookingdaylimit is now the post-booking maximum.
			var postBookingLimit = ParsePositiveDirectAdmissionInt(roomTypeToken["prebookingdaylimit"] ?? roomTypeToken["Prebookingdaylimit"]);
			if (postBookingLimit <= 0)
				postBookingLimit = 120;
			var daysOfStay = (toDate.Date - admitDate.Date).Days + 1;

			if (admitDate.Date < DateTime.Today || toDate.Date < DateTime.Today)
				return "Booking dates cannot be earlier than the current date.";
			if (!ignoreMinimumBookingDays && minBookingDays > 0 && daysOfStay < minBookingDays)
				return "To Date does not meet the selected room type minimum stay.";
			if (maxBookingDays > 0 && daysOfStay > maxBookingDays)
				return "To Date exceeds the selected room type maximum stay.";
			if (admitDate.Date > DateTime.Today.AddDays(postBookingLimit) || toDate.Date > DateTime.Today.AddDays(postBookingLimit))
				return "Booking dates exceed the selected Room Type post-booking limit.";

			return "";
		}

		private async Task<string> GetActiveIpdConflictMessage(Guid patientProfileId, Guid? ipdApplicationFormIdBeingSaved, string loginUserId)
		{
			const string validationFailureMessage = "Unable to verify whether the patient has an active IPD application. Please retry.";

			if (patientProfileId == Guid.Empty)
				return "";

			try
			{
				var activeIpdJson = await ApiClient.Get_ApiValues(getHttpClient(),
					"api/IPDApplicationForm/get_Active_IPD_Application?patientprofileid=" + patientProfileId
					+ "&loginUserID=" + loginUserId);
				var activeIpdToken = ParseFirstJsonToken(activeIpdJson);
				if (activeIpdToken == null)
					return validationFailureMessage;

				var validationSucceededToken = activeIpdToken["validationSucceeded"] ?? activeIpdToken["validationsucceeded"];
				if (validationSucceededToken != null
					&& (!bool.TryParse(validationSucceededToken.ToString(), out var validationSucceeded) || !validationSucceeded))
					return (activeIpdToken["message"]?.ToString() ?? validationFailureMessage).Trim();

				var hasActiveIpdToken = activeIpdToken["hasActiveIPD"] ?? activeIpdToken["hasactiveipd"] ?? activeIpdToken["hasActiveIpd"];
				if (hasActiveIpdToken == null)
					return validationFailureMessage;

				var hasActiveIpd = hasActiveIpdToken != null && bool.TryParse(hasActiveIpdToken.ToString(), out var parsedHasActive) && parsedHasActive;
				if (!hasActiveIpd)
					return "";

				var activeIpdIdText = (activeIpdToken["ipdapplicationformid"] ?? activeIpdToken["IPDApplicationFormid"])?.ToString();
				// Editing/resuming the very same record isn't a conflict -- only block when the
				// active IPD found belongs to a *different* application than the one being saved.
				if (Guid.TryParse(activeIpdIdText, out var activeIpdId) && activeIpdId == ipdApplicationFormIdBeingSaved)
					return "";

				return "Patient already has an active IPD application.";
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unable to validate active IPD status for patient {PatientProfileId}.", patientProfileId);
				return validationFailureMessage;
			}
		}

		private static int ParsePositiveDirectAdmissionInt(JToken token)
		{
			if (token == null)
				return 0;
			return int.TryParse(token.ToString(), out var value) && value > 0 ? value : 0;
		}

		private async Task<Guid?> ResolveDefaultDirectAdmissionConsent(Guid? tenantId)
		{
			if (tenantId == null || tenantId == Guid.Empty)
				return null;

			var consentJson = await ApiClient.Get_ApiValues(getHttpClient(),
				"api/IPDApplicationForm/lookup_IPDApplicationForm_consentform?tenantid="
				+ tenantId
				+ "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
			if (string.IsNullOrWhiteSpace(consentJson) || consentJson.Length <= 2)
				return null;

			var consentRows = JArray.Parse(consentJson);
			var registrationConsent = consentRows.FirstOrDefault(row =>
				string.Equals((row["consenttype"] ?? row["name"])?.ToString()?.Trim(), "Registration", StringComparison.OrdinalIgnoreCase));
			var selected = registrationConsent ?? consentRows.FirstOrDefault();
			var idText = selected?["PatientConsentid"]?.ToString() ?? selected?["patientconsentid"]?.ToString();
			return Guid.TryParse(idText, out var consentId) ? consentId : null;
		}

		private async Task<string> GetDirectRoomAvailabilityConflictMessage(
			Guid roomId,
			Guid roomTypeId,
			DateTime admitDate,
			DateTime toDate,
			Guid? tenantId,
			string loginUserId)
		{
			if (roomId == Guid.Empty || roomTypeId == Guid.Empty || !tenantId.HasValue || tenantId.Value == Guid.Empty)
				return "Unable to verify the selected room. Please choose the room again.";

			try
			{
				// Use the same strict DB availability source that populates the room dropdown.
				var availabilityJson = await ApiClient.Get_ApiValues(getHttpClient(),
					"api/Room/Get_Room_List_Available?tenantid=" + tenantId.Value
					+ "&roomtype=" + roomTypeId
					+ "&fromdate=" + Uri.EscapeDataString(admitDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))
					+ "&todate=" + Uri.EscapeDataString(toDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))
					+ "&loginUserID=" + loginUserId);

				var availableRooms = ParseJsonArrayOrEmpty(availabilityJson);
				var selectedRoomIsAvailable = availableRooms.Any(row =>
				{
					var availableRoomIdText = (row["Roomid"] ?? row["roomid"] ?? row["RoomID"])?.ToString();
					return Guid.TryParse(availableRoomIdText, out var availableRoomId) && availableRoomId == roomId;
				});

				return selectedRoomIsAvailable
					? ""
					: "Selected room is no longer available for the selected date range. Please choose another room.";
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unable to validate Direct IPD room {RoomId} availability.", roomId);
				return "Unable to verify room availability. Please refresh the available rooms and retry.";
			}
		}

		private async Task<DirectAdmissionRoomResolution> ResolveCalendarDirectAdmissionRooms(
			string selectionJson,
			Guid tenantId,
			string loginUserId)
		{
			var result = new DirectAdmissionRoomResolution();
			if (string.IsNullOrWhiteSpace(selectionJson))
				return result;

			List<CalendarDirectIPDRoomSegmentModel> postedRooms;
			try
			{
				postedRooms = JsonConvert.DeserializeObject<List<CalendarDirectIPDRoomSegmentModel>>(selectionJson)
					?? new List<CalendarDirectIPDRoomSegmentModel>();
			}
			catch
			{
				result.Error = "The calendar room selection is invalid. Please return to the calendar and select the rooms again.";
				return result;
			}

			foreach (var postedRoom in postedRooms)
			{
				var role = string.Equals(postedRoom.role, "Attendant", StringComparison.OrdinalIgnoreCase)
					? "Attendant"
					: string.Equals(postedRoom.role, "Patient", StringComparison.OrdinalIgnoreCase) ? "Patient" : "";
				if (postedRoom.roomid == Guid.Empty || string.IsNullOrWhiteSpace(role)
					|| postedRoom.fromdate == default || postedRoom.todate == default
					|| postedRoom.todate.Date < postedRoom.fromdate.Date)
				{
					result.Error = "Every calendar selection must contain a valid room, role, and date range.";
					return result;
				}

				var roomJson = await ApiClient.Get_ApiValues(getHttpClient(),
					"api/Room/getById_Room?Roomid=" + postedRoom.roomid
					+ "&loginUserID=" + loginUserId);
				var roomToken = ParseFirstJsonToken(roomJson);
				if (roomToken == null)
				{
					result.Error = "A selected room could not be loaded. Please refresh the calendar and retry.";
					return result;
				}

				var roomTenantText = (roomToken["tenantid"] ?? roomToken["Tenantid"] ?? roomToken["TenantId"])?.ToString();
				if (Guid.TryParse(roomTenantText, out var roomTenantId) && roomTenantId != tenantId)
				{
					result.Error = "A selected room does not belong to the active organization.";
					return result;
				}

				var roomTypeText = (roomToken["roomtype"] ?? roomToken["Roomtype"]
					?? roomToken["roomtypeid"] ?? roomToken["RoomTypeid"])?.ToString();
				if (!Guid.TryParse(roomTypeText, out var roomTypeId) || roomTypeId == Guid.Empty)
				{
					result.Error = "Room master setup is incomplete for one of the selected rooms.";
					return result;
				}
				var blockText = (roomToken["block"] ?? roomToken["Block"])?.ToString();
				var buildingText = (roomToken["building"] ?? roomToken["Building"])?.ToString();
				var floorText = (roomToken["floor"] ?? roomToken["Floor"])?.ToString();
				if (!Guid.TryParse(blockText, out var blockId) || blockId == Guid.Empty
					|| !Guid.TryParse(buildingText, out var buildingId) || buildingId == Guid.Empty
					|| !Guid.TryParse(floorText, out var floorId) || floorId == Guid.Empty)
				{
					result.Error = "Room master setup is incomplete. Block, building, floor, and room type must all be mapped.";
					return result;
				}

				result.Rooms.Add(new DirectAdmissionRoomSelection
				{
					Role = role,
					RoomId = postedRoom.roomid,
					RoomTypeId = roomTypeId,
					FromDate = postedRoom.fromdate.Date,
					ToDate = postedRoom.todate.Date
				});
			}

			foreach (var roleGroup in result.Rooms.GroupBy(room => room.Role, StringComparer.OrdinalIgnoreCase))
			{
				var ordered = roleGroup.OrderBy(room => room.FromDate).ToList();
				for (var index = 1; index < ordered.Count; index++)
				{
					if (ordered[index].FromDate <= ordered[index - 1].ToDate)
					{
						result.Error = roleGroup.Key + " room selections cannot overlap.";
						return result;
					}
					if (ordered[index].FromDate != ordered[index - 1].ToDate.AddDays(1))
					{
						result.Error = roleGroup.Key + " room selections must form one continuous stay without missing dates.";
						return result;
					}
				}
			}

			var patientRooms = result.Rooms.Where(room => room.Role == "Patient").ToList();
			if (patientRooms.Count == 0)
			{
				result.Error = "Select at least one patient room before continuing.";
				return result;
			}
			var patientFrom = patientRooms.Min(room => room.FromDate);
			var patientTo = patientRooms.Max(room => room.ToDate);
			if (result.Rooms.Any(room => room.Role == "Attendant"
				&& (room.FromDate < patientFrom || room.ToDate > patientTo)))
			{
				result.Error = "Attendant room dates must be within the patient's selected stay.";
				return result;
			}

			for (var left = 0; left < result.Rooms.Count; left++)
			{
				for (var right = left + 1; right < result.Rooms.Count; right++)
				{
					if (result.Rooms[left].RoomId == result.Rooms[right].RoomId
						&& result.Rooms[left].FromDate <= result.Rooms[right].ToDate
						&& result.Rooms[left].ToDate >= result.Rooms[right].FromDate)
					{
						result.Error = "The same room cannot be assigned to the patient and attendant for overlapping dates.";
						return result;
					}
				}
			}

			return result;
		}

		private sealed class DirectAdmissionRoomSelection
		{
			public string Role { get; set; }
			public Guid RoomTypeId { get; set; }
			public Guid RoomId { get; set; }
			public DateTime FromDate { get; set; }
			public DateTime ToDate { get; set; }
		}

		private sealed class DirectAdmissionRoomResolution
		{
			public List<DirectAdmissionRoomSelection> Rooms { get; set; } = new List<DirectAdmissionRoomSelection>();
			public string Error { get; set; } = "";
		}

		private async Task<string> SaveDirectAdmissionRoomAllotment(IPDApplicationFormModel sourceModel, List<DirectAdmissionRoomSelection> directRooms, DateTime admitDate, DateTime toDate, Dictionary<Guid, decimal> packageRoomCoverage, string loginUserId)
		{
			var allotModel = new IPDApplicationFormModel
			{
				IPDApplicationFormid = sourceModel.IPDApplicationFormid,
				tenantid = sourceModel.tenantid,
				bookingreferencenumber = sourceModel.bookingreferencenumber,
				verifiedstatus = sourceModel.verifiedstatus,
				modifieduser = new Guid(loginUserId),
				craftmyapp_actionmethodname = "Allot_Room",
				room = new List<IPDApplicationForm_roomModel>()
			};

			for (var index = 0; index < directRooms.Count; index++)
			{
				var directRoom = directRooms[index];
				var roomJson = await ApiClient.Get_ApiValues(getHttpClient(), "api/Room/getById_Room?Roomid=" + directRoom.RoomId);
				var roomToken = ParseFirstJsonToken(roomJson);
				decimal costPerDay = 0;
				var isAttendantRoom = string.Equals(directRoom.Role, "Attendant", StringComparison.OrdinalIgnoreCase);
				decimal.TryParse((roomToken?[isAttendantRoom ? "attendantcostperday" : "costperday"]
					?? roomToken?[isAttendantRoom ? "Attendantcostperday" : "Costperday"]
					?? roomToken?["costperday"]
					?? roomToken?["Costperday"])?.ToString(), out costPerDay);
				var packageCoverage = (!isAttendantRoom && packageRoomCoverage.TryGetValue(directRoom.RoomTypeId, out var coveredPercent))
					? Math.Max(0, Math.Min(100, coveredPercent))
					: 100;
				var hasPackageCoverage = !isAttendantRoom && sourceModel.packagename.HasValue && sourceModel.packagename.Value != Guid.Empty && packageRoomCoverage.ContainsKey(directRoom.RoomTypeId);

				allotModel.room.Add(new IPDApplicationForm_roomModel
				{
					allottedto = directRoom.Role,
					roomnumber = directRoom.RoomId,
					fromdate = directRoom.FromDate,
					todate = directRoom.ToDate,
					roomtype = directRoom.RoomTypeId,
					costperday = costPerDay,
					percentagecovered = packageCoverage,
					ispackageroom = hasPackageCoverage,
					record_order = index,
					cma_client_row_id = "direct-" + directRoom.Role.ToLowerInvariant(),
					IPDApplicationForm_roomid = Guid.Empty,
					IPDApplicationFormid = sourceModel.IPDApplicationFormid,
					craftmyapp_actionmethodname = "Allot_Room"
				});
			}

			var result = await ApiClient.Post_ApiValuesGetString(getHttpClient(), "api/IPDApplicationForm/Allot_Room", allotModel);
			return (result ?? "").Replace("\"", "").Contains("201.1") ? "Success" : result;
		}

		private async Task<string> CreateDirectAdmissionRoomReceivables(IPDApplicationFormModel sourceModel, DirectAdmissionRoomSelection directRoom, DateTime admitDate, DateTime toDate, Dictionary<Guid, decimal> packageRoomCoverage, string loginUserId, bool createBookingDeposit, int packageCoveredDays)
		{
			var roomJson = await ApiClient.Get_ApiValues(getHttpClient(), "api/Room/getById_Room?Roomid=" + directRoom.RoomId);
			var roomToken = ParseFirstJsonToken(roomJson);
			if (roomToken == null)
				return "Selected room details could not be loaded for receivable creation.";

			decimal costPerDay = 0;
			var isAttendantRoom = string.Equals(directRoom.Role, "Attendant", StringComparison.OrdinalIgnoreCase);
			decimal.TryParse((roomToken[isAttendantRoom ? "attendantcostperday" : "costperday"]
				?? roomToken[isAttendantRoom ? "Attendantcostperday" : "Costperday"]
				?? roomToken["costperday"]
				?? roomToken["Costperday"])?.ToString(), out costPerDay);

			var bookingDepositToken = isAttendantRoom
				? roomToken["attendantbookingdeposit"] ?? roomToken["Attendantbookingdeposit"]
				: roomToken["bookingdeposit"] ?? roomToken["Bookingdeposit"];
			decimal bookingDeposit = 0;
			decimal.TryParse(bookingDepositToken?.ToString(), out bookingDeposit);

			var roomNumber = (roomToken["roomnumber"] ?? roomToken["Roomnumber"])?.ToString();
			if (createBookingDeposit && bookingDeposit > 0)
			{
				var bookingDepositModel = new ReceivableModel
				{
					Receivableid = Guid.NewGuid(),
					tenantid = sourceModel.tenantid,
					receivabledate = admitDate.Date,
					patientname = sourceModel.patientname,
					ipdnumber = sourceModel.IPDApplicationFormid,
					receivablefor = isAttendantRoom
						? "IPD Booking Deposit - Attendant"
						: "IPD Booking Deposit - Patient",
					room = directRoom.RoomId,
					amount = bookingDeposit,
					remarks = "Direct IPD " + directRoom.Role.ToLowerInvariant() + " room booking deposit"
						+ (string.IsNullOrWhiteSpace(roomNumber) ? "" : " - Room " + roomNumber),
					craftmyapp_actionmethodname = "Add_Receivable",
					createduser = new Guid(loginUserId)
				};

				var bookingDepositResult = await ApiClient.Post_ApiValuesGetString(getHttpClient(), "api/Receivable/Add_Receivable", bookingDepositModel);
				if (!(bookingDepositResult ?? "").Replace("\"", "").Contains("201.1"))
					return bookingDepositResult;
			}

			if (costPerDay <= 0)
				return directRoom.Role + " room cost is not configured for the selected room.";

			var coveragePercent = 0m;
			if (!isAttendantRoom)
				packageRoomCoverage.TryGetValue(directRoom.RoomTypeId, out coveragePercent);
			var chargeDayIndex = 0;
			for (var chargeDate = directRoom.FromDate.Date; chargeDate <= directRoom.ToDate.Date; chargeDate = chargeDate.AddDays(1), chargeDayIndex++)
			{
				var dailyCharge = costPerDay;
				if (!isAttendantRoom && chargeDayIndex < packageCoveredDays && coveragePercent > 0)
					dailyCharge = costPerDay * ((100 - Math.Max(0, Math.Min(100, coveragePercent))) / 100);
				if (dailyCharge <= 0)
					continue;
				var receivableModel = new ReceivableModel
				{
					Receivableid = Guid.NewGuid(),
					tenantid = sourceModel.tenantid,
					receivabledate = chargeDate,
					patientname = sourceModel.patientname,
					ipdnumber = sourceModel.IPDApplicationFormid,
					receivablefor = "Room",
					room = directRoom.RoomId,
					amount = dailyCharge,
					remarks = "Direct IPD " + directRoom.Role.ToLowerInvariant() + " room charge"
						+ (string.IsNullOrWhiteSpace(roomNumber) ? "" : " - Room " + roomNumber)
						+ " (" + chargeDate.ToString("dd/MM/yyyy") + ")",
					craftmyapp_actionmethodname = "Add_Receivable",
					createduser = new Guid(loginUserId)
				};

				var result = await ApiClient.Post_ApiValuesGetString(getHttpClient(), "api/Receivable/Add_Receivable", receivableModel);
				if (!(result ?? "").Replace("\"", "").Contains("201.1"))
					return result;
			}

			return "Success";
		}

		private async Task<string> CreateDirectAdmissionPackageReceivable(IPDApplicationFormModel sourceModel, decimal packageCost, DateTime admitDate, DateTime toDate, string loginUserId)
		{
			var receivableModel = new ReceivableModel
			{
				Receivableid = Guid.NewGuid(),
				tenantid = sourceModel.tenantid,
				receivabledate = admitDate.Date,
				patientname = sourceModel.patientname,
				ipdnumber = sourceModel.IPDApplicationFormid,
				receivablefor = "Package",
				package = sourceModel.packagename,
				amount = packageCost,
				remarks = "Direct IPD package charge (" + admitDate.ToString("dd/MM/yyyy") + " to " + toDate.ToString("dd/MM/yyyy") + ")",
				craftmyapp_actionmethodname = "Add_Receivable",
				createduser = new Guid(loginUserId)
			};

			var result = await ApiClient.Post_ApiValuesGetString(getHttpClient(), "api/Receivable/Add_Receivable", receivableModel);
			return (result ?? "").Replace("\"", "").Contains("201.1") ? "Success" : result;
		}

		private static JToken ParseFirstJsonToken(string json)
		{
			if (string.IsNullOrWhiteSpace(json) || json.Length <= 2)
				return null;
			var token = JToken.Parse(json);
			return token.Type == JTokenType.Array ? token.First : token;
		}

		private static JArray ParseJsonArrayOrEmpty(string json)
		{
			if (string.IsNullOrWhiteSpace(json) || json.Length <= 2)
				return new JArray();

			var token = JToken.Parse(json);
			if (token.Type == JTokenType.Array)
				return (JArray)token;

			if (token.Type == JTokenType.Object)
			{
				var detail = token["detail"] ?? token["data"] ?? token["rows"] ?? token["result"];
				if (detail is JArray detailArray)
					return detailArray;

				return new JArray(token);
			}

			return new JArray();
		}

		private async Task<string> GetDirectRoomTypeGenderMismatch(Guid roomTypeId, string patientGender, string role, string loginUserId)
		{
			var normalizedPatientGender = NormalizeDirectAdmissionGender(patientGender);
			if (string.IsNullOrWhiteSpace(normalizedPatientGender) || roomTypeId == Guid.Empty)
				return "";

			var roomTypeJson = await ApiClient.Get_ApiValues(getHttpClient(),
				"api/RoomType/getById_RoomType?RoomTypeid=" + roomTypeId
				+ "&loginUserID=" + loginUserId);
			var roomTypeToken = ParseFirstJsonToken(roomTypeJson);
			var suitability = (roomTypeToken?["gendersuitability"] ?? roomTypeToken?["Gendersuitability"] ?? roomTypeToken?["genderSuitability"] ?? roomTypeToken?["GenderSuitability"])?.ToString();
			var allowedGenders = GetDirectAdmissionAllowedGenders(suitability);
			if (!allowedGenders.Any() || allowedGenders.Contains(normalizedPatientGender))
				return "";

			var roomTypeName = (roomTypeToken?["name"] ?? roomTypeToken?["Name"] ?? roomTypeToken?["roomtypename"] ?? roomTypeToken?["Roomtypename"])?.ToString();
			if (string.IsNullOrWhiteSpace(roomTypeName))
				roomTypeName = "Selected " + role.ToLowerInvariant() + " room type";

			return roomTypeName + " is suitable for " + string.Join(", ", allowedGenders.Select(ToDirectAdmissionGenderLabel)) + " patient(s). Selected patient gender is " + ToDirectAdmissionGenderLabel(normalizedPatientGender) + ".";
		}

		private static string NormalizeDirectAdmissionGender(string value)
		{
			var text = (value ?? "").Trim().ToLowerInvariant();
			if (string.IsNullOrWhiteSpace(text))
				return "";
			if (text.Contains("female") || text == "f")
				return "female";
			if (text.Contains("male") || text == "m")
				return "male";
			if (text.Contains("other"))
				return "other";
			return "";
		}

		private static List<string> GetDirectAdmissionAllowedGenders(string value)
		{
			var text = (value ?? "").Trim();
			if (string.IsNullOrWhiteSpace(text))
				return new List<string>();
			var lower = text.ToLowerInvariant();
			if (lower == "all" || lower == "any" || lower == "both" || lower == "common")
				return new List<string>();

			return text
				.Split(new[] { ',', '|', '/', ';', '&' }, StringSplitOptions.RemoveEmptyEntries)
				.SelectMany(part => part.Split(new[] { " and ", " And ", " AND " }, StringSplitOptions.RemoveEmptyEntries))
				.Select(NormalizeDirectAdmissionGender)
				.Where(gender => !string.IsNullOrWhiteSpace(gender))
				.Distinct()
				.ToList();
		}

		private static string ToDirectAdmissionGenderLabel(string value)
		{
			if (value == "female")
				return "Female";
			if (value == "male")
				return "Male";
			if (value == "other")
				return "Other";
			return value ?? "";
		}

		private static bool IsAllowedDirectMedicalRecordFile(IFormFile file)
		{
			if (file == null || file.Length <= 0)
				return false;

			var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
			var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
			{
				".pdf", ".doc", ".docx",
				".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp", ".tif", ".tiff"
			};

			if (string.IsNullOrWhiteSpace(extension) || !allowedExtensions.Contains(extension))
				return false;

			var header = new byte[12];
			using (var stream = file.OpenReadStream())
			{
				var bytesRead = stream.Read(header, 0, header.Length);
				if (bytesRead < 4)
					return false;
			}

			bool StartsWith(params byte[] signature) =>
				header.Length >= signature.Length &&
				signature.Select((value, index) => header[index] == value).All(matches => matches);

			switch (extension)
			{
				case ".pdf":
					return StartsWith(0x25, 0x50, 0x44, 0x46); // %PDF
				case ".doc":
					return StartsWith(0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1);
				case ".docx":
					return StartsWith(0x50, 0x4B, 0x03, 0x04); // ZIP/Office Open XML
				case ".jpg":
				case ".jpeg":
					return StartsWith(0xFF, 0xD8, 0xFF);
				case ".png":
					return StartsWith(0x89, 0x50, 0x4E, 0x47);
				case ".gif":
					return StartsWith(0x47, 0x49, 0x46, 0x38);
				case ".bmp":
					return StartsWith(0x42, 0x4D);
				case ".webp":
					return StartsWith(0x52, 0x49, 0x46, 0x46) &&
						header[8] == 0x57 && header[9] == 0x45 && header[10] == 0x42 && header[11] == 0x50;
				case ".tif":
				case ".tiff":
					return StartsWith(0x49, 0x49, 0x2A, 0x00) || StartsWith(0x4D, 0x4D, 0x00, 0x2A);
				default:
					return false;
			}
		}

		private async Task<string> ValidateAllotRoomPostBookingLimits(IPDApplicationFormModel model, string loginUserId)
		{
			if (model?.room == null)
				return "";

			var limitsByRoomType = new Dictionary<Guid, int>();
			foreach (var room in model.room)
			{
				if (!room.roomtype.HasValue || room.roomtype.Value == Guid.Empty || !room.fromdate.HasValue || !room.todate.HasValue)
					continue;

				var roomTypeId = room.roomtype.Value;
				if (!limitsByRoomType.TryGetValue(roomTypeId, out var postBookingLimit))
				{
					var roomTypeJson = await ApiClient.Get_ApiValues(getHttpClient(),
						"api/RoomType/getById_RoomType?RoomTypeid=" + roomTypeId
						+ "&loginUserID=" + loginUserId);
					var roomType = ParseFirstJsonToken(roomTypeJson);
					var rawLimit = roomType?["prebookingdaylimit"] ?? roomType?["Prebookingdaylimit"] ?? roomType?["PreBookingDayLimit"];
					postBookingLimit = rawLimit != null && int.TryParse(rawLimit.ToString(), out var parsedLimit) && parsedLimit > 0
						? parsedLimit
						: 120;
					limitsByRoomType[roomTypeId] = postBookingLimit;
				}

				var minimumDate = DateTime.Today;
				var maximumDate = minimumDate.AddDays(postBookingLimit);
				if (room.fromdate.Value.Date < minimumDate || room.todate.Value.Date < minimumDate)
					return "Room booking dates cannot be earlier than the current date.";
				if (room.fromdate.Value.Date > maximumDate || room.todate.Value.Date > maximumDate)
					return "Room booking dates cannot exceed the selected Room Type post-booking limit of " + postBookingLimit + " days.";
			}

			return "";
		}

		public virtual async Task<IActionResult> Allot_Room(string IPDApplicationFormid)
			  {

                    string redirectTo="";
                    if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
                            DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
                            DataView dv = new DataView(NalamVazharole_JSON);
                            dv.RowFilter = "controllername='IPDApplicationForm' AND viewname='list'";

                            if(dv.Count  >0){
                                redirectTo = dv[0]["actionmethodname"] as string;
							 
                            }

                            try{
                                     var jsonObjIPDApplicationForm = await ApiClient.Get_ApiValues(getHttpClient(), "api/IPDApplicationForm/getById_IPDApplicationForm?IPDApplicationFormid="+IPDApplicationFormid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                                if(jsonObjIPDApplicationForm.Length > 2)
                                {
                                  
                                    var model = JsonConvert.DeserializeObject<IPDApplicationFormModel>(jsonObjIPDApplicationForm);

                                    // Booking deposit is required by default during room allotment.
                                    // Front Desk can explicitly clear the checkbox when it is not required.
                                    if (model != null)
                                        model.isbookingdepositmandatory = true;
                                     
                                    return View(model);
                                }
                                else
                                {
                    
                                    TempData["message"] = "Data Not Found - Contact Administrator";
                                    return RedirectToAction(redirectTo);
						 
                                }

                            }catch(Exception ex){
                               _logger.LogError(ex,"An exception occurred in - IPDApplicationForm / Allot_Room, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                                TempData["errMessage"] = "Error while fetching data - Contact Administrator";
                                return RedirectToAction(redirectTo);
                            }

                    }
                    TempData["errMessage"] = "Session Expired";
                    return RedirectToAction("Logout", "users");
                }
		[HttpPost()]
		public virtual async Task<string> Allot_Room(IPDApplicationFormModel model, IFormCollection collection)
		{
			string strReturnMessage = "";
			try
			{
				model.craftmyapp_actionmethodname = "Allot_Room";

				if (HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
					model.modifieduser = new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
				else
					return "Session Expired";

				// Allot_Room only needs IPDApplicationFormid + room rows - clear all other field validations
				ModelState.Clear();

				ModelState.Remove("uploadpassportcopy");
				ModelState.Remove("uploadvisacopy");
				ModelState.Remove("consentfile");

				var postBookingValidation = await ValidateAllotRoomPostBookingLimits(
					model,
					HttpContext.Session.GetString("NalamVazhaloginUserID"));
				if (!string.IsNullOrWhiteSpace(postBookingValidation))
					return postBookingValidation;

				if (ModelState.IsValid)
				{
					IPDApplicationFormModelValidator validator = new IPDApplicationFormModelValidator();
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

						var files = Request.Form.Files;
						foreach (var file in files)
						{
							var filename = ContentDispositionHeaderValue
							.Parse(file.ContentDisposition)
							.FileName
							.Trim('"');
							string fileExtention = "." + filename.Split('.').Last();
							Random rnd = new Random();
							string uploadFileName = System.Text.RegularExpressions.Regex.Replace(filename.Split('.').First(), @"[^0-9a-zA-Z_.]+", "").Replace(" ", String.Empty) + "_" + "IPDApplicationForm_" + rnd.Next(1, 10000).ToString() + DateTime.Now.ToString("ddMMyyHHmmss") + fileExtention;
							if (fileExtention != ".")
							{
								Stream stream = file.OpenReadStream();
								string fileURL = await util.fileSystem.UploadFileAsync(stream, Configuration.GetSection("AzureBlobStorageSetttings:folderName").Value, uploadFileName);
								string row_id_medicalrecords_medicalrecordfile = file.Name.Replace("medicalrecords_medicalrecordfile_cma_uploader", "");
								if (file.Name == "medicalrecords_medicalrecordfile_cma_uploader" + row_id_medicalrecords_medicalrecordfile)
								{
									var dependent_model = (from x in model.medicalrecords.OfType<IPDApplicationForm_medicalrecordsModel>() where x.cma_client_row_id == row_id_medicalrecords_medicalrecordfile select x).FirstOrDefault();
									dependent_model.medicalrecordfile += "|" + uploadFileName + "|";
								}
								if (file.Name == "uploadpassportcopy")
								{
									uploadFileName = "|" + fileURL + "|";
									model.uploadpassportcopy += uploadFileName;
								}
								if (file.Name == "uploadvisacopy")
								{
									uploadFileName = "|" + fileURL + "|";
									model.uploadvisacopy += uploadFileName;
								}
								if (file.Name == "consentfile")
								{
									uploadFileName = "|" + fileURL + "|";
									model.consentfile += uploadFileName;
								}
							}
						}



						strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(), "api/IPDApplicationForm/Allot_Room", model);


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
				_logger.LogError(ex, "An exception occurred in - IPDApplicationForm / Allot_Room, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

				strReturnMessage = ex.Message;
			}
			ViewData["message"] = strReturnMessage;
			if (strReturnMessage.Replace("\"", "") == "201.1")
			{
				TempData["message"] = "Success";

				return "Success";
			}
			else if (strReturnMessage.StartsWith("BadRequest", StringComparison.OrdinalIgnoreCase))
			{
				strReturnMessage = strReturnMessage.Replace("\"", "").Replace("BadRequest :", "");
				TempData["message"] = strReturnMessage;

				return strReturnMessage;
			}
			else
			{
				if (strReturnMessage == "401.1")
					strReturnMessage = "Authorization Failed";

				return strReturnMessage;
			}

		}
        public virtual async Task<IActionResult> Transfer_Room(string IPDApplicationFormid)
        {
            string redirectTo = "";
            if (HttpContext.Session.GetString("NalamVazharole_JSON") != null)
            {
                DataTable NalamVazharole_JSON = HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
                DataView dv = new DataView(NalamVazharole_JSON);
                dv.RowFilter = "controllername='IPDApplicationForm' AND viewname='list'";
                if (dv.Count > 0)
                    redirectTo = dv[0]["actionmethodname"] as string;

                try
                {
                    var jsonObjIPDApplicationForm = await ApiClient.Get_ApiValues(getHttpClient(), "api/IPDApplicationForm/getById_IPDApplicationForm?IPDApplicationFormid=" + IPDApplicationFormid + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    if (jsonObjIPDApplicationForm.Length > 2)
                    {
                        var model = JsonConvert.DeserializeObject<IPDApplicationFormModel>(jsonObjIPDApplicationForm);

                        ViewBag.PatientGender = "";
                        ViewBag.AttendantGender = "";

                        try
                        {
                            var allInfoJson = await ApiClient.Get_ApiValues(
                                getHttpClient(),
                                "api/IPDApplicationForm/getById_allinfo_IPDApplicationForm?IPDApplicationFormid=" + IPDApplicationFormid
                            );

                            if (!string.IsNullOrWhiteSpace(allInfoJson) && allInfoJson.Length > 2)
                            {
                                JToken allInfoToken = JToken.Parse(allInfoJson);

                                // API returns array, so take first record
                                if (allInfoToken.Type == JTokenType.Array)
                                {
                                    allInfoToken = allInfoToken.FirstOrDefault();
                                }

                                if (allInfoToken != null)
                                {
                                    // Patient Gender
                                    ViewBag.PatientGender =
                                        allInfoToken["gender"]?.ToString()
                                        ?? allInfoToken["Gender"]?.ToString()
                                        ?? "";

                                    // Attendant info key from allinfo API
                                    var attendantInfoToken =
                                        allInfoToken["automaton_IPDApplicationForm_attendantinfo"]
                                        ?? allInfoToken["attendantinfo"]
                                        ?? allInfoToken["attendantInfo"]
                                        ?? allInfoToken["AttendantInfo"];

                                    if (attendantInfoToken != null)
                                    {
                                        JToken attendantArrayToken = attendantInfoToken;

                                        // In your response this is coming as JSON string, so parse it
                                        if (attendantInfoToken.Type == JTokenType.String)
                                        {
                                            var attendantInfoText = attendantInfoToken.ToString();

                                            if (!string.IsNullOrWhiteSpace(attendantInfoText) && attendantInfoText != "[]")
                                            {
                                                attendantArrayToken = JToken.Parse(attendantInfoText);
                                            }
                                        }

                                        if (attendantArrayToken != null &&
                                            attendantArrayToken.Type == JTokenType.Array &&
                                            attendantArrayToken.Any())
                                        {
                                            var firstAttendant = attendantArrayToken.First();

                                            ViewBag.AttendantGender =
                                                firstAttendant["Gender"]?.ToString()
                                                ?? firstAttendant["gender"]?.ToString()
                                                ?? "";
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ViewBag.PatientGender = "";
                            ViewBag.AttendantGender = "";

                            _logger.LogError(
                                ex,
                                "Error while reading patient/attendant gender in Transfer_Room"
                            );
                        }

                        var roomJson = await ApiClient.Get_ApiValues(getHttpClient(), "api/IPDApplicationForm/getById_room?IPDApplicationFormid=" + IPDApplicationFormid + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
                        if (!string.IsNullOrWhiteSpace(roomJson) && roomJson.Length > 2)
                            model.room = JsonConvert.DeserializeObject<List<IPDApplicationForm_roomModel>>(roomJson);

                        return View(model);
                    }

                    TempData["message"] = "Data Not Found - Contact Administrator";
                    return RedirectToAction(redirectTo);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An exception occurred in - IPDApplicationForm / Transfer_Room, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));
                    TempData["errMessage"] = "Error while fetching data - Contact Administrator";
                    return RedirectToAction(redirectTo);
                }
            }

            TempData["errMessage"] = "Session Expired";
            return RedirectToAction("Logout", "users");
        }

        [HttpPost()]
		public virtual async Task<string> Transfer_Room(IPDApplicationFormModel model, IFormCollection collection)
		{
			string strReturnMessage = "";
			try
			{
				model.craftmyapp_actionmethodname = "Transfer_Room";
				if (HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
					model.modifieduser = new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
				else
					return "Session Expired";

				if (!model.tenantid.HasValue || model.tenantid == Guid.Empty)
				{
					var chosenTenantId = HttpContext.Session.GetString("NalamVazhachoosedtenantid");
					if (Guid.TryParse(chosenTenantId, out var tenantGuid))
						model.tenantid = tenantGuid;
				}

				ModelState.Clear();

				if (model.IPDApplicationFormid == null || model.room == null || !model.room.Any())
					return "Transfer room details are required";

				strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(), "api/IPDApplicationForm/Transfer_Room", model);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - IPDApplicationForm / Transfer_Room, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));
				strReturnMessage = ex.Message;
			}

			if (strReturnMessage.Replace("\"", "") == "201.1")
			{
				TempData["message"] = "Success";
				return "Success";
			}

			if (strReturnMessage.StartsWith("BadRequest", StringComparison.OrdinalIgnoreCase))
				strReturnMessage = strReturnMessage.Replace("\"", "").Replace("BadRequest :", "");
			else if (strReturnMessage == "401.1")
				strReturnMessage = "Authorization Failed";

			return strReturnMessage;
		}

        public virtual async Task<IActionResult> Extend_Stay(string IPDApplicationFormid)
        {
            string redirectTo = "";

            if (HttpContext.Session.GetString("NalamVazharole_JSON") != null)
            {
                DataTable NalamVazharole_JSON = HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
                DataView dv = new DataView(NalamVazharole_JSON);
                dv.RowFilter = "controllername='IPDApplicationForm' AND viewname='list'";

                if (dv.Count > 0)
                    redirectTo = dv[0]["actionmethodname"] as string;

                try
                {
                    var jsonObjIPDApplicationForm = await ApiClient.Get_ApiValues(
                        getHttpClient(),
                        "api/IPDApplicationForm/getById_IPDApplicationForm?IPDApplicationFormid=" + IPDApplicationFormid +
                        "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID")
                    );

                    if (jsonObjIPDApplicationForm.Length > 2)
                    {
                        var model = JsonConvert.DeserializeObject<IPDApplicationFormModel>(jsonObjIPDApplicationForm);

                        ViewBag.PatientGender = "";
                        ViewBag.AttendantGender = "";

                        try
                        {
                            var allInfoJson = await ApiClient.Get_ApiValues(
                                getHttpClient(),
                                "api/IPDApplicationForm/getById_allinfo_IPDApplicationForm?IPDApplicationFormid=" + IPDApplicationFormid
                            );

                            if (!string.IsNullOrWhiteSpace(allInfoJson) && allInfoJson.Length > 2)
                            {
                                JToken allInfoToken = JToken.Parse(allInfoJson);

                                if (allInfoToken.Type == JTokenType.Array)
                                {
                                    allInfoToken = allInfoToken.FirstOrDefault();
                                }

                                if (allInfoToken != null)
                                {
                                    ViewBag.PatientGender =
                                        allInfoToken["gender"]?.ToString()
                                        ?? allInfoToken["Gender"]?.ToString()
                                        ?? "";

                                    var attendantInfoToken =
                                        allInfoToken["automaton_IPDApplicationForm_attendantinfo"]
                                        ?? allInfoToken["attendantinfo"]
                                        ?? allInfoToken["attendantInfo"]
                                        ?? allInfoToken["AttendantInfo"];

                                    if (attendantInfoToken != null)
                                    {
                                        JToken attendantArrayToken = attendantInfoToken;

                                        if (attendantInfoToken.Type == JTokenType.String)
                                        {
                                            var attendantInfoText = attendantInfoToken.ToString();

                                            if (!string.IsNullOrWhiteSpace(attendantInfoText) && attendantInfoText != "[]")
                                            {
                                                attendantArrayToken = JToken.Parse(attendantInfoText);
                                            }
                                        }

                                        if (attendantArrayToken != null &&
                                            attendantArrayToken.Type == JTokenType.Array &&
                                            attendantArrayToken.Any())
                                        {
                                            var firstAttendant = attendantArrayToken.First();

                                            ViewBag.AttendantGender =
                                                firstAttendant["Gender"]?.ToString()
                                                ?? firstAttendant["gender"]?.ToString()
                                                ?? "";
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ViewBag.PatientGender = "";
                            ViewBag.AttendantGender = "";

                            _logger.LogError(ex, "Error while reading patient/attendant gender in IPDApplicationForm / Extend_Stay");
                        }

                        var roomJson = await ApiClient.Get_ApiValues(
                            getHttpClient(),
                            "api/IPDApplicationForm/getById_room?IPDApplicationFormid=" + IPDApplicationFormid +
                            "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID")
                        );

                        if (!string.IsNullOrWhiteSpace(roomJson) && roomJson.Length > 2)
                        {
                            model.room = JsonConvert.DeserializeObject<List<IPDApplicationForm_roomModel>>(roomJson);
                        }

                        return View(model);
                    }

                    TempData["message"] = "Data Not Found - Contact Administrator";
                    return RedirectToAction(redirectTo);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "An exception occurred in - IPDApplicationForm / Extend_Stay, Error Message : " +
                        (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message)
                    );

                    TempData["errMessage"] = "Error while fetching data - Contact Administrator";
                    return RedirectToAction(redirectTo);
                }
            }

            TempData["errMessage"] = "Session Expired";
            return RedirectToAction("Logout", "users");
        }

        [HttpPost()]
		public virtual async Task<string> Extend_Stay(IPDApplicationFormModel model, IFormCollection collection)
		{
			string strReturnMessage = "";
			try
			{
				model.craftmyapp_actionmethodname = "Extend_Stay";
				if (HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
					model.modifieduser = new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
				else
					return "Session Expired";

				if (!model.tenantid.HasValue || model.tenantid == Guid.Empty)
				{
					var chosenTenantId = HttpContext.Session.GetString("NalamVazhachoosedtenantid");
					if (Guid.TryParse(chosenTenantId, out var tenantGuid))
						model.tenantid = tenantGuid;
				}

				ModelState.Clear();

				if (model.IPDApplicationFormid == null || model.room == null || !model.room.Any())
					return "Extend stay details are required";

				strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(), "api/IPDApplicationForm/Extend_Stay", model);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - IPDApplicationForm / Extend_Stay, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));
				strReturnMessage = ex.Message;
			}

			if (strReturnMessage.Replace("\"", "") == "201.1")
			{
				TempData["message"] = "Success";
				return "Success";
			}

			if (strReturnMessage.StartsWith("BadRequest", StringComparison.OrdinalIgnoreCase))
				strReturnMessage = strReturnMessage.Replace("\"", "").Replace("BadRequest :", "");
			else if (strReturnMessage == "401.1")
				strReturnMessage = "Authorization Failed";

			return strReturnMessage;
		}

		public virtual async Task<IActionResult> Confirm_Arrival(string IPDApplicationFormid)
		{

			string redirectTo = "";
			if (HttpContext.Session.GetString("NalamVazharole_JSON") != null)
			{
				DataTable NalamVazharole_JSON = HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
				DataView dv = new DataView(NalamVazharole_JSON);
				dv.RowFilter = "controllername='IPDApplicationForm' AND viewname='list'";

				if (dv.Count > 0)
				{
					redirectTo = dv[0]["actionmethodname"] as string;

				}

				try
				{
					var jsonObjIPDApplicationForm = await ApiClient.Get_ApiValues(getHttpClient(), "api/IPDApplicationForm/getById_IPDApplicationForm?IPDApplicationFormid=" + IPDApplicationFormid + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
					if (jsonObjIPDApplicationForm.Length > 2)
					{

						var model = JsonConvert.DeserializeObject<IPDApplicationFormModel>(jsonObjIPDApplicationForm);




						return View(model);
					}
					else
					{

						TempData["message"] = "Data Not Found - Contact Administrator";
						return RedirectToAction(redirectTo);

					}

				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "An exception occurred in - IPDApplicationForm / Confirm_Arrival, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

					TempData["errMessage"] = "Error while fetching data - Contact Administrator";
					return RedirectToAction(redirectTo);
				}

			}
			TempData["errMessage"] = "Session Expired";
			return RedirectToAction("Logout", "users");
		}
		[HttpPost()]
		public virtual async Task<string> Confirm_Arrival(IPDApplicationFormModel model, IFormCollection collection)
		{
			string strReturnMessage = "";
			try
			{
				ModelState.Remove("IPDApplicationFormid");
				ModelState.Remove("craftmyapp_actionmethodname");
				model.craftmyapp_actionmethodname = "Confirm_Arrival";


				if (HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
					model.modifieduser = new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
				else
					return "Session Expired";
 


						strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(), "api/IPDApplicationForm/Confirm_Arrival", model);
					 
				 
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - IPDApplicationForm / Confirm_Arrival, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

				strReturnMessage = ex.Message;
			}
			ViewData["message"] = strReturnMessage;
			if (strReturnMessage.Replace("\"", "") == "201.1")
			{
				TempData["message"] = "Success";

				return "Success";
			}
			else if (strReturnMessage.StartsWith("BadRequest", StringComparison.OrdinalIgnoreCase))
			{
				strReturnMessage = strReturnMessage.Replace("\"", "").Replace("BadRequest :", "");
				TempData["message"] = strReturnMessage;

				return strReturnMessage;
			}
			else
			{
				if (strReturnMessage == "401.1")
					strReturnMessage = "Authorization Failed";

				return strReturnMessage;
			}

		}


		public virtual async Task<IActionResult> Update_IPD_Application_Form(string IPDApplicationFormid)
			  {

                    string redirectTo="";
                    if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
                            DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
                            DataView dv = new DataView(NalamVazharole_JSON);
                            dv.RowFilter = "controllername='IPDApplicationForm' AND viewname='list'";

                            if(dv.Count  >0){
                                redirectTo = dv[0]["actionmethodname"] as string;
							 
                            }

                            try{
                                     var jsonObjIPDApplicationForm = await ApiClient.Get_ApiValues(getHttpClient(), "api/IPDApplicationForm/getById_IPDApplicationForm?IPDApplicationFormid="+IPDApplicationFormid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                                if(jsonObjIPDApplicationForm.Length > 2)
                                {
                                  
                                    var model = JsonConvert.DeserializeObject<IPDApplicationFormModel>(jsonObjIPDApplicationForm);


                
                                     
                                    return View("Add_IPD_Application_Form",model);
                                }
                                else
                                {
                    
                                    TempData["message"] = "Data Not Found - Contact Administrator";
                                    return RedirectToAction(redirectTo);
						 
                                }

                            }catch(Exception ex){
                               _logger.LogError(ex,"An exception occurred in - IPDApplicationForm / Update_IPD_Application_Form, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                                TempData["errMessage"] = "Error while fetching data - Contact Administrator";
                                return RedirectToAction(redirectTo);
                            }

                    }
                    TempData["errMessage"] = "Session Expired";
                    return RedirectToAction("Logout", "users");
                }	

			  // Kept-for-links alias: Update_IPD_Application_Form already renders the redesigned
			  // view (it returns View("Add_IPD_Application_Form", model)), so this just forwards.
			  public virtual async Task<IActionResult> Update_IPD_Application_Form_New(string IPDApplicationFormid)
			  {
					return await Update_IPD_Application_Form(IPDApplicationFormid);
			  }
			  [HttpPost()]
				public virtual async Task<string> Update_IPD_Application_Form(IPDApplicationFormModel model, IFormCollection collection)
				{
					string strReturnMessage = "";
					try
					{
							ModelState.Remove("IPDApplicationFormid");
                            ModelState.Remove("craftmyapp_actionmethodname");
                             model.craftmyapp_actionmethodname="Update_IPD_Application_Form";
							
							
							if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
					model.modifieduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
					else
					return "Session Expired";
							
                            model.uploadpassportcopy = collection["uploadpassportcopy_existing"];
model.uploadvisacopy = collection["uploadvisacopy_existing"];
model.consentfile = collection["consentfile_existing"];

                            ModelState.Remove("uploadpassportcopy");
ModelState.Remove("uploadvisacopy");
ModelState.Remove("consentfile");

                            var isDraft = string.Equals(model.bookingstatus, "Draft", StringComparison.OrdinalIgnoreCase);
                            if (isDraft)
                                ModelState.Clear();

							if (ModelState.IsValid)
							{
									IPDApplicationFormModelValidator validator = new IPDApplicationFormModelValidator();
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
                                        
										var files = Request.Form.Files;
foreach (var file in files) 
{
var filename = ContentDispositionHeaderValue
.Parse(file.ContentDisposition)
.FileName
.Trim('"'); 
string fileExtention = "." + filename.Split('.').Last(); 
Random rnd = new Random();
string uploadFileName = System.Text.RegularExpressions.Regex.Replace(filename.Split('.').First(), @"[^0-9a-zA-Z_.]+", "").Replace(" ", String.Empty)+"_"+"IPDApplicationForm_" +rnd.Next(1, 10000).ToString() + DateTime.Now.ToString("ddMMyyHHmmss")+ fileExtention;
if (fileExtention != ".")
{
Stream stream = file.OpenReadStream();
string fileURL=await util.fileSystem.UploadFileAsync(stream, Configuration.GetSection("AzureBlobStorageSetttings:folderName").Value, uploadFileName);
string row_id_medicalrecords_medicalrecordfile = file.Name.Replace("medicalrecords_medicalrecordfile_cma_uploader", "");
if (file.Name == "medicalrecords_medicalrecordfile_cma_uploader"+row_id_medicalrecords_medicalrecordfile)
{
var dependent_model = (from x in model.medicalrecords.OfType<IPDApplicationForm_medicalrecordsModel>() where x.cma_client_row_id == row_id_medicalrecords_medicalrecordfile select x).FirstOrDefault();
                                        dependent_model.medicalrecordfile += "|" + uploadFileName + "|";
}
if (file.Name == "uploadpassportcopy")
{
uploadFileName = "|" + fileURL +"|";
model.uploadpassportcopy +=  uploadFileName ;
}
if (file.Name == "uploadvisacopy")
{
uploadFileName = "|" + fileURL +"|";
model.uploadvisacopy +=  uploadFileName ;
}
if (file.Name == "consentfile")
{
uploadFileName = "|" + fileURL +"|";
model.consentfile +=  uploadFileName ;
}
}
}

                                        
                                        
                                        strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/IPDApplicationForm/Update_IPD_Application_Form", model);
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
                      _logger.LogError(ex,"An exception occurred in - IPDApplicationForm / Update_IPD_Application_Form, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
						strReturnMessage = ex.Message;
					}
					ViewData["message"] = strReturnMessage;
					    if(strReturnMessage.Replace("\"", "")=="201.1"){
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

			// ── Update_Medical_Info (GET) ─────────────────────────────────────────
			[HttpGet()]
			public virtual async Task<string> Get_IPD_Application_Form_JSON(string IPDApplicationFormid)
			{
				return await ApiClient.Get_ApiValues(getHttpClient(),
					"api/IPDApplicationForm/getById_IPDApplicationForm?IPDApplicationFormid=" + IPDApplicationFormid
					+ "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
			}

			[HttpGet()]
			public virtual async Task<string> Get_Direct_IPD_Draft_JSON(string IPDApplicationFormid)
			{
				try
				{
					if (string.IsNullOrWhiteSpace(IPDApplicationFormid)) return "{}";

					var loginUserId = HttpContext.Session.GetString("NalamVazhaloginUserID") ?? "";
					var httpClient = getHttpClient();
					var mainJson = await ApiClient.Get_ApiValues(httpClient,
						"api/IPDApplicationForm/getById_IPDApplicationForm?IPDApplicationFormid=" + IPDApplicationFormid
						+ "&loginUserID=" + loginUserId);

					var detail = ParseFirstJsonToken(mainJson) as JObject ?? new JObject();

					var allInfoJson = await ApiClient.Get_ApiValues(httpClient,
						"api/IPDApplicationForm/getById_allinfo_IPDApplicationForm?IPDApplicationFormid=" + IPDApplicationFormid);
					var allInfo = ParseFirstJsonToken(allInfoJson) as JObject;
					if (allInfo != null)
					{
						foreach (var prop in allInfo.Properties())
						{
							if (detail[prop.Name] == null || string.IsNullOrWhiteSpace(detail[prop.Name]?.ToString()))
								detail[prop.Name] = prop.Value;
						}
					}
                var packageToken =
detail.GetValue("packagename", StringComparison.OrdinalIgnoreCase)
?? allInfo?.GetValue("packagename", StringComparison.OrdinalIgnoreCase)
?? detail.GetValue("TreatmentPackageid", StringComparison.OrdinalIgnoreCase)
?? allInfo?.GetValue("TreatmentPackageid", StringComparison.OrdinalIgnoreCase);

                if (packageToken != null &&
                    !string.IsNullOrWhiteSpace(packageToken.ToString()))
                {
                    detail["packagename"] = packageToken;
                }
                var roomPreferenceJson = await ApiClient.Get_ApiValues(httpClient,
						"api/IPDApplicationForm/getById_roompreference?IPDApplicationFormid=" + IPDApplicationFormid
						+ "&loginUserID=" + loginUserId);
					detail["roompreference"] = ParseJsonArrayOrEmpty(roomPreferenceJson);

					var roomJson = await ApiClient.Get_ApiValues(httpClient,
						"api/IPDApplicationForm/getById_room?IPDApplicationFormid=" + IPDApplicationFormid
						+ "&loginUserID=" + loginUserId);
					detail["room"] = ParseJsonArrayOrEmpty(roomJson);

					return detail.ToString(Formatting.None);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "An exception occurred in - IPDApplicationForm / Get_Direct_IPD_Draft_JSON");
					return "{}";
				}
			}

			[HttpPost()]
			public virtual async Task<string> Update_Package_Info([FromBody] IPDPackageUpdateModel model)
			{
				try
				{
					if (model == null || string.IsNullOrWhiteSpace(model.IPDApplicationFormid))
						return "IPDApplicationFormid is required";
					return await ApiClient.Post_ApiValuesGetString(getHttpClient(),
						"api/IPDApplicationForm/Update_IPD_Package_Info", model);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "An exception occurred in - IPDApplicationForm / Update_Package_Info: " + ex.Message);
					return ex.Message;
				}
			}

			public virtual async Task<IActionResult> Update_Medical_Info(string IPDApplicationFormid)
			{
				string redirectTo = "";
				if (HttpContext.Session.GetString("NalamVazharole_JSON") != null)
				{
					DataTable NalamVazharole_JSON = HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
					DataView dv = new DataView(NalamVazharole_JSON);
					dv.RowFilter = "controllername='IPDApplicationForm' AND viewname='list'";
					if (dv.Count > 0)
						redirectTo = dv[0]["actionmethodname"] as string;

					try
					{
						var jsonObj = await ApiClient.Get_ApiValues(getHttpClient(),
							"api/IPDApplicationForm/getById_IPDApplicationForm?IPDApplicationFormid=" + IPDApplicationFormid
							+ "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
						if (jsonObj.Length > 2)
						{
							var model = JsonConvert.DeserializeObject<IPDApplicationFormModel>(jsonObj);
							return View("Update_Medical_Info", model);
						}
						TempData["message"] = "Data Not Found - Contact Administrator";
						return RedirectToAction(redirectTo);
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, "An exception occurred in - IPDApplicationForm / Update_Medical_Info (GET): " + ex.Message);
						TempData["errMessage"] = "Error while fetching data - Contact Administrator";
						return RedirectToAction(redirectTo);
					}
				}
				TempData["errMessage"] = "Session Expired";
				return RedirectToAction("Logout", "users");
			}

			// ── Update_Medical_Info (POST) ────────────────────────────────────────
			[HttpPost()]
			public virtual async Task<string> Update_Medical_Info(IPDApplicationFormModel model, IFormCollection collection)
			{
				string strReturnMessage = "";
				try
				{
					if (HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
						model.modifieduser = new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
					else
						return "Session Expired";

					model.craftmyapp_actionmethodname = "Update_IPD_Application_Medical_Info";
					model.medicalinfo ??= new List<IPDApplicationForm_medicalinfoModel>();
					model.medicationinfo ??= new List<IPDApplicationForm_medicationinfoModel>();
					model.medicalrecords ??= new List<IPDApplicationForm_medicalrecordsModel>();

					// Handle medicalrecords file uploads without loading/updating the full IPD form.
					var files = Request.Form.Files;
					foreach (var file in files)
					{
						var filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
						string fileExtention = "." + filename.Split('.').Last();
						Random rnd = new Random();
						string uploadFileName = System.Text.RegularExpressions.Regex.Replace(filename.Split('.').First(), @"[^0-9a-zA-Z_.]+", "")
							.Replace(" ", string.Empty) + "_IPDApplicationForm_" + rnd.Next(1, 10000).ToString()
							+ DateTime.Now.ToString("ddMMyyHHmmss") + fileExtention;
						if (fileExtention != ".")
						{
							Stream stream = file.OpenReadStream();
							await util.fileSystem.UploadFileAsync(stream,
								Configuration.GetSection("AzureBlobStorageSetttings:folderName").Value, uploadFileName);
							string rowId = file.Name.Replace("medicalrecords_medicalrecordfile_cma_uploader", "");
							if (file.Name == "medicalrecords_medicalrecordfile_cma_uploader" + rowId)
							{
								var dep = (from x in model.medicalrecords.OfType<IPDApplicationForm_medicalrecordsModel>()
										   where x.cma_client_row_id == rowId
										   select x).FirstOrDefault();
								if (dep != null)
									dep.medicalrecordfile += "|" + uploadFileName + "|";
							}
						}
					}

					strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),
						"api/IPDApplicationForm/Update_IPD_Application_Medical_Info", model);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "An exception occurred in - IPDApplicationForm / Update_Medical_Info (POST): " + ex.Message);
					strReturnMessage = ex.Message;
				}

				if (strReturnMessage.Replace("\"", "") == "201.1")
				{
					TempData["message"] = "Success";
					return "Success";
				}
				else if (strReturnMessage.StartsWith("BadRequest", StringComparison.OrdinalIgnoreCase))
				{
					strReturnMessage = strReturnMessage.Replace("\"", "").Replace("BadRequest :", "");
					TempData["message"] = strReturnMessage;
					return strReturnMessage;
				}
				else
				{
					if (strReturnMessage == "401.1")
						strReturnMessage = "Authorization Failed";
					return strReturnMessage;
				}
			}

			public virtual async Task<IActionResult> Remove_IPD_Application_Form(string IPDApplicationFormid)
			{
				var sessionRole = HttpContext.Session.GetString("NalamVazhauserrole") ?? "";
				if (sessionRole.Equals("Health Seeker", StringComparison.OrdinalIgnoreCase))
				{
					TempData["errMessage"] = "IPD application removal is not allowed from this URL.";
					return RedirectToAction("Index", "PatientDashboard");
				}

				string message = "";
				try
				{
						message = await ApiClient.Get_ApiValues(getHttpClient(), "api/IPDApplicationForm/Remove_IPD_Application_Form?IPDApplicationFormid="+IPDApplicationFormid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
						 if(message.Replace("\"","").Contains("201.1"))
						{
							TempData["message"] = "Success";

						}else{
							TempData["errMessage"] = message.Replace("\"","");
						}
						
				
				
				}
				catch (Exception ex)
				{
                     _logger.LogError(ex,"An exception occurred in - IPDApplicationForm / Remove_IPD_Application_Form, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                
					 TempData["errMessage"] = ex.Message;
					 message = ex.Message;
				}

				string redirectTo="";
						if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
					DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
						 DataView dv = new DataView(NalamVazharole_JSON);
						 dv.RowFilter = "controllername='IPDApplicationForm' AND viewname='list'";

						if(dv.Count  >0){
						    redirectTo = dv[0]["actionmethodname"] as string;
							 
						}

					}
				
				return RedirectToAction(redirectTo);
			}

			        public virtual IActionResult Added_IPD_Application_Form()
			        {
				        var sessionRole = HttpContext.Session.GetString("NalamVazhauserrole") ?? "";
				        if (sessionRole.Equals("Health Seeker", StringComparison.OrdinalIgnoreCase))
				        {
					        TempData["errMessage"] = "Please use the patient dashboard to view your IPD applications.";
					        return RedirectToAction("Index", "PatientDashboard");
				        }

				        return View();
			        }
			        	
			        [HttpGet()]
			        public virtual async Task<string> get_Added_IPD_Application_Form(string tenantid
,string patientname
,string bookingstatus
, string verifiedstatus
, int? pagesize=100 , int? pagenumber=0,string searchterm="",string sortFieldsJson="",
string createddate_automatonfrom="", string createddate_automatonto="",
string bookingnumber="", string workflowstatus="", string financialstatus="", string paymentmethod="")
			        {
				        var sessionRole = HttpContext.Session.GetString("NalamVazhauserrole") ?? "";
				        if (sessionRole.Equals("Health Seeker", StringComparison.OrdinalIgnoreCase))
					        return "[]";
				        
				        return await ApiClient.Get_ApiValues(getHttpClient(), "api/IPDApplicationForm/Added_IPD_Application_Form?tenantid="+tenantid+"&patientname="+patientname + "&bookingstatus=" + bookingstatus + "&verifiedstatus="+verifiedstatus+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID")
+ "&pagesize=" + pagesize + "&pagenumber="+ pagenumber + "&searchterm="+ searchterm + "&sort_fields=" + sortFieldsJson
+ "&createddate_automatonfrom=" + System.Net.WebUtility.UrlEncode(createddate_automatonfrom) + "&createddate_automatonto=" + System.Net.WebUtility.UrlEncode(createddate_automatonto)
+ "&bookingnumber=" + System.Net.WebUtility.UrlEncode(bookingnumber) + "&workflowstatus=" + System.Net.WebUtility.UrlEncode(workflowstatus) + "&financialstatus=" + System.Net.WebUtility.UrlEncode(financialstatus) + "&paymentmethod=" + System.Net.WebUtility.UrlEncode(paymentmethod));
			        }
		[HttpGet()]
		public virtual async Task<string> count_of_IPDApplicationForm_bookingstatus(string tenantid
, string patientname
, string bookingstatus
)
		{

			return await ApiClient.Get_ApiValues(getHttpClient(), "api/IPDApplicationForm/count_of_IPDApplicationForm_bookingstatus?tenantid=" + tenantid + "&patientname=" + patientname + "&bookingstatus=" + bookingstatus + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID")
);
		}

		[HttpGet()]
											public virtual async Task<string> get_all_PatientProfile(string tenantid,string searchterm, string pagesize="1000", string pagenumber="1")
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/PatientProfile/get_all_PatientProfile?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											 
											[HttpGet()]
											public virtual async Task<string> get_all_Country(string tenantid)
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/Country/get_all_Country?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											 
											[HttpGet()]
											public virtual async Task<string> get_all_PatientConsent(string tenantid)
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/PatientConsent/get_all_PatientConsent?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											 
											[HttpGet()]
											public virtual async Task<string> get_all_TreatmentPackage(string tenantid)
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/TreatmentPackage/get_all_TreatmentPackage?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											

                                        public virtual IActionResult View_IPD_Application_Form()
                                        {
                                            return View();
                                        }

			        public virtual IActionResult IPD_Application_Forms_for_Review()
			        {
				        return View();
			        }


        [HttpGet()]
        public virtual async Task<string> get_IPD_Room_Calendar_Cards(
    string tenantid,
    string fromdate = "",
    string todate = "")
        {
            var query =
                "api/IPDApplicationForm/get_IPD_Room_Calendar_Cards"
                + "?tenantid="
                + Uri.EscapeDataString(tenantid ?? "")
                + "&fromdate="
                + Uri.EscapeDataString(fromdate ?? "")
                + "&todate="
                + Uri.EscapeDataString(todate ?? "")
                + "&loginUserID="
                + Uri.EscapeDataString(
                    HttpContext.Session.GetString(
                        "NalamVazhaloginUserID"
                    ) ?? ""
                );

            return await ApiClient.Get_ApiValues(
                getHttpClient(),
                query
            );
        }

        [HttpGet()]
        public virtual async Task<string> get_IPD_Application_Forms_for_Review(
string tenantid,
string patientname,
string bookingstatus,
string verifiedstatus,
string createddate_automatonfrom = "",
string createddate_automatonto = "",
int? pagesize = 100,
int? pagenumber = 0,
string searchterm = "",
string sortFieldsJson = "")
        {
            return await ApiClient.Get_ApiValues(
                getHttpClient(),
                "api/IPDApplicationForm/IPD_Application_Forms_for_Review?tenantid=" + tenantid +
                "&patientname=" + patientname +
                "&bookingstatus=" + bookingstatus +
                "&verifiedstatus=" + verifiedstatus +
                "&createddate_automatonfrom=" + createddate_automatonfrom +
                "&createddate_automatonto=" + createddate_automatonto +
                "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID") +
                "&pagesize=" + pagesize +
                "&pagenumber=" + pagenumber +
                "&searchterm=" + searchterm +
                "&sort_fields=" + sortFieldsJson
            );
        }



        [HttpGet()]
			public virtual async Task<string> count_of_IPDApplicationForm(string tenantid
,string patientname, string bookingstatus
)
			{
				
				return await ApiClient.Get_ApiValues(getHttpClient(), "api/IPDApplicationForm/count_of_IPDApplicationForm?tenantid="+tenantid+"&patientname="+patientname + "&bookingstatus=" + bookingstatus + "&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID")
);
			}
			 

            [HttpPost()]
            public virtual async Task<string> verify_IPDApplicationForm([FromBody]IPDApplicationFormReviewModel model)
			{
				string message = "";
				try
				{

							if (HttpContext.Session.GetString("NalamVazhauserrole") == "Doctor"
								&& string.Equals((model.verifiedstatus ?? "").Trim(), "Approved", StringComparison.OrdinalIgnoreCase))
							{
								model.verifiedstatus = "IPD Approved by Doctor";

							}

						
					 	message = await ApiClient.Post_ApiValuesGetString(getHttpClient(), "api/IPDApplicationForm/verify_IPDApplicationForm", model);
						if(message.Replace("\"","")=="201.1")
						{
							TempData["message"] = "Success";

							// Send Provisional Booking email when status set to Provisional Booking
							if (model.verifiedstatus == "Approved" ||model.verifiedstatus =="IPD Appoved by Doctor" || model.verifiedstatus == "Approved by Doctor")
							{
							
							}

						}else{
							TempData["errMessage"] = message.Replace("\"","");
						}

						message=message.Replace("\"","");
						
				
				
				}
				catch (Exception ex)
				{
                    
                      _logger.LogError(ex,"An exception occurred in - IPDApplicationForm / verify_IPDApplicationForm, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                  
					 TempData["errMessage"] = ex.Message;
					 message = ex.Message;
				}
 
				
				return message;
			}

                                        public virtual IActionResult Review_IPD_Application_Form()
                                        {
                                            return View();
                                        }

			        public virtual IActionResult Approved_IPD_Application_Forms()
			        {
				        return View();
			        }
			        	
			        [HttpGet()]
			        public virtual async Task<string> get_Approved_IPD_Application_Forms(string tenantid
,string patientname
, string bookingstatus
, int? pagesize=100 , int? pagenumber=0,string searchterm="",string sortFieldsJson="")
			        {
				        
				        return await ApiClient.Get_ApiValues(getHttpClient(), "api/IPDApplicationForm/Approved_IPD_Application_Forms?tenantid="+tenantid+"&patientname="+patientname+ "&bookingstatus=" + bookingstatus+ "&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID")
+ "&pagesize=" + pagesize + "&pagenumber="+ pagenumber + "&searchterm="+ searchterm + "&sort_fields=" + sortFieldsJson);
			        }
			         

                                        public virtual IActionResult Approved_IPD_Application_Form()
                                        {
                                            return View();
                                        }

				
			  public virtual async Task<string> getById_allinfo_IPDApplicationForm(string IPDApplicationFormid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/IPDApplicationForm/getById_allinfo_IPDApplicationForm?IPDApplicationFormid="+IPDApplicationFormid);
					 
			  }
[HttpGet()]
			    public virtual async Task<string> lookup_IPDApplicationForm_byBookingRef(string tenantid, string bookingreferencenumber)
			    {
				    return await ApiClient.Get_ApiValues(getHttpClient(), "api/IPDApplicationForm/IPD_Application_Forms_for_Review?tenantid="+tenantid+"&searchterm="+bookingreferencenumber+"&pagesize=1&pagenumber=0&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			    }
[HttpGet()]
                    public virtual async Task<string> get_Active_IPD_Application(String patientprofileid)
                    {
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/IPDApplicationForm/get_Active_IPD_Application?patientprofileid="+patientprofileid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }
[HttpGet()]
                    public virtual async Task<string> lookup_IPDApplicationForm_patientname(String tenantid,string searchterm, int? pagesize, int? pagenumber)
                    {
                        
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/IPDApplicationForm/lookup_IPDApplicationForm_patientname?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }
[HttpGet()]
			    public virtual async Task<string> lookup_IPDApplicationForm_countryoforigin()
			    {
                    
				    return await ApiClient.Get_ApiValues(getHttpClient(), "api/IPDApplicationForm/lookup_IPDApplicationForm_countryoforigin?loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			    }
[HttpGet()]
			    public virtual async Task<string> lookup_IPDApplicationForm_passportissuingcountry()
			    {
                    
				    return await ApiClient.Get_ApiValues(getHttpClient(), "api/IPDApplicationForm/lookup_IPDApplicationForm_passportissuingcountry?loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			    }
[HttpGet()]
			    public virtual async Task<string> lookup_IPDApplicationForm_visaissuedcountry()
			    {
                    
				    return await ApiClient.Get_ApiValues(getHttpClient(), "api/IPDApplicationForm/lookup_IPDApplicationForm_visaissuedcountry?loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			    }
[HttpGet()]
			    public virtual async Task<string> lookup_IPDApplicationForm_consentform(String tenantid)
			    {
                    
				    return await ApiClient.Get_ApiValues(getHttpClient(), "api/IPDApplicationForm/lookup_IPDApplicationForm_consentform?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			    }
[HttpGet()]
			    public virtual async Task<string> lookup_IPDApplicationForm_packagename(String tenantid)
			    {

				    return await ApiClient.Get_ApiValues(getHttpClient(), "api/IPDApplicationForm/lookup_IPDApplicationForm_packagename?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			    }

            /// <summary>
            /// Returns the room types (with package-specific costperday) linked to a TreatmentPackage.
            /// Used by Allot_Room to filter the room-type dropdown and apply package pricing.
            /// </summary>
            [HttpGet]
            public virtual async Task<string> get_roomtypes_by_package(string TreatmentPackageid)
            {
                if (string.IsNullOrWhiteSpace(TreatmentPackageid))
                    return "[]";
                return await ApiClient.Get_ApiValues(getHttpClient(),
                    "api/TreatmentPackage/getById_roomtypes?TreatmentPackageid="
                    + Uri.EscapeDataString(TreatmentPackageid)
                    + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
            }

[HttpGet()]
                        public virtual async Task<string> lookup_IPDApplicationForm_medicalinfo_medicalconditionname(string searchterm, int? pagesize, int? pagenumber)
                        {
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/IPDApplicationForm/lookup_IPDApplicationForm_medicalinfo_medicalconditionname?searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                        }
[HttpGet()]
                        public virtual async Task<string> lookup_IPDApplicationForm_roompreference_roomtype(String tenantid)
                        {
                            return await ApiClient.Get_ApiValues(getHttpClient(), "api/IPDApplicationForm/lookup_IPDApplicationForm_roompreference_roomtype?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                        }
                        public virtual async Task<string> lookup_IPDApplicationForm_attendantroompreference_roomtypeatt(String tenantid)
                        {
                            return await ApiClient.Get_ApiValues(getHttpClient(), "api/IPDApplicationForm/lookup_IPDApplicationForm_attendantroompreference_roomtypeatt?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                        }
[HttpGet()]
                        public virtual async Task<string> lookup_IPDApplicationForm_room_roomnumber(String tenantid)
                        {
                            return await ApiClient.Get_ApiValues(getHttpClient(), "api/IPDApplicationForm/lookup_IPDApplicationForm_room_roomnumber?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                        }

[HttpGet()]
			    public virtual async Task<string> lookup_IPDApplicationForm_roomnumber(String tenantid,String roomtype)
			    {
				    return await ApiClient.Get_ApiValues(getHttpClient(), "api/IPDApplicationForm/lookup_IPDApplicationForm_roomnumber?tenantid="+tenantid+"&roomtype="+roomtype+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			    } 
											[HttpGet()]
											public virtual async Task<string> get_all_Room(string tenantid)
											{
											 
											    return await ApiClient.Get_ApiValues(getHttpClient(), "api/Room/get_all_Room?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											
 
											[HttpGet()]
											public virtual async Task<string> get_all_RoomType(string tenantid, string patientGender = null, string attendantGender = null)
											{
												var q = "api/RoomType/get_all_RoomType?tenantid=" + Uri.EscapeDataString(tenantid ?? "")
													+ "&loginUserID=" + Uri.EscapeDataString(HttpContext.Session.GetString("NalamVazhaloginUserID") ?? "");
												if (!string.IsNullOrWhiteSpace(patientGender))
													q += "&patientGender=" + Uri.EscapeDataString(patientGender.Trim());
												if (!string.IsNullOrWhiteSpace(attendantGender))
													q += "&attendantGender=" + Uri.EscapeDataString(attendantGender.Trim());
											    return await ApiClient.Get_ApiValues(getHttpClient(), q);
											}
											
 
                            [HttpGet()]
                            public virtual async Task<string> get_all_MedicalCondition(string tenantid,string searchterm, string pagesize="1000", string pagenumber="1")
                            {

                                return await ApiClient.Get_ApiValues(getHttpClient(), "api/MedicalCondition/get_all_MedicalCondition?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                            }


		// Session-safe HttpClient for unauthenticated (email-link) payment pages
		private HttpClient getAnonymousHttpClient()
		{
			var httpClient = new HttpClient();
			httpClient.BaseAddress = new Uri(url);
			httpClient.DefaultRequestHeaders.Accept.Clear();
			httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
			var token = HttpContext.Session.GetString("NalamVazhatoken") ?? "";
			httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
			var authProvider = HttpContext.Session.GetString("NalamVazhaAuthProvider");
			if (!string.IsNullOrEmpty(authProvider))
				httpClient.DefaultRequestHeaders.Add("AuthProvider", authProvider);
			return httpClient;
		}

		[AllowAnonymous]
		[HttpGet()]
		public virtual async Task<IActionResult> Initiate_Payment(string IPDApplicationFormid)
		{
			try
			{
				var json = await ApiClient.Get_ApiValues(getAnonymousHttpClient(),
					"api/IPDApplicationForm/Get_IPD_Payment_Details?IPDApplicationFormid=" + IPDApplicationFormid +
					"&loginUserID=" + (HttpContext.Session.GetString("NalamVazhaloginUserID") ?? ""));

				if (json.Length > 2)
				{
					var model = JsonConvert.DeserializeObject<NalamVazha.Models.IPDPaymentDetailsModel>(json);
					if (model != null && string.IsNullOrWhiteSpace(model.blocked_room_details_json))
					{
						model.blocked_room_details_json = await BuildBlockedRoomDetailsJsonFallback(IPDApplicationFormid);
					}
					var summaryJson = await ApiClient.Get_ApiValues(getAnonymousHttpClient(),
						"api/IPDApplicationForm/Get_IPD_Billing_Summary?IPDApplicationFormid=" + IPDApplicationFormid);
					ViewBag.BillingSummary = !string.IsNullOrWhiteSpace(summaryJson) && summaryJson.Length > 2
						? JsonConvert.DeserializeObject<NalamVazha.Models.IPDBillingSummaryModel>(summaryJson)
						: null;
					return View(model);
				}
				else
				{
					TempData["errMessage"] = "IPD record not found";
					return View(new NalamVazha.Models.IPDPaymentDetailsModel());
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - IPDApplicationForm / Initiate_Payment: " + ex.Message);
				TempData["errMessage"] = "Error while fetching data - Contact Administrator";
				return View(new NalamVazha.Models.IPDPaymentDetailsModel());
			}
		}



		[AllowAnonymous]
		[HttpGet()]
		public virtual async Task<IActionResult> Manual_Deposit_Receipt(string IPDApplicationFormid,string BillingPaymentid = "")
		{
			try
			{
				var json = await ApiClient.Get_ApiValues(getAnonymousHttpClient(),
					"api/IPDApplicationForm/Get_IPD_Payment_Details?IPDApplicationFormid=" + IPDApplicationFormid +
					"&loginUserID=" + (HttpContext.Session.GetString("NalamVazhaloginUserID") ?? ""));

				if (json.Length > 2)
				{
					var ipdPayment = JsonConvert.DeserializeObject<NalamVazha.Models.IPDPaymentDetailsModel>(json);
					if (ipdPayment != null && string.IsNullOrWhiteSpace(ipdPayment.blocked_room_details_json))
						ipdPayment.blocked_room_details_json = await BuildBlockedRoomDetailsJsonFallback(IPDApplicationFormid);

					// Same as Manual_Deposit_Receipt_Old: "Amount to receive" = patient booking deposit only (not attendant).
					decimal pt = ipdPayment.patient_booking_deposit.GetValueOrDefault();
					if (pt <= 0) pt = ipdPayment.bookingdepositamount.GetValueOrDefault();
					if (pt <= 0) pt = ipdPayment.advanceamount.GetValueOrDefault();
					decimal amountToReceive = pt;
					if (amountToReceive <= 0) amountToReceive = ipdPayment.advanceamount.GetValueOrDefault();

					var billing = new NalamVazha.Models.BillingPaymentModel
					{
						amount = amountToReceive,
						receivedamount = amountToReceive,
						craftmyapp_actionmethodname = "Add_Billing_Payment",
						isdeleted = false
					};




					if (Guid.TryParse(ipdPayment.tenantid, out var tid))
						billing.tenantid = tid;
					if (Guid.TryParse(IPDApplicationFormid, out var ipdGuid))
						billing.ipdnumber = ipdGuid;
					if (!string.IsNullOrEmpty(ipdPayment.patientname) && Guid.TryParse(ipdPayment.patientname, out var pname))
						billing.patientname = pname;
					if (!string.IsNullOrEmpty(ipdPayment.patientvisitid) && Guid.TryParse(ipdPayment.patientvisitid, out var pvisit) && pvisit != Guid.Empty)
						billing.patientvisit = pvisit;


					if (BillingPaymentid != "")
					{
						var jsonObjBillingPayment = await ApiClient.Get_ApiValues(getHttpClient(), "api/BillingPayment/getById_BillingPayment?BillingPaymentid=" + BillingPaymentid + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
						if (jsonObjBillingPayment.Length > 2)
						{

							billing = JsonConvert.DeserializeObject<BillingPaymentModel>(jsonObjBillingPayment);





						}
					}

					ViewBag.IpdPaymentDetails = ipdPayment;
					ViewBag.IPDApplicationFormid = IPDApplicationFormid;
					return View(billing);
				}

				TempData["errMessage"] = "IPD record not found";
				return View(new NalamVazha.Models.BillingPaymentModel { craftmyapp_actionmethodname = "Add_Billing_Payment" });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - IPDApplicationForm / Manual_Deposit_Receipt: " + ex.Message);
				TempData["errMessage"] = "Error while fetching data - Contact Administrator";
				return View(new NalamVazha.Models.BillingPaymentModel { craftmyapp_actionmethodname = "Add_Billing_Payment" });
			}
		}


		[AllowAnonymous]
		[HttpPost()]
		public virtual async Task<IActionResult> Create_Razorpay_Order([FromBody] NalamVazha.Models.IPDRazorpayOrderRequestModel request)
		{
			try
			{
				if (request == null || string.IsNullOrWhiteSpace(request.IPDApplicationFormid))
				{
					// Fallback: read from form in case client sends application/x-www-form-urlencoded
					var formId = Request.Form["IPDApplicationFormid"].ToString();
					if (string.IsNullOrWhiteSpace(formId))
						return new ContentResult { Content = JsonConvert.SerializeObject(new { error = "IPDApplicationFormid is required." }), ContentType = "application/json", StatusCode = 400 };
					request = new NalamVazha.Models.IPDRazorpayOrderRequestModel { IPDApplicationFormid = formId };
				}
				var requestBody = new NalamVazha.Models.IPDRazorpayOrderRequestModel
				{
					IPDApplicationFormid = request.IPDApplicationFormid
				};
				var json = await ApiClient.Post_ApiValuesGetRawString(getAnonymousHttpClient(),
					"api/IPDApplicationForm/Create_Razorpay_Order", requestBody);
				return Content(json, "application/json");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - IPDApplicationForm / Create_Razorpay_Order: " + ex.Message);
				var errorJson = JsonConvert.SerializeObject(new { error = ex.Message });
				return new ContentResult { Content = errorJson, ContentType = "application/json", StatusCode = 400 };
			}
		}

		[AllowAnonymous]
		[HttpGet()]
		public virtual async Task<IActionResult> Get_IPD_Booking_Deposit_Payment_Status(string IPDApplicationFormid)
		{
			try
			{
				var json = await ApiClient.Get_ApiValues(getAnonymousHttpClient(),
					"api/IPDApplicationForm/Get_IPD_Booking_Deposit_Payment_Status?IPDApplicationFormid=" + IPDApplicationFormid);
				return Content(json, "application/json");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - IPDApplicationForm / Get_IPD_Booking_Deposit_Payment_Status: " + ex.Message);
				var errorJson = JsonConvert.SerializeObject(new { status = "Open", canPay = false, message = ex.Message });
				return new ContentResult { Content = errorJson, ContentType = "application/json", StatusCode = 400 };
			}
		}

		private async Task<string> BuildBlockedRoomDetailsJsonFallback(string ipdApplicationFormId)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(ipdApplicationFormId)) return "[]";
				var roomJson = await ApiClient.Get_ApiValues(getAnonymousHttpClient(),
					"api/IPDApplicationForm/getById_room?IPDApplicationFormid=" + ipdApplicationFormId +
					"&loginUserID=" + (HttpContext.Session.GetString("NalamVazhaloginUserID") ?? ""));
				if (string.IsNullOrWhiteSpace(roomJson) || roomJson.Length <= 2) return "[]";

				var arr = JArray.Parse(roomJson);
				var output = new JArray();
				foreach (var row in arr)
				{
					var isDeleted = (row["isdeleted"]?.ToString() ?? "").Trim().ToLower();
					if (isDeleted == "true" || isDeleted == "1") continue;

					var roomId = (row["roomnumber"]?.ToString() ?? "").Trim();
					var roomNo = "";
					if (!string.IsNullOrWhiteSpace(roomId))
					{
						try
						{
							var oneRoomJson = await ApiClient.Get_ApiValues(getAnonymousHttpClient(), "api/Room/getById_Room?Roomid=" + roomId);
							if (!string.IsNullOrWhiteSpace(oneRoomJson) && oneRoomJson.Length > 2)
							{
								JToken oneRoomToken = oneRoomJson.TrimStart().StartsWith("[")
									? (JArray.Parse(oneRoomJson).First ?? new JObject())
									: JToken.Parse(oneRoomJson);
								roomNo = (oneRoomToken?["roomnumber"]?.ToString() ?? "").Trim();
							}
						}
						catch { }
					}

					output.Add(new JObject
					{
						["allottedto"] = (row["allottedto"]?.ToString() ?? "").Trim(),
						["roomnumber"] = roomNo,
						["fromdate"] = (row["fromdate"]?.ToString() ?? "").Trim(),
						["todate"] = (row["todate"]?.ToString() ?? "").Trim(),
						["roomid"] = roomId
					});
				}
				return output.ToString(Formatting.None);
			}
			catch
			{
				return "[]";
			}
		}

		[AllowAnonymous]
		[HttpPost()]
		public virtual async Task<IActionResult> Complete_IPD_Payment([FromBody] NalamVazha.Models.IPDCompletePaymentModel model)
		{
			try
			{
				var json = await ApiClient.Post_ApiValuesGetRawString(getAnonymousHttpClient(),
					"api/IPDApplicationForm/Complete_IPD_Payment", model);
				return Content(json, "application/json");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - IPDApplicationForm / Complete_IPD_Payment: " + ex.Message);
				return BadRequest(ex.Message);
			}
		}

		[AllowAnonymous]
		[HttpPost()]
		public virtual async Task<IActionResult> Create_IPD_Balance_Razorpay_Order([FromBody] NalamVazha.Models.IPDRazorpayOrderRequestModel request)
		{
			try
			{
				if (request == null || string.IsNullOrWhiteSpace(request.IPDApplicationFormid))
				{
					var formId = Request.Form["IPDApplicationFormid"].ToString();
					if (string.IsNullOrWhiteSpace(formId))
						return new ContentResult { Content = JsonConvert.SerializeObject(new { error = "IPDApplicationFormid is required." }), ContentType = "application/json", StatusCode = 400 };
					request = new NalamVazha.Models.IPDRazorpayOrderRequestModel { IPDApplicationFormid = formId, paymenttype = "Balance Payment" };
				}
				var requestBody = new NalamVazha.Models.IPDRazorpayOrderRequestModel
				{
					IPDApplicationFormid = request.IPDApplicationFormid,
					paymenttype = "Balance Payment",
					receivableids = request.receivableids ?? new List<Guid>()
				};
				var json = await ApiClient.Post_ApiValuesGetRawString(getAnonymousHttpClient(),
					"api/IPDApplicationForm/Create_IPD_Balance_Razorpay_Order", requestBody);
				return Content(json, "application/json");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - IPDApplicationForm / Create_IPD_Balance_Razorpay_Order: " + ex.Message);
				var errorJson = JsonConvert.SerializeObject(new { error = ex.Message });
				return new ContentResult { Content = errorJson, ContentType = "application/json", StatusCode = 400 };
			}
		}

		[AllowAnonymous]
		[HttpPost()]
		public virtual async Task<IActionResult> Complete_IPD_Balance_Payment([FromBody] NalamVazha.Models.IPDCompletePaymentModel model)
		{
			try
			{
				var json = await ApiClient.Post_ApiValuesGetRawString(getAnonymousHttpClient(),
					"api/IPDApplicationForm/Complete_IPD_Balance_Payment", model);
				return Content(json, "application/json");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - IPDApplicationForm / Complete_IPD_Balance_Payment: " + ex.Message);
				return BadRequest(ex.Message);
			}
		}

		[AllowAnonymous]
		[HttpPost()]
		public virtual async Task<IActionResult> Fail_IPD_Payment([FromBody] NalamVazha.Models.IPDFailedPaymentModel model)
		{
			try
			{
				var json = await ApiClient.Post_ApiValuesGetRawString(getAnonymousHttpClient(),
					"api/IPDApplicationForm/Fail_IPD_Payment", model);
				return Content(json, "application/json");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - IPDApplicationForm / Fail_IPD_Payment: " + ex.Message);
				return BadRequest(ex.Message);
			}
		}

		/// <summary>
		/// Opens dedicated cancellation page and shows refund details.
		/// </summary>
		[AllowAnonymous]
		[HttpGet()]
		public virtual async Task<IActionResult> Get_IPD_Refund_Details(string IPDApplicationFormid)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(IPDApplicationFormid))
				{
					TempData["errMessage"] = "IPDApplicationFormid is required";
					return RedirectToAction("Dashboard", "FrontDesk");
				}
				var role = HttpContext.Session.GetString("NalamVazhauserrole") ?? "";
				var cBy = role.Equals("Health Seeker", StringComparison.OrdinalIgnoreCase) ? "Patient" : "Hospital";
				return RedirectToAction("Cancel_IPD", new { IPDApplicationFormid = IPDApplicationFormid, cancellationby = cBy });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - IPDApplicationForm / Get_IPD_Refund_Details: " + ex.Message);
				TempData["errMessage"] = "Unable to open cancellation page";
				return RedirectToAction("Dashboard", "FrontDesk");
			}
		}

		/// <summary>Proxy: returns refund breakdown JSON for the given IPD application and cancellationby.</summary>
		[AllowAnonymous]
		[HttpGet()]
		public virtual async Task<IActionResult> Get_Cancel_IPD_Payment_History_Data(string IPDApplicationFormid)
		{
			try
			{
				var json = await ApiClient.Get_ApiValues(getAnonymousHttpClient(),
					$"api/IPDApplicationForm/Get_Cancel_IPD_Payment_History?IPDApplicationFormid={IPDApplicationFormid}");
				return Content(json, "application/json");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - IPDApplicationForm / Get_Cancel_IPD_Payment_History_Data: " + ex.Message);
				return Content("[]", "application/json");
			}
		}

		[AllowAnonymous]
		[HttpGet()]
		public virtual async Task<IActionResult> Get_IPD_Cancellation_Request_Remarks_Data(string IPDApplicationFormid)
		{
			try
			{
				var json = await ApiClient.Get_ApiValues(getAnonymousHttpClient(),
					$"api/IPDApplicationForm/Get_IPD_Cancellation_Request_Remarks?IPDApplicationFormid={IPDApplicationFormid}");
				return Content(json, "application/json");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex,
					"Get_IPD_Cancellation_Request_Remarks_Data failed for IPD {IPDApplicationFormid}",
					IPDApplicationFormid);
				return Content("{\"remarks\":\"\"}", "application/json");
			}
		}

		/// <summary>Proxy: returns IPD billing summary JSON (totals, paid, balance) for use on view pages.</summary>
		[HttpGet()]
		public virtual async Task<IActionResult> Get_IPD_Billing_Summary_Data(string IPDApplicationFormid)
		{
			try
			{
				var json = await ApiClient.Get_ApiValues(getHttpClient(),
					"api/IPDApplicationForm/Get_IPD_Billing_Summary?IPDApplicationFormid=" + IPDApplicationFormid +
					"&loginUserID=" + (HttpContext.Session.GetString("NalamVazhaloginUserID") ?? ""));
				return Content(string.IsNullOrWhiteSpace(json) ? "{}" : json, "application/json");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - IPDApplicationForm / Get_IPD_Billing_Summary_Data: " + ex.Message);
				return Content("{}", "application/json");
			}
		}

		[HttpGet()]
		public virtual async Task<IActionResult> Get_IPD_All_Receivables_Data(string IPDApplicationFormid)
		{
			try
			{
				var json = await ApiClient.Get_ApiValues(getHttpClient(),
					"api/IPDApplicationForm/Get_IPD_All_Receivables?IPDApplicationFormid=" + IPDApplicationFormid +
					"&loginUserID=" + (HttpContext.Session.GetString("NalamVazhaloginUserID") ?? ""));
				return Content(string.IsNullOrWhiteSpace(json) ? "[]" : json, "application/json");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Get_IPD_All_Receivables_Data error: " + ex.Message);
				return Content("[]", "application/json");
			}
		}

		[HttpGet()]
		public virtual async Task<IActionResult> Get_IPD_Billing_Payments_List_Data(string IPDApplicationFormid)
		{
			try
			{
				var json = await ApiClient.Get_ApiValues(getHttpClient(),
					"api/IPDApplicationForm/Get_IPD_Billing_Payments_List?IPDApplicationFormid=" + IPDApplicationFormid +
					"&loginUserID=" + (HttpContext.Session.GetString("NalamVazhaloginUserID") ?? ""));
				return Content(string.IsNullOrWhiteSpace(json) ? "[]" : json, "application/json");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Get_IPD_Billing_Payments_List_Data error: " + ex.Message);
				return Content("[]", "application/json");
			}
		}

   

        public byte[] GeneratePdfFromString(string htmlContent, string nofooter = "")
        {
            return GeneratePdf(htmlContent, nofooter);
        }

        [HttpGet]
        public virtual async Task<IActionResult> IPD_Billing_Summary_PDF(string IPDApplicationFormid, bool download = false)
        {
            if (string.IsNullOrEmpty(IPDApplicationFormid))
            {
                TempData["errMessage"] = "Invalid request";
                return RedirectToAction("Approved_IPD_Application_Forms");
            }

            try
            {
                var client = getHttpClient();

                string loginUserID =
                    HttpContext.Session.GetString("NalamVazhaloginUserID") ?? "";

                var summaryJson = await ApiClient.Get_ApiValues(
                    client,
                    "api/IPDApplicationForm/Get_IPD_Billing_Summary?IPDApplicationFormid=" +
                    IPDApplicationFormid +
                    "&loginUserID=" + loginUserID
                );

              var summary =
                    (!string.IsNullOrWhiteSpace(summaryJson) && summaryJson.Length > 2)
                    ? JsonConvert.DeserializeObject<NalamVazha.Models.IPDBillingSummaryModel>(summaryJson)
                    : null;

                if (summary != null && string.IsNullOrWhiteSpace(summary.blocked_room_details_json))
                {
                    summary.blocked_room_details_json = await BuildBlockedRoomDetailsJsonFallback(IPDApplicationFormid);
                }
                var receivablesJson = await ApiClient.Get_ApiValues(
                    client,
                    "api/IPDApplicationForm/Get_IPD_All_Receivables?IPDApplicationFormid=" +
                    IPDApplicationFormid +
                    "&loginUserID=" + loginUserID
                );

                var receivables =
                    JsonConvert.DeserializeObject<List<dynamic>>(receivablesJson ?? "[]")
                    ?? new List<dynamic>();

                var paymentsJson = await ApiClient.Get_ApiValues(
                    client,
                    "api/IPDApplicationForm/Get_IPD_Billing_Payments_List?IPDApplicationFormid=" +
                    IPDApplicationFormid +
                    "&loginUserID=" + loginUserID
                );

                var payments =
                    JsonConvert.DeserializeObject<List<dynamic>>(paymentsJson ?? "[]")
                    ?? new List<dynamic>();


                // VIEW MODE
                if (!download)
                {
                    ViewBag.Summary = summary;
                    ViewBag.Receivables = receivables;
                    ViewBag.Payments = payments;
                    ViewBag.GeneratedOn =
                        DateTime.Now.ToString("dd/MM/yyyy hh:mm tt");

                    ViewBag.IPDApplicationFormid = IPDApplicationFormid;

                    return View();
                }


                // LOGO URL
                string organizationLogo =
                    summary != null
                    ? Convert.ToString(summary.organizationlogo)
                    : "";



                string logoPath = "";

                if (!string.IsNullOrWhiteSpace(summary?.organizationlogo))
                {
                    try
                    {
                        string organizationLogoFile =
                            summary.organizationlogo.Replace("|", "").Trim();

                        string logoBlobUrl =
                            GetIPDBillingFileUrl(organizationLogoFile);
                        
                        string logoDirectory = Path.Combine(
                            hostingEnv.WebRootPath,
                            "uploads",
                            "IPDBilling"
                        );

                        if (!Directory.Exists(logoDirectory))
                        {
                            Directory.CreateDirectory(logoDirectory);
                        }

                        string logoFileName =
                            Path.GetFileName(organizationLogoFile);

                        if (string.IsNullOrWhiteSpace(logoFileName))
                        {
                            logoFileName = "organization-logo.png";
                        }

                        string localLogoPath = Path.Combine(
                            logoDirectory,
                            logoFileName
                        );

                        using (var httpClientLogo = new HttpClient())
                        {
                            httpClientLogo.Timeout = TimeSpan.FromSeconds(30);

                            using (var response = await httpClientLogo.GetAsync(logoBlobUrl))
                            {
                                response.EnsureSuccessStatusCode();

                                byte[] logoBytes =
                                    await response.Content.ReadAsByteArrayAsync();

                                await System.IO.File.WriteAllBytesAsync(
                                    localLogoPath,
                                    logoBytes
                                );
                            }
                        }

                        if (System.IO.File.Exists(localLogoPath))
                        {
                            logoPath = localLogoPath;
                        }

                        _logger.LogInformation(
                            "IPD billing logo downloaded. Path: {LogoPath}, Exists: {Exists}",
                            localLogoPath,
                            System.IO.File.Exists(localLogoPath)
                        );
                    }
                    catch (Exception logoEx)
                    {
                        logoPath = "";

                        _logger.LogError(
                            logoEx,
                            "Unable to download IPD billing organization logo. Logo value: {Logo}",
                            summary?.organizationlogo
                        );
                    }
                }


                // PDF HTML
                string htmlContent = IPDBillingPdfTemplate.BuildHtml(
                    summary,
                    receivables,
                    payments,
                    DateTime.Now.ToString("dd/MM/yyyy hh:mm tt"),
                    logoPath
                );

                string footerHtml = IPDBillingPdfTemplate.BuildFooterHtml();
                byte[] pdfBytes = GeneratePdf(htmlContent, footerHtml);


                // SAFE FILE NAME
                string bookingRef =
                    Convert.ToString(
                        summary?.bookingreferencenumber
                        ?? IPDApplicationFormid
                    );

                string safeFileName =
                    bookingRef
                    .Replace("/", "_")
                    .Replace("\\", "_")
                    .Replace(":", "_")
                    .Replace(" ", "_");


                return File(
                    pdfBytes,
                    "application/pdf",
                    "IPD_Billing_Statement_" + safeFileName + ".pdf"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "IPD_Billing_Summary_PDF error: " + ex.Message
                );

                TempData["errMessage"] =
                    "Error loading billing summary";

                return RedirectToAction(
                    "Approved_IPD_Application_Forms"
                );
            }
        }

        private string GetIPDBillingFileUrl(string filename)
        {
            filename = (filename ?? "")
                .Replace("|", "")
                .Trim();

            string cloudPath =
                HttpContext.Session.GetString("NalamVazhacloudpath") ?? "";

            string sasToken =
                HttpContext.Session.GetString("NalamVazhaSAStoken") ?? "";

            if (string.IsNullOrWhiteSpace(filename))
            {
                return "";
            }

            // The database may already contain the complete blob URL.
            if (Uri.TryCreate(filename, UriKind.Absolute, out var absoluteUri))
            {
                if (!string.IsNullOrWhiteSpace(sasToken) &&
                    !filename.Contains("?"))
                {
                    return filename + sasToken;
                }

                return filename;
            }

            if (string.IsNullOrWhiteSpace(cloudPath))
            {
                return "";
            }

            return cloudPath.TrimEnd('/')
                   + "/"
                   + filename.TrimStart('/')
                   + sasToken;
        }
        private async Task<string> RenderViewToStringAsync(string viewName, object model)
          {
    ViewData.Model = model;

    using (var sw = new StringWriter())
    {
        var viewResult = _razorViewEngine.FindView(ControllerContext, viewName, false);

        if (viewResult.View == null)
        {
            throw new Exception("View not found: " + viewName);
        }

        var viewContext = new ViewContext(
            ControllerContext,
            viewResult.View,
            ViewData,
            TempData,
            sw,
            new HtmlHelperOptions()
        );

        await viewResult.View.RenderAsync(viewContext);
        return sw.ToString();
    }
}
        private byte[] GeneratePdf(string htmlContent, string footerHtml = null)
        {
            string footerFilePath = null;

            var globalSettings = new GlobalSettings
            {
                ColorMode = DinkToPdf.ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10, Bottom = string.IsNullOrEmpty(footerHtml) ? 0 : 16 },
            };

            var footerSettings = new FooterSettings { FontSize = 8 };

            if (!string.IsNullOrEmpty(footerHtml))
            {
                footerFilePath = Path.Combine(Path.GetTempPath(), $"ipd_footer_{Guid.NewGuid()}.html");
                System.IO.File.WriteAllText(footerFilePath, footerHtml);
                footerSettings.HtmUrl = footerFilePath;
                footerSettings.Spacing = 6;
            }

            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = htmlContent,
                WebSettings = { DefaultEncoding = "utf-8", LoadImages = true },
                HeaderSettings = { FontSize = 10, Left = "", Right = "", Line = false },
                FooterSettings = footerSettings,
            };

            var htmlToPdfDocument = new HtmlToPdfDocument
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings },
            };

            try
            {
                return _converter.Convert(htmlToPdfDocument);
            }
            finally
            {
                if (footerFilePath != null && System.IO.File.Exists(footerFilePath))
                {
                    try { System.IO.File.Delete(footerFilePath); } catch { }
                }
            }
        }

        /// <summary>Proxy: returns refund breakdown JSON for the given IPD application and cancellationby.</summary>
        [AllowAnonymous]
		[HttpGet()]
		public virtual async Task<IActionResult> Get_IPD_Refund_Details_Data(string IPDApplicationFormid, string cancellationby = "Hospital")
		{
			try
			{
				var json = await ApiClient.Get_ApiValues(getAnonymousHttpClient(),
					$"api/IPDApplicationForm/Get_IPD_Refund_Details?IPDApplicationFormid={IPDApplicationFormid}&cancellationby={cancellationby}");
				return Content(json, "application/json");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - IPDApplicationForm / Get_IPD_Refund_Details_Data: " + ex.Message);
				// Return empty JSON array so the UI can handle "no refund policy" gracefully
				return Content("[]", "application/json");
			}
		}

		/// <summary>
		/// Proxy: cancels IPD booking. Injects session user role before forwarding to WebAPI.
		/// If refundmode=RazorPay the WebAPI triggers Razorpay automatically;
		/// otherwise records a manual Cash/UPI/Card/NetBanking refund.
		/// </summary>
		[AllowAnonymous]
		[HttpPost()]
		public virtual async Task<IActionResult> Cancel_IPD_Booking([FromBody] NalamVazha.Models.IPDCancellationRequestModel model)
		{
			try
			{
				// Inject session user role so WebAPI can log who processed the refund
				model.sessionuserrole = HttpContext.Session.GetString("NalamVazhauserrole") ?? "Unknown";

				var json = await ApiClient.Post_ApiValuesGetRawString(getAnonymousHttpClient(),
					"api/IPDApplicationForm/Cancel_IPD_Booking", model);
				return Content(json, "application/json");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - IPDApplicationForm / Cancel_IPD_Booking: " + ex.Message);
				return BadRequest(ex.Message);
			}
		}

		[AllowAnonymous]
		[HttpPost()]
		public virtual async Task<IActionResult> Process_IPD_Overpaid_Refund([FromBody] IPDOverpaidRefundRequestModel model)
		{
			if (model == null || string.IsNullOrWhiteSpace(model.IPDApplicationFormid))
				return BadRequest("Invalid IPD refund request.");
			if (model.refundamount <= 0)
				return BadRequest("Refund amount should be greater than zero.");
			if (string.IsNullOrWhiteSpace(model.refundmode))
				return BadRequest("Refund mode is required.");

			try
			{
				var client = getHttpClient();
				var loginUserId = HttpContext.Session.GetString("NalamVazhaloginUserID");
				var receivablesJson = await ApiClient.Get_ApiValues(client,
					"api/IPDApplicationForm/Get_IPD_All_Receivables?IPDApplicationFormid=" + model.IPDApplicationFormid +
					"&loginUserID=" + loginUserId);
				var paymentsJson = await ApiClient.Get_ApiValues(client,
					"api/IPDApplicationForm/Get_IPD_Billing_Payments_List?IPDApplicationFormid=" + model.IPDApplicationFormid +
					"&loginUserID=" + loginUserId);

				var receivableRows = JArray.Parse(string.IsNullOrWhiteSpace(receivablesJson) ? "[]" : receivablesJson);
				var paymentRows = JArray.Parse(string.IsNullOrWhiteSpace(paymentsJson) ? "[]" : paymentsJson);
				decimal statementTotal = receivableRows.Sum(row => ParseBillingDecimal(row["amount"]?.ToString()));
				decimal paidFromReceivables = receivableRows.Sum(row => ParseBillingDecimal(row["paidamount"]?.ToString()));
				decimal netPayments = paymentRows.Sum(row => ParseBillingDecimal(row["amount"]?.ToString()));
				decimal statementReceived = netPayments > 0 ? netPayments : paidFromReceivables;
				decimal refundableBalance = statementReceived > statementTotal
					? statementReceived - statementTotal
					: 0m;

				if (refundableBalance <= 0.009m)
					return Conflict("This IPD has no remaining refundable balance. Please refresh the dashboard.");
				if (Math.Abs(model.refundamount - refundableBalance) > 0.009m)
					return Conflict($"The remaining refundable balance is ₹{refundableBalance:N2}. Please refresh and try again.");

				var ipdJson = await ApiClient.Get_ApiValues(client,
					"api/IPDApplicationForm/Get_IPD_Payment_Details?IPDApplicationFormid=" + model.IPDApplicationFormid +
					"&loginUserID=" + loginUserId);

				var ipdPayment = ipdJson.Length > 2
					? JsonConvert.DeserializeObject<NalamVazha.Models.IPDPaymentDetailsModel>(ipdJson)
					: null;

				var refundReason = string.IsNullOrWhiteSpace(model.refundreason)
					? "Overpayment refund"
					: model.refundreason.Trim();

				var refundBilling = new NalamVazha.Models.BillingPaymentModel
				{
					BillingPaymentid = Guid.NewGuid(),
					receivablefor = "Overpayment Refund",
					paymentdate = DateTime.Now.Date,
					amount = -model.refundamount,
					receivedamount = model.refundamount,
					currency = "INR",
					conversionrate = 1,
					paymentmode = model.refundmode,
					paymentstatus = "Refund Initiated",
					refundmode = model.refundmode,
					refundedamount = model.refundamount,
					refundreason = refundReason,
					refundstatus = "Refund Initiated",
					remarks = "Overpayment Refund - " + refundReason,
					isdeleted = false,
					craftmyapp_actionmethodname = "Add_Billing_Payment"
				};

				if (Guid.TryParse(model.IPDApplicationFormid, out var ipdGuid))
					refundBilling.ipdnumber = ipdGuid;
				if (Guid.TryParse(loginUserId, out var createdUser))
					refundBilling.createduser = createdUser;
				if (ipdPayment != null)
				{
					if (Guid.TryParse(ipdPayment.tenantid, out var tenantId))
						refundBilling.tenantid = tenantId;
					if (Guid.TryParse(ipdPayment.patientname, out var patientId))
						refundBilling.patientname = patientId;
				}

				var result = await ApiClient.Post_ApiValuesGetString(client,
					"api/BillingPayment/Add_Billing_Payment", refundBilling);

				if (string.Equals(result, "201.1", StringComparison.OrdinalIgnoreCase)
					|| string.Equals(result, "\"201.1\"", StringComparison.OrdinalIgnoreCase))
				{
					return Json(new
					{
						message = "201.1",
						refundamount = model.refundamount,
						refundmode = model.refundmode
					});
				}

				return BadRequest(result);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - IPDApplicationForm / Process_IPD_Overpaid_Refund: " + ex.Message);
				return BadRequest(ex.Message);
			}
		}

		[HttpGet()]
		public virtual async Task<IActionResult> Cancel_IPD(string IPDApplicationFormid, string cancellationby = "Hospital", bool triggerrefund = false, bool overpaymentrefund = false)
		{
			if (string.IsNullOrEmpty(IPDApplicationFormid))
			{
				TempData["errMessage"] = "Invalid request";
				return RedirectToAction("Dashboard", "FrontDesk");
			}
			try
			{
				var client = getHttpClient();
				var summaryJson = await ApiClient.Get_ApiValues(client,
					"api/IPDApplicationForm/Get_IPD_Billing_Summary?IPDApplicationFormid=" + IPDApplicationFormid +
					"&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));

				if (summaryJson.Length <= 2)
				{
					TempData["errMessage"] = "IPD record not found";
					return RedirectToAction("Dashboard", "FrontDesk");
				}

				var summaryModel = JsonConvert.DeserializeObject<NalamVazha.Models.IPDBillingSummaryModel>(summaryJson);
				var billing = new NalamVazha.Models.BillingPaymentModel();

				if (Guid.TryParse(IPDApplicationFormid, out var ipdGuid))
					billing.ipdnumber = ipdGuid;
				if (!string.IsNullOrEmpty(summaryModel.patientvisitid) && Guid.TryParse(summaryModel.patientvisitid, out var pvisit) && pvisit != Guid.Empty)
					billing.patientvisit = pvisit;

				try
				{
					var ipdJson = await ApiClient.Get_ApiValues(client,
						"api/IPDApplicationForm/Get_IPD_Payment_Details?IPDApplicationFormid=" + IPDApplicationFormid +
						"&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
					if (ipdJson.Length > 2)
					{
						var ipdPayment = JsonConvert.DeserializeObject<NalamVazha.Models.IPDPaymentDetailsModel>(ipdJson);
						if (ipdPayment != null)
						{
							if (Guid.TryParse(ipdPayment.tenantid, out var tid))
								billing.tenantid = tid;
							if (!string.IsNullOrEmpty(ipdPayment.patientname) && Guid.TryParse(ipdPayment.patientname, out var pname))
								billing.patientname = pname;
							if (!string.IsNullOrEmpty(ipdPayment.IPDApplicationFormid) && Guid.TryParse(ipdPayment.IPDApplicationFormid, out var ipdnum))
								billing.ipdnumber = ipdnum;
						}
					}
				}
				catch { /* non-critical */ }

				decimal overpaidRefundAmount = 0m;
				if (overpaymentrefund)
				{
					try
					{
						var receivablesJson = await ApiClient.Get_ApiValues(client,
							"api/IPDApplicationForm/Get_IPD_All_Receivables?IPDApplicationFormid=" + IPDApplicationFormid +
							"&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
						var paymentsJson = await ApiClient.Get_ApiValues(client,
							"api/IPDApplicationForm/Get_IPD_Billing_Payments_List?IPDApplicationFormid=" + IPDApplicationFormid +
							"&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));

						var receivableRows = Newtonsoft.Json.Linq.JArray.Parse(string.IsNullOrWhiteSpace(receivablesJson) ? "[]" : receivablesJson);
						var paymentRows = Newtonsoft.Json.Linq.JArray.Parse(string.IsNullOrWhiteSpace(paymentsJson) ? "[]" : paymentsJson);

						decimal statementTotal = 0m;
						decimal statementPaidFromReceivables = 0m;
						foreach (var receivableRow in receivableRows)
						{
							statementTotal += ParseBillingDecimal(receivableRow["amount"]?.ToString());
							statementPaidFromReceivables += ParseBillingDecimal(receivableRow["paidamount"]?.ToString());
						}

						decimal statementPayments = 0m;
						foreach (var paymentRow in paymentRows)
						{
							statementPayments += ParseBillingDecimal(paymentRow["amount"]?.ToString());
						}

						var statementReceived = statementPayments > 0 ? statementPayments : statementPaidFromReceivables;
						overpaidRefundAmount = statementReceived > statementTotal ? statementReceived - statementTotal : 0m;
					}
					catch (Exception overpaidEx)
					{
						_logger.LogError(overpaidEx, "Cancel_IPD overpaid refund amount calculation failed: " + overpaidEx.Message);
						overpaidRefundAmount = 0m;
					}
				}

				if (overpaymentrefund && overpaidRefundAmount <= 0.009m)
				{
					TempData["errMessage"] = "This IPD has no remaining refundable balance.";
					return RedirectToAction("Dashboard", "FrontDesk");
				}

				ViewBag.BillingSummary = summaryModel;
				ViewBag.IPDApplicationFormid = IPDApplicationFormid;
				ViewBag.CancellationBy = cancellationby;
				ViewBag.TriggerRefund = triggerrefund;
				ViewBag.IsOverpaymentRefund = overpaymentrefund;
				ViewBag.OverpaidRefundAmount = overpaidRefundAmount;
				var sessionRole = HttpContext.Session.GetString("NalamVazhauserrole") ?? "";
				ViewBag.IsHealthSeeker = sessionRole.Equals("Health Seeker", StringComparison.OrdinalIgnoreCase);
				ViewBag.IsFrontDeskAdmin = sessionRole.Equals("Frontdesk Admin", StringComparison.OrdinalIgnoreCase)
					|| sessionRole.Equals("Front Desk Admin", StringComparison.OrdinalIgnoreCase);
				return View(billing);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Cancel_IPD error: " + ex.Message);
				TempData["errMessage"] = "Error loading cancellation page";
				return RedirectToAction("Dashboard", "FrontDesk");
			}
		}

[HttpGet()]
			public virtual async Task<string> lookup_change_IPDApplicationForm_consentform(string PatientConsentid)
			{
				return await ApiClient.Get_ApiValues(getHttpClient(), "api/IPDApplicationForm/lookup_change_IPDApplicationForm_consentform?PatientConsentid="+PatientConsentid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			}

			/// <summary>
			/// Stream consent file from blob so it can be embedded in iframe.
			/// </summary>
			[HttpGet()]
			public virtual async Task<IActionResult> ViewConsentFile(string file)
			{
				return await StreamConsentFile(file);
			}

			[HttpGet()]
			public virtual async Task<IActionResult> ViewConsentFileById(Guid PatientConsentid)
			{
				if (PatientConsentid == Guid.Empty)
					return NotFound();

				var consentJson = await ApiClient.Get_ApiValues(getHttpClient(),
					"api/IPDApplicationForm/lookup_change_IPDApplicationForm_consentform?PatientConsentid="
					+ PatientConsentid
					+ "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
				var consent = ParseJsonArrayOrEmpty(consentJson).FirstOrDefault() as JObject;
				var consentFile = consent?.GetValue("consentfile", StringComparison.OrdinalIgnoreCase)?.ToString();
				return await StreamConsentFile(consentFile);
			}

			private async Task<IActionResult> StreamConsentFile(string file)
			{
				if (string.IsNullOrWhiteSpace(file) || file.Contains(".."))
					return NotFound();
				string url = util.fileSystem.GetFileURl(file.Trim().Trim('|').Split('|', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault());
				if (string.IsNullOrEmpty(url))
					return NotFound();
				try
				{
					using var client = new HttpClient();
					using var response = await client.GetAsync(url);
					if (!response.IsSuccessStatusCode)
						return NotFound();
					var bytes = await response.Content.ReadAsByteArrayAsync();
					var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/pdf";
					Response.Headers.Remove("X-Frame-Options");
					Response.Headers.Append("Content-Disposition", "inline");
					return File(bytes, contentType, enableRangeProcessing: true);
				}
				catch
				{
					return NotFound();
				}
			}

			/// <summary>
			/// HTML wrapper that renders the consent PDF via PDF.js and notifies parent when user scrolls to bottom.
			/// </summary>
			[HttpGet()]
			public virtual IActionResult ConsentPdfViewer(string file)
			{
				if (string.IsNullOrWhiteSpace(file) || file.Contains(".."))
					return NotFound();
				ViewBag.PdfFile = file.Trim();
				ViewBag.PdfUrl = Url.Action("ViewConsentFile", "IPDApplicationForm", new { file = file.Trim() });
				return View("ConsentPdfViewer");
			}

[HttpGet()]
			public virtual async Task<string> GetPincodeList(string pincode, int? pagesize = 100, int? pagenumber = 0)
			{
				return await ApiClient.Get_ApiValues(getHttpClient(), "api/PincodeMaster/Pincode_List?pincode=" + (string.IsNullOrEmpty(pincode) ? "" : Uri.EscapeDataString(pincode)) + "&pagesize=" + (pagesize ?? 100) + "&pagenumber=" + (pagenumber ?? 0) + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
			}


		[HttpPost()]
		public virtual async Task<IActionResult> Allot_Room_IPDReview([FromBody] RoomAllotmentRequest request)
		{
			try
			{
				if (HttpContext.Session.GetString("NalamVazhaloginUserID") == null)
					return Json(new { success = false, message = "Session Expired" });

				var loginUserID = HttpContext.Session.GetString("NalamVazhaloginUserID");
				var httpClient = getHttpClient();
				var results = new List<string>();

				// ──────────────────────────────────────────────────────────
				// 1. Resolve date range from IPD preferred dates of admission
				// ──────────────────────────────────────────────────────────
				DateTime arrivalDate = DateTime.Today;
				DateTime departureDate = DateTime.Today.AddDays(1);

				if (!string.IsNullOrEmpty(request.ipdApplicationFormId))
				{
					var prefDatesJson = await ApiClient.Get_ApiValues(httpClient,
						"api/IPDApplicationForm/getById_preferreddatesofadmission?IPDApplicationFormid="
						+ request.ipdApplicationFormId
						+ "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));

					if (!string.IsNullOrEmpty(prefDatesJson) && prefDatesJson.Length > 2)
					{
						var prefDatesList = JsonConvert.DeserializeObject<List<dynamic>>(prefDatesJson);
						if (prefDatesList != null && prefDatesList.Count > 0)
						{
							DateTime? earliestArrival = null;
							DateTime? earliestDeparture = null;

							foreach (var entry in prefDatesList)
							{
								DateTime entryArrival = DateTime.MinValue;
								DateTime entryDeparture = DateTime.MinValue;

								string arrStr = entry.dateofarrival?.ToString() ?? "";
								if (!string.IsNullOrEmpty(arrStr))
								{
									if (!DateTime.TryParseExact(arrStr, new[] { "dd/MM/yyyy", "dd/MM/yyyy HH:mm:ss", "yyyy-MM-dd", "yyyy-MM-ddTHH:mm:ss" },
											System.Globalization.CultureInfo.InvariantCulture,
											System.Globalization.DateTimeStyles.None, out entryArrival))
										DateTime.TryParse(arrStr, out entryArrival);
								}

								string depStr = entry.dateofdeparture?.ToString() ?? "";
								if (!string.IsNullOrEmpty(depStr))
								{
									if (!DateTime.TryParseExact(depStr, new[] { "dd/MM/yyyy", "dd/MM/yyyy HH:mm:ss", "yyyy-MM-dd", "yyyy-MM-ddTHH:mm:ss" },
											System.Globalization.CultureInfo.InvariantCulture,
											System.Globalization.DateTimeStyles.None, out entryDeparture))
										DateTime.TryParse(depStr, out entryDeparture);
								}

								if (entryArrival > DateTime.MinValue && (earliestArrival == null || entryArrival < earliestArrival))
									earliestArrival = entryArrival;

								if (entryDeparture > DateTime.MinValue && (earliestDeparture == null || entryDeparture < earliestDeparture))
									earliestDeparture = entryDeparture;
							}

							if (earliestArrival.HasValue) arrivalDate = earliestArrival.Value.Date;
							if (earliestDeparture.HasValue) departureDate = earliestDeparture.Value.Date;
						}
					}
				}

				if (request.dateOfArrival.HasValue) arrivalDate = request.dateOfArrival.Value.Date;
				if (request.dateOfDeparture.HasValue) departureDate = request.dateOfDeparture.Value.Date;

				if (arrivalDate < DateTime.Today) arrivalDate = DateTime.Today;
				if (departureDate < arrivalDate) departureDate = arrivalDate;

				int totalDays = (int)(departureDate - arrivalDate).TotalDays + 1;

				// ──────────────────────────────────────────────────────────
				// 2. Process each room
				// ──────────────────────────────────────────────────────────
				foreach (var roomId in request.roomIds)
				{
					var roomJson = await ApiClient.Get_ApiValues(httpClient,
						"api/Room/getById_Room?Roomid=" + roomId);

					if (string.IsNullOrEmpty(roomJson) || roomJson.Length <= 2)
					{
						results.Add("Room not found: " + roomId);
						continue;
					}

					var roomObj = JsonConvert.DeserializeObject<dynamic>(roomJson is string s && s.StartsWith("[")
						? JsonConvert.DeserializeObject<dynamic[]>(s)[0].ToString()
						: roomJson);

					// ──────────────────────────────────────────────────────
					// 3. Check existing occupancy for conflict
					// ──────────────────────────────────────────────────────
					var fromStr = arrivalDate.ToString("yyyy-MM-dd");
					var toStr = departureDate.ToString("yyyy-MM-dd");

					var occupancyJson = await ApiClient.Get_ApiValues(httpClient,
						"api/RoomOccupancyStatus/Room_Occupancy_Status_List?tenantid="
						+ "&block=&building=&floor="
						+ "&room=" + roomId
						+ "&rodate_automatonfrom=" + fromStr
						+ "&rodate_automatonto=" + toStr
						+ "&status="
						+ "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID")
						+ "&pagesize=1000&pagenumber=0");

					if (!string.IsNullOrEmpty(occupancyJson) && occupancyJson.Length > 2)
					{
						var occupancyObj = JsonConvert.DeserializeObject<dynamic>(occupancyJson);
						var existingRecords = occupancyObj?.detail;
						if (existingRecords != null)
						{
							var conflictDates = new List<string>();
							foreach (var rec in existingRecords)
							{
								string recStatus = (rec.status?.ToString() ?? "").Trim().ToLower();
								if (recStatus == "occupied" || recStatus == "booked" || recStatus == "blocked")
									conflictDates.Add(rec.rodate?.ToString() ?? "");
							}

							if (conflictDates.Count > 0)
							{
								string roomNumber = roomObj.roomnumber?.ToString() ?? roomId;
								results.Add("Room " + roomNumber + " is already occupied/booked/blocked for " + conflictDates.Count + " day(s) in the selected range");
								continue;
							}
						}
					}

					// ──────────────────────────────────────────────────────
					// 4. Create occupancy entries for each day with status Blocked
					// ──────────────────────────────────────────────────────
					Guid? roomTenantId = null;
					string roomTenantStr = roomObj.tenantid?.ToString();
					if (!string.IsNullOrEmpty(roomTenantStr) && Guid.TryParse(roomTenantStr, out var parsedRoomTenantId))
						roomTenantId = parsedRoomTenantId;
					if (roomTenantId == null)
					{
						var sessionTenant = HttpContext.Session.GetString("NalamVazhachoosedtenantid")
							?? HttpContext.Session.GetString("NalamVazhatenantid");
						if (!string.IsNullOrEmpty(sessionTenant) && Guid.TryParse(sessionTenant, out var parsedSessionTenantId))
							roomTenantId = parsedSessionTenantId;
					}

					for (int i = 0; i < totalDays; i++)
					{
						var model = new RoomOccupancyStatusModel
						{
							RoomOccupancyStatusid = Guid.NewGuid(),
							tenantid = roomTenantId,
							block = Guid.Parse(roomObj.block.ToString()),
							building = Guid.Parse(roomObj.building.ToString()),
							floor = Guid.Parse(roomObj.floor.ToString()),
							ipdno = Guid.Parse(request.ipdApplicationFormId.ToString()),
							room = Guid.Parse(roomId),
							bookeddate = arrivalDate.AddDays(i),
							status = "Blocked",
							craftmyapp_actionmethodname = "Add_Room_Occupancy_Status",
							createduser = new Guid(loginUserID)
						};

						var result = await ApiClient.Post_ApiValuesGetString(httpClient,
							"api/RoomOccupancyStatus/Add_Room_Occupancy_Status", model);

						results.Add(result);
					}
				}

				// ──────────────────────────────────────────────────────────
				// 5. Return result
				// ──────────────────────────────────────────────────────────
				var conflictMessages = results.Where(r => r != null && r.StartsWith("Room ") && r.Contains("already occupied")).ToList();
				var successResults = results.Where(r => r != null && r.Replace("\"", "").Contains("201.1")).ToList();

				if (conflictMessages.Count > 0 && successResults.Count == 0)
					return Json(new { success = false, message = string.Join("\n", conflictMessages) });

				if (conflictMessages.Count > 0 && successResults.Count > 0)
					return Json(new { success = true, message = "Partial allotment. " + string.Join("; ", conflictMessages) });

				bool allSuccess = results.All(r => r != null && r.Replace("\"", "").Contains("201.1"));
				if (allSuccess)
					return Json(new { success = true, message = "Room allotted for " + totalDays + " day(s): " + arrivalDate.ToString("dd/MM/yyyy") + " to " + departureDate.ToString("dd/MM/yyyy") + "." });

				return Json(new { success = false, message = string.Join("; ", results) });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - IPDApplicationForm / Allot_Room");
				return Json(new { success = false, message = ex.Message });
			}
		}

		[HttpPost()]
		public virtual async Task<IActionResult> Mark_IPD_Waitlisted([FromBody] NalamVazha.Models.IPDBookingStatusUpdateModel model)
		{
			try
			{
				var loginUserId = HttpContext.Session.GetString("NalamVazhaloginUserID");
				if (string.IsNullOrEmpty(loginUserId))
					return Json(new { success = false, message = "Session Expired" });
				if (model == null || string.IsNullOrEmpty(model.IPDApplicationFormid))
					return Json(new { success = false, message = "IPDApplicationFormid is required" });

				model.bookingstatus = "Waitlisted";
				model.modifieduser = loginUserId;

				var json = await ApiClient.Post_ApiValuesGetRawString(getHttpClient(),
					"api/IPDApplicationForm/Update_IPD_Booking_Status", model);

				var msg = (json ?? "").Replace("\"", "");
				var ok = msg.Contains("201.1");
				return Json(new { success = ok, message = msg });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Mark_IPD_Waitlisted error: " + ex.Message);
				return Json(new { success = false, message = ex.Message });
			}
		}

		[HttpPost()]
		public virtual async Task<IActionResult> Mark_IPD_Rejected([FromBody] NalamVazha.Models.IPDBookingStatusUpdateModel model)
		{
			try
			{
				var loginUserId = HttpContext.Session.GetString("NalamVazhaloginUserID");
				if (string.IsNullOrEmpty(loginUserId))
					return Json(new { success = false, message = "Session Expired" });
				if (model == null || string.IsNullOrEmpty(model.IPDApplicationFormid))
					return Json(new { success = false, message = "IPDApplicationFormid is required" });

				model.bookingstatus = "Rejected";
				model.modifieduser = loginUserId;

				var json = await ApiClient.Post_ApiValuesGetRawString(getHttpClient(),
					"api/IPDApplicationForm/Update_IPD_Booking_Status", model);

				var msg = (json ?? "").Replace("\"", "");
				var ok = msg.Contains("201.1");
				return Json(new { success = ok, message = msg });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Mark_IPD_Rejected error: " + ex.Message);
				return Json(new { success = false, message = ex.Message });
			}
		}

		[HttpGet()]
		public virtual async Task<IActionResult> IPD_Screening(string IPDApplicationFormid)
	{
			string redirectTo = "";
			if (HttpContext.Session.GetString("NalamVazharole_JSON") != null)
			{
				//DataTable NalamVazharole_JSON = HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
				//DataView dv = new DataView(NalamVazharole_JSON);
				//dv.RowFilter = "controllername='IPDApplicationForm' AND viewname='list'";

				//if (dv.Count > 0)
				//	redirectTo = dv[0]["actionmethodname"] as string;

				try
				{
					var client = getHttpClient();
					var loginId = HttpContext.Session.GetString("NalamVazhaloginUserID");
					var jsonIpd = await ApiClient.Get_ApiValues(client,
						"api/IPDApplicationForm/getById_IPDApplicationForm?IPDApplicationFormid=" + IPDApplicationFormid + "&loginUserID=" + loginId);
					if (jsonIpd.Length <= 2)
					{
						TempData["message"] = "Data Not Found - Contact Administrator";
						return RedirectToAction(string.IsNullOrEmpty(redirectTo) ? "Approved_IPD_Application_Forms" : redirectTo);
					}

					var ipd = JsonConvert.DeserializeObject<IPDApplicationFormModel>(jsonIpd);
					var vm = new IPDScreeningViewModel { IpdForm = ipd, QuestionRows = new List<IPDScreeningQuestionRow>() };

					var jsonAssessList = await ApiClient.Get_ApiValues(client,
						"api/Assessment/getAssessments_by_ipdform?ipdapplicationformid=" + IPDApplicationFormid + "&loginUserID=" + loginId);
					string assessmentId = null;
					if (!string.IsNullOrWhiteSpace(jsonAssessList) && jsonAssessList.TrimStart().StartsWith("["))
					{
						try
						{
							var arr = JArray.Parse(jsonAssessList);
							if (arr.Count > 0)
							{
								var first = arr[0];
								assessmentId = first["Assessmentid"]?.ToString() ?? first["assessmentid"]?.ToString();
							}
						}
						catch (Exception exParse)
						{
							_logger.LogWarning(exParse, "IPD_Screening: could not parse assessment list JSON");
						}
					}

					if (!string.IsNullOrEmpty(assessmentId))
					{
						vm.HasAssessment = true;
						var jsonAss = await ApiClient.Get_ApiValues(client,
							"api/Assessment/getById_Assessment?Assessmentid=" + assessmentId + "&loginUserID=" + loginId);
						if (jsonAss.Length > 2)
							vm.Assessment = JsonConvert.DeserializeObject<AssessmentModel>(jsonAss);

						var qJson = await ApiClient.Get_ApiValues(client,
							"api/Assessment/getById_assessmentquestions?Assessmentid=" + assessmentId + "&loginUserID=" + loginId);
						var aJson = await ApiClient.Get_ApiValues(client,
							"api/Assessment/getById_patientanswers?Assessmentid=" + assessmentId + "&loginUserID=" + loginId);
						vm.QuestionRows = BuildIPDScreeningQuestionRows(qJson, aJson);
					}
					else
						vm.InfoMessage = "No assessment is linked to this IPD application.";

					var bs = ipd.bookingstatus ?? "";
					var userRole = HttpContext.Session.GetString("NalamVazhauserrole") ?? "";
					ViewBag.AllowDoctorToAdmitPatients = await GetAllowDoctorToAdmitPatients(ipd?.tenantid);
					vm.CanConfirmAdmission = vm.HasAssessment
						&& userRole != "Health Seeker"
						&& bs != "Admission Confirmed"
						&& bs != "Admitted"
						&& bs != "Cancelled"
						&& bs != "Rejected";

					return View(vm);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "IPD_Screening error: " + ex.Message);
					TempData["errMessage"] = "Error while loading screening";
					return RedirectToAction(string.IsNullOrEmpty(redirectTo) ? "Approved_IPD_Application_Forms" : redirectTo);
				}
			}
			TempData["errMessage"] = "Session Expired";
			return RedirectToAction("Logout", "users");
		}

		private static List<IPDScreeningQuestionRow> BuildIPDScreeningQuestionRows(string questionsJson, string answersJson)
		{
			var rows = new List<IPDScreeningQuestionRow>();
			if (string.IsNullOrWhiteSpace(questionsJson) || questionsJson.Length <= 2)
				return rows;

			JArray qArr;
			try { qArr = JArray.Parse(questionsJson); }
			catch { return rows; }

			var ansByRef = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			if (!string.IsNullOrWhiteSpace(answersJson) && answersJson.Length > 2)
			{
				try
				{
					var aArr = JArray.Parse(answersJson);
					foreach (var a in aArr)
					{
						var k = a["Questionsref"]?.ToString() ?? a["questionsref"]?.ToString();
						var v = a["Answervalue"]?.ToString() ?? a["answervalue"]?.ToString();
						if (!string.IsNullOrEmpty(k) && !ansByRef.ContainsKey(k))
							ansByRef[k] = v ?? "";
					}
				}
				catch { /* ignore */ }
			}

			foreach (var q in qArr)
			{
				var qid = q["Assessment_assessmentquestionsid"]?.ToString() ?? q["assessment_assessmentquestionsid"]?.ToString();
				var qtext = q["Questions"]?.ToString() ?? q["questions"]?.ToString();
				var atype = q["Answertype"]?.ToString() ?? q["answertype"]?.ToString();
				var ans = "";
				if (qid != null && ansByRef.TryGetValue(qid, out var av))
					ans = av;
				if (string.IsNullOrEmpty(ans))
					ans = "—";
				rows.Add(new IPDScreeningQuestionRow
				{
					QuestionText = qtext ?? "",
					AnswerText = ans,
					AnswerType = atype ?? ""
				});
			}
			return rows;
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

		[HttpPost()]
		public virtual async Task<IActionResult> Mark_IPD_Admitted([FromBody] IPDBookingStatusUpdateModel model)
		{
			try
			{
				var loginUserId = HttpContext.Session.GetString("NalamVazhaloginUserID");
				if (string.IsNullOrEmpty(loginUserId))
					return Json(new { success = false, message = "Session Expired" });
				if (model == null || string.IsNullOrEmpty(model.IPDApplicationFormid))
					return Json(new { success = false, message = "IPDApplicationFormid is required" });

				model.bookingstatus = "Admitted";
				model.modifieduser = loginUserId;

				var json = await ApiClient.Post_ApiValuesGetRawString(getHttpClient(),
					"api/IPDApplicationForm/Update_IPD_Booking_Status", model);

				var msg = (json ?? "").Replace("\"", "");
				var ok = msg.Contains("201.1");

				if (ok)
				{
					var completeBody = new CompleteIPScreeningForIPDRequestModel { IPDApplicationFormid = model.IPDApplicationFormid };
					var apptJson = await ApiClient.Post_ApiValuesGetRawString(getHttpClient(),
						"api/OPDForm/Complete_IP_New_For_IPD_Booking", completeBody);
					var apptMsg = (apptJson ?? "").Replace("\"", "");
					if (!apptMsg.Contains("201.1"))
						_logger.LogWarning("Mark_IPD_Admitted: booking status updated but IP New appointment completion failed: {Msg}", apptMsg);
				}

				return Json(new { success = ok, message = msg });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Mark_IPD_Admitted error: " + ex.Message);
				return Json(new { success = false, message = ex.Message });
			}
		}

		[HttpPost()]
		public virtual async Task<IActionResult> Complete_IP_New_Appointments([FromBody] IPDBookingStatusUpdateModel model)
		{
			try
			{
				var loginUserId = HttpContext.Session.GetString("NalamVazhaloginUserID");
				if (string.IsNullOrEmpty(loginUserId))
					return Json(new { success = false, message = "Session Expired" });
				if (model == null || string.IsNullOrEmpty(model.IPDApplicationFormid))
					return Json(new { success = false, message = "IPDApplicationFormid is required" });

				var completeBody = new CompleteIPScreeningForIPDRequestModel { IPDApplicationFormid = model.IPDApplicationFormid };
				var json = await ApiClient.Post_ApiValuesGetRawString(getHttpClient(),
					"api/OPDForm/Complete_IP_New_For_IPD_Booking", completeBody);
				var msg = (json ?? "").Replace("\"", "");
				var ok = msg.Contains("201.1");
				return Json(new { success = ok, message = msg });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Complete_IP_New_Appointments error: " + ex.Message);
				return Json(new { success = false, message = ex.Message });
			}
		}

		[HttpPost()]
		public virtual async Task<IActionResult> Initiate_IPD_Discharge([FromBody] IPDBookingStatusUpdateModel model)
		{
			try
			{
				var loginUserId = HttpContext.Session.GetString("NalamVazhaloginUserID");
				if (string.IsNullOrEmpty(loginUserId))
					return Json(new { success = false, message = "Session Expired" });
				if (model == null || string.IsNullOrEmpty(model.IPDApplicationFormid))
					return Json(new { success = false, message = "IPDApplicationFormid is required" });
				if (!HasUserRole("Frontdesk Admin"))
					return Json(new { success = false, message = "Only Frontdesk Admin can initiate discharge." });

				var ipdJson = await ApiClient.Get_ApiValues(getHttpClient(),
					"api/IPDApplicationForm/getById_IPDApplicationForm?IPDApplicationFormid=" + model.IPDApplicationFormid +
					"&loginUserID=" + loginUserId);
				var ipd = string.IsNullOrWhiteSpace(ipdJson) || ipdJson.Length <= 2
					? null
					: JsonConvert.DeserializeObject<IPDApplicationFormModel>(ipdJson);
				if (ipd == null)
					return Json(new { success = false, message = "IPD application not found." });
				if (!string.Equals(ipd.bookingstatus?.Trim(), "Admitted", StringComparison.OrdinalIgnoreCase))
					return Json(new { success = false, message = "Discharge can be initiated only for an admitted patient." });
				var outstandingBalance = await GetIpdOutstandingBalance(ipd, model.IPDApplicationFormid, loginUserId);
				if (outstandingBalance > 0.009m)
					return Json(new { success = false, message = $"Discharge cannot be initiated. Rs. {outstandingBalance:N2} is still pending." });

				model.bookingstatus = "Discharge Initiated";
				model.modifieduser = loginUserId;
				var json = await ApiClient.Post_ApiValuesGetRawString(getHttpClient(),
					"api/IPDApplicationForm/Update_IPD_Booking_Status", model);
				var msg = (json ?? "").Replace("\"", "");
				return Json(new { success = msg.Contains("201.1"), message = msg });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Initiate_IPD_Discharge error: " + ex.Message);
				return Json(new { success = false, message = ex.Message });
			}
		}

		[HttpPost()]
		public virtual async Task<IActionResult> Mark_IPD_Discharged([FromBody] IPDBookingStatusUpdateModel model)
		{
			try
			{
				var loginUserId = HttpContext.Session.GetString("NalamVazhaloginUserID");
				if (string.IsNullOrEmpty(loginUserId))
					return Json(new { success = false, message = "Session Expired" });
				if (model == null || string.IsNullOrEmpty(model.IPDApplicationFormid))
					return Json(new { success = false, message = "IPDApplicationFormid is required" });
				if (!HasUserRole("Frontdesk Admin"))
					return Json(new { success = false, message = "Only Frontdesk Admin can mark a patient as discharged." });

				var ipdJson = await ApiClient.Get_ApiValues(getHttpClient(),
					"api/IPDApplicationForm/getById_IPDApplicationForm?IPDApplicationFormid=" + model.IPDApplicationFormid +
					"&loginUserID=" + loginUserId);
				var ipd = string.IsNullOrWhiteSpace(ipdJson) || ipdJson.Length <= 2
					? null
					: JsonConvert.DeserializeObject<IPDApplicationFormModel>(ipdJson);
				if (ipd == null)
					return Json(new { success = false, message = "IPD application not found." });
				if (!string.Equals(ipd.bookingstatus?.Trim(), "Discharge Initiated", StringComparison.OrdinalIgnoreCase))
					return Json(new { success = false, message = "Discharge must be initiated before the patient can be marked as discharged." });
				var outstandingBalance = await GetIpdOutstandingBalance(ipd, model.IPDApplicationFormid, loginUserId);
				if (outstandingBalance > 0.009m)
					return Json(new { success = false, message = $"Patient cannot be discharged. Rs. {outstandingBalance:N2} is still pending." });

				var workflow = await GetDischargeWorkflowState(model.IPDApplicationFormid, loginUserId);
				if (!workflow.FeedbackSubmitted)
					return Json(new { success = false, message = "Health Seeker feedback must be submitted before discharge." });
				if (!workflow.ChecklistCompleted)
					return Json(new { success = false, message = "The discharge checklist must be verified before discharge." });

				model.bookingstatus = "Discharged";
				model.modifieduser = loginUserId;

				var json = await ApiClient.Post_ApiValuesGetRawString(getHttpClient(),
					"api/IPDApplicationForm/Mark_IPD_Discharged", model);

				var msg = (json ?? "").Replace("\"", "");
				var ok = msg.Contains("201.1");
				return Json(new { success = ok, message = msg });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Mark_IPD_Discharged error: " + ex.Message);
				return Json(new { success = false, message = ex.Message });
			}
		}

		private async Task<decimal> GetIpdOutstandingBalance(
			IPDApplicationFormModel ipd,
			string ipdApplicationFormId,
			string loginUserId)
		{
			if (ipd == null || ipd.patientname == Guid.Empty || !Guid.TryParse(ipdApplicationFormId, out var ipdId))
				return decimal.MaxValue;

			var json = await ApiClient.Get_ApiValues(getHttpClient(),
				"api/BillingPayment/Get_Unified_Pending_Receivables?PatientID=" + ipd.patientname +
				"&IPDNo=" + ipdId + "&Type=IPD&loginUserID=" + loginUserId);
			var rows = JsonConvert.DeserializeObject<List<UnifiedReceivableItemModel>>(json ?? "[]")
				?? new List<UnifiedReceivableItemModel>();
			return Math.Max(0m, rows.Sum(x => x.balance));
		}

		[HttpGet()]
		public virtual async Task<IActionResult> Get_IPD_Discharge_Workflow_State(string IPDApplicationFormid)
		{
			var loginUserId = HttpContext.Session.GetString("NalamVazhaloginUserID");
			if (string.IsNullOrEmpty(loginUserId) || !HasUserRole("Frontdesk Admin"))
				return Json(new { feedbackSubmitted = false, checklistCompleted = false, hasChecklistTemplate = false, checklistAssessmentId = "" });
			if (!Guid.TryParse(IPDApplicationFormid, out _))
				return Json(new { feedbackSubmitted = false, checklistCompleted = false, hasChecklistTemplate = false, checklistAssessmentId = "" });

			var state = await GetDischargeWorkflowState(IPDApplicationFormid, loginUserId);
			return Json(new
			{
				feedbackSubmitted = state.FeedbackSubmitted,
				checklistCompleted = state.ChecklistCompleted,
				hasChecklistTemplate = state.HasChecklistTemplate,
				checklistAssessmentId = state.ChecklistAssessmentId
			});
		}

		[HttpPost()]
		public virtual async Task<IActionResult> Mark_IPD_Admission_Confirmed([FromBody] IPDBookingStatusUpdateModel model)
		{
			try
			{
				var loginUserId = HttpContext.Session.GetString("NalamVazhaloginUserID");
				if (string.IsNullOrEmpty(loginUserId))
					return Json(new { success = false, message = "Session Expired" });
				if (model == null || string.IsNullOrEmpty(model.IPDApplicationFormid))
					return Json(new { success = false, message = "IPDApplicationFormid is required" });

				var client = getHttpClient();
				var requestedDirectAdmission = string.Equals(model.bookingstatus ?? "", "Admitted", StringComparison.OrdinalIgnoreCase)
					|| string.Equals(model.bookingstatus ?? "", "Arrival Confirmed", StringComparison.OrdinalIgnoreCase);

				if (requestedDirectAdmission)
				{
					if (HttpContext.Session.GetString("NalamVazhauserrole") != "Doctor")
						return Json(new { success = false, message = "Doctor direct admission is allowed only for Doctor login." });

					var jsonIpd = await ApiClient.Get_ApiValues(client,
						"api/IPDApplicationForm/getById_IPDApplicationForm?IPDApplicationFormid=" + model.IPDApplicationFormid
						+ "&loginUserID=" + loginUserId);
					if (string.IsNullOrWhiteSpace(jsonIpd) || jsonIpd.Length <= 2)
						return Json(new { success = false, message = "IPD application not found" });

					var ipd = JsonConvert.DeserializeObject<IPDApplicationFormModel>(jsonIpd);
					if (!await GetAllowDoctorToAdmitPatients(ipd?.tenantid))
						return Json(new { success = false, message = "Doctor direct admission is not enabled for this healthcare provider." });
				}

				//model.bookingstatus = "Approved for Admission";
				model.bookingstatus = requestedDirectAdmission ? "Arrival Confirmed" : "Admission Approved";
				if (requestedDirectAdmission)
					model.verifiedstatus = "Direct Admission";
				model.modifieduser = loginUserId;
				var json = await ApiClient.Post_ApiValuesGetRawString(client,
					"api/IPDApplicationForm/Update_IPD_Booking_Status", model);

				var msg = (json ?? "").Replace("\"", "");
				var ok = msg.Contains("201.1");
				string appointmentMsg = null;
				if (ok)
				{
					var completeBody = new CompleteIPScreeningForIPDRequestModel { IPDApplicationFormid = model.IPDApplicationFormid };
					var apptJson = await ApiClient.Post_ApiValuesGetRawString(client,
						requestedDirectAdmission
							? "api/OPDForm/Complete_IP_New_For_IPD_Booking"
							: "api/OPDForm/Complete_IP_Screening_For_IPD_Booking", completeBody);
					appointmentMsg = (apptJson ?? "").Replace("\"", "");
					if (!appointmentMsg.Contains("201.1"))
						_logger.LogWarning("Mark_IPD_Admission_Confirmed: booking status updated but appointment completion failed: {Msg}", appointmentMsg);
				}
				return Json(new { success = ok, message = msg, bookingstatus = model.bookingstatus, appointmentCompletionMessage = appointmentMsg });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Mark_IPD_Admission_Confirmed error: " + ex.Message);
				return Json(new { success = false, message = ex.Message });
			}
		}


		[HttpPost()]
		public virtual async Task<IActionResult> Mark_IPD_Admission_InEligible([FromBody] IPDBookingStatusUpdateModel model)
		{
			try
			{
				var loginUserId = HttpContext.Session.GetString("NalamVazhaloginUserID");
				if (string.IsNullOrEmpty(loginUserId))
					return Json(new { success = false, message = "Session Expired" });
				if (model == null || string.IsNullOrEmpty(model.IPDApplicationFormid))
					return Json(new { success = false, message = "IPDApplicationFormid is required" });

				model.bookingstatus = "Rejected";
				model.modifieduser = loginUserId;
				var client = getHttpClient();
				var json = await ApiClient.Post_ApiValuesGetRawString(client,
					"api/IPDApplicationForm/Update_IPD_Booking_Status", model);

				var msg = (json ?? "").Replace("\"", "");
				var ok = msg.Contains("201.1");
				string appointmentMsg = null;
				if (ok)
				{
					var completeBody = new CompleteIPScreeningForIPDRequestModel { IPDApplicationFormid = model.IPDApplicationFormid };
					var apptJson = await ApiClient.Post_ApiValuesGetRawString(client,
						"api/OPDForm/Complete_IP_Screening_For_IPD_Booking", completeBody);
					appointmentMsg = (apptJson ?? "").Replace("\"", "");
					if (!appointmentMsg.Contains("201.1"))
						_logger.LogWarning("Mark_IPD_Admission_InEligible: booking status updated but IP Screening appointment completion failed: {Msg}", appointmentMsg);
				}
				return Json(new { success = ok, message = msg, appointmentCompletionMessage = appointmentMsg });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Mark_IPD_Admission_InEligible error: " + ex.Message);
				return Json(new { success = false, message = ex.Message });
			}
		}

		public class RoomAllotmentRequest
		{
			public List<string> roomIds { get; set; }
			public string bookingId { get; set; }
			public string ipdApplicationFormId { get; set; }
			public DateTime? dateOfArrival { get; set; }
			public DateTime? dateOfDeparture { get; set; }
		}

		/// <summary>Returns patient GUID and tenant info for a given IPD form — used by the Allot Doctor modal in FrontDesk Dashboard.</summary>
		[HttpGet()]
		public virtual async Task<string> Get_IPD_PatientDetails(string IPDApplicationFormid)
		{
			return await ApiClient.Get_ApiValues(getHttpClient(),
				"api/IPDApplicationForm/Get_IPD_Payment_Details?IPDApplicationFormid=" + IPDApplicationFormid +
				"&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
		}

		// -- STEP 6: Patient Arrival & Charges
		[HttpGet()]
		public virtual async Task<IActionResult> Patient_Arrival_Charges(string IPDApplicationFormid)
		{
			try
			{
				var json = await ApiClient.Get_ApiValues(getHttpClient(),
					"api/IPDApplicationForm/Get_IPD_Payment_Details?IPDApplicationFormid=" + IPDApplicationFormid +
					"&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
				if (json.Length > 2)
				{
					var ipdPayment = JsonConvert.DeserializeObject<NalamVazha.Models.IPDPaymentDetailsModel>(json);

					// Consultation Fee + Admission Fee = receivable amount
					decimal consultationFee = 0;
					decimal admissionFee = 600;// from configured admission fee or skip this if not applicable
					decimal amountToReceive = consultationFee + admissionFee;

					var billing = new NalamVazha.Models.BillingPaymentModel
					{
						amount = amountToReceive,
						receivedamount = amountToReceive,
						craftmyapp_actionmethodname = "Add_Billing_Payment",
						isdeleted = false
					};

					if (ipdPayment != null)
					{
						if (Guid.TryParse(ipdPayment.tenantid, out var tid))
							billing.tenantid = tid;
						if (Guid.TryParse(IPDApplicationFormid, out var ipdGuid))
							billing.ipdnumber = ipdGuid;
						if (!string.IsNullOrEmpty(ipdPayment.patientname) && Guid.TryParse(ipdPayment.patientname, out var pname))
							billing.patientname = pname;
						if (!string.IsNullOrEmpty(ipdPayment.patientvisitid) && Guid.TryParse(ipdPayment.patientvisitid, out var pvisit) && pvisit != Guid.Empty)
							billing.patientvisit = pvisit;
					}

					ViewBag.IpdPaymentDetails = ipdPayment;
					ViewBag.IPDApplicationFormid = IPDApplicationFormid;
					return View(billing);
				}
				TempData["errMessage"] = "IPD record not found";
				return RedirectToAction("Approved_IPD_Application_Forms");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Patient_Arrival_Charges error: " + ex.Message);
				TempData["errMessage"] = "Error loading record";
				return RedirectToAction("Approved_IPD_Application_Forms");
			}
		}



		[HttpGet()]
		public virtual async Task<IActionResult> Pre_Admission_Consultation(string IPDApplicationFormid)
		{
			try
			{
				var json = await ApiClient.Get_ApiValues(getHttpClient(),
					"api/IPDApplicationForm/Get_IPD_Payment_Details?IPDApplicationFormid=" + IPDApplicationFormid +
					"&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
				if (json.Length > 2)
				{
					var ipdPayment = JsonConvert.DeserializeObject<NalamVazha.Models.IPDPaymentDetailsModel>(json);

					// Consultation fee is determined dynamically in the view via IP New task type fee schedule.
					decimal amountToReceive = 0;

					var billing = new NalamVazha.Models.BillingPaymentModel
					{
						amount = amountToReceive,
						receivedamount = amountToReceive,
						craftmyapp_actionmethodname = "Add_Billing_Payment",
						isdeleted = false
					};

					if (ipdPayment != null)
					{
						if (Guid.TryParse(ipdPayment.tenantid, out var tid))
							billing.tenantid = tid;
						if (Guid.TryParse(IPDApplicationFormid, out var ipdGuid))
							billing.ipdnumber = ipdGuid;
						if (!string.IsNullOrEmpty(ipdPayment.patientname) && Guid.TryParse(ipdPayment.patientname, out var pname))
							billing.patientname = pname;
						if (!string.IsNullOrEmpty(ipdPayment.patientvisitid) && Guid.TryParse(ipdPayment.patientvisitid, out var pvisit) && pvisit != Guid.Empty)
							billing.patientvisit = pvisit;
					}

					ViewBag.IpdPaymentDetails    = ipdPayment;
					ViewBag.IPDApplicationFormid  = IPDApplicationFormid;
					ViewBag.PatientGuid           = ipdPayment?.patientname ?? "";
					ViewBag.TenantId              = ipdPayment?.tenantid   ?? "";
					return View(billing);
				}
				TempData["errMessage"] = "IPD record not found";
				return RedirectToAction("Approved_IPD_Application_Forms");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Patient_Arrival_Charges error: " + ex.Message);
				TempData["errMessage"] = "Error loading record";
				return RedirectToAction("Approved_IPD_Application_Forms");
			}
		}


		[HttpPost()]
		public virtual async Task<IActionResult> Record_Patient_Arrival([FromBody] NalamVazha.Models.IPDPatientArrivalRequestModel model)
		{
			try
			{
				var json = await ApiClient.Post_ApiValuesGetRawString(getHttpClient(),
					"api/IPDApplicationForm/Record_Patient_Arrival", model);
				return Content(json, "application/json");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Record_Patient_Arrival error: " + ex.Message);
				return BadRequest(ex.Message);
			}
		}

		// -- STEP 7: Final Admission (merged with Billing Dashboard: show remaining amount, receive payment, then confirm admitted)
		[HttpGet()]
		public virtual async Task<IActionResult> Final_Admission(string IPDApplicationFormid)
		{
			try
			{
				var json = await ApiClient.Get_ApiValues(getHttpClient(),
					"api/IPDApplicationForm/Get_IPD_Billing_Summary?IPDApplicationFormid=" + IPDApplicationFormid +
					"&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
				if (json.Length > 2)
				{
					var model = JsonConvert.DeserializeObject<NalamVazha.Models.IPDBillingSummaryModel>(json);
					ViewBag.IPDApplicationFormid = IPDApplicationFormid;
					ViewBag.patientvisitid = model.patientvisitid;
					return View(model);
				}
				TempData["errMessage"] = "IPD record not found";
				return RedirectToAction("Approved_IPD_Application_Forms");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Final_Admission error: " + ex.Message);
				TempData["errMessage"] = "Error loading record";
				return RedirectToAction("Approved_IPD_Application_Forms");
			}
		}

		[HttpPost()]
		public virtual async Task<IActionResult> Confirm_Final_Admission([FromBody] NalamVazha.Models.IPDFinalAdmissionRequestModel model)
		{
			try
			{
				var json = await ApiClient.Post_ApiValuesGetRawString(getHttpClient(),
					"api/IPDApplicationForm/Final_Admission", model);
				return Content(json, "application/json");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Confirm_Final_Admission error: " + ex.Message);
				return BadRequest(ex.Message);
			}
		}

		// -- STEP 8: Billing Dashboard – shows billing summary + cancel/refund for any paid status
		[AllowAnonymous]
		[HttpGet()]
		public virtual async Task<IActionResult> Billing_Dashboard(string IPDApplicationFormid, bool opencancel = false, bool triggerrefund = false)
		{
			var sessionRole = HttpContext.Session.GetString("NalamVazhauserrole") ?? "";
			var isHealthSeeker = sessionRole.Equals("Health Seeker", StringComparison.OrdinalIgnoreCase);
			if (string.IsNullOrEmpty(IPDApplicationFormid))
			{
				TempData["errMessage"] = "Invalid request";
				return isHealthSeeker
					? RedirectToAction("Index", "PatientDashboard", new { isDashboardView = "Y" })
					: RedirectToAction("Approved_IPD_Application_Forms");
			}
			try
			{
				// Keep the Health Seeker's bearer token/session on every billing API call.
				var client = getHttpClient();
				var json = await ApiClient.Get_ApiValues(client,
					"api/IPDApplicationForm/Get_IPD_Billing_Summary?IPDApplicationFormid=" + IPDApplicationFormid +
					"&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
				if (json.Length > 2)
				{
					var summaryModel = JsonConvert.DeserializeObject<NalamVazha.Models.IPDBillingSummaryModel>(json);
					// Remaining balance is the receivable amount
					decimal amountToReceive = summaryModel.remainingbalance > 0 ? summaryModel.remainingbalance : 0;

					var billing = new NalamVazha.Models.BillingPaymentModel
					{
						amount = amountToReceive,
						receivedamount = amountToReceive,
						craftmyapp_actionmethodname = "Add_Billing_Payment",
						isdeleted = false
					};

					if (Guid.TryParse(IPDApplicationFormid, out var ipdGuid))
						billing.ipdnumber = ipdGuid;
					if (!string.IsNullOrEmpty(summaryModel.patientvisitid) && Guid.TryParse(summaryModel.patientvisitid, out var pvisit) && pvisit != Guid.Empty)
						billing.patientvisit = pvisit;

					// Also fetch IPD payment details for tenantid and patientname
					var healthSeekerOwnsIpd = !isHealthSeeker;
					var healthSeekerHasPendingBalance = !isHealthSeeker;
					try
					{
						var ipdJson = await ApiClient.Get_ApiValues(client,
							"api/IPDApplicationForm/Get_IPD_Payment_Details?IPDApplicationFormid=" + IPDApplicationFormid +
							"&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
						if (ipdJson.Length > 2)
						{
							var ipdPayment = JsonConvert.DeserializeObject<NalamVazha.Models.IPDPaymentDetailsModel>(ipdJson);
							if (ipdPayment != null)
							{
								healthSeekerOwnsIpd = !isHealthSeeker || string.Equals(
									ipdPayment.patientname,
									HttpContext.Session.GetString("NalamVazhaloginUserID"),
									StringComparison.OrdinalIgnoreCase);
								if (Guid.TryParse(ipdPayment.tenantid, out var tid))
									billing.tenantid = tid;
								if (!string.IsNullOrEmpty(ipdPayment.patientname) && Guid.TryParse(ipdPayment.patientname, out var pname))
								{
									billing.patientname = pname;
									try
									{
										var pendingJson = await ApiClient.Get_ApiValues(client,
											"api/BillingPayment/Get_Unified_Pending_Receivables?PatientID=" + pname
											+ "&IPDNo=" + IPDApplicationFormid
											+ "&Type=IPD"
											+ "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
										var pendingRows = JsonConvert.DeserializeObject<List<NalamVazha.Models.UnifiedReceivableItemModel>>(pendingJson ?? "[]")
											?? new List<NalamVazha.Models.UnifiedReceivableItemModel>();
										ViewBag.UnifiedPendingReceivables = pendingRows;
										var outstanding = pendingRows.Sum(x => x.balance);
										healthSeekerHasPendingBalance = !isHealthSeeker || outstanding > 0;
										var receivableTotal = pendingRows.Sum(x => x.amount);
										var rowPaidTotal = pendingRows.Sum(x => x.paidamount);
										summaryModel.remainingbalance = outstanding;
										billing.amount = outstanding > 0 ? outstanding : 0;
										billing.receivedamount = billing.amount;
										if (pendingRows.Count > 0)
										{
											summaryModel.totalamount = receivableTotal;
											summaryModel.totalamountpaid = rowPaidTotal;
										}
									}
									catch (Exception rxEx)
									{
										_logger.LogError(rxEx, "Billing_Dashboard unified pending receivables load failed: " + rxEx.Message);
										ViewBag.UnifiedPendingReceivables = new List<NalamVazha.Models.UnifiedReceivableItemModel>();
									}
								}

								if (!string.IsNullOrEmpty(ipdPayment.IPDApplicationFormid) && Guid.TryParse(ipdPayment.IPDApplicationFormid, out var ipdnumber))
									billing.ipdnumber = ipdnumber;

								 
							}
						}
					}
					catch { /* non-critical — billing form still works without these */ }
					if (isHealthSeeker && (!healthSeekerOwnsIpd || !healthSeekerHasPendingBalance))
					{
						TempData["errMessage"] = healthSeekerOwnsIpd
							? "There is no pending IPD balance to pay."
							: "You can only access billing for your own IPD booking.";
						return RedirectToAction("Index", "PatientDashboard", new { isDashboardView = "Y" });
					}

					try
					{
						var paymentHistoryJson = await ApiClient.Get_ApiValues(client,
							"api/IPDApplicationForm/Get_Cancel_IPD_Payment_History?IPDApplicationFormid=" + IPDApplicationFormid);
						ViewBag.PaymentHistoryJson = string.IsNullOrWhiteSpace(paymentHistoryJson) ? "[]" : paymentHistoryJson;
						var paymentRows = Newtonsoft.Json.Linq.JArray.Parse(Convert.ToString(ViewBag.PaymentHistoryJson) ?? "[]");
						decimal recordedPaidAmount = 0;
						foreach (var paymentRow in paymentRows)
						{
							var status = paymentRow["paymentstatus"]?.ToString() ?? "";
							if (!status.Equals("Success", StringComparison.OrdinalIgnoreCase)) continue;
							var amount = ParseBillingDecimal(paymentRow["amount"]?.ToString());
							var refunded = ParseBillingDecimal(paymentRow["refundedamount"]?.ToString());
							recordedPaidAmount += amount - refunded;
						}
						ViewBag.RecordedPaidAmount = recordedPaidAmount;
					}
					catch (Exception payEx)
					{
						_logger.LogError(payEx, "Billing_Dashboard payment history load failed: " + payEx.Message);
						ViewBag.PaymentHistoryJson = "[]";
						ViewBag.RecordedPaidAmount = 0m;
					}

					try
					{
						var receivablesJson = await ApiClient.Get_ApiValues(client,
							"api/IPDApplicationForm/Get_IPD_All_Receivables?IPDApplicationFormid=" + IPDApplicationFormid +
							"&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
						var paymentsJson = await ApiClient.Get_ApiValues(client,
							"api/IPDApplicationForm/Get_IPD_Billing_Payments_List?IPDApplicationFormid=" + IPDApplicationFormid +
							"&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));

						var receivableRows = Newtonsoft.Json.Linq.JArray.Parse(string.IsNullOrWhiteSpace(receivablesJson) ? "[]" : receivablesJson);
						var paymentRows = Newtonsoft.Json.Linq.JArray.Parse(string.IsNullOrWhiteSpace(paymentsJson) ? "[]" : paymentsJson);

						decimal statementTotal = 0m;
						decimal statementPaidFromReceivables = 0m;
						foreach (var receivableRow in receivableRows)
						{
							statementTotal += ParseBillingDecimal(receivableRow["amount"]?.ToString());
							statementPaidFromReceivables += ParseBillingDecimal(receivableRow["paidamount"]?.ToString());
						}

						decimal statementPayments = 0m;
						foreach (var paymentRow in paymentRows)
						{
							statementPayments += ParseBillingDecimal(paymentRow["amount"]?.ToString());
						}

						var statementReceived = statementPayments > 0 ? statementPayments : statementPaidFromReceivables;
						ViewBag.StatementExtraPaid = statementReceived > statementTotal ? statementReceived - statementTotal : 0m;
					}
					catch (Exception statementEx)
					{
						_logger.LogError(statementEx, "Billing_Dashboard statement extra paid calculation failed: " + statementEx.Message);
						ViewBag.StatementExtraPaid = 0m;
					}

					ViewBag.BillingSummary = summaryModel;
					ViewBag.IPDApplicationFormid = IPDApplicationFormid;
					ViewBag.OpenCancel = opencancel;
					ViewBag.TriggerRefund = triggerrefund;
					return View(billing);
				}
				TempData["errMessage"] = "IPD record not found";
				return isHealthSeeker
					? RedirectToAction("Index", "PatientDashboard", new { isDashboardView = "Y" })
					: RedirectToAction("Approved_IPD_Application_Forms");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Billing_Dashboard error: " + ex.Message);
				TempData["errMessage"] = "Error loading billing dashboard";
				return isHealthSeeker
					? RedirectToAction("Index", "PatientDashboard", new { isDashboardView = "Y" })
					: RedirectToAction("Approved_IPD_Application_Forms");
			}
		}





// -- STEP 9: IPD Payment Collection – list all payments for an IPD, show room cost summary, receive balance
		private static decimal ParseBillingDecimal(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
				return 0m;

			var cleanedValue = new string(value.Where(ch => char.IsDigit(ch) || ch == '.' || ch == '-').ToArray());
			return decimal.TryParse(cleanedValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedValue)
				? parsedValue
				: 0m;
		}

	[HttpGet()]
	public virtual async Task<IActionResult> IPD_Payment_Collection(string IPDApplicationFormid)
	{
		if (string.IsNullOrEmpty(IPDApplicationFormid))
		{
			TempData["errMessage"] = "Invalid request";
			return RedirectToAction("Approved_IPD_Application_Forms");
		}
		try
		{
				var client = getHttpClient();
				// 1. Billing summary (room cost, totals, remaining balance)
				var summaryJson = await ApiClient.Get_ApiValues(client,
				"api/IPDApplicationForm/Get_IPD_Billing_Summary?IPDApplicationFormid=" + IPDApplicationFormid +
				"&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));

			if (summaryJson.Length <= 2)
			{
				TempData["errMessage"] = "IPD record not found";
				return RedirectToAction("Approved_IPD_Application_Forms");
			}

			var summaryModel = JsonConvert.DeserializeObject<NalamVazha.Models.IPDBillingSummaryModel>(summaryJson);

			// 2. Build BillingPaymentModel for the "receive payment" form
			decimal amountToReceive = summaryModel.remainingbalance > 0 ? summaryModel.remainingbalance : 0;
			var billing = new NalamVazha.Models.BillingPaymentModel
			{
				amount = amountToReceive,
				receivedamount = amountToReceive,
				craftmyapp_actionmethodname = "Add_Billing_Payment",
				isdeleted = false
			};

			if (Guid.TryParse(IPDApplicationFormid, out var ipdGuid))
				billing.ipdnumber = ipdGuid;
			if (!string.IsNullOrEmpty(summaryModel.patientvisitid) && Guid.TryParse(summaryModel.patientvisitid, out var pvisit) && pvisit != Guid.Empty)
				billing.patientvisit = pvisit;

			// 3. Fetch IPD payment details for tenantid, patientname, room details
			try
			{
				var ipdJson = await ApiClient.Get_ApiValues(client,
					"api/IPDApplicationForm/Get_IPD_Payment_Details?IPDApplicationFormid=" + IPDApplicationFormid +
					"&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
				if (ipdJson.Length > 2)
				{
					var ipdPayment = JsonConvert.DeserializeObject<NalamVazha.Models.IPDPaymentDetailsModel>(ipdJson);
					if (ipdPayment != null)
					{
						if (Guid.TryParse(ipdPayment.tenantid, out var tid))
							billing.tenantid = tid;
						if (!string.IsNullOrEmpty(ipdPayment.patientname) && Guid.TryParse(ipdPayment.patientname, out var pname))
							billing.patientname = pname;
						if (!string.IsNullOrEmpty(ipdPayment.IPDApplicationFormid) && Guid.TryParse(ipdPayment.IPDApplicationFormid, out var ipdnum))
							billing.ipdnumber = ipdnum;
					}
				}
			}
			catch { /* non-critical */ }

			ViewBag.BillingSummary = summaryModel;
			ViewBag.IPDApplicationFormid = IPDApplicationFormid;
			return View(billing);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "IPD_Payment_Collection error: " + ex.Message);
			TempData["errMessage"] = "Error loading payment collection page";
			return RedirectToAction("Approved_IPD_Application_Forms");
		}
	}


// -- Patient Confirm Arrival Page (anonymous - linked from email)
	/// <summary>
	/// Patient accesses this page from the arrival confirmation email link.
	/// On GET: shows a confirmation page. On POST: calls Confirm_Patient_Arrival API.
	/// </summary>
	[AllowAnonymous]
	[HttpGet()]
	public virtual IActionResult Confirm_Patient_Arrival(string IPDApplicationFormid)
	{
		ViewBag.IPDApplicationFormid = IPDApplicationFormid;
		return View();
	}

	[AllowAnonymous]
	[HttpPost()]
	public virtual async Task<IActionResult> Confirm_Patient_Arrival_Post(string IPDApplicationFormid, string estimatedarrival = null, string specialrequest = null)
	{
		try
		{
			var requestBody = new NalamVazha.Models.IPDPatientArrivalConfirmModel { IPDApplicationFormid = IPDApplicationFormid, estimatedarrival = estimatedarrival, specialrequest = specialrequest };
			var json = await ApiClient.Post_ApiValuesGetString(getAnonymousHttpClient(), "api/IPDApplicationForm/Confirm_Patient_Arrival", requestBody);
			if (json.Replace("\"","").Contains("201.1"))
			{
				TempData["message"] = "Your arrival has been confirmed. We look forward to welcoming you.";
				return RedirectToAction("Confirm_Patient_Arrival", new { IPDApplicationFormid = IPDApplicationFormid });
			}
			TempData["errMessage"] = json.Replace("\"","");
			return RedirectToAction("Confirm_Patient_Arrival", new { IPDApplicationFormid = IPDApplicationFormid });
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Confirm_Patient_Arrival_Post error: " + ex.Message);
			TempData["errMessage"] = "Error confirming arrival. Please try again.";
			return RedirectToAction("Confirm_Patient_Arrival", new { IPDApplicationFormid = IPDApplicationFormid });
		}
	}
        [HttpGet]
        public async Task<IActionResult> validate_GroupCode(string tenantid, string groupcode)
        {
            try
            {
                var json = await ApiClient.Get_ApiValues(getHttpClient(),
                    "api/IPDApplicationForm/validate_GroupCode?tenantid=" + tenantid + "&groupcode=" + groupcode);
                var result = JsonConvert.DeserializeObject<bool>(json);
                return Json(new { isValid = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "validate_GroupCode error: " + ex.Message);
                return Json(new { isValid = false });
            }
        }
        [HttpPost()]
        public virtual async Task<string>
    Process_IPD_Room_Calendar_Action(
        [FromBody]
        IPDCalendarActionModel model
    )
        {
            string strReturnMessage = "";

            try
            {
                var loginUserID =
                    HttpContext.Session.GetString(
                        "NalamVazhaloginUserID"
                    );

                if (
                    string.IsNullOrWhiteSpace(
                        loginUserID
                    )
                )
                {
                    return "Session Expired";
                }

                if (
                    model == null ||
                    model.items == null ||
                    model.items.Count == 0
                )
                {
                    return
                        "At least one IPD application is required.";
                }

                model.modifieduser =
                    new Guid(loginUserID);

                if (
                    !model.tenantid.HasValue ||
                    model.tenantid == Guid.Empty
                )
                {
                    var selectedTenantID =
                        HttpContext.Session.GetString(
                            "NalamVazhachoosedtenantid"
                        )
                        ??
                        HttpContext.Session.GetString(
                            "NalamVazhatenantid"
                        );

                    if (
                        Guid.TryParse(
                            selectedTenantID,
                            out var tenantGuid
                        )
                    )
                    {
                        model.tenantid =
                            tenantGuid;
                    }
                }

                strReturnMessage =
                    await ApiClient
                        .Post_ApiValuesGetString(
                            getHttpClient(),

                            "api/IPDApplicationForm/" +
                            "Process_IPD_Room_Calendar_Action",

                            model
                        );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Process_IPD_Room_Calendar_Action failed"
                );

                strReturnMessage =
                    ex.Message;
            }

            var cleanMessage =
                (strReturnMessage ?? "")
                    .Replace("\"", "")
                    .Trim();

            if (
                cleanMessage ==
                "201.1"
            )
            {
                return "Success";
            }

            if (
                strReturnMessage != null &&
                strReturnMessage.StartsWith(
                    "BadRequest",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                return strReturnMessage
                    .Replace("\"", "")
                    .Replace(
                        "BadRequest :",
                        ""
                    );
            }

            if (
                cleanMessage ==
                "401.1"
            )
            {
                return
                    "Authorization Failed";
            }

            return cleanMessage;
        }


        [HttpGet()]
        public virtual async Task<string> lookup_IPDApplicationForm_packagename_by_roomtype(
    string tenantid,
    string roomtypeid)
        {
            return await ApiClient.Get_ApiValues(
                getHttpClient(),
                "api/IPDApplicationForm/lookup_IPDApplicationForm_packagename_by_roomtype?tenantid="
                + tenantid
                + "&roomtypeid="
                + roomtypeid
                + "&loginUserID="
                + HttpContext.Session.GetString("NalamVazhaloginUserID")
            );
        }
    }

	public class IPDOverpaidRefundRequestModel
	{
		public string IPDApplicationFormid { get; set; }
		public string refundmode { get; set; }
		public decimal refundamount { get; set; }
		public string refundreason { get; set; }
	}


			}
