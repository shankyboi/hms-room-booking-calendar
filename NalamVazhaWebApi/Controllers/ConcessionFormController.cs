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
				//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 05/26/2026 07:14:50
				public class ConcessionFormController : BaseController
				{
				    public ConcessionFormController(IHttpContextAccessor httpContextAccessor,IOptions<ConnectionSettings> connectionSettings, ILoggerFactory loggerFactory, IConfiguration configuration,IWebHostEnvironment hostingEnvironment)
				    {
					     _configuration = configuration;
					     _logger = loggerFactory.CreateLogger<ConcessionFormController>();
					     _connectionSettings = connectionSettings;
					     objConcessionFormDAL = new ConcessionFormDAL(_connectionSettings.Value.ConnectionString);
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
				private ConcessionFormDAL objConcessionFormDAL;
                private External_System_DAL obj_External_System_DAL;
				private IOptions<ConnectionSettings> _connectionSettings;
				private ILogger _logger;
				private IConfiguration _configuration;
				private IWebHostEnvironment hostingEnv;
                private ExternalSystemUtility objExternalSystemUtitlity;
                private string pvar_tenantid="|";
                private string pvar_usersid="";

                private static bool CsvContains(string value, params string[] expectedValues)
                {
                    if (string.IsNullOrWhiteSpace(value)) return false;
                    var values = value.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(v => v.Trim());
                    return values.Any(v => expectedValues.Any(expected => string.Equals(v, expected, StringComparison.OrdinalIgnoreCase)));
                }

                private static void NormalizeConcessionForm(ConcessionFormModel model)
                {
                    if (model.requestedconcessionamount.HasValue && model.requestedconcessionamount.Value > 0)
                        model.requestedconcessionamount = -model.requestedconcessionamount.Value;

                    if (!string.Equals(model.haveyoutraveledinternationallyinthelast5years, "Yes", StringComparison.OrdinalIgnoreCase))
                    {
                        model.purposeoftravel = null;
                        model.otherpleasespecify = null;
                    }
                    else if (!CsvContains(model.purposeoftravel, "Others"))
                    {
                        model.otherpleasespecify = null;
                    }

                    if (!CsvContains(model.doanyofthefollowingapplytoyou, "Others", "Other"))
                        model.specifyotherindustry = null;
                }

                private void EnsureConcessionFormLookups(ConcessionFormModel model)
                {
                    if (CsvContains(model.whichofthesedoyouown, "None"))
                        objConcessionFormDAL.EnsureWhichOfTheseDoYouOwnNoneLookup();
                }

                private void FillMissingPatientDisplayNames(DataTable concessionForms)
                {
                    if (concessionForms == null || !concessionForms.Columns.Contains("patientname") || !concessionForms.Columns.Contains("patientname_master"))
                        return;

                    var patientProfileDAL = new PatientProfileDAL(_connectionSettings.Value.ConnectionString);
                    foreach (DataRow row in concessionForms.Rows)
                    {
                        var currentName = row["patientname_master"] == DBNull.Value ? "" : row["patientname_master"]?.ToString()?.Trim();
                        if (!string.IsNullOrWhiteSpace(currentName))
                            continue;

                        var patientId = row["patientname"] == DBNull.Value ? "" : row["patientname"]?.ToString();
                        if (string.IsNullOrWhiteSpace(patientId))
                            continue;

                        var patient = patientProfileDAL.getById_PatientProfile(patientId);
                        var displayName = string.Join(" ", new[] { patient?.firstname, patient?.lastname }.Where(name => !string.IsNullOrWhiteSpace(name))).Trim();
                        if (!string.IsNullOrWhiteSpace(displayName))
                            row["patientname_master"] = displayName;
                    }
                }

			    [HttpGet()]
			  [ActionName("getById_earningmembers")]
			  public virtual System.Data.DataTable getById_earningmembers(string ConcessionFormid)
			  {
					DataTable dtConcessionForm_earningmembers = new DataTable();
					try
					{
						dtConcessionForm_earningmembers = objConcessionFormDAL.getById_earningmembers(ConcessionFormid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_earningmembers, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
    
					}
					return dtConcessionForm_earningmembers;


			  }


            [HttpPost()]
            [ActionName("Add_Concession_Form")]
            public virtual IActionResult Add_Concession_Form([FromBody]ConcessionFormModel model)
            { 
              string message = "";
                
                access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Add_Concession_Form";
            try{
            NormalizeConcessionForm(model);

            if (ModelState.IsValid)
            {

            	ConcessionFormModelValidator validator = new ConcessionFormModelValidator();
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

		       
                                 

                                
            		                
                                     EnsureConcessionFormLookups(model);
                                     message = objConcessionFormDAL.Add_Concession_Form(model);
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

            	_logger.LogError("ConcessionFormModel - Add_Concession_Form , Validation Error :" + message);
            	message = ("Validation Error : " + message);
            }






            }catch(Exception ex){
               message=ex.Message;
               _logger.LogError(ex,"An exception occurred in - Add_Concession_Form, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
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
			  [ActionName("getById_ConcessionForm")]
			  public virtual ConcessionFormModel getById_ConcessionForm(string ConcessionFormid,string loginUserID="")
			  { 
				    ConcessionFormModel objConcessionForm = new ConcessionFormModel();
					try
					{
						  objConcessionForm = objConcessionFormDAL.getById_ConcessionForm(ConcessionFormid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_ConcessionForm, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
            
					}
					return objConcessionForm;

			  }
			  [HttpPost()]
			  [ActionName("Update_Concession_Form")]
			  public virtual IActionResult Update_Concession_Form([FromBody]ConcessionFormModel model)
			  { 
				    string message = "";
                   access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Update_Concession_Form";

					try{
					NormalizeConcessionForm(model);

					if (ModelState.IsValid)
					{

						ConcessionFormModelValidator validator = new ConcessionFormModelValidator();
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

		       
                                 
		       
                                	
							    EnsureConcessionFormLookups(model);
							    message = objConcessionFormDAL.Update_Concession_Form(model);	
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

						_logger.LogError("ConcessionFormModel - Update_Concession_Form, Validation Error :" + message);
					
						//return BadRequest("Validation Error : " + message);
					}






					}catch(Exception ex){
                        
						message=ex.Message;
					    _logger.LogError(ex,"An exception occurred in - Update_Concession_Form, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
           
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
            public virtual async Task<string> Remove_Concession_Form(string ConcessionFormid,string loginUserID="")
			{
					string message ="";
                    access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Remove_Concession_Form";

					try{
						
						  var authHeader = HttpContext.Request.Headers["Authorization"][0];
	                        if (authHeader.StartsWith("Bearer "))
	                        {
		                        
		                      
		                      
		                      var token = authHeader.Substring("Bearer ".Length);
		                         
		                        String[] userdetails=obj_External_System_DAL.get_users_by_token(token);
		                        loginUserID=userdetails[0].ToString();
                                obj_access_logsdetailsModel.access_logsid=new Guid(userdetails[1].ToString());
		       
                                 
                        	 message = objConcessionFormDAL.Remove_Concession_Form(ConcessionFormid,loginUserID);
						    }
	                        else{
		                        message = "Invalid Token";
		                       
	                        }
					 

					}catch(Exception ex){
						message=ex.Message;
                         _logger.LogError(ex,"An exception occurred in - Remove_Concession_Form, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                         
					}
                    if(obj_access_logsdetailsModel.access_logsid !=null)
                    {
                         obj_access_logsdetailsModel.api_response=message.Replace("\"",""); 
                         obj_External_System_DAL.create_access_logs_details(obj_access_logsdetailsModel);
                    }
                 
					return message;

			}
[HttpGet()]
			
			[ActionName("Added_Concession_Form")]
			public virtual System.Data.DataTable Added_Concession_Form(string tenantid
,string verifiedstatus=""
)
			{
					 
				  	DataTable dtConcessionForm = new DataTable();
					try
					{
                        tenantid=pvar_tenantid;
						dtConcessionForm = objConcessionFormDAL.Added_Concession_Form( tenantid
, verifiedstatus
);
                        FillMissingPatientDisplayNames(dtConcessionForm);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - Added_Concession_Form, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                       
					}
					return dtConcessionForm;

			   }
			   
[HttpGet()]
			
			[ActionName("get_all_ConcessionForm")]
			public virtual System.Data.DataTable get_all_ConcessionForm(string tenantid,string loginUserID="")
			{
					 
				  	DataTable dtConcessionForm = new DataTable();
					try
					{
                        tenantid=pvar_tenantid;
						dtConcessionForm = objConcessionFormDAL.get_all_ConcessionForm(tenantid);
					}
					catch (Exception ex)
					{
                        _logger.LogError(ex,"An exception occurred in - get_all_ConcessionForm, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
					   
					}
					return dtConcessionForm;

			   }
[HttpGet()]
			
			[ActionName("count_of_ConcessionForm")]
			public virtual System.Data.DataTable count_of_ConcessionForm(string tenantid
)
			{
					 
				  	DataTable dtConcessionForm = new DataTable();
					try
					{
                        tenantid=pvar_tenantid;
						dtConcessionForm = objConcessionFormDAL.count_of_ConcessionForm( tenantid
);
					}
					catch (Exception ex)
					{
                          _logger.LogError(ex,"An exception occurred in - count_of_ConcessionForm, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                         
					}
					return dtConcessionForm;

			   }
			   
[HttpGet()]
			
			[ActionName("Concession_Form_for_Review")]
			public virtual System.Data.DataTable Concession_Form_for_Review(string tenantid
,string verifiedstatus=""
)
			{
					 
				  	DataTable dtConcessionForm = new DataTable();
					try
					{
                        tenantid=pvar_tenantid;
						dtConcessionForm = objConcessionFormDAL.Concession_Form_for_Review( tenantid
, verifiedstatus
);
                        FillMissingPatientDisplayNames(dtConcessionForm);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - Concession_Form_for_Review, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                       
					}
					return dtConcessionForm;

			   }
			   
[HttpPost()]
            [ActionName("verify_ConcessionForm")]       
            public virtual string verify_ConcessionForm([FromBody] ConcessionFormReviewModel model)
			{
					string message ="";

					try{
						  model.verifiedby = pvar_usersid;
						  message = objConcessionFormDAL.verify_ConcessionForm(model);
						 
						 

					}catch(Exception ex){
						message=ex.Message;
                         _logger.LogError(ex,"An exception occurred in - verify_ConcessionForm, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                          
					}
					
					return message;

			}
[HttpGet()]
			
			[ActionName("Concession_Form_List")]
			public virtual System.Data.DataTable Concession_Form_List(string tenantid
)
			{
					 
				  	DataTable dtConcessionForm = new DataTable();
					try
					{
                        tenantid=pvar_tenantid;
						dtConcessionForm = objConcessionFormDAL.Concession_Form_List( tenantid
);
                        FillMissingPatientDisplayNames(dtConcessionForm);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - Concession_Form_List, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                       
					}
					return dtConcessionForm;

			   }
			   
[HttpGet()]
			  [ActionName("getById_allinfo_ConcessionForm")]
			  public virtual System.Data.DataTable getById_allinfo_ConcessionForm(string ConcessionFormid)
			  { 
				    DataTable dtConcessionForm = new DataTable();
					try
					{
						  dtConcessionForm = objConcessionFormDAL.getById_allinfo_ConcessionForm(ConcessionFormid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_allinfo_ConcessionForm, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
   
					}
					return dtConcessionForm;

			  }
[HttpGet()]
			  [ActionName("get_Existing_Concession_Form")]
			  public virtual System.Data.DataTable get_Existing_Concession_Form(String tenantid,String patientname,String ipdnumber,string loginUserID="")
			  {
									DataTable dtConcessionForm = new DataTable();
									try
									{
                                        tenantid=pvar_tenantid;
										dtConcessionForm = objConcessionFormDAL.get_Existing_Concession_Form(tenantid,patientname,ipdnumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - get_Existing_Concession_Form, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
									}
									return dtConcessionForm;
			  }
[HttpGet()]
							[ActionName("lookup_ConcessionForm_patientname")]
							public virtual System.Data.DataTable lookup_ConcessionForm_patientname(String tenantid,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtConcessionForm = new DataTable();
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
										dtConcessionForm = objConcessionFormDAL.lookup_ConcessionForm_patientname(tenantid,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_ConcessionForm_patientname, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtConcessionForm;
							}
[HttpGet()]
							[ActionName("lookup_ConcessionForm_bookingreferencenumber")]
							public virtual System.Data.DataTable lookup_ConcessionForm_bookingreferencenumber(String tenantid,String patientname,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtConcessionForm = new DataTable();
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
										dtConcessionForm = objConcessionFormDAL.lookup_ConcessionForm_bookingreferencenumber(tenantid,patientname,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_ConcessionForm_bookingreferencenumber, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtConcessionForm;
							}






				}


			}
