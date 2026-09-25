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
				//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:34
				public class ClinicalAppointmentController : BaseController
				{
				    public ClinicalAppointmentController(IHttpContextAccessor httpContextAccessor,IOptions<ConnectionSettings> connectionSettings, ILoggerFactory loggerFactory, IConfiguration configuration,IWebHostEnvironment hostingEnvironment)
				    {
					     _configuration = configuration;
					     _logger = loggerFactory.CreateLogger<ClinicalAppointmentController>();
					     _connectionSettings = connectionSettings;
					     objClinicalAppointmentDAL = new ClinicalAppointmentDAL(_connectionSettings.Value.ConnectionString);
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
				private ClinicalAppointmentDAL objClinicalAppointmentDAL;
                private External_System_DAL obj_External_System_DAL;
				private IOptions<ConnectionSettings> _connectionSettings;
				private ILogger _logger;
				private IConfiguration _configuration;
				private IWebHostEnvironment hostingEnv;
                private ExternalSystemUtility objExternalSystemUtitlity;
                private string pvar_tenantid="|";
                private string pvar_usersid="";

			    [HttpGet()]
			  [ActionName("getById_reshedulehistory")]
			  public virtual System.Data.DataTable getById_reshedulehistory(string ClinicalAppointmentid)
			  {
					DataTable dtClinicalAppointment_reshedulehistory = new DataTable();
					try
					{
						dtClinicalAppointment_reshedulehistory = objClinicalAppointmentDAL.getById_reshedulehistory(ClinicalAppointmentid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_reshedulehistory, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
    
					}
					return dtClinicalAppointment_reshedulehistory;


			  }


            [HttpPost()]
            [ActionName("Add_Clinical_Appointment")]
            public virtual IActionResult Add_Clinical_Appointment([FromBody]ClinicalAppointmentModel model)
            { 
              string message = "";
                
                access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Add_Clinical_Appointment";
            try{

            if (ModelState.IsValid)
            {

            	ClinicalAppointmentModelValidator validator = new ClinicalAppointmentModelValidator();
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

		       
                                 

                                
            		                
                                     message = objClinicalAppointmentDAL.Add_Clinical_Appointment(model);
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

            	_logger.LogError("ClinicalAppointmentModel - Add_Clinical_Appointment , Validation Error :" + message);
            	message = ("Validation Error : " + message);
            }






            }catch(Exception ex){
               message=ex.Message;
               _logger.LogError(ex,"An exception occurred in - Add_Clinical_Appointment, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
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
			  [ActionName("getById_ClinicalAppointment")]
			  public virtual ClinicalAppointmentModel getById_ClinicalAppointment(string ClinicalAppointmentid,string loginUserID="")
			  { 
				    ClinicalAppointmentModel objClinicalAppointment = new ClinicalAppointmentModel();
					try
					{
						  objClinicalAppointment = objClinicalAppointmentDAL.getById_ClinicalAppointment(ClinicalAppointmentid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_ClinicalAppointment, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
            
					}
					return objClinicalAppointment;

			  }
			  [HttpPost()]
			  [ActionName("Reschedule_Appointment")]
			  public virtual IActionResult Reschedule_Appointment([FromBody]ClinicalAppointmentModel model)
			  { 
				    string message = "";
                   access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Reschedule_Appointment";

					try{

					if (ModelState.IsValid)
					{

						ClinicalAppointmentModelValidator validator = new ClinicalAppointmentModelValidator();
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

		       
                                 
		       

							    message = objClinicalAppointmentDAL.Reschedule_Appointment(model);
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

						_logger.LogError("ClinicalAppointmentModel - Reschedule_Appointment, Validation Error :" + message);
					
						//return BadRequest("Validation Error : " + message);
					}






					}catch(Exception ex){
                        
						message=ex.Message;
					    _logger.LogError(ex,"An exception occurred in - Reschedule_Appointment, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
           
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
			  [ActionName("Update_Clinical_Appointment")]
			  public virtual IActionResult Update_Clinical_Appointment([FromBody]ClinicalAppointmentModel model)
			  { 
				    string message = "";
                   access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Update_Clinical_Appointment";

					try{

					if (ModelState.IsValid)
					{

						ClinicalAppointmentModelValidator validator = new ClinicalAppointmentModelValidator();
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

		       
                                 
		       
                                	
							    message = objClinicalAppointmentDAL.Update_Clinical_Appointment(model);	
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

						_logger.LogError("ClinicalAppointmentModel - Update_Clinical_Appointment, Validation Error :" + message);
					
						//return BadRequest("Validation Error : " + message);
					}






					}catch(Exception ex){
                        
						message=ex.Message;
					    _logger.LogError(ex,"An exception occurred in - Update_Clinical_Appointment, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
           
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
            public virtual async Task<string> Remove_Clinical_Appointment(string ClinicalAppointmentid,string loginUserID="")
			{
					string message ="";
                    access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Remove_Clinical_Appointment";

					try{
						
						  var authHeader = HttpContext.Request.Headers["Authorization"][0];
	                        if (authHeader.StartsWith("Bearer "))
	                        {
		                        
		                      
		                      
		                      var token = authHeader.Substring("Bearer ".Length);
		                         
		                        String[] userdetails=obj_External_System_DAL.get_users_by_token(token);
		                        loginUserID=userdetails[0].ToString();
                                obj_access_logsdetailsModel.access_logsid=new Guid(userdetails[1].ToString());
		       
                                 
                        	 message = objClinicalAppointmentDAL.Remove_Clinical_Appointment(ClinicalAppointmentid,loginUserID);
						    }
	                        else{
		                        message = "Invalid Token";
		                       
	                        }
					 

					}catch(Exception ex){
						message=ex.Message;
                         _logger.LogError(ex,"An exception occurred in - Remove_Clinical_Appointment, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                         
					}
                    if(obj_access_logsdetailsModel.access_logsid !=null)
                    {
                         obj_access_logsdetailsModel.api_response=message.Replace("\"",""); 
                         obj_External_System_DAL.create_access_logs_details(obj_access_logsdetailsModel);
                    }
                 
					return message;

			}
[HttpGet()]
			
			[ActionName("Clinical_Appointment_List")]
			public virtual JObject Clinical_Appointment_List(string tenantid
,string patient=""
,string practitioner=""
,string appointmentdate_automatonfrom=""
,string appointmentdate_automatonto=""
,string loginUserID="",int? pagesize=100 , int? pagenumber=0,string searchterm="",string  sort_fields = "")
			{
					 
				  	JObject jsonClinicalAppointment = null;
					try
					{
                        tenantid=pvar_tenantid;
						jsonClinicalAppointment = objClinicalAppointmentDAL.Clinical_Appointment_List( tenantid
, patient
, practitioner
, appointmentdate_automatonfrom
, appointmentdate_automatonto
,pagesize,pagenumber,searchterm,sort_fields);
					}
					catch (Exception ex)
					{
                             _logger.LogError(ex,"An exception occurred in - Clinical_Appointment_List, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                        
					}
					return jsonClinicalAppointment;

			   }
			   
[HttpGet()]
			
			[ActionName("get_all_ClinicalAppointment")]
			public virtual System.Data.DataTable get_all_ClinicalAppointment(string tenantid,string searchterm="",string pagesize = "50", string pagenumber = "1",string loginUserID="")
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
				  	DataTable dtClinicalAppointment = new DataTable();
					try
					{
                        tenantid=pvar_tenantid;
						dtClinicalAppointment = objClinicalAppointmentDAL.get_all_ClinicalAppointment(tenantid,searchterm,pageSize,pageNumber);
					}
					catch (Exception ex)
					{
                          _logger.LogError(ex,"An exception occurred in - get_all_ClinicalAppointment, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
					    
					}
					return dtClinicalAppointment;

			   }
[HttpGet()]
			  [ActionName("getById_allinfo_ClinicalAppointment")]
			  public virtual System.Data.DataTable getById_allinfo_ClinicalAppointment(string ClinicalAppointmentid)
			  { 
				    DataTable dtClinicalAppointment = new DataTable();
					try
					{
						  dtClinicalAppointment = objClinicalAppointmentDAL.getById_allinfo_ClinicalAppointment(ClinicalAppointmentid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_allinfo_ClinicalAppointment, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
   
					}
					return dtClinicalAppointment;

			  }
[HttpGet()]
							[ActionName("lookup_ClinicalAppointment_patient")]
							public virtual System.Data.DataTable lookup_ClinicalAppointment_patient(String tenantid,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtClinicalAppointment = new DataTable();
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
										dtClinicalAppointment = objClinicalAppointmentDAL.lookup_ClinicalAppointment_patient(tenantid,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_ClinicalAppointment_patient, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtClinicalAppointment;
							}
[HttpGet()]
							[ActionName("lookup_ClinicalAppointment_practitioner")]
							public virtual System.Data.DataTable lookup_ClinicalAppointment_practitioner(String tenantid,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtClinicalAppointment = new DataTable();
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
										dtClinicalAppointment = objClinicalAppointmentDAL.lookup_ClinicalAppointment_practitioner(tenantid,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_ClinicalAppointment_practitioner, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtClinicalAppointment;
							}
[HttpGet()]
							[ActionName("lookup_ClinicalAppointment_actualpractitioner")]
							public virtual System.Data.DataTable lookup_ClinicalAppointment_actualpractitioner(String tenantid,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtClinicalAppointment = new DataTable();
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
										dtClinicalAppointment = objClinicalAppointmentDAL.lookup_ClinicalAppointment_actualpractitioner(tenantid,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_ClinicalAppointment_actualpractitioner, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtClinicalAppointment;
							}

[HttpGet()]
							[ActionName("lookup_change_ClinicalAppointment_practitioner")]
							public virtual System.Data.DataTable lookup_change_ClinicalAppointment_practitioner(string Peopleid,string loginUserID="")
							{
									DataTable dtPeople = new DataTable();
									try
									{
										dtPeople = objClinicalAppointmentDAL.lookup_change_ClinicalAppointment_practitioner(Peopleid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_change_ClinicalAppointment_practitioner, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtPeople;
							}


		[HttpPost()]
		[ActionName("UpdateAppointmentStatus")]
		public virtual IActionResult UpdateAppointmentStatus([FromBody] ClinicalAppointmentModel model)
		{
			string message = "";
			access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
			obj_access_logsdetailsModel.action_method_name = "UpdateAppointmentStatus";

			try
			{
				if (model == null
					|| model.ClinicalAppointmentid == null
					|| string.IsNullOrWhiteSpace(model.status))
				{
					message = "Validation Error : ClinicalAppointmentid and status are required.";
				}
				else
				{
					var authHeader = HttpContext.Request.Headers["Authorization"][0];
					if (authHeader.StartsWith("Bearer "))
					{
						var token = authHeader.Substring("Bearer ".Length);
						String[] userdetails = obj_External_System_DAL.get_users_by_token(token);

						// modifieduser already set by admin controller from session;
						// override here from token to stay consistent with WebApi pattern
						model.modifieduser = new Guid(userdetails[0].ToString());
						obj_access_logsdetailsModel.access_logsid = new Guid(userdetails[1].ToString());

						message = objClinicalAppointmentDAL.UpdateAppointmentStatus(
							model.ClinicalAppointmentid.ToString(),
							model.status,
							model.modifieduser.ToString());
					}
					else
					{
						message = "Invalid Token";
					}
				}
			}
			catch (Exception ex)
			{
				message = ex.Message;
				_logger.LogError(ex, "An exception occurred in - UpdateAppointmentStatus, Error Message : "
					+ (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace}" : ex.Message));
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

		[ActionName("get_all_TaskType")]
		public virtual System.Data.DataTable get_all_TaskType(string tenantid, string loginUserID = "")
		{

			DataTable dtTaskType = new DataTable();
			try
			{
				tenantid = pvar_tenantid;
				dtTaskType = objClinicalAppointmentDAL.get_all_TaskType(tenantid);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - get_all_TaskType, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));


			}
			return dtTaskType;

		}

        [HttpGet()]
        [ActionName("Get_Clinical_Slot_Data")]
        public virtual DataTable Get_Clinical_Slot_Data(
    string tenantid,
    string fromdate,
    string todate,
    string loginUserID = "")
        {
            DataTable dt =
                new DataTable();

            try
            {
                
                dt =
                    objClinicalAppointmentDAL
                        .Get_Clinical_Slot_Data(
                            tenantid,
                            fromdate,
                            todate
                        );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An exception occurred in - " +
                    "Get_Clinical_Slot_Data, Error Message : " +
                    ex.Message
                );
            }

            return dt;
        }

        [HttpGet]
        [ActionName("Get_Clinical_Slot_Doctor_Schedule")]
        public virtual DataTable Get_Clinical_Slot_Doctor_Schedule(
    string tenantid,
    string selecteddate,
    string loginUserID = "")
        {
            DataTable dt =
                new DataTable();

            try
            {
                dt =
                    objClinicalAppointmentDAL
                        .Get_Clinical_Slot_Doctor_Schedule(
                            tenantid,
                            selecteddate
                        );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while loading Clinical Slot Doctor Schedule"
                );
            }

            return dt;
        }

    }


			}
