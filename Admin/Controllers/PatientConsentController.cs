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

                
                
                
				//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:52
				
                
                
                
                
				public class PatientConsentController : BaseController
				{
					private const string SingleConsentFileMessage = "Only one consent file can be uploaded.";

					private static string GetSingleExistingConsentFile(string existingFiles)
					{
						var firstFile = (existingFiles ?? string.Empty)
							.Split('|', StringSplitOptions.RemoveEmptyEntries)
							.FirstOrDefault();

						return string.IsNullOrWhiteSpace(firstFile) ? string.Empty : $"|{firstFile}|";
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
                    private readonly ILogger<PatientConsentController> _logger;
                    
                    
                    StorageUtil util;
					public PatientConsentController(IConfiguration configuration,IHttpContextAccessor accessor,IOptions<ApiSettings> ApiSettings, IOptions<MailSettings> MailSettings, IWebHostEnvironment env, ILogger<PatientConsentController> logger):base( configuration)
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
	              

					
			  public virtual IActionResult Add_Patient_Consent()
			  {
					return View();
			  }	
			  [HttpPost()]
			public virtual async Task<string> Add_Patient_Consent(PatientConsentModel model, IFormCollection collection)
			{
				string strReturnMessage = "";
				
				try
				{
					ModelState.Remove("PatientConsentid");
					ModelState.Remove("createduser");
                    ModelState.Remove("craftmyapp_actionmethodname");
                    model.craftmyapp_actionmethodname="Add_Patient_Consent";
					if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
								model.createduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
								else
								return "Session Expired";                    
					ModelState.Remove("consentfile");

                   
					
			 	    
					 if (ModelState.IsValid)
					 {
							 PatientConsentModelValidator validator = new PatientConsentModelValidator();
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
								 model.PatientConsentid =Guid.NewGuid(); 
                                 model.consentfile = GetSingleExistingConsentFile(collection["consentfile_existing"]);

                                  var files = Request.Form.Files
									  .Where(file => string.Equals(file.Name, "consentfile", StringComparison.OrdinalIgnoreCase))
									  .ToList();
								  if (files.Count > 1)
								  {
									  return SingleConsentFileMessage;
								  }
foreach (var file in files) 
{
var filename = ContentDispositionHeaderValue
.Parse(file.ContentDisposition)
.FileName
.Trim('"'); 
string fileExtention = "." + filename.Split('.').Last(); 
Random rnd = new Random();
string uploadFileName = System.Text.RegularExpressions.Regex.Replace(filename.Split('.').First(), @"[^0-9a-zA-Z_.]+", "").Replace(" ", String.Empty)+"_"+"PatientConsent_" +rnd.Next(1, 10000).ToString() + DateTime.Now.ToString("ddMMyyHHmmss")+ fileExtention;
if (fileExtention != ".")
{
Stream stream = file.OpenReadStream();
string fileURL=await util.fileSystem.UploadFileAsync(stream, Configuration.GetSection("AzureBlobStorageSetttings:folderName").Value, uploadFileName);
if (file.Name == "consentfile")
{
uploadFileName = "|" + fileURL +"|";
model.consentfile +=  uploadFileName ;
}
}
}

								 
                                 strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/PatientConsent/Add_Patient_Consent", model);
                                    
								 
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
                 
                 _logger.LogError(ex,"An exception occurred in - PatientConsent / Add_Patient_Consent, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
               
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

				
			  public virtual async Task<IActionResult> Update_Patient_Consent(string PatientConsentid)
			  {

                    string redirectTo="";
                    if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
                            DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
                            DataView dv = new DataView(NalamVazharole_JSON);
                            dv.RowFilter = "controllername='PatientConsent' AND viewname='list'";

                            if(dv.Count  >0){
                                redirectTo = dv[0]["actionmethodname"] as string;
							 
                            }

                            try{
                                     var jsonObjPatientConsent = await ApiClient.Get_ApiValues(getHttpClient(), "api/PatientConsent/getById_PatientConsent?PatientConsentid="+PatientConsentid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                                if(jsonObjPatientConsent.Length > 2)
                                {
                                  
                                    var model = JsonConvert.DeserializeObject<PatientConsentModel>(jsonObjPatientConsent);


                
                                     
                                    return View("Add_Patient_Consent", model);
                                }
                                else
                                {
                    
                                    TempData["message"] = "Data Not Found - Contact Administrator";
                                    return RedirectToAction(redirectTo);
						 
                                }

                            }catch(Exception ex){
                               _logger.LogError(ex,"An exception occurred in - PatientConsent / Update_Patient_Consent, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                                TempData["errMessage"] = "Error while fetching data - Contact Administrator";
                                return RedirectToAction(redirectTo);
                            }

                    }
                    TempData["errMessage"] = "Session Expired";
                    return RedirectToAction("Logout", "users");
                }	
			  [HttpPost()]
				public virtual async Task<string> Update_Patient_Consent(PatientConsentModel model, IFormCollection collection)
				{
					string strReturnMessage = "";
					try
					{
							ModelState.Remove("PatientConsentid");
                            ModelState.Remove("craftmyapp_actionmethodname");
                             model.craftmyapp_actionmethodname="Update_Patient_Consent";
							
							
							if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
					model.modifieduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
					else
					return "Session Expired";
							
                            model.consentfile = GetSingleExistingConsentFile(collection["consentfile_existing"]);

                            ModelState.Remove("consentfile");

							if (ModelState.IsValid)
							{
									PatientConsentModelValidator validator = new PatientConsentModelValidator();
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
                                        
										var files = Request.Form.Files
											.Where(file => string.Equals(file.Name, "consentfile", StringComparison.OrdinalIgnoreCase))
											.ToList();
										if (files.Count > 1)
										{
											return SingleConsentFileMessage;
										}
foreach (var file in files) 
{
var filename = ContentDispositionHeaderValue
.Parse(file.ContentDisposition)
.FileName
.Trim('"'); 
string fileExtention = "." + filename.Split('.').Last(); 
Random rnd = new Random();
string uploadFileName = System.Text.RegularExpressions.Regex.Replace(filename.Split('.').First(), @"[^0-9a-zA-Z_.]+", "").Replace(" ", String.Empty)+"_"+"PatientConsent_" +rnd.Next(1, 10000).ToString() + DateTime.Now.ToString("ddMMyyHHmmss")+ fileExtention;
if (fileExtention != ".")
{
Stream stream = file.OpenReadStream();
string fileURL=await util.fileSystem.UploadFileAsync(stream, Configuration.GetSection("AzureBlobStorageSetttings:folderName").Value, uploadFileName);
if (file.Name == "consentfile")
{
uploadFileName = "|" + fileURL +"|";
model.consentfile +=  uploadFileName ;
}
}
}

                                        
                                        
                                        strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/PatientConsent/Update_Patient_Consent", model);
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
                      _logger.LogError(ex,"An exception occurred in - PatientConsent / Update_Patient_Consent, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
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
public virtual async Task<IActionResult> Remove_Patient_Consent(string PatientConsentid)
			{
				string message = "";
				try
				{
						message = await ApiClient.Get_ApiValues(getHttpClient(), "api/PatientConsent/Remove_Patient_Consent?PatientConsentid="+PatientConsentid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
						 if(message.Replace("\"","").Contains("201.1"))
						{
							TempData["message"] = "Success";

						}else{
							TempData["errMessage"] = message.Replace("\"","");
						}
						
				
				
				}
				catch (Exception ex)
				{
                     _logger.LogError(ex,"An exception occurred in - PatientConsent / Remove_Patient_Consent, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                
					 TempData["errMessage"] = ex.Message;
					 message = ex.Message;
				}

				string redirectTo="";
						if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
					DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
						 DataView dv = new DataView(NalamVazharole_JSON);
						 dv.RowFilter = "controllername='PatientConsent' AND viewname='list'";

						if(dv.Count  >0){
						    redirectTo = dv[0]["actionmethodname"] as string;
							 
						}

					}
				
				return RedirectToAction(redirectTo);
			}

                                        public virtual IActionResult View_Patient_Consent()
                                        {
                                            return View();
                                        }

			public virtual IActionResult Patient_Consent_List()
			{
				return View();
			}
				
			[HttpGet()]
			public virtual async Task<string> get_Patient_Consent_List(string tenantid
)
			{
				if (string.IsNullOrWhiteSpace(tenantid) ||
					tenantid == "00000000-0000-0000-0000-000000000000")
				{
					tenantid = HttpContext.Session.GetString("NalamVazhachoosedtenantid")
						?? HttpContext.Session.GetString("NalamVazhatenantid");
				}

				return await ApiClient.Get_ApiValues(getHttpClient(), "api/PatientConsent/Patient_Consent_List?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID")
);
			}
			 

				
			  public virtual async Task<string> getById_allinfo_PatientConsent(string PatientConsentid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/PatientConsent/getById_allinfo_PatientConsent?PatientConsentid="+PatientConsentid);
					 
			  }






                    
                     
                        

				}


			}
