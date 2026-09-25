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

                
                
                
				//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:35
				
                
                
                
                
				public class ClinicalAppointmentController : BaseController
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
                    private readonly ILogger<ClinicalAppointmentController> _logger;
                    
                    
                    StorageUtil util;
					public ClinicalAppointmentController(IConfiguration configuration,IHttpContextAccessor accessor,IOptions<ApiSettings> ApiSettings, IOptions<MailSettings> MailSettings, IWebHostEnvironment env, ILogger<ClinicalAppointmentController> logger):base( configuration)
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

        public IActionResult Clinical_Slot(string isDashboardView = "Y")
        {
            ViewBag.Title = "Doctor Slot Calendar";
            ViewBag.IsDashboardView = isDashboardView;

            return View();
        }

        [HttpGet()]
        public virtual async Task<string>
    Get_Clinical_Slot_Data(
        string fromdate,
        string todate)
        {
            try
            {
                var tenantid =
                    HttpContext.Session.GetString(
                        "NalamVazhachoosedtenantid"
                    );

                if (
                    string.IsNullOrWhiteSpace(
                        tenantid
                    )
                )
                {
                    tenantid =
                        HttpContext.Session.GetString(
                            "NalamVazhatenantid"
                        );
                }
                var loginUserID =
                    HttpContext.Session.GetString(
                        "NalamVazhaloginUserID"
                    );

                return await ApiClient
                    .Get_ApiValues(
                        getHttpClient(),

                        "api/ClinicalAppointment/" +
                        "Get_Clinical_Slot_Data" +

                        "?tenantid=" +
                        Uri.EscapeDataString(
                            tenantid ?? ""
                        ) +

                        "&fromdate=" +
                        Uri.EscapeDataString(
                            fromdate ?? ""
                        ) +

                        "&todate=" +
                        Uri.EscapeDataString(
                            todate ?? ""
                        ) +

                        "&loginUserID=" +
                        Uri.EscapeDataString(
                            loginUserID ?? ""
                        )
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Clinical Slot data loading failed."
                );

                return "[]";
            }
        }

        public virtual async Task<string> getById_reshedulehistory(string ClinicalAppointmentid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/ClinicalAppointment/getById_reshedulehistory?ClinicalAppointmentid="+ClinicalAppointmentid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
					 
			  }


		public virtual IActionResult Add_Clinical_Appointment(
string appointmentdate = null,
string durationfrom = null,
string durationto = null,
string practitioner = null,
string patient = null,
string status = null,
string origin = null,
string tenantid = null,
string opdformid = null,
string tasktype = null,
string popup = null
)
		{
			ViewBag.AppointmentDate = appointmentdate;
			ViewBag.DurationFrom = durationfrom;
			ViewBag.DurationTo = durationto;
			ViewBag.Practitioner = practitioner;

			ViewBag.Patient = patient;
			ViewBag.Status = status;
			ViewBag.Origin = origin;
			ViewBag.TenantId = tenantid;
			ViewBag.OPDFormId = opdformid;
			ViewBag.TaskType = tasktype;

			ViewBag.UsePopupLayout = (popup == "1" || string.Equals(popup, "true", StringComparison.OrdinalIgnoreCase));

			// Return a model so Add_Clinical_Appointment view can pre-select tasktype dropdown.
			return View(new ClinicalAppointmentModel { tasktype = tasktype });
		}
		[HttpPost()]
			public virtual async Task<string> Add_Clinical_Appointment(ClinicalAppointmentModel model, IFormCollection collection)
			{
				string strReturnMessage = "";
				
				try
				{
					ModelState.Remove("ClinicalAppointmentid");
					ModelState.Remove("createduser");
                    ModelState.Remove("craftmyapp_actionmethodname");
                    model.craftmyapp_actionmethodname="Add_Clinical_Appointment";
					if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
								model.createduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
								else
								return "Session Expired";                    
					ModelState.Remove("photo");

                   
					
			 	    
					 if (ModelState.IsValid)
					 {
							 ClinicalAppointmentModelValidator validator = new ClinicalAppointmentModelValidator();
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
								 model.ClinicalAppointmentid =Guid.NewGuid(); 
                                 model.photo = collection["photo_existing"];

                                  var files = Request.Form.Files;
foreach (var file in files) 
{
var filename = ContentDispositionHeaderValue
.Parse(file.ContentDisposition)
.FileName
.Trim('"'); 
string fileExtention = "." + filename.Split('.').Last(); 
Random rnd = new Random();
string uploadFileName = System.Text.RegularExpressions.Regex.Replace(filename.Split('.').First(), @"[^0-9a-zA-Z_.]+", "").Replace(" ", String.Empty)+"_"+"ClinicalAppointment_" +rnd.Next(1, 10000).ToString() + DateTime.Now.ToString("ddMMyyHHmmss")+ fileExtention;
if (fileExtention != ".")
{
Stream stream = file.OpenReadStream();
string fileURL=await util.fileSystem.UploadFileAsync(stream, Configuration.GetSection("AzureBlobStorageSetttings:folderName").Value, uploadFileName);
if (file.Name == "photo")
{
uploadFileName = "|" + fileURL +"|";
model.photo +=  uploadFileName ;
}
}
}

								 
                                 strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/ClinicalAppointment/Add_Clinical_Appointment", model);
                                    
								 
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
                 
                 _logger.LogError(ex,"An exception occurred in - ClinicalAppointment / Add_Clinical_Appointment, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
               
				 strReturnMessage = ex.Message;
			 }
		     ViewData["message"] = strReturnMessage;
			 if(strReturnMessage.Replace("\"", "").Contains("201.1")){
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

				
			  public virtual async Task<IActionResult> Reschedule_Appointment(string ClinicalAppointmentid)
			  {

                    string redirectTo="";
                    if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
                            DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
                            DataView dv = new DataView(NalamVazharole_JSON);
                            dv.RowFilter = "controllername='ClinicalAppointment' AND viewname='list'";

                            if(dv.Count  >0){
                                redirectTo = dv[0]["actionmethodname"] as string;
							 
                            }

                            try{
                                     var jsonObjClinicalAppointment = await ApiClient.Get_ApiValues(getHttpClient(), "api/ClinicalAppointment/getById_ClinicalAppointment?ClinicalAppointmentid="+ClinicalAppointmentid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                                if(jsonObjClinicalAppointment.Length > 2)
                                {
                                  
                                    var model = JsonConvert.DeserializeObject<ClinicalAppointmentModel>(jsonObjClinicalAppointment);


                
                                     
                                    return View(model);
                                }
                                else
                                {
                    
                                    TempData["message"] = "Data Not Found - Contact Administrator";
                                    return RedirectToAction(redirectTo);
						 
                                }

                            }catch(Exception ex){
                               _logger.LogError(ex,"An exception occurred in - ClinicalAppointment / Reschedule_Appointment, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                                TempData["errMessage"] = "Error while fetching data - Contact Administrator";
                                return RedirectToAction(redirectTo);
                            }

                    }
                    TempData["errMessage"] = "Session Expired";
                    return RedirectToAction("Logout", "users");
                }	
			  [HttpPost()]
				public virtual async Task<string> Reschedule_Appointment(ClinicalAppointmentModel model, IFormCollection collection)
				{
					string strReturnMessage = "";
					try
					{
							ModelState.Remove("ClinicalAppointmentid");
                            ModelState.Remove("craftmyapp_actionmethodname");
                             model.craftmyapp_actionmethodname="Reschedule_Appointment";
							
							
							if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
					model.modifieduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
					else
					return "Session Expired";
							
                            model.photo = collection["photo_existing"];

                            ModelState.Remove("photo");

							if (ModelState.IsValid)
							{
									ClinicalAppointmentModelValidator validator = new ClinicalAppointmentModelValidator();
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
string uploadFileName = System.Text.RegularExpressions.Regex.Replace(filename.Split('.').First(), @"[^0-9a-zA-Z_.]+", "").Replace(" ", String.Empty)+"_"+"ClinicalAppointment_" +rnd.Next(1, 10000).ToString() + DateTime.Now.ToString("ddMMyyHHmmss")+ fileExtention;
if (fileExtention != ".")
{
Stream stream = file.OpenReadStream();
string fileURL=await util.fileSystem.UploadFileAsync(stream, Configuration.GetSection("AzureBlobStorageSetttings:folderName").Value, uploadFileName);
if (file.Name == "photo")
{
uploadFileName = "|" + fileURL +"|";
model.photo +=  uploadFileName ;
}
}
}

                                        
                                        
                                        strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/ClinicalAppointment/Reschedule_Appointment", model);
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
                      _logger.LogError(ex,"An exception occurred in - ClinicalAppointment / Reschedule_Appointment, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
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

				
			  public virtual async Task<IActionResult> Update_Clinical_Appointment(string ClinicalAppointmentid)
			  {

                    string redirectTo="";
                    if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
                            DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
                            DataView dv = new DataView(NalamVazharole_JSON);
                            dv.RowFilter = "controllername='ClinicalAppointment' AND viewname='list'";

                            if(dv.Count  >0){
                                redirectTo = dv[0]["actionmethodname"] as string;
							 
                            }

                            try{
                                     var jsonObjClinicalAppointment = await ApiClient.Get_ApiValues(getHttpClient(), "api/ClinicalAppointment/getById_ClinicalAppointment?ClinicalAppointmentid="+ClinicalAppointmentid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                                if(jsonObjClinicalAppointment.Length > 2)
                                {
                                  
                                    var model = JsonConvert.DeserializeObject<ClinicalAppointmentModel>(jsonObjClinicalAppointment);


                
                                     
                                    return View(model);
                                }
                                else
                                {
                    
                                    TempData["message"] = "Data Not Found - Contact Administrator";
                                    return RedirectToAction(redirectTo);
						 
                                }

                            }catch(Exception ex){
                               _logger.LogError(ex,"An exception occurred in - ClinicalAppointment / Update_Clinical_Appointment, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                                TempData["errMessage"] = "Error while fetching data - Contact Administrator";
                                return RedirectToAction(redirectTo);
                            }

                    }
                    TempData["errMessage"] = "Session Expired";
                    return RedirectToAction("Logout", "users");
                }	
			  [HttpPost()]
				public virtual async Task<string> Update_Clinical_Appointment(ClinicalAppointmentModel model, IFormCollection collection)
				{
					string strReturnMessage = "";
					try
					{
							ModelState.Remove("ClinicalAppointmentid");
                            ModelState.Remove("craftmyapp_actionmethodname");
                             model.craftmyapp_actionmethodname="Update_Clinical_Appointment";
							
							
							if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
					model.modifieduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
					else
					return "Session Expired";
							
                            model.photo = collection["photo_existing"];

                            ModelState.Remove("photo");

							if (ModelState.IsValid)
							{
									ClinicalAppointmentModelValidator validator = new ClinicalAppointmentModelValidator();
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
string uploadFileName = System.Text.RegularExpressions.Regex.Replace(filename.Split('.').First(), @"[^0-9a-zA-Z_.]+", "").Replace(" ", String.Empty)+"_"+"ClinicalAppointment_" +rnd.Next(1, 10000).ToString() + DateTime.Now.ToString("ddMMyyHHmmss")+ fileExtention;
if (fileExtention != ".")
{
Stream stream = file.OpenReadStream();
string fileURL=await util.fileSystem.UploadFileAsync(stream, Configuration.GetSection("AzureBlobStorageSetttings:folderName").Value, uploadFileName);
if (file.Name == "photo")
{
uploadFileName = "|" + fileURL +"|";
model.photo +=  uploadFileName ;
}
}
}

                                        
                                        
                                        strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/ClinicalAppointment/Update_Clinical_Appointment", model);
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
                      _logger.LogError(ex,"An exception occurred in - ClinicalAppointment / Update_Clinical_Appointment, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
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
public virtual async Task<IActionResult> Remove_Clinical_Appointment(string ClinicalAppointmentid)
			{
				string message = "";
				try
				{
						message = await ApiClient.Get_ApiValues(getHttpClient(), "api/ClinicalAppointment/Remove_Clinical_Appointment?ClinicalAppointmentid="+ClinicalAppointmentid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
						 if(message.Replace("\"","").Contains("201.1"))
						{
							TempData["message"] = "Success";

						}else{
							TempData["errMessage"] = message.Replace("\"","");
						}
						
				
				
				}
				catch (Exception ex)
				{
                     _logger.LogError(ex,"An exception occurred in - ClinicalAppointment / Remove_Clinical_Appointment, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                
					 TempData["errMessage"] = ex.Message;
					 message = ex.Message;
				}

				string redirectTo="";
						if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
					DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
						 DataView dv = new DataView(NalamVazharole_JSON);
						 dv.RowFilter = "controllername='ClinicalAppointment' AND viewname='list'";

						if(dv.Count  >0){
						    redirectTo = dv[0]["actionmethodname"] as string;
							 
						}

					}
				
				return RedirectToAction(redirectTo);
			}

                                        public virtual IActionResult View_Clinical_Appointment()
                                        {
                                            return View();
                                        }

			        public virtual IActionResult Clinical_Appointment_List()
			        {
				        return View();
			        }
			        	
			        [HttpGet()]
			        public virtual async Task<string> get_Clinical_Appointment_List(string tenantid
,string tasktype
,string patient
,string practitioner
,string appointmentdate_automatonfrom
,string appointmentdate_automatonto
,string status
,string origin
, int? pagesize=100 , int? pagenumber=0,string searchterm="",string sortFieldsJson="")
			        {

				        return await ApiClient.Get_ApiValues(getHttpClient(), "api/ClinicalAppointment/Clinical_Appointment_List?tenantid="+tenantid+"&tasktype="+tasktype+"&patient="+patient+"&practitioner="+practitioner+"&appointmentdate_automatonfrom="+appointmentdate_automatonfrom+"&appointmentdate_automatonto="+appointmentdate_automatonto+"&status="+status+"&origin="+origin+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID")
+ "&pagesize=" + pagesize + "&pagenumber="+ pagenumber + "&searchterm="+ searchterm + "&sort_fields=" + sortFieldsJson);
			        }
			          
											[HttpGet()]
											public virtual async Task<string> get_all_TaskType(string tenantid)
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/ClinicalAppointment/get_all_TaskType?tenantid=" + tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}


		 
		[HttpGet()]
											public virtual async Task<string> get_all_PatientProfile(string tenantid,string searchterm, string pagesize="1000", string pagenumber="1")
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/PatientProfile/get_all_PatientProfile?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											 
											[HttpGet()]
											public virtual async Task<string> get_all_People(string tenantid,string searchterm, string pagesize="1000", string pagenumber="1")
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/People/get_all_People?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											

				
			  public virtual async Task<string> getById_allinfo_ClinicalAppointment(string ClinicalAppointmentid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/ClinicalAppointment/getById_allinfo_ClinicalAppointment?ClinicalAppointmentid="+ClinicalAppointmentid);
					 
			  }
[HttpGet()]
                    public virtual async Task<string> lookup_ClinicalAppointment_patient(String tenantid,string searchterm, int? pagesize, int? pagenumber)
                    {
                        
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/ClinicalAppointment/lookup_ClinicalAppointment_patient?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }
[HttpGet()]
                    public virtual async Task<string> lookup_ClinicalAppointment_practitioner(String tenantid,string searchterm, int? pagesize, int? pagenumber)
                    {
                        
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/ClinicalAppointment/lookup_ClinicalAppointment_practitioner?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }
[HttpGet()]
                    public virtual async Task<string> lookup_ClinicalAppointment_actualpractitioner(String tenantid,string searchterm, int? pagesize, int? pagenumber)
                    {
                        
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/ClinicalAppointment/lookup_ClinicalAppointment_actualpractitioner?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }



[HttpGet()]
			public virtual async Task<string> lookup_change_ClinicalAppointment_practitioner(string Peopleid)
			{
				return await ApiClient.Get_ApiValues(getHttpClient(), "api/ClinicalAppointment/lookup_change_ClinicalAppointment_practitioner?Peopleid="+Peopleid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			}



		[HttpPost]
		[ActionName("UpdateAppointmentStatus")]
		public virtual async Task<IActionResult> UpdateAppointmentStatus([FromBody] UpdateAppointmentStatusRequest request)
		{
			try
			{
				if (HttpContext.Session.GetString("NalamVazhaloginUserID") == null)
					return Json(new { success = false, message = "Session Expired" });

				var isCancellation = string.Equals(request.status, "Cancelled", StringComparison.OrdinalIgnoreCase);
				var cancellationReason = (request.cancellationreason ?? "").Trim();

				var loginUserID = HttpContext.Session.GetString("NalamVazhaloginUserID");
				var httpClient = getHttpClient();

				// Fetch existing record
				var jsonObj = await ApiClient.Get_ApiValues(httpClient,
					"api/ClinicalAppointment/getById_ClinicalAppointment?ClinicalAppointmentid=" + request.ClinicalAppointmentid + "&loginUserID=" + loginUserID);

				if (string.IsNullOrEmpty(jsonObj) || jsonObj.Length <= 2)
					return Json(new { success = false, message = "Appointment not found" });

				var model = JsonConvert.DeserializeObject<ClinicalAppointmentModel>(jsonObj);

				// Preserve the record and update its status. Cancellation reasons are retained
				// in the existing appointment history collection for audit/reporting.
				model.status = request.status;
				model.modifieduser = new Guid(loginUserID);
				model.craftmyapp_actionmethodname = "Update_Clinical_Appointment";
				if (isCancellation && !string.IsNullOrWhiteSpace(cancellationReason))
				{
					model.reshedulehistory ??= new List<ClinicalAppointment_reshedulehistoryModel>();
					model.reshedulehistory.Add(new ClinicalAppointment_reshedulehistoryModel
					{
						resheduleddatetime = DateTime.Now,
						reshedulereason = "Appointment cancelled: " + cancellationReason,
						record_order = model.reshedulehistory.Count,
						craftmyapp_actionmethodname = "Update_Clinical_Appointment"
					});
				}

				var result = await ApiClient.Post_ApiValuesGetString(httpClient,
					isCancellation && !string.IsNullOrWhiteSpace(cancellationReason)
						? "api/ClinicalAppointment/Update_Clinical_Appointment"
						: "api/ClinicalAppointment/UpdateAppointmentStatus", model);

				if (result != null && result.Replace("\"", "").Contains("201.1"))
				{
					if (isCancellation)
					{
						_ = new MailSender().sendNotification(
							"ClinicalAppointment", "AppointmentCancelled",
							request.ClinicalAppointmentid,
							_mailSettings, loginUserID, httpClient,
							tenantid: model.tenantid?.ToString() ?? "");
						_ = new MailSender().sendNotification(
							"ClinicalAppointment", "AppointmentCancelledDoctor",
							request.ClinicalAppointmentid,
							_mailSettings, loginUserID, httpClient,
							tenantid: model.tenantid?.ToString() ?? "");
					}

					return Json(new { success = true });
				}

				return Json(new { success = false, message = result });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - ClinicalAppointment / UpdateAppointmentStatus");
				return Json(new { success = false, message = ex.Message });
			}
		}

		public class UpdateAppointmentStatusRequest
		{
			public string ClinicalAppointmentid { get; set; }
			public string status { get; set; }
			public string cancellationreason { get; set; }
		}
		[HttpGet()]
			public virtual async Task<string> count_of_ClinicalAppointment_tasktype(string tenantid
,string tasktype
,string practitioner
,string appointmentdate_automatonfrom
,string appointmentdate_automatonto
,string status
,string origin
)
			{

				return await ApiClient.Get_ApiValues(getHttpClient(), "api/ClinicalAppointment/count_of_ClinicalAppointment_tasktype?tenantid="+tenantid+"&tasktype="+tasktype+"&practitioner="+practitioner+"&appointmentdate_automatonfrom="+appointmentdate_automatonfrom+"&appointmentdate_automatonto="+appointmentdate_automatonto+"&status="+status+"&origin="+origin+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID")
);
			}

			[HttpPost()]
			public virtual async Task<IActionResult> Reschedule_Clinical_Appointment([FromBody] RescheduleRequest request)
			{
				try
				{
					if (request == null || string.IsNullOrWhiteSpace(request.ClinicalAppointmentid))
						return Json(new { success = false, message = "Appointment is required." });

					if (HttpContext.Session.GetString("NalamVazhaloginUserID") == null)
						return Json(new { success = false, message = "Session Expired" });

					var userRole = (HttpContext.Session.GetString("NalamVazhauserrole") ?? "").Trim();
					var isFrontDesk = userRole.Equals("Frontdesk Admin", StringComparison.OrdinalIgnoreCase)
						|| userRole.Equals("Front Desk Admin", StringComparison.OrdinalIgnoreCase);
					if (!isFrontDesk)
						return Json(new { success = false, message = "Please contact the Front Desk to make changes to this appointment." });

					var loginUserID = HttpContext.Session.GetString("NalamVazhaloginUserID");

					var httpClient = getHttpClient();

					// Fetch existing record
					var jsonObj = await ApiClient.Get_ApiValues(httpClient,
						"api/ClinicalAppointment/getById_ClinicalAppointment?ClinicalAppointmentid=" + request.ClinicalAppointmentid + "&loginUserID=" + loginUserID);

					if (string.IsNullOrEmpty(jsonObj) || jsonObj.Length <= 2)
						return Json(new { success = false, message = "Appointment not found" });

					var model = JsonConvert.DeserializeObject<ClinicalAppointmentModel>(jsonObj);
					if (model == null)
						return Json(new { success = false, message = "Appointment not found" });

					if (IsClinicalAppointmentReassignmentLockedStatus(model.status))
						return Json(new { success = false, message = "Completed or cancelled appointments cannot be reassigned." });

					var originalPractitioner = model.practitioner;
					model.craftmyapp_actionmethodname = "Reschedule_Appointment";
					model.modifieduser = new Guid(loginUserID);

					// Update the existing appointment; this endpoint never creates a second record.
					if (!string.IsNullOrEmpty(request.appointmentdate))
					{
						if (!DateTime.TryParseExact(request.appointmentdate,
							new[] { "yyyy-MM-dd", "dd/MM/yyyy", "d/M/yyyy" },
							CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
							return Json(new { success = false, message = "Invalid appointment date." });

						model.appointmentdate = dt;
					}

					if (model.appointmentdate.Date < DateTime.Today)
						return Json(new { success = false, message = "Past appointment dates are not allowed." });

					if (!string.IsNullOrEmpty(request.durationfrom))
						model.durationfrom = request.durationfrom;
					if (!string.IsNullOrEmpty(request.durationto))
						model.durationto = request.durationto;
					if (!string.IsNullOrEmpty(request.practitioner))
					{
						if (!Guid.TryParse(request.practitioner, out var practitionerId))
							return Json(new { success = false, message = "Invalid doctor." });

						model.practitioner = practitionerId;
						model.actualpractitioner = model.practitioner;
					}

					if (request.requireDoctorChange &&
						(!model.practitioner.HasValue || model.practitioner == originalPractitioner))
						return Json(new { success = false, message = "Please select a different doctor for reassignment." });

					if (!TimeSpan.TryParse(model.durationfrom, out var durationFrom) ||
						!TimeSpan.TryParse(model.durationto, out var durationTo) ||
						durationTo <= durationFrom)
						return Json(new { success = false, message = "Please select a valid appointment slot." });

					if (!request.allowOverlap && await HasClinicalAppointmentOverlap(httpClient, model, loginUserID))
						return Json(new { success = false, message = "This doctor already has an appointment at that time." });

					var doctorChanged = model.practitioner != originalPractitioner;
					if (doctorChanged)
					{
						model.reshedulehistory ??= new List<ClinicalAppointment_reshedulehistoryModel>();
						var nextOrder = model.reshedulehistory
							.Where(item => item?.record_order != null)
							.Select(item => item.record_order.Value)
							.DefaultIfEmpty(-1)
							.Max() + 1;
						var actorName = (HttpContext.Session.GetString("NalamVazhausername") ?? "Front Desk").Trim();
						model.reshedulehistory.Add(new ClinicalAppointment_reshedulehistoryModel
						{
							resheduleddatetime = DateTime.Now,
							reshedulereason = $"Doctor reassigned from {originalPractitioner} to {model.practitioner} by {actorName} ({loginUserID}).",
							record_order = nextOrder,
							craftmyapp_actionmethodname = "Reschedule_Appointment"
						});
					}

					var result = await ApiClient.Post_ApiValuesGetString(httpClient,
						"api/ClinicalAppointment/Reschedule_Appointment", model);

					if (result != null && result.Replace("\"", "").Contains("201.1"))
					{
						// Notify health seeker (patient)
						_ = new MailSender().sendNotification(
							"ClinicalAppointment", "AppointmentRescheduled",
							request.ClinicalAppointmentid,
							_mailSettings, loginUserID, httpClient,
							tenantid: model.tenantid?.ToString() ?? "");

						// Notify doctor
						_ = new MailSender().sendNotification(
							"ClinicalAppointment", "AppointmentRescheduledDoctor",
							request.ClinicalAppointmentid,
							_mailSettings, loginUserID, httpClient,
							tenantid: model.tenantid?.ToString() ?? "");

						return Json(new { success = true });
					}

					return Json(new { success = false, message = result });
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "An exception occurred in - ClinicalAppointment / Reschedule_Clinical_Appointment");
					return Json(new { success = false, message = ex.Message });
				}
			}

			private async Task<bool> HasClinicalAppointmentOverlap(HttpClient httpClient, ClinicalAppointmentModel model, string loginUserID)
			{
				if (model == null || model.practitioner == null)
					return false;

				if (!TimeSpan.TryParse(model.durationfrom, out var targetFrom) ||
					!TimeSpan.TryParse(model.durationto, out var targetTo) ||
					targetTo <= targetFrom)
					return false;

				var targetDate = model.appointmentdate.Date;
				var appointmentDate = targetDate.ToString("yyyy-MM-dd");
				var tenantId = model.tenantid?.ToString() ?? "";
				var practitionerId = model.practitioner.Value.ToString();
				var requestUrl =
					"api/ClinicalAppointment/get_Clinical_Appointment_List?tenantid=" + Uri.EscapeDataString(tenantId) +
					"&tasktype=&practitioner=" + Uri.EscapeDataString(practitionerId) +
					"&appointmentdate_automatonfrom=" + Uri.EscapeDataString(appointmentDate) +
					"&appointmentdate_automatonto=" + Uri.EscapeDataString(appointmentDate) +
					"&status=&origin=&pagesize=500&pagenumber=0&loginUserID=" + Uri.EscapeDataString(loginUserID ?? "");

				var json = await ApiClient.Get_ApiValues(httpClient, requestUrl);
				if (string.IsNullOrWhiteSpace(json))
					return false;

				json = json.Trim();
				if (!json.StartsWith("{") && !json.StartsWith("["))
					return false;

				using var doc = JsonDocument.Parse(json);
				JsonElement detail;
				if (doc.RootElement.ValueKind == JsonValueKind.Array)
				{
					detail = doc.RootElement;
				}
				else if (!doc.RootElement.TryGetProperty("detail", out detail) || detail.ValueKind != JsonValueKind.Array)
				{
					return false;
				}

				foreach (var row in detail.EnumerateArray())
				{
					var id = GetJsonString(row, "clinicalappointmentid", "ClinicalAppointmentid");
					if (!string.IsNullOrWhiteSpace(id) &&
						string.Equals(id, model.ClinicalAppointmentid?.ToString(), StringComparison.OrdinalIgnoreCase))
						continue;

					var status = GetJsonString(row, "status", "Status");
					if (IsClinicalAppointmentCancelledStatus(status))
						continue;

					var rowPractitioner = GetJsonString(row, "practitioner", "Practitioner");
					if (!string.Equals(rowPractitioner, practitionerId, StringComparison.OrdinalIgnoreCase))
						continue;

					var rowDate = GetJsonString(row, "appointmentdate", "Appointmentdate");
					if (!TryParseClinicalAppointmentDate(rowDate, out var existingDate) || existingDate.Date != targetDate)
						continue;

					if (!TimeSpan.TryParse(GetJsonString(row, "durationfrom", "Durationfrom"), out var existingFrom) ||
						!TimeSpan.TryParse(GetJsonString(row, "durationto", "Durationto"), out var existingTo))
						continue;

					if (targetFrom < existingTo && targetTo > existingFrom)
						return true;
				}

				return false;
			}

			private static bool IsClinicalAppointmentCancelledStatus(string status)
			{
				var normalized = (status ?? "").Trim().ToLowerInvariant()
					.Replace(" ", "")
					.Replace("-", "");
				return normalized == "cancelled"
					|| normalized == "canceled"
					|| normalized == "cancelledbypatient"
					|| normalized == "cancelledbyfrontdesk"
					|| normalized == "admittedcancelled";
			}

			private static bool IsClinicalAppointmentReassignmentLockedStatus(string status)
			{
				var normalized = (status ?? "").Trim().ToLowerInvariant()
					.Replace(" ", "")
					.Replace("-", "");
				return normalized.Contains("cancel")
					|| normalized.Contains("completed")
					|| normalized == "complete";
			}

			private static string GetJsonString(JsonElement row, params string[] names)
			{
				foreach (var name in names)
				{
					if (row.TryGetProperty(name, out var value) && value.ValueKind != JsonValueKind.Null)
						return value.ToString();
				}
				return "";
			}

			private static bool TryParseClinicalAppointmentDate(string value, out DateTime date)
			{
				return DateTime.TryParseExact(value ?? "",
					new[] { "dd/MM/yyyy", "yyyy-MM-dd", "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-dd HH:mm:ss" },
					CultureInfo.InvariantCulture,
					DateTimeStyles.None,
					out date)
					|| DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
			}

			public class RescheduleRequest
			{
				public string ClinicalAppointmentid { get; set; }
				public string appointmentdate { get; set; }
				public string durationfrom { get; set; }
				public string durationto { get; set; }
				public string practitioner { get; set; }
				public bool allowOverlap { get; set; }
				public bool requireDoctorChange { get; set; }
			}

			[HttpGet]
			public virtual async Task<string> GetPeopleIdByUserId(string usersid)
			{
				return await ApiClient.Get_ApiValues(getHttpClient(),
					"api/People/GetPeopleIdByUserId?usersid=" + usersid);
			}


        [HttpGet]
        public virtual async Task<string>
    Get_Clinical_Slot_Doctor_Schedule(
        string selecteddate)
        {
            try
            {
                var tenantid =
                    HttpContext.Session.GetString(
                        "NalamVazhachoosedtenantid"
                    );

                if (
                    string.IsNullOrWhiteSpace(
                        tenantid
                    )
                )
                {
                    tenantid =
                        HttpContext.Session.GetString(
                            "NalamVazhatenantid"
                        );
                }


                var loginUserID =
                    HttpContext.Session.GetString(
                        "NalamVazhaloginUserID"
                    );


                var apiUrl =
                    "api/ClinicalAppointment/" +
                    "Get_Clinical_Slot_Doctor_Schedule" +

                    "?tenantid=" +
                    Uri.EscapeDataString(
                        tenantid ?? ""
                    ) +

                    "&selecteddate=" +
                    Uri.EscapeDataString(
                        selecteddate ?? ""
                    ) +

                    "&loginUserID=" +
                    Uri.EscapeDataString(
                        loginUserID ?? ""
                    );


                return await ApiClient
                    .Get_ApiValues(
                        getHttpClient(),
                        apiUrl
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Get_Clinical_Slot_Doctor_Schedule failed."
                );

                return "[]";
            }
        }


        [HttpGet]
        public virtual async Task<string> Get_Calendar_Holiday(
    string tenantid,
    string datefrom,
    string dateto)
        {
            var httpClient = getHttpClient();

            var loginUserId =
                HttpContext.Session
                    .GetString("NalamVazhaloginUserID");

            var result =
                await ApiClient.Get_ApiValues(
                    httpClient,
                    "api/HolidayCalendar/Get_Holiday_By_Date"
                    + "?tenantid=" + tenantid
                    + "&datefrom=" + datefrom
                    + "&dateto=" + dateto
                    + "&loginUserID=" + loginUserId
                );

            return result;
        }

        [HttpGet]
        public virtual async Task<string>
            Get_Calendar_ClinicalAppointment_ById(
                string ClinicalAppointmentid)
        {
            var loginUserID =
                HttpContext.Session.GetString(
                    "NalamVazhaloginUserID"
                );

            return await ApiClient.Get_ApiValues(
                getHttpClient(),
                "api/ClinicalAppointment/getById_ClinicalAppointment"
                + "?ClinicalAppointmentid=" + ClinicalAppointmentid
                + "&loginUserID=" + loginUserID
            );
        }

    }


			}
