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

                
                
                
				//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:50
				
                
                
                
                
				public class TreatmentPackageController : BaseController
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
                    private readonly ILogger<TreatmentPackageController> _logger;
                    
                    
                    StorageUtil util;
					public TreatmentPackageController(IConfiguration configuration,IHttpContextAccessor accessor,IOptions<ApiSettings> ApiSettings, IOptions<MailSettings> MailSettings, IWebHostEnvironment env, ILogger<TreatmentPackageController> logger):base( configuration)
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
	              

					
				
			  public virtual async Task<string> getById_roomtypes(string TreatmentPackageid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/TreatmentPackage/getById_roomtypes?TreatmentPackageid="+TreatmentPackageid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
					 
			  }


				
			  public virtual async Task<string> getById_therapy(string TreatmentPackageid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/TreatmentPackage/getById_therapy?TreatmentPackageid="+TreatmentPackageid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
					 
			  }


				
			  public virtual async Task<string> getById_therapykits(string TreatmentPackageid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/TreatmentPackage/getById_therapykits?TreatmentPackageid="+TreatmentPackageid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
					 
			  }


				
			  public virtual async Task<string> getById_therapyitems(string TreatmentPackageid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/TreatmentPackage/getById_therapyitems?TreatmentPackageid="+TreatmentPackageid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
					 
			  }


				
			  public virtual async Task<string> getById_medicines(string TreatmentPackageid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/TreatmentPackage/getById_medicines?TreatmentPackageid="+TreatmentPackageid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
					 
			  }


				
			  public virtual async Task<string> getById_refundpolicy(string TreatmentPackageid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/TreatmentPackage/getById_refundpolicy?TreatmentPackageid="+TreatmentPackageid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
					 
			  }


			  public virtual IActionResult Add_Treatment_Package()
			  {
					return View();
			  }	
			  [HttpPost()]
			public virtual async Task<string> Add_Treatment_Package(TreatmentPackageModel model, IFormCollection collection)
			{
				string strReturnMessage = "";
				
				try
				{
					ModelState.Remove("TreatmentPackageid");
					ModelState.Remove("createduser");
                    ModelState.Remove("craftmyapp_actionmethodname");
                    model.craftmyapp_actionmethodname="Add_Treatment_Package";
					if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
								model.createduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
								else
								return "Session Expired";                    
					
                   
					
			 	    
					 if (ModelState.IsValid)
					 {
							 TreatmentPackageModelValidator validator = new TreatmentPackageModelValidator();
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
								 model.TreatmentPackageid =Guid.NewGuid(); 
                                 
                                  
								 
                                 strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/TreatmentPackage/Add_Treatment_Package", model);
                                    
								 
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
                 
                 _logger.LogError(ex,"An exception occurred in - TreatmentPackage / Add_Treatment_Package, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
               
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

				
			  public virtual async Task<IActionResult> Update_Treatment_Package(string TreatmentPackageid)
			  {

                    string redirectTo="";
                    if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
                            DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
                            DataView dv = new DataView(NalamVazharole_JSON);
                            dv.RowFilter = "controllername='TreatmentPackage' AND viewname='list'";

                            if(dv.Count  >0){
                                redirectTo = dv[0]["actionmethodname"] as string;
							 
                            }

                            try{
                                     var jsonObjTreatmentPackage = await ApiClient.Get_ApiValues(getHttpClient(), "api/TreatmentPackage/getById_TreatmentPackage?TreatmentPackageid="+TreatmentPackageid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                                if(jsonObjTreatmentPackage.Length > 2)
                                {
                                  
                                    var model = JsonConvert.DeserializeObject<TreatmentPackageModel>(jsonObjTreatmentPackage);


                
                                     
                                    return View("Add_Treatment_Package", model);
                                }
                                else
                                {
                    
                                    TempData["message"] = "Data Not Found - Contact Administrator";
                                    return RedirectToAction(redirectTo);
						 
                                }

                            }catch(Exception ex){
                               _logger.LogError(ex,"An exception occurred in - TreatmentPackage / Update_Treatment_Package, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                                TempData["errMessage"] = "Error while fetching data - Contact Administrator";
                                return RedirectToAction(redirectTo);
                            }

                    }
                    TempData["errMessage"] = "Session Expired";
                    return RedirectToAction("Logout", "users");
                }	
			  [HttpPost()]
				public virtual async Task<string> Update_Treatment_Package(TreatmentPackageModel model, IFormCollection collection)
				{
					string strReturnMessage = "";
					try
					{
							ModelState.Remove("TreatmentPackageid");
                            ModelState.Remove("craftmyapp_actionmethodname");
                             model.craftmyapp_actionmethodname="Update_Treatment_Package";
							
							
							if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
					model.modifieduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
					else
					return "Session Expired";
							
                            
                            
							if (ModelState.IsValid)
							{
									TreatmentPackageModelValidator validator = new TreatmentPackageModelValidator();
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
                                        
										
                                        
                                        
                                        strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/TreatmentPackage/Update_Treatment_Package", model);
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
                      _logger.LogError(ex,"An exception occurred in - TreatmentPackage / Update_Treatment_Package, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
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
public virtual async Task<IActionResult> Remove_Treatment_Package(string TreatmentPackageid)
			{
				string message = "";
				try
				{
						message = await ApiClient.Get_ApiValues(getHttpClient(), "api/TreatmentPackage/Remove_Treatment_Package?TreatmentPackageid="+TreatmentPackageid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
						 if(message.Replace("\"","").Contains("201.1"))
						{
							TempData["message"] = "Success";

						}else{
							TempData["errMessage"] = message.Replace("\"","");
						}
						
				
				
				}
				catch (Exception ex)
				{
                     _logger.LogError(ex,"An exception occurred in - TreatmentPackage / Remove_Treatment_Package, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                
					 TempData["errMessage"] = ex.Message;
					 message = ex.Message;
				}

				string redirectTo="";
						if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
					DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
						 DataView dv = new DataView(NalamVazharole_JSON);
						 dv.RowFilter = "controllername='TreatmentPackage' AND viewname='list'";

						if(dv.Count  >0){
						    redirectTo = dv[0]["actionmethodname"] as string;
							 
						}

					}
				
				return RedirectToAction(redirectTo);
			}

                                        public virtual IActionResult View_Treatment_Package()
                                        {
                                            return View();
                                        }

			public virtual IActionResult Treatment_Package_List()
			{
				return View();
			}
				
			[HttpGet()]
			public virtual async Task<string> get_Treatment_Package_List(string tenantid
)
			{
				
				return await ApiClient.Get_ApiValues(getHttpClient(), "api/TreatmentPackage/Treatment_Package_List?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID")
);
			}
			 

				
			  public virtual async Task<string> getById_allinfo_TreatmentPackage(string TreatmentPackageid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/TreatmentPackage/getById_allinfo_TreatmentPackage?TreatmentPackageid="+TreatmentPackageid);
					 
			  }

[HttpGet()]
                        public virtual async Task<string> lookup_TreatmentPackage_roomtypes_roomtype(String tenantid)
                        {
                            return await ApiClient.Get_ApiValues(getHttpClient(), "api/TreatmentPackage/lookup_TreatmentPackage_roomtypes_roomtype?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                        }
[HttpGet()]
                        public virtual async Task<string> lookup_TreatmentPackage_therapy_therapyname(String tenantid)
                        {
                            return await ApiClient.Get_ApiValues(getHttpClient(), "api/TreatmentPackage/lookup_TreatmentPackage_therapy_therapyname?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                        }
[HttpGet()]
                        public virtual async Task<string> lookup_TreatmentPackage_therapykits_therapykitname(String tenantid)
                        {
                            return await ApiClient.Get_ApiValues(getHttpClient(), "api/TreatmentPackage/lookup_TreatmentPackage_therapykits_therapykitname?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                        }
[HttpGet()]
                        public virtual async Task<string> lookup_TreatmentPackage_therapyitems_therapyitem(String tenantid)
                        {
                            return await ApiClient.Get_ApiValues(getHttpClient(), "api/TreatmentPackage/lookup_TreatmentPackage_therapyitems_therapyitem?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                        }
[HttpGet()]
                        public virtual async Task<string> lookup_TreatmentPackage_medicines_medicinename(String tenantid,string searchterm, int? pagesize, int? pagenumber)
                        {
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/TreatmentPackage/lookup_TreatmentPackage_medicines_medicinename?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                        }

 
                            [HttpGet()]
                            public virtual async Task<string> get_all_Medicine(string tenantid,string searchterm, string pagesize="1000", string pagenumber="1")
                            {

                                return await ApiClient.Get_ApiValues(getHttpClient(), "api/Medicine/get_all_Medicine?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                            }
                            
 
											[HttpGet()]
											public virtual async Task<string> get_all_TherapyItem(string tenantid)
											{
											 
											    return await ApiClient.Get_ApiValues(getHttpClient(), "api/TherapyItem/get_all_TherapyItem?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											
[HttpGet()]
			    public virtual async Task<string> lookup_TreatmentPackage_therapykitname(String tenantid,String therapyitem)
			    {
				    return await ApiClient.Get_ApiValues(getHttpClient(), "api/TreatmentPackage/lookup_TreatmentPackage_therapykitname?tenantid="+tenantid+"&therapyitem="+therapyitem+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			    } 
											[HttpGet()]
											public virtual async Task<string> get_all_TherapyKit(string tenantid)
											{
											 
											    return await ApiClient.Get_ApiValues(getHttpClient(), "api/TherapyKit/get_all_TherapyKit?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											
 
											[HttpGet()]
											public virtual async Task<string> get_all_Therapies(string tenantid)
											{
											 
											    return await ApiClient.Get_ApiValues(getHttpClient(), "api/Therapies/get_all_Therapies?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											
 
											[HttpGet()]
											public virtual async Task<string> get_all_RoomType(string tenantid)
											{
											 
											    return await ApiClient.Get_ApiValues(getHttpClient(), "api/RoomType/get_all_RoomType?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											


[HttpGet()]
			public virtual async Task<string> lookup_change_medicines_TreatmentPackage_medicinename(string Medicineid)
			{
				return await ApiClient.Get_ApiValues(getHttpClient(), "api/TreatmentPackage/lookup_change_medicines_TreatmentPackage_medicinename?Medicineid="+Medicineid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			}
[HttpGet()]
			public virtual async Task<string> lookup_change_therapyitems_TreatmentPackage_therapyitem(string TherapyItemid)
			{
				return await ApiClient.Get_ApiValues(getHttpClient(), "api/TreatmentPackage/lookup_change_therapyitems_TreatmentPackage_therapyitem?TherapyItemid="+TherapyItemid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			}
[HttpGet()]
			public virtual async Task<string> lookup_change_therapykits_TreatmentPackage_therapykitname(string TherapyKitid)
			{
				return await ApiClient.Get_ApiValues(getHttpClient(), "api/TreatmentPackage/lookup_change_therapykits_TreatmentPackage_therapykitname?TherapyKitid="+TherapyKitid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			}
[HttpGet()]
			public virtual async Task<string> lookup_change_therapy_TreatmentPackage_therapyname(string Therapiesid)
			{
				return await ApiClient.Get_ApiValues(getHttpClient(), "api/TreatmentPackage/lookup_change_therapy_TreatmentPackage_therapyname?Therapiesid="+Therapiesid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			}
[HttpGet()]
			public virtual async Task<string> lookup_change_roomtypes_TreatmentPackage_roomtype(string RoomTypeid)
			{
				return await ApiClient.Get_ApiValues(getHttpClient(), "api/TreatmentPackage/lookup_change_roomtypes_TreatmentPackage_roomtype?RoomTypeid="+RoomTypeid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			}


                    
                     
                        

				}


			}
