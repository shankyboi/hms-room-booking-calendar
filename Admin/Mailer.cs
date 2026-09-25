using Microsoft.Extensions.Options;
using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Web;
using System.IO;
using System.Linq;

public class Mailer
{
    private IOptions<MailSettings> _mailSettings;
    private NalamVazha.Models.MailBoxModel _mailBox;



    public string fromMail { get; set; }
    public string Password { get; set; }
    public string displayName { get; set; }


    public string hostName { get; set; }
    public int portNumber { get; set; }
    public bool enableSSL { get; set; }
    public bool issecured { get; set; }

    public string adminEmail { get; set; }


    public Mailer(IOptions<MailSettings> MailSettings)
    {

        _mailSettings = MailSettings;
        fromMail = _mailSettings.Value.fromMail;
        Password = _mailSettings.Value.Password;
        displayName = _mailSettings.Value.displayName;
        adminEmail = _mailSettings.Value.adminEmail;

        hostName = _mailSettings.Value.hostName;
        portNumber = _mailSettings.Value.portNumber;
        enableSSL = _mailSettings.Value.enableSSL;
        issecured = _mailSettings.Value.issecured;

    }
    public Mailer(NalamVazha.Models.MailBoxModel mailBox)
    {
        _mailBox = mailBox;
        fromMail = mailBox?.senderemail ?? "";
        Password = mailBox?.password ?? "";
        displayName = mailBox?.senderdisplayname ?? "";
        adminEmail = mailBox?.senderemail ?? "";

        hostName = mailBox?.emailhostname ?? "";
        portNumber = mailBox?.portnumber ?? 25;
        enableSSL = true;
        issecured = true;
    }
    public Mailer()
    {
    }

    public bool SendMail(string toEmails, string subject, string body, bool isHtml, string[] attachments,
            bool handleException)
    {
        if (_mailBox != null)
            return new NalamVazha.DAL.EmailService().Send(
                _mailBox, toEmails, "", subject, body, isHtml, attachments, false);
        bool retValue = false;
        try
        {

            //var client = new SmtpClient();
            var mail = new MailMessage();
            foreach (string toEmail in toEmails.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                mail.To.Add(toEmail.Trim());
            mail.Subject = subject;
            mail.SubjectEncoding = Encoding.UTF8;
            mail.Body = body;
            mail.BodyEncoding = Encoding.UTF8;
            mail.IsBodyHtml = isHtml;
            mail.Priority = MailPriority.Normal;
            mail.From = new MailAddress(fromMail, displayName);
            if (attachments != null)
                foreach (var attachmentPath in attachments.Where(path => !string.IsNullOrWhiteSpace(path) && File.Exists(path)))
                    mail.Attachments.Add(new Attachment(attachmentPath));
            //MailAddress copy = new MailAddress(adminEmail);
            // mail.Bcc.Add(copy);

            //var inlineLogo =
            //    new Attachment(ConfigurationManager.AppSettings["EmailLogo"]);



            //mail.Attachments.Add(inlineLogo);
            //const string contentId = "Image";
            //inlineLogo.ContentId = contentId;

            ////To make the image display as inline and not as attachment

            //inlineLogo.ContentDisposition.Inline = true;
            //inlineLogo.ContentDisposition.DispositionType = DispositionTypeNames.Inline;

            ////To embed image in email
            //if (mail.Body != null)
            //    mail.Body = mail.Body.Replace("{LOGOIMAGE}", " <img src=\"cid:" + contentId + "\">");

            //if (attachments != null)
            //{
            //    foreach (string filePath in attachments)
            //    {

            //        var mailAttachment = new Attachment(filePath);
            //        mail.Attachments.Add(mailAttachment);
            //    }
            //}

            LogginLib.WriteLog("---mailer code worked--");
            LogginLib.WriteLog(portNumber.ToString());
            // client.Send(mail);
            // smtp settings
            var smtp = new System.Net.Mail.SmtpClient();
            {
                smtp.Host = hostName;
                smtp.Port = portNumber;
                smtp.EnableSsl = enableSSL;
                smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                smtp.Credentials = new NetworkCredential(fromMail, Password);
                smtp.Timeout = 20000;


            }



            smtp.Send(mail);
            // Passing values to smtp object
            //smtp.Send("themoviemb@gmail.com", toEmails, subject, body);

            retValue = true;
        }
        catch (Exception ex)
        {
            retValue = false;
            LogginLib.WriteLog("Mail Exception");

            LogginLib.WriteLog(ex);

            LogginLib.WriteLog("Mail Exception");
        }
        return retValue;
    }


    public bool SendMail_TLS(string toEmails, string ccEmails, string subject, string body, bool isHtml, string[] attachments,
           bool handleException)
    {
        if (_mailBox != null)
            return new NalamVazha.DAL.EmailService().Send(
                _mailBox, toEmails, ccEmails, subject, body, isHtml, attachments);
        bool retValue = false;
        try
        {
            var mail = new MailMessage();
            foreach (string toEmail in toEmails.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                mail.To.Add(toEmail.Trim());
            if (!string.IsNullOrWhiteSpace(ccEmails))
                foreach (string cc in ccEmails.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                    mail.CC.Add(cc.Trim());
            mail.Subject = subject;
            mail.SubjectEncoding = Encoding.UTF8;
            mail.Body = body;
            mail.BodyEncoding = Encoding.UTF8;
            mail.IsBodyHtml = isHtml;
            mail.Priority = MailPriority.Normal;
            mail.From = new MailAddress(fromMail, displayName);
            if (attachments != null)
                foreach (var attachmentPath in attachments.Where(path => !string.IsNullOrWhiteSpace(path) && File.Exists(path)))
                    mail.Attachments.Add(new Attachment(attachmentPath));
            if (!string.IsNullOrWhiteSpace(adminEmail))
                mail.Bcc.Add(new MailAddress(adminEmail));

            var smtp = new System.Net.Mail.SmtpClient();
            if (issecured)
            {
                smtp.Host = hostName;
                smtp.Port = portNumber;
                smtp.EnableSsl = enableSSL;
                smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                smtp.Credentials = new NetworkCredential(fromMail, Password);
                smtp.Timeout = 20000;
            }
            else
            {
                smtp.Host = hostName;
                smtp.Port = portNumber;
                smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                smtp.Timeout = 20000;
            }

            smtp.Send(mail);
            retValue = true;
        }
        catch (Exception ex)
        {
            LogginLib.WriteLog("SendMail Exception Message: " + ex.Message);
            if (ex.InnerException != null)
                LogginLib.WriteLog(" SendMail Exception Inner:   " + ex.InnerException);
        }
        return retValue;
    }

    public bool SendMail_TLS(string toEmails, string subject, string body, bool isHtml, string[] attachments,
           bool handleException)
    {
        if (_mailBox != null)
            return new NalamVazha.DAL.EmailService().Send(
                _mailBox, toEmails, "", subject, body, isHtml, attachments);
        bool retValue = false;
        try
        {

            //var client = new SmtpClient();
            var mail = new MailMessage();
            foreach (string toEmail in toEmails.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                mail.To.Add(toEmail.Trim());
            mail.Subject = subject;
            mail.SubjectEncoding = Encoding.UTF8;
            mail.Body = body;
            mail.BodyEncoding = Encoding.UTF8;
            mail.IsBodyHtml = isHtml;
            mail.Priority = MailPriority.Normal;
            mail.From = new MailAddress(fromMail, displayName);
            if (!string.IsNullOrWhiteSpace(adminEmail))
                mail.Bcc.Add(new MailAddress(adminEmail));

            //var inlineLogo =
            //    new Attachment(ConfigurationManager.AppSettings["EmailLogo"]);



            //mail.Attachments.Add(inlineLogo);
            //const string contentId = "Image";
            //inlineLogo.ContentId = contentId;

            ////To make the image display as inline and not as attachment

            //inlineLogo.ContentDisposition.Inline = true;
            //inlineLogo.ContentDisposition.DispositionType = DispositionTypeNames.Inline;

            ////To embed image in email
            //if (mail.Body != null)
            //    mail.Body = mail.Body.Replace("{LOGOIMAGE}", " <img src=\"cid:" + contentId + "\">");

            //if (attachments != null)
            //{
            //    foreach (string filePath in attachments)
            //    {

            //        var mailAttachment = new Attachment(filePath);
            //        mail.Attachments.Add(mailAttachment);
            //    }
            //}

      
            var smtp = new System.Net.Mail.SmtpClient();

            if (issecured)
            {

                LogginLib.WriteLog("Secured");
                smtp.Host = hostName;
                smtp.Port = portNumber;
                smtp.EnableSsl = enableSSL;
                smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                smtp.Credentials = new NetworkCredential(fromMail, Password);
                smtp.Timeout = 20000;


            }
            else
            {

                LogginLib.WriteLog("Not Secured");
                smtp.Host = hostName;
                smtp.Port = portNumber;
                smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                smtp.Timeout = 20000;


            }


            smtp.Send(mail);

            // Passing values to smtp object
            //smtp.Send("themoviemb@gmail.com", toEmails, subject, body);

            retValue = true;
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


