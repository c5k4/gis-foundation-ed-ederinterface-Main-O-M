using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETGIS_AssetSync_Process
{
    public sealed class Email
    {
        public Email()
        {
            IsHTML = true;
            Attachments = new List<string>();
        }
        public string From { get; set; }
        public string FromDisplayName { get; set; }
        public string To { get; set; }
        public string CC { get; set; }
        public string BCC { get; set; }
        public string Subject { get; set; }
        public string BodyText { get; set; }
        public IList<string> Attachments { get; set; }
        public object Parameters { get; set; }
        public bool IsHTML { get; set; }
        public bool IsHighPriority { get; set; }
    }
}
