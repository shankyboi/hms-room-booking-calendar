namespace Admin.Controllers
			{
				using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
				using Microsoft.AspNetCore.Hosting;
				using Microsoft.AspNetCore.Http;
				using Microsoft.AspNetCore.HttpOverrides;
				using Microsoft.AspNetCore.Mvc;
				using Microsoft.AspNetCore.Mvc.Infrastructure;
                using Microsoft.Extensions.Configuration;
                using Microsoft.Extensions.Logging;
				using Microsoft.Extensions.Options;
				using NalamVazha.Models;
				using Newtonsoft.Json;
				using System;
				using System.Collections.Generic;
				using System.Data;
    using System.Diagnostics;
	            using System.Globalization;
				using System.IO;
				using System.Linq;
				using System.Net;
				using System.Net.Http;
				using System.Net.Http.Formatting;
				using System.Net.Http.Headers;
	using System.Text;
	using System.Text.Json;
                using System.Threading;
				using System.Threading.Tasks;




    //This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:22
				
                
                
                
                
				public class usersController : BaseController
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
                    private readonly ILogger<usersController> _logger;
                    
                    
                    StorageUtil util;
					public usersController(IConfiguration configuration,IHttpContextAccessor accessor,IOptions<ApiSettings> ApiSettings, IOptions<MailSettings> MailSettings, IWebHostEnvironment env, ILogger<usersController> logger):base( configuration)
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

					private string GetLoginProfilePictureUrl(DataRow userRow)
					{
						var profilePicture = userRow["profilepicture"]?.ToString();
						if (!string.IsNullOrWhiteSpace(profilePicture))
							return util.fileSystem.GetFileURl(profilePicture);

						var userRole = userRow["userrole"]?.ToString();
						var username = userRow["username"]?.ToString();
						var usersId = userRow["usersid"]?.ToString();
						var pendingPhoto = HttpContext.Session.GetString("NalamVazhaPendingPatientProfilePicture");
						var pendingEmail = HttpContext.Session.GetString("NalamVazhaPendingPatientProfileEmail");
						var pendingPatientProfileId = HttpContext.Session.GetString("NalamVazhaPendingPatientProfileId");

						if (string.Equals(userRole, "Health Seeker", StringComparison.OrdinalIgnoreCase)
							&& !string.IsNullOrWhiteSpace(pendingPhoto)
							&& (string.Equals(username, pendingEmail, StringComparison.OrdinalIgnoreCase)
								|| string.Equals(usersId, pendingPatientProfileId, StringComparison.OrdinalIgnoreCase)))
						{
							return util.fileSystem.GetFileURl(pendingPhoto);
						}

						return baseUrl + "/Icon_set/User.png";
					}

					private void ClearPendingPatientProfilePicture(DataRow userRow)
					{
						var username = userRow["username"]?.ToString();
						var usersId = userRow["usersid"]?.ToString();
						var pendingEmail = HttpContext.Session.GetString("NalamVazhaPendingPatientProfileEmail");
						var pendingPatientProfileId = HttpContext.Session.GetString("NalamVazhaPendingPatientProfileId");

						if (string.Equals(username, pendingEmail, StringComparison.OrdinalIgnoreCase)
							|| string.Equals(usersId, pendingPatientProfileId, StringComparison.OrdinalIgnoreCase))
						{
							HttpContext.Session.Remove("NalamVazhaPendingPatientProfilePicture");
							HttpContext.Session.Remove("NalamVazhaPendingPatientProfileEmail");
							HttpContext.Session.Remove("NalamVazhaPendingPatientProfileId");
						}
					}
						 
 
                     public virtual IActionResult audit()
			         {
					        return View();
			         }
	              

					
			  public virtual IActionResult CreatePlatform()
			  {
					return View();
			  }	
			  [HttpPost()]
			public virtual async Task<string> CreatePlatform(usersModel model, IFormCollection collection)
			{
				string strReturnMessage = "";
			var client = getHttpClient();
			try
				{
					ModelState.Remove("usersid");
					ModelState.Remove("createduser");
                    ModelState.Remove("craftmyapp_actionmethodname");
                    model.craftmyapp_actionmethodname="CreatePlatform";
					                    
					ModelState.Remove("profilepicture");

                   
					
			 	    
					 if (ModelState.IsValid)
					 {
							 usersModelValidator validator = new usersModelValidator();
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
								 model.usersid =Guid.NewGuid(); 
                                 model.profilepicture = collection["profilepicture_existing"];

                                  var files = Request.Form.Files;
foreach (var file in files) 
{
var filename = ContentDispositionHeaderValue
.Parse(file.ContentDisposition)
.FileName
.Trim('"'); 
string fileExtention = "." + filename.Split('.').Last(); 
Random rnd = new Random();
string uploadFileName = System.Text.RegularExpressions.Regex.Replace(filename.Split('.').First(), @"[^0-9a-zA-Z_.]+", "").Replace(" ", String.Empty)+"_"+"users_" +rnd.Next(1, 10000).ToString() + DateTime.Now.ToString("ddMMyyHHmmss")+ fileExtention;
if (fileExtention != ".")
{
Stream stream = file.OpenReadStream();
string fileURL=await util.fileSystem.UploadFileAsync(stream, Configuration.GetSection("AzureBlobStorageSetttings:folderName").Value, uploadFileName);
if (file.Name == "profilepicture")
{
uploadFileName = "|" + fileURL +"|";
model.profilepicture = fileURL;
}
}
}

								 
                                 strReturnMessage = await ApiClient.Post_ApiValuesGetString(client,"api/users/Register_Profile", model);
                                    
								 
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
                 
                 _logger.LogError(ex,"An exception occurred in - users / CreatePlatform, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
               
				 strReturnMessage = ex.Message;
			 }
		     ViewData["message"] = strReturnMessage;
			 if(strReturnMessage.Replace("\"", "").Contains("201.1")){
				 TempData["message"] = "Success";
				 
                
                        MailSender maillog = new MailSender();
                        bool userMailsent = await maillog.sendMail("users"
                            , "registration"
                            , model.tenantid.ToString()
                            , "registration"
                            , _mailSettings
                            , "13EF2D93-CD5C-49DB-AE70-8FB33399275C"
                            , client, model.emailid, model.userpassword, baseUrl);

                        if (userMailsent)
                        {
                            TempData["message"] = "User Info Sent to your registered email";
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

			  public virtual IActionResult Register_Profile()
			  {
					return View();
			  }	
			  [HttpPost()]
			public virtual async Task<string> Register_Profile(usersModel model, IFormCollection collection)
			{
				string strReturnMessage = "";
			var client = getHttpClient();
			try
				{
					ModelState.Remove("usersid");
					ModelState.Remove("createduser");
                    ModelState.Remove("craftmyapp_actionmethodname");
                    model.craftmyapp_actionmethodname="Register_Profile";
					                    
					ModelState.Remove("profilepicture");

                   
					
			 	    
					 if (ModelState.IsValid)
					 {
							 usersModelValidator validator = new usersModelValidator();
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
								 model.usersid =Guid.NewGuid(); 
                                 model.profilepicture = collection["profilepicture_existing"];

                                  var files = Request.Form.Files;
foreach (var file in files) 
{
var filename = ContentDispositionHeaderValue
.Parse(file.ContentDisposition)
.FileName
.Trim('"'); 
string fileExtention = "." + filename.Split('.').Last(); 
Random rnd = new Random();
string uploadFileName = System.Text.RegularExpressions.Regex.Replace(filename.Split('.').First(), @"[^0-9a-zA-Z_.]+", "").Replace(" ", String.Empty)+"_"+"users_" +rnd.Next(1, 10000).ToString() + DateTime.Now.ToString("ddMMyyHHmmss")+ fileExtention;
if (fileExtention != ".")
{
Stream stream = file.OpenReadStream();
string fileURL=await util.fileSystem.UploadFileAsync(stream, Configuration.GetSection("AzureBlobStorageSetttings:folderName").Value, uploadFileName);
if (file.Name == "profilepicture")
{
uploadFileName = "|" + fileURL +"|";
model.profilepicture = fileURL;
}
}
}

								 
                                 strReturnMessage = await ApiClient.Post_ApiValuesGetString(client,"api/users/Register_Profile", model);
                                    
								 
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
                 
                 _logger.LogError(ex,"An exception occurred in - users / Register_Profile, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
               
				 strReturnMessage = ex.Message;
			 }
		     ViewData["message"] = strReturnMessage;
			 if(strReturnMessage.Replace("\"", "").Contains("201.1")){
				 TempData["message"] = "Success";
				 
                
                        MailSender maillog = new MailSender();
                        bool userMailsent = await maillog.sendMail("users"
                            , "registration"
                            , model.tenantid.ToString()
                            , "registration"
                            , _mailSettings
                            , "13EF2D93-CD5C-49DB-AE70-8FB33399275C"
                            , client, model.emailid, model.userpassword, baseUrl);

                        if (userMailsent)
                        {
                            TempData["message"] = "User Info Sent to your registered email";
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

				
			  public virtual async Task<IActionResult> EditPlatform(string usersid)
			  {

                    string redirectTo="";
                    if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
                            DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
                            DataView dv = new DataView(NalamVazharole_JSON);
                            dv.RowFilter = "controllername='users' AND viewname='list'";

                            if(dv.Count  >0){
                                redirectTo = dv[0]["actionmethodname"] as string;
							 
                            }

                            try{
                                     var jsonObjusers = await ApiClient.Get_ApiValues(getHttpClient(), "api/users/getById_users?usersid="+usersid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                                if(jsonObjusers.Length > 2)
                                {
                                  
                                    var model = JsonConvert.DeserializeObject<usersModel>(jsonObjusers);


                
                                     
                                    return View(model);
                                }
                                else
                                {
                    
                                    TempData["message"] = "Data Not Found - Contact Administrator";
                                    return RedirectToAction(redirectTo);
						 
                                }

                            }catch(Exception ex){
                               _logger.LogError(ex,"An exception occurred in - users / EditPlatform, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                                TempData["errMessage"] = "Error while fetching data - Contact Administrator";
                                return RedirectToAction(redirectTo);
                            }

                    }
                    TempData["errMessage"] = "Session Expired";
                    return RedirectToAction("Logout", "users");
                }	
			  [HttpPost()]
				public virtual async Task<string> EditPlatform(usersModel model, IFormCollection collection)
				{
					string strReturnMessage = "";
					try
					{
							ModelState.Remove("usersid");
                            ModelState.Remove("craftmyapp_actionmethodname");
                             model.craftmyapp_actionmethodname="EditPlatform";
							
							model.userrole =collection["userrole"];
							if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
					model.modifieduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
					else
					return "Session Expired";
							
                            model.profilepicture = collection["profilepicture_existing"];

                            ModelState.Remove("profilepicture");

							if (ModelState.IsValid)
							{
									usersModelValidator validator = new usersModelValidator();
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
string uploadFileName = System.Text.RegularExpressions.Regex.Replace(filename.Split('.').First(), @"[^0-9a-zA-Z_.]+", "").Replace(" ", String.Empty)+"_"+"users_" +rnd.Next(1, 10000).ToString() + DateTime.Now.ToString("ddMMyyHHmmss")+ fileExtention;
if (fileExtention != ".")
{
Stream stream = file.OpenReadStream();
string fileURL=await util.fileSystem.UploadFileAsync(stream, Configuration.GetSection("AzureBlobStorageSetttings:folderName").Value, uploadFileName);
if (file.Name == "profilepicture")
{
uploadFileName = "|" + fileURL +"|";
model.profilepicture = fileURL;
}
}
}

                                        
                                        
                                        strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/users/Update_Profile", model);
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
                      _logger.LogError(ex,"An exception occurred in - users / EditPlatform, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
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

				
			  public virtual async Task<IActionResult> UpdateProfile(string usersid)
			  {

                    string redirectTo="";
                    if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
                            DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
                            DataView dv = new DataView(NalamVazharole_JSON);
                            dv.RowFilter = "controllername='users' AND viewname='list'";

                            if(dv.Count  >0){
                                redirectTo = dv[0]["actionmethodname"] as string;
							 
                            }

                            try{
                                     var jsonObjusers = await ApiClient.Get_ApiValues(getHttpClient(), "api/users/getById_users?usersid="+usersid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                                if(jsonObjusers.Length > 2)
                                {
                                  
                                    var model = JsonConvert.DeserializeObject<usersModel>(jsonObjusers);


                
                                     
                                    return View(model);
                                }
                                else
                                {
                    
                                    TempData["message"] = "Data Not Found - Contact Administrator";
                                    return RedirectToAction(redirectTo);
						 
                                }

                            }catch(Exception ex){
                               _logger.LogError(ex,"An exception occurred in - users / UpdateProfile, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                                TempData["errMessage"] = "Error while fetching data - Contact Administrator";
                                return RedirectToAction(redirectTo);
                            }

                    }
                    TempData["errMessage"] = "Session Expired";
                    return RedirectToAction("Logout", "users");
                }	
			  [HttpPost()]
				public virtual async Task<string> UpdateProfile(usersModel model, IFormCollection collection)
				{
					string strReturnMessage = "";
					try
					{
							ModelState.Remove("usersid");
                            ModelState.Remove("craftmyapp_actionmethodname");
                             model.craftmyapp_actionmethodname="UpdateProfile";
							
							model.userrole =collection["userrole"];
							if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
					model.modifieduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
					else
					return "Session Expired";
							
                            model.profilepicture = collection["profilepicture_existing"];

                            ModelState.Remove("profilepicture");

							if (ModelState.IsValid)
							{
									usersModelValidator validator = new usersModelValidator();
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
string uploadFileName = System.Text.RegularExpressions.Regex.Replace(filename.Split('.').First(), @"[^0-9a-zA-Z_.]+", "").Replace(" ", String.Empty)+"_"+"users_" +rnd.Next(1, 10000).ToString() + DateTime.Now.ToString("ddMMyyHHmmss")+ fileExtention;
if (fileExtention != ".")
{
Stream stream = file.OpenReadStream();
string fileURL=await util.fileSystem.UploadFileAsync(stream, Configuration.GetSection("AzureBlobStorageSetttings:folderName").Value, uploadFileName);
if (file.Name == "profilepicture")
{
uploadFileName = "|" + fileURL +"|";
model.profilepicture = fileURL;
}
}
}

                                        
                                        strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/users/Update_Profile", model);
                                        if(strReturnMessage == "Success" && model.profilepicture !=  null && model.profilepicture!="")
                                                    HttpContext.Session.SetString("NalamVazhaprofilepicture", util.fileSystem.GetFileURl(model.profilepicture.ToString()));
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
                      _logger.LogError(ex,"An exception occurred in - users / UpdateProfile, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
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

				
			  public virtual async Task<IActionResult> Update_Profile(string usersid)
			  {

                    string redirectTo="";
                    if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
                            DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
                            DataView dv = new DataView(NalamVazharole_JSON);
                            dv.RowFilter = "controllername='users' AND viewname='list'";

                            if(dv.Count  >0){
                                redirectTo = dv[0]["actionmethodname"] as string;
							 
                            }

                            try{
                                     var jsonObjusers = await ApiClient.Get_ApiValues(getHttpClient(), "api/users/getById_users?usersid="+usersid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                                if(jsonObjusers.Length > 2)
                                {
                                  
                                    var model = JsonConvert.DeserializeObject<usersModel>(jsonObjusers);


                
                                     
                                    return View(model);
                                }
                                else
                                {
                    
                                    TempData["message"] = "Data Not Found - Contact Administrator";
                                    return RedirectToAction(redirectTo);
						 
                                }

                            }catch(Exception ex){
                               _logger.LogError(ex,"An exception occurred in - users / Update_Profile, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                                TempData["errMessage"] = "Error while fetching data - Contact Administrator";
                                return RedirectToAction(redirectTo);
                            }

                    }
                    TempData["errMessage"] = "Session Expired";
                    return RedirectToAction("Logout", "users");
                }	
			  [HttpPost()]
				public virtual async Task<string> Update_Profile(usersModel model, IFormCollection collection)
				{
					string strReturnMessage = "";
					try
					{
							ModelState.Remove("usersid");
                            ModelState.Remove("craftmyapp_actionmethodname");
                             model.craftmyapp_actionmethodname="Update_Profile";
							
							model.userrole =collection["userrole"];
							if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
					model.modifieduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
					else
					return "Session Expired";
							
                            model.profilepicture = collection["profilepicture_existing"];

                            ModelState.Remove("profilepicture");

							if (ModelState.IsValid)
							{
									usersModelValidator validator = new usersModelValidator();
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
string uploadFileName = System.Text.RegularExpressions.Regex.Replace(filename.Split('.').First(), @"[^0-9a-zA-Z_.]+", "").Replace(" ", String.Empty)+"_"+"users_" +rnd.Next(1, 10000).ToString() + DateTime.Now.ToString("ddMMyyHHmmss")+ fileExtention;
if (fileExtention != ".")
{
Stream stream = file.OpenReadStream();
string fileURL=await util.fileSystem.UploadFileAsync(stream, Configuration.GetSection("AzureBlobStorageSetttings:folderName").Value, uploadFileName);
if (file.Name == "profilepicture")
{
uploadFileName = "|" + fileURL +"|";
model.profilepicture = fileURL;
}
}
}

                                        strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/users/Update_Profile", model);
                                        if(strReturnMessage == "Success" && model.usersid.ToString() == HttpContext.Session.GetString("NalamVazhaloginUserID") && model.profilepicture !=  null && model.profilepicture!="")
                                                    HttpContext.Session.SetString("NalamVazhaprofilepicture", util.fileSystem.GetFileURl(model.profilepicture.ToString()));
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
                      _logger.LogError(ex,"An exception occurred in - users / Update_Profile, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
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
public virtual async Task<IActionResult> Suspend_Profile(string usersid)
			{
				string message = "";
				try
				{
						message = await ApiClient.Get_ApiValues(getHttpClient(), "api/users/Suspend_Profile?usersid="+usersid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
						 if(message.Replace("\"","").Contains("201.1"))
						{
							TempData["message"] = "Success";

						}else{
							TempData["errMessage"] = message.Replace("\"","");
						}
						
				
				
				}
				catch (Exception ex)
				{
                     _logger.LogError(ex,"An exception occurred in - users / Suspend_Profile, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                
					 TempData["errMessage"] = ex.Message;
					 message = ex.Message;
				}

				string redirectTo="";
						if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
					DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
						 DataView dv = new DataView(NalamVazharole_JSON);
						 dv.RowFilter = "controllername='users' AND viewname='list'";

						if(dv.Count  >0){
						    redirectTo = dv[0]["actionmethodname"] as string;
							 
						}

					}
				
				return RedirectToAction(redirectTo);
			}
public virtual async Task<IActionResult> DeletePlatform(string usersid)
			{
				string message = "";
				try
				{
						message = await ApiClient.Get_ApiValues(getHttpClient(), "api/users/Suspend_Profile?usersid="+usersid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
				}
				catch (Exception ex)
				{
                 _logger.LogError(ex,"An exception occurred in - users / DeletePlatform, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
               
				message = ex.Message;
				}
				TempData["message"] = message;
				return RedirectToAction("IndexPlatform");
			}
		public async Task<string> get_Transcript(string input)
		{
			try
			{
				string targetLanguage = HttpContext.Request.Cookies["NalamVazhaCurrentCulture"];
				if (targetLanguage != null)
				{
					string[] parts = targetLanguage.Split('-');
					string targetLanguage1 = parts[0]; // This will split target LAguage 

					string apiUrl = $"https://inputtools.google.com/request?text={Uri.EscapeDataString(input)}&ime=transliteration_en_{targetLanguage1}&num=1&cp=0&cs=1&ie=utf-8&oe=utf-8&app=demopage";

					using (HttpClient client = new HttpClient())
					{
						client.DefaultRequestHeaders.Add("Referer", "https://www.google.com/");

						HttpResponseMessage response = await client.GetAsync(apiUrl);
						response.EnsureSuccessStatusCode();

						string responseBody = await response.Content.ReadAsStringAsync();

						var jsonResult = JsonDocument.Parse(responseBody);
						var root = jsonResult.RootElement;

						var translations = root[1][0][1];

						foreach (var translation in translations.EnumerateArray())
						{
							var transliteratedText = translation.GetString();
							Console.WriteLine("Transliterated Text: " + transliteratedText);
						}

						// Return the first transliterated text
						return translations[0].GetString();

					}

				}
				return input;
			}
			catch (Exception ex)
			{
				return input;

			}
		}
	 	
			
        public virtual IActionResult SessionExpired()
        {
            TempData["messagereferer"] = "donotrefer";
	        TempData["message"] = "Session Expired , Please login again !";
            var tenantid = Request.Query["tenantid"].ToString();
            return RedirectToAction("Login", new { tenantid = string.IsNullOrWhiteSpace(tenantid) ? (string)null : tenantid });
        }
        public virtual IActionResult AuthorizationFailed()
        {
            TempData["messagereferer"] = "donotrefer";
	        TempData["message"] = "API Authorization failed ,Please login again !";
            var tenantid = Request.Query["tenantid"].ToString();
            return RedirectToAction("Login", new { tenantid = string.IsNullOrWhiteSpace(tenantid) ? (string)null : tenantid });
        }
        public virtual IActionResult RoleAuthorizationFailed()
        {
            TempData["messagereferer"] = "donotrefer";
	        TempData["message"] = "Not authorized to view this page !";
            var tenantid = Request.Query["tenantid"].ToString();
            return RedirectToAction("Login", new { tenantid = string.IsNullOrWhiteSpace(tenantid) ? (string)null : tenantid });
        }
		


			
                        [HttpGet()]
                        public virtual async Task<string> get_roles(string parentname)
                        {
				 
		                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/users/get_roles?roles="+HttpContext.Session.GetString("NalamVazhauserrole") +"&parentname="+ parentname);
				 
			
                        }
                        public async Task<ActionResult> ChangeCurrentCulture(string culture)
                        {

                        Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
                        Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture;
                        HttpResponseMessage response_menu = await ApiClient.GET_ApiValuesGetRespnse(getHttpClient(), "api/users/get_project_Menu?viewactionroles="+ HttpContext.Session.GetString("NalamVazhauserrole") + "&SubSystem=Admin");
            
                        if (response_menu.IsSuccessStatusCode)
                        {

                                string menu_JSON = await response_menu.Content.ReadAsStringAsync();


                            var assembly = typeof(usersController).Assembly;
                            System.Resources.ResourceManager _resourceManager = new System.Resources.ResourceManager("Admin.Resource", assembly);
 
                                List<dynamic> menuItems = JsonConvert.DeserializeObject<List<dynamic>>(menu_JSON);

                            foreach (dynamic menuItem in menuItems)
                            { 
		                        string projectElementName = System.Text.RegularExpressions.Regex.Replace(menuItem.projectelementname.ToString(), @"\s+", "").ToLower();
		                        string translatedProjectElementName = _resourceManager.GetString(projectElementName);
		                        string parentnode = System.Text.RegularExpressions.Regex.Replace(menuItem.parentnode.ToString(), @"\s+", "").ToLower();
		                        string translatedparentnode = _resourceManager.GetString(parentnode);
		                        menuItem.projectelementname = translatedProjectElementName;
		                        menuItem.parentnode = translatedparentnode;
                            }
	                        menu_JSON = JsonConvert.SerializeObject(menuItems);
	                        HttpContext.Session.SetString("NalamVazhamenu_JSON", menu_JSON);

				 


                        }



	                        Response.Cookies.Append("NalamVazhaCurrentCulture", culture, new CookieOptions
                        {
                            Expires = DateTimeOffset.UtcNow.AddDays(10), // Cookie expires in 1 hour
                            HttpOnly = true // Makes the cookie accessible only through the HTTP protocol, not JavaScript
                        });
                        string referrerUrl = HttpContext.Request.Headers["Referer"];
                        if (referrerUrl != null)
                        {
	                        return Redirect(referrerUrl);
                        }
                        else
                        {
	                        return RedirectToAction("Home", "users");
                        }
                        }
[AllowAnonymous]
            public virtual IActionResult Home()
			{
                HttpContext.Session.SetString("ReferrerUrl", "");
                TempData["messagereferer"] = "donotrefer";
            var tenantid = Request.Query["tenantid"].ToString();
            return RedirectToAction("Login", new { tenantid = string.IsNullOrWhiteSpace(tenantid) ? (string)null : tenantid });
            }
         
        [AllowAnonymous]
        public virtual async Task<IActionResult> Login()
        {
            var subdomainTenantId = HttpContext.Items["SubdomainTenantId"] as string;

            if (!string.IsNullOrEmpty(subdomainTenantId))
            {
                // Subdomain-based access: populate ViewBag so the login page can show tenant branding
                ViewBag.TenantName = HttpContext.Items["SubdomainTenantName"] as string;
                var logoPath = (HttpContext.Items["SubdomainLogo"] as string)
                    ?.Split('|').FirstOrDefault(s => !string.IsNullOrWhiteSpace(s))?.Trim();
                ViewBag.TenantLogo = string.IsNullOrEmpty(logoPath) ? null : util.fileSystem.GetFileURl(logoPath);
                ViewBag.SubdomainTenantId = subdomainTenantId;
            }
            else if (string.IsNullOrWhiteSpace(Request.Query["tenantid"].ToString()))
            {
                // No subdomain and no query param → show tenant picker
                return RedirectToAction("Index");
            }
            else
            {
                // No subdomain, but tenantid query param exists -> load tenant branding (logo/name).
                var tenantidQuery = Request.Query["tenantid"].ToString();
                if (!string.IsNullOrWhiteSpace(tenantidQuery))
                    await SetTenantBrandingForLoginAsync(tenantidQuery);
            }

            HttpContext.Session.SetString("ReferrerUrl", "");
            string referrerUrl = HttpContext.Request.Headers["Referer"];
            if (referrerUrl != null && TempData["messagereferer"] == null)
            {
                HttpContext.Session.SetString("ReferrerUrl", referrerUrl);

            }
            return View();
        }

		[AllowAnonymous]
		public virtual IActionResult LoginPlatform()
		{
		     HttpContext.Session.SetString("ReferrerUrl", "");
			string referrerUrl = HttpContext.Request.Headers["Referer"];
			if (referrerUrl != null && TempData["messagereferer"] == null)
			{
				HttpContext.Session.SetString("ReferrerUrl", referrerUrl);

			}
			return View();
		}
		[HttpPost()]
		[AllowAnonymous]


		public virtual async Task<IActionResult> LoginPlatform(userloginModel model)
		{
			string message = "";
			string strReturnMessage = "";
			string loginType = Request.Form["loginType"];
			client = getHttpClient();

			try
			{
				userlockoutModel objuserlockoutModel = new userlockoutModel();
				objuserlockoutModel.username = model.username;
				objuserlockoutModel.latlan = model.latlan;
				string remoteipaddress = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
				objuserlockoutModel.remoteipaddress = remoteipaddress;
				objuserlockoutModel.clientipaddress = model.clientipaddress;

				strReturnMessage = await ApiClient.Post_ApiValuesGetString(client, "api/userlockout/verify_userlockout", objuserlockoutModel);
				if (strReturnMessage != "Allow")
				{
					ViewData["message"] = strReturnMessage;
					return View(model);
				}

				HttpResponseMessage response = new HttpResponseMessage();


				if (loginType == "SendOTP")
				{
					if (string.IsNullOrEmpty(model.username))
					{
						ViewData["message"] = "Please enter your email address.";
						ViewData["ShowEmailField"] = true;
						return View(model);
					}

					// CORRECT: Call WebAPI endpoint (NOT DAL)
					response = await ApiClient.Post_ApiValuesGetRespnse(
						client,
						"api/users/RequestPatientOTP",
						model);

					if (response.IsSuccessStatusCode)
					{
						var result = await response.Content.ReadAsAsync<dynamic>();
						string otp = result.otp;
						string targetEmail = result.email;
						string otplogid = result.otpsid;
						string tenantid = Convert.ToString(result.tenantid);

						if (string.IsNullOrEmpty(targetEmail))
						{
							ViewData["message"] = "Please enter a valid email address.";
							ViewData["ShowEmailField"] = true;
							return View(model);
						}

						// Send OTP Email
						MailSender maillog = new MailSender();
						bool mailSent = await maillog.sendMail(
							"OTP",
							"OTP Generator",
							otplogid.ToString(),
							"OTP",
							_mailSettings,
							otplogid.ToString(),
							client,
							otp,
							targetEmail,
							tenantid: tenantid
						);

						if (mailSent)
						{
							// Show OTP input field
							TempData["OTPEmail"] = model.username;
							ViewBag.OTPEmail = model.username;
							ViewData["ShowOTPField"] = true;
							ViewData["ShowOTPInput"] = true;
							ViewData["message"] = "OTP has been sent to your email. Please check your inbox.";
							return View(model);
						}
						else
						{
							ViewData["message"] = "Failed to send OTP. Please try again.";
							ViewData["ShowEmailField"] = true;
							return View(model);
						}
					}
					else
					{
						var errorMsg = await response.Content.ReadAsStringAsync();
						ViewData["message"] = errorMsg.Replace("\"", "");
						ViewData["ShowEmailField"] = true;
						return View(model);
					}
				}


				else if (loginType == "VerifyOTP")
				{
					string otp = Request.Form["otp_input"];
					string savedEmail = Request.Form["username"].ToString();

					// Restore email into model for view
					model.username = savedEmail;

					if (string.IsNullOrEmpty(otp))
					{
						ViewData["message"] = "Please enter the OTP sent to your email.";
						ViewData["ShowOTPField"] = true;
						ViewData["ShowOTPInput"] = true;
						TempData["OTPEmail"] = savedEmail;
						ViewBag.OTPEmail = savedEmail;
						return View(model);
					}

					// Validate OTP format (should be 6 digits)
					if (!System.Text.RegularExpressions.Regex.IsMatch(otp, @"^\d{6}$"))
					{
						ViewData["message"] = "OTP must be 6 digits.";
						ViewData["ShowOTPField"] = true;
						ViewData["ShowOTPInput"] = true;
						TempData["OTPEmail"] = savedEmail;
						ViewBag.OTPEmail = savedEmail;
						return View(model);
					}

					// CORRECT: Call WebAPI endpoint (NOT DAL directly)
					var otpData = new { email = savedEmail, otp = otp };
					response = await ApiClient.Post_ApiValuesGetRespnse(
						client,
						"api/users/VerifyPatientOTP",
						otpData);

					if (response.IsSuccessStatusCode)
					{
						var result = await response.Content.ReadAsAsync<dynamic>();
						bool success = result.success;
						string spMessage = result.message;

						if (success)
						{
							// OTP verified successfully
							// Store email in session for patient profile creation
							HttpContext.Session.SetString("NalamVazhaauthenticatedEmail", savedEmail);
							HttpContext.Session.SetString("NalamVazhaotpVerified", "true");


							message = "OTP Verified Successfully";
							ViewData["message"] = message;

							// Redirect to create patient profile page
							HttpContext.Session.SetString("NalamVazhaauthenticatedEmail", savedEmail);
							HttpContext.Session.SetString("NalamVazhaotpVerified", "true");
							return RedirectToAction("Quick_Add_Patient_Profile", "PatientProfile",
								new { tenantid = "ceb6549a-884c-427b-a7e4-7c1185786a93" });

						}
						else
						{
							ViewData["message"] = spMessage;
							ViewData["ShowOTPField"] = true;
							ViewData["ShowOTPInput"] = true;
							TempData["OTPEmail"] = savedEmail;
							ViewBag.OTPEmail = savedEmail;
							return View(model);
						}
					}
					else
					{
						var errorMsg = await response.Content.ReadAsStringAsync();
						ViewData["message"] = "Invalid or Expired OTP. Please try again.";
						ViewData["ShowOTPField"] = true;
						ViewData["ShowOTPInput"] = true;
						TempData["OTPEmail"] = savedEmail;
						ViewBag.OTPEmail = savedEmail;
						return View(model);
					}
				}


				else
				{
					if (string.IsNullOrEmpty(model.username) || string.IsNullOrEmpty(model.userpassword))
					{
						ViewData["message"] = "Please enter both email and password.";
						return View(model);
					}

					model.source = "Internal";
					response = await ApiClient.Post_ApiValuesGetRespnse(client, "api/users/CheckAuthentication", model);
				}


				if (response.IsSuccessStatusCode)
				{
					DataTable dt = await response.Content.ReadAsAsync<DataTable>();
					if (dt.Rows.Count > 0)
					{
						// Set User Session Variables
						HttpContext.Session.SetString("NalamVazhausername", dt.Rows[0]["username"].ToString());
						HttpContext.Session.SetString("NalamVazhaloginUserID", dt.Rows[0]["usersid"].ToString());

						HttpContext.Session.SetString("NalamVazhatoken", dt.Rows[0]["token"].ToString());
						HttpContext.Session.SetString("NalamVazhacloudpath", Configuration.GetSection("AzureBlobStorageSetttings:baseURL").Value.ToString() + "/" + Configuration.GetSection("AzureBlobStorageSetttings:folderName").Value.ToString());
						HttpContext.Session.SetString("NalamVazhaSAStoken", util.fileSystem.getSASToken());

						HttpContext.Session.SetString("NalamVazhauserrole", dt.Rows[0]["userrole"].ToString());

						if (dt.Rows[0]["profilepicture"].ToString() != null && dt.Rows[0]["profilepicture"].ToString() != "")
							HttpContext.Session.SetString("NalamVazhaprofilepicture", util.fileSystem.GetFileURl(dt.Rows[0]["profilepicture"].ToString()));
						else
							HttpContext.Session.SetString("NalamVazhaprofilepicture", baseUrl + "/Icon_set/User.png");

						HttpContext.Session.SetString("NalamVazhatenantlogo", baseUrl + "/images/logo.png");

						message = "Login Success";

						HttpContext.Session.SetString("NalamVazhashowtenant", "N");
						HttpContext.Session.SetString("NalamVazhachoosedtenantid", dt.Rows[0]["tenantid"].ToString());
						if (dt.Rows[0]["viewertenantids"].ToString().Contains(","))
						{
							HttpContext.Session.SetString("NalamVazhashowtenant", "Y");
							HttpContext.Session.SetString("NalamVazhachoosedtenantid", "");
						}

						HttpContext.Session.SetString("NalamVazhachoosedtenantname", dt.Rows[0]["tenantname"].ToString());
						HttpContext.Session.SetString("natureofbusiness", dt.Rows[0]["natureofbusiness"].ToString());
                        HttpContext.Session.SetString("NalamVazhaLoginTime", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());

                        if (dt.Rows[0]["tenantid"].ToString() != "00000000-0000-0000-0000-000000000000")
						{
							ViewData["message"] = "You are not authorized to access NalamVazha , Please contact administrator";
							return View(model);

							HttpContext.Session.SetString("partyname", dt.Rows[0]["natureofbusiness"].ToString());
							HttpContext.Session.SetString("module", "client");

							if (dt.Rows[0]["tenantname"].ToString() == "")
							{
								ViewData["message"] = "You are not authorized to access NalamVazha , Please contact administrator";
								return View(model);
							}
						}
						else
						{
							HttpContext.Session.SetString("partyname", dt.Rows[0]["natureofbusiness"].ToString());
							HttpContext.Session.SetString("NalamVazhashowtenant", "Y");
							HttpContext.Session.SetString("module", "admin");
							HttpContext.Session.SetString("NalamVazhachoosedtenantid", "");
						}


						HttpResponseMessage response_roles = await ApiClient.GET_ApiValuesGetRespnse(client, "api/users/get_roleAuthorizations?viewactionroles=" + dt.Rows[0]["userrole"].ToString());

						if (response_roles.IsSuccessStatusCode)
						{
							DataTable dt_roles = await response_roles.Content.ReadAsAsync<DataTable>();
							string role_JSON = await response_roles.Content.ReadAsStringAsync();

							if (dt_roles.Rows.Count == 0)
							{
								ViewData["message"] = "You are not authorized to access NalamVazha , Please contact administrator";
								return View(model);
							}

							HttpContext.Session.SetSession("NalamVazharoles", dt_roles);
							HttpContext.Session.SetString("NalamVazharole_JSON", role_JSON);
						}


						HttpResponseMessage response_menu = await ApiClient.GET_ApiValuesGetRespnse(client, "api/users/get_project_Menu?viewactionroles=" + dt.Rows[0]["userrole"].ToString() + "&SubSystem=Admin");

						if (response_menu.IsSuccessStatusCode)
						{
							DataTable dt_menu = await response_menu.Content.ReadAsAsync<DataTable>();
							string menu_JSON = await response_menu.Content.ReadAsStringAsync();
							HttpContext.Session.SetSession("NalamVazhamenu", dt_menu);

							var assembly = typeof(usersController).Assembly;
							System.Resources.ResourceManager _resourceManager = new System.Resources.ResourceManager("Admin.Resource", assembly);

							List<dynamic> menuItems = JsonConvert.DeserializeObject<List<dynamic>>(menu_JSON);

							foreach (dynamic menuItem in menuItems)
							{
								string projectElementName = System.Text.RegularExpressions.Regex.Replace(menuItem.projectelementname.ToString(), @"\s+", "").ToLower();
								string translatedProjectElementName = _resourceManager.GetString(projectElementName);
								string parentnode = System.Text.RegularExpressions.Regex.Replace(menuItem.parentnode.ToString(), @"\s+", "").ToLower();
								string translatedparentnode = _resourceManager.GetString(parentnode);
								menuItem.projectelementname = translatedProjectElementName;
								menuItem.parentnode = translatedparentnode;
							}
							menu_JSON = JsonConvert.SerializeObject(menuItems);
							HttpContext.Session.SetString("NalamVazhamenu_JSON", menu_JSON);
						}
					}
					else
					{
						// User authentication failed - log the attempt
						message = await ApiClient.Post_ApiValuesGetString(client, "api/userlockout/ins_userlockout", objuserlockoutModel);
						ViewData["message"] = "Invalid login identifier or password. Please try again.";
						return View(model);
					}
				}
				else
				{
					message = "Response Failed";
					ViewData["message"] = message;
					return View(model);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - users / Login, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));
				message = ex.Message;
				ViewData["message"] = message;
				return View(model);
			}


			ViewData["message"] = message;
			if (message == "Login Success")
			{
				// Clear OTP session variables if they exist
				HttpContext.Session.Remove("NalamVazhaauthenticatedEmail");
				HttpContext.Session.Remove("NalamVazhaotpVerified");


				switch (HttpContext.Session.GetString("NalamVazhauserrole"))
				{
					case "Clinical Director":
						return RedirectToAction("Clinical_Appointment_List", "ClinicalAppointment", new { @isDashboardView = "Y" });
						break;
					case "Platform Admin":
						return RedirectToAction("View_List_of_Healthcare_Provider", "tenant", new { @isDashboardView = "Y" });
						break;
					case "Doctor":
						return RedirectToAction("IPD_Application_Forms_for_Review", "IPDApplicationForm", new { @isDashboardView = "Y" });
						break;
					case "Intern Doctor":
						return RedirectToAction("Index", "InternDoctorDashboard");
						break;
					case "Frontdesk Admin":
						return RedirectToAction("IPD_Application_Forms_for_Review", "IPDApplicationForm", new { @isDashboardView = "Y" });
						break;
					case "Health Seeker":
						return RedirectToAction("Added_IPD_Application_Form", "IPDApplicationForm", new { @isDashboardView = "Y" });
						break;
					case "HR Associate":
						return RedirectToAction("Staff_Attendance_List", "StaffAttendance", new { @isDashboardView = "Y" });
						break;
					case "Healthcare Provider Admin":
						return RedirectToAction("Dashboard", "HealthcareProviderAdmin", new { @isDashboardView = "Y" });
						break;
				}

				return RedirectToAction("UpdateProfile", "users", new { @usersid = HttpContext.Session.GetString("NalamVazhaloginUserID") });
			}
			else
			{
				return View(model);
			}
		}

		[AllowAnonymous]
            public virtual IActionResult LoginDemo()
            {
                HttpContext.Session.SetString("ReferrerUrl", "");
                return View();
            }
            public virtual IActionResult Logout()
			{

			var tenantid = Request.Query["tenantid"].ToString();
			HttpContext.Session.Clear();
			TempData["messagereferer"] = "donotrefer";
			if (tenantid == "")
				return RedirectToAction("Index");

			return RedirectToAction("Login", new { tenantid = string.IsNullOrWhiteSpace(tenantid) ? (string)null : tenantid });

			
          /*  
            //var shortcode  = HttpContext.Session.GetString("NalamVazhaSubdomainShortcode");
			var tenantid   = Request.Query["tenantid"].ToString();
				HttpContext.Session.Clear();
				TempData["messagereferer"] = "donotrefer";

				// If tenant was accessed via path shortcode, redirect back to PathBase/{shortcode}
				// so the middleware re-resolves it and shows the correct login page.
				//if (!string.IsNullOrEmpty(shortcode))
				//{
				//	var pb = HttpContext.Request.PathBase.Value?.TrimEnd('/') ?? "";
				//	return Redirect($"{pb}/{shortcode}");
				//}

				if (tenantid == "")
					return RedirectToAction("LoginPlatform");

				return RedirectToAction("Login", new { tenantid = string.IsNullOrWhiteSpace(tenantid) ? (string)null : tenantid });*/
			}

        [HttpGet]
        [AllowAnonymous]
        public IActionResult LoginWithOTP()
        {
            ViewData["ShowEmailField"] = true;
            return View("Login", new userloginModel());
        }


        [HttpPost()]
        [AllowAnonymous]
        public virtual async Task<IActionResult> Login(userloginModel model)
        {
            string message = "";
            string strReturnMessage = "";
            string loginType = Request.Form["loginType"];
            client = getHttpClient();
            bool isOTPLogin = false;

            try
            {
                // Treat every supported login identifier consistently.
                model.username = model.username?.Trim();

                // Subdomain-based tenancy: override model.tenantid from middleware-resolved tenant
                var subdomainTenantIdStr = HttpContext.Items["SubdomainTenantId"] as string;
                if (!string.IsNullOrEmpty(subdomainTenantIdStr) && Guid.TryParse(subdomainTenantIdStr, out var subTenantGuid))
                    model.tenantid = subTenantGuid;

                // Ensure tenant logo/name is available even when we return View(model) on auth failure.
                var tenantIdForBranding = model.tenantid?.ToString();
                if (string.IsNullOrWhiteSpace(tenantIdForBranding))
                    tenantIdForBranding = Request.Query["tenantid"].ToString();
                await SetTenantBrandingForLoginAsync(tenantIdForBranding);

                userlockoutModel objuserlockoutModel = new userlockoutModel();
                objuserlockoutModel.username = model.username;
                objuserlockoutModel.latlan = model.latlan;
                string remoteipaddress = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
                objuserlockoutModel.remoteipaddress = remoteipaddress;
                objuserlockoutModel.clientipaddress = model.clientipaddress;

                strReturnMessage = await ApiClient.Post_ApiValuesGetString(client, "api/userlockout/verify_userlockout", objuserlockoutModel);
                if (strReturnMessage != "Allow")
                {
                    ViewData["message"] = strReturnMessage;
                    return View(model);
                }

                HttpResponseMessage response = new HttpResponseMessage();

               
                if (loginType == "SendOTP")
                {
                    if (string.IsNullOrEmpty(model.username))
                    {
                        ViewData["message"] = "Please enter your email address.";
                        ViewData["ShowEmailField"] = true;
                        return View(model);
                    }

                    response = await ApiClient.Post_ApiValuesGetRespnse(
                        client,
                        "api/users/RequestPatientOTP",
                        model);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsAsync<dynamic>();
                        string otp = result.otp;
                        string targetEmail = result.email;
                        string otplogid = result.otpsid;
                        string tenantid = Convert.ToString(result.tenantid);

                        if (string.IsNullOrEmpty(targetEmail))
                        {
                            ViewData["message"] = "Please enter a valid email address.";
                            ViewData["ShowEmailField"] = true;
                            return View(model);
                        }

                        // Send OTP Email
                        MailSender maillog = new MailSender();
                        bool mailSent = await maillog.sendMail(
                            "OTP",
                            "OTP Generator",
                            otplogid.ToString(),
                            "OTP",
                            _mailSettings,
                            otplogid.ToString(),
                            client,
                            otp,
                            targetEmail,
                            tenantid: tenantid
                        );

                        if (mailSent)
                        {
                            TempData["OTPEmail"] = model.username;
                            ViewBag.OTPEmail = model.username;
                            ViewData["ShowOTPField"] = true;
                            ViewData["ShowOTPInput"] = true;
                            ViewData["message"] = "OTP has been sent to your email. Please check your inbox.";
                            return View(model);
                        }
                        else
                        {
                            ViewData["message"] = "Failed to send OTP. Please try again.";
                            ViewData["ShowEmailField"] = true;
                            return View(model);
                        }
                    }
                    else
                    {
                        var errorMsg = await response.Content.ReadAsStringAsync();
                        ViewData["message"] = errorMsg.Replace("\"", "");
                        ViewData["ShowEmailField"] = true;
                        return View(model);
                    }
                }

               
                else if (loginType == "VerifyOTP")
                {
                    string otp = Request.Form["otp_input"];
                    string savedEmail = Request.Form["username"].ToString();
                    model.username = savedEmail;

                    if (string.IsNullOrEmpty(otp))
                    {
                        ViewData["message"] = "Please enter the OTP sent to your email.";
                        ViewData["ShowOTPField"] = true;
                        ViewData["ShowOTPInput"] = true;
                        TempData["OTPEmail"] = savedEmail;
                        ViewBag.OTPEmail = savedEmail;
                        return View(model);
                    }

                    if (!System.Text.RegularExpressions.Regex.IsMatch(otp, @"^\d{6}$"))
                    {
                        ViewData["message"] = "OTP must be 6 digits.";
                        ViewData["ShowOTPField"] = true;
                        ViewData["ShowOTPInput"] = true;
                        TempData["OTPEmail"] = savedEmail;
                        ViewBag.OTPEmail = savedEmail;
                        return View(model);
                    }

                    var otpData = new { email = savedEmail, otp = otp };
                    response = await ApiClient.Post_ApiValuesGetRespnse(
                        client,
                        "api/users/VerifyPatientOTP",
                        otpData);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsAsync<dynamic>();
                        bool success = result.success;
                        string spMessage = result.message;

                        if (success)
                        {
                            string tenantid = Request.Query["tenantid"].ToString();
                            model.source = "Internal";
                            response = await ApiClient.Post_ApiValuesGetRespnse(
                                client,
                                "api/users/CheckAuthentication",
                                new { username = savedEmail, userpassword = "", source = "Internal" });

                            if (response.IsSuccessStatusCode)
                            {
                                DataTable dt = await response.Content.ReadAsAsync<DataTable>();
                                if (dt != null && dt.Rows.Count > 0)
                                {
                                    HttpContext.Session.SetString("NalamVazhausername", dt.Rows[0]["username"].ToString());
                                    HttpContext.Session.SetString("NalamVazhaloginUserID", dt.Rows[0]["usersid"].ToString());
                                    HttpContext.Session.SetString("NalamVazhatoken", dt.Rows[0]["token"].ToString());
                                    HttpContext.Session.SetString("NalamVazhacloudpath", Configuration.GetSection("AzureBlobStorageSetttings:baseURL").Value.ToString() + "/" + Configuration.GetSection("AzureBlobStorageSetttings:folderName").Value.ToString());
                                    HttpContext.Session.SetString("NalamVazhaSAStoken", util.fileSystem.getSASToken());
                                    HttpContext.Session.SetString("NalamVazhauserrole", dt.Rows[0]["userrole"].ToString());
                                    string fullName = $"{dt.Rows[0]["firstname"]?.ToString()} {dt.Rows[0]["lastname"]?.ToString()}".Trim();

                                    HttpContext.Session.SetString("firstname", fullName);

                                    HttpContext.Session.SetString("NalamVazhaprofilepicture", GetLoginProfilePictureUrl(dt.Rows[0]));
                                    ClearPendingPatientProfilePicture(dt.Rows[0]);

                                    HttpContext.Session.SetString("NalamVazhatenantlogo", baseUrl + "/images/logo.png");
                                    HttpContext.Session.SetString("NalamVazhashowtenant", "N");
                                    HttpContext.Session.SetString("NalamVazhachoosedtenantid", dt.Rows[0]["tenantid"].ToString());

                                    if (dt.Rows[0]["viewertenantids"].ToString().Contains(","))
                                    {
                                        HttpContext.Session.SetString("NalamVazhashowtenant", "Y");
                                        HttpContext.Session.SetString("NalamVazhachoosedtenantid", "");
                                    }

                                    HttpContext.Session.SetString("NalamVazhachoosedtenantname", dt.Rows[0]["tenantname"].ToString());
                                    HttpContext.Session.SetString("natureofbusiness", dt.Rows[0]["natureofbusiness"].ToString());

                                    if (dt.Rows[0]["tenantid"].ToString() != "00000000-0000-0000-0000-000000000000")
                                    {
                                        HttpContext.Session.SetString("partyname", dt.Rows[0]["natureofbusiness"].ToString());
                                        HttpContext.Session.SetString("module", "client");

                                        if (dt.Rows[0]["tenantname"].ToString() == "")
                                        {
                                            ViewData["message"] = "You are not authorized to access NalamVazha , Please contact administrator";
                                            return View(model);
                                        }
                                    }
                                    else
                                    {
                                        HttpContext.Session.SetString("partyname", dt.Rows[0]["natureofbusiness"].ToString());
                                        HttpContext.Session.SetString("NalamVazhashowtenant", "Y");
                                        HttpContext.Session.SetString("module", "admin");
                                        HttpContext.Session.SetString("NalamVazhachoosedtenantid", "");
                                    }

                                    // Get roles
                                    HttpResponseMessage response_roles = await ApiClient.GET_ApiValuesGetRespnse(client, "api/users/get_roleAuthorizations?viewactionroles=" + dt.Rows[0]["userrole"].ToString());

                                    if (response_roles.IsSuccessStatusCode)
                                    {
                                        DataTable dt_roles = await response_roles.Content.ReadAsAsync<DataTable>();
                                        string role_JSON = await response_roles.Content.ReadAsStringAsync();

                                        if (dt_roles.Rows.Count == 0)
                                        {
                                            ViewData["message"] = "You are not authorized to access NalamVazha , Please contact administrator";
                                            return View(model);
                                        }

                                        HttpContext.Session.SetSession("NalamVazharoles", dt_roles);
                                        HttpContext.Session.SetString("NalamVazharole_JSON", role_JSON);
                                    }

                                    // Get menu
                                    HttpResponseMessage response_menu = await ApiClient.GET_ApiValuesGetRespnse(client, "api/users/get_project_Menu?viewactionroles=" + dt.Rows[0]["userrole"].ToString() + "&SubSystem=Admin");

                                    if (response_menu.IsSuccessStatusCode)
                                    {
                                        DataTable dt_menu = await response_menu.Content.ReadAsAsync<DataTable>();
                                        string menu_JSON = await response_menu.Content.ReadAsStringAsync();
                                        HttpContext.Session.SetSession("NalamVazhamenu", dt_menu);

                                        var assembly = typeof(usersController).Assembly;
                                        System.Resources.ResourceManager _resourceManager = new System.Resources.ResourceManager("Admin.Resource", assembly);

                                        List<dynamic> menuItems = JsonConvert.DeserializeObject<List<dynamic>>(menu_JSON);

                                        foreach (dynamic menuItem in menuItems)
                                        {
                                            string projectElementName = System.Text.RegularExpressions.Regex.Replace(menuItem.projectelementname.ToString(), @"\s+", "").ToLower();
                                            string translatedProjectElementName = _resourceManager.GetString(projectElementName);
                                            string parentnode = System.Text.RegularExpressions.Regex.Replace(menuItem.parentnode.ToString(), @"\s+", "").ToLower();
                                            string translatedparentnode = _resourceManager.GetString(parentnode);
                                            menuItem.projectelementname = translatedProjectElementName;
                                            menuItem.parentnode = translatedparentnode;
                                        }
                                        menu_JSON = JsonConvert.SerializeObject(menuItems);
                                        HttpContext.Session.SetString("NalamVazhamenu_JSON", menu_JSON);
                                    }

                                    message = "Login Success";
                                    isOTPLogin = true;
                                    goto LoginRedirect;
                                }
                                else
                                {
                                    //HttpContext.Session.SetString("NalamVazhaauthenticatedEmail", savedEmail);
                                    //HttpContext.Session.SetString("NalamVazhaotpVerified", "true");
                                    //return RedirectToAction("Quick_Add_Patient_Profile", "PatientProfile",
                                    //    new { tenantid = tenantid });

                                    ViewData["message"] = "User not found.";
                                    ViewData["ShowOTPField"] = true;
                                    ViewData["ShowOTPInput"] = true;
                                    TempData["OTPEmail"] = savedEmail;
                                    ViewBag.OTPEmail = savedEmail;
                                    return View(model);
                                }
                            }
                            else
                            {
                                ViewData["message"] = "Failed to verify user. Please try again.";
                                ViewData["ShowOTPField"] = true;
                                ViewData["ShowOTPInput"] = true;
                                TempData["OTPEmail"] = savedEmail;
                                ViewBag.OTPEmail = savedEmail;
                                return View(model);
                            }
                        }
                        else
                        {
                            ViewData["message"] = spMessage;
                            ViewData["ShowOTPField"] = true;
                            ViewData["ShowOTPInput"] = true;
                            TempData["OTPEmail"] = savedEmail;
                            ViewBag.OTPEmail = savedEmail;
                            return View(model);
                        }
                    }
                    else
                    {
                        ViewData["message"] = "Invalid or Expired OTP. Please try again.";
                        ViewData["ShowOTPField"] = true;
                        ViewData["ShowOTPInput"] = true;
                        TempData["OTPEmail"] = savedEmail;
                        ViewBag.OTPEmail = savedEmail;
                        return View(model);
                    }
                }

               
                else if (!isOTPLogin)
                {
                    if (string.IsNullOrEmpty(model.username) || string.IsNullOrEmpty(model.userpassword))
                    {
                        ViewData["message"] = "Please enter your login identifier and password.";
                        return View(model);
                    }

                    model.source = "Internal";
                    response = await ApiClient.Post_ApiValuesGetRespnse(client, "api/users/CheckAuthentication", model);
                }

               
                if (response.IsSuccessStatusCode && !isOTPLogin)
                {
                    DataTable dt = await response.Content.ReadAsAsync<DataTable>();
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        // Set User Session Variables
                        HttpContext.Session.SetString("NalamVazhausername", dt.Rows[0]["username"].ToString());
                        HttpContext.Session.SetString("NalamVazhaloginUserID", dt.Rows[0]["usersid"].ToString());
                        HttpContext.Session.SetString("NalamVazhatoken", dt.Rows[0]["token"].ToString());
                        HttpContext.Session.SetString("NalamVazhacloudpath", Configuration.GetSection("AzureBlobStorageSetttings:baseURL").Value.ToString() + "/" + Configuration.GetSection("AzureBlobStorageSetttings:folderName").Value.ToString());
                        HttpContext.Session.SetString("NalamVazhaSAStoken", util.fileSystem.getSASToken());
                        HttpContext.Session.SetString("NalamVazhauserrole", dt.Rows[0]["userrole"].ToString());
                        string fullName = $"{dt.Rows[0]["firstname"]?.ToString()} {dt.Rows[0]["lastname"]?.ToString()}".Trim();

                        HttpContext.Session.SetString("firstname", fullName);

                        HttpContext.Session.SetString("NalamVazhaprofilepicture", GetLoginProfilePictureUrl(dt.Rows[0]));
                        ClearPendingPatientProfilePicture(dt.Rows[0]);

                        HttpContext.Session.SetString("NalamVazhatenantlogo", baseUrl + "/images/logo.png");
                        message = "Login Success";

                        HttpContext.Session.SetString("NalamVazhashowtenant", "N");
                        HttpContext.Session.SetString("NalamVazhachoosedtenantid", dt.Rows[0]["tenantid"].ToString());
                        if (dt.Rows[0]["viewertenantids"].ToString().Contains(","))
                        {
                            HttpContext.Session.SetString("NalamVazhashowtenant", "Y");
                            HttpContext.Session.SetString("NalamVazhachoosedtenantid", "");
                        }

                        HttpContext.Session.SetString("NalamVazhachoosedtenantname", dt.Rows[0]["tenantname"].ToString());
                        HttpContext.Session.SetString("natureofbusiness", dt.Rows[0]["natureofbusiness"].ToString());
                        if (dt.Rows[0]["tenantid"].ToString() != model.tenantid.ToString())
                        {
							ViewData["message"] = "You are not authorized to access this Hospital , Please contact administrator";
							return View(model);
						}
                        if (dt.Rows[0]["tenantid"].ToString() != "00000000-0000-0000-0000-000000000000")
                        {
                            HttpContext.Session.SetString("partyname", dt.Rows[0]["natureofbusiness"].ToString());
                            HttpContext.Session.SetString("module", "client");

                            if (dt.Rows[0]["tenantname"].ToString() == "")
                            {
                                ViewData["message"] = "You are not authorized to access NalamVazha , Please contact administrator";
                                return View(model);
                            }
                        }
                        else
                        {
                            HttpContext.Session.SetString("partyname", dt.Rows[0]["natureofbusiness"].ToString());
                            HttpContext.Session.SetString("NalamVazhashowtenant", "Y");
                            HttpContext.Session.SetString("module", "admin");
                            HttpContext.Session.SetString("NalamVazhachoosedtenantid", "");
                        }

                        // Get roles
                        HttpResponseMessage response_roles = await ApiClient.GET_ApiValuesGetRespnse(client, "api/users/get_roleAuthorizations?viewactionroles=" + dt.Rows[0]["userrole"].ToString());

                        if (response_roles.IsSuccessStatusCode)
                        {
                            DataTable dt_roles = await response_roles.Content.ReadAsAsync<DataTable>();
                            string role_JSON = await response_roles.Content.ReadAsStringAsync();

                            if (dt_roles.Rows.Count == 0)
                            {
                                ViewData["message"] = "You are not authorized to access NalamVazha , Please contact administrator";
                                return View(model);
                            }

                            HttpContext.Session.SetSession("NalamVazharoles", dt_roles);
                            HttpContext.Session.SetString("NalamVazharole_JSON", role_JSON);
                        }

                        // Get menu
                        HttpResponseMessage response_menu = await ApiClient.GET_ApiValuesGetRespnse(client, "api/users/get_project_Menu?viewactionroles=" + dt.Rows[0]["userrole"].ToString() + "&SubSystem=Admin");

                        if (response_menu.IsSuccessStatusCode)
                        {
                            DataTable dt_menu = await response_menu.Content.ReadAsAsync<DataTable>();
                            string menu_JSON = await response_menu.Content.ReadAsStringAsync();
                            HttpContext.Session.SetSession("NalamVazhamenu", dt_menu);

                            var assembly = typeof(usersController).Assembly;
                            System.Resources.ResourceManager _resourceManager = new System.Resources.ResourceManager("Admin.Resource", assembly);

                            List<dynamic> menuItems = JsonConvert.DeserializeObject<List<dynamic>>(menu_JSON);

                            foreach (dynamic menuItem in menuItems)
                            {
                                string projectElementName = System.Text.RegularExpressions.Regex.Replace(menuItem.projectelementname.ToString(), @"\s+", "").ToLower();
                                string translatedProjectElementName = _resourceManager.GetString(projectElementName);
                                string parentnode = System.Text.RegularExpressions.Regex.Replace(menuItem.parentnode.ToString(), @"\s+", "").ToLower();
                                string translatedparentnode = _resourceManager.GetString(parentnode);
                                menuItem.projectelementname = translatedProjectElementName;
                                menuItem.parentnode = translatedparentnode;
                            }
                            menu_JSON = JsonConvert.SerializeObject(menuItems);
                            HttpContext.Session.SetString("NalamVazhamenu_JSON", menu_JSON);
                        }
                    }
                    else
                    {
                        // User authentication failed - log the attempt
                        message = await ApiClient.Post_ApiValuesGetString(client, "api/userlockout/ins_userlockout", objuserlockoutModel);
                        ViewData["message"] = "Invalid login identifier or password. Please try again.";
                        return View(model);
                    }
                }
                else if (!isOTPLogin)
                {
                    message = "Response Failed";
                    ViewData["message"] = message;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred in - users / Login, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));
                message = ex.Message;
                ViewData["message"] = message;
                return View(model);
            }

        LoginRedirect:
            ViewData["message"] = message;
            if (message == "Login Success")
            {
                // Clear OTP session variables if they exist
                HttpContext.Session.Remove("NalamVazhaauthenticatedEmail");
                HttpContext.Session.Remove("NalamVazhaotpVerified");

                // If the tenant was resolved from a path shortcode (e.g. /nk),
                // insert /{shortcode} AFTER the app PathBase so the URL stays correct
                // on both local (/nk/Controller/Action) and virtual-path deployments
                // (/NalamVazhaDev/Admin/nk/Controller/Action).
                var shortcode = HttpContext.Session.GetString("NalamVazhaSubdomainShortcode");
                // Url.Content("~/") is the authoritative source for PathBase:
                //   · Local dev  → "/"  → TrimEnd → ""
                //   · Production → "/NalamVazhaDev/Admin/" → TrimEnd → "/NalamVazhaDev/Admin"
                // This works correctly regardless of ASPNETCORE_ENVIRONMENT name or middleware order.
                var appRoot = Url.Content("~/").TrimEnd('/');   // e.g. "/NalamVazhaDev/Admin"
                string TenantUrl(string relativeUrl)
                {
                    if (string.IsNullOrEmpty(shortcode) || string.IsNullOrEmpty(relativeUrl))
                        return relativeUrl;
                    // Url.Action() includes appRoot prefix when PathBase is configured.
                    // Strip it to get the pure route, then rebuild: appRoot + /shortcode + route
                    var rest = !string.IsNullOrEmpty(appRoot) &&
                               relativeUrl.StartsWith(appRoot, StringComparison.OrdinalIgnoreCase)
                               ? relativeUrl.Substring(appRoot.Length)   // → /Controller/Action?...
                               : relativeUrl;
                    return $"{appRoot}/{shortcode}{rest}";
                }

                switch (HttpContext.Session.GetString("NalamVazhauserrole"))
                {
                    case "Clinical Director":
                        return Redirect(TenantUrl(Url.Action("Clinical_Appointment_List", "ClinicalAppointment", new { isDashboardView = "Y" })));
                    case "Platform Admin":
                        return Redirect(TenantUrl(Url.Action("View_List_of_Healthcare_Provider", "tenant", new { isDashboardView = "Y" })));
                    case "Doctor":
					case "Intern Doctor":
						return Redirect(TenantUrl(Url.Action("Index", "DoctorDashboard", new { isDashboardView = "Y" })));

					//case "Intern Doctor":
					//	return RedirectToAction("Index", "InternDoctorDashboard");
						 
					case "Frontdesk Admin":
                        return Redirect(TenantUrl(Url.Action("Dashboard", "FrontDesk", new { isDashboardView = "Y" })));
                    case "Health Seeker":
                        return Redirect(TenantUrl(Url.Action("Index", "PatientDashboard", new { isDashboardView = "Y" })));
                    case "HR Associate":
                        return Redirect(TenantUrl(Url.Action("Staff_Attendance_List", "StaffAttendance", new { isDashboardView = "Y" })));
                    case "Healthcare Provider Admin":
                        return Redirect(TenantUrl(Url.Action("Dashboard", "HealthcareProviderAdmin", new { isDashboardView = "Y" })));
                }

                return Redirect(TenantUrl(Url.Action("UpdateProfile", "users", new { usersid = HttpContext.Session.GetString("NalamVazhaloginUserID") })));
            }
            else
            {
                return View(model);
            }
        }




        [HttpPost]
        public virtual async Task<String> appregister([FromBody] userloginModel model)
        {
            string message = "";
            string redirecturl = "";

            try
            {
				//IOS , ANDROID and WEB etc
                if (model.source == null || model.source == "")
                {

                    message = "|source is required|";

                }
				//Anything
                if (model.username == null || model.username == "")
                {

                    message += "|username is required|";

                }
				 
				//UUID Of android or IOS 
                if (model.deviceid == null || model.deviceid == "")
                {

                    message += "|deviceid is required|";

                }

                if (model.notificationid == null || model.notificationid == "")
                {

                    message += "|notificationid is required|";

                }


                if (model.accesskey == null || model.accesskey == "")
                {
                    message += "|accesskey is Required|";
                }

                else if (model.accesskey != accesskey)
                {
                    message += "|Invalid accesskey|";
                }
                 
                if (message == "")
                {
                     HttpResponseMessage response = await ApiClient.Post_ApiValuesGetRespnse(getHttpClient(),"api/users/CheckAuthentication", model);
						
                    if (response.IsSuccessStatusCode)
                    {
                        DataTable dt = await response.Content.ReadAsAsync<DataTable>();
                        if (dt.Rows.Count > 0)
                        {

                            message = "Login Success";
                            redirecturl = adminUrl + "/" + "users/toapp?usersid=" + dt.Rows[0]["usersid"].ToString();
                            

                            
HttpContext.Session.SetString("NalamVazhashowtenant", "N");
                         HttpContext.Session.SetString("NalamVazhachoosedtenantid", dt.Rows[0]["tenantid"].ToString());
                        if (dt.Rows[0]["viewertenantids"].ToString().Contains(","))
                        {
                            HttpContext.Session.SetString("NalamVazhashowtenant", "Y");
                            HttpContext.Session.SetString("NalamVazhachoosedtenantid", "");
                        }

                                                                               HttpContext.Session.SetString("NalamVazhachoosedtenantname", dt.Rows[0]["tenantname"].ToString());

                                                                         
																			 
                                                                             HttpContext.Session.SetString("natureofbusiness", dt.Rows[0]["natureofbusiness"].ToString());
                                                                    
																			if(dt.Rows[0]["tenantid"].ToString() !="00000000-0000-0000-0000-000000000000")
																			{
                                                                                     HttpContext.Session.SetString("partyname",  dt.Rows[0]["natureofbusiness"].ToString());
																					HttpContext.Session.SetString("module", "client");

                                                                                                   
                                                                                           
                                                                                    if (dt.Rows[0]["tenantname"].ToString() == "")
                                                                                    {
                                                                                         message="You are not authorized to access NalamVazha , Please contact administrator";
                                                                                        
                                                                                    }   

																			} else
						                                                    {
                                                                                HttpContext.Session.SetString("partyname",  dt.Rows[0]["natureofbusiness"].ToString());
                                                                                 HttpContext.Session.SetString("NalamVazhashowtenant", "Y");
                                                                                                            HttpContext.Session.SetString("module", "admin");

                                                                                HttpContext.Session.SetString("NalamVazhachoosedtenantid", "");
                                                                             }




                                            


                            //ROLE SECTION
                            HttpResponseMessage response_roles = await ApiClient.GET_ApiValuesGetRespnse(client,"api/users/get_roleAuthorizations?viewactionroles="+dt.Rows[0]["userrole"].ToString());

							
							if (response_roles.IsSuccessStatusCode)
							{
								
								DataTable dt_roles = await response_roles.Content.ReadAsAsync<DataTable>();
								if(dt_roles.Rows.Count ==0){
                                        message="You are not authorized to access NalamVazha , Please contact administrator";
                                         
                                }
            
								 
							}


 

                        }
                        else
                        {
                            message = "user not available";

                        }
                    }
                    else
                    {
                        message = "NalamVazha's API Response Failed";
                    }
                }
                else
                {

                    message = "Validation Failed : " + message;
                }




            }
            catch (Exception ex)
            {
                
                  _logger.LogError(ex,"An exception occurred in - users / appregister, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                  
                message = ex.Message;
            }

        

            if (message == "Login Success")
            {
                return JsonConvert.SerializeObject(new
                {
                    status = "success",
                    message = "success",
                    redirecturl = redirecturl
                });

            }
            else
            {

                return JsonConvert.SerializeObject(new
                {
                    status = "failed",
                    message = message,
                    redirecturl = ""
                });
            }
        }

		[AllowAnonymous]
        public virtual IActionResult applogin()
        {
            var tenantid = Request.Query["tenantid"].ToString();
            return RedirectToAction("Login", new { tenantid = string.IsNullOrWhiteSpace(tenantid) ? (string)null : tenantid });
        }

        //Mobile Or UUID FROM Mobile APP
		//uuid or mobile number is username
[AllowAnonymous]
        [HttpPost]
        public virtual async Task<String> applogin(userloginModel model)
        {
            string message = "";
            string redirecturl = "";

            try
            {

                if (model.source == null || model.source == "")
                {

                    message = "|source required|";

                }

                if (model.username == null || model.username == "")
                {

                    message += "|username required|";

                }

                if (model.accesskey == null || model.accesskey == "")
                {
                    message += "|accesskey Required|";
                }

                else if (model.accesskey != accesskey)
                {

                    Console.WriteLine(model.accesskey);
                    Console.WriteLine(accesskey);

                    message += "|Invalid accesskey|";
                }

           

                if (message == "")
                {
  				 HttpResponseMessage response = await ApiClient.Post_ApiValuesGetRespnse(getHttpClient(),"api/users/CheckAuthentication", model);
				

                    if (response.IsSuccessStatusCode)
                    {
                        DataTable dt = await response.Content.ReadAsAsync<DataTable>();
                        if (dt.Rows.Count > 0)
                        {
                            message = "Login Success";
    
                            redirecturl = adminUrl + "/" + "users/toapp?usersid=" + dt.Rows[0]["usersid"].ToString();
                            

                            
HttpContext.Session.SetString("NalamVazhashowtenant", "N");
                         HttpContext.Session.SetString("NalamVazhachoosedtenantid", dt.Rows[0]["tenantid"].ToString());
                        if (dt.Rows[0]["viewertenantids"].ToString().Contains(","))
                        {
                            HttpContext.Session.SetString("NalamVazhashowtenant", "Y");
                            HttpContext.Session.SetString("NalamVazhachoosedtenantid", "");
                        }

                                                                               HttpContext.Session.SetString("NalamVazhachoosedtenantname", dt.Rows[0]["tenantname"].ToString());

                                                                         
																			 
                                                                             HttpContext.Session.SetString("natureofbusiness", dt.Rows[0]["natureofbusiness"].ToString());
                                                                    
																			if(dt.Rows[0]["tenantid"].ToString() !="00000000-0000-0000-0000-000000000000")
																			{
                                                                                     HttpContext.Session.SetString("partyname",  dt.Rows[0]["natureofbusiness"].ToString());
																					HttpContext.Session.SetString("module", "client");

                                                                                                   
                                                                                           
                                                                                    if (dt.Rows[0]["tenantname"].ToString() == "")
                                                                                    {
                                                                                         message="You are not authorized to access NalamVazha , Please contact administrator";
                                                                                        
                                                                                    }   

																			} else
						                                                    {
                                                                                HttpContext.Session.SetString("partyname",  dt.Rows[0]["natureofbusiness"].ToString());
                                                                                 HttpContext.Session.SetString("NalamVazhashowtenant", "Y");
                                                                                                            HttpContext.Session.SetString("module", "admin");

                                                                                HttpContext.Session.SetString("NalamVazhachoosedtenantid", "");
                                                                             }




                                            
 
                            HttpResponseMessage response_roles = await ApiClient.GET_ApiValuesGetRespnse(client,"api/users/get_roleAuthorizations?viewactionroles="+dt.Rows[0]["userrole"].ToString());

							
							if (response_roles.IsSuccessStatusCode)
							{
								
								DataTable dt_roles = await response_roles.Content.ReadAsAsync<DataTable>();
								if(dt_roles.Rows.Count ==0){
                                        message="You are not authorized to access NalamVazha , Please contact administrator";
                                         
                                }
            
								 
							}

                         }
                        else
                        {
                            message = "user not available";
  

                        }
                    }
                    else
                    {
                        message = "NalamVazha's API Response Failed";
                    }
                }
                else
                {

                    message = "Validation Failed : " + message;
                }




            }
            catch (Exception ex)
            {
                
                 _logger.LogError(ex,"An exception occurred in - users / applogin, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                  
                message = ex.Message;
            }

            

            if (message == "Login Success")
            {
                return JsonConvert.SerializeObject(new
                {
                    status = "success",
                    message = "success",
                    redirecturl = redirecturl
                });

            }
            else
            {

                return JsonConvert.SerializeObject(new
                {
                    status = "failed",
                    message = message,
                    redirecturl = ""
                });
            }
        }
 //Go_to_app_with_user_id
        public virtual async Task<IActionResult> toapp(string usersid)
        {
            string message = "";

            userloginModel model = new userloginModel();
            try
            {
                model.username = usersid;
                model.source = "allowbyid";
                HttpResponseMessage response = await ApiClient.Post_ApiValuesGetRespnse(getHttpClient(),"api/users/CheckAuthentication", model);
					
               

                if (response.IsSuccessStatusCode)
                {
                    DataTable dt = await response.Content.ReadAsAsync<DataTable>();
                    if (dt.Rows.Count > 0)
                    {
                            HttpContext.Session.SetString("NalamVazhausername", dt.Rows[0]["username"].ToString());
							HttpContext.Session.SetString("NalamVazhaloginUserID", dt.Rows[0]["usersid"].ToString());
							
							HttpContext.Session.SetString("NalamVazhatoken", dt.Rows[0]["token"].ToString());
							HttpContext.Session.SetString("NalamVazhauserrole", dt.Rows[0]["userrole"].ToString());
                            if(dt.Rows[0]["profilepicture"].ToString()!=null && dt.Rows[0]["profilepicture"].ToString()!="")
                            HttpContext.Session.SetString("NalamVazhaprofilepicture", util.fileSystem.GetFileURl(dt.Rows[0]["profilepicture"].ToString()));
                            else
                            HttpContext.Session.SetString("NalamVazhaprofilepicture", baseUrl+"/Icon_set/User.png");

                            HttpContext.Session.SetString("NalamVazhatenantlogo", baseUrl+"/images/logo.png");
                            message = "Login Success";

                         

							
HttpContext.Session.SetString("NalamVazhashowtenant", "N");
                         HttpContext.Session.SetString("NalamVazhachoosedtenantid", dt.Rows[0]["tenantid"].ToString());
                        if (dt.Rows[0]["viewertenantids"].ToString().Contains(","))
                        {
                            HttpContext.Session.SetString("NalamVazhashowtenant", "Y");
                            HttpContext.Session.SetString("NalamVazhachoosedtenantid", "");
                        }

                                                                               HttpContext.Session.SetString("NalamVazhachoosedtenantname", dt.Rows[0]["tenantname"].ToString());

                                                                         
																			 
                                                                             HttpContext.Session.SetString("natureofbusiness", dt.Rows[0]["natureofbusiness"].ToString());
                                                                    
																			if(dt.Rows[0]["tenantid"].ToString() !="00000000-0000-0000-0000-000000000000")
																			{
                                                                                     HttpContext.Session.SetString("partyname",  dt.Rows[0]["natureofbusiness"].ToString());
																					HttpContext.Session.SetString("module", "client");

                                                                                                   
                                                                                           
                                                                                    if (dt.Rows[0]["tenantname"].ToString() == "")
                                                                                    {
                                                                                        ViewData["message"] = "You are not authorized to access NalamVazha , Please contact administrator";
                                                                                        return View(model);
                                                                                    }   

																			} else
						                                                    {
                                                                                HttpContext.Session.SetString("partyname",  dt.Rows[0]["natureofbusiness"].ToString());
                                                                                 HttpContext.Session.SetString("NalamVazhashowtenant", "Y");
                                                                                                            HttpContext.Session.SetString("module", "admin");

                                                                                HttpContext.Session.SetString("NalamVazhachoosedtenantid", "");
                                                                             }




                                            

							//ROLE SECTION



							HttpResponseMessage response_roles = await ApiClient.GET_ApiValuesGetRespnse(client,"api/users/get_roleAuthorizations?viewactionroles="+dt.Rows[0]["userrole"].ToString());

							
							if (response_roles.IsSuccessStatusCode)
							{
								
								DataTable dt_roles = await response_roles.Content.ReadAsAsync<DataTable>();
								string role_JSON = await response_roles.Content.ReadAsStringAsync();
                                if(dt_roles.Rows.Count ==0){
                                        ViewData["message"]="You are not authorized to access NalamVazha , Please contact administrator";
                                        return View(model); 
                                }
            
								HttpContext.Session.SetSession("NalamVazharoles", dt_roles);
								HttpContext.Session.SetString("NalamVazharole_JSON", role_JSON);
								

								
							} 
												


							//ROLE SECTION



							//MENU SECTION



							HttpResponseMessage response_menu = await ApiClient.GET_ApiValuesGetRespnse(client,"api/users/get_project_Menu?viewactionroles="+dt.Rows[0]["userrole"].ToString()+"&SubSystem=Admin");
							
							if (response_menu.IsSuccessStatusCode)
							{
								
								DataTable dt_menu = await response_menu.Content.ReadAsAsync<DataTable>();
								string menu_JSON = await response_menu.Content.ReadAsStringAsync();
								HttpContext.Session.SetSession("NalamVazhamenu", dt_menu);
								 var assembly = typeof(usersController).Assembly;
                                    System.Resources.ResourceManager _resourceManager = new System.Resources.ResourceManager("Admin.Resource", assembly);
 
                                     List<dynamic> menuItems = JsonConvert.DeserializeObject<List<dynamic>>(menu_JSON);

                                    foreach (dynamic menuItem in menuItems)
                                    { 
					                    string projectElementName = System.Text.RegularExpressions.Regex.Replace(menuItem.projectelementname.ToString(), @"\s+", "").ToLower();
					                    string translatedProjectElementName = _resourceManager.GetString(projectElementName);
					                    string parentnode = System.Text.RegularExpressions.Regex.Replace(menuItem.parentnode.ToString(), @"\s+", "").ToLower();
					                    string translatedparentnode = _resourceManager.GetString(parentnode);
					                    menuItem.projectelementname = translatedProjectElementName;
					                    menuItem.parentnode = translatedparentnode;
                                    }
				                    menu_JSON = JsonConvert.SerializeObject(menuItems);
			                        HttpContext.Session.SetString("NalamVazhamenu_JSON", menu_JSON);
								

								
							} 
												


							//MENU SECTION
					


                    }
                    else
                    {
                         message = "user not exists";


                    }
                }
                else
                {
                    message = "Response Failed";
                }
            }
            catch (Exception ex)
            {
               
                _logger.LogError(ex,"An exception occurred in - users / toapp, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                 
                message = ex.Message;
            }
            TempData["message"] = message;
            if (message == "Login Success")
            {
                 
                 return RedirectToAction("UpdateProfile","users", new {@usersid=HttpContext.Session.GetString("NalamVazhaloginUserID")});
					
            }
            else
            {

                var tenantid = Request.Query["tenantid"].ToString();
                return RedirectToAction("Login", new { tenantid = string.IsNullOrWhiteSpace(tenantid) ? (string)null : tenantid });
            }
        }
        public virtual IActionResult forgotpassword()
        {
            HttpContext.Session.SetString("ReferrerUrl", "");
            TempData["messagereferer"] = "donotrefer";
            return View();
        }
        [HttpPost()]
        public virtual async Task<IActionResult> forgotpassword(userloginModel loginmodel)
        {
            string message = "";
            string pwd = "";
            usersModel model = new usersModel();
			var client = getHttpClient();
			try
            {
                loginmodel.source = "Internal";
                HttpResponseMessage response_Password = await ApiClient.Post_ApiValuesGetRespnse(getHttpClient(), "api/users/get_decryptedPassword", loginmodel);
                if (response_Password.IsSuccessStatusCode)
                {
                    message = await response_Password.Content.ReadAsAsync<string>();

                    loginmodel.userpassword = message;
                    pwd = message;
                }
                HttpResponseMessage response = await ApiClient.Post_ApiValuesGetRespnse(client, "api/users/CheckAuthentication", loginmodel);

                if (response.IsSuccessStatusCode)
                {
                    var jsonObj = await response.Content.ReadAsStringAsync();
                    if (jsonObj.Length > 2)
                    {
                        jsonObj = jsonObj.Substring(1, jsonObj.Length - 2);
                        model = JsonConvert.DeserializeObject<usersModel>(jsonObj);
                        message = "Login Success";
                        model.userpassword = pwd;
                    }
                    else
                    {
                        message = "Login Failed";
                    }
                }
                else
                {
                    message = "Response Failed";
                }
            }
            catch (Exception ex)
            {
               
                 _logger.LogError(ex,"An exception occurred in - users / forgotpassword, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
               
                message = ex.Message;
            }
            ViewData["message"] = "User Info not available - Please contact Admin ";
            if (message == "Login Success")
            {

                if (model.emailid != null && model.emailid != "")
                {

                    MailSender maillog = new MailSender();
                    bool mailsent = await maillog.sendMail("users"
                        , "forgotpassword"
                        , model.usersid.ToString()
                        , "Forgot Password"
                        , _mailSettings
                        , "13EF2D93-CD5C-49DB-AE70-8FB33399275C"
                        , client, model.emailid, model.userpassword, tenantid: model.tenantid?.ToString() ?? "");

                    if (mailsent)
                    {
                        ViewData["message"] = "User Info Sent to your registered email";
                    }
                    else
                    {
                        ViewData["message"] = "User Info found - Mail Sending Failed - Please contact Admin ";
                    }


                }
                else
                {
                    ViewData["message"] = "User Info found - Mail Sending Failed - Please contact Admin ";
                }



                return View(loginmodel);
            }
            else
            {
                return View(loginmodel);
            }
;
        }

        public virtual async Task<string> get_Dashboard_Items(string viewactionroles)
		{
			string dashmenu = await ApiClient.Get_ApiValues(getHttpClient(), "api/users/get_Dashboard_Items?viewactionroles=" + HttpContext.Session.GetString("NalamVazhauserrole"));
			if (dashmenu != "")
			{
				var assembly = typeof(usersController).Assembly;
				System.Resources.ResourceManager _resourceManager = new System.Resources.ResourceManager("Admin.Resource", assembly);

				List<dynamic> menuItems = JsonConvert.DeserializeObject<List<dynamic>>(dashmenu);

				foreach (dynamic menuItem in menuItems)
				{
					string actiondisplayname = System.Text.RegularExpressions.Regex.Replace(menuItem.actiondisplayname.ToString(), @"\s+", "").ToLower();
					string translatedactiondisplayname = _resourceManager.GetString(actiondisplayname);
					menuItem.actiondisplayname = translatedactiondisplayname;
				}
				dashmenu = JsonConvert.SerializeObject(menuItems);
				
			}
			return dashmenu;

		}


			public virtual IActionResult List_of_User_Profiles()
			{
				return View();
			}
			public virtual IActionResult IndexPlatform()
					{
						 HttpContext.Session.SetString("NalamVazhachoosedtenantid","");
						return View();
					}	
			[HttpGet()]
			public virtual async Task<string> get_List_of_User_Profiles(string tenantid
)
			{
				
				return await ApiClient.Get_ApiValues(getHttpClient(), "api/users/List_of_User_Profiles?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID")
);
			}
			 

                                        public virtual IActionResult Detail()
                                        {
                                            return View();
                                        }

				
			  public virtual async Task<IActionResult> ChangePassword(string usersid)
			  {
                    string redirectTo="";
            if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
                
			DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
			DataView dv = new DataView(NalamVazharole_JSON);
			dv.RowFilter = "controllername='users' AND viewname='list'";

			if(dv.Count  >0){
				redirectTo = dv[0]["actionmethodname"] as string;
							 
			}	 
                    try
                        {
                            var jsonObjusers = await ApiClient.Get_ApiValues(getHttpClient(), "api/users/getById_users?usersid="+usersid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                                 
						         var model = JsonConvert.DeserializeObject<usersModel>(jsonObjusers);


                
					      
                     
						   
						     

                            usersChangePasswordModel objModel=new usersChangePasswordModel();     
                            objModel.usersid=model.usersid;
						    return View(objModel);

                        }catch(Exception ex){
                            _logger.LogError(ex,"An exception occurred in - users / ChangePassword, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                            TempData["errMessage"] = "Error while fetching data - Contact Administrator";
                            return RedirectToAction(redirectTo);
                        }
					 

                }
                TempData["errMessage"] = "Session Expired";
                return RedirectToAction("Logout","users");

			  }	
			  [HttpPost()]
				public virtual async Task<string> ChangePassword(usersChangePasswordModel model, IFormCollection collection)
				{
					string strReturnMessage = "";
					try
					{
							ModelState.Remove("usersid");
							 
							if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
					model.modifieduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
					else
					return "Session Expired";
							 
							if (ModelState.IsValid)
							{
									    strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/users/ChangePassword", model);
									  
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
                        _logger.LogError(ex,"An exception occurred in - users / ChangePassword, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
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

				
			  public virtual async Task<string> getById_allinfo_users(string usersid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/users/getById_allinfo_users?usersid="+usersid);
					 
			  }






                     public string download(string filename)
                        {
                            StorageUtil util = new StorageUtil(Configuration);
                            string url = util.fileSystem.GetFileURl(filename);
                            return url;

                        }
		[AllowAnonymous]
		public virtual async Task<IActionResult> Index()
		{
			List<tenantModel> tenants = await GetTenantListForLandingAsync();
			var list = tenants.Select(t =>
			{
				var logoPath = t.organizationlogo?.Split('|').FirstOrDefault(s => !string.IsNullOrWhiteSpace(s))?.Trim();
				return new TenantLandingItem
				{
					tenantid = t.tenantid,
					businessname = t.businessname,
					logoUrl = string.IsNullOrEmpty(logoPath) ? null : util.fileSystem.GetFileURl(logoPath)
				};
			}).ToList();
			return View(list);
		}
		[AllowAnonymous]
		[HttpGet()]
		public virtual async Task<string> GetTenantListForLanding()
		{
			var tenants = await GetTenantListForLandingAsync();
			return JsonConvert.SerializeObject(tenants);
		}
		private async Task<List<tenantModel>> GetTenantListForLandingAsync()
		{
			try
			{
				using (var client = new HttpClient())
				{
					client.BaseAddress = new Uri(url);
					client.DefaultRequestHeaders.Accept.Clear();
					client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
					var query = "api/tenant/get_project_tenant_public?tenantid=00000000-0000-0000-0000-000000000000&accesskey=" + Uri.EscapeDataString(accesskey ?? "");
					var response = await client.GetAsync(query);
					if (!response.IsSuccessStatusCode) return new List<tenantModel>();
					var json = await response.Content.ReadAsStringAsync();
					if (string.IsNullOrWhiteSpace(json) || json == "[]" || json == "null") return new List<tenantModel>();
					var list = JsonConvert.DeserializeObject<List<tenantModel>>(json);
					return list ?? new List<tenantModel>();
				}
			}
			catch (Exception ex)
			{
				_logger?.LogWarning(ex, "GetTenantListForLanding failed");
				return new List<tenantModel>();
			}
		}

        private async Task SetTenantBrandingForLoginAsync(string? tenantId)
        {
            try
            {
                // Prefer subdomain branding when available.
                var subdomainTenantId = HttpContext.Items["SubdomainTenantId"] as string;
                if (!string.IsNullOrWhiteSpace(subdomainTenantId))
                {
                    ViewBag.TenantName = HttpContext.Items["SubdomainTenantName"] as string;
                    var logoPath = (HttpContext.Items["SubdomainLogo"] as string)
                        ?.Split('|').FirstOrDefault(s => !string.IsNullOrWhiteSpace(s))?.Trim();
                    ViewBag.TenantLogo = string.IsNullOrEmpty(logoPath) ? null : util.fileSystem.GetFileURl(logoPath);
                    ViewBag.SubdomainTenantId = subdomainTenantId;
                    return;
                }

                if (string.IsNullOrWhiteSpace(tenantId))
                    return;

                var tenants = await GetTenantListForLandingAsync();
                var tenant = tenants?.FirstOrDefault(t =>
                    t?.tenantid.HasValue == true &&
                    string.Equals(t.tenantid.Value.ToString(), tenantId, StringComparison.OrdinalIgnoreCase));

                if (tenant == null)
                    return;

                ViewBag.TenantName = tenant.businessname;
                var logoPathQuery = tenant.organizationlogo?.Split('|')
                    .FirstOrDefault(s => !string.IsNullOrWhiteSpace(s))?.Trim();
                ViewBag.TenantLogo = string.IsNullOrEmpty(logoPathQuery) ? null : util.fileSystem.GetFileURl(logoPathQuery);
                ViewBag.SubdomainTenantId = tenantId;
            }
            catch
            {
                // Don't break login on branding failures.
            }
        }

	}


			}
