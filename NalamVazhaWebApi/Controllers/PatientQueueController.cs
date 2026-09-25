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
				//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/30/2026 16:15:06
				public class PatientQueueController : BaseController
				{
				    public PatientQueueController(IHttpContextAccessor httpContextAccessor,IOptions<ConnectionSettings> connectionSettings, ILoggerFactory loggerFactory, IConfiguration configuration,IWebHostEnvironment hostingEnvironment)
				    {
					     _configuration = configuration;
					     _logger = loggerFactory.CreateLogger<PatientQueueController>();
					     _connectionSettings = connectionSettings;
					     objPatientQueueDAL = new PatientQueueDAL(_connectionSettings.Value.ConnectionString);
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
				private PatientQueueDAL objPatientQueueDAL;
                private External_System_DAL obj_External_System_DAL;
				private IOptions<ConnectionSettings> _connectionSettings;
				private ILogger _logger;
				private IConfiguration _configuration;
				private IWebHostEnvironment hostingEnv;
                private ExternalSystemUtility objExternalSystemUtitlity;
                private string pvar_tenantid="|";
                private string pvar_usersid="";

			    
            [HttpPost()]
            [ActionName("Add_Patient_Queue")]
            public virtual IActionResult Add_Patient_Queue([FromBody]PatientQueueModel model)
            { 
              string message = "";
                
                access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Add_Patient_Queue";
            try{

            if (ModelState.IsValid)
            {

            	PatientQueueModelValidator validator = new PatientQueueModelValidator();
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

		       
                                 

                                
            		                
                                     message = objPatientQueueDAL.Add_Patient_Queue(model);
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

            	_logger.LogError("PatientQueueModel - Add_Patient_Queue , Validation Error :" + message);
            	message = ("Validation Error : " + message);
            }






            }catch(Exception ex){
               message=ex.Message;
               _logger.LogError(ex,"An exception occurred in - Add_Patient_Queue, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
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
			  [ActionName("getById_PatientQueue")]
			  public virtual PatientQueueModel getById_PatientQueue(string PatientQueueid,string loginUserID="")
			  { 
				    PatientQueueModel objPatientQueue = new PatientQueueModel();
					try
					{
						  objPatientQueue = objPatientQueueDAL.getById_PatientQueue(PatientQueueid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_PatientQueue, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
            
					}
					return objPatientQueue;

			  }
			  [HttpPost()]
			  [ActionName("Update_Patient_Queue")]
			  public virtual IActionResult Update_Patient_Queue([FromBody]PatientQueueModel model)
			  { 
				    string message = "";
                   access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Update_Patient_Queue";

					try{

					if (ModelState.IsValid)
					{

						PatientQueueModelValidator validator = new PatientQueueModelValidator();
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

		       
                                 
		       
                                	
							    message = objPatientQueueDAL.Update_Patient_Queue(model);	
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

						_logger.LogError("PatientQueueModel - Update_Patient_Queue, Validation Error :" + message);
					
						//return BadRequest("Validation Error : " + message);
					}






					}catch(Exception ex){
                        
						message=ex.Message;
					    _logger.LogError(ex,"An exception occurred in - Update_Patient_Queue, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
           
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
            public virtual async Task<string> Remove_Patient_Queue(string PatientQueueid,string loginUserID="")
			{
					string message ="";
                    access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Remove_Patient_Queue";

					try{
						
						  var authHeader = HttpContext.Request.Headers["Authorization"][0];
	                        if (authHeader.StartsWith("Bearer "))
	                        {
		                        
		                      
		                      
		                      var token = authHeader.Substring("Bearer ".Length);
		                         
		                        String[] userdetails=obj_External_System_DAL.get_users_by_token(token);
		                        loginUserID=userdetails[0].ToString();
                                obj_access_logsdetailsModel.access_logsid=new Guid(userdetails[1].ToString());
		       
                                 
                        	 message = objPatientQueueDAL.Remove_Patient_Queue(PatientQueueid,loginUserID);
						    }
	                        else{
		                        message = "Invalid Token";
		                       
	                        }
					 

					}catch(Exception ex){
						message=ex.Message;
                         _logger.LogError(ex,"An exception occurred in - Remove_Patient_Queue, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                         
					}
                    if(obj_access_logsdetailsModel.access_logsid !=null)
                    {
                         obj_access_logsdetailsModel.api_response=message.Replace("\"",""); 
                         obj_External_System_DAL.create_access_logs_details(obj_access_logsdetailsModel);
                    }
                 
					return message;

			}
[HttpGet()]
			
			[ActionName("Patient_Queue_List")]
			public virtual JObject Patient_Queue_List(string tenantid
,string patientvisit=""
,string visitdatetime_automatonfrom=""
,string visitdatetime_automatonto=""
,string patientname=""
,string queuename=""
,string queuestatus=""
,string loginUserID="",int? pagesize=100 , int? pagenumber=0,string searchterm="",string  sort_fields = "")
			{
					 
				  	JObject jsonPatientQueue = null;
					try
					{
                        tenantid=pvar_tenantid;
						jsonPatientQueue = objPatientQueueDAL.Patient_Queue_List( tenantid
, patientvisit
, visitdatetime_automatonfrom
, visitdatetime_automatonto
, patientname
, queuename
, queuestatus
,pagesize,pagenumber,searchterm,sort_fields);
					}
					catch (Exception ex)
					{
                             _logger.LogError(ex,"An exception occurred in - Patient_Queue_List, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                        
					}
					return jsonPatientQueue;

			   }
			   
[HttpGet()]
			
			[ActionName("get_all_PatientQueue")]
			public virtual System.Data.DataTable get_all_PatientQueue(string tenantid,string searchterm="",string pagesize = "50", string pagenumber = "1",string loginUserID="")
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
				  	DataTable dtPatientQueue = new DataTable();
					try
					{
                        tenantid=pvar_tenantid;
						dtPatientQueue = objPatientQueueDAL.get_all_PatientQueue(tenantid,searchterm,pageSize,pageNumber);
					}
					catch (Exception ex)
					{
                          _logger.LogError(ex,"An exception occurred in - get_all_PatientQueue, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
					    
					}
					return dtPatientQueue;

			   }
[HttpGet()]
			
			[ActionName("count_of_PatientQueue_queuestatus")]
			public virtual System.Data.DataTable count_of_PatientQueue_queuestatus(string tenantid
,string patientvisit=""
,string visitdatetime_automatonfrom=""
,string visitdatetime_automatonto=""
,string patientname=""
,string queuename=""
,string queuestatus=""
)
			{
					 
				  	DataTable dtPatientQueue = new DataTable();
					try
					{
                        tenantid=pvar_tenantid;
						dtPatientQueue = objPatientQueueDAL.count_of_PatientQueue_queuestatus( tenantid
, patientvisit
, visitdatetime_automatonfrom
, visitdatetime_automatonto
, patientname
, queuename
, queuestatus
);
					}
					catch (Exception ex)
					{
                          _logger.LogError(ex,"An exception occurred in - count_of_PatientQueue_queuestatus, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                         
					}
					return dtPatientQueue;

			   }
			   
[HttpGet()]
			  [ActionName("getById_allinfo_PatientQueue")]
			  public virtual System.Data.DataTable getById_allinfo_PatientQueue(string PatientQueueid)
			  { 
				    DataTable dtPatientQueue = new DataTable();
					try
					{
						  dtPatientQueue = objPatientQueueDAL.getById_allinfo_PatientQueue(PatientQueueid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_allinfo_PatientQueue, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
   
					}
					return dtPatientQueue;

			  }
[HttpGet()]
							[ActionName("lookup_PatientQueue_patientvisit")]
							public virtual System.Data.DataTable lookup_PatientQueue_patientvisit(String tenantid,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtPatientQueue = new DataTable();
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
										dtPatientQueue = objPatientQueueDAL.lookup_PatientQueue_patientvisit(tenantid,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_PatientQueue_patientvisit, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtPatientQueue;
							}
[HttpGet()]
							[ActionName("lookup_PatientQueue_patientname")]
							public virtual System.Data.DataTable lookup_PatientQueue_patientname(String tenantid,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtPatientQueue = new DataTable();
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
										dtPatientQueue = objPatientQueueDAL.lookup_PatientQueue_patientname(tenantid,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_PatientQueue_patientname, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtPatientQueue;
							}
[HttpGet()]
							[ActionName("lookup_PatientQueue_queuename")]
							public virtual System.Data.DataTable lookup_PatientQueue_queuename(String tenantid,string loginUserID="")
							{
									DataTable dtPatientQueue = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtPatientQueue = objPatientQueueDAL.lookup_PatientQueue_queuename(tenantid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_PatientQueue_queuename, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtPatientQueue;
							}

[HttpGet()]
							[ActionName("lookup_change_PatientQueue_patientvisit")]
							public virtual System.Data.DataTable lookup_change_PatientQueue_patientvisit(string PatientVisitid,string loginUserID="")
							{
									DataTable dtPatientVisit = new DataTable();
									try
									{
										dtPatientVisit = objPatientQueueDAL.lookup_change_PatientQueue_patientvisit(PatientVisitid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_change_PatientQueue_patientvisit, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtPatientVisit;
							}





				}


			}
