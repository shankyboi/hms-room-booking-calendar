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
	using Newtonsoft.Json.Linq;




	//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:54





	public class OPDFormController : BaseController
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
		private readonly ILogger<OPDFormController> _logger;


		StorageUtil util;
					public OPDFormController(IConfiguration configuration,IHttpContextAccessor accessor,IOptions<ApiSettings> ApiSettings, IOptions<MailSettings> MailSettings, IWebHostEnvironment env, ILogger<OPDFormController> logger):base( configuration)
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

        private static bool RequiresOPDReviewReason(string status)
        {
            var normalized = (status ?? string.Empty).Trim().ToUpperInvariant();
            return normalized == "REWORK" ||
                   normalized == "OPD REWORK" ||
                   normalized == "REJECT" ||
                   normalized == "REJECTED" ||
                   normalized == "OPD REJECT" ||
                   normalized == "OPD REJECTED";
        }


		public virtual IActionResult audit()
		{
			return View();
		}




		public virtual async Task<string> getById_medicalinfo(string OPDFormid)
		{
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/OPDForm/getById_medicalinfo?OPDFormid="+OPDFormid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));

		}



		public virtual async Task<string> getById_medicationinfo(string OPDFormid)
		{
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/OPDForm/getById_medicationinfo?OPDFormid="+OPDFormid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));

		}



		public virtual async Task<string> getById_medicalrecords(string OPDFormid)
		{
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/OPDForm/getById_medicalrecords?OPDFormid="+OPDFormid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));

		}

		public virtual async Task<string> getById_appointmentpreferences(string OPDFormid)
		{
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/OPDForm/getById_appointmentpreferences?OPDFormid="+OPDFormid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));

		}


		public virtual IActionResult Add_OPD_Form()
		{
			return View();
		}

		[HttpGet]
		public virtual async Task<string> Get_Booking_Emergency_Contacts(string OPDFormid)
		{
			return await ApiClient.Get_ApiValues(getHttpClient(), "api/OPDForm/Get_Booking_Emergency_Contacts?OPDFormid=" + OPDFormid);
		}
		[HttpPost()]
		public virtual async Task<string> Add_OPD_Form(OPDFormModel model, IFormCollection collection)
		{
			string strReturnMessage = "";

			try
			{
				ModelState.Remove("OPDFormid");
				ModelState.Remove("createduser");
				ModelState.Remove("craftmyapp_actionmethodname");
                    model.craftmyapp_actionmethodname="Add_OPD_Form";
					if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
								model.createduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
				else
					return "Session Expired";

				var isFrontDesk = string.Equals(HttpContext.Session.GetString("NalamVazhauserrole"), "Frontdesk Admin", StringComparison.OrdinalIgnoreCase)
					|| string.Equals(HttpContext.Session.GetString("NalamVazhauserrole"), "Front Desk Admin", StringComparison.OrdinalIgnoreCase);
				if (isFrontDesk && (!model.preferreddoctor.HasValue
					|| model.preferreddoctor.Value == Guid.Empty
					|| !Guid.TryParse(collection["hf_slot_practitioner"].ToString(), out _)
					|| string.IsNullOrWhiteSpace(collection["hf_slot_date"].ToString())
					|| string.IsNullOrWhiteSpace(collection["hf_slot_from"].ToString())
					|| string.IsNullOrWhiteSpace(collection["hf_slot_to"].ToString())))
					return "Preferred doctor and time slot are required for a Front Desk OPD booking.";




				if (ModelState.IsValid)
				{
					OPDFormModelValidator validator = new OPDFormModelValidator();
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
								 model.OPDFormid =Guid.NewGuid();

						var files = Request.Form.Files;
						foreach (var file in files)
						{
							var filename = ContentDispositionHeaderValue
							.Parse(file.ContentDisposition)
							.FileName
							.Trim('"');
							string fileExtention = "." + filename.Split('.').Last();
							Random rnd = new Random();
string uploadFileName = System.Text.RegularExpressions.Regex.Replace(filename.Split('.').First(), @"[^0-9a-zA-Z_.]+", "").Replace(" ", String.Empty)+"_"+"OPDForm_" +rnd.Next(1, 10000).ToString() + DateTime.Now.ToString("ddMMyyHHmmss")+ fileExtention;
							if (fileExtention != ".")
							{
								Stream stream = file.OpenReadStream();
string fileURL=await util.fileSystem.UploadFileAsync(stream, Configuration.GetSection("AzureBlobStorageSetttings:folderName").Value, uploadFileName);
								string row_id_medicalrecords_medicalrecordfile = file.Name.Replace("medicalrecords_medicalrecordfile_cma_uploader", "");
if (file.Name == "medicalrecords_medicalrecordfile_cma_uploader"+row_id_medicalrecords_medicalrecordfile)
								{
									var dependent_model = (from x in model.medicalrecords.OfType<OPDForm_medicalrecordsModel>() where x.cma_client_row_id == row_id_medicalrecords_medicalrecordfile select x).FirstOrDefault();
									dependent_model.medicalrecordfile += "|" + uploadFileName + "|";
								}
							}
						}


                                 strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/OPDForm/Add_OPD_Form", model);


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

                 _logger.LogError(ex,"An exception occurred in - OPDForm / Add_OPD_Form, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));

				strReturnMessage = ex.Message;
			}
			ViewData["message"] = strReturnMessage;
			 if(strReturnMessage.Replace("\"", "").Contains("201.1")){
				TempData["message"] = "Success";
				MailSender maillog = new MailSender();
				bool mailsent = await maillog.sendNotification("OPDForm"
				, "ReadyForReview"
				, model.OPDFormid.ToString()
				, _mailSettings
				, model.createduser.ToString()
				, client
				, tenantid: model.tenantid?.ToString() ?? "");

				try
				{
					var loginUserId = HttpContext.Session.GetString("NalamVazhaloginUserID") ?? "";
					var practitionerText = collection["hf_slot_practitioner"].ToString();
					var appointmentDateText = collection["hf_slot_date"].ToString();
					var durationFrom = collection["hf_slot_from"].ToString();
					var durationTo = collection["hf_slot_to"].ToString();

					var dateParsed = DateTime.TryParseExact(appointmentDateText,
						new[] { "dd/MM/yyyy", "d/M/yyyy", "dd-MM-yyyy", "d-M-yyyy" },
						CultureInfo.InvariantCulture, DateTimeStyles.None, out var appointmentDate);

					if (Guid.TryParse(model.task?.ToString(), out var taskGuid)
						&& Guid.TryParse(practitionerText, out var practitionerGuid)
						&& dateParsed
						&& !string.IsNullOrWhiteSpace(durationFrom)
						&& !string.IsNullOrWhiteSpace(durationTo))
					{
						var tenantName = HttpContext.Session.GetString("NalamVazhachoosedtenantname") ?? "";
						var inviteSent = await SendOPDOnlineMeetingInviteForOnlineTask(
							client,
							loginUserId,
							taskGuid.ToString(),
							model.patientname,
							practitionerGuid,
							appointmentDate,
							durationFrom,
							durationTo,
							model.OPDFormid.ToString(),
							tenantName,
							"Add_OPD_Form");

						if (inviteSent)
							HttpContext.Session.SetString(GetOPDOnlineInviteSessionKey(model.OPDFormid.ToString()), "1");
					}
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Add_OPD_Form - online meeting invite check failed: " + ex.Message);
				}

				return "Success|" + model.OPDFormid.ToString();
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

		/// <summary>
		/// Creates a ClinicalAppointment after a Health Seeker saves an OPD form and selects a slot.
		/// Resolves the OPD task type from the doctor's People_clinicaltaskinfo — same approach as IPD.
		/// </summary>
		[HttpPost()]
		public virtual async Task<string> Create_OPD_Clinical_Appointment(
			string tenantid,
			Guid patient,
			Guid practitioner,
			string appointmentdate,
			string durationfrom,
			string durationto,
			string status,
			string opdformid = "",
			string taskid = ""
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
				//if (practitioner == Guid.Empty) return "Doctor is required";

				if (!DateTime.TryParseExact(appointmentdate,
						new[] { "dd/MM/yyyy", "d/M/yyyy", "dd/MM/yyyy HH:mm", "d/M/yyyy HH:mm", "dd-MM-yyyy", "d-M-yyyy", "yyyy-MM-dd", "yyyy-MM-dd HH:mm" },
						CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime apptDate))
					return "Invalid appointment date";

				if (string.IsNullOrWhiteSpace(durationfrom) || string.IsNullOrWhiteSpace(durationto))
					return "Slot time is required";

				if (!Guid.TryParse(taskid, out Guid selectedTaskId) || selectedTaskId == Guid.Empty)
					return "Please choose Booking For.";

				var httpClient = getHttpClient();
				var eligibilityJson = await ApiClient.Get_ApiValues(
					httpClient,
					"api/OPDForm/Validate_OPD_Task_Eligibility?patientname=" + patient
					+ "&task=" + selectedTaskId
					+ "&excludeopdformid=" + Uri.EscapeDataString(opdformid ?? "")
					+ "&loginUserID=" + loginUserId);
				if (string.IsNullOrWhiteSpace(eligibilityJson))
					return "Unable to validate the OPD consultation task.";

				var eligibilityResult = eligibilityJson.Trim();
				if (eligibilityResult.StartsWith("\"") && eligibilityResult.EndsWith("\""))
				{
					try { eligibilityResult = JsonConvert.DeserializeObject<string>(eligibilityResult) ?? eligibilityResult.Trim('"'); }
					catch { eligibilityResult = eligibilityResult.Trim('"'); }
				}
				if (!string.Equals(eligibilityResult, "201.1", StringComparison.Ordinal))
					return eligibilityResult;

				// ── STEP 1: Get doctor's clinical task info ───────────────────────────
				var clinicalTaskInfoJson = await ApiClient.Get_ApiValues(
					httpClient,
					$"api/People/getById_clinicaltaskinfo?Peopleid={practitioner}&loginUserID={loginUserId}");

				if (string.IsNullOrWhiteSpace(clinicalTaskInfoJson))
					return "No clinical task info found for the selected doctor";

				clinicalTaskInfoJson = clinicalTaskInfoJson.Trim();
				if (clinicalTaskInfoJson.StartsWith("\"") && clinicalTaskInfoJson.EndsWith("\""))
					clinicalTaskInfoJson = clinicalTaskInfoJson.Trim('"');

				JArray clinicalTaskInfo;
				try { clinicalTaskInfo = JArray.Parse(clinicalTaskInfoJson); }
				catch { return "Unable to parse clinical task info"; }

                // ── STEP 2: Resolve OPD task type NAME from clinicaltaskinfo ────────────
                // Mirrors Create_IPD_Clinical_Appointment exactly:
                //   • Look up each Task by taskname GUID — filter to OP* tasks
                //   • Look up TaskType by tasktype GUID — extract tasktypename
                //   • Pass the name string to the appointment model (not the GUID)
                var taskCache = new Dictionary<string, TaskModel>(StringComparer.OrdinalIgnoreCase);
                var taskTypeCache = new Dictionary<Guid, string>();

                string resolvedTaskTypeName = null;
                string resolvedTaskName = null;

                foreach (var row in clinicalTaskInfo)
				{
					var taskId = row["taskname"]?.ToString();
					if (!Guid.TryParse(taskId, out var configuredTaskId) || configuredTaskId != selectedTaskId)
						continue;

					// ── Fetch Task ────────────────────────────────────────────────────
					if (!taskCache.TryGetValue(taskId, out var task))
					{
						var taskJson = await ApiClient.Get_ApiValues(
							httpClient,
							$"api/Task/getById_Task?Taskid={taskId}&loginUserID={loginUserId}");

						if (string.IsNullOrWhiteSpace(taskJson)) continue;

						task = JsonConvert.DeserializeObject<TaskModel>(taskJson);
						if (task == null) continue;
						taskCache[taskId] = task;
					}

					// Filter to OPD tasks only
					var normalizedTaskName = new string((task.taskname ?? "")
						.Where(char.IsLetterOrDigit)
						.Select(char.ToLowerInvariant)
						.ToArray());
					if (normalizedTaskName != "opnew"
						&& normalizedTaskName != "opfollowup"
						&& normalizedTaskName != "onlineopnew"
						&& normalizedTaskName != "onlineopfollowup")
						continue;

					// Tenant check
					if (task.tenantid != tenantGuid) continue;
                    resolvedTaskName = task.taskname?.Trim();
                    var taskTypeId = task.tasktype;
					if (taskTypeId == Guid.Empty) continue;

					// ── Fetch TaskType — get the name string ─────────────────────────
					if (!taskTypeCache.TryGetValue(taskTypeId, out var taskTypeName))
					{
						var ttJson = await ApiClient.Get_ApiValues(
							httpClient,
							$"api/TaskType/getById_TaskType?TaskTypeid={taskTypeId}&loginUserID={loginUserId}");

						if (string.IsNullOrWhiteSpace(ttJson)) continue;

						var tt = JsonConvert.DeserializeObject<TaskTypeModel>(ttJson);
						if (tt == null || string.IsNullOrWhiteSpace(tt.tasktypename)) continue;

						taskTypeName = tt.tasktypename;
						taskTypeCache[taskTypeId] = taskTypeName;
					}

					// Resolve the task type for the exact task selected on the OPD form.
					resolvedTaskTypeName = taskTypeName;
					break;
				}
                if (string.IsNullOrWhiteSpace(resolvedTaskName))
                    return "Unable to resolve the selected OPD task name.";
                if (string.IsNullOrWhiteSpace(resolvedTaskTypeName))
					return "The selected OPD task is not configured for the selected doctor.";

				var holidayDate = apptDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
				var holidayJson = await ApiClient.Get_ApiValues(
					httpClient,
					"api/HolidayCalendar/Get_Holiday_Blocked_Dates?tenantid=" + tenantGuid
					+ "&taskid=" + Uri.EscapeDataString(taskid ?? "")
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
							var bookedCount = (firstBlocked["bookedcount"] ?? firstBlocked["BookedCount"])?.ToString();
							var countMessage = string.IsNullOrWhiteSpace(allowedCount)
								? ""
								: $" Allowed count: {bookedCount ?? "0"}/{allowedCount}.";
							return apptDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
								+ " is a Holiday"
								+ (string.IsNullOrWhiteSpace(holidayName) ? "" : " (" + holidayName + ")")
								+ ". " + resolvedTaskTypeName + " is not available on this day."
								+ countMessage;
						}
					}
					catch
					{
						// If the holiday lookup response is not JSON, continue with normal appointment validation.
					}
				}

				// ── STEP 3: Create the appointment ───────────────────────────────────
				var model = new ClinicalAppointmentModel
				{
					craftmyapp_actionmethodname = "Add_Clinical_Appointment",
					tenantid = tenantGuid,
					tasktype = resolvedTaskTypeName, // FINAL VALUE — name string, not GUID
                    taskname = selectedTaskId.ToString(),
                    patient = patient,
					practitioner = practitioner,
					actualpractitioner = practitioner,
					appointmentdate = apptDate,
					durationfrom = durationfrom,
					durationto = durationto,
					status = status,
					origin = "OPD",
					bookingid = string.IsNullOrWhiteSpace(opdformid) ? null : opdformid,
				};

				var result = await ApiClient.Post_ApiValuesGetString(
					httpClient,
					"api/ClinicalAppointment/Add_Clinical_Appointment",
					model);

				if (result != null && result.Replace("\"", "").Contains("201.1"))
				{
					/* Send screening invite only for Scheduled (front desk direct add OR reviewer-assigned slot).
					   Health seeker Requested appointments do not trigger the invite here. */
					if (string.Equals(status, "Scheduled", StringComparison.OrdinalIgnoreCase)
						&& !string.IsNullOrWhiteSpace(taskid))
					{
						var inviteSessionKey = GetOPDOnlineInviteSessionKey(opdformid);
						var inviteAlreadySent = !string.IsNullOrWhiteSpace(opdformid)
							&& string.Equals(HttpContext.Session.GetString(inviteSessionKey), "1", StringComparison.Ordinal);
						if (inviteAlreadySent)
						{
							HttpContext.Session.Remove(inviteSessionKey);
						}
						else
						{
						try
						{
							var taskJson = await ApiClient.Get_ApiValues(httpClient,
								$"api/Task/getById_Task?Taskid={taskid}&loginUserID={loginUserId}");
							if (!string.IsNullOrWhiteSpace(taskJson))
							{
								var taskObj = JsonConvert.DeserializeObject<TaskModel>(taskJson);
								var taskName = taskObj?.taskname ?? "";
								bool isOnlineOP =
									taskName.IndexOf("Online OP New", StringComparison.OrdinalIgnoreCase) >= 0
									|| taskName.IndexOf("Online OP Follow up", StringComparison.OrdinalIgnoreCase) >= 0;
								if (isOnlineOP)
								{
									_logger?.LogInformation("Online OPD {OPDFormid} scheduled; doctor email deferred until payment success", opdformid);
								}
							}
						}
						catch (Exception ex)
						{
							_logger?.LogError(ex, "Create_OPD_Clinical_Appointment – online meeting invite check failed: " + ex.Message);
						}
						}
					}
					return "Success";
				}

				return result ?? "Failed to create clinical appointment";
			}
			catch (Exception ex)
			{
				_logger?.LogError(ex, "Create_OPD_Clinical_Appointment failed: " + ex.Message);
				return ex.Message;
			}
		}


		public virtual async Task<IActionResult> Update_OPD_Form(string OPDFormid)
		{

                    string redirectTo="";
                    if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
                            DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
				DataView dv = new DataView(NalamVazharole_JSON);
				dv.RowFilter = "controllername='OPDForm' AND viewname='list'";

                            if(dv.Count  >0){
					redirectTo = dv[0]["actionmethodname"] as string;

				}

                            try{
                                     var jsonObjOPDForm = await ApiClient.Get_ApiValues(getHttpClient(), "api/OPDForm/getById_OPDForm?OPDFormid="+OPDFormid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                                if(jsonObjOPDForm.Length > 2)
				{

						var model = JsonConvert.DeserializeObject<OPDFormModel>(jsonObjOPDForm);

						var jsonObjAppointment = await ApiClient.Get_ApiValues(getHttpClient(),
							"api/OPDForm/Get_Appointment_By_OPDForm?OPDFormid=" + OPDFormid +
							"&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));

						if (!string.IsNullOrWhiteSpace(jsonObjAppointment) && jsonObjAppointment.Length > 2)
						{
							try
							{
								var appointmentRows = JArray.Parse(jsonObjAppointment);
								var appointment = appointmentRows.FirstOrDefault() as JObject;
								if (appointment != null)
								{
									ViewBag.ExistingOPDSlotFrom = appointment["durationfrom"]?.ToString()
										?? appointment["DurationFrom"]?.ToString();
									ViewBag.ExistingOPDSlotTo = appointment["durationto"]?.ToString()
										?? appointment["DurationTo"]?.ToString();
									ViewBag.ExistingOPDSlotDate = appointment["appointmentdate"]?.ToString()
										?? appointment["AppointmentDate"]?.ToString();
									ViewBag.ExistingOPDSlotPractitioner = appointment["practitioner"]?.ToString()
										?? appointment["Practitioner"]?.ToString();
									ViewBag.ExistingOPDSlotPractitionerName = appointment["practitionername"]?.ToString()
										?? appointment["PractitionerName"]?.ToString();
									ViewBag.ExistingOPDClinicalAppointmentId = appointment["clinicalappointmentid"]?.ToString()
										?? appointment["ClinicalAppointmentid"]?.ToString()
										?? appointment["ClinicalAppointmentId"]?.ToString();
								}
							}
							catch (Exception ex)
							{
								_logger.LogError(ex, "Update_OPD_Form - failed to read existing OPD appointment slot: " + ex.Message);
							}
						}


						return View("Add_OPD_Form", model);
					}
					else
					{

						TempData["message"] = "Data Not Found - Contact Administrator";
						return RedirectToAction(redirectTo);

					}

                            }catch(Exception ex){
                               _logger.LogError(ex,"An exception occurred in - OPDForm / Update_OPD_Form, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));

					TempData["errMessage"] = "Error while fetching data - Contact Administrator";
					return RedirectToAction(redirectTo);
				}

			}
			TempData["errMessage"] = "Session Expired";
			return RedirectToAction("Logout", "users");
		}
		[HttpPost()]
		public virtual async Task<string> Update_OPD_Form(OPDFormModel model, IFormCollection collection)
		{
			string strReturnMessage = "";
			try
			{
				ModelState.Remove("OPDFormid");
				ModelState.Remove("craftmyapp_actionmethodname");
                             model.craftmyapp_actionmethodname="Update_OPD_Form";


							if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
					model.modifieduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
				else
					return "Session Expired";



				if (ModelState.IsValid)
				{
					OPDFormModelValidator validator = new OPDFormModelValidator();
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
string uploadFileName = System.Text.RegularExpressions.Regex.Replace(filename.Split('.').First(), @"[^0-9a-zA-Z_.]+", "").Replace(" ", String.Empty)+"_"+"OPDForm_" +rnd.Next(1, 10000).ToString() + DateTime.Now.ToString("ddMMyyHHmmss")+ fileExtention;
							if (fileExtention != ".")
							{
								Stream stream = file.OpenReadStream();
string fileURL=await util.fileSystem.UploadFileAsync(stream, Configuration.GetSection("AzureBlobStorageSetttings:folderName").Value, uploadFileName);
								string row_id_medicalrecords_medicalrecordfile = file.Name.Replace("medicalrecords_medicalrecordfile_cma_uploader", "");
if (file.Name == "medicalrecords_medicalrecordfile_cma_uploader"+row_id_medicalrecords_medicalrecordfile)
								{
									var dependent_model = (from x in model.medicalrecords.OfType<OPDForm_medicalrecordsModel>() where x.cma_client_row_id == row_id_medicalrecords_medicalrecordfile select x).FirstOrDefault();
									dependent_model.medicalrecordfile += "|" + uploadFileName + "|";
								}
							}
						}



                                        strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/OPDForm/Update_OPD_Form", model);
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
                      _logger.LogError(ex,"An exception occurred in - OPDForm / Update_OPD_Form, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));

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
		public virtual async Task<IActionResult> Remove_OPD_Form(string OPDFormid)
		{
			var sessionRole = HttpContext.Session.GetString("NalamVazhauserrole") ?? "";
			if (sessionRole.Equals("Health Seeker", StringComparison.OrdinalIgnoreCase))
			{
				TempData["errMessage"] = "OPD form removal is not allowed from this URL.";
				return RedirectToAction("Index", "PatientDashboard");
			}

			string message = "";
			try
			{
						message = await ApiClient.Get_ApiValues(getHttpClient(), "api/OPDForm/Remove_OPD_Form?OPDFormid="+OPDFormid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
						 if(message.Replace("\"","").Contains("201.1"))
				{
					TempData["message"] = "Success";

						}else{
							TempData["errMessage"] = message.Replace("\"","");
				}



			}
			catch (Exception ex)
			{
                     _logger.LogError(ex,"An exception occurred in - OPDForm / Remove_OPD_Form, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));


				TempData["errMessage"] = ex.Message;
				message = ex.Message;
			}

				string redirectTo="";
						if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
					DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
				DataView dv = new DataView(NalamVazharole_JSON);
				dv.RowFilter = "controllername='OPDForm' AND viewname='list'";

						if(dv.Count  >0){
					redirectTo = dv[0]["actionmethodname"] as string;

				}

			}

			return RedirectToAction(redirectTo);
		}

		public virtual IActionResult Added_OPD_Form()
		{
			var sessionRole = HttpContext.Session.GetString("NalamVazhauserrole") ?? "";
			if (sessionRole.Equals("Health Seeker", StringComparison.OrdinalIgnoreCase))
			{
				TempData["errMessage"] = "Please use the patient dashboard to view your OPD forms.";
				return RedirectToAction("Index", "PatientDashboard");
			}

			return View();
		}

		[HttpGet()]
		public virtual async Task<string> get_Added_OPD_Form(string tenantid
,string patientname
, string verifiedstatus
, int? pagesize=100 , int? pagenumber=0,string searchterm="",string sortFieldsJson="",
    string createddate_automatonfrom = "",
    string createddate_automatonto = "",
    string bookingnumber = "", string workflowstatus = "", string financialstatus = "", string paymentmethod = "")
		{
			var sessionRole = HttpContext.Session.GetString("NalamVazhauserrole") ?? "";
			if (sessionRole.Equals("Health Seeker", StringComparison.OrdinalIgnoreCase))
				return "[]";

				        return await ApiClient.Get_ApiValues(getHttpClient(), "api/OPDForm/Added_OPD_Form?tenantid="+tenantid+"&patientname="+patientname+"&verifiedstatus="+verifiedstatus+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID")
+ "&pagesize=" + pagesize + "&pagenumber="+ pagenumber + "&searchterm="+ searchterm + "&sort_fields=" + sortFieldsJson + "&createddate_automatonfrom=" + createddate_automatonfrom + "&createddate_automatonto=" + createddate_automatonto
+ "&bookingnumber=" + System.Net.WebUtility.UrlEncode(bookingnumber) + "&workflowstatus=" + System.Net.WebUtility.UrlEncode(workflowstatus) + "&financialstatus=" + System.Net.WebUtility.UrlEncode(financialstatus) + "&paymentmethod=" + System.Net.WebUtility.UrlEncode(paymentmethod));
		}




		[HttpGet()]
											public virtual async Task<string> get_all_PatientProfile(string tenantid,string searchterm, string pagesize="1000", string pagenumber="1")
		{

											return await ApiClient.Get_ApiValues(getHttpClient(), "api/PatientProfile/get_all_PatientProfile?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
		}
		[HttpGet()]
		public virtual async Task<string> get_all_Task(string tenantid)
		{

			return await ApiClient.Get_ApiValues(getHttpClient(), "api/Task/get_all_Task?tenantid=" + tenantid + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
		}


		[HttpGet()]
											public virtual async Task<string> get_all_People(string tenantid,string searchterm, string pagesize="1000", string pagenumber="1")
		{

											return await ApiClient.Get_ApiValues(getHttpClient(), "api/People/get_all_People?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
		}


		public virtual IActionResult View_OPD_Form()
		{
			return View();
		}

		public virtual IActionResult OPD_Forms_for_Review()
		{
			return View();
		}

		[HttpGet()]
		public virtual async Task<string> get_OPD_Forms_for_Review(string tenantid
,string patientname
, string verifiedstatus
, int? pagesize=100 , int? pagenumber=0,string searchterm="",string sortFieldsJson="")
		{

				        return await ApiClient.Get_ApiValues(getHttpClient(), "api/OPDForm/OPD_Forms_for_Review?tenantid="+tenantid+"&patientname="+patientname+"&verifiedstatus="+verifiedstatus+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID")
+ "&pagesize=" + pagesize + "&pagenumber="+ pagenumber + "&searchterm="+ searchterm + "&sort_fields=" + sortFieldsJson);
		}

		[HttpGet()]
		public virtual async Task<string> Get_Latest_Previous_OPD_Prefill_By_Patient(string tenantid, string patientname)
		{
			return await ApiClient.Get_ApiValues(getHttpClient(), "api/OPDForm/Get_Latest_Previous_OPD_Prefill_By_Patient?tenantid=" + tenantid + "&patientname=" + patientname + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
		}



		[HttpGet()]
		public virtual async Task<string> count_of_OPDForm(string tenantid
,string patientname
)
		{

				return await ApiClient.Get_ApiValues(getHttpClient(), "api/OPDForm/count_of_OPDForm?tenantid="+tenantid+"&patientname="+patientname+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID")
);
		}
        [HttpPost]
        public virtual async Task<string> verify_OPDForm(
    [FromBody] OPDFormReviewModel model)
        {
            string message = "";

            if (model == null)
                return "Invalid OPD review request.";

            if (RequiresOPDReviewReason(model.verifiedstatus) &&
                string.IsNullOrWhiteSpace(model.reviewcomments))
                return "Review comments are required for Rework or Reject.";

            try
            {
                message =
                    await ProcessSingleOPDReview(model);


                if (
                    message.Replace("\"", "") ==
                    "201.1"
                )
                {
                    TempData["message"] =
                        "Success";
                }
                else
                {
                    TempData["errMessage"] =
                        message.Replace("\"", "");
                }


                return message.Replace(
                    "\"",
                    ""
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An exception occurred in - OPDForm / verify_OPDForm: " +
                    ex.Message
                );

                TempData["errMessage"] =
                    ex.Message;

                return ex.Message;
            }
        }
        private async Task<string> ProcessSingleOPDReview(
    OPDFormReviewModel model)
        {
            string message = "";

            bool isOnlineAppointmentMode =
                (model.appointmentmode ?? "")
                    .IndexOf(
                        "online",
                        StringComparison.OrdinalIgnoreCase
                    ) >= 0;


            /*
             * STEP 1
             * Verify OPD
             */
            message =
                await ApiClient.Post_ApiValuesGetString(
                    getHttpClient(),

                    "api/OPDForm/verify_OPDForm",

                    model
                );


            message =
                message.Replace("\"", "");


            if (message != "201.1")
            {
                return message;
            }


            /*
             * Only Approved needs appointment processing.
             */
            bool isApproved =
                string.Equals(
                    model.verifiedstatus,
                    "Approved",
                    StringComparison.OrdinalIgnoreCase
                )
                ||
                string.Equals(
                    model.verifiedstatus,
                    "OPD Approved",
                    StringComparison.OrdinalIgnoreCase
                );


            if (
                !isApproved ||
                string.IsNullOrWhiteSpace(
                    model.OPDFormid
                )
            )
            {
                return "201.1";
            }


            /*
             * STEP 2
             * Online payment email
             */
            if (isOnlineAppointmentMode)
            {
                await SendOnlineOPDPaymentEmail(
                    model
                );
            }


            /*
             * STEP 3
             * Check if Health Seeker already created
             * Requested appointment.
             */

            var client =
                getHttpClient();


            var apptJson =
                await ApiClient.Get_ApiValues(
                    client,

                    "api/OPDForm/" +
                    "Get_Appointment_By_OPDForm" +
                    "?OPDFormid=" +
                    model.OPDFormid
                );


            Guid ivPatient =
                Guid.Empty;

            Guid ivPractitioner =
                Guid.Empty;

            DateTime ivApptDate =
                DateTime.MinValue;

            string ivDurationFrom =
                null;

            string ivDurationTo =
                null;

            bool ivInviteHandledByCreate =
                false;

            bool requestedApptFound =
                false;


            if (
                !string.IsNullOrWhiteSpace(
                    apptJson
                )
                &&
                apptJson.Length > 2
            )
            {
                try
                {
                    var apptRows =
                        JsonConvert.DeserializeObject<JArray>(
                            apptJson
                        );


                    var appt =
                        apptRows?[0];


                    var apptId =
                        appt?[
                            "clinicalappointmentid"
                        ]?.ToString();


                    if (
                        !string.IsNullOrWhiteSpace(
                            apptId
                        )
                    )
                    {
                        requestedApptFound =
                            true;


                        /*
                         * Requested → Scheduled
                         */
                        var statusModel =
                            new AppointmentStatusUpdateModel
                            {
                                ClinicalAppointmentid =
                                    new Guid(apptId),

                                status =
                                    "Scheduled"
                            };


                        await ApiClient
                            .Post_ApiValuesGetString(
                                client,

                                "api/OPDForm/" +
                                "Update_Appointment_Status",

                                statusModel
                            );


                        Guid.TryParse(
                            appt?["patient"]?.ToString(),
                            out ivPatient
                        );


                        Guid.TryParse(
                            appt?["practitioner"]?.ToString(),
                            out ivPractitioner
                        );


                        ivDurationFrom =
                            appt?[
                                "durationfrom"
                            ]?.ToString();


                        ivDurationTo =
                            appt?[
                                "durationto"
                            ]?.ToString();


                        var apptDateStr =
                            appt?[
                                "appointmentdate"
                            ]?.ToString();


                        if (
                            !string.IsNullOrWhiteSpace(
                                apptDateStr
                            )
                        )
                        {
                            DateTime.TryParse(
                                apptDateStr,
                                out ivApptDate
                            );
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,

                        "verify_OPDForm - failed to update " +
                        "Requested appointment: " +
                        ex.Message
                    );
                }
            }


            /*
             * STEP 4
             * No Requested appointment
             * → Clinical Slot / reviewer selected slot
             * → Create Scheduled ClinicalAppointment
             */

            if (
                !requestedApptFound

                &&
                !string.IsNullOrWhiteSpace(
                    model.practitioner
                )

                &&
                !string.IsNullOrWhiteSpace(
                    model.durationfrom
                )

                &&
                !string.IsNullOrWhiteSpace(
                    model.durationto
                )

                &&
                !string.IsNullOrWhiteSpace(
                    model.patient
                )
            )
            {
                try
                {
                    var createResult =
                        await Create_OPD_Clinical_Appointment(
                            tenantid:
                                model.tenantid ?? "",

                            patient:
                                new Guid(
                                    model.patient
                                ),

                            practitioner:
                                new Guid(
                                    model.practitioner
                                ),

                            appointmentdate:
                                model.appointmentdate ?? "",

                            durationfrom:
                                model.durationfrom,

                            durationto:
                                model.durationto,

                            status:
                                "Scheduled",

                            opdformid:
                                model.OPDFormid,

                            taskid:
                                model.task ?? ""
                        );


                    if (
                        !string.Equals(
                            createResult,
                            "Success",
                            StringComparison.OrdinalIgnoreCase
                        )
                    )
                    {
                        return createResult;
                    }


                    ivInviteHandledByCreate =
                        true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,

                        "verify_OPDForm - failed to create " +
                        "Scheduled appointment: " +
                        ex.Message
                    );

                    return ex.Message;
                }
            }


            /*
             * STEP 5
             * Existing Requested appointment + Online OP
             */
            if (
                !ivInviteHandledByCreate

                &&
                isOnlineAppointmentMode

                &&
                !string.IsNullOrWhiteSpace(
                    model.task
                )

                &&
                ivPractitioner !=
                    Guid.Empty
            )
            {
                try
                {
                    var loginUserId =
                        HttpContext.Session
                            .GetString(
                                "NalamVazhaloginUserID"
                            )
                        ?? "";


                    var taskJson =
                        await ApiClient.Get_ApiValues(
                            client,

                            $"api/Task/getById_Task" +
                            $"?Taskid={model.task}" +
                            $"&loginUserID={loginUserId}"
                        );


                    if (
                        !string.IsNullOrWhiteSpace(
                            taskJson
                        )
                    )
                    {
                        var taskObj =
                            JsonConvert
                                .DeserializeObject<TaskModel>(
                                    taskJson
                                );


                        var taskName =
                            taskObj?.taskname ?? "";


                        bool isOnlineOP =
                            taskName.IndexOf(
                                "Online OP New",
                                StringComparison.OrdinalIgnoreCase
                            ) >= 0

                            ||

                            taskName.IndexOf(
                                "Online OP Follow up",
                                StringComparison.OrdinalIgnoreCase
                            ) >= 0;


                        if (isOnlineOP)
                        {
                            _logger.LogInformation(
                                "Online OPD {OPDFormid} approved; " +
                                "doctor email deferred until payment success",
                                model.OPDFormid
                            );
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,

                        "verify_OPDForm - online meeting " +
                        "invite check failed: " +
                        ex.Message
                    );
                }
            }


            return "201.1";
        }
        [HttpPost]
        public virtual async Task<IActionResult>
    verify_OPDForm_Bulk(
        [FromBody] OPDBulkReviewRequest model)
        {
            if (
                model == null ||
                model.items == null ||
                model.items.Count == 0
            )
            {
                return BadRequest(
                    "No OPD records selected."
                );
            }


            var results =
                new List<object>();


            foreach (var item in model.items)
            {
                var singleModel =
                    new OPDFormReviewModel
                    {
                        OPDFormid =
                            item.OPDFormid,

                        verifiedstatus =
                            model.verifiedstatus,

                        reviewcomments =
                            model.reviewcomments,

                        /*
                         * IMPORTANT:
                         * task belongs to THIS OPD.
                         */
                        task =
                            item.task,

                        appointmentmode =
                            item.appointmentmode,

                        tenantid =
    !string.IsNullOrWhiteSpace(item.tenantid)
        ? item.tenantid
        : HttpContext.Session.GetString("NalamVazhachoosedtenantid") ?? "",

                        patient =
                            item.patient,

                        practitioner =
                            item.practitioner,

                        appointmentdate =
                            item.appointmentdate,

                        durationfrom =
                            item.durationfrom,

                        durationto =
                            item.durationto
                    };


                string result;


                try
                {
                    result =
                        await ProcessSingleOPDReview(
                            singleModel
                        );
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,

                        "Bulk OPD processing failed. " +
                        "OPDFormid: {OPDFormid}",

                        item.OPDFormid
                    );


                    result =
                        ex.Message;
                }


                results.Add(
                    new
                    {
                        OPDFormid =
                            item.OPDFormid,

                        result =
                            result
                    }
                );


                /*
                 * Stop immediately on failure.
                 *
                 * Safer for your current bulk action.
                 */
                if (
                    !string.Equals(
                        result?.Replace("\"", ""),
                        "201.1",
                        StringComparison.Ordinal
                    )
                )
                {
                    return BadRequest(
                        new
                        {
                            message =
                                result,

                            failedOPDFormid =
                                item.OPDFormid,

                            results =
                                results
                        }
                    );
                }
            }


            return Ok(
                new
                {
                    message =
                        "201.1",

                    processedCount =
                        model.items.Count,

                    results =
                        results
                }
            );
        }

        //[HttpPost()]
        //public virtual async Task<string> verify_OPDForm([FromBody]OPDFormReviewModel model)
        //{
        //	string message = "";
        //	try
        //	{
        //		bool isOnlineAppointmentMode =
        //			(model.appointmentmode ?? "").IndexOf("online", StringComparison.OrdinalIgnoreCase) >= 0;
        //		message = await ApiClient.Post_ApiValuesGetString(getHttpClient(), "api/OPDForm/verify_OPDForm", model);
        //				if(message.Replace("\"","")=="201.1")
        //		{
        //			TempData["message"] = "Success";

        //			/* ── After successful Approve: handle ClinicalAppointment ── */
        //			if ((string.Equals(model.verifiedstatus, "Approved", StringComparison.OrdinalIgnoreCase)
        //				|| string.Equals(model.verifiedstatus, "OPD Approved", StringComparison.OrdinalIgnoreCase))
        //				&& !string.IsNullOrWhiteSpace(model.OPDFormid))
        //			{
        //				if (isOnlineAppointmentMode)
        //					await SendOnlineOPDPaymentEmail(model);

        //				/* Look for a 'Requested' appointment created by the health seeker */
        //				var apptJson = await ApiClient.Get_ApiValues(client,
        //					"api/OPDForm/Get_Appointment_By_OPDForm?OPDFormid=" + model.OPDFormid);

        //				/* Track appointment data for the online meeting invite */
        //				Guid ivPatient = Guid.Empty;
        //				Guid ivPractitioner = Guid.Empty;
        //				DateTime ivApptDate = DateTime.MinValue;
        //				string ivDurationFrom = null;
        //				string ivDurationTo = null;
        //				bool ivInviteHandledByCreate = false;

        //				bool requestedApptFound = false;
        //				if (!string.IsNullOrWhiteSpace(apptJson) && apptJson.Length > 2)
        //				{
        //					try
        //					{
        //						var apptRows = JsonConvert.DeserializeObject<JArray>(apptJson);
        //						var appt = apptRows?[0];
        //						var apptId = appt?["clinicalappointmentid"]?.ToString();
        //						if (!string.IsNullOrWhiteSpace(apptId))
        //						{
        //							requestedApptFound = true;
        //							/* Update 'Requested' → 'Scheduled' */
        //							var statusModel = new AppointmentStatusUpdateModel
        //							{
        //								ClinicalAppointmentid = new Guid(apptId),
        //								status = "Scheduled"
        //							};
        //							await ApiClient.Post_ApiValuesGetString(client,
        //								"api/OPDForm/Update_Appointment_Status", statusModel);

        //							/* Capture appointment data for meeting invite */
        //							Guid.TryParse(appt?["patient"]?.ToString(), out ivPatient);
        //							Guid.TryParse(appt?["practitioner"]?.ToString(), out ivPractitioner);
        //							ivDurationFrom = appt?["durationfrom"]?.ToString();
        //							ivDurationTo   = appt?["durationto"]?.ToString();
        //							var apptDateStr = appt?["appointmentdate"]?.ToString();
        //							if (!string.IsNullOrWhiteSpace(apptDateStr))
        //								DateTime.TryParse(apptDateStr, out ivApptDate);
        //						}
        //					}
        //					catch (Exception ex)
        //					{
        //						_logger.LogError(ex, "verify_OPDForm – failed to update Requested appointment: " + ex.Message);
        //					}
        //				}

        //				/* No 'Requested' appointment → reviewer assigned slot → create Scheduled appointment */
        //				if (!requestedApptFound
        //					&& !string.IsNullOrWhiteSpace(model.practitioner)
        //					&& !string.IsNullOrWhiteSpace(model.durationfrom)
        //					&& !string.IsNullOrWhiteSpace(model.durationto)
        //					&& !string.IsNullOrWhiteSpace(model.patient))
        //				{
        //					try
        //					{
        //						await Create_OPD_Clinical_Appointment(
        //							tenantid:        model.tenantid ?? "",
        //							patient:         new Guid(model.patient),
        //							practitioner:    new Guid(model.practitioner),
        //							appointmentdate: model.appointmentdate ?? "",
        //							durationfrom:    model.durationfrom,
        //							durationto:      model.durationto,
        //							status:          "Scheduled",
        //							opdformid:       model.OPDFormid,
        //							taskid:          model.task ?? ""
        //						);
        //						/* Invite is handled by Create_OPD_Clinical_Appointment for this path */
        //						ivInviteHandledByCreate = true;
        //					}
        //					catch (Exception ex)
        //					{
        //						_logger.LogError(ex, "verify_OPDForm – failed to create Scheduled appointment: " + ex.Message);
        //					}
        //				}

        //				/* Send screening meeting link when task is Online OP New / Online OP Follow up.
        //				   Skipped when Create_OPD_Clinical_Appointment already fired the invite. */
        //				if (!ivInviteHandledByCreate && isOnlineAppointmentMode && !string.IsNullOrWhiteSpace(model.task) && ivPractitioner != Guid.Empty)
        //				{
        //					try
        //					{
        //						var loginUserId = HttpContext.Session.GetString("NalamVazhaloginUserID") ?? "";
        //						var taskJson = await ApiClient.Get_ApiValues(client,
        //							$"api/Task/getById_Task?Taskid={model.task}&loginUserID={loginUserId}");
        //						if (!string.IsNullOrWhiteSpace(taskJson))
        //						{
        //							var taskObj = JsonConvert.DeserializeObject<TaskModel>(taskJson);
        //							var taskName = taskObj?.taskname ?? "";
        //							bool isOnlineOP =
        //								taskName.IndexOf("Online OP New", StringComparison.OrdinalIgnoreCase) >= 0
        //								|| taskName.IndexOf("Online OP Follow up", StringComparison.OrdinalIgnoreCase) >= 0;
        //							if (isOnlineOP)
        //							{
        //								_logger.LogInformation("Online OPD {OPDFormid} approved; doctor email deferred until payment success", model.OPDFormid);
        //							}
        //						}
        //					}
        //					catch (Exception ex)
        //					{
        //						_logger.LogError(ex, "verify_OPDForm – online meeting invite check failed: " + ex.Message);
        //					}
        //				}

        //			}

        //				}else{
        //					TempData["errMessage"] = message.Replace("\"","");
        //		}

        //				message=message.Replace("\"","");



        //	}
        //	catch (Exception ex)
        //	{

        //                    _logger.LogError(ex,"An exception occurred in - OPDForm / verify_OPDForm, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));

        //		TempData["errMessage"] = ex.Message;
        //		message = ex.Message;
        //	}


        //	return message;
        //}

        private string GetOPDOnlineInviteSessionKey(string opdformid)
		{
			return "OPDOnlineInviteSent_" + (opdformid ?? "");
		}

		private async Task<bool> SendOPDOnlineMeetingInviteForOnlineTask(
			HttpClient httpClient,
			string loginUserId,
			string taskid,
			Guid patient,
			Guid practitioner,
			DateTime apptDate,
			string durationfrom,
			string durationto,
			string bookingid,
			string tenantName,
			string logSource)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(taskid)
					|| patient == Guid.Empty
					|| practitioner == Guid.Empty
					|| apptDate == DateTime.MinValue
					|| string.IsNullOrWhiteSpace(durationfrom)
					|| string.IsNullOrWhiteSpace(durationto))
					return false;

				var taskJson = await ApiClient.Get_ApiValues(httpClient,
					$"api/Task/getById_Task?Taskid={taskid}&loginUserID={loginUserId}");
				if (string.IsNullOrWhiteSpace(taskJson))
					return false;

				var taskObj = JsonConvert.DeserializeObject<TaskModel>(taskJson);
				var taskName = taskObj?.taskname ?? "";
				bool isOnlineOP =
					taskName.IndexOf("Online OP New", StringComparison.OrdinalIgnoreCase) >= 0
					|| taskName.IndexOf("Online OP Follow up", StringComparison.OrdinalIgnoreCase) >= 0;

				if (!isOnlineOP)
					return false;

				// Defer doctor emails until payment succeeds; this avoids the old early/duplicate senior invite.
				return true;
			}
			catch (Exception ex)
			{
				_logger?.LogError(ex, logSource + " - online meeting invite check failed: " + ex.Message);
				return false;
			}
		}

		private async Task SendOPDOnlineMeetingInvite(
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
				var alertContent = dt.Rows[0]["alertcontent"].ToString();

				/* Reuse the IPD Allot_Doctor template — replace IPD-specific text for OPD context */
				alertContent = alertContent
					.Replace("IPD screening appointment", "OPD online appointment")
					.Replace("IPD Screening appointment", "OPD online appointment")
					.Replace("IPD screening", "OPD online");
				alertSubject = alertSubject
					.Replace("IPD screening appointment", "OPD online appointment")
					.Replace("IPD Screening appointment", "OPD online appointment")
					.Replace("IPD screening", "OPD online");

				var meetingLinkHtml = !string.IsNullOrWhiteSpace(meetingLink)
					? $"<p style='margin:0 0 16px;'><strong>Meeting Link:</strong> <a href='{meetingLink}' style='color:#04927B;'>{meetingLink}</a></p>"
					: "";

				alertContent = alertContent
					.Replace("{tenantname}",      tenantName)
					.Replace("{patientname}",     patientName)
					.Replace("{doctorname}",      doctorName)
					.Replace("{appointmentdate}", apptDate.ToString("dd/MM/yyyy"))
					.Replace("{durationfrom}",    durationfrom)
					.Replace("{durationto}",      durationto)
					.Replace("~meetinglink~",     meetingLinkHtml)
					.Replace("{currentDate}",     DateTime.Now.ToString("dd MMM yyyy"));

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
				_logger?.LogError(ex, "OPD online meeting invite email failed: " + ex.Message);
			}
		}

	[HttpGet()]
		public virtual async Task<string> Get_Appointment_By_OPDForm(string OPDFormid)
	{
		return await ApiClient.Get_ApiValues(getHttpClient(),
			"api/OPDForm/Get_Appointment_By_OPDForm?OPDFormid=" + OPDFormid +
			"&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
	}

	[HttpPost()]
	public virtual async Task<string> Reschedule_OPD_Clinical_Appointment(
		string clinicalappointmentid,
		string tenantid,
		Guid patient,
		Guid practitioner,
		string appointmentdate,
		string durationfrom,
		string durationto,
		string status,
		string opdformid = "")
	{
		try
		{
			var loginUserId = HttpContext.Session.GetString("NalamVazhaloginUserID");
			if (string.IsNullOrWhiteSpace(loginUserId))
				return "Session Expired";

			if (string.IsNullOrWhiteSpace(clinicalappointmentid))
				return "Appointment not found";

			if (!Guid.TryParse(clinicalappointmentid, out var appointmentGuid))
				return "Invalid appointment id";

			if (!Guid.TryParse(tenantid, out var tenantGuid))
				return "Invalid tenantid";

			if (patient == Guid.Empty) return "Patient is required";
			if (practitioner == Guid.Empty) return "Doctor is required";

			if (!DateTime.TryParseExact(appointmentdate,
					new[] { "dd/MM/yyyy", "d/M/yyyy", "dd-MM-yyyy", "d-M-yyyy", "yyyy-MM-dd" },
					CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime apptDate))
				return "Invalid appointment date";

			if (string.IsNullOrWhiteSpace(durationfrom) || string.IsNullOrWhiteSpace(durationto))
				return "Slot time is required";

			var httpClient = getHttpClient();
			var jsonObj = await ApiClient.Get_ApiValues(httpClient,
				"api/ClinicalAppointment/getById_ClinicalAppointment?ClinicalAppointmentid=" + appointmentGuid +
				"&loginUserID=" + loginUserId);

			if (string.IsNullOrWhiteSpace(jsonObj) || jsonObj.Length <= 2)
				return "Appointment not found";

			var model = JsonConvert.DeserializeObject<ClinicalAppointmentModel>(jsonObj);
			if (model == null)
				return "Appointment not found";

			model.tenantid = tenantGuid;
			model.patient = patient;
			model.practitioner = practitioner;
			model.actualpractitioner = practitioner;
			model.appointmentdate = apptDate;
			model.durationfrom = durationfrom;
			model.durationto = durationto;
			model.status = string.IsNullOrWhiteSpace(status) ? model.status : status;
			model.origin = string.IsNullOrWhiteSpace(model.origin) ? "OPD" : model.origin;
			model.bookingid = string.IsNullOrWhiteSpace(opdformid) ? model.bookingid : opdformid;
			model.modifieduser = new Guid(loginUserId);
			model.craftmyapp_actionmethodname = "Reschedule_Appointment";

			var result = await ApiClient.Post_ApiValuesGetString(httpClient,
				"api/ClinicalAppointment/Reschedule_Appointment",
				model);

			if (result != null && result.Replace("\"", "").Contains("201.1"))
				return "Success";

			return result ?? "Failed to update clinical appointment";
		}
		catch (Exception ex)
		{
			_logger?.LogError(ex, "Reschedule_OPD_Clinical_Appointment failed: " + ex.Message);
			return ex.Message;
		}
	}

		public virtual IActionResult Review_OPD()
		{
			return View();
		}

		public virtual IActionResult Approved_OPD_Forms()
		{
			return View();
		}

		[HttpGet()]
		public virtual async Task<string> get_Approved_OPD_Forms(string tenantid
,string patientname
,string preferreddate_automatonfrom
,string preferreddate_automatonto
,string preferreddoctor
, int? pagesize=100 , int? pagenumber=0,string searchterm="",string sortFieldsJson="")
		{

					        return await ApiClient.Get_ApiValues(getHttpClient(), "api/OPDForm/Approved_OPD_Forms?tenantid="+Uri.EscapeDataString(tenantid ?? string.Empty)+"&patientname="+Uri.EscapeDataString(patientname ?? string.Empty)+"&preferreddate_automatonfrom="+Uri.EscapeDataString(preferreddate_automatonfrom ?? string.Empty)+"&preferreddate_automatonto="+Uri.EscapeDataString(preferreddate_automatonto ?? string.Empty)+"&preferreddoctor="+Uri.EscapeDataString(preferreddoctor ?? string.Empty)+"&loginUserID="+Uri.EscapeDataString(HttpContext.Session.GetString("NalamVazhaloginUserID") ?? string.Empty)
+ "&pagesize=" + pagesize + "&pagenumber="+ pagenumber + "&searchterm="+ Uri.EscapeDataString(searchterm ?? string.Empty) + "&sort_fields=" + Uri.EscapeDataString(sortFieldsJson ?? string.Empty));
		}


		public virtual IActionResult View_Approved_OPD_Form()
		{
			return View();
		}


		public virtual async Task<string> getById_allinfo_OPDForm(string OPDFormid)
		{
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/OPDForm/getById_allinfo_OPDForm?OPDFormid="+OPDFormid);

		}
		[HttpGet()]
		public virtual async Task<string> lookup_OPDForm_task(String tenantid)
		{

			return await ApiClient.Get_ApiValues(getHttpClient(), "api/OPDForm/lookup_OPDForm_task?tenantid=" + tenantid + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
		}

		private async Task<bool> SendOPDWorkflowDoctorEmail(string opdFormId, bool sendToIntern)
		{
			try
			{
				var loginUserId = HttpContext.Session.GetString("NalamVazhaloginUserID") ?? "";
				var client = getHttpClient();
				var json = await ApiClient.Get_ApiValues(client,
					"api/OPDForm/Get_OPD_Online_Doctor_Workflow_Context?OPDFormid=" + opdFormId + "&loginUserID=" + loginUserId);
				var row = string.IsNullOrWhiteSpace(json) ? null : JsonConvert.DeserializeObject<JArray>(json)?.FirstOrDefault();
				if (row == null) return false;

				var mode = row["appointmentmode"]?.ToString() ?? "";
				var taskName = row["taskname"]?.ToString() ?? "";
				if (mode.IndexOf("online", StringComparison.OrdinalIgnoreCase) < 0
					&& taskName.IndexOf("online", StringComparison.OrdinalIgnoreCase) < 0) return false;

				var email = row[sendToIntern ? "interndoctoremail" : "seniordoctoremail"]?.ToString()?.Trim();
				var healthSeekerEmail = row["patientemail"]?.ToString()?.Trim();
				var doctorName = row[sendToIntern ? "interndoctorname" : "seniordoctorname"]?.ToString()?.Trim();
				var meetingLink = row[sendToIntern ? "internmeetinglink" : "seniormeetinglink"]?.ToString()?.Trim();
				if (string.IsNullOrWhiteSpace(email))
				{
					_logger.LogWarning("OPD workflow email skipped: {Role} email is missing for OPD {OPDFormid}", sendToIntern ? "intern" : "senior", opdFormId);
					return false;
				}
				if (string.IsNullOrWhiteSpace(meetingLink))
				{
					_logger.LogWarning("OPD workflow email skipped: {Role} screeningmeetinglink is missing for OPD {OPDFormid}", sendToIntern ? "intern" : "senior", opdFormId);
					return false;
				}

				string Enc(string value) => System.Net.WebUtility.HtmlEncode(value ?? "");
				var patientName = row["patientname"]?.ToString() ?? "-";
				var opdNumber = row["bookingreferencenumber"]?.ToString() ?? "-";
				var appointmentDate = row["appointmentdate"]?.ToString() ?? "-";
				var duration = (row["durationfrom"]?.ToString() ?? "-") + " - " + (row["durationto"]?.ToString() ?? "-");
				var instruction = sendToIntern
					? "Please conduct the patient screening, record the history and assessment, and submit it for senior doctor review."
					: "The intern has completed the assessment. Please review the submitted assessment and complete the senior consultation.";
				var subject = (sendToIntern ? "OPD Screening Required" : "OPD Assessment Review Required") + " - " + opdNumber;
				var body = $"<p>Dear {Enc(doctorName)},</p><p>{Enc(instruction)}</p>" +
					$"<p><strong>Patient:</strong> {Enc(patientName)}<br/><strong>OPD Number:</strong> {Enc(opdNumber)}<br/>" +
					$"<strong>Appointment:</strong> {Enc(appointmentDate)} {Enc(duration)}</p>" +
					$"<p><strong>Meeting Link:</strong> <a href='{Enc(meetingLink)}'>{Enc(meetingLink)}</a></p>";

				var tenantId = row["tenantid"]?.ToString() ?? HttpContext.Session.GetString("NalamVazhachoosedtenantid") ?? "";
				var mailBox = await new MailSender().GetTenantMailBox(client, tenantId,
					MailSender.ResolveApplicableService("ClinicalAppointment", "Allot_Doctor"));
				if (mailBox == null) return false;
				var sent = new Mailer(mailBox).SendMail_TLS(
					email,
					string.IsNullOrWhiteSpace(healthSeekerEmail) ? null : healthSeekerEmail,
					subject, body, true, null, false);
				await ApiClient.Post_ApiValuesGetString(client, "api/MailLogs/Create_MailLog", new MailLogsModel
				{
					entityname = "OPDForm", entityid = opdFormId,
					mailfor = sendToIntern ? "Intern Screening" : "Senior Assessment Review",
					mailto = email, mailsubject = subject, mailbody = body, issent = sent,
					createduser = Guid.TryParse(loginUserId, out var userId) ? userId : Guid.Empty,
					craftmyapp_actionmethodname = "Create_MailLog"
				});
				return sent;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "OPD workflow doctor email failed for {OPDFormid}", opdFormId);
				return false;
			}
		}

		private async Task<bool> SendOnlineOPDPaymentEmail(OPDFormReviewModel model)
		{
			if (model == null
				|| string.IsNullOrWhiteSpace(model.OPDFormid)
				|| string.IsNullOrWhiteSpace(model.patient)
				|| (model.appointmentmode ?? "").IndexOf("online", StringComparison.OrdinalIgnoreCase) < 0)
				return false;

			try
			{
				var loginUserId = HttpContext.Session.GetString("NalamVazhaloginUserID") ?? "";
				var client = getHttpClient();
				var patientJson = await ApiClient.Get_ApiValues(client,
					"api/PatientProfile/getById_PatientProfile?PatientProfileid=" + model.patient
					+ "&loginUserID=" + loginUserId);
				var opdJson = await ApiClient.Get_ApiValues(client,
					"api/OPDForm/getById_allinfo_OPDForm?OPDFormid=" + model.OPDFormid
					+ "&loginUserID=" + loginUserId);
				var patient = string.IsNullOrWhiteSpace(patientJson)
					? null
					: JsonConvert.DeserializeObject<PatientProfileModel>(patientJson);
				var opdRow = string.IsNullOrWhiteSpace(opdJson)
					? null
					: JsonConvert.DeserializeObject<JArray>(opdJson)?.FirstOrDefault();
				var bookingReference = opdRow?["bookingreferencenumber"]?.ToString()?.Trim();
				if (string.IsNullOrWhiteSpace(bookingReference))
					bookingReference = "OPD Appointment";
				var email = patient?.emailaddress?.Trim();
				if (string.IsNullOrWhiteSpace(email))
				{
					_logger.LogWarning("Online OPD payment email skipped for {OPDFormid}: Health Seeker email is missing.", model.OPDFormid);
					return false;
				}

				var relativeUrl = Url.Action("OPD_Consultation_Fee", "OPDForm",
					new { OPDFormid = model.OPDFormid });
				var paymentUrl = $"{Request.Scheme}://{Request.Host}{relativeUrl}";
				var patientName = $"{patient.firstname} {patient.lastname}".Trim();
				var subject = "OPD Consultation Fee Payment - " + bookingReference;
				string Enc(string value) => System.Net.WebUtility.HtmlEncode(value ?? "");
				var body =
					$"<p>Dear {Enc(patientName)},</p>" +
					"<p>Please pay the consultation fee to confirm your appointment. Kindly note that the doctor will review your case only after the payment has been completed.</p>" +
					$"<p><strong>OPD Number:</strong> {Enc(bookingReference)}</p>" +
					$"<p><a href='{Enc(paymentUrl)}' style='display:inline-block;padding:10px 18px;background:#087861;color:#fff;text-decoration:none;border-radius:5px;'>Pay OPD Consultation Fee</a></p>" +
					$"<p>If the button does not open, use this link:<br/><a href='{Enc(paymentUrl)}'>{Enc(paymentUrl)}</a></p>" +
					"<p>If you experience any difficulty making the payment, please contact the Front Desk for assistance.</p>";

				var tenantId = model.tenantid ?? HttpContext.Session.GetString("NalamVazhachoosedtenantid") ?? "";
				var mailBox = await new MailSender().GetTenantMailBox(client, tenantId,
					MailSender.ResolveApplicableService("ClinicalAppointment", "Allot_Doctor"));
				if (mailBox == null) return false;

				var sent = new Mailer(mailBox).SendMail_TLS(email, null, subject, body, true, null, false);
				await ApiClient.Post_ApiValuesGetString(client, "api/MailLogs/Create_MailLog", new MailLogsModel
				{
					entityname = "OPDForm",
					entityid = model.OPDFormid,
					mailfor = "Health Seeker OPD Payment",
					mailto = email,
					mailsubject = subject,
					mailbody = body,
					issent = sent,
					createduser = Guid.TryParse(loginUserId, out var userId) ? userId : Guid.Empty,
					craftmyapp_actionmethodname = "Create_MailLog"
				});
				return sent;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Online OPD payment email failed for {OPDFormid}", model.OPDFormid);
				return false;
			}
		}

		[HttpGet()]
		public virtual async Task<string> Get_OPD_Task_Eligibility(string patientname, string excludeopdformid = "")
		{
			return await ApiClient.Get_ApiValues(
				getHttpClient(),
				"api/OPDForm/Get_OPD_Task_Eligibility?patientname=" + Uri.EscapeDataString(patientname ?? "")
				+ "&excludeopdformid=" + Uri.EscapeDataString(excludeopdformid ?? "")
				+ "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
		}

		[HttpGet()]
                    public virtual async Task<string> lookup_OPDForm_patientname(String tenantid,string searchterm, int? pagesize, int? pagenumber)
		{

                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/OPDForm/lookup_OPDForm_patientname?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
		}
		[HttpGet()]
                    public virtual async Task<string> lookup_OPDForm_preferreddoctor(String tenantid,string searchterm, int? pagesize, int? pagenumber)
		{

                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/OPDForm/lookup_OPDForm_preferreddoctor?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
		}

		[HttpGet()]
		public virtual async Task<string> lookup_OPDForm_medicalinfo_medicalconditionname(string searchterm, int? pagesize, int? pagenumber)
		{
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/OPDForm/lookup_OPDForm_medicalinfo_medicalconditionname?searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
		}


		[HttpGet()]
                            public virtual async Task<string> get_all_MedicalCondition(string tenantid,string searchterm, string pagesize="1000", string pagenumber="1")
		{

                                return await ApiClient.Get_ApiValues(getHttpClient(), "api/MedicalCondition/get_all_MedicalCondition?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
		}

		[HttpGet()]
		public virtual async Task<string> Get_Doctor_Available_Slots(string Peopleid, string appointmentdate, Guid? taskid = null, string taskname = "", string loginUserID = "")
		{
			var tenantid = HttpContext.Session.GetString("NalamVazhachoosedtenantid") ?? "";
			return await ApiClient.Get_ApiValues(getHttpClient(),
				"api/OPDForm/Get_Doctor_Available_Slots?Peopleid=" + Peopleid +
				"&appointmentdate=" + appointmentdate +
				"&tenantid=" + tenantid+
		(taskid.HasValue ? "&taskid=" + taskid.Value : "")+
				"&taskname=" + Uri.EscapeDataString(taskname ?? "") +
				"&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
		}

		[HttpGet()]
		public virtual async Task<string> Get_Doctor_Available_Slots_By_Preferences(string Peopleid, string appointmentdate, string slotpreferences, Guid? taskid = null, string taskname = "", string loginUserID = "")
		{
			var tenantid = HttpContext.Session.GetString("NalamVazhachoosedtenantid") ?? "";
			return await ApiClient.Get_ApiValues(getHttpClient(),
				"api/OPDForm/Get_Doctor_Available_Slots_By_Preferences?Peopleid=" + Peopleid +
				"&appointmentdate=" + appointmentdate +
				"&slotpreferences=" + Uri.EscapeDataString(slotpreferences ?? "") +
				"&tenantid=" + tenantid +
				(taskid.HasValue ? "&taskid=" + taskid.Value : "") +
				"&taskname=" + Uri.EscapeDataString(taskname ?? "") +
				"&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
		}
		[HttpGet()]
		public virtual async Task<string> Get_Doctor_Consultation_Fee(string Peopleid, string tasktype, string loginUserID = "")
		{
			return await ApiClient.Get_ApiValues(getHttpClient(), "api/OPDForm/Get_Doctor_Consultation_Fee?Peopleid=" + Peopleid + "&tasktype=" + tasktype + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
		}

		/// <summary>Returns IP New task type fee schedule for a doctor — used in IPD Pre-Admission Consultation.</summary>
		[HttpGet()]
		public virtual async Task<string> Get_IPD_Doctor_Consultation_Fee(string Peopleid, string tasktype = "IP New", string loginUserID = "")
		{
			return await ApiClient.Get_ApiValues(getHttpClient(),
				"api/OPDForm/Get_IPD_Doctor_Consultation_Fee?Peopleid=" + Peopleid +
				"&tasktype=" + Uri.EscapeDataString(tasktype) +
				"&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
		}

		[HttpGet()]
		public virtual async Task<IActionResult> OPD_Consultation_Fee(string OPDFormid)
		{
			if (string.IsNullOrEmpty(OPDFormid))
			{
				TempData["errMessage"] = "Invalid request";
				return RedirectToAction("Approved_OPD_Forms");
			}
			try
			{
				var loginUserId = HttpContext.Session.GetString("NalamVazhaloginUserID");
				var sessionRole = HttpContext.Session.GetString("NalamVazhauserrole") ?? "";
				ViewBag.CanAddReceivable = false;
				if (sessionRole.Equals("Frontdesk Admin", StringComparison.OrdinalIgnoreCase))
				{
					var eligibilityJson = await ApiClient.Get_ApiValues(getHttpClient(),
						"api/OPDForm/Get_OPD_Action_Eligibility?OPDFormid=" + Uri.EscapeDataString(OPDFormid) +
						"&userrole=" + Uri.EscapeDataString(sessionRole) +
						"&loginUserID=" + Uri.EscapeDataString(loginUserId ?? ""));
					var eligibilityRows = !string.IsNullOrWhiteSpace(eligibilityJson) && eligibilityJson.Length > 2
						? JsonConvert.DeserializeObject<JArray>(eligibilityJson)
						: null;
					ViewBag.CanAddReceivable = eligibilityRows?.FirstOrDefault()?["canaddreceivable"]?.Value<bool>() == true;
				}

				// Load from Receivable table (auto-generated consultation fee row)
				var json = await ApiClient.Get_ApiValues(getHttpClient(),
					"api/OPDForm/OPD_Consultation_Fee?OPDFormid=" + OPDFormid);
				if (string.IsNullOrEmpty(json) || json.Length <= 2)
				{
					var opdJson = await ApiClient.Get_ApiValues(getHttpClient(),
						"api/OPDForm/getById_OPDForm?OPDFormid=" + OPDFormid + "&loginUserID=" + loginUserId);
					var opdForm = !string.IsNullOrWhiteSpace(opdJson) && opdJson.Length > 2
						? JsonConvert.DeserializeObject<OPDFormModel>(opdJson)
						: null;

					var infoJson = await ApiClient.Get_ApiValues(getHttpClient(),
						"api/OPDForm/getById_allinfo_OPDForm?OPDFormid=" + OPDFormid + "&loginUserID=" + loginUserId);
					JToken infoRow = null;
					if (!string.IsNullOrWhiteSpace(infoJson) && infoJson.Length > 2)
					{
						var infoRows = JsonConvert.DeserializeObject<JArray>(infoJson);
						infoRow = infoRows?.FirstOrDefault();
					}

					var selectedTaskType = infoRow?["taskname"]?.ToString()
						?? infoRow?["task_master"]?.ToString()
						?? "";
					if (string.IsNullOrWhiteSpace(selectedTaskType) && opdForm?.task != null)
					{
						var taskJson = await ApiClient.Get_ApiValues(getHttpClient(),
							"api/Task/getById_Task?Taskid=" + opdForm.task + "&loginUserID=" + loginUserId);
						if (!string.IsNullOrWhiteSpace(taskJson) && taskJson.Length > 2)
						{
							var taskModel = JsonConvert.DeserializeObject<TaskModel>(taskJson);
							selectedTaskType = taskModel?.taskname ?? "";
						}
					}
					if (string.IsNullOrWhiteSpace(selectedTaskType))
						selectedTaskType = "OPD Consultation Fee";

					ViewBag.BookingReference = infoRow?["bookingreferencenumber"]?.ToString()
						?? opdForm?.bookingreferencenumber
						?? "-";
					var patientNameWithoutReceivable = infoRow?["patientname"]?.ToString() ?? "-";
					ViewBag.PatientName = patientNameWithoutReceivable;
					ViewBag.PatientHeaderDisplay = await ResolveOPDPatientHeaderName(opdForm?.patientname.ToString(), patientNameWithoutReceivable, loginUserId);
					var doctorNameWithoutReceivable = CleanOPDDisplayValue(infoRow?["preferreddoctor_master"]?.ToString());
					if (string.IsNullOrWhiteSpace(doctorNameWithoutReceivable))
						doctorNameWithoutReceivable = CleanOPDDisplayValue(infoRow?["preferreddoctor"]?.ToString());
					if (string.IsNullOrWhiteSpace(doctorNameWithoutReceivable))
						doctorNameWithoutReceivable = await ResolveOPDConsultingDoctorName(OPDFormid, loginUserId);
					ViewBag.DoctorName = string.IsNullOrWhiteSpace(doctorNameWithoutReceivable) ? "-" : doctorNameWithoutReceivable;
					ViewBag.OPDFormid = OPDFormid;
					ViewBag.receivablefor = selectedTaskType;
					ViewBag.HasConsultationFeeReceivable = false;
					ViewBag.TotalFee = 0m;
					ViewBag.FeeRows = new JArray();
					ViewBag.BillingSummary = null;

					var billingWithoutReceivable = new BillingPaymentModel
					{
						amount = 0,
						receivedamount = 0,
						craftmyapp_actionmethodname = "Add_Billing_Payment",
						isdeleted = false
					};

					if (opdForm?.tenantid != null)
						billingWithoutReceivable.tenantid = opdForm.tenantid;
					if (opdForm != null && opdForm.patientname != Guid.Empty)
						billingWithoutReceivable.patientname = opdForm.patientname;
					if (Guid.TryParse(OPDFormid, out var opdWithoutReceivableGuid))
						billingWithoutReceivable.opdnumber = opdWithoutReceivableGuid;

					return View(billingWithoutReceivable);
				}
				var rows = JsonConvert.DeserializeObject<JArray>(json);
				var first = rows?[0];

				var patientprofileidStr = first?["patientname"]?.ToString();
				var patientNameWithReceivable = first?["patientname_master"]?.ToString();
				ViewBag.BookingReference = first?["bookingreferencenumber"]?.ToString();
				ViewBag.PatientName      = patientNameWithReceivable;
				ViewBag.PatientHeaderDisplay = await ResolveOPDPatientHeaderName(patientprofileidStr, patientNameWithReceivable, loginUserId);
				var doctorName = CleanOPDDisplayValue(first?["preferreddoctor_master"]?.ToString());
				if (string.IsNullOrWhiteSpace(doctorName))
					doctorName = await ResolveOPDConsultingDoctorName(OPDFormid, loginUserId);
				ViewBag.DoctorName       = string.IsNullOrWhiteSpace(doctorName) ? "-" : doctorName;
				ViewBag.OPDFormid        = OPDFormid;
				ViewBag.VerifiedStatus   = first?["verifiedstatus"]?.ToString() ?? "";

				var patientvisitidStr  = first?["patientvisit"]?.ToString();
				var tenantidStr        = first?["tenantid"]?.ToString();
				var rawReceivableFor = first?["receivablefor"]?.ToString() ?? "";
				ViewBag.receivablefor = string.IsNullOrWhiteSpace(rawReceivableFor) ? "OPD New" : rawReceivableFor.Trim();

				// Each row in `rows` is one receivable entry — use as fee rows directly
				var allReceivableRows = rows ?? new JArray();
				if (!string.IsNullOrWhiteSpace(tenantidStr))
				{
					var receivablesJson = await ApiClient.Get_ApiValues(getHttpClient(),
						"api/Receivable/Receivables?tenantid=" + Uri.EscapeDataString(tenantidStr) +
						"&opdnumber=" + Uri.EscapeDataString(OPDFormid) +
						"&pagesize=200&pagenumber=0&loginUserID=" + Uri.EscapeDataString(loginUserId ?? ""));
					if (!string.IsNullOrWhiteSpace(receivablesJson) && receivablesJson.Length > 2)
					{
						var receivablesResult = JsonConvert.DeserializeObject<JObject>(receivablesJson);
						var linkedRows = receivablesResult?["detail"] as JArray;
						if (linkedRows != null && linkedRows.Any())
							allReceivableRows = linkedRows;
					}
				}

				string NormalizeReceivableType(string value) =>
					new string((value ?? "").Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();
				bool IsDepositOrCreditAdjustment(string typeKey) =>
					typeKey.Contains("deposit")
					|| typeKey.Contains("credit")
					|| typeKey.Contains("advance");

				decimal discountAmount = 0;
				decimal concessionAmount = 0;
				decimal receivableCreditAdjustment = 0;
				var feeRows = new JArray();
				foreach (var receivableRow in allReceivableRows)
				{
					if (!decimal.TryParse(receivableRow["amount"]?.ToString(),
						System.Globalization.NumberStyles.Any,
						System.Globalization.CultureInfo.InvariantCulture, out var receivableAmount))
						continue;

					var receivableType = receivableRow["receivablefor"]?.ToString()?.Trim() ?? "";
					var receivableTypeKey = NormalizeReceivableType(receivableType);
					if (receivableTypeKey == "discount")
					{
						discountAmount += Math.Abs(receivableAmount);
						continue;
					}
					if (receivableTypeKey == "concession")
					{
						concessionAmount += Math.Abs(receivableAmount);
						continue;
					}
					if (receivableAmount < 0m && IsDepositOrCreditAdjustment(receivableTypeKey))
					{
						receivableCreditAdjustment += Math.Abs(receivableAmount);
						continue;
					}

					if (receivableAmount > 0)
						feeRows.Add(receivableRow);
				}

				decimal totalFee = 0;
				foreach (var r in feeRows)
				{
					if (decimal.TryParse(r["amount"]?.ToString(), System.Globalization.NumberStyles.Any,
						System.Globalization.CultureInfo.InvariantCulture, out var f))
						totalFee += f;
				}

				// A receivable is only a charge; paid totals come from actual OPD payment transactions.
				decimal actualTotalPaid = 0;
				decimal refundAmountFromPayments = 0;
				decimal paymentCreditAdjustment = 0;
				var paymentsByReceivableType = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
				var paymentsJson = await ApiClient.Get_ApiValues(getHttpClient(),
					"api/BillingPayment/Get_Billing_Payments_For_OPD?OPDFormid=" + Uri.EscapeDataString(OPDFormid));
				if (!string.IsNullOrWhiteSpace(paymentsJson) && paymentsJson.Length > 2)
				{
					var paymentRows = JsonConvert.DeserializeObject<JArray>(paymentsJson) ?? new JArray();
					foreach (var paymentRow in paymentRows)
					{
						var paymentStatus = (paymentRow["paymentstatus"]?.ToString() ?? "").Trim().ToLowerInvariant();
						if (paymentStatus.Contains("failed") || paymentStatus.Contains("pending") || paymentStatus.Contains("cancel"))
							continue;
						if (decimal.TryParse(paymentRow["amount"]?.ToString(),
							System.Globalization.NumberStyles.Any,
							System.Globalization.CultureInfo.InvariantCulture, out var paymentAmount))
						{
							var paymentReceivableType = paymentRow["receivablefor"]?.ToString() ?? "";
							if (paymentReceivableType.IndexOf("refund", StringComparison.OrdinalIgnoreCase) >= 0)
							{
								refundAmountFromPayments += Math.Abs(paymentAmount);
								continue;
							}
							actualTotalPaid += paymentAmount;
							var paymentTypeKey = NormalizeReceivableType(paymentRow["receivablefor"]?.ToString());
							if (paymentAmount > 0m && IsDepositOrCreditAdjustment(paymentTypeKey))
								paymentCreditAdjustment += paymentAmount;
							if (paymentAmount > 0m && !string.IsNullOrWhiteSpace(paymentTypeKey))
								paymentsByReceivableType[paymentTypeKey] = paymentsByReceivableType.GetValueOrDefault(paymentTypeKey) + paymentAmount;
						}
					}
				}

				decimal recordedRefundAmount = refundAmountFromPayments;
				var reconciliationQuoteJson = await ApiClient.Get_ApiValues(getHttpClient(),
					"api/OPDForm/Get_OPD_Cancellation_Quote?OPDFormid=" + Uri.EscapeDataString(OPDFormid) +
					"&loginUserID=" + Uri.EscapeDataString(loginUserId ?? ""));
				if (!string.IsNullOrWhiteSpace(reconciliationQuoteJson) && reconciliationQuoteJson.Length > 2)
				{
					var reconciliationRows = JsonConvert.DeserializeObject<JArray>(reconciliationQuoteJson);
					if (decimal.TryParse(reconciliationRows?.FirstOrDefault()?["alreadyrefunded"]?.ToString(),
						System.Globalization.NumberStyles.Any,
						System.Globalization.CultureInfo.InvariantCulture, out var quoteRefundedAmount))
						recordedRefundAmount = Math.Max(recordedRefundAmount, quoteRefundedAmount);
				}
				// The OPD payment endpoint can omit manual refunds because they are negative
				// billing entries. Read the billing ledger as a fallback so an already
				// processed cash/manual refund is not offered for a second time.
				if (recordedRefundAmount <= 0m && !string.IsNullOrWhiteSpace(tenantidStr) && !string.IsNullOrWhiteSpace(patientprofileidStr))
				{
					var refundLedgerJson = await ApiClient.Get_ApiValues(getHttpClient(),
						"api/BillingPayment/Billing_Payment_List?tenantid=" + Uri.EscapeDataString(tenantidStr) +
						"&paymentdate_automatonfrom=&paymentdate_automatonto=&patientname=" + Uri.EscapeDataString(patientprofileidStr) +
						"&receivablefor=" + Uri.EscapeDataString("Cancellation Refund") +
						"&pagesize=200&pagenumber=0&searchterm=&sort_fields=&loginUserID=" + Uri.EscapeDataString(loginUserId ?? ""));
					if (!string.IsNullOrWhiteSpace(refundLedgerJson) && refundLedgerJson.Length > 2)
					{
						var refundLedgerToken = JToken.Parse(refundLedgerJson);
						var refundLedgerRows = refundLedgerToken is JArray ledgerArray
							? ledgerArray
							: refundLedgerToken["detail"] as JArray ?? new JArray();
						foreach (var refundRow in refundLedgerRows)
						{
							if (!string.Equals(refundRow["opdnumber"]?.ToString(), OPDFormid, StringComparison.OrdinalIgnoreCase)) continue;
							var status = (refundRow["refundstatus"]?.ToString() ?? refundRow["paymentstatus"]?.ToString() ?? "").Trim();
							if (status.IndexOf("failed", StringComparison.OrdinalIgnoreCase) >= 0 || status.IndexOf("cancel", StringComparison.OrdinalIgnoreCase) >= 0) continue;
							decimal.TryParse(refundRow["refundedamount"]?.ToString(), System.Globalization.NumberStyles.Any,
								System.Globalization.CultureInfo.InvariantCulture, out var refundedAmount);
							if (refundedAmount <= 0m && decimal.TryParse(refundRow["amount"]?.ToString(), System.Globalization.NumberStyles.Any,
								System.Globalization.CultureInfo.InvariantCulture, out var refundLedgerAmount))
								refundedAmount = Math.Abs(refundLedgerAmount);
							recordedRefundAmount += Math.Max(0m, refundedAmount);
						}
					}
				}
				var grossTotalPaid = Math.Max(0m, actualTotalPaid);
				actualTotalPaid = Math.Max(0m, grossTotalPaid - recordedRefundAmount);

				var netCharges = new List<decimal>();
				var allocatedAmounts = new List<decimal>();
				var receivableTypeKeys = new List<string>();
				var discountTargetIndex = -1;
				for (var index = 0; index < feeRows.Count; index++)
				{
					var feeRow = feeRows[index];
					decimal.TryParse(feeRow["amount"]?.ToString(),
						System.Globalization.NumberStyles.Any,
						System.Globalization.CultureInfo.InvariantCulture, out var chargeAmount);
					var typeKey = NormalizeReceivableType(feeRow["receivablefor"]?.ToString());
					if (discountTargetIndex < 0 && (typeKey == "opnew" || typeKey == "onlineopnew" || typeKey == "opfollowup" || typeKey == "onlineopfollowup"))
						discountTargetIndex = index;
					netCharges.Add(Math.Max(0m, chargeAmount));
					allocatedAmounts.Add(0m);
					receivableTypeKeys.Add(typeKey);
				}

				if (discountTargetIndex < 0 && netCharges.Count > 0) discountTargetIndex = 0;
				var remainingDiscount = discountAmount;
				for (var offset = 0; offset < netCharges.Count && remainingDiscount > 0m; offset++)
				{
					var index = (discountTargetIndex + offset) % netCharges.Count;
					var appliedDiscount = Math.Min(netCharges[index], remainingDiscount);
					netCharges[index] -= appliedDiscount;
					remainingDiscount -= appliedDiscount;
				}

				decimal allocatedPaymentTotal = 0m;
				decimal remainingPaymentBudget = actualTotalPaid;
				for (var index = 0; index < feeRows.Count; index++)
				{
					if (remainingPaymentBudget <= 0m) break;
					if (!paymentsByReceivableType.TryGetValue(receivableTypeKeys[index], out var matchingPayment) || matchingPayment <= 0m) continue;
					var allocated = Math.Min(Math.Min(netCharges[index], matchingPayment), remainingPaymentBudget);
					allocatedAmounts[index] += allocated;
					allocatedPaymentTotal += allocated;
					remainingPaymentBudget -= allocated;
					paymentsByReceivableType[receivableTypeKeys[index]] = matchingPayment - allocated;
				}

				var fallbackPayment = Math.Max(0m, actualTotalPaid - allocatedPaymentTotal);
				for (var offset = 0; offset < feeRows.Count && fallbackPayment > 0m; offset++)
				{
					var index = discountTargetIndex < 0 ? offset : (discountTargetIndex + offset) % feeRows.Count;
					var remainingCharge = Math.Max(0m, netCharges[index] - allocatedAmounts[index]);
					var allocated = Math.Min(remainingCharge, fallbackPayment);
					allocatedAmounts[index] += allocated;
					fallbackPayment -= allocated;
				}

				for (var index = 0; index < feeRows.Count; index++)
				{
					var allocatedPaid = allocatedAmounts[index];
					var netCharge = netCharges[index];
					feeRows[index]["paidamount"] = allocatedPaid;
					feeRows[index]["paymentstatus"] = netCharge <= 0m || allocatedPaid >= netCharge
						? "Paid"
						: allocatedPaid <= 0m
						? "Pending"
						: "Partially Paid";
				}

				decimal displayTotalCharge = totalFee;
				decimal depositCreditAdjustment = Math.Max(receivableCreditAdjustment, paymentCreditAdjustment);
				decimal netPayableAmount   = Math.Max(0, displayTotalCharge - discountAmount - concessionAmount - receivableCreditAdjustment);
				decimal displayBalance     = Math.Max(0, netPayableAmount - actualTotalPaid);
				decimal refundDue          = Math.Max(0, actualTotalPaid - netPayableAmount);
				string billingCurrency = first?["currency"]?.ToString();
				if (string.IsNullOrWhiteSpace(billingCurrency))
					billingCurrency = "INR";

				var summary = new OPDBillingSummaryModel
				{
					patientname      = ViewBag.PatientName,
					consultingdoctor = ViewBag.DoctorName,
					totalcharge      = displayTotalCharge,
					totalpaid        = grossTotalPaid,
					totalrefunded    = recordedRefundAmount,
					netpaid          = actualTotalPaid,
					balance          = displayBalance,
					subtotal         = displayTotalCharge,
					discountamount   = discountAmount,
					concessionamount = concessionAmount,
					depositcreditadjustment = depositCreditAdjustment,
					currentamountpayable = displayBalance
					,refunddue = refundDue
					,currencycode = billingCurrency.Trim().ToUpperInvariant()
					,amountinwords = CurrencyAmountInWords.Convert(displayBalance, billingCurrency)
				};
				if (!string.IsNullOrEmpty(patientvisitidStr) && Guid.TryParse(patientvisitidStr, out var pvSummaryGuid))
					summary.patientvisitid = pvSummaryGuid;

				decimal amountToReceive = summary.balance > 0 ? summary.balance : 0;
				var billing = new BillingPaymentModel
				{
					amount                      = amountToReceive,
					receivedamount              = amountToReceive,
					craftmyapp_actionmethodname = "Add_Billing_Payment",
					isdeleted                   = false
				};

				if (!string.IsNullOrEmpty(patientvisitidStr)   && Guid.TryParse(patientvisitidStr,   out var pvGuid))
					billing.patientvisit = pvGuid;
				if (!string.IsNullOrEmpty(tenantidStr)          && Guid.TryParse(tenantidStr,          out var tGuid))
					billing.tenantid = tGuid;
				if (!string.IsNullOrEmpty(patientprofileidStr)  && Guid.TryParse(patientprofileidStr,  out var ppGuid))
					billing.patientname = ppGuid;
				if (Guid.TryParse(OPDFormid, out var opdGuid))
					billing.opdnumber = opdGuid;

				ViewBag.FeeRows        = feeRows;
				ViewBag.BillingSummary = summary;
				ViewBag.TotalFee       = totalFee;
				ViewBag.HasConsultationFeeReceivable = true;

				return View(billing);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "OPD_Consultation_Fee error: " + ex.Message);
				TempData["errMessage"] = "Error loading consultation fee";
				return RedirectToAction("Approved_OPD_Forms");
			}
		}

		[HttpPost()]
		public virtual async Task<IActionResult> Add_OPD_Manual_Receivable([FromBody] ReceivableModel model)
		{
			try
			{
				var userId = HttpContext.Session.GetString("NalamVazhaloginUserID");
				var userRole = HttpContext.Session.GetString("NalamVazhauserrole") ?? "";
				if (string.IsNullOrWhiteSpace(userId))
					return Unauthorized("Session Expired");
				if (!userRole.Equals("Frontdesk Admin", StringComparison.OrdinalIgnoreCase))
					return StatusCode(StatusCodes.Status403Forbidden, "Only Front Desk can add an OPD receivable.");
				if (model?.opdnumber == null || model.opdnumber == Guid.Empty)
					return BadRequest("OPDFormid is required.");

				var eligibilityJson = await ApiClient.Get_ApiValues(getHttpClient(),
					"api/OPDForm/Get_OPD_Action_Eligibility?OPDFormid=" + model.opdnumber +
					"&userrole=" + Uri.EscapeDataString(userRole) +
					"&loginUserID=" + Uri.EscapeDataString(userId));
				var eligibilityRows = !string.IsNullOrWhiteSpace(eligibilityJson) && eligibilityJson.Length > 2
					? JsonConvert.DeserializeObject<JArray>(eligibilityJson)
					: null;
				if (eligibilityRows?.FirstOrDefault()?["canaddreceivable"]?.Value<bool>() != true)
					return Conflict("A receivable cannot be added at the current OPD stage.");

				model.craftmyapp_actionmethodname = "Add_Receivable";
				model.createduser = Guid.Parse(userId);
				var apiResult = await ApiClient.Post_ApiValuesGetRawString(
					getHttpClient(),
					"api/OPDForm/Add_OPD_Manual_Receivable",
					model);
				if (string.IsNullOrWhiteSpace(apiResult))
					return BadRequest("Receivable could not be created.");
				try
				{
					var resultObject = JObject.Parse(apiResult);
					if (string.Equals(resultObject["message"]?.ToString(), "201.1", StringComparison.OrdinalIgnoreCase))
						return Ok(new { message = "201.1" });
				}
				catch { }
				return BadRequest(apiResult.Replace("\"", ""));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Add_OPD_Manual_Receivable failed.");
				return BadRequest(ex.Message);
			}
		}

		private static string CleanOPDDisplayValue(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
				return "";

			value = value.Trim();
			return value == "-" ? "" : value;
		}

		private async Task<string> ResolveOPDPatientHeaderName(string patientProfileId, string fallbackName, string loginUserId)
		{
			var fallback = CleanOPDDisplayValue(fallbackName);
			if (string.IsNullOrWhiteSpace(patientProfileId) || !Guid.TryParse(patientProfileId, out var patientGuid) || patientGuid == Guid.Empty)
				return string.IsNullOrWhiteSpace(fallback) ? "-" : fallback;

			try
			{
				var patientJson = await ApiClient.Get_ApiValues(getHttpClient(),
					"api/PatientProfile/getById_PatientProfile?PatientProfileid=" + patientGuid + "&loginUserID=" + loginUserId);
				if (!string.IsNullOrWhiteSpace(patientJson) && patientJson.Length > 2)
				{
					var patient = JsonConvert.DeserializeObject<PatientProfileModel>(patientJson);
					var header = string.Join(" ", new[]
					{
						CleanOPDDisplayValue(patient?.firstname),
						CleanOPDDisplayValue(patient?.lastname),
						CleanOPDDisplayValue(patient?.registrationid)
					}.Where(part => !string.IsNullOrWhiteSpace(part)));
					if (!string.IsNullOrWhiteSpace(header))
						return header;
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "ResolveOPDPatientHeaderName error: " + ex.Message);
			}

			return string.IsNullOrWhiteSpace(fallback) ? "-" : fallback;
		}

		private async Task<string> ResolveOPDConsultingDoctorName(string OPDFormid, string loginUserId)
		{
			if (string.IsNullOrWhiteSpace(OPDFormid))
				return "";

			try
			{
				var infoJson = await ApiClient.Get_ApiValues(getHttpClient(),
					"api/OPDForm/getById_allinfo_OPDForm?OPDFormid=" + OPDFormid + "&loginUserID=" + loginUserId);
				if (!string.IsNullOrWhiteSpace(infoJson) && infoJson.Length > 2)
				{
					var infoRows = JsonConvert.DeserializeObject<JArray>(infoJson);
					var infoRow = infoRows?.FirstOrDefault();
					var infoDoctorName = CleanOPDDisplayValue(infoRow?["preferreddoctor_master"]?.ToString());
					if (string.IsNullOrWhiteSpace(infoDoctorName))
						infoDoctorName = CleanOPDDisplayValue(infoRow?["preferreddoctor"]?.ToString());
					if (!string.IsNullOrWhiteSpace(infoDoctorName))
						return infoDoctorName;
				}

				var appointmentJson = await ApiClient.Get_ApiValues(getHttpClient(),
					"api/OPDForm/Get_Appointment_By_OPDForm?OPDFormid=" + OPDFormid + "&loginUserID=" + loginUserId);
				if (!string.IsNullOrWhiteSpace(appointmentJson) && appointmentJson.Length > 2)
				{
					var appointmentRows = JsonConvert.DeserializeObject<JArray>(appointmentJson);
					var appointment = appointmentRows?.FirstOrDefault();
					var appointmentDoctorName = CleanOPDDisplayValue(appointment?["practitionername"]?.ToString());
					if (string.IsNullOrWhiteSpace(appointmentDoctorName))
						appointmentDoctorName = CleanOPDDisplayValue(appointment?["PractitionerName"]?.ToString());
					if (string.IsNullOrWhiteSpace(appointmentDoctorName))
						appointmentDoctorName = CleanOPDDisplayValue(appointment?["actualpractitioner"]?.ToString());
					if (string.IsNullOrWhiteSpace(appointmentDoctorName))
						appointmentDoctorName = CleanOPDDisplayValue(appointment?["ActualPractitioner"]?.ToString());
					if (!string.IsNullOrWhiteSpace(appointmentDoctorName))
						return appointmentDoctorName;
				}
			}
			catch (Exception ex)
			{
				_logger?.LogError(ex, "ResolveOPDConsultingDoctorName failed for OPD {OPDFormid}: {Message}", OPDFormid, ex.Message);
			}

			return "";
		}
		[HttpGet()]
		public virtual async Task<IActionResult> Get_OPDForm_Doctor(string OPDFormid)
		{
			if (string.IsNullOrEmpty(OPDFormid)) return Json(null);
			var loginUserId = HttpContext.Session.GetString("NalamVazhaloginUserID");
			var opdJson = await ApiClient.Get_ApiValues(getHttpClient(),
				"api/OPDForm/getById_OPDForm?OPDFormid=" + OPDFormid + "&loginUserID=" + loginUserId);
			if (string.IsNullOrEmpty(opdJson) || opdJson.Length <= 2) return Json(null);
			var opdForm = JsonConvert.DeserializeObject<OPDFormModel>(opdJson);
			return Json(new { peopleid = opdForm?.preferreddoctor?.ToString() });
		}

		[HttpPost()]
		public virtual async Task<string> Update_Appointment_Status([FromBody] AppointmentStatusUpdateModel model)
		{
			string message = "";
			try
			{
				message = await ApiClient.Post_ApiValuesGetString(getHttpClient(), "api/OPDForm/Update_Appointment_Status", model);
				if (message.Replace("\"", "") == "201.1")
				{
					TempData["message"] = "Success";

				}
				else
				{
					TempData["errMessage"] = message.Replace("\"", "");
				}

				message = message.Replace("\"", "");



			}
			catch (Exception ex)
			{

				_logger.LogError(ex, "An exception occurred in - OPDForm / verify_OPDForm, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

				TempData["errMessage"] = ex.Message;
				message = ex.Message;
			}


			return message;
		}
		[HttpGet()]
		public virtual async Task<IActionResult> Cancel_OPD(string OPDFormid, bool triggerrefund = false, bool overpaymentrefund = false)
		{
			if (string.IsNullOrEmpty(OPDFormid))
			{
				TempData["errMessage"] = "Invalid request";
				return RedirectToAction("Approved_OPD_Forms");
			}
			try
			{
				var loginUserId = HttpContext.Session.GetString("NalamVazhaloginUserID");
				var infoJson = await ApiClient.Get_ApiValues(getHttpClient(),
					"api/OPDForm/getById_allinfo_OPDForm?OPDFormid=" + OPDFormid + "&loginUserID=" + loginUserId);

				if (!string.IsNullOrWhiteSpace(infoJson) && infoJson.Length > 2)
				{
					var rows = Newtonsoft.Json.JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JArray>(infoJson);
					var row  = rows?[0];
					if (row != null)
					{
						ViewBag.PatientName      = row["patientname"]?.ToString() ?? "-";
						ViewBag.BookingRef       = row["bookingreferencenumber"]?.ToString() ?? "-";
						ViewBag.PreferredDoctor  = row["preferreddoctor"]?.ToString() ?? "-";
						ViewBag.VerifiedStatus   = row["verifiedstatus"]?.ToString() ?? "-";
						ViewBag.PatientProfileId = row["patientid"]?.ToString();
						ViewBag.TenantId         = row["tenantid"]?.ToString();
					}
				}

				// Fetch payment rows to determine total paid and whether RazorPay was used
				decimal totalPaid = 0;
				bool hasRazorPayPayment = false;
				try
				{
					var payRowsJson = await ApiClient.Get_ApiValues(getHttpClient(),
						"api/BillingPayment/Get_Billing_Payments_For_OPD?OPDFormid=" + OPDFormid);
					if (!string.IsNullOrEmpty(payRowsJson) && payRowsJson.Length > 2)
					{
						var payRows = Newtonsoft.Json.JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JArray>(payRowsJson);
						if (payRows != null)
						{
							foreach (var pr in payRows)
							{
								var mode = pr["paymentmode"]?.ToString() ?? "";
								var amt  = pr["amount"]?.ToString() ?? "0";
								var txn = pr["transactionreference"]?.ToString() ?? "";
								if (decimal.TryParse(amt, System.Globalization.NumberStyles.Any,
									System.Globalization.CultureInfo.InvariantCulture, out var rowAmt) && rowAmt > 0)
								{
									totalPaid += rowAmt;
									if (mode.IndexOf("Online", StringComparison.OrdinalIgnoreCase) >= 0
										|| mode.IndexOf("Razor", StringComparison.OrdinalIgnoreCase) >= 0
										|| txn.StartsWith("pay_", StringComparison.OrdinalIgnoreCase))
										hasRazorPayPayment = true;
								}
							}
						}
					}
				}
				catch (Exception ex) { _logger.LogWarning(ex, "Cancel_OPD – billing summary: " + ex.Message); }

				var sessionRole      = HttpContext.Session.GetString("NalamVazhauserrole") ?? "";
				var isHealthSeeker   = sessionRole.Equals("Health Seeker",   StringComparison.OrdinalIgnoreCase);
				var isFrontDeskAdmin = sessionRole.Equals("Frontdesk Admin", StringComparison.OrdinalIgnoreCase);
				var isRefundOnly = triggerrefund && isFrontDeskAdmin;
				var eligibilityJson = await ApiClient.Get_ApiValues(getHttpClient(),
					"api/OPDForm/Get_OPD_Action_Eligibility?OPDFormid=" + Uri.EscapeDataString(OPDFormid) +
					"&userrole=" + Uri.EscapeDataString(sessionRole) +
					"&loginUserID=" + Uri.EscapeDataString(loginUserId ?? ""));
				var eligibilityRows = !string.IsNullOrWhiteSpace(eligibilityJson) && eligibilityJson.Length > 2
					? JsonConvert.DeserializeObject<JArray>(eligibilityJson)
					: null;
				if (!isRefundOnly && eligibilityRows?.FirstOrDefault()?["cancancel"]?.Value<bool>() != true)
				{
					TempData["errMessage"] = "This OPD cannot be cancelled by the current user or at its current stage.";
					return isHealthSeeker
						? RedirectToAction("Index", "PatientDashboard")
						: RedirectToAction("Dashboard", "FrontDesk");
				}

				var quoteJson = await ApiClient.Get_ApiValues(getHttpClient(),
					"api/OPDForm/Get_OPD_Cancellation_Quote?OPDFormid=" + Uri.EscapeDataString(OPDFormid) +
					"&loginUserID=" + Uri.EscapeDataString(loginUserId ?? ""));
				var quoteRows = !string.IsNullOrWhiteSpace(quoteJson) && quoteJson.Length > 2
					? JsonConvert.DeserializeObject<JArray>(quoteJson)
					: null;
				var quote = quoteRows?.FirstOrDefault();
				decimal ReadQuoteDecimal(string name) =>
					decimal.TryParse(quote?[name]?.ToString(), System.Globalization.NumberStyles.Any,
						System.Globalization.CultureInfo.InvariantCulture, out var parsed) ? parsed : 0m;

				ViewBag.OPDFormid            = OPDFormid;
				ViewBag.IsHealthSeeker       = isHealthSeeker;
				ViewBag.IsFrontDeskAdmin     = isFrontDeskAdmin;
				ViewBag.CancellationBy       = isHealthSeeker ? "Patient" : "FrontDesk";
				ViewBag.GrossCharge          = ReadQuoteDecimal("grosscharge");
				ViewBag.DiscountAmount       = ReadQuoteDecimal("discountamount");
				ViewBag.NetPayable           = ReadQuoteDecimal("netpayable");
				ViewBag.TotalPaid            = ReadQuoteDecimal("successfulpaid");
				ViewBag.AlreadyRefunded      = ReadQuoteDecimal("alreadyrefunded");
				ViewBag.Balance              = ReadQuoteDecimal("balance");
				var alreadyRefunded = ReadQuoteDecimal("alreadyrefunded");
				var totalPaidFromQuote = ReadQuoteDecimal("successfulpaid");
				var remainingPaidAmount = Math.Max(0m, totalPaidFromQuote - alreadyRefunded);
				var overpaymentAmount = Math.Max(0m, totalPaidFromQuote - ReadQuoteDecimal("netpayable") - alreadyRefunded);
				// Cancellation and the later Trigger Refund screen must quote the same
				// remaining paid amount.  Previously the cancellation screen used the
				// quote's overpayment-only value, so a fully paid consultation displayed
				// Rs. 0 even though Trigger Refund correctly displayed the payment.
				var refundableAmount = overpaymentrefund
					? Math.Min(remainingPaidAmount, overpaymentAmount)
					: remainingPaidAmount;
				ViewBag.RefundableAmount     = refundableAmount;
				ViewBag.OnlineRefundableAmount = Math.Min(ReadQuoteDecimal("onlinerefundableamount"), refundableAmount);
				ViewBag.HasRazorPayPayment   = quote?["hasrazorpaypayment"]?.Value<bool>() ?? hasRazorPayPayment;
				ViewBag.IsRefundOnly         = isRefundOnly;
				ViewBag.IsOverpaymentRefund  = overpaymentrefund;
				return View();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Cancel_OPD error: " + ex.Message);
				TempData["errMessage"] = "Error loading cancellation page";
				return RedirectToAction("Approved_OPD_Forms");
			}
		}

		[HttpPost()]
		public virtual async Task<IActionResult> Cancel_OPD_Booking([FromBody] OPDCancellationRequest model)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(model?.OPDFormid))
					return BadRequest("OPDFormid is required");

				var loginUserId = HttpContext.Session.GetString("NalamVazhaloginUserID");
				var sessionRole = HttpContext.Session.GetString("NalamVazhauserrole") ?? "";
				var isRefundOnly = model.triggerrefund && sessionRole.Equals("Frontdesk Admin", StringComparison.OrdinalIgnoreCase);
				if (string.IsNullOrWhiteSpace(loginUserId))
					return Unauthorized("Session Expired");

				var eligibilityJson = await ApiClient.Get_ApiValues(getHttpClient(),
					"api/OPDForm/Get_OPD_Action_Eligibility?OPDFormid=" + Uri.EscapeDataString(model.OPDFormid) +
					"&userrole=" + Uri.EscapeDataString(sessionRole) +
					"&loginUserID=" + Uri.EscapeDataString(loginUserId));
				var eligibilityRows = !string.IsNullOrWhiteSpace(eligibilityJson) && eligibilityJson.Length > 2
					? JsonConvert.DeserializeObject<JArray>(eligibilityJson)
					: null;
				if (!isRefundOnly && eligibilityRows?.FirstOrDefault()?["cancancel"]?.Value<bool>() != true)
					return StatusCode(StatusCodes.Status403Forbidden, "This OPD cannot be cancelled by the current user or at its current stage.");

				var quoteJson = await ApiClient.Get_ApiValues(getHttpClient(),
					"api/OPDForm/Get_OPD_Cancellation_Quote?OPDFormid=" + Uri.EscapeDataString(model.OPDFormid) +
					"&loginUserID=" + Uri.EscapeDataString(loginUserId));
				var quoteRows = !string.IsNullOrWhiteSpace(quoteJson) && quoteJson.Length > 2
					? JsonConvert.DeserializeObject<JArray>(quoteJson)
					: null;
				var quote = quoteRows?.FirstOrDefault();
				if (quote == null)
					return BadRequest("Unable to calculate the cancellation amount. Please refresh and try again.");
				decimal.TryParse(quote["refundableamount"]?.ToString(), System.Globalization.NumberStyles.Any,
					System.Globalization.CultureInfo.InvariantCulture, out var availableRefundAmount);
				decimal.TryParse(quote["onlinerefundableamount"]?.ToString(), System.Globalization.NumberStyles.Any,
					System.Globalization.CultureInfo.InvariantCulture, out var onlineRefundableAmount);
				decimal.TryParse(quote["successfulpaid"]?.ToString(), System.Globalization.NumberStyles.Any,
					System.Globalization.CultureInfo.InvariantCulture, out var successfulPaidAmount);
				decimal.TryParse(quote["alreadyrefunded"]?.ToString(), System.Globalization.NumberStyles.Any,
					System.Globalization.CultureInfo.InvariantCulture, out var alreadyRefundedAmount);
				var remainingPaidAmount = Math.Max(0m, successfulPaidAmount - alreadyRefundedAmount);
				decimal.TryParse(quote["netpayable"]?.ToString(), System.Globalization.NumberStyles.Any,
					System.Globalization.CultureInfo.InvariantCulture, out var quoteNetPayable);
				var overpaymentAmount = Math.Max(0m, successfulPaidAmount - quoteNetPayable - alreadyRefundedAmount);
				// Keep POST validation identical to the amount shown by Cancel_OPD.
				availableRefundAmount = model.overpaymentrefund
					? Math.Min(remainingPaidAmount, overpaymentAmount)
					: remainingPaidAmount;
				onlineRefundableAmount = Math.Min(onlineRefundableAmount, availableRefundAmount);

				decimal refundRecorded = 0m;
				string refundReference = "";
				string refundStatus = "";
				bool isRazorPayRefund = string.Equals(model.refundmode, "RazorPay", StringComparison.OrdinalIgnoreCase);
				if (sessionRole.Equals("Frontdesk Admin", StringComparison.OrdinalIgnoreCase)
					&& availableRefundAmount > 0
					&& string.IsNullOrWhiteSpace(model.refundmode))
					return BadRequest("Refund mode is required because a refundable amount is available.");
				if (!string.IsNullOrWhiteSpace(model.refundmode) && (!model.manualrefundamount.HasValue || model.manualrefundamount.Value <= 0))
					return BadRequest("Refund amount must be greater than zero.");
				if (!string.IsNullOrWhiteSpace(model.refundmode) && model.manualrefundamount.HasValue)
				{
					if (model.manualrefundamount.Value > availableRefundAmount)
						return BadRequest($"Refund amount Rs. {model.manualrefundamount.Value:N2} cannot exceed the refundable amount Rs. {availableRefundAmount:N2}.");
					if (isRazorPayRefund && model.manualrefundamount.Value > onlineRefundableAmount)
						return BadRequest($"Razorpay refund amount Rs. {model.manualrefundamount.Value:N2} cannot exceed the online refundable amount Rs. {onlineRefundableAmount:N2}.");
				}

				// 1. Mark OPD form as Cancelled (using dedicated cancel endpoint that bypasses the
				//    "already reviewed" guard in verify_OPDForm)
				if (isRazorPayRefund)
				{
					var refundResultRaw = await ApiClient.Post_ApiValuesGetRawString(
						getHttpClient(),
						"api/OPDForm/Process_OPD_Razorpay_Refund",
						new
						{
							OPDFormid = model.OPDFormid,
							tenantid = model.tenantid,
							refundamount = model.manualrefundamount.Value,
							cancellationreason = model.cancellationreason
						});
					Newtonsoft.Json.Linq.JObject refundResult;
					try
					{
						refundResult = Newtonsoft.Json.Linq.JObject.Parse(string.IsNullOrWhiteSpace(refundResultRaw) ? "{}" : refundResultRaw);
					}
					catch
					{
						return BadRequest(string.IsNullOrWhiteSpace(refundResultRaw) ? "Razorpay refund failed." : refundResultRaw);
					}
					if (!string.Equals(refundResult["message"]?.ToString()?.Replace("\"", ""), "201.1", StringComparison.OrdinalIgnoreCase))
						return BadRequest(refundResult["message"]?.ToString() ?? refundResult["error"]?.ToString() ?? "Razorpay refund failed.");
					refundReference = refundResult["refundid"]?.ToString() ?? "";
					refundStatus = refundResult["refundstatus"]?.ToString() ?? "pending";
				}

				if (!isRefundOnly)
				{
				var cancelModel = new NalamVazha.Models.OPDFormReviewModel
				{
					OPDFormid      = model.OPDFormid,
					verifiedstatus = "Cancelled",
					verifiedby     = loginUserId,
					reviewcomments = model.cancellationreason
				};
				var opdResult = await ApiClient.Post_ApiValuesGetString(getHttpClient(),
					"api/OPDForm/Cancel_OPD_Direct", cancelModel);

				if (!opdResult.Replace("\"", "").Contains("201.1"))
					return Json(new { message = opdResult.Replace("\"", "") });

				// 2. Find and cancel the linked clinical appointment
				var apptJson = await ApiClient.Get_ApiValues(getHttpClient(),
					"api/OPDForm/Get_Appointment_By_OPDForm?OPDFormid=" + model.OPDFormid);

				if (!string.IsNullOrWhiteSpace(apptJson) && apptJson.Length > 2)
				{
					try
					{
						var apptRows = Newtonsoft.Json.JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JArray>(apptJson);
						var appt     = apptRows?[0];
						var apptId   = appt?["ClinicalAppointmentid"]?.ToString()
						            ?? appt?["clinicalappointmentid"]?.ToString();
						if (!string.IsNullOrWhiteSpace(apptId) && Guid.TryParse(apptId, out var apptGuid))
						{
							var statusModel = new NalamVazha.Models.AppointmentStatusUpdateModel
							{
								ClinicalAppointmentid = apptGuid,
								status = "Cancelled"
							};
							await ApiClient.Post_ApiValuesGetString(getHttpClient(),
								"api/OPDForm/Update_Appointment_Status", statusModel);
						}
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, "Cancel_OPD – failed to cancel linked appointment: " + ex.Message);
					}
				}
				}

				// 3. Record refund entry in BillingPayment (FrontDesk only, when refund mode is provided)
				if (!string.IsNullOrWhiteSpace(model.refundmode) && model.manualrefundamount.HasValue && model.manualrefundamount.Value > 0)
				{
					try
					{
						var refundBilling = new NalamVazha.Models.BillingPaymentModel
						{
							BillingPaymentid            = Guid.NewGuid(),
							tenantid                    = Guid.TryParse(model.tenantid, out var rtid) ? rtid : (Guid?)null,
							patientname                 = Guid.TryParse(model.patientprofileid, out var rpid) ? rpid : (Guid?)null,
							opdnumber                   = Guid.TryParse(model.OPDFormid, out var roid) ? roid : (Guid?)null,
							receivablefor               = "Cancellation Refund",
							paymentdate                 = DateTime.Today,
							amount                      = -model.manualrefundamount.Value,
							receivedamount              = model.manualrefundamount.Value,
							currency                    = "INR",
							conversionrate              = 1,
							paymentmode                 = model.refundmode,
							transactionreference        = string.IsNullOrWhiteSpace(refundReference) ? null : refundReference,
							paymentstatus               = isRazorPayRefund ? "Refund Initiated" : "Refunded",
							refundmode                  = model.refundmode,
							refundedamount              = model.manualrefundamount.Value,
							refundreferencenumber       = string.IsNullOrWhiteSpace(refundReference) ? null : refundReference,
							refundstatus                = isRazorPayRefund
								? (string.IsNullOrWhiteSpace(refundStatus) ? "Refund Initiated" : refundStatus)
								: "Refunded",
							remarks                     = isRazorPayRefund
								? "OPD RazorPay Refund ID: " + refundReference + ". Status: " + refundStatus + ". " + model.cancellationreason
								: "OPD Cancellation Refund - " + model.cancellationreason,
							craftmyapp_actionmethodname = "Add_Billing_Payment",
							createduser                 = Guid.TryParse(loginUserId, out var ruid) ? ruid : Guid.Empty,
							isdeleted                   = false
						};
						var refundInsertResult = await ApiClient.Post_ApiValuesGetString(
							getHttpClient(), "api/BillingPayment/Add_Billing_Payment", refundBilling);
						var normalizedRefundInsertResult = (refundInsertResult ?? "").Replace("\"", "").Trim();
						if (!normalizedRefundInsertResult.Contains("201.1", StringComparison.OrdinalIgnoreCase))
						{
							_logger.LogError("OPD refund BillingPayment insert failed for OPD {OPDFormid}. API response: {ApiResponse}",
								model.OPDFormid, refundInsertResult);
							return StatusCode(StatusCodes.Status500InternalServerError, new
							{
								message = "Refund could not be recorded in BillingPayment.",
								error = normalizedRefundInsertResult
							});
						}
						refundRecorded = model.manualrefundamount.Value;
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, "Cancel_OPD - refund BillingPayment insert failed: " + ex.Message);
						return StatusCode(StatusCodes.Status500InternalServerError, new
						{
							message = "Refund could not be recorded in BillingPayment.",
							error = ex.Message
						});
					}
				}

				return Json(new { message = "201.1", refundamount = refundRecorded, refundmode = model.refundmode ?? "", refundid = refundReference, refundstatus = refundStatus });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Cancel_OPD_Booking error: " + ex.Message);
				return BadRequest(ex.Message);
			}
		}

		[AllowAnonymous]
		[HttpGet()]
		public virtual async Task<IActionResult> Get_Cancel_OPD_Payment_History_Data(string OPDFormid)
		{
			try
			{
				var json = await ApiClient.Get_ApiValues(getHttpClient(),
					$"api/BillingPayment/Get_OPD_Billing_Summary_By_OPDForm?OPDFormid={OPDFormid}");
				return Content(json ?? "[]", "application/json");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Get_Cancel_OPD_Payment_History_Data error: " + ex.Message);
				return Content("[]", "application/json");
			}
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
		/// <summary>
		/// Creates a Razorpay order for OPD consultation fee payment.
		/// Called by the Health Seeker from OPD_Consultation_Fee view.
		/// </summary>
		[HttpPost]
		public virtual async Task<IActionResult> Create_OPD_Razorpay_Order([FromBody] OPDRazorpayOrderRequest request)
		{
			try
			{
				var loginUserId = HttpContext.Session.GetString("NalamVazhaloginUserID");
				if (string.IsNullOrWhiteSpace(loginUserId))
					return Json(new { error = "Session Expired" });

				if (request == null || string.IsNullOrWhiteSpace(request.OPDFormid))
					return BadRequest(new { error = "OPDFormid is required" });

				var result = await ApiClient.Post_ApiValuesGetRawString(
					getHttpClient(),
					"api/OPDForm/Create_OPD_Razorpay_Order",
					new { OPDFormid = request.OPDFormid, loginUserID = loginUserId });
				if (string.IsNullOrWhiteSpace(result))
					return Json(new { error = "Failed to create order" });

				// API returns a JSON object — pass it straight to the client
				var orderObj = JsonConvert.DeserializeObject<object>(result);
				return Json(orderObj);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Create_OPD_Razorpay_Order failed: " + ex.Message);
				return Json(new { error = ex.Message });
			}
		}

		/// <summary>
		/// Completes the OPD Razorpay payment after the Razorpay handler callback.
		/// </summary>
		[HttpPost]
		public virtual async Task<IActionResult> Complete_OPD_Payment([FromBody] OPDCompletePaymentRequest request)
		{
			try
			{
				var loginUserId = HttpContext.Session.GetString("NalamVazhaloginUserID");
				if (string.IsNullOrWhiteSpace(loginUserId))
					return Json(new { message = "Session Expired" });

				if (request == null || string.IsNullOrWhiteSpace(request.OPDFormid))
					return Json(new { message = "OPDFormid is required" });

				request.loginUserID = loginUserId;

				var result = await ApiClient.Post_ApiValuesGetRawString(
					getHttpClient(),
					"api/OPDForm/Complete_OPD_Payment",
					request);

				var paymentResult = JsonConvert.DeserializeObject<JObject>(result ?? "{}");
				if (paymentResult != null
					&& (paymentResult["message"]?.ToString() == "201.1" || paymentResult["success"]?.Value<bool>() == true)
					&& paymentResult["duplicate"]?.Value<bool>() != true)
				{
					await SendOPDWorkflowDoctorEmail(request.OPDFormid, true);
				}
				var resultObj = paymentResult ?? JsonConvert.DeserializeObject<JObject>("{}");
				return Json(resultObj);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Complete_OPD_Payment failed: " + ex.Message);
				return Json(new { message = ex.Message });
			}
		}

		[HttpPost]
		public virtual async Task<IActionResult> Fail_OPD_Payment([FromBody] OPDFailedPaymentRequest request)
		{
			try
			{
				var loginUserId = HttpContext.Session.GetString("NalamVazhaloginUserID");
				if (string.IsNullOrWhiteSpace(loginUserId))
					return Json(new { message = "Session Expired" });

				if (request == null || string.IsNullOrWhiteSpace(request.paymentrequestid))
					return BadRequest(new { message = "paymentrequestid is required" });

				var result = await ApiClient.Post_ApiValuesGetRawString(
					getHttpClient(),
					"api/OPDForm/Fail_OPD_Payment",
					request);

				var resultObj = JsonConvert.DeserializeObject<object>(result ?? "{}");
				return Json(resultObj);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Fail_OPD_Payment failed: " + ex.Message);
				return Json(new { message = ex.Message });
			}
		}
	}   // end OPDFormController


	// ---- Request / response models for OPD Razorpay -------------------------

	/// <summary>Payload sent by the browser to Create_OPD_Razorpay_Order.</summary>
	public class OPDRazorpayOrderRequest
	{
		public string OPDFormid { get; set; }
	}

	public class OPDCompletePaymentRequest
	{
		public string OPDFormid { get; set; }
		public string paymentrequestid { get; set; }
		public string razorpay_order_id { get; set; }
		public string razorpay_payment_id { get; set; }
		public string razorpay_signature { get; set; }
		public decimal? amount { get; set; }
		public string loginUserID { get; set; } // set server-side
	}

	public class OPDFailedPaymentRequest
	{
		public string OPDFormid { get; set; }
		public string paymentrequestid { get; set; }
		public string razorpay_order_id { get; set; }
		public string razorpay_payment_id { get; set; }
		public string error_code { get; set; }
		public string error_description { get; set; }
		public string error_source { get; set; }
		public string error_step { get; set; }
		public string error_reason { get; set; }
	}

		public class OPDCancellationRequest
		{
			public bool triggerrefund { get; set; }
			public bool overpaymentrefund { get; set; }
		public string OPDFormid { get; set; }
		public string cancellationreason { get; set; }
		public string cancellationby { get; set; }
		public string refundmode { get; set; }
		public decimal? manualrefundamount { get; set; }
		public string patientprofileid { get; set; }
		public string tenantid { get; set; }
	}

}
