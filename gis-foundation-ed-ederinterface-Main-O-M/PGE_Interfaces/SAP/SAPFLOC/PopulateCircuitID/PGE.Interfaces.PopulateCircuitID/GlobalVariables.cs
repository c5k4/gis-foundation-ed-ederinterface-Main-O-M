using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using PGE_DBPasswordManagement;

namespace PGE.Interface.PopulateCircuitID
{
    public static class GlobalVariables
    {
        //Define Config Parameters
        // m4jf edgisreach 919 - get sde connection using PGE_DBPasswordManagement

        //public static  string EderConnectionString = ConfigurationManager.AppSettings["EDER_CONN_SDE_FILE"];
        //public static string WberConnectionString = ConfigurationManager.AppSettings["WIP_CONN_SDE_FILE"];

        public static string EderConnectionString = ReadEncryption.GetSDEPath(ConfigurationManager.AppSettings["EDER_SDEConnection"].ToUpper());
        public static string WberConnectionString = ReadEncryption.GetSDEPath(ConfigurationManager.AppSettings["WIP_SDEConnection"].ToUpper());



        public static string sInputCSVFileName=ConfigurationManager.AppSettings["INPUT_CSVNAME"];
        public static string sOUtPutFilePath = ConfigurationManager.AppSettings["OUTPUT_FILEPATH"];
        public static string sLogFilePath = ConfigurationManager.AppSettings["LOGPATH"];
        public static string sCSVField_WorkOrder = ConfigurationManager.AppSettings["CSV_FIELD_WORKORDER"];
        public static string sRunFromCSV = ConfigurationManager.AppSettings["RUN_FROM_CSV"];
        public static string sTriggerFilePath = ConfigurationManager.AppSettings["OUTPUT_FILEPATH"].ToString() + "\\" + ConfigurationManager.AppSettings["TRIGGER_FILENAME"].ToString();
     
        //Define Conductor FeatureClass Name
        public static string sPriOHConductorFeatureClassName =  "EDGIS.PriOHConductor";
        public static string sPriUGConductorFeatureClassName =  "EDGIS.PriUGConductor";
        public static string sSecOHConductorFeatureClassName =  "EDGIS.SecOHConductor";
        public static string sSecUGConductorFeatureClassName = "EDGIS.SecUGConductor";

        //Define Conductor Fields Name
        public static string sConductorObjectIdFieldName = "OBJECTID";
        public static string sConductorCircuitIDFieldName = "CIRCUITID";

        //Define Wip Polygon FeatureClass Name
        public static string sWIPPolygonFeatureclassName = "WEBR.WIP_SPVW";

        //Define WIP Changes Table Name
        public static string sWIPChangesTableName = "EDGIS.PGE_WIPCHANGES";
        //Define PGE WorkOrder Table Name
        public static string sPGEWorkOderTableName = "WEBR.PGE_PMORDER";

        public static string sInstallJobNumberFieldName = "INSTALLJOBNUMBER";

        //Define Output File Field Name
         public static string sInstallJobNumberOutputFieldName = "INSTALLJOBNUMBER";
         public static string sCircuitIDOutputFieldName = "CIRCUITID";
 
        public static double MaximumBufferDistance = Convert.ToDouble(ConfigurationManager.AppSettings["MAX_BUFFER_DISTANCE"].ToString());
        public static string WIP_OBJECTID = "WIP_OBJECTID";

    }
}
