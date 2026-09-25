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

                
                
                
				//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:42
				
                
                
                
                
				public class EnquiryFormController : BaseController
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
                    private readonly ILogger<EnquiryFormController> _logger;
                    
                    
                    StorageUtil util;
					public EnquiryFormController(IConfiguration configuration,IHttpContextAccessor accessor,IOptions<ApiSettings> ApiSettings, IOptions<MailSettings> MailSettings, IWebHostEnvironment env, ILogger<EnquiryFormController> logger):base( configuration)
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
	              

					
				
			  public virtual async Task<string> getById_medicalinfo(string EnquiryFormid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/EnquiryForm/getById_medicalinfo?EnquiryFormid="+EnquiryFormid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
					 
			  }


			  public virtual async Task<IActionResult> Add_Enquiry()
			  {
					var model = new EnquiryFormModel();
					var userRole = HttpContext.Session.GetString("NalamVazhauserrole");
					var loginUserID = HttpContext.Session.GetString("NalamVazhaloginUserID");
					if (string.Equals(userRole, "Health Seeker", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(loginUserID))
					{
						try
						{
							var patientJson = await ApiClient.Get_ApiValues(getHttpClient(), "api/PatientProfile/getById_allinfo_PatientProfile?PatientProfileid=" + Uri.EscapeDataString(loginUserID));
							if (!string.IsNullOrWhiteSpace(patientJson) && patientJson.TrimStart().StartsWith("["))
							{
								var arr = Newtonsoft.Json.Linq.JArray.Parse(patientJson);
								var first = arr?.FirstOrDefault() as Newtonsoft.Json.Linq.JObject;
								if (first != null)
								{
									model.patientname = Guid.Parse(loginUserID);
									model.firstname = first["firstname"]?.ToString() ?? "";
									model.lastname = first["lastname"]?.ToString() ?? "";
									model.gender = first["gender"]?.ToString() ?? "";
									model.age = first["age"] != null ? (long)first["age"] : 0;
									model.phonenumber = first["mobilenumber"]?.ToString() ?? "";
									model.emailaddress = first["emailaddress"]?.ToString() ?? "";
									model.enquirystatus = "New";
								}
							}
							else if (!string.IsNullOrWhiteSpace(patientJson) && patientJson.Length > 2)
							{
								var patient = JsonConvert.DeserializeObject<PatientProfileModel>(patientJson);
								if (patient != null)
								{
									model.patientname = Guid.Parse(loginUserID);
									model.firstname = patient.firstname ?? "";
									model.lastname = patient.lastname ?? "";
									model.gender = patient.gender ?? "";
									model.age = (long)patient.age;
									model.phonenumber = patient.mobilenumber ?? "";
									model.emailaddress = patient.emailaddress ?? "";
									model.enquirystatus = "New";
								}
							}
						}
						catch (Exception ex)
						{
							_logger.LogWarning(ex, "Add_Enquiry: Could not pre-fill Health Seeker patient profile.");
						}
					}
					return View(model);
			  }	
			  [HttpPost()]
			public virtual async Task<string> Add_Enquiry(EnquiryFormModel model, IFormCollection collection)
			{
				string strReturnMessage = "";
				
				try
				{
					ModelState.Remove("EnquiryFormid");
					ModelState.Remove("createduser");
                    ModelState.Remove("craftmyapp_actionmethodname");
                    model.craftmyapp_actionmethodname="Add_Enquiry";
					if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
								model.createduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
								else
								return "Session Expired";                    
					
                   
					
			 	    
					 if (ModelState.IsValid)
					 {
							 EnquiryFormModelValidator validator = new EnquiryFormModelValidator();
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
								 model.EnquiryFormid =Guid.NewGuid(); 
                                 
                                  
								 
                                 strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/EnquiryForm/Add_Enquiry", model);
                                    
								 
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
                 
                 _logger.LogError(ex,"An exception occurred in - EnquiryForm / Add_Enquiry, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
               
				 strReturnMessage = ex.Message;
			 }
		     ViewData["message"] = strReturnMessage;
			 if(strReturnMessage.Replace("\"", "").Contains("201.1")){
				 TempData["message"] = "Success";
				 MailSender maillog = new MailSender();
                    bool mailsent = await maillog.sendNotification("EnquiryForm"
                    , "ReadyForReview"
                    , model.EnquiryFormid.ToString()
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

				
			  public virtual async Task<IActionResult> Update_Enquiry(string EnquiryFormid)
			  {
                    // Only Health Seeker and Frontdesk Admin may update an enquiry
                    var _updateRole = HttpContext.Session.GetString("NalamVazhauserrole");
                    if (!string.Equals(_updateRole, "Health Seeker", StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(_updateRole, "Frontdesk Admin", StringComparison.OrdinalIgnoreCase))
                    {
                        TempData["errMessage"] = "Access Denied";
                        return RedirectToAction("RoleAuthorizationFailed", "users");
                    }

                    string redirectTo="";
                    if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
                            DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
                            DataView dv = new DataView(NalamVazharole_JSON);
                            dv.RowFilter = "controllername='EnquiryForm' AND viewname='list'";

                            if(dv.Count  >0){
                                redirectTo = dv[0]["actionmethodname"] as string;
							 
                            }

                            try{
                                     var jsonObjEnquiryForm = await ApiClient.Get_ApiValues(getHttpClient(), "api/EnquiryForm/getById_EnquiryForm?EnquiryFormid="+EnquiryFormid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                                if(jsonObjEnquiryForm.Length > 2)
                                {
                                  
                                    var model = JsonConvert.DeserializeObject<EnquiryFormModel>(jsonObjEnquiryForm);

                                    // Health Seeker can only update their own enquiries
                                    if (string.Equals(HttpContext.Session.GetString("NalamVazhauserrole"), "Health Seeker", StringComparison.OrdinalIgnoreCase))
                                    {
                                        if (model.patientname == null ||
                                            !string.Equals(model.patientname.ToString(), HttpContext.Session.GetString("NalamVazhaloginUserID"), StringComparison.OrdinalIgnoreCase))
                                        {
                                            TempData["errMessage"] = "Access Denied - You can only update your own enquiries";
                                            return RedirectToAction(redirectTo);
                                        }
                                    }

                                    return View("Update_Enquiry", model);
                                }
                                else
                                {
                    
                                    TempData["message"] = "Data Not Found - Contact Administrator";
                                    return RedirectToAction(redirectTo);
						 
                                }

                            }catch(Exception ex){
                               _logger.LogError(ex,"An exception occurred in - EnquiryForm / Update_Enquiry, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                                TempData["errMessage"] = "Error while fetching data - Contact Administrator";
                                return RedirectToAction(redirectTo);
                            }

                    }
                    TempData["errMessage"] = "Session Expired";
                    return RedirectToAction("Logout", "users");
                }	
			  [HttpPost()]
				public virtual async Task<string> Update_Enquiry(EnquiryFormModel model, IFormCollection collection)
				{
					string strReturnMessage = "";
					try
					{
							ModelState.Remove("EnquiryFormid");
                            ModelState.Remove("craftmyapp_actionmethodname");
                             model.craftmyapp_actionmethodname="Update_Enquiry";
							
							
							if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
					model.modifieduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
					else
					return "Session Expired";
							
                            
                            
							if (ModelState.IsValid)
							{
									EnquiryFormModelValidator validator = new EnquiryFormModelValidator();
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
                                        
										
                                        
                                        
                                        strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/EnquiryForm/Update_Enquiry", model);
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
                      _logger.LogError(ex,"An exception occurred in - EnquiryForm / Update_Enquiry, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
						strReturnMessage = ex.Message;
					}
					ViewData["message"] = strReturnMessage;
					    if(strReturnMessage.Replace("\"", "")=="201.1"){
							TempData["message"] = "Success";

							// Send notification when Frontdesk Admin updates the enquiry status
							var _frontdeskRole = HttpContext.Session.GetString("NalamVazhauserrole");
							if (string.Equals(_frontdeskRole, "Frontdesk Admin", StringComparison.OrdinalIgnoreCase))
							{
								MailSender maillog = new MailSender();
								bool mailsent = await maillog.sendNotification("EnquiryForm"
									, "EnquiryStatusUpdate"
									, model.EnquiryFormid.ToString()
									, _mailSettings
									, model.modifieduser.ToString()
									, client
									, tenantid: model.tenantid?.ToString() ?? "");
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
public virtual async Task<IActionResult> Remove_Enquiry(string EnquiryFormid)
			{
				string message = "";
				try
				{
						message = await ApiClient.Get_ApiValues(getHttpClient(), "api/EnquiryForm/Remove_Enquiry?EnquiryFormid="+EnquiryFormid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
						 if(message.Replace("\"","").Contains("201.1"))
						{
							TempData["message"] = "Success";

						}else{
							TempData["errMessage"] = message.Replace("\"","");
						}
						
				
				
				}
				catch (Exception ex)
				{
                     _logger.LogError(ex,"An exception occurred in - EnquiryForm / Remove_Enquiry, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                
					 TempData["errMessage"] = ex.Message;
					 message = ex.Message;
				}

				string redirectTo="";
						if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
					DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
						 DataView dv = new DataView(NalamVazharole_JSON);
						 dv.RowFilter = "controllername='EnquiryForm' AND viewname='list'";

						if(dv.Count  >0){
						    redirectTo = dv[0]["actionmethodname"] as string;
							 
						}

					}
				
				return RedirectToAction(redirectTo);
			}

			        public virtual IActionResult Added_Enquiries()
			        {
				        return View();
			        }

        [HttpGet()]
        public virtual async Task<string> get_Added_Enquiries(
string tenantid,
string enquirynumber,
string patientname,
string phonenumber,
string emailaddress,
string enquirystatus,
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
                "api/EnquiryForm/Added_Enquiries?tenantid=" + tenantid +
                "&enquirynumber=" + enquirynumber +
                "&patientname=" + patientname +
                "&phonenumber=" + phonenumber +
                "&emailaddress=" + emailaddress +
                "&enquirystatus=" + enquirystatus +
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
											public virtual async Task<string> get_all_EnquiryType(string tenantid)
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/EnquiryType/get_all_EnquiryType?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											 
											[HttpGet()]
											public virtual async Task<string> get_all_PatientProfile(string tenantid,string searchterm, string pagesize="1000", string pagenumber="1")
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/PatientProfile/get_all_PatientProfile?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											 
											[HttpGet()]
											public virtual async Task<string> get_all_RoomType(string tenantid)
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/RoomType/get_all_RoomType?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											

			 
			[HttpGet()]
			public virtual async Task<string> count_of_EnquiryForm_enquirystatus(string tenantid
,string enquirynumber
,string patientname
,string phonenumber
,string emailaddress
,string enquirystatus
)
			{
				
				return await ApiClient.Get_ApiValues(getHttpClient(), "api/EnquiryForm/count_of_EnquiryForm_enquirystatus?tenantid="+tenantid+"&enquirynumber="+enquirynumber+"&patientname="+patientname+"&phonenumber="+phonenumber+"&emailaddress="+emailaddress+"&enquirystatus="+enquirystatus+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID")
);
			}
			 

                                        public virtual IActionResult Enquiry_Details()
                                        {
                                            return View();
                                        }

			        public virtual IActionResult Enquiries_for_Review()
			        {
				        return View();
			        }
			        	
			        [HttpGet()]
			        public virtual async Task<string> get_Enquiries_for_Review(string tenantid
,string enquirystatus
,string verifiedstatus
, int? pagesize=100 , int? pagenumber=0,string searchterm="",string sortFieldsJson="")
			        {
				        
				        return await ApiClient.Get_ApiValues(getHttpClient(), "api/EnquiryForm/Enquiries_for_Review?tenantid="+tenantid+"&enquirystatus="+enquirystatus+"&verifiedstatus="+verifiedstatus+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID")
+ "&pagesize=" + pagesize + "&pagenumber="+ pagenumber + "&searchterm="+ searchterm + "&sort_fields=" + sortFieldsJson);
			        }
			         

			 
			[HttpGet()]
			public virtual async Task<string> count_of_EnquiryForm(string tenantid
,string enquirystatus
)
			{
				
				return await ApiClient.Get_ApiValues(getHttpClient(), "api/EnquiryForm/count_of_EnquiryForm?tenantid="+tenantid+"&enquirystatus="+enquirystatus+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID")
);
			}
			 

            [HttpPost()]
            public virtual async Task<string> verify_EnquiryForm([FromBody]EnquiryFormReviewModel model)
			{
				string message = "";
				try
				{
					 	message = await ApiClient.Post_ApiValuesGetString(getHttpClient(), "api/EnquiryForm/verify_EnquiryForm", model);
						if(message.Replace("\"","")=="201.1")
						{
							TempData["message"] = "Success";

						}else{
							TempData["errMessage"] = message.Replace("\"","");
						}

						message=message.Replace("\"","");
						
				
				
				}
				catch (Exception ex)
				{
                    
                      _logger.LogError(ex,"An exception occurred in - EnquiryForm / verify_EnquiryForm, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                  
					 TempData["errMessage"] = ex.Message;
					 message = ex.Message;
				}
 
				
				return message;
			}

                                        public virtual IActionResult View_Enquiry()
                                        {
                                            return View();
                                        }

			        public virtual IActionResult Enquiries()
			        {
				        return View();
			        }
			        	
			        [HttpGet()]
			        public virtual async Task<string> get_Enquiries(string tenantid
,string enquirystatus
, int? pagesize=100 , int? pagenumber=0,string searchterm="",string sortFieldsJson="")
			        {
				        
				        return await ApiClient.Get_ApiValues(getHttpClient(), "api/EnquiryForm/Enquiries?tenantid="+tenantid+"&enquirystatus="+enquirystatus+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID")
+ "&pagesize=" + pagesize + "&pagenumber="+ pagenumber + "&searchterm="+ searchterm + "&sort_fields=" + sortFieldsJson);
			        }
			         

                                        public virtual IActionResult View_Enquiry_Detail()
                                        {
                                            return View();
                                        }

				
			  public virtual async Task<string> getById_allinfo_EnquiryForm(string EnquiryFormid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/EnquiryForm/getById_allinfo_EnquiryForm?EnquiryFormid="+EnquiryFormid);
					 
			  }
[HttpGet()]
			    public virtual async Task<string> lookup_EnquiryForm_enquirytype(String tenantid)
			    {
                    
				    return await ApiClient.Get_ApiValues(getHttpClient(), "api/EnquiryForm/lookup_EnquiryForm_enquirytype?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			    }
[HttpGet()]
                    public virtual async Task<string> lookup_EnquiryForm_patientname(String tenantid,string searchterm, int? pagesize, int? pagenumber)
                    {
                        
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/EnquiryForm/lookup_EnquiryForm_patientname?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }
[HttpGet()]
			    public virtual async Task<string> lookup_EnquiryForm_preferredroomtype(String tenantid)
			    {
                    
				    return await ApiClient.Get_ApiValues(getHttpClient(), "api/EnquiryForm/lookup_EnquiryForm_preferredroomtype?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			    }

[HttpGet()]
                        public virtual async Task<string> lookup_EnquiryForm_medicalinfo_medicalcondition(string searchterm, int? pagesize, int? pagenumber)
                        {
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/EnquiryForm/lookup_EnquiryForm_medicalinfo_medicalcondition?searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                        }

 
                            [HttpGet()]
                            public virtual async Task<string> get_all_MedicalCondition(string tenantid,string searchterm, string pagesize="1000", string pagenumber="1")
                            {

                                return await ApiClient.Get_ApiValues(getHttpClient(), "api/MedicalCondition/get_all_MedicalCondition?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                            }
                            

[HttpGet()]
			public virtual async Task<string> lookup_change_EnquiryForm_enquirytype(string EnquiryTypeid)
			{
				return await ApiClient.Get_ApiValues(getHttpClient(), "api/EnquiryForm/lookup_change_EnquiryForm_enquirytype?EnquiryTypeid="+EnquiryTypeid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			}

[HttpGet()]
			public virtual async Task<string> lookup_change_medicalinfo_EnquiryForm_medicalcondition(string MedicalConditionid)
			{
				return await ApiClient.Get_ApiValues(getHttpClient(), "api/EnquiryForm/lookup_change_medicalinfo_EnquiryForm_medicalcondition?MedicalConditionid="+MedicalConditionid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			}


                    
                     
                        

				}


			}
