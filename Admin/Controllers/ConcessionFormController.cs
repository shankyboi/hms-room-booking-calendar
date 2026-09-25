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

                
                
                
				//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 05/26/2026 07:14:50
				
                
                
                
                
				public class ConcessionFormController : BaseController
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
                    private readonly ILogger<ConcessionFormController> _logger;
                    
                    
                    StorageUtil util;
					public ConcessionFormController(IConfiguration configuration,IHttpContextAccessor accessor,IOptions<ApiSettings> ApiSettings, IOptions<MailSettings> MailSettings, IWebHostEnvironment env, ILogger<ConcessionFormController> logger):base( configuration)
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
	              

					
				
			  public virtual async Task<string> getById_earningmembers(string ConcessionFormid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/ConcessionForm/getById_earningmembers?ConcessionFormid="+ConcessionFormid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
					 
			  }

			  public virtual async Task<string> get_Existing_Concession_Form(String tenantid,String patientname,String ipdnumber)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/ConcessionForm/get_Existing_Concession_Form?tenantid="+tenantid+"&patientname="+patientname+"&ipdnumber="+ipdnumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			  }

			  private static bool CsvContains(string value, params string[] expectedValues)
			  {
					if (string.IsNullOrWhiteSpace(value)) return false;
					var values = value.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(v => v.Trim());
					return values.Any(v => expectedValues.Any(expected => string.Equals(v, expected, StringComparison.OrdinalIgnoreCase)));
			  }

			  private static void NormalizeConcessionForm(ConcessionFormModel model)
			  {
					if (model.requestedconcessionamount.HasValue && model.requestedconcessionamount.Value > 0)
						model.requestedconcessionamount = -model.requestedconcessionamount.Value;

					if (!string.Equals(model.haveyoutraveledinternationallyinthelast5years, "Yes", StringComparison.OrdinalIgnoreCase))
					{
						model.purposeoftravel = null;
						model.otherpleasespecify = null;
					}
					else if (!CsvContains(model.purposeoftravel, "Others"))
					{
						model.otherpleasespecify = null;
					}

					if (!CsvContains(model.doanyofthefollowingapplytoyou, "Others", "Other"))
						model.specifyotherindustry = null;
			  }

			  private async Task<bool> ExistingConcessionFormExists(Guid? tenantid, Guid patientname, Guid bookingreferencenumber, Guid? currentConcessionFormid = null)
			  {
					var tenant = tenantid?.ToString() ?? HttpContext.Session.GetString("NalamVazhachoosedtenantid") ?? "";
					if (string.IsNullOrWhiteSpace(tenant)) return false;
					var existingConcessionJson = await ApiClient.Get_ApiValues(getHttpClient(), "api/ConcessionForm/get_Existing_Concession_Form?tenantid="+tenant+"&patientname="+patientname+"&ipdnumber="+bookingreferencenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
					if (string.IsNullOrWhiteSpace(existingConcessionJson) || existingConcessionJson.Length <= 2) return false;

					var existingRows = Newtonsoft.Json.Linq.JArray.Parse(existingConcessionJson);
					var existingRow = existingRows.FirstOrDefault();
					var concessionFormId = existingRow?["concessionformid"]?.ToString() ?? existingRow?["ConcessionFormid"]?.ToString();
					if (string.IsNullOrWhiteSpace(concessionFormId)) return false;
					if (currentConcessionFormid.HasValue && string.Equals(concessionFormId, currentConcessionFormid.Value.ToString(), StringComparison.OrdinalIgnoreCase)) return false;
					return true;
			  }

			  public virtual async Task<IActionResult> Add_Concession_Form(Guid? patientname = null, string ipdnumber = null)
			  {
					ViewBag.PrefillPatientName = patientname?.ToString() ?? "";
					ViewBag.PrefillIPDNumber = ipdnumber ?? "";
					ViewBag.PrefillIPDBookingReference = "";
					if (Guid.TryParse(ipdnumber, out var ipdApplicationFormId))
					{
						var ipdJson = await ApiClient.Get_ApiValues(getHttpClient(), "api/IPDApplicationForm/getById_IPDApplicationForm?IPDApplicationFormid="+ipdApplicationFormId+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
						if (!string.IsNullOrWhiteSpace(ipdJson) && ipdJson.Length > 2)
						{
							var ipdModel = JsonConvert.DeserializeObject<IPDApplicationFormModel>(ipdJson);
							if (ipdModel != null)
							{
								ViewBag.PrefillPatientName = ipdModel.patientname.ToString();
								ViewBag.PrefillIPDBookingReference = ipdModel.bookingreferencenumber ?? "";
							}
						}
					}

					var resolvedPatientName = ViewBag.PrefillPatientName as string;
					if (!string.IsNullOrWhiteSpace(resolvedPatientName) && !string.IsNullOrWhiteSpace(ipdnumber))
					{
						var existingConcessionJson = await ApiClient.Get_ApiValues(getHttpClient(), "api/ConcessionForm/get_Existing_Concession_Form?tenantid="+HttpContext.Session.GetString("NalamVazhachoosedtenantid")+"&patientname="+resolvedPatientName+"&ipdnumber="+ipdnumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
						if (!string.IsNullOrWhiteSpace(existingConcessionJson) && existingConcessionJson.Length > 2)
						{
							var existingRows = Newtonsoft.Json.Linq.JArray.Parse(existingConcessionJson);
							var existingRow = existingRows.FirstOrDefault();
							var concessionFormId = existingRow?["concessionformid"]?.ToString() ?? existingRow?["ConcessionFormid"]?.ToString();
							var verifiedStatus = existingRow?["verifiedstatus"]?.ToString();
							if (!string.IsNullOrWhiteSpace(concessionFormId))
							{
								if (string.Equals(verifiedStatus, "Approved", StringComparison.OrdinalIgnoreCase))
								{
									return RedirectToAction("ApprovedDetail_Concession_Form", new { ConcessionFormid = concessionFormId });
								}
								return RedirectToAction("Update_Concession_Form", new { ConcessionFormid = concessionFormId });
							}
						}
					}
					return View();
			  }	

			  public virtual async Task<IActionResult> Add_ConcessionForm(Guid? patientname = null, string ipdnumber = null)
			  {
					return await Add_Concession_Form(patientname, ipdnumber);
			  }

			  [HttpPost()]
			public virtual async Task<string> Add_Concession_Form(ConcessionFormModel model, IFormCollection collection)
			{
				string strReturnMessage = "";
				
				try
				{
					ModelState.Remove("ConcessionFormid");
					ModelState.Remove("createduser");
                    ModelState.Remove("craftmyapp_actionmethodname");
                    model.craftmyapp_actionmethodname="Add_Concession_Form";
					if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
								model.createduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
								else
								return "Session Expired";                    
					
                   
					model.whichofthesedoyouown = collection["whichofthesedoyouown"];
model.purposeoftravel = collection["purposeoftravel"];
model.doanyofthefollowingapplytoyou = collection["doanyofthefollowingapplytoyou"];
					if (model.bookingreferencenumber == Guid.Empty &&
						Guid.TryParse(collection["prefillipdnumber"].ToString(), out var prefilledIpdApplicationFormId))
					{
						model.bookingreferencenumber = prefilledIpdApplicationFormId;
						ModelState.Remove(nameof(model.bookingreferencenumber));
					}
NormalizeConcessionForm(model);

					if (await ExistingConcessionFormExists(model.tenantid, model.patientname, model.bookingreferencenumber))
						return "A concession form already exists for this IPD. Please update the existing concession form.";

			 	    
					 if (ModelState.IsValid)
					 {
							 ConcessionFormModelValidator validator = new ConcessionFormModelValidator();
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
								 model.ConcessionFormid =Guid.NewGuid(); 
                                 
                                  
								 
                                 strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/ConcessionForm/Add_Concession_Form", model);
                                    
								 
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
                 
                 _logger.LogError(ex,"An exception occurred in - ConcessionForm / Add_Concession_Form, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
               
				 strReturnMessage = ex.Message;
			 }
		     ViewData["message"] = strReturnMessage;
			 if(strReturnMessage.Replace("\"", "").Contains("201.1")){
				 TempData["message"] = "Success";
				 MailSender maillog = new MailSender();
                    bool mailsent = await maillog.sendNotification("ConcessionForm"
                    , "ReadyForReview"
                    , model.ConcessionFormid.ToString()
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

				
			  public virtual async Task<IActionResult> Update_Concession_Form(string ConcessionFormid)
			  {

                    string redirectTo="";
                    if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
                            DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
                            DataView dv = new DataView(NalamVazharole_JSON);
                            dv.RowFilter = "controllername='ConcessionForm' AND viewname='list'";

                            if(dv.Count  >0){
                                redirectTo = dv[0]["actionmethodname"] as string;
							 
                            }

                            try{
                                     var jsonObjConcessionForm = await ApiClient.Get_ApiValues(getHttpClient(), "api/ConcessionForm/getById_ConcessionForm?ConcessionFormid="+ConcessionFormid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                                if(jsonObjConcessionForm.Length > 2)
                                {
                                  
									var model = JsonConvert.DeserializeObject<ConcessionFormModel>(jsonObjConcessionForm);
									if (model?.requestedconcessionamount != null)
										model.requestedconcessionamount = Math.Abs(model.requestedconcessionamount.Value);


                
                                     
                                    return View("Add_Concession_Form", model);
                                }
                                else
                                {
                    
                                    TempData["message"] = "Data Not Found - Contact Administrator";
                                    return RedirectToAction(redirectTo);
						 
                                }

                            }catch(Exception ex){
                               _logger.LogError(ex,"An exception occurred in - ConcessionForm / Update_Concession_Form, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                                TempData["errMessage"] = "Error while fetching data - Contact Administrator";
                                return RedirectToAction(redirectTo);
                            }

                    }
                    TempData["errMessage"] = "Session Expired";
                    return RedirectToAction("Logout", "users");
                }	
			  [HttpPost()]
				public virtual async Task<string> Update_Concession_Form(ConcessionFormModel model, IFormCollection collection)
				{
					string strReturnMessage = "";
					try
					{
							ModelState.Remove("ConcessionFormid");
                            ModelState.Remove("craftmyapp_actionmethodname");
                             model.craftmyapp_actionmethodname="Update_Concession_Form";
							
							
							if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
					model.modifieduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
					else
					return "Session Expired";
							model.whichofthesedoyouown = collection["whichofthesedoyouown"];
model.purposeoftravel = collection["purposeoftravel"];
model.doanyofthefollowingapplytoyou = collection["doanyofthefollowingapplytoyou"];
NormalizeConcessionForm(model);

                            
                            
							if (ModelState.IsValid)
							{
									ConcessionFormModelValidator validator = new ConcessionFormModelValidator();
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
                                        
										
                                        
                                        
                                        strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/ConcessionForm/Update_Concession_Form", model);
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
                      _logger.LogError(ex,"An exception occurred in - ConcessionForm / Update_Concession_Form, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
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
public virtual async Task<IActionResult> Remove_Concession_Form(string ConcessionFormid)
			{
				string message = "";
				try
				{
						message = await ApiClient.Get_ApiValues(getHttpClient(), "api/ConcessionForm/Remove_Concession_Form?ConcessionFormid="+ConcessionFormid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
						 if(message.Replace("\"","").Contains("201.1"))
						{
							TempData["message"] = "Success";

						}else{
							TempData["errMessage"] = message.Replace("\"","");
						}
						
				
				
				}
				catch (Exception ex)
				{
                     _logger.LogError(ex,"An exception occurred in - ConcessionForm / Remove_Concession_Form, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                
					 TempData["errMessage"] = ex.Message;
					 message = ex.Message;
				}

				string redirectTo="";
						if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
					DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
						 DataView dv = new DataView(NalamVazharole_JSON);
						 dv.RowFilter = "controllername='ConcessionForm' AND viewname='list'";

						if(dv.Count  >0){
						    redirectTo = dv[0]["actionmethodname"] as string;
							 
						}

					}
				
				return RedirectToAction(redirectTo);
			}

			public virtual IActionResult Added_Concession_Form()
			{
				return View();
			}
				
			[HttpGet()]
			public virtual async Task<string> get_Added_Concession_Form(string tenantid
,string verifiedstatus
)
			{
				
				return await ApiClient.Get_ApiValues(getHttpClient(), "api/ConcessionForm/Added_Concession_Form?tenantid="+tenantid+"&verifiedstatus="+verifiedstatus+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID")
);
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
											

                                        public virtual IActionResult MakerDetail_Concession_Form()
                                        {
                                            return View();
                                        }

			public virtual IActionResult Concession_Form_for_Review()
			{
				return View();
			}
				
			[HttpGet()]
			public virtual async Task<string> get_Concession_Form_for_Review(string tenantid
,string verifiedstatus
)
			{
				
				return await ApiClient.Get_ApiValues(getHttpClient(), "api/ConcessionForm/Concession_Form_for_Review?tenantid="+tenantid+"&verifiedstatus="+verifiedstatus+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID")
);
			}
			 

			 
			[HttpGet()]
			public virtual async Task<string> count_of_ConcessionForm(string tenantid
)
			{
				
				return await ApiClient.Get_ApiValues(getHttpClient(), "api/ConcessionForm/count_of_ConcessionForm?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID")
);
			}
			 

            [HttpPost()]
            public virtual async Task<string> verify_ConcessionForm([FromBody]ConcessionFormReviewModel model)
			{
				string message = "";
				try
				{
					 	message = await ApiClient.Post_ApiValuesGetString(getHttpClient(), "api/ConcessionForm/verify_ConcessionForm", model);
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
                    
                      _logger.LogError(ex,"An exception occurred in - ConcessionForm / verify_ConcessionForm, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                  
					 TempData["errMessage"] = ex.Message;
					 message = ex.Message;
				}
 
				
				return message;
			}

                                        public virtual IActionResult CheckerDetail_Concession_Form()
                                        {
                                            return View();
                                        }

			public virtual IActionResult Concession_Form_List()
			{
				return View();
			}
				
			[HttpGet()]
			public virtual async Task<string> get_Concession_Form_List(string tenantid
)
			{
				
				return await ApiClient.Get_ApiValues(getHttpClient(), "api/ConcessionForm/Concession_Form_List?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID")
);
			}
			 

                                        public virtual IActionResult ApprovedDetail_Concession_Form()
                                        {
                                            return View();
                                        }

				
			  public virtual async Task<string> getById_allinfo_ConcessionForm(string ConcessionFormid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/ConcessionForm/getById_allinfo_ConcessionForm?ConcessionFormid="+ConcessionFormid);
					 
			  }
[HttpGet()]
                    public virtual async Task<string> lookup_ConcessionForm_patientname(String tenantid,string searchterm, int? pagesize, int? pagenumber)
                    {
                        
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/ConcessionForm/lookup_ConcessionForm_patientname?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }
[HttpGet()]
                    public virtual async Task<string> lookup_ConcessionForm_bookingreferencenumber(String tenantid,String patientname,string searchterm, int? pagesize, int? pagenumber)
                    {
                        
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/ConcessionForm/lookup_ConcessionForm_bookingreferencenumber?tenantid="+tenantid+"&patientname="+patientname+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }






                    
                     
                        

				}


			}
