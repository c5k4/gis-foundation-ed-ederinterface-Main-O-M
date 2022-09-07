using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TLM.Common
{
    public class GISAttributes
    {
        public string DisplayName { get; set; }
        public string FieldName { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
       
        public string ValueDescription 
        {
            get 
            {
                if (Type.Contains("Date"))
                    return (Value.Trim().Length > 0 ? formatDate(long.Parse(Value)).ToShortDateString() : "");
                else if (lookUp == null)
                    return Value;
                else if (lookUp.ContainsKey(Value))
                    return lookUp[Value];
                else
                    return Value;
            } 
            
         }
        public Dictionary<string, string> lookUp { get; set; }

        private static DateTime formatDate(long milliSeconds)
        {
            var date = new DateTime(1970, 1, 1, 0, 0, 0);
            return date.AddMilliseconds(milliSeconds);
            //TimeSpan time = TimeSpan.FromMilliseconds(1325376000000);
        }
    }
}