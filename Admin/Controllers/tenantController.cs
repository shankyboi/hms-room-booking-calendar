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

                
                
                
				//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:18
				
                
                
                
                
				public class tenantController : BaseController
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
                    private readonly ILogger<tenantController> _logger;
                    
                    
                    StorageUtil util;
					public tenantController(IConfiguration configuration,IHttpContextAccessor accessor,IOptions<ApiSettings> ApiSettings, IOptions<MailSettings> MailSettings, IWebHostEnvironment env, ILogger<tenantController> logger):base( configuration)
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
	              
        [HttpPost]
        public IActionResult ClearSession()
        {
            HttpContext.Session.Clear();
            return Json(new { success = true });
        }

					
			  public virtual IActionResult Create_Healthcare_Provider()
			  {
					return View();
			  }	
			  [HttpPost()]
			public virtual async Task<string> Create_Healthcare_Provider(tenantModel model, IFormCollection collection)
			{
				string strReturnMessage = "";
				var client=getHttpClient();
				try
				{
					ModelState.Remove("tenantid");
					ModelState.Remove("createduser");
                    ModelState.Remove("craftmyapp_actionmethodname");
                    model.craftmyapp_actionmethodname="Create_Healthcare_Provider";
					                    
					ModelState.Remove("organizationlogo");

                   
					
			 	    
					 if (ModelState.IsValid)
					 {
							 tenantModelValidator validator = new tenantModelValidator();
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
								 model.tenantid =Guid.NewGuid(); 
                                 model.organizationlogo = collection["organizationlogo_existing"];

                                  var files = Request.Form.Files;
foreach (var file in files) 
{
var filename = ContentDispositionHeaderValue
.Parse(file.ContentDisposition)
.FileName
.Trim('"'); 
string fileExtention = "." + filename.Split('.').Last(); 
Random rnd = new Random();
string uploadFileName = System.Text.RegularExpressions.Regex.Replace(filename.Split('.').First(), @"[^0-9a-zA-Z_.]+", "").Replace(" ", String.Empty)+"_"+"tenant_" +rnd.Next(1, 10000).ToString() + DateTime.Now.ToString("ddMMyyHHmmss")+ fileExtention;
if (fileExtention != ".")
{
Stream stream = file.OpenReadStream();
string fileURL=await util.fileSystem.UploadFileAsync(stream, Configuration.GetSection("AzureBlobStorageSetttings:folderName").Value, uploadFileName);
if (file.Name == "organizationlogo")
{
uploadFileName = "|" + fileURL +"|";
model.organizationlogo +=  uploadFileName ;
}
}
}

								 
                                 strReturnMessage = await ApiClient.Post_ApiValuesGetString(client,"api/tenant/Create_Healthcare_Provider", model);
                                    
								 
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
                 
                 _logger.LogError(ex,"An exception occurred in - tenant / Create_Healthcare_Provider, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
               
				 strReturnMessage = ex.Message;
			 }
		     ViewData["message"] = strReturnMessage;
			 if(strReturnMessage.Replace("\"", "").Contains("201.1")){
				 TempData["message"] = "Success";
				 
                
                MailSender maillog_user = new MailSender();
                 bool userMailsent = await maillog_user.sendMail("users"
                    , "registration"
                    , model.tenantid.ToString()
                    , "registration"
                    , _mailSettings
                    , "13EF2D93-CD5C-49DB-AE70-8FB33399275C"
                    , client, model.businessemail, model.password, baseUrl);

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

				
			  public virtual async Task<IActionResult> Update_Healthcare_Provider(string tenantid)
			  {

                    string redirectTo="";
                    if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
                            DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
                            DataView dv = new DataView(NalamVazharole_JSON);
                            dv.RowFilter = "controllername='tenant' AND viewname='list'";

                            if(dv.Count  >0){
                                redirectTo = dv[0]["actionmethodname"] as string;
							 
                            }

                            try{
                                     var jsonObjtenant = await ApiClient.Get_ApiValues(getHttpClient(), "api/tenant/getById_tenant?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                                if(jsonObjtenant.Length > 2)
                                {
                                  
                                    var model = JsonConvert.DeserializeObject<tenantModel>(jsonObjtenant);


                
                                     
                                    return View(model);
                                }
                                else
                                {
                    
                                    TempData["message"] = "Data Not Found - Contact Administrator";
                                    return RedirectToAction(redirectTo);
						 
                                }

                            }catch(Exception ex){
                               _logger.LogError(ex,"An exception occurred in - tenant / Update_Healthcare_Provider, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                                TempData["errMessage"] = "Error while fetching data - Contact Administrator";
                                return RedirectToAction(redirectTo);
                            }

                    }
                    TempData["errMessage"] = "Session Expired";
                    return RedirectToAction("Logout", "users");
                }	
			  [HttpPost()]
				public virtual async Task<string> Update_Healthcare_Provider(tenantModel model, IFormCollection collection)
				{
					string strReturnMessage = "";
					try
					{
							ModelState.Remove("tenantid");
                            ModelState.Remove("craftmyapp_actionmethodname");
                             model.craftmyapp_actionmethodname="Update_Healthcare_Provider";
							
							
							if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
					model.modifieduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
					else
					return "Session Expired";
							
                            model.organizationlogo = collection["organizationlogo_existing"];

                            ModelState.Remove("organizationlogo");

							if (ModelState.IsValid)
							{
									tenantModelValidator validator = new tenantModelValidator();
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
string uploadFileName = System.Text.RegularExpressions.Regex.Replace(filename.Split('.').First(), @"[^0-9a-zA-Z_.]+", "").Replace(" ", String.Empty)+"_"+"tenant_" +rnd.Next(1, 10000).ToString() + DateTime.Now.ToString("ddMMyyHHmmss")+ fileExtention;
if (fileExtention != ".")
{
Stream stream = file.OpenReadStream();
string fileURL=await util.fileSystem.UploadFileAsync(stream, Configuration.GetSection("AzureBlobStorageSetttings:folderName").Value, uploadFileName);
if (file.Name == "organizationlogo")
{
uploadFileName = "|" + fileURL +"|";
model.organizationlogo +=  uploadFileName ;
}
}
}

                                        
                                        
                                        strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/tenant/Update_Healthcare_Provider", model);
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
                      _logger.LogError(ex,"An exception occurred in - tenant / Update_Healthcare_Provider, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
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
public virtual async Task<IActionResult> Remove_Healthcare_Provider(string tenantid)
			{
				string message = "";
				try
				{
						message = await ApiClient.Get_ApiValues(getHttpClient(), "api/tenant/Remove_Healthcare_Provider?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
						 if(message.Replace("\"","").Contains("201.1"))
						{
							TempData["message"] = "Success";

						}else{
							TempData["errMessage"] = message.Replace("\"","");
						}
						
				
				
				}
				catch (Exception ex)
				{
                     _logger.LogError(ex,"An exception occurred in - tenant / Remove_Healthcare_Provider, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                
					 TempData["errMessage"] = ex.Message;
					 message = ex.Message;
				}

				string redirectTo="";
						if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
					DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
						 DataView dv = new DataView(NalamVazharole_JSON);
						 dv.RowFilter = "controllername='tenant' AND viewname='list'";

						if(dv.Count  >0){
						    redirectTo = dv[0]["actionmethodname"] as string;
							 
						}

					}
				
				return RedirectToAction(redirectTo);
			}

                                        public virtual IActionResult ViewDetails_Healthcare_Provider()
                                        {
                                            return View();
                                        }

			public virtual IActionResult View_List_of_Healthcare_Provider()
			{
				return View();
			}
				
			[HttpGet()]
			public virtual async Task<string> get_View_List_of_Healthcare_Provider(string tenantid = "")
			{
				return await ApiClient.Get_ApiValues(getHttpClient(), "api/tenant/View_List_of_Healthcare_Provider?loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID")+"&tenantid="+tenantid);
			}
			  
											[HttpGet()]
											public virtual async Task<string> get_all_Country(string tenantid)
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/Country/get_all_Country?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											 
											[HttpGet()]
											public virtual async Task<string> get_all_tenant(string tenantid)
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/tenant/get_all_tenant?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											
[HttpGet()]
			public virtual async Task<string> get_project_tenant(string tenantid)
			{
	 			return await ApiClient.Get_ApiValues(getHttpClient(), "api/tenant/get_project_tenant?tenantid="+tenantid);
			}

				
			  public virtual async Task<string> getById_allinfo_tenant(string tenantid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/tenant/getById_allinfo_tenant?tenantid="+tenantid);
					 
			  }
[HttpGet()]
			    public virtual async Task<string> lookup_tenant_country()
			    {
                    
				    return await ApiClient.Get_ApiValues(getHttpClient(), "api/tenant/lookup_tenant_country?loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			    }
[HttpGet()]
			    public virtual async Task<string> lookup_tenant_parentid()
			    {
                    
				    return await ApiClient.Get_ApiValues(getHttpClient(), "api/tenant/lookup_tenant_parentid?loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			    }
[HttpGet()]
                    public virtual async Task<string> get_parentid(string businesstype)
                    {
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/tenant/get_parentid?businesstype="+businesstype);
                    }
[HttpGet()]
			public virtual async Task<string> GetPincodeList(string pincode, int? pagesize = 100, int? pagenumber = 0)
			{
				return await ApiClient.Get_ApiValues(getHttpClient(), "api\\PincodeMaster\\Pincode_List?pincode=" + (string.IsNullOrEmpty(pincode) ? "" : Uri.EscapeDataString(pincode)) + "&pagesize=" + (pagesize ?? 100) + "&pagenumber=" + (pagenumber ?? 0) + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
			}






                    
                     
                        

				}


			}
