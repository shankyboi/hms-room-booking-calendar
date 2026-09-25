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
				//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:36
				public class ClinicalTaskController : BaseController
				{
				    public ClinicalTaskController(IHttpContextAccessor httpContextAccessor,IOptions<ConnectionSettings> connectionSettings, ILoggerFactory loggerFactory, IConfiguration configuration,IWebHostEnvironment hostingEnvironment)
				    {
					     _configuration = configuration;
					     _logger = loggerFactory.CreateLogger<ClinicalTaskController>();
					     _connectionSettings = connectionSettings;
					     objClinicalTaskDAL = new ClinicalTaskDAL(_connectionSettings.Value.ConnectionString);
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
				private ClinicalTaskDAL objClinicalTaskDAL;
                private External_System_DAL obj_External_System_DAL;
				private IOptions<ConnectionSettings> _connectionSettings;
				private ILogger _logger;
				private IConfiguration _configuration;
				private IWebHostEnvironment hostingEnv;
                private ExternalSystemUtility objExternalSystemUtitlity;
                private string pvar_tenantid="|";
                private string pvar_usersid="";

			    [HttpGet()]
			  [ActionName("getById_taskduration")]
			  public virtual System.Data.DataTable getById_taskduration(string ClinicalTaskid)
			  {
					DataTable dtClinicalTask_taskduration = new DataTable();
					try
					{
						dtClinicalTask_taskduration = objClinicalTaskDAL.getById_taskduration(ClinicalTaskid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_taskduration, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
    
					}
					return dtClinicalTask_taskduration;


			  }


            [HttpPost()]
            [ActionName("Add_Clinical_Task")]
            public virtual IActionResult Add_Clinical_Task([FromBody]ClinicalTaskModel model)
            { 
              string message = "";
                
                access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Add_Clinical_Task";
            try{

            if (ModelState.IsValid)
            {

            	ClinicalTaskModelValidator validator = new ClinicalTaskModelValidator();
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

		       
                                 

                                
            		                
                                     message = objClinicalTaskDAL.Add_Clinical_Task(model);
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

            	_logger.LogError("ClinicalTaskModel - Add_Clinical_Task , Validation Error :" + message);
            	message = ("Validation Error : " + message);
            }






            }catch(Exception ex){
               message=ex.Message;
               _logger.LogError(ex,"An exception occurred in - Add_Clinical_Task, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
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
			  [ActionName("getById_ClinicalTask")]
			  public virtual ClinicalTaskModel getById_ClinicalTask(string ClinicalTaskid,string loginUserID="")
			  { 
				    ClinicalTaskModel objClinicalTask = new ClinicalTaskModel();
					try
					{
						  objClinicalTask = objClinicalTaskDAL.getById_ClinicalTask(ClinicalTaskid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_ClinicalTask, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
            
					}
					return objClinicalTask;

			  }
			  [HttpPost()]
			  [ActionName("Update_Clinical_Task")]
			  public virtual IActionResult Update_Clinical_Task([FromBody]ClinicalTaskModel model)
			  { 
				    string message = "";
                   access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Update_Clinical_Task";

					try{

					if (ModelState.IsValid)
					{

						ClinicalTaskModelValidator validator = new ClinicalTaskModelValidator();
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

		       
                                 
		       
                                	
							    message = objClinicalTaskDAL.Update_Clinical_Task(model);	
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

						_logger.LogError("ClinicalTaskModel - Update_Clinical_Task, Validation Error :" + message);
					
						//return BadRequest("Validation Error : " + message);
					}






					}catch(Exception ex){
                        
						message=ex.Message;
					    _logger.LogError(ex,"An exception occurred in - Update_Clinical_Task, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
           
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
            public virtual async Task<string> Remove_Clinical_Task(string ClinicalTaskid,string loginUserID="")
			{
					string message ="";
                    access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Remove_Clinical_Task";

					try{
						
						  var authHeader = HttpContext.Request.Headers["Authorization"][0];
	                        if (authHeader.StartsWith("Bearer "))
	                        {
		                        
		                      
		                      
		                      var token = authHeader.Substring("Bearer ".Length);
		                         
		                        String[] userdetails=obj_External_System_DAL.get_users_by_token(token);
		                        loginUserID=userdetails[0].ToString();
                                obj_access_logsdetailsModel.access_logsid=new Guid(userdetails[1].ToString());
		       
                                 
                        	 message = objClinicalTaskDAL.Remove_Clinical_Task(ClinicalTaskid,loginUserID);
						    }
	                        else{
		                        message = "Invalid Token";
		                       
	                        }
					 

					}catch(Exception ex){
						message=ex.Message;
                         _logger.LogError(ex,"An exception occurred in - Remove_Clinical_Task, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                         
					}
                    if(obj_access_logsdetailsModel.access_logsid !=null)
                    {
                         obj_access_logsdetailsModel.api_response=message.Replace("\"",""); 
                         obj_External_System_DAL.create_access_logs_details(obj_access_logsdetailsModel);
                    }
                 
					return message;

			}
[HttpGet()]
			
			[ActionName("Clinical_Task_List")]
			public virtual System.Data.DataTable Clinical_Task_List(string tenantid
)
			{
					 
				  	DataTable dtClinicalTask = new DataTable();
					try
					{
                        tenantid=pvar_tenantid;
						dtClinicalTask = objClinicalTaskDAL.Clinical_Task_List( tenantid
);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - Clinical_Task_List, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                       
					}
					return dtClinicalTask;

			   }
			   
[HttpGet()]
			
			[ActionName("get_all_ClinicalTask")]
			public virtual System.Data.DataTable get_all_ClinicalTask(string tenantid,string loginUserID="")
			{
					 
				  	DataTable dtClinicalTask = new DataTable();
					try
					{
                        tenantid=pvar_tenantid;
						dtClinicalTask = objClinicalTaskDAL.get_all_ClinicalTask(tenantid);
					}
					catch (Exception ex)
					{
                        _logger.LogError(ex,"An exception occurred in - get_all_ClinicalTask, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
					   
					}
					return dtClinicalTask;

			   }
[HttpGet()]
			  [ActionName("getById_allinfo_ClinicalTask")]
			  public virtual System.Data.DataTable getById_allinfo_ClinicalTask(string ClinicalTaskid)
			  { 
				    DataTable dtClinicalTask = new DataTable();
					try
					{
						  dtClinicalTask = objClinicalTaskDAL.getById_allinfo_ClinicalTask(ClinicalTaskid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_allinfo_ClinicalTask, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
   
					}
					return dtClinicalTask;

			  }
[HttpGet()]
							[ActionName("lookup_ClinicalTask_workprofile")]
							public virtual System.Data.DataTable lookup_ClinicalTask_workprofile(String tenantid,string loginUserID="")
							{
									DataTable dtClinicalTask = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtClinicalTask = objClinicalTaskDAL.lookup_ClinicalTask_workprofile(tenantid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_ClinicalTask_workprofile, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtClinicalTask;
							}
[HttpGet()]
							[ActionName("lookup_ClinicalTask_competency")]
							public virtual System.Data.DataTable lookup_ClinicalTask_competency(String tenantid,string loginUserID="")
							{
									DataTable dtClinicalTask = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtClinicalTask = objClinicalTaskDAL.lookup_ClinicalTask_competency(tenantid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_ClinicalTask_competency, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtClinicalTask;
							}
[HttpGet()]
							[ActionName("lookup_ClinicalTask_taskduration_workprofile")]
							public virtual System.Data.DataTable lookup_ClinicalTask_taskduration_workprofile(String tenantid,string loginUserID="")
							{
									DataTable dtClinicalTask = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtClinicalTask = objClinicalTaskDAL.lookup_ClinicalTask_taskduration_workprofile(tenantid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_ClinicalTask_taskduration_workprofile, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtClinicalTask;
							}
[HttpGet()]
							[ActionName("lookup_ClinicalTask_taskduration_tasktype")]
							public virtual System.Data.DataTable lookup_ClinicalTask_taskduration_tasktype(String tenantid,string loginUserID="")
							{
									DataTable dtClinicalTask = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtClinicalTask = objClinicalTaskDAL.lookup_ClinicalTask_taskduration_tasktype(tenantid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_ClinicalTask_taskduration_tasktype, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtClinicalTask;
							}
[HttpGet()]
							[ActionName("lookup_ClinicalTask_taskduration_taskname")]
							public virtual System.Data.DataTable lookup_ClinicalTask_taskduration_taskname(String tenantid,String tasktype,string loginUserID="")
							{
									DataTable dtClinicalTask = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtClinicalTask = objClinicalTaskDAL.lookup_ClinicalTask_taskduration_taskname(tenantid,tasktype);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_ClinicalTask_taskduration_taskname, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtClinicalTask;
							}






				}


			}
