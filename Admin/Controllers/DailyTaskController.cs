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

                
                
                
				//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/03/2026 08:39:41
				
                
                
                
                
				public class DailyTaskController : BaseController
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
                    private readonly ILogger<DailyTaskController> _logger;
                    
                    
                    StorageUtil util;

					[HttpPost]
					public virtual async Task<string> Send_Task_Escalation(string taskTemplateId, string dailyTaskId, string comments)
					{
						try
						{
							var tenantId = HttpContext.Session.GetString("NalamVazhachoosedtenantid");
							var loginUserId = HttpContext.Session.GetString("NalamVazhaloginUserID");
							if (string.IsNullOrWhiteSpace(tenantId) || string.IsNullOrWhiteSpace(loginUserId))
								return "Session Expired";
							if (!Guid.TryParse(taskTemplateId, out _) || !Guid.TryParse(dailyTaskId, out _))
								return "Invalid task.";

							var escalationJson = await ApiClient.Get_ApiValues(getHttpClient(),
								"api/TaskTemplate/getById_escalationdetails?TaskTemplateid=" + Uri.EscapeDataString(taskTemplateId) +
								"&loginUserID=" + Uri.EscapeDataString(loginUserId));
							var escalationRows = JsonConvert.DeserializeObject<DataTable>(escalationJson);
							var recipient = escalationRows?.AsEnumerable()
								.Select(row => row["emailid"]?.ToString()?.Trim())
								.FirstOrDefault(email => !string.IsNullOrWhiteSpace(email));
							if (string.IsNullOrWhiteSpace(recipient))
								return "Escalation details are not configured for this task template.";

							var mailSender = new MailSender();
							var mailBox = await mailSender.GetTenantMailBox(getHttpClient(), tenantId,
								MailSender.ResolveApplicableService("DailyTask", "Escalation"));
							if (mailBox == null)
								return "Mail box is not configured for this Healthcare Provider.";

							var safeComments = WebUtility.HtmlEncode(comments?.Trim() ?? "");
							var subject = "Daily Task Escalation";
							var body = "<p>A Daily Task has been escalated.</p>" +
								"<p><strong>Task ID:</strong> " + WebUtility.HtmlEncode(dailyTaskId) + "</p>" +
								"<p><strong>Comments:</strong> " + safeComments + "</p>";
							return new Mailer(mailBox).SendMail_TLS(recipient, null, subject, body, true, null, true)
								? "Success" : "Unable to send escalation email.";
						}
						catch (Exception ex)
						{
							_logger.LogError(ex, "Unable to send Daily Task escalation email.");
							return "Unable to send escalation email.";
						}
					}
					public DailyTaskController(IConfiguration configuration,IHttpContextAccessor accessor,IOptions<ApiSettings> ApiSettings, IOptions<MailSettings> MailSettings, IWebHostEnvironment env, ILogger<DailyTaskController> logger):base( configuration)
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
	              

					
			  public virtual IActionResult Add_Daily_Task()
			  {
					return View();
			  }	
			  [HttpPost()]
			public virtual async Task<string> Add_Daily_Task(DailyTaskModel model, IFormCollection collection)
			{
				string strReturnMessage = "";
				
				try
				{
					ModelState.Remove("DailyTaskid");
					ModelState.Remove("createduser");
                    ModelState.Remove("craftmyapp_actionmethodname");
                    model.craftmyapp_actionmethodname="Add_Daily_Task";
					if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
								model.createduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
								else
								return "Session Expired";                    
					
                   
					
			 	    
					 if (ModelState.IsValid)
					 {
							 DailyTaskModelValidator validator = new DailyTaskModelValidator();
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
								 model.DailyTaskid =Guid.NewGuid(); 
                                 
                                  
								 
                                 strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/DailyTask/Add_Daily_Task", model);
                                    
								 
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
                 
                 _logger.LogError(ex,"An exception occurred in - DailyTask / Add_Daily_Task, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
               
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

		[HttpPost()]
		public virtual async Task<string> Close_Daily_Task(CloseDailyTaskModel model)
		{
			if (HttpContext.Session.GetString("NalamVazhaloginUserID") == null)
				return "Session Expired";

			string response = await ApiClient.Post_ApiValuesGetString(
				getHttpClient(), "api/DailyTask/Close_Daily_Task", model);
			string normalized = response.Replace("\"", "").Trim();
			if (normalized.Contains("201.1")) return "Success";
			return normalized.Replace("BadRequest :", "").Trim();
		}

				
			  public virtual async Task<IActionResult> Update_Daily_Task(string DailyTaskid)
			  {

                    string redirectTo="";
                    if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
                            DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
                            DataView dv = new DataView(NalamVazharole_JSON);
                            dv.RowFilter = "controllername='DailyTask' AND viewname='list'";

                            if(dv.Count  >0){
                                redirectTo = dv[0]["actionmethodname"] as string;
							 
                            }

                            try{
                                     var jsonObjDailyTask = await ApiClient.Get_ApiValues(getHttpClient(), "api/DailyTask/getById_DailyTask?DailyTaskid="+DailyTaskid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                                if(jsonObjDailyTask.Length > 2)
                                {
                                  
                                    var model = JsonConvert.DeserializeObject<DailyTaskModel>(jsonObjDailyTask);


                
                                     
                                    return View(model);
                                }
                                else
                                {
                    
                                    TempData["message"] = "Data Not Found - Contact Administrator";
                                    return RedirectToAction(redirectTo);
						 
                                }

                            }catch(Exception ex){
                               _logger.LogError(ex,"An exception occurred in - DailyTask / Update_Daily_Task, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                                TempData["errMessage"] = "Error while fetching data - Contact Administrator";
                                return RedirectToAction(redirectTo);
                            }

                    }
                    TempData["errMessage"] = "Session Expired";
                    return RedirectToAction("Logout", "users");
                }	
			  [HttpPost()]
				public virtual async Task<string> Update_Daily_Task(DailyTaskModel model, IFormCollection collection)
				{
					string strReturnMessage = "";
					try
					{
							ModelState.Remove("DailyTaskid");
                            ModelState.Remove("craftmyapp_actionmethodname");
                             model.craftmyapp_actionmethodname="Update_Daily_Task";
							
							
							if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
					model.modifieduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
					else
					return "Session Expired";
							
                            
                            
							if (ModelState.IsValid)
							{
									DailyTaskModelValidator validator = new DailyTaskModelValidator();
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
                                        
										
                                        
                                        
                                        strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/DailyTask/Update_Daily_Task", model);
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
                      _logger.LogError(ex,"An exception occurred in - DailyTask / Update_Daily_Task, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
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
public virtual async Task<IActionResult> Remove_Daily_Task(string DailyTaskid)
			{
				string message = "";
				try
				{
						message = await ApiClient.Get_ApiValues(getHttpClient(), "api/DailyTask/Remove_Daily_Task?DailyTaskid="+DailyTaskid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
						 if(message.Replace("\"","").Contains("201.1"))
						{
							TempData["message"] = "Success";

						}else{
							TempData["errMessage"] = message.Replace("\"","");
						}
						
				
				
				}
				catch (Exception ex)
				{
                     _logger.LogError(ex,"An exception occurred in - DailyTask / Remove_Daily_Task, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                
					 TempData["errMessage"] = ex.Message;
					 message = ex.Message;
				}

				string redirectTo="";
						if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
					DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
						 DataView dv = new DataView(NalamVazharole_JSON);
						 dv.RowFilter = "controllername='DailyTask' AND viewname='list'";

						if(dv.Count  >0){
						    redirectTo = dv[0]["actionmethodname"] as string;
							 
						}

					}
				
				return RedirectToAction(redirectTo);
			}

                                        public virtual IActionResult Detail_Daily_Task()
                                        {
                                            return View();
                                        }

			        public virtual IActionResult Daily_Task__List()
			        {
				        return View();
			        }

                    [HttpGet]
                    public virtual async Task<string> get_all_users(string tenantid)
                    {
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/users/get_all_users?tenantid=" + tenantid + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }
			        	
			        [HttpGet()]
			        public virtual async Task<string> get_Daily_Task__List(string tenantid
,string tasktype
,string taskname
,string patientname
,string ipdreferencenumber
,string opdreferencenumber
, int? pagesize=100 , int? pagenumber=0,string searchterm="",string sortFieldsJson="")
			        {
				        
				        return await ApiClient.Get_ApiValues(getHttpClient(), "api/DailyTask/Daily_Task__List?tenantid="+tenantid+"&tasktype="+tasktype+"&taskname="+taskname+"&patientname="+patientname+"&ipdreferencenumber="+ipdreferencenumber+"&opdreferencenumber="+opdreferencenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID")
+ "&pagesize=" + pagesize + "&pagenumber="+ pagenumber + "&searchterm="+ searchterm + "&sort_fields=" + sortFieldsJson);
			        }
			          
											[HttpGet()]
											public virtual async Task<string> get_all_TypeofTask(string tenantid,string searchterm, string pagesize="1000", string pagenumber="1")
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/TypeofTask/get_all_TypeofTask?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											 
											[HttpGet()]
											public virtual async Task<string> get_all_TaskTemplate(string tenantid,string searchterm, string pagesize="1000", string pagenumber="1")
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/TaskTemplate/get_all_TaskTemplate?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											 
											[HttpGet()]
											public virtual async Task<string> get_all_PatientProfile(string tenantid,string searchterm, string pagesize="1000", string pagenumber="1")
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/PatientProfile/get_all_PatientProfile?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											 
											[HttpGet()]
											public virtual async Task<string> get_all_IPDApplicationForm(string tenantid,string searchterm, string pagesize="1000", string pagenumber="1")
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/IPDApplicationForm/get_all_IPDApplicationForm?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											 
											[HttpGet()]
											public virtual async Task<string> get_all_OPDForm(string tenantid,string searchterm, string pagesize="1000", string pagenumber="1")
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/OPDForm/get_all_OPDForm?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											

				
			  public virtual async Task<string> getById_allinfo_DailyTask(string DailyTaskid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/DailyTask/getById_allinfo_DailyTask?DailyTaskid="+DailyTaskid);
					 
			  }
[HttpGet()]
                    public virtual async Task<string> lookup_DailyTask_tasktype(String tenantid,string searchterm, int? pagesize, int? pagenumber)
                    {
                        
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/DailyTask/lookup_DailyTask_tasktype?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }
[HttpGet()]
                    public virtual async Task<string> lookup_DailyTask_taskname(String tenantid,String tasktype,string searchterm, int? pagesize, int? pagenumber)
                    {
                        
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/DailyTask/lookup_DailyTask_taskname?tenantid="+tenantid+"&tasktype="+tasktype+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }
[HttpGet()]
                    public virtual async Task<string> lookup_DailyTask_patientname(String tenantid,string searchterm, int? pagesize, int? pagenumber)
                    {
                        
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/DailyTask/lookup_DailyTask_patientname?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }
[HttpGet()]
                    public virtual async Task<string> lookup_DailyTask_ipdreferencenumber(String tenantid,String patientname,string searchterm, int? pagesize, int? pagenumber)
                    {
                        
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/DailyTask/lookup_DailyTask_ipdreferencenumber?tenantid="+tenantid+"&patientname="+patientname+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }
[HttpGet()]
                    public virtual async Task<string> lookup_DailyTask_opdreferencenumber(String tenantid,String patientname,string searchterm, int? pagesize, int? pagenumber)
                    {
                        
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/DailyTask/lookup_DailyTask_opdreferencenumber?tenantid="+tenantid+"&patientname="+patientname+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }









                    [HttpGet()]
                    public virtual async Task<IActionResult> get_DailyTask_detail(string DailyTaskid)
                    {
                        try
                        {
                            var json = await ApiClient.Get_ApiValues(getHttpClient(),
                                "api/DailyTask/getById_allinfo_DailyTask?DailyTaskid=" + DailyTaskid +
                                "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
                            ViewBag.TaskDetailJson = json ?? "{}";
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "get_DailyTask_detail error");
                            ViewBag.TaskDetailJson = "{}";
                        }
                        ViewBag.DailyTaskId  = DailyTaskid ?? "";
                        ViewBag.TenantId     = HttpContext.Session.GetString("NalamVazhachoosedtenantid") ?? "";
                        ViewBag.LoginUserId  = HttpContext.Session.GetString("NalamVazhaloginUserID") ?? "";
                        ViewBag.Username     = HttpContext.Session.GetString("NalamVazhausername") ?? "";
                        return PartialView("_DailyTask_Detail");
                    }

				}


			}
