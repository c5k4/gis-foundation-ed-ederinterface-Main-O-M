using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using PGE_DBPasswordManagement;

namespace PGE.Interfaces.SAP.ExportToSAP
{
    class ReadConfigurations
    {
       
        #region Global Variables

       
        public static string EDER_CONNECTIONSTRING = string.Empty;
        
        public static string OUTPUT_FILE = string.Empty;
       
        public static string DATE_FORMAT = string.Empty;
        
        public static string LOGCONFIG = ConfigurationManager.AppSettings["LogConfigName"];
        
        
        public static string Flag = string.Empty;
       
        public static string SETTINGS_CONNECTIONSTRING = string.Empty;

       
        public static string Proc_Unique_Records = string.Empty;
        public static string Output_File_Archive = string.Empty;
       
        public static string Qry_Clear_Curr_Data = string.Empty;
        public static string PGE_STG2_GUID_UPDT_STG1_SP = string.Empty;
       
        public static string SP_EDER_TO_SAP_STATUS_INSERT = string.Empty;
        public static string Col_GEN_GUID = string.Empty;
        public static string Col_SPGUID = string.Empty;
        public static string Col_SPID = string.Empty;
        public static string Col_CGC12 = string.Empty;
        public static string Col_CID = string.Empty;
        public static string Col_Updated_Field = string.Empty;
        public static string Col_Updated_Table = string.Empty;
        public static string Col_FeatureGUID = string.Empty;
        public static string Col_Postdate = string.Empty;
        public static string Col_Comments = string.Empty;
        
        public static string FieldsToInsert_EDGIS_GIS_CHANGES_SAP = ConfigurationManager.AppSettings["FieldsToInsert_EDGIS_GIS_CHANGES_SAP"];  
        
        public static string Qry_Fetch_EqpId_Frm_Settings = string.Empty;
        public static string Qry_Select_EDER_TO_SAP = string.Empty;
        public static string Qry_Update_EDER_TO_SAP = string.Empty;
        public static string Export_Header = string.Empty;
        public static string VersionOperationRetryCount = string.Empty;
        #endregion


        public static void ReadFromConfiguration()
        {
            try
            {
                // m4jf edgisrearch 919 
                //EDER_CONNECTIONSTRING = ConfigurationManager.AppSettings["EDER_ConnectionString"];
                EDER_CONNECTIONSTRING = ReadEncryption.GetConnectionStr(ConfigurationManager.AppSettings["EDER_ConnectionStr"].ToUpper());
                //SETTINGS_CONNECTIONSTRING = ConfigurationManager.AppSettings["Settings_ConnectionString"];

                SETTINGS_CONNECTIONSTRING = ReadEncryption.GetConnectionStr(ConfigurationManager.AppSettings["EDAUX_ConnectionStr"].ToUpper());
                Export_Header = ConfigurationManager.AppSettings["Export_Header"];

                DATE_FORMAT = ConfigurationManager.AppSettings["Date_Format"];

                Qry_Clear_Curr_Data = ConfigurationManager.AppSettings["Qry_Clear_Curr_Data"];

                Flag = ConfigurationManager.AppSettings["Flag"];

                Output_File_Archive = ConfigurationManager.AppSettings["Output_File_Archive"];
                OUTPUT_FILE = ConfigurationManager.AppSettings["Output_File"];

                Proc_Unique_Records = ConfigurationManager.AppSettings["Proc_Unique_Records"];

                PGE_STG2_GUID_UPDT_STG1_SP = ConfigurationManager.AppSettings["PGE_STG2_GUID_UPDT_STG1_SP"];

                SP_EDER_TO_SAP_STATUS_INSERT = ConfigurationManager.AppSettings["SP_EDER_TO_SAP_STATUS_INSERT"];
                Col_GEN_GUID = ConfigurationManager.AppSettings["Col_GEN_GUID"];
                Col_SPGUID = ConfigurationManager.AppSettings["Col_SPGUID"];
                Col_SPID = ConfigurationManager.AppSettings["Col_SPID"];
                Col_CGC12 = ConfigurationManager.AppSettings["Col_CGC12"];
                Col_CID = ConfigurationManager.AppSettings["Col_CID"];
                Col_Updated_Field = ConfigurationManager.AppSettings["Col_Updated_Field"];
                Col_Updated_Table = ConfigurationManager.AppSettings["Col_Updated_Table"];
                Col_FeatureGUID = ConfigurationManager.AppSettings["Col_FeatureGUID"];
                Col_Postdate = ConfigurationManager.AppSettings["Col_Postdate"];
                Col_Comments = ConfigurationManager.AppSettings["Col_Comments"];

                
                Qry_Fetch_EqpId_Frm_Settings = ConfigurationManager.AppSettings["Qry_Fetch_EqpId_Frm_Settings"];
                Qry_Select_EDER_TO_SAP = ConfigurationManager.AppSettings["Qry_Select_EDER_TO_SAP"];
                Qry_Update_EDER_TO_SAP = ConfigurationManager.AppSettings["Qry_Update_EDER_TO_SAP"];

                try
                {
                    VersionOperationRetryCount = ConfigurationManager.AppSettings["VersionOperationRetryCount"];
                }
                catch
                {
                    VersionOperationRetryCount = "0";
                }
            }
            catch (Exception ex)
            {
                
                    Console.WriteLine("Exception " + ex.Message + " Occured in Reading Config File");
            }
        }





        public const string EDConnection = "EDConnection";

        // m4jf edgisrearch 919
        //public const string EDWorkSpaceConnString = "EDWorkSpaceConnString";
        public const string EDWorkSpaceConnString = "EDER_SDEConnection";

        public const string EDWorkSpaceConnString_sde = "EDWorkSpaceConnString_sde";


        public const string GenerationInfoTableName = "GenerationInfoTableName";
        public const string GenerationInfoStageTableName = "GenerationInfoStageTableName";
        public const string SMGenerationStageTableName = "SMGenerationStageTableName";
        public const string EDGISChangesToSAPTableName = "EDGISChangesToSAPTableName";

        public const string ServiceLocationTableName = "ServiceLocationTableName";


        public const string SAPDailyInterfaceVersionName = "SAPDailyInterfaceVersionName";
        public const string TargetVersionName = "TargetVersionName";
        public const string QueryToGetRecordsFromGenInfoStage = "QueryToGetRecordsFromGenInfoStage";

        public const string ColumnsToTransferFromStageToMainGenInfoTable = "ColumnsToTransferFromStageToMainGenInfoTable";
        public const string Col_CommonFieldStageToMainGenInfoTable = "Col_CommonFieldStageToMainGenInfoTable";


        public const string Col_GlobalID = "Col_GlobalID";
        public const string Col_GlobalID_Settings = "Col_GlobalID_Settings";
        public const string Col_DateCreated = "Col_DateCreated";
        public const string Col_CreatedBy = "Col_CreatedBy";
        public const string Col_DateModified = "Col_DateModified";
        public const string Col_ModifiedBy = "Col_ModifiedBy";
        public const string Col_ServicePointGuid = "Col_ServicePointGuid";
        public const string Col_Action = "ACTION";

        public const string Col_GENSYMBOLOGY = "Col_GENSYMBOLOGY";
        public const string Col_SERVICELOCATIONGUID = "Col_SERVICELOCATIONGUID";
        public const string Col_PROJECTNAME = "Col_PROJECTNAME";
        public const string Col_EFFRATINGINVKW = "Col_EFFRATINGINVKW";
        public const string Col_EFFRATINGMACHKW = "Col_EFFRATINGMACHKW";
        public const string Col_GENCATEGORY = "Col_GENCATEGORY";
        public const string Col_LABELTEXT = "Col_LABELTEXT";


        public const string Col_Updated_in_main = "Col_Updated_in_main";
        public const string Col_Updated_in_ED_main = "Col_Updated_in_ED_main";

        public const string GenCategoryValue_Primary = "GenCategoryValue_Primary";
        public const string GenCategoryValue_Secondary = "GenCategoryValue_Secondary";

        public const string GenCategoryDomainValue_Primary = "GenCategoryDomainValue_Primary";
        public const string GenCategoryDomainValue_Secondary = "GenCategoryDomainValue_Secondary";



        public const string DTCol_GlobalIDMain = "DTCol_GlobalIDMain";
        public const string DTCol_GlobalIDStage = "DTCol_GlobalIDStage";
        public const string DTCol_SPGuidStage = "DTCol_SPGuidStage";
        public const string DTCol_Action = "DTCol_Action";



        public const string SAP_Daily_Interface_User = "SAP_Daily_Interface_User";

        public const string Date_Time_Format_gen_info_main_table = "Date_Time_Format_gen_info_main_table";

        // Constant for capturing GIS updates to be sent to SAP .. Start

        public const string parentVersionName = "parentVersionName";
        public const string childVersionName = "childVersionName";

        public const string changedTables = "ChangedTables";

        public const string ChangedTables_generationinfo = "ChangedTables_generationinfo";
        public const string ChangedTables_servicepoint = "ChangedTables_servicepoint";
        public const string ChangedTables_primarymeter = "ChangedTables_primarymeter";
        public const string ChangedTables_transformer = "ChangedTables_transformer";

        public const string ChangedColumns_SERVICEPOINTGUID = "ChangedColumns_SERVICEPOINTGUID";
        public const string ChangedColumns_SERVICEPOINTID = "ChangedColumns_SERVICEPOINTID";
        public const string ChangedColumns_CIRCUITID = "ChangedColumns_CIRCUITID";
        public const string ChangedColumns_CGC12 = "ChangedColumns_CGC12";


        public const string ColName_GLOBALID = "ColName_GLOBALID";
        public const string ColName_SERVICEPOINTGUID = "ColName_SERVICEPOINTGUID";
        public const string ColName_SERVICEPOINTID = "ColName_SERVICEPOINTID";
        public const string ColName_CGC12 = "ColName_CGC12";
        public const string ColName_CIRCUITID = "ColName_CIRCUITID";

        public const string DoNotRollVersions = "DoNotRollVersions";

        // Constant for capturing GIS updates to be sent to SAP .. End



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


    }
}
