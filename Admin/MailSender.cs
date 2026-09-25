using System;
using System.Data;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Net;
using FluentValidation.Results;
using NalamVazha.Models;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.HttpOverrides;
using System.Text;
using System.Text.RegularExpressions;
namespace Admin.Controllers
{
    public class MailSender
    {
        public async Task<string> GetTenantName(HttpClient client, string tenantid, string applicableService = "General")
        {
            if (string.IsNullOrWhiteSpace(tenantid))
                return "";

            try
            {
                string json = await ApiClient.Get_ApiValues(
                    client,
                    "api/tenant/getById_tenant?tenantid=" + Uri.EscapeDataString(tenantid));
                tenantModel tenant = string.IsNullOrWhiteSpace(json)
                    ? null
                    : JsonConvert.DeserializeObject<tenantModel>(json);
                string tenantName = tenant?.businessname?.Trim() ?? "";
                if (!string.IsNullOrWhiteSpace(tenantName))
                    return tenantName;
            }
            catch
            {
                // Fall through to the tenant mailbox display name.
            }

            try
            {
                MailBoxModel mailBox = await GetTenantMailBox(client, tenantid, applicableService);
                return mailBox?.senderdisplayname?.Trim() ?? "";
            }
            catch
            {
                return "";
            }
        }

        public static string ApplyTenantBranding(string content, string tenantName)
        {
            if (string.IsNullOrEmpty(content) || string.IsNullOrWhiteSpace(tenantName))
                return content;

            string branded = Regex.Replace(
                content,
                @"\{tenantname\}\s*,?(?:\s*<br\s*/?>|\s*\r?\n|\s*)\s*\{tenantname\}(?:\s+Team)?",
                "{tenantname}",
                RegexOptions.IgnoreCase);
            branded = branded.Replace("{tenantname}", tenantName, StringComparison.OrdinalIgnoreCase);
            branded = Regex.Replace(
                branded,
                @"Regards\s*,?(?:\s*<br\s*/?>|\s*\r?\n|\s*)\s*Nalam(?:Vazha)?(?:\s+Health)?(?:\s+Team)?",
                tenantName,
                RegexOptions.IgnoreCase);
			branded = Regex.Replace(
				branded,
				@"\bFor\s+(?:Nisargopachar\s+Kendra|" + Regex.Escape(tenantName) + @")(?=\s*(?:<br\s*/?>|</p>|</div>|$))",
				"From " + tenantName,
				RegexOptions.IgnoreCase);
            branded = Regex.Replace(branded, @"\bNalam(?:Vazha)?(?:\s+Health)?\b", tenantName, RegexOptions.IgnoreCase);
            return Regex.Replace(branded, @"\bRegards\b", tenantName, RegexOptions.IgnoreCase);
        }

		public static string ApplyPaymentLink(string content, string paymentLink)
		{
			if (string.IsNullOrEmpty(content) || string.IsNullOrEmpty(paymentLink))
				return content;

			const string tilde = @"(?:~|&#0*126;|&#x0*7e;|&tilde;)";
			string replaced = Regex.Replace(
				content,
				tilde + @"(?:\s|&nbsp;)*payment\s*link(?:\s|&nbsp;)*" + tilde,
				_ => paymentLink,
				RegexOptions.IgnoreCase);

			return Regex.Replace(
				replaced,
				@"\{\s*payment\s*link\s*\}",
				_ => paymentLink,
				RegexOptions.IgnoreCase);
		}

        public static string ResolveApplicableService(string entityname, string entityactionname)
        {
            bool isRegistrationMail =
                string.Equals(entityname, "OTP", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(entityactionname, "registration", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(entityactionname, "forgotpassword", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(entityactionname, "OTP Generator", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(entityactionname, "User credientils", StringComparison.OrdinalIgnoreCase);

            return isRegistrationMail ? "Registration" : "General";
        }

        private static bool HasApplicableService(DataRow row, string applicableService)
        {
            if (row == null || string.IsNullOrWhiteSpace(applicableService) || !row.Table.Columns.Contains("applicableservice"))
                return false;

            string rawValue = row["applicableservice"]?.ToString() ?? "";
            if (string.IsNullOrWhiteSpace(rawValue))
                return false;

            string[] tokens = rawValue
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();

            return tokens.Any(x => string.Equals(x, applicableService, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<MailBoxModel> GetTenantMailBox(HttpClient client, string tenantid, string applicableService)
        {
            if (string.IsNullOrWhiteSpace(tenantid))
                return null;

            HttpResponseMessage response = await ApiClient.GET_ApiValuesGetRespnse(
                client,
                "api/MailBox/get_all_MailBox?tenantid=" + Uri.EscapeDataString(tenantid));

            if (!response.IsSuccessStatusCode)
                return null;

            DataTable mailBoxes = await response.Content.ReadAsAsync<DataTable>();
            if (mailBoxes == null || mailBoxes.Rows.Count == 0)
                return null;

            DataRow row = mailBoxes.AsEnumerable()
                .FirstOrDefault(x => HasApplicableService(x, applicableService))
                ?? mailBoxes.AsEnumerable().FirstOrDefault(x => HasApplicableService(x, "All"))
                ?? mailBoxes.Rows[0];

            return new MailBoxModel
            {
                senderdisplayname = row.Table.Columns.Contains("senderdisplayname") ? row["senderdisplayname"]?.ToString() : "",
                senderemail = row.Table.Columns.Contains("senderemail") ? row["senderemail"]?.ToString() : "",
                password = row.Table.Columns.Contains("password") ? row["password"]?.ToString() : "",
                emailhostname = row.Table.Columns.Contains("emailhostname") ? row["emailhostname"]?.ToString() : "",
                applicableservice = row.Table.Columns.Contains("applicableservice") ? row["applicableservice"]?.ToString() : "",
                portnumber = row.Table.Columns.Contains("portnumber") && row["portnumber"] != DBNull.Value
                    ? Convert.ToInt32(row["portnumber"])
                    : 25
            };
        }

        private static string ResolveTenantId(string explicitTenantId, string entityname, string entityactionname, string entityid)
        {
            if (!string.IsNullOrWhiteSpace(explicitTenantId))
                return explicitTenantId;

            if (string.Equals(entityname, "users", StringComparison.OrdinalIgnoreCase)
                && (string.Equals(entityactionname, "registration", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(entityactionname, "forgotpassword", StringComparison.OrdinalIgnoreCase)))
                return entityid;

            return "";
        }

        public async Task<bool> sendMail(
             string entityname
            , string entityactionname
            , string entityid
            , string mailfor

            , IOptions<MailSettings> _mailSettings
            , string loginUserID
            , HttpClient client
            , string optionaldataOne = ""
            , string optionaldataTwo = ""
            , string adminURL = ""
            , string tenantid = "")
        {




            DataTable mailData = new DataTable();
            tenantid = ResolveTenantId(tenantid, entityname, entityactionname, entityid);
            string applicableService = ResolveApplicableService(entityname, entityactionname);
            string tenantName = await GetTenantName(client, tenantid, applicableService);

            bool mailSent = false;
            HttpResponseMessage response_alert_maildata = await ApiClient.GET_ApiValuesGetRespnse(client, "api/MailLogs/MailSender?mailfor=" + entityactionname + "&entityid=" + entityid + "&createduser=" + loginUserID);


            if (response_alert_maildata.IsSuccessStatusCode)
            {
                mailData = await response_alert_maildata.Content.ReadAsAsync<DataTable>();

                if (mailData.Rows.Count > 0 || entityname == "OTP")
                {

                    HttpResponseMessage response_alert_templates = await ApiClient.GET_ApiValuesGetRespnse(client, "api/AlertTemplates/Alert_Templates_List?tenantid=" + Uri.EscapeDataString(tenantid) + "&entityname=" + entityname + "&entityaction=" + entityactionname + "");


                    if (response_alert_templates.IsSuccessStatusCode)
                    {
                        DataTable dt_alert_template = await response_alert_templates.Content.ReadAsAsync<DataTable>();
                        if (dt_alert_template.Rows.Count > 0)
                        {
                            if (dt_alert_template.Rows[0]["alerttype"].ToString() == "Email")
                            {
                                string alertcontent = dt_alert_template.Rows[0]["alertcontent"].ToString();
                                string alertsubject = "";
                                switch (entityactionname)
                                {

                                    case "forgotpassword":
                                    case "registration":
                                        alertcontent = alertcontent.Replace("{toname}", mailData.Rows[0]["toname"].ToString())
                                            .Replace("{username}", mailData.Rows[0]["username"].ToString())
                                            .Replace("{userpassword}", optionaldataTwo)
                                            .Replace("{userrole}", mailData.Rows[0]["userrole"].ToString())
                                            .Replace("{clienturl}", adminURL);


                                        alertcontent = alertcontent.Replace("~clickhere~", "<a href='" + adminURL + "'>Click Here</a>");


                                        alertsubject = dt_alert_template.Rows[0]["alertsubject"].ToString();

                                        break;

                                    case "OTP Generator":
                                        alertcontent = alertcontent.Replace("{OTP}", optionaldataOne.ToString());
                                        alertsubject = dt_alert_template.Rows[0]["alertsubject"].ToString();

                                        break;

                                    case "User credientils":
                                        alertcontent = alertcontent.Replace("{username}", optionaldataTwo.ToString())
                                        .Replace("{password}", optionaldataOne.ToString());
                                        alertsubject = dt_alert_template.Rows[0]["alertsubject"].ToString();
                                        
                                        break;

                                    default:
                                        break;

                                }

                                alertsubject = ApplyTenantBranding(alertsubject, tenantName);
                                alertcontent = ApplyTenantBranding(alertcontent, tenantName);

                                MailBoxModel mailBox = await GetTenantMailBox(client, tenantid, applicableService);
                                Mailer objmail = mailBox == null ? null : new Mailer(mailBox);

                                string tomail = "";

                                if (entityname == "OTP")
                                {
                                    tomail = optionaldataTwo;
                                }
                                else
                                {
                                    tomail = mailData.Rows[0]["receiveremail"].ToString();
                                }

                                if (string.IsNullOrWhiteSpace(tomail)
                                    && (string.Equals(entityactionname, "registration", StringComparison.OrdinalIgnoreCase)
                                        || string.Equals(entityactionname, "forgotpassword", StringComparison.OrdinalIgnoreCase)))
                                {
                                    tomail = optionaldataOne;
                                }

                                if (tomail != "" && objmail != null)
                                {
                                    mailSent = objmail.SendMail_TLS(tomail, alertsubject, alertcontent, true, null, true);


                                    MailLogsModel objMailLogsModel = new MailLogsModel();
                                    objMailLogsModel.entityname = entityname;
                                    objMailLogsModel.entityid = entityid;
                                    objMailLogsModel.mailfor = mailfor;
                                    objMailLogsModel.mailto = tomail;
                                    objMailLogsModel.mailsubject = alertsubject;
                                    objMailLogsModel.mailbody = alertcontent;
                                    objMailLogsModel.issent = mailSent;
                                    objMailLogsModel.createduser = new Guid(loginUserID);
                                    objMailLogsModel.craftmyapp_actionmethodname = "Create_MailLog";
                                    string strMailLogsReturnMessage = await ApiClient.Post_ApiValuesGetString(client, "api/MailLogs/Create_MailLog", objMailLogsModel);
                                }

                            }
                        }
                    }

                }

            }

			return mailSent;
		}
                public async Task<bool> sendNotification(
             string entityname
            , string entityactionname
            , string entityid
            , IOptions<MailSettings> _mailSettings
            , string loginUserID
            , HttpClient client
            , string adminURL = ""
            , string tenantid = "")
        {
            bool mailSent = false;
            try
            {
                tenantid = ResolveTenantId(tenantid, entityname, entityactionname, entityid);
                string applicableService = ResolveApplicableService(entityname, entityactionname);
                string tenantName = await GetTenantName(client, tenantid, applicableService);





                mailmodel mailData = new mailmodel();

                HttpResponseMessage response_alert_maildata = await ApiClient.GET_ApiValuesGetRespnse(client, "api/MailLogs/Mailer?tenantid=" + Uri.EscapeDataString(tenantid) + "&entityname=" + entityname + "&entityactionname=" + entityactionname + "&entityid=" + entityid);



                if (response_alert_maildata.IsSuccessStatusCode)
                {
                    mailData = await response_alert_maildata.Content.ReadAsAsync<mailmodel>();
					if (string.IsNullOrWhiteSpace(tenantName))
						tenantName = mailData.tenantname?.Trim() ?? "";
                    // Rich-text templates can persist the tilde as either "~" or
                    // "&#126;" and users may vary the token's casing/spacing.
                    // Replace every supported form with the payment action link.
                    if (!string.IsNullOrEmpty(mailData.mailbody))
                    {
                        string paymentUrl = adminURL.TrimEnd('/') + "/IPDApplicationForm/Initiate_Payment?IPDApplicationFormid=" + entityid;
                        string paymentLinkText = string.Equals(entityactionname, "PaymentReminder", StringComparison.OrdinalIgnoreCase)
                            ? "Pay Now"
                            : "Click Here to Pay Booking Deposit";
                        string paymentLink = "<a href='" + paymentUrl + "' style='display:inline-block;padding:10px 20px;background-color:#3399cc;color:#ffffff;text-decoration:none;border-radius:4px;font-weight:bold;'>" + paymentLinkText + "</a>";
						mailData.mailbody = ApplyPaymentLink(mailData.mailbody, paymentLink);
                    }

                    // Replace {currentDate} with today's date
                    if (!string.IsNullOrEmpty(mailData.mailbody))
                        mailData.mailbody = mailData.mailbody.Replace("{currentDate}", DateTime.Now.ToString("dd MMM yyyy"));

                    mailData.mailsubject = ApplyTenantBranding(mailData.mailsubject, tenantName);
                    mailData.mailbody = ApplyTenantBranding(mailData.mailbody, tenantName);


                    if (mailData.mailsubject != "")
                    {


                        MailBoxModel mailBox = await GetTenantMailBox(client, tenantid, applicableService);
                        Mailer objmail = mailBox == null ? null : new Mailer(mailBox);


                        if (mailData.mailto != "" && objmail != null)
                        {
                            mailSent = objmail.SendMail_TLS(mailData.mailto, mailData.mailsubject, mailData.mailbody, true, null, true);
                        }
                    }
                }



                // A reminder becomes part of the duplicate-send history only after
                // the mail server confirms that the message was sent successfully.
                if (mailSent)
                {
                    MailLogsModel objMailLogsModel = new MailLogsModel();
                    objMailLogsModel.entityname = entityname;
                    objMailLogsModel.entityid = entityid;
                    objMailLogsModel.mailfor = entityname + " / " + entityactionname;
                    objMailLogsModel.mailto = mailData.mailto;
                    objMailLogsModel.mailsubject = mailData.mailsubject;
                    objMailLogsModel.mailbody = mailData.mailbody;
                    objMailLogsModel.issent = true;
                    objMailLogsModel.createduser = new Guid(loginUserID);
                    objMailLogsModel.craftmyapp_actionmethodname = "Create_MailLog";

                    string strMailLogsReturnMessage = await ApiClient.Post_ApiValuesGetString(client, "api/MailLogs/Create_MailLog", objMailLogsModel);
                }
            }
            catch (Exception ex)
            {

            }

            return mailSent;
        }
        public async Task<bool> sendWhatsAppNotification(
     string entityname
    , string entityactionname
    , string entityid
    , IOptions<MailSettings> _mailSettings
    , string loginUserID
    , HttpClient client)
        {
            bool mailSent = false;
            try
            {





                mailmodel mailData = new mailmodel();

                HttpResponseMessage response_alert_maildata = await ApiClient.GET_ApiValuesGetRespnse(client, "api/MailLogs/WhatsApp?entityname=" + entityname + "&entityactionname=" + entityactionname + "&entityid=" + entityid);



                if (response_alert_maildata.IsSuccessStatusCode)
                {
                    mailData = await response_alert_maildata.Content.ReadAsAsync<mailmodel>();



                    if (mailData.mailsubject != "")
                    {


                        if (mailData.mailto != "")
                        {


                            bool retValue = false;
                            try
                            {
                                string apiUrl = "http://103.212.121.131/api/v1/postmessage.ashx";
                                string bearerToken = "j30qfoeyrddqh19m1w90ryu7ttnax4jwfkjccses";
                                using (HttpClient wappclient = new HttpClient())
                                {
                                    string[] destinationNumbers = mailData.mailto.Split(',');


                                    foreach (string destNumber in destinationNumbers)
                                    {

                                        string jsonBody = $@"
                                        {{
                                            ""Destination"": ""{destNumber}"",
                                            ""body"": ""{mailData.mailbody}"",
                                            ""datetosend"": ""{DateTime.Now.ToString("yyyy-MM-dd HH:mm")}"" 
                                        }}";

                                        // Set the authorization header with the bearer token
                                        wappclient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

                                        // Set the content type
                                        HttpContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                                        // Send the POST request
                                        HttpResponseMessage response = await wappclient.PostAsync(apiUrl, content);

                                        // Check if the request was successful
                                        if (response.IsSuccessStatusCode)
                                        {
                                            // Read the response content if needed
                                            string responseBody = await response.Content.ReadAsStringAsync();
                                            LogginLib.WriteLog("SendMail Exception Message: " + responseBody);

                                            retValue = true;
                                        }
                                        else
                                        {
                                            LogginLib.WriteLog("SendMail Exception Message: " + response.StatusCode);

                                        }
                                    }
                                }


                            }
                            catch (Exception ex)
                            {
                                // LogginLib.WriteLog(ex);

                                LogginLib.WriteLog("SendMail Exception Message: " + ex.Message);
                                if (ex.InnerException != null)
                                    LogginLib.WriteLog(" SendMail Exception Inner:   " + ex.InnerException);


                            }
                            LogginLib.WriteLog("---mailer code worked--");
                            return retValue;
                        }


                    }
                }



                MailLogsModel objMailLogsModel = new MailLogsModel();
                objMailLogsModel.entityname = entityname;
                objMailLogsModel.entityid = entityid;
                objMailLogsModel.mailfor = entityname + " / " + entityactionname;
                objMailLogsModel.mailto = mailData.mailto;
                objMailLogsModel.mailsubject = mailData.mailsubject;
                objMailLogsModel.mailbody = mailData.mailbody;
                objMailLogsModel.issent = mailSent;
                objMailLogsModel.createduser = new Guid(loginUserID);
                objMailLogsModel.craftmyapp_actionmethodname = "Create_MailLog";


                string strMailLogsReturnMessage = await ApiClient.Post_ApiValuesGetString(client, "api/MailLogs/Create_MailLog", objMailLogsModel);
            }
            catch (Exception ex)
            {

            }

            return mailSent;
        }
    }
}


