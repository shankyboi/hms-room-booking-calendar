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
				//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/23/2026 13:27:20
				public class AssessmentQuestionController : BaseController
				{
				    public AssessmentQuestionController(IHttpContextAccessor httpContextAccessor,IOptions<ConnectionSettings> connectionSettings, ILoggerFactory loggerFactory, IConfiguration configuration,IWebHostEnvironment hostingEnvironment)
				    {
					     _configuration = configuration;
					     _logger = loggerFactory.CreateLogger<AssessmentQuestionController>();
					     _connectionSettings = connectionSettings;
					     objAssessmentQuestionDAL = new AssessmentQuestionDAL(_connectionSettings.Value.ConnectionString);
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
				private AssessmentQuestionDAL objAssessmentQuestionDAL;
                private External_System_DAL obj_External_System_DAL;
				private IOptions<ConnectionSettings> _connectionSettings;
				private ILogger _logger;
				private IConfiguration _configuration;
				private IWebHostEnvironment hostingEnv;
                private ExternalSystemUtility objExternalSystemUtitlity;
                private string pvar_tenantid="|";
                private string pvar_usersid="";

			    
            [HttpPost()]
            [ActionName("Add_Assessment_Question")]
            public virtual IActionResult Add_Assessment_Question([FromBody]AssessmentQuestionModel model)
            { 
              string message = "";
                
                access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Add_Assessment_Question";
            try{

            if (ModelState.IsValid)
            {

            	AssessmentQuestionModelValidator validator = new AssessmentQuestionModelValidator();
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

		       
                                 

                                
            		                
                                     message = objAssessmentQuestionDAL.Add_Assessment_Question(model);
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

            	_logger.LogError("AssessmentQuestionModel - Add_Assessment_Question , Validation Error :" + message);
            	message = ("Validation Error : " + message);
            }






            }catch(Exception ex){
               message=ex.Message;
               _logger.LogError(ex,"An exception occurred in - Add_Assessment_Question, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
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
			  [ActionName("getById_AssessmentQuestion")]
			  public virtual AssessmentQuestionModel getById_AssessmentQuestion(string AssessmentQuestionid,string loginUserID="")
			  { 
				    AssessmentQuestionModel objAssessmentQuestion = new AssessmentQuestionModel();
					try
					{
						  objAssessmentQuestion = objAssessmentQuestionDAL.getById_AssessmentQuestion(AssessmentQuestionid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_AssessmentQuestion, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
            
					}
					return objAssessmentQuestion;

			  }
			  [HttpPost()]
			  [ActionName("Update_Assessment_Question")]
			  public virtual IActionResult Update_Assessment_Question([FromBody]AssessmentQuestionModel model)
			  { 
				    string message = "";
                   access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Update_Assessment_Question";

					try{

					if (ModelState.IsValid)
					{

						AssessmentQuestionModelValidator validator = new AssessmentQuestionModelValidator();
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

		       
                                 
		       
                                	
							    message = objAssessmentQuestionDAL.Update_Assessment_Question(model);	
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

						_logger.LogError("AssessmentQuestionModel - Update_Assessment_Question, Validation Error :" + message);
					
						//return BadRequest("Validation Error : " + message);
					}






					}catch(Exception ex){
                        
						message=ex.Message;
					    _logger.LogError(ex,"An exception occurred in - Update_Assessment_Question, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
           
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
            public virtual async Task<string> Remove_Assessment_Question(string AssessmentQuestionid,string loginUserID="")
			{
					string message ="";
                    access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Remove_Assessment_Question";

					try{
						
						  var authHeader = HttpContext.Request.Headers["Authorization"][0];
	                        if (authHeader.StartsWith("Bearer "))
	                        {
		                        
		                      
		                      
		                      var token = authHeader.Substring("Bearer ".Length);
		                         
		                        String[] userdetails=obj_External_System_DAL.get_users_by_token(token);
		                        loginUserID=userdetails[0].ToString();
                                obj_access_logsdetailsModel.access_logsid=new Guid(userdetails[1].ToString());
		       
                                 
                        	 message = objAssessmentQuestionDAL.Remove_Assessment_Question(AssessmentQuestionid,loginUserID);
						    }
	                        else{
		                        message = "Invalid Token";
		                       
	                        }
					 

					}catch(Exception ex){
						message=ex.Message;
                         _logger.LogError(ex,"An exception occurred in - Remove_Assessment_Question, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                         
					}
                    if(obj_access_logsdetailsModel.access_logsid !=null)
                    {
                         obj_access_logsdetailsModel.api_response=message.Replace("\"",""); 
                         obj_External_System_DAL.create_access_logs_details(obj_access_logsdetailsModel);
                    }
                 
					return message;

			}
[HttpGet()]
			
			[ActionName("Assessment_Question_List")]
			public virtual System.Data.DataTable Assessment_Question_List(string tenantid
,string questioncategory=""
,string questionsubcategory=""
)
			{
					 
				  	DataTable dtAssessmentQuestion = new DataTable();
					try
					{
                        tenantid=pvar_tenantid;
						dtAssessmentQuestion = objAssessmentQuestionDAL.Assessment_Question_List( tenantid
, questioncategory
, questionsubcategory
);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - Assessment_Question_List, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                       
					}
					return dtAssessmentQuestion;

			   }
			   
[HttpGet()]
			
			[ActionName("get_all_AssessmentQuestion")]
			public virtual System.Data.DataTable get_all_AssessmentQuestion(string tenantid,string loginUserID="")
			{
					 
				  	DataTable dtAssessmentQuestion = new DataTable();
					try
					{
                        tenantid=pvar_tenantid;
						dtAssessmentQuestion = objAssessmentQuestionDAL.get_all_AssessmentQuestion(tenantid);
					}
					catch (Exception ex)
					{
                        _logger.LogError(ex,"An exception occurred in - get_all_AssessmentQuestion, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
					   
					}
					return dtAssessmentQuestion;

			   }
[HttpGet()]
			  [ActionName("getById_allinfo_AssessmentQuestion")]
			  public virtual System.Data.DataTable getById_allinfo_AssessmentQuestion(string AssessmentQuestionid)
			  { 
				    DataTable dtAssessmentQuestion = new DataTable();
					try
					{
						  dtAssessmentQuestion = objAssessmentQuestionDAL.getById_allinfo_AssessmentQuestion(AssessmentQuestionid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_allinfo_AssessmentQuestion, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
   
					}
					return dtAssessmentQuestion;

			  }
[HttpGet()]
							[ActionName("lookup_AssessmentQuestion_questioncategory")]
							public virtual System.Data.DataTable lookup_AssessmentQuestion_questioncategory(String tenantid,string loginUserID="")
							{
									DataTable dtAssessmentQuestion = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtAssessmentQuestion = objAssessmentQuestionDAL.lookup_AssessmentQuestion_questioncategory(tenantid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_AssessmentQuestion_questioncategory, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtAssessmentQuestion;
							}
[HttpGet()]
							[ActionName("lookup_AssessmentQuestion_questionsubcategory")]
							public virtual System.Data.DataTable lookup_AssessmentQuestion_questionsubcategory(String tenantid,String questioncategory,string loginUserID="")
							{
									DataTable dtAssessmentQuestion = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtAssessmentQuestion = objAssessmentQuestionDAL.lookup_AssessmentQuestion_questionsubcategory(tenantid,questioncategory);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_AssessmentQuestion_questionsubcategory, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtAssessmentQuestion;
							}






				}


			}
