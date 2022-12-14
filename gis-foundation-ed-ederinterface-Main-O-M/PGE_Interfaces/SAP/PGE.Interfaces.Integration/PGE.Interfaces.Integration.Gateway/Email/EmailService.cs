using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;

namespace PGE.Interfaces.Integration.Gateway
{
    public class SmtpClientWrapper
    {
        private readonly SmtpClient client = new SmtpClient();

        public SmtpClientWrapper()
        {
            //client.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;
            //client.Credentials = CredentialCache.DefaultNetworkCredentials;
        }

        public virtual void Send(MailMessage message)
        {
            client.Send(message);
            message.Attachments.Dispose();
        }
    }

    public class EmailService
    {
        public static void Send(Action<Email> exp)
        {
            Email email = new Email();
            exp.Invoke(email);
            SmtpClientWrapper smtpClient = new SmtpClientWrapper();
            MailMessage message = new MailMessage();
            message.IsBodyHtml = email.IsHTML;
            message.From = new MailAddress(email.From.Trim(), email.FromDisplayName);

            foreach (string path in email.Attachments)
            {
                message.Attachments.Add(new Attachment(path));
            }

            foreach (string address in email.To.Split(';'))
            {
                if (address.Length == 0)
                {
                    continue;
                }

                message.To.Add(new MailAddress(address));
            }

            message.Subject = email.Subject;
            message.Body = email.BodyText;
            message.Priority = email.IsHighPriority ? MailPriority.High : MailPriority.Normal;
            smtpClient.Send(message);
        }
    }
}
