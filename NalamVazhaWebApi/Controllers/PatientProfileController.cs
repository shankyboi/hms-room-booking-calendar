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
				//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:58
				public class PatientProfileController : BaseController
				{
				    public PatientProfileController(IHttpContextAccessor httpContextAccessor,IOptions<ConnectionSettings> connectionSettings, ILoggerFactory loggerFactory, IConfiguration configuration,IWebHostEnvironment hostingEnvironment)
				    {
					     _configuration = configuration;
					     _logger = loggerFactory.CreateLogger<PatientProfileController>();
					     _connectionSettings = connectionSettings;
					     objPatientProfileDAL = new PatientProfileDAL(_connectionSettings.Value.ConnectionString);
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
				private PatientProfileDAL objPatientProfileDAL;
                private External_System_DAL obj_External_System_DAL;
				private IOptions<ConnectionSettings> _connectionSettings;
				private ILogger _logger;
				private IConfiguration _configuration;
				private IWebHostEnvironment hostingEnv;
                private ExternalSystemUtility objExternalSystemUtitlity;
                private string pvar_tenantid="|";
                private string pvar_usersid="";

			    [HttpGet()]
			  [ActionName("getById_emergencycontactinfo")]
			  public virtual System.Data.DataTable getById_emergencycontactinfo(string PatientProfileid)
			  {
					DataTable dtPatientProfile_emergencycontactinfo = new DataTable();
					try
					{
						dtPatientProfile_emergencycontactinfo = objPatientProfileDAL.getById_emergencycontactinfo(PatientProfileid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_emergencycontactinfo, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
    
					}
					return dtPatientProfile_emergencycontactinfo;


			  }


            [HttpPost()]
            [ActionName("Add_Patient_Profile")]
            public virtual IActionResult Add_Patient_Profile([FromBody]PatientProfileModel model)
            { 
              string message = "";
                
                access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Add_Patient_Profile";
            try{

            if (ModelState.IsValid)
            {

            	PatientProfileModelValidator validator = new PatientProfileModelValidator();
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

		       
                                 

                                
            		                
                                     message = objPatientProfileDAL.Add_Patient_Profile(model);
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

            	_logger.LogError("PatientProfileModel - Add_Patient_Profile , Validation Error :" + message);
            	message = ("Validation Error : " + message);
            }






            }catch(Exception ex){
               message=ex.Message;
               _logger.LogError(ex,"An exception occurred in - Add_Patient_Profile, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
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
			  [ActionName("getById_PatientProfile")]
			  public virtual PatientProfileModel getById_PatientProfile(string PatientProfileid,string loginUserID="")
			  { 
				    PatientProfileModel objPatientProfile = new PatientProfileModel();
					try
					{
						  objPatientProfile = objPatientProfileDAL.getById_PatientProfile(PatientProfileid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_PatientProfile, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
            
					}
					return objPatientProfile;

			  }
			  [HttpPost()]
			  [ActionName("Add_to_Blacklist")]
			  public virtual IActionResult Add_to_Blacklist([FromBody]PatientProfileModel model)
			  { 
				    string message = "";
                   access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Add_to_Blacklist";

					try{
				 
						 
                            var authHeader = HttpContext.Request.Headers["Authorization"][0];
	                        if (authHeader.StartsWith("Bearer "))
	                        {
		                       
                                 
		                      
                                var token = authHeader.Substring("Bearer ".Length);
		                        String[] userdetails=obj_External_System_DAL.get_users_by_token(token);
                                model.modifieduser=new Guid(userdetails[0].ToString());
                                obj_access_logsdetailsModel.access_logsid=new Guid(userdetails[1].ToString());

		       
                                 
		       
                                	
							    message = objPatientProfileDAL.Add_to_Blacklist(model);	
                            }
                            else{
                                message = "Invalid Token";
                                 
                            }
							
						 


					 






					}catch(Exception ex){
                        
						message=ex.Message;
					    _logger.LogError(ex,"An exception occurred in - Add_to_Blacklist, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
           
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
		[ActionName("Modify_Patient_Category")]
		public virtual IActionResult Modify_Patient_Category([FromBody] PatientProfileModel model)
		{
			string message = "";
			access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
			obj_access_logsdetailsModel.action_method_name = "Add_to_Blacklist";

			try
			{


				var authHeader = HttpContext.Request.Headers["Authorization"][0];
				if (authHeader.StartsWith("Bearer "))
				{



					var token = authHeader.Substring("Bearer ".Length);
					String[] userdetails = obj_External_System_DAL.get_users_by_token(token);
					model.modifieduser = new Guid(userdetails[0].ToString());
					obj_access_logsdetailsModel.access_logsid = new Guid(userdetails[1].ToString());





					message = objPatientProfileDAL.Modify_Patient_Category(model);
				}
				else
				{
					message = "Invalid Token";

				}











			}
			catch (Exception ex)
			{

				message = ex.Message;
				_logger.LogError(ex, "An exception occurred in - Add_to_Blacklist, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

			}

			if (obj_access_logsdetailsModel.access_logsid != null)
			{
				obj_access_logsdetailsModel.api_response = message.Replace("\"", "");
				obj_External_System_DAL.create_access_logs_details(obj_access_logsdetailsModel);
			}

			if (message.Replace("\"", "") == "201.1")
				return Ok(message);
			else if (message.Replace("\"", "") == "401.1")
				return Unauthorized(message);
			else
				return BadRequest(message);




		}


		[HttpPost()]
			  [ActionName("Mark_as_Deceased")]
			  public virtual IActionResult Mark_as_Deceased([FromBody]PatientProfileModel model)
			  { 
				    string message = "";
                   access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Mark_as_Deceased";

					try{

				
                            var authHeader = HttpContext.Request.Headers["Authorization"][0];
	                        if (authHeader.StartsWith("Bearer "))
	                        {
		                       
                                 
		                      
                                var token = authHeader.Substring("Bearer ".Length);
		                        String[] userdetails=obj_External_System_DAL.get_users_by_token(token);
                                model.modifieduser=new Guid(userdetails[0].ToString());
                                obj_access_logsdetailsModel.access_logsid=new Guid(userdetails[1].ToString());

		       
                                 
		       
                                	
							    message = objPatientProfileDAL.Mark_as_Deceased(model);	
                            }
                            else{
                                message = "Invalid Token";
                                 
                            }
							
						


					


					}catch(Exception ex){
                        
						message=ex.Message;
					    _logger.LogError(ex,"An exception occurred in - Mark_as_Deceased, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
           
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
			  [ActionName("Update_Emergency_Contact_Info")]
			  public virtual IActionResult Update_Emergency_Contact_Info([FromBody] PatientProfileEmergencyContactUpdateModel model)
			  {
				  string message = "";
				  try
				  {
					  if (model == null || model.PatientProfileid == Guid.Empty ||
						  model.emergencycontactinfo == null || model.emergencycontactinfo.Count == 0)
						  return BadRequest("Please add at least one emergency contact.");

					  var contactValidator = new PatientProfile_emergencycontactinfoModelValidator();
					  var validationErrors = model.emergencycontactinfo
						  .SelectMany(contact => contactValidator.Validate(contact).Errors)
						  .Select(error => error.ErrorMessage.Replace("{propertyName}", error.PropertyName))
						  .ToList();
					  if (validationErrors.Count > 0)
						  return BadRequest(string.Join(" | ", validationErrors));

					  var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
					  if (!authHeader.StartsWith("Bearer "))
						  return Unauthorized("Invalid Token");

					  var token = authHeader.Substring("Bearer ".Length);
					  String[] userdetails = obj_External_System_DAL.get_users_by_token(token);
					  var modifieduser = new Guid(userdetails[0]);
					  message = objPatientProfileDAL.Update_Emergency_Contact_Info(model, modifieduser);
				  }
				  catch (Exception ex)
				  {
					  message = ex.Message;
					  _logger.LogError(ex, "Unable to update patient emergency contact information.");
				  }

				  if (message == "201.1")
					  return Ok(message);
				  if (message == "401.1")
					  return Unauthorized(message);
				  return BadRequest(message);
			  }

			  [HttpPost()]
			  [ActionName("Update_Patient_Profile")]
			  public virtual IActionResult Update_Patient_Profile([FromBody]PatientProfileModel model)
			  { 
				    string message = "";
                   access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Update_Patient_Profile";

					try{

					if (ModelState.IsValid)
					{

						PatientProfileModelValidator validator = new PatientProfileModelValidator();
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

		       
                                 
		       
                                	
							    message = objPatientProfileDAL.Update_Patient_Profile(model);	
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

						_logger.LogError("PatientProfileModel - Update_Patient_Profile, Validation Error :" + message);
					
						//return BadRequest("Validation Error : " + message);
					}






					}catch(Exception ex){
                        
						message=ex.Message;
					    _logger.LogError(ex,"An exception occurred in - Update_Patient_Profile, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
           
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
            public virtual async Task<string> Remove_Patient_Profile(string PatientProfileid,string loginUserID="")
			{
					string message ="";
                    access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Remove_Patient_Profile";

					try{
						
						  var authHeader = HttpContext.Request.Headers["Authorization"][0];
	                        if (authHeader.StartsWith("Bearer "))
	                        {
		                        
		                      
		                      
		                      var token = authHeader.Substring("Bearer ".Length);
		                         
		                        String[] userdetails=obj_External_System_DAL.get_users_by_token(token);
		                        loginUserID=userdetails[0].ToString();
                                obj_access_logsdetailsModel.access_logsid=new Guid(userdetails[1].ToString());
		       
                                 
                        	 message = objPatientProfileDAL.Remove_Patient_Profile(PatientProfileid,loginUserID);
						    }
	                        else{
		                        message = "Invalid Token";
		                       
	                        }
					 

					}catch(Exception ex){
						message=ex.Message;
                         _logger.LogError(ex,"An exception occurred in - Remove_Patient_Profile, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                         
					}
                    if(obj_access_logsdetailsModel.access_logsid !=null)
                    {
                         obj_access_logsdetailsModel.api_response=message.Replace("\"",""); 
                         obj_External_System_DAL.create_access_logs_details(obj_access_logsdetailsModel);
                    }
                 
					return message;

			}




        [AllowAnonymous]
        [HttpPost()]
        [ActionName("Quick_Add_Patient_Profile")]
        public virtual IActionResult Quick_Add_Patient_Profile([FromBody] PatientProfileModel model)
        {
            string message = "";
            string plainPassword = "";
            try
            {
                // Validate accesskey
                //string key = Request.Headers["X-AccessKey"].ToString();
                //string configuredKey = _configuration["SecurityKey"]?.ToString();
                //if (string.IsNullOrEmpty(configuredKey) || string.IsNullOrEmpty(key) || key != configuredKey)
                //    return Unauthorized("Invalid access key.");

                if (ModelState.IsValid)
                {
                    PatientProfileModelValidator validator = new PatientProfileModelValidator();
                    ValidationResult results = validator.Validate(model);
                    if (!results.IsValid)
                    {
                        var errorCollection = string.Join(" | ", results.Errors.Select(e => e.ErrorMessage.Replace("{propertyName}", e.PropertyName)));
                        message = "Validation Error : " + errorCollection;
                    }
                    else
                    {
                        // Call new Quick Add DAL
                        message = objPatientProfileDAL.Quick_Add_Patient_Profile(model, out plainPassword);
                    }
                }
                else
                {
                    var errorCollection = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    message = "Validation Error : " + errorCollection;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                _logger.LogError(ex, "An exception occurred in - Quick_Add_Patient_Profile, Error Message : "
                    + (ex.StackTrace != null ? ex.StackTrace.ToString() : ex.Message));
            }

            if (message.Replace("\"", "").Contains("201.1"))
                return Ok("201.1|" + plainPassword);
            else if (message.Replace("\"", "") == "401.1")
                return Unauthorized(message);
            else
                return BadRequest(message);
        }
        [HttpGet()]
			
			[ActionName("Patient_Profiles")]
			public virtual JObject Patient_Profiles(string tenantid
,string loginUserID="",int? pagesize=100 , int? pagenumber=0,string searchterm="",string  sort_fields = "")
			{
					 
				  	JObject jsonPatientProfile = null;
					try
					{
                        tenantid=pvar_tenantid;
						jsonPatientProfile = objPatientProfileDAL.Patient_Profiles( tenantid
,pagesize,pagenumber,searchterm,sort_fields);
					}
					catch (Exception ex)
					{
                             _logger.LogError(ex,"An exception occurred in - Patient_Profiles, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                        
					}
					return jsonPatientProfile;

			   }
			   
[HttpGet()]
			
			[ActionName("get_all_PatientProfile")]
			public virtual System.Data.DataTable get_all_PatientProfile(string tenantid,string searchterm="",string pagesize = "50", string pagenumber = "1",string loginUserID="")
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
				  	DataTable dtPatientProfile = new DataTable();
					try
					{
                        tenantid=pvar_tenantid;
						dtPatientProfile = objPatientProfileDAL.get_all_PatientProfile(tenantid,searchterm,pageSize,pageNumber);
					}
					catch (Exception ex)
					{
                          _logger.LogError(ex,"An exception occurred in - get_all_PatientProfile, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
					    
					}
					return dtPatientProfile;

			   }
[HttpGet()]
			  [ActionName("getById_allinfo_PatientProfile")]
			  public virtual System.Data.DataTable getById_allinfo_PatientProfile(string PatientProfileid)
			  { 
				    DataTable dtPatientProfile = new DataTable();
					try
					{
						  dtPatientProfile = objPatientProfileDAL.getById_allinfo_PatientProfile(PatientProfileid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_allinfo_PatientProfile, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
   
					}
					return dtPatientProfile;

			  }
        [AllowAnonymous]
        [HttpGet()]
							[ActionName("lookup_PatientProfile_countryoforigin")]
							public virtual System.Data.DataTable lookup_PatientProfile_countryoforigin(string loginUserID="")
							{
									DataTable dtPatientProfile = new DataTable();
									try
									{   
                                        
                                        
										dtPatientProfile = objPatientProfileDAL.lookup_PatientProfile_countryoforigin();
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_PatientProfile_countryoforigin, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtPatientProfile;
							}
        [AllowAnonymous]
        [HttpGet()]
							[ActionName("lookup_PatientProfile_occupation")]
							public virtual System.Data.DataTable lookup_PatientProfile_occupation(string loginUserID="")
							{
									DataTable dtPatientProfile = new DataTable();
									try
									{   
                                        
                                        
										dtPatientProfile = objPatientProfileDAL.lookup_PatientProfile_occupation();
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_PatientProfile_occupation, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtPatientProfile;
							}
        [AllowAnonymous]
        [HttpGet()]
							[ActionName("lookup_PatientProfile_referralsource")]
							public virtual System.Data.DataTable lookup_PatientProfile_referralsource(String tenantid,string loginUserID="")
							{
									DataTable dtPatientProfile = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtPatientProfile = objPatientProfileDAL.lookup_PatientProfile_referralsource(tenantid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_PatientProfile_referralsource, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtPatientProfile;
							}
        [AllowAnonymous]
        [HttpGet()]
							[ActionName("lookup_PatientProfile_reasonforblacklisting")]
							public virtual System.Data.DataTable lookup_PatientProfile_reasonforblacklisting(String tenantid,string loginUserID="")
							{
									DataTable dtPatientProfile = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtPatientProfile = objPatientProfileDAL.lookup_PatientProfile_reasonforblacklisting(tenantid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_PatientProfile_reasonforblacklisting, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtPatientProfile;
							}

		[HttpGet()]

		[ActionName("Patient_Profile_List")]
		public virtual JObject Patient_Profile_List(string tenantid
, string firstname = ""
, string dateofbirth_automatonfrom = ""
, string dateofbirth_automatonto = ""
, string emailaddress = ""
, string mobilenumber = ""
, string whatsappnumber = ""
, string idprooftype = ""
, string idproofnumber = ""
, string pzip = ""
, string blacklisted = ""
, string deceased = ""
, string loginUserID = "", int? pagesize = 100, int? pagenumber = 0, string searchterm = "", string sort_fields = "")
		{

			JObject jsonPatientProfile = null;
			try
			{
				tenantid = pvar_tenantid;
				jsonPatientProfile = objPatientProfileDAL.Patient_Profile_List(tenantid
, firstname
, dateofbirth_automatonfrom
, dateofbirth_automatonto
, emailaddress
, mobilenumber
, whatsappnumber
, idprooftype
, idproofnumber
, pzip
, blacklisted
, deceased
, pagesize, pagenumber, searchterm, sort_fields);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - Patient_Profile_List, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

			}
			return jsonPatientProfile;

		}


		[HttpGet()]
		[ActionName("lookup_PatientProfile_patientcategory")]
		public virtual System.Data.DataTable lookup_PatientProfile_patientcategory(String tenantid, string loginUserID = "")
		{
			DataTable dtPatientProfile = new DataTable();
			try
			{
				tenantid = pvar_tenantid;

				dtPatientProfile = objPatientProfileDAL.lookup_PatientProfile_patientcategory(tenantid);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - lookup_PatientProfile_patientcategory, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

			}
			return dtPatientProfile;
		}




	}


}
