using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;

namespace PGE.Interfaces.Integration.Gateway
{
    class EmailHelper
    {

        public static void SendMailToUser(string strmsg,string strinterfacename)
        {
            string Subject = default;
            string mailBody = default;
            string details = default;
            string ToLanID = ConfigurationManager.AppSettings["ToMailIds"];
            string fromLanId = ConfigurationManager.AppSettings["FromLanID"];
            string DisplayName = ConfigurationManager.AppSettings["DisplayName"];


            try
            {

                
                //
                Common._log.Info("sending email to user ");
                int flag = 0;
                

                string Attachmentsourcepath = System.Environment.GetEnvironmentVariable("USERPROFILE") + "\\Application Data\\ESRI\\PGE Integration\\log.txt";
                string Attachmenttargetpath = System.Environment.GetEnvironmentVariable("USERPROFILE") + "\\Application Data\\ESRI\\PGE Integration\\Error_Summary.txt";
                File.Copy(Attachmentsourcepath, Attachmenttargetpath);

                string responsehtml = default;
                Subject = String.Format(ConfigurationManager.AppSettings["EmailSubject"]) + " " + strinterfacename;
                mailBody = strmsg;
                mailBody = String.Format(ConfigurationManager.AppSettings["EmailBody"], "", strmsg);

                EmailService.Send(mail =>
                {
                    mail.From = fromLanId;
                    mail.FromDisplayName = DisplayName;
                    mail.To = ToLanID;
                    mail.Subject = Subject;
                    mail.BodyText = mailBody;                    
                    mail.IsHTML = true;
                    mail.Attachments.Add(Attachmenttargetpath);
                });
                Console.WriteLine("Email sent to: " + ToLanID);

            }

            catch (Exception ex)
            {
                Console.WriteLine("Error while sending email to user");
                Common._log.Error("Error while sending email to user");
            }

        }
    }
}
