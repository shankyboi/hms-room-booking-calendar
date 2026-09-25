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
				//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/01/2026 11:54:48
				public class AppoinmentController : BaseController
				{
				    public AppoinmentController(IHttpContextAccessor httpContextAccessor,IOptions<ConnectionSettings> connectionSettings, ILoggerFactory loggerFactory, IConfiguration configuration,IWebHostEnvironment hostingEnvironment)
				    {
					     _configuration = configuration;
					     _logger = loggerFactory.CreateLogger<AppoinmentController>();
					     _connectionSettings = connectionSettings;
					     objAppoinmentDAL = new AppoinmentDAL(_connectionSettings.Value.ConnectionString);
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
				private AppoinmentDAL objAppoinmentDAL;
                private External_System_DAL obj_External_System_DAL;
				private IOptions<ConnectionSettings> _connectionSettings;
				private ILogger _logger;
				private IConfiguration _configuration;
				private IWebHostEnvironment hostingEnv;
                private ExternalSystemUtility objExternalSystemUtitlity;
                private string pvar_tenantid="|";
                private string pvar_usersid="";

			    
            [HttpPost()]
            [ActionName("Add_Appoinment")]
            public virtual IActionResult Add_Appoinment([FromBody]AppoinmentModel model)
            { 
              string message = "";
                
                access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Add_Appoinment";
            try{

            if (ModelState.IsValid)
            {

            	AppoinmentModelValidator validator = new AppoinmentModelValidator();
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

		       
                                 

                                
            		                
                                     message = objAppoinmentDAL.Add_Appoinment(model);
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

            	_logger.LogError("AppoinmentModel - Add_Appoinment , Validation Error :" + message);
            	message = ("Validation Error : " + message);
            }






            }catch(Exception ex){
               message=ex.Message;
               _logger.LogError(ex,"An exception occurred in - Add_Appoinment, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
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
			  [ActionName("getById_Appoinment")]
			  public virtual AppoinmentModel getById_Appoinment(string Appoinmentid,string loginUserID="")
			  { 
				    AppoinmentModel objAppoinment = new AppoinmentModel();
					try
					{
						  objAppoinment = objAppoinmentDAL.getById_Appoinment(Appoinmentid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_Appoinment, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
            
					}
					return objAppoinment;

			  }
			  [HttpPost()]
			  [ActionName("Update_Appoinment")]
			  public virtual IActionResult Update_Appoinment([FromBody]AppoinmentModel model)
			  { 
				    string message = "";
                   access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Update_Appoinment";

					try{

					if (ModelState.IsValid)
					{

						AppoinmentModelValidator validator = new AppoinmentModelValidator();
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

		       
                                 
		       
                                	
							    message = objAppoinmentDAL.Update_Appoinment(model);	
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

						_logger.LogError("AppoinmentModel - Update_Appoinment, Validation Error :" + message);
					
						//return BadRequest("Validation Error : " + message);
					}






					}catch(Exception ex){
                        
						message=ex.Message;
					    _logger.LogError(ex,"An exception occurred in - Update_Appoinment, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
           
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
            public virtual async Task<string> Remove_Appoinment(string Appoinmentid,string loginUserID="")
			{
					string message ="";
                    access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Remove_Appoinment";

					try{
						
						  var authHeader = HttpContext.Request.Headers["Authorization"][0];
	                        if (authHeader.StartsWith("Bearer "))
	                        {
		                        
		                      
		                      
		                      var token = authHeader.Substring("Bearer ".Length);
		                         
		                        String[] userdetails=obj_External_System_DAL.get_users_by_token(token);
		                        loginUserID=userdetails[0].ToString();
                                obj_access_logsdetailsModel.access_logsid=new Guid(userdetails[1].ToString());
		       
                                 
                        	 message = objAppoinmentDAL.Remove_Appoinment(Appoinmentid,loginUserID);
						    }
	                        else{
		                        message = "Invalid Token";
		                       
	                        }
					 

					}catch(Exception ex){
						message=ex.Message;
                         _logger.LogError(ex,"An exception occurred in - Remove_Appoinment, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                         
					}
                    if(obj_access_logsdetailsModel.access_logsid !=null)
                    {
                         obj_access_logsdetailsModel.api_response=message.Replace("\"",""); 
                         obj_External_System_DAL.create_access_logs_details(obj_access_logsdetailsModel);
                    }
                 
					return message;

			}
[HttpGet()]
			
			[ActionName("Appoinment_List")]
			public virtual JObject Appoinment_List(string tenantid
,string patient=""
,string origin=""
,string bookingreferencenumber=""
,string doctor=""
,string appointmentdate=""
,string task=""
,string status=""
,string loginUserID="",int? pagesize=100 , int? pagenumber=0,string searchterm="",string  sort_fields = "")
			{
					 
				  	JObject jsonAppoinment = null;
					try
					{
                        tenantid=pvar_tenantid;
						jsonAppoinment = objAppoinmentDAL.Appoinment_List( tenantid
, patient
, origin
, bookingreferencenumber
, doctor
, appointmentdate
, task
, status
,pagesize,pagenumber,searchterm,sort_fields);
					}
					catch (Exception ex)
					{
                             _logger.LogError(ex,"An exception occurred in - Appoinment_List, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                        
					}
					return jsonAppoinment;

			   }
			   
[HttpGet()]
			
			[ActionName("get_all_Appoinment")]
			public virtual System.Data.DataTable get_all_Appoinment(string tenantid,string searchterm="",string pagesize = "50", string pagenumber = "1",string loginUserID="")
			{
					  int? pageSize = null; 
                     if (!string.IsNullOrEmpty(pagesize))
                     {                 
                     pageSize = int.Parse(pagesize);
                     }
                     int? pageNumber = null;
                     if (!string.IsNullOrEmpty(pagenumber))
                     {
 
                     pageNumber = int.Parse(pagenumber);
                     pageNumber--;//0 based index for page numbers
                     }
				  	DataTable dtAppoinment = new DataTable();
					try
					{
                        tenantid=pvar_tenantid;
						dtAppoinment = objAppoinmentDAL.get_all_Appoinment(tenantid,searchterm,pageSize,pageNumber);
					}
					catch (Exception ex)
					{
                          _logger.LogError(ex,"An exception occurred in - get_all_Appoinment, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
					    
					}
					return dtAppoinment;

			   }
[HttpGet()]
			  [ActionName("getById_allinfo_Appoinment")]
			  public virtual System.Data.DataTable getById_allinfo_Appoinment(string Appoinmentid)
			  { 
				    DataTable dtAppoinment = new DataTable();
					try
					{
						  dtAppoinment = objAppoinmentDAL.getById_allinfo_Appoinment(Appoinmentid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_allinfo_Appoinment, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
   
					}
					return dtAppoinment;

			  }
[HttpGet()]
							[ActionName("lookup_Appoinment_patient")]
							public virtual System.Data.DataTable lookup_Appoinment_patient(String tenantid,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtAppoinment = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        int? pageSize = null;
            if (!string.IsNullOrEmpty(pagesize))
            {
                pageSize = int.Parse(pagesize);
            }

            int? pageNumber = null;
            if (!string.IsNullOrEmpty(pagenumber))
            {
                pageNumber = int.Parse(pagenumber);
                pageNumber--;//0 based index for page numbers
            }
										dtAppoinment = objAppoinmentDAL.lookup_Appoinment_patient(tenantid,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_Appoinment_patient, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtAppoinment;
							}
[HttpGet()]
							[ActionName("lookup_Appoinment_origin")]
							public virtual System.Data.DataTable lookup_Appoinment_origin(String tenantid,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtAppoinment = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        int? pageSize = null;
            if (!string.IsNullOrEmpty(pagesize))
            {
                pageSize = int.Parse(pagesize);
            }

            int? pageNumber = null;
            if (!string.IsNullOrEmpty(pagenumber))
            {
                pageNumber = int.Parse(pagenumber);
                pageNumber--;//0 based index for page numbers
            }
										dtAppoinment = objAppoinmentDAL.lookup_Appoinment_origin(tenantid,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_Appoinment_origin, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtAppoinment;
							}
[HttpGet()]
							[ActionName("lookup_Appoinment_bookingreferencenumber")]
							public virtual System.Data.DataTable lookup_Appoinment_bookingreferencenumber(String tenantid,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtAppoinment = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        int? pageSize = null;
            if (!string.IsNullOrEmpty(pagesize))
            {
                pageSize = int.Parse(pagesize);
            }

            int? pageNumber = null;
            if (!string.IsNullOrEmpty(pagenumber))
            {
                pageNumber = int.Parse(pagenumber);
                pageNumber--;//0 based index for page numbers
            }
										dtAppoinment = objAppoinmentDAL.lookup_Appoinment_bookingreferencenumber(tenantid,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_Appoinment_bookingreferencenumber, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtAppoinment;
							}
[HttpGet()]
							[ActionName("lookup_Appoinment_doctor")]
							public virtual System.Data.DataTable lookup_Appoinment_doctor(String tenantid,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtAppoinment = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        int? pageSize = null;
            if (!string.IsNullOrEmpty(pagesize))
            {
                pageSize = int.Parse(pagesize);
            }

            int? pageNumber = null;
            if (!string.IsNullOrEmpty(pagenumber))
            {
                pageNumber = int.Parse(pagenumber);
                pageNumber--;//0 based index for page numbers
            }
										dtAppoinment = objAppoinmentDAL.lookup_Appoinment_doctor(tenantid,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_Appoinment_doctor, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtAppoinment;
							}
[HttpGet()]
							[ActionName("lookup_Appoinment_appointmentdate")]
							public virtual System.Data.DataTable lookup_Appoinment_appointmentdate(String tenantid,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtAppoinment = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        int? pageSize = null;
            if (!string.IsNullOrEmpty(pagesize))
            {
                pageSize = int.Parse(pagesize);
            }

            int? pageNumber = null;
            if (!string.IsNullOrEmpty(pagenumber))
            {
                pageNumber = int.Parse(pagenumber);
                pageNumber--;//0 based index for page numbers
            }
										dtAppoinment = objAppoinmentDAL.lookup_Appoinment_appointmentdate(tenantid,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_Appoinment_appointmentdate, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtAppoinment;
							}
[HttpGet()]
							[ActionName("lookup_Appoinment_task")]
							public virtual System.Data.DataTable lookup_Appoinment_task(String tenantid,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtAppoinment = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        int? pageSize = null;
            if (!string.IsNullOrEmpty(pagesize))
            {
                pageSize = int.Parse(pagesize);
            }

            int? pageNumber = null;
            if (!string.IsNullOrEmpty(pagenumber))
            {
                pageNumber = int.Parse(pagenumber);
                pageNumber--;//0 based index for page numbers
            }
										dtAppoinment = objAppoinmentDAL.lookup_Appoinment_task(tenantid,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_Appoinment_task, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtAppoinment;
							}
[HttpGet()]
							[ActionName("lookup_Appoinment_duration")]
							public virtual System.Data.DataTable lookup_Appoinment_duration(String tenantid,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtAppoinment = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        int? pageSize = null;
            if (!string.IsNullOrEmpty(pagesize))
            {
                pageSize = int.Parse(pagesize);
            }

            int? pageNumber = null;
            if (!string.IsNullOrEmpty(pagenumber))
            {
                pageNumber = int.Parse(pagenumber);
                pageNumber--;//0 based index for page numbers
            }
										dtAppoinment = objAppoinmentDAL.lookup_Appoinment_duration(tenantid,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_Appoinment_duration, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtAppoinment;
							}
[HttpGet()]
							[ActionName("lookup_Appoinment_status")]
							public virtual System.Data.DataTable lookup_Appoinment_status(String tenantid,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtAppoinment = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        int? pageSize = null;
            if (!string.IsNullOrEmpty(pagesize))
            {
                pageSize = int.Parse(pagesize);
            }

            int? pageNumber = null;
            if (!string.IsNullOrEmpty(pagenumber))
            {
                pageNumber = int.Parse(pagenumber);
                pageNumber--;//0 based index for page numbers
            }
										dtAppoinment = objAppoinmentDAL.lookup_Appoinment_status(tenantid,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_Appoinment_status, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtAppoinment;
							}






				}


			}
