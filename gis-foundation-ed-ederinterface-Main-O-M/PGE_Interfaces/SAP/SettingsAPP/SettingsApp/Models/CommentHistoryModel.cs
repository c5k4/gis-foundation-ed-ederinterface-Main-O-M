using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SettingsApp.Models
{
    public class CommentHistoryModel
    {
        public List<Common.History> CommentHistory { get; set; }
        public string GlobalID { get; set; }
        public string comments { get; set; }
        public bool IsAdmin { get; set; }
    }
}