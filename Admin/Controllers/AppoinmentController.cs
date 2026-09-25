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

                
                
                
				//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/01/2026 11:54:48
				
                
                
                
                
				public class AppoinmentController : BaseController
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
                    private readonly ILogger<AppoinmentController> _logger;
                    
                    
                    StorageUtil util;
					public AppoinmentController(IConfiguration configuration,IHttpContextAccessor accessor,IOptions<ApiSettings> ApiSettings, IOptions<MailSettings> MailSettings, IWebHostEnvironment env, ILogger<AppoinmentController> logger):base( configuration)
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
	              

					
			  public virtual IActionResult Add_Appoinment()
			  {
					return View();
			  }	
			  [HttpPost()]
			public virtual async Task<string> Add_Appoinment(AppoinmentModel model, IFormCollection collection)
			{
				string strReturnMessage = "";
				
				try
				{
					ModelState.Remove("Appoinmentid");
					ModelState.Remove("createduser");
                    ModelState.Remove("craftmyapp_actionmethodname");
                    model.craftmyapp_actionmethodname="Add_Appoinment";
					if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
								model.createduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
								else
								return "Session Expired";                    
					
                   
					
			 	    
					 if (ModelState.IsValid)
					 {
							 AppoinmentModelValidator validator = new AppoinmentModelValidator();
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
								 model.Appoinmentid =Guid.NewGuid(); 
                                 
                                  
								 
                                 strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/Appoinment/Add_Appoinment", model);
                                    
								 
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
                 
                 _logger.LogError(ex,"An exception occurred in - Appoinment / Add_Appoinment, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
               
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

				
			  public virtual async Task<IActionResult> Update_Appoinment(string Appoinmentid)
			  {

                    string redirectTo="";
                    if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
                            DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
                            DataView dv = new DataView(NalamVazharole_JSON);
                            dv.RowFilter = "controllername='Appoinment' AND viewname='list'";

                            if(dv.Count  >0){
                                redirectTo = dv[0]["actionmethodname"] as string;
							 
                            }

                            try{
                                     var jsonObjAppoinment = await ApiClient.Get_ApiValues(getHttpClient(), "api/Appoinment/getById_Appoinment?Appoinmentid="+Appoinmentid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                                if(jsonObjAppoinment.Length > 2)
                                {
                                  
                                    var model = JsonConvert.DeserializeObject<AppoinmentModel>(jsonObjAppoinment);


                
                                     
                                    return View("Add_Appoinment", model);
                                }
                                else
                                {
                    
                                    TempData["message"] = "Data Not Found - Contact Administrator";
                                    return RedirectToAction(redirectTo);
						 
                                }

                            }catch(Exception ex){
                               _logger.LogError(ex,"An exception occurred in - Appoinment / Update_Appoinment, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                                TempData["errMessage"] = "Error while fetching data - Contact Administrator";
                                return RedirectToAction(redirectTo);
                            }

                    }
                    TempData["errMessage"] = "Session Expired";
                    return RedirectToAction("Logout", "users");
                }	
			  [HttpPost()]
				public virtual async Task<string> Update_Appoinment(AppoinmentModel model, IFormCollection collection)
				{
					string strReturnMessage = "";
					try
					{
							ModelState.Remove("Appoinmentid");
                            ModelState.Remove("craftmyapp_actionmethodname");
                             model.craftmyapp_actionmethodname="Update_Appoinment";
							
							
							if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
					model.modifieduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
					else
					return "Session Expired";
							
                            
                            
							if (ModelState.IsValid)
							{
									AppoinmentModelValidator validator = new AppoinmentModelValidator();
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
                                        
										
                                        
                                        
                                        strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/Appoinment/Update_Appoinment", model);
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
                      _logger.LogError(ex,"An exception occurred in - Appoinment / Update_Appoinment, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
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
public virtual async Task<IActionResult> Remove_Appoinment(string Appoinmentid)
			{
				string message = "";
				try
				{
						message = await ApiClient.Get_ApiValues(getHttpClient(), "api/Appoinment/Remove_Appoinment?Appoinmentid="+Appoinmentid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
						 if(message.Replace("\"","").Contains("201.1"))
						{
							TempData["message"] = "Success";

						}else{
							TempData["errMessage"] = message.Replace("\"","");
						}
						
				
				
				}
				catch (Exception ex)
				{
                     _logger.LogError(ex,"An exception occurred in - Appoinment / Remove_Appoinment, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                
					 TempData["errMessage"] = ex.Message;
					 message = ex.Message;
				}

				string redirectTo="";
						if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
					DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
						 DataView dv = new DataView(NalamVazharole_JSON);
						 dv.RowFilter = "controllername='Appoinment' AND viewname='list'";

						if(dv.Count  >0){
						    redirectTo = dv[0]["actionmethodname"] as string;
							 
						}

					}
				
				return RedirectToAction(redirectTo);
			}

                                        public virtual IActionResult Detail_Appoinment()
                                        {
                                            return View();
                                        }

			        public virtual IActionResult Appoinment_List()
			        {
				        return View();
			        }
			        	
			        [HttpGet()]
			        public virtual async Task<string> get_Appoinment_List(string tenantid
,string patient
,string origin
,string bookingreferencenumber
,string doctor
,string appointmentdate
,string task
,string status
, int? pagesize=100 , int? pagenumber=0,string searchterm="",string sortFieldsJson="")
			        {
				        
				        return await ApiClient.Get_ApiValues(getHttpClient(), "api/Appoinment/Appoinment_List?tenantid="+tenantid+"&patient="+patient+"&origin="+origin+"&bookingreferencenumber="+bookingreferencenumber+"&doctor="+doctor+"&appointmentdate="+Uri.EscapeDataString(appointmentdate ?? "")+"&task="+task+"&status="+status+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID")
+ "&pagesize=" + pagesize + "&pagenumber="+ pagenumber + "&searchterm="+ searchterm + "&sort_fields=" + sortFieldsJson);
			        }
			          
											[HttpGet()]
											public virtual async Task<string> get_all_ClinicalAppointment(string tenantid,string searchterm, string pagesize="1000", string pagenumber="1")
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/ClinicalAppointment/get_all_ClinicalAppointment?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											

				
			  public virtual async Task<string> getById_allinfo_Appoinment(string Appoinmentid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/Appoinment/getById_allinfo_Appoinment?Appoinmentid="+Appoinmentid);
					 
			  }
[HttpGet()]
                    public virtual async Task<string> lookup_Appoinment_patient(String tenantid,string searchterm, int? pagesize, int? pagenumber)
                    {
                        
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/Appoinment/lookup_Appoinment_patient?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }
[HttpGet()]
                    public virtual async Task<string> lookup_Appoinment_origin(String tenantid,string searchterm, int? pagesize, int? pagenumber)
                    {
                        
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/Appoinment/lookup_Appoinment_origin?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }
[HttpGet()]
                    public virtual async Task<string> lookup_Appoinment_bookingreferencenumber(String tenantid,string searchterm, int? pagesize, int? pagenumber)
                    {
                        
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/Appoinment/lookup_Appoinment_bookingreferencenumber?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }
[HttpGet()]
                    public virtual async Task<string> lookup_Appoinment_doctor(String tenantid,string searchterm, int? pagesize, int? pagenumber)
                    {
                        
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/Appoinment/lookup_Appoinment_doctor?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }
[HttpGet()]
                    public virtual async Task<string> lookup_Appoinment_appointmentdate(String tenantid,string searchterm, int? pagesize, int? pagenumber)
                    {
                        
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/Appoinment/lookup_Appoinment_appointmentdate?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }
[HttpGet()]
                    public virtual async Task<string> lookup_Appoinment_task(String tenantid,string searchterm, int? pagesize, int? pagenumber)
                    {
                        
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/Appoinment/lookup_Appoinment_task?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }
[HttpGet()]
                    public virtual async Task<string> lookup_Appoinment_duration(String tenantid,string searchterm, int? pagesize, int? pagenumber)
                    {
                        
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/Appoinment/lookup_Appoinment_duration?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }
[HttpGet()]
                    public virtual async Task<string> lookup_Appoinment_status(String tenantid,string searchterm, int? pagesize, int? pagenumber)
                    {
                        
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/Appoinment/lookup_Appoinment_status?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }






                    
                     
                        

				}


			}
