namespace NalamVazhaWebApi
{
	using System;
	using System.Data;
	using System.Net;
	using System.Net.Mail;
	using System.Text;
	using Microsoft.Extensions.Logging;
	using NalamVazha.DAL;
	using NalamVazha.Models;

	/// <summary>
	/// Sends OPD workflow mail from the entities resolved by
	/// Get_OPD_Online_Doctor_Workflow_Context.
	/// </summary>
	public sealed class OPDWorkflowNotificationHelper
	{
		private static readonly Guid SystemUserId =
			new Guid("3076e1d2-a59b-4ee4-bca6-ede6ff1a1888");

		private readonly string _connectionString;
		private readonly ILogger _logger;

		public OPDWorkflowNotificationHelper(string connectionString, ILogger logger)
		{
			_connectionString = connectionString;
			_logger = logger;
		}

		public bool SendInternScreeningEmail(Guid opdFormId, Guid? createdUser)
		{
			try
			{
				var context = new OPDFormDAL(_connectionString)
					.Get_OPD_Online_Doctor_Workflow_Context(opdFormId.ToString());
				if (context == null || context.Rows.Count == 0) return false;

				var row = context.Rows[0];
				var appointmentMode = Value(row, "appointmentmode");
				var taskName = Value(row, "taskname");
				if (appointmentMode.IndexOf("online", StringComparison.OrdinalIgnoreCase) < 0
					&& taskName.IndexOf("online", StringComparison.OrdinalIgnoreCase) < 0)
					return false;

				var email = Value(row, "interndoctoremail").Trim();
				var healthSeekerEmail = Value(row, "patientemail").Trim();
				var meetingLink = Value(row, "internmeetinglink").Trim();
				if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(meetingLink))
				{
					_logger?.LogWarning(
						"OPD intern screening email skipped for {OPDFormId}: mapped intern email or meeting link is missing.",
						opdFormId);
					return false;
				}

				string Enc(string value) => WebUtility.HtmlEncode(value ?? "");
				var opdNumber = Value(row, "bookingreferencenumber", "-");
				var subject = "OPD Screening Required - " + opdNumber;
				var body =
					$"<p>Dear {Enc(Value(row, "interndoctorname"))},</p>" +
					"<p>Please conduct the patient screening, record the history and assessment, and submit it for senior doctor review.</p>" +
					$"<p><strong>Patient:</strong> {Enc(Value(row, "patientname", "-"))}<br/>" +
					$"<strong>OPD Number:</strong> {Enc(opdNumber)}<br/>" +
					$"<strong>Appointment:</strong> {Enc(Value(row, "appointmentdate", "-"))} " +
					$"{Enc(Value(row, "durationfrom", "-"))} - {Enc(Value(row, "durationto", "-"))}</p>" +
					$"<p><strong>Meeting Link:</strong> <a href='{Enc(meetingLink)}'>{Enc(meetingLink)}</a></p>";

				var sent = Send(row, email, healthSeekerEmail, subject, body);
				new MailLogsDAL(_connectionString, "").Create_MailLog(new MailLogsModel
				{
					entityname = "OPDForm",
					entityid = opdFormId.ToString(),
					mailfor = "Intern Screening",
					mailto = email,
					mailsubject = subject,
					mailbody = body,
					issent = sent,
					createduser = createdUser ?? SystemUserId,
					craftmyapp_actionmethodname = "Create_MailLog"
				});
				return sent;
			}
			catch (Exception ex)
			{
				_logger?.LogError(ex, "OPD intern screening email failed for {OPDFormId}", opdFormId);
				return false;
			}
		}

		private bool Send(DataRow context, string recipient, string ccRecipient, string subject, string body)
		{
			var tenantId = Value(context, "tenantid");
			var mailboxes = new MailBoxDAL(_connectionString).Mail_Box_List(tenantId);
			if (mailboxes == null || mailboxes.Rows.Count == 0)
			{
				_logger?.LogWarning("No tenant mailbox is configured for OPD {OPDFormId}.",
					Value(context, "opdformid"));
				return false;
			}

			var mailbox = mailboxes.Rows[0];
			using var message = new MailMessage
			{
				From = new MailAddress(Value(mailbox, "senderemail"), Value(mailbox, "senderdisplayname")),
				Subject = subject,
				SubjectEncoding = Encoding.UTF8,
				Body = body,
				BodyEncoding = Encoding.UTF8,
				IsBodyHtml = true
			};
			message.To.Add(recipient);
			if (!string.IsNullOrWhiteSpace(ccRecipient)
				&& !string.Equals(recipient, ccRecipient, StringComparison.OrdinalIgnoreCase))
				message.CC.Add(ccRecipient);

			using var smtp = new SmtpClient
			{
				Host = Value(mailbox, "emailhostname"),
				Port = int.TryParse(Value(mailbox, "portnumber"), out var port) ? port : 25,
				EnableSsl = true,
				DeliveryMethod = SmtpDeliveryMethod.Network,
				Credentials = new NetworkCredential(
					Value(mailbox, "senderemail"),
					Value(mailbox, "password")),
				Timeout = 20000
			};
			smtp.Send(message);
			return true;
		}

		private static string Value(DataRow row, string column, string fallback = "")
		{
			if (row == null || !row.Table.Columns.Contains(column) || row[column] == DBNull.Value)
				return fallback;
			var value = row[column]?.ToString();
			return string.IsNullOrWhiteSpace(value) ? fallback : value;
		}
	}
}
