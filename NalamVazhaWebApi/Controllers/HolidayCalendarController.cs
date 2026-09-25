namespace NalamVazhaWebApi.Controllers
			{
				using System;
				using System.Data;
				using System.Linq;
                using Newtonsoft.Json.Linq;
				using Microsoft.AspNetCore.Mvc;
				using System.Collections.Generic;
				using Microsoft.Extensions.Options;
				using Microsoft.Extensions.Logging;
				using Microsoft.AspNetCore.Authorization;
				using Microsoft.Extensions.Configuration;
				using System.IdentityModel.Tokens.Jwt;
				using System.Security.Claims;
				using System.Text;
				using Microsoft.IdentityModel.Tokens;
				using NalamVazha.Models;
				using NalamVazha.DAL;
				using FluentValidation.Results;

				using Microsoft.AspNetCore.Hosting;
				using System.IO;
				using System.Net.Http.Headers;
                using Microsoft.AspNetCore.Http;
                using Newtonsoft.Json;
                using System.Threading.Tasks;
				[Route("api/[controller]/[action]")]
				//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/30/2026 12:48:26
				public class HolidayCalendarController : BaseController
				{
				    public HolidayCalendarController(IHttpContextAccessor httpContextAccessor,IOptions<ConnectionSettings> connectionSettings, ILoggerFactory loggerFactory, IConfiguration configuration,IWebHostEnvironment hostingEnvironment)
				    {
					     _configuration = configuration;
					     _logger = loggerFactory.CreateLogger<HolidayCalendarController>();
					     _connectionSettings = connectionSettings;
					     objHolidayCalendarDAL = new HolidayCalendarDAL(_connectionSettings.Value.ConnectionString);
                         obj_External_System_DAL =new External_System_DAL(_connectionSettings.Value.ConnectionString);
                         objExternalSystemUtitlity = new ExternalSystemUtility(_connectionSettings, _configuration);
					     hostingEnv = hostingEnvironment;

                            var authHeader = httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();

                            if (authHeader.StartsWith("Bearer "))
                            {
                                   var token = authHeader.Substring("Bearer ".Length);
                                   string usersid = obj_External_System_DAL.get_users_info_by_token(token);
                                   string tenantid = string.IsNullOrEmpty(httpContextAccessor.HttpContext.Request.Query["tenantid"]) ? Guid.Empty.ToString(): httpContextAccessor.HttpContext.Request.Query["tenantid"].ToString();
                                   pvar_tenantid = usersid + "|" + tenantid;//usersid+tenantid
                                   pvar_usersid=usersid;

                            }
				    }
				private HolidayCalendarDAL objHolidayCalendarDAL;
                private External_System_DAL obj_External_System_DAL;
				private IOptions<ConnectionSettings> _connectionSettings;
				private ILogger _logger;
				private IConfiguration _configuration;
				private IWebHostEnvironment hostingEnv;
                private ExternalSystemUtility objExternalSystemUtitlity;
                private string pvar_tenantid="|";
                private string pvar_usersid="";

			    [HttpGet()]
			  [ActionName("getById_taskallowed")]
			  public virtual System.Data.DataTable getById_taskallowed(string HolidayCalendarid)
			  {
					DataTable dtHolidayCalendar_taskallowed = new DataTable();
					try
					{
						dtHolidayCalendar_taskallowed = objHolidayCalendarDAL.getById_taskallowed(HolidayCalendarid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_taskallowed, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
    
					}
					return dtHolidayCalendar_taskallowed;


			  }


            [HttpPost()]
            [ActionName("Add_Holiday")]
            public virtual IActionResult Add_Holiday([FromBody]HolidayCalendarModel model)
            { 
              string message = "";
                
                access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Add_Holiday";
            try{

            if (ModelState.IsValid)
            {

            	HolidayCalendarModelValidator validator = new HolidayCalendarModelValidator();
            	ValidationResult results = validator.Validate(model);
            	if (!results.IsValid)
            	{
            		var errorCollection = string.Join(" | ", results.Errors.Select(e => e.ErrorMessage.Replace("{propertyName}",e.PropertyName)));
             		message = ("Validation Error : " + errorCollection);


            	}else{

                                   var authHeader = HttpContext.Request.Headers["Authorization"][0];
                                if (authHeader.StartsWith("Bearer "))
                                {
                                     
		                      
                                var token = authHeader.Substring("Bearer ".Length);
		                        String[] userdetails=obj_External_System_DAL.get_users_by_token(token);
                                model.createduser=new Guid(userdetails[0].ToString());
                                obj_access_logsdetailsModel.access_logsid=new Guid(userdetails[1].ToString());

		       
                                 

                                
            		                
                                     message = objHolidayCalendarDAL.Add_Holiday(model);
                               }
                                else{
                                  message = "Invalid Token";
                                 }

            	}


            }
            else
            {
            	var errorCollection = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            	message = errorCollection.ToString();

            	_logger.LogError("HolidayCalendarModel - Add_Holiday , Validation Error :" + message);
            	message = ("Validation Error : " + message);
            }






            }catch(Exception ex){
               message=ex.Message;
               _logger.LogError(ex,"An exception occurred in - Add_Holiday, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
            }
            if(obj_access_logsdetailsModel.access_logsid !=null)
            {
                 obj_access_logsdetailsModel.api_response=message.Replace("\"",""); 
                 obj_External_System_DAL.create_access_logs_details(obj_access_logsdetailsModel);
            }

            if(message.Replace("\"","").Contains("201.1"))
                    return Ok(message);
                    else if(message.Replace("\"","")=="401.1")
                    return Unauthorized(message);
                    else
                    return BadRequest(message);


             }
[HttpGet()]
			  [ActionName("getById_HolidayCalendar")]
			  public virtual HolidayCalendarModel getById_HolidayCalendar(string HolidayCalendarid,string loginUserID="")
			  { 
				    HolidayCalendarModel objHolidayCalendar = new HolidayCalendarModel();
					try
					{
						  objHolidayCalendar = objHolidayCalendarDAL.getById_HolidayCalendar(HolidayCalendarid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_HolidayCalendar, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
            
					}
					return objHolidayCalendar;

			  }
			  [HttpPost()]
			  [ActionName("Update_Holiday")]
			  public virtual IActionResult Update_Holiday([FromBody]HolidayCalendarModel model)
			  { 
				    string message = "";
                   access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Update_Holiday";

					try{

					if (ModelState.IsValid)
					{

						HolidayCalendarModelValidator validator = new HolidayCalendarModelValidator();
						ValidationResult results = validator.Validate(model);
						if (!results.IsValid)
						{
							var errorCollection = string.Join(" | ", results.Errors.Select(e => e.ErrorMessage.Replace("{propertyName}",e.PropertyName)));
							message = errorCollection.ToString();
							//return BadRequest("Validation Error : " + message);

						}else{
                            var authHeader = HttpContext.Request.Headers["Authorization"][0];
	                        if (authHeader.StartsWith("Bearer "))
	                        {
		                       
                                 
		                      
                                var token = authHeader.Substring("Bearer ".Length);
		                        String[] userdetails=obj_External_System_DAL.get_users_by_token(token);
                                model.modifieduser=new Guid(userdetails[0].ToString());
                                obj_access_logsdetailsModel.access_logsid=new Guid(userdetails[1].ToString());

		       
                                 
		       
                                	
							    message = objHolidayCalendarDAL.Update_Holiday(model);	
                            }
                            else{
                                message = "Invalid Token";
                                 
                            }
							
						}


					}
					else
					{
						var errorCollection = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
						message = errorCollection.ToString();

						_logger.LogError("HolidayCalendarModel - Update_Holiday, Validation Error :" + message);
					
						//return BadRequest("Validation Error : " + message);
					}






					}catch(Exception ex){
                        
						message=ex.Message;
					    _logger.LogError(ex,"An exception occurred in - Update_Holiday, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
           
					}

                    if(obj_access_logsdetailsModel.access_logsid !=null)
                    {
                            obj_access_logsdetailsModel.api_response=message.Replace("\"",""); 
                            obj_External_System_DAL.create_access_logs_details(obj_access_logsdetailsModel);
                    }

					if(message.Replace("\"","")=="201.1")
					return Ok(message);
					else if(message.Replace("\"","")=="401.1")
					return Unauthorized(message);
					else
					return BadRequest(message);

					


			   }
[HttpGet()]
            public virtual async Task<string> Remove_Holiday(string HolidayCalendarid,string loginUserID="")
			{
					string message ="";
                    access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Remove_Holiday";

					try{
						
						  var authHeader = HttpContext.Request.Headers["Authorization"][0];
	                        if (authHeader.StartsWith("Bearer "))
	                        {
		                        
		                      
		                      
		                      var token = authHeader.Substring("Bearer ".Length);
		                         
		                        String[] userdetails=obj_External_System_DAL.get_users_by_token(token);
		                        loginUserID=userdetails[0].ToString();
                                obj_access_logsdetailsModel.access_logsid=new Guid(userdetails[1].ToString());
		       
                                 
                        	 message = objHolidayCalendarDAL.Remove_Holiday(HolidayCalendarid,loginUserID);
						    }
	                        else{
		                        message = "Invalid Token";
		                       
	                        }
					 

					}catch(Exception ex){
						message=ex.Message;
                         _logger.LogError(ex,"An exception occurred in - Remove_Holiday, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                         
					}
                    if(obj_access_logsdetailsModel.access_logsid !=null)
                    {
                         obj_access_logsdetailsModel.api_response=message.Replace("\"",""); 
                         obj_External_System_DAL.create_access_logs_details(obj_access_logsdetailsModel);
                    }
                 
					return message;

			}
[HttpGet()]
			
			[ActionName("Holiday_Calendar")]
			public virtual System.Data.DataTable Holiday_Calendar(string tenantid
,string holidaydate_automatonfrom=""
,string holidaydate_automatonto=""
)
			{
					 
				  	DataTable dtHolidayCalendar = new DataTable();
					try
					{
                        tenantid=pvar_tenantid;
						dtHolidayCalendar = objHolidayCalendarDAL.Holiday_Calendar( tenantid
, holidaydate_automatonfrom
, holidaydate_automatonto
);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - Holiday_Calendar, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                       
					}
					return dtHolidayCalendar;

			   }
			   
[HttpGet()]
			
			[ActionName("get_all_HolidayCalendar")]
			public virtual System.Data.DataTable get_all_HolidayCalendar(string tenantid,string loginUserID="")
			{
					 
				  	DataTable dtHolidayCalendar = new DataTable();
					try
					{
                        tenantid=pvar_tenantid;
						dtHolidayCalendar = objHolidayCalendarDAL.get_all_HolidayCalendar(tenantid);
					}
					catch (Exception ex)
					{
                        _logger.LogError(ex,"An exception occurred in - get_all_HolidayCalendar, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
					   
					}
					return dtHolidayCalendar;

			   }
[HttpGet()]
			  [ActionName("getById_allinfo_HolidayCalendar")]
			  public virtual System.Data.DataTable getById_allinfo_HolidayCalendar(string HolidayCalendarid)
			  { 
				    DataTable dtHolidayCalendar = new DataTable();
					try
					{
						  dtHolidayCalendar = objHolidayCalendarDAL.getById_allinfo_HolidayCalendar(HolidayCalendarid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_allinfo_HolidayCalendar, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
   
					}
					return dtHolidayCalendar;

			  }
[HttpGet()]
							[ActionName("lookup_HolidayCalendar_taskallowed_taskname")]
							public virtual System.Data.DataTable lookup_HolidayCalendar_taskallowed_taskname(String tenantid,string loginUserID="")
							{
									DataTable dtHolidayCalendar = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtHolidayCalendar = objHolidayCalendarDAL.lookup_HolidayCalendar_taskallowed_taskname(tenantid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_HolidayCalendar_taskallowed_taskname, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtHolidayCalendar;
							}


							[HttpGet()]
							[ActionName("Get_Holiday_Blocked_Dates")]
							public virtual System.Data.DataTable Get_Holiday_Blocked_Dates(string tenantid, string taskid = "", string taskname = "", string datefrom = "", string dateto = "", string loginUserID = "")
							{
									DataTable dtHolidayCalendar = new DataTable();
									try
									{
										if (string.IsNullOrEmpty(tenantid))
											tenantid = pvar_tenantid;
										dtHolidayCalendar = objHolidayCalendarDAL.Get_Holiday_Blocked_Dates(tenantid, taskid, taskname, datefrom, dateto);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - Get_Holiday_Blocked_Dates, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
									}
									return dtHolidayCalendar;
							}

        [HttpGet()]
        [ActionName("Get_Holiday_By_Date")]
        public virtual async Task<IActionResult> Get_Holiday_By_Date(
            Guid tenantid,
            DateTime datefrom,
            DateTime dateto,
            string loginUserID = "")
        {
            try
            {
                var result =
                    await objHolidayCalendarDAL
                        .Get_Holiday_By_Date(
                            tenantid,
                            datefrom,
                            dateto
                        );

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error in Get_Holiday_By_Date: " +
                    ex.Message
                );

                return StatusCode(
                    500,
                    ex.Message
                );
            }
        }



    }


			}
