namespace NalamVazha.DAL
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Mail;
    using System.Text;
    using NalamVazha.Models;

    public interface IEmailService
    {
        bool Send(MailBoxModel mailBox, string toEmails, string ccEmails, string subject,
            string body, bool isHtml, string[] attachments, bool bccSender = true);
    }

    /// <summary>Shared SMTP sender used by both HTTP-triggered mail and background jobs.</summary>
    public sealed class EmailService : IEmailService
    {
        public bool Send(MailBoxModel mailBox, string toEmails, string ccEmails, string subject,
            string body, bool isHtml, string[] attachments, bool bccSender = true)
        {
            if (mailBox == null || string.IsNullOrWhiteSpace(mailBox.senderemail) ||
                string.IsNullOrWhiteSpace(mailBox.emailhostname))
                return false;

            try
            {
                using var message = new MailMessage
                {
                    From = new MailAddress(mailBox.senderemail, mailBox.senderdisplayname),
                    Subject = subject ?? "",
                    SubjectEncoding = Encoding.UTF8,
                    Body = body ?? "",
                    BodyEncoding = Encoding.UTF8,
                    IsBodyHtml = isHtml,
                    Priority = MailPriority.Normal
                };

                AddAddresses(message.To, toEmails);
                AddAddresses(message.CC, ccEmails);
                if (message.To.Count == 0) return false;

                if (bccSender)
                    message.Bcc.Add(new MailAddress(mailBox.senderemail));

                if (attachments != null)
                {
                    foreach (string path in attachments.Where(path =>
                        !string.IsNullOrWhiteSpace(path) && File.Exists(path)))
                        message.Attachments.Add(new Attachment(path));
                }

                using var smtp = new SmtpClient
                {
                    Host = mailBox.emailhostname,
                    Port = mailBox.portnumber <= 0 ? 25 : mailBox.portnumber,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(mailBox.senderemail, mailBox.password),
                    Timeout = 20000
                };
                smtp.Send(message);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void AddAddresses(MailAddressCollection collection, string addresses)
        {
            if (string.IsNullOrWhiteSpace(addresses)) return;
            foreach (string address in addresses.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                string trimmed = address.Trim();
                if (!string.IsNullOrWhiteSpace(trimmed)) collection.Add(trimmed);
            }
        }
    }
}
