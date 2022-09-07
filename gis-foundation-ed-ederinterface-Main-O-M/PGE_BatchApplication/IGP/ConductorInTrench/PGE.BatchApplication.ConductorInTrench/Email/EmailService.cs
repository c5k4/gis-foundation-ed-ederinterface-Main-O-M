using System;
using System.Net.Mail;
using System.Configuration;
using System.IO;

namespace PGE.BatchApplication.ConductorInTrench
{
    public class SmtpClientWrapper
    {
        private readonly SmtpClient client = new SmtpClient();

        public SmtpClientWrapper()
        {
            client.Host = ConfigurationManager.AppSettings["clientHost"]; 
        }

        public virtual void Send(MailMessage message)
        {
            client.Send(message);
            message.Attachments.Dispose();
        }
    }

    public class EmailService
    {
        public static bool Send(Action<Email> argExp)
        {
            string fileName = null;
            string mailContent = null;
            bool bMailSentSuccess = false;
            try
            {
                Email email = new Email();
                argExp.Invoke(email);
                SmtpClientWrapper smtpClient = new SmtpClientWrapper();
                
                MailMessage message = new MailMessage();
                message.IsBodyHtml = email.IsHTML;
                message.From = new MailAddress(email.From.Trim(), email.FromDisplayName);

                mailContent = Common._mailContent;
                fileName = ConfigurationManager.AppSettings["fileName"] + "_" + DateTime.Today.Year + "-" + DateTime.Today.Month + "-" + DateTime.Today.Day + ConfigurationManager.AppSettings["fileExtension"];
               
                try
                {
                    string fileNameToSave = ConfigurationManager.AppSettings["fileName"] + ConfigurationManager.AppSettings["fileExtension"];
                    string filePathToSave = System.AppDomain.CurrentDomain.BaseDirectory + fileNameToSave;
                    File.WriteAllText(filePathToSave, mailContent);
                }
                catch (Exception exp)
                {
                    Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                }               

                message.Attachments.Add(Attachment.CreateAttachmentFromString(mailContent, fileName));
                
                foreach (string address in email.To.Split(','))
                {
                    if (address.Length == 0)
                    {
                        continue;
                    }

                    string address_trim = address.Trim();

                    message.To.Add(new MailAddress(address_trim));
                }

                message.Subject = email.Subject;
                message.Body = email.BodyText;
                message.Priority = email.IsHighPriority ? MailPriority.High : MailPriority.Normal;
                smtpClient.Send(message);
                bMailSentSuccess = true;
            }
            catch (Exception ex)
            {
                bMailSentSuccess = false;
                Common._log.Error("Exception : " + ex.Message + " |  Stack Trace | " + ex.StackTrace);
                throw ex;
            }
            return bMailSentSuccess;
        }
    }
}
