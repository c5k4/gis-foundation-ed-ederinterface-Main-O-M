// ========================================================================
// Copyright © 2021 PGE.
// <history> 
// Email Service. (EDGISREARCH - 378)
// TCS M4JF 04/07/2021 Created
// </history>
// All rights reserved.
// ========================================================================
using System;
using System.Net.Mail;

namespace PGE.BatchApplication.OldSessionNotification
{
           
    public class SmtpClientWrapper
    {
        
        private readonly SmtpClient client = new SmtpClient();

        public SmtpClientWrapper()
        {
            //client.Host = "mailhost.comp.pge.com";  
        }

        public virtual void Send(MailMessage message)
        {
            
                client.Send(message);
                message.Attachments.Dispose();
        }
    }

    public class EmailService
    {
        public static Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, ReadConfigurations.LOGCONFIG);

        /// <summary>
        /// Send Email .
        /// </summary>
        /// <param name="exp"></param>
        public static void Send(Action<Email> exp)
        {
            try
            {
                Email email = new Email();
                exp.Invoke(email);
                SmtpClientWrapper smtpClient = new SmtpClientWrapper();
                MailMessage message = new MailMessage();
                message.IsBodyHtml = email.IsHTML;
                message.From = new MailAddress(email.From.Trim(), email.FromDisplayName);

                foreach (string path in email.Attachments)
                {
                    // Add attachment to email.
                    message.Attachments.Add(new Attachment(path));
                }

                foreach (string address in email.To.Split(';'))
                {
                    if (address.Length == 0)
                    {
                        continue;
                    }

                    message.To.Add(new MailAddress(address + "@pge.com"));
                }

                foreach(string ccAddress in email.CC.Split(';'))
                {
                    if (ccAddress.Length == 0)
                    {
                        continue;
                    }
                   
                    message.CC.Add(new MailAddress(ccAddress + "@pge.com"));
                }

                message.Subject = email.Subject;
                message.Body = email.BodyText;
                message.Priority = email.IsHighPriority ? MailPriority.High : MailPriority.Normal;
                smtpClient.Send(message);
            }
            catch(Exception ex)
            {
                _log.Error(ex.Message);

            }
        }

    }
}
