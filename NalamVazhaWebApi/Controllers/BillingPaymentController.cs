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
	using System.Runtime.InteropServices;
	using Npgsql;
	using NpgsqlTypes;

	[Route("api/[controller]/[action]")]
	//This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:29
	public class BillingPaymentController : BaseController
	{
		public BillingPaymentController(IHttpContextAccessor httpContextAccessor, IOptions<ConnectionSettings> connectionSettings, ILoggerFactory loggerFactory, IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
		{
			_configuration = configuration;
			_logger = loggerFactory.CreateLogger<BillingPaymentController>();
			_connectionSettings = connectionSettings;
			objBillingPaymentDAL = new BillingPaymentDAL(_connectionSettings.Value.ConnectionString);
			obj_External_System_DAL = new External_System_DAL(_connectionSettings.Value.ConnectionString);
			objExternalSystemUtitlity = new ExternalSystemUtility(_connectionSettings, _configuration);
			hostingEnv = hostingEnvironment;

			var authHeader = httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();

			if (authHeader.StartsWith("Bearer "))
			{
				var token = authHeader.Substring("Bearer ".Length);
				string usersid = obj_External_System_DAL.get_users_info_by_token(token);
				string tenantid = string.IsNullOrEmpty(httpContextAccessor.HttpContext.Request.Query["tenantid"]) ? Guid.Empty.ToString() : httpContextAccessor.HttpContext.Request.Query["tenantid"].ToString();
				pvar_tenantid = usersid + "|" + tenantid;//usersid+tenantid
				pvar_usersid = usersid;

			}
		}
		private BillingPaymentDAL objBillingPaymentDAL;
		private External_System_DAL obj_External_System_DAL;
		private IOptions<ConnectionSettings> _connectionSettings;
		private ILogger _logger;
		private IConfiguration _configuration;
		private IWebHostEnvironment hostingEnv;
		private ExternalSystemUtility objExternalSystemUtitlity;
		private const string PackageChangeAdditionalBookingDepositMarker = "Auto:PackageChange:AdditionalBookingDeposit";

		private static bool IsPackageChangeAdditionalBookingDeposit(string remarks)
		{
			return !string.IsNullOrWhiteSpace(remarks)
				&& remarks.IndexOf(PackageChangeAdditionalBookingDepositMarker, StringComparison.OrdinalIgnoreCase) >= 0;
		}

		private static bool IsPackageChangeAdditionalBookingDeposit(DataRow row)
		{
			if (row == null || !row.Table.Columns.Contains("remarks")) return false;
			return IsPackageChangeAdditionalBookingDeposit(Convert.ToString(row["remarks"] ?? string.Empty));
		}

		private static bool ShouldAdvanceBookingDepositStatus(string receivableFor, string paymentStatus, string remarks)
		{
			return string.Equals(receivableFor, "IPD Booking Deposit", StringComparison.OrdinalIgnoreCase)
				&& string.Equals(paymentStatus, "Success", StringComparison.OrdinalIgnoreCase)
				&& !IsPackageChangeAdditionalBookingDeposit(remarks);
		}

		private static bool HasBookingDepositReceivable(DataTable pendingReceivables, bool packageChange)
		{
			if (pendingReceivables == null) return false;
			foreach (DataRow row in pendingReceivables.Rows)
			{
				var receivableFor = Convert.ToString(row["receivablefor"] ?? string.Empty);
				if (!string.Equals(receivableFor, "IPD Booking Deposit", StringComparison.OrdinalIgnoreCase)) continue;
				if (IsPackageChangeAdditionalBookingDeposit(row) == packageChange) return true;
			}
			return false;
		}

		private static bool IsPackageChangeBookingDepositPayment(BillingPaymentModel model, DataTable pendingReceivables)
		{
			if (!ShouldAdvanceBookingDepositStatus(model.receivablefor, model.paymentstatus, model.remarks)) return false;
			return HasBookingDepositReceivable(pendingReceivables, packageChange: true)
				&& !HasBookingDepositReceivable(pendingReceivables, packageChange: false);
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

		private static bool IsRazorpayPaymentMode(string paymentMode)
		{
			return !string.IsNullOrWhiteSpace(paymentMode)
				&& paymentMode.IndexOf("razor", StringComparison.OrdinalIgnoreCase) >= 0;
		}

		private static bool IsSuccessfulCollection(BillingPaymentModel model)
		{
			if (model == null || model.amount <= 0) return false;
			return string.Equals(model.paymentstatus, "Success", StringComparison.OrdinalIgnoreCase)
				|| string.Equals(model.paymentstatus, "Paid", StringComparison.OrdinalIgnoreCase)
				|| string.Equals(model.paymentstatus, "Completed", StringComparison.OrdinalIgnoreCase);
		}

		private static bool IsSuccessfulCollection(UnifiedPaymentPostModel model)
		{
			if (model == null || (model.receivedamount ?? 0) <= 0) return false;
			var status = string.IsNullOrWhiteSpace(model.paymentstatus) ? "Success" : model.paymentstatus;
			return string.Equals(status, "Success", StringComparison.OrdinalIgnoreCase)
				|| string.Equals(status, "Paid", StringComparison.OrdinalIgnoreCase)
				|| string.Equals(status, "Completed", StringComparison.OrdinalIgnoreCase);
		}

		private static bool IsRefundOperation(BillingPaymentModel model)
		{
			if (model == null) return false;
			return (model.refundedamount ?? 0) > 0
				|| !string.IsNullOrWhiteSpace(model.refundmode)
				|| !string.IsNullOrWhiteSpace(model.refundstatus)
				|| (model.receivablefor ?? string.Empty)
					.IndexOf("refund", StringComparison.OrdinalIgnoreCase) >= 0;
		}

		private static bool IsRefundOperation(UnifiedPaymentPostModel model)
		{
			return model != null
				&& (model.receivablefor ?? string.Empty)
					.IndexOf("refund", StringComparison.OrdinalIgnoreCase) >= 0;
		}

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

		private bool IsCancelledOPD(Guid opdFormId)
		{
			if (opdFormId == Guid.Empty) return false;
			using (var connection = new NpgsqlConnection(_connectionSettings.Value.ConnectionString))
			using (var command = new NpgsqlCommand(@"
				SELECT EXISTS (
					SELECT 1
					FROM OPDForm o
					WHERE o.opdformid = @pvar_opdformid
					  AND COALESCE(o.isdeleted, false) = false
					  AND POSITION('cancel' IN LOWER(COALESCE(o.verifiedstatus, ''))) > 0
				)", connection))
			{
				command.Parameters.AddWithValue("pvar_opdformid", NpgsqlDbType.Uuid, opdFormId);
				connection.Open();
				return Convert.ToBoolean(command.ExecuteScalar());
			}
		}

		private bool ContainsCancelledOPDReceivable(
			UnifiedPaymentPostModel model,
			DataTable pendingReceivables)
		{
			if (model == null
				|| !string.Equals(model.Type ?? "OPD", "OPD", StringComparison.OrdinalIgnoreCase)
				|| !IsSuccessfulCollection(model)
				|| IsRefundOperation(model))
				return false;

			if (model.opdnumber.HasValue && IsCancelledOPD(model.opdnumber.Value))
				return true;
			if (model.IPDNo.HasValue && IsCancelledOPD(model.IPDNo.Value))
				return true;

			var selectedIds = new HashSet<Guid>((model.Payments ?? new List<UnifiedPaymentInputModel>())
				.Where(x => x != null && x.PayNow > 0 && x.Receivableid != Guid.Empty)
				.Select(x => x.Receivableid));
			if (selectedIds.Count == 0 && pendingReceivables != null)
			{
				foreach (DataRow row in pendingReceivables.Rows)
				{
					if (ReadDecimalColumn(row, "balance") <= 0) continue;
					var receivableId = ReadGuidColumn(row, "receivableid");
					if (receivableId != Guid.Empty) selectedIds.Add(receivableId);
				}
			}
			if (selectedIds.Count == 0) return false;

			using (var connection = new NpgsqlConnection(_connectionSettings.Value.ConnectionString))
			using (var command = new NpgsqlCommand(@"
				SELECT EXISTS (
					SELECT 1
					FROM Receivable r
					INNER JOIN OPDForm o ON o.opdformid = r.opdnumber
					WHERE r.receivableid = ANY(@pvar_receivableids)
					  AND COALESCE(r.isdeleted, false) = false
					  AND COALESCE(o.isdeleted, false) = false
					  AND POSITION('cancel' IN LOWER(COALESCE(o.verifiedstatus, ''))) > 0
				)", connection))
			{
				command.Parameters.AddWithValue(
					"pvar_receivableids",
					NpgsqlDbType.Array | NpgsqlDbType.Uuid,
					selectedIds.ToArray());
				connection.Open();
				return Convert.ToBoolean(command.ExecuteScalar());
			}
		}

		private static bool IsBookingDepositCollectionAttempt(UnifiedPaymentPostModel model, DataTable pendingReceivables)
		{
			if (model == null || pendingReceivables == null || (model.receivedamount ?? 0) <= 0) return false;

			var selectedReceivableIds = new HashSet<Guid>(
				(model.Payments ?? new List<UnifiedPaymentInputModel>())
					.Where(x => x != null && x.PayNow > 0 && x.Receivableid != Guid.Empty)
					.Select(x => x.Receivableid));

			foreach (DataRow row in pendingReceivables.Rows)
			{
				var receivableFor = Convert.ToString(ReadColumn(row, "receivablefor") ?? string.Empty);
				if (!string.Equals(receivableFor, "IPD Booking Deposit", StringComparison.OrdinalIgnoreCase)) continue;

				var receivableId = ReadGuidColumn(row, "Receivableid");
				if (selectedReceivableIds.Count > 0)
					return selectedReceivableIds.Contains(receivableId);

				if (ReadDecimalColumn(row, "balance") > 0)
					return true;
			}

			return false;
		}

		private bool HasActiveRazorpayRequest(string markerPrefix, string paymentType = null)
		{
			using (var conn = new NpgsqlConnection(_connectionSettings.Value.ConnectionString))
			{
				conn.Open();
				using (var cmd = new NpgsqlCommand("SELECT \"BillingPayment_Has_Active_Razorpay_Request\"(@pvar_paymentmarker,@pvar_paymenttype,@pvar_activeafter)", conn))
				{
					cmd.Parameters.AddWithValue("pvar_paymentmarker", NpgsqlDbType.Varchar, markerPrefix + "%");
					cmd.Parameters.AddWithValue("pvar_paymenttype", NpgsqlDbType.Varchar, (object)paymentType ?? DBNull.Value);
					cmd.Parameters.AddWithValue("pvar_activeafter", NpgsqlDbType.Timestamp, GetApplicationNow().AddMinutes(-15));
					return Convert.ToBoolean(cmd.ExecuteScalar());
				}
			}
		}

		private string GetActiveRazorpayConflictMessage(BillingPaymentModel model)
		{
			if (!IsSuccessfulCollection(model) || IsRazorpayPaymentMode(model.paymentmode)) return string.Empty;
			if (model.opdnumber.HasValue && HasActiveRazorpayRequest($"OPD:{model.opdnumber.Value}:"))
			{
				return "A Razorpay payment is already in progress for this OPD. Please ask the health seeker to close/cancel the payment popup or wait for it to expire before receiving payment at the front desk.";
			}
			if (model.ipdnumber.HasValue && HasActiveRazorpayRequest($"IPD:{model.ipdnumber.Value}:"))
			{
				return "A Razorpay payment is already in progress for this IPD. Please ask the health seeker to close/cancel the payment popup or wait for it to expire before receiving payment at the front desk.";
			}
			return string.Empty;
		}

		private string GetActiveRazorpayConflictMessage(UnifiedPaymentPostModel model, DataTable pendingReceivables)
		{
			if (!IsSuccessfulCollection(model) || IsRazorpayPaymentMode(model.paymentmode)) return string.Empty;
			if (model.IPDNo.HasValue
				&& string.Equals(model.Type ?? "", "IPD", StringComparison.OrdinalIgnoreCase)
				&& IsBookingDepositCollectionAttempt(model, pendingReceivables)
				&& HasActiveRazorpayRequest($"IPD:{model.IPDNo.Value}:", "Booking Deposit"))
			{
				return "A Razorpay booking-deposit payment is already in progress for this IPD. Please ask the health seeker to close/cancel the payment popup or wait for it to expire before receiving payment at the front desk.";
			}
			return string.Empty;
		}

		private string pvar_tenantid = "|";
		private string pvar_usersid = "";


		[HttpPost()]
		[ActionName("Add_Billing_Payment")]
		public virtual IActionResult Add_Billing_Payment([FromBody] BillingPaymentModel model)
		{
			string message = "";

			access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
			obj_access_logsdetailsModel.action_method_name = "Add_Billing_Payment";
			try
			{
				// Missing non-nullable DateTime values bind as DateTime.MinValue. Npgsql
				// represents that value as PostgreSQL -infinity, so default it at the
				// authoritative API boundary before validation and persistence.
				if (model != null && (model.paymentdate == default || model.paymentdate.Year < 1900))
					model.paymentdate = GetApplicationNow().Date;

				if (ModelState.IsValid)
				{

					BillingPaymentModelValidator validator = new BillingPaymentModelValidator();
					ValidationResult results = validator.Validate(model);
					if (!results.IsValid)
					{
						var errorCollection = string.Join(" | ", results.Errors.Select(e => e.ErrorMessage.Replace("{propertyName}", e.PropertyName)));
						message = ("Validation Error : " + errorCollection);


					}
					else
					{

						var authHeader = HttpContext.Request.Headers["Authorization"][0];
						if (authHeader.StartsWith("Bearer "))
						{


							var token = authHeader.Substring("Bearer ".Length);
							String[] userdetails = obj_External_System_DAL.get_users_by_token(token);
							model.createduser = new Guid(userdetails[0].ToString());
							obj_access_logsdetailsModel.access_logsid = new Guid(userdetails[1].ToString());

							message = IsSuccessfulCollection(model)
								&& !IsRefundOperation(model)
								&& model.opdnumber.HasValue
								&& IsCancelledOPD(model.opdnumber.Value)
								? "This OPD booking is cancelled. A new payment cannot be collected."
								: GetActiveRazorpayConflictMessage(model);
							if (!string.IsNullOrWhiteSpace(message))
							{
								_logger.LogWarning(
									"Blocked manual BillingPayment. OPD: {opdId}, IPD: {ipdId}, PaymentMode: {paymentMode}, Reason: {reason}",
									model.opdnumber, model.ipdnumber, model.paymentmode, message);
							}
							else
							{

							DataTable pendingBeforePayment = null;
							if (model.patientname.HasValue && model.ipdnumber.HasValue)
							{
								pendingBeforePayment = objBillingPaymentDAL.Get_Unified_Pending_Receivables(model.patientname.Value, model.ipdnumber, "IPD");
							}

							message = objBillingPaymentDAL.Add_Billing_Payment(model);

							if (message == "201.1")
							{
								if (model.opdnumber.HasValue
									&& model.BillingPaymentid.HasValue
									&& string.Equals(model.paymentstatus, "Success", StringComparison.OrdinalIgnoreCase))
								{
									try
									{
										objBillingPaymentDAL.UpdateOPDConsultationFeeReceivable(
											model.opdnumber.Value,
											model.amount,
											model.BillingPaymentid.Value);
									}
									catch (Exception exRx)
									{
										_logger.LogError(exRx, "OPD receivable update failed for OPD {opdId}: {msg}",
											model.opdnumber, exRx.Message);
									}
								}

								if (model.receivablefor == "Pre-Admission Consultation" && model.paymentstatus == "Success")
								{
									if (model.ipdnumber.HasValue && model.patientname.HasValue && model.createduser.HasValue)
									{
										var receivableDal = new ReceivableDAL(_connectionSettings.Value.ConnectionString);
										var receivableModel = new ReceivableModel
										{
											Receivableid = Guid.NewGuid(),
											tenantid = model.tenantid,
											receivabledate = DateTime.Now.Date,
											patientname = model.patientname.Value,
											ipdnumber = model.ipdnumber.Value,
											receivablefor = "Pre-Admission Consultation",
											amount = model.amount,
											remarks = "Auto:PreAdmissionConsultation",
											createduser = model.createduser.Value,
											craftmyapp_actionmethodname = "Add_Receivable",
											paidamount=model.amount,
											billingpaymentid=model.BillingPaymentid,
											paymentstatus="Paid"
										
										};
										var receivableInsertMessage = receivableDal.Add_Receivable(receivableModel);
										if (!string.Equals(receivableInsertMessage, "201.1", StringComparison.OrdinalIgnoreCase))
										{
											_logger.LogError("Pre-Admission receivable insert failed for IPD {ipdId}: {message}",
												model.ipdnumber, receivableInsertMessage);
										}
									}
									else
									{
										_logger.LogError("Pre-Admission receivable insert skipped due to missing identifiers. IPD: {ipdId}, Patient: {patientId}, User: {userId}",
											model.ipdnumber, model.patientname, model.createduser);
									}
								}
								if (ShouldAdvanceBookingDepositStatus(model.receivablefor, model.paymentstatus, model.remarks)
									&& !IsPackageChangeBookingDepositPayment(model, pendingBeforePayment))
								{
									var objIPDApplicationFormDAL = new IPDApplicationFormDAL(_connectionSettings.Value.ConnectionString);
									var currentIpd = objIPDApplicationFormDAL.getById_IPDApplicationForm(model.ipdnumber.ToString());
									if (!string.Equals(currentIpd?.bookingstatus ?? string.Empty, "Admitted", StringComparison.OrdinalIgnoreCase))
										objIPDApplicationFormDAL.Update_IPD_BookingStatus(model.ipdnumber.ToString(), "Provisional Confirmed", pvar_usersid);
								}
								if (model.receivablefor == "IPD Admission Fees" && model.paymentstatus == "Success")
								{
									var objIPDApplicationFormDAL = new IPDApplicationFormDAL(_connectionSettings.Value.ConnectionString);
									//objIPDApplicationFormDAL.Update_IPD_BookingStatus(model.ipdnumber.ToString(), "Pre-Admission Initiated", pvar_usersid);
									var currentIpd = objIPDApplicationFormDAL.getById_IPDApplicationForm(model.ipdnumber.ToString());
									if (!string.Equals(currentIpd?.bookingstatus ?? string.Empty, "Admitted", StringComparison.OrdinalIgnoreCase))
										objIPDApplicationFormDAL.Update_IPD_BookingStatus(model.ipdnumber.ToString(), "Consultation Scheduled", pvar_usersid);
								}
								if (model.receivablefor == "Pre-Admission Consultation" && model.paymentstatus == "Success")
								{
									var objIPDApplicationFormDAL = new IPDApplicationFormDAL(_connectionSettings.Value.ConnectionString);
									//objIPDApplicationFormDAL.Update_IPD_BookingStatus(model.ipdnumber.ToString(), "Awaiting Consultation", pvar_usersid);
									var currentIpd = objIPDApplicationFormDAL.getById_IPDApplicationForm(model.ipdnumber.ToString());
									if (!string.Equals(currentIpd?.bookingstatus ?? string.Empty, "Admitted", StringComparison.OrdinalIgnoreCase))
										objIPDApplicationFormDAL.Update_IPD_BookingStatus(model.ipdnumber.ToString(), "Consultation Scheduled", pvar_usersid);
								}
								if (model.receivablefor == "IPD Booking Advance" && model.paymentstatus == "Success")
								{
									var objIPDApplicationFormDAL = new IPDApplicationFormDAL(_connectionSettings.Value.ConnectionString);
									var currentIpd = objIPDApplicationFormDAL.getById_IPDApplicationForm(model.ipdnumber.ToString());
									if (!string.Equals(currentIpd?.bookingstatus ?? string.Empty, "Admitted", StringComparison.OrdinalIgnoreCase))
										objIPDApplicationFormDAL.Update_IPD_BookingStatus(model.ipdnumber.ToString(), "Admitted", pvar_usersid);
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
				else
				{
					var errorCollection = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
					message = errorCollection.ToString();

					_logger.LogError("BillingPaymentModel - Add_Billing_Payment , Validation Error :" + message);
					message = ("Validation Error : " + message);
				}






			}
			catch (Exception ex)
			{
				message = ex.Message;
				_logger.LogError(ex, "An exception occurred in - Add_Billing_Payment, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

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
		[ActionName("getById_BillingPayment")]
		public virtual BillingPaymentModel getById_BillingPayment(string BillingPaymentid, string loginUserID = "")
		{
			BillingPaymentModel objBillingPayment = new BillingPaymentModel();
			try
			{
				objBillingPayment = objBillingPaymentDAL.getById_BillingPayment(BillingPaymentid);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - getById_BillingPayment, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

			}
			return objBillingPayment;

		}
		[HttpGet()]
		[ActionName("lookup_BillingPayment_refundedby")]
		public virtual System.Data.DataTable lookup_BillingPayment_refundedby(String tenantid, string loginUserID = "")
		{
			DataTable dtBillingPayment = new DataTable();
			try
			{
				tenantid = pvar_tenantid;

				dtBillingPayment = objBillingPaymentDAL.lookup_BillingPayment_refundedby(tenantid);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - lookup_BillingPayment_refundedby, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

			}
			return dtBillingPayment;
		}

		[HttpPost()]
		[ActionName("Update_Billing_Payment")]
		public virtual IActionResult Update_Billing_Payment([FromBody] BillingPaymentModel model)
		{
			string message = "";
			access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
			obj_access_logsdetailsModel.action_method_name = "Update_Billing_Payment";

			try
			{

				if (ModelState.IsValid)
				{

					BillingPaymentModelValidator validator = new BillingPaymentModelValidator();
					ValidationResult results = validator.Validate(model);
					if (!results.IsValid)
					{
						var errorCollection = string.Join(" | ", results.Errors.Select(e => e.ErrorMessage.Replace("{propertyName}", e.PropertyName)));
						message = errorCollection.ToString();
						//return BadRequest("Validation Error : " + message);

					}
					else
					{
						var authHeader = HttpContext.Request.Headers["Authorization"][0];
						if (authHeader.StartsWith("Bearer "))
						{



							var token = authHeader.Substring("Bearer ".Length);
							String[] userdetails = obj_External_System_DAL.get_users_by_token(token);
							model.modifieduser = new Guid(userdetails[0].ToString());
							obj_access_logsdetailsModel.access_logsid = new Guid(userdetails[1].ToString());




							DataTable pendingBeforePayment = null;
							if (model.patientname.HasValue && model.ipdnumber.HasValue)
							{
								pendingBeforePayment = objBillingPaymentDAL.Get_Unified_Pending_Receivables(model.patientname.Value, model.ipdnumber, "IPD");
							}

							message = objBillingPaymentDAL.Update_Billing_Payment(model);


							if (ShouldAdvanceBookingDepositStatus(model.receivablefor, model.paymentstatus, model.remarks)
								&& !IsPackageChangeBookingDepositPayment(model, pendingBeforePayment))
							{
								var objIPDApplicationFormDAL = new IPDApplicationFormDAL(_connectionSettings.Value.ConnectionString);
								objIPDApplicationFormDAL.Update_IPD_BookingStatus(model.ipdnumber.ToString(), "Provisional Confirmed", pvar_usersid);
							}
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

					_logger.LogError("BillingPaymentModel - Update_Billing_Payment, Validation Error :" + message);

					//return BadRequest("Validation Error : " + message);
				}






			}
			catch (Exception ex)
			{

				message = ex.Message;
				_logger.LogError(ex, "An exception occurred in - Update_Billing_Payment, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

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
		public virtual async Task<string> Remove_Billing_Payment(string BillingPaymentid, string loginUserID = "")
		{
			string message = "";
			access_logsdetailsModel obj_access_logsdetailsModel = new access_logsdetailsModel();
			obj_access_logsdetailsModel.action_method_name = "Remove_Billing_Payment";

			try
			{

				var authHeader = HttpContext.Request.Headers["Authorization"][0];
				if (authHeader.StartsWith("Bearer "))
				{



					var token = authHeader.Substring("Bearer ".Length);

					String[] userdetails = obj_External_System_DAL.get_users_by_token(token);
					loginUserID = userdetails[0].ToString();
					obj_access_logsdetailsModel.access_logsid = new Guid(userdetails[1].ToString());


					message = objBillingPaymentDAL.Remove_Billing_Payment(BillingPaymentid, loginUserID);
				}
				else
				{
					message = "Invalid Token";

				}


			}
			catch (Exception ex)
			{
				message = ex.Message;
				_logger.LogError(ex, "An exception occurred in - Remove_Billing_Payment, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

			}
			if (obj_access_logsdetailsModel.access_logsid != null)
			{
				obj_access_logsdetailsModel.api_response = message.Replace("\"", "");
				obj_External_System_DAL.create_access_logs_details(obj_access_logsdetailsModel);
			}

			return message;

		}

		[HttpGet()]

		[ActionName("Billing_Payment_List")]
		public virtual JObject Billing_Payment_List(string tenantid
, string paymentdate_automatonfrom = ""
, string paymentdate_automatonto = ""
, string patientname = ""
, string receivablefor = ""
, string loginUserID = "", int? pagesize = 100, int? pagenumber = 0, string searchterm = "", string sort_fields = "")
		{

			JObject jsonBillingPayment = null;
			try
			{
				tenantid = pvar_tenantid;
				jsonBillingPayment = objBillingPaymentDAL.Billing_Payment_List(tenantid
, paymentdate_automatonfrom
, paymentdate_automatonto
, patientname
, receivablefor
, pagesize, pagenumber, searchterm, sort_fields);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - Billing_Payment_List, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

			}
			return jsonBillingPayment;

		}


		[HttpGet()]

		[ActionName("get_all_BillingPayment")]
		public virtual System.Data.DataTable get_all_BillingPayment(string tenantid, string searchterm = "", string pagesize = "50", string pagenumber = "1", string loginUserID = "")
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
			DataTable dtBillingPayment = new DataTable();
			try
			{
				tenantid = pvar_tenantid;
				dtBillingPayment = objBillingPaymentDAL.get_all_BillingPayment(tenantid, searchterm, pageSize, pageNumber);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - get_all_BillingPayment, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));


			}
			return dtBillingPayment;

		}
		[HttpGet()]
		[ActionName("getById_allinfo_BillingPayment")]
		public virtual System.Data.DataTable getById_allinfo_BillingPayment(string BillingPaymentid)
		{
			DataTable dtBillingPayment = new DataTable();
			try
			{
				dtBillingPayment = objBillingPaymentDAL.getById_allinfo_BillingPayment(BillingPaymentid);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - getById_allinfo_BillingPayment, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

			}
			return dtBillingPayment;

		}
		[HttpGet()]
		[ActionName("lookup_BillingPayment_patientvisit")]
		public virtual System.Data.DataTable lookup_BillingPayment_patientvisit(String tenantid, String patientname, string searchterm = "", string pagesize = "50", string pagenumber = "1", string loginUserID = "")
		{
			DataTable dtBillingPayment = new DataTable();
			try
			{
				tenantid = pvar_tenantid;
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
				dtBillingPayment = objBillingPaymentDAL.lookup_BillingPayment_patientvisit(tenantid, patientname, searchterm, pageSize, pageNumber);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - lookup_BillingPayment_patientvisit, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

			}
			return dtBillingPayment;
		}

		[HttpGet()]
		[ActionName("lookup_BillingPayment_patientname")]
		public virtual System.Data.DataTable lookup_BillingPayment_patientname(String tenantid, string searchterm = "", string pagesize = "50", string pagenumber = "1", string loginUserID = "")
		{
			DataTable dtBillingPayment = new DataTable();
			try
			{
				tenantid = pvar_tenantid;
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
				dtBillingPayment = objBillingPaymentDAL.lookup_BillingPayment_patientname(tenantid, searchterm, pageSize, pageNumber);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - lookup_BillingPayment_patientname, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

			}
			return dtBillingPayment;
		}

		[HttpGet()]
		[ActionName("lookup_BillingPayment_collectedby")]
		public virtual System.Data.DataTable lookup_BillingPayment_collectedby(String tenantid, string loginUserID = "")
		{
			DataTable dtBillingPayment = new DataTable();
			try
			{
				tenantid = pvar_tenantid;

				dtBillingPayment = objBillingPaymentDAL.lookup_BillingPayment_collectedby(tenantid);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - lookup_BillingPayment_collectedby, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

			}
			return dtBillingPayment;
		}

		[HttpGet()]
		[ActionName("lookup_change_BillingPayment_patientvisit")]
		public virtual System.Data.DataTable lookup_change_BillingPayment_patientvisit(string PatientVisitid, string loginUserID = "")
		{
			DataTable dtPatientVisit = new DataTable();
			try
			{
				dtPatientVisit = objBillingPaymentDAL.lookup_change_BillingPayment_patientvisit(PatientVisitid);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - lookup_change_BillingPayment_patientvisit, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

			}
			return dtPatientVisit;
		}
		[HttpGet()]
		[ActionName("Get_OPD_Billing_Summary")]
		public virtual System.Data.DataTable Get_OPD_Billing_Summary(string PatientVisitid, string loginUserID = "")
		{
			DataTable dtSummary = new DataTable();
			try
			{
				dtSummary = objBillingPaymentDAL.Get_OPD_Billing_Summary(PatientVisitid);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - Get_OPD_Billing_Summary, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));
			}
			return dtSummary;
		}

		[HttpGet()]
		[ActionName("Get_OPD_Billing_Summary_By_OPDForm")]
		public virtual System.Data.DataTable Get_OPD_Billing_Summary_By_OPDForm(string OPDFormid, string loginUserID = "")
		{
			DataTable dtSummary = new DataTable();
			try
			{
				dtSummary = objBillingPaymentDAL.Get_OPD_Billing_Summary_By_OPDForm(OPDFormid);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - Get_OPD_Billing_Summary_By_OPDForm, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));
			}
			return dtSummary;
		}

		[HttpGet()]
		[ActionName("Get_Billing_Payments_For_OPD")]
		public virtual IActionResult Get_Billing_Payments_For_OPD(string OPDFormid, string loginUserID = "")
		{
			try
			{
				var dataTable = objBillingPaymentDAL.Get_Billing_Payments_For_OPD(OPDFormid);
				if (dataTable == null || dataTable.Rows.Count == 0) return Ok(new List<object>());
				var result = new List<object>();
				foreach (System.Data.DataRow row in dataTable.Rows)
				{
					result.Add(new
					{
						billingpaymentid   = row["billingpaymentid"]?.ToString(),
						receivablefor      = row["receivablefor"]?.ToString(),
						amount             = row["amount"]?.ToString(),
						paymentmode        = row["paymentmode"]?.ToString(),
						transactionreference = row["transactionreference"]?.ToString(),
						paymentstatus      = row["paymentstatus"]?.ToString(),
						remarks            = row["remarks"]?.ToString(),
						createddate        = row["createddate"]?.ToString()
					});
				}
				return Ok(result);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Get_Billing_Payments_For_OPD error: " + ex.Message);
				return Ok(new List<object>());
			}
		}

		[HttpGet()]
		[ActionName("lookup_BillingPayment_ipdnumber")]
		public virtual System.Data.DataTable lookup_BillingPayment_ipdnumber(String tenantid, String patientname, string searchterm = "", string pagesize = "50", string pagenumber = "1", string loginUserID = "")
		{
			DataTable dtBillingPayment = new DataTable();
			try
			{
				tenantid = pvar_tenantid;
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
				dtBillingPayment = objBillingPaymentDAL.lookup_BillingPayment_ipdnumber(tenantid, patientname, searchterm, pageSize, pageNumber);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - lookup_BillingPayment_ipdnumber, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

			}
			return dtBillingPayment;
		}
		[HttpGet()]
		[ActionName("lookup_BillingPayment_opdnumber")]
		public virtual System.Data.DataTable lookup_BillingPayment_opdnumber(String tenantid, String patientname, string searchterm = "", string pagesize = "50", string pagenumber = "1", string loginUserID = "")
		{
			DataTable dtBillingPayment = new DataTable();
			try
			{
				tenantid = pvar_tenantid;
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
				dtBillingPayment = objBillingPaymentDAL.lookup_BillingPayment_opdnumber(tenantid, patientname, searchterm, pageSize, pageNumber);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - lookup_BillingPayment_opdnumber, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace.ToString()}" : ex.Message));

			}
			return dtBillingPayment;
		}

		[HttpGet()]
		[ActionName("Get_Unified_Pending_Receivables")]
		public virtual System.Data.DataTable Get_Unified_Pending_Receivables(Guid PatientID, Guid? IPDNo, string Type = "OPD")
		{
			var dt = new DataTable();
			try
			{
				dt = objBillingPaymentDAL.Get_Unified_Pending_Receivables(PatientID, IPDNo, Type);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - Get_Unified_Pending_Receivables, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace}" : ex.Message));
			}
			return dt;
		}

		[HttpGet()]
		[ActionName("Get_HealthSeeker_Payments")]
		public virtual System.Data.DataTable Get_HealthSeeker_Payments(
			string tenantid,
			string patientid,
			string paymentdate_automatonfrom = "",
			string paymentdate_automatonto = "",
			int? pagesize = 200,
			int? pagenumber = 0,
			string loginUserID = "")
		{
			var dt = new DataTable();
			try
			{
				if (!Guid.TryParse(patientid, out var patientGuid))
					return dt;
				Guid? tenantGuid = Guid.TryParse(tenantid, out var parsedTenant) ? parsedTenant : (Guid?)null;
				dt = objBillingPaymentDAL.Get_HealthSeeker_Payments(
					tenantGuid,
					patientGuid,
					paymentdate_automatonfrom,
					paymentdate_automatonto,
					pagesize,
					pagenumber);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred in - Get_HealthSeeker_Payments, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace}" : ex.Message));
			}
			return dt;
		}
        [HttpGet]
        [ActionName("Get_Unified_Room_Cost_Summary")]
        public virtual IActionResult Get_Unified_Room_Cost_Summary(Guid IPDNo)
        {
            try
            {
                var authHeader = HttpContext.Request.Headers["Authorization"][0];

                if (!authHeader.StartsWith("Bearer "))
                    return BadRequest("Invalid Token");

                var token = authHeader.Substring("Bearer ".Length);
                var userdetails = obj_External_System_DAL.get_users_by_token(token);

                var rows = objBillingPaymentDAL.GetUnifiedRoomCostSummary(IPDNo);

                return Ok(rows);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get_Unified_Room_Cost_Summary failed: " + ex.Message);
                return BadRequest(ex.Message);
            }
        }
        [HttpPost()]
		[ActionName("Apply_Unified_Receivable_Payments")]
		public virtual IActionResult Apply_Unified_Receivable_Payments([FromBody] UnifiedPaymentPostModel model)
		{
			string message;
			try
			{
				var authHeader = HttpContext.Request.Headers["Authorization"][0];
				if (!authHeader.StartsWith("Bearer "))
				{
					return BadRequest("Invalid Token");
				}

				var token = authHeader.Substring("Bearer ".Length);
				var userdetails = obj_External_System_DAL.get_users_by_token(token);
				var createdUser = new Guid(userdetails[0].ToString());
				var loginUserId = userdetails[0].ToString();
				var pendingBeforePayment = objBillingPaymentDAL.Get_Unified_Pending_Receivables(
					model.PatientID,
					model.IPDNo,
					model.Type ?? "OPD");

				if (ContainsCancelledOPDReceivable(model, pendingBeforePayment))
				{
					_logger.LogWarning(
						"Blocked payment collection for cancelled OPD. Patient: {patientId}, OPD: {opdId}",
						model.PatientID,
						model.opdnumber ?? model.IPDNo);
					return BadRequest("This OPD booking is cancelled. A new payment cannot be collected.");
				}

				message = GetActiveRazorpayConflictMessage(model, pendingBeforePayment);
				if (!string.IsNullOrWhiteSpace(message))
				{
					_logger.LogWarning("Blocked unified payment collection because Razorpay booking deposit is active. IPD: {ipdId}, PaymentMode: {paymentMode}",
						model?.IPDNo, model?.paymentmode);
					return BadRequest(message);
				}

				message = objBillingPaymentDAL.Add_Unified_Payment_Collection(model, createdUser);

				if ((message ?? string.Empty).Replace("\"", "").Contains("201.1")
					&& model != null
					&& model.IPDNo.HasValue
					&& string.Equals(model.Type ?? "", "IPD", StringComparison.OrdinalIgnoreCase)
					&& string.Equals(model.paymentstatus ?? "Success", "Success", StringComparison.OrdinalIgnoreCase))
				{
					decimal remainingToAllocate = model.receivedamount ?? 0;
					var coveredReceivableFor = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
					foreach (DataRow row in pendingBeforePayment.Rows)
					{
						var receivableFor = Convert.ToString(row["receivablefor"] ?? string.Empty);
						var balance = 0m;
						decimal.TryParse(Convert.ToString(row["balance"] ?? "0"), out balance);
						if (balance <= 0) continue;

						if (remainingToAllocate + 0.009m >= balance)
						{
							if (!string.Equals(receivableFor, "IPD Booking Deposit", StringComparison.OrdinalIgnoreCase)
								|| !IsPackageChangeAdditionalBookingDeposit(row))
							{
								coveredReceivableFor.Add(receivableFor);
							}
							remainingToAllocate -= balance;
						}
						else
						{
							break;
						}
					}

					// Fallback for non-deposit workflows only. A booking deposit must never
					// advance the booking until its receivable has been fully covered by the
					// cumulative payment allocation above.
					if (coveredReceivableFor.Count == 0
						&& !string.IsNullOrWhiteSpace(model.receivablefor)
						&& !string.Equals(model.receivablefor, "IPD Booking Deposit", StringComparison.OrdinalIgnoreCase))
					{
						coveredReceivableFor.Add(model.receivablefor.Trim());
					}

					var objIPDApplicationFormDAL = new IPDApplicationFormDAL(_connectionSettings.Value.ConnectionString);
					var currentIpd = objIPDApplicationFormDAL.getById_IPDApplicationForm(model.IPDNo.ToString());
					var currentBookingStatus = currentIpd?.bookingstatus ?? string.Empty;
					if (!string.Equals(currentBookingStatus, "Admitted", StringComparison.OrdinalIgnoreCase))
					{
						if (coveredReceivableFor.Contains("IPD Booking Deposit"))
						{
							objIPDApplicationFormDAL.Update_IPD_BookingStatus(model.IPDNo.ToString(), "Provisional Confirmed", loginUserId);
						}
						if (coveredReceivableFor.Contains("IPD Screening Fee"))
						{
							//objIPDApplicationFormDAL.Update_IPD_BookingStatus(model.IPDNo.ToString(), "Screening Fee Paid", loginUserId);
						}
						if (coveredReceivableFor.Contains("IPD Admission Fees"))
						{
							//objIPDApplicationFormDAL.Update_IPD_BookingStatus(model.IPDNo.ToString(), "Pre-Admission Initiated", loginUserId);
							objIPDApplicationFormDAL.Update_IPD_BookingStatus(model.IPDNo.ToString(), "Consultation Scheduled", loginUserId);
						}
						if (coveredReceivableFor.Contains("Pre-Admission Consultation"))
						{
							//objIPDApplicationFormDAL.Update_IPD_BookingStatus(model.IPDNo.ToString(), "Awaiting Consultation", loginUserId);
							objIPDApplicationFormDAL.Update_IPD_BookingStatus(model.IPDNo.ToString(), "Consultation Scheduled", loginUserId);
						}
						if (coveredReceivableFor.Contains("IPD Booking Advance"))
						{
							objIPDApplicationFormDAL.Update_IPD_BookingStatus(model.IPDNo.ToString(), "Admitted", loginUserId);
						}
					}
				}
			}
			catch (Exception ex)
			{
				message = ex.Message;
				_logger.LogError(ex, "An exception occurred in - Apply_Unified_Receivable_Payments, Error Message : " + (ex.StackTrace != null ? $", Stack Trace: {ex.StackTrace}" : ex.Message));
			}

			if ((message ?? string.Empty).Replace("\"", "").Contains("201.1"))
				return Ok(message);
			if ((message ?? string.Empty).Replace("\"", "") == "401.1")
				return Unauthorized(message);
			return BadRequest(message);
		}




	}


}
