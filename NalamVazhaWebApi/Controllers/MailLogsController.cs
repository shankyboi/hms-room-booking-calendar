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
				//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:28
				public class MailLogsController : BaseController
				{
				    public MailLogsController(IHttpContextAccessor httpContextAccessor,IOptions<ConnectionSettings> connectionSettings, ILoggerFactory loggerFactory, IConfiguration configuration,IWebHostEnvironment hostingEnvironment)
				    {
					     _configuration = configuration;
					     _logger = loggerFactory.CreateLogger<MailLogsController>();
					     _connectionSettings = connectionSettings;
					     objMailLogsDAL = new MailLogsDAL(_connectionSettings.Value.ConnectionString,_configuration["approvalurl"]);
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
				private MailLogsDAL objMailLogsDAL;
                private External_System_DAL obj_External_System_DAL;
				private IOptions<ConnectionSettings> _connectionSettings;
				private ILogger _logger;
				private IConfiguration _configuration;
				private IWebHostEnvironment hostingEnv;
                private ExternalSystemUtility objExternalSystemUtitlity;
                private string pvar_tenantid="|";
                private string pvar_usersid="";

			    [AllowAnonymous()]
                                                [HttpGet()]
                                                [ActionName("MailSender")]
                                                public virtual System.Data.DataTable MailSender(string mailfor, string entityid, string createduser)
                                                {

	                                                DataTable dtMailLogs = new DataTable();
	                                                try
	                                                {
		                                                dtMailLogs = objMailLogsDAL.MailSender(mailfor,entityid, createduser);
	                                                }
	                                                catch (Exception ex)
	                                                {
                                                        _logger.LogError(ex,"An exception occurred in - MailSender, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                          
                                                       
	                                                }
	                                                return dtMailLogs;

                                                }
                                                    
                                                 [HttpGet()]
                                                [AllowAnonymous()]
                                                [ActionName("Mailer")]
                                                public virtual mailmodel Mailer(string entityname, string entityactionname, string entityid, string tenantid = "")
                                                {

                                                    mailmodel objmailmodel = new mailmodel();

                                                    try
                                                    {
                                                        objmailmodel = objMailLogsDAL.Mailer(entityname, entityactionname, entityid, tenantid);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                    
                                                        _logger.LogError(ex,"An exception occurred in - Mailer, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
            
                                                    }
                                                    return objmailmodel;

                                                }
                                                [HttpGet()]
                                                [ActionName("WhatsApp")]
                                                public virtual mailmodel WhatsApp(string entityname, string entityactionname, string entityid, string tenantid = "")
                                                {

                                                    mailmodel objmailmodel = new mailmodel();

                                                    try
                                                    {
                                                        objmailmodel = objMailLogsDAL.WhatsApp(entityname, entityactionname, entityid, tenantid);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                      _logger.LogError(ex,"An exception occurred in - WhatsApp, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
                                                    }
                                                    return objmailmodel;

                                                }

                                                
[AllowAnonymous()]
            [HttpPost()]
            [ActionName("Create_MailLog")]
            public virtual IActionResult Create_MailLog([FromBody]MailLogsModel model)
            { 
              string message = "";
              
            try{

            if (ModelState.IsValid)
            {

            	MailLogsModelValidator validator = new MailLogsModelValidator();
            	ValidationResult results = validator.Validate(model);
            	if (!results.IsValid)
            	{
            		var errorCollection = string.Join(" | ", results.Errors.Select(e => e.ErrorMessage.Replace("{propertyName}",e.PropertyName)));
             		message = ("Validation Error : " + errorCollection);


            	}else{
                    
                         message = objMailLogsDAL.Create_MailLog(model);
                        
            	}


            }
            else
            {
            	var errorCollection = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            	message = errorCollection.ToString();

            	_logger.LogError("MailLogsModel - Create_MailLog , Validation Error :" + message);
            	message = ("Validation Error : " + message);
            }






            }catch(Exception ex){
            	message=ex.Message; 
                _logger.LogError(ex,"An exception occurred in - Create_MailLog, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
            	 
            }

                

           if(message.Replace("\"","").Contains("201.1"))
                    return Ok(message);
                    else if(message.Replace("\"","")=="401.1")
                    return Unauthorized(message);
                    else
                    return BadRequest(message);


             }
[HttpGet()]
			  [ActionName("getById_MailLogs")]
			  public virtual MailLogsModel getById_MailLogs(string MailLogsid,string loginUserID="")
			  { 
				    MailLogsModel objMailLogs = new MailLogsModel();
					try
					{
						  objMailLogs = objMailLogsDAL.getById_MailLogs(MailLogsid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_MailLogs, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
            
					}
					return objMailLogs;

			  }
			  [HttpPost()]
			  [ActionName("Update_MailLog")]
			  public virtual IActionResult Update_MailLog([FromBody]MailLogsModel model)
			  { 
				    string message = "";
                   access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Update_MailLog";

					try{

					if (ModelState.IsValid)
					{

						MailLogsModelValidator validator = new MailLogsModelValidator();
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

		       
                                 
		       
                                	
							    message = objMailLogsDAL.Update_MailLog(model);	
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

						_logger.LogError("MailLogsModel - Update_MailLog, Validation Error :" + message);
					
						//return BadRequest("Validation Error : " + message);
					}






					}catch(Exception ex){
                        
						message=ex.Message;
					    _logger.LogError(ex,"An exception occurred in - Update_MailLog, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
           
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
            public virtual async Task<string> Remove_MailLog(string MailLogsid,string loginUserID="")
			{
					string message ="";
                    access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Remove_MailLog";

					try{
						
						  var authHeader = HttpContext.Request.Headers["Authorization"][0];
	                        if (authHeader.StartsWith("Bearer "))
	                        {
		                        
		                      
		                      
		                      var token = authHeader.Substring("Bearer ".Length);
		                         
		                        String[] userdetails=obj_External_System_DAL.get_users_by_token(token);
		                        loginUserID=userdetails[0].ToString();
                                obj_access_logsdetailsModel.access_logsid=new Guid(userdetails[1].ToString());
		       
                                 
                        	 message = objMailLogsDAL.Remove_MailLog(MailLogsid,loginUserID);
						    }
	                        else{
		                        message = "Invalid Token";
		                       
	                        }
					 

					}catch(Exception ex){
						message=ex.Message;
                         _logger.LogError(ex,"An exception occurred in - Remove_MailLog, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                         
					}
                    if(obj_access_logsdetailsModel.access_logsid !=null)
                    {
                         obj_access_logsdetailsModel.api_response=message.Replace("\"",""); 
                         obj_External_System_DAL.create_access_logs_details(obj_access_logsdetailsModel);
                    }
                 
					return message;

			}
[HttpGet()]
			
			[ActionName("MailLogs_List")]
			public virtual JObject MailLogs_List(string entityname=""
,string mailfor=""
,string loginUserID="",int? pagesize=100 , int? pagenumber=0,string searchterm="",string  sort_fields = "")
			{
					 
				  	JObject jsonMailLogs = null;
					try
					{
                        
						jsonMailLogs = objMailLogsDAL.MailLogs_List( entityname
, mailfor
,pagesize,pagenumber,searchterm,sort_fields);
					}
					catch (Exception ex)
					{
                             _logger.LogError(ex,"An exception occurred in - MailLogs_List, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                        
					}
					return jsonMailLogs;

			   }
			   
[HttpGet()]
			
			[ActionName("get_all_MailLogs")]
			public virtual System.Data.DataTable get_all_MailLogs(string tenantid,string searchterm="",string pagesize = "50", string pagenumber = "1",string loginUserID="")
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
				  	DataTable dtMailLogs = new DataTable();
					try
					{
                        tenantid=pvar_tenantid;
						dtMailLogs = objMailLogsDAL.get_all_MailLogs(tenantid,searchterm,pageSize,pageNumber);
					}
					catch (Exception ex)
					{
                          _logger.LogError(ex,"An exception occurred in - get_all_MailLogs, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
					    
					}
					return dtMailLogs;

			   }
[HttpGet()]
			  [ActionName("getById_allinfo_MailLogs")]
			  public virtual System.Data.DataTable getById_allinfo_MailLogs(string MailLogsid)
			  { 
				    DataTable dtMailLogs = new DataTable();
					try
					{
						  dtMailLogs = objMailLogsDAL.getById_allinfo_MailLogs(MailLogsid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_allinfo_MailLogs, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
   
					}
					return dtMailLogs;

			  }






				}


			}
