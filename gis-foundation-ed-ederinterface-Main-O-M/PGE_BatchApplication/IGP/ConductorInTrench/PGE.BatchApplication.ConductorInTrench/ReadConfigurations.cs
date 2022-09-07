using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGE_DBPasswordManagement;

namespace PGE.BatchApplication.ConductorInTrench
{
    public class ReadConfigurations
    {

        public static string LOGCONFIG = System.Configuration.ConfigurationManager.AppSettings["LogConfigName"];
        // m4jf edgisrearh 919
       // public const string ConnString_EDWorkSpace = "ConnString_EDWorkSpace";
        public static string ConnString_EDWorkSpace = ReadEncryption.GetSDEPath(System.Configuration.ConfigurationManager.AppSettings["EDER_SDEConnection"].ToUpper());


       // public static string connString_edgis = "Provider=MSDAORA;Persist Secutiry Info=True;PLSQLRSet=1;"+ ReadEncryption.GetConnectionStr(System.Configuration.ConfigurationManager.AppSettings["EDER_ConnectionStr_EDGIS"].ToUpper());
        public static string connString_igpciteditor = "Provider=MSDAORA;Persist Secutiry Info=True;PLSQLRSet=1;" + ReadEncryption.GetConnectionStr(System.Configuration.ConfigurationManager.AppSettings["EDER_ConnectionStr_IGPCITEDITOR"].ToUpper());
        public static string ConnectionString_pgedata = ReadEncryption.GetConnectionStr(System.Configuration.ConfigurationManager.AppSettings["EDER_ConnectionStr_PGEDATA"].ToUpper());

        public static string ConnectionString_igpciteditor = ReadEncryption.GetConnectionStr(System.Configuration.ConfigurationManager.AppSettings["EDER_ConnectionStr_IGPCITEDITOR"].ToUpper());

        //  public static string ConnectionString_edgis = ReadEncryption.GetConnectionStr(System.Configuration.ConfigurationManager.AppSettings["EDER_ConnectionStr_EDGIS"].ToUpper());


        public const string searchDistance = "searchDistance";     
        public const string searchDistance_50 = "searchDistance_50";
        public const string searchDistance_double = "searchDistance_double"; 

        public const string linefractions = "linefractions";
        public const string numberOfPointsToBeConsidered = "numberOfPointsToBeConsidered";
        public const string logNeededWhileExecuting = "logNeededWhileExecuting";
        public const string overwriteFilledductValue = "overwriteFilledductValue";            
        
        public const string col_OBJECTID = "OBJECTID";
        public const string col_GLOBALID = "GLOBALID";
        public const string col_CIRCUITID = "CIRCUITID";
        public const string col_FILLEDDUCT = "FILLEDDUCT";      
       
        public const string col_COMMENTS = "COMMENTS";
        public const string col_DISTRICT = "DISTRICT";
        public const string col_DIVISION = "DIVISION";
        public const string col_LOCALOFFICEID = "LOCALOFFICEID";
        public const string col_COUNTY = "COUNTY";
        public const string col_SUBSTATIONID = "SUBSTATIONID";
        public const string col_SUBSTATIONNAME = "SUBSTATIONNAME";

        public const string col_STATUS = "STATUS";
        public const string col_PHASEDESIGNATION = "PHASEDESIGNATION";
        public const string col_PGE_CONDUCTORCODE = "PGE_CONDUCTORCODE";
        public const string col_FILLEDDUCT_MANUAL = "FILLEDDUCT_MANUAL";
        public const string col_FILLEDDUCT_CAPTURED = "FILLEDDUCT_CAPTURED";

        public const string col_CIT_UPDATEDON = "CIT_UPDATEDON";
        public const string col_DATEMODIFIED = "DATEMODIFIED";
        public const string col_LASTUSER = "LASTUSER";
        public const string col_PROCESSED_ON = "PROCESSED_ON";
        public const string col_REQUESTED_ON = "REQUESTED_ON";
        public const string col_REQUESTED_BY = "REQUESTED_BY";
        public const string col_SUBTYPECD = "SUBTYPECD";
        public const string col_CODE = "CODE";
        public const string col_DESCRIPTION = "DESCRIPTION";

        public const string col_VERSION_NAME = "VERSION_NAME";     

        public const string col_Exclusion = "EXCLUSION";
        public const string col_Is50Scale = "IS50SCALE";
        public const string col_OIDS_Needed = "OIDS_NEEDED";
        public const string col_OIDS_All = "OIDS_ALL";
        public const string col_WEIGHT = "WEIGHT";
        public const string col_CONDUITTYPE = "CONDUIT_TYPE";
        public const string col_USER = "USER";

        public const string col_EMAILADDRESS = "EMAILADDRESS";
        public const string col_WEBRURL = "WEBRURL";
        public const string col_PROCESSED = "PROCESSED";

        public const string col_CIT_LASTMAILSENT = "CIT_LASTMAILSENT";   

        public const string val_SYSTEM = "SYSTEM";
        public const string val_New = "New";
        

        public const string process_DATAUPDATE = "DATAUPDATE";
        public const string process_VERSIONPOST = "VERSIONPOST";
        public const string process_type_SCHEDULE = "SCHEDULE";
        public const string process_type_BULK = "BULK";

        public const string dateFormat_Database = "dd-MMM-yy";           

        public const string val_SendMailDaysDifference = "SendMailDaysDifference";
        public const string val_SendMailDay = "SendMailDay";
        public const string val_StatusInService = "5";


        public const string subFields = "GLOBALID,OBJECTID,SHAPE,STATUS,CIRCUITID,PHASEDESIGNATION,SUBTYPECD,LOCALOFFICEID,COUNTY";

        public const string strExclusionBasedOnConductorCodes = "ExclusionBasedOnConductorCodes";
        public const string strExclusionBasedOnStatus = "ExclusionBasedOnStatus";
        public const string strExclusionBasedOnLength = "ExclusionBasedOnLength";   
        
        public const string cutoffLengthConductorToExclude = "cutoffLengthConductorToExclude";        
        
        #region Table Names 
             
        public const string tableNameToSavePriUGException = "tableNameToSavePriUGException";          
        public const string tableNameToSaveFinalData = "tableNameToSaveFinalData";
        public const string tableNameChangeDetection = "tableNameChangeDetection";
        public const string tableNameForManualUpdates = "tableNameForManualUpdates";
        public const string tableNameToSaveVersionData = "tableNameToSaveVersionData";

        public const string tableNameForConflictInformation = "tableNameForConflictInformation";
        
     
        public const string tableNameallPriUGCond_Rule1_FD = "tableNameallPriUGCond_Rule1_FD";
        public const string tableNameallPriUGCond_50Scale = "tableNameallPriUGCond_50Scale";

        public const string tableNameForConductorCodes = "tableNameForConductorCodes";    

        #endregion Table Names     

        #region queries constants       

        public const string queryToGetConduitSystemDuctBankData = "queryToGetConduitSystemDuctBankData";   
        public const string queryToGetRule1Records = "queryToGetRule1Records";
        public const string queryToGetVersionName = "queryToGetVersionName";
        

        #endregion queries constants

        #region Geodatabase Constants

        public const string FC_fifty_Scale_Boundary = "EDGIS.PGE_50SCALEBOUNDARY";
        public const string FC_PRIUGCONDUCTOR = "EDGIS.PRIUGCONDUCTOR";
        public const string FC_CONDUITSYSTEM = "EDGIS.CONDUITSYSTEM";

        public const string TB_PRIUGCONDUCTORINFO = "EDGIS.PRIUGCONDUCTORINFO";
        public const string TB_CODESDESCRIPTION = "EDGIS.PGE_CODES_AND_DESCRIPTIONS";
        public const string TB_CIRCUITSOURCE = "EDGIS.CIRCUITSOURCE";
        public const string TB_CIT_PGE_LASTMAILSENT = "PGEDATA.CIT_PGE_LASTMAILSENT";        

        #endregion Geodatabase Constants

        public const string defaultVersionName = "SDE.DEFAULT";

        public const string ConductorLengthToCalculateParallelLengthIsGreaterThanCutOffLength = "ConductorLengthToCalculateParallelLengthIsGreaterThanCutOffLength";
        
        
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
