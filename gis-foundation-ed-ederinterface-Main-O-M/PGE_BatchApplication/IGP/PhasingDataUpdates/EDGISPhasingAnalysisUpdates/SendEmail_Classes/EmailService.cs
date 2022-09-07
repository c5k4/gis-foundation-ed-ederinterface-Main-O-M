using System;
using System.Net.Mail;

namespace PGE.BatchApplication.IGPPhaseUpdate
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
                    if (!string.IsNullOrEmpty(path))
                    {
                        try
                        {
                            message.Attachments.Add(new Attachment(path));
                        }
                        catch { }
                    }
                }

                foreach (string address in email.To.Split(';'))
                {
                    if (address.Length == 0)
                    {
                        continue;
                    }

                    message.To.Add(new MailAddress(address + "@pge.com"));
                }

                message.Subject = email.Subject;
                message.Body = email.BodyText;
                message.Priority = email.IsHighPriority ? MailPriority.High : MailPriority.Normal;
                smtpClient.Send(message);
            }
            catch { }
        }

    }
}
