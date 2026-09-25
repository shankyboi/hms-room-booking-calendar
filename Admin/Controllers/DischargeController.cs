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

                
                
                
				//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/01/2026 11:43:42
				
                
                
                
                
				public class DischargeController : BaseController
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
                    private readonly ILogger<DischargeController> _logger;
                    
                    
                    StorageUtil util;
					public DischargeController(IConfiguration configuration,IHttpContextAccessor accessor,IOptions<ApiSettings> ApiSettings, IOptions<MailSettings> MailSettings, IWebHostEnvironment env, ILogger<DischargeController> logger):base( configuration)
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
	              

					
			  public virtual IActionResult Add_Discharge()
			  {
					return View();
			  }	
			  [HttpPost()]
			public virtual async Task<string> Add_Discharge(DischargeModel model, IFormCollection collection)
			{
				string strReturnMessage = "";
				
				try
				{
					ModelState.Remove("Dischargeid");
					ModelState.Remove("createduser");
                    ModelState.Remove("craftmyapp_actionmethodname");
                    model.craftmyapp_actionmethodname="Add_Discharge";
					if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
								model.createduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
								else
								return "Session Expired";                    
					
                   
					
			 	    
					 if (ModelState.IsValid)
					 {
							 DischargeModelValidator validator = new DischargeModelValidator();
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
								 model.Dischargeid =Guid.NewGuid(); 
                                 
                                  
								 
                                 strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/Discharge/Add_Discharge", model);
                                    
								 
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
                 
                 _logger.LogError(ex,"An exception occurred in - Discharge / Add_Discharge, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
               
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

				
			  public virtual async Task<IActionResult> Update_Discharge(string Dischargeid)
			  {

                    string redirectTo="";
                    if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
                            DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
                            DataView dv = new DataView(NalamVazharole_JSON);
                            dv.RowFilter = "controllername='Discharge' AND viewname='list'";

                            if(dv.Count  >0){
                                redirectTo = dv[0]["actionmethodname"] as string;
							 
                            }

                            try{
                                     var jsonObjDischarge = await ApiClient.Get_ApiValues(getHttpClient(), "api/Discharge/getById_Discharge?Dischargeid="+Dischargeid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                                if(jsonObjDischarge.Length > 2)
                                {
                                  
                                    var model = JsonConvert.DeserializeObject<DischargeModel>(jsonObjDischarge);


                
                                     
                                    return View("Add_Discharge", model);
                                }
                                else
                                {
                    
                                    TempData["message"] = "Data Not Found - Contact Administrator";
                                    return RedirectToAction(redirectTo);
						 
                                }

                            }catch(Exception ex){
                               _logger.LogError(ex,"An exception occurred in - Discharge / Update_Discharge, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                                TempData["errMessage"] = "Error while fetching data - Contact Administrator";
                                return RedirectToAction(redirectTo);
                            }

                    }
                    TempData["errMessage"] = "Session Expired";
                    return RedirectToAction("Logout", "users");
                }	
			  [HttpPost()]
				public virtual async Task<string> Update_Discharge(DischargeModel model, IFormCollection collection)
				{
					string strReturnMessage = "";
					try
					{
							ModelState.Remove("Dischargeid");
                            ModelState.Remove("craftmyapp_actionmethodname");
                             model.craftmyapp_actionmethodname="Update_Discharge";
							
							
							if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
					model.modifieduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
					else
					return "Session Expired";
							
                            
                            
							if (ModelState.IsValid)
							{
									DischargeModelValidator validator = new DischargeModelValidator();
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
                                        
										
                                        
                                        
                                        strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/Discharge/Update_Discharge", model);
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
                      _logger.LogError(ex,"An exception occurred in - Discharge / Update_Discharge, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
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
public virtual async Task<IActionResult> Remove_Discharge(string Dischargeid)
			{
				string message = "";
				try
				{
						message = await ApiClient.Get_ApiValues(getHttpClient(), "api/Discharge/Remove_Discharge?Dischargeid="+Dischargeid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
						 if(message.Replace("\"","").Contains("201.1"))
						{
							TempData["message"] = "Success";

						}else{
							TempData["errMessage"] = message.Replace("\"","");
						}
						
				
				
				}
				catch (Exception ex)
				{
                     _logger.LogError(ex,"An exception occurred in - Discharge / Remove_Discharge, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                
					 TempData["errMessage"] = ex.Message;
					 message = ex.Message;
				}

				string redirectTo="";
						if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
					DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
						 DataView dv = new DataView(NalamVazharole_JSON);
						 dv.RowFilter = "controllername='Discharge' AND viewname='list'";

						if(dv.Count  >0){
						    redirectTo = dv[0]["actionmethodname"] as string;
							 
						}

					}
				
				return RedirectToAction(redirectTo);
			}

                                        public virtual IActionResult Detail_Discharge()
                                        {
                                            return View();
                                        }

			        public virtual IActionResult Discharge_List()
			        {
				        return View();
			        }
			        	
			        [HttpGet()]
			        public virtual async Task<string> get_Discharge_List(string tenantid
,string ipdnumber
,string patient
,string room
, string discharge
, string paymentstatus
,string refundstatus
,string feedbackstatus
, int? pagesize=100 , int? pagenumber=0,string searchterm="",string sortFieldsJson="")
			        {
				        
				        return await ApiClient.Get_ApiValues(getHttpClient(), "api/Discharge/Discharge_List?tenantid="+tenantid+"&ipdnumber="+ipdnumber+"&patient="+patient+"&room="+room + "&discharge=" + Uri.EscapeDataString(discharge ?? "") + "&paymentstatus="+paymentstatus+"&refundstatus="+refundstatus+"&feedbackstatus="+feedbackstatus+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID")
+ "&pagesize=" + pagesize + "&pagenumber="+ pagenumber + "&searchterm="+ searchterm + "&sort_fields=" + sortFieldsJson);
			        }
			          
											[HttpGet()]
											public virtual async Task<string> get_all_IPDApplicationForm(string tenantid,string searchterm, string pagesize="1000", string pagenumber="1")
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/IPDApplicationForm/get_all_IPDApplicationForm?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											 
											[HttpGet()]
											public virtual async Task<string> get_all_PatientProfile(string tenantid,string searchterm, string pagesize="1000", string pagenumber="1")
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/PatientProfile/get_all_PatientProfile?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											 
											[HttpGet()]
											public virtual async Task<string> get_all_Room(string tenantid)
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/Room/get_all_Room?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											 
											[HttpGet()]
											public virtual async Task<string> get_all_RoomAllocation(string tenantid,string searchterm, string pagesize="1000", string pagenumber="1")
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/RoomAllocation/get_all_RoomAllocation?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											 
											[HttpGet()]
											public virtual async Task<string> get_all_BillingPayment(string tenantid,string searchterm, string pagesize="1000", string pagenumber="1")
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/BillingPayment/get_all_BillingPayment?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											

				
			  public virtual async Task<string> getById_allinfo_Discharge(string Dischargeid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/Discharge/getById_allinfo_Discharge?Dischargeid="+Dischargeid);
					 
			  }
[HttpGet()]
                    public virtual async Task<string> lookup_Discharge_ipdnumber(String tenantid,string searchterm, int? pagesize, int? pagenumber)
                    {
                        
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/Discharge/lookup_Discharge_ipdnumber?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }
[HttpGet()]
                    public virtual async Task<string> lookup_Discharge_patient(String tenantid,string searchterm, int? pagesize, int? pagenumber)
                    {
                        
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/Discharge/lookup_Discharge_patient?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }
[HttpGet()]
			    public virtual async Task<string> lookup_Discharge_room(String tenantid)
			    {
                    
				    return await ApiClient.Get_ApiValues(getHttpClient(), "api/Discharge/lookup_Discharge_room?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			    }
[HttpGet()]
                    public virtual async Task<string> lookup_Discharge_discharge(String tenantid,String ipdnumber,String room,string searchterm, int? pagesize, int? pagenumber)
                    {
                        
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/Discharge/lookup_Discharge_discharge?tenantid="+tenantid+"&ipdnumber="+ipdnumber+"&room="+room+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }
[HttpGet()]
                    public virtual async Task<string> lookup_Discharge_pendingamount(String tenantid,String ipdnumber,String patient,String room,string searchterm, int? pagesize, int? pagenumber)
                    {
                        
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/Discharge/lookup_Discharge_pendingamount?tenantid="+tenantid+"&ipdnumber="+ipdnumber+"&patient="+patient+"&room="+room+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }
[HttpGet()]
                    public virtual async Task<string> lookup_Discharge_paymentstatus(String tenantid,String ipdnumber,String patient,String room,string searchterm, int? pagesize, int? pagenumber)
                    {
                        
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/Discharge/lookup_Discharge_paymentstatus?tenantid="+tenantid+"&ipdnumber="+ipdnumber+"&patient="+patient+"&room="+room+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }
[HttpGet()]
                    public virtual async Task<string> lookup_Discharge_refundamount(String tenantid,String ipdnumber,String patient,String room,string searchterm, int? pagesize, int? pagenumber)
                    {
                        
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/Discharge/lookup_Discharge_refundamount?tenantid="+tenantid+"&ipdnumber="+ipdnumber+"&patient="+patient+"&room="+room+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }
[HttpGet()]
                    public virtual async Task<string> lookup_Discharge_refundstatus(String tenantid,String ipdnumber,String patient,String room,string searchterm, int? pagesize, int? pagenumber)
                    {
                        
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/Discharge/lookup_Discharge_refundstatus?tenantid="+tenantid+"&ipdnumber="+ipdnumber+"&patient="+patient+"&room="+room+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }






                    
                     
                        

				}


			}
