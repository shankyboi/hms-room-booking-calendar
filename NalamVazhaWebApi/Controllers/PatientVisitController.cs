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
				//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:00
				public class PatientVisitController : BaseController
				{
				    public PatientVisitController(IHttpContextAccessor httpContextAccessor,IOptions<ConnectionSettings> connectionSettings, ILoggerFactory loggerFactory, IConfiguration configuration,IWebHostEnvironment hostingEnvironment)
				    {
					     _configuration = configuration;
					     _logger = loggerFactory.CreateLogger<PatientVisitController>();
					     _connectionSettings = connectionSettings;
					     objPatientVisitDAL = new PatientVisitDAL(_connectionSettings.Value.ConnectionString);
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
				private PatientVisitDAL objPatientVisitDAL;
                private External_System_DAL obj_External_System_DAL;
				private IOptions<ConnectionSettings> _connectionSettings;
				private ILogger _logger;
				private IConfiguration _configuration;
				private IWebHostEnvironment hostingEnv;
                private ExternalSystemUtility objExternalSystemUtitlity;
                private string pvar_tenantid="|";
                private string pvar_usersid="";

			    
            [HttpPost()]
            [ActionName("Add_Patient_Visit")]
            public virtual IActionResult Add_Patient_Visit([FromBody]PatientVisitModel model)
            { 
              string message = "";
                
                access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Add_Patient_Visit";
            try{

            if (ModelState.IsValid)
            {

            	PatientVisitModelValidator validator = new PatientVisitModelValidator();
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

		       
                                 

                                
            		                
                                     message = objPatientVisitDAL.Add_Patient_Visit(model);
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

            	_logger.LogError("PatientVisitModel - Add_Patient_Visit , Validation Error :" + message);
            	message = ("Validation Error : " + message);
            }






            }catch(Exception ex){
               message=ex.Message;
               _logger.LogError(ex,"An exception occurred in - Add_Patient_Visit, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
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
			  [ActionName("getById_PatientVisit")]
			  public virtual PatientVisitModel getById_PatientVisit(string PatientVisitid,string loginUserID="")
			  { 
				    PatientVisitModel objPatientVisit = new PatientVisitModel();
					try
					{
						  objPatientVisit = objPatientVisitDAL.getById_PatientVisit(PatientVisitid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_PatientVisit, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
            
					}
					return objPatientVisit;

			  }
			  [HttpPost()]
			  [ActionName("Change_Status")]
			  public virtual IActionResult Change_Status([FromBody]PatientVisitModel model)
			  { 
				    string message = "";
                   access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Change_Status";

					try{

					if (ModelState.IsValid)
					{

						PatientVisitModelValidator validator = new PatientVisitModelValidator();
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

		       
                                 
		       
                                	
							    message = objPatientVisitDAL.Change_Status(model);	
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

						_logger.LogError("PatientVisitModel - Change_Status, Validation Error :" + message);
					
						//return BadRequest("Validation Error : " + message);
					}






					}catch(Exception ex){
                        
						message=ex.Message;
					    _logger.LogError(ex,"An exception occurred in - Change_Status, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
           
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

			  [HttpPost()]
			  [ActionName("Update_Patient_Visit")]
			  public virtual IActionResult Update_Patient_Visit([FromBody]PatientVisitModel model)
			  { 
				    string message = "";
                   access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Update_Patient_Visit";

					try{

					if (ModelState.IsValid)
					{

						PatientVisitModelValidator validator = new PatientVisitModelValidator();
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

		       
                                 
		       
                                	
							    message = objPatientVisitDAL.Update_Patient_Visit(model);	
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

						_logger.LogError("PatientVisitModel - Update_Patient_Visit, Validation Error :" + message);
					
						//return BadRequest("Validation Error : " + message);
					}






					}catch(Exception ex){
                        
						message=ex.Message;
					    _logger.LogError(ex,"An exception occurred in - Update_Patient_Visit, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
           
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
            public virtual async Task<string> Remove_Patient_Visit(string PatientVisitid,string loginUserID="")
			{
					string message ="";
                    access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Remove_Patient_Visit";

					try{
						
						  var authHeader = HttpContext.Request.Headers["Authorization"][0];
	                        if (authHeader.StartsWith("Bearer "))
	                        {
		                        
		                      
		                      
		                      var token = authHeader.Substring("Bearer ".Length);
		                         
		                        String[] userdetails=obj_External_System_DAL.get_users_by_token(token);
		                        loginUserID=userdetails[0].ToString();
                                obj_access_logsdetailsModel.access_logsid=new Guid(userdetails[1].ToString());
		       
                                 
                        	 message = objPatientVisitDAL.Remove_Patient_Visit(PatientVisitid,loginUserID);
						    }
	                        else{
		                        message = "Invalid Token";
		                       
	                        }
					 

					}catch(Exception ex){
						message=ex.Message;
                         _logger.LogError(ex,"An exception occurred in - Remove_Patient_Visit, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                         
					}
                    if(obj_access_logsdetailsModel.access_logsid !=null)
                    {
                         obj_access_logsdetailsModel.api_response=message.Replace("\"",""); 
                         obj_External_System_DAL.create_access_logs_details(obj_access_logsdetailsModel);
                    }
                 
					return message;

			}
[HttpGet()]
			
			[ActionName("Patient_Visit_List")]
			public virtual JObject Patient_Visit_List(string tenantid
,string loginUserID="",int? pagesize=100 , int? pagenumber=0,string searchterm="",string  sort_fields = "")
			{
					 
				  	JObject jsonPatientVisit = null;
					try
					{
                        tenantid=pvar_tenantid;
						jsonPatientVisit = objPatientVisitDAL.Patient_Visit_List( tenantid
,pagesize,pagenumber,searchterm,sort_fields);
					}
					catch (Exception ex)
					{
                             _logger.LogError(ex,"An exception occurred in - Patient_Visit_List, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                        
					}
					return jsonPatientVisit;

			   }
			   
[HttpGet()]
			
			[ActionName("get_all_PatientVisit")]
			public virtual System.Data.DataTable get_all_PatientVisit(string tenantid,string searchterm="",string pagesize = "50", string pagenumber = "1",string loginUserID="")
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
				  	DataTable dtPatientVisit = new DataTable();
					try
					{
                        tenantid=pvar_tenantid;
						dtPatientVisit = objPatientVisitDAL.get_all_PatientVisit(tenantid,searchterm,pageSize,pageNumber);
					}
					catch (Exception ex)
					{
                          _logger.LogError(ex,"An exception occurred in - get_all_PatientVisit, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
					    
					}
					return dtPatientVisit;

			   }
[HttpGet()]
			  [ActionName("getById_allinfo_PatientVisit")]
			  public virtual System.Data.DataTable getById_allinfo_PatientVisit(string PatientVisitid)
			  { 
				    DataTable dtPatientVisit = new DataTable();
					try
					{
						  dtPatientVisit = objPatientVisitDAL.getById_allinfo_PatientVisit(PatientVisitid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_allinfo_PatientVisit, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
   
					}
					return dtPatientVisit;

			  }
[HttpGet()]
							[ActionName("lookup_PatientVisit_patientname")]
							public virtual System.Data.DataTable lookup_PatientVisit_patientname(String tenantid,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtPatientVisit = new DataTable();
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
										dtPatientVisit = objPatientVisitDAL.lookup_PatientVisit_patientname(tenantid,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_PatientVisit_patientname, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtPatientVisit;
							}
[HttpGet()]
							[ActionName("lookup_PatientVisit_ipdnumber")]
							public virtual System.Data.DataTable lookup_PatientVisit_ipdnumber(String tenantid,String patientname,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtPatientVisit = new DataTable();
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
										dtPatientVisit = objPatientVisitDAL.lookup_PatientVisit_ipdnumber(tenantid,patientname,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_PatientVisit_ipdnumber, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtPatientVisit;
							}
[HttpGet()]
							[ActionName("lookup_PatientVisit_opdnumber")]
							public virtual System.Data.DataTable lookup_PatientVisit_opdnumber(String tenantid,String patientname,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtPatientVisit = new DataTable();
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
										dtPatientVisit = objPatientVisitDAL.lookup_PatientVisit_opdnumber(tenantid,patientname,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_PatientVisit_opdnumber, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtPatientVisit;
							}
[HttpGet()]
							[ActionName("lookup_PatientVisit_consultingdoctor")]
							public virtual System.Data.DataTable lookup_PatientVisit_consultingdoctor(String tenantid,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtPatientVisit = new DataTable();
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
										dtPatientVisit = objPatientVisitDAL.lookup_PatientVisit_consultingdoctor(tenantid,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_PatientVisit_consultingdoctor, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtPatientVisit;
							}


		[HttpGet()]
		[ActionName("GetPatientVisitByOPDForm")]
		public virtual IActionResult GetPatientVisitByOPDForm(string opdformid, string loginUserID = "")
		{
			try
			{
				if (string.IsNullOrEmpty(opdformid))
					return Ok(new { patientvisitid = (string)null, visitnumber = (string)null });

				var dt = objPatientVisitDAL.GetPatientVisitByOPDForm(opdformid);
				if (dt.Rows.Count == 0)
					return Ok(new { patientvisitid = (string)null, visitnumber = (string)null });

				return Ok(new
				{
					patientvisitid = dt.Rows[0]["PatientVisitid"]?.ToString(),
					visitnumber = dt.Rows[0]["visitnumber"]?.ToString()
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "GetPatientVisitByOPDForm failed: " + ex.Message);
				return Ok(new { patientvisitid = (string)null, visitnumber = (string)null });
			}
		}




	}


}
