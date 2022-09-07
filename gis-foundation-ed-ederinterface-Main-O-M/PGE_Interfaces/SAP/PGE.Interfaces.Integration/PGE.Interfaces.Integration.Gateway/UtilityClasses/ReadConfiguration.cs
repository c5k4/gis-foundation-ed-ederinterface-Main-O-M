using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using PGE_DBPasswordManagement;

namespace PGE.Interfaces.Integration.Gateway
{
    public class ReadConfiguration
    {
        public static string oracleConnectionString = string.Empty;
        public static string wipOracleConnectionString = string.Empty;
        public static string intExecutionSummary = string.Empty;

        public static string objectId = string.Empty;
        public static string sapEquipId = string.Empty;
        public static string batchID = string.Empty;
        public static string recordID = string.Empty;
        public static string sapEquipType = string.Empty;
        public static string equipmentName = string.Empty;
        public static string relatedRecordID = string.Empty;
        public static string error_description = string.Empty;
        public static string guid = string.Empty;
        public static string creationDate = string.Empty;
        public static string flag = string.Empty;
        public static string ed07StagingTable = string.Empty;
        public static string ed07StagingTableName = string.Empty;
        public static string sapRelatedRecordID = string.Empty;
        public static string sapBatchId =string.Empty;

        #region variables (OutBound) - (V3SF)

        //Configration Tables
        public static string INTEGRATION_FIELD_MAPPING = string.Empty;
        public static string INTERFACE_STAGINGTABLE_CONFIG = string.Empty;
        public static string INTEGRATION_LASTRUN_DATE = string.Empty;

        //Common GIS/SAP Columns
        public static string SAPRecordIdField = string.Empty;
        public static string SAPBatchIdField = string.Empty;
        public static string GISErrorField = string.Empty;
        public static string GISProcessFlagField = string.Empty;
        public static string GISProcessedTime = string.Empty;

        //Configuration Table Columns (INTERFACE_STAGINGTABLE_CONFIG)
        public static string ISC_interface_name = string.Empty;
        public static string ISC_staging_table = string.Empty;
        public static string ISC_database_name = string.Empty;
        public static string ISC_interface_number = string.Empty;
        public static string ISC_batch_size = string.Empty;

        //Configuration Table Columns (INTEGRATION_FIELD_MAPPING)
        public static string IFM_INTERFACE_NAME = string.Empty;
        public static string IFM_GIS_FIELD = string.Empty;
        public static string IFM_SAP_FIELD = string.Empty;
        public static string IFM_SAP_SEQUENCE = string.Empty;

        //Layer 7 (Post Configuration)
        public static string PostContentType = string.Empty;
        public static string PostMethod = string.Empty;
        public static string Credentials_serviceUserName = string.Empty;
        public static string Credentials_servicePassword = string.Empty;

        //Layer 7 (Response Columns)
        public static string PostResponse_metadata = string.Empty;
        public static string PostResponse_results = string.Empty;
        public static string PostResponseStatus_Success = string.Empty;
        public static string PostResponseStatus_Fail = string.Empty;

        #endregion

        //Last Run Date Table Columns (INTEGRATION_LASTRUN_DATE)
        public static string ILD_INTERFACE_NAME = string.Empty;
        public static string ILD_LASTRUN_DATE = string.Empty;

        //Read configuration from app.config 

        //To validate the merage
        public static string urlED07 = string.Empty;
        public static string urlED11 = string.Empty;
        public static string urlED13 = string.Empty;
        public static string urlED13A = string.Empty;
        public static string urlED14 = string.Empty;
        public static string urlED15Indv = string.Empty;
        public static string urlED15Summary = string.Empty;
        public static string requestMethod = string.Empty;
        public static string ed14StagingTable = string.Empty;
        public static string ed14StagingTableName = string.Empty;
        public static string ed11StagingTable = string.Empty;
        public static string ed11StagingTableName = string.Empty;
        public static string ed13StagingTable = string.Empty;
        public static string ed13StagingTableName = string.Empty;
        public static string ed13AStagingTable = string.Empty;
        public static string ed13AStagingTableName = string.Empty;
        public static string PROJECTID_I = string.Empty;
        public static string PROJECTID_D = string.Empty;
        public static string PROJECTID_U = string.Empty;
        public static string EQUIPMENTNUMBER_I = string.Empty;
        public static string EQUIPMENTNUMBER_d = string.Empty;
        public static string EQUIPMENTNUMBER_u = string.Empty;
        public static string GUID_I = string.Empty;
        public static string GUID_D = string.Empty;
        public static string GUID_U = string.Empty;
        public static string TRANSACTIONTYPE = string.Empty;
        public static string POLECLASS = string.Empty;
        public static string POLETYPE = string.Empty;
        public static string HEIGHT = string.Empty;
        public static string POLECNTYNAME = string.Empty;
        public static string LATITUDE_I = string.Empty;
        public static string LONGITUDE_I = string.Empty;
        public static string DIVISION = string.Empty;
        public static string DISTRICT = string.Empty;
        public static string LOCDESC2 = string.Empty;
        public static string CITY = string.Empty;
        public static string ZIP = string.Empty;
        public static string BARCODE = string.Empty;
        public static string MAPNAME = string.Empty;
        public static string MAPOFFICE = string.Empty;
        public static string STARTUPDATE_I = string.Empty;
        public static string STARTUPDATE_D = string.Empty;
        public static string JOINTPOLENBR = string.Empty;
        public static string ATTRIBUTENAME_U = string.Empty;
        public static string OLDVALUE_U = string.Empty;
        public static string NEWVALUE_U = string.Empty;



        public static string erDAT = string.Empty;
        public static string qmDAT = string.Empty;
        public static string qmDB = string.Empty;
        public static string Ltrmn = string.Empty;
        public static string notificationNo = string.Empty;
        public static string notificationType = string.Empty;
        public static string notificationStatus = string.Empty;
        public static string sapEquiIDED14 = string.Empty;
        public static string equipTypeED14 = string.Empty;
        // V3SF ED07 Delayed Retry Enhancement (16-June-2021) [START]        
        public static int ED07RETRYCOUNTLIMIT = 0;
        public static int ED07RETRYDELAY = 0;
        // V3SF ED07 Delayed Retry Enhancement (16-June-2021) [END]

        #region ED15 Summary variables
        public static string ed15SummStagingTable = string.Empty;
        public static string ed15StagingTableName = string.Empty;
        public static string EFF_RATING_MACH_KW = string.Empty;
        public static string EFF_RATING_INV_KW = string.Empty;
        public static string EFF_RATING_MACH_KVA = string.Empty;
        public static string EFF_RATING_INV_KVA = string.Empty;
        public static string MAX_STORAGE_CAPACITY = string.Empty;
        //WG1 changes 05/13/2022 start
        public static string CHARGE_DEMAND_KW = string.Empty;
        public static string TOT_SYS_LIMITED_EXPORT_KW = string.Empty;
        public static string DERATED_TOT_NP_CAP_INV_KW = string.Empty;
        public static string PROJECT_TOTAL_EXPORT_KW = string.Empty;
        public static string LIMITED = string.Empty;
        //WG1 changes 05/13/2022 end
        #endregion

        #region ED15 Indv variables 
        //ED15 Indv variables
        public static string quantity = string.Empty;
        public static string noOfPhases = string.Empty;
        public static string enosProjRef = string.Empty;
        public static string enosEqupRef = string.Empty;
        public static string ED15INDVStagingTable = string.Empty;
        public static string ED15INDVStagingTableName = string.Empty;
        public static string ptc_Rating = string.Empty;
        public static string invEffe = string.Empty;
        public static string npRating = string.Empty;
        public static string pf = string.Empty;
        public static string effRatingKW = string.Empty;
        public static string effRatingKVA = string.Empty;
        public static string ratedVoltage = string.Empty;
        public static string ssReactance = string.Empty;
        public static string ssResistance = string.Empty;
        public static string transReactance = string.Empty;
        public static string transResistance = string.Empty;
        public static string subReactance = string.Empty;
        public static string subResistance = string.Empty;
        public static string negReactance = string.Empty;
        public static string negResistance = string.Empty;
        public static string zeroResistance = string.Empty;
        public static string zeroReactance = string.Empty;
        public static string grdReacatnace = string.Empty;
        public static string grdResistance = string.Empty;
        public static string ptoDate = string.Empty;
        //WG1 changes 05/13/2022 start
        public static string npCapaicity = string.Empty;
        public static string deratedRatingKW = string.Empty;
        public static string dearatedTotalNPKW = string.Empty;
        public static string usageAseKW = string.Empty;
        public static string techTypeCD = string.Empty;
        public static string sapAction = string.Empty;
        public static string limitexportplan = string.Empty;
        public static string limitedexportpcs = string.Empty;
        public static string spcrdpcscert = string.Empty;
        public static string maxexpctexport = string.Empty;
        public static string operatingmode = string.Empty;
        //WG1 changes 05/13/2022 end
        #endregion

        public static string orderCREATEDATE = string.Empty;

        public static string serviceUserName = string.Empty;
        public static string servicePassword = string.Empty;

        public static void LoadConfiguration()
        {
            try
            {
                // m4jf edgisrearch 919
                //oracleConnectionString = ConfigurationManager.AppSettings["OracleConnectionString"].ToString();
                oracleConnectionString = ReadEncryption.GetConnectionStr(ConfigurationManager.AppSettings["EDER_ConnectionStr"].ToString().ToUpper());

                // V3SF ED07 Delayed Retry Enhancement (16-June-2021) [START]
                //ED07
                try { ED07RETRYCOUNTLIMIT = Convert.ToInt32(ConfigurationManager.AppSettings["ED07RETRYCOUNTLIMIT"].ToString()); } catch { ED07RETRYCOUNTLIMIT = 0; }
                try { ED07RETRYDELAY = Convert.ToInt32(ConfigurationManager.AppSettings["ED07RETRYDELAY"].ToString()); } catch { ED07RETRYDELAY = 0; }
                // V3SF ED07 Delayed Retry Enhancement (16-June-2021) [END]

                objectId = ConfigurationManager.AppSettings["OBJECTID"].ToString();
                sapEquipId = ConfigurationManager.AppSettings["SAP_EQUIPMENT_ID"].ToString();
                batchID = ConfigurationManager.AppSettings["BATCHID"].ToString();
                recordID = ConfigurationManager.AppSettings["RECORDID"].ToString();
                sapEquipType = ConfigurationManager.AppSettings["SAP_EQUIPMENT_TYPE"].ToString();
                equipmentName = ConfigurationManager.AppSettings["EQUIPMENT_NAME"].ToString();
                relatedRecordID = ConfigurationManager.AppSettings["RELATEDRECORDID"].ToString();
                error_description = ConfigurationManager.AppSettings["ERROR_DESCRIPTION"].ToString();
                guid = ConfigurationManager.AppSettings["GISGUID"].ToString();
                creationDate = ConfigurationManager.AppSettings["CREATIONDATE"].ToString();
                flag = ConfigurationManager.AppSettings["PROCESSEDFLAG"].ToString();
                ed07StagingTable = ConfigurationManager.AppSettings["ED07StagingTable"].ToString();
                ed07StagingTableName = ConfigurationManager.AppSettings["ED07StagingTableName"].ToString();
                sapRelatedRecordID = ConfigurationManager.AppSettings["SAPRELATEDRECORDID"].ToString();
                sapBatchId = ConfigurationManager.AppSettings["SAPBatchID"].ToString();

                intExecutionSummary = ConfigurationManager.AppSettings["IntExecutionSummaryExePath"];

                //Read configuration from app.config 
                // m4jf edgisrearch 919 - commented because it is already assigned with same value at line 196 above
               // oracleConnectionString = ConfigurationManager.AppSettings["OracleConnectionString"].ToString();

                // m4jf edgisrearch 919
                //wipOracleConnectionString = ConfigurationManager.AppSettings["WIPOracleConnectionString"].ToString();
                wipOracleConnectionString = ReadEncryption.GetConnectionStr(ConfigurationManager.AppSettings["WIP_ConnectionStr"].ToString().ToUpper());

                objectId = ConfigurationManager.AppSettings["OBJECTID"].ToString();
                sapEquipId = ConfigurationManager.AppSettings["SAP_EQUIPMENT_ID"].ToString();
                batchID = ConfigurationManager.AppSettings["BATCHID"].ToString();
                recordID = ConfigurationManager.AppSettings["RECORDID"].ToString();
                sapEquipType = ConfigurationManager.AppSettings["SAP_EQUIPMENT_TYPE"].ToString();
                equipmentName = ConfigurationManager.AppSettings["EQUIPMENT_NAME"].ToString();
                relatedRecordID = ConfigurationManager.AppSettings["RELATEDRECORDID"].ToString();
                error_description = ConfigurationManager.AppSettings["ERROR_DESCRIPTION"].ToString();
                guid = ConfigurationManager.AppSettings["GISGUID"].ToString();
                creationDate = ConfigurationManager.AppSettings["CREATIONDATE"].ToString();
                flag = ConfigurationManager.AppSettings["PROCESSEDFLAG"].ToString();

                //ED14 Fields
                erDAT = ConfigurationManager.AppSettings["ERDAT"].ToString();
                qmDAT = ConfigurationManager.AppSettings["QMDAT"].ToString();
                qmDB = ConfigurationManager.AppSettings["QMDAB"].ToString();
                Ltrmn = ConfigurationManager.AppSettings["LTRMN"].ToString();
                notificationNo  = ConfigurationManager.AppSettings["NOTIFICATIONNUM"].ToString();
                notificationType  = ConfigurationManager.AppSettings["NOTIFICATIONTYPE"].ToString();
                notificationStatus  = ConfigurationManager.AppSettings["NOTIFICATIONSTATUS"].ToString();
                sapEquiIDED14  = ConfigurationManager.AppSettings["SAPEQUIPMENTID"].ToString();
                equipTypeED14  = ConfigurationManager.AppSettings["EQUIPMENTTYPE"].ToString();

                urlED07 = ConfigurationManager.AppSettings["URLED07"].ToString();
                urlED11 = ConfigurationManager.AppSettings["URLED11"].ToString();
                urlED13 = ConfigurationManager.AppSettings["URLED13"].ToString();
                urlED13A = ConfigurationManager.AppSettings["URLED13A"].ToString();
                urlED14 = ConfigurationManager.AppSettings["URLED14"].ToString();
                urlED15Indv = ConfigurationManager.AppSettings["URLED15INDV"].ToString();
                urlED15Summary = ConfigurationManager.AppSettings["URLED15SUMMARY"].ToString();
                requestMethod = ConfigurationManager.AppSettings["RequestMethod"].ToString();
                serviceUserName = Convert.ToString(ConfigurationManager.AppSettings["Credentials_serviceUserName"]).Split('@')[0]; ;
                servicePassword = ReadEncryption.GetPassword(Convert.ToString(ConfigurationManager.AppSettings["Credentials_serviceUserName"]));

                ed07StagingTable = ConfigurationManager.AppSettings["ED07StagingTable"].ToString();
                ed07StagingTableName = ConfigurationManager.AppSettings["ED07StagingTableName"].ToString();
                ed14StagingTable = ConfigurationManager.AppSettings["ED14StagingTable"].ToString();
                ed14StagingTableName = ConfigurationManager.AppSettings["ED14StagingTableName"].ToString();

                ed13StagingTable = ConfigurationManager.AppSettings["ED13StagingTable"].ToString();
                ed13StagingTableName = ConfigurationManager.AppSettings["ED13StagingTableName"].ToString();
                orderCREATEDATE = ConfigurationManager.AppSettings["ORDERCREATEDATE"].ToString();

                ed13AStagingTable = ConfigurationManager.AppSettings["ED13AStagingTable"].ToString();
                ed13AStagingTableName = ConfigurationManager.AppSettings["ED13AStagingTableName"].ToString();
                #region  Ed11 Configuration
                ed11StagingTable = ConfigurationManager.AppSettings["ED11StagingTable"].ToString();
                ed11StagingTableName = ConfigurationManager.AppSettings["ED11StagingTableName"].ToString();
                PROJECTID_I = ConfigurationManager.AppSettings["PROJECTID_I"].ToString();
                PROJECTID_U = ConfigurationManager.AppSettings["PROJECTID_U"].ToString();
                PROJECTID_D = ConfigurationManager.AppSettings["PROJECTID_D"].ToString();
                EQUIPMENTNUMBER_I = ConfigurationManager.AppSettings["EQUIPMENTNUMBER_I"].ToString();
                EQUIPMENTNUMBER_d = ConfigurationManager.AppSettings["EQUIPMENTNUMBER_D"].ToString();
                EQUIPMENTNUMBER_u = ConfigurationManager.AppSettings["EQUIPMENTNUMBER_U"].ToString();
                GUID_I = ConfigurationManager.AppSettings["GUID_I"].ToString();
                GUID_D = ConfigurationManager.AppSettings["GUID_D"].ToString();
                GUID_U = ConfigurationManager.AppSettings["GUID_U"].ToString();
                TRANSACTIONTYPE = ConfigurationManager.AppSettings["TRANSACTIONTYPE"].ToString();
                POLECLASS = ConfigurationManager.AppSettings["POLECLASS"].ToString();
                POLETYPE = ConfigurationManager.AppSettings["POLETYPE"].ToString();
                HEIGHT = ConfigurationManager.AppSettings["HEIGHT"].ToString();
                POLECNTYNAME = ConfigurationManager.AppSettings["POLECNTYNAME"].ToString();
                LATITUDE_I = ConfigurationManager.AppSettings["LATITUDE_I"].ToString();
                LONGITUDE_I = ConfigurationManager.AppSettings["LONGITUDE_I"].ToString();
                DIVISION = ConfigurationManager.AppSettings["DIVISION"].ToString();
                DISTRICT = ConfigurationManager.AppSettings["DISTRICT"].ToString();
                LOCDESC2 = ConfigurationManager.AppSettings["LOCDESC2"].ToString();
                CITY = ConfigurationManager.AppSettings["CITY"].ToString();
                ZIP = ConfigurationManager.AppSettings["ZIP"].ToString();
                BARCODE = ConfigurationManager.AppSettings["BARCODE"].ToString();
                MAPNAME = ConfigurationManager.AppSettings["MAPNAME"].ToString();
                MAPOFFICE = ConfigurationManager.AppSettings["MAPOFFICE"].ToString();
                STARTUPDATE_I = ConfigurationManager.AppSettings["STARTUPDATE_I"].ToString();
                STARTUPDATE_D = ConfigurationManager.AppSettings["STARTUPDATE_D"].ToString();
                JOINTPOLENBR = ConfigurationManager.AppSettings["JOINTPOLENBR"].ToString();
                ATTRIBUTENAME_U = ConfigurationManager.AppSettings["ATTRIBUTENAME_U"].ToString();
                OLDVALUE_U = ConfigurationManager.AppSettings["OLDVALUE_U"].ToString();
                NEWVALUE_U = ConfigurationManager.AppSettings["NEWVALUE_U"].ToString();
                #endregion

                #region ED15 Summary Configuration
                //ED15 Summary Configuration

                ed15SummStagingTable = ConfigurationManager.AppSettings["ED15SummaryStagingTable"].ToString();
                ed15StagingTableName = ConfigurationManager.AppSettings["ED15SummaryStagingTableName"].ToString();
                EFF_RATING_MACH_KW = ConfigurationManager.AppSettings["EFF_RATING_MACH_KW"].ToString();
                EFF_RATING_INV_KW = ConfigurationManager.AppSettings["EFF_RATING_INV_KW"].ToString();
                EFF_RATING_MACH_KVA = ConfigurationManager.AppSettings["EFF_RATING_MACH_KVA"].ToString();
                EFF_RATING_INV_KVA = ConfigurationManager.AppSettings["EFF_RATING_INV_KVA"].ToString();
                MAX_STORAGE_CAPACITY = ConfigurationManager.AppSettings["MAX_STORAGE_CAPACITY"].ToString();
                //WG1 changes 05/13/2022 start
                CHARGE_DEMAND_KW = ConfigurationManager.AppSettings["CHARGE_DEMAND_KW"].ToString();
                TOT_SYS_LIMITED_EXPORT_KW = ConfigurationManager.AppSettings["TOT_SYS_LIMITED_EXPORT_KW"].ToString();
                DERATED_TOT_NP_CAP_INV_KW = ConfigurationManager.AppSettings["DERATED_TOT_NP_CAP_INV_KW"].ToString();
                PROJECT_TOTAL_EXPORT_KW = ConfigurationManager.AppSettings["PROJECT_TOTAL_EXPORT_KW"].ToString();
                LIMITED = ConfigurationManager.AppSettings["LIMITED"].ToString();
                //WG1 changes 05/13/2022 end
                #endregion

                #region ED15 INDV Configuration
                quantity = ConfigurationManager.AppSettings["QUANTITY"].ToString();
                noOfPhases = ConfigurationManager.AppSettings["NUMBER_OF_PHASES"].ToString();
                enosEqupRef = ConfigurationManager.AppSettings["ENOS_EQUIP_REF"].ToString();
                enosProjRef = ConfigurationManager.AppSettings["ENOS_PROJ_REF"].ToString();
                ED15INDVStagingTable = ConfigurationManager.AppSettings["ED15INDVStagingTable"].ToString();
                ED15INDVStagingTableName = ConfigurationManager.AppSettings["ED15INDVStagingTableName"].ToString();
                ptc_Rating = ConfigurationManager.AppSettings["PTC_RATING"].ToString();
                invEffe = ConfigurationManager.AppSettings["INVERTER_EFFICIENCY"].ToString();
                npRating = ConfigurationManager.AppSettings["NAMEPLATE_RATING"].ToString();
                pf = ConfigurationManager.AppSettings["POWER_FACTOR"].ToString();
                effRatingKW = ConfigurationManager.AppSettings["EFF_RATING_KW"].ToString();
                effRatingKVA = ConfigurationManager.AppSettings["EFF_RATING_KVA"].ToString();
                ratedVoltage = ConfigurationManager.AppSettings["RATED_VOLTAGE"].ToString();
                ssReactance = ConfigurationManager.AppSettings["SS_REACTANCE"].ToString();
                ssResistance = ConfigurationManager.AppSettings["SS_RESISTANCE"].ToString();
                transReactance = ConfigurationManager.AppSettings["TRANS_REACTANCE"].ToString();
                transResistance = ConfigurationManager.AppSettings["TRANS_RESISTANCE"].ToString();
                subReactance = ConfigurationManager.AppSettings["SUBTRANS_REACTANCE"].ToString();
                subResistance = ConfigurationManager.AppSettings["SUBTRANS_RESISTANCE"].ToString();
                negReactance = ConfigurationManager.AppSettings["NEG_REACTANCE"].ToString();
                negResistance = ConfigurationManager.AppSettings["NEG_RESISTANCE"].ToString();
                zeroResistance = ConfigurationManager.AppSettings["ZERO_RESISTANCE"].ToString();
                zeroReactance = ConfigurationManager.AppSettings["ZERO_REACTANCE"].ToString();
                grdReacatnace = ConfigurationManager.AppSettings["GRD_REACTANCE"].ToString();
                grdResistance = ConfigurationManager.AppSettings["GRD_RESISTANCE"].ToString();
                ptoDate = ConfigurationManager.AppSettings["PTO_DATE"].ToString();
                //WG1 changes 05/13/2022 start
                npCapaicity = ConfigurationManager.AppSettings["NAMEPLATE_CAPACITY"].ToString();
                deratedRatingKW = ConfigurationManager.AppSettings["DERATED_RATING_PER_UNIT_KW"].ToString();
                dearatedTotalNPKW = ConfigurationManager.AppSettings["DERATED_INV_TOTAL_NP_KW"].ToString();
                usageAseKW = ConfigurationManager.AppSettings["EST_ANN_USAGE_AES_KW"].ToString();
                techTypeCD = ConfigurationManager.AppSettings["TECH_TYPE_CD"].ToString();
                sapAction = ConfigurationManager.AppSettings["SAPACTION"].ToString();
                limitexportplan = ConfigurationManager.AppSettings["LIMIT_EXPORT_PLAN"].ToString();
                limitedexportpcs = ConfigurationManager.AppSettings["LIMITED_EXPORT_PCS"].ToString();
                spcrdpcscert = ConfigurationManager.AppSettings["SP_CRDPCS_CERT"].ToString();
                maxexpctexport = ConfigurationManager.AppSettings["MAX_EXPCT_EXPORT"].ToString();
                operatingmode = ConfigurationManager.AppSettings["OPERATING_MODE"].ToString();

                //WG1 changes 05/13/2022 end


                //noOfPhases= ConfigurationManager.AppSettings["QUANTITY"].ToString(); 
                // = ConfigurationManager.AppSettings[""].ToString();
                // = ConfigurationManager.AppSettings[""].ToString();
                // = ConfigurationManager.AppSettings[""].ToString();
                // = ConfigurationManager.AppSettings[""].ToString();
                // = ConfigurationManager.AppSettings[""].ToString();
                // = ConfigurationManager.AppSettings[""].ToString();
                // = ConfigurationManager.AppSettings[""].ToString();
                // = ConfigurationManager.AppSettings[""].ToString();
                // = ConfigurationManager.AppSettings[""].ToString();
                // = ConfigurationManager.AppSettings[""].ToString();

                #endregion
            }
            catch (Exception ex)
            {
                Common._log.Error("Exception Occure while reading configuration from config file." + ex.Message);
            }

        }

        /// <summary>
        /// (V3SF)
        /// Load outbound Configurations
        /// </summary>
        public static void readConfigValues()
        {
            try
            {
                //Base Connection String

                // m4jf edgisrearch 919
                //oracleConnectionString = ConfigurationManager.AppSettings["OracleConnectionString"].ToString();
                oracleConnectionString = ReadEncryption.GetConnectionStr(ConfigurationManager.AppSettings["EDER_ConnectionStr"].ToString().ToUpper());

                intExecutionSummary = ConfigurationManager.AppSettings["IntExecutionSummaryExePath"];

                GISProcessedTime = ConfigurationManager.AppSettings["GISProcessedTime"].ToString();

                //Last Run Date Table Columns (INTEGRATION_LASTRUN_DATE)
                ILD_INTERFACE_NAME = ConfigurationManager.AppSettings["ILD_INTERFACE_NAME"].ToString();
                ILD_LASTRUN_DATE = ConfigurationManager.AppSettings["ILD_LASTRUN_DATE"].ToString();

                //Configration Tables
                INTEGRATION_FIELD_MAPPING = ConfigurationManager.AppSettings["INTEGRATION_FIELD_MAPPING"].ToString();
                INTERFACE_STAGINGTABLE_CONFIG = ConfigurationManager.AppSettings["INTERFACE_STAGINGTABLE_CONFIG"].ToString();
                INTEGRATION_LASTRUN_DATE = ConfigurationManager.AppSettings["INTEGRATION_LASTRUN_DATE"].ToString();

                //INTERFACE_STAGINGTABLE_CONFIG Columns Names
                ISC_interface_name = ConfigurationManager.AppSettings["ISC_interface_name"].ToString();
                ISC_staging_table = ConfigurationManager.AppSettings["ISC_staging_table"].ToString();
                ISC_database_name = ConfigurationManager.AppSettings["ISC_database_name"].ToString();
                ISC_interface_number = ConfigurationManager.AppSettings["ISC_interface_number"].ToString();
                ISC_batch_size = ConfigurationManager.AppSettings["ISC_batch_size"].ToString();

                //INTEGRATION_FIELD_MAPPING Column Names
                IFM_INTERFACE_NAME = ConfigurationManager.AppSettings["IFM_INTERFACE_NAME"].ToString();
                IFM_GIS_FIELD = ConfigurationManager.AppSettings["IFM_GIS_FIELD"].ToString();
                IFM_SAP_FIELD = ConfigurationManager.AppSettings["IFM_SAP_FIELD"].ToString();
                IFM_SAP_SEQUENCE = ConfigurationManager.AppSettings["IFM_SAP_SEQUENCE"].ToString();

                //Staging Table Common Columns
                SAPRecordIdField = ConfigurationManager.AppSettings["SAPRecordIdField"].ToString();
                SAPBatchIdField = ConfigurationManager.AppSettings["SAPBatchIdField"].ToString();
                GISErrorField = ConfigurationManager.AppSettings["GISErrorField"].ToString();
                GISProcessFlagField = ConfigurationManager.AppSettings["GISProcessFlagField"].ToString();

                //Post Configrations
                PostContentType = ConfigurationManager.AppSettings["PostContentType"].ToString();
                PostMethod = ConfigurationManager.AppSettings["PostMethod"].ToString();
                Credentials_serviceUserName = Convert.ToString(ConfigurationManager.AppSettings["Credentials_serviceUserName"]).Split('@')[0];
                Credentials_servicePassword = ReadEncryption.GetPassword(Convert.ToString(ConfigurationManager.AppSettings["Credentials_serviceUserName"]));

                //Response Values
                PostResponse_metadata = ConfigurationManager.AppSettings["PostResponse_metadata"].ToString();
                PostResponse_results = ConfigurationManager.AppSettings["PostResponse_results"].ToString();
                PostResponseStatus_Success = ConfigurationManager.AppSettings["PostResponseStatus_Success"].ToString();
                PostResponseStatus_Fail = ConfigurationManager.AppSettings["PostResponseStatus_Fail"].ToString();
            }
            catch (Exception ex)
            {
                Common._log.Error("Unable to Read Outound Configration :: " + ex.Message + " from " + ex.StackTrace);
                throw ex;
            }
        }

    }
}
