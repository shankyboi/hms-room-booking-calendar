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

                
                
                
				//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:36
				
                
                
                
                
				public class ClinicalTaskController : BaseController
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
                    private readonly ILogger<ClinicalTaskController> _logger;
                    
                    
                    StorageUtil util;
					public ClinicalTaskController(IConfiguration configuration,IHttpContextAccessor accessor,IOptions<ApiSettings> ApiSettings, IOptions<MailSettings> MailSettings, IWebHostEnvironment env, ILogger<ClinicalTaskController> logger):base( configuration)
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
	              

					
				
			  public virtual async Task<string> getById_taskduration(string ClinicalTaskid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/ClinicalTask/getById_taskduration?ClinicalTaskid="+ClinicalTaskid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
					 
			  }


			  public virtual IActionResult Add_Clinical_Task()
			  {
					return View();
			  }	
			  [HttpPost()]
			public virtual async Task<string> Add_Clinical_Task(ClinicalTaskModel model, IFormCollection collection)
			{
				string strReturnMessage = "";
				
				try
				{
					ModelState.Remove("ClinicalTaskid");
					ModelState.Remove("createduser");
                    ModelState.Remove("craftmyapp_actionmethodname");
                    model.craftmyapp_actionmethodname="Add_Clinical_Task";
					if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
								model.createduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
								else
								return "Session Expired";                    
					
                   
					
			 	    
					 if (ModelState.IsValid)
					 {
							 ClinicalTaskModelValidator validator = new ClinicalTaskModelValidator();
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
								 model.ClinicalTaskid =Guid.NewGuid(); 
                                 
                                  
								 
                                 strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/ClinicalTask/Add_Clinical_Task", model);
                                    
								 
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
                 
                 _logger.LogError(ex,"An exception occurred in - ClinicalTask / Add_Clinical_Task, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
               
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

				
			  public virtual async Task<IActionResult> Update_Clinical_Task(string ClinicalTaskid)
			  {

                    string redirectTo="";
                    if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
                            DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
                            DataView dv = new DataView(NalamVazharole_JSON);
                            dv.RowFilter = "controllername='ClinicalTask' AND viewname='list'";

                            if(dv.Count  >0){
                                redirectTo = dv[0]["actionmethodname"] as string;
							 
                            }

                            try{
                                     var jsonObjClinicalTask = await ApiClient.Get_ApiValues(getHttpClient(), "api/ClinicalTask/getById_ClinicalTask?ClinicalTaskid="+ClinicalTaskid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                                if(jsonObjClinicalTask.Length > 2)
                                {
                                  
                                    var model = JsonConvert.DeserializeObject<ClinicalTaskModel>(jsonObjClinicalTask);


                
                                     
                                    return View("Add_Clinical_Task", model);
                                }
                                else
                                {
                    
                                    TempData["message"] = "Data Not Found - Contact Administrator";
                                    return RedirectToAction(redirectTo);
						 
                                }

                            }catch(Exception ex){
                               _logger.LogError(ex,"An exception occurred in - ClinicalTask / Update_Clinical_Task, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                                TempData["errMessage"] = "Error while fetching data - Contact Administrator";
                                return RedirectToAction(redirectTo);
                            }

                    }
                    TempData["errMessage"] = "Session Expired";
                    return RedirectToAction("Logout", "users");
                }	
			  [HttpPost()]
				public virtual async Task<string> Update_Clinical_Task(ClinicalTaskModel model, IFormCollection collection)
				{
					string strReturnMessage = "";
					try
					{
							ModelState.Remove("ClinicalTaskid");
                            ModelState.Remove("craftmyapp_actionmethodname");
                             model.craftmyapp_actionmethodname="Update_Clinical_Task";
							
							
							if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
					model.modifieduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
					else
					return "Session Expired";
							
                            
                            
							if (ModelState.IsValid)
							{
									ClinicalTaskModelValidator validator = new ClinicalTaskModelValidator();
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
                                        
										
                                        
                                        
                                        strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/ClinicalTask/Update_Clinical_Task", model);
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
                      _logger.LogError(ex,"An exception occurred in - ClinicalTask / Update_Clinical_Task, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
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
public virtual async Task<IActionResult> Remove_Clinical_Task(string ClinicalTaskid)
			{
				string message = "";
				try
				{
						message = await ApiClient.Get_ApiValues(getHttpClient(), "api/ClinicalTask/Remove_Clinical_Task?ClinicalTaskid="+ClinicalTaskid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
						 if(message.Replace("\"","").Contains("201.1"))
						{
							TempData["message"] = "Success";

						}else{
							TempData["errMessage"] = message.Replace("\"","");
						}
						
				
				
				}
				catch (Exception ex)
				{
                     _logger.LogError(ex,"An exception occurred in - ClinicalTask / Remove_Clinical_Task, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                
					 TempData["errMessage"] = ex.Message;
					 message = ex.Message;
				}

				string redirectTo="";
						if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
					DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
						 DataView dv = new DataView(NalamVazharole_JSON);
						 dv.RowFilter = "controllername='ClinicalTask' AND viewname='list'";

						if(dv.Count  >0){
						    redirectTo = dv[0]["actionmethodname"] as string;
							 
						}

					}
				
				return RedirectToAction(redirectTo);
			}

                                        public virtual IActionResult View_Clinical_Task()
                                        {
                                            return View();
                                        }

			public virtual IActionResult Clinical_Task_List()
			{
				return View();
			}
				
			[HttpGet()]
			public virtual async Task<string> get_Clinical_Task_List(string tenantid
)
			{
				
				return await ApiClient.Get_ApiValues(getHttpClient(), "api/ClinicalTask/Clinical_Task_List?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID")
);
			}
			  
											[HttpGet()]
											public virtual async Task<string> get_all_WorkProfile(string tenantid)
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/WorkProfile/get_all_WorkProfile?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											 
											[HttpGet()]
											public virtual async Task<string> get_all_Competency(string tenantid)
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/Competency/get_all_Competency?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											

				
			  public virtual async Task<string> getById_allinfo_ClinicalTask(string ClinicalTaskid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/ClinicalTask/getById_allinfo_ClinicalTask?ClinicalTaskid="+ClinicalTaskid);
					 
			  }
[HttpGet()]
			    public virtual async Task<string> lookup_ClinicalTask_workprofile(String tenantid)
			    {
                    
				    return await ApiClient.Get_ApiValues(getHttpClient(), "api/ClinicalTask/lookup_ClinicalTask_workprofile?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			    }
[HttpGet()]
			    public virtual async Task<string> lookup_ClinicalTask_competency(String tenantid)
			    {
                    
				    return await ApiClient.Get_ApiValues(getHttpClient(), "api/ClinicalTask/lookup_ClinicalTask_competency?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			    }

[HttpGet()]
                        public virtual async Task<string> lookup_ClinicalTask_taskduration_workprofile(String tenantid)
                        {
                            return await ApiClient.Get_ApiValues(getHttpClient(), "api/ClinicalTask/lookup_ClinicalTask_taskduration_workprofile?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                        }
[HttpGet()]
                        public virtual async Task<string> lookup_ClinicalTask_taskduration_tasktype(String tenantid)
                        {
                            return await ApiClient.Get_ApiValues(getHttpClient(), "api/ClinicalTask/lookup_ClinicalTask_taskduration_tasktype?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                        }
[HttpGet()]
                        public virtual async Task<string> lookup_ClinicalTask_taskduration_taskname(String tenantid,String tasktype)
                        {
                            return await ApiClient.Get_ApiValues(getHttpClient(), "api/ClinicalTask/lookup_ClinicalTask_taskduration_taskname?tenantid="+tenantid+"&tasktype="+tasktype+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                        }

[HttpGet()]
			    public virtual async Task<string> lookup_ClinicalTask_taskname(String tenantid,String tasktype)
			    {
				    return await ApiClient.Get_ApiValues(getHttpClient(), "api/ClinicalTask/lookup_ClinicalTask_taskname?tenantid="+tenantid+"&tasktype="+tasktype+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			    } 
											[HttpGet()]
											public virtual async Task<string> get_all_Task(string tenantid)
											{
											 
											    return await ApiClient.Get_ApiValues(getHttpClient(), "api/Task/get_all_Task?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											
 
											[HttpGet()]
											public virtual async Task<string> get_all_TaskType(string tenantid)
											{
											 
											    return await ApiClient.Get_ApiValues(getHttpClient(), "api/TaskType/get_all_TaskType?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											




                    
                     
                        

				}


			}
