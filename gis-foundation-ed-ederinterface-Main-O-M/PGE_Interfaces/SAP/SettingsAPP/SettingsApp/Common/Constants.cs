using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace SettingsApp.Common
{
    public class Constants
    {

        public static string prefix = ConfigurationManager.AppSettings["Environment"];
        private static string NASServer = ConfigurationManager.AppSettings[string.Concat(prefix, "_", "DocumentumNAS")];
        public static string ADDomain = ConfigurationManager.AppSettings["Domain"];
        public static string GISServiceURI = ConfigurationManager.AppSettings[string.Concat(prefix, "_", "GISService")];
        public static string GISSubStationServiceURI = ConfigurationManager.AppSettings[string.Concat(prefix, "_", "GISSubStationService")];
        public static string GISArcFMURI = string.Concat(GISServiceURI, "/exts/ArcFMMapServer/id");
        public static string GISDomainURL = string.Concat(GISServiceURI, "/layers");
        public static string DocumentumServiceURI = ConfigurationManager.AppSettings[string.Concat(prefix, "_", "DocumentumService")];
        public static string Documentum_File_Path = ConfigurationManager.AppSettings[string.Concat(prefix, "_", "DocumentumFilePath")];
        public static string Documentum_File_Drop = string.Concat(NASServer);
        
        public static string Documentum_Tracking_SourceSystemName = "EDGIS";
        public static string Documentum_Tracking_CreationMatrixKey = "ELDS";
        public static string Documentum_Tracking_SecurityClassification = "PG&E Internal";
        public static string DocumentumURL = string.Concat(ConfigurationManager.AppSettings[string.Concat(prefix, "_", "DocumentumURL")],"{0}/{1}/{2}/{3}/{4}");
        public static bool LoggingEnabled = bool.Parse(ConfigurationManager.AppSettings[string.Concat(prefix, "_", "EnableLogging")]);

        public static string Admin_Group_Name = ConfigurationManager.AppSettings[string.Concat(prefix, "_", "AdminGroupName")];
        //INC000004403314
        public static string Admin_Group_Name_Electric_Operations = ConfigurationManager.AppSettings[string.Concat(prefix, "_", "AdminGroupName_Electric_Operations")];
        //ENOS2EDGIS
        public static string Super_User_Group_Name = ConfigurationManager.AppSettings[string.Concat(prefix, "_", "SuperUserGroupName")];
        /* HC - capitalize the operating #, district, division*/
        //TODO:
        public static string BuildDocumentumUrl(bool peerReview,string division, string district, string deviceName, string operatingNumber)
        {
            division = division.ToUpper() ?? division;
            district = district.ToUpper() ?? district;
            deviceName = deviceName.ToUpper() ?? deviceName;
            deviceName = deviceName.Replace("TRIPSAVER", "RECLOSER");
            operatingNumber = operatingNumber.ToUpper() ?? operatingNumber;
            if(peerReview)
                return string.Format(Constants.DocumentumURL, "Device Settings-Peer Review", division.Replace("/", "-"), district.Replace("/", "-"), deviceName, operatingNumber);
            else
                return string.Format(Constants.DocumentumURL, "Device Settings-Settings Configuration", division.Replace("/", "-"), district.Replace("/", "-"), deviceName, operatingNumber);
        }

        public const string FileUploadError = "Request cannot be processed. Please try again or contact IT Support.";   //INC000004113163
        public const string SelectFileError = "Please select a file to upload.";
        public const string FileContentError = "File selected is empty! Please select a file with content to upload.";

        #region "Error Messages"

        public static string GetNoDeviceFoundError(string deviceType, string guidID)
        {
            System.Text.StringBuilder oError = new System.Text.StringBuilder();
            //oError.Append("The device requested does not exist in settings. There are a few known cases that can cause this:").Append("<br /><br />");
            oError.Append("#1: Some Device Types do not support settings, yet because of a limitation in webr have a More Info option in the gis: ").Append("<br/>");
            oError.Append("Note: List of those devices: 'Fixed Bank Capacitor'").Append("<br/><br/>");
            oError.Append("#2 Confirm that the device exists in GIS ").Append("<br/>");
            oError.Append("Note: If the device does not exist in GIS, please ask the mapping department to add it.").Append("<br/><br/>");
            oError.Append("#3 Confirm that the device in GIS has a valid District, Division, Operating Number and Bank Code. ").Append("<br/>");
            oError.Append("Note: If the device exists in GIS but the district, division or operating number are blank, please ask your mapping department  to have the device fixed.  ").Append("<br/><br/>");
            oError.Append("If the above is all correct and the device is not found, please contact PGE IT to check the settings logs for this object. Full information supplied was: ").Append("<br/>");
            oError.Append("Device Type: ").Append(deviceType).Append("<br/>");
            oError.Append("GUID: ").Append(guidID);

            return oError.ToString();
        
        }
        #endregion
    }
}