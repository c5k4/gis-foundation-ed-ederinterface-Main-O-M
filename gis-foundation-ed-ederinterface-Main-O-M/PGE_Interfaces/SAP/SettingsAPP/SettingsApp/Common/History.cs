using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SettingsApp.Common
{
    public class History
    {
        public decimal ID { set; get; }
        public string GlobalID { set; get; }
        public string DeviceTableName { set; get; }
        public string DeviceHistoryTableName { set; get; }
        public System.DateTime? WorkDate { set; get; }
        public string WorkType { set; get; }
        public System.DateTime? EntryDate { set; get; }
        public string Note { set; get; }
        public string PerformedBy { set; get; }
        public decimal? HistoryID { get; set; }
        public bool UserIsAdmin { set; get; }
    }
}