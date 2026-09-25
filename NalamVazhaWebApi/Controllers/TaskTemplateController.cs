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
				//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/03/2026 08:27:28
				public class TaskTemplateController : BaseController
				{
				    public TaskTemplateController(IHttpContextAccessor httpContextAccessor,IOptions<ConnectionSettings> connectionSettings, ILoggerFactory loggerFactory, IConfiguration configuration,IWebHostEnvironment hostingEnvironment)
				    {
					     _configuration = configuration;
					     _logger = loggerFactory.CreateLogger<TaskTemplateController>();
					     _connectionSettings = connectionSettings;
					     objTaskTemplateDAL = new TaskTemplateDAL(_connectionSettings.Value.ConnectionString);
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
				private TaskTemplateDAL objTaskTemplateDAL;
                private External_System_DAL obj_External_System_DAL;
				private IOptions<ConnectionSettings> _connectionSettings;
				private ILogger _logger;
				private IConfiguration _configuration;
				private IWebHostEnvironment hostingEnv;
                private ExternalSystemUtility objExternalSystemUtitlity;
                private string pvar_tenantid="|";
                private string pvar_usersid="";

			    [HttpGet()]
			  [ActionName("getById_escalationdetails")]
			  public virtual System.Data.DataTable getById_escalationdetails(string TaskTemplateid)
			  {
					DataTable dtTaskTemplate_escalationdetails = new DataTable();
					try
					{
						dtTaskTemplate_escalationdetails = objTaskTemplateDAL.getById_escalationdetails(TaskTemplateid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_escalationdetails, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
    
					}
					return dtTaskTemplate_escalationdetails;


			  }

[HttpGet()]
			  [ActionName("getById_nextactiondetails")]
			  public virtual System.Data.DataTable getById_nextactiondetails(string TaskTemplateid)
			  {
					DataTable dtTaskTemplate_nextactiondetails = new DataTable();
					try
					{
						dtTaskTemplate_nextactiondetails = objTaskTemplateDAL.getById_nextactiondetails(TaskTemplateid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_nextactiondetails, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
    
					}
					return dtTaskTemplate_nextactiondetails;


			  }


            [HttpPost()]
            [ActionName("Add_Task_Template")]
            public virtual IActionResult Add_Task_Template([FromBody]TaskTemplateModel model)
            { 
              string message = "";
                
                access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Add_Task_Template";
            try{

            if (ModelState.IsValid)
            {

            	TaskTemplateModelValidator validator = new TaskTemplateModelValidator();
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

		       
                                 

                                
            		                
                                     message = objTaskTemplateDAL.Add_Task_Template(model);
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

            	_logger.LogError("TaskTemplateModel - Add_Task_Template , Validation Error :" + message);
            	message = ("Validation Error : " + message);
            }






            }catch(Exception ex){
               message=ex.Message;
               _logger.LogError(ex,"An exception occurred in - Add_Task_Template, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
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
			  [ActionName("getById_TaskTemplate")]
			  public virtual TaskTemplateModel getById_TaskTemplate(string TaskTemplateid,string loginUserID="")
			  { 
				    TaskTemplateModel objTaskTemplate = new TaskTemplateModel();
					try
					{
						  objTaskTemplate = objTaskTemplateDAL.getById_TaskTemplate(TaskTemplateid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_TaskTemplate, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
            
					}
					return objTaskTemplate;

			  }
			  [HttpPost()]
			  [ActionName("Update_Task_Template")]
			  public virtual IActionResult Update_Task_Template([FromBody]TaskTemplateModel model)
			  { 
				    string message = "";
                   access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Update_Task_Template";

					try{

					if (ModelState.IsValid)
					{

						TaskTemplateModelValidator validator = new TaskTemplateModelValidator();
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

		       
                                 
		       
                                	
							    message = objTaskTemplateDAL.Update_Task_Template(model);	
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

						_logger.LogError("TaskTemplateModel - Update_Task_Template, Validation Error :" + message);
					
						//return BadRequest("Validation Error : " + message);
					}






					}catch(Exception ex){
                        
						message=ex.Message;
					    _logger.LogError(ex,"An exception occurred in - Update_Task_Template, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
           
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
            public virtual async Task<string> Remove_Task_Template(string TaskTemplateid,string loginUserID="")
			{
					string message ="";
                    access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Remove_Task_Template";

					try{
						
						  var authHeader = HttpContext.Request.Headers["Authorization"][0];
	                        if (authHeader.StartsWith("Bearer "))
	                        {
		                        
		                      
		                      
		                      var token = authHeader.Substring("Bearer ".Length);
		                         
		                        String[] userdetails=obj_External_System_DAL.get_users_by_token(token);
		                        loginUserID=userdetails[0].ToString();
                                obj_access_logsdetailsModel.access_logsid=new Guid(userdetails[1].ToString());
		       
                                 
                        	 message = objTaskTemplateDAL.Remove_Task_Template(TaskTemplateid,loginUserID);
						    }
	                        else{
		                        message = "Invalid Token";
		                       
	                        }
					 

					}catch(Exception ex){
						message=ex.Message;
                         _logger.LogError(ex,"An exception occurred in - Remove_Task_Template, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                         
					}
                    if(obj_access_logsdetailsModel.access_logsid !=null)
                    {
                         obj_access_logsdetailsModel.api_response=message.Replace("\"",""); 
                         obj_External_System_DAL.create_access_logs_details(obj_access_logsdetailsModel);
                    }
                 
					return message;

			}
[HttpGet()]
			
			[ActionName("Task_Template_List")]
			public virtual JObject Task_Template_List(string tenantid
,string loginUserID="",int? pagesize=100 , int? pagenumber=0,string searchterm="",string  sort_fields = "")
			{
					 
				  	JObject jsonTaskTemplate = null;
					try
					{
                        tenantid=pvar_tenantid;
						jsonTaskTemplate = objTaskTemplateDAL.Task_Template_List( tenantid
,pagesize,pagenumber,searchterm,sort_fields);
					}
					catch (Exception ex)
					{
                             _logger.LogError(ex,"An exception occurred in - Task_Template_List, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                        
					}
					return jsonTaskTemplate;

			   }
			   
[HttpGet()]
			
			[ActionName("get_all_TaskTemplate")]
			public virtual System.Data.DataTable get_all_TaskTemplate(string tenantid,string searchterm="",string pagesize = "50", string pagenumber = "1",string loginUserID="")
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
				  	DataTable dtTaskTemplate = new DataTable();
					try
					{
                        tenantid=pvar_tenantid;
						dtTaskTemplate = objTaskTemplateDAL.get_all_TaskTemplate(tenantid,searchterm,pageSize,pageNumber);
					}
					catch (Exception ex)
					{
                          _logger.LogError(ex,"An exception occurred in - get_all_TaskTemplate, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
					    
					}
					return dtTaskTemplate;

			   }
[HttpGet()]
			  [ActionName("getById_allinfo_TaskTemplate")]
			  public virtual System.Data.DataTable getById_allinfo_TaskTemplate(string TaskTemplateid)
			  { 
				    DataTable dtTaskTemplate = new DataTable();
					try
					{
						  dtTaskTemplate = objTaskTemplateDAL.getById_allinfo_TaskTemplate(TaskTemplateid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_allinfo_TaskTemplate, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
   
					}
					return dtTaskTemplate;

			  }
[HttpGet()]
							[ActionName("lookup_TaskTemplate_tasktype")]
							public virtual System.Data.DataTable lookup_TaskTemplate_tasktype(String tenantid,string loginUserID="")
							{
									DataTable dtTaskTemplate = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtTaskTemplate = objTaskTemplateDAL.lookup_TaskTemplate_tasktype(tenantid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_TaskTemplate_tasktype, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtTaskTemplate;
							}
[HttpGet()]
							[ActionName("lookup_TaskTemplate_escalationdetails_notifyto")]
							public virtual System.Data.DataTable lookup_TaskTemplate_escalationdetails_notifyto(String tenantid,string loginUserID="")
							{
									DataTable dtTaskTemplate = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtTaskTemplate = objTaskTemplateDAL.lookup_TaskTemplate_escalationdetails_notifyto(tenantid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_TaskTemplate_escalationdetails_notifyto, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtTaskTemplate;
							}
[HttpGet()]
							[ActionName("lookup_TaskTemplate_nextactiondetails_actiontype")]
							public virtual System.Data.DataTable lookup_TaskTemplate_nextactiondetails_actiontype(String tenantid,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtTaskTemplate = new DataTable();
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
										dtTaskTemplate = objTaskTemplateDAL.lookup_TaskTemplate_nextactiondetails_actiontype(tenantid,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_TaskTemplate_nextactiondetails_actiontype, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtTaskTemplate;
							}
[HttpGet()]
							[ActionName("lookup_TaskTemplate_nextactiondetails_actionname")]
							public virtual System.Data.DataTable lookup_TaskTemplate_nextactiondetails_actionname(String tenantid,String actiontype,string loginUserID="")
							{
									DataTable dtTaskTemplate = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtTaskTemplate = objTaskTemplateDAL.lookup_TaskTemplate_nextactiondetails_actionname(tenantid,actiontype);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_TaskTemplate_nextactiondetails_actionname, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtTaskTemplate;
							}


[HttpGet()]
							[ActionName("lookup_change_escalationdetails_TaskTemplate_notifyto")]
							public virtual System.Data.DataTable lookup_change_escalationdetails_TaskTemplate_notifyto(string usersid,string loginUserID="")
							{
									DataTable dtusers = new DataTable();
									try
									{
										dtusers = objTaskTemplateDAL.lookup_change_escalationdetails_TaskTemplate_notifyto(usersid);
									}
									catch (Exception ex)
									{
									  _logger.LogError(ex,"An exception occurred in - lookup_change_escalationdetails_TaskTemplate_notifyto, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtusers;
							}




				}


			}
