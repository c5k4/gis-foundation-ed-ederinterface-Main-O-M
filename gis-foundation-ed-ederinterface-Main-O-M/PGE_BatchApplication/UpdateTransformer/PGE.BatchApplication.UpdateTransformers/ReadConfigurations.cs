using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.BatchApplication.UpdateFeatures
{
    public class ReadConfigurations
    {

        public static string LOGCONFIG = System.Configuration.ConfigurationManager.AppSettings["LogConfigName"];

        //public const string EDConnection = "EDConnection";

        // M4JF EDGISREARC 919 
        //public const string EDWorkSpaceConnString = "EDWorkSpaceConnString";
        public const string EDWorkSpaceConnString = "EDER_SDEConnection";
        public const string EDConnectionString = "EDER_ConnectionStr";
        public const string EDConnection_Str = "EDER_ConnectionStr_RELEDITOR";

        public const string EDWorkSpaceConnString_sde = "EDWorkSpaceConnString_sde";
        public const string ServiceLocationClassName = "ServiceLocationClassName";
        public const string TransformerClassName = "TransformerClassName";
        public const string TransformerUnitClassName = "TransformerUnitClassName";

        public const string TargetVersionName = "TargetVersionName";
        public const string VersionNamePrefix = "VersionNamePrefix";
        public const string DefaultVersionName = "DefaultVersionName";
        public const string SessionName = "SessionName";
        // m4jf edgisrearc 919 - commeneted below lines as username and password will not be available in config as part of edgis rearch improvement
        //public const string SMUser = "SMUser";
        //public const string SMUPWD = "SMUPWD";
        //public const string SMDB = "SMDB";

        public const string Col_GLOBALID = "Col_GLOBALID";
        public const string Col_GENCATEGORY = "Col_GENCATEGORY";
        public const string Col_LABELTEXT = "Col_LABELTEXT";
        public const string Col_VERSIONNAME = "Col_VERSIONNAME";

        public const string minValue = "minValue";
        public const string maxValue = "maxValue";
        public const string multiplier = "multiplier";

        public const string QueryToGetRecords = "QueryToGetRecords";

        public const string QueryToGetTrUnitRecords = "QueryToGetTrUnitRecords";        

        public const string localOfficeQueryValue = "localOfficeQueryValue";
        public const string SessionNumberStartValue = "SessionNumberStartValue";

        //public const string connString = "connString";
        public const string connStringreleditor = "connStringreleditor";
        public const string PhaseVerifiedValue = "PhaseVerifiedValue";
        

        public static string GetValue(string key)
        {
            string setting = null;
            try
            {
                setting = System.Configuration.ConfigurationManager.AppSettings[key];
            }
            catch { }
            return setting;
        }

        public static string[] GetCommaSeparatedList(string Key, string[] Default)
        {
            string[] output = Default;
            string value = GetValue(Key);
            if (value != null)
            {
                try
                {
                    string[] temp = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    output = new string[temp.Length];
                    for (int i = 0; i < temp.Length; i++)
                    {
                        output[i] = temp[i].Trim();
                    }

                }
                catch { }
            }
            return output;
        }
    }
}
