using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using PGE_DBPasswordManagement;

namespace PGE.Interfaces.SAP.LoadData
{
    class ReadConfigurations
    {
        #region Global Variables

        public static string EDER_CONNECTIONSTRING = string.Empty;
        public static string SETTINGS_CONNECTION_STRING = string.Empty;
        public static string Date_Format_For_Update = string.Empty;
        public static string STAGE1_SUMMARY_TABLE_NAME = string.Empty;
        public static string QUERY_STRING_ON_SUMMARY_TABLE = string.Empty;
        public static string UPDATE_STRING_ON_SUMMARY_TABLE = string.Empty;
        public static string QUERY_STRING_ON_MAIN_TABLE = string.Empty;
        public static string QUERY_STRING_ON_MAIN_SMGEN_TABLE = string.Empty;
        public static string QUERY_STRING_ON_MAIN_SMPRO_TABLE = string.Empty;
        public static string QUERY_STRING_ON_MAIN_SMRELAY_TABLE = string.Empty;
        public static string QUERY_STRING_ON_MAIN_SMGENERATOR_TABLE = string.Empty;
        public static string QUERY_STRING_ON_MAIN_SMGENEQUP_TABLE = string.Empty;
        public static string STAGE1_EQP_TABLE_NAME = string.Empty;

        public static string STAGE2_GEN_INFO_TABLE_NAME = string.Empty;
        public static string STAGE2_SM_GENINFOINSERT_QUERY_STRING = string.Empty;
        public static string FIELDS_TO_INSERT_STAGE2_GENINFO = string.Empty;
        public static string VersionName = string.Empty;
        
        public static string STAGE2_SM_GEN_TABLE_NAME = string.Empty;
        public static string FIELDS_TO_INSERT_STAGE2_SMGEN = string.Empty;
        public static string FIELDS_TO_INSERT_MAIN_SMGEN = string.Empty;
        public static string FIELDS_TO_UPDATE_MAIN_SMGEN = string.Empty;
        public static string FIELDS_TO_QUERY_STAGE2_SMGEN = string.Empty;

        public static string STAGE2_SM_PROTECTION_TABLE_NAME = string.Empty;
        public static string FIELDS_TO_INSERT_STAGE2_SMPRO = string.Empty;
        public static string FIELDS_TO_INSERT_MAIN_SMPRO = string.Empty;
        public static string FIELDS_TO_UPDATE_MAIN_SMPRO = string.Empty;
        public static string FIELDS_TO_QUERY_STAGE2_SMPRO = string.Empty;

        public static string STAGE2_SM_RELAY_TABLE_NAME = string.Empty;

        public static string STAGE2_SM_GENRATOR_TABLE_NAME = string.Empty;
        public static string FIELDS_TO_INSERT_STAGE2_SMGENERATOR = string.Empty;
        public static string FIELDS_TO_INSERT_MAIN_SMGENERATOR = string.Empty;
        public static string FIELDS_TO_UPDATE_MAIN_SMGENERATOR = string.Empty;
        public static string FIELDS_TO_QUERY_STAGE2_SMGENERATOR = string.Empty;

        public static string STAGE2_SM_GEN_EQUIP_TABLE_NAME = string.Empty;
        public static string FIELDS_TO_INSERT_STAGE2_SMGENEQUIP = string.Empty;
        public static string FIELDS_TO_INSERT_MAIN_SMGENEQUIP = string.Empty;
        public static string FIELDS_TO_UPDATE_MAIN_SMGENEQUIP = string.Empty;
        public static string FIELDS_TO_QUERY_STAGE2_SMGENEQUIP = string.Empty;

        public static string MAIN_SERVICE_POINT_TABLE_NAME = string.Empty;
        public static string MAIN_PRIMARY_METER_TABLE_NAME = string.Empty;
        public static string MAIN_TRANSFORMER_TABLE_NAME = string.Empty;
        public static string MAIN_GEN_INFO_TABLE_NAME = string.Empty;
        public static string MAIN_SM_GEN_TABLE_NAME = string.Empty;
        public static string MAIN_SM_PROTECTION_TABLE_NAME = string.Empty;
        public static string MAIN_SM_RELAY_TABLE_NAME = string.Empty;
        public static string MAIN_SM_GENRATOR_TABLE_NAME = string.Empty;
        public static string MAIN_SM_GEN_EQUIP_TABLE_NAME = string.Empty;

        public static string QUERY_STRING_ON_STG2_SM_GEN_TABLE = string.Empty;
        public static string QUERY_STRING_ON_STG2_SM_PRO_TABLE = string.Empty;
        public static string QUERY_STRING_ON_STG2_SM_GENERATOR_TABLE = string.Empty;
        public static string QUERY_STRING_ON_STG2_SM_GEN_EQUIP_TABLE = string.Empty;
        public static string STAGE2_SM_GEN_GLOBALID_EXIST_IN_EDGIS = string.Empty;


        public static string TRIGGERS_ASSIGNED_IN_SETTINGS = string.Empty;
        public static string TRIGGERS_ASSIGNED_IN_STAGE2 = string.Empty;

        public static string SP_NAME_TO_DISABLE_TRIGGERS = string.Empty;
        public static string SP_NAME_TO_ENABLE_TRIGGERS = string.Empty;
        public static string SP_NAME_TO_DELETE_PROCESSED_DATA_IN_STAGE2 = string.Empty;

        public static string UPDATE_QUERY_IN_STAGE2 = string.Empty;


        public static string UPDATE_FIELD_IN_STAGE2_GENERATIONINFO= string.Empty;
        public static string UPDATE_FIELD_IN_STAGE2_SM_GEN_EQUIP= string.Empty;
        public static string UPDATE_FIELD_IN_STAGE2_SM_GENERATOR= string.Empty;
        public static string UPDATE_FIELD_IN_STAGE2_SM_PROTECTION= string.Empty;
        public static string UPDATE_FIELD_IN_STAGE2_SM_RELAY= string.Empty;
        public static string UPDATE_FIELD_IN_STAGE2_SM_GENERATION= string.Empty;

        public static string PTO_DATE_INDEX = string.Empty;
        public static string Fields_To_INSERT_IN_HIST_SmGen= string.Empty;
        public static string Fields_To_INSERT_IN_HIST_SmProtection= string.Empty;
        public static string Fields_To_INSERT_IN_HIST_SmGenerator= string.Empty;
        public static string Fields_To_INSERT_IN_HIST_SmGenEquipment= string.Empty;
        public static string Fields_To_INSERT_IN_HIST_SmRelay = string.Empty;
        public static string Fields_To_SELECT_For_HIST_SmGen = string.Empty;
        public static string Fields_To_SELECT_For_HIST_SmProtection = string.Empty;
        public static string Fields_To_SELECT_For_HIST_SmGenerator = string.Empty;
        public static string Fields_To_SELECT_For_HIST_SmGenEquipment = string.Empty;
        #region Global Variables 
        
        public static string INPUT_FOLDER_PATH = string.Empty;
       
        public static string ARCHIVE_FOLDER_PATH = string.Empty;
        public static string SUMMARY_FILE_NAME = string.Empty;
        public static string EQP_FILE_NAME = string.Empty;
        public static string DELIMITER = string.Empty;
        public static string SUMMARY_TABLE_NAME = string.Empty;
        public static string EQP_TABLE_NAME = string.Empty;
        public static string SUMMARY_COLUMN_LIST = string.Empty;
        public static string EQP_COLUMN_LIST = string.Empty;
        public static string SAP_FILE_PATH = string.Empty;
        public static string OUTPUT_FILE = string.Empty;
        public static string QUERY_SELECTALL = string.Empty;
        public static string SP_Data_Validation = string.Empty;
        public static string FN_Delete_Stg2 = string.Empty;
        public static string FN_Main_To_Stg2_Update_Delete = string.Empty;
        public static string SP_Stg1_To_Stg2_DI = string.Empty;
        public static string FN_Calc_Gen_Type = string.Empty;
        public static string SP_Stg2_2_Main_EDGIS_GenInfo = string.Empty;
        public static string SP_Stg2_2_SETT = string.Empty;
        public static string COL_POWER_SOURCE = string.Empty;
        public static string COL_STATUS = string.Empty;
        public static string COL_STATUS_MESSAGE = string.Empty;
        public static string COL_UL1741_CERTIFICATION = string.Empty;
        public static string COL_UL1741_SA_CERTIFICATION = string.Empty;
        public static string COL_STAUS = string.Empty;
        public static string COL_RATED_DISCHARGE = string.Empty;
        public static string Summary_Insert_Columns = string.Empty;
        public static string Equip_Insert_Columns = string.Empty;
        public static char[] TEXTFILE_DELIMETER;
        public static string DATE_FORMAT = string.Empty;
        public static string Date_Format_for_GenInfo = string.Empty;
        public static string action = string.Empty;
        public static string sap_notification = string.Empty;
        public static string project_name = string.Empty;
        public static string power_source = string.Empty;
        public static string eff_rating_mach_kw = string.Empty;
        public static string eff_rating_inv_kw = string.Empty;
        public static string eff_rating_mach_kva = string.Empty;
        public static string eff_rating_inv_kva = string.Empty;
        public static string max_cap = string.Empty;
        public static string charge_demand_kw = string.Empty;
        public static string program_type = string.Empty;
        public static string service_pt = string.Empty;
        public static string guid = string.Empty;
        public static string objid = string.Empty;
        public static string summary_col = string.Empty;
        public static string File_Format = string.Empty;
        public static string type_Int32 = string.Empty;
        public static string type_String = string.Empty;
        public static string LOGCONFIG = ConfigurationManager.AppSettings["LogConfigName"];
        public static string Input_DM_DI_Flag = string.Empty;
        public static string FN_Del_Stg2_Return = string.Empty;
        public static string FN_Main_Stg2_Return = string.Empty;
        public static string SP_Stg1_To_Stg2_DI_Return = string.Empty;
        public static string Input_Action = string.Empty;
        public static string Flag = string.Empty;
        public static string FN_GenType_Return = string.Empty;
        public static string DataMigration = string.Empty;
        public static string DailyInterface = string.Empty;
        public static char File_Separator;
        public static string  Qry_Select_Setting_SPID= string.Empty;
        public static string Table_Servicepoint = string.Empty;
        public static string Table_PrimaryMeter = string.Empty;
        public static string Table_Transformer = string.Empty;
        public static string Table_Gen_Eqp_Stg = string.Empty;
        public static string proc_stage_insrt = string.Empty;
        public static string proc_changed_cid_insrt = string.Empty;
       
        
        public static string proc_changed_spid_insrt = string.Empty;
        public static string PGE_UPDT_STATUS_STG2_2_STG1_SP = string.Empty; 
        public static string Qry_Slct_Stage = string.Empty;
        public static string Qry_Slct_Changed_CID = string.Empty;
        public static string Qry_Slct_Changed_SPID = string.Empty;
        public static string Proc_Unique_Records = string.Empty;
        public static string Output_File_Archive = string.Empty;
        public static string SUMMARY_STG_CURR_SEQ = string.Empty;
        public static string EQP_STG_CURR_SEQ = string.Empty;
        public static string Query_Get_Curr_Seq = string.Empty;
        public static string Qry_Clear_Curr_Data = string.Empty;
        public static string PGE_STG2_GUID_UPDT_STG1_SP = string.Empty;
        public static string Qry_Distinct_Action = string.Empty;
        public static string SP_EDER_TO_SAP_STATUS_INSERT = string.Empty;
        public static string Col_GEN_GUID =  string.Empty;
        public static string Col_SPGUID =  string.Empty;
        public static string Col_SPID =  string.Empty;
        public static string Col_CGC12 =  string.Empty;
        public static string Col_CID =  string.Empty;
        public static string Col_Updated_Field =  string.Empty;
        public static string Col_Updated_Table =  string.Empty;
        public static string Col_FeatureGUID =  string.Empty;
        public static string Col_Postdate =  string.Empty;
        public static string Col_Comments =  string.Empty;
        public static string Col_Updated_in_Settings = string.Empty;
        public static string Col_Cedsa_Match_Found = string.Empty;
        public static string FieldsToInsert_EDGIS_GIS_CHANGES_SAP =  string.Empty;
        public static string Value_Current_Future = string.Empty;
        public static string Qry_Fetch_EqpId_Frm_Settings = string.Empty;
        public static string Qry_Select_EDER_TO_SAP = string.Empty;
        public static string Qry_Update_EDER_TO_SAP = string.Empty;
        public static string Migrate_CEDSADATA = string.Empty;
        public static string SAPToEDERSTAGTableName = ConfigurationManager.AppSettings["SAP_TO_EDER_STAGGING_TABLENAME"];
        public static string COL_SAPTOGIS_STG_TABLE = ConfigurationManager.AppSettings["COL_SAPTOGIS_STG_TABLE"]; 
        public static string ColName_PROCESS_NAME = ConfigurationManager.AppSettings["ColName_PROCESS_NAME"]; 
        public static string ColName_STAGE_STATUS = ConfigurationManager.AppSettings["ColName_STAGE_STATUS"]; 
         public static string ColName_STATUS_DATE = ConfigurationManager.AppSettings["ColName_STATUS_DATE"];
         public static string SP_INSERT_IN_STAGE_ARCHIVE = ConfigurationManager.AppSettings["SP_INSERT_IN_STAGE_ARCHIVE"];
         public static string SP_PGE_UPDATE_SMGEN_GENTYPE = string.Empty;
         public static string Qry_UpdateVersionName = string.Empty;
         public static string PTO_DATE_FORMAT = string.Empty;
         public static string SP_CEDSA_DATA_VLDTN = string.Empty;
         public static string SP_CEDSA_2_SAP = string.Empty;
         public static string CEDSA_KEYWORD = string.Empty;
         public static string Settings_User = ConfigurationManager.AppSettings["SETTINGS_USER"];

        // m4jf edgisrearch - 416 ed15 improvements
        public static string intExecutionSummary = ConfigurationManager.AppSettings["IntExecutionSummaryExePath"];
        public static string UserName = ConfigurationManager.AppSettings["UserName"];
        public static string MAIL_SUBJECT_POSTFAIL = ConfigurationManager.AppSettings["MAIL_SUBJECT_POSTFAIL"];
        public static string MAIL_BODY_POSTFAIL = ConfigurationManager.AppSettings["MAIL_BODY_POSTFAIL"];
        public static string MAIL_TO_POSTFAIL = ConfigurationManager.AppSettings["MAIL_TO_POSTFAIL"];

        #endregion

        #endregion
        public static void ReadFromConfiguration()
        {
            try
            {
                Migrate_CEDSADATA = ConfigurationManager.AppSettings["Migrate_CEDSADATA"];
                SP_CEDSA_DATA_VLDTN = ConfigurationManager.AppSettings["SP_CEDSA_DATA_VLDTN"];
                CEDSA_KEYWORD = ConfigurationManager.AppSettings["CEDSA_KEYWORD"];
                SP_CEDSA_2_SAP = ConfigurationManager.AppSettings["SP_CEDSA_2_SAP"];
                PTO_DATE_FORMAT = ConfigurationManager.AppSettings["PTO_DATE_FORMAT"];
                Date_Format_For_Update = ConfigurationManager.AppSettings["Date_Format_For_Update"];
                VersionName = ConfigurationManager.AppSettings["VERSIONNAME"];
                Qry_UpdateVersionName = ConfigurationManager.AppSettings["Qry_UpdateVersionName"];
                SP_PGE_UPDATE_SMGEN_GENTYPE = ConfigurationManager.AppSettings["SP_PGE_UPDATE_SMGEN_GENTYPE"];


                // m4jf edgisrearch 919
                //SETTINGS_CONNECTION_STRING = ConfigurationManager.AppSettings["Settings_ConnectionString"];
                //EDER_CONNECTIONSTRING = ConfigurationManager.AppSettings["EDER_ConnectionString"];
                SETTINGS_CONNECTION_STRING = ReadEncryption.GetConnectionStr(ConfigurationManager.AppSettings["EDAUX_ConnectionStr"]);
                EDER_CONNECTIONSTRING = ReadEncryption.GetConnectionStr(ConfigurationManager.AppSettings["EDER_ConnectionStr"]);

                //LOGCONFIG = 
                STAGE1_SUMMARY_TABLE_NAME = ConfigurationManager.AppSettings["Stage1_Summary_Table_Name"];
                QUERY_STRING_ON_SUMMARY_TABLE = ConfigurationManager.AppSettings["Query_on_SummaryTable"];
                UPDATE_STRING_ON_SUMMARY_TABLE = ConfigurationManager.AppSettings["Update_on_SummaryTable"];
                QUERY_STRING_ON_MAIN_TABLE = ConfigurationManager.AppSettings["Query_on_MainTable"];
                STAGE1_EQP_TABLE_NAME = ConfigurationManager.AppSettings["Stage1_Equipment_Table_Name"];

                STAGE2_GEN_INFO_TABLE_NAME = ConfigurationManager.AppSettings["Stage2_Generation_info_Table_Name"];
                STAGE2_SM_GEN_TABLE_NAME = ConfigurationManager.AppSettings["Stage2_SM_Generation_Table_Name"];
                STAGE2_SM_PROTECTION_TABLE_NAME = ConfigurationManager.AppSettings["Stage2_SM_Protection_Table_Name"];
                STAGE2_SM_RELAY_TABLE_NAME = ConfigurationManager.AppSettings["Stage2_SM_Relay_Table_Name"];
                STAGE2_SM_GENRATOR_TABLE_NAME = ConfigurationManager.AppSettings["Stage2_SM_Generator_Table_Name"];
                STAGE2_SM_GEN_EQUIP_TABLE_NAME = ConfigurationManager.AppSettings["Stage2_SM_Gen_Equipment_Table_Name"];

                STAGE2_SM_GEN_GLOBALID_EXIST_IN_EDGIS = ConfigurationManager.AppSettings["Query_To_Check_GlobalId_Available_in_EDGIS"]; 


                MAIN_SERVICE_POINT_TABLE_NAME = ConfigurationManager.AppSettings["Main_Service_Point_Table_Name"];
                MAIN_PRIMARY_METER_TABLE_NAME = ConfigurationManager.AppSettings["Main_Primary_Meter_Table_Name"];
                MAIN_TRANSFORMER_TABLE_NAME = ConfigurationManager.AppSettings["Main_Transformer_Table_Name"];
                MAIN_GEN_INFO_TABLE_NAME = ConfigurationManager.AppSettings["Main_Generation_Info_Table_Name"];
                
                MAIN_SERVICELOCATION_TABLE_NAME = ConfigurationManager.AppSettings["Main_Servicelocation_Table_Name"];//derms changes 04/20/2021
                MAIN_SM_GEN_TABLE_NAME = ConfigurationManager.AppSettings["Main_SM_Generation_Table_Name"];
                QUERY_STRING_ON_MAIN_SMGEN_TABLE = ConfigurationManager.AppSettings["Query_on_MainSmGenTable"];
                MAIN_SM_PROTECTION_TABLE_NAME = ConfigurationManager.AppSettings["Main_SM_Protection_Table_Name"];
                QUERY_STRING_ON_MAIN_SMPRO_TABLE = ConfigurationManager.AppSettings["Query_on_MainSmProTable"];
                MAIN_SM_RELAY_TABLE_NAME = ConfigurationManager.AppSettings["Main_SM_Relay_Table_Name"];
                QUERY_STRING_ON_MAIN_SMRELAY_TABLE = ConfigurationManager.AppSettings["Query_on_MainSmRelayTable"];
                MAIN_SM_GENRATOR_TABLE_NAME = ConfigurationManager.AppSettings["Main_SM_Generator_Table_Name"];
                QUERY_STRING_ON_MAIN_SMGENERATOR_TABLE = ConfigurationManager.AppSettings["Query_on_MainSmGeneratorTable"];
                MAIN_SM_GEN_EQUIP_TABLE_NAME = ConfigurationManager.AppSettings["Main_SM_Gen_Equipment_Table_Name"];
                QUERY_STRING_ON_MAIN_SMGENEQUP_TABLE = ConfigurationManager.AppSettings["Query_on_MainSmGenEquipTable"];

                STAGE2_SM_GENINFOINSERT_QUERY_STRING = ConfigurationManager.AppSettings["Insert_on_Stage2SmGenInfoTable"];
                STAGE2_SM_GENINFOINSERT_QUERY_STRING = ConfigurationManager.AppSettings["Insert_on_Stage2SmGenTable"];

                FIELDS_TO_INSERT_STAGE2_GENINFO = ConfigurationManager.AppSettings["Fields_To_Insert_Stage2_GenInfo"];

                FIELDS_TO_INSERT_STAGE2_SMGEN = ConfigurationManager.AppSettings["Fields_To_Insert_Stage2_SmGen"];
                FIELDS_TO_QUERY_STAGE2_SMGEN = ConfigurationManager.AppSettings["Fields_To_Query_Stage2_SmGen"];
                FIELDS_TO_INSERT_MAIN_SMGEN = ConfigurationManager.AppSettings["Fields_To_Insert_Main_SmGen"];
                FIELDS_TO_UPDATE_MAIN_SMGEN = ConfigurationManager.AppSettings["Fields_To_Insert_Main_SmGen"];

                FIELDS_TO_INSERT_STAGE2_SMPRO = ConfigurationManager.AppSettings["Fields_To_Insert_Stage2_SmProtection"];
                FIELDS_TO_QUERY_STAGE2_SMPRO = ConfigurationManager.AppSettings["Fields_To_Query_Stage2_SmProtection"];
                FIELDS_TO_INSERT_MAIN_SMPRO = ConfigurationManager.AppSettings["Fields_To_Insert_Main_SmProtection"];
                FIELDS_TO_UPDATE_MAIN_SMPRO = ConfigurationManager.AppSettings["Fields_To_Update_Main_SmProtection"];


                FIELDS_TO_INSERT_STAGE2_SMGENERATOR = ConfigurationManager.AppSettings["Fields_To_Insert_Stage2_SmGenerator"];
                FIELDS_TO_QUERY_STAGE2_SMGENERATOR = ConfigurationManager.AppSettings["Fields_To_Query_Stage2_SmGenerator"];
                FIELDS_TO_INSERT_MAIN_SMGENERATOR = ConfigurationManager.AppSettings["Fields_To_Insert_Main_SmGenerator"];
                FIELDS_TO_UPDATE_MAIN_SMGENERATOR = ConfigurationManager.AppSettings["Fields_To_Insert_Main_SmGenerator"];


                FIELDS_TO_INSERT_STAGE2_SMGENEQUIP = ConfigurationManager.AppSettings["Fields_To_Insert_Stage2_SmGenEquipment"];
                FIELDS_TO_QUERY_STAGE2_SMGENEQUIP = ConfigurationManager.AppSettings["Fields_To_Query_Stage2_SmGenEquipment"];
                FIELDS_TO_INSERT_MAIN_SMGENEQUIP = ConfigurationManager.AppSettings["Fields_To_Insert_Main_SmGenEquipment"];
                FIELDS_TO_UPDATE_MAIN_SMGENEQUIP = ConfigurationManager.AppSettings["Fields_To_Insert_Main_SmGenEquipment"];

                Fields_To_INSERT_IN_HIST_SmGen = ConfigurationManager.AppSettings["Fields_To_INSERT_IN_HIST_SmGen"];
                Fields_To_INSERT_IN_HIST_SmProtection = ConfigurationManager.AppSettings["Fields_To_INSERT_IN_HIST_SmProtection"];
                Fields_To_INSERT_IN_HIST_SmGenerator = ConfigurationManager.AppSettings["Fields_To_INSERT_IN_HIST_SmGenerator"];
                Fields_To_INSERT_IN_HIST_SmGenEquipment = ConfigurationManager.AppSettings["Fields_To_INSERT_IN_HIST_SmGenEquipment"];
                Fields_To_INSERT_IN_HIST_SmRelay = ConfigurationManager.AppSettings["Fields_To_INSERT_IN_HIST_SmRelay"];
                Fields_To_SELECT_For_HIST_SmGen = ConfigurationManager.AppSettings["Fields_To_SELECT_For_HIST_SmGen"];
                Fields_To_SELECT_For_HIST_SmProtection = ConfigurationManager.AppSettings["Fields_To_SELECT_For_HIST_SmProtection"];
                Fields_To_SELECT_For_HIST_SmGenerator = ConfigurationManager.AppSettings["Fields_To_SELECT_For_HIST_SmGenerator"];
                Fields_To_SELECT_For_HIST_SmGenEquipment = ConfigurationManager.AppSettings["Fields_To_SELECT_For_HIST_SmGenEquipment"];

                QUERY_STRING_ON_STG2_SM_GEN_TABLE = ConfigurationManager.AppSettings["Query_on_SM_Gen_Table"];
                QUERY_STRING_ON_STG2_SM_PRO_TABLE= ConfigurationManager.AppSettings["Query_on_SM_Pro_Table"];
                QUERY_STRING_ON_STG2_SM_GENERATOR_TABLE= ConfigurationManager.AppSettings["Query_on_SM_Generator_Table"];
                QUERY_STRING_ON_STG2_SM_GEN_EQUIP_TABLE = ConfigurationManager.AppSettings["Query_on_SM_Gen_Equip_Table"];

                

                TRIGGERS_ASSIGNED_IN_SETTINGS = ConfigurationManager.AppSettings["Triggers_Assigned_in_SETTINGS"];
                TRIGGERS_ASSIGNED_IN_STAGE2 = ConfigurationManager.AppSettings["Triggers_Assigned_in_STAGE2"];

                SP_NAME_TO_DISABLE_TRIGGERS = ConfigurationManager.AppSettings["SP_Name_To_Disable_Triggers"];
                SP_NAME_TO_ENABLE_TRIGGERS = ConfigurationManager.AppSettings["SP_Name_To_Enable_Triggers"];
                SP_NAME_TO_DELETE_PROCESSED_DATA_IN_STAGE2 = ConfigurationManager.AppSettings["SP_Name_To_Delete_Data_Stage2"];

                UPDATE_FIELD_IN_STAGE2_GENERATIONINFO = ConfigurationManager.AppSettings["Update_Field_In_Stage2_GenerationInfo"];
                UPDATE_FIELD_IN_STAGE2_SM_GEN_EQUIP = ConfigurationManager.AppSettings["Update_Field_In_Stage2_SM_Gen_Equip"];
                UPDATE_FIELD_IN_STAGE2_SM_GENERATOR = ConfigurationManager.AppSettings["Update_Field_In_Stage2_SM_Generator"];
                UPDATE_FIELD_IN_STAGE2_SM_PROTECTION = ConfigurationManager.AppSettings["Update_Field_In_Stage2_SM_Protection"];
                UPDATE_FIELD_IN_STAGE2_SM_RELAY = ConfigurationManager.AppSettings["Update_Field_In_Stage2_SM_Relay"];
                UPDATE_FIELD_IN_STAGE2_SM_GENERATION = ConfigurationManager.AppSettings["Update_Field_In_Stage2_SM_Generation"];

                PTO_DATE_INDEX = ConfigurationManager.AppSettings["PTO_DATE_INDEX"];
               

                
                #region Reading from Config
                Value_Current_Future = ConfigurationManager.AppSettings["Value_Current_Future"];
                INPUT_FOLDER_PATH = ConfigurationManager.AppSettings["Path_To_SAP_Files"];
                ARCHIVE_FOLDER_PATH = ConfigurationManager.AppSettings["Path_To_Archive"];
                Settings_User = ConfigurationManager.AppSettings["SETTINGS_USER"];
                
                SUMMARY_FILE_NAME = ConfigurationManager.AppSettings["Summary_File_Name"];
                EQP_FILE_NAME = ConfigurationManager.AppSettings["Equipment_File_Name"];
                DELIMITER = ConfigurationManager.AppSettings["Delimiter"];
                SUMMARY_TABLE_NAME = ConfigurationManager.AppSettings["Summary_Table_Name"];
                EQP_TABLE_NAME = ConfigurationManager.AppSettings["Equipment_Table_Name"];
                SUMMARY_COLUMN_LIST = ConfigurationManager.AppSettings["Summary_Column_List"];
                EQP_COLUMN_LIST = ConfigurationManager.AppSettings["Equip_Insert_Columns"];
                SAP_FILE_PATH = ConfigurationManager.AppSettings["Path_To_SAP_Files"];
               
                SP_Data_Validation = ConfigurationManager.AppSettings["SP_Data_Validation"];
                FN_Delete_Stg2 = ConfigurationManager.AppSettings["FN_Delete_Stg2"];
                FN_Main_To_Stg2_Update_Delete = ConfigurationManager.AppSettings["FN_Main_To_Stg2_Update_Delete"];
                SP_Stg1_To_Stg2_DI = ConfigurationManager.AppSettings["SP_Stg1_To_Stg2_DI"];
                FN_Calc_Gen_Type = ConfigurationManager.AppSettings["FN_Calc_Gen_Type"];
                SP_Stg2_2_Main_EDGIS_GenInfo = ConfigurationManager.AppSettings["SP_Stg2_2_Main_EDGIS_GenInfo"];
                SP_Stg2_2_SETT = ConfigurationManager.AppSettings["SP_Stg2_2_SETT"];
                COL_POWER_SOURCE = ConfigurationManager.AppSettings["COL_POWER_SOURCE"];
                COL_STATUS = ConfigurationManager.AppSettings["COL_STATUS"];
                COL_STAUS = ConfigurationManager.AppSettings["COL_STAUS"];
                COL_STATUS_MESSAGE = ConfigurationManager.AppSettings["COL_STATUS_MESSAGE"];
                COL_UL1741_CERTIFICATION = ConfigurationManager.AppSettings["COL_UL1741_CERTIFICATION"];
                COL_UL1741_SA_CERTIFICATION = ConfigurationManager.AppSettings["COL_UL1741_SA_CERTIFICATION"];
                COL_RATED_DISCHARGE = ConfigurationManager.AppSettings["COL_RATED_DISCHARGE"];
                Summary_Insert_Columns = ConfigurationManager.AppSettings["Summary_Insert_Columns"];
                Equip_Insert_Columns = ConfigurationManager.AppSettings["Equip_Column_List"];
                TEXTFILE_DELIMETER = new char[] { '|' };
                DATE_FORMAT = ConfigurationManager.AppSettings["Date_Format"];
                action = ConfigurationManager.AppSettings["Summary_Action"];
                sap_notification = ConfigurationManager.AppSettings["Summary_SAP_Noti"];
                project_name = ConfigurationManager.AppSettings["Summary_Project_Name"];
                power_source = ConfigurationManager.AppSettings["Summary_Power_Source"];
                eff_rating_mach_kw = ConfigurationManager.AppSettings["Summary_Mach_KW"];
                eff_rating_inv_kw = ConfigurationManager.AppSettings["Summary_Inv_KW"];
                eff_rating_mach_kva = ConfigurationManager.AppSettings["Summary_Mach_KVA"];
                eff_rating_inv_kva = ConfigurationManager.AppSettings["Summary_Inv_KVA"];
                max_cap = ConfigurationManager.AppSettings["Summary_Max_Cap"];
                charge_demand_kw = ConfigurationManager.AppSettings["Summary_Charfe_Dmd"];
                program_type = ConfigurationManager.AppSettings["Summary_Program_Type"];
                service_pt = ConfigurationManager.AppSettings["Summary_Spid"];
                guid = ConfigurationManager.AppSettings["Summary_Guid"];
                objid = ConfigurationManager.AppSettings["Summary_Objid"];
                summary_col = ConfigurationManager.AppSettings["Summary_Insert_Columns"];
                File_Format = ConfigurationManager.AppSettings["File_Format"];
                type_Int32 = "System.Int32";
                type_String = "System.String";
                Qry_Clear_Curr_Data = ConfigurationManager.AppSettings["Qry_Clear_Curr_Data"];
                Input_DM_DI_Flag = ConfigurationManager.AppSettings["Input_DM_DI_Flag"];
                FN_Del_Stg2_Return = ConfigurationManager.AppSettings["FN_Del_Stg2_Return"];
                FN_Main_Stg2_Return = ConfigurationManager.AppSettings["FN_Main_Stg2_Return"];
                Input_Action = ConfigurationManager.AppSettings["Input_Action"];
                Flag = ConfigurationManager.AppSettings["Flag"];
                FN_GenType_Return = ConfigurationManager.AppSettings["FN_GenType_Return"];
                DailyInterface = ConfigurationManager.AppSettings["DailyInterface"];
                DataMigration = ConfigurationManager.AppSettings["DataMigration"];
                File_Separator = Convert.ToChar(ConfigurationManager.AppSettings["File_Separator"]);
                proc_stage_insrt = ConfigurationManager.AppSettings["Proc_Stage"];
                proc_changed_cid_insrt= ConfigurationManager.AppSettings["Proc_Changed_Cid"];
                Table_Servicepoint = ConfigurationManager.AppSettings["Table_Servicepoint"];
                Table_PrimaryMeter = ConfigurationManager.AppSettings["Table_PrimaryMeter"];
                Table_Transformer = ConfigurationManager.AppSettings["Table_Transformer"];
                Table_Gen_Eqp_Stg = ConfigurationManager.AppSettings["Table_Gen_Eqp_Stg"];               
                proc_changed_spid_insrt = ConfigurationManager.AppSettings["Proc_Changed_Spid"];
                SUMMARY_STG_CURR_SEQ = ConfigurationManager.AppSettings["Summary_Stg_Seq"];
                EQP_STG_CURR_SEQ = ConfigurationManager.AppSettings["Eqp_Stg_Seq"];
                Query_Get_Curr_Seq = ConfigurationManager.AppSettings["Query_Get_Curr_Seq"];
               

                Output_File_Archive = ConfigurationManager.AppSettings["Output_File_Archive"];
                OUTPUT_FILE = ConfigurationManager.AppSettings["Output_File"];
                Qry_Slct_Stage = ConfigurationManager.AppSettings["Qry_Slct_Stage"];
                Qry_Slct_Changed_CID = ConfigurationManager.AppSettings["Qry_Slct_Changed_CID"];
                Qry_Slct_Changed_SPID = ConfigurationManager.AppSettings["Qry_Slct_Changed_SPID"];
                Proc_Unique_Records = ConfigurationManager.AppSettings["Proc_Unique_Records"];
                PGE_UPDT_STATUS_STG2_2_STG1_SP = ConfigurationManager.AppSettings["PGE_UPDT_STATUS_STG2_2_STG1_SP"];
                PGE_STG2_GUID_UPDT_STG1_SP = ConfigurationManager.AppSettings["PGE_STG2_GUID_UPDT_STG1_SP"];
                Qry_Distinct_Action = ConfigurationManager.AppSettings["Qry_Distinct_Action"];
                Qry_Select_Setting_SPID = ConfigurationManager.AppSettings["Qry_Select_Setting_SPID"];
                Date_Format_for_GenInfo = ConfigurationManager.AppSettings["Date_Format_for_GenInfo"];
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

                Col_Updated_in_Settings = ConfigurationManager.AppSettings["Col_Updated_in_Settings"];
                Col_Cedsa_Match_Found = ConfigurationManager.AppSettings["Col_Cedsa_Match_Found"];
                FieldsToInsert_EDGIS_GIS_CHANGES_SAP = ConfigurationManager.AppSettings["FieldsToInsert_EDGIS_GIS_CHANGES_SAP"];
                Qry_Fetch_EqpId_Frm_Settings = ConfigurationManager.AppSettings["Qry_Fetch_EqpId_Frm_Settings"];
                Qry_Select_EDER_TO_SAP = ConfigurationManager.AppSettings["Qry_Select_EDER_TO_SAP"];
                Qry_Update_EDER_TO_SAP = ConfigurationManager.AppSettings["Qry_Update_EDER_TO_SAP"];
                #endregion
                
              

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception " + ex.Message + " Occured in Reading Config File");
                MainToStage2._log.Error(ex.Message);
            }
        }
        public static void EraseAllStatic()
        {
            EDER_CONNECTIONSTRING = string.Empty;
            SETTINGS_CONNECTION_STRING = string.Empty;


            STAGE1_SUMMARY_TABLE_NAME = string.Empty;
            QUERY_STRING_ON_SUMMARY_TABLE = string.Empty;
            STAGE1_EQP_TABLE_NAME = string.Empty;

            STAGE2_GEN_INFO_TABLE_NAME = string.Empty;
            STAGE2_SM_GEN_TABLE_NAME = string.Empty;
            STAGE2_SM_PROTECTION_TABLE_NAME = string.Empty;
            STAGE2_SM_RELAY_TABLE_NAME = string.Empty;
            STAGE2_SM_GENRATOR_TABLE_NAME = string.Empty;
            STAGE2_SM_GEN_EQUIP_TABLE_NAME = string.Empty;


            MAIN_SERVICE_POINT_TABLE_NAME = string.Empty;
            MAIN_PRIMARY_METER_TABLE_NAME = string.Empty;
            MAIN_TRANSFORMER_TABLE_NAME = string.Empty;
            MAIN_GEN_INFO_TABLE_NAME = string.Empty;

            MAIN_SM_GEN_TABLE_NAME = string.Empty;
            MAIN_SM_PROTECTION_TABLE_NAME = string.Empty;
            MAIN_SM_RELAY_TABLE_NAME = string.Empty;
            MAIN_SM_RELAY_TABLE_NAME = string.Empty;
            MAIN_SM_GEN_EQUIP_TABLE_NAME = string.Empty;


            
        }



       

        public const string EDConnection = "EDConnection";
        public const string EDWorkSpaceConnString = "EDWorkSpaceConnString";
        public const string EDWorkSpaceConnString_sde = "EDWorkSpaceConnString_sde";


        public const string GenerationInfoTableName = "GenerationInfoTableName";
        public const string GenerationInfoStageTableName = "GenerationInfoStageTableName";
        public const string SMGenerationStageTableName = "SMGenerationStageTableName";
        public const string EDGISChangesToSAPTableName = "EDGISChangesToSAPTableName";

        public const string ServiceLocationTableName = "ServiceLocationTableName";
        


        public const string SAPDailyInterfaceVersionName = "SAPDailyInterfaceVersionName";
        public const string TargetVersionName = "TargetVersionName";
        public const string QueryToGetRecordsFromGenInfoStage = "QueryToGetRecordsFromGenInfoStage";
        //DERMS change 04/22/2021
        public static string MAIN_SERVICELOCATION_TABLE_NAME = string.Empty;

        public const string ServicePointTableName = "ServicePointTableName";
        public const string DERMSPrimarySymbolNumberValue = "DERMSPrimarySymbolNumberValue";
        public const string DERMSSecondarySymbolNumberValue = "DERMSSecondarySymbolNumberValue";
        public const string PrimarySymbolNumberValue = "PrimarySymbolNumberValue";
        public const string SecondarySymbolNumberValue = "SecondarySymbolNumberValue";
        public const string ServicePointQuery = "ServicePointQuery";
        public const string GenInfoQuery = "GenInfoQuery";
        //public const string UpdateSymbolNumber = "UpdateSymbolNumber";
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
        public const string Col_VERSIONNAME = "Col_VERSIONNAME";
        public const string Col_SYMBOLNUMBER = "Col_SYMBOLNUMBER";

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
