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

                
                
                
				//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 04/21/2026 05:37:59
				
                
                
                
                
				public class ReceivableController : BaseController
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
                    private readonly ILogger<ReceivableController> _logger;
                    
                    
                    StorageUtil util;

					private static void NormalizeReceivableAmountSign(ReceivableModel model)
					{
						if (model == null || string.IsNullOrWhiteSpace(model.receivablefor) || model.amount == 0)
							return;

						var receivableFor = model.receivablefor.Trim();
						if (receivableFor.Equals("Discount", StringComparison.OrdinalIgnoreCase)
							|| receivableFor.Equals("Cancellation Refund", StringComparison.OrdinalIgnoreCase)
							|| receivableFor.Equals("Concession", StringComparison.OrdinalIgnoreCase))
						{
							model.amount = -Math.Abs(model.amount);
						}
						else if (receivableFor.Equals("Penalty", StringComparison.OrdinalIgnoreCase))
						{
							model.amount = Math.Abs(model.amount);
						}
					}

					private static bool UsesPositiveAmountDisplay(string receivableFor)
					{
						if (string.IsNullOrWhiteSpace(receivableFor))
							return false;

						var value = receivableFor.Trim();
						return value.Equals("Discount", StringComparison.OrdinalIgnoreCase)
							|| value.Equals("Cancellation Refund", StringComparison.OrdinalIgnoreCase)
							|| value.Equals("Concession", StringComparison.OrdinalIgnoreCase)
							|| value.Equals("Penalty", StringComparison.OrdinalIgnoreCase);
					}

					public ReceivableController(IConfiguration configuration,IHttpContextAccessor accessor,IOptions<ApiSettings> ApiSettings, IOptions<MailSettings> MailSettings, IWebHostEnvironment env, ILogger<ReceivableController> logger):base( configuration)
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
	              

					
			  public virtual async Task<IActionResult> Add_Receivable(Guid? patientname = null, string ipdnumber = null, string IPDApplicationFormid = null, string opdnumber = null)
			  {
					var resolvedIpdNumber = !string.IsNullOrWhiteSpace(ipdnumber) ? ipdnumber : IPDApplicationFormid;
					var resolvedPatientName = patientname?.ToString() ?? "";
					ViewBag.PrefillPatientName = resolvedPatientName;
					ViewBag.PrefillIPDNumber = resolvedIpdNumber ?? "";
					ViewBag.PrefillOPDNumber = opdnumber ?? "";
					ViewBag.PrefillOPDReference = opdnumber ?? "";
					ViewBag.PrefillPatientDisplayName = "";
					ViewBag.PrefillTenantId = HttpContext.Session.GetString("NalamVazhachoosedtenantid") ?? "";

					if (string.IsNullOrWhiteSpace(resolvedPatientName) && Guid.TryParse(resolvedIpdNumber, out var ipdApplicationFormId))
					{
						try
						{
							var ipdJson = await ApiClient.Get_ApiValues(getHttpClient(), "api/IPDApplicationForm/getById_IPDApplicationForm?IPDApplicationFormid="+ipdApplicationFormId+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
							if (!string.IsNullOrWhiteSpace(ipdJson) && ipdJson.Length > 2)
							{
								var ipdModel = JsonConvert.DeserializeObject<IPDApplicationFormModel>(ipdJson);
								if (ipdModel != null && ipdModel.patientname != Guid.Empty)
								{
									resolvedPatientName = ipdModel.patientname.ToString();
									ViewBag.PrefillPatientName = resolvedPatientName;
								}
							}
						}
						catch (Exception ex)
						{
							_logger.LogError(ex, "An exception occurred while resolving IPD patient prefill for Receivable / Add_Receivable");
						}
					}

					if (string.IsNullOrWhiteSpace(resolvedPatientName) && Guid.TryParse(opdnumber, out var opdFormId))
					{
						try
						{
							var opdJson = await ApiClient.Get_ApiValues(getHttpClient(), "api/OPDForm/getById_OPDForm?OPDFormid="+opdFormId+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
							if (!string.IsNullOrWhiteSpace(opdJson) && opdJson.Length > 2)
							{
								var opdModel = JsonConvert.DeserializeObject<OPDFormModel>(opdJson);
								if (opdModel != null && opdModel.patientname != Guid.Empty)
								{
									resolvedPatientName = opdModel.patientname.ToString();
									ViewBag.PrefillPatientName = resolvedPatientName;
								}
								if (opdModel != null && !string.IsNullOrWhiteSpace(opdModel.bookingreferencenumber))
								{
									ViewBag.PrefillOPDReference = opdModel.bookingreferencenumber;
								}
							}
						}
						catch (Exception ex)
						{
							_logger.LogError(ex, "An exception occurred while resolving OPD patient prefill for Receivable / Add_Receivable");
						}
					}

					if (!string.IsNullOrWhiteSpace(opdnumber) && Guid.TryParse(resolvedPatientName, out var patientProfileId))
					{
						try
						{
							var patientJson = await ApiClient.Get_ApiValues(getHttpClient(), "api/PatientProfile/getById_PatientProfile?PatientProfileid="+patientProfileId+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
							if (!string.IsNullOrWhiteSpace(patientJson) && patientJson.Length > 2)
							{
								var patientModel = JsonConvert.DeserializeObject<PatientProfileModel>(patientJson);
								if (patientModel != null)
								{
									ViewBag.PrefillPatientDisplayName = string.Join(" ", new[] { patientModel.firstname, patientModel.lastname }.Where(value => !string.IsNullOrWhiteSpace(value)));
								}
							}
						}
						catch (Exception ex)
						{
							_logger.LogError(ex, "An exception occurred while resolving OPD patient header for Receivable / Add_Receivable");
						}
					}

					return View();
			  }	
			  [HttpPost()]
			public virtual async Task<string> Add_Receivable(ReceivableModel model, IFormCollection collection)
			{
				string strReturnMessage = "";
				
				try
				{
					ModelState.Remove("Receivableid");
                    ModelState.Remove("createduser");
                    ModelState.Remove("craftmyapp_actionmethodname");
                    model.craftmyapp_actionmethodname="Add_Receivable";
					NormalizeReceivableAmountSign(model);
					if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
								model.createduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
								else
								return "Session Expired";                    

					if (model.patientname == Guid.Empty && model.opdnumber.HasValue)
					{
						var opdJson = await ApiClient.Get_ApiValues(getHttpClient(), "api/OPDForm/getById_OPDForm?OPDFormid="+model.opdnumber.Value+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
						if (!string.IsNullOrWhiteSpace(opdJson) && opdJson.Length > 2)
						{
							var opdModel = JsonConvert.DeserializeObject<OPDFormModel>(opdJson);
							if (opdModel != null && opdModel.patientname != Guid.Empty)
							{
								model.patientname = opdModel.patientname;
								ModelState.Remove("patientname");
							}
						}
					}
					
                   
					
			 	    
					 if (ModelState.IsValid)
					 {
							 ReceivableModelValidator validator = new ReceivableModelValidator();
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
								 model.Receivableid =Guid.NewGuid(); 
								 model.paidamount = 0m;
								 model.paymentstatus = "Pending";
								 model.billingpaymentid = null;
                                 
                                  
								 
                                 strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/Receivable/Add_Receivable", model);
                                    
								 
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
                 
                 _logger.LogError(ex,"An exception occurred in - Receivable / Add_Receivable, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
               
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

				
			  public virtual async Task<IActionResult> Update_Receivable(string Receivableid)
			  {

                    string redirectTo="";
                    if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
                            DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
                            DataView dv = new DataView(NalamVazharole_JSON);
                            dv.RowFilter = "controllername='Receivable' AND viewname='list'";

                            if(dv.Count  >0){
                                redirectTo = dv[0]["actionmethodname"] as string;
							 
                            }

                            try{
                                     var jsonObjReceivable = await ApiClient.Get_ApiValues(getHttpClient(), "api/Receivable/getById_Receivable?Receivableid="+Receivableid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                                if(jsonObjReceivable.Length > 2)
                                {
                                  
									var model = JsonConvert.DeserializeObject<ReceivableModel>(jsonObjReceivable);
									if (model != null && UsesPositiveAmountDisplay(model.receivablefor))
										model.amount = Math.Abs(model.amount);


                
                                     
                                    return View("Add_Receivable", model);
                                }
                                else
                                {
                    
                                    TempData["message"] = "Data Not Found - Contact Administrator";
                                    return RedirectToAction(redirectTo);
						 
                                }

                            }catch(Exception ex){
                               _logger.LogError(ex,"An exception occurred in - Receivable / Update_Receivable, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                                TempData["errMessage"] = "Error while fetching data - Contact Administrator";
                                return RedirectToAction(redirectTo);
                            }

                    }
                    TempData["errMessage"] = "Session Expired";
                    return RedirectToAction("Logout", "users");
                }	
			  [HttpPost()]
				public virtual async Task<string> Update_Receivable(ReceivableModel model, IFormCollection collection)
				{
					string strReturnMessage = "";
					try
					{
							ModelState.Remove("Receivableid");
                            ModelState.Remove("craftmyapp_actionmethodname");
                             model.craftmyapp_actionmethodname="Update_Receivable";
							NormalizeReceivableAmountSign(model);
							
							
							if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
					model.modifieduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
					else
					return "Session Expired";
							
                            
                            
							if (ModelState.IsValid)
							{
									ReceivableModelValidator validator = new ReceivableModelValidator();
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
                                        
										
                                        
                                        
                                        strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/Receivable/Update_Receivable", model);
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
                      _logger.LogError(ex,"An exception occurred in - Receivable / Update_Receivable, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
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
public virtual async Task<IActionResult> Remove_Receivable(string Receivableid)
			{
				string message = "";
				try
				{
						message = await ApiClient.Get_ApiValues(getHttpClient(), "api/Receivable/Remove_Receivable?Receivableid="+Receivableid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
						 if(message.Replace("\"","").Contains("201.1"))
						{
							TempData["message"] = "Success";

						}else{
							TempData["errMessage"] = message.Replace("\"","");
						}
						
				
				
				}
				catch (Exception ex)
				{
                     _logger.LogError(ex,"An exception occurred in - Receivable / Remove_Receivable, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                
					 TempData["errMessage"] = ex.Message;
					 message = ex.Message;
				}

				string redirectTo="";
						if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
					DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
						 DataView dv = new DataView(NalamVazharole_JSON);
						 dv.RowFilter = "controllername='Receivable' AND viewname='list'";

						if(dv.Count  >0){
						    redirectTo = dv[0]["actionmethodname"] as string;
							 
						}

					}
				
				return RedirectToAction(redirectTo);
			}

                                        public virtual IActionResult View_Receivable()
                                        {
                                            return View();
                                        }

			        public virtual IActionResult Receivables()
			        {
				        return View();
			        }
			        	
			        [HttpGet()]
			        public virtual async Task<string> get_Receivables(string tenantid
,string receivabledate_automatonfrom
,string receivabledate_automatonto
,string patientname
,string ipdnumber
,string opdnumber
,string receivablefor
, int? pagesize=100 , int? pagenumber=0,string searchterm="",string sortFieldsJson="")
			        {
				        
				        return await ApiClient.Get_ApiValues(getHttpClient(), "api/Receivable/Receivables?tenantid="+tenantid+"&receivabledate_automatonfrom="+receivabledate_automatonfrom+"&receivabledate_automatonto="+receivabledate_automatonto+"&patientname="+patientname+"&ipdnumber="+ipdnumber+"&opdnumber="+opdnumber+"&receivablefor="+receivablefor+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID")
+ "&pagesize=" + pagesize + "&pagenumber="+ pagenumber + "&searchterm="+ searchterm + "&sort_fields=" + sortFieldsJson);
			        }
			          
											[HttpGet()]
											public virtual async Task<string> get_all_PatientProfile(string tenantid,string searchterm, string pagesize="1000", string pagenumber="1")
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/PatientProfile/get_all_PatientProfile?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											 
											[HttpGet()]
											public virtual async Task<string> get_all_PatientVisit(string tenantid,string searchterm, string pagesize="1000", string pagenumber="1")
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/PatientVisit/get_all_PatientVisit?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
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
											 
											[HttpGet()]
											public virtual async Task<string> get_all_Therapies(string tenantid)
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/Therapies/get_all_Therapies?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											 
											[HttpGet()]
											public virtual async Task<string> get_all_TherapyKit(string tenantid)
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/TherapyKit/get_all_TherapyKit?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											 
											[HttpGet()]
											public virtual async Task<string> get_all_Medicine(string tenantid,string searchterm, string pagesize="1000", string pagenumber="1")
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/Medicine/get_all_Medicine?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											 
											[HttpGet()]
											public virtual async Task<string> get_all_TreatmentPackage(string tenantid)
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/TreatmentPackage/get_all_TreatmentPackage?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											 
											[HttpGet()]
											public virtual async Task<string> get_all_Room(string tenantid)
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/Room/get_all_Room?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											

				
			  public virtual async Task<string> getById_allinfo_Receivable(string Receivableid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/Receivable/getById_allinfo_Receivable?Receivableid="+Receivableid);
					 
			  }
[HttpGet()]
                    public virtual async Task<string> lookup_Receivable_patientname(String tenantid,string searchterm, int? pagesize, int? pagenumber)
                    {
                        
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/Receivable/lookup_Receivable_patientname?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }
[HttpGet()]
                    public virtual async Task<string> lookup_Receivable_patientvisit(String tenantid,String patientname,string searchterm, int? pagesize, int? pagenumber)
                    {
                        
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/Receivable/lookup_Receivable_patientvisit?tenantid="+tenantid+"&patientname="+patientname+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }
[HttpGet()]
                    public virtual async Task<string> lookup_Receivable_ipdnumber(String tenantid,String patientname,string searchterm, int? pagesize, int? pagenumber)
                    {
                        
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/Receivable/lookup_Receivable_ipdnumber?tenantid="+tenantid+"&patientname="+patientname+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }
[HttpGet()]
                    public virtual async Task<string> get_Approved_Concession_Amount(String tenantid,String patientname,String ipdnumber)
                    {
                        
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/Receivable/get_Approved_Concession_Amount?tenantid="+tenantid+"&patientname="+patientname+"&ipdnumber="+ipdnumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }
[HttpGet()]
                    public virtual async Task<string> lookup_Receivable_opdnumber(String tenantid,String patientname,string searchterm, int? pagesize, int? pagenumber)
                    {
                        
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/Receivable/lookup_Receivable_opdnumber?tenantid="+tenantid+"&patientname="+patientname+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }
[HttpGet()]
			    public virtual async Task<string> lookup_Receivable_therapy(String tenantid)
			    {
                    
				    return await ApiClient.Get_ApiValues(getHttpClient(), "api/Receivable/lookup_Receivable_therapy?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			    }
[HttpGet()]
			    public virtual async Task<string> lookup_Receivable_therapykit(String tenantid)
			    {
                    
				    return await ApiClient.Get_ApiValues(getHttpClient(), "api/Receivable/lookup_Receivable_therapykit?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			    }
[HttpGet()]
                    public virtual async Task<string> lookup_Receivable_medicine(String tenantid,string searchterm, int? pagesize, int? pagenumber)
                    {
                        
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/Receivable/lookup_Receivable_medicine?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }
[HttpGet()]
			    public virtual async Task<string> lookup_Receivable_package(String tenantid)
			    {
                    
				    return await ApiClient.Get_ApiValues(getHttpClient(), "api/Receivable/lookup_Receivable_package?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			    }
[HttpGet()]
			    public virtual async Task<string> lookup_Receivable_room(String tenantid)
			    {
                    
				    return await ApiClient.Get_ApiValues(getHttpClient(), "api/Receivable/lookup_Receivable_room?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			    }



[HttpGet()]
			public virtual async Task<string> lookup_change_Receivable_medicine(string Medicineid)
			{
				return await ApiClient.Get_ApiValues(getHttpClient(), "api/Receivable/lookup_change_Receivable_medicine?Medicineid="+Medicineid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			}
[HttpGet()]
			public virtual async Task<string> lookup_change_Receivable_therapykit(string TherapyKitid)
			{
				return await ApiClient.Get_ApiValues(getHttpClient(), "api/Receivable/lookup_change_Receivable_therapykit?TherapyKitid="+TherapyKitid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			}
[HttpGet()]
			public virtual async Task<string> lookup_change_Receivable_therapy(string Therapiesid)
			{
				return await ApiClient.Get_ApiValues(getHttpClient(), "api/Receivable/lookup_change_Receivable_therapy?Therapiesid="+Therapiesid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			}



                    
                     
                        

				}


			}
