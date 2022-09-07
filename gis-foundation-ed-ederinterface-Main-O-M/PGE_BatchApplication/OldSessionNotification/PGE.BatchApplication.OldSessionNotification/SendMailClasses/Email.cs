// ========================================================================
// Copyright © 2021 PGE.
// <history> 
// This class represents an EMail to be sent.(EDGISREARCH - 378)
// TCS M4JF 04/07/2021 Created
// </history>
// All rights reserved.
// ========================================================================
using System.Collections.Generic;

namespace PGE.BatchApplication.OldSessionNotification
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



