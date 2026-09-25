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

                
                
                
				//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/23/2026 14:00:33
				
                
                
                
                
				public class PeopleTimePreferenceController : BaseController
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
                    private readonly ILogger<PeopleTimePreferenceController> _logger;
                    
                    
                    StorageUtil util;
					public PeopleTimePreferenceController(IConfiguration configuration,IHttpContextAccessor accessor,IOptions<ApiSettings> ApiSettings, IOptions<MailSettings> MailSettings, IWebHostEnvironment env, ILogger<PeopleTimePreferenceController> logger):base( configuration)
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
	              

					
				
			  public virtual async Task<string> getById_timepreference(string PeopleTimePreferenceid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/PeopleTimePreference/getById_timepreference?PeopleTimePreferenceid="+PeopleTimePreferenceid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
					 
			  }

				
			  public virtual async Task<string> prefill_PeopleTimePreference_timepreference(string people)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/PeopleTimePreference/prefill_PeopleTimePreference_timepreference?people="+people+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
					 
			  }

			  public virtual IActionResult Add_People_Time_Preference()
			  {
					return View();
			  }	
			  [HttpPost()]
			public virtual async Task<string> Add_People_Time_Preference(PeopleTimePreferenceModel model, IFormCollection collection)
			{
				string strReturnMessage = "";
				
				try
				{
					ModelState.Remove("PeopleTimePreferenceid");
					ModelState.Remove("createduser");
                    ModelState.Remove("craftmyapp_actionmethodname");
                    model.craftmyapp_actionmethodname="Add_People_Time_Preference";
					if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
								model.createduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
								else
								return "Session Expired";                    
					
                   
					
			 	    
					 if (ModelState.IsValid)
					 {
							 PeopleTimePreferenceModelValidator validator = new PeopleTimePreferenceModelValidator();
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
								 model.PeopleTimePreferenceid =Guid.NewGuid(); 
                                 
                                  
								 
                                 strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/PeopleTimePreference/Add_People_Time_Preference", model);
                                    
								 
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
                 
                 _logger.LogError(ex,"An exception occurred in - PeopleTimePreference / Add_People_Time_Preference, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
               
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

				
			  public virtual async Task<IActionResult> Update_People_Time_Preference(string PeopleTimePreferenceid)
			  {

                    string redirectTo="";
                    if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
                            DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
                            DataView dv = new DataView(NalamVazharole_JSON);
                            dv.RowFilter = "controllername='PeopleTimePreference' AND viewname='list'";

                            if(dv.Count  >0){
                                redirectTo = dv[0]["actionmethodname"] as string;
							 
                            }

                            try{
                                     var jsonObjPeopleTimePreference = await ApiClient.Get_ApiValues(getHttpClient(), "api/PeopleTimePreference/getById_PeopleTimePreference?PeopleTimePreferenceid="+PeopleTimePreferenceid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                                if(jsonObjPeopleTimePreference.Length > 2)
                                {
                                  
                                    var model = JsonConvert.DeserializeObject<PeopleTimePreferenceModel>(jsonObjPeopleTimePreference);


                
                                     
                                    return View("Add_People_Time_Preference", model);
                                }
                                else
                                {
                    
                                    TempData["message"] = "Data Not Found - Contact Administrator";
                                    return RedirectToAction(redirectTo);
						 
                                }

                            }catch(Exception ex){
                               _logger.LogError(ex,"An exception occurred in - PeopleTimePreference / Update_People_Time_Preference, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                                TempData["errMessage"] = "Error while fetching data - Contact Administrator";
                                return RedirectToAction(redirectTo);
                            }

                    }
                    TempData["errMessage"] = "Session Expired";
                    return RedirectToAction("Logout", "users");
                }	
			  [HttpPost()]
				public virtual async Task<string> Update_People_Time_Preference(PeopleTimePreferenceModel model, IFormCollection collection)
				{
					string strReturnMessage = "";
					try
					{
							ModelState.Remove("PeopleTimePreferenceid");
                            ModelState.Remove("craftmyapp_actionmethodname");
                             model.craftmyapp_actionmethodname="Update_People_Time_Preference";
							
							
							if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
					model.modifieduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
					else
					return "Session Expired";
							
                            
                            
							if (ModelState.IsValid)
							{
									PeopleTimePreferenceModelValidator validator = new PeopleTimePreferenceModelValidator();
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
                                        
										
                                        
                                        
                                        strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/PeopleTimePreference/Update_People_Time_Preference", model);
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
                      _logger.LogError(ex,"An exception occurred in - PeopleTimePreference / Update_People_Time_Preference, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
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
public virtual async Task<IActionResult> Remove_People_Time_Preference(string PeopleTimePreferenceid)
			{
				string message = "";
				try
				{
						message = await ApiClient.Get_ApiValues(getHttpClient(), "api/PeopleTimePreference/Remove_People_Time_Preference?PeopleTimePreferenceid="+PeopleTimePreferenceid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
						 if(message.Replace("\"","").Contains("201.1"))
						{
							TempData["message"] = "Success";

						}else{
							TempData["errMessage"] = message.Replace("\"","");
						}
						
				
				
				}
				catch (Exception ex)
				{
                     _logger.LogError(ex,"An exception occurred in - PeopleTimePreference / Remove_People_Time_Preference, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                
					 TempData["errMessage"] = ex.Message;
					 message = ex.Message;
				}

				string redirectTo="";
						if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
					DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
						 DataView dv = new DataView(NalamVazharole_JSON);
						 dv.RowFilter = "controllername='PeopleTimePreference' AND viewname='list'";

						if(dv.Count  >0){
						    redirectTo = dv[0]["actionmethodname"] as string;
							 
						}

					}
				
				return RedirectToAction(redirectTo);
			}

                                        public virtual IActionResult View_People_Time_Preference()
                                        {
                                            return View();
                                        }

			public virtual IActionResult People_Time_Preference_List()
			{
				return View();
			}
				
			[HttpGet()]
			public virtual async Task<string> get_People_Time_Preference_List(string tenantid
,string shiftname
,string people
)
			{
				
				return await ApiClient.Get_ApiValues(getHttpClient(), "api/PeopleTimePreference/People_Time_Preference_List?tenantid="+tenantid+"&shiftname="+shiftname+"&people="+people+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID")
);
			}
			  
											[HttpGet()]
											public virtual async Task<string> get_all_Shift(string tenantid,string searchterm, string pagesize="1000", string pagenumber="1")
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/Shift/get_all_Shift?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											 
											[HttpGet()]
											public virtual async Task<string> get_all_People(string tenantid,string searchterm, string pagesize="1000", string pagenumber="1")
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/People/get_all_People?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											

				
			  public virtual async Task<string> getById_allinfo_PeopleTimePreference(string PeopleTimePreferenceid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/PeopleTimePreference/getById_allinfo_PeopleTimePreference?PeopleTimePreferenceid="+PeopleTimePreferenceid);
					 
			  }
[HttpGet()]
                    public virtual async Task<string> lookup_PeopleTimePreference_shiftname(String tenantid,string searchterm, int? pagesize, int? pagenumber)
                    {
                        
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/PeopleTimePreference/lookup_PeopleTimePreference_shiftname?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }
[HttpGet()]
                    public virtual async Task<string> lookup_PeopleTimePreference_people(String tenantid, String workprofile, string searchterm, int? pagesize, int? pagenumber)
                    {
                        
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/PeopleTimePreference/lookup_PeopleTimePreference_people?tenantid="+tenantid+"&workprofile="+workprofile+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }

[HttpGet()]
                        public virtual async Task<string> lookup_PeopleTimePreference_timepreference_clinicaltask(String tenantid)
                        {
                            return await ApiClient.Get_ApiValues(getHttpClient(), "api/PeopleTimePreference/lookup_PeopleTimePreference_timepreference_clinicaltask?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                        }
[HttpGet()]
                        public virtual async Task<string> lookup_PeopleTimePreference_timepreference_tasktype(String tenantid)
                        {
                            return await ApiClient.Get_ApiValues(getHttpClient(), "api/PeopleTimePreference/lookup_PeopleTimePreference_timepreference_tasktype?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                        }
[HttpGet()]
                        public virtual async Task<string> lookup_PeopleTimePreference_timepreference_taskname(String tenantid,String tasktype)
                        {
                            return await ApiClient.Get_ApiValues(getHttpClient(), "api/PeopleTimePreference/lookup_PeopleTimePreference_timepreference_taskname?tenantid="+tenantid+"&tasktype="+tasktype+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                        }

 
											[HttpGet()]
											public virtual async Task<string> get_all_(string tenantid)
											{
											 
											    return await ApiClient.Get_ApiValues(getHttpClient(), "api//get_all_?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											
[HttpGet()]
			    public virtual async Task<string> lookup_PeopleTimePreference_taskname(String tenantid,String tasktype)
			    {
				    return await ApiClient.Get_ApiValues(getHttpClient(), "api/PeopleTimePreference/lookup_PeopleTimePreference_taskname?tenantid="+tenantid+"&tasktype="+tasktype+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
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

		[HttpGet()]
		public virtual async Task<string> get_all_WorkProfile(string tenantid)
		{

			return await ApiClient.Get_ApiValues(getHttpClient(), "api/WorkProfile/get_all_WorkProfile?tenantid=" + tenantid + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
		}
		[HttpGet()]
		public virtual async Task<string> lookup_PeopleTimePreference_workprofile(String tenantid)
		{

			return await ApiClient.Get_ApiValues(getHttpClient(), "api/PeopleTimePreference/lookup_PeopleTimePreference_workprofile?tenantid=" + tenantid + "&loginUserID=" + HttpContext.Session.GetString("NalamVazhaloginUserID"));
		}







	}


			}
