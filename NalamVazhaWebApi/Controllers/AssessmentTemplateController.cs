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
				//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/23/2026 13:35:11
				public class AssessmentTemplateController : BaseController
				{
				    public AssessmentTemplateController(IHttpContextAccessor httpContextAccessor,IOptions<ConnectionSettings> connectionSettings, ILoggerFactory loggerFactory, IConfiguration configuration,IWebHostEnvironment hostingEnvironment)
				    {
					     _configuration = configuration;
					     _logger = loggerFactory.CreateLogger<AssessmentTemplateController>();
					     _connectionSettings = connectionSettings;
					     objAssessmentTemplateDAL = new AssessmentTemplateDAL(_connectionSettings.Value.ConnectionString);
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
				private AssessmentTemplateDAL objAssessmentTemplateDAL;
                private External_System_DAL obj_External_System_DAL;
				private IOptions<ConnectionSettings> _connectionSettings;
				private ILogger _logger;
				private IConfiguration _configuration;
				private IWebHostEnvironment hostingEnv;
                private ExternalSystemUtility objExternalSystemUtitlity;
                private string pvar_tenantid="|";
                private string pvar_usersid="";

			    [HttpGet()]
			  [ActionName("getById_templatequestions")]
			  public virtual System.Data.DataTable getById_templatequestions(string AssessmentTemplateid)
			  {
					DataTable dtAssessmentTemplate_templatequestions = new DataTable();
					try
					{
						dtAssessmentTemplate_templatequestions = objAssessmentTemplateDAL.getById_templatequestions(AssessmentTemplateid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_templatequestions, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
    
					}
					return dtAssessmentTemplate_templatequestions;


			  }

[HttpGet()]
			  [ActionName("getById_templateapplicability")]
			  public virtual System.Data.DataTable getById_templateapplicability(string AssessmentTemplateid)
			  {
					DataTable dtAssessmentTemplate_templateapplicability = new DataTable();
					try
					{
						dtAssessmentTemplate_templateapplicability = objAssessmentTemplateDAL.getById_templateapplicability(AssessmentTemplateid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_templateapplicability, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
    
					}
					return dtAssessmentTemplate_templateapplicability;


			  }


            [HttpPost()]
            [ActionName("Add_Assessment_Template")]
            public virtual IActionResult Add_Assessment_Template([FromBody]AssessmentTemplateModel model)
            { 
              string message = "";
                
                access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Add_Assessment_Template";
            try{

            if (ModelState.IsValid)
            {

            	AssessmentTemplateModelValidator validator = new AssessmentTemplateModelValidator();
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

		       
                                 

                                
            		                
                                     message = objAssessmentTemplateDAL.Add_Assessment_Template(model);
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

            	_logger.LogError("AssessmentTemplateModel - Add_Assessment_Template , Validation Error :" + message);
            	message = ("Validation Error : " + message);
            }






            }catch(Exception ex){
               message=ex.Message;
               _logger.LogError(ex,"An exception occurred in - Add_Assessment_Template, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
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
			  [ActionName("getById_AssessmentTemplate")]
			  public virtual AssessmentTemplateModel getById_AssessmentTemplate(string AssessmentTemplateid,string loginUserID="")
			  { 
				    AssessmentTemplateModel objAssessmentTemplate = new AssessmentTemplateModel();
					try
					{
						  objAssessmentTemplate = objAssessmentTemplateDAL.getById_AssessmentTemplate(AssessmentTemplateid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_AssessmentTemplate, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
            
					}
					return objAssessmentTemplate;

			  }
			  [HttpPost()]
			  [ActionName("Update_Assessment_Template")]
			  public virtual IActionResult Update_Assessment_Template([FromBody]AssessmentTemplateModel model)
			  { 
				    string message = "";
                   access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Update_Assessment_Template";

					try{

					if (ModelState.IsValid)
					{

						AssessmentTemplateModelValidator validator = new AssessmentTemplateModelValidator();
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

		       
                                 
		       
                                	
							    message = objAssessmentTemplateDAL.Update_Assessment_Template(model);	
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

						_logger.LogError("AssessmentTemplateModel - Update_Assessment_Template, Validation Error :" + message);
					
						//return BadRequest("Validation Error : " + message);
					}






					}catch(Exception ex){
                        
						message=ex.Message;
					    _logger.LogError(ex,"An exception occurred in - Update_Assessment_Template, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
           
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
            public virtual async Task<string> Remove_Assessment_Template(string AssessmentTemplateid,string loginUserID="")
			{
					string message ="";
                    access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Remove_Assessment_Template";

					try{
						
						  var authHeader = HttpContext.Request.Headers["Authorization"][0];
	                        if (authHeader.StartsWith("Bearer "))
	                        {
		                        
		                      
		                      
		                      var token = authHeader.Substring("Bearer ".Length);
		                         
		                        String[] userdetails=obj_External_System_DAL.get_users_by_token(token);
		                        loginUserID=userdetails[0].ToString();
                                obj_access_logsdetailsModel.access_logsid=new Guid(userdetails[1].ToString());
		       
                                 
                        	 message = objAssessmentTemplateDAL.Remove_Assessment_Template(AssessmentTemplateid,loginUserID);
						    }
	                        else{
		                        message = "Invalid Token";
		                       
	                        }
					 

					}catch(Exception ex){
						message=ex.Message;
                         _logger.LogError(ex,"An exception occurred in - Remove_Assessment_Template, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                         
					}
                    if(obj_access_logsdetailsModel.access_logsid !=null)
                    {
                         obj_access_logsdetailsModel.api_response=message.Replace("\"",""); 
                         obj_External_System_DAL.create_access_logs_details(obj_access_logsdetailsModel);
                    }
                 
					return message;

			}
[HttpGet()]
			
			[ActionName("Assessment_Template_List")]
			public virtual System.Data.DataTable Assessment_Template_List(string tenantid,string taskname
)
			{
					 
				  	DataTable dtAssessmentTemplate = new DataTable();
					try
					{
                        tenantid=pvar_tenantid;
						dtAssessmentTemplate = objAssessmentTemplateDAL.Assessment_Template_List( tenantid, taskname
);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - Assessment_Template_List, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                       
					}
					return dtAssessmentTemplate;

			   }
			   
[HttpGet()]
			
			[ActionName("get_all_AssessmentTemplate")]
			public virtual System.Data.DataTable get_all_AssessmentTemplate(string tenantid,string loginUserID="")
			{
					 
				  	DataTable dtAssessmentTemplate = new DataTable();
					try
					{
                        tenantid=pvar_tenantid;
						dtAssessmentTemplate = objAssessmentTemplateDAL.get_all_AssessmentTemplate(tenantid);
					}
					catch (Exception ex)
					{
                        _logger.LogError(ex,"An exception occurred in - get_all_AssessmentTemplate, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
					   
					}
					return dtAssessmentTemplate;

			   }
[HttpGet()]
			  [ActionName("getById_allinfo_AssessmentTemplate")]
			  public virtual System.Data.DataTable getById_allinfo_AssessmentTemplate(string AssessmentTemplateid)
			  { 
				    DataTable dtAssessmentTemplate = new DataTable();
					try
					{
						  dtAssessmentTemplate = objAssessmentTemplateDAL.getById_allinfo_AssessmentTemplate(AssessmentTemplateid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_allinfo_AssessmentTemplate, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
   
					}
					return dtAssessmentTemplate;

			  }
[HttpGet()]
							[ActionName("lookup_AssessmentTemplate_templatequestions_questioncategory")]
							public virtual System.Data.DataTable lookup_AssessmentTemplate_templatequestions_questioncategory(String tenantid,string loginUserID="")
							{
									DataTable dtAssessmentTemplate = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtAssessmentTemplate = objAssessmentTemplateDAL.lookup_AssessmentTemplate_templatequestions_questioncategory(tenantid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_AssessmentTemplate_templatequestions_questioncategory, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtAssessmentTemplate;
							}
[HttpGet()]
							[ActionName("lookup_AssessmentTemplate_templatequestions_questionsub")]
							public virtual System.Data.DataTable lookup_AssessmentTemplate_templatequestions_questionsub(String tenantid,String questioncategory,string loginUserID="")
							{
									DataTable dtAssessmentTemplate = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtAssessmentTemplate = objAssessmentTemplateDAL.lookup_AssessmentTemplate_templatequestions_questionsub(tenantid,questioncategory);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_AssessmentTemplate_templatequestions_questionsub, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtAssessmentTemplate;
							}
[HttpGet()]
							[ActionName("lookup_AssessmentTemplate_templatequestions_question")]
							public virtual System.Data.DataTable lookup_AssessmentTemplate_templatequestions_question(String tenantid,String questioncategory,String questionsub,string loginUserID="")
							{
									DataTable dtAssessmentTemplate = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtAssessmentTemplate = objAssessmentTemplateDAL.lookup_AssessmentTemplate_templatequestions_question(tenantid,questioncategory,questionsub);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_AssessmentTemplate_templatequestions_question, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtAssessmentTemplate;
							}
[HttpGet()]
							[ActionName("lookup_AssessmentTemplate_templateapplicability_patientcategory")]
							public virtual System.Data.DataTable lookup_AssessmentTemplate_templateapplicability_patientcategory(String tenantid,string loginUserID="")
							{
									DataTable dtAssessmentTemplate = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtAssessmentTemplate = objAssessmentTemplateDAL.lookup_AssessmentTemplate_templateapplicability_patientcategory(tenantid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_AssessmentTemplate_templateapplicability_patientcategory, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtAssessmentTemplate;
							}
[HttpGet()]
							[ActionName("lookup_AssessmentTemplate_templateapplicability_medicalcondition")]
							public virtual System.Data.DataTable lookup_AssessmentTemplate_templateapplicability_medicalcondition(string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtAssessmentTemplate = new DataTable();
									try
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
										dtAssessmentTemplate = objAssessmentTemplateDAL.lookup_AssessmentTemplate_templateapplicability_medicalcondition(searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_AssessmentTemplate_templateapplicability_medicalcondition, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtAssessmentTemplate;
							}


[HttpGet()]
							[ActionName("lookup_change_templatequestions_AssessmentTemplate_question")]
							public virtual System.Data.DataTable lookup_change_templatequestions_AssessmentTemplate_question(string AssessmentQuestionid,string loginUserID="")
							{
									DataTable dtAssessmentQuestion = new DataTable();
									try
									{
										dtAssessmentQuestion = objAssessmentTemplateDAL.lookup_change_templatequestions_AssessmentTemplate_question(AssessmentQuestionid);
									}
									catch (Exception ex)
									{
									  _logger.LogError(ex,"An exception occurred in - lookup_change_templatequestions_AssessmentTemplate_question, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtAssessmentQuestion;
							}

		[HttpGet()]
		[ActionName("lookup_AssessmentTemplate_taskname")]
		public virtual System.Data.DataTable lookup_AssessmentTemplate_taskname(String tenantid, string loginUserID = "")
		{
			DataTable dtAssessmentTemplate = new DataTable();
			try
			{
				tenantid = pvar_tenantid;

				dtAssessmentTemplate = objAssessmentTemplateDAL.lookup_AssessmentTemplate_taskname(tenantid);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - lookup_AssessmentTemplate_taskname, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

			}
			return dtAssessmentTemplate;
		}



	}


}
