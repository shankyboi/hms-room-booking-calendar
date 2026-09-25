namespace NalamVazhaWebApi.Controllers
			{
				using FluentValidation.Results;
				using System.Collections.Concurrent;
				using Microsoft.AspNetCore.Authorization;
				using Microsoft.AspNetCore.Hosting;
                using Microsoft.AspNetCore.Http;
				using Microsoft.AspNetCore.Mvc;
				using Microsoft.Extensions.Configuration;
				using Microsoft.Extensions.Logging;
				using Microsoft.Extensions.Options;
				using Microsoft.IdentityModel.Tokens;
				using NalamVazha.DAL;
				using NalamVazha.Models;
                using Newtonsoft.Json;
                using Newtonsoft.Json.Linq;
	using RestSharp;
	using Npgsql;
	using System;
				using System.Collections.Generic;
				using System.Data;
				using System.IdentityModel.Tokens.Jwt;
				using System.IO;
				using System.Linq;
				using System.Net.Http.Headers;
				using System.Security.Claims;
				using System.Text;
                using System.Threading.Tasks;
	using NpgsqlTypes;

	[Route("api/[controller]/[action]")]
				//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:54
				public class OPDFormController : BaseController
				{
				    public OPDFormController(IHttpContextAccessor httpContextAccessor,IOptions<ConnectionSettings> connectionSettings, ILoggerFactory loggerFactory, IConfiguration configuration,IWebHostEnvironment hostingEnvironment)
				    {
					     _configuration = configuration;
					     _logger = loggerFactory.CreateLogger<OPDFormController>();
					     _connectionSettings = connectionSettings;
					     objOPDFormDAL = new OPDFormDAL(_connectionSettings.Value.ConnectionString);
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
				private OPDFormDAL objOPDFormDAL;
                private External_System_DAL obj_External_System_DAL;
				private IOptions<ConnectionSettings> _connectionSettings;
				private ILogger _logger;
				private IConfiguration _configuration;
				private IWebHostEnvironment hostingEnv;
                private ExternalSystemUtility objExternalSystemUtitlity;
                private string pvar_tenantid="|";
                private string pvar_usersid="";
                private static readonly ConcurrentDictionary<string, object> _opdPaymentLocks = new ConcurrentDictionary<string, object>();
                private static readonly ConcurrentDictionary<string, object> _opdRefundLocks = new ConcurrentDictionary<string, object>();

                private static bool RequiresOPDReviewReason(string status)
                {
                    var normalized = (status ?? string.Empty).Trim().ToUpperInvariant();
                    return normalized == "REWORK" ||
                           normalized == "OPD REWORK" ||
                           normalized == "REJECT" ||
                           normalized == "REJECTED" ||
                           normalized == "OPD REJECT" ||
                           normalized == "OPD REJECTED";
                }

			    [HttpGet()]
			  [ActionName("getById_medicalinfo")]
			  public virtual System.Data.DataTable getById_medicalinfo(string OPDFormid)
			  {
					DataTable dtOPDForm_medicalinfo = new DataTable();
					try
					{
						dtOPDForm_medicalinfo = objOPDFormDAL.getById_medicalinfo(OPDFormid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_medicalinfo, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));

					}
					return dtOPDForm_medicalinfo;


			  }

[HttpGet()]
			  [ActionName("getById_medicationinfo")]
			  public virtual System.Data.DataTable getById_medicationinfo(string OPDFormid)
			  {
					DataTable dtOPDForm_medicationinfo = new DataTable();
					try
					{
						dtOPDForm_medicationinfo = objOPDFormDAL.getById_medicationinfo(OPDFormid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_medicationinfo, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));

					}
					return dtOPDForm_medicationinfo;


			  }

[HttpGet()]
			  [ActionName("getById_medicalrecords")]
			  public virtual System.Data.DataTable getById_medicalrecords(string OPDFormid)
			  {
					DataTable dtOPDForm_medicalrecords = new DataTable();
					try
					{
						dtOPDForm_medicalrecords = objOPDFormDAL.getById_medicalrecords(OPDFormid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_medicalrecords, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));

					}
					return dtOPDForm_medicalrecords;


			  }

[HttpGet()]
			  [ActionName("getById_appointmentpreferences")]
			  public virtual System.Data.DataTable getById_appointmentpreferences(string OPDFormid)
			  {
					DataTable dtOPDForm_appointmentpreferences = new DataTable();
					try
					{
						dtOPDForm_appointmentpreferences = objOPDFormDAL.getById_appointmentpreferences(OPDFormid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_appointmentpreferences, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));

					}
					return dtOPDForm_appointmentpreferences;


			  }


            [HttpGet]
            public IActionResult Get_Booking_Emergency_Contacts(Guid OPDFormid)
            {
                return Ok(new BookingEmergencyContactDAL(_connectionSettings.Value.ConnectionString).Get("OPDForm", OPDFormid));
            }

            [HttpPost()]
            [ActionName("Add_OPD_Form")]
            public virtual IActionResult Add_OPD_Form([FromBody]OPDFormModel model)
            {
              string message = "";

                access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Add_OPD_Form";
            try{

            if (ModelState.IsValid)
            {

            	OPDFormModelValidator validator = new OPDFormModelValidator();
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






                                     // Pre-set OPDFormid so it is available for receivable creation after insert
                                     if (model.OPDFormid == null) model.OPDFormid = Guid.NewGuid();

                                     message = objOPDFormDAL.Add_OPD_Form(model);
                                     if (message == "201.1" && model.OPDFormid.HasValue)
                                     {
                                         var emergencyContactDAL = new BookingEmergencyContactDAL(_connectionSettings.Value.ConnectionString);
                                         if (model.patientname != Guid.Empty && model.createduser.HasValue)
                                             emergencyContactDAL.SaveOpdAndMergePatientProfile(
                                                 model.OPDFormid.Value,
                                                 model.patientname,
                                                 model.bookingemergencycontactsjson,
                                                 model.createduser.Value);
                                         else
                                             emergencyContactDAL.Save("OPDForm", model.OPDFormid.Value, model.bookingemergencycontactsjson);
                                     }

                                     if (message == "201.1" &&
                                         (string.Equals(model.verifiedstatus, "Approved", StringComparison.OrdinalIgnoreCase) ||
                                          string.Equals(model.verifiedstatus, "OPD Approved", StringComparison.OrdinalIgnoreCase)))
                                     {
                                         try { objOPDFormDAL.Ensure_OPD_Consultation_Fee_Receivable(model.OPDFormid?.ToString(), model.createduser?.ToString()); }
                                         catch (Exception rxEx) { _logger.LogError(rxEx, "Ensure_OPD_Consultation_Fee_Receivable (add) error: " + rxEx.Message); }
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

            	_logger.LogError("OPDFormModel - Add_OPD_Form , Validation Error :" + message);
            	message = ("Validation Error : " + message);
            }






            }catch(Exception ex){
               message=ex.Message;
               _logger.LogError(ex,"An exception occurred in - Add_OPD_Form, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));

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
			  [ActionName("getById_OPDForm")]
			  public virtual OPDFormModel getById_OPDForm(string OPDFormid,string loginUserID="")
			  {
				    OPDFormModel objOPDForm = new OPDFormModel();
					try
					{
						  objOPDForm = objOPDFormDAL.getById_OPDForm(OPDFormid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_OPDForm, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));

					}
					return objOPDForm;

			  }
			  [HttpPost()]
			  [ActionName("Update_OPD_Form")]
			  public virtual IActionResult Update_OPD_Form([FromBody]OPDFormModel model)
			  {
				    string message = "";
                   access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Update_OPD_Form";

					try{

					if (ModelState.IsValid)
					{

						OPDFormModelValidator validator = new OPDFormModelValidator();
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





                                message = objOPDFormDAL.Update_OPD_Form(model);
                                if (message == "201.1" && model.OPDFormid.HasValue)
                                {
                                    var emergencyContactDAL = new BookingEmergencyContactDAL(_connectionSettings.Value.ConnectionString);
                                    if (model.patientname != Guid.Empty && model.modifieduser.HasValue)
                                        emergencyContactDAL.SaveOpdAndMergePatientProfile(
                                            model.OPDFormid.Value,
                                            model.patientname,
                                            model.bookingemergencycontactsjson,
                                            model.modifieduser.Value);
                                    else
                                        emergencyContactDAL.Save("OPDForm", model.OPDFormid.Value, model.bookingemergencycontactsjson);
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

						_logger.LogError("OPDFormModel - Update_OPD_Form, Validation Error :" + message);

						//return BadRequest("Validation Error : " + message);
					}






					}catch(Exception ex){

						message=ex.Message;
					    _logger.LogError(ex,"An exception occurred in - Update_OPD_Form, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));

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
            public virtual async Task<string> Remove_OPD_Form(string OPDFormid,string loginUserID="")
			{
					string message ="";
                    access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Remove_OPD_Form";

					try{

						  var authHeader = HttpContext.Request.Headers["Authorization"][0];
	                        if (authHeader.StartsWith("Bearer "))
	                        {



		                      var token = authHeader.Substring("Bearer ".Length);

		                        String[] userdetails=obj_External_System_DAL.get_users_by_token(token);
		                        loginUserID=userdetails[0].ToString();
                                obj_access_logsdetailsModel.access_logsid=new Guid(userdetails[1].ToString());


                        	 message = objOPDFormDAL.Remove_OPD_Form(OPDFormid,loginUserID);
						    }
	                        else{
		                        message = "Invalid Token";

	                        }


					}catch(Exception ex){
						message=ex.Message;
                         _logger.LogError(ex,"An exception occurred in - Remove_OPD_Form, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));

					}
                    if(obj_access_logsdetailsModel.access_logsid !=null)
                    {
                         obj_access_logsdetailsModel.api_response=message.Replace("\"","");
                         obj_External_System_DAL.create_access_logs_details(obj_access_logsdetailsModel);
                    }

					return message;

			}
        [HttpGet()]
        [ActionName("Added_OPD_Form")]
        public virtual JObject Added_OPD_Form(
            string tenantid,
            string patientname = "",
            string verifiedstatus = "",
            string loginUserID = "",
            int? pagesize = 100,
            int? pagenumber = 0,
            string searchterm = "",
            string sort_fields = "",
            string createddate_automatonfrom = "",
            string createddate_automatonto = "",
            string bookingnumber = "", string workflowstatus = "", string financialstatus = "", string paymentmethod = "")
        {
            JObject jsonOPDForm = null;

            try
            {
                tenantid = pvar_tenantid;

                jsonOPDForm = objOPDFormDAL.Added_OPD_Form(
                    tenantid,
                    patientname,
                    verifiedstatus,
                    pagesize,
                    pagenumber,
                    searchterm,
                    sort_fields,
                    createddate_automatonfrom,
                    createddate_automatonto,
                    bookingnumber, workflowstatus, financialstatus, paymentmethod
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An exception occurred in - Added_OPD_Form, Error Message : " +
                    (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message)
                );
            }

            return jsonOPDForm;
        }

        [HttpGet()]

			[ActionName("get_all_OPDForm")]
			public virtual System.Data.DataTable get_all_OPDForm(string tenantid,string searchterm="",string pagesize = "50", string pagenumber = "1",string loginUserID="")
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
				  	DataTable dtOPDForm = new DataTable();
					try
					{
                        tenantid=pvar_tenantid;
						dtOPDForm = objOPDFormDAL.get_all_OPDForm(tenantid,searchterm,pageSize,pageNumber);
					}
					catch (Exception ex)
					{
                          _logger.LogError(ex,"An exception occurred in - get_all_OPDForm, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));


					}
					return dtOPDForm;

			   }
[HttpGet()]

			[ActionName("count_of_OPDForm")]
			public virtual System.Data.DataTable count_of_OPDForm(string tenantid
,string patientname=""
)
		{

				  	DataTable dtOPDForm = new DataTable();
					try
					{
                        tenantid=pvar_tenantid;
						dtOPDForm = objOPDFormDAL.count_of_OPDForm( tenantid
, patientname
);
					}
					catch (Exception ex)
					{
                          _logger.LogError(ex,"An exception occurred in - count_of_OPDForm, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));

					}
					return dtOPDForm;

			   }

[HttpGet()]

			[ActionName("OPD_Forms_for_Review")]
			public virtual JObject OPD_Forms_for_Review(string tenantid
,string patientname=""
, string verifiedstatus=""
,string loginUserID="",int? pagesize=100 , int? pagenumber=0,string searchterm="",string  sort_fields = "")
			{

				  	JObject jsonOPDForm = null;
					try
					{
                        tenantid=pvar_tenantid;
						jsonOPDForm = objOPDFormDAL.OPD_Forms_for_Review( tenantid
, patientname
, verifiedstatus
,pagesize,pagenumber,searchterm,sort_fields);
					}
					catch (Exception ex)
					{
                             _logger.LogError(ex,"An exception occurred in - OPD_Forms_for_Review, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));

					}
					return jsonOPDForm;

			   }

[HttpPost()]
		[ActionName("Cancel_OPD_Direct")]
		public virtual IActionResult Cancel_OPD_Direct([FromBody] OPDFormReviewModel model)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(model?.OPDFormid))
					return BadRequest("OPDFormid is required");
				var userId = string.IsNullOrWhiteSpace(model.verifiedby) ? pvar_usersid : model.verifiedby;
				var result = objOPDFormDAL.Cancel_OPD_Direct(model.OPDFormid, userId, model.reviewcomments);
				return Content("\"" + result + "\"", "application/json");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Cancel_OPD_Direct error: " + ex.Message);
				return BadRequest(ex.Message);
			}
		}

[HttpPost()]
            [ActionName("verify_OPDForm")]
            public virtual string verify_OPDForm([FromBody] OPDFormReviewModel model)
			{
					string message ="";

					if (model == null)
						return "Invalid OPD review request.";

					if (RequiresOPDReviewReason(model.verifiedstatus) &&
						string.IsNullOrWhiteSpace(model.reviewcomments))
						return "Review comments are required for Rework or Reject.";

					try{
						  model.verifiedby = pvar_usersid;
						  message = objOPDFormDAL.verify_OPDForm(model);

						  if (message == "201.1" &&
							  (string.Equals(model.verifiedstatus, "Approved", StringComparison.OrdinalIgnoreCase) ||
							   string.Equals(model.verifiedstatus, "OPD Approved", StringComparison.OrdinalIgnoreCase)))
						  {
							  try { objOPDFormDAL.Ensure_OPD_Consultation_Fee_Receivable(model.OPDFormid, pvar_usersid); }
							  catch (Exception rxEx) { _logger.LogError(rxEx, "Ensure_OPD_Consultation_Fee_Receivable (verify) error: " + rxEx.Message); }
						  }

					}catch(Exception ex){
						message=ex.Message;
                         _logger.LogError(ex,"An exception occurred in - verify_OPDForm, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));

					}

					return message;

			}
[HttpGet()]

			[ActionName("Approved_OPD_Forms")]
			public virtual JObject Approved_OPD_Forms(string tenantid
,string patientname=""
,string preferreddate_automatonfrom=""
,string preferreddate_automatonto=""
,string preferreddoctor=""
, string loginUserID="",int? pagesize=100 , int? pagenumber=0,string searchterm="",string  sort_fields = "")
			{

				  	JObject jsonOPDForm = null;
					try
					{
                        tenantid=pvar_tenantid;
						jsonOPDForm = objOPDFormDAL.Approved_OPD_Forms( tenantid
, patientname
, preferreddate_automatonfrom
, preferreddate_automatonto
, preferreddoctor
, pagesize,pagenumber,searchterm,sort_fields);
					}
					catch (Exception ex)
					{
                             _logger.LogError(ex,"An exception occurred in - Approved_OPD_Forms, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));

					}
					return jsonOPDForm;

			   }

[HttpGet()]
			  [ActionName("Get_Appointment_By_OPDForm")]
			  public virtual System.Data.DataTable Get_Appointment_By_OPDForm(string OPDFormid)
			  {
				  DataTable dtAppt = new DataTable();
				  try
				  {
					  dtAppt = objOPDFormDAL.Get_Appointment_By_OPDForm(OPDFormid);
				  }
				  catch (Exception ex)
				  {
					  _logger.LogError(ex, "An exception occurred in - Get_Appointment_By_OPDForm, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));
				  }
				  return dtAppt;
			  }

[HttpGet()]
			  [ActionName("OPD_Consultation_Fee")]
			  public virtual System.Data.DataTable OPD_Consultation_Fee(string OPDFormid)
			  {
				  DataTable dtFee = new DataTable();
				  try
				  {
					  ReconcileOPDRazorpayBillingToReceivable(OPDFormid);
					  dtFee = objOPDFormDAL.Get_OPD_Consultation_Fee(OPDFormid);
				  }
				  catch (Exception ex)
				  {
					  _logger.LogError(ex, "An exception occurred in - OPD_Consultation_Fee, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));
				  }
				  return dtFee;
			  }

[HttpGet()]
			  [ActionName("getById_allinfo_OPDForm")]
			  public virtual System.Data.DataTable getById_allinfo_OPDForm(string OPDFormid)
			  {
				    DataTable dtOPDForm = new DataTable();
					try
					{
						  dtOPDForm = objOPDFormDAL.getById_allinfo_OPDForm(OPDFormid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_allinfo_OPDForm, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));

					}
					return dtOPDForm;

			  }

		[HttpGet()]
		[ActionName("Get_OPD_Action_Eligibility")]
		public virtual DataTable Get_OPD_Action_Eligibility(
			string OPDFormid,
			string userrole = "",
			string loginUserID = "")
		{
			try
			{
				return objOPDFormDAL.Get_OPD_Action_Eligibility(
					OPDFormid,
					pvar_usersid,
					userrole);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Get_OPD_Action_Eligibility failed for {OPDFormid}", OPDFormid);
				return new DataTable();
			}
		}

		[HttpGet()]
		[ActionName("Get_OPD_Cancellation_Quote")]
		public virtual DataTable Get_OPD_Cancellation_Quote(string OPDFormid, string loginUserID = "")
		{
			try
			{
				return objOPDFormDAL.Get_OPD_Cancellation_Quote(OPDFormid);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Get_OPD_Cancellation_Quote failed for {OPDFormid}", OPDFormid);
				return new DataTable();
			}
		}

		[HttpPost()]
		[ActionName("Add_OPD_Manual_Receivable")]
		public virtual IActionResult Add_OPD_Manual_Receivable([FromBody] ReceivableModel model)
		{
			try
			{
				if (model?.opdnumber == null || model.opdnumber == Guid.Empty)
					return BadRequest("OPDFormid is required.");
				var result = objOPDFormDAL.Add_OPD_Manual_Receivable(
					model.opdnumber.Value.ToString(),
					model.tenantid,
					model.patientname,
					model.receivablefor,
					model.amount,
					model.remarks,
					pvar_usersid);
				return string.Equals(result, "201.1", StringComparison.OrdinalIgnoreCase)
					? Ok(new { message = "201.1" })
					: BadRequest(result);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Add_OPD_Manual_Receivable failed.");
				return BadRequest(ex.Message);
			}
		}
		[HttpGet()]
			  [ActionName("Get_Latest_Previous_OPD_Prefill_By_Patient")]
			  public virtual System.Data.DataTable Get_Latest_Previous_OPD_Prefill_By_Patient(string tenantid, string patientname)
			  {
				    DataTable dtOPDForm = new DataTable();
					try
					{
						  dtOPDForm = objOPDFormDAL.Get_Latest_Previous_OPD_Prefill_By_Patient(tenantid, patientname);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - Get_Latest_Previous_OPD_Prefill_By_Patient, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));

					}
					return dtOPDForm;

			  }
		[HttpGet()]
		[ActionName("lookup_OPDForm_task")]
		public virtual System.Data.DataTable lookup_OPDForm_task(String tenantid, string loginUserID = "")
		{
			DataTable dtOPDForm = new DataTable();
			try
			{
				tenantid = pvar_tenantid;

				dtOPDForm = objOPDFormDAL.lookup_OPDForm_task(tenantid);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - lookup_OPDForm_task, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

			}
			return dtOPDForm;
		}

		[HttpGet()]
		[ActionName("Get_OPD_Task_Eligibility")]
		public virtual IActionResult Get_OPD_Task_Eligibility(string patientname, string excludeopdformid = "", string loginUserID = "")
		{
			try { return Ok(new { tasktype = objOPDFormDAL.Get_OPD_Task_Eligibility(patientname, excludeopdformid) }); }
			catch (Exception ex)
			{
				_logger.LogError(ex, "Get_OPD_Task_Eligibility failed for patient {Patient}", patientname);
				return BadRequest(ex.Message);
			}
		}

		[HttpGet()]
		[ActionName("Validate_OPD_Task_Eligibility")]
		public virtual IActionResult Validate_OPD_Task_Eligibility(string patientname, string task, string excludeopdformid = "", string loginUserID = "")
		{
			try { return Ok(objOPDFormDAL.Validate_OPD_Task_Eligibility(patientname, task, excludeopdformid)); }
			catch (Exception ex)
			{
				_logger.LogError(ex, "Validate_OPD_Task_Eligibility failed for patient {Patient} and task {Task}", patientname, task);
				return BadRequest(ex.Message);
			}
		}

		[HttpGet()]
		[ActionName("Get_OPD_Online_Doctor_Workflow_Context")]
		public virtual DataTable Get_OPD_Online_Doctor_Workflow_Context(string OPDFormid, string loginUserID = "")
		{
			try { return objOPDFormDAL.Get_OPD_Online_Doctor_Workflow_Context(OPDFormid); }
			catch (Exception ex)
			{
				_logger.LogError(ex, "Get_OPD_Online_Doctor_Workflow_Context failed for {OPDFormid}", OPDFormid);
				return new DataTable();
			}
		}

		[HttpGet()]
							[ActionName("lookup_OPDForm_patientname")]
							public virtual System.Data.DataTable lookup_OPDForm_patientname(String tenantid,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtOPDForm = new DataTable();
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
										dtOPDForm = objOPDFormDAL.lookup_OPDForm_patientname(tenantid,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_OPDForm_patientname, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));

									}
									return dtOPDForm;
							}
[HttpGet()]
							[ActionName("lookup_OPDForm_preferreddoctor")]
							public virtual System.Data.DataTable lookup_OPDForm_preferreddoctor(String tenantid,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtOPDForm = new DataTable();
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
										dtOPDForm = objOPDFormDAL.lookup_OPDForm_preferreddoctor(tenantid,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_OPDForm_preferreddoctor, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));

									}
									return dtOPDForm;
							}
[HttpGet()]
							[ActionName("lookup_OPDForm_medicalinfo_medicalconditionname")]
							public virtual System.Data.DataTable lookup_OPDForm_medicalinfo_medicalconditionname(string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtOPDForm = new DataTable();
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
										dtOPDForm = objOPDFormDAL.lookup_OPDForm_medicalinfo_medicalconditionname(searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_OPDForm_medicalinfo_medicalconditionname, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));

									}
									return dtOPDForm;
							}

		[HttpGet()]
		[ActionName("Get_Doctor_Available_Slots")]
		public virtual System.Data.DataTable Get_Doctor_Available_Slots(string Peopleid, string appointmentdate, string tenantid = "",string taskid = "", string taskname = "", string loginUserID = "")
		{
			DataTable dtSlots = new DataTable();
			try
			{
				// Fall back to session tenant when caller does not supply one
				if (string.IsNullOrEmpty(tenantid))
					tenantid = pvar_tenantid;
				dtSlots = objOPDFormDAL.Get_Doctor_Available_Slots(Peopleid, appointmentdate, tenantid, taskid, taskname);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - Get_Doctor_Available_Slots, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));
			}
			return dtSlots;
		}

		[HttpGet()]
		[ActionName("Get_Doctor_Available_Slots_By_Preferences")]
		public virtual System.Data.DataTable Get_Doctor_Available_Slots_By_Preferences(string Peopleid, string appointmentdate, string slotpreferences, string tenantid = "", string taskid = "", string taskname = "", string loginUserID = "")
		{
			DataTable dtSlots = new DataTable();
			try
			{
				if (string.IsNullOrEmpty(tenantid))
					tenantid = pvar_tenantid;
				dtSlots = objOPDFormDAL.Get_Doctor_Available_Slots_By_Preferences(Peopleid, appointmentdate, slotpreferences, tenantid, taskid, taskname);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - Get_Doctor_Available_Slots_By_Preferences, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));
			}
			return dtSlots;
		}

		[HttpPost()]
		[ActionName("Update_Appointment_Status")]
		public virtual IActionResult Update_Appointment_Status([FromBody] AppointmentStatusUpdateModel model)
		{
			string message = "";
			access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
			obj_access_logsdetailsModel.action_method_name = "Update_Appointment_Status";
			try
			{
				var authHeader = HttpContext.Request.Headers["Authorization"][0];
				if (authHeader.StartsWith("Bearer "))
				{
					var token = authHeader.Substring("Bearer ".Length);
					String[] userdetails = obj_External_System_DAL.get_users_by_token(token);
					string modifieduser = userdetails[0].ToString();
					obj_access_logsdetailsModel.access_logsid = new Guid(userdetails[1].ToString());

					message = objOPDFormDAL.Update_Appointment_Status(
						model.ClinicalAppointmentid?.ToString(),
						model.status,
						modifieduser
					);
				}
				else
				{
					message = "Invalid Token";
				}
			}
			catch (Exception ex)
			{
				message = ex.Message;
				_logger.LogError(ex, "An exception occurred in - Update_Appointment_Status, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));
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
		[ActionName("Get_OPD_Consultation_Fee_By_OPDForm")]
		public virtual System.Data.DataTable Get_OPD_Consultation_Fee_By_OPDForm(string OPDFormid, string loginUserID = "")
		{
			DataTable dtResult = new DataTable();
			try
			{
				dtResult = objOPDFormDAL.Get_OPD_Consultation_Fee_By_OPDForm(OPDFormid);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - Get_OPD_Consultation_Fee_By_OPDForm, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));
			}
			return dtResult;
		}

		[HttpGet()]
		[ActionName("Get_Doctor_Consultation_Fee")]
		public virtual System.Data.DataTable Get_Doctor_Consultation_Fee(string Peopleid, string tasktype = "", string loginUserID = "")
		{
			DataTable dtFee = new DataTable();
			try
			{
				dtFee = objOPDFormDAL.Get_Doctor_Consultation_Fee(Peopleid, tasktype);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - Get_Doctor_Consultation_Fee, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));
			}
			return dtFee;
		}

		[HttpGet()]
		[ActionName("Get_IPD_Doctor_Consultation_Fee")]
		public virtual System.Data.DataTable Get_IPD_Doctor_Consultation_Fee(string Peopleid, string tasktype = "", string loginUserID = "")
		{
			DataTable dtFee = new DataTable();
			try
			{
				dtFee = objOPDFormDAL.Get_IPD_Doctor_Consultation_Fee(Peopleid, tasktype);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - Get_IPD_Doctor_Consultation_Fee, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));
			}
			return dtFee;
		}

		[HttpPost()]
		[ActionName("Complete_IP_Screening_For_IPD_Booking")]
		public virtual IActionResult Complete_IP_Screening_For_IPD_Booking([FromBody] CompleteIPScreeningForIPDRequestModel model)
		{
			string message = "";
			access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
			obj_access_logsdetailsModel.action_method_name = "Complete_IP_Screening_For_IPD_Booking";
			try
			{
				var authHeader = HttpContext.Request.Headers["Authorization"][0];
				if (authHeader.StartsWith("Bearer "))
				{
					var token = authHeader.Substring("Bearer ".Length);
					String[] userdetails = obj_External_System_DAL.get_users_by_token(token);
					string modifieduser = userdetails[0].ToString();
					obj_access_logsdetailsModel.access_logsid = new Guid(userdetails[1].ToString());

					if (model == null || string.IsNullOrWhiteSpace(model.IPDApplicationFormid))
						message = "IPDApplicationFormid is required";
					else
						message = objOPDFormDAL.Complete_IP_Screening_Appointments_For_IPD(
							model.IPDApplicationFormid,
							modifieduser
						);
				}
				else
					message = "Invalid Token";
			}
			catch (Exception ex)
			{
				message = ex.Message;
				_logger.LogError(ex, "Complete_IP_Screening_For_IPD_Booking error: " + ex.Message);
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

		[HttpPost()]
		[ActionName("Complete_IP_New_For_IPD_Booking")]
		public virtual IActionResult Complete_IP_New_For_IPD_Booking([FromBody] CompleteIPScreeningForIPDRequestModel model)
		{
			string message = "";
			access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
			obj_access_logsdetailsModel.action_method_name = "Complete_IP_New_For_IPD_Booking";
			try
			{
				var authHeader = HttpContext.Request.Headers["Authorization"][0];
				if (authHeader.StartsWith("Bearer "))
				{
					var token = authHeader.Substring("Bearer ".Length).Trim();
					var userdetails = obj_External_System_DAL.get_users_by_token(token);
					var modifieduser = userdetails[0].ToString();
					obj_access_logsdetailsModel.access_logsid = new Guid(userdetails[1].ToString());

					if (model == null || string.IsNullOrWhiteSpace(model.IPDApplicationFormid))
						message = "IPDApplicationFormid is required";
					else
						message = objOPDFormDAL.Complete_IP_New_Appointments_For_IPD(
							model.IPDApplicationFormid,
							modifieduser
						);
				}
				else
					message = "Invalid Token";
			}
			catch (Exception ex)
			{
				message = ex.Message;
				_logger.LogError(ex, "Complete_IP_New_For_IPD_Booking error: " + ex.Message);
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


		// -------------------------------------------------------------------------
		// RAZORPAY OPD PAYMENT
		// -------------------------------------------------------------------------

		/// <summary>
		/// Step 1 � Creates a Razorpay order for OPD consultation fee.
		/// Mirrors Create_Razorpay_Order in IPDApplicationFormController exactly.
		/// Resolves the actual task type name from the fee rows (e.g. "OP New", "OP Follow-up")
		/// and uses it as paymenttype on the PaymentRequest record.
		/// </summary>
		[HttpPost()]
		[ActionName("Create_OPD_Razorpay_Order")]
		public virtual IActionResult Create_OPD_Razorpay_Order([FromBody] NalamVazha.Models.OPDRazorpayOrderRequestModel model)
		{
			try
			{
				if (model == null || string.IsNullOrWhiteSpace(model.OPDFormid))
					return BadRequest("OPDFormid is required");
				if (!Guid.TryParse(model.OPDFormid, out var opdFormGuid))
					return BadRequest("Invalid OPDFormid.");

				var lockObj = _opdPaymentLocks.GetOrAdd(model.OPDFormid, _ => new object());
				lock (lockObj)
				{
				if (HasSuccessfulOPDPayment(opdFormGuid))
					return Conflict("This OPD consultation fee is already paid. Please refresh the page.");
				if (HasActiveOPDRazorpayRequest(opdFormGuid))
					return Conflict("A Razorpay payment is already in progress for this OPD booking. Please close the existing payment popup or wait a few minutes before trying again.");

				// 1. Get OPD payment details from the Receivable table (balance = amount - paidamount)
				var dataTable = objOPDFormDAL.Get_OPD_Consultation_Fee(model.OPDFormid);
				if (dataTable == null || dataTable.Rows.Count == 0)
					return NotFound("No outstanding receivable found for this OPD form");

				var row = dataTable.Rows[0];
				string tenantid = row["tenantid"]?.ToString();
				string patientprofileid = row["patientname"]?.ToString(); // patientname column = PatientProfileid UUID
				string bookingreferencenumber = row["bookingreferencenumber"]?.ToString();
				string customername = row["patientname_master"]?.ToString();
				string customerphone = row["mobilenumber"]?.ToString();
				if (IsCancelledOPDStatus(row))
					return Conflict("This OPD booking is cancelled. Payment cannot be initiated.");

				// Sum receivable balance across all rows for this OPD form
				decimal totalAmount = 0, totalPaid = 0;
				string resolvedTaskType = null;
				foreach (DataRow r in dataTable.Rows)
				{
					decimal.TryParse(r["amount"]?.ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var a);
					decimal.TryParse(r["paidamount"]?.ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var p);
					totalAmount += a;
					totalPaid += p;
					if (resolvedTaskType == null)
					{
						var rf = r["receivablefor"]?.ToString();
						if (!string.IsNullOrWhiteSpace(rf)) resolvedTaskType = rf;
					}
				}
				decimal consultationFee = totalAmount - totalPaid;
				if (string.IsNullOrWhiteSpace(resolvedTaskType))
					resolvedTaskType = "OPD Consultation Fee";

				if (consultationFee <= 0)
					return BadRequest("No outstanding balance � cannot create payment order");

				// 2. Get Razorpay config for tenant
				var objPaymentConfigDAL = new PaymentConfigDAL(_connectionSettings.Value.ConnectionString);
				var configTable = objPaymentConfigDAL.get_Payment_Config(tenantid, "RazorPay");
				if (configTable == null || configTable.Rows.Count == 0)
					return BadRequest("Razorpay payment config not found for tenant");

				string paymentConfigid = configTable.Rows[0]["paymentconfigid"]?.ToString();
				var keyinfoTable = objPaymentConfigDAL.getById_keyinfo(paymentConfigid);
				if (keyinfoTable == null || keyinfoTable.Rows.Count == 0)
					return BadRequest("Razorpay key info not configured");

				string keyid = keyinfoTable.Rows[0]["keyid"]?.ToString();
				string keysecret = keyinfoTable.Rows[0]["keysecret"]?.ToString();
				string merchantid = keyinfoTable.Rows[0]["merchantid"]?.ToString();

				// 3. Create Razorpay order via RestSharp
				long amountInPaise = Convert.ToInt64(consultationFee * 100);
				var rzpClient = new RestSharp.RestClient("https://api.razorpay.com/v1");
				var rzpRequest = new RestSharp.RestRequest("orders", RestSharp.Method.Post);
				rzpRequest.AddHeader("Authorization",
					"Basic " + Convert.ToBase64String(
						System.Text.Encoding.UTF8.GetBytes(keyid + ":" + keysecret)));
				rzpRequest.AddJsonBody(new
				{
					amount = amountInPaise,
					currency = "INR",
					receipt = bookingreferencenumber,
					notes = new { opdformid = model.OPDFormid }
				});
				var rzpResponse = rzpClient.Execute(rzpRequest);
				if (!rzpResponse.IsSuccessful)
					return BadRequest("Failed to create Razorpay order: " + rzpResponse.Content);

				var rzpOrder = JObject.Parse(rzpResponse.Content);
				string razorpayOrderId = rzpOrder["id"]?.ToString();

				// 4. Save PaymentRequest record � paymenttype = resolved task type name
				var objPaymentRequestDAL = new PaymentRequestDAL(_connectionSettings.Value.ConnectionString);
				var paymentRequestId = Guid.NewGuid();
				var paymentRequest = new NalamVazha.Models.PaymentRequestModel
				{
					PaymentRequestid = paymentRequestId,
					tenantid = Guid.TryParse(tenantid, out var tid) ? tid : (Guid?)null,
					paymentgateway = "RazorPay",
					requestdatetime = GetApplicationNow(),
					patientname = Guid.TryParse(patientprofileid, out var pid) ? pid : (Guid?)null,
					paymenttype = resolvedTaskType,
					merchantid = merchantid,
					orderid = razorpayOrderId,
					amount = consultationFee,
					currency = "INR",
					paymentid = BuildOPDPaymentRequestMarker(model.OPDFormid, paymentRequestId),
					customername = (customername).Trim(),
					customerphone = customerphone,
					orderpaymentdesc = resolvedTaskType + " - " + bookingreferencenumber,
					createduser = Guid.TryParse(pvar_usersid, out var uid) ? uid : (Guid?)null,
					craftmyapp_actionmethodname = "Add_Payment_Request"
				};
				objPaymentRequestDAL.Add_Payment_Request(paymentRequest);

				return Ok(new
				{
					razorpay_key_id = keyid,
					order_id = razorpayOrderId,
					amount = amountInPaise,
					currency = "INR",
					description = resolvedTaskType + " - " + bookingreferencenumber,
					name = (customername).Trim(),
					contact = customerphone,
					paymentrequestid = paymentRequest.PaymentRequestid.ToString()
				});
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Create_OPD_Razorpay_Order error: " + ex.Message);
				return BadRequest(ex.Message);
			}
		}

		/// <summary>
		/// Step 2 � Verifies Razorpay signature, saves PaymentResponse + BillingPayment.
		/// Mirrors Complete_IPD_Payment in IPDApplicationFormController exactly.
		/// paymenttype / paymentfor = resolved task type name from the OPD fee rows.
		/// </summary>
		[HttpPost()]
		[ActionName("Complete_OPD_Payment")]
		public virtual IActionResult Complete_OPD_Payment(
			[FromBody] NalamVazha.Models.OPDCompletePaymentModel model)
		{
			try
			{
				if (model == null || string.IsNullOrWhiteSpace(model.OPDFormid))
					return BadRequest("OPDFormid is required");
				if (!Guid.TryParse(model.OPDFormid, out var opdFormGuid))
					return BadRequest("Invalid OPDFormid.");

				var lockObj = _opdPaymentLocks.GetOrAdd(model.OPDFormid, _ => new object());
				lock (lockObj)
				{
				// 1. Fetch PaymentRequest to get tenantid + amount
				var objPaymentRequestDAL = new PaymentRequestDAL(_connectionSettings.Value.ConnectionString);
				var paymentReq = objPaymentRequestDAL.getById_PaymentRequest(model.paymentrequestid);
				if (paymentReq == null)
					return NotFound("Payment request record not found");
				if (!string.Equals(paymentReq.orderid, model.razorpay_order_id, StringComparison.OrdinalIgnoreCase))
					return BadRequest("Razorpay order does not match this payment request.");
				if (HasPaidPaymentResponse(model.razorpay_payment_id))
					return Ok(new { success = true, message = "201.1", duplicate = true });
				if (HasSuccessfulOPDPayment(opdFormGuid))
					return Conflict("This OPD consultation fee is already paid. Please refresh the page.");

				string tenantid = paymentReq.tenantid?.ToString();
				decimal amount = paymentReq.amount;
				var currentOpdTable = objOPDFormDAL.Get_OPD_Consultation_Fee(model.OPDFormid);
				if (currentOpdTable == null || currentOpdTable.Rows.Count == 0)
					return Conflict("This OPD consultation fee is no longer payable. Please refresh the page.");
				if (IsCancelledOPDStatus(currentOpdTable.Rows[0]))
					return Conflict("This OPD booking is cancelled. Payment cannot be completed.");
				decimal currentOutstanding = CalculateCurrentOPDOutstanding(currentOpdTable);
				if (currentOutstanding <= 0)
					return Conflict("This OPD consultation fee is already paid. Please refresh the page.");
				if (Math.Round(currentOutstanding, 2) != Math.Round(amount, 2))
					return Conflict("OPD payable amount has changed. Please refresh the page and start payment again.");

				// 2. Get Razorpay key secret for signature verification
				var objPaymentConfigDAL = new PaymentConfigDAL(_connectionSettings.Value.ConnectionString);
				var configTable = objPaymentConfigDAL.get_Payment_Config(tenantid, "RazorPay");
				if (configTable == null || configTable.Rows.Count == 0)
					return BadRequest("Razorpay config not found");

				string paymentConfigid = configTable.Rows[0]["paymentconfigid"]?.ToString();
				var keyinfoTable = objPaymentConfigDAL.getById_keyinfo(paymentConfigid);
				if (keyinfoTable == null || keyinfoTable.Rows.Count == 0)
					return BadRequest("Razorpay key info not configured");

				string keysecret = keyinfoTable.Rows[0]["keysecret"]?.ToString();

				// 3. Verify Razorpay HMAC-SHA256 signature
				string signaturePayload = model.razorpay_order_id + "|" + model.razorpay_payment_id;
				using (var hmac = new System.Security.Cryptography.HMACSHA256(
					System.Text.Encoding.UTF8.GetBytes(keysecret)))
				{
					var computedHash = hmac.ComputeHash(
						System.Text.Encoding.UTF8.GetBytes(signaturePayload));
					string computedSignature = BitConverter.ToString(computedHash)
						.Replace("-", "").ToLower();
					if (computedSignature != model.razorpay_signature?.ToLower())
						return Unauthorized("Payment signature verification failed");
				}

				string paymentMode = FetchRazorpayPaymentMode(keyinfoTable, keysecret, model.razorpay_payment_id);

				// 4. Save PaymentResponse
				var objPaymentResponseDAL = new PaymentResponseDAL(_connectionSettings.Value.ConnectionString);
				var paymentResponse = new NalamVazha.Models.PaymentResponseModel
				{
					PaymentResponseid = Guid.NewGuid(),
					tenantid = Guid.TryParse(tenantid, out var tid) ? tid : (Guid?)null,
					paymentrequest = paymentReq.PaymentRequestid,
					paymenttype = paymentReq.paymenttype, // carries resolved task type from Step 1
					transactiontime = GetApplicationNow(),
					orderid = model.razorpay_order_id,
					paymentid = model.razorpay_payment_id,
					status = "Paid",
					amount = amount,
					paymentmethod = paymentMode,
					responsesignature = model.razorpay_signature,
					createduser = Guid.TryParse(pvar_usersid, out var uid) ? uid : (Guid?)null,
					craftmyapp_actionmethodname = "Add_Payment_Response"
				};
				SavePaymentResponseOrThrow(objPaymentResponseDAL, paymentResponse);

				// 5. Get OPD details for billing record + re-resolve task type (from Receivable table)
				var opdTable = currentOpdTable;
				string patientGuid = opdTable?.Rows.Count > 0 ? opdTable.Rows[0]["patientname"]?.ToString() : null;
				string bookingreferencenumber = opdTable?.Rows.Count > 0 ? opdTable.Rows[0]["bookingreferencenumber"]?.ToString() : "";
				string patientvisitidStr = opdTable?.Rows.Count > 0 ? opdTable.Rows[0]["patientvisit"]?.ToString() : null;

				// Resolve task type � paymentReq.paymenttype holds it from Step 1 (set from receivablefor)
				string resolvedTaskType = paymentReq.paymenttype;
				if (string.IsNullOrWhiteSpace(resolvedTaskType) && opdTable != null)
				{
					foreach (DataRow r in opdTable.Rows)
					{
						var rf = r["receivablefor"]?.ToString();
						if (!string.IsNullOrWhiteSpace(rf)) { resolvedTaskType = rf; break; }
					}
				}
				if (string.IsNullOrWhiteSpace(resolvedTaskType))
					resolvedTaskType = "OPD Consultation Fee";

				string billingRemarks =
					$"{resolvedTaskType}. Booking Ref: {bookingreferencenumber}. " +
					$"Amount Paid: {amount:N2}. " +
					$"Razorpay Order: {model.razorpay_order_id}. Payment ID: {model.razorpay_payment_id}.";

				// 7. Save BillingPayment record � paymentfor = resolved task type name
				var objBillingPaymentDAL = new BillingPaymentDAL(_connectionSettings.Value.ConnectionString);
				var billing = new NalamVazha.Models.BillingPaymentModel
				{
					BillingPaymentid = Guid.NewGuid(),
					tenantid = Guid.TryParse(tenantid, out var btid) ? btid : (Guid?)null,
					paymentdate = GetApplicationNow().Date,
					receivablefor = resolvedTaskType,
					patientvisit = Guid.TryParse(patientvisitidStr, out var pvid) ? pvid : (Guid?)null,
					opdnumber = Guid.TryParse(model.OPDFormid, out var opd) ? opd : (Guid?)null,
					patientname = Guid.TryParse(patientGuid, out var bpid) ? bpid : (Guid?)null,
					amount = amount,
					currency = "INR",
					paymentmode = paymentMode,
					transactionreference = model.razorpay_payment_id,
					paymentstatus = "Success",
					collectedby = Guid.TryParse(pvar_usersid, out var cbid) ? cbid : (Guid?)null,
					remarks = billingRemarks,
					createduser = Guid.TryParse(pvar_usersid, out var bcuid) ? bcuid : (Guid?)null,
					craftmyapp_actionmethodname = "Add_Billing_Payment"
				};
				var billingMessage = objBillingPaymentDAL.Add_Billing_Payment(billing);
				if (!(billingMessage ?? "").Replace("\"", "").Contains("201.1"))
					throw new Exception("Payment captured, but billing entry was not saved: " + billingMessage);

				// Update Receivable table to reflect the Razorpay payment
				if (billing.opdnumber.HasValue && billing.BillingPaymentid.HasValue)
				{
					objBillingPaymentDAL.UpdateOPDConsultationFeeReceivable(
						billing.opdnumber.Value,
						amount,
						billing.BillingPaymentid.Value);
				}

				return Ok(new { message = "201.1", billingpaymentid = billing.BillingPaymentid.ToString() });
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Complete_OPD_Payment error: " + ex.Message);
				return BadRequest(ex.Message);
			}
		}

		[HttpPost()]
		[ActionName("Fail_OPD_Payment")]
		public virtual IActionResult Fail_OPD_Payment([FromBody] OPDFailedPaymentModel model)
		{
			try
			{
				if (model == null || string.IsNullOrWhiteSpace(model.paymentrequestid))
					return BadRequest("paymentrequestid is required");

				var objPaymentRequestDAL = new PaymentRequestDAL(_connectionSettings.Value.ConnectionString);
				var paymentReq = objPaymentRequestDAL.getById_PaymentRequest(model.paymentrequestid);
				if (paymentReq == null)
					return NotFound("Payment request record not found");

				var objPaymentResponseDAL = new PaymentResponseDAL(_connectionSettings.Value.ConnectionString);
				var paymentResponse = new PaymentResponseModel
				{
					PaymentResponseid = Guid.NewGuid(),
					tenantid = paymentReq.tenantid,
					paymentrequest = paymentReq.PaymentRequestid,
					paymenttype = string.IsNullOrWhiteSpace(paymentReq.paymenttype) ? "OPD Consultation Fee" : paymentReq.paymenttype,
					transactiontime = GetApplicationTime(model.failed_at),
					orderid = string.IsNullOrWhiteSpace(model.razorpay_order_id) ? paymentReq.orderid : model.razorpay_order_id,
					paymentid = model.razorpay_payment_id,
					status = "Failed",
					amount = paymentReq.amount,
					gatewayresponsecode = model.error_code,
					gatewayresponsemessage = model.error_description,
					banktransactionid = model.error_step,
					responsesignature = model.error_reason,
					createduser = Guid.TryParse(pvar_usersid, out var uid) ? uid : (Guid?)null,
					craftmyapp_actionmethodname = "Add_Payment_Response"
				};
				SavePaymentResponseOrThrow(objPaymentResponseDAL, paymentResponse);
				return Ok(new { message = "201.1", paymentresponseid = paymentResponse.PaymentResponseid.ToString() });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Fail_OPD_Payment error: " + ex.Message);
				return BadRequest(ex.Message);
			}
		}

		[HttpPost()]
		[ActionName("Process_OPD_Razorpay_Refund")]
		public virtual IActionResult Process_OPD_Razorpay_Refund([FromBody] OPDRazorpayRefundRequestModel model)
		{
			try
			{
				if (model == null || string.IsNullOrWhiteSpace(model.OPDFormid))
					return BadRequest("OPDFormid is required");
				if (!Guid.TryParse(model.OPDFormid, out var opdGuid))
					return BadRequest("Invalid OPDFormid.");
				if (model.refundamount <= 0)
					return BadRequest("Refund amount must be greater than zero.");

				var refundLockObj = _opdRefundLocks.GetOrAdd(model.OPDFormid, _ => new object());
				lock (refundLockObj)
				{
				if (HasOPDRefundInitiated(opdGuid))
					return Conflict("This OPD refund is already initiated. Please refresh the page.");

				var billingDal = new BillingPaymentDAL(_connectionSettings.Value.ConnectionString);
				var billingTable = billingDal.Get_Billing_Payments_For_OPD(model.OPDFormid);
				if (billingTable == null || billingTable.Rows.Count == 0)
					return BadRequest("No OPD billing payment found to refund.");

				string razorpayPaymentId = null;
				decimal onlinePaidBalance = 0m;
				foreach (DataRow row in billingTable.Rows)
				{
					string mode = row.Table.Columns.Contains("paymentmode") ? row["paymentmode"]?.ToString() ?? "" : "";
					string txn = row.Table.Columns.Contains("transactionreference") ? row["transactionreference"]?.ToString() ?? "" : "";
					decimal amount = row.Table.Columns.Contains("amount") && row["amount"] != DBNull.Value ? Convert.ToDecimal(row["amount"]) : 0m;
					string receivableFor = row.Table.Columns.Contains("receivablefor") ? row["receivablefor"]?.ToString() ?? "" : "";
					string paymentStatus = row.Table.Columns.Contains("paymentstatus") ? row["paymentstatus"]?.ToString() ?? "" : "";
					bool isRefundRow = receivableFor.Equals("Cancellation Refund", StringComparison.OrdinalIgnoreCase) || amount < 0;
					bool isSuccessful = paymentStatus.Equals("Paid", StringComparison.OrdinalIgnoreCase)
						|| paymentStatus.Equals("Success", StringComparison.OrdinalIgnoreCase)
						|| paymentStatus.Equals("Completed", StringComparison.OrdinalIgnoreCase);
					bool isOnline = mode.IndexOf("Online", StringComparison.OrdinalIgnoreCase) >= 0
						|| mode.IndexOf("Razor", StringComparison.OrdinalIgnoreCase) >= 0
						|| txn.StartsWith("pay_", StringComparison.OrdinalIgnoreCase);
					if (isRefundRow)
					{
						onlinePaidBalance -= Math.Abs(amount);
					}
					else if (isSuccessful && isOnline && amount > 0)
					{
						onlinePaidBalance += amount;
						if (string.IsNullOrWhiteSpace(razorpayPaymentId) && txn.StartsWith("pay_", StringComparison.OrdinalIgnoreCase))
							razorpayPaymentId = txn;
					}
				}

				if (string.IsNullOrWhiteSpace(razorpayPaymentId))
					return BadRequest("No Razorpay payment reference found to process OPD refund.");
				if (model.refundamount > onlinePaidBalance)
					return BadRequest($"Refund amount ₹{model.refundamount:N2} cannot exceed online paid balance ₹{onlinePaidBalance:N2}.");

				long refundAmountInPaise = (long)Math.Round(model.refundamount * 100, MidpointRounding.AwayFromZero);
				if (refundAmountInPaise < 100)
					return BadRequest($"Refund amount ₹{model.refundamount:N2} is below the minimum refund amount of ₹1.00 supported by Razorpay.");

				var objPaymentConfigDAL = new PaymentConfigDAL(_connectionSettings.Value.ConnectionString);
				var configTable = objPaymentConfigDAL.get_Payment_Config(model.tenantid, "RazorPay");
				if (configTable == null || configTable.Rows.Count == 0)
					return BadRequest("Razorpay payment config not found for tenant.");

				string paymentConfigid = configTable.Rows[0]["paymentconfigid"]?.ToString();
				var keyinfoTable = objPaymentConfigDAL.getById_keyinfo(paymentConfigid);
				if (keyinfoTable == null || keyinfoTable.Rows.Count == 0)
					return BadRequest("Razorpay key info not configured.");

				string keyid = keyinfoTable.Rows[0]["keyid"]?.ToString();
				string keysecret = keyinfoTable.Rows[0]["keysecret"]?.ToString();
				var rzpClient = new RestSharp.RestClient("https://api.razorpay.com/v1");
				var rzpRequest = new RestSharp.RestRequest($"payments/{razorpayPaymentId}/refund", RestSharp.Method.Post);
				rzpRequest.AddHeader("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(keyid + ":" + keysecret)));
				rzpRequest.AddJsonBody(new
				{
					amount = refundAmountInPaise,
					speed = "normal",
					notes = new
					{
						opdformid = model.OPDFormid,
						reason = model.cancellationreason ?? "OPD Booking Cancelled"
					},
					receipt = "OPD-REFUND-" + model.OPDFormid
				});

				var rzpResponse = rzpClient.Execute(rzpRequest);
				if (!rzpResponse.IsSuccessful)
				{
					_logger.LogError("OPD Razorpay refund failed: " + rzpResponse.Content);
					return BadRequest("Razorpay refund failed: " + rzpResponse.Content);
				}

				var refundObj = JObject.Parse(rzpResponse.Content);
				string refundId = refundObj["id"]?.ToString();
				string refundStatus = refundObj["status"]?.ToString() ?? "pending";
				return Ok(new
				{
					message = "201.1",
					refundid = refundId,
					refundstatus = refundStatus,
					refundamount = model.refundamount
				});
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Process_OPD_Razorpay_Refund error: " + ex.Message);
				return BadRequest(ex.Message);
			}
		}

		private static DateTime GetApplicationNow()
		{
			string[] timeZoneIds = { "India Standard Time", "Asia/Kolkata" };
			foreach (var timeZoneId in timeZoneIds)
			{
				try
				{
					return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZoneId));
				}
				catch (TimeZoneNotFoundException) { }
				catch (InvalidTimeZoneException) { }
			}
			return DateTime.Now;
		}

		private static DateTime GetApplicationTime(string sourceTime)
		{
			if (!string.IsNullOrWhiteSpace(sourceTime)
				&& DateTimeOffset.TryParse(sourceTime, null, System.Globalization.DateTimeStyles.RoundtripKind, out var parsedTime))
			{
				string[] timeZoneIds = { "India Standard Time", "Asia/Kolkata" };
				foreach (var timeZoneId in timeZoneIds)
				{
					try
					{
						return TimeZoneInfo.ConvertTime(parsedTime, TimeZoneInfo.FindSystemTimeZoneById(timeZoneId)).DateTime;
					}
					catch (TimeZoneNotFoundException) { }
					catch (InvalidTimeZoneException) { }
				}
				return parsedTime.LocalDateTime;
			}

			return GetApplicationNow();
		}

		private static string BuildOPDPaymentRequestMarker(string opdFormId, Guid paymentRequestId)
		{
			return $"OPD:{opdFormId}:REQ:{paymentRequestId}";
		}

		private static bool IsCancelledOPDStatus(DataRow row)
		{
			string[] statusColumns = { "bookingstatus", "status", "appointmentstatus", "reviewstatus" };
			foreach (var columnName in statusColumns)
			{
				if (!row.Table.Columns.Contains(columnName)) continue;
				var status = row[columnName]?.ToString() ?? "";
				if (status.IndexOf("cancel", StringComparison.OrdinalIgnoreCase) >= 0)
					return true;
			}
			return false;
		}

		private static decimal CalculateCurrentOPDOutstanding(DataTable dataTable)
		{
			decimal totalAmount = 0, totalPaid = 0;
			foreach (DataRow row in dataTable.Rows)
			{
				decimal.TryParse(row["amount"]?.ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var amount);
				decimal.TryParse(row["paidamount"]?.ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var paid);
				totalAmount += amount;
				totalPaid += paid;
			}
			return totalAmount - totalPaid;
		}

		private void ReconcileOPDRazorpayBillingToReceivable(string opdFormId)
		{
			if (string.IsNullOrWhiteSpace(opdFormId) || !Guid.TryParse(opdFormId, out var opdGuid))
				return;

			using var conn = new NpgsqlConnection(_connectionSettings.Value.ConnectionString);
			conn.Open();

			decimal pendingBalance = 0m;
			using (var balanceCmd = new NpgsqlCommand("SELECT \"Get_OPD_Razorpay_Pending_Balance\"(@pvar_opdnumber)", conn))
			{
				balanceCmd.Parameters.AddWithValue("pvar_opdnumber", NpgsqlDbType.Uuid, opdGuid);
				pendingBalance = Convert.ToDecimal(balanceCmd.ExecuteScalar());
			}

			if (pendingBalance <= 0)
				return;

			var paidBillings = new List<(Guid id, decimal amount)>();
			using (var billingCmd = new NpgsqlCommand("SELECT * FROM \"Get_OPD_Razorpay_Paid_Billings\"(@pvar_opdnumber)", conn))
			{
				billingCmd.Parameters.AddWithValue("pvar_opdnumber", NpgsqlDbType.Uuid, opdGuid);
				using var reader = billingCmd.ExecuteReader();
				while (reader.Read())
					paidBillings.Add((reader.GetGuid(0), reader.GetDecimal(1)));
			}

			if (paidBillings.Count == 0)
			{
				var recoveredBilling = TryCreateMissingOPDRazorpayBilling(opdGuid);
				if (recoveredBilling.HasValue)
					paidBillings.Add(recoveredBilling.Value);
			}

			if (paidBillings.Count == 0)
				return;

			var billingDal = new BillingPaymentDAL(_connectionSettings.Value.ConnectionString);
			foreach (var billing in paidBillings)
			{
				if (pendingBalance <= 0) break;
				var applying = Math.Min(pendingBalance, billing.amount);
				billingDal.UpdateOPDConsultationFeeReceivable(opdGuid, applying, billing.id);
				pendingBalance -= applying;
			}
		}

		private (Guid id, decimal amount)? TryCreateMissingOPDRazorpayBilling(Guid opdGuid)
		{
			using var conn = new NpgsqlConnection(_connectionSettings.Value.ConnectionString);
			conn.Open();

			using var cmd = new NpgsqlCommand("SELECT * FROM \"Get_OPD_Missing_Razorpay_Billing_Source\"(@pvar_opdnumber)", conn);
			cmd.Parameters.AddWithValue("pvar_opdnumber", NpgsqlDbType.Uuid, opdGuid);

			using var reader = cmd.ExecuteReader();
			if (!reader.Read())
				return null;

			var tenantId = reader.IsDBNull(0) ? (Guid?)null : reader.GetGuid(0);
			var patientId = reader.IsDBNull(1) ? (Guid?)null : reader.GetGuid(1);
			var patientVisitId = reader.IsDBNull(2) ? (Guid?)null : reader.GetGuid(2);
			var receivableFor = reader.IsDBNull(3) ? "OPD Consultation Fee" : reader.GetString(3);
			var createdUser = reader.IsDBNull(4) ? (Guid?)null : reader.GetGuid(4);
			var amount = reader.GetDecimal(5);
			var paymentId = reader.IsDBNull(6) ? "" : reader.GetString(6);
			var orderId = reader.IsDBNull(7) ? "" : reader.GetString(7);
			reader.Close();

			var billingId = Guid.NewGuid();
			var billing = new BillingPaymentModel
			{
				BillingPaymentid = billingId,
				tenantid = tenantId,
				paymentdate = GetApplicationNow().Date,
				receivablefor = receivableFor,
				patientvisit = patientVisitId,
				opdnumber = opdGuid,
				patientname = patientId,
				amount = amount,
				currency = "INR",
				paymentmode = "Online - Razorpay",
				transactionreference = paymentId,
				paymentstatus = "Success",
				remarks = $"Recovered missing OPD billing entry from Razorpay response. Razorpay Order: {orderId}. Payment ID: {paymentId}.",
				createduser = createdUser,
				craftmyapp_actionmethodname = "Add_Billing_Payment"
			};

			var billingDal = new BillingPaymentDAL(_connectionSettings.Value.ConnectionString);
			var message = billingDal.Add_Billing_Payment(billing);
			if (!(message ?? "").Replace("\"", "").Contains("201.1"))
				throw new Exception("Unable to recover missing OPD Razorpay billing entry: " + message);

			return (billingId, amount);
		}

		private bool HasPaidPaymentResponse(string razorpayPaymentId)
		{
			if (string.IsNullOrWhiteSpace(razorpayPaymentId)) return false;
			using (var conn = new NpgsqlConnection(_connectionSettings.Value.ConnectionString))
			{
				conn.Open();
				using (var cmd = new NpgsqlCommand("SELECT \"OPD_Has_Paid_Payment_Response\"(@pvar_paymentid)", conn))
				{
					cmd.Parameters.AddWithValue("pvar_paymentid", NpgsqlDbType.Varchar, (object)razorpayPaymentId ?? DBNull.Value);
					return Convert.ToBoolean(cmd.ExecuteScalar());
				}
			}
		}

		private static string FetchRazorpayPaymentMode(DataTable keyinfoTable, string keysecret, string paymentId)
		{
			try
			{
				string keyid = keyinfoTable?.Rows.Count > 0 ? keyinfoTable.Rows[0]["keyid"]?.ToString() : "";
				if (string.IsNullOrWhiteSpace(keyid) || string.IsNullOrWhiteSpace(keysecret) || string.IsNullOrWhiteSpace(paymentId))
					return "Online - Razorpay";

				var rzpFetchClient = new RestSharp.RestClient("https://api.razorpay.com/v1");
				var fetchReq = new RestSharp.RestRequest("payments/" + paymentId, RestSharp.Method.Get);
				fetchReq.AddHeader("Authorization", "Basic " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(keyid + ":" + keysecret)));
				var fetchResp = rzpFetchClient.Execute(fetchReq);
				if (fetchResp.IsSuccessful)
				{
					var pData = JObject.Parse(fetchResp.Content);
					return MapRazorpayMethod(pData["method"]?.ToString());
				}
			}
			catch { }

			return "Online - Razorpay";
		}

		private static string MapRazorpayMethod(string method)
		{
			return (method ?? "").ToLowerInvariant() switch
			{
				"upi" => "Online - UPI",
				"netbanking" => "Online - Net Banking",
				"card" => "Online - Card",
				"wallet" => "Online - Wallet",
				_ => "Online - Razorpay"
			};
		}

		private static void SavePaymentResponseOrThrow(PaymentResponseDAL paymentResponseDal, PaymentResponseModel paymentResponse)
		{
			var responseMessage = paymentResponseDal.Add_Payment_Response(paymentResponse);
			if (!(responseMessage ?? "").Replace("\"", "").Contains("201.1"))
				throw new Exception("Payment response was not saved: " + responseMessage);
		}

		private bool HasSuccessfulOPDPayment(Guid opdFormId)
		{
			using (var conn = new NpgsqlConnection(_connectionSettings.Value.ConnectionString))
			{
				conn.Open();
				using (var cmd = new NpgsqlCommand("SELECT \"OPD_Has_Successful_Payment\"(@pvar_opdnumber)", conn))
				{
					cmd.Parameters.AddWithValue("pvar_opdnumber", NpgsqlDbType.Uuid, opdFormId);
					return Convert.ToBoolean(cmd.ExecuteScalar());
				}
			}
		}

		private bool HasOPDRefundInitiated(Guid opdFormId)
		{
			using (var conn = new NpgsqlConnection(_connectionSettings.Value.ConnectionString))
			{
				conn.Open();
				using (var cmd = new NpgsqlCommand("SELECT \"OPD_Has_Refund_Initiated\"(@pvar_opdnumber)", conn))
				{
					cmd.Parameters.AddWithValue("pvar_opdnumber", NpgsqlDbType.Uuid, opdFormId);
					return Convert.ToBoolean(cmd.ExecuteScalar());
				}
			}
		}

		private bool HasActiveOPDRazorpayRequest(Guid opdFormId)
		{
			using (var conn = new NpgsqlConnection(_connectionSettings.Value.ConnectionString))
			{
				conn.Open();
				using (var cmd = new NpgsqlCommand("SELECT \"OPD_Has_Active_Razorpay_Request\"(@pvar_opdnumber,@pvar_activeafter)", conn))
				{
					cmd.Parameters.AddWithValue("pvar_opdnumber", NpgsqlDbType.Uuid, opdFormId);
					cmd.Parameters.AddWithValue("pvar_activeafter", NpgsqlDbType.Timestamp, GetApplicationNow().AddMinutes(-15));
					return Convert.ToBoolean(cmd.ExecuteScalar());
				}
			}
		}

	}


}
