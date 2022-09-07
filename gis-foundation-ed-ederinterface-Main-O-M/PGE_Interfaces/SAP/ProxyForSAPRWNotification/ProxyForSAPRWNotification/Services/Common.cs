using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Web;
using PGE.Interfaces.ProxyForSAPRWNotification.Models;

namespace PGE.Interfaces.ProxyForSAPRWNotification.Services
{
    public class Common
    {

        public void SendEmailtoUser(HttpRequestMessage request, string notificationId, string lanId,string submitDate,string error)
        {
            if (request == null) throw new ArgumentNullException("request");
            string mailFrom = ConfigurationManager.AppSettings["MailFrom"];
            string mailTo = ConfigurationManager.AppSettings["MailTo"];
            MailMessage mail = new MailMessage(mailFrom, mailTo);
            SmtpClient client = new SmtpClient
            {
                Port = 25,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Host = "mailhost.utility.pge.com"
            };
            MailAddress mailAddressCc = new MailAddress(lanId + "@pge.com");
            mail.CC.Add(mailAddressCc);
            mail.Subject = ConfigurationManager.AppSettings["MailSubject"];
            string mailbody = ConfigurationManager.AppSettings["MailBody"] + removeBracket(notificationId) + "<b>" + "</b>" + " has failed. Please Check. ";
            mail.IsBodyHtml = true;
            mail.BodyEncoding = System.Text.Encoding.UTF8;
            mail.Body = "Dear Support Team, <br><br>" + mailbody + "<br><br>"
                        + "Notification_ID: " + "<b>" + "</b>" + removeBracket(notificationId) + "<br><br>"
                        + "Submit Date: " + "<b>" + "</b>" + submitDate + "<br><br>"
                        + "Error: " + "<b>" + "</b>" + error;

            client.Send(mail);
        }

        private string removeBracket(string notificationId )
        {
            notificationId = notificationId.Replace("{", "");
            notificationId = notificationId.Replace("}", "");
            return notificationId;
        }
    }
}