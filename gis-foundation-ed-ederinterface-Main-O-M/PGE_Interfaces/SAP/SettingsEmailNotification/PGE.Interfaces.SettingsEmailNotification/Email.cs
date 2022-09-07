using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Configuration;
using System.Reflection;
using Diagnostics = PGE.Common.Delivery.Diagnostics;

namespace PGE.Interfaces.SettingsEmailNotification
{
    public class Email
    {
        private static readonly Diagnostics.Log4NetLogger _logger = new Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "SettingsEmailNotification.log4net.config");

        public List<string> To { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string LocalOffice { get; set; }
        public string District { get; set; }
        public string Division { get; set; }

        /// <summary>
        /// Sends email
        /// </summary>
        /// <returns></returns>
        public bool SendMail()
        {
            bool success = true;
            try
            {
                string smtpServer = ConfigurationManager.AppSettings["SMTPServer"];                 //SMTPHelper.SMTPServer;
                string fromUser = this.From;                                                        //SMTPHelper.MailFromUser;
                //string reciepientEmailAddress = this.To;

                if (!string.IsNullOrEmpty(smtpServer) && !string.IsNullOrEmpty(fromUser) && this.To.Count > 0)
                {
                    MailMessage message = new MailMessage();
                    message.IsBodyHtml = true;
                    foreach (string emailAddress in this.To)
                    {
                        message.To.Add(new MailAddress(emailAddress));
                    }
                    message.From = new MailAddress(fromUser);
                    message.Subject = this.Subject; 
                    message.Body = this.Body;
                    SmtpClient client = new SmtpClient(smtpServer);
                    client.Send(message);
                }
                else
                {
                    if (string.IsNullOrEmpty(smtpServer))
                    {
                        _logger.Error("SMTP server not found");
                        throw new Exception("SMTP details missing");
                    }

                    if (string.IsNullOrEmpty(fromUser))
                    {
                        _logger.Error("Sender is not available in config file");
                        throw new Exception("SMTP details missing");
                    }

                    if (this.To.Count == 0)
                    {
                        _logger.Info("Mailing address is not available for local office: " + this.LocalOffice + " district: " + District + " division:" + Division);
                    }
                } 
            }
            catch (Exception ex)
            {
                _logger.Error("PGE.Interfaces.SettingsEmailNotification.Email.SendMail() - " + ex.Message);
                success = false;
                throw ex;
            }

            return success;
        }
    }
}
