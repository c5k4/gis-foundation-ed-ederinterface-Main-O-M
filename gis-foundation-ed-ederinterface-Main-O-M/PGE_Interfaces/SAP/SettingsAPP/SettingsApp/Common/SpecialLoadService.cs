using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SettingsApp.Common
{
    public class SpecialLoadService
    {
        public static string GetValueFromAttributes(Dictionary<string, string> attributeValues,string KEY)
        {
            var ObjectId = string.Empty;
            if (attributeValues.ContainsKey(KEY))
            {
                if(attributeValues[KEY]!=null)  ObjectId = Convert.ToString(attributeValues[KEY]);
            }

            return ObjectId;
        }

        
        
    }
}