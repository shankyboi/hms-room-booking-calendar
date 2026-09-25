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
				//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/23/2026 16:41:43
				public class AssessmentController : BaseController
				{
				    public AssessmentController(IHttpContextAccessor httpContextAccessor,IOptions<ConnectionSettings> connectionSettings, ILoggerFactory loggerFactory, IConfiguration configuration,IWebHostEnvironment hostingEnvironment)
				    {
					     _configuration = configuration;
					     _logger = loggerFactory.CreateLogger<AssessmentController>();
					     _connectionSettings = connectionSettings;
					     objAssessmentDAL = new AssessmentDAL(_connectionSettings.Value.ConnectionString);
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
				private AssessmentDAL objAssessmentDAL;
                private External_System_DAL obj_External_System_DAL;
				private IOptions<ConnectionSettings> _connectionSettings;
				private ILogger _logger;
				private IConfiguration _configuration;
				private IWebHostEnvironment hostingEnv;
                private ExternalSystemUtility objExternalSystemUtitlity;
                private string pvar_tenantid="|";
                private string pvar_usersid="";

                private Guid? ResolveAssessmentOPDForm(AssessmentModel model)
                {
                    if (model?.opdform.HasValue == true && model.opdform.Value != Guid.Empty)
                        return model.opdform;

                    if (model?.Assessmentid.HasValue != true || model.Assessmentid.Value == Guid.Empty)
                        return null;

                    var savedAssessment = objAssessmentDAL.getById_Assessment(model.Assessmentid.Value.ToString());
                    return savedAssessment?.opdform.HasValue == true
                        && savedAssessment.opdform.Value != Guid.Empty
                            ? savedAssessment.opdform
                            : null;
                }

				private static bool IsAssessmentDraftSaveRequested(AssessmentModel model)
				{
					var action = model?.eligibleforfinaladmission?.Trim();
					return string.Equals(action, "patient-saveasdraft", StringComparison.OrdinalIgnoreCase)
						|| string.Equals(action, "doctor-saveasdraft", StringComparison.OrdinalIgnoreCase)
						|| string.Equals(action, "fdesk-saveasdraft", StringComparison.OrdinalIgnoreCase);
				}

			    [HttpGet()]
			  [ActionName("getById_assessmentquestions")]
			  public virtual System.Data.DataTable getById_assessmentquestions(string Assessmentid)
			  {
					DataTable dtAssessment_assessmentquestions = new DataTable();
					try
					{
						dtAssessment_assessmentquestions = objAssessmentDAL.getById_assessmentquestions(Assessmentid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_assessmentquestions, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
    
					}
					return dtAssessment_assessmentquestions;


			  }
[HttpGet()]
			  [ActionName("prefill_Assessment_assessmentquestions")]
              public virtual System.Data.DataTable prefill_Assessment_assessmentquestions(string questionnairetemplate)
			 {
					 DataTable dtPrefill = new DataTable();
					try
					{
						dtPrefill = objAssessmentDAL.prefill_Assessment_assessmentquestions(questionnairetemplate);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_prefill_Assessment_assessmentquestions, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
     
					}
					return dtPrefill;


			 }

            // ── Patient Answers endpoints ────────────────────────────────────────────
            [HttpPost()]
            [ActionName("Save_patientanswers")]
            public virtual IActionResult Save_patientanswers([FromBody] JObject body)
            {
                string message = "";
                try
                {
                    string assessmentid = body["assessmentid"]?.ToString();
                    string answers_json = body["answers_json"]?.ToString();
                    if (string.IsNullOrEmpty(assessmentid))
                        return BadRequest("assessmentid is required");
                    message = objAssessmentDAL.Save_patientanswers(assessmentid, answers_json);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Save_patientanswers error: " + ex.Message);
                    return BadRequest(ex.Message);
                }
                return Ok(message);
            }

            [HttpGet()]
            [ActionName("getById_patientanswers")]
            public virtual System.Data.DataTable getById_patientanswers(string Assessmentid)
            {
                DataTable dt = new DataTable();
                try { dt = objAssessmentDAL.getById_patientanswers(Assessmentid); }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "getById_patientanswers error: " + ex.Message);
                }
                return dt;
            }

            [HttpGet()]
            [ActionName("getLatestPreviousPatientAnswers")]
            public virtual System.Data.DataTable getLatestPreviousPatientAnswers(
                string patientname, string questionnairetemplate, string taskname = "", string opdform = "", string ipdform = "")
            {
                try
                {
                    return objAssessmentDAL.getLatestPreviousPatientAnswers(
                        patientname, questionnairetemplate, taskname, opdform, ipdform);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unable to load previous assessment answers for patient {Patient}", patientname);
                    return new DataTable();
                }
            }

            [HttpGet()]
            [ActionName("getLatestAssessmentForPrefill")]
            public virtual System.Data.DataTable getLatestAssessmentForPrefill(
                string patientname, string taskname = "", string opdform = "", string ipdform = "")
            {
                try
                {
                    return objAssessmentDAL.getLatestAssessmentForPrefill(
                        patientname, taskname, opdform, ipdform);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unable to load latest assessment prefill for patient {Patient}", patientname);
                    return new DataTable();
                }
            }

            [HttpGet()]
            [ActionName("getAssessments_by_ipdform")]
            public virtual System.Data.DataTable getAssessments_by_ipdform(string ipdapplicationformid)
            {
                DataTable dt = new DataTable();
                try { dt = objAssessmentDAL.getAssessments_by_ipdform(ipdapplicationformid); }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "getAssessments_by_ipdform error: " + ex.Message);
                }
                return dt;
            }

			[HttpGet]
			[ActionName("Get_IPD_Assessment_Task_State")]
			public virtual System.Data.DataTable Get_IPD_Assessment_Task_State(string ipdapplicationformid)
			{
				if (!Guid.TryParse(ipdapplicationformid, out _)) return new System.Data.DataTable();
				return objAssessmentDAL.Get_IPD_Assessment_Task_State(ipdapplicationformid);
			}

            [HttpGet()]
            [ActionName("GetAssessmentHistory")]
            public virtual System.Data.DataTable GetAssessmentHistory(
                string assessmentid = "",
                string ipdformid = "",
                string opdformid = "",
                string loginUserID = "")
            {
                try
                {
                    Guid parsed;
                    Guid? assessmentId = Guid.TryParse(assessmentid, out parsed) ? parsed : (Guid?)null;
                    Guid? ipdFormId = Guid.TryParse(ipdformid, out parsed) ? parsed : (Guid?)null;
                    Guid? opdFormId = Guid.TryParse(opdformid, out parsed) ? parsed : (Guid?)null;
                    if (!assessmentId.HasValue && !ipdFormId.HasValue && !opdFormId.HasValue)
                        return new DataTable();
                    return objAssessmentDAL.GetAssessmentHistory(assessmentId, ipdFormId, opdFormId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "GetAssessmentHistory failed.");
                    return new DataTable();
                }
            }

            [HttpGet()]
            [ActionName("get_Assessments_for_IPD")]
            public virtual System.Data.DataTable get_Assessments_for_IPD(string ipdapplicationformid)
            {
                DataTable dt = new DataTable();
                try { dt = objAssessmentDAL.get_Assessments_for_IPD(ipdapplicationformid); }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "get_Assessments_for_IPD error: " + ex.Message);
                }
                return dt;
            }
            // ────────────────────────────────────────────────────────────────────────

            [HttpPost()]
            [ActionName("Add_Assessment")]
            public virtual IActionResult Add_Assessment([FromBody]AssessmentModel model)
            { 
              string message = "";
                
                access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Add_Assessment";
            try{

            if (ModelState.IsValid)
            {

            	AssessmentModelValidator validator = new AssessmentModelValidator();
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

		       
                                 

                                
            		                
									 message = objAssessmentDAL.Add_Assessment(model);
									 if (message.Replace("\"", "").Contains("201.1"))
									 {
										 var opdFormId = ResolveAssessmentOPDForm(model);
										 if (opdFormId.HasValue)
											 message = IsAssessmentDraftSaveRequested(model)
												 ? objAssessmentDAL.SaveOPDAssessmentDraftStatus(opdFormId, model.createduser.Value)
												 : objAssessmentDAL.AdvanceOPDAssessmentStatus(opdFormId, model.createduser.Value);
									 }
									 else
                                     {
                                         _logger.LogError(
                                             "Add_Assessment database operation failed for assessment {AssessmentId}, OPD {OPDFormId}: {Response}",
                                             model.Assessmentid, model.opdform, message);
                                     }
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

            	_logger.LogError("AssessmentModel - Add_Assessment , Validation Error :" + message);
            	message = ("Validation Error : " + message);
            }






            }catch(Exception ex){
               message=ex.Message;
               _logger.LogError(ex,"An exception occurred in - Add_Assessment, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
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
			  [ActionName("getAssessmentByOpdForm")]
			  public virtual string getAssessmentByOpdForm(string opdformid, string loginUserID = "")
			  {
					try { return objAssessmentDAL.getAssessmentByOpdForm(opdformid); }
					catch (Exception ex)
					{
						_logger.LogError(ex, "An exception occurred in - getAssessmentByOpdForm: " + ex.Message);
						return "";
					}
			  }

[HttpGet()]
			  [ActionName("getById_Assessment")]
			  public virtual AssessmentModel getById_Assessment(string Assessmentid,string loginUserID="")
			  { 
				    AssessmentModel objAssessment = new AssessmentModel();
					try
					{
						  objAssessment = objAssessmentDAL.getById_Assessment(Assessmentid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_Assessment, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
            
					}
					return objAssessment;

			  }
			  [HttpPost()]
			  [ActionName("Update_Assessment")]
			  public virtual IActionResult Update_Assessment([FromBody]AssessmentModel model)
			  { 
				    string message = "";
                   access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Update_Assessment";

					try{

					if (ModelState.IsValid)
					{

						AssessmentModelValidator validator = new AssessmentModelValidator();
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
								var opdFormId = ResolveAssessmentOPDForm(model);
								model.opdform = opdFormId;
								if (opdFormId.HasValue
									&& !objAssessmentDAL.CanSaveOPDAssessment(opdFormId, model.modifieduser.Value))
								{
									message = "You are not authorized to review this OPD assessment at its current stage.";
								}
								else
								{

		       
                                 
		       
                                	
							    message = objAssessmentDAL.Update_Assessment(model);
								    if (message.Replace("\"", "").Contains("201.1") && opdFormId.HasValue)
								        message = IsAssessmentDraftSaveRequested(model)
											? objAssessmentDAL.SaveOPDAssessmentDraftStatus(opdFormId, model.modifieduser.Value)
											: objAssessmentDAL.AdvanceOPDAssessmentStatus(opdFormId, model.modifieduser.Value);
								}
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

						_logger.LogError("AssessmentModel - Update_Assessment, Validation Error :" + message);
					
						//return BadRequest("Validation Error : " + message);
					}






					}catch(Exception ex){
                        
						message=ex.Message;
					    _logger.LogError(ex,"An exception occurred in - Update_Assessment, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
           
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
            public virtual async Task<string> Remove_Assessment(string Assessmentid,string loginUserID="")
			{
					string message ="";
                    access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Remove_Assessment";

					try{
						
						  var authHeader = HttpContext.Request.Headers["Authorization"][0];
	                        if (authHeader.StartsWith("Bearer "))
	                        {
		                        
		                      
		                      
		                      var token = authHeader.Substring("Bearer ".Length);
		                         
		                        String[] userdetails=obj_External_System_DAL.get_users_by_token(token);
		                        loginUserID=userdetails[0].ToString();
                                obj_access_logsdetailsModel.access_logsid=new Guid(userdetails[1].ToString());
		       
                                 
                        	 message = objAssessmentDAL.Remove_Assessment(Assessmentid,loginUserID);
						    }
	                        else{
		                        message = "Invalid Token";
		                       
	                        }
					 

					}catch(Exception ex){
						message=ex.Message;
                         _logger.LogError(ex,"An exception occurred in - Remove_Assessment, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                         
					}
                    if(obj_access_logsdetailsModel.access_logsid !=null)
                    {
                         obj_access_logsdetailsModel.api_response=message.Replace("\"",""); 
                         obj_External_System_DAL.create_access_logs_details(obj_access_logsdetailsModel);
                    }
                 
					return message;

			}
[HttpGet()]
			
			[ActionName("Assessment_List")]
			public virtual JObject Assessment_List(string tenantid
,string patientname=""
,string patientvisit=""
,string assessmentdate_automatonfrom=""
,string assessmentdate_automatonto=""
,string loginUserID="",int? pagesize=100 , int? pagenumber=0,string searchterm="",string  sort_fields = "")
			{
					 
				  	JObject jsonAssessment = null;
					try
					{
                        tenantid=pvar_tenantid;
						jsonAssessment = objAssessmentDAL.Assessment_List( tenantid
, patientname
, patientvisit
, assessmentdate_automatonfrom
, assessmentdate_automatonto
,pagesize,pagenumber,searchterm,sort_fields);
					}
					catch (Exception ex)
					{
                             _logger.LogError(ex,"An exception occurred in - Assessment_List, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                        
					}
					return jsonAssessment;

			   }

[HttpGet()]
			[ActionName("Patient_Profile_Assessments")]
			public virtual System.Data.DataTable Patient_Profile_Assessments(string tenantid, string patientname="", string loginUserID="")
			{
					DataTable dtAssessment = new DataTable();
					try
					{
						tenantid=pvar_tenantid;
						dtAssessment = objAssessmentDAL.Patient_Profile_Assessments(tenantid, patientname);
					}
					catch (Exception ex)
					{
						_logger.LogError(ex,"An exception occurred in - Patient_Profile_Assessments, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
					}
					return dtAssessment;

			   }
			   
[HttpGet()]
			
			[ActionName("get_all_Assessment")]
			public virtual System.Data.DataTable get_all_Assessment(string tenantid,string searchterm="",string pagesize = "50", string pagenumber = "1",string loginUserID="")
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
				  	DataTable dtAssessment = new DataTable();
					try
					{
                        tenantid=pvar_tenantid;
						dtAssessment = objAssessmentDAL.get_all_Assessment(tenantid,searchterm,pageSize,pageNumber);
					}
					catch (Exception ex)
					{
                          _logger.LogError(ex,"An exception occurred in - get_all_Assessment, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
					    
					}
					return dtAssessment;

			   }
[HttpGet()]
			  [ActionName("getById_allinfo_Assessment")]
			  public virtual System.Data.DataTable getById_allinfo_Assessment(string Assessmentid)
			  { 
				    DataTable dtAssessment = new DataTable();
					try
					{
						  dtAssessment = objAssessmentDAL.getById_allinfo_Assessment(Assessmentid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_allinfo_Assessment, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
   
					}
					return dtAssessment;

			  }
[HttpGet()]
							[ActionName("lookup_Assessment_patientname")]
							public virtual System.Data.DataTable lookup_Assessment_patientname(String tenantid,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtAssessment = new DataTable();
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
										dtAssessment = objAssessmentDAL.lookup_Assessment_patientname(tenantid,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_Assessment_patientname, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtAssessment;
							}
[HttpGet()]
							[ActionName("lookup_Assessment_patientvisit")]
							public virtual System.Data.DataTable lookup_Assessment_patientvisit(String tenantid,String patientname,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtAssessment = new DataTable();
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
										dtAssessment = objAssessmentDAL.lookup_Assessment_patientvisit(tenantid,patientname,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_Assessment_patientvisit, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtAssessment;
							}
[HttpGet()]
							[ActionName("lookup_Assessment_doctorname")]
							public virtual System.Data.DataTable lookup_Assessment_doctorname(String tenantid,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtAssessment = new DataTable();
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
										dtAssessment = objAssessmentDAL.lookup_Assessment_doctorname(tenantid,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_Assessment_doctorname, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtAssessment;
							}
[HttpGet()]
							[ActionName("lookup_Assessment_ipdform")]
							public virtual System.Data.DataTable lookup_Assessment_ipdform(String tenantid,String patientname,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtAssessment = new DataTable();
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
										dtAssessment = objAssessmentDAL.lookup_Assessment_ipdform(tenantid,patientname,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_Assessment_ipdform, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtAssessment;
							}
[HttpGet()]
							[ActionName("lookup_Assessment_opdform")]
							public virtual System.Data.DataTable lookup_Assessment_opdform(String tenantid,String patientname,String doctorname,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtAssessment = new DataTable();
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
										dtAssessment = objAssessmentDAL.lookup_Assessment_opdform(tenantid,patientname,doctorname,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_Assessment_opdform, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtAssessment;
							}
[HttpGet()]
							[ActionName("get_Assessment_suggested_templates")]
							public virtual System.Data.DataTable get_Assessment_suggested_templates(String ipdapplicationformid, String userrole, String taskname = "", string loginUserID="")
							{
									DataTable dt = new DataTable();
									try
									{
										dt = objAssessmentDAL.get_Assessment_suggested_templates(ipdapplicationformid, userrole, taskname);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - get_Assessment_suggested_templates, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
									}
									return dt;
							}
[HttpGet()]
							[ActionName("get_opd_assessmenttemplates")]
							public virtual System.Data.DataTable get_opd_assessmenttemplates(String opdformid, String taskname, string loginUserID="")
							{
									DataTable dt = new DataTable();
									try
									{
										dt = objAssessmentDAL.get_opd_assessmenttemplates(opdformid, taskname);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - get_opd_assessmenttemplates, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
									}
									return dt;
							}
[HttpGet()]
							[ActionName("lookup_Assessment_questionnairetemplate")]
							public virtual System.Data.DataTable lookup_Assessment_questionnairetemplate(String tenantid,string loginUserID="")
							{
									DataTable dtAssessment = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtAssessment = objAssessmentDAL.lookup_Assessment_questionnairetemplate(tenantid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_Assessment_questionnairetemplate, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtAssessment;
							}
[HttpGet()]
							[ActionName("lookup_Assessment_assessmentquestions_questions")]
							public virtual System.Data.DataTable lookup_Assessment_assessmentquestions_questions(String tenantid,string loginUserID="")
							{
									DataTable dtAssessment = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtAssessment = objAssessmentDAL.lookup_Assessment_assessmentquestions_questions(tenantid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_Assessment_assessmentquestions_questions, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtAssessment;
							}
[HttpGet()]
							[ActionName("lookup_Assessment_assessmentquestions_questioncategory")]
							public virtual System.Data.DataTable lookup_Assessment_assessmentquestions_questioncategory(String tenantid,string loginUserID="")
							{
									DataTable dtAssessment = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtAssessment = objAssessmentDAL.lookup_Assessment_assessmentquestions_questioncategory(tenantid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_Assessment_assessmentquestions_questioncategory, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtAssessment;
							}
[HttpGet()]
							[ActionName("lookup_Assessment_assessmentquestions_questionsub")]
							public virtual System.Data.DataTable lookup_Assessment_assessmentquestions_questionsub(String tenantid,String questioncategory,string loginUserID="")
							{
									DataTable dtAssessment = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtAssessment = objAssessmentDAL.lookup_Assessment_assessmentquestions_questionsub(tenantid,questioncategory);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_Assessment_assessmentquestions_questionsub, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtAssessment;
							}
[HttpGet()]
							[ActionName("lookup_Assessment_assessmentquestions_question")]
							public virtual System.Data.DataTable lookup_Assessment_assessmentquestions_question(String tenantid,String questioncategory,String questionsub,string loginUserID="")
							{
									DataTable dtAssessment = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtAssessment = objAssessmentDAL.lookup_Assessment_assessmentquestions_question(tenantid,questioncategory,questionsub);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_Assessment_assessmentquestions_question, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtAssessment;
							}


[HttpGet()]
							[ActionName("lookup_change_assessmentquestions_Assessment_question")]
							public virtual System.Data.DataTable lookup_change_assessmentquestions_Assessment_question(string AssessmentQuestionid,string loginUserID="")
							{
									DataTable dtAssessmentQuestion = new DataTable();
									try
									{
										dtAssessmentQuestion = objAssessmentDAL.lookup_change_assessmentquestions_Assessment_question(AssessmentQuestionid);
									}
									catch (Exception ex)
									{
									  _logger.LogError(ex,"An exception occurred in - lookup_change_assessmentquestions_Assessment_question, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtAssessmentQuestion;
							}




				}


			}
