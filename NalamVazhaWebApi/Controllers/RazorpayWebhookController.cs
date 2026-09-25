namespace NalamVazhaWebApi.Controllers
{
	using System;
	using System.Data;
	using System.Security.Cryptography;
	using System.Text;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.Logging;
	using Microsoft.Extensions.Options;
	using NalamVazha.DAL;
	using NalamVazha.Models;
	using Newtonsoft.Json.Linq;
	using Npgsql;
	using NpgsqlTypes;

	[Route("api/[controller]/[action]")]
	public class RazorpayWebhookController : ControllerBase
	{
		private readonly IOptions<ConnectionSettings> _connectionSettings;
		private readonly IConfiguration _configuration;
		private readonly ILogger _logger;

		public RazorpayWebhookController(IOptions<ConnectionSettings> connectionSettings, IConfiguration configuration, ILoggerFactory loggerFactory)
		{
			_connectionSettings = connectionSettings;
			_configuration = configuration;
			_logger = loggerFactory.CreateLogger<RazorpayWebhookController>();
		}

		[HttpPost]
		[AllowAnonymous]
		[ActionName("Handle")]
		public IActionResult Handle([FromBody] JObject payload)
		{
			try
			{
				if (payload == null) return BadRequest("Webhook payload is required.");
				if (!IsWebhookSignatureValid(payload))
					return Unauthorized("Invalid Razorpay webhook signature.");

				string eventName = payload["event"]?.ToString() ?? "";
				if (!string.Equals(eventName, "payment.captured", StringComparison.OrdinalIgnoreCase)
					&& !string.Equals(eventName, "payment.authorized", StringComparison.OrdinalIgnoreCase)
					&& !string.Equals(eventName, "payment.failed", StringComparison.OrdinalIgnoreCase))
				{
					return Ok(new { message = "ignored", eventName });
				}

				var payment = payload["payload"]?["payment"]?["entity"] as JObject;
				if (payment == null) return BadRequest("Payment entity missing.");

				string orderId = payment["order_id"]?.ToString();
				string paymentId = payment["id"]?.ToString();
				if (string.IsNullOrWhiteSpace(orderId) || string.IsNullOrWhiteSpace(paymentId))
					return BadRequest("Razorpay order_id/payment id missing.");

				if (IsSuccessfulPaymentEvent(eventName) && HasPaidPaymentResponse(paymentId))
					return Ok(new { message = "201.1", duplicate = true });

				var request = FindPaymentRequestByOrderId(orderId);
				if (request == null)
					return NotFound("Payment request not found for Razorpay order.");

				if (request.PaymentMarker.StartsWith("IPD:", StringComparison.OrdinalIgnoreCase))
					return RecoverIPDPayment(request, payment, eventName);
				if (request.PaymentMarker.StartsWith("OPD:", StringComparison.OrdinalIgnoreCase))
					return RecoverOPDPayment(request, payment, eventName);

				return Ok(new { message = "ignored", reason = "Unknown payment request marker." });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Razorpay webhook failed: " + ex.Message);
				return BadRequest(ex.Message);
			}
		}

		private IActionResult RecoverIPDPayment(WebhookPaymentRequest request, JObject payment, string eventName)
		{
			string ipdFormId = ExtractMarkedId(request.PaymentMarker, "IPD");
			if (!Guid.TryParse(ipdFormId, out var ipdGuid))
				return BadRequest("Invalid IPD marker in payment request.");
			bool isSuccessfulPayment = IsSuccessfulPaymentEvent(eventName);
			if (isSuccessfulPayment && HasSuccessfulIPDBookingDepositPayment(ipdGuid))
				return Ok(new { message = "201.1", duplicate = true });

			string paymentId = payment["id"]?.ToString();
			string method = payment["method"]?.ToString();
			string orderId = payment["order_id"]?.ToString();
			string responseStatus = isSuccessfulPayment ? "Paid" : "Failed";
			string errorCode = payment["error_code"]?.ToString();
			string errorDescription = payment["error_description"]?.ToString();
			string errorReason = payment["error_reason"]?.ToString();
			if (!isSuccessfulPayment && HasDuplicateFailedPaymentResponse(request.PaymentRequestId, request.OrderId, paymentId, errorCode, errorReason))
				return Ok(new { message = "201.1", duplicate = true, type = "IPD" });

			var paymentResponseDal = new PaymentResponseDAL(_connectionSettings.Value.ConnectionString);
			var response = new PaymentResponseModel
			{
				PaymentResponseid = Guid.NewGuid(),
				tenantid = request.TenantId,
				paymentrequest = request.PaymentRequestId,
				paymenttype = "Booking Deposit",
				transactiontime = GetApplicationNow(),
				orderid = string.IsNullOrWhiteSpace(orderId) ? request.OrderId : orderId,
				paymentid = paymentId,
				status = responseStatus,
				amount = request.Amount,
				paymentmethod = ResolvePaymentResponseMethod(method),
				banktransactionid = payment["error_step"]?.ToString(),
				gatewayresponsecode = errorCode,
				gatewayresponsemessage = errorDescription,
				responsesignature = errorReason,
				createduser = request.CreatedUser,
				craftmyapp_actionmethodname = "Add_Payment_Response"
			};
			SavePaymentResponseOrThrow(paymentResponseDal, response);
			if (!isSuccessfulPayment)
				return Ok(new { message = "201.1", recovered = true, type = "IPD", status = "Failed" });

			var ipdDal = new IPDApplicationFormDAL(_connectionSettings.Value.ConnectionString);
			var ipdTable = ipdDal.Get_IPD_Payment_Details(ipdFormId);
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
				currency = request.Currency,
				conversionrate = 1,
				paymentmode = MapRazorpayMethod(method),
				transactionreference = paymentId,
				paymentstatus = "Success",
				collectedby = request.CreatedUser,
				counterid = "Through Razorpay",
				remarks = $"Recovered by Razorpay webhook. Razorpay Order: {request.OrderId}. Payment ID: {paymentId}.",
				createduser = request.CreatedUser,
				craftmyapp_actionmethodname = "Add_Billing_Payment"
			};
			var billingMessage = billingDal.Add_Billing_Payment(billing);
			if (!(billingMessage ?? "").Replace("\"", "").Contains("201.1"))
				throw new Exception(billingMessage);

			billingDal.ApplyIPDRazorpayReceivablePayment(ipdGuid, request.Amount, billing.BillingPaymentid.Value, request.CreatedUser);
			ipdDal.Update_IPD_BookingStatus(ipdFormId, "Provisional Confirmed", request.CreatedUser?.ToString());

			return Ok(new { message = "201.1", recovered = true, type = "IPD" });
		}

		private IActionResult RecoverOPDPayment(WebhookPaymentRequest request, JObject payment, string eventName)
		{
			string opdFormId = ExtractMarkedId(request.PaymentMarker, "OPD");
			if (!Guid.TryParse(opdFormId, out var opdGuid))
				return BadRequest("Invalid OPD marker in payment request.");
			bool isSuccessfulPayment = IsSuccessfulPaymentEvent(eventName);
			if (isSuccessfulPayment && HasSuccessfulOPDPayment(opdGuid))
				return Ok(new { message = "201.1", duplicate = true });

			string paymentId = payment["id"]?.ToString();
			string method = payment["method"]?.ToString();
			string orderId = payment["order_id"]?.ToString();
			string responseStatus = isSuccessfulPayment ? "Paid" : "Failed";
			string errorCode = payment["error_code"]?.ToString();
			string errorDescription = payment["error_description"]?.ToString();
			string errorReason = payment["error_reason"]?.ToString();
			if (!isSuccessfulPayment && HasDuplicateFailedPaymentResponse(request.PaymentRequestId, request.OrderId, paymentId, errorCode, errorReason))
				return Ok(new { message = "201.1", duplicate = true, type = "OPD" });

			var paymentResponseDal = new PaymentResponseDAL(_connectionSettings.Value.ConnectionString);
			var response = new PaymentResponseModel
			{
				PaymentResponseid = Guid.NewGuid(),
				tenantid = request.TenantId,
				paymentrequest = request.PaymentRequestId,
				paymenttype = request.PaymentType,
				transactiontime = GetApplicationNow(),
				orderid = string.IsNullOrWhiteSpace(orderId) ? request.OrderId : orderId,
				paymentid = paymentId,
				status = responseStatus,
				amount = request.Amount,
				paymentmethod = ResolvePaymentResponseMethod(method),
				banktransactionid = payment["error_step"]?.ToString(),
				gatewayresponsecode = errorCode,
				gatewayresponsemessage = errorDescription,
				responsesignature = errorReason,
				createduser = request.CreatedUser,
				craftmyapp_actionmethodname = "Add_Payment_Response"
			};
			SavePaymentResponseOrThrow(paymentResponseDal, response);
			if (!isSuccessfulPayment)
				return Ok(new { message = "201.1", recovered = true, type = "OPD", status = "Failed" });

			var opdDal = new OPDFormDAL(_connectionSettings.Value.ConnectionString);
			var opdTable = opdDal.Get_OPD_Consultation_Fee(opdFormId);
			string patientGuid = opdTable?.Rows.Count > 0 ? opdTable.Rows[0]["patientname"]?.ToString() : null;
			string patientVisit = opdTable?.Rows.Count > 0 ? opdTable.Rows[0]["patientvisit"]?.ToString() : null;

			var billingDal = new BillingPaymentDAL(_connectionSettings.Value.ConnectionString);
			var billing = new BillingPaymentModel
			{
				BillingPaymentid = Guid.NewGuid(),
				tenantid = request.TenantId,
				paymentdate = GetApplicationNow().Date,
				receivablefor = string.IsNullOrWhiteSpace(request.PaymentType) ? "OPD Consultation Fee" : request.PaymentType,
				patientvisit = Guid.TryParse(patientVisit, out var pvid) ? pvid : (Guid?)null,
				opdnumber = opdGuid,
				patientname = Guid.TryParse(patientGuid, out var pid) ? pid : (Guid?)null,
				amount = request.Amount,
				currency = request.Currency,
				paymentmode = MapRazorpayMethod(method),
				transactionreference = paymentId,
				paymentstatus = "Paid",
				collectedby = request.CreatedUser,
				remarks = $"Recovered by Razorpay webhook. Razorpay Order: {request.OrderId}. Payment ID: {paymentId}.",
				createduser = request.CreatedUser,
				craftmyapp_actionmethodname = "Add_Billing_Payment"
			};
			var billingMessage = billingDal.Add_Billing_Payment(billing);
			if (!(billingMessage ?? "").Replace("\"", "").Contains("201.1"))
				throw new Exception(billingMessage);

			billingDal.UpdateOPDConsultationFeeReceivable(opdGuid, request.Amount, billing.BillingPaymentid.Value);
			new OPDWorkflowNotificationHelper(_connectionSettings.Value.ConnectionString, _logger)
				.SendInternScreeningEmail(opdGuid, request.CreatedUser);
			return Ok(new { message = "201.1", recovered = true, type = "OPD" });
		}

		private bool IsWebhookSignatureValid(JObject payload)
		{
			string secret = _configuration["Razorpay:WebhookSecret"] ?? _configuration["RazorPay:WebhookSecret"];
			if (string.IsNullOrWhiteSpace(secret))
				return true;

			string signature = Request.Headers["X-Razorpay-Signature"].ToString();
			if (string.IsNullOrWhiteSpace(signature))
				return false;

			string body = payload.ToString(Newtonsoft.Json.Formatting.None);
			using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
			string computed = BitConverter.ToString(hmac.ComputeHash(Encoding.UTF8.GetBytes(body))).Replace("-", "").ToLowerInvariant();
			return string.Equals(computed, signature, StringComparison.OrdinalIgnoreCase);
		}

		private WebhookPaymentRequest FindPaymentRequestByOrderId(string orderId)
		{
			using var conn = new NpgsqlConnection(_connectionSettings.Value.ConnectionString);
			conn.Open();
			using var cmd = new NpgsqlCommand("SELECT * FROM \"Get_Razorpay_Webhook_Payment_Request\"(@pvar_orderid)", conn);
			cmd.Parameters.AddWithValue("pvar_orderid", NpgsqlDbType.Varchar, (object)orderId ?? DBNull.Value);
			using var reader = cmd.ExecuteReader();
			if (!reader.Read()) return null;
			return new WebhookPaymentRequest
			{
				PaymentRequestId = reader.GetGuid(0),
				TenantId = reader.IsDBNull(1) ? (Guid?)null : reader.GetGuid(1),
				PaymentType = reader.IsDBNull(2) ? "" : reader.GetString(2),
				OrderId = reader.IsDBNull(3) ? "" : reader.GetString(3),
				PaymentMarker = reader.IsDBNull(4) ? "" : reader.GetString(4),
				Amount = reader.IsDBNull(5) ? 0 : reader.GetDecimal(5),
				Currency = reader.IsDBNull(6) ? "INR" : reader.GetString(6),
				CreatedUser = reader.IsDBNull(7) ? (Guid?)null : reader.GetGuid(7)
			};
		}

		private bool HasPaidPaymentResponse(string razorpayPaymentId)
		{
			if (string.IsNullOrWhiteSpace(razorpayPaymentId)) return false;
			using var conn = new NpgsqlConnection(_connectionSettings.Value.ConnectionString);
			conn.Open();
			using var cmd = new NpgsqlCommand("SELECT \"Razorpay_Webhook_Has_Paid_Payment_Response\"(@pvar_paymentid)", conn);
			cmd.Parameters.AddWithValue("pvar_paymentid", NpgsqlDbType.Varchar, (object)razorpayPaymentId ?? DBNull.Value);
			return Convert.ToBoolean(cmd.ExecuteScalar());
		}

		private bool HasSuccessfulIPDBookingDepositPayment(Guid ipdFormId)
		{
			using var conn = new NpgsqlConnection(_connectionSettings.Value.ConnectionString);
			conn.Open();
			using var cmd = new NpgsqlCommand("SELECT \"Razorpay_Webhook_Has_Successful_IPD_Booking_Deposit\"(@pvar_ipdnumber)", conn);
			cmd.Parameters.AddWithValue("pvar_ipdnumber", NpgsqlDbType.Uuid, ipdFormId);
			return Convert.ToBoolean(cmd.ExecuteScalar());
		}

		private bool HasSuccessfulOPDPayment(Guid opdFormId)
		{
			using var conn = new NpgsqlConnection(_connectionSettings.Value.ConnectionString);
			conn.Open();
			using var cmd = new NpgsqlCommand("SELECT \"Razorpay_Webhook_Has_Successful_OPD_Payment\"(@pvar_opdnumber)", conn);
			cmd.Parameters.AddWithValue("pvar_opdnumber", NpgsqlDbType.Uuid, opdFormId);
			return Convert.ToBoolean(cmd.ExecuteScalar());
		}

		private bool HasDuplicateFailedPaymentResponse(Guid paymentRequestId, string orderId, string paymentId, string errorCode, string errorReason)
		{
			using var conn = new NpgsqlConnection(_connectionSettings.Value.ConnectionString);
			conn.Open();
			using var cmd = new NpgsqlCommand(@"
				SELECT EXISTS (
					SELECT 1
					FROM PaymentResponse
					WHERE COALESCE(isdeleted, false) = false
					  AND paymentrequest = @pvar_paymentrequest
					  AND LOWER(COALESCE(status, '')) = 'failed'
					  AND COALESCE(orderid, '') = COALESCE(@pvar_orderid, '')
					  AND (
						  (COALESCE(@pvar_paymentid, '') <> '' AND COALESCE(paymentid, '') = COALESCE(@pvar_paymentid, ''))
						  OR (
							  COALESCE(@pvar_paymentid, '') = ''
							  AND COALESCE(paymentid, '') = ''
							  AND COALESCE(gatewayresponsecode, '') = COALESCE(@pvar_errorcode, '')
							  AND COALESCE(responsesignature, '') = COALESCE(@pvar_errorreason, '')
						  )
					  )
				)", conn);
			cmd.Parameters.AddWithValue("pvar_paymentrequest", NpgsqlDbType.Uuid, paymentRequestId);
			cmd.Parameters.AddWithValue("pvar_orderid", NpgsqlDbType.Varchar, (object)orderId ?? DBNull.Value);
			cmd.Parameters.AddWithValue("pvar_paymentid", NpgsqlDbType.Varchar, (object)paymentId ?? DBNull.Value);
			cmd.Parameters.AddWithValue("pvar_errorcode", NpgsqlDbType.Varchar, (object)errorCode ?? DBNull.Value);
			cmd.Parameters.AddWithValue("pvar_errorreason", NpgsqlDbType.Varchar, (object)errorReason ?? DBNull.Value);
			return Convert.ToBoolean(cmd.ExecuteScalar());
		}

		private static string ExtractMarkedId(string marker, string type)
		{
			string prefix = type + ":";
			if (string.IsNullOrWhiteSpace(marker) || !marker.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
				return "";
			int reqIndex = marker.IndexOf(":REQ:", StringComparison.OrdinalIgnoreCase);
			return reqIndex > prefix.Length ? marker.Substring(prefix.Length, reqIndex - prefix.Length) : "";
		}

		private static bool IsSuccessfulPaymentEvent(string eventName)
		{
			return string.Equals(eventName, "payment.captured", StringComparison.OrdinalIgnoreCase)
				|| string.Equals(eventName, "payment.authorized", StringComparison.OrdinalIgnoreCase);
		}

		private static void SavePaymentResponseOrThrow(PaymentResponseDAL paymentResponseDal, PaymentResponseModel response)
		{
			var responseMessage = paymentResponseDal.Add_Payment_Response(response);
			if (!(responseMessage ?? "").Replace("\"", "").Contains("201.1"))
				throw new Exception("Payment response was not saved: " + responseMessage);
		}

		private static string ResolvePaymentResponseMethod(string method)
		{
			return string.IsNullOrWhiteSpace(method) ? null : MapRazorpayMethod(method);
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

		private class WebhookPaymentRequest
		{
			public Guid PaymentRequestId { get; set; }
			public Guid? TenantId { get; set; }
			public string PaymentType { get; set; }
			public string OrderId { get; set; }
			public string PaymentMarker { get; set; }
			public decimal Amount { get; set; }
			public string Currency { get; set; }
			public Guid? CreatedUser { get; set; }
		}
	}
}
