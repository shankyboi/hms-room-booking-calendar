namespace NalamVazhaWebApi.Controllers
			{
				using System;
				using System.Collections.Concurrent;
				using System.Data;
				using System.Linq;
                using Newtonsoft.Json.Linq;
				using Microsoft.AspNetCore.Mvc;
				using System.Collections.Generic;
				using Microsoft.Extensions.Options;
				using Microsoft.Extensions.Logging;
	using FluentValidation.Results;
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
	using RestSharp;
	using Npgsql;
	using static NalamVazha.DAL.IPDApplicationFormDAL;
	[Route("api/[controller]/[action]")]
				//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:02
				public class IPDApplicationFormController : BaseController
				{
				    public IPDApplicationFormController(IHttpContextAccessor httpContextAccessor,IOptions<ConnectionSettings> connectionSettings, ILoggerFactory loggerFactory, IConfiguration configuration,IWebHostEnvironment hostingEnvironment)
				    {
					     _configuration = configuration;
					     _logger = loggerFactory.CreateLogger<IPDApplicationFormController>();
					     _connectionSettings = connectionSettings;
					     objIPDApplicationFormDAL = new IPDApplicationFormDAL(_connectionSettings.Value.ConnectionString);
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
				private IPDApplicationFormDAL objIPDApplicationFormDAL;
                private External_System_DAL obj_External_System_DAL;
				private IOptions<ConnectionSettings> _connectionSettings;
				private ILogger _logger;
				private IConfiguration _configuration;
				private IWebHostEnvironment hostingEnv;
				private static readonly ConcurrentDictionary<string, object> _ipdPaymentLocks = new ConcurrentDictionary<string, object>();
				private static readonly ConcurrentDictionary<string, object> _ipdCancellationLocks = new ConcurrentDictionary<string, object>();
                private ExternalSystemUtility objExternalSystemUtitlity;

                private IPDNotificationHelper CreateNotifier() =>
                    new IPDNotificationHelper(
						_connectionSettings.Value.ConnectionString,
						_logger,
						_configuration["approvalurl"] ?? "");
                private string pvar_tenantid="|";
                private string pvar_usersid="";

                private static string ValidateDirectPlannedAdmissionDate(IPDApplicationFormModel model)
                {
                    if (!string.Equals(model?.verifiedstatus, "Direct Admission", StringComparison.OrdinalIgnoreCase))
                        return null;

                    var plannedDate = model.plannedadmissionstartdate
                        ?? model.preferreddatesofadmission?.OrderBy(x => x.record_order).FirstOrDefault()?.dateofarrival;

                    if (!plannedDate.HasValue)
                        return "Planned admission start date is required for Direct IPD.";

                    return plannedDate.Value.Date < DateTime.Today
                        ? "Planned admission start date must be today or a future date."
                        : null;
                }

			[HttpGet]
			public IActionResult Get_Booking_Emergency_Contacts(Guid IPDApplicationFormid)
			{
				return Ok(new BookingEmergencyContactDAL(_connectionSettings.Value.ConnectionString).Get("IPDApplicationForm", IPDApplicationFormid));
			}

			    [HttpGet()]
			  [ActionName("getById_preferreddatesofadmission")]
			  public virtual System.Data.DataTable getById_preferreddatesofadmission(string IPDApplicationFormid)
			  {
					DataTable dtIPDApplicationForm_preferreddatesofadmission = new DataTable();
					try
					{
						dtIPDApplicationForm_preferreddatesofadmission = objIPDApplicationFormDAL.getById_preferreddatesofadmission(IPDApplicationFormid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_preferreddatesofadmission, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
    
					}
					return dtIPDApplicationForm_preferreddatesofadmission;


			  }

			[HttpGet, ActionName("getById_attendantpreferreddates")]
			public virtual DataTable getById_attendantpreferreddates(string IPDApplicationFormid)
			{
				return objIPDApplicationFormDAL.getById_attendantpreferreddates(IPDApplicationFormid);
			}

			[HttpGet, ActionName("getById_attendantroompreference")]
			public virtual DataTable getById_attendantroompreference(string IPDApplicationFormid)
			{
				return objIPDApplicationFormDAL.getById_attendantroompreference(IPDApplicationFormid);
			}

[HttpGet()]
			  [ActionName("get_Active_IPD_Application")]
			  public virtual object get_Active_IPD_Application(string patientprofileid)
			  {
					try
					{
						var dt = objIPDApplicationFormDAL.get_Active_IPD_Application(patientprofileid);
						bool hasActive = false;
						string ipdId = "";
						string bs = "";
						if (dt != null && dt.Rows.Count > 0)
						{
							var r = dt.Rows[0];
							hasActive = r["hasactiveipd"] != DBNull.Value && Convert.ToBoolean(r["hasactiveipd"]);
							ipdId = r["ipdapplicationformid"] == DBNull.Value ? "" : r["ipdapplicationformid"].ToString();
							bs = r["bookingstatus"] == DBNull.Value ? "" : r["bookingstatus"].ToString();
						}
						return new
						{
							validationSucceeded = true,
							hasActiveIPD = hasActive,
							ipdapplicationformid = ipdId,
							bookingstatus = bs
						};
					}
					catch (Exception ex)
					{
						_logger.LogError(ex,"An exception occurred in - get_Active_IPD_Application, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
						return new
						{
							validationSucceeded = false,
							hasActiveIPD = false,
							ipdapplicationformid = "",
							bookingstatus = "",
							message = "Unable to verify whether the patient has an active IPD application. Please retry."
						};
					}
			  }

[HttpGet()]
			  [ActionName("getById_medicalinfo")]
			  public virtual System.Data.DataTable getById_medicalinfo(string IPDApplicationFormid)
			  {
					DataTable dtIPDApplicationForm_medicalinfo = new DataTable();
					try
					{
						dtIPDApplicationForm_medicalinfo = objIPDApplicationFormDAL.getById_medicalinfo(IPDApplicationFormid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_medicalinfo, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
    
					}
					return dtIPDApplicationForm_medicalinfo;


			  }

[HttpGet()]
			  [ActionName("getById_medicationinfo")]
			  public virtual System.Data.DataTable getById_medicationinfo(string IPDApplicationFormid)
			  {
					DataTable dtIPDApplicationForm_medicationinfo = new DataTable();
					try
					{
						dtIPDApplicationForm_medicationinfo = objIPDApplicationFormDAL.getById_medicationinfo(IPDApplicationFormid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_medicationinfo, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
    
					}
					return dtIPDApplicationForm_medicationinfo;


			  }

[HttpGet()]
			  [ActionName("getById_medicalrecords")]
			  public virtual System.Data.DataTable getById_medicalrecords(string IPDApplicationFormid)
			  {
					DataTable dtIPDApplicationForm_medicalrecords = new DataTable();
					try
					{
						dtIPDApplicationForm_medicalrecords = objIPDApplicationFormDAL.getById_medicalrecords(IPDApplicationFormid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_medicalrecords, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
    
					}
					return dtIPDApplicationForm_medicalrecords;


			  }

[HttpGet()]
			  [ActionName("getById_attendantinfo")]
			  public virtual System.Data.DataTable getById_attendantinfo(string IPDApplicationFormid)
			  {
					DataTable dtIPDApplicationForm_attendantinfo = new DataTable();
					try
					{
						dtIPDApplicationForm_attendantinfo = objIPDApplicationFormDAL.getById_attendantinfo(IPDApplicationFormid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_attendantinfo, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
    
					}
					return dtIPDApplicationForm_attendantinfo;


			  }

[HttpGet()]
			  [ActionName("getById_roompreference")]
			  public virtual System.Data.DataTable getById_roompreference(string IPDApplicationFormid)
			  {
					DataTable dtIPDApplicationForm_roompreference = new DataTable();
					try
					{
						dtIPDApplicationForm_roompreference = objIPDApplicationFormDAL.getById_roompreference(IPDApplicationFormid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_roompreference, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
    
					}
					return dtIPDApplicationForm_roompreference;


			  }

[HttpGet()]
			  [ActionName("getById_room")]
			  public virtual System.Data.DataTable getById_room(string IPDApplicationFormid)
			  {
					DataTable dtIPDApplicationForm_room = new DataTable();
					try
					{
						dtIPDApplicationForm_room = objIPDApplicationFormDAL.getById_room(IPDApplicationFormid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_room, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
    
					}
					return dtIPDApplicationForm_room;


			  }


            [HttpPost()]
            [ActionName("Add_IPD_Application_Form")]
            public virtual IActionResult Add_IPD_Application_Form([FromBody]IPDApplicationFormModel model)
            { 
              string message = "";

                var plannedAdmissionError = ValidateDirectPlannedAdmissionDate(model);
                if (!string.IsNullOrWhiteSpace(plannedAdmissionError))
                    return BadRequest(plannedAdmissionError);
                
                access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Add_IPD_Application_Form";
            try{
            var isDraft = string.Equals(model.bookingstatus, "Draft", StringComparison.OrdinalIgnoreCase);
            if (isDraft)
                ModelState.Clear();

            if (ModelState.IsValid)
            {

            	IPDApplicationFormModelValidator validator = new IPDApplicationFormModelValidator();
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

		       
                                 

                                
            		                
									 if (!model.IPDApplicationFormid.HasValue) model.IPDApplicationFormid = Guid.NewGuid();
									 message = objIPDApplicationFormDAL.Add_IPD_Application_Form(model);
                                     if (message == "201.1" && model.IPDApplicationFormid.HasValue)
                                         new BookingEmergencyContactDAL(_connectionSettings.Value.ConnectionString).Save("IPDApplicationForm", model.IPDApplicationFormid.Value, model.bookingemergencycontactsjson);
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

            	_logger.LogError("IPDApplicationFormModel - Add_IPD_Application_Form , Validation Error :" + message);
            	message = ("Validation Error : " + message);
            }






            }catch(Exception ex){
               message=ex.Message;
               _logger.LogError(ex,"An exception occurred in - Add_IPD_Application_Form, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
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
			  [ActionName("getById_IPDApplicationForm")]
			  public virtual IPDApplicationFormModel getById_IPDApplicationForm(string IPDApplicationFormid,string loginUserID="")
			  { 
				    IPDApplicationFormModel objIPDApplicationForm = new IPDApplicationFormModel();
					try
					{
						  objIPDApplicationForm = objIPDApplicationFormDAL.getById_IPDApplicationForm(IPDApplicationFormid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_IPDApplicationForm, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
            
					}
					return objIPDApplicationForm;

			  }
			  [HttpPost()]
			  [ActionName("Allot_Room")]
			  public virtual IActionResult Allot_Room([FromBody]IPDApplicationFormModel model)
			  { 
				    string message = "";
                   access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Allot_Room";

					try{

					if (ModelState.IsValid)
					{

						IPDApplicationFormModelValidator validator = new IPDApplicationFormModelValidator();
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

		       
                                 
		       
                                	
								var statusBeforeRoomAllotment = model.IPDApplicationFormid.HasValue
									? objIPDApplicationFormDAL.getById_IPDApplicationForm(
										model.IPDApplicationFormid.Value.ToString())?.bookingstatus
									: "";

							    message = objIPDApplicationFormDAL.Allot_Room(model);

								if (message.Replace("\"", "") == "201.1" && model.IPDApplicationFormid.HasValue)
								{
									var statusAfterRoomAllotment = objIPDApplicationFormDAL.getById_IPDApplicationForm(
										model.IPDApplicationFormid.Value.ToString())?.bookingstatus;
									if (!string.Equals(statusBeforeRoomAllotment, statusAfterRoomAllotment, StringComparison.OrdinalIgnoreCase)
										&& (string.Equals(statusAfterRoomAllotment, "Provisional Booking", StringComparison.OrdinalIgnoreCase)
											|| string.Equals(statusAfterRoomAllotment, "Provisional Confirmed", StringComparison.OrdinalIgnoreCase)))
									{
										CreateNotifier().SendBookingStatusNotification(
											model.IPDApplicationFormid.Value.ToString(), statusAfterRoomAllotment);
									}
								}

							// ── Generate Receivables after successful room allotment ─────────────────────
							if (message.Replace("\"", "") == "201.1" &&
								model.room != null && model.IPDApplicationFormid.HasValue)
							{
								var receivableDAL = new NalamVazha.DAL.ReceivableDAL(
									_connectionSettings.Value.ConnectionString);
								var roomDALForCost = new NalamVazha.DAL.RoomDAL(
									_connectionSettings.Value.ConnectionString);
								bool hasPackage = model.packagename.HasValue;
								bool packageReceivableCreated = false;
								Guid ipdId = model.IPDApplicationFormid.Value;
								Guid? tenantId = model.tenantid;
								Guid createdBy = model.modifieduser ?? Guid.Empty;

								//foreach (var r in model.room)
								//{
								//	if (!r.roomnumber.HasValue || !r.fromdate.HasValue ||
								//		!r.todate.HasValue || !r.costperday.HasValue)
								//		continue;

								//	bool isAttendant = string.Equals(
								//		r.allottedto, "Attendant",
								//		StringComparison.OrdinalIgnoreCase);

								//	// Always resolve cost from the allotted Room record:
								//	//   attendant row → Room.attendantcostperday
								//	//   patient row   → Room.costperday
								//	decimal effectiveCostPerDay = r.costperday.Value;
								//	try
								//	{
								//		var roomDetails = roomDALForCost.GetRoomCostFields(r.roomnumber.Value);
								//		if (roomDetails != null)
								//		{
								//			if (isAttendant && roomDetails.attendantcostperday.HasValue)
								//				effectiveCostPerDay = roomDetails.attendantcostperday.Value;
								//			else if (!isAttendant && roomDetails.costperday.HasValue)
								//				effectiveCostPerDay = roomDetails.costperday.Value;
								//		}
								//	}
								//	catch (Exception roomEx)
								//	{
								//		_logger.LogError(roomEx,
								//			"Allot_Room: failed to fetch room cost for room " +
								//			r.roomnumber + ", falling back to submitted value");
								//	}

								//	try
								//	{
								//		if (!isAttendant && hasPackage)
								//		{
								//			// ── Patient room + package ──────────────────────────────
								//			// 1. Create ONE package receivable for the full package cost
								//			if (!packageReceivableCreated)
								//			{
								//				receivableDAL.CreatePackageReceivable(
								//					ipdFormId: ipdId,
								//					packageId: model.packagename.Value,
								//					tenantId: tenantId,
								//					createdBy: createdBy);
								//				packageReceivableCreated = true;
								//			}

								//			// 2. If room not 100% covered, add per-day surcharge receivable
								//			decimal pct = r.percentagecovered.HasValue ? r.percentagecovered.Value : 100m;
								//			if (pct < 100m)
								//			{
								//				decimal surchargeCpd =
								//					Math.Round(effectiveCostPerDay * (100m - pct) / 100m, 2);
								//				if (surchargeCpd > 0)
								//				{
								//					receivableDAL.CreateRoomReceivables(
								//						ipdFormId: ipdId,
								//						roomId: r.roomnumber.Value,
								//						fromDate: r.fromdate.Value,
								//						toDate: r.todate.Value,
								//						costPerDay: surchargeCpd,
								//						isAttendant: false,
								//						tenantId: tenantId,
								//						createdBy: createdBy,
								//						remarks: "AllotRoom:PatientSurcharge");
								//				}
								//			}
								//		}
								//		else
								//		{
								//			// ── Attendant room, or patient room without package ─────
								//			receivableDAL.CreateRoomReceivables(
								//				ipdFormId: ipdId,
								//				roomId: r.roomnumber.Value,
								//				fromDate: r.fromdate.Value,
								//				toDate: r.todate.Value,
								//				costPerDay: effectiveCostPerDay,
								//				isAttendant: isAttendant,
								//				tenantId: tenantId,
								//				createdBy: createdBy);
								//		}
								//	}
								//	catch (Exception rxEx)
								//	{
								//		_logger.LogError(rxEx,
								//			"Allot_Room: receivable error for room " +
								//			r.roomnumber + ": " + rxEx.Message);
								//	}
								//}

								// ── Provisional Booking Deposit receivables ─────────────
								try
								{
									receivableDAL.CreateProvisionalBookingDepositReceivables(ipdId, tenantId, createdBy);
								}
								catch (Exception rxEx)
								{
									_logger.LogError(rxEx, "Allot_Room: Provisional Booking Deposit receivable error: " + rxEx.Message);
								}
							}
							// ─────────────────────────────────────────────────────────────



						}
						else
						{
                                message = "Invalid Token";
                                 
                            }
							
						}


					}
					else
					{
						var errorCollection = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
						message = errorCollection.ToString();

						_logger.LogError("IPDApplicationFormModel - Allot_Room, Validation Error :" + message);
					
						//return BadRequest("Validation Error : " + message);
					}






					}catch(Exception ex){
                        
						message=ex.Message;
					    _logger.LogError(ex,"An exception occurred in - Allot_Room, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
           
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
        [ActionName("Transfer_Room")]
        public virtual IActionResult Transfer_Room([FromBody] IPDApplicationFormModel model)
        {
            string message = "";
            access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
            obj_access_logsdetailsModel.action_method_name = "Transfer_Room";

            try
            {
                var authHeader = HttpContext.Request.Headers["Authorization"][0];
                if (!authHeader.StartsWith("Bearer "))
                    return Unauthorized("Invalid Token");

                var token = authHeader.Substring("Bearer ".Length);
                String[] userdetails = obj_External_System_DAL.get_users_by_token(token);
                model.modifieduser = new Guid(userdetails[0].ToString());
                obj_access_logsdetailsModel.access_logsid = new Guid(userdetails[1].ToString());

                if (model.IPDApplicationFormid == null || model.room == null || !model.room.Any())
                    return BadRequest("Transfer room details are required");

                var transferRoom = model.room.First();
                if (!transferRoom.roomnumber.HasValue || !transferRoom.fromdate.HasValue || !transferRoom.todate.HasValue || !transferRoom.costperday.HasValue)
                    return BadRequest("Room, transfer date, end date and cost per day are required");

                var transferDateValidation = ValidateTransferRoomDate(model.IPDApplicationFormid.Value, transferRoom);
                if (!string.IsNullOrWhiteSpace(transferDateValidation))
                    return BadRequest(transferDateValidation);

                message = objIPDApplicationFormDAL.Transfer_Room(model);

                if (message.Replace("\"", "") == "201.1")
                {
                    Guid ipdId = model.IPDApplicationFormid.Value;
                    Guid createdBy = model.modifieduser ?? Guid.Empty;
                    var transferRole = string.IsNullOrWhiteSpace(transferRoom.allottedto) ? "Patient" : transferRoom.allottedto.Trim();

                    bool hasPatient = transferRole.IndexOf("Patient", StringComparison.OrdinalIgnoreCase) >= 0;
                    bool hasAttendant = transferRole.IndexOf("Attendant", StringComparison.OrdinalIgnoreCase) >= 0;
                    bool isCombined = hasPatient && hasAttendant;
                    bool isAttendant = hasAttendant && !hasPatient;

                    // For combined transfers, normalize each role separately so that
                    // rows with allottedto = "Patient" or "Attendant" are both updated correctly
                    var rolesToNormalize = isCombined
                        ? new[] { "Patient", "Attendant" }
                        : new[] { transferRole };

                    string normalizeMessage = "201.1";
                    foreach (var singleRole in rolesToNormalize)
                    {
                        normalizeMessage = objIPDApplicationFormDAL.NormalizeTransferRoomDateRanges(
                            ipdId,
                            singleRole,
                            transferRoom.roomnumber.Value,
                            transferRoom.fromdate.Value,
                            transferRoom.todate.Value,
                            model.modifieduser);

                        if ((normalizeMessage ?? "").Replace("\"", "") != "201.1")
                        {
                            message = normalizeMessage;
                            break;
                        }
                    }

                    if ((normalizeMessage ?? "").Replace("\"", "") == "201.1")
                    {
                        var receivableDAL = new NalamVazha.DAL.ReceivableDAL(_connectionSettings.Value.ConnectionString);

                        // Invalidate Patient receivables from transfer date

                        var rxMessage = receivableDAL.SyncRoomReceivablesAfterTransferSimplePackageReprice(
                            ipdFormId: ipdId,
                            fromDate: transferRoom.fromdate.Value,
                            toDate: transferRoom.todate.Value,
                            newRoomId: transferRoom.roomnumber.Value,
                            newCostPerDay: transferRoom.costperday.Value,
                            transferRole: transferRole,
                            modifiedBy: createdBy
                        );

                        if ((rxMessage ?? "").Replace("\"", "") != "201.1")
                        {
                            message = rxMessage;
                        }


                        try
                        {
                            var roomDAL = new NalamVazha.DAL.RoomDAL(_connectionSettings.Value.ConnectionString);
                            var occupancyDAL = new NalamVazha.DAL.RoomOccupancyStatusDAL(_connectionSettings.Value.ConnectionString);
                            var newRoomDetails = roomDAL.getById_Room(transferRoom.roomnumber.Value.ToString());

                            if (newRoomDetails != null)
                            {
                                var transferDate = transferRoom.fromdate.Value.Date;
                                var toDate = transferRoom.todate.Value.Date;

                                // Release old occupancy records for selected role/date range
	                                var releaseMessage = occupancyDAL.Release_Room_Occupancy_For_Transfer(
	                                    ipdno: ipdId,
	                                    fromDate: transferDate,
	                                    toDate: toDate,
	                                    modifieduser: createdBy,
	                                    allottedTo: transferRole
	                                );

	                                if ((releaseMessage ?? "").Replace("\"", "") != "201.1")
	                                {
	                                    message = "Unable to release the previous room: " + releaseMessage;
	                                    _logger.LogError("Transfer_Room: previous room release failed. {ReleaseMessage}", releaseMessage);
	                                }
	                                else
	                                {
	                                    // Block new room only after the old room was released successfully.
	                                    Guid? patientNameId = objIPDApplicationFormDAL.GetPatientNameByIPD(ipdId);
	                                    Guid? patientVisitId = objIPDApplicationFormDAL.GetPatientVisitByIPD(ipdId);
	                                    Guid roomTenantId = newRoomDetails.tenantid ?? model.tenantid ?? Guid.Empty;

	                                    var occModel = new NalamVazha.Models.RoomOccupancyStatusModel
	                                    {
	                                        RoomOccupancyStatusid = Guid.NewGuid(),
	                                        tenantid = roomTenantId,
	                                        roomallocationno = "TRANSFER",
	                                        patientvisit = patientVisitId,
	                                        patientname = patientNameId,
	                                        ipdno = ipdId,
	                                        block = newRoomDetails.block,
	                                        building = newRoomDetails.building,
	                                        floor = newRoomDetails.floor,
	                                        room = transferRoom.roomnumber.Value,
	                                        bookeddate = transferDate,
	                                        status = "Occupied",
	                                        bookedfor = transferRole,
	                                        createduser = createdBy
	                                    };

	                                    var insertOccMessage = occupancyDAL.Insert_RoomAllocation_And_Occupancy_For_Transfer(
	                                        model: occModel,
	                                        fromDate: transferDate,
	                                        toDate: toDate,
	                                        allottedTo: transferRole
	                                    );

	                                    if ((insertOccMessage ?? "").Replace("\"", "") != "201.1")
	                                        message = "Unable to occupy the transferred room: " + insertOccMessage;
	                                }

	                            }
                        }
                        catch (Exception occEx)
                        {
                            _logger.LogError(occEx, "Transfer_Room: RoomOccupancyStatus update failed");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                _logger.LogError(ex, "An exception occurred in - Transfer_Room, Error Message : " +
                    (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace}" : ex.Message));
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


      


        private string ValidateTransferRoomDate(Guid ipdApplicationFormId, IPDApplicationForm_roomModel transferRoom)
        {
            var role = string.IsNullOrWhiteSpace(transferRoom.allottedto) ? "Patient" : transferRoom.allottedto.Trim();

            bool hasPatient = role.IndexOf("Patient", StringComparison.OrdinalIgnoreCase) >= 0;
            bool hasAttendant = role.IndexOf("Attendant", StringComparison.OrdinalIgnoreCase) >= 0;
            bool isCombined = hasPatient && hasAttendant;

            var rolesToValidate = isCombined
     ? new[] { "Patient", "Attendant" }
     : new[] { role };

            var roleStayRanges = new Dictionary<string, TransferStayRangeModel>();

            foreach (var singleRole in rolesToValidate)
            {
                var stayRange = objIPDApplicationFormDAL.GetIPDRoomStayRange(
                    ipdApplicationFormId,
                    singleRole
                );

                if (stayRange == null)
                    return "No " + singleRole + " stay details found.";

                var newRoomFromDate = transferRoom.fromdate.Value.Date;
                var newRoomToDate = transferRoom.todate.Value.Date;

                var minDate = stayRange.StayFrom.Date;
                var maxDate = stayRange.StayTo.Date;

                roleStayRanges[singleRole] = stayRange;

                if (newRoomFromDate > newRoomToDate)
                    return "Transfer date must be on or before Transfer Until date.";

                if (newRoomFromDate < minDate || newRoomFromDate > maxDate)
                {
                    return "Transfer date must be between " +
                           minDate.ToString("dd/MM/yyyy") +
                           " and " +
                           maxDate.ToString("dd/MM/yyyy") +
                           " for the " + singleRole + " stay.";
                }

                if (newRoomToDate != maxDate)
                {
                    return "Transfer Until date must be " +
                           maxDate.ToString("dd/MM/yyyy") +
                           " for the " + singleRole + " stay.";
                }
            }

            if (isCombined)
            {
                var patientRange = roleStayRanges["Patient"];
                var attendantRange = roleStayRanges["Attendant"];

                if (patientRange.StayFrom.Date != attendantRange.StayFrom.Date ||
                    patientRange.StayTo.Date != attendantRange.StayTo.Date)
                {
                    return "Sorry, Patient and Attendant cannot be transferred together because their stay dates are different. " +
                           "Patient stay: " + patientRange.StayFrom.ToString("dd/MM/yyyy") +
                           " to " + patientRange.StayTo.ToString("dd/MM/yyyy") + ". " +
                           "Attendant stay: " + attendantRange.StayFrom.ToString("dd/MM/yyyy") +
                           " to " + attendantRange.StayTo.ToString("dd/MM/yyyy") + ".";
                }
            }

            return "";
        }

        
        private class ActiveRoomAllocation
			  {
				  public Guid? RoomRowId { get; set; }
				  public DateTime FromDate { get; set; }
				  public DateTime ToDate { get; set; }
			  }

        [HttpPost()]
        [ActionName("Extend_Stay")]
        public virtual IActionResult Extend_Stay([FromBody] IPDApplicationFormModel model)
        {
            string message = "";
            access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
            obj_access_logsdetailsModel.action_method_name = "Extend_Stay";

            try
            {
                var authHeader = HttpContext.Request.Headers["Authorization"][0];

                if (!authHeader.StartsWith("Bearer "))
                    return Unauthorized("Invalid Token");

                var token = authHeader.Substring("Bearer ".Length);
                string[] userdetails = obj_External_System_DAL.get_users_by_token(token);

                model.modifieduser = new Guid(userdetails[0].ToString());
                obj_access_logsdetailsModel.access_logsid = new Guid(userdetails[1].ToString());

                if (model.IPDApplicationFormid == null || model.room == null || !model.room.Any())
                    return BadRequest("Extend stay details are required.");

                var extendRoom = model.room.First();

                if (!extendRoom.roomnumber.HasValue ||
                    !extendRoom.fromdate.HasValue ||
                    !extendRoom.todate.HasValue ||
                    !extendRoom.costperday.HasValue)
                {
                    return BadRequest("Room, extension from date, new checkout date and cost per day are required.");
                }

                if (extendRoom.todate.Value.Date < extendRoom.fromdate.Value.Date)
                    return BadRequest("New checkout date should be on or after extension from date.");

                string allottedTo = (extendRoom.allottedto ?? "").Trim();

                bool hasPatient = allottedTo.IndexOf("Patient", StringComparison.OrdinalIgnoreCase) >= 0;
                bool hasAttendant = allottedTo.IndexOf("Attendant", StringComparison.OrdinalIgnoreCase) >= 0;
                bool isCombined = hasPatient && hasAttendant;
                bool isAttendantOnly = hasAttendant && !hasPatient;

                if (!hasPatient && !hasAttendant)
                    return BadRequest("Please select Extend For.");

                Guid ipdId = model.IPDApplicationFormid.Value;
                Guid createdBy = model.modifieduser ?? Guid.Empty;

                // Validate before saving
                var validationMessage = ValidateExtendStayDateAndRole(ipdId, extendRoom);

                if (!string.IsNullOrWhiteSpace(validationMessage))
                    return BadRequest(validationMessage);

                // Save extend stay main data
                message = objIPDApplicationFormDAL.Extend_Stay(model);

				if ((message ?? "").Replace("\"", "").Trim() == "201.1")
				{
					var receivableDAL = new NalamVazha.DAL.ReceivableDAL(_connectionSettings.Value.ConnectionString);

					string extendRemarks = isCombined ? "ExtendStay:Patient, Attendant"
										 : isAttendantOnly ? "ExtendStay:Attendant"
														   : "ExtendStay:Patient";

					/*
                        Billing rule:
                        Patient only              -> patient room cost
                        Attendant only            -> attendant room cost
                        Patient + Attendant       -> patient room cost only
                    */

					if (hasPatient)
					{
						var rxRows = receivableDAL.CreateRoomReceivables(
							ipdFormId: ipdId,
							roomId: extendRoom.roomnumber.Value,
							fromDate: extendRoom.fromdate.Value,
							toDate: extendRoom.todate.Value,
							costPerDay: extendRoom.costperday.Value,
							isAttendant: false,
							tenantId: model.tenantid,
							createdBy: createdBy,
							remarks: extendRemarks
						);

						if (rxRows <= 0)
						{
							return BadRequest("Patient CreateRoomReceivables failed. No room receivable rows were created.");
						}
					}

					if (isAttendantOnly)
					{
						var rxRows = receivableDAL.CreateRoomReceivables(
							ipdFormId: ipdId,
							roomId: extendRoom.roomnumber.Value,
							fromDate: extendRoom.fromdate.Value,
							toDate: extendRoom.todate.Value,
							costPerDay: extendRoom.costperday.Value,
							isAttendant: true,
							tenantId: model.tenantid,
							createdBy: createdBy,
							remarks: extendRemarks
						);

						if (rxRows <= 0)
						{
							return BadRequest("Attendant CreateRoomReceivables failed. No room receivable rows were created.");
						}
					}


					try
					{
						string packageRepriceMessage =
							receivableDAL.RecalculateActiveRoomReceivablesByPackageAfterRoomChange(
								ipdFormId: ipdId,
								modifiedBy: createdBy
							);

						if ((packageRepriceMessage ?? "").Replace("\"", "").Trim() != "201.1")
						{
							return BadRequest(packageRepriceMessage);
						}
					}

                    catch (Exception pkgEx)
                    {
                        _logger.LogError(
                            pkgEx,
                            "Extend_Stay: Package room receivable recalculation failed"
                        );

                        return BadRequest(
                            "Extend stay saved, but package room cost recalculation failed: " + pkgEx.Message
                        );
                    }
                    try
                    {
                        InsertExtendStayOccupancy(model, extendRoom, ipdId, createdBy);
                    }
                    catch (Exception occEx)
                    {
                        _logger.LogError(occEx, "Extend_Stay: RoomOccupancyStatus insert failed");
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;

                _logger.LogError(
                    ex,
                    "An exception occurred in - Extend_Stay, Error Message : " +
                    (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace}" : ex.Message)
                );
            }

            if (obj_access_logsdetailsModel.access_logsid != null)
            {
                obj_access_logsdetailsModel.api_response = (message ?? "").Replace("\"", "");
                obj_External_System_DAL.create_access_logs_details(obj_access_logsdetailsModel);
            }

            if ((message ?? "").Replace("\"", "").Trim() == "201.1")
                return Ok(message);
            else if ((message ?? "").Replace("\"", "").Trim() == "401.1")
                return Unauthorized(message);
            else
                return BadRequest(message);
        }

  

        private string ValidateExtendStayDateAndRole(
    Guid ipdApplicationFormId,
    IPDApplicationForm_roomModel extendRoom)
        {
            string allottedTo = (extendRoom.allottedto ?? "").Trim();

            bool hasPatient = allottedTo.IndexOf("Patient", StringComparison.OrdinalIgnoreCase) >= 0;
            bool hasAttendant = allottedTo.IndexOf("Attendant", StringComparison.OrdinalIgnoreCase) >= 0;
            bool isCombined = hasPatient && hasAttendant;

            if (!hasPatient && !hasAttendant)
                return "Please select Extend For.";

            if (!extendRoom.fromdate.HasValue || !extendRoom.todate.HasValue)
                return "Extension from date and new checkout date are required.";

            DateTime extensionFrom = extendRoom.fromdate.Value.Date;
            DateTime newCheckout = extendRoom.todate.Value.Date;

            if (newCheckout < extensionFrom)
                return "New checkout date should be on or after extension from date.";

            DataTable currentRooms = objIPDApplicationFormDAL.getById_room(ipdApplicationFormId.ToString());

            if (currentRooms == null || currentRooms.Rows.Count == 0)
                return "No current room allocation found.";

            DateTime? patientToDate = null;
            DateTime? attendantToDate = null;

            foreach (DataRow row in currentRooms.Rows)
            {
                bool isDeleted = false;

                if (currentRooms.Columns.Contains("isdeleted") && row["isdeleted"] != DBNull.Value)
                    bool.TryParse(row["isdeleted"].ToString(), out isDeleted);

                if (isDeleted)
                    continue;

                string rowAllottedTo = "";

                if (currentRooms.Columns.Contains("allottedto") && row["allottedto"] != DBNull.Value)
                    rowAllottedTo = row["allottedto"].ToString();

                if (string.IsNullOrWhiteSpace(rowAllottedTo))
                    continue;

                DateTime rowToDate;

                if (!currentRooms.Columns.Contains("todate") ||
                    row["todate"] == DBNull.Value ||
                    !DateTime.TryParse(row["todate"].ToString(), out rowToDate))
                {
                    continue;
                }

                bool rowHasPatient = rowAllottedTo.IndexOf("Patient", StringComparison.OrdinalIgnoreCase) >= 0;
                bool rowHasAttendant = rowAllottedTo.IndexOf("Attendant", StringComparison.OrdinalIgnoreCase) >= 0;

                if (hasPatient && rowHasPatient)
                {
                    if (!patientToDate.HasValue || rowToDate.Date > patientToDate.Value.Date)
                        patientToDate = rowToDate.Date;
                }

                if (hasAttendant && rowHasAttendant)
                {
                    if (!attendantToDate.HasValue || rowToDate.Date > attendantToDate.Value.Date)
                        attendantToDate = rowToDate.Date;
                }
            }

            if (hasPatient && !patientToDate.HasValue)
                return "No active Patient room allocation found.";

            if (hasAttendant && !attendantToDate.HasValue)
                return "No active Attendant room allocation found.";

            if (isCombined)
            {
                if (patientToDate.Value.Date != attendantToDate.Value.Date)
                {
                    return "Cannot extend Patient and Attendant together because their current stay dates are different. Please extend them separately or align their stay dates first.";
                }

                DateTime expectedExtensionFrom = patientToDate.Value.Date.AddDays(1);

                if (extensionFrom != expectedExtensionFrom)
                {
                    return "Extension From Date must be the next day after the common Patient and Attendant checkout date.";
                }
            }
            else if (hasPatient)
            {
                DateTime expectedExtensionFrom = patientToDate.Value.Date.AddDays(1);

                if (extensionFrom != expectedExtensionFrom)
                {
                    return "Extension From Date must be the next day after Patient checkout date.";
                }
            }
            else if (hasAttendant)
            {
                DateTime expectedExtensionFrom = attendantToDate.Value.Date.AddDays(1);

                if (extensionFrom != expectedExtensionFrom)
                {
                    return "Extension From Date must be the next day after Attendant checkout date.";
                }
            }

            return "";
        }
        private void InsertExtendStayOccupancy(
    IPDApplicationFormModel model,
    IPDApplicationForm_roomModel extendRoom,
    Guid ipdId,
    Guid createdBy)
        {
            var roomDAL = new NalamVazha.DAL.RoomDAL(_connectionSettings.Value.ConnectionString);
            var occupancyDAL = new NalamVazha.DAL.RoomOccupancyStatusDAL(_connectionSettings.Value.ConnectionString);
            var allocationDAL = new NalamVazha.DAL.RoomAllocationDAL(_connectionSettings.Value.ConnectionString);

            var roomDetails = roomDAL.getById_Room(extendRoom.roomnumber.Value.ToString());

            if (roomDetails == null)
                return;

            DateTime extensionFrom = extendRoom.fromdate.Value.Date;
            DateTime newToDate = extendRoom.todate.Value.Date;

            Guid? patientNameId = objIPDApplicationFormDAL.GetPatientNameByIPD(ipdId);
            Guid? patientVisitId = objIPDApplicationFormDAL.GetPatientVisitByIPD(ipdId);

            Guid? allocationId = allocationDAL.UpdateToDateAndGetId(
                ipdId,
                extendRoom.roomnumber.Value,
                extendRoom.allottedto ?? "",
                newToDate,
                createdBy
            );

            Guid roomTenantId = Guid.Empty;

            if (roomDetails.tenantid.HasValue && roomDetails.tenantid.Value != Guid.Empty)
                roomTenantId = roomDetails.tenantid.Value;
            else if (model.tenantid.HasValue && model.tenantid.Value != Guid.Empty)
                roomTenantId = model.tenantid.Value;
            string allottedTo = string.IsNullOrWhiteSpace(extendRoom.allottedto)
    ? "Patient"
    : extendRoom.allottedto.Trim();
            int totalDays = Math.Max(1, (int)(newToDate - extensionFrom).TotalDays + 1);

            var occModel = new NalamVazha.Models.RoomOccupancyStatusModel
            {
                RoomOccupancyStatusid = Guid.NewGuid(),
                tenantid = roomTenantId,

                // This will be used as prefix/reference.
                // If allocationId is not needed, you can use "EXTEND".
                roomallocationno = allocationId.HasValue ? allocationId.Value.ToString() : "EXTEND",

                patientvisit = patientVisitId,
                patientname = patientNameId,
                ipdno = ipdId,

                block = roomDetails.block,
                building = roomDetails.building,
                floor = roomDetails.floor,
                room = extendRoom.roomnumber.Value,

                // Only start date is needed here.
                // SP will internally insert each date until newToDate.
                bookeddate = extensionFrom,

                status = "Occupied",
                bookedfor = allottedTo,
                createduser = model.modifieduser
            };

            var insertOccMessage = occupancyDAL.Insert_RoomAllocation_And_Occupancy_For_Transfer(
                model: occModel,
                fromDate: extensionFrom,
                toDate: newToDate,
                allottedTo: allottedTo
            );

        }
        [HttpPost()]
			  [ActionName("Update_IPD_Application_Form")]
			  public virtual IActionResult Update_IPD_Application_Form([FromBody]IPDApplicationFormModel model)
			  { 
				    string message = "";
                    var plannedAdmissionError = ValidateDirectPlannedAdmissionDate(model);
                    if (!string.IsNullOrWhiteSpace(plannedAdmissionError))
                        return BadRequest(plannedAdmissionError);
                   access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Update_IPD_Application_Form";

					try{

					var isDraft = string.Equals(model.bookingstatus, "Draft", StringComparison.OrdinalIgnoreCase);
					if (isDraft)
						ModelState.Clear();

					if (ModelState.IsValid)
					{

						IPDApplicationFormModelValidator validator = new IPDApplicationFormModelValidator();
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

		       
                                 
		       
                                	
							    message = objIPDApplicationFormDAL.Update_IPD_Application_Form(model);
                                if (message == "201.1" && model.IPDApplicationFormid.HasValue)
                                    new BookingEmergencyContactDAL(_connectionSettings.Value.ConnectionString).Save("IPDApplicationForm", model.IPDApplicationFormid.Value, model.bookingemergencycontactsjson);
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

						_logger.LogError("IPDApplicationFormModel - Update_IPD_Application_Form, Validation Error :" + message);
					
						//return BadRequest("Validation Error : " + message);
					}






					}catch(Exception ex){
                        
						message=ex.Message;
					    _logger.LogError(ex,"An exception occurred in - Update_IPD_Application_Form, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
           
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
			  [ActionName("Update_IPD_Application_Medical_Info")]
			  public virtual IActionResult Update_IPD_Application_Medical_Info([FromBody]IPDApplicationFormModel model)
			  {
				    string message = "";
                   access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Update_IPD_Application_Medical_Info";

					try{
						if (model == null || model.IPDApplicationFormid == null || model.IPDApplicationFormid == Guid.Empty)
						{
							message = "IPDApplicationFormid is required";
						}
						else
						{
                            var authHeader = HttpContext.Request.Headers["Authorization"][0];
	                        if (authHeader.StartsWith("Bearer "))
	                        {
                                var token = authHeader.Substring("Bearer ".Length);
		                        String[] userdetails=obj_External_System_DAL.get_users_by_token(token);
                                model.modifieduser=new Guid(userdetails[0].ToString());
                                obj_access_logsdetailsModel.access_logsid=new Guid(userdetails[1].ToString());
								model.craftmyapp_actionmethodname = "Update_IPD_Application_Medical_Info";

							    message = objIPDApplicationFormDAL.Update_IPD_Application_Medical_Info(model);
								if ((message ?? "").Replace("\"", "") == "201.1" && model.packagename.HasValue)
								{
									if (model.IPDApplicationFormid.HasValue && model.IPDApplicationFormid.Value != Guid.Empty)
									{
										var packageMessage = objIPDApplicationFormDAL.Update_IPD_Package_And_Booking_Deposit_Adjustment(
											model.IPDApplicationFormid.Value,
											model.packagename.HasValue && model.packagename.Value != Guid.Empty ? model.packagename : null,
											model.modifieduser ?? Guid.Empty);
										if (!(packageMessage ?? "").Replace("\"", "").Contains("201.1"))
											message = packageMessage;
									}
								}
							}
                            else{
                                message = "Invalid Token";
                            }
						}
					}catch(Exception ex){
						message=ex.Message;
					    _logger.LogError(ex,"An exception occurred in - Update_IPD_Application_Medical_Info, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
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
			  [ActionName("Update_IPD_Package_Info")]
			  public virtual IActionResult Update_IPD_Package_Info([FromBody] IPDPackageUpdateModel model)
			  {
					string message = "";
					try
					{
						var authHeader = HttpContext.Request.Headers["Authorization"][0];
						if (authHeader.StartsWith("Bearer "))
						{
							var token = authHeader.Substring("Bearer ".Length);
							String[] userdetails = obj_External_System_DAL.get_users_by_token(token);
							var modifiedUser = Guid.TryParse(userdetails[0]?.ToString(), out var parsedUser) ? parsedUser : Guid.Empty;
							var packageId = Guid.TryParse(model?.packagename, out var parsedPackage) ? parsedPackage : (Guid?)null;
							if (model == null || !Guid.TryParse(model.IPDApplicationFormid, out var ipdId))
								message = "Invalid IPDApplicationFormid";
							else
								message = objIPDApplicationFormDAL.Update_IPD_Package_And_Booking_Deposit_Adjustment(ipdId, packageId, modifiedUser);
						}
						else
						{
							message = "Invalid Token";
						}
					}
					catch (Exception ex)
					{
						message = ex.Message;
						_logger.LogError(ex, "An exception occurred in - Update_IPD_Package_Info, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace}" : ex.Message));
					}

					if (message.Replace("\"", "") == "201.1")
						return Ok(message);
					else if (message.Replace("\"", "") == "401.1")
						return Unauthorized(message);
					else
						return BadRequest(message);
			  }
[HttpGet()]
            public virtual async Task<string> Remove_IPD_Application_Form(string IPDApplicationFormid,string loginUserID="")
			{
					string message ="";
                    access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
                   obj_access_logsdetailsModel.action_method_name="Remove_IPD_Application_Form";

					try{
						
						  var authHeader = HttpContext.Request.Headers["Authorization"][0];
	                        if (authHeader.StartsWith("Bearer "))
	                        {
		                        
		                      
		                      
		                      var token = authHeader.Substring("Bearer ".Length);
		                         
		                        String[] userdetails=obj_External_System_DAL.get_users_by_token(token);
		                        loginUserID=userdetails[0].ToString();
                                obj_access_logsdetailsModel.access_logsid=new Guid(userdetails[1].ToString());
		       
                                 
                        	 message = objIPDApplicationFormDAL.Remove_IPD_Application_Form(IPDApplicationFormid,loginUserID);
						    }
	                        else{
		                        message = "Invalid Token";
		                       
	                        }
					 

					}catch(Exception ex){
						message=ex.Message;
                         _logger.LogError(ex,"An exception occurred in - Remove_IPD_Application_Form, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                         
					}
                    if(obj_access_logsdetailsModel.access_logsid !=null)
                    {
                         obj_access_logsdetailsModel.api_response=message.Replace("\"",""); 
                         obj_External_System_DAL.create_access_logs_details(obj_access_logsdetailsModel);
                    }
                 
					return message;

			}
		[HttpPost()]
		[ActionName("Confirm_Arrival")]
		public virtual IActionResult Confirm_Arrival([FromBody] IPDApplicationFormModel model)
		{
			string message = "";
			access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
			obj_access_logsdetailsModel.action_method_name = "Confirm_Arrival";

			try
			{
				 
					 
						var authHeader = HttpContext.Request.Headers["Authorization"][0];
						if (authHeader.StartsWith("Bearer "))
						{



							var token = authHeader.Substring("Bearer ".Length);
							String[] userdetails = obj_External_System_DAL.get_users_by_token(token);
							model.modifieduser = new Guid(userdetails[0].ToString());
							obj_access_logsdetailsModel.access_logsid = new Guid(userdetails[1].ToString());





							message = objIPDApplicationFormDAL.Confirm_Arrival(model);

							if (message.Replace("\"", "") == "201.1" && model.IPDApplicationFormid.HasValue)
							{
								try
								{
									var receivableDAL = new NalamVazha.DAL.ReceivableDAL(_connectionSettings.Value.ConnectionString);
									receivableDAL.CreateAdmissionFeeReceivable(
										model.IPDApplicationFormid.Value,
										model.tenantid,
										model.modifieduser ?? Guid.Empty);
								}
								catch (Exception rxEx)
								{
									_logger.LogError(rxEx, "Confirm_Arrival: Admission Fee receivable error: " + rxEx.Message);
								}
							}

						}
						else
						{
							message = "Invalid Token";

						}

					 
 






			}
			catch (Exception ex)
			{

				message = ex.Message;
				_logger.LogError(ex, "An exception occurred in - Confirm_Arrival, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

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

        [HttpGet()]
        [ActionName("Added_IPD_Application_Form")]
        public virtual JObject Added_IPD_Application_Form(
        string tenantid,
        string patientname = "",
        string bookingstatus = "",
        string verifiedstatus = "",
        string createddate_automatonfrom = "",
        string createddate_automatonto = "",
        string loginUserID = "",
        int? pagesize = 100,
        int? pagenumber = 0,
        string searchterm = "",
        string sort_fields = "",
        string bookingnumber = "", string workflowstatus = "", string financialstatus = "", string paymentmethod = "")
        {
            JObject jsonIPDApplicationForm = null;

            try
            {
                tenantid = pvar_tenantid;

                jsonIPDApplicationForm = objIPDApplicationFormDAL.Added_IPD_Application_Form(
                    tenantid,
                    patientname,
                    bookingstatus,
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
                _logger.LogError(ex,
                    "An exception occurred in - Added_IPD_Application_Form, Error Message : " +
                    (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));
            }

            return jsonIPDApplicationForm;
        }

        [HttpGet()]
			
			[ActionName("get_all_IPDApplicationForm")]
			public virtual System.Data.DataTable get_all_IPDApplicationForm(string tenantid,string searchterm="",string pagesize = "50", string pagenumber = "1",string loginUserID="")
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
				  	DataTable dtIPDApplicationForm = new DataTable();
					try
					{
                        tenantid=pvar_tenantid;
						dtIPDApplicationForm = objIPDApplicationFormDAL.get_all_IPDApplicationForm(tenantid,searchterm,pageSize,pageNumber);
					}
					catch (Exception ex)
					{
                          _logger.LogError(ex,"An exception occurred in - get_all_IPDApplicationForm, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
					    
					}
					return dtIPDApplicationForm;

			   }
[HttpGet()]
			
			[ActionName("count_of_IPDApplicationForm")]
			public virtual System.Data.DataTable count_of_IPDApplicationForm(string tenantid
,string patientname="", string bookingstatus = ""
)
			{
					 
				  	DataTable dtIPDApplicationForm = new DataTable();
					try
					{
                        tenantid=pvar_tenantid;
						dtIPDApplicationForm = objIPDApplicationFormDAL.count_of_IPDApplicationForm( tenantid
, patientname, bookingstatus
);
					}
					catch (Exception ex)
					{
                          _logger.LogError(ex,"An exception occurred in - count_of_IPDApplicationForm, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                         
					}
					return dtIPDApplicationForm;

			   }
		[HttpGet()]

		[ActionName("count_of_IPDApplicationForm_bookingstatus")]
		public virtual System.Data.DataTable count_of_IPDApplicationForm_bookingstatus(string tenantid
, string patientname = ""
, string bookingstatus = ""
)
		{

			DataTable dtIPDApplicationForm = new DataTable();
			try
			{
				tenantid = pvar_tenantid;
				dtIPDApplicationForm = objIPDApplicationFormDAL.count_of_IPDApplicationForm_bookingstatus(tenantid
, patientname
, bookingstatus
);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - count_of_IPDApplicationForm_bookingstatus, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

			}
			return dtIPDApplicationForm;

		}

		[HttpGet()]

        [ActionName("IPD_Application_Forms_for_Review")]
        public virtual JObject IPD_Application_Forms_for_Review(
    string tenantid,
    string patientname = "",
    string bookingstatus = "",
    string verifiedstatus = "",
    string createddate_automatonfrom = "",
    string createddate_automatonto = "",
    string loginUserID = "",
    int? pagesize = 100,
    int? pagenumber = 0,
    string searchterm = "",
    string sort_fields = "")
        {
					 
				  	JObject jsonIPDApplicationForm = null;
					try
					{
                        tenantid=pvar_tenantid;
                jsonIPDApplicationForm = objIPDApplicationFormDAL.IPD_Application_Forms_for_Review(
tenantid,
patientname,
bookingstatus,
verifiedstatus,
createddate_automatonfrom,
createddate_automatonto,
pagesize,
pagenumber,
searchterm,
sort_fields);
            }
					catch (Exception ex)
					{
                             _logger.LogError(ex,"An exception occurred in - IPD_Application_Forms_for_Review, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                        
					}
					return jsonIPDApplicationForm;

			   }
			   
  
            
		[HttpGet()]
		[AllowAnonymous]
		[ActionName("Get_IPD_Payment_Details")]
		public virtual IActionResult Get_IPD_Payment_Details(string IPDApplicationFormid)
		{
			try
			{
				var dataTable = objIPDApplicationFormDAL.Get_IPD_Payment_Details(IPDApplicationFormid);
				if (dataTable == null || dataTable.Rows.Count == 0)
					return NotFound("No IPD record found");

				var row = dataTable.Rows[0];
				string latestMobileNumber = GetLatestPatientMobileNumber(row);
				decimal packageBookingDeposit = row.Table.Columns.Contains("packagebookingdeposit") && row["packagebookingdeposit"] != DBNull.Value
					? Convert.ToDecimal(row["packagebookingdeposit"])
					: 0;

				decimal distinctPatientBookingDepositTotal = 0;
				if (dataTable.Columns.Contains("patient_booking_deposit"))
				{
					distinctPatientBookingDepositTotal = dataTable.AsEnumerable()
						.Where(r => r["patient_booking_deposit"] != DBNull.Value)
						.Select(r => Convert.ToDecimal(r["patient_booking_deposit"]))
						.Distinct()
						.Sum();
				}

				var result = new NalamVazha.Models.IPDPaymentDetailsModel
				{
					IPDApplicationFormid = row["ipdapplicationformid"]?.ToString(),
					bookingreferencenumber = row["bookingreferencenumber"]?.ToString(),
					firstname = row["firstname"]?.ToString(),
					lastname = row["lastname"]?.ToString(),
					mobilenumber = latestMobileNumber,
					packagename = row.Table.Columns.Contains("packagename") ? row["packagename"]?.ToString() : "",
					packagename_master = row["packagename_master"]?.ToString(),
					package_refundpolicy_json = row.Table.Columns.Contains("package_refundpolicy_json") ? row["package_refundpolicy_json"]?.ToString() : "",
					bookingdepositamount = packageBookingDeposit + distinctPatientBookingDepositTotal,
					bookingstatus = row["bookingstatus"]?.ToString(),
					tenantid = row["tenantid"]?.ToString(),
					patientname = row["patientname"]?.ToString(),
					blocked_room_details_json= row["blocked_room_details_json"]?.ToString(),
					patient_booking_deposit = row.Table.Columns.Contains("patient_booking_deposit") && row["patient_booking_deposit"] != DBNull.Value
						? Convert.ToDecimal(row["patient_booking_deposit"]) : 0,
					attendant_booking_deposit = row.Table.Columns.Contains("attendant_booking_deposit") && row["attendant_booking_deposit"] != DBNull.Value
						? Convert.ToDecimal(row["attendant_booking_deposit"]) : 0,
					advanceamount = row.Table.Columns.Contains("advanceamount") && row["advanceamount"] != DBNull.Value
						? Convert.ToDecimal(row["advanceamount"]) : 0,
					totalamount = row.Table.Columns.Contains("totalamount") && row["totalamount"] != DBNull.Value
						? Convert.ToDecimal(row["totalamount"]) : 0,
					daysofstay = row.Table.Columns.Contains("daysofstay") && row["daysofstay"] != DBNull.Value
						? Convert.ToInt32(row["daysofstay"]) : 0,
					accommodationtype = row.Table.Columns.Contains("accommodationtype") ? row["accommodationtype"]?.ToString() ?? "" : "",
					attendantcount = row.Table.Columns.Contains("attendantcount") && row["attendantcount"] != DBNull.Value
						? Convert.ToInt32(row["attendantcount"]) : 0,
					attendant_room_cost = row.Table.Columns.Contains("attendant_room_cost") && row["attendant_room_cost"] != DBNull.Value
						? Convert.ToDecimal(row["attendant_room_cost"]) : 0


				};
				// Populate booking deposit receivables from the Receivable table
				try
				{
					var receivablesDt = objIPDApplicationFormDAL.Get_IPD_BookingDeposit_Receivables(IPDApplicationFormid);
					var receivablesList = new List<object>();
					decimal receivableBalance = 0;
					foreach (DataRow r in receivablesDt.Rows)
					{
						var bal = Convert.ToDecimal(r["balance"]);
						receivableBalance += bal;
						receivablesList.Add(new {
							receivableid = r["receivableid"]?.ToString(),
							receivabledate = r["receivabledate"]?.ToString(),
							receivablefor = r["receivablefor"]?.ToString(),
							remarks = r["remarks"]?.ToString(),
							category = r["category"]?.ToString(),
							amount = Convert.ToDecimal(r["amount"]),
							paidamount = Convert.ToDecimal(r["paidamount"]),
							balance = bal,
							ismandatory = r["ismandatory"] != DBNull.Value && Convert.ToBoolean(r["ismandatory"])
						});
					}
					result.booking_deposit_receivables_json = JsonConvert.SerializeObject(receivablesList);
					result.booking_deposit_receivable_balance = receivableBalance;
				}
				catch (Exception exR)
				{
					_logger.LogWarning("Get_IPD_Payment_Details: receivable query failed: " + exR.Message);
				}

				return Ok(result);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Get_IPD_Payment_Details error: " + ex.Message);
				return BadRequest(ex.Message);
			}
		}

        [HttpPost()]
        [ActionName("Update_IPD_Booking_Status")]
        public virtual IActionResult Update_IPD_Booking_Status([FromBody] NalamVazha.Models.IPDBookingStatusUpdateModel model)
        {
            string message = "";

            try
            {
                if (!HttpContext.Request.Headers.ContainsKey("Authorization"))
                {
                    message = "Invalid Token";
                }
                else
                {
                    var authHeader = HttpContext.Request.Headers["Authorization"][0];

                    if (!authHeader.StartsWith("Bearer "))
                    {
                        message = "Invalid Token";
                    }
                    else
                    {
                        var token = authHeader.Substring("Bearer ".Length);
                        String[] userdetails = obj_External_System_DAL.get_users_by_token(token);

                        model.modifieduser = userdetails[0].ToString();

                        var currentIpd = objIPDApplicationFormDAL.getById_IPDApplicationForm(
                            model.IPDApplicationFormid
                        );
                        var previousBookingStatus = currentIpd?.bookingstatus ?? "";

						// Direct admission is an independent verification state. Room allotment
						// and booking-status updates must not leave it as "Approved".
						if (string.Equals(model.verifiedstatus, "Direct Admission", StringComparison.OrdinalIgnoreCase))
						{
							var verifiedStatusMessage = objIPDApplicationFormDAL.Update_IPD_VerifiedStatus(
								model.IPDApplicationFormid,
								"Direct Admission",
								model.modifieduser);

							if ((verifiedStatusMessage ?? "").Replace("\"", "") != "201.1")
								return BadRequest(verifiedStatusMessage);
						}

                        message = objIPDApplicationFormDAL.Update_IPD_BookingStatus(
                            model.IPDApplicationFormid,
                            model.bookingstatus,
                            model.modifieduser
                        );

						if ((message ?? "").Replace("\"", "") == "201.1"
							&& string.Equals(model.bookingstatus, "Provisional Booking", StringComparison.OrdinalIgnoreCase)
							&& !string.Equals(previousBookingStatus, "Provisional Booking", StringComparison.OrdinalIgnoreCase))
						{
							CreateNotifier().SendBookingStatusNotification(
								model.IPDApplicationFormid, "Provisional Booking");
						}

                        if ((message ?? "").Replace("\"", "") == "201.1"
                            && IsNotEligibleBookingStatus(model.bookingstatus)
                            && IsScreeningOrConsultationScheduled(previousBookingStatus))
                        {
                            var roomOccupancyStatusDal = new NalamVazha.DAL.RoomOccupancyStatusDAL(
                                _connectionSettings.Value.ConnectionString
                            );
                            var roomCancellationMessage = roomOccupancyStatusDal.Release_Room_On_Cancellation(
                                model.IPDApplicationFormid,
                                model.modifieduser
                            );

                            if ((roomCancellationMessage ?? "").Replace("\"", "") != "201.1")
                            {
                                _logger.LogWarning(
                                    "Update_IPD_Booking_Status: room cancellation failed for IPD {IPDId}: {Msg}",
                                    model.IPDApplicationFormid,
                                    roomCancellationMessage
                                );
                            }
                        }

                        if ((message ?? "").Replace("\"", "") == "201.1"
                            && string.Equals(model.bookingstatus ?? "", "Admission Approved", StringComparison.OrdinalIgnoreCase))
                        {
                            var screeningFeeMsg = objIPDApplicationFormDAL.Ensure_ScreeningFee_Receivable(
                                model.IPDApplicationFormid,
                                model.modifieduser
                            );

                            if (!(screeningFeeMsg ?? "").Replace("\"", "").Contains("201.1"))
                            {
                                _logger.LogWarning(
                                    "Update_IPD_Booking_Status: Screening Fee receivable creation failed: {Msg}",
                                    screeningFeeMsg
                                );
                            }
                        }

                        var effectiveVerifiedStatus = model.verifiedstatus ?? "";

                        if (string.IsNullOrWhiteSpace(effectiveVerifiedStatus))
                        {
                            try
                            {
                                var ipdDetails = objIPDApplicationFormDAL.getById_IPDApplicationForm(
                                    model.IPDApplicationFormid
                                );

                                effectiveVerifiedStatus = ipdDetails?.verifiedstatus ?? "";
                            }
                            catch (Exception statusEx)
                            {
                                _logger.LogWarning(
                                    statusEx,
                                    "Update_IPD_Booking_Status: unable to resolve verifiedstatus for IPD " + model.IPDApplicationFormid
                                );
                            }
                        }

                        if ((message ?? "").Replace("\"", "") == "201.1"
                            && string.Equals(model.bookingstatus ?? "", "Admitted", StringComparison.OrdinalIgnoreCase)
                            && !model.skiproomreceivables
                            && !string.Equals(effectiveVerifiedStatus, "Direct Admission", StringComparison.OrdinalIgnoreCase))
                        {
                            try
                            {
                                var ipdPayTable = objIPDApplicationFormDAL.Get_IPD_Payment_Details(
                                    model.IPDApplicationFormid
                                );

                                if (ipdPayTable != null && ipdPayTable.Rows.Count > 0)
                                {
                                    var ipdPayRow = ipdPayTable.Rows[0];

                                    Guid ipdId = Guid.Parse(model.IPDApplicationFormid);

                                    Guid? tenantId = Guid.TryParse(
                                        ipdPayRow["tenantid"]?.ToString(),
                                        out var tid
                                    )
                                        ? tid
                                        : (Guid?)null;

                                    Guid createdBy = Guid.TryParse(
                                        model.modifieduser,
                                        out var cid
                                    )
                                        ? cid
                                        : Guid.Empty;

                                    Guid pkgTest = Guid.Empty;

                                    bool hasPackage =
                                        ipdPayRow["packagename"] != DBNull.Value
                                        && Guid.TryParse(ipdPayRow["packagename"]?.ToString(), out pkgTest)
                                        && pkgTest != Guid.Empty;

                                    Guid packageId = hasPackage ? pkgTest : Guid.Empty;

                                    var receivableDAL = new NalamVazha.DAL.ReceivableDAL(
                                        _connectionSettings.Value.ConnectionString
                                    );

                                    var roomDALForCost = new NalamVazha.DAL.RoomDAL(
                                        _connectionSettings.Value.ConnectionString
                                    );

                                    bool packageReceivableCreated = false;

                                    int packageDays = 0;

                                    try
                                    {
                                        if (ipdPayRow.Table.Columns.Contains("noofdays")
                                            && ipdPayRow["noofdays"] != DBNull.Value)
                                        {
                                            packageDays = Convert.ToInt32(ipdPayRow["noofdays"]);
                                        }
                                        else if (ipdPayRow.Table.Columns.Contains("packagedays")
                                            && ipdPayRow["packagedays"] != DBNull.Value)
                                        {
                                            packageDays = Convert.ToInt32(ipdPayRow["packagedays"]);
                                        }
                                        else if (ipdPayRow.Table.Columns.Contains("numberofdays")
                                            && ipdPayRow["numberofdays"] != DBNull.Value)
                                        {
                                            packageDays = Convert.ToInt32(ipdPayRow["numberofdays"]);
                                        }
                                       
                                    }
                                    catch
                                    {
                                        packageDays = 0;
                                    }

                                    string blockedJson =
                                        ipdPayRow.Table.Columns.Contains("blocked_room_details_json")
                                        && ipdPayRow["blocked_room_details_json"] != DBNull.Value
                                            ? ipdPayRow["blocked_room_details_json"].ToString()
                                            : "[]";

                                    JArray rooms;

                                    try
                                    {
                                        rooms = JArray.Parse(blockedJson);
                                    }
                                    catch
                                    {
                                        rooms = new JArray();
                                    }

                                    var orderedRooms = rooms
                                        .OrderBy(r =>
                                        {
                                            DateTime parsedFrom;

                                            return DateTime.TryParse(
                                                r["fromdate"]?.ToString(),
                                                out parsedFrom
                                            )
                                                ? parsedFrom.Date
                                                : DateTime.MaxValue;
                                        })
                                        .ToList();

                                    int totalPatientBillingRoomDays = 0;

                                    foreach (var r in orderedRooms)
                                    {
                                        string roleText = r["allottedto"]?.ToString() ?? "";

                                        bool rowHasPatient =
                                            roleText.IndexOf("Patient", StringComparison.OrdinalIgnoreCase) >= 0;

                                        bool rowHasAttendant =
                                            roleText.IndexOf("Attendant", StringComparison.OrdinalIgnoreCase) >= 0;

                                        bool rowIsAttendantOnly = rowHasAttendant && !rowHasPatient;

                                        bool rowIsPatientBilling = rowHasPatient || !rowIsAttendantOnly;

                                        if (!rowIsPatientBilling)
                                        {
                                            continue;
                                        }

                                        DateTime rowFromDate;
                                        DateTime rowToDate;

                                        if (!DateTime.TryParse(r["fromdate"]?.ToString(), out rowFromDate))
                                        {
                                            continue;
                                        }

                                        if (!DateTime.TryParse(r["todate"]?.ToString(), out rowToDate))
                                        {
                                            continue;
                                        }

                                        if (rowToDate.Date < rowFromDate.Date)
                                        {
                                            continue;
                                        }

                                        totalPatientBillingRoomDays += Math.Max(
                                            1,
                                            (int)(rowToDate.Date - rowFromDate.Date).TotalDays + 1
                                        );
                                    }

                                    int remainingPackageDiscountDays = packageDays;

                                    foreach (var rm in orderedRooms)
                                    {
                                        string allottedTo = rm["allottedto"]?.ToString() ?? "";

                                        bool hasPatientRole =
                                            allottedTo.IndexOf("Patient", StringComparison.OrdinalIgnoreCase) >= 0;

                                        bool hasAttendantRole =
                                            allottedTo.IndexOf("Attendant", StringComparison.OrdinalIgnoreCase) >= 0;

                                        bool isAttendantOnly = hasAttendantRole && !hasPatientRole;

                                        bool isPatientBilling = hasPatientRole || !isAttendantOnly;

                                        Guid roomId;

                                        if (!Guid.TryParse(rm["roomid"]?.ToString(), out roomId))
                                        {
                                            continue;
                                        }

                                        DateTime fromDate;
                                        DateTime toDate;

                                        if (!DateTime.TryParse(rm["fromdate"]?.ToString(), out fromDate))
                                        {
                                            continue;
                                        }

                                        if (!DateTime.TryParse(rm["todate"]?.ToString(), out toDate))
                                        {
                                            continue;
                                        }

                                        if (toDate.Date < fromDate.Date)
                                        {
                                            continue;
                                        }

                                        decimal effectiveCostPerDay = 0m;

                                        try
                                        {
                                            var roomDetails = roomDALForCost.GetRoomCostFields(roomId);

                                            if (roomDetails != null)
                                            {
                                                if (isAttendantOnly && roomDetails.attendantcostperday.HasValue)
                                                {
                                                    effectiveCostPerDay = roomDetails.attendantcostperday.Value;
                                                }
                                                else if (roomDetails.costperday.HasValue)
                                                {
                                                    effectiveCostPerDay = roomDetails.costperday.Value;
                                                }
                                            }
                                        }
                                        catch (Exception roomEx)
                                        {
                                            _logger.LogError(
                                                roomEx,
                                                "Mark_IPD_Admitted: failed to fetch room cost for room " + roomId
                                            );
                                        }

                                        if (effectiveCostPerDay <= 0)
                                        {
                                            try
                                            {
                                                effectiveCostPerDay = Convert.ToDecimal(
                                                    rm[isAttendantOnly ? "attendantcostperday" : "costperday"]
                                                );
                                            }
                                            catch
                                            {
                                                effectiveCostPerDay = 0m;
                                            }
                                        }

                                        if (effectiveCostPerDay <= 0)
                                        {
                                            continue;
                                        }

                                        try
                                        {
                                            if (hasPackage && isPatientBilling)
                                            {
                                                if (!packageReceivableCreated)
                                                {
                                                    receivableDAL.CreatePackageReceivable(
                                                        ipdFormId: ipdId,
                                                        packageId: packageId,
                                                        tenantId: tenantId,
                                                        createdBy: createdBy
                                                    );

                                                    packageReceivableCreated = true;
                                                }

                                                decimal packageDiscountPercent = 0m;

                                                if (rm["percentagecovered"] != null)
                                                {
                                                    try
                                                    {
                                                        packageDiscountPercent = Convert.ToDecimal(
                                                            rm["percentagecovered"]
                                                        );
                                                    }
                                                    catch
                                                    {
                                                        packageDiscountPercent = 0m;
                                                    }
                                                }

                                                if (packageDiscountPercent < 0m)
                                                {
                                                    packageDiscountPercent = 0m;
                                                }

                                                if (packageDiscountPercent > 100m)
                                                {
                                                    packageDiscountPercent = 100m;
                                                }

                                                int roomDays = Math.Max(
                                                    1,
                                                    (int)(toDate.Date - fromDate.Date).TotalDays + 1
                                                );

                                                decimal effectiveDiscountPercent = packageDiscountPercent;

                                                if (packageDays > 0
                                                    && totalPatientBillingRoomDays > 0
                                                    && totalPatientBillingRoomDays < packageDays)
                                                {
                                                    effectiveDiscountPercent = Math.Round(
                                                        packageDiscountPercent * totalPatientBillingRoomDays / packageDays,
                                                        2
                                                    );
                                                }

                                                if (effectiveDiscountPercent < 0m)
                                                {
                                                    effectiveDiscountPercent = 0m;
                                                }

                                                if (effectiveDiscountPercent > 100m)
                                                {
                                                    effectiveDiscountPercent = 100m;
                                                }

                                                decimal discountedCostPerDay = Math.Round(
                                                    effectiveCostPerDay * (100m - effectiveDiscountPercent) / 100m,
                                                    2
                                                );

                                                int discountDaysForThisRoom = 0;

                                                if (remainingPackageDiscountDays > 0)
                                                {
                                                    discountDaysForThisRoom = Math.Min(
                                                        roomDays,
                                                        remainingPackageDiscountDays
                                                    );
                                                }

                                                int fullChargeDaysForThisRoom = roomDays - discountDaysForThisRoom;

                                                if (discountDaysForThisRoom > 0)
                                                {
                                                    DateTime discountFromDate = fromDate.Date;
                                                    DateTime discountToDate = fromDate.Date.AddDays(
                                                        discountDaysForThisRoom - 1
                                                    );

                                                    string packageRemarks =
                                                        "Admission:PatientPackageRoomDiscount" +
                                                        " | PackageDiscount:" + packageDiscountPercent.ToString("0.##") + "%" +
                                                        " | EffectiveDiscount:" + effectiveDiscountPercent.ToString("0.##") + "%" +
                                                        " | PackageDays:" + packageDays +
                                                        " | TotalPatientRoomDays:" + totalPatientBillingRoomDays +
                                                        " | DiscountedDays:" + discountDaysForThisRoom +
                                                        " | OriginalCPD:" + effectiveCostPerDay.ToString("0.00") +
                                                        " | ChargedCPD:" + discountedCostPerDay.ToString("0.00") +
                                                        " | AllottedTo:" + allottedTo;

                                                    receivableDAL.CreateRoomReceivables(
                                                        ipdFormId: ipdId,
                                                        roomId: roomId,
                                                        fromDate: discountFromDate,
                                                        toDate: discountToDate,
                                                        costPerDay: discountedCostPerDay,
                                                        isAttendant: false,
                                                        tenantId: tenantId,
                                                        createdBy: createdBy,
                                                        remarks: packageRemarks
                                                    );

                                                    remainingPackageDiscountDays -= discountDaysForThisRoom;
                                                }

                                                if (fullChargeDaysForThisRoom > 0)
                                                {
                                                    DateTime fullFromDate = fromDate.Date.AddDays(
                                                        discountDaysForThisRoom
                                                    );

                                                    DateTime fullToDate = toDate.Date;

                                                    string fullChargeRemarks =
                                                        "Admission:PatientRoomExtraDaysAfterPackage" +
                                                        " | PackageDiscount:NotApplicableForExtraDays" +
                                                        " | PackageDays:" + packageDays +
                                                        " | TotalPatientRoomDays:" + totalPatientBillingRoomDays +
                                                        " | FullChargeDays:" + fullChargeDaysForThisRoom +
                                                        " | OriginalCPD:" + effectiveCostPerDay.ToString("0.00") +
                                                        " | ChargedCPD:" + effectiveCostPerDay.ToString("0.00") +
                                                        " | AllottedTo:" + allottedTo;

                                                    receivableDAL.CreateRoomReceivables(
                                                        ipdFormId: ipdId,
                                                        roomId: roomId,
                                                        fromDate: fullFromDate,
                                                        toDate: fullToDate,
                                                        costPerDay: effectiveCostPerDay,
                                                        isAttendant: false,
                                                        tenantId: tenantId,
                                                        createdBy: createdBy,
                                                        remarks: fullChargeRemarks
                                                    );
                                                }
                                            }
                                            else
                                            {
                                                string normalRemarks = isAttendantOnly
                                                    ? "Admission:AttendantRoom | PackageDiscount:NotApplicable" +
                                                      " | OriginalCPD:" + effectiveCostPerDay.ToString("0.00") +
                                                      " | ChargedCPD:" + effectiveCostPerDay.ToString("0.00") +
                                                      " | AllottedTo:" + allottedTo
                                                    : "Admission:PatientRoom | PackageDiscount:NotApplicable" +
                                                      " | OriginalCPD:" + effectiveCostPerDay.ToString("0.00") +
                                                      " | ChargedCPD:" + effectiveCostPerDay.ToString("0.00") +
                                                      " | AllottedTo:" + allottedTo;

                                                receivableDAL.CreateRoomReceivables(
                                                    ipdFormId: ipdId,
                                                    roomId: roomId,
                                                    fromDate: fromDate,
                                                    toDate: toDate,
                                                    costPerDay: effectiveCostPerDay,
                                                    isAttendant: isAttendantOnly,
                                                    tenantId: tenantId,
                                                    createdBy: createdBy,
                                                    remarks: normalRemarks
                                                );
                                            }
                                        }
                                        catch (Exception rxEx)
                                        {
                                            _logger.LogError(
                                                rxEx,
                                                "Mark_IPD_Admitted: receivable error for room " + roomId + ": " + rxEx.Message
                                            );
                                        }
                                    }

                                    try
                                    {
                                        receivableDAL.CreateProvisionalBookingDepositReceivables(
                                            ipdId,
                                            tenantId,
                                            createdBy
                                        );
                                    }
                                    catch (Exception rxEx)
                                    {
                                        _logger.LogError(
                                            rxEx,
                                            "Mark_IPD_Admitted: Provisional Booking Deposit receivable error: " + rxEx.Message
                                        );
                                    }
                                }
                            }
                            catch (Exception rxEx)
                            {
                                _logger.LogError(
                                    rxEx,
                                    "Mark_IPD_Admitted: receivable generation error: " + rxEx.Message
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;

                _logger.LogError(
                    ex,
                    "Update_IPD_Booking_Status error: " + ex.Message
                );
            }

            if ((message ?? "").Replace("\"", "") == "201.1")
            {
                return Ok(message);
            }
            else if ((message ?? "").Replace("\"", "") == "401.1")
            {
                return Unauthorized(message);
            }
            else
            {
                return BadRequest(message);
            }
        }

        private static bool IsNotEligibleBookingStatus(string bookingStatus)
        {
            return string.Equals(bookingStatus?.Trim(), "Rejected", StringComparison.OrdinalIgnoreCase)
                || string.Equals(bookingStatus?.Trim(), "Not Eligible", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsScreeningOrConsultationScheduled(string bookingStatus)
        {
            return string.Equals(bookingStatus?.Trim(), "Screening Scheduled", StringComparison.OrdinalIgnoreCase)
                || string.Equals(bookingStatus?.Trim(), "Consultation Scheduled", StringComparison.OrdinalIgnoreCase);
        }

        [HttpPost()]
		[ActionName("Mark_IPD_Discharged")]
		public virtual IActionResult Mark_IPD_Discharged([FromBody] NalamVazha.Models.IPDBookingStatusUpdateModel model)
		{
			string message = "";
			try
			{
				var authHeader = HttpContext.Request.Headers["Authorization"][0];
				if (authHeader.StartsWith("Bearer "))
				{
					var token = authHeader.Substring("Bearer ".Length);
					String[] userdetails = obj_External_System_DAL.get_users_by_token(token);
					model.modifieduser = userdetails[0].ToString();
					var currentIpd = objIPDApplicationFormDAL.getById_IPDApplicationForm(model.IPDApplicationFormid);
					if (currentIpd == null)
						return BadRequest("IPD application not found.");
					if (!string.Equals(currentIpd.bookingstatus?.Trim(), "Discharge Initiated", StringComparison.OrdinalIgnoreCase))
						return BadRequest("Discharge must be initiated before the patient can be marked as discharged.");
					var billingDal = new BillingPaymentDAL(_connectionSettings.Value.ConnectionString);
					var pendingRows = billingDal.Get_Unified_Pending_Receivables(
						currentIpd.patientname,
						Guid.Parse(model.IPDApplicationFormid),
						"IPD");
					var outstandingBalance = pendingRows.AsEnumerable()
						.Sum(row => row.Field<decimal?>("balance") ?? 0m);
					if (outstandingBalance > 0.009m)
						return BadRequest($"Patient cannot be discharged. Rs. {outstandingBalance:N2} is still pending.");
					// 1. Update booking status to Discharged
					message = objIPDApplicationFormDAL.Update_IPD_BookingStatus(model.IPDApplicationFormid, "Discharged", model.modifieduser);
					if ((message ?? "").Replace("\"", "") == "201.1")
					{
						// 2. Release allocated room(s) — mark as Vacant
						var roomDAL = new NalamVazha.DAL.RoomOccupancyStatusDAL(_connectionSettings.Value.ConnectionString);
						var roomMsg = roomDAL.Release_Rooms_On_Discharge(model.IPDApplicationFormid, model.modifieduser);
						if (!(roomMsg ?? "").Replace("\"", "").Contains("201.1"))
							_logger.LogWarning("Mark_IPD_Discharged: room release failed for {IPD}: {Msg}", model.IPDApplicationFormid, roomMsg);
					}
				}
				else
					message = "Invalid Token";
			}
			catch (Exception ex)
			{
				message = ex.Message;
				_logger.LogError(ex, "Mark_IPD_Discharged error: " + ex.Message);
			}
			if (message.Replace("\"", "") == "201.1") return Ok(message);
			else if (message.Replace("\"", "") == "401.1") return Unauthorized(message);
			else return BadRequest(message);
		}


		[HttpPost()]
		[AllowAnonymous]
		[ActionName("Create_Razorpay_Order")]
		public virtual IActionResult Create_Razorpay_Order([FromBody] NalamVazha.Models.IPDRazorpayOrderRequestModel model)
		{
			try
			{
				if (model == null || string.IsNullOrWhiteSpace(model.IPDApplicationFormid))
					return BadRequest("IPDApplicationFormid is required");
				if (!Guid.TryParse(model.IPDApplicationFormid, out var ipdFormGuid))
					return BadRequest("Invalid IPDApplicationFormid.");

				var lockObj = _ipdPaymentLocks.GetOrAdd(model.IPDApplicationFormid, _ => new object());
				lock (lockObj)
				{
				if (HasSuccessfulIPDBookingDepositPayment(ipdFormGuid))
					return Conflict("Booking deposit is already paid for this IPD booking. Please refresh the page.");

				// 1. Get IPD payment details
				var dataTable = objIPDApplicationFormDAL.Get_IPD_Payment_Details(model.IPDApplicationFormid);
				if (dataTable == null || dataTable.Rows.Count == 0)
					return NotFound("IPD record not found");

				var row = dataTable.Rows[0];
				string tenantid = row["tenantid"]?.ToString();
				string patientname = row["patientname"]?.ToString();
				string bookingreferencenumber = row["bookingreferencenumber"]?.ToString();
				if (IsCancelledIPDStatus(row))
					return Conflict("This IPD booking is cancelled. Payment cannot be initiated.");
				if (HasActiveIPDRazorpayRequest(ipdFormGuid))
					return Conflict("A Razorpay payment is already in progress for this IPD booking. Please close the existing payment popup or wait a few minutes before trying again.");

				// Charge all mandatory booking-deposit receivables, including attendant deposit.
				decimal depositAmount = 0;
				decimal mandatoryReceivableBalance = 0;
				try
				{
					var receivablesDt = objIPDApplicationFormDAL.Get_IPD_BookingDeposit_Receivables(model.IPDApplicationFormid);
					foreach (DataRow rr in receivablesDt.Rows)
					{
						bool isMandatory = rr["ismandatory"] != DBNull.Value && Convert.ToBoolean(rr["ismandatory"]);
						if (isMandatory)
							mandatoryReceivableBalance += Convert.ToDecimal(rr["balance"]);
					}
				}
				catch (Exception exR)
				{
					_logger.LogWarning("Create_Razorpay_Order: receivable query failed, falling back: " + exR.Message);
				}

				decimal configuredDeposit = 0;
				if (row.Table.Columns.Contains("patient_booking_deposit") && row["patient_booking_deposit"] != System.DBNull.Value)
					configuredDeposit = Convert.ToDecimal(row["patient_booking_deposit"]);
				if (configuredDeposit <= 0 && row.Table.Columns.Contains("packagebookingdeposit") && row["packagebookingdeposit"] != System.DBNull.Value)
					configuredDeposit = Convert.ToDecimal(row["packagebookingdeposit"]);
				if (configuredDeposit <= 0 && row.Table.Columns.Contains("advanceamount") && row["advanceamount"] != System.DBNull.Value)
					configuredDeposit = Convert.ToDecimal(row["advanceamount"]);

				depositAmount = mandatoryReceivableBalance > 0 ? mandatoryReceivableBalance : configuredDeposit;

				if (depositAmount <= 0)
				{
					return BadRequest("Booking deposit amount is not available for this IPD booking.");
				}
				string customerfirstname = row["firstname"]?.ToString();
				string customerlastname = row["lastname"]?.ToString();
				string customerphone = GetLatestPatientMobileNumber(row);

				// 2. Get Razorpay config for tenant
				var objPaymentConfigDAL = new PaymentConfigDAL(_connectionSettings.Value.ConnectionString);
				var configTable = objPaymentConfigDAL.get_Payment_Config(tenantid, "RazorPay");
				if (configTable == null || configTable.Rows.Count == 0)
					return BadRequest("Razorpay payment config not found for tenant");

				var configRow = configTable.Rows[0];
				string paymentConfigid = configRow["paymentconfigid"]?.ToString();
				var keyinfoTable = objPaymentConfigDAL.getById_keyinfo(paymentConfigid);
				if (keyinfoTable == null || keyinfoTable.Rows.Count == 0)
					return BadRequest("Razorpay key info not configured");

				string keyid = keyinfoTable.Rows[0]["keyid"]?.ToString();
				string keysecret = keyinfoTable.Rows[0]["keysecret"]?.ToString();
				string merchantid = keyinfoTable.Rows[0]["merchantid"]?.ToString();

				// 3. Create Razorpay order via RestSharp
				long amountInPaise = Convert.ToInt64(depositAmount * 100);
				var rzpClient = new RestSharp.RestClient("https://api.razorpay.com/v1");
				var rzpRequest = new RestSharp.RestRequest("orders", RestSharp.Method.Post);
				rzpRequest.AddHeader("Authorization", "Basic " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(keyid + ":" + keysecret)));
				rzpRequest.AddJsonBody(new
				{
					amount = amountInPaise,
					currency = "INR",
					receipt = bookingreferencenumber,
					notes = new { ipdapplicationformid = model.IPDApplicationFormid }
				});
				var rzpResponse = rzpClient.Execute(rzpRequest);
				if (!rzpResponse.IsSuccessful)
					return BadRequest("Failed to create Razorpay order: " + rzpResponse.Content);

				var rzpOrder = Newtonsoft.Json.Linq.JObject.Parse(rzpResponse.Content);
				string razorpayOrderId = rzpOrder["id"]?.ToString();

				// 4. Save PaymentRequest record
				var objPaymentRequestDAL = new PaymentRequestDAL(_connectionSettings.Value.ConnectionString);
				var paymentRequestId = Guid.NewGuid();
				var paymentRequest = new NalamVazha.Models.PaymentRequestModel
				{
					PaymentRequestid = paymentRequestId,
					tenantid = Guid.TryParse(tenantid, out var tid) ? tid : (Guid?)null,
					paymentgateway = "RazorPay",
					requestdatetime = GetApplicationNow(),
					patientname = Guid.TryParse(patientname, out var pid) ? pid : (Guid?)null,
					paymenttype = "Booking Deposit",
					merchantid = merchantid,
					orderid = razorpayOrderId,
					amount = depositAmount,
					currency = "INR",
					paymentid = BuildIPDPaymentRequestMarker(model.IPDApplicationFormid, paymentRequestId),
					customername = (customerfirstname + " " + customerlastname).Trim(),
					customerphone = customerphone,
					orderpaymentdesc = "IPD Booking Deposit - " + bookingreferencenumber,
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
					description = "IPD Booking Deposit - " + bookingreferencenumber,
					name = (customerfirstname + " " + customerlastname).Trim(),
					contact = customerphone,
					paymentrequestid = paymentRequest.PaymentRequestid.ToString()
				});
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Create_Razorpay_Order error: " + ex.Message);
				return BadRequest(ex.Message);
			}
		}

		[HttpGet()]
		[AllowAnonymous]
		[ActionName("Get_IPD_Booking_Deposit_Payment_Status")]
		public virtual IActionResult Get_IPD_Booking_Deposit_Payment_Status(string IPDApplicationFormid)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(IPDApplicationFormid))
					return BadRequest(new { status = "Open", canPay = false, message = "IPDApplicationFormid is required." });
				if (!Guid.TryParse(IPDApplicationFormid, out var ipdFormGuid))
					return BadRequest(new { status = "Open", canPay = false, message = "Invalid IPDApplicationFormid." });

				if (HasSuccessfulIPDBookingDepositPayment(ipdFormGuid))
				{
					return Ok(new
					{
						status = "Paid",
						canPay = false,
						message = "Booking deposit is already paid for this IPD booking."
					});
				}

				if (HasActiveIPDRazorpayRequest(ipdFormGuid))
				{
					return Ok(new
					{
						status = "In Progress",
						canPay = false,
						message = "A Razorpay booking-deposit payment is already in progress. Please complete, close/cancel the payment popup, or wait for it to expire before trying again."
					});
				}

				return Ok(new { status = "Open", canPay = true, message = "" });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Get_IPD_Booking_Deposit_Payment_Status error: " + ex.Message);
				return BadRequest(new { status = "Open", canPay = false, message = ex.Message });
			}
		}

		[HttpPost()]
		[AllowAnonymous]
		[ActionName("Complete_IPD_Payment")]
		public virtual IActionResult Complete_IPD_Payment([FromBody] IPDCompletePaymentModel model)
		{
			try
			{
				if (model == null || string.IsNullOrWhiteSpace(model.IPDApplicationFormid))
					return BadRequest("IPDApplicationFormid is required");
				if (!Guid.TryParse(model.IPDApplicationFormid, out var ipdFormGuid))
					return BadRequest("Invalid IPDApplicationFormid.");

				var lockObj = _ipdPaymentLocks.GetOrAdd(model.IPDApplicationFormid, _ => new object());
				lock (lockObj)
				{
				// 1. Fetch payment request to get key secret for signature verification
				var objPaymentRequestDAL = new PaymentRequestDAL(_connectionSettings.Value.ConnectionString);
				var paymentReq = objPaymentRequestDAL.getById_PaymentRequest(model.paymentrequestid);
				if (paymentReq == null)
					return NotFound("Payment request record not found");
				if (!string.Equals(paymentReq.orderid, model.razorpay_order_id, StringComparison.OrdinalIgnoreCase))
					return BadRequest("Razorpay order does not match this payment request.");
				if (HasPaidPaymentResponse(model.razorpay_payment_id))
				{
					return Ok(new { success = true, message = "201.1", duplicate = true, bookingstatus = "Provisional Confirmed" });
				}

				string tenantid = paymentReq.tenantid?.ToString();
				decimal amount = paymentReq.amount ;
				var currentIpdTable = objIPDApplicationFormDAL.Get_IPD_Payment_Details(model.IPDApplicationFormid);
				if (currentIpdTable == null || currentIpdTable.Rows.Count == 0)
					return BadRequest("IPD record not found");
				if (IsCancelledIPDStatus(currentIpdTable.Rows[0]))
					return Conflict("This IPD booking is cancelled. Payment cannot be completed.");
				var bookingDepositAlreadyPaid = HasSuccessfulIPDBookingDepositPayment(ipdFormGuid);
				decimal currentDepositAmount = CalculateCurrentIPDBookingDepositAmount(model.IPDApplicationFormid, currentIpdTable.Rows[0]);
				if (!bookingDepositAlreadyPaid)
				{
					if (currentDepositAmount <= 0)
						return Conflict("Booking deposit is no longer payable. Please refresh the page.");
					if (Math.Round(currentDepositAmount, 2) != Math.Round(amount, 2))
						return Conflict("Booking deposit amount has changed. Please refresh the page and start payment again.");
				}

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
				using (var hmac = new System.Security.Cryptography.HMACSHA256(System.Text.Encoding.UTF8.GetBytes(keysecret)))
				{
					var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(signaturePayload));
					string computedSignature = BitConverter.ToString(computedHash).Replace("-", "").ToLower();
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
					paymenttype = "Booking Deposit",
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
				if (bookingDepositAlreadyPaid)
				{
					_logger.LogWarning("Recorded Razorpay response after booking deposit was already paid at front desk. IPD: {ipdId}, Order: {orderId}, Payment: {paymentId}",
						model.IPDApplicationFormid, model.razorpay_order_id, model.razorpay_payment_id);
					return Conflict("Payment captured in Razorpay, but booking deposit was already received at the front desk. Razorpay response has been recorded for reconciliation.");
				}

				// 5. Get patient details and amount for billing
				var ipdTable = currentIpdTable;
				string patientGuid = ipdTable?.Rows.Count > 0 ? ipdTable.Rows[0]["patientname"]?.ToString() : null;

				// Calculate total amount and advance for remarks
				decimal totalAmount = amount;
				decimal advanceAmount = amount;
				string roomInfo = "";
				string packageInfo = "";
				if (ipdTable != null && ipdTable.Rows.Count > 0)
				{
					var ipdRow = ipdTable.Rows[0];
					if (ipdRow.Table.Columns.Contains("totalamount") && ipdRow["totalamount"] != System.DBNull.Value)
						totalAmount = Convert.ToDecimal(ipdRow["totalamount"]);
					if (ipdRow.Table.Columns.Contains("advanceamount") && ipdRow["advanceamount"] != System.DBNull.Value)
						advanceAmount = Convert.ToDecimal(ipdRow["advanceamount"]);
					if (ipdRow.Table.Columns.Contains("packagename_master"))
						packageInfo = ipdRow["packagename_master"]?.ToString() ?? "";
					if (ipdRow.Table.Columns.Contains("accommodationtype"))
						roomInfo = ipdRow["accommodationtype"]?.ToString() ?? "";
				}

				string billingRemarks = $"Room Booking Advance. Package: {packageInfo}. Room: {roomInfo}. " +
					$"Total Amount: {totalAmount:N2}. Advance Paid: {advanceAmount:N2}. " +
					$"Razorpay Order: {model.razorpay_order_id}. Payment ID: {model.razorpay_payment_id}.";

				// 6. Save BillingPayment record
				var objBillingPaymentDAL = new BillingPaymentDAL(_connectionSettings.Value.ConnectionString);
				Guid? ipdFormId = ipdFormGuid;
				var billing = new NalamVazha.Models.BillingPaymentModel
				{
					BillingPaymentid = Guid.NewGuid(),
					tenantid = Guid.TryParse(tenantid, out var btid) ? btid : (Guid?)null,
					paymentdate = GetApplicationNow().Date,
					receivablefor = "IPD Booking Deposit",
					ipdnumber = ipdFormId,
					patientname = Guid.TryParse(patientGuid, out var bpid) ? bpid : (Guid?)null,
					amount = amount,
					receivedamount = amount,
					currency = "INR",
					conversionrate = 1,
					paymentmode = paymentMode,
					transactionreference = model.razorpay_payment_id,
					paymentstatus = "Success",
					collectedby = Guid.TryParse(pvar_usersid, out var cbid) ? cbid : (Guid?)null,
					counterid = "Through Razorpay",
					remarks = billingRemarks,
					createduser = Guid.TryParse(pvar_usersid, out var bcuid) ? bcuid : (Guid?)null,
					craftmyapp_actionmethodname = "Add_Billing_Payment"
				};
				var billingMessage = objBillingPaymentDAL.Add_Billing_Payment(billing);
				if (!(billingMessage ?? "").Replace("\"", "").Contains("201.1"))
					throw new Exception(billingMessage);
				if (ipdFormId.HasValue)
				{
					objBillingPaymentDAL.ApplyIPDRazorpayReceivablePayment(
						ipdFormId.Value,
						amount,
						billing.BillingPaymentid.Value,
						Guid.TryParse(pvar_usersid, out var modifiedBy) ? modifiedBy : (Guid?)null);
				}

				// 7. Update IPD booking status to "Provisional Confirmed" (payment received)
				var statusMessage = objIPDApplicationFormDAL.Update_IPD_BookingStatus(model.IPDApplicationFormid, "Provisional Confirmed", pvar_usersid);
				if ((statusMessage ?? "").Replace("\"", "") != "201.1")
					throw new Exception("Payment recorded but IPD booking status update failed: " + statusMessage);

				return Ok(new { success = true, message = "201.1", billingpaymentid = billing.BillingPaymentid.ToString(), bookingstatus = "Provisional Confirmed" });
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Complete_IPD_Payment error: " + ex.Message);
				return BadRequest(ex.Message);
			}
		}
		// ─────────────────────────────────────────────────────────────────────────
		// CANCELLATION & REFUND
		// ─────────────────────────────────────────────────────────────────────────

		private static object ReadColumn(DataRow row, string columnName)
		{
			if (row == null || string.IsNullOrWhiteSpace(columnName)) return null;
			foreach (DataColumn column in row.Table.Columns)
			{
				if (string.Equals(column.ColumnName, columnName, StringComparison.OrdinalIgnoreCase))
					return row[column];
			}
			return null;
		}

		private static decimal ReadDecimalColumn(DataRow row, string columnName)
		{
			var value = ReadColumn(row, columnName);
			if (value == null || value == DBNull.Value) return 0;
			return decimal.TryParse(Convert.ToString(value), out var parsed) ? parsed : 0;
		}

		private static Guid ReadGuidColumn(DataRow row, string columnName)
		{
			var value = ReadColumn(row, columnName);
			return Guid.TryParse(Convert.ToString(value), out var parsed) ? parsed : Guid.Empty;
		}

		private static bool ReadBoolColumn(DataRow row, string columnName)
		{
			var value = ReadColumn(row, columnName);
			if (value == null || value == DBNull.Value) return false;
			return bool.TryParse(Convert.ToString(value), out var parsed) && parsed;
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

		private static string BuildIPDPaymentRequestMarker(string ipdApplicationFormId, Guid paymentRequestId)
		{
			return $"IPD:{ipdApplicationFormId}:REQ:{paymentRequestId}";
		}

		private bool IsCancelledIPDStatus(DataRow row)
		{
			string[] statusColumns = { "bookingstatus", "status", "applicationstatus" };
			foreach (var columnName in statusColumns)
			{
				if (!row.Table.Columns.Contains(columnName)) continue;
				var status = row[columnName]?.ToString() ?? "";
				if (status.IndexOf("cancel", StringComparison.OrdinalIgnoreCase) >= 0)
					return true;
			}
			return false;
		}

		private decimal CalculateCurrentIPDBookingDepositAmount(string ipdApplicationFormId, DataRow row)
		{
			decimal mandatoryReceivableBalance = 0;
			try
			{
				var receivablesDt = objIPDApplicationFormDAL.Get_IPD_BookingDeposit_Receivables(ipdApplicationFormId);
				foreach (DataRow rr in receivablesDt.Rows)
				{
					bool isMandatory = rr["ismandatory"] != DBNull.Value && Convert.ToBoolean(rr["ismandatory"]);
					if (isMandatory)
						mandatoryReceivableBalance += Convert.ToDecimal(rr["balance"]);
				}
			}
			catch (Exception exR)
			{
				_logger.LogWarning("CalculateCurrentIPDBookingDepositAmount: receivable query failed, falling back: " + exR.Message);
			}

			decimal configuredDeposit = 0;
			if (row.Table.Columns.Contains("patient_booking_deposit") && row["patient_booking_deposit"] != System.DBNull.Value)
				configuredDeposit = Convert.ToDecimal(row["patient_booking_deposit"]);
			if (configuredDeposit <= 0 && row.Table.Columns.Contains("packagebookingdeposit") && row["packagebookingdeposit"] != System.DBNull.Value)
				configuredDeposit = Convert.ToDecimal(row["packagebookingdeposit"]);
			if (configuredDeposit <= 0 && row.Table.Columns.Contains("advanceamount") && row["advanceamount"] != System.DBNull.Value)
				configuredDeposit = Convert.ToDecimal(row["advanceamount"]);

			if (mandatoryReceivableBalance > 0)
				return mandatoryReceivableBalance;
			return configuredDeposit;
		}

		private bool HasPaidPaymentResponse(string razorpayPaymentId)
		{
			if (string.IsNullOrWhiteSpace(razorpayPaymentId)) return false;
			using (var conn = new NpgsqlConnection(_connectionSettings.Value.ConnectionString))
			{
				conn.Open();
				using (var cmd = new NpgsqlCommand(@"
					SELECT EXISTS (
						SELECT 1
						FROM PaymentResponse
						WHERE COALESCE(isdeleted, false) = false
						  AND paymentid = @paymentid
						  AND LOWER(COALESCE(status, '')) IN ('paid', 'success')
					)", conn))
				{
					cmd.Parameters.AddWithValue("paymentid", razorpayPaymentId);
					return Convert.ToBoolean(cmd.ExecuteScalar());
				}
			}
		}

		private bool HasDuplicateFailedPaymentResponse(Guid paymentRequestId, string orderId, string paymentId, string errorCode, string errorReason)
		{
			using (var conn = new NpgsqlConnection(_connectionSettings.Value.ConnectionString))
			{
				conn.Open();
				using (var cmd = new NpgsqlCommand("SELECT \"IPD_Has_Duplicate_Failed_Payment_Response\"(@pvar_paymentrequest,@pvar_orderid,@pvar_paymentid,@pvar_errorcode,@pvar_errorreason)", conn))
				{
					cmd.Parameters.AddWithValue("pvar_paymentrequest", NpgsqlTypes.NpgsqlDbType.Uuid, paymentRequestId);
					cmd.Parameters.AddWithValue("pvar_orderid", NpgsqlTypes.NpgsqlDbType.Varchar, (object)orderId ?? DBNull.Value);
					cmd.Parameters.AddWithValue("pvar_paymentid", NpgsqlTypes.NpgsqlDbType.Varchar, (object)paymentId ?? DBNull.Value);
					cmd.Parameters.AddWithValue("pvar_errorcode", NpgsqlTypes.NpgsqlDbType.Varchar, (object)errorCode ?? DBNull.Value);
					cmd.Parameters.AddWithValue("pvar_errorreason", NpgsqlTypes.NpgsqlDbType.Varchar, (object)errorReason ?? DBNull.Value);
					return Convert.ToBoolean(cmd.ExecuteScalar());
				}
			}
		}

		private bool HasSuccessfulIPDBookingDepositPayment(Guid ipdApplicationFormId)
		{
			using (var conn = new NpgsqlConnection(_connectionSettings.Value.ConnectionString))
			{
				conn.Open();
				using (var cmd = new NpgsqlCommand("SELECT \"IPD_Has_Successful_Booking_Deposit_Payment\"(@pvar_ipdapplicationformid)", conn))
				{
					cmd.Parameters.AddWithValue("pvar_ipdapplicationformid", ipdApplicationFormId);
					return Convert.ToBoolean(cmd.ExecuteScalar());
				}
			}
		}

		private bool HasIPDCancellationOrRefund(Guid ipdApplicationFormId)
		{
			using (var conn = new NpgsqlConnection(_connectionSettings.Value.ConnectionString))
			{
				conn.Open();
				using (var cmd = new NpgsqlCommand("SELECT \"IPD_Has_Cancellation_Or_Refund\"(@pvar_ipdapplicationformid)", conn))
				{
					cmd.Parameters.AddWithValue("pvar_ipdapplicationformid", ipdApplicationFormId);
					return Convert.ToBoolean(cmd.ExecuteScalar());
				}
			}
		}

		private bool HasActiveIPDRazorpayRequest(Guid ipdApplicationFormId)
		{
			using (var conn = new NpgsqlConnection(_connectionSettings.Value.ConnectionString))
			{
				conn.Open();
				using (var cmd = new NpgsqlCommand("SELECT \"IPD_Has_Active_Razorpay_Request\"(@pvar_ipdapplicationformid,@pvar_activeafter)", conn))
				{
					cmd.Parameters.AddWithValue("pvar_ipdapplicationformid", ipdApplicationFormId);
					cmd.Parameters.AddWithValue("pvar_activeafter", GetApplicationNow().AddMinutes(-15));
					return Convert.ToBoolean(cmd.ExecuteScalar());
				}
			}
		}

		private List<UnifiedPaymentInputModel> BuildSelectedIPDBalancePayments(Guid patientId, Guid ipdFormId, List<Guid> selectedReceivableIds)
		{
			if (selectedReceivableIds == null || selectedReceivableIds.Count == 0)
				throw new Exception("Please select at least one pending bill to pay.");

			var selectedSet = new HashSet<Guid>(selectedReceivableIds.Where(x => x != Guid.Empty));
			if (selectedSet.Count == 0)
				throw new Exception("Please select at least one pending bill to pay.");

			var billingDal = new BillingPaymentDAL(_connectionSettings.Value.ConnectionString);
			var pending = billingDal.Get_Unified_Pending_Receivables(patientId, ipdFormId, "IPD");
			var payments = new List<UnifiedPaymentInputModel>();

			foreach (DataRow row in pending.Rows)
			{
				var rowBalance = ReadDecimalColumn(row, "balance");
				var receivableId = ReadGuidColumn(row, "Receivableid");
				if (receivableId == Guid.Empty || !selectedSet.Contains(receivableId)) continue;

				var balance = rowBalance;
				if (balance <= 0) continue;

				payments.Add(new UnifiedPaymentInputModel
				{
					Receivableid = receivableId,
					ReceivableFor = Convert.ToString(ReadColumn(row, "receivablefor"))?.Trim(),
					Balance = balance,
					PayNow = balance,
					IsMandatory = ReadBoolColumn(row, "ismandatory")
				});
			}

			if (payments.Count == 0)
				throw new Exception("Selected bills are not payable or are already paid.");

			var missing = selectedSet.Except(payments.Select(x => x.Receivableid)).ToList();
			if (missing.Count > 0)
				throw new Exception("One or more selected bills are no longer payable. Please refresh and try again.");

			return payments;
		}

		[HttpPost()]
		[AllowAnonymous]
		[ActionName("Create_IPD_Balance_Razorpay_Order")]
		public virtual IActionResult Create_IPD_Balance_Razorpay_Order([FromBody] NalamVazha.Models.IPDRazorpayOrderRequestModel model)
		{
			try
			{
				if (model == null || string.IsNullOrWhiteSpace(model.IPDApplicationFormid))
					return BadRequest("IPDApplicationFormid is required");

				var summary = BuildIPDBillingSummary(model.IPDApplicationFormid, out var row);
				if (!Guid.TryParse(model.IPDApplicationFormid, out var ipdGuid))
					return BadRequest("Invalid IPDApplicationFormid.");
				if (!Guid.TryParse(Convert.ToString(ReadColumn(row, "patientname")), out var patientGuid))
					return BadRequest("Patient not found for this IPD booking.");

				var selectedPayments = BuildSelectedIPDBalancePayments(patientGuid, ipdGuid, model.receivableids);
				decimal balanceAmount = selectedPayments.Sum(x => x.PayNow);
				if (balanceAmount <= 0)
					return BadRequest("No remaining balance is payable for this IPD booking.");

				string tenantid = row["tenantid"]?.ToString();
				string patientname = row["patientname"]?.ToString();
				string bookingreferencenumber = row["bookingreferencenumber"]?.ToString();
				string customerfirstname = row["firstname"]?.ToString();
				string customerlastname = row["lastname"]?.ToString();
				string customerphone = GetLatestPatientMobileNumber(row);

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

				long amountInPaise = Convert.ToInt64(balanceAmount * 100);
				var rzpClient = new RestSharp.RestClient("https://api.razorpay.com/v1");
				var rzpRequest = new RestSharp.RestRequest("orders", RestSharp.Method.Post);
				rzpRequest.AddHeader("Authorization", "Basic " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(keyid + ":" + keysecret)));
				rzpRequest.AddJsonBody(new
				{
					amount = amountInPaise,
					currency = "INR",
					receipt = bookingreferencenumber,
					notes = new { ipdapplicationformid = model.IPDApplicationFormid, paymenttype = "Balance Payment", selectedbills = selectedPayments.Count }
				});
				var rzpResponse = rzpClient.Execute(rzpRequest);
				if (!rzpResponse.IsSuccessful)
					return BadRequest("Failed to create Razorpay order: " + rzpResponse.Content);

				var rzpOrder = Newtonsoft.Json.Linq.JObject.Parse(rzpResponse.Content);
				string razorpayOrderId = rzpOrder["id"]?.ToString();

				var objPaymentRequestDAL = new PaymentRequestDAL(_connectionSettings.Value.ConnectionString);
				var paymentRequestId = Guid.NewGuid();
				var paymentRequest = new NalamVazha.Models.PaymentRequestModel
				{
					PaymentRequestid = paymentRequestId,
					tenantid = Guid.TryParse(tenantid, out var tid) ? tid : (Guid?)null,
					paymentgateway = "RazorPay",
					requestdatetime = GetApplicationNow(),
					patientname = Guid.TryParse(patientname, out var pid) ? pid : (Guid?)null,
					paymenttype = "Balance Payment",
					merchantid = merchantid,
					orderid = razorpayOrderId,
					amount = balanceAmount,
					currency = "INR",
					paymentid = BuildIPDPaymentRequestMarker(model.IPDApplicationFormid, paymentRequestId),
					customername = (customerfirstname + " " + customerlastname).Trim(),
					customerphone = customerphone,
					orderpaymentdesc = "IPD Balance Payment - " + bookingreferencenumber + " (" + selectedPayments.Count + " bills)",
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
					description = "IPD Balance Payment - " + bookingreferencenumber,
					name = (customerfirstname + " " + customerlastname).Trim(),
					contact = customerphone,
					paymentrequestid = paymentRequest.PaymentRequestid.ToString(),
					receivableids = selectedPayments.Select(x => x.Receivableid.ToString()).ToList()
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Create_IPD_Balance_Razorpay_Order error: " + ex.Message);
				return BadRequest(ex.Message);
			}
		}

		[HttpPost()]
		[AllowAnonymous]
		[ActionName("Complete_IPD_Balance_Payment")]
		public virtual IActionResult Complete_IPD_Balance_Payment([FromBody] IPDCompletePaymentModel model)
		{
			try
			{
				var objPaymentRequestDAL = new PaymentRequestDAL(_connectionSettings.Value.ConnectionString);
				var paymentReq = objPaymentRequestDAL.getById_PaymentRequest(model.paymentrequestid);
				if (paymentReq == null)
					return NotFound("Payment request record not found");
				if (!string.Equals(paymentReq.paymenttype, "Balance Payment", StringComparison.OrdinalIgnoreCase))
					return BadRequest("Payment request is not for IPD balance payment.");

				string tenantid = paymentReq.tenantid?.ToString();
				decimal amount = paymentReq.amount;
				if (!Guid.TryParse(model.IPDApplicationFormid, out var ipdGuid))
					return BadRequest("Invalid IPD payment request.");
				if (!paymentReq.patientname.HasValue)
					return BadRequest("Patient not found for this payment request.");
				var selectedPayments = BuildSelectedIPDBalancePayments(paymentReq.patientname.Value, ipdGuid, model.receivableids);
				var selectedAmount = selectedPayments.Sum(x => x.PayNow);
				if (Math.Abs(selectedAmount - amount) > 0.009m)
					return BadRequest("Selected bill amount does not match Razorpay order amount. Please refresh and try again.");

				var objPaymentConfigDAL = new PaymentConfigDAL(_connectionSettings.Value.ConnectionString);
				var configTable = objPaymentConfigDAL.get_Payment_Config(tenantid, "RazorPay");
				if (configTable == null || configTable.Rows.Count == 0)
					return BadRequest("Razorpay config not found");

				string paymentConfigid = configTable.Rows[0]["paymentconfigid"]?.ToString();
				var keyinfoTable = objPaymentConfigDAL.getById_keyinfo(paymentConfigid);
				if (keyinfoTable == null || keyinfoTable.Rows.Count == 0)
					return BadRequest("Razorpay key info not configured");

				string keysecret = keyinfoTable.Rows[0]["keysecret"]?.ToString();
				string signaturePayload = model.razorpay_order_id + "|" + model.razorpay_payment_id;
				using (var hmac = new System.Security.Cryptography.HMACSHA256(System.Text.Encoding.UTF8.GetBytes(keysecret)))
				{
					var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(signaturePayload));
					string computedSignature = BitConverter.ToString(computedHash).Replace("-", "").ToLower();
					if (computedSignature != model.razorpay_signature?.ToLower())
						return Unauthorized("Payment signature verification failed");
				}

				string paymentMode = FetchRazorpayPaymentMode(keyinfoTable, keysecret, model.razorpay_payment_id);

				var objPaymentResponseDAL = new PaymentResponseDAL(_connectionSettings.Value.ConnectionString);
				var paymentResponse = new NalamVazha.Models.PaymentResponseModel
				{
					PaymentResponseid = Guid.NewGuid(),
					tenantid = Guid.TryParse(tenantid, out var tid) ? tid : (Guid?)null,
					paymentrequest = paymentReq.PaymentRequestid,
					paymenttype = "Balance Payment",
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

				var ipdTable = objIPDApplicationFormDAL.Get_IPD_Payment_Details(model.IPDApplicationFormid);
				string patientGuid = ipdTable?.Rows.Count > 0 ? ipdTable.Rows[0]["patientname"]?.ToString() : null;

				var selectedPaymentFor = string.Join(", ", selectedPayments
					.Select(x => x.ReceivableFor)
					.Where(x => !string.IsNullOrWhiteSpace(x))
					.Distinct(StringComparer.OrdinalIgnoreCase));
				if (string.IsNullOrWhiteSpace(selectedPaymentFor))
					selectedPaymentFor = "IPD Payment";

				string billingRemarks = $"IPD selected balance received via Razorpay for {selectedPayments.Count} bill(s). Razorpay Order: {model.razorpay_order_id}. Payment ID: {model.razorpay_payment_id}.";
				var objBillingPaymentDAL = new BillingPaymentDAL(_connectionSettings.Value.ConnectionString);
				var billing = new NalamVazha.Models.BillingPaymentModel
				{
					BillingPaymentid = Guid.NewGuid(),
					tenantid = Guid.TryParse(tenantid, out var btid) ? btid : (Guid?)null,
					paymentdate = GetApplicationNow().Date,
					receivablefor = selectedPaymentFor,
					ipdnumber = ipdGuid,
					patientname = Guid.TryParse(patientGuid, out var bpid) ? bpid : (Guid?)null,
					amount = amount,
					receivedamount = amount,
					currency = "INR",
					conversionrate = 1,
					paymentmode = paymentMode,
					transactionreference = model.razorpay_payment_id,
					paymentstatus = "Success",
					collectedby = Guid.TryParse(pvar_usersid, out var cbid) ? cbid : (Guid?)null,
					counterid = "Through Razorpay",
					remarks = billingRemarks,
					createduser = Guid.TryParse(pvar_usersid, out var bcuid) ? bcuid : (Guid?)null,
					craftmyapp_actionmethodname = "Add_Billing_Payment"
				};
				var billingMessage = objBillingPaymentDAL.Add_Billing_Payment(billing);
				if (!(billingMessage ?? "").Replace("\"", "").Contains("201.1"))
					throw new Exception(billingMessage);
				objBillingPaymentDAL.ApplySelectedIPDBalanceRazorpayReceivablePayment(
					selectedPayments,
					billing.BillingPaymentid.Value,
					Guid.TryParse(pvar_usersid, out var modifiedBy) ? modifiedBy : (Guid?)null);

				return Ok(new { success = true, message = "201.1", billingpaymentid = billing.BillingPaymentid.ToString() });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Complete_IPD_Balance_Payment error: " + ex.Message);
				return BadRequest(ex.Message);
			}
		}

		[HttpPost()]
		[AllowAnonymous]
		[ActionName("Fail_IPD_Payment")]
		public virtual IActionResult Fail_IPD_Payment([FromBody] IPDFailedPaymentModel model)
		{
			try
			{
				if (model == null || string.IsNullOrWhiteSpace(model.paymentrequestid))
					return BadRequest("paymentrequestid is required");

				var objPaymentRequestDAL = new PaymentRequestDAL(_connectionSettings.Value.ConnectionString);
				var paymentReq = objPaymentRequestDAL.getById_PaymentRequest(model.paymentrequestid);
				if (paymentReq == null)
					return NotFound("Payment request record not found");

				string failedOrderId = string.IsNullOrWhiteSpace(model.razorpay_order_id) ? paymentReq.orderid : model.razorpay_order_id;
				if (paymentReq.PaymentRequestid.HasValue
					&& HasDuplicateFailedPaymentResponse(paymentReq.PaymentRequestid.Value, failedOrderId, model.razorpay_payment_id, model.error_code, model.error_reason))
					return Ok(new { message = "201.1", duplicate = true });

				string tenantid = paymentReq.tenantid?.ToString();
				var objPaymentResponseDAL = new PaymentResponseDAL(_connectionSettings.Value.ConnectionString);
				var paymentResponse = new NalamVazha.Models.PaymentResponseModel
				{
					PaymentResponseid = Guid.NewGuid(),
					tenantid = Guid.TryParse(tenantid, out var tid) ? tid : (Guid?)null,
					paymentrequest = paymentReq.PaymentRequestid,
					paymenttype = string.IsNullOrWhiteSpace(paymentReq.paymenttype) ? "Booking Deposit" : paymentReq.paymenttype,
					transactiontime = GetApplicationTime(model.failed_at),
					orderid = failedOrderId,
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

				if (!string.Equals(paymentReq.paymenttype, "Balance Payment", StringComparison.OrdinalIgnoreCase)
					&& Guid.TryParse(model.IPDApplicationFormid, out var ipdFormGuid))
				{
					var objBillingPaymentDAL = new BillingPaymentDAL(_connectionSettings.Value.ConnectionString);
					objBillingPaymentDAL.MarkIPDRazorpayReceivablesFailed(
						ipdFormGuid,
						Guid.TryParse(pvar_usersid, out var modifiedBy) ? modifiedBy : (Guid?)null);
				}

				return Ok(new { message = "201.1", paymentresponseid = paymentResponse.PaymentResponseid.ToString() });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Fail_IPD_Payment error: " + ex.Message);
				return BadRequest(ex.Message);
			}
		}

		[HttpGet()]
		[AllowAnonymous]
		[ActionName("Get_Cancel_IPD_Payment_History")]
		public virtual IActionResult Get_Cancel_IPD_Payment_History(string IPDApplicationFormid)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(IPDApplicationFormid))
					return BadRequest("IPDApplicationFormid is required");

				var dataTable = objIPDApplicationFormDAL.Get_Cancel_IPD_Payment_History(IPDApplicationFormid);
				if (dataTable == null || dataTable.Rows.Count == 0)
					return Ok(new List<object>());

				var result = new List<object>();
				foreach (System.Data.DataRow row in dataTable.Rows)
				{
					result.Add(new
					{
						billingpaymentid     = row["billingpaymentid"]?.ToString(),
						receiptno            = row["receiptno"]?.ToString(),
						receivableid         = row["receivableid"]?.ToString(),
						receivablefor        = row["receivablefor"]?.ToString(),
						paymentdate          = row["paymentdate"]?.ToString(),
						createddate          = row["createddate"],
						amount               = row["amount"] != System.DBNull.Value ? Convert.ToDecimal(row["amount"]) : 0,
						paymentmode          = row["paymentmode"]?.ToString(),
						paymentstatus        = row["paymentstatus"]?.ToString(),
						collectedby_master   = ResolveIPDPaymentCollectedBy(row),
						refundstatus         = row["refundstatus"]?.ToString(),
						refundedamount       = row["refundedamount"] != System.DBNull.Value ? Convert.ToDecimal(row["refundedamount"]) : 0,
						remarks              = row["remarks"]?.ToString(),
						transactionreference = row["transactionreference"]?.ToString(),
						room                 = row["room"]?.ToString(),
						room_master          = row["room_master"]?.ToString()
					});
				}
				return Ok(result);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Get_Cancel_IPD_Payment_History error: " + ex.Message);
				return BadRequest(ex.Message);
			}
		}

		/// <summary>
		/// Returns the refund breakdown for an IPD booking.
		/// Matches the current get_ipd_refund_details.sql output shape.
		/// GET api/IPDApplicationForm/Get_IPD_Refund_Details?IPDApplicationFormid=...&cancellationby=Patient|Hospital
		/// </summary>
		[HttpGet()]
		[AllowAnonymous]
		[ActionName("Get_IPD_Refund_Details")]
		public virtual IActionResult Get_IPD_Refund_Details(string IPDApplicationFormid, string cancellationby = "Hospital")
		{
			try
			{
				if (string.IsNullOrWhiteSpace(IPDApplicationFormid))
					return BadRequest("IPDApplicationFormid is required");

				var dataTable = objIPDApplicationFormDAL.Get_IPD_Refund_Details(IPDApplicationFormid, cancellationby);
				if (dataTable == null || dataTable.Rows.Count == 0)
					return Ok(new List<object>());

				var result = new List<object>();
				foreach (System.Data.DataRow row in dataTable.Rows)
				{
					result.Add(new
					{
						receivablefor                = row["receivablefor"]?.ToString(),
						paid_amount               = row["paid_amount"] != System.DBNull.Value ? Convert.ToDecimal(row["paid_amount"]) : 0,
						refund_percentage_applied = row["refund_percentage_applied"] != System.DBNull.Value ? Convert.ToDecimal(row["refund_percentage_applied"]) : 0,
						refund_amount             = row["refund_amount"] != System.DBNull.Value ? Convert.ToDecimal(row["refund_amount"]) : 0,
						total_refund_amount       = row["total_refund_amount"] != System.DBNull.Value ? Convert.ToDecimal(row["total_refund_amount"]) : 0,
						policy_table_used         = row["policy_table_used"]?.ToString(),
						cancellationby            = cancellationby
					});
				}
				return Ok(result);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Get_IPD_Refund_Details error: " + ex.Message);
				return BadRequest(ex.Message);
			}
		}

		/// <summary>
		/// Cancels an IPD booking. If refundmode=RazorPay triggers Razorpay API refund automatically;
		/// otherwise records a manual refund (Cash/UPI/Card/NetBanking).
		/// Saves refundmode, refundreason, refundedby (session user) to BillingPayment.
		/// POST api/IPDApplicationForm/Cancel_IPD_Booking
		/// </summary>
		[HttpPost()]
		[AllowAnonymous]
		[ActionName("Cancel_IPD_Booking")]
		public virtual IActionResult Cancel_IPD_Booking([FromBody] NalamVazha.Models.IPDCancellationRequestModel model)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(model?.IPDApplicationFormid) || string.IsNullOrWhiteSpace(model?.cancellationby))
					return BadRequest("IPDApplicationFormid and cancellationby are required");

				if (string.IsNullOrWhiteSpace(model.cancellationreason))
					return BadRequest("Cancellation reason is required");

				if (!Guid.TryParse(model.IPDApplicationFormid, out var ipdCancellationGuid))
					return BadRequest("Invalid IPDApplicationFormid.");

				var cancellationLockObj = _ipdCancellationLocks.GetOrAdd(model.IPDApplicationFormid, _ => new object());
				lock (cancellationLockObj)
				{
				if (HasIPDCancellationOrRefund(ipdCancellationGuid))
					return Conflict("This IPD booking cancellation/refund is already initiated. Please refresh the page.");

				// Health Seeker: only mark booking as Cancelled by Patient; Front Desk processes refund later.
				if ((model.sessionuserrole ?? "").Equals("Health Seeker", StringComparison.OrdinalIgnoreCase))
				{
					model.cancellationby = "Patient";
					var ipdTableHs = objIPDApplicationFormDAL.Get_IPD_Payment_Details(model.IPDApplicationFormid);
					if (ipdTableHs == null || ipdTableHs.Rows.Count == 0)
						return BadRequest("IPD record not found");

					//objIPDApplicationFormDAL.Update_IPD_BookingStatus(model.IPDApplicationFormid, "Cancelled by Patient", pvar_usersid);
					objIPDApplicationFormDAL.Save_IPD_Cancellation_Request_Remarks(
						model.IPDApplicationFormid,
						model.cancellationreason,
						pvar_usersid);
					objIPDApplicationFormDAL.Update_IPD_BookingStatus(model.IPDApplicationFormid, "Cancellation Requested", pvar_usersid);

					var objClinicalAppointmentDALHs = new NalamVazha.DAL.ClinicalAppointmentDAL(_connectionSettings.Value.ConnectionString);
					string appointmentCancelResultHs = objClinicalAppointmentDALHs.Cancel_IPD_Appointments_By_Booking(model.IPDApplicationFormid, pvar_usersid);
					if (appointmentCancelResultHs != "201.1")
						_logger.LogWarning($"Cancel_IPD_Appointments_By_Booking returned: {appointmentCancelResultHs} for IPD {model.IPDApplicationFormid}");

					var objRoomOccupancyStatusDALHs = new NalamVazha.DAL.RoomOccupancyStatusDAL(_connectionSettings.Value.ConnectionString);
					string roomReleaseResultHs = objRoomOccupancyStatusDALHs.Release_Room_On_Cancellation(model.IPDApplicationFormid, pvar_usersid);
					if (roomReleaseResultHs != "201.1")
						_logger.LogWarning($"Release_Room_On_Cancellation returned: {roomReleaseResultHs} for IPD {model.IPDApplicationFormid}");

					var hsRow = ipdTableHs.Rows[0];
					CreateNotifier().SendIPDCancellationNotification(
						model.IPDApplicationFormid,
						hsRow["tenantid"]?.ToString(),
						hsRow["bookingreferencenumber"]?.ToString(),
						$"{hsRow["firstname"]} {hsRow["lastname"]}".Trim(),
						model.cancellationreason,
						GetCancellationRefundAmount(model.IPDApplicationFormid, "Patient"),
						"HealthSeekerRequested");

					//return Ok(new
					//{
					//	message          = "201.1",
					//	refundid         = (string)null,
					//	refundamount     = 0m,
					//	refundstatus     = "PendingFrontDesk",
					//	refundmode       = "Cash",
					//	israzorpayrefund = false,
					//	bookingstatus    = "Cancelled by Patient",
					//	roomreleased     = roomReleaseResultHs == "201.1",
					//	refunddeferred   = true
					//});
					return Ok(new
					{
						message = "201.1",
						refundid = (string)null,
						refundamount = 0m,
						refundstatus = "PendingFrontDesk",
						refundmode = "Cash",
						israzorpayrefund = false,
						bookingstatus = "Cancellation Requested",
						appointmentscancelled = appointmentCancelResultHs == "201.1",
						roomreleased = roomReleaseResultHs == "201.1",
						refunddeferred = true
					});
				}

				bool hasRefundMode = !string.IsNullOrWhiteSpace(model.refundmode);
				bool isRazorpayRefund = hasRefundMode && model.refundmode.Equals("RazorPay", StringComparison.OrdinalIgnoreCase);

				// 1. Get refund breakdown rows (may be empty when no refund policy is configured)
				var refundTable = objIPDApplicationFormDAL.Get_IPD_Refund_Details(model.IPDApplicationFormid, model.cancellationby);

				// Sum policy-calculated refund amounts across all returned rows
				decimal policyRefundAmount = 0;
				decimal policyPaidAmount = 0;
				if (refundTable != null && refundTable.Rows.Count > 0)
				{
					foreach (System.Data.DataRow r in refundTable.Rows)
					{
						if (refundTable.Columns.Contains("refund_amount") && r["refund_amount"] != System.DBNull.Value)
							policyRefundAmount += Convert.ToDecimal(r["refund_amount"]);
						else if (refundTable.Columns.Contains("total_refund_amount") && r["total_refund_amount"] != System.DBNull.Value)
							policyRefundAmount += Convert.ToDecimal(r["total_refund_amount"]);

						if (refundTable.Columns.Contains("paid_amount") && r["paid_amount"] != System.DBNull.Value)
							policyPaidAmount += Convert.ToDecimal(r["paid_amount"]);
						else if (refundTable.Columns.Contains("total_paid_amount") && r["total_paid_amount"] != System.DBNull.Value)
							policyPaidAmount += Convert.ToDecimal(r["total_paid_amount"]);
					}
				}

				// IPD cancellation refunds are all-or-nothing: always use the full policy-calculated
				// refundable amount and reject any partial amount sent by the client.
				decimal totalRefundAmount = policyRefundAmount;
				if (!isRazorpayRefund && model.manualrefundamount.HasValue
					&& Math.Round(model.manualrefundamount.Value, 2) != Math.Round(policyRefundAmount, 2))
				{
					return BadRequest($"Partial refund is not allowed for cancelled IPD bookings. Refund the full amount Rs.{policyRefundAmount:N2}.");
				}

				if (totalRefundAmount < 0)
					return BadRequest("Refund amount cannot be negative");

				if (totalRefundAmount > 0 && !hasRefundMode)
					return BadRequest("Refund mode is required");

				if (totalRefundAmount == 0 && !hasRefundMode)
					model.refundmode = "Cash"; // default for zero-refund cancellations

				// Hard guard: refund must never exceed what was paid.
				// Prefer policy table paid_amount; if unavailable, this check is skipped here and
				// Razorpay path will still be validated using original online paid amount.
				if (policyPaidAmount > 0 && totalRefundAmount > policyPaidAmount)
					return BadRequest($"Refund amount ₹{totalRefundAmount:N2} cannot exceed paid amount ₹{policyPaidAmount:N2}");

				// 2. Get IPD details (tenant, patient, booking reference)
				var ipdTable = objIPDApplicationFormDAL.Get_IPD_Payment_Details(model.IPDApplicationFormid);
				if (ipdTable == null || ipdTable.Rows.Count == 0)
					return BadRequest("IPD record not found");

				var ipdRow         = ipdTable.Rows[0];
				string tenantid    = ipdRow["tenantid"]?.ToString();
				string patientGuid = ipdRow["patientname"]?.ToString();
				string bookingRef  = ipdRow["bookingreferencenumber"]?.ToString();

				// ── RAZORPAY PATH ──────────────────────────────────────────────────────
				// 3. Fetch BillingPayment rows up-front (used by both Razorpay and non-Razorpay paths)
				var billingTable = objIPDApplicationFormDAL.Get_BillingPayment_ByIPD(model.IPDApplicationFormid);

				string refundId     = null;
				string refundStatus = isRazorpayRefund ? "Pending" : "Manual";

				if (isRazorpayRefund)
				{
					if (billingTable == null || billingTable.Rows.Count == 0)
						return BadRequest("No billing payment found for this IPD booking");

					// Q1 Guard: only allow Razorpay refund if original payment was made online.
					// paymentmode must contain Online/RazorPay OR transactionreference must start with pay_ (Razorpay format).
					bool wasOnlinePayment = false;
					decimal onlinePaidBalance = 0;
					foreach (System.Data.DataRow br in billingTable.Rows)
					{
						string pm  = br.Table.Columns.Contains("paymentmode") ? br["paymentmode"]?.ToString() ?? "" : "";
						string txn = br["transactionreference"]?.ToString() ?? "";
						string receivableFor = br.Table.Columns.Contains("receivablefor") ? br["receivablefor"]?.ToString() ?? "" : "";
						decimal paidAmount = br.Table.Columns.Contains("amount") && br["amount"] != System.DBNull.Value ? Convert.ToDecimal(br["amount"]) : 0;
						decimal alreadyRefunded = br.Table.Columns.Contains("refundedamount") && br["refundedamount"] != System.DBNull.Value ? Convert.ToDecimal(br["refundedamount"]) : 0;
						bool isRefundRow = receivableFor.Equals("Cancellation Refund", StringComparison.OrdinalIgnoreCase) || paidAmount < 0;
						if (pm.IndexOf("Online", StringComparison.OrdinalIgnoreCase) >= 0
						     || pm.IndexOf("Razor", StringComparison.OrdinalIgnoreCase) >= 0
						     || txn.StartsWith("pay_", StringComparison.OrdinalIgnoreCase))
						{
							wasOnlinePayment = true;
							if (!isRefundRow)
								onlinePaidBalance += Math.Max(0, paidAmount - alreadyRefunded);
						}
					}

					if (!wasOnlinePayment)
						return BadRequest("The original booking payment was not made online. Please select Cash, UPI, Card, or NetBanking as the refund mode instead of RazorPay.");

					if (totalRefundAmount > onlinePaidBalance)
						return BadRequest($"Refund amount ₹{totalRefundAmount:N2} cannot exceed online paid balance ₹{onlinePaidBalance:N2}");

					// 3a. Get Razorpay key/secret for tenant
					var objPaymentConfigDAL = new PaymentConfigDAL(_connectionSettings.Value.ConnectionString);
					var configTable = objPaymentConfigDAL.get_Payment_Config(tenantid, "RazorPay");
					if (configTable == null || configTable.Rows.Count == 0)
						return BadRequest("Razorpay config not found for tenant");

					string paymentConfigid = configTable.Rows[0]["paymentconfigid"]?.ToString();
					var keyinfoTable = objPaymentConfigDAL.getById_keyinfo(paymentConfigid);
					if (keyinfoTable == null || keyinfoTable.Rows.Count == 0)
						return BadRequest("Razorpay key info not configured");

					string keyid     = keyinfoTable.Rows[0]["keyid"]?.ToString();
					string keysecret = keyinfoTable.Rows[0]["keysecret"]?.ToString();

					// 3b. Get original Razorpay payment_id from BillingPayment
					// billingTable already fetched and validated before Razorpay block - see above

					string razorpayPaymentId = null;
					foreach (System.Data.DataRow br in billingTable.Rows)
					{
						string txn = br["transactionreference"]?.ToString();
						if (!string.IsNullOrWhiteSpace(txn))
						{
							razorpayPaymentId = txn;
							break;
						}
					}

					if (string.IsNullOrWhiteSpace(razorpayPaymentId))
						return BadRequest("No Razorpay payment reference found to process refund");

					// 3d. Call Razorpay Refund API (server-side)
					if (totalRefundAmount > 0)
					{
						// Use Math.Round to avoid decimal truncation, then convert to paise (integer)
						long refundAmountInPaise = (long)Math.Round(totalRefundAmount * 100, MidpointRounding.AwayFromZero);

						// Razorpay minimum refund is ₹1 (100 paise)
						if (refundAmountInPaise < 100)
						{
							_logger.LogError($"Refund amount {totalRefundAmount} ({refundAmountInPaise} paise) is below Razorpay minimum of ₹1 (100 paise)");
							return BadRequest($"Refund amount ₹{totalRefundAmount:N2} is below the minimum refund amount of ₹1.00 supported by Razorpay");
						}

						var rzpClient  = new RestSharp.RestClient("https://api.razorpay.com/v1");
						var rzpRequest = new RestSharp.RestRequest($"payments/{razorpayPaymentId}/refund", RestSharp.Method.Post);
						rzpRequest.AddHeader("Authorization",
							"Basic " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(keyid + ":" + keysecret)));
						rzpRequest.AddJsonBody(new
						{
							amount  = refundAmountInPaise,
							speed   = "normal",
							notes   = new
							{
								ipdapplicationformid = model.IPDApplicationFormid,
								cancellationby       = model.cancellationby,
								cancelledby_role     = model.sessionuserrole ?? "",
								reason               = model.cancellationreason ?? "IPD Booking Cancelled"
							},
							receipt = "REFUND-" + bookingRef
						});

						var rzpResponse = rzpClient.Execute(rzpRequest);
						if (!rzpResponse.IsSuccessful)
						{
							_logger.LogError("Razorpay refund failed: " + rzpResponse.Content);
							return BadRequest("Razorpay refund failed: " + rzpResponse.Content);
						}

						var rzpRefundObj = Newtonsoft.Json.Linq.JObject.Parse(rzpResponse.Content);
						refundId     = rzpRefundObj["id"]?.ToString();
						refundStatus = rzpRefundObj["status"]?.ToString() ?? "pending";
					}
				}
				// ── END RAZORPAY PATH ──────────────────────────────────────────────────

				// Build a structured refund reason capturing all key audit info
				string structuredReason =
					$"Cancelled by: {model.cancellationby}. " +
					$"Refund Mode: {model.refundmode}. " +
					$"Processed by Role: {model.sessionuserrole ?? "N/A"}. " +
					$"Reason: {model.cancellationreason ?? "N/A"}.";

				// 4. Save PaymentResponse record for the refund
				var objPaymentResponseDAL = new PaymentResponseDAL(_connectionSettings.Value.ConnectionString);
				var refundResponse = new NalamVazha.Models.PaymentResponseModel
				{
					PaymentResponseid = Guid.NewGuid(),
					tenantid          = Guid.TryParse(tenantid, out var tid) ? tid : (Guid?)null,
					paymenttype       = "Room Booking Advance",
					transactiontime   = GetApplicationNow(),
					orderid           = "REFUND-" + bookingRef,
					paymentid         = refundId ?? model.refundmode,  // for non-Razorpay, store mode as reference
					status            = "Refund Initiated",
					amount            = totalRefundAmount,
					refundedamount    = totalRefundAmount,
					refundreason      = structuredReason,
					createduser       = Guid.TryParse(pvar_usersid, out var uid) ? uid : (Guid?)null,
					craftmyapp_actionmethodname = "Add_Payment_Response"
				};
				objPaymentResponseDAL.Add_Payment_Response(refundResponse);

				// 5. Save BillingPayment refund record (negative amount = outgoing refund)
				var objBillingPaymentDAL = new BillingPaymentDAL(_connectionSettings.Value.ConnectionString);
				var refundBilling = new NalamVazha.Models.BillingPaymentModel
				{
					BillingPaymentid     = Guid.NewGuid(),
					tenantid             = Guid.TryParse(tenantid, out var btid) ? btid : (Guid?)null,
					paymentdate          = GetApplicationNow().Date,
					receivablefor = "Cancellation Refund",
					ipdnumber= Guid.TryParse(model.IPDApplicationFormid, out var pvid) ? pvid : (Guid?)null,
					//		patientvisit         = Guid.TryParse(model.IPDApplicationFormid, out var pvid) ? pvid : (Guid?)null,
					patientname          = Guid.TryParse(patientGuid, out var bpid) ? bpid : (Guid?)null,
					amount               = -totalRefundAmount,
					paymentmode          = model.refundmode,                             // e.g. "Cash", "UPI", "RazorPay"
					transactionreference = refundId ?? "",                               // Razorpay refund ID if applicable
					paymentstatus        = "Refund Initiated",
					refundmode           = model.refundmode,                             // dedicated refund mode column
					refundedamount       = totalRefundAmount,
					refundstatus         = string.Equals(refundStatus, "Refunded", StringComparison.OrdinalIgnoreCase)
					                         ? "Refund Initiated"
					                         : (string.IsNullOrWhiteSpace(refundStatus) ? "Refund Initiated" : refundStatus),
					refundedby           = Guid.TryParse(pvar_usersid, out var rbid) ? rbid : (Guid?)null,
					refundreason         = structuredReason,
					collectedby          = Guid.TryParse(pvar_usersid, out var cbid) ? cbid : (Guid?)null,
					remarks              = isRazorpayRefund
					                         ? $"RazorPay Refund ID: {refundId}. Status: {refundStatus}."
					                         : $"Manual {model.refundmode} refund recorded by {model.sessionuserrole ?? "N/A"}.",
					createduser          = Guid.TryParse(pvar_usersid, out var bcuid) ? bcuid : (Guid?)null,
					craftmyapp_actionmethodname = "Add_Billing_Payment",
					currency="INR"
				};
				objBillingPaymentDAL.Add_Billing_Payment(refundBilling);

				// 6. Update IPD booking status based on who cancelled
				//var cancelStatus = model.cancellationby.Equals("Patient", StringComparison.OrdinalIgnoreCase) ? "Cancelled by Patient" : "Cancelled by Front Desk";
				var cancelStatus = model.cancellationby.Equals("Patient", StringComparison.OrdinalIgnoreCase) ? "Cancellation Requested" : "Cancelled";
				if (totalRefundAmount > 0)
					cancelStatus = "Cancelled - Refunded";
				objIPDApplicationFormDAL.Update_IPD_BookingStatus(model.IPDApplicationFormid, cancelStatus, pvar_usersid);

				var objClinicalAppointmentDAL = new NalamVazha.DAL.ClinicalAppointmentDAL(_connectionSettings.Value.ConnectionString);
				string appointmentCancelResult = objClinicalAppointmentDAL.Cancel_IPD_Appointments_By_Booking(model.IPDApplicationFormid, pvar_usersid);
				if (appointmentCancelResult != "201.1")
					_logger.LogWarning($"Cancel_IPD_Appointments_By_Booking returned: {appointmentCancelResult} for IPD {model.IPDApplicationFormid}");

				// 7. Release allocated room (Q3): soft-deletes all active RoomOccupancyStatus records
				// for this IPD (isdeleted=true, status='Cancelled') so the room becomes immediately
				// available for re-allocation without any manual intervention.
				var objRoomOccupancyStatusDAL = new NalamVazha.DAL.RoomOccupancyStatusDAL(_connectionSettings.Value.ConnectionString);
				string roomReleaseResult = objRoomOccupancyStatusDAL.Release_Room_On_Cancellation(model.IPDApplicationFormid, pvar_usersid);
				if (roomReleaseResult != "201.1")
					_logger.LogWarning($"Release_Room_On_Cancellation returned: {roomReleaseResult} for IPD {model.IPDApplicationFormid}");

				CreateNotifier().SendIPDCancellationNotification(
					model.IPDApplicationFormid,
					tenantid,
					bookingRef,
					$"{ipdRow["firstname"]} {ipdRow["lastname"]}".Trim(),
					model.cancellationreason,
					totalRefundAmount,
					model.cancellationby.Equals("Patient", StringComparison.OrdinalIgnoreCase)
						? "HealthSeekerRefundApproved"
						: "HospitalCancelled");

				return Ok(new
				{
					message          = "201.1",
					refundid         = refundId,
					refundamount     = totalRefundAmount,
					refundstatus     = refundStatus,
					refundmode       = model.refundmode,
					israzorpayrefund = isRazorpayRefund,
					bookingstatus    = cancelStatus,

					appointmentscancelled = appointmentCancelResult == "201.1",
					roomreleased     = roomReleaseResult == "201.1"
				});
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Cancel_IPD_Booking error: " + ex.Message);
				return BadRequest(ex.Message);
			}
		}

		private decimal GetCancellationRefundAmount(string ipdApplicationFormId, string cancellationBy)
		{
			decimal total = 0m;
			var table = objIPDApplicationFormDAL.Get_IPD_Refund_Details(ipdApplicationFormId, cancellationBy);
			if (table == null) return total;
			foreach (System.Data.DataRow row in table.Rows)
			{
				if (table.Columns.Contains("refund_amount") && row["refund_amount"] != DBNull.Value)
					total += Convert.ToDecimal(row["refund_amount"]);
				else if (table.Columns.Contains("total_refund_amount") && row["total_refund_amount"] != DBNull.Value)
					total += Convert.ToDecimal(row["total_refund_amount"]);
			}
			return total;
		}

		[HttpGet()]
		[AllowAnonymous]
		[ActionName("Get_IPD_Cancellation_Request_Remarks")]
		public virtual IActionResult Get_IPD_Cancellation_Request_Remarks(string IPDApplicationFormid)
		{
			if (string.IsNullOrWhiteSpace(IPDApplicationFormid) ||
				!Guid.TryParse(IPDApplicationFormid, out _))
				return BadRequest("Valid IPDApplicationFormid is required.");

			return Ok(new
			{
				remarks = objIPDApplicationFormDAL.Get_IPD_Cancellation_Request_Remarks(IPDApplicationFormid)
			});
		}

	[HttpPost()]
		[ActionName("verify_IPDApplicationForm")]
		public virtual string verify_IPDApplicationForm([FromBody] IPDApplicationFormReviewModel model)
			{
					string message ="";

					try{
						  if (string.Equals(model?.verifiedstatus, "Rework", StringComparison.OrdinalIgnoreCase)
							  && string.IsNullOrWhiteSpace(model.reviewcomments))
						  {
							  return "A reason is required when sending an IPD application for rework.";
						  }

						  var wasAlreadyReworkRequested = false;
						  if (string.Equals(model?.verifiedstatus, "Rework", StringComparison.OrdinalIgnoreCase))
						  {
							  var current = objIPDApplicationFormDAL.getById_IPDApplicationForm(model.IPDApplicationFormid);
							  wasAlreadyReworkRequested = string.Equals(
								  current?.bookingstatus?.Trim(), "Rework Requested", StringComparison.OrdinalIgnoreCase);
							  if (wasAlreadyReworkRequested)
								  return "201.1";
						  }

						  model.verifiedby = pvar_usersid;
						  message = objIPDApplicationFormDAL.verify_IPDApplicationForm(model);

						  if (message.Replace("\"", "") == "201.1")
						  {
							  switch (model.verifiedstatus)
							  {
								  case "Approved":
									  // Front desk approves -> Provisional Booking (admin blocks room via Allot_Room)
									  var provisionalResult = objIPDApplicationFormDAL.Update_IPD_BookingStatus(
										  model.IPDApplicationFormid, "Provisional Booking", pvar_usersid);
									  if (provisionalResult.Replace("\"", "") == "201.1")
									  {
										  CreateNotifier().SendBookingStatusNotification(
											  model.IPDApplicationFormid, "Provisional Booking");
									  }
									  break;
								  case "Rework":
									  // Front desk sends back to patient with comments
									  var reworkResult = objIPDApplicationFormDAL.Update_IPD_BookingStatus(
										  model.IPDApplicationFormid, "Rework Requested", pvar_usersid);
									  if (!wasAlreadyReworkRequested && reworkResult.Replace("\"", "") == "201.1")
									  {
										  CreateNotifier().SendIPDReworkNotification(
											  model.IPDApplicationFormid, model.reviewcomments);
									  }
									  break;
								  case "Rejected":
									  objIPDApplicationFormDAL.Update_IPD_BookingStatus(
										  model.IPDApplicationFormid, "Rejected", pvar_usersid);
									  break;
								  case "Doctor Review":
									  // Front desk escalates to doctor(s) for medical opinion
									  objIPDApplicationFormDAL.Update_IPD_BookingStatus(
										  model.IPDApplicationFormid, "Doctor Review", pvar_usersid);
									  break;
							  }
						  }

					}catch(Exception ex){
						message=ex.Message;
                         _logger.LogError(ex,"An exception occurred in - verify_IPDApplicationForm, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));

					}

						return message;

			}

	// -- STEP 3: Doctor Review
	/// <summary>
	/// Any doctor can approve or reject an IPD form escalated for doctor review.
	/// Approval -> bookingstatus = "Provisional Booking"; Rejection -> "Rejected".
	/// </summary>
	[HttpPost()]
	[ActionName("Doctor_Review_IPD")]
	public virtual IActionResult Doctor_Review_IPD([FromBody] NalamVazha.Models.IPDDoctorReviewModel model)
	{
		string message = "";
		try
		{
			var authHeader = HttpContext.Request.Headers["Authorization"][0];
			if (!authHeader.StartsWith("Bearer "))
				return Unauthorized("Invalid Token");

			var token = authHeader.Substring("Bearer ".Length);
			string[] userdetails = obj_External_System_DAL.get_users_by_token(token);
			string doctorUserId = userdetails[0].ToString();

			// Record review via shared verify DAL (reuses verify_IPDApplicationForm SP)
			var reviewModel = new IPDApplicationFormReviewModel
			{
				IPDApplicationFormid = model.IPDApplicationFormid,
				verifiedby = doctorUserId,
				verifiedstatus = model.reviewaction == "Approved" ? "Approved" : "Rejected",
				reviewcomments = model.reviewcomments
			};
			message = objIPDApplicationFormDAL.verify_IPDApplicationForm(reviewModel);

			if (message.Replace("\"", "") == "201.1")
			{
				string newStatus = model.reviewaction == "Approved" ? "Provisional Booking" : "Rejected";
				var statusResult = objIPDApplicationFormDAL.Update_IPD_BookingStatus(
					model.IPDApplicationFormid, newStatus, doctorUserId);
				if (newStatus == "Provisional Booking" && statusResult.Replace("\"", "") == "201.1")
				{
					CreateNotifier().SendBookingStatusNotification(
						model.IPDApplicationFormid, newStatus);
				}
			}

			if (message.Replace("\"", "") == "201.1") return Ok(message);
			else if (message.Replace("\"", "") == "401.1") return Unauthorized(message);
			else return BadRequest(message);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Doctor_Review_IPD error: " + ex.Message);
			return BadRequest(ex.Message);
		}
	}

	// -- STEP 6a: Patient Confirms Arrival (anonymous - accessed via email link)
	/// <summary>
	/// Patient clicks Confirm Arrival link from email. No login required.
	/// Sets bookingstatus = Confirmed for Arrival.
	/// </summary>
	[HttpPost()]
	[AllowAnonymous]
	[ActionName("Confirm_Patient_Arrival")]
	public virtual IActionResult Confirm_Patient_Arrival([FromBody] NalamVazha.Models.IPDPatientArrivalConfirmModel model)
	{
		try
		{
			if (string.IsNullOrEmpty(model?.IPDApplicationFormid))
				return BadRequest("IPD form ID is required");

			//string result = objIPDApplicationFormDAL.Update_IPD_BookingStatus(model.IPDApplicationFormid, "Confirmed for Arrival", pvar_usersid);
			string result = objIPDApplicationFormDAL.Update_IPD_BookingStatus(model.IPDApplicationFormid, "Arrival Confirmed", pvar_usersid);

			if (result.Replace("\"", "") == "201.1")
				return Ok(new { message = "201.1" });

			return BadRequest(result);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Confirm_Patient_Arrival error: " + ex.Message);
			return BadRequest(ex.Message);
		}
	}
[HttpGet()]
			
			[ActionName("Approved_IPD_Application_Forms")]
			public virtual JObject Approved_IPD_Application_Forms(string tenantid
,string patientname="", string bookingstatus = ""
, string loginUserID="",int? pagesize=100 , int? pagenumber=0,string searchterm="",string  sort_fields = "")
			{
					 
				  	JObject jsonIPDApplicationForm = null;
					try
					{
                        tenantid=pvar_tenantid;
						jsonIPDApplicationForm = objIPDApplicationFormDAL.Approved_IPD_Application_Forms( tenantid
, patientname, bookingstatus
, pagesize,pagenumber,searchterm,sort_fields);
					}
					catch (Exception ex)
					{
                             _logger.LogError(ex,"An exception occurred in - Approved_IPD_Application_Forms, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
                        
					}
					return jsonIPDApplicationForm;

			   }
			   
[HttpGet()]
			  [ActionName("getById_allinfo_IPDApplicationForm")]
			  public virtual System.Data.DataTable getById_allinfo_IPDApplicationForm(string IPDApplicationFormid)
			  { 
				    DataTable dtIPDApplicationForm = new DataTable();
					try
					{
						  dtIPDApplicationForm = objIPDApplicationFormDAL.getById_allinfo_IPDApplicationForm(IPDApplicationFormid);
					}
					catch (Exception ex)
					{
                         _logger.LogError(ex,"An exception occurred in - getById_allinfo_IPDApplicationForm, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
   
					}
					return dtIPDApplicationForm;

			  }
[HttpGet()]
							[ActionName("lookup_IPDApplicationForm_patientname")]
							public virtual System.Data.DataTable lookup_IPDApplicationForm_patientname(String tenantid,string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtIPDApplicationForm = new DataTable();
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
										dtIPDApplicationForm = objIPDApplicationFormDAL.lookup_IPDApplicationForm_patientname(tenantid,searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_IPDApplicationForm_patientname, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtIPDApplicationForm;
							}
[HttpGet()]
							[ActionName("lookup_IPDApplicationForm_countryoforigin")]
							public virtual System.Data.DataTable lookup_IPDApplicationForm_countryoforigin(string loginUserID="")
							{
									DataTable dtIPDApplicationForm = new DataTable();
									try
									{   
                                        
                                        
										dtIPDApplicationForm = objIPDApplicationFormDAL.lookup_IPDApplicationForm_countryoforigin();
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_IPDApplicationForm_countryoforigin, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtIPDApplicationForm;
							}
[HttpGet()]
							[ActionName("lookup_IPDApplicationForm_passportissuingcountry")]
							public virtual System.Data.DataTable lookup_IPDApplicationForm_passportissuingcountry(string loginUserID="")
							{
									DataTable dtIPDApplicationForm = new DataTable();
									try
									{   
                                        
                                        
										dtIPDApplicationForm = objIPDApplicationFormDAL.lookup_IPDApplicationForm_passportissuingcountry();
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_IPDApplicationForm_passportissuingcountry, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtIPDApplicationForm;
							}
[HttpGet()]
							[ActionName("lookup_IPDApplicationForm_visaissuedcountry")]
							public virtual System.Data.DataTable lookup_IPDApplicationForm_visaissuedcountry(string loginUserID="")
							{
									DataTable dtIPDApplicationForm = new DataTable();
									try
									{   
                                        
                                        
										dtIPDApplicationForm = objIPDApplicationFormDAL.lookup_IPDApplicationForm_visaissuedcountry();
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_IPDApplicationForm_visaissuedcountry, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtIPDApplicationForm;
							}
[HttpGet()]
							[ActionName("lookup_IPDApplicationForm_consentform")]
							public virtual System.Data.DataTable lookup_IPDApplicationForm_consentform(String tenantid,string loginUserID="")
							{
									DataTable dtIPDApplicationForm = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtIPDApplicationForm = objIPDApplicationFormDAL.lookup_IPDApplicationForm_consentform(tenantid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_IPDApplicationForm_consentform, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtIPDApplicationForm;
							}
[HttpGet()]
							[ActionName("lookup_IPDApplicationForm_packagename")]
							public virtual System.Data.DataTable lookup_IPDApplicationForm_packagename(String tenantid,string loginUserID="")
							{
									DataTable dtIPDApplicationForm = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtIPDApplicationForm = objIPDApplicationFormDAL.lookup_IPDApplicationForm_packagename(tenantid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_IPDApplicationForm_packagename, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtIPDApplicationForm;
							}
[HttpGet()]
							[ActionName("lookup_IPDApplicationForm_medicalinfo_medicalconditionname")]
							public virtual System.Data.DataTable lookup_IPDApplicationForm_medicalinfo_medicalconditionname(string searchterm="",string pagesize="50", string pagenumber="1",string loginUserID="")
							{
									DataTable dtIPDApplicationForm = new DataTable();
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
										dtIPDApplicationForm = objIPDApplicationFormDAL.lookup_IPDApplicationForm_medicalinfo_medicalconditionname(searchterm,pageSize, pageNumber);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_IPDApplicationForm_medicalinfo_medicalconditionname, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtIPDApplicationForm;
							}
[HttpGet()]
							[ActionName("lookup_IPDApplicationForm_roompreference_roomtype")]
							public virtual System.Data.DataTable lookup_IPDApplicationForm_roompreference_roomtype(String tenantid,string loginUserID="")
							{
									DataTable dtIPDApplicationForm = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtIPDApplicationForm = objIPDApplicationFormDAL.lookup_IPDApplicationForm_roompreference_roomtype(tenantid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_IPDApplicationForm_roompreference_roomtype, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtIPDApplicationForm;
							}

							[HttpGet, ActionName("lookup_IPDApplicationForm_attendantroompreference_roomtypeatt")]
							public virtual DataTable lookup_IPDApplicationForm_attendantroompreference_roomtypeatt(String tenantid,string loginUserID="")
							{
								return objIPDApplicationFormDAL.lookup_IPDApplicationForm_attendantroompreference_roomtypeatt(tenantid);
							}
[HttpGet()]
							[ActionName("lookup_IPDApplicationForm_room_roomnumber")]
							public virtual System.Data.DataTable lookup_IPDApplicationForm_room_roomnumber(String tenantid,string loginUserID="")
							{
									DataTable dtIPDApplicationForm = new DataTable();
									try
									{   
                                        tenantid=pvar_tenantid;
                                        
										dtIPDApplicationForm = objIPDApplicationFormDAL.lookup_IPDApplicationForm_room_roomnumber(tenantid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_IPDApplicationForm_room_roomnumber, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtIPDApplicationForm;
							}

[HttpGet()]
							[ActionName("lookup_change_IPDApplicationForm_consentform")]
							public virtual System.Data.DataTable lookup_change_IPDApplicationForm_consentform(string PatientConsentid,string loginUserID="")
							{
									DataTable dtPatientConsent = new DataTable();
									try
									{
										dtPatientConsent = objIPDApplicationFormDAL.lookup_change_IPDApplicationForm_consentform(PatientConsentid);
									}
									catch (Exception ex)
									{
										 _logger.LogError(ex,"An exception occurred in - lookup_change_IPDApplicationForm_consentform, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" :ex.Message));
             
									}
									return dtPatientConsent;
							}

		// â”€â”€ STEP 6: Patient Arrival & Charges â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
		[HttpPost()]
		[ActionName("Record_Patient_Arrival")]
		public virtual IActionResult Record_Patient_Arrival([FromBody] NalamVazha.Models.IPDPatientArrivalRequestModel model)
		{
			try
			{
				// 1. Get IPD payment details to obtain patient Guid and tenantid
				var ipdTable = objIPDApplicationFormDAL.Get_IPD_Payment_Details(model.IPDApplicationFormid);
				if (ipdTable == null || ipdTable.Rows.Count == 0)
					return NotFound("IPD record not found");

				var ipdRow = ipdTable.Rows[0];
				string tenantid   = ipdRow["tenantid"]?.ToString();
				string patientGuid = ipdRow["patientname"]?.ToString();

				// 2. Create PatientVisit record
				var newVisitId = Guid.NewGuid();
				var objPatientVisitDAL = new NalamVazha.DAL.PatientVisitDAL(_connectionSettings.Value.ConnectionString);
				var patientVisit = new NalamVazha.Models.PatientVisitModel
				{
					PatientVisitid  = newVisitId,
					tenantid        = Guid.TryParse(tenantid, out var tvid) ? tvid : (Guid?)null,
					patientname     = Guid.TryParse(patientGuid, out var ppid) ? ppid : Guid.Empty,
					visittype       = "IPD",
					visitdatetime   = DateTime.Now,
					ipdnumber       = Guid.TryParse(model.IPDApplicationFormid, out var ipdid) ? ipdid : (Guid?)null,
					visitstatus     = "Active",
					notes           = string.IsNullOrEmpty(model.notes) ? "Patient arrived for IPD admission" : model.notes,
					createduser     = Guid.TryParse(pvar_usersid, out var pvcuid) ? pvcuid : (Guid?)null,
					craftmyapp_actionmethodname = "Add_Patient_Visit"
				};
				string pvMsg = objPatientVisitDAL.Add_Patient_Visit(patientVisit);

				// 3. Charge Consultation Fee
				var objBillingDAL = new NalamVazha.DAL.BillingPaymentDAL(_connectionSettings.Value.ConnectionString);
				var consultBilling = new NalamVazha.Models.BillingPaymentModel
				{
					BillingPaymentid = Guid.NewGuid(),
					tenantid         = Guid.TryParse(tenantid, out var bctid) ? bctid : (Guid?)null,
					receivablefor = "Consultation Fee",
					patientvisit     = newVisitId,
					patientname      = Guid.TryParse(patientGuid, out var bcpid) ? bcpid : (Guid?)null,
					amount           = model.consultationfee > 0 ? model.consultationfee : 600,
					receivedamount   = model.consultationfee > 0 ? model.consultationfee : 600,
					currency         = "INR",
					conversionrate   = 1,
					paymentmode      = string.IsNullOrEmpty(model.paymentmode) ? "Cash" : model.paymentmode,
					paymentstatus    = "Paid",
					collectedby      = Guid.TryParse(pvar_usersid, out var bccid) ? bccid : (Guid?)null,
					remarks          = "Consultation fee charged on patient arrival",
					createduser      = Guid.TryParse(pvar_usersid, out var bcucid) ? bcucid : (Guid?)null,
					craftmyapp_actionmethodname = "Add_Billing_Payment"
				};
				objBillingDAL.Add_Billing_Payment(consultBilling);

				// 4. Create Admission Fee receivable (paid via Unified Payment page)
				var admissionFeeMsg = objIPDApplicationFormDAL.Ensure_AdmissionFee_Receivable(model.IPDApplicationFormid, pvar_usersid);
				if (!(admissionFeeMsg ?? "").Replace("\"", "").Contains("201.1"))
					_logger.LogWarning("Record_Patient_Arrival: Admission Fee receivable creation failed: {Msg}", admissionFeeMsg);

				// 5. Update booking status to "Admission Confirmed"
				objIPDApplicationFormDAL.Update_IPD_BookingStatus(model.IPDApplicationFormid, "Admission Confirmed", pvar_usersid);

				return Ok(new { message = "201.1", patientvisitid = newVisitId.ToString() });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Record_Patient_Arrival error: " + ex.Message);
				return BadRequest(ex.Message);
			}
		}

		// â”€â”€ STEP 7: Final Admission â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
		[HttpPost()]
		[ActionName("Final_Admission")]
		public virtual IActionResult Final_Admission([FromBody] NalamVazha.Models.IPDFinalAdmissionRequestModel model)
		{
			try
			{
				// 1. Receive remaining/pending payment (balance) if provided – create billing entry like Patient_Arrival_Charges
				if (model.advancedeposit > 0)
				{
					var ipdTable = objIPDApplicationFormDAL.Get_IPD_Payment_Details(model.IPDApplicationFormid);
					if (ipdTable != null && ipdTable.Rows.Count > 0)
					{
						var ipdRow = ipdTable.Rows[0];
						var objBillingDAL = new NalamVazha.DAL.BillingPaymentDAL(_connectionSettings.Value.ConnectionString);
						var balanceBilling = new NalamVazha.Models.BillingPaymentModel
						{
							BillingPaymentid = Guid.NewGuid(),
							tenantid = Guid.TryParse(ipdRow["tenantid"]?.ToString(), out var fatid) ? fatid : (Guid?)null,
							receivablefor = "Balance Payment",
							patientvisit = Guid.TryParse(model.PatientVisitId, out var fapvid) ? fapvid : (Guid?)null,
							patientname = Guid.TryParse(ipdRow["patientname"]?.ToString(), out var fapid) ? fapid : (Guid?)null,
							amount = model.advancedeposit,
							receivedamount = model.advancedeposit,
							currency = "INR",
							conversionrate = 1,
							paymentmode = string.IsNullOrEmpty(model.paymentmode) ? "Cash" : model.paymentmode,
							paymentstatus = "Paid",
							collectedby = Guid.TryParse(pvar_usersid, out var facid) ? facid : (Guid?)null,
							remarks = string.IsNullOrEmpty(model.notes) ? "Remaining amount received at final admission" : model.notes,
							createduser = Guid.TryParse(pvar_usersid, out var faucid) ? faucid : (Guid?)null,
							craftmyapp_actionmethodname = "Add_Billing_Payment"
						};
						objBillingDAL.Add_Billing_Payment(balanceBilling);
					}
				}

				// 2. Update RoomOccupancyStatus from "Blocked" to "Admitted" (Step 7f)
				//var objRoomOccDAL = new NalamVazha.DAL.RoomOccupancyStatusDAL(_connectionSettings.Value.ConnectionString);
				//objRoomOccDAL.Update_Room_Occupancy_By_IPD(model.IPDApplicationFormid, "Admitted", pvar_usersid);

				// 3. Update IPD booking status to "Admitted"
				objIPDApplicationFormDAL.Update_IPD_BookingStatus(model.IPDApplicationFormid, "Admitted", pvar_usersid);
				return Ok(new { message = "201.1" });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Final_Admission error: " + ex.Message);
				return BadRequest(ex.Message);
			}
		}

		[HttpGet()]
		[ActionName("Get_IPD_Billing_Summary")]
		public virtual IActionResult Get_IPD_Billing_Summary(string IPDApplicationFormid)
		{
			try
			{
				var summary = BuildIPDBillingSummary(IPDApplicationFormid, out _);
				return Ok(summary);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Get_IPD_Billing_Summary error: " + ex.Message);
				return BadRequest(ex.Message);
			}
		}

		private NalamVazha.Models.IPDBillingSummaryModel BuildIPDBillingSummary(string IPDApplicationFormid, out DataRow ipdRow)
		{
			RecoverCapturedIPDBookingDepositIfNeeded(IPDApplicationFormid);

			var ipdTable = objIPDApplicationFormDAL.Get_IPD_Payment_Details(IPDApplicationFormid);
			if (ipdTable == null || ipdTable.Rows.Count == 0)
				throw new Exception("IPD record not found");

			ipdRow = ipdTable.Rows[0];
			string tenantid = ipdRow["tenantid"]?.ToString();
			string latestMobileNumber = GetLatestPatientMobileNumber(ipdRow);

			decimal totalAmt = ipdRow.Table.Columns.Contains("totalamount") && ipdRow["totalamount"] != DBNull.Value ? Convert.ToDecimal(ipdRow["totalamount"]) : 0;
			decimal advanceAmt = ipdRow.Table.Columns.Contains("advanceamount") && ipdRow["advanceamount"] != DBNull.Value ? Convert.ToDecimal(ipdRow["advanceamount"]) : 0;

			int daysofstay = 1;
			if (ipdRow.Table.Columns.Contains("daysofstay") && ipdRow["daysofstay"] != DBNull.Value && ipdRow["daysofstay"] != null)
			{
				try { daysofstay = Convert.ToInt32(ipdRow["daysofstay"]); } catch { daysofstay = 1; }
			}
			if (daysofstay <= 0) daysofstay = 1;

			string blockedRoomJson = "";
			if (ipdRow.Table.Columns.Contains("blocked_room_details_json") && ipdRow["blocked_room_details_json"] != DBNull.Value && ipdRow["blocked_room_details_json"] != null)
				blockedRoomJson = ipdRow["blocked_room_details_json"].ToString();

			decimal payableFromBlockedRooms = ComputePayableFromBlockedRoomsJson(blockedRoomJson, daysofstay);
			decimal displayTotalAmount = payableFromBlockedRooms > 0 ? payableFromBlockedRooms : totalAmt;

			string patientvisitid = ipdRow.Table.Columns.Contains("patientvisitid") && ipdRow["patientvisitid"] != DBNull.Value ? ipdRow["patientvisitid"].ToString() : "00000000-0000-0000-0000-000000000000";
			string estimatedArrival = ipdRow.Table.Columns.Contains("estimatedarrival") && ipdRow["estimatedarrival"] != DBNull.Value ? ipdRow["estimatedarrival"].ToString() : "";
			var ipdModel = objIPDApplicationFormDAL.getById_IPDApplicationForm(IPDApplicationFormid);
			if (ipdModel != null && ipdModel.estimatedarrival.HasValue)
			{
				estimatedArrival = ipdModel.estimatedarrival.Value.ToString("dd/MM/yyyy HH:mm");
			}

			var objBillingDAL = new NalamVazha.DAL.BillingPaymentDAL(_connectionSettings.Value.ConnectionString);
			var paymentsTable = objBillingDAL.Get_Billing_Payments_For_IPD(IPDApplicationFormid, tenantid);

				decimal bookingDeposit   = 0;
				decimal consultationFee  = 0;
				decimal admissionFee     = 0;
				decimal advanceDeposit   = 0;
				decimal balancePayment   = 0;
				string billingCurrency   = "INR";

			if (paymentsTable != null)
			{
				foreach (DataRow row in paymentsTable.Rows)
				{
						if (row.Table.Columns.Contains("currency")
							&& row["currency"] != DBNull.Value
							&& !string.IsNullOrWhiteSpace(row["currency"].ToString()))
							billingCurrency = row["currency"].ToString().Trim().ToUpperInvariant();
						string pFor  = row["receivablefor"]?.ToString() ?? "";
						decimal amt  = row["amount"] != DBNull.Value ? Convert.ToDecimal(row["amount"]) : 0;
						if (pFor == "IPD Booking Deposit")   bookingDeposit  += amt;
						else if (pFor == "Consultation Fee")  consultationFee += amt;
						else if (pFor == "Admission Fee")     admissionFee    += amt;
						else if (pFor == "IPD Booking Deposit1")   advanceDeposit  += amt;
						else if (pFor == "Balance Payment")   balancePayment += amt;
				}
			}

			decimal totalPaid = bookingDeposit + consultationFee + admissionFee + advanceDeposit + balancePayment;
			decimal discountAmount = 0m;
			decimal concessionAmount = 0m;

			// The billing statement and payment screens must use the same receivable ledger total.
			// Room cost alone does not include deposits or other billable services.
			var receivablesTable = objIPDApplicationFormDAL.Get_All_IPD_Receivables(IPDApplicationFormid);
			if (receivablesTable != null && receivablesTable.Rows.Count > 0)
			{
				decimal receivableSubtotal = 0m;
				foreach (DataRow receivable in receivablesTable.Rows)
				{
					if (receivable.Table.Columns.Contains("amount") && receivable["amount"] != DBNull.Value)
					{
						var receivableAmount = Convert.ToDecimal(receivable["amount"]);
						receivableSubtotal += receivableAmount;
						if (receivableAmount < 0
							&& receivable.Table.Columns.Contains("receivablefor")
							&& string.Equals(receivable["receivablefor"]?.ToString()?.Trim(), "Concession", StringComparison.OrdinalIgnoreCase))
						{
							concessionAmount += Math.Abs(receivableAmount);
						}
					}
				}
				displayTotalAmount = receivableSubtotal;
			}

			// Deposits recorded as receivable payments are already included in totalPaid.
			// Do not subtract them a second time as a separate adjustment.
			// displayTotalAmount already includes the stored negative concession.
			// concessionAmount is positive display metadata, not a second deduction.
			decimal currentAmountPayable = displayTotalAmount - discountAmount - totalPaid;

			return new NalamVazha.Models.IPDBillingSummaryModel
			{
				IPDApplicationFormid  = IPDApplicationFormid,
				bookingreferencenumber = ipdRow["bookingreferencenumber"]?.ToString(),
					patientfullname       = (ipdRow["firstname"]?.ToString() + " " + ipdRow["lastname"]?.ToString()).Trim(),
					bookingstatus         = ipdRow["bookingstatus"]?.ToString(),
					totalamount           = displayTotalAmount,
					advanceamount         = advanceAmt,
					bookingdepositpaid    = bookingDeposit,
					consultationfeepaid   = consultationFee,
					admissionfeepaid      = admissionFee,
					totalamountpaid       = totalPaid,
					remainingbalance      = currentAmountPayable,
					subtotal              = displayTotalAmount,
					discountamount        = discountAmount,
					concessionamount      = concessionAmount,
					depositcreditadjustment = 0m,
					amountalreadypaid     = totalPaid,
					currentamountpayable  = currentAmountPayable,
					currencycode          = billingCurrency,
					amountinwords         = NalamVazha.Models.CurrencyAmountInWords.Convert(currentAmountPayable, billingCurrency),
					patientvisitid= patientvisitid,
				blocked_room_details_json = blockedRoomJson,
				daysofstay = daysofstay,
				tenantname = ipdRow["tenantname"]?.ToString(),
				organizationlogo = ipdRow["organizationlogo"]?.ToString(),
				addressline1 = ipdRow["addressline1"]?.ToString(),
				estimatedarrival = estimatedArrival,
				mobilenumber = latestMobileNumber,
				groupcode = ipdRow["groupcode"]?.ToString(),
			};
		}

		private void RecoverCapturedIPDBookingDepositIfNeeded(string ipdApplicationFormId)
		{
			if (string.IsNullOrWhiteSpace(ipdApplicationFormId) || !Guid.TryParse(ipdApplicationFormId, out var ipdGuid))
				return;
			if (HasSuccessfulIPDBookingDepositPayment(ipdGuid))
				return;

			var candidates = GetRecoverableIPDRazorpayRequests(ipdGuid);
			foreach (var request in candidates)
			{
				if (string.IsNullOrWhiteSpace(request.OrderId)) continue;
				var capturedPayment = TryGetCapturedRazorpayPayment(request);
				if (capturedPayment == null) continue;
				RecoverIPDBookingDepositFromCapturedPayment(ipdApplicationFormId, ipdGuid, request, capturedPayment);
				return;
			}
		}

		private List<RecoverableRazorpayRequest> GetRecoverableIPDRazorpayRequests(Guid ipdGuid)
		{
			var requests = new List<RecoverableRazorpayRequest>();
			using var conn = new NpgsqlConnection(_connectionSettings.Value.ConnectionString);
			conn.Open();
			using var cmd = new NpgsqlCommand("SELECT * FROM \"Get_Recoverable_IPD_Razorpay_Requests\"(@pvar_ipdapplicationformid)", conn);
			cmd.Parameters.AddWithValue("pvar_ipdapplicationformid", NpgsqlTypes.NpgsqlDbType.Uuid, ipdGuid);
			using var reader = cmd.ExecuteReader();
			while (reader.Read())
			{
				requests.Add(new RecoverableRazorpayRequest
				{
					PaymentRequestId = reader.GetGuid(0),
					TenantId = reader.IsDBNull(1) ? (Guid?)null : reader.GetGuid(1),
					OrderId = reader.IsDBNull(2) ? "" : reader.GetString(2),
					PaymentMarker = reader.IsDBNull(3) ? "" : reader.GetString(3),
					Amount = reader.IsDBNull(4) ? 0 : reader.GetDecimal(4),
					Currency = reader.IsDBNull(5) ? "INR" : reader.GetString(5),
					CreatedUser = reader.IsDBNull(6) ? (Guid?)null : reader.GetGuid(6)
				});
			}
			return requests;
		}

		private JObject TryGetCapturedRazorpayPayment(RecoverableRazorpayRequest request)
		{
			try
			{
				if (!request.TenantId.HasValue) return null;
				var configDal = new PaymentConfigDAL(_connectionSettings.Value.ConnectionString);
				var configTable = configDal.get_Payment_Config(request.TenantId.Value.ToString(), "RazorPay");
				if (configTable == null || configTable.Rows.Count == 0) return null;

				var paymentConfigId = configTable.Rows[0]["paymentconfigid"]?.ToString();
				var keyInfoTable = configDal.getById_keyinfo(paymentConfigId);
				if (keyInfoTable == null || keyInfoTable.Rows.Count == 0) return null;

				var keyId = keyInfoTable.Rows[0]["keyid"]?.ToString();
				var keySecret = keyInfoTable.Rows[0]["keysecret"]?.ToString();
				if (string.IsNullOrWhiteSpace(keyId) || string.IsNullOrWhiteSpace(keySecret)) return null;

				var client = new RestSharp.RestClient("https://api.razorpay.com/v1");
				var apiRequest = new RestSharp.RestRequest("orders/" + request.OrderId + "/payments", RestSharp.Method.Get);
				apiRequest.AddHeader("Authorization", "Basic " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(keyId + ":" + keySecret)));
				var response = client.Execute(apiRequest);
				if (!response.IsSuccessful || string.IsNullOrWhiteSpace(response.Content)) return null;

				var payload = JObject.Parse(response.Content);
				var items = payload["items"] as JArray;
				if (items == null) return null;
				foreach (var item in items.OfType<JObject>())
				{
					var status = item["status"]?.ToString();
					var paymentId = item["id"]?.ToString();
					if (!string.IsNullOrWhiteSpace(paymentId)
						&& (string.Equals(status, "captured", StringComparison.OrdinalIgnoreCase)
							|| string.Equals(status, "authorized", StringComparison.OrdinalIgnoreCase)))
					{
						return item;
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex, "Unable to recover captured IPD Razorpay payment for order {orderId}", request.OrderId);
			}
			return null;
		}

		private void RecoverIPDBookingDepositFromCapturedPayment(string ipdApplicationFormId, Guid ipdGuid, RecoverableRazorpayRequest request, JObject payment)
		{
			var paymentId = payment["id"]?.ToString();
			if (string.IsNullOrWhiteSpace(paymentId) || HasPaidPaymentResponse(paymentId) || HasSuccessfulIPDBookingDepositPayment(ipdGuid))
				return;

			var paymentResponseDal = new PaymentResponseDAL(_connectionSettings.Value.ConnectionString);
			var response = new PaymentResponseModel
			{
				PaymentResponseid = Guid.NewGuid(),
				tenantid = request.TenantId,
				paymentrequest = request.PaymentRequestId,
				paymenttype = "Booking Deposit",
				transactiontime = GetApplicationNow(),
				orderid = request.OrderId,
				paymentid = paymentId,
				status = "Paid",
				amount = request.Amount,
				paymentmethod = MapRazorpayMethod(payment["method"]?.ToString()),
				createduser = request.CreatedUser,
				craftmyapp_actionmethodname = "Add_Payment_Response"
			};
			SavePaymentResponseOrThrow(paymentResponseDal, response);

			var ipdTable = objIPDApplicationFormDAL.Get_IPD_Payment_Details(ipdApplicationFormId);
			string patientGuid = ipdTable?.Rows.Count > 0 ? ipdTable.Rows[0]["patientname"]?.ToString() : null;

			var billingDal = new BillingPaymentDAL(_connectionSettings.Value.ConnectionString);
			var billing = new BillingPaymentModel
			{
				BillingPaymentid = Guid.NewGuid(),
				tenantid = request.TenantId,
				paymentdate = GetApplicationNow().Date,
				receivablefor = "IPD Booking Deposit",
				ipdnumber = ipdGuid,
				patientname = Guid.TryParse(patientGuid, out var pid) ? pid : (Guid?)null,
				amount = request.Amount,
				receivedamount = request.Amount,
				currency = string.IsNullOrWhiteSpace(request.Currency) ? "INR" : request.Currency,
				conversionrate = 1,
				paymentmode = MapRazorpayMethod(payment["method"]?.ToString()),
				transactionreference = paymentId,
				paymentstatus = "Success",
				collectedby = request.CreatedUser,
				counterid = "Through Razorpay",
				remarks = $"Recovered captured Razorpay payment. Razorpay Order: {request.OrderId}. Payment ID: {paymentId}.",
				createduser = request.CreatedUser,
				craftmyapp_actionmethodname = "Add_Billing_Payment"
			};
			var billingMessage = billingDal.Add_Billing_Payment(billing);
			if (!(billingMessage ?? "").Replace("\"", "").Contains("201.1"))
				throw new Exception("Unable to recover captured IPD Razorpay billing entry: " + billingMessage);

			billingDal.ApplyIPDRazorpayReceivablePayment(ipdGuid, request.Amount, billing.BillingPaymentid.Value, request.CreatedUser);
			objIPDApplicationFormDAL.Update_IPD_BookingStatus(ipdApplicationFormId, "Provisional Confirmed", request.CreatedUser?.ToString());
		}

		private static string ResolveIPDPaymentCollectedBy(DataRow row)
		{
			if (row == null)
				return "";

			string GetValue(string columnName)
			{
				return row.Table.Columns.Contains(columnName) && row[columnName] != DBNull.Value
					? row[columnName]?.ToString() ?? ""
					: "";
			}

			var status = GetValue("paymentstatus");
			var mode = GetValue("paymentmode");
			var transactionReference = GetValue("transactionreference");
			var remarks = GetValue("remarks");
			var counter = GetValue("counterid");

			var isSuccessful = status.Equals("Success", StringComparison.OrdinalIgnoreCase)
				|| status.Equals("Paid", StringComparison.OrdinalIgnoreCase)
				|| status.Equals("Completed", StringComparison.OrdinalIgnoreCase);

			var isRazorpay = counter.Equals("Through Razorpay", StringComparison.OrdinalIgnoreCase)
				|| mode.IndexOf("Razorpay", StringComparison.OrdinalIgnoreCase) >= 0
				|| transactionReference.StartsWith("pay_", StringComparison.OrdinalIgnoreCase)
				|| remarks.IndexOf("Razorpay", StringComparison.OrdinalIgnoreCase) >= 0;

			if (isSuccessful && isRazorpay)
				return "Through Razorpay";

			return GetValue("collectedby_master");
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
					var pData = Newtonsoft.Json.Linq.JObject.Parse(fetchResp.Content);
					return MapRazorpayMethod(pData["method"]?.ToString());
				}
			}
			catch { }

			return "Online - Razorpay";
		}

		private static void SavePaymentResponseOrThrow(PaymentResponseDAL paymentResponseDal, PaymentResponseModel paymentResponse)
		{
			var responseMessage = paymentResponseDal.Add_Payment_Response(paymentResponse);
			if (!(responseMessage ?? "").Replace("\"", "").Contains("201.1"))
				throw new Exception("Payment response was not saved: " + responseMessage);
		}

		private class RecoverableRazorpayRequest
		{
			public Guid PaymentRequestId { get; set; }
			public Guid? TenantId { get; set; }
			public string OrderId { get; set; }
			public string PaymentMarker { get; set; }
			public decimal Amount { get; set; }
			public string Currency { get; set; }
			public Guid? CreatedUser { get; set; }
		}

		private string GetLatestPatientMobileNumber(DataRow ipdRow)
		{
			string fallbackMobile = ipdRow?.Table.Columns.Contains("mobilenumber") == true
				? ipdRow["mobilenumber"]?.ToString()
				: "";

			try
			{
				string patientProfileId = ipdRow?.Table.Columns.Contains("patientname") == true
					? ipdRow["patientname"]?.ToString()
					: "";
				if (string.IsNullOrWhiteSpace(patientProfileId))
					return fallbackMobile;

				var patientProfileDAL = new PatientProfileDAL(_connectionSettings.Value.ConnectionString);
				var patient = patientProfileDAL.getById_PatientProfile(patientProfileId);
				return string.IsNullOrWhiteSpace(patient?.mobilenumber) ? fallbackMobile : patient.mobilenumber;
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex, "Unable to refresh patient mobile number for IPD payment.");
				return fallbackMobile;
			}
		}

		[HttpGet()]
		[ActionName("Get_IPD_All_Receivables")]
		public virtual IActionResult Get_IPD_All_Receivables(string IPDApplicationFormid)
		{
			try
			{
				var dt = objIPDApplicationFormDAL.Get_All_IPD_Receivables(IPDApplicationFormid);
				var list = new List<object>();
				var roomDAL = new NalamVazha.DAL.RoomDAL(_connectionSettings.Value.ConnectionString);
				var roomNumberById = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
				foreach (DataRow r in dt.Rows)
				{
					string receivableFor = r["receivablefor"]?.ToString() ?? "";
					string remarks = r["remarks"]?.ToString() ?? "";
					if (string.Equals(receivableFor, "Cancellation Refund", StringComparison.OrdinalIgnoreCase)
						&& remarks.IndexOf("Auto:PackageChange:BookingDepositRefund", StringComparison.OrdinalIgnoreCase) >= 0)
					{
						receivableFor = "Package Change Refund";
					}

					string room = r.Table.Columns.Contains("room") ? r["room"]?.ToString() ?? "" : "";
					string roomNumber = r.Table.Columns.Contains("roomnumber") ? r["roomnumber"]?.ToString() ?? ""
						: (r.Table.Columns.Contains("room_master") ? r["room_master"]?.ToString() ?? "" : "");

					if (string.IsNullOrWhiteSpace(roomNumber) && Guid.TryParse(room, out _))
					{
						if (!roomNumberById.TryGetValue(room, out roomNumber))
						{
							try { roomNumber = roomDAL.getById_Room(room)?.roomnumber ?? ""; }
							catch { roomNumber = ""; }
							roomNumberById[room] = roomNumber;
						}
					}

					list.Add(new
					{
						receivableno   = r["receivableno"]?.ToString(),
						receivabledate = r["receivabledate"]?.ToString(),
						receivablefor  = receivableFor,
						room           = room,
						roomnumber     = roomNumber,
						amount         = r["amount"] != DBNull.Value ? Convert.ToDecimal(r["amount"]) : 0m,
						paidamount     = r["paidamount"] != DBNull.Value ? Convert.ToDecimal(r["paidamount"]) : 0m,
						balance        = r["balance"] != DBNull.Value ? Convert.ToDecimal(r["balance"]) : 0m,
						paymentstatus  = r["paymentstatus"]?.ToString(),
						remarks        = remarks
					});
				}
				return Ok(list);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Get_IPD_All_Receivables error: " + ex.Message);
				return BadRequest(ex.Message);
			}
		}

		[HttpGet()]
		[ActionName("Get_IPD_Billing_Payments_List")]
		public virtual IActionResult Get_IPD_Billing_Payments_List(string IPDApplicationFormid)
		{
			try
			{
				var objBillingDAL = new NalamVazha.DAL.BillingPaymentDAL(_connectionSettings.Value.ConnectionString);
				var dt = objBillingDAL.Get_Billing_Payments_For_IPD(IPDApplicationFormid, null);
				var list = new List<object>();
				foreach (DataRow r in dt.Rows)
				{
					list.Add(new
					{
						receivablefor        = r["receivablefor"]?.ToString(),
						amount               = r["amount"] != DBNull.Value ? Convert.ToDecimal(r["amount"]) : 0m,
						paymentmode          = r["paymentmode"]?.ToString(),
						transactionreference = r["transactionreference"]?.ToString(),
						paymentstatus        = r["paymentstatus"]?.ToString(),
						remarks              = r["remarks"]?.ToString(),
                        paymentdate          = r["paymentdate"]?.ToString(),
                        receivedamount       = r["receivedamount"]?.ToString(),
                        currency = r["currency"]?.ToString(),
                        createddate          = r["createddate"] != DBNull.Value ? Convert.ToDateTime(r["createddate"]).ToString("dd/MM/yyyy") : ""
					});
				}
				return Ok(list);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Get_IPD_Billing_Payments_List error: " + ex.Message);
				return BadRequest(ex.Message);
			}
		}

		/// <summary>Sum of (cost per day × days per row) — same rules as Initiate_Payment blocked room table.</summary>
		private static decimal ComputePayableFromBlockedRoomsJson(string json, int fallbackDays)
		{
			if (string.IsNullOrWhiteSpace(json)) return 0;
			JArray arr;
			try { arr = JArray.Parse(json); }
			catch { return 0; }
			if (arr.Count == 0) return 0;
			decimal total = 0;
			foreach (var room in arr)
			{
				var role = (room["allottedto"]?.ToString() ?? "").Trim();
				decimal patientCpd = 0, attendantCpd = 0;
				if (room["costperday"] != null && room["costperday"].Type != JTokenType.Null)
					try { patientCpd = Convert.ToDecimal(room["costperday"]); } catch { }
				if (room["attendantcostperday"] != null && room["attendantcostperday"].Type != JTokenType.Null)
					try { attendantCpd = Convert.ToDecimal(room["attendantcostperday"]); } catch { }
				var rowCost = role.Equals("Attendant", StringComparison.OrdinalIgnoreCase) ? attendantCpd : patientCpd;
				var fromDateRaw = (room["fromdate"]?.ToString() ?? "").Trim();
				var toDateRaw = (room["todate"]?.ToString() ?? "").Trim();
				int rowDays = fallbackDays;
				if (DateTime.TryParse(fromDateRaw, out var parsedFrom) && DateTime.TryParse(toDateRaw, out var parsedTo))
					rowDays = Math.Max(1, (parsedTo.Date - parsedFrom.Date).Days + 1);
				total += rowCost * rowDays;
			}
			return total;
		}

		[HttpPost()]
		[ActionName("Create_Screening_Fee_Receivable")]
		public virtual IActionResult Create_Screening_Fee_Receivable(string IPDApplicationFormid)
		{
			try
			{
				var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
				if (authHeader == null || !authHeader.StartsWith("Bearer "))
					return Unauthorized("Invalid Token");

				var token = authHeader.Substring("Bearer ".Length);
				string[] userdetails = obj_External_System_DAL.get_users_by_token(token);
				string userId = userdetails[0].ToString();

				var msg = objIPDApplicationFormDAL.Ensure_ScreeningFee_Receivable(IPDApplicationFormid, userId);
				if ((msg ?? "").Replace("\"", "") == "201.1") return Ok(msg);
				return BadRequest(msg);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Create_Screening_Fee_Receivable error: " + ex.Message);
				return BadRequest(ex.Message);
			}
		}

		[HttpPost()]
		[ActionName("Create_IPD_Appointment_Fee_Receivable")]
		public virtual IActionResult Create_IPD_Appointment_Fee_Receivable(string IPDApplicationFormid, string practitioner, string tasktype)
		{
			try
			{
				var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
				if (authHeader == null || !authHeader.StartsWith("Bearer "))
					return Unauthorized("Invalid Token");

				var token = authHeader.Substring("Bearer ".Length);
				string[] userdetails = obj_External_System_DAL.get_users_by_token(token);
				string userId = userdetails[0].ToString();

				var msg = objIPDApplicationFormDAL.Ensure_IPDAppointmentFee_Receivable(IPDApplicationFormid, practitioner, tasktype, userId);
				if ((msg ?? "").Replace("\"", "") == "201.1") return Ok(msg);
				return BadRequest(msg);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Create_IPD_Appointment_Fee_Receivable error: " + ex.Message);
				return BadRequest(ex.Message);
			}
		}


  

        [HttpGet()]
        [ActionName("validate_GroupCode")]
        public IActionResult validate_GroupCode(string tenantid, string groupcode)
        {
            try
            {
                var result = objIPDApplicationFormDAL.ValidateGroupCode(tenantid, groupcode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "validate_GroupCode error: " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet()]
        [ActionName("get_IPD_Room_Calendar_Cards")]
        public virtual DataTable get_IPD_Room_Calendar_Cards(
    string tenantid,
    string fromdate = "",
    string todate = "",
    string loginUserID = "")
        {
            var dtCalendarCards = new DataTable();

            try
            {
                /*
                 * Stored procedure tenant format:
                 *
                 * loginUserID|tenantid
                 *
                 * This allows the PostgreSQL function to use viewer tenants
                 * when tenantid is empty.
                 */
                var pvar_tenantid =
                    (loginUserID ?? "") + "|" + (tenantid ?? "");

                dtCalendarCards =
                    objIPDApplicationFormDAL
                        .get_IPD_Room_Calendar_Cards(
                            pvar_tenantid,
                            fromdate,
                            todate
                        );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An exception occurred in " +
                    "get_IPD_Room_Calendar_Cards. " +
                    "Error Message: {Message}",
                    ex.Message
                );
            }

            return dtCalendarCards;
        }
        [HttpGet()]
        [ActionName("lookup_IPDApplicationForm_packagename_by_roomtype")]
        public virtual IActionResult lookup_IPDApplicationForm_packagename_by_roomtype(
    string tenantid,
    string roomtypeid,
    string loginUserID = "")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tenantid))
                    return BadRequest("tenantid is required.");

                if (string.IsNullOrWhiteSpace(roomtypeid))
                    return BadRequest("roomtypeid is required.");

                if (!Guid.TryParse(tenantid, out Guid tenantGuid))
                    return BadRequest("Invalid tenantid.");

                if (!Guid.TryParse(roomtypeid, out Guid roomTypeGuid))
                    return BadRequest("Invalid roomtypeid.");

                DataTable data = objIPDApplicationFormDAL.lookup_IPDApplicationForm_packagename_by_roomtype(
                    tenantGuid,
                    roomTypeGuid
                );

                return Ok(JsonConvert.SerializeObject(data));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "lookup_IPDApplicationForm_packagename_by_roomtype failed");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost()]
        [ActionName("Process_IPD_Room_Calendar_Action")]
        public virtual IActionResult Process_IPD_Room_Calendar_Action(
     [FromBody] IPDCalendarActionModel model
 )
        {
            string message = "";

            try
            {
                if (
                    model == null ||
                    model.items == null ||
                    model.items.Count == 0
                )
                {
                    message =
                        "At least one IPD application is required.";
                }
                else if (
                    !HttpContext.Request.Headers
                        .ContainsKey("Authorization")
                )
                {
                    message = "Invalid Token";
                }
                else
                {
                    var authHeader =
                        HttpContext.Request.Headers[
                            "Authorization"
                        ][0];

                    if (
                        authHeader.StartsWith(
                            "Bearer "
                        )
                    )
                    {
                        var token =
                            authHeader.Substring(
                                "Bearer ".Length
                            );

                        String[] userdetails =
                            obj_External_System_DAL
                                .get_users_by_token(
                                    token
                                );

                        model.modifieduser =
                            new Guid(
                                userdetails[0]
                                    .ToString()
                            );


                        /*
                         * Capture booking status before action
                         */
                        var statusBeforeAction =
                            new Dictionary<Guid, string>();

                        foreach (var item in model.items)
                        {
                            if (!item.IPDApplicationFormid.HasValue)
                                continue;

                            Guid applicationId =
                                item.IPDApplicationFormid.Value;

                            var existing =
                                objIPDApplicationFormDAL
                                    .getById_IPDApplicationForm(
                                        applicationId.ToString()
                                    );

                            statusBeforeAction[
                                applicationId
                            ] =
                                existing?.bookingstatus ?? "";
                        }


                        /*
                         * Execute Calendar Action
                         */
                        message =
                            objIPDApplicationFormDAL
                                .Process_IPD_Room_Calendar_Action(
                                    model
                                );


                        /*
                         * Same notification logic as Allot_Room:
                         *
                         * Send only when status changes
                         * TO Provisional Booking.
                         */
                        if (
                            (message ?? "")
                                .Replace("\"", "")
                                .Trim() == "201.1"
                        )
                        {
                            foreach (var item in model.items)
                            {
                                if (!item.IPDApplicationFormid.HasValue)
                                    continue;

                                Guid applicationId =
                                    item.IPDApplicationFormid.Value;

                                var afterRecord =
                                    objIPDApplicationFormDAL
                                        .getById_IPDApplicationForm(
                                            applicationId.ToString()
                                        );

                                var statusAfterAction =
                                    afterRecord?.bookingstatus ?? "";

                                var statusBefore =
                                    statusBeforeAction
                                        .ContainsKey(applicationId)
                                        ? statusBeforeAction[applicationId]
                                        : "";


                                if (
                                    !string.Equals(
                                        statusBefore,
                                        "Provisional Booking",
                                        StringComparison.OrdinalIgnoreCase
                                    )
                                    &&
                                    string.Equals(
                                        statusAfterAction,
                                        "Provisional Booking",
                                        StringComparison.OrdinalIgnoreCase
                                    )
                                )
                                {
                                    CreateNotifier()
                                        .SendBookingStatusNotification(
                                            applicationId.ToString(),
                                            "Provisional Booking"
                                        );
                                }
                            }
                        }
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

                _logger.LogError(
                    ex,
                    "Process_IPD_Room_Calendar_Action failed"
                );
            }

            if (
                (message ?? "")
                    .Replace("\"", "")
                    .Trim() == "201.1"
            )
            {
                return Ok(message);
            }
            else if (
                (message ?? "")
                    .Replace("\"", "")
                    .Trim() == "401.1"
            )
            {
                return Unauthorized(message);
            }
            else
            {
                return BadRequest(message);
            }
        }
    }


			}
