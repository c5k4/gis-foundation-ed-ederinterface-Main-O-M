using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Common.Delivery.Systems
{
    /// <summary>
    /// 
    /// </summary>
    public class EmailFacade
    {
        /// <summary>
        /// Sends email using the given parameters
        /// </summary>
        /// <param name="smtpServer">IP Address/Machine Name of the SMTP Server</param>
        /// <param name="from">email address that will be used as the user sending the email</param>
        /// <param name="to">Comma seperated email addresses</param>
        /// <param name="subject">Subject of the message</param>
        /// <param name="message">Actual message body of the E-mail</param>
        /// <returns>True if success/False if failed sending email</returns>
        public static bool SendEmail(string smtpServer, string from, string to, string subject, string message)
        {
            try
            {
                MailMessage mailMessage = new MailMessage();
                string[] toArray = to.Split(",".ToCharArray());
                for (int i = 0; i < toArray.Length; i++)
                    mailMessage.To.Add(toArray[i]);

                // It's possible to get the following error when setting the subject.
                // ArgumentException: The specified string is not in the form required for a subject.
                // It turns out that setting the Subject on a System.Net.Mail.Message internally calls MailBnfHelper.HasCROrLF which does exactly what it says.
                // So the solution is to replace carriage return or line feeds in the subject.
                mailMessage.Subject = subject.Replace('\r', ' ').Replace('\n', ' ');
                mailMessage.From = new MailAddress(from);
                mailMessage.Body = message;

                SmtpClient smtp = new SmtpClient(smtpServer);
                //smtp.Send(mailMessage);
            }
            catch (Exception ex)
            {
                EventLogger.Warn(ex.Message + "\n" + ex.StackTrace); 
                return false;
            }

            return true;
        }

    }
}
