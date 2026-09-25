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
				//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:41
				public class EnquiryFormController : BaseController
				{
				    public EnquiryFormController(IHttpContextAccessor httpContextAccessor,IOptions<ConnectionSettings> connectionSettings, ILoggerFactory loggerFactory, IConfiguration configuration,IWebHostEnvironment hostingEnvironment)
				    {
					     _configuration = configuration;
					     _logger = loggerFactory.CreateLogger<EnquiryFormController>();
					     _connectionSettings = connectionSettings;
					     objEnquiryFormDAL = new EnquiryFormDAL(_connectionSettings.Value.ConnectionString);
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
				private EnquiryFormDAL objEnquiryFormDAL;
                private External_System_DAL obj_External_System_DAL;
				private IOptions<ConnectionSettings> _connectionSettings;
				private ILogger _logger;
				private IConfiguration _configuration;
				private IWebHostEnvironment hostingEnv;
                private ExternalSystemUtility objExternalSystemUtitlity;
                private string pvar_tenantid="|";
                private string pvar_usersid="";

			    [HttpGet()]
			  [ActionName("getById_medicalinfo")]
			  public virtual System.Data.DataTable getById_medicalinfo(string EnquiryFormid)
			  {
					DataTable dtEnquiryForm_medicalinfo = new DataTable();
					try
					{
						dtEnquiryForm_medicalinfo = objEnquiryFormDAL.getById_medicalinfo(EnquiryFormid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_medicalinfo, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
    
					}
					return dtEnquiryForm_medicalinfo;


			  }


            [HttpPost()]
            [AllowAnonymous()]
            [ActionName("Add_Enquiry")]
            public virtual IActionResult Add_Enquiry([FromBody]EnquiryFormModel model)
            { 
              string message = "";
                
                access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Add_Enquiry";
            try{

            if (ModelState.IsValid)
            {

            	EnquiryFormModelValidator validator = new EnquiryFormModelValidator();
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

		       
                                 

                                
            		                
                                     message = objEnquiryFormDAL.Add_Enquiry(model);
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

            	_logger.LogError("EnquiryFormModel - Add_Enquiry , Validation Error :" + message);
            	message = ("Validation Error : " + message);
            }






            }catch(Exception ex){
               message=ex.Message;
               _logger.LogError(ex,"An exception occurred in - Add_Enquiry, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
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


		[HttpPost()]
		[AllowAnonymous()]
		[ActionName("Add_Enquiry_Public")]
		public virtual IActionResult Add_Enquiry_Public([FromBody] EnquiryFormModel model)
		{
			string message = "";

			access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
			obj_access_logsdetailsModel.action_method_name = "Add_Enquiry";
			try
			{

				if (ModelState.IsValid)
				{

					EnquiryFormModelValidator validator = new EnquiryFormModelValidator();
					ValidationResult results = validator.Validate(model);
					if (!results.IsValid)
					{
						var errorCollection = string.Join(" | ", results.Errors.Select(e => e.ErrorMessage.Replace("{propertyName}", e.PropertyName)));
						message = ("Validation Error : " + errorCollection);


					}
					else
					{

					 



							message = objEnquiryFormDAL.Add_Enquiry(model);
						 

					}


				}
				else
				{
					var errorCollection = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
					message = errorCollection.ToString();

					_logger.LogError("EnquiryFormModel - Add_Enquiry , Validation Error :" + message);
					message = ("Validation Error : " + message);
				}






			}
			catch (Exception ex)
			{
				message = ex.Message;
				_logger.LogError(ex, "An exception occurred in - Add_Enquiry, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

			}
			if (obj_access_logsdetailsModel.access_logsid != null)
			{
				obj_access_logsdetailsModel.api_response = message.Replace("\"", "");
				obj_External_System_DAL.create_access_logs_details(obj_access_logsdetailsModel);
			}

			if (message.Replace("\"", "").Contains("201.1"))
				return Ok(message);
			else if (message.Replace("\"", "") == "401.1")
				return Unauthorized(message);
			else
				return BadRequest(message);


		}
		[HttpGet()]
			  [AllowAnonymous()]
			  [ActionName("getById_EnquiryForm")]
			  public virtual EnquiryFormModel getById_EnquiryForm(string EnquiryFormid,string loginUserID="")
			  { 
				    EnquiryFormModel objEnquiryForm = new EnquiryFormModel();
					try
					{
						  objEnquiryForm = objEnquiryFormDAL.getById_EnquiryForm(EnquiryFormid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_EnquiryForm, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
            
					}
					return objEnquiryForm;

			  }
			  [HttpPost()]
			  [ActionName("Update_Enquiry")]
			  public virtual IActionResult Update_Enquiry([FromBody]EnquiryFormModel model)
			  { 
				    string message = "";
                   access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Update_Enquiry";

					try{

					if (ModelState.IsValid)
					{

						EnquiryFormModelValidator validator = new EnquiryFormModelValidator();
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

		       
                                 
		       
                                	
							    message = objEnquiryFormDAL.Update_Enquiry(model);	
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

						_logger.LogError("EnquiryFormModel - Update_Enquiry, Validation Error :" + message);
					
						//return BadRequest("Validation Error : " + message);
					}






					}catch(Exception ex){
                        
						message=ex.Message;
					    _logger.LogError(ex,"An exception occurred in - Update_Enquiry, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
           
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
            public virtual async Task<string> Remove_Enquiry(string EnquiryFormid,string loginUserID="")
			{
					string message ="";
                    access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Remove_Enquiry";

					try{
						
						  var authHeader = HttpContext.Request.Headers["Authorization"][0];
	                        if (authHeader.StartsWith("Bearer "))
	                        {
		                        
		                      
		                      
		                      var token = authHeader.Substring("Bearer ".Length);
		                         
		                        String[] userdetails=obj_External_System_DAL.get_users_by_token(token);
		                        loginUserID=userdetails[0].ToString();
                                obj_access_logsdetailsModel.access_logsid=new Guid(userdetails[1].ToString());
		       
                                 
                        	 message = objEnquiryFormDAL.Remove_Enquiry(EnquiryFormid,loginUserID);
						    }
	                        else{
		                        message = "Invalid Token";
		                       
	                        }
					 

					}catch(Exception ex){
						message=ex.Message;
                         _logger.LogError(ex,"An exception occurred in - Remove_Enquiry, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                         
					}
                    if(obj_access_logsdetailsModel.access_logsid !=null)
                    {
                         obj_access_logsdetailsModel.api_response=message.Replace("\"",""); 
                         obj_External_System_DAL.create_access_logs_details(obj_access_logsdetailsModel);
                    }
                 
					return message;

			}
        [HttpGet]
        [ActionName("Added_Enquiries")]
        public virtual JObject Added_Enquiries(
            string tenantid,
            string enquirynumber = "",
            string patientname = "",
            string phonenumber = "",
            string emailaddress = "",
            string enquirystatus = "",
            string verifiedstatus = "",
            string createddate_automatonfrom = "",
            string createddate_automatonto = "",
            string loginUserID = "",
            int? pagesize = 100,
            int? pagenumber = 0,
            string searchterm = "",
            string sort_fields = "")
        {
            JObject jsonEnquiryForm = null;

            try
            {
                tenantid = pvar_tenantid;

                jsonEnquiryForm = objEnquiryFormDAL.Added_Enquiries(
                    tenantid,
                    enquirynumber,
                    patientname,
                    phonenumber,
                    emailaddress,
                    enquirystatus,
                    verifiedstatus,
                    pagesize,
                    pagenumber,
                    searchterm,
                    sort_fields,
                    createddate_automatonfrom,
                    createddate_automatonto
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "An exception occurred in - Added_Enquiries, Error Message : " +
                    (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));
            }

            return jsonEnquiryForm;
        }

        [HttpGet()]
			
			[ActionName("get_all_EnquiryForm")]
			public virtual System.Data.DataTable get_all_EnquiryForm(string tenantid,string searchterm="",string pagesize = "50", string pagenumber = "1",string loginUserID="")
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
				  	DataTable dtEnquiryForm = new DataTable();
					try
					{
                        tenantid=pvar_tenantid;
						dtEnquiryForm = objEnquiryFormDAL.get_all_EnquiryForm(tenantid,searchterm,pageSize,pageNumber);
					}
					catch (Exception ex)
					{
                          _logger.LogError(ex,"An exception occurred in - get_all_EnquiryForm, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
					    
					}
					return dtEnquiryForm;

			   }
[HttpGet()]
			
			[ActionName("count_of_EnquiryForm")]
			public virtual System.Data.DataTable count_of_EnquiryForm(string tenantid
,string enquirynumber=""
,string patientname=""
,string phonenumber=""
,string emailaddress=""
,string enquirystatus=""
)
			{
					 
				  	DataTable dtEnquiryForm = new DataTable();
					try
					{
                        tenantid=pvar_tenantid;
						dtEnquiryForm = objEnquiryFormDAL.count_of_EnquiryForm( tenantid
, enquirynumber
, patientname
, phonenumber
, emailaddress
, enquirystatus
);
					}
					catch (Exception ex)
					{
                          _logger.LogError(ex,"An exception occurred in - count_of_EnquiryForm, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                         
					}
					return dtEnquiryForm;

			   }
			   
[HttpGet()]
			
			[ActionName("count_of_EnquiryForm_enquirystatus")]
			public virtual System.Data.DataTable count_of_EnquiryForm_enquirystatus(string tenantid
,string enquirynumber=""
,string patientname=""
,string phonenumber=""
,string emailaddress=""
,string enquirystatus=""
)
			{
					 
				  	DataTable dtEnquiryForm = new DataTable();
					try
					{
                        tenantid=pvar_tenantid;
						dtEnquiryForm = objEnquiryFormDAL.count_of_EnquiryForm_enquirystatus( tenantid
, enquirynumber
, patientname
, phonenumber
, emailaddress
, enquirystatus
);
					}
					catch (Exception ex)
					{
                          _logger.LogError(ex,"An exception occurred in - count_of_EnquiryForm_enquirystatus, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                         
					}
					return dtEnquiryForm;

			   }
			   
[HttpGet()]
			
			[ActionName("Enquiries_for_Review")]
			public virtual JObject Enquiries_for_Review(string tenantid
,string enquirystatus=""
,string verifiedstatus=""
,string loginUserID="",int? pagesize=100 , int? pagenumber=0,string searchterm="",string  sort_fields = "")
			{
					 
				  	JObject jsonEnquiryForm = null;
					try
					{
                        tenantid=pvar_tenantid;
						jsonEnquiryForm = objEnquiryFormDAL.Enquiries_for_Review( tenantid
, enquirystatus
, verifiedstatus
,pagesize,pagenumber,searchterm,sort_fields);
					}
					catch (Exception ex)
					{
                             _logger.LogError(ex,"An exception occurred in - Enquiries_for_Review, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                        
					}
					return jsonEnquiryForm;

			   }
			   
[HttpPost()]
            [ActionName("verify_EnquiryForm")]       
            public virtual string verify_EnquiryForm([FromBody] EnquiryFormReviewModel model)
			{
					string message ="";

					try{
						  model.verifiedby = pvar_usersid;
						  message = objEnquiryFormDAL.verify_EnquiryForm(model);
						 
						 

					}catch(Exception ex){
						message=ex.Message;
                         _logger.LogError(ex,"An exception occurred in - verify_EnquiryForm, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                          
					}
					
					return message;

			}
[HttpGet()]
			
			[ActionName("Enquiries")]
			public virtual JObject Enquiries(string tenantid
,string enquirystatus=""
,string loginUserID="",int? pagesize=100 , int? pagenumber=0,string searchterm="",string  sort_fields = "")
			{
					 
				  	JObject jsonEnquiryForm = null;
					try
					{
                        tenantid=pvar_tenantid;
						jsonEnquiryForm = objEnquiryFormDAL.Enquiries( tenantid
, enquirystatus
,pagesize,pagenumber,searchterm,sort_fields);
					}
					catch (Exception ex)
					{
                             _logger.LogError(ex,"An exception occurred in - Enquiries, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                        
					}
					return jsonEnquiryForm;

			   }
			   
[HttpGet()]
			  [ActionName("getById_allinfo_EnquiryForm")]
			  public virtual System.Data.DataTable getById_allinfo_EnquiryForm(string EnquiryFormid)
			  { 
				    DataTable dtEnquiryForm = new DataTable();
					try
					{
						  dtEnquiryForm = objEnquiryFormDAL.getById_allinfo_EnquiryForm(EnquiryFormid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_allinfo_EnquiryForm, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
   
					}
					return dtEnquiryForm;

			  }
[HttpGet()]
							[AllowAnonymous()]
							[ActionName("lookup_EnquiryForm_enquirytype")]
							public virtual System.Data.DataTable lookup_EnquiryForm_enquirytype(String tenantid,string loginUserID="")
							{
									DataTable dtEnquiryForm = new DataTable();
									try
									{   
										// For anonymous (e.g. PublicEnquiry): use query tenantid so enquiry types are filtered by tenant
										if (string.IsNullOrEmpty(pvar_tenantid) || pvar_tenantid == "|")
										{
											var q = string.IsNullOrEmpty(tenantid) ? "" : tenantid.Trim();
											tenantid = string.IsNullOrEmpty(q) ? "|" : "|" + q;
										}
										else
											tenantid = pvar_tenantid;
										dtEnquiryForm = objEnquiryFormDAL.lookup_EnquiryForm_enquirytype(tenantid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_EnquiryForm_enquirytype, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtEnquiryForm;
							}
[HttpGet()]
							[ActionName("lookup_EnquiryForm_patientname")]
							public virtual System.Data.DataTable lookup_EnquiryForm_patientname(String tenantid,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtEnquiryForm = new DataTable();
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
										dtEnquiryForm = objEnquiryFormDAL.lookup_EnquiryForm_patientname(tenantid,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_EnquiryForm_patientname, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtEnquiryForm;
							}
[HttpGet()]
							[AllowAnonymous()]
							[ActionName("lookup_EnquiryForm_preferredroomtype")]
							public virtual System.Data.DataTable lookup_EnquiryForm_preferredroomtype(String tenantid,string loginUserID="")
							{
									DataTable dtEnquiryForm = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtEnquiryForm = objEnquiryFormDAL.lookup_EnquiryForm_preferredroomtype(tenantid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_EnquiryForm_preferredroomtype, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtEnquiryForm;
							}
[HttpGet()]
							[ActionName("lookup_EnquiryForm_medicalinfo_medicalcondition")]
							public virtual System.Data.DataTable lookup_EnquiryForm_medicalinfo_medicalcondition(string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtEnquiryForm = new DataTable();
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
										dtEnquiryForm = objEnquiryFormDAL.lookup_EnquiryForm_medicalinfo_medicalcondition(searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_EnquiryForm_medicalinfo_medicalcondition, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtEnquiryForm;
							}

[HttpGet()]
							[AllowAnonymous()]
							[ActionName("lookup_change_EnquiryForm_enquirytype")]
							public virtual System.Data.DataTable lookup_change_EnquiryForm_enquirytype(string EnquiryTypeid,string loginUserID="")
							{
									DataTable dtEnquiryType = new DataTable();
									try
									{
										dtEnquiryType = objEnquiryFormDAL.lookup_change_EnquiryForm_enquirytype(EnquiryTypeid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_change_EnquiryForm_enquirytype, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtEnquiryType;
							}

[HttpGet()]
							[ActionName("lookup_change_medicalinfo_EnquiryForm_medicalcondition")]
							public virtual System.Data.DataTable lookup_change_medicalinfo_EnquiryForm_medicalcondition(string MedicalConditionid,string loginUserID="")
							{
									DataTable dtMedicalCondition = new DataTable();
									try
									{
										dtMedicalCondition = objEnquiryFormDAL.lookup_change_medicalinfo_EnquiryForm_medicalcondition(MedicalConditionid);
									}
									catch (Exception ex)
									{
									  _logger.LogError(ex,"An exception occurred in - lookup_change_medicalinfo_EnquiryForm_medicalcondition, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtMedicalCondition;
							}




				}


			}
