namespace NalamVazhaWebApi
{
	using System;
	using System.Collections.Generic;
	using System.Net;
	using System.Net.Mail;
	using System.Text;
	using Microsoft.Extensions.Logging;
	using NalamVazha.DAL;
	using NalamVazha.Models;

	// ══════════════════════════════════════════════════════════════════════════
	//  Booking-status → entity-action name map
	// ══════════════════════════════════════════════════════════════════════════

	/// <summary>
	/// Maps a human-readable booking status string to the camel-case action name
	/// used in Alert Templates (entityaction column).
	/// </summary>
	public static class IPDBookingStatusActionMap
	{
		private static readonly Dictionary<string, string> _map =
			new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
			{
				{ "Pending",                              "Pending"               },
				{ "Provisional Booking",                  "ProvisionalBooking"    },
				{ "Rejected",                             "Rejected"              },
				{ "Waitlisted",                           "Waitlisted"            },
				{ "Approved for Admission",               "ApprovedForAdmission"  },
				{ "Rework Requested",                     "ReworkRequested"       },
				{ "Doctor Review",                        "DoctorReview"          },
				{ "Provisional Confirmed",                "ProvisionalConfirmed"  },
				{ "Confirmed for Arrival",                "ConfirmedForArrival"   },
				{ "Admission Confirmed",                  "AdmissionConfirmed"    },
				{ "Cancelled",                            "AdmittedCancelled"     },
				{ "Admitted Cancelled",                   "AdmittedCancelled"     },
				{ "Admitted",                             "Admitted"              },
				{ "Screening completed by the patient",   "ScreeningCompleted"    },
				{ "Pre-Admission Initiated",              "PreAdmissionInitiated" },
				{ "Awaiting Consultation",                "AwaitingConsultation"  },
				{ "Assessment Approved",                  "AssessmentApproved"    },
				{ "Assessment Rejected",                  "AssessmentRejected"    },
				{ "Confirmed Billing",                    "ConfirmedBilling"      },
				{ "Confirm Admission",                    "ConfirmAdmission"      },
			};

		public static string ToActionName(string bookingStatus)
		{
			if (string.IsNullOrWhiteSpace(bookingStatus)) return bookingStatus ?? "";
			return _map.TryGetValue(bookingStatus.Trim(), out var action)
				? action
				: bookingStatus.Replace(" ", "");
		}
	}

	// ══════════════════════════════════════════════════════════════════════════
	//  Role-based recipient routing  (TO only — no CC)
	// ══════════════════════════════════════════════════════════════════════════

	/// <summary>
	/// Defines which roles receive the notification email (TO) for each booking status.
	///
	/// Roles recognised: Patient | Doctor | FrontDesk
	///
	/// Status → TO mapping
	/// ───────────────────────────────────────────────────────
	/// Patient statuses  → Patient
	/// Doctor statuses   → Doctor
	/// Staff statuses    → FrontDesk
	/// Multi-role        → Patient + FrontDesk
	/// </summary>
	public static class IPDNotificationRoleRouter
	{
		public enum Role { Patient, Doctor, FrontDesk }

		// Key = entityaction name produced by IPDBookingStatusActionMap
		private static readonly Dictionary<string, List<Role>> _routes =
			new Dictionary<string, List<Role>>(StringComparer.OrdinalIgnoreCase)
			{
                // ── Patient notified ─────────────────────────────────────────
                { "Pending",                new List<Role>{ Role.Patient                 } },
				{ "ProvisionalBooking",     new List<Role>{ Role.Patient, Role.FrontDesk } },
				{ "ProvisionalConfirmed",   new List<Role>{ Role.Patient                 } },
				{ "ScreeningCompleted",     new List<Role>{ Role.Patient                 } },
				{ "ApprovedForAdmission",   new List<Role>{ Role.Patient                 } },
				{ "ConfirmedForArrival",    new List<Role>{ Role.Patient                 } },
				{ "PreAdmissionInitiated",  new List<Role>{ Role.Patient                 } },
				{ "Rejected",               new List<Role>{ Role.Patient                 } },
				{ "Waitlisted",             new List<Role>{ Role.Patient                 } },
				{ "AdmissionConfirmed",     new List<Role>{ Role.Patient                 } },
				{ "AdmittedCancelled",      new List<Role>{ Role.Patient                 } },
				{ "Admitted",               new List<Role>{ Role.Patient                 } },
				{ "AssessmentRejected",     new List<Role>{ Role.Patient                 } },
				{ "ConfirmAdmission",       new List<Role>{ Role.Patient                 } },

                // ── Doctor notified ──────────────────────────────────────────
                { "AwaitingConsultation",   new List<Role>{ Role.Doctor                  } },
				{ "DoctorReview",           new List<Role>{ Role.Doctor                  } },

                // ── FrontDesk notified ───────────────────────────────────────
				{ "ReworkRequested",        new List<Role>{ Role.Patient                 } },

                // ── Patient + FrontDesk notified ─────────────────────────────
                // Assessment Approved → moves toward Admission (billing / confirm)
                { "AssessmentApproved",     new List<Role>{ Role.Patient, Role.FrontDesk } },
				{ "ConfirmedBilling",       new List<Role>{ Role.Patient, Role.FrontDesk } },
			};

		/// <summary>Returns the TO roles for the given action name, or FrontDesk as safe default.</summary>
		public static IReadOnlyList<Role> GetToRoles(string entityActionName)
		{
			if (!string.IsNullOrWhiteSpace(entityActionName) &&
				_routes.TryGetValue(entityActionName, out var roles))
				return roles;

			return new List<Role> { Role.FrontDesk };
		}
	}

	// ══════════════════════════════════════════════════════════════════════════
	//  Main notification helper
	// ══════════════════════════════════════════════════════════════════════════

	/// <summary>
	/// Sends email notifications for IPD booking status changes.
	///
	/// • SMTP comes exclusively from the tenant's MailBox DB record — never appsettings.json.
	/// • Recipients are role-based (TO only, no CC).
	/// • The SP "getById_sp_all_IPDApplicationForm" must expose:
	///     patient_emailaddress    – patient e-mail
	///     doctor_emailaddress     – assigned doctor e-mail
	///     frontdesk_emailaddress  – front-desk / case-manager e-mail
	/// </summary>
	public class IPDNotificationHelper
	{
		private readonly string _connectionString;
		private readonly ILogger _logger;
		private readonly string _applicationUrl;

		public IPDNotificationHelper(string connectionString, ILogger logger, string applicationUrl = "")
		{
			_connectionString = connectionString;
            _logger           = logger;
			_applicationUrl   = applicationUrl ?? "";
		}

		// ── Public entry point ────────────────────────────────────────────────

		/// <summary>
		/// SLA breach notification — mailto comes directly from Check_ProcessSLA_Breach SP.
		/// No role routing needed; SP already resolved the correct recipients.
		/// </summary>
		public void SendIPDNotification(
			string ipdApplicationFormId,
			string bookingStatus,
			string tenantId,
			string mailto)          // ← direct from SP
		{
			try
		{
				string entityActionName = IPDBookingStatusActionMap.ToActionName(bookingStatus);
				if (string.IsNullOrWhiteSpace(entityActionName)) return;
				if (string.IsNullOrWhiteSpace(mailto)) return;

				// 1. Build mail content from Alert Template
				var mailLogsDAL = new MailLogsDAL(_connectionString, _applicationUrl);
				mailmodel mailData = mailLogsDAL.Mailer(
					"IPDApplicationForm", entityActionName, ipdApplicationFormId, tenantId);

				if (string.IsNullOrWhiteSpace(mailData?.mailsubject)) return;

				// 2. Load tenant SMTP
				SmtpConfig smtpSettings = GetSmtpSettings(tenantId);
				if (smtpSettings == null) return;

				// 3. Send directly to SP-resolved mailto
				bool sent = SendSmtp(smtpSettings, mailto, mailData.mailsubject, mailData.mailbody);

				// 4. Log
				mailLogsDAL.Create_MailLog(new MailLogsModel
				{
					entityname = "IPDApplicationForm",
					entityid = ipdApplicationFormId,
					mailfor = "SLA Breach / " + entityActionName,
					mailto = mailto,
					mailsubject = mailData.mailsubject,
					mailbody = mailData.mailbody,
					issent = sent,
					createduser = new Guid("3076e1d2-a59b-4ee4-bca6-ede6ff1a1888"),
					craftmyapp_actionmethodname = "Create_MailLog"
				});
			}
			catch (Exception ex)
			{
				_logger?.LogError(ex,
					"IPDNotificationHelper.SendIPDSLANotification failed for {Id} / {Status}",
					ipdApplicationFormId, bookingStatus);
			}
		}

		/// <summary>
		/// Sends a booking-status notification to the recipients configured for the status.
		/// Recipient and tenant details are resolved from the IPD application.
		/// </summary>
		public void SendBookingStatusNotification(
			string ipdApplicationFormId,
			string bookingStatus,
			string tenantId = null)
		{
			try
			{
				string entityActionName = IPDBookingStatusActionMap.ToActionName(bookingStatus);
				if (string.IsNullOrWhiteSpace(entityActionName)) return;

				var emails = GetRoleEmails(ipdApplicationFormId, tenantId);
				tenantId = string.IsNullOrWhiteSpace(tenantId) ? emails.TenantId : tenantId;
				string mailto = BuildAddressList(
					IPDNotificationRoleRouter.GetToRoles(entityActionName), emails);
				if (string.IsNullOrWhiteSpace(mailto)) return;

				var mailLogsDAL = new MailLogsDAL(_connectionString, _applicationUrl);
				mailmodel mailData = mailLogsDAL.Mailer(
					"IPDApplicationForm", entityActionName, ipdApplicationFormId, tenantId);
				if (string.IsNullOrWhiteSpace(mailData?.mailsubject)) return;

				SmtpConfig smtpSettings = GetSmtpSettings(tenantId);
				if (smtpSettings == null) return;

				bool sent = SendSmtp(smtpSettings, mailto, mailData.mailsubject, mailData.mailbody);
				mailLogsDAL.Create_MailLog(new MailLogsModel
				{
					entityname = "IPDApplicationForm",
					entityid = ipdApplicationFormId,
					mailfor = "Booking Status / " + entityActionName,
					mailto = mailto,
					mailsubject = mailData.mailsubject,
					mailbody = mailData.mailbody,
					issent = sent,
					createduser = new Guid("3076e1d2-a59b-4ee4-bca6-ede6ff1a1888"),
					craftmyapp_actionmethodname = "Create_MailLog"
				});
			}
			catch (Exception ex)
			{
				_logger?.LogError(ex,
					"IPDNotificationHelper.SendBookingStatusNotification failed for {Id} / {Status}",
					ipdApplicationFormId, bookingStatus);
			}
		}

		/// <summary>Sends the patient an IPD rework request including the review reason.</summary>
		public void SendIPDReworkNotification(
			string ipdApplicationFormId,
			string reworkReason,
			string tenantId = null)
		{
			try
			{
				var details = GetRoleEmails(ipdApplicationFormId, tenantId);
				tenantId = string.IsNullOrWhiteSpace(tenantId) ? details.TenantId : tenantId;
				if (string.IsNullOrWhiteSpace(details.Patient)) return;

				var smtpSettings = GetSmtpSettings(tenantId);
				if (smtpSettings == null) return;

				string safeName = WebUtility.HtmlEncode(details.PatientName);
				string safeReference = WebUtility.HtmlEncode(details.BookingReference);
				string safeReason = WebUtility.HtmlEncode(reworkReason ?? "");
				string subject = $"Action required: IPD application rework - {details.BookingReference}";
				string body =
					"<div style=\"font-family:Arial,sans-serif;color:#333;line-height:1.6\">" +
					"<h2 style=\"color:#c2410c\">IPD Application Requires Rework</h2>" +
					$"<p>Dear {safeName},</p>" +
					"<p>Your IPD application requires additional information or corrections before it can proceed.</p>" +
					$"<p><strong>Booking reference:</strong> {safeReference}</p>" +
					$"<div style=\"padding:12px 14px;background:#fff7ed;border-left:4px solid #f97316\"><strong>Reason for rework</strong><br/>{safeReason}</div>" +
					"<p>Please sign in, update the application as requested, and submit it again for review.</p>" +
					"<p>Regards,<br/>Hospital Front Desk</p></div>";

				bool sent = SendSmtp(smtpSettings, details.Patient, subject, body);
				new MailLogsDAL(_connectionString, "").Create_MailLog(new MailLogsModel
				{
					entityname = "IPDApplicationForm",
					entityid = ipdApplicationFormId,
					mailfor = "Booking Status / ReworkRequested",
					mailto = details.Patient,
					mailsubject = subject,
					mailbody = body,
					issent = sent,
					createduser = new Guid("3076e1d2-a59b-4ee4-bca6-ede6ff1a1888"),
					craftmyapp_actionmethodname = "Create_MailLog"
				});
			}
			catch (Exception ex)
			{
				_logger?.LogError(ex,
					"IPDNotificationHelper.SendIPDReworkNotification failed for {Id}",
					ipdApplicationFormId);
			}
		}

		/// <summary>
		/// Sends the cancellation outcome to both the Health Seeker and Front Desk.
		/// Cancellation mail is deliberately built here because it includes runtime
		/// refund and remarks values that are not available to the generic alert template.
		/// </summary>
		public void SendIPDCancellationNotification(
			string ipdApplicationFormId,
			string tenantId,
			string bookingReference,
			string patientName,
			string cancellationRemarks,
			decimal refundAmount,
			string cancellationStage)
		{
			try
			{
				var emails = GetRoleEmails(ipdApplicationFormId, tenantId);
				var mailto = BuildAddressList(
					new[] { IPDNotificationRoleRouter.Role.Patient, IPDNotificationRoleRouter.Role.FrontDesk },
					emails);
				if (string.IsNullOrWhiteSpace(mailto)) return;

				var smtpSettings = GetSmtpSettings(tenantId);
				if (smtpSettings == null) return;

				string safeReference = WebUtility.HtmlEncode(bookingReference ?? "");
				string safePatientName = WebUtility.HtmlEncode(patientName ?? "");
				string safeRemarks = WebUtility.HtmlEncode(cancellationRemarks ?? "");
				string amount = refundAmount.ToString("N2", System.Globalization.CultureInfo.GetCultureInfo("en-IN"));
				string subject;
				string heading;
				string message;

				switch ((cancellationStage ?? "").Trim())
				{
					case "HealthSeekerRequested":
						subject = $"IPD cancellation requested - {bookingReference}";
						heading = "IPD Cancellation Requested by Health Seeker";
						message = $"The IPD booking has been cancelled by the Health Seeker. " +
							$"The estimated refund amount is <strong>₹{amount}</strong>. " +
							"The refund will be processed after the cancellation is reviewed and approved by the Front Desk.";
						break;
					case "HealthSeekerRefundApproved":
						subject = $"IPD cancellation approved and refund processed - {bookingReference}";
						heading = "IPD Cancellation Approved";
						message = $"The Health Seeker's cancellation request has been approved by the Front Desk, " +
							$"and the refund of <strong>₹{amount}</strong> has been processed successfully.";
						break;
					default:
						subject = $"IPD booking cancelled by hospital - {bookingReference}";
						heading = "IPD Booking Cancelled by Hospital";
						message = $"The IPD booking has been cancelled by the hospital. " +
							$"The applicable refund amount is <strong>₹{amount}</strong> and has been processed as per the cancellation policy.";
						break;
				}

				string body =
					"<div style=\"font-family:Arial,sans-serif;color:#333;line-height:1.6\">" +
					$"<h2 style=\"color:#176b5b\">{heading}</h2>" +
					$"<p>Dear {safePatientName},</p>" +
					$"<p>{message}</p>" +
					"<table style=\"border-collapse:collapse;width:100%;max-width:620px\">" +
					$"<tr><td style=\"padding:8px;border:1px solid #ddd\"><strong>Booking reference</strong></td><td style=\"padding:8px;border:1px solid #ddd\">{safeReference}</td></tr>" +
					$"<tr><td style=\"padding:8px;border:1px solid #ddd\"><strong>Refund amount</strong></td><td style=\"padding:8px;border:1px solid #ddd\">₹{amount}</td></tr>" +
					$"<tr><td style=\"padding:8px;border:1px solid #ddd\"><strong>Cancellation remarks</strong></td><td style=\"padding:8px;border:1px solid #ddd\">{safeRemarks}</td></tr>" +
					"</table>" +
					"<p>If you need clarification, please contact the Front Desk.</p>" +
					"<p>Regards,<br/>Hospital Front Desk</p></div>";

				bool sent = SendSmtp(smtpSettings, mailto, subject, body);
				new MailLogsDAL(_connectionString, "").Create_MailLog(new MailLogsModel
				{
					entityname = "IPDApplicationForm",
					entityid = ipdApplicationFormId,
					mailfor = "IPD Cancellation / " + cancellationStage,
					mailto = mailto,
					mailsubject = subject,
					mailbody = body,
					issent = sent,
					createduser = new Guid("3076e1d2-a59b-4ee4-bca6-ede6ff1a1888"),
					craftmyapp_actionmethodname = "Create_MailLog"
				});
			}
			catch (Exception ex)
			{
				_logger?.LogError(ex,
					"IPDNotificationHelper.SendIPDCancellationNotification failed for {Id}",
					ipdApplicationFormId);
			}
		}

		// ── Private helpers ───────────────────────────────────────────────────

		/// <summary>
		/// Fetches role e-mail addresses from the IPD entity SP.
		/// Expected columns: patient_emailaddress, doctor_emailaddress, frontdesk_emailaddress.
		/// </summary>
		private RoleEmails GetRoleEmails(string ipdApplicationFormId, string tenantId = null)
		{
			var result = new RoleEmails();
			try
			{
				using var conn = new Npgsql.NpgsqlConnection(_connectionString);
				conn.Open();
				using var cmd = new Npgsql.NpgsqlCommand(
					"SELECT * FROM \"getById_sp_all_IPDApplicationForm\"(@pvar_ipdapplicationformid)", conn);
				cmd.CommandType = System.Data.CommandType.Text;
				cmd.Parameters.AddWithValue("pvar_ipdapplicationformid",
					(object)ipdApplicationFormId ?? DBNull.Value);

				using var reader = cmd.ExecuteReader();
				if (reader.Read())
				{
					result.TenantId = SafeString(reader, "tenantid");
					result.PatientName = SafeString(reader, "patientname");
					result.BookingReference = SafeString(reader, "bookingreferencenumber");
					result.Patient = SafeString(reader, "patient_emailaddress");
					result.Doctor = SafeString(reader, "doctor_emailaddress");
					result.FrontDesk = SafeString(reader, "frontdesk_emailaddress");
				}
				reader.Close();

				if (string.IsNullOrWhiteSpace(result.FrontDesk) && Guid.TryParse(tenantId, out var parsedTenantId))
				{
					using var frontDeskCmd = new Npgsql.NpgsqlCommand(
						@"SELECT string_agg(DISTINCT NULLIF(btrim(emailid), ''), ',')
						  FROM users
						  WHERE COALESCE(isdeleted, false) = false
						    AND regexp_replace(LOWER(COALESCE(userrole, '')), '[^a-z]', '', 'g')
						        IN ('frontdesk', 'frontdeskadmin')
						    AND (
						        tenantid = @tenantid
						        OR @tenantid::text = ANY(
						            string_to_array(
						                regexp_replace(COALESCE(viewertenantids, ''), '\s', '', 'g'),
						                ','
						            )
						        )
						    )", conn);
					frontDeskCmd.Parameters.AddWithValue("tenantid", parsedTenantId);
					result.FrontDesk = frontDeskCmd.ExecuteScalar()?.ToString() ?? "";
					if (string.IsNullOrWhiteSpace(result.FrontDesk))
					{
						_logger?.LogWarning(
							"IPDNotificationHelper: no active Front Desk e-mail found for tenant {TenantId}.",
							tenantId);
					}
				}
			}
			catch (Exception ex)
			{
				_logger?.LogError(ex,
					"IPDNotificationHelper: failed to load role e-mails for IPD {Id}",
					ipdApplicationFormId);
			}
			return result;
		}

		/// <summary>Converts a list of roles to a comma-separated e-mail string.</summary>
		private static string BuildAddressList(
			IEnumerable<IPDNotificationRoleRouter.Role> roles,
			RoleEmails emails)
		{
			var list = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (var role in roles)
			{
				string addr = role switch
				{
					IPDNotificationRoleRouter.Role.Patient => emails.Patient,
					IPDNotificationRoleRouter.Role.Doctor => emails.Doctor,
					IPDNotificationRoleRouter.Role.FrontDesk => emails.FrontDesk,
					_ => ""
				};
				if (!string.IsNullOrWhiteSpace(addr))
				{
					foreach (var email in addr.Split(',', StringSplitOptions.RemoveEmptyEntries))
					{
						var trimmed = email.Trim();
						if (!string.IsNullOrWhiteSpace(trimmed)) list.Add(trimmed);
					}
				}
			}
			return string.Join(",", list);
		}

		/// <summary>
		/// Loads SMTP config from the tenant's MailBox DB record.
		/// Returns null if no record exists — caller skips the send.
		/// </summary>
		private SmtpConfig GetSmtpSettings(string tenantId)
		{
			if (string.IsNullOrWhiteSpace(tenantId))
			{
                _logger?.LogWarning("IPDNotificationHelper: tenantId is empty — cannot load MailBox, skipping notification.");
				return null;
			}
			try
			{
				var mailBoxDAL = new MailBoxDAL(_connectionString);
				var rows = mailBoxDAL.Mail_Box_List(tenantId);
				if (rows != null && rows.Rows.Count > 0)
				{
					var row = rows.Rows[0];
					return new SmtpConfig
					{
						FromMail = row["senderemail"]?.ToString() ?? "",
						Password = row["password"]?.ToString() ?? "",
						DisplayName = row["senderdisplayname"]?.ToString() ?? "",
						HostName = row["emailhostname"]?.ToString() ?? "",
						PortNumber = row["portnumber"] != DBNull.Value
										? Convert.ToInt32(row["portnumber"]) : 25,
						EnableSSL = true
					};
				}
                _logger?.LogWarning("IPDNotificationHelper: No MailBox record found for tenant {TenantId} — skipping notification.", tenantId);
				return null;
			}
			catch (Exception ex)
			{
                _logger?.LogError(ex, "IPDNotificationHelper: failed to load MailBox for tenant {TenantId}.", tenantId);
				return null;
			}
		}

		private bool SendSmtp(SmtpConfig cfg, string toEmails, string subject, string body)
		{
			try
			{
				var mail = new MailMessage
				{
					From = new MailAddress(cfg.FromMail, cfg.DisplayName),
					Subject = subject,
					SubjectEncoding = Encoding.UTF8,
					Body = body,
					BodyEncoding = Encoding.UTF8,
					IsBodyHtml = true,
					Priority = MailPriority.Normal
				};

				foreach (var addr in toEmails.Split(',', StringSplitOptions.RemoveEmptyEntries))
				{
					var trimmed = addr.Trim();
					if (!string.IsNullOrEmpty(trimmed)) mail.To.Add(trimmed);
				}

				if (mail.To.Count == 0) return false;

				using var smtp = new SmtpClient
				{
                    Host             = cfg.HostName,
                    Port             = cfg.PortNumber,
                    EnableSsl        = cfg.EnableSSL,
                    DeliveryMethod   = SmtpDeliveryMethod.Network,
                    Credentials      = new NetworkCredential(cfg.FromMail, cfg.Password),
                    Timeout          = 20000
				};
				smtp.Send(mail);
				return true;
			}
			catch (Exception ex)
			{
                _logger?.LogError(ex, "IPDNotificationHelper.SendSmtp failed: {Message}", ex.Message);
				return false;
			}
		}

		// ── Utilities ─────────────────────────────────────────────────────────

		private static string SafeString(System.Data.IDataReader reader, string column)
		{
			try { return reader[column]?.ToString() ?? ""; }
			catch { return ""; }
		}

		// ── Inner types ───────────────────────────────────────────────────────

		private class RoleEmails
		{
			public string TenantId { get; set; } = "";
			public string PatientName { get; set; } = "";
			public string BookingReference { get; set; } = "";
			public string Patient { get; set; } = "";
			public string Doctor { get; set; } = "";
			public string FrontDesk { get; set; } = "";
		}

		private class SmtpConfig
		{
            public string FromMail    { get; set; }
            public string Password    { get; set; }
			public string DisplayName { get; set; }
            public string HostName    { get; set; }
            public int    PortNumber  { get; set; }
            public bool   EnableSSL   { get; set; }
		}
	}
}
