namespace Admin.Controllers
			{
				using System;
				using System.Data;
				using System.Linq;
				using Microsoft.AspNetCore.Mvc;
				using Newtonsoft.Json;
				using Newtonsoft.Json.Linq;
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

                
                
                
				//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:58
				
                
                
                
                
				public class PatientProfileController : BaseController
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
                    private readonly ILogger<PatientProfileController> _logger;
                    
                    
                    StorageUtil util;
					public PatientProfileController(IConfiguration configuration,IHttpContextAccessor accessor,IOptions<ApiSettings> ApiSettings, IOptions<MailSettings> MailSettings, IWebHostEnvironment env, ILogger<PatientProfileController> logger):base( configuration)
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
	              

					
				
			  public virtual async Task<string> getById_emergencycontactinfo(string PatientProfileid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/PatientProfile/getById_emergencycontactinfo?PatientProfileid="+PatientProfileid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
					 
			  }
       


        public virtual IActionResult Add_Patient_Profile()
			  {
					return View();
			  }	
			  [HttpPost()]
			public virtual async Task<string> Add_Patient_Profile(PatientProfileModel model, IFormCollection collection)
			{
				string strReturnMessage = "";
				var client = getHttpClient();
				try
				{
					ModelState.Remove("PatientProfileid");
					ModelState.Remove("createduser");
                    ModelState.Remove("craftmyapp_actionmethodname");
                    model.craftmyapp_actionmethodname="Add_Patient_Profile";
					if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
								model.createduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
								else
								return "Session Expired";                    
					ModelState.Remove("photo");
ModelState.Remove("uploadidproof");

                   
					model.languagesknown = collection["languagesknown"];
model.languagespreferrable = collection["languagespreferrable"];
model.creativeactivities = collection["creativeactivities"];

			 	    
					 if (ModelState.IsValid)
					 {
							 PatientProfileModelValidator validator = new PatientProfileModelValidator();
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
								 model.PatientProfileid =Guid.NewGuid(); 
                                 model.photo = collection["photo_existing"];
model.uploadidproof = collection["uploadidproof_existing"];

                                  var files = Request.Form.Files;
foreach (var file in files) 
{
var filename = ContentDispositionHeaderValue
.Parse(file.ContentDisposition)
.FileName
.Trim('"'); 
string fileExtention = "." + filename.Split('.').Last(); 
Random rnd = new Random();
string uploadFileName = System.Text.RegularExpressions.Regex.Replace(filename.Split('.').First(), @"[^0-9a-zA-Z_.]+", "").Replace(" ", String.Empty)+"_"+"PatientProfile_" +rnd.Next(1, 10000).ToString() + DateTime.Now.ToString("ddMMyyHHmmss")+ fileExtention;
if (fileExtention != ".")
{
Stream stream = file.OpenReadStream();
string fileURL=await util.fileSystem.UploadFileAsync(stream, Configuration.GetSection("AzureBlobStorageSetttings:folderName").Value, uploadFileName);
if (file.Name == "photo")
{
uploadFileName = "|" + fileURL +"|";
model.photo +=  uploadFileName ;
}
if (file.Name == "uploadidproof")
{
uploadFileName = "|" + fileURL +"|";
model.uploadidproof +=  uploadFileName ;
}
}
}

								 
                                 strReturnMessage = await ApiClient.Post_ApiValuesGetString(client,"api/PatientProfile/Add_Patient_Profile", model);
                                    
								 
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
                 
                 _logger.LogError(ex,"An exception occurred in - PatientProfile / Add_Patient_Profile, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
               
				 strReturnMessage = ex.Message;
			 }
		     ViewData["message"] = strReturnMessage;
			 if(strReturnMessage.Replace("\"", "").Contains("201.1")){
				 TempData["message"] = "Success";

                var parts = strReturnMessage.Replace("\"", "").Split('|');
				bool credentialsSent = false;
				if (parts.Length >= 3)
				{
                    string username = parts[1];
                    string plainPassword = parts[2];

                    MailSender maillog = new MailSender();
                    credentialsSent = await maillog.sendMail(
                        "OTP",
                        "User credientils",
                       model.PatientProfileid.ToString(),
                        "OTP",
                        _mailSettings,
                       model.createduser.ToString(),
                        client,

                         plainPassword,
                         username,
                         tenantid: model.tenantid?.ToString() ?? ""
                    );
                }
                return credentialsSent ? "Success|CredentialsSent" : "Success";
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

				
			  public virtual async Task<IActionResult> Add_to_Blacklist(string PatientProfileid)
			  {

                    string redirectTo="";
                    if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
                            DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
                            DataView dv = new DataView(NalamVazharole_JSON);
                            dv.RowFilter = "controllername='PatientProfile' AND viewname='list'";

                            if(dv.Count  >0){
                                redirectTo = dv[0]["actionmethodname"] as string;
							 
                            }

                            try{
                                     var jsonObjPatientProfile = await ApiClient.Get_ApiValues(getHttpClient(), "api/PatientProfile/getById_PatientProfile?PatientProfileid="+PatientProfileid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                                if(jsonObjPatientProfile.Length > 2)
                                {
                                  
                                    var model = JsonConvert.DeserializeObject<PatientProfileModel>(jsonObjPatientProfile);


                
                                     
                                    return View(model);
                                }
                                else
                                {
                    
                                    TempData["message"] = "Data Not Found - Contact Administrator";
                                    return RedirectToAction(redirectTo);
						 
                                }

                            }catch(Exception ex){
                               _logger.LogError(ex,"An exception occurred in - PatientProfile / Add_to_Blacklist, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                                TempData["errMessage"] = "Error while fetching data - Contact Administrator";
                                return RedirectToAction(redirectTo);
                            }

                    }
                    TempData["errMessage"] = "Session Expired";
                    return RedirectToAction("Logout", "users");
                }	
			  [HttpPost()]
				public virtual async Task<string> Add_to_Blacklist(PatientProfileModel model, IFormCollection collection)
				{
					string strReturnMessage = "";
					try
					{
							ModelState.Remove("PatientProfileid");
                            ModelState.Remove("craftmyapp_actionmethodname");
                             model.craftmyapp_actionmethodname="Add_to_Blacklist";
							
							
															if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
													model.modifieduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
													else
													return "Session Expired";
															model.languagesknown = collection["languagesknown"];
								model.languagespreferrable = collection["languagespreferrable"];
								model.creativeactivities = collection["creativeactivities"];

															model.photo = collection["photo_existing"];
								model.uploadidproof = collection["uploadidproof_existing"];

															ModelState.Remove("photo");
								ModelState.Remove("uploadidproof");

							 
                                        
                                        
                                        strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/PatientProfile/Add_to_Blacklist", model);
 									 
					}
					catch (Exception ex)
					{
                      _logger.LogError(ex,"An exception occurred in - PatientProfile / Add_to_Blacklist, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
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

		public virtual async Task<IActionResult> Modify_Patient_Category(string PatientProfileid)
		{

			string redirectTo = "";
			if (HttpContext.Session.GetString("NalamVazharole_JSON") != null)
			{
				DataTable NalamVazharole_JSON = HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
				DataView dv = new DataView(NalamVazharole_JSON);
				dv.RowFilter = "controllername='PatientProfile' AND viewname='list'";

				if (dv.Count > 0)
				{
					redirectTo = dv[0]["actionmethodname"] as string;

				}

				try
				{
					var jsonObjPatientProfile = await ApiClient.Get_ApiValues(getHttpClient(), "api/PatientProfile/getById_PatientProfile?PatientProfileid=" + PatientProfileid + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
					if (jsonObjPatientProfile.Length > 2)
					{

						var model = JsonConvert.DeserializeObject<PatientProfileModel>(jsonObjPatientProfile);




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
					_logger.LogError(ex, "An exception occurred in - PatientProfile / Add_to_Blacklist, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

					TempData["errMessage"] = "Error while fetching data - Contact Administrator";
					return RedirectToAction(redirectTo);
				}

			}
			TempData["errMessage"] = "Session Expired";
			return RedirectToAction("Logout", "users");
		}
		[HttpPost()]
		public virtual async Task<string> Modify_Patient_Category(PatientProfileModel model, IFormCollection collection)
		{
			string strReturnMessage = "";
			try
			{
				ModelState.Remove("PatientProfileid");
				ModelState.Remove("craftmyapp_actionmethodname");
				model.craftmyapp_actionmethodname = "Add_to_Blacklist";


				if (HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
					model.modifieduser = new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
				else
					return "Session Expired";
				 

				strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(), "api/PatientProfile/Modify_Patient_Category", model);

			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - PatientProfile / Add_to_Blacklist, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

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




		public virtual async Task<IActionResult> Mark_as_Deceased(string PatientProfileid)
			  {

                    string redirectTo="";
                    if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
                            DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
                            DataView dv = new DataView(NalamVazharole_JSON);
                            dv.RowFilter = "controllername='PatientProfile' AND viewname='list'";

                            if(dv.Count  >0){
                                redirectTo = dv[0]["actionmethodname"] as string;
							 
                            }

                            try{
                                     var jsonObjPatientProfile = await ApiClient.Get_ApiValues(getHttpClient(), "api/PatientProfile/getById_PatientProfile?PatientProfileid="+PatientProfileid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                                if(jsonObjPatientProfile.Length > 2)
                                {
                                  
                                    var model = JsonConvert.DeserializeObject<PatientProfileModel>(jsonObjPatientProfile);


                
                                     
                                    return View(model);
                                }
                                else
                                {
                    
                                    TempData["message"] = "Data Not Found - Contact Administrator";
                                    return RedirectToAction(redirectTo);
						 
                                }

                            }catch(Exception ex){
                               _logger.LogError(ex,"An exception occurred in - PatientProfile / Mark_as_Deceased, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                                TempData["errMessage"] = "Error while fetching data - Contact Administrator";
                                return RedirectToAction(redirectTo);
                            }

                    }
                    TempData["errMessage"] = "Session Expired";
                    return RedirectToAction("Logout", "users");
                }	
			  [HttpPost()]
				public virtual async Task<string> Mark_as_Deceased(PatientProfileModel model, IFormCollection collection)
				{
					string strReturnMessage = "";
					try
					{
							ModelState.Remove("PatientProfileid");
                            ModelState.Remove("craftmyapp_actionmethodname");
                             model.craftmyapp_actionmethodname="Mark_as_Deceased";
							
							
							if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
					model.modifieduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
					else
					return "Session Expired";
							model.languagesknown = collection["languagesknown"];
model.languagespreferrable = collection["languagespreferrable"];
model.creativeactivities = collection["creativeactivities"];

                            model.photo = collection["photo_existing"];
model.uploadidproof = collection["uploadidproof_existing"];

                            ModelState.Remove("photo");
ModelState.Remove("uploadidproof");

						

                                        
                                        
                                        strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/PatientProfile/Mark_as_Deceased", model);
 									
							
							
					}
					catch (Exception ex)
					{
                      _logger.LogError(ex,"An exception occurred in - PatientProfile / Mark_as_Deceased, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
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

				
			  public virtual async Task<IActionResult> Update_Patient_Profile(string PatientProfileid)
			  {

                    string redirectTo="";
                    if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
                            DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
                            DataView dv = new DataView(NalamVazharole_JSON);
                            dv.RowFilter = "controllername='PatientProfile' AND viewname='list'";

                            if(dv.Count  >0){
                                redirectTo = dv[0]["actionmethodname"] as string;
							 
                            }

                            try{
                                     var jsonObjPatientProfile = await ApiClient.Get_ApiValues(getHttpClient(), "api/PatientProfile/getById_PatientProfile?PatientProfileid="+PatientProfileid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                                if(jsonObjPatientProfile.Length > 2)
                                {
                                  
                                    var model = JsonConvert.DeserializeObject<PatientProfileModel>(jsonObjPatientProfile);


                
                                     
                                    return View("Update_Patient_Profile",model);
                                }
                                else
                                {
                    
                                    TempData["message"] = "Data Not Found - Contact Administrator";
                                    return RedirectToAction(redirectTo);
						 
                                }

                            }catch(Exception ex){
                               _logger.LogError(ex,"An exception occurred in - PatientProfile / Update_Patient_Profile, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                                TempData["errMessage"] = "Error while fetching data - Contact Administrator";
                                return RedirectToAction(redirectTo);
                            }

                    }
                    TempData["errMessage"] = "Session Expired";
                    return RedirectToAction("Logout", "users");
                }


        [AllowAnonymous]
        public virtual async Task<IActionResult> Quick_Add_Patient_Profile(string tenantid)
        {
			//if (HttpContext.Session.GetString("NalamVazhaotpVerified") != "true")
			//    return RedirectToAction("Login", "users");
			tenantid = tenantid?.Trim();
			if (string.IsNullOrWhiteSpace(tenantid))
				tenantid = HttpContext.Items["SubdomainTenantId"] as string;

			if (!Guid.TryParse(tenantid, out var resolvedTenantId) || resolvedTenantId == Guid.Empty)
				return RedirectToAction("Index", "users");

			tenantid = resolvedTenantId.ToString();
			HttpContext.Session.Clear();
			HttpContext.Session.SetString("QuickAddTenantId", tenantid);
            HttpContext.Session.SetString("NalamVazhachoosedtenantid", tenantid);
            ViewBag.TenantId = tenantid;
            ViewBag.OtpVerified = "true";
            ViewBag.EnablePatientAutoLogin = false;

            if (Guid.TryParse(tenantid, out var tenantGuid) && tenantGuid != Guid.Empty)
            {
                try
                {
                    var tenantJson = await ApiClient.Get_ApiValues(
                        getHttpClient(),
                        "api/tenant/getById_tenant?tenantid=" + tenantGuid);
                    var tenant = JsonConvert.DeserializeObject<tenantModel>(tenantJson);
                    ViewBag.EnablePatientAutoLogin = tenant?.enablepatientautologin == true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Unable to load patient auto-login configuration for tenant " + tenantid);
                }
            }


            //var model = new PatientProfileModel();
            //string email = HttpContext.Session.GetString("NalamVazhaauthenticatedEmail");
            //if (!string.IsNullOrEmpty(email))
            //    model.emailaddress = email;

            return View();
        }

        private static bool IsAllowedQuickAddIdProofFile(IFormFile file)
        {
            if (file == null || string.IsNullOrWhiteSpace(file.FileName))
                return false;

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (extension == ".pdf")
                return true;

            if (!string.IsNullOrWhiteSpace(file.ContentType)
                && file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                return true;

            return extension == ".png"
                || extension == ".jpg"
                || extension == ".jpeg"
                || extension == ".gif"
                || extension == ".bmp"
                || extension == ".webp";
        }

        private static string BuildQuickAddPatientProfileUploadFileName(string originalFileName)
        {
            var extension = Path.GetExtension(originalFileName);
            var baseName = Path.GetFileNameWithoutExtension(originalFileName) ?? "";
            baseName = System.Text.RegularExpressions.Regex.Replace(baseName, @"[^0-9a-zA-Z_.]+", "").Replace(" ", string.Empty);

            var suffix = "_PatientProfile_" + new Random().Next(1, 10000).ToString() + DateTime.Now.ToString("ddMMyyHHmmss") + extension;
            var maxBaseLength = 48 - suffix.Length;
            if (maxBaseLength < 1)
                maxBaseLength = 1;

            if (string.IsNullOrWhiteSpace(baseName))
                baseName = "Profile";

            if (baseName.Length > maxBaseLength)
                baseName = baseName.Substring(0, maxBaseLength);

            return baseName + suffix;
        }

        private static string GetPatientProfileValidationMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return message;

            var cleanMessage = message.Replace("\"", "").Replace("BadRequest :", "").Trim();
            var lowerMessage = cleanMessage.ToLowerInvariant();

            if (lowerMessage.Contains("users_emailid_key")
                || (lowerMessage.Contains("duplicate key") && lowerMessage.Contains("email")))
                return "This email address is already registered. Please use another email address or login with the existing account.";

            if (lowerMessage.Contains("mobile")
                && (lowerMessage.Contains("duplicate key") || lowerMessage.Contains("unique constraint")))
                return "This mobile number is already registered. Please use another mobile number.";

            if ((lowerMessage.Contains("idproof") || lowerMessage.Contains("id proof"))
                && (lowerMessage.Contains("duplicate key") || lowerMessage.Contains("unique constraint")))
                return "This ID proof number is already registered. Please check the ID proof details.";

            return cleanMessage;
        }

        [AllowAnonymous]
        [HttpPost()]
        public virtual async Task<string> Quick_Add_Patient_Profile(PatientProfileModel model, IFormCollection collection)
        {
			 
			string strReturnMessage = "";
             client = getHttpClient();
            try
            {
                string otpVerified = HttpContext.Session.GetString("NalamVazhaotpVerified");
                if (string.IsNullOrEmpty(otpVerified))
                    otpVerified = collection["otpVerified"].ToString();
                if (otpVerified != "true")
                    return "Session Expired";


                ModelState.Remove("PatientProfileid");
                ModelState.Remove("createduser");
                ModelState.Remove("craftmyapp_actionmethodname");
                model.craftmyapp_actionmethodname = "Quick_Add_Patient_Profile";

                // Get tenantid from session
                var tenantid = HttpContext.Session.GetString("QuickAddTenantId");
                model.tenantid = string.IsNullOrEmpty(tenantid) ? Guid.Empty : new Guid(tenantid);

                ModelState.Remove("photo");
                ModelState.Remove("uploadidproof");

                model.languagesknown = collection["languagesknown"];
                model.languagespreferrable = collection["languagespreferrable"];
                model.creativeactivities = collection["creativeactivities"];

                if (ModelState.IsValid)
                {
                    PatientProfileModelValidator validator = new PatientProfileModelValidator();
                    ValidationResult results = validator.Validate(model);
                    if (!results.IsValid)
                    {
                        var errorCollection = string.Join(" | ", results.Errors.Select(e => e.ErrorMessage.Replace("{propertyName}", e.PropertyName)));
                        strReturnMessage = errorCollection.ToString();
                    }
                    else
                    {
                        model.PatientProfileid = Guid.NewGuid();
                        model.photo = collection["photo_existing"];
                        model.uploadidproof = collection["uploadidproof_existing"];

                        var files = Request.Form.Files;
                        foreach (var file in files)
                        {
                            if (file.Name == "uploadidproof" && !IsAllowedQuickAddIdProofFile(file))
                            {
                                return System.Text.Json.JsonSerializer.Serialize(new
                                {
                                    success = false,
                                    message = "Upload ID Proof accepts PDF or image files only."
                                });
                            }

                            var filename = ContentDispositionHeaderValue
                                .Parse(file.ContentDisposition)
                                .FileName.Trim('"');
                            string fileExtention = "." + filename.Split('.').Last();
                            string uploadFileName = BuildQuickAddPatientProfileUploadFileName(filename);

                            if (fileExtention != ".")
                            {
                                Stream stream = file.OpenReadStream();
                                string fileURL = await util.fileSystem.UploadFileAsync(
                                    stream,
                                    Configuration.GetSection("AzureBlobStorageSetttings:folderName").Value,
                                    uploadFileName);
                                if (file.Name == "photo") model.photo += "|" + fileURL + "|";
                                if (file.Name == "uploadidproof") model.uploadidproof += "|" + fileURL + "|";
                            }
                        }

                        // Call WebAPI
                        
                        //client.DefaultRequestHeaders.Add("X-AccessKey", accesskey);
                        strReturnMessage = await ApiClient.Post_ApiValuesGetString(
                            client,
                            "api/PatientProfile/Quick_Add_Patient_Profile",
                            model);
                    }
                }
                else
                {
                    var errorMessages = ModelState.Where(entry => entry.Value.Errors.Any())
                        .SelectMany(entry => entry.Value.Errors.Select(error => $"{entry.Key}: {error.ErrorMessage}"));
                    strReturnMessage = string.Join(" | ", errorMessages);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred in - PatientProfile / Quick_Add_Patient_Profile, Error Message : "
                    + (ex.StackTrace != null ? ex.StackTrace.ToString() : ex.Message));
                strReturnMessage = ex.Message;
            }

            if (strReturnMessage.Replace("\"", "").Contains("201.1"))
            {
                // Get plain password from WebAPI response "201.1|plainpassword"
                string plainPassword = "";
                string[] responseParts = strReturnMessage.Replace("\"", "").Split('|');
                if (responseParts.Length > 1)
                    plainPassword = responseParts[1];

                // Send welcome email with credentials
                try
                {
                    MailSender maillog = new MailSender();
                    bool mailSent = await maillog.sendMail(
                        "OTP",
                        "User credientils",
                        model.PatientProfileid.ToString(),
                        "OTP",
                        _mailSettings,
                       model.PatientProfileid.ToString(),
                        client,                      

                         plainPassword,
                         model.emailaddress,
                         tenantid: model.tenantid?.ToString() ?? ""
                    );
                }
                catch (Exception mailEx)
                {
                    _logger.LogError(mailEx, "Email sending failed for - " + model.emailaddress);
                }

                HttpContext.Session.Remove("NalamVazhaauthenticatedEmail");
                HttpContext.Session.Remove("NalamVazhaotpVerified");
                TempData["successmessage"] = "Profile created. Please check your mail for credentials.";
				return System.Text.Json.JsonSerializer.Serialize(new
				{
					success = true,
					patientProfileId = model.PatientProfileid,
					name = ((model.firstname ?? "") + " " + (model.lastname ?? "")).Trim(),
					message = "Patient created successfully"
				});
			}
            else if (strReturnMessage.StartsWith("BadRequest", StringComparison.OrdinalIgnoreCase))
            {
                strReturnMessage = GetPatientProfileValidationMessage(strReturnMessage);
                TempData["message"] = strReturnMessage;
                return strReturnMessage;
            }
            else
            {
                if (strReturnMessage == "401.1")
                    strReturnMessage = "Authorization Failed";
                else
                    strReturnMessage = GetPatientProfileValidationMessage(strReturnMessage);
                return strReturnMessage;
            }
        }



		[AllowAnonymous]
		[HttpPost()]
		public virtual async Task<string> Quick_Add_Patient_Profile_Global (PatientProfileModel model, IFormCollection collection)
		{


			string strReturnMessage = "";
			client = getHttpClient();
			try
			{
				string otpVerified = HttpContext.Session.GetString("NalamVazhaotpVerified");
				if (string.IsNullOrEmpty(otpVerified))
					otpVerified = collection["otpVerified"].ToString();
				if (otpVerified != "true")
					return "Session Expired";


				ModelState.Remove("PatientProfileid");
				ModelState.Remove("createduser");
				ModelState.Remove("craftmyapp_actionmethodname");
				model.craftmyapp_actionmethodname = "Quick_Add_Patient_Profile";

				// Get tenantid from session
				var tenantid = HttpContext.Session.GetString("QuickAddTenantId");
				model.tenantid = string.IsNullOrEmpty(tenantid) ? Guid.Empty : new Guid(tenantid);

				ModelState.Remove("photo");
				ModelState.Remove("uploadidproof");

				model.languagesknown = collection["languagesknown"];
				model.languagespreferrable = collection["languagespreferrable"];
				model.creativeactivities = collection["creativeactivities"];

				if (ModelState.IsValid)
				{
					PatientProfileModelValidator validator = new PatientProfileModelValidator();
					ValidationResult results = validator.Validate(model);
					if (!results.IsValid)
					{
						var errorCollection = string.Join(" | ", results.Errors.Select(e => e.ErrorMessage.Replace("{propertyName}", e.PropertyName)));
						strReturnMessage = errorCollection.ToString();
					}
					else
					{
						model.PatientProfileid = Guid.NewGuid();
						model.photo = collection["photo_existing"];
						model.uploadidproof = collection["uploadidproof_existing"];

						var files = Request.Form.Files;
						foreach (var file in files)
						{
							var filename = ContentDispositionHeaderValue
								.Parse(file.ContentDisposition)
								.FileName.Trim('"');
							string fileExtention = "." + filename.Split('.').Last();
							string uploadFileName = BuildQuickAddPatientProfileUploadFileName(filename);

							if (fileExtention != ".")
							{
								Stream stream = file.OpenReadStream();
								string fileURL = await util.fileSystem.UploadFileAsync(
									stream,
									Configuration.GetSection("AzureBlobStorageSetttings:folderName").Value,
									uploadFileName);
								if (file.Name == "photo") model.photo += "|" + fileURL + "|";
								if (file.Name == "uploadidproof") model.uploadidproof += "|" + fileURL + "|";
							}
						}

						// Call WebAPI

						//client.DefaultRequestHeaders.Add("X-AccessKey", accesskey);
						strReturnMessage = await ApiClient.Post_ApiValuesGetString(
							client,
							"api/PatientProfile/Quick_Add_Patient_Profile",
							model);
					}
				}
				else
				{
					var errorMessages = ModelState.Where(entry => entry.Value.Errors.Any())
						.SelectMany(entry => entry.Value.Errors.Select(error => $"{entry.Key}: {error.ErrorMessage}"));
					strReturnMessage = string.Join(" | ", errorMessages);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - PatientProfile / Quick_Add_Patient_Profile, Error Message : "
					+ (ex.StackTrace != null ? ex.StackTrace.ToString() : ex.Message));
				strReturnMessage = ex.Message;
			}

			if (strReturnMessage.Replace("\"", "").Contains("201.1"))
			{
				// Get plain password from WebAPI response "201.1|plainpassword"
				string plainPassword = "";
				string[] responseParts = strReturnMessage.Replace("\"", "").Split('|');
				if (responseParts.Length > 1)
					plainPassword = responseParts[1];

				// Send welcome email with credentials
				bool credentialsSent = false;
				try
				{
					MailSender maillog = new MailSender();
					credentialsSent = await maillog.sendMail(
						"OTP",
						"User credientils",
						model.PatientProfileid.ToString(),
						"OTP",
						_mailSettings,
					   model.createduser.ToString(),
						client,

						 plainPassword,
						 model.emailaddress,
						 tenantid: model.tenantid?.ToString() ?? ""
					);
				}
				catch (Exception mailEx)
				{
					_logger.LogError(mailEx, "Email sending failed for - " + model.emailaddress);
				}

				HttpContext.Session.Remove("NalamVazhaauthenticatedEmail");
				HttpContext.Session.Remove("NalamVazhaotpVerified");
				if (!string.IsNullOrWhiteSpace(model.photo))
				{
					HttpContext.Session.SetString("NalamVazhaPendingPatientProfilePicture", model.photo);
					HttpContext.Session.SetString("NalamVazhaPendingPatientProfileEmail", model.emailaddress ?? "");
					HttpContext.Session.SetString("NalamVazhaPendingPatientProfileId", model.PatientProfileid.ToString());
				}
				TempData["successmessage"] = "Profile created. Please check your mail for credentials.";
				return "Success|" + model.emailaddress + "|" + plainPassword
					+ (credentialsSent ? "|CredentialsSent" : "");
			}
			else if (strReturnMessage.StartsWith("BadRequest", StringComparison.OrdinalIgnoreCase))
			{
				strReturnMessage = GetPatientProfileValidationMessage(strReturnMessage);
				TempData["message"] = strReturnMessage;
				return strReturnMessage;
			}
			else
			{
				if (strReturnMessage == "401.1")
					strReturnMessage = "Authorization Failed";
				else
					strReturnMessage = GetPatientProfileValidationMessage(strReturnMessage);
				return strReturnMessage;
			}
		}

		[HttpPost()]
				public virtual async Task<string> Update_Patient_Profile(PatientProfileModel model, IFormCollection collection)
				{
					string strReturnMessage = "";
					string requestedEmail = model.emailaddress?.Trim() ?? "";
					string previousEmail = "";
					PatientProfileModel existingProfile = null;
					var client = getHttpClient();
					try
					{
						if (model.PatientProfileid.HasValue && model.PatientProfileid.Value != Guid.Empty)
						{
							var existingJson = await ApiClient.Get_ApiValues(client,
								"api/PatientProfile/getById_PatientProfile?PatientProfileid=" + model.PatientProfileid
								+ "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
							if (!string.IsNullOrWhiteSpace(existingJson) && existingJson.Length > 2)
							{
								existingProfile = JsonConvert.DeserializeObject<PatientProfileModel>(existingJson);
								previousEmail = existingProfile?.emailaddress?.Trim() ?? "";
							}
						}
							ModelState.Remove("PatientProfileid");
                            ModelState.Remove("craftmyapp_actionmethodname");
                             model.craftmyapp_actionmethodname="Update_Patient_Profile";
							
							
							if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
					model.modifieduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
					else
					return "Session Expired";
							model.languagesknown = collection["languagesknown"];
model.languagespreferrable = collection["languagespreferrable"];
model.creativeactivities = collection["creativeactivities"];

                            model.photo = collection["photo_existing"];
model.uploadidproof = collection["uploadidproof_existing"];

                            ModelState.Remove("photo");
ModelState.Remove("uploadidproof");

							if (ModelState.IsValid)
							{
									PatientProfileModelValidator validator = new PatientProfileModelValidator();
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
										var emailChanged = !string.IsNullOrWhiteSpace(requestedEmail)
											&& !string.Equals(previousEmail, requestedEmail, StringComparison.OrdinalIgnoreCase);
										if (emailChanged)
										{
											var validationMessage = await ApiClient.Post_ApiValuesGetString(client,
												"api/users/ManageProfileEmailChange", new JObject
												{
													["profiletype"] = "PatientProfile",
													["profileid"] = model.PatientProfileid,
													["tenantid"] = model.tenantid,
													["newemail"] = requestedEmail,
													["apply"] = false
												});
											if (validationMessage.Replace("\"", "") != "201.1")
												return validationMessage.Replace("\"", "").Replace("BadRequest :", "");
											model.emailaddress = previousEmail;
										}
                                        
										var files = Request.Form.Files;
foreach (var file in files) 
{
var filename = ContentDispositionHeaderValue
.Parse(file.ContentDisposition)
.FileName
.Trim('"'); 
string fileExtention = "." + filename.Split('.').Last(); 
Random rnd = new Random();
string uploadFileName = System.Text.RegularExpressions.Regex.Replace(filename.Split('.').First(), @"[^0-9a-zA-Z_.]+", "").Replace(" ", String.Empty)+"_"+"PatientProfile_" +rnd.Next(1, 10000).ToString() + DateTime.Now.ToString("ddMMyyHHmmss")+ fileExtention;
if (fileExtention != ".")
{
Stream stream = file.OpenReadStream();
string fileURL=await util.fileSystem.UploadFileAsync(stream, Configuration.GetSection("AzureBlobStorageSetttings:folderName").Value, uploadFileName);
if (file.Name == "photo")
{
uploadFileName = "|" + fileURL +"|";
model.photo +=  uploadFileName ;
}
if (file.Name == "uploadidproof")
{
uploadFileName = "|" + fileURL +"|";
model.uploadidproof +=  uploadFileName ;
}
}
}

                                        
                                        
										strReturnMessage = await ApiClient.Post_ApiValuesGetString(client,"api/PatientProfile/Update_Patient_Profile", model);
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
                      _logger.LogError(ex,"An exception occurred in - PatientProfile / Update_Patient_Profile, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));

						strReturnMessage = ex.Message;
					}
					ViewData["message"] = strReturnMessage;
					    if(strReturnMessage.Replace("\"", "")=="201.1"){
							TempData["message"] = "Success";
							var emailChanged = !string.IsNullOrWhiteSpace(requestedEmail)
								&& !string.Equals(previousEmail, requestedEmail, StringComparison.OrdinalIgnoreCase);
							if (emailChanged)
							{
								var otpResponse = await ApiClient.Post_ApiValuesGetRespnse(client,
									"api/users/RequestPatientOTP", new userloginModel { username = requestedEmail });
								if (!otpResponse.IsSuccessStatusCode)
									return "Profile updated, but OTP could not be generated. Your login email was not changed. "
										+ (await otpResponse.Content.ReadAsStringAsync()).Replace("\"", "");

								var otpResult = JObject.Parse(await otpResponse.Content.ReadAsStringAsync());
								var generatedOtp = otpResult["otp"]?.ToString();
								var otpLogId = otpResult["otpsid"]?.ToString();
								var tenantId = otpResult["tenantid"]?.ToString();
								if (string.IsNullOrWhiteSpace(tenantId) || tenantId == Guid.Empty.ToString())
									tenantId = existingProfile?.tenantid?.ToString() ?? model.tenantid?.ToString();
								if (string.IsNullOrWhiteSpace(generatedOtp) || string.IsNullOrWhiteSpace(otpLogId)
									|| string.IsNullOrWhiteSpace(tenantId))
									return "OTP response was incomplete. Your login email was not changed.";

								var mailSender = new MailSender();
								var sent = await mailSender.sendMail("OTP", "OTP Generator", otpLogId, "OTP",
									_mailSettings, HttpContext.Session.GetString("NalamVazhaloginUserID"),
									client, generatedOtp, requestedEmail, tenantid: tenantId);
								if (!sent)
									return "Profile updated, but the OTP email could not be sent. Your login email was not changed.";

								var pendingKey = model.PatientProfileid.Value.ToString();
								HttpContext.Session.SetString("PendingPatientEmail:" + pendingKey, requestedEmail);
								HttpContext.Session.SetString("PendingPatientTenant:" + pendingKey, model.tenantid?.ToString() ?? "");
								HttpContext.Session.SetString("PendingPatientEmailExpires:" + pendingKey,
									DateTimeOffset.UtcNow.AddMinutes(10).ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture));
								return "OTP_REQUIRED|" + requestedEmail;
							}

							// Refresh the cached header identity only when a Health Seeker saves
							// their own patient profile. Staff can also edit patient profiles, so
							// their session identity must remain unchanged.
							var sessionRole = HttpContext.Session.GetString("NalamVazhauserrole");
							var sessionUserId = HttpContext.Session.GetString("NalamVazhaloginUserID");
							var sessionUsername = HttpContext.Session.GetString("NalamVazhausername");
							var isOwnProfileId = model.PatientProfileid.HasValue &&
								Guid.TryParse(sessionUserId, out var loggedInUserId) &&
								model.PatientProfileid.Value == loggedInUserId;
							var isOwnProfileEmail = !string.IsNullOrWhiteSpace(sessionUsername) &&
								!string.IsNullOrWhiteSpace(model.emailaddress) &&
								sessionUsername.Trim().Equals(model.emailaddress.Trim(), StringComparison.OrdinalIgnoreCase);

							if (sessionRole?.Equals("Health Seeker", StringComparison.OrdinalIgnoreCase) == true &&
								(isOwnProfileId || isOwnProfileEmail))
							{
								var updatedFullName = $"{model.firstname} {model.lastname}".Trim();
								if (!string.IsNullOrWhiteSpace(updatedFullName))
									HttpContext.Session.SetString("firstname", updatedFullName);

								if (!string.IsNullOrWhiteSpace(model.emailaddress))
									HttpContext.Session.SetString("NalamVazhausername", model.emailaddress.Trim());

								if (!string.IsNullOrWhiteSpace(model.photo))
									HttpContext.Session.SetString("NalamVazhaprofilepicture", util.fileSystem.GetFileURl(model.photo));
							}

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

		[HttpPost]
		public virtual async Task<string> Verify_Patient_Profile_Email_OTP(string PatientProfileid, string otp)
		{
			if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("NalamVazhaloginUserID")))
				return "Session Expired";
			if (!Guid.TryParse(PatientProfileid, out var profileId))
				return "Invalid profile.";

			var emailKey = "PendingPatientEmail:" + PatientProfileid;
			var tenantKey = "PendingPatientTenant:" + PatientProfileid;
			var expiryKey = "PendingPatientEmailExpires:" + PatientProfileid;
			var email = HttpContext.Session.GetString(emailKey);
			var tenant = HttpContext.Session.GetString(tenantKey);
			if (string.IsNullOrWhiteSpace(email) || !Guid.TryParse(tenant, out var tenantId)
				|| !long.TryParse(HttpContext.Session.GetString(expiryKey), out var expiry)
				|| DateTimeOffset.UtcNow.ToUnixTimeSeconds() > expiry)
				return "The email verification request has expired. Please save again.";

			var client = getHttpClient();
			var result = await ApiClient.Post_ApiValuesGetString(client,
				"api/users/ManageProfileEmailChange", new JObject
				{
					["profiletype"] = "PatientProfile", ["profileid"] = profileId,
					["tenantid"] = tenantId, ["newemail"] = email, ["otp"] = otp, ["apply"] = true
				});
			if (result.Replace("\"", "") != "201.1")
				return result.Replace("\"", "").Replace("BadRequest :", "");

			HttpContext.Session.Remove(emailKey);
			HttpContext.Session.Remove(tenantKey);
			HttpContext.Session.Remove(expiryKey);
			if (HttpContext.Session.GetString("NalamVazhaloginUserID") == PatientProfileid)
				HttpContext.Session.SetString("NalamVazhausername", email);
			return "Success";
		}

		[HttpPost]
		public virtual async Task<string> Update_Emergency_Contact_Info([FromBody] PatientProfileEmergencyContactUpdateModel request)
		{
			try
			{
				var loginUserId = HttpContext.Session.GetString("NalamVazhaloginUserID");
				if (string.IsNullOrWhiteSpace(loginUserId))
					return "Session Expired";

				if (request == null || request.PatientProfileid == Guid.Empty ||
					request.emergencycontactinfo == null || request.emergencycontactinfo.Count == 0)
					return "Please add at least one emergency contact.";

				var order = 0;
				foreach (var contact in request.emergencycontactinfo)
				{
					contact.PatientProfileid = request.PatientProfileid;
					contact.record_order = ++order;
					contact.craftmyapp_actionmethodname = "Update_Patient_Profile";
				}

				return await ApiClient.Post_ApiValuesGetString(getHttpClient(),
					"api/PatientProfile/Update_Emergency_Contact_Info", request);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unable to update patient emergency contact information.");
				return ex.Message;
			}
		}
public virtual async Task<IActionResult> Remove_Patient_Profile(string PatientProfileid)
			{
				string message = "";
				try
				{
						message = await ApiClient.Get_ApiValues(getHttpClient(), "api/PatientProfile/Remove_Patient_Profile?PatientProfileid="+PatientProfileid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
						 if(message.Replace("\"","").Contains("201.1"))
						{
							TempData["message"] = "Success";

						}else{
							TempData["errMessage"] = message.Replace("\"","");
						}
						
				
				
				}
				catch (Exception ex)
				{
                     _logger.LogError(ex,"An exception occurred in - PatientProfile / Remove_Patient_Profile, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                
					 TempData["errMessage"] = ex.Message;
					 message = ex.Message;
				}

				string redirectTo="";
						if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
					DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
						 DataView dv = new DataView(NalamVazharole_JSON);
						 dv.RowFilter = "controllername='PatientProfile' AND viewname='list'";

						if(dv.Count  >0){
						    redirectTo = dv[0]["actionmethodname"] as string;
							 
						}

					}
				
				return RedirectToAction(redirectTo);
			}

                                        public virtual IActionResult View_Patient_Profile()
                                        {
                                            return View();
                                        }
		 public virtual IActionResult Patient_Profile_List()
			        {
				        return View();
			        }
			        	
			        public virtual IActionResult Patient_Profiles()
			        {
				        return View();
			        }
			        	
			        [HttpGet()]
			        public virtual async Task<string> get_Patient_Profiles(string tenantid
, int? pagesize=100 , int? pagenumber=0,string searchterm="",string sortFieldsJson="")
			        {
				        
				        return await ApiClient.Get_ApiValues(getHttpClient(), "api/PatientProfile/Patient_Profiles?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID")
+ "&pagesize=" + pagesize + "&pagenumber="+ pagenumber + "&searchterm="+ searchterm + "&sort_fields=" + sortFieldsJson);
			        }
		[HttpGet()]
		public virtual async Task<string> get_Patient_Profile_List(string tenantid
, string firstname
, string dateofbirth_automatonfrom
, string dateofbirth_automatonto
, string emailaddress
, string mobilenumber
, string whatsappnumber
, string idprooftype
, string idproofnumber
, string pzip
, string blacklisted
, string deceased
, int? pagesize = 100, int? pagenumber = 0, string searchterm = "", string sortFieldsJson = "")
		{

			return await ApiClient.Get_ApiValues(getHttpClient(), "api/PatientProfile/Patient_Profile_List?tenantid=" + tenantid + "&firstname=" + firstname + "&dateofbirth_automatonfrom=" + dateofbirth_automatonfrom + "&dateofbirth_automatonto=" + dateofbirth_automatonto + "&emailaddress=" + emailaddress + "&mobilenumber=" + mobilenumber + "&whatsappnumber=" + whatsappnumber + "&idprooftype=" + idprooftype + "&idproofnumber=" + idproofnumber + "&pzip=" + pzip + "&blacklisted=" + blacklisted + "&deceased=" + deceased + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID")
+ "&pagesize=" + pagesize + "&pagenumber=" + pagenumber + "&searchterm=" + searchterm + "&sort_fields=" + sortFieldsJson);
		}

		/// <summary>Quick-add a patient with only mandatory fields. Returns JSON { success, patientProfileId, name, message }.</summary>
		[HttpPost()]
		public virtual async Task<IActionResult> QuickAdd_Patient_Profile(PatientProfileModel model, IFormCollection collection)
		{
			string strReturnMessage = "";

			var client = getHttpClient();
			try
			{
				ModelState.Remove("PatientProfileid");
				ModelState.Remove("createduser");
				ModelState.Remove("craftmyapp_actionmethodname");
				model.craftmyapp_actionmethodname = "Add_Patient_Profile";

				var postedDateOfBirth = collection["dateofbirth"].ToString().Trim();
				if (!DateTime.TryParseExact(
					postedDateOfBirth,
					new[] { "yyyy-MM-dd", "dd/MM/yyyy", "d/M/yyyy" },
					CultureInfo.InvariantCulture,
					DateTimeStyles.None,
					out var parsedDateOfBirth))
				{
					return Json(new { success = false, message = "Enter a valid Date of Birth in dd/MM/yyyy format." });
				}
				model.dateofbirth = parsedDateOfBirth.Date;
				ModelState.Remove("dateofbirth");

				if (HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
					model.createduser = new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
				else
					return Json(new { success = false, message = "Session Expired" });

				ModelState.Remove("photo");
				ModelState.Remove("uploadidproof");

				model.languagesknown = collection["languagesknown"];
				model.languagespreferrable = collection["languagespreferrable"];
				model.creativeactivities = collection["creativeactivities"];
				if (model.pzip <= 0)
					return Json(new { success = false, message = "A valid PIN Code is required." });
				model.czip = model.pzip;

				if (ModelState.IsValid)
				{
					PatientProfileModelValidator validator = new PatientProfileModelValidator();
					ValidationResult results = validator.Validate(model);
					if (!results.IsValid)
					{
						var errorCollection = string.Join(" | ", results.Errors.Select(e => e.ErrorMessage.Replace("{propertyName}", e.PropertyName)));
						return Json(new { success = false, message = errorCollection });
					}
					else
					{
						model.PatientProfileid = Guid.NewGuid();
						model.photo = collection["photo_existing"];
						model.uploadidproof = collection["uploadidproof_existing"];

						var files = Request.Form.Files;
						foreach (var file in files)
						{
							var filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
							string fileExtention = "." + filename.Split('.').Last();
							Random rnd = new Random();
							string uploadFileName = System.Text.RegularExpressions.Regex.Replace(filename.Split('.').First(), @"[^0-9a-zA-Z_.]+", "").Replace(" ", String.Empty) + "_" + "PatientProfile_" + rnd.Next(1, 10000).ToString() + DateTime.Now.ToString("ddMMyyHHmmss") + fileExtention;
							if (fileExtention != ".")
							{
								Stream stream = file.OpenReadStream();
								string fileURL = await util.fileSystem.UploadFileAsync(stream, Configuration.GetSection("AzureBlobStorageSetttings:folderName").Value, uploadFileName);
								if (file.Name == "photo") { model.photo += "|" + fileURL + "|"; }
								if (file.Name == "uploadidproof") { model.uploadidproof += "|" + fileURL + "|"; }
							}
						}

						strReturnMessage = await ApiClient.Post_ApiValuesGetString(client, "api/PatientProfile/Quick_Add_Patient_Profile", model);
					}
				}
				else
				{
					var errorMessages = ModelState.Where(entry => entry.Value.Errors.Any()).SelectMany(entry => entry.Value.Errors.Select(error => $"{entry.Key}: {error.ErrorMessage}"));
					return Json(new { success = false, message = string.Join(" | ", errorMessages) });
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "QuickAdd_Patient_Profile error: " + ex.Message);
				return Json(new { success = false, message = ex.Message });
			}

			if (strReturnMessage.Replace("\"", "").Contains("201.1"))
			{
				// Get plain password from WebAPI response "201.1|plainpassword"
				string plainPassword = "";
				string[] responseParts = strReturnMessage.Replace("\"", "").Split('|');
				if (responseParts.Length > 1)
					plainPassword = responseParts[1];

				// Send welcome email with credentials
				try
				{
					MailSender maillog = new MailSender();
					bool mailSent = await maillog.sendMail(
						"OTP",
						"User credientils",
						model.PatientProfileid.ToString(),
						"OTP",
						_mailSettings,
					   model.createduser.ToString(),
						client,

						 plainPassword,
						 model.emailaddress,
						 tenantid: model.tenantid?.ToString() ?? ""
					);
				}
				catch (Exception mailEx)
				{
					_logger.LogError(mailEx, "Email sending failed for - " + model.emailaddress);
				}

				HttpContext.Session.Remove("NalamVazhaauthenticatedEmail");
				HttpContext.Session.Remove("NalamVazhaotpVerified");
				TempData["successmessage"] = "Profile created. Please check your mail for credentials.";
				return Json(new
				{
					success = true,
					patientProfileId = model.PatientProfileid,
					name = ((model.firstname ?? "") + " " + (model.lastname ?? "")).Trim(),
					message = "Patient created successfully"
				});
			}
			else if (strReturnMessage.StartsWith("BadRequest", StringComparison.OrdinalIgnoreCase))
			{
				strReturnMessage = strReturnMessage.Replace("\"", "").Replace("BadRequest :", "");
				TempData["message"] = strReturnMessage;
				return Json(new { success = false, message = strReturnMessage });
			}
			else
			{
				if (strReturnMessage == "401.1")
					strReturnMessage = "Authorization Failed";
				return Json(new { success = false, message = strReturnMessage });
			}
		}

		[HttpGet()]
											public virtual async Task<string> get_all_Country(string tenantid)
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/Country/get_all_Country?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											 
											[HttpGet()]
											public virtual async Task<string> get_all_Occupation(string tenantid)
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/Occupation/get_all_Occupation?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											 
											[HttpGet()]
											public virtual async Task<string> get_all_ReferralSource(string tenantid)
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/ReferralSource/get_all_ReferralSource?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											 
											[HttpGet()]
											public virtual async Task<string> get_all_Blacklistreason(string tenantid)
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/Blacklistreason/get_all_Blacklistreason?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											

				
			  public virtual async Task<string> getById_allinfo_PatientProfile(string PatientProfileid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/PatientProfile/getById_allinfo_PatientProfile?PatientProfileid="+PatientProfileid);
					 
			  }
        [AllowAnonymous]
        [HttpGet()]
			    public virtual async Task<string> lookup_PatientProfile_countryoforigin()
			    {
                    
				    return await ApiClient.Get_ApiValues(getHttpClient(), "api/PatientProfile/lookup_PatientProfile_countryoforigin?loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			    }
        [AllowAnonymous]
        [HttpGet()]
			    public virtual async Task<string> lookup_PatientProfile_occupation()
			    {
                    
				    return await ApiClient.Get_ApiValues(getHttpClient(), "api/PatientProfile/lookup_PatientProfile_occupation?loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			    }
        [AllowAnonymous]
        [HttpGet()]
			    public virtual async Task<string> lookup_PatientProfile_referralsource(String tenantid)
			    {
                    
				    return await ApiClient.Get_ApiValues(getHttpClient(), "api/PatientProfile/lookup_PatientProfile_referralsource?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			    }
        [AllowAnonymous]
        [HttpGet()]
			    public virtual async Task<string> lookup_PatientProfile_reasonforblacklisting(String tenantid)
			    {

				    return await ApiClient.Get_ApiValues(getHttpClient(), "api/PatientProfile/lookup_PatientProfile_reasonforblacklisting?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			    }

[HttpGet()]
			public virtual async Task<string> GetPincodeList(string pincode, int? pagesize = 100, int? pagenumber = 0)
			{
				return await ApiClient.Get_ApiValues(getHttpClient(), "api/PincodeMaster/Pincode_List?pincode=" + (string.IsNullOrEmpty(pincode) ? "" : Uri.EscapeDataString(pincode)) + "&pagesize=" + (pagesize ?? 100) + "&pagenumber=" + (pagenumber ?? 0) + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
			}
		[HttpGet()]
		public virtual async Task<string> lookup_PatientProfile_patientcategory(String tenantid)
		{

			return await ApiClient.Get_ApiValues(getHttpClient(), "api/PatientProfile/lookup_PatientProfile_patientcategory?tenantid=" + tenantid + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
		}




	}


}
