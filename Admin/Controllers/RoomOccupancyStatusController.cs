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

                
                
                
				//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:05
				
                
                
                
                
				public class RoomOccupancyStatusController : BaseController
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
                    private readonly ILogger<RoomOccupancyStatusController> _logger;
                    
                    
                    StorageUtil util;
					public RoomOccupancyStatusController(IConfiguration configuration,IHttpContextAccessor accessor,IOptions<ApiSettings> ApiSettings, IOptions<MailSettings> MailSettings, IWebHostEnvironment env, ILogger<RoomOccupancyStatusController> logger):base( configuration)
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

        public IActionResult Booking_Calendar(string isDashboardView = "Y")
        {
            ViewBag.Title = "Booking Calendar";
            ViewBag.IsDashboardView = isDashboardView;

            return View();
        }

        [HttpPost]
        public virtual async Task<IActionResult> SetManualRoomAvailability([FromBody] ManualRoomAvailabilityModel model)
        {
            var userId = HttpContext.Session.GetString("NalamVazhaloginUserID");
            var tenantId = HttpContext.Session.GetString("NalamVazhachoosedtenantid");
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized("Session Expired");
            if (!Guid.TryParse(tenantId, out var selectedTenantId))
                return BadRequest("Select a tenant before updating room availability.");

            model.tenantid = selectedTenantId;
            var response = await ApiClient.Post_ApiValuesGetString(
                getHttpClient(),
                "api/RoomAllocation/SetManualRoomAvailability",
                model);

            var normalized = (response ?? "").Replace("\"", "").Trim();
            return normalized == "201.1"
                ? Ok(new { success = true })
                : BadRequest(normalized);
        }


        public virtual IActionResult Add_Room_Occupancy_Status()
			  {
					return View();
			  }	
			  [HttpPost()]
			public virtual async Task<string> Add_Room_Occupancy_Status(RoomOccupancyStatusModel model, IFormCollection collection)
			{
				string strReturnMessage = "";
				
				try
				{
					ModelState.Remove("RoomOccupancyStatusid");
					ModelState.Remove("createduser");
                    ModelState.Remove("craftmyapp_actionmethodname");
                    model.craftmyapp_actionmethodname="Add_Room_Occupancy_Status";
					if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
								model.createduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
								else
								return "Session Expired";                    
					
                   
					
			 	    
					 if (ModelState.IsValid)
					 {
							 RoomOccupancyStatusModelValidator validator = new RoomOccupancyStatusModelValidator();
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
								 model.RoomOccupancyStatusid =Guid.NewGuid(); 
                                 
                                  
								 
                                 strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/RoomOccupancyStatus/Add_Room_Occupancy_Status", model);
                                    
								 
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
                 
                 _logger.LogError(ex,"An exception occurred in - RoomOccupancyStatus / Add_Room_Occupancy_Status, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
               
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

				
			  public virtual async Task<IActionResult> Update_Room_Occupancy_Status(string RoomOccupancyStatusid)
			  {

                    string redirectTo="";
                    if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
                            DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
                            DataView dv = new DataView(NalamVazharole_JSON);
                            dv.RowFilter = "controllername='RoomOccupancyStatus' AND viewname='list'";

                            if(dv.Count  >0){
                                redirectTo = dv[0]["actionmethodname"] as string;
							 
                            }

                            try{
                                     var jsonObjRoomOccupancyStatus = await ApiClient.Get_ApiValues(getHttpClient(), "api/RoomOccupancyStatus/getById_RoomOccupancyStatus?RoomOccupancyStatusid="+RoomOccupancyStatusid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                                if(jsonObjRoomOccupancyStatus.Length > 2)
                                {
                                  
                                    var model = JsonConvert.DeserializeObject<RoomOccupancyStatusModel>(jsonObjRoomOccupancyStatus);


                
                                     
                                    return View("Add_Room_Occupancy_Status", model);
                                }
                                else
                                {
                    
                                    TempData["message"] = "Data Not Found - Contact Administrator";
                                    return RedirectToAction(redirectTo);
						 
                                }

                            }catch(Exception ex){
                               _logger.LogError(ex,"An exception occurred in - RoomOccupancyStatus / Update_Room_Occupancy_Status, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                                TempData["errMessage"] = "Error while fetching data - Contact Administrator";
                                return RedirectToAction(redirectTo);
                            }

                    }
                    TempData["errMessage"] = "Session Expired";
                    return RedirectToAction("Logout", "users");
                }	
			  [HttpPost()]
				public virtual async Task<string> Update_Room_Occupancy_Status(RoomOccupancyStatusModel model, IFormCollection collection)
				{
					string strReturnMessage = "";
					try
					{
							ModelState.Remove("RoomOccupancyStatusid");
                            ModelState.Remove("craftmyapp_actionmethodname");
                             model.craftmyapp_actionmethodname="Update_Room_Occupancy_Status";
							
							
							if(HttpContext.Session.GetString("NalamVazhaloginUserID") != null)
					model.modifieduser =new Guid(HttpContext.Session.GetString("NalamVazhaloginUserID"));
					else
					return "Session Expired";
							
                            
                            
							if (ModelState.IsValid)
							{
									RoomOccupancyStatusModelValidator validator = new RoomOccupancyStatusModelValidator();
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
                                        
										
                                        
                                        
                                        strReturnMessage = await ApiClient.Post_ApiValuesGetString(getHttpClient(),"api/RoomOccupancyStatus/Update_Room_Occupancy_Status", model);
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
                      _logger.LogError(ex,"An exception occurred in - RoomOccupancyStatus / Update_Room_Occupancy_Status, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
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
public virtual async Task<IActionResult> Remove_Room_Occupancy_Status(string RoomOccupancyStatusid)
			{
				string message = "";
				try
				{
						message = await ApiClient.Get_ApiValues(getHttpClient(), "api/RoomOccupancyStatus/Remove_Room_Occupancy_Status?RoomOccupancyStatusid="+RoomOccupancyStatusid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
						 if(message.Replace("\"","").Contains("201.1"))
						{
							TempData["message"] = "Success";

						}else{
							TempData["errMessage"] = message.Replace("\"","");
						}
						
				
				
				}
				catch (Exception ex)
				{
                     _logger.LogError(ex,"An exception occurred in - RoomOccupancyStatus / Remove_Room_Occupancy_Status, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
              
                
					 TempData["errMessage"] = ex.Message;
					 message = ex.Message;
				}

				string redirectTo="";
						if(HttpContext.Session.GetString("NalamVazharole_JSON") != null){
					DataTable NalamVazharole_JSON =HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
						 DataView dv = new DataView(NalamVazharole_JSON);
						 dv.RowFilter = "controllername='RoomOccupancyStatus' AND viewname='list'";

						if(dv.Count  >0){
						    redirectTo = dv[0]["actionmethodname"] as string;
							 
						}

					}
				
				return RedirectToAction(redirectTo);
			}

                                        public virtual IActionResult Detail_Room_Occupancy_Status()
                                        {
                                            return View();
                                        }

			        public virtual IActionResult Room_Occupancy_Status_List()
			        {
				        return View();
			        }
			        	
			        [HttpGet()]
			        public virtual async Task<string> get_Room_Occupancy_Status_List(string tenantid
,string patientvisit
,string patientname
,string ipdno
, int? pagesize=100 , int? pagenumber=0,string searchterm="",string sortFieldsJson="", bool returnAllRecords=false)
			        {
				        
				        return await ApiClient.Get_ApiValues(getHttpClient(), "api/RoomOccupancyStatus/Room_Occupancy_Status_List?tenantid="+tenantid+"&patientvisit="+patientvisit+"&patientname="+patientname+"&ipdno="+ipdno+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID")
+ "&pagesize=" + pagesize + "&pagenumber="+ pagenumber + "&searchterm="+ searchterm + "&sort_fields=" + sortFieldsJson + "&returnAllRecords=" + returnAllRecords);
			        }
			          
											[HttpGet()]
											public virtual async Task<string> get_all_PatientVisit(string tenantid,string searchterm, string pagesize="1000", string pagenumber="1")
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/PatientVisit/get_all_PatientVisit?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
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
											public virtual async Task<string> get_all_Block(string tenantid)
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/Block/get_all_Block?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											 
											[HttpGet()]
											public virtual async Task<string> get_all_Building(string tenantid)
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/Building/get_all_Building?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											 
											[HttpGet()]
											public virtual async Task<string> get_all_Floor(string tenantid)
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/Floor/get_all_Floor?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											 
											[HttpGet()]
											public virtual async Task<string> get_all_Room(string tenantid)
											{
											 
											return await ApiClient.Get_ApiValues(getHttpClient(), "api/Room/get_all_Room?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
											}
											

				
			  public virtual async Task<string> getById_allinfo_RoomOccupancyStatus(string RoomOccupancyStatusid)
			  {
					return await ApiClient.Get_ApiValues(getHttpClient(), "api/RoomOccupancyStatus/getById_allinfo_RoomOccupancyStatus?RoomOccupancyStatusid="+RoomOccupancyStatusid);
					 
			  }
[HttpGet()]
                    public virtual async Task<string> lookup_RoomOccupancyStatus_patientvisit(String tenantid,string searchterm, int? pagesize, int? pagenumber)
                    {
                        
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/RoomOccupancyStatus/lookup_RoomOccupancyStatus_patientvisit?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }
[HttpGet()]
                    public virtual async Task<string> lookup_RoomOccupancyStatus_patientname(String tenantid,string searchterm, int? pagesize, int? pagenumber)
                    {
                        
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/RoomOccupancyStatus/lookup_RoomOccupancyStatus_patientname?tenantid="+tenantid+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }
[HttpGet()]
                    public virtual async Task<string> lookup_RoomOccupancyStatus_ipdno(String tenantid,String patientname,string searchterm, int? pagesize, int? pagenumber)
                    {
                        
                        return await ApiClient.Get_ApiValues(getHttpClient(), "api/RoomOccupancyStatus/lookup_RoomOccupancyStatus_ipdno?tenantid="+tenantid+"&patientname="+patientname+"&searchterm="+searchterm+"&pagesize="+pagesize+"&pagenumber="+pagenumber+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
                    }
[HttpGet()]
			    public virtual async Task<string> lookup_RoomOccupancyStatus_block(String tenantid)
			    {
                    
				    return await ApiClient.Get_ApiValues(getHttpClient(), "api/RoomOccupancyStatus/lookup_RoomOccupancyStatus_block?tenantid="+tenantid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			    }
[HttpGet()]
			    public virtual async Task<string> lookup_RoomOccupancyStatus_building(String tenantid,String block)
			    {
                    
				    return await ApiClient.Get_ApiValues(getHttpClient(), "api/RoomOccupancyStatus/lookup_RoomOccupancyStatus_building?tenantid="+tenantid+"&block="+block+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			    }
[HttpGet()]
			    public virtual async Task<string> lookup_RoomOccupancyStatus_floor(String tenantid,String block,String building)
			    {
                    
				    return await ApiClient.Get_ApiValues(getHttpClient(), "api/RoomOccupancyStatus/lookup_RoomOccupancyStatus_floor?tenantid="+tenantid+"&block="+block+"&building="+building+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			    }
[HttpGet()]
			    public virtual async Task<string> lookup_RoomOccupancyStatus_room(String tenantid,String block,String building,String floor)
			    {
                    
				    return await ApiClient.Get_ApiValues(getHttpClient(), "api/RoomOccupancyStatus/lookup_RoomOccupancyStatus_room?tenantid="+tenantid+"&block="+block+"&building="+building+"&floor="+floor+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			    }



[HttpGet()]
			public virtual async Task<string> lookup_change_RoomOccupancyStatus_patientvisit(string PatientVisitid)
			{
				return await ApiClient.Get_ApiValues(getHttpClient(), "api/RoomOccupancyStatus/lookup_change_RoomOccupancyStatus_patientvisit?PatientVisitid="+PatientVisitid+"&loginUserID="+HttpContext.Session.GetString("NalamVazhaloginUserID"));
			}



                    
                     
                        

				}


			}
