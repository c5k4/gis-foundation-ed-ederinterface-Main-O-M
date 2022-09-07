using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using PGE_DBPasswordManagement;
//using System.Threading.Tasks;

namespace PGE.BatchApplication.IGPPhaseUpdate
{
    public class ReadConfigurations
    {

         public static string LOGCONFIG = System.Configuration.ConfigurationManager.AppSettings["LogConfigName"];
         public static string DAPHIEFilePath = System.Configuration.ConfigurationManager.AppSettings["DAPHIE_FolderPath"].ToString();
         public static string RelatedFeatureUpdateSPName = ConfigurationManager.AppSettings["RELATED_INFO_SP_NAME_CONDINFO"];
         public static string RelatedFeatureUpdateSPName_TransUnit = ConfigurationManager.AppSettings["RELATED_INFO_SP_NAME_TRANSUNIT"];
         public static string UpdateUnProcessedPhase_SPName = ConfigurationManager.AppSettings["UPDATE_UNPROCESSED_PHASE_SP_NAME"];
         public static string UpdateOpenPointPhase_SPName = ConfigurationManager.AppSettings["UPDATE_OPENPOINT_PHASE_SP_NAME"];
         public static string CSVFILEPATH= ConfigurationManager.AppSettings["CSVFILEPATH"];
         public static string OutPutTableName = ConfigurationManager.AppSettings["TraceResultTableName"];

        // M4JD EDGISREARCH 919
        // public static string SDEWorkSpaceConnString = ConfigurationManager.AppSettings["SDEWorkSpaceConnString"];
        public static string SDEWorkSpaceConnString = ReadEncryption.GetSDEPath(ConfigurationManager.AppSettings["EDER_SDEConnection"].ToUpper());


        //public static string OracleConnString = ConfigurationManager.AppSettings["OracleConnectionString"];
        public static string OracleConnString = ReadEncryption.GetConnectionStr(ConfigurationManager.AppSettings["EDER_ConnectionStr"].ToUpper());


        public static string ChangeUserName = ConfigurationManager.AppSettings["CHANGEUSERNAME"];
         public static int MaxNumProcessors = Convert.ToInt32(ConfigurationManager.AppSettings["MAX_PROCESS_NUMBERS"]);
         public static int ThresholdProcessCount = Convert.ToInt32(ConfigurationManager.AppSettings["THRESHOLD_PROCESS_COUNT"]);
         public static int ProcessingHours = Convert.ToInt32(ConfigurationManager.AppSettings["PROCESSING_HOURS"]);
         public static int ProcessingHours_UpdateSession = Convert.ToInt32(ConfigurationManager.AppSettings["PROCESSING_HOURS_UPDATESESSION"]);
         public static string FeederNetworkTraceTableName = ConfigurationManager.AppSettings["TRACE_TABLENAME"];
         public static string FeederNetworkTraceTableName_DataMart = ConfigurationManager.AppSettings["TRACE_TABLENAME_DATAMART"];
         public static string Stage_To_Be_Completed = ConfigurationManager.AppSettings["STAGE_TO_BE_COMPLETED"];
         public static string Status_To_Be_Skipped = ConfigurationManager.AppSettings["STATUS_TO_BE_SKIPPED"];
         public static string PrimaryFeatureClasses = ConfigurationManager.AppSettings["PRIMARY_NETWORK_FEATURE_CLASSES"];
         public static string CheckGUIDSP = System.Configuration.ConfigurationManager.AppSettings["checkGUID"].ToString();
         public static string ChecksubtypeSP = System.Configuration.ConfigurationManager.AppSettings["checkSubtype"].ToString();
         public static string IGPSequence = System.Configuration.ConfigurationManager.AppSettings["IGP_SEQUENCE"].ToString();
         public static string POST_QUEUE_STATE = System.Configuration.ConfigurationManager.AppSettings["GDBM_POST_QUEUE_STATE"].ToString();
         public static string GDBMLimit = System.Configuration.ConfigurationManager.AppSettings["GDBM_LIMIT"].ToString();
         public static string GDBMPriority = System.Configuration.ConfigurationManager.AppSettings["GDBM_PRIORITY"].ToString();
         public const string GDBITEMSTableName = "SDE.GDB_ITEMS";
         public static string ApplicationUser = System.Configuration.ConfigurationManager.AppSettings["APPLICATION_USER"].ToString();
         public static string DeviceList=  System.Configuration.ConfigurationManager.AppSettings["DEVICE_LIST"].ToString();
         public const string GUIDFIELDNAME ="GLOBALID";
         public const string RegulatorGuidFieldName = "REGULATORGUID";
         public const string StepDownGuidFieldName = "STEPDOWNGUID";
         public const string OBJECTIDFIeldName = "OBJECTID";
         public const string PhasingVerifiedStatusFldName = "PHASINGVERIFIEDSTATUS";
         public const string PhaseDesignationFieldName = "PHASEDESIGNATION";
         public const string NORMALPOSITION_A_FieldName = "NORMALPOSITION_A";
         public const string NORMALPOSITION_B_FieldName = "NORMALPOSITION_B";
         public const string NORMALPOSITION_C_FieldName = "NORMALPOSITION_C";
         public const string NORMALPOSITION_FieldName = "NORMALPOSITION";
         public const string TransformerType_FieldName = "TRANSFORMERTYPE";
         public const string TransformerGuid_FieldName = "TRANSFORMERGUID";
         public const string OperatingNumber_FieldName = "OPERATINGNUMBER";
         public static IList<string> AutoPhaseAssignClassList=FillAutoPhaseAssignList() ;
         public const string FEATURE_CLASSES_TO_BE_UPDATED = "FEATURE_CLASSES_TO_BE_UPDATED";
         public const string IGPPHID_04_Error = "Null Value/Values";
         public const string IGPPHID_01_Error = "Phase designation out of range";
         public const string defaultVersionName = "SDE.DEFAULT";
         public static string sQAQCErrorFile = string.Empty;
         public static string sDMSErrorFile = string.Empty;
         public static int DMSRuleErrorCount = -1;
         public static string DMSTABLEUPDATESPNAME = GetValue("UPDATE_DMS_TABLE_SP_NAME");
         public static string DMSATTRUPDATESPNAME = GetValue("UPDATE_DMS_ATTR_SP_NAME");
         public static string DMSCHECKPHASEMISMATCHRULESPNAME = GetValue("CHECK_DMS_PHASEMISMATCH_SP_NAME");
        //GDBM Tables
         
         
        #region Project Tables
         public static string DAPHIETransformerTableName = GetValue("DAPHIE_TRANSFORMER_TABLENAME");
         public static string DAPHIEConductorTableName = GetValue("DAPHIE_CONDUCTORS_TABLENAME");
         public static string DAPHIEMeterTableName = GetValue("DAPHIE_METERS_TABLENAME");
         public static string UnprocessedTableName = GetValue("UNPROCESSEDRECORDS_TABLENAME");
         public static string StatisticalReportTableName = GetValue("STATISTICALREPORT_TABLENAME");
         public static string QAQCTableName = GetValue("QAQC_TABLENAME");
         public static string ConfigMergePhaseTableName = GetValue("CONFIG_MERGE_PHASE_TABLENAME");
         public static string ConfigDetailsTableName = GetValue("CONFIG_DETAILS_TABLENAME");
         //public static string ConfigTransformerPhaseTableName = GetValue("CONFIG_TRANSFORMER_PHASE_TABLENAME");
         public static string ValidationSeverityMapTableName = GetValue("CONFIG_VALIDATION_SEVERITY_TABLENAME");
         public static string DAPHIEFileDetTableName = GetValue("DAPHIEFILE_DET_TABLENAME");
       //  public static string DEENERGIZEDTableName = GetValue("DEENERGIZED_INFO_TABLENAME");
          

         #endregion
         public struct Process_TableName
         {
             public const string MM_SessionTableName = "PROCESS.MM_SESSION";
             public const string MM_CurrentPxStateTableName = "process.MM_PX_CURRENT_STATE";
             public const string MM_PxVersionTableName = "process.MM_PX_VERSIONS";
             public const string MM_PxUserTableName = "process.mm_px_user";
             public const string MM_PxStateTableName = "PROCESS.MM_PX_STATE";
            
         }

         public struct Conductors_ViewName
         {
             public const string PrimaryUGConductorViewName = "EDGIS.ZZ_MV_PRIUGCONDUCTOR";
             public const string PrimaryOHConductorViewName = "EDGIS.ZZ_MV_PRIOHCONDUCTOR";
             public const string SecondaryUGConductorViewName = "EDGIS.ZZ_MV_SECUGCONDUCTOR";
             public const string SecondaryOHConductorViewName = "EDGIS.ZZ_MV_SECOHCONDUCTOR";
             public const string DistBusBarViewName = "EDGIS.ZZ_MV_DISTBUSBAR";
             public const string PrimaryRiserViewName = "EDGIS.ZZ_MV_PRIMARYRISER";
             public const string TransformerLeadViewName = "EDGIS.ZZ_MV_TRANSFORMERLEAD";
         }
         public struct Conductors
         {
             public const string PrimaryUGConductorClassName = "EDGIS.PRIUGCONDUCTOR";
             public const string PrimaryOHConductorClassName = "EDGIS.PRIOHCONDUCTOR";
             public const string SecondaryUGConductorClassName = "EDGIS.SECUGCONDUCTOR";
             public const string SecondaryOHConductorClassName = "EDGIS.SECOHCONDUCTOR";
             public const string TransformerLeadClassName = "EDGIS.TRANSFORMERLEAD";
             public const string DCConductorClassName = "EDGIS.DCCONDUCTOR";
         }
         public struct Devices_ViewName
         {
             public const string TransformerViewName = "EDGIS.ZZ_MV_TRANSFORMER";
             public const string TransformerUnitViewName = "EDGIS.ZZ_MV_TRANSFORMERUNIT";
             public const string SwitchViewName = "EDGIS.ZZ_MV_SWITCH";
             public const string OPenPointViewName = "EDGIS.ZZ_MV_OPENPOINT";
             public const string DPDViewName = "EDGIS.ZZ_MV_DYNAMICPROTECTIVEDEVICE";
             public const string FUSEViewName = "EDGIS.ZZ_MV_FUSE";
             public const string STEPDOWNViewName = "EDGIS.ZZ_MV_STEPDOWN";
             public const string StepDownUnitViewName = "EDGIS.ZZ_MV_STEPDOWNUNIT";
             public const string VOLTAGEREGULATORViewName = "EDGIS.ZZ_MV_VOLTAGEREGULATOR";
             public const string VoltageRegUnitTableViewName = "EDGIS.ZZ_MV_VOLTAGEREGULATORUNIT";
             public const string TieViewName = "EDGIS.ZZ_MV_TIE";
             public const string FAULTINDICATORViewName = "EDGIS.ZZ_MV_FAULTINDICATOR";
             public const string ElectricNetJunctionViewName = "EDGIS.ZZ_MV_ELECTRICDISTNETWORK_JUNCTIONS";
             public const string PrimaryMeterViewName = "EDGIS.ZZ_MV_PRIMARYMETER";
             public const string CapacitorBankViewName = "EDGIS.ZZ_MV_CAPACITORBANK";
         }
         public struct Devices
         {
             public const string TransformerClassName = "EDGIS.TRANSFORMER";
             public const string VOLTAGEREGULATORClassName = "EDGIS.VOLTAGEREGULATOR";
             public const string StepDownTableName = "EDGIS.STEPDOWN";          
             public const string SwitchClassName = "EDGIS.SWITCH";
             public const string FUSEClassName = "EDGIS.FUSE";
             public const string DPDClassName = "EDGIS.DYNAMICPROTECTIVEDEVICE";
             public const string OPENPOINTClassName = "EDGIS.OPENPOINT";
             public const string TieClassName = "EDGIS.TIE";
         }
         public struct RelatedTables
         {           
             
             public const string STEPDOWNUNITableName = "EDGIS.STEPDOWNUNIT";
             public const string VOLTAGEREGULATORUNITTableName = "EDGIS.VOLTAGEREGULATORUNIT";
             public const string TransformerUnitName = "EDGIS.TRANSFORMERUNIT";
             public const string PriOHCONDUCTORINFOTableName = "EDGIS.PRIOHCONDUCTORINFO";
             public const string PriUGCONDUCTORINFOTableName = "EDGIS.PRIUGCONDUCTORINFO";
             public const string SECOHCONDUCTORINFOTableName = "EDGIS.SECOHCONDUCTORINFO";
             public const string SECUGCONDUCTORINFOTableName = "EDGIS.SECUGCONDUCTORINFO";

         }
         public struct DeliveryPoints
         {
             public const string ServiceLocationClassName = "EDGIS.SERVICELOCATION";
             public const string OpenPointClassName_Sec = "EDGIS.OPENPOINT";
         }

         public struct DeliveryPoints_View
         {
             public const string ServiceLocationClassName = "EDGIS.ZZ_MV_SERVICELOCATION";
         }
        
         public struct DAPHIETablesFields
         {
             public const string ConductorPhaseField = "PHASE_PREDICTION";
             public const string ConductorGUIDField = "GLOBALID";
             public const string ServicePhaseField = "PHASE_PREDICTION";
             public const string ServiceGUIDField = "GLOBALID";
             public const string Service_TransformerGuidField = "TRANSFORMERGUID";
             public const string CircuitIdField = "CIRCUITID";

             public const string Current_StatusField = "CURRENT_STATUS";
             public const string StatusField = "STATUS";
             public const string SessionNameField = "SESSION_NAME";
             public const string Serial_NoField = "SERIAL_NO";
             public static string BATCH_NUMBER = "BATCHID";
         }

         public struct TraceNGDBTablesFields
         {
             public const string PhysicalName_FieldName = "PHYSICALNAME";
             public const string ToFeatureOid_FieldName = "TO_FEATURE_OID";
             public const string ToFeatureEid_FieldName = "TO_FEATURE_EID";
             public const string FromFeatureEid_FieldName = "FROM_FEATURE_EID";
             public const string ToFeatureGuid_FieldName = "TO_FEATURE_GLOBALID";
             public const string ToFeatureFeederInfo_FieldName = "TO_FEATURE_FEEDERINFO";
             public const string MinBranch_FieldName = "MIN_BRANCH";
         }

         public struct ConfigTablesFields
         {
             public const string ReturnPhase_FieldName = "RETURN_PHASE";
             public const string ExistingPhase_FieldName = "EXISTING_PHASE";
             public const string ServicePhase_FieldName = "SERVICE_PHASE";
             public const string DaphiePhase_FieldName = "DAPHIE_PHASE";
             public const string PhaseUpdated_FieldName = "PHASE_TO_BE_UPDATED";
             public const string Transformer_Code_FieldName = "CODE";
             public const string Transformer_Phase_FieldName = "PHASE";
             public const string Validation_Name_FieldName = "NAME";
             public const string Validation_Severity_FieldName = "SEVERITY";
             public const string Validation_EnabledClasses_FieldName = "ENABLED_CLASSES";
         }

         public struct UnprocessedTablesFields
         {
             public const string ValuePhase_FieldName = "VALUE";
             public const string FeatureGuid_FieldName = "FEATURE_GUID";
             public const string Processed_FieldName = "PROCESSED";
             public const string CircuitId_FieldName = "CIRCUITID";
             public const string BatchId_FieldName = "BATCHID";
             public const string REC_TYPE_FieldName = "REC_TYPE";
         }

         public struct STATUS_DAPHIEFILE_DET
         {
             public const string Status_DELETED = "DELETED";
             
             public const string Status_Calculated = "CALCULATED";
             public const string Status_CalculationError = "CALCULATION_ERROR";
             public const string Status_Updated = "UPDATED";
             public const string Status_UpdateError = "UPDATE_ERROR";
             public const string Status_QAQCPassed = "QAQC_PASSED";
             public const string Status_USERQUEUE = "USER_QUEUE";
             public const string Status_POSTED = "POSTED";
             public const string Status_POSTERROR = "POST_ERROR";
             public const string Status_RECONCILEERROR = "RECONCILE_ERROR";
             public const string Status_CONFLICT = "CONFLICT";
             public const string Status_Loaded = "LOADED";
             public const string Status_LoadError = "LOAD_ERROR";
             public const string Status_ValidateError = "VALIDATE_ERROR";
             public const string Status_VALIDATED = "VALIDATED";
             public const string CurrentStatus_Assigned = "ASSIGNED";
             public const string CurrentStatus_InProgress = "IN_PROGRESS";
             public const string CurrentStatus_CycleCompleted = "CYCLE_COMPLETED";
             public const string CurrentStatus_OverFlow = "OVER_FLOW";
             public const string status_INVALIDJSON = "INVALID_JSON";
             public const string Status_LimitExceeded = "LIMIT_EXCEEDED";
             public const string CurrentStatus_InProgress_Session = "IN_PROGRESS_SESSION";
             public const string CurrentStatus_Assigned_Session = "ASSIGNED_SESSION";
             public const string CurrentStatus_ProcessCompleted = "PROCESS_COMPLETED";
         }
       
         public struct Validation_ModelNames_Class
         {
             public const string CGC12ClassMN = "PGE_CGC12";
             public const string PGETransformer = "PGE_TRANSFORMER";
             public const string PrimaryMeter = "PGE_PRIMARYMETER";
             public const string PGESecondaryLoadPoint = "PGE_SECONDARYLOADPOINT";
             public const string SEC_GRID_SPOT_NTWRK = "SEC_GRID_SPOT_NTWRK";
         }

         public struct Validation_ModelNames_Field
         {
             public const string FieldModelCGC12 = "PGE_CGC12";
             public const string Status = "PGE_STATUS";
             public const string FeederManagerNonTraceable = "FDRMGRNONTRACEABLE";
             public const string PhaseDesignation = "PHASEDESIGNATION";
         }

         public struct Validation_DomainNames
         {
             public const string PhaseDesignationDomainName = "Phase Designation";
         }

         public struct ERROR_MESSAGES
         {
             public const string ERROR_CODE_01 = "No Service Locations found in JSON for three phase Transformer:DAPHIE";
             public const string ERROR_CODE_02 = "No Secondary Conductors found in Trace table for three phase Transformer:TECHNICAL";
             public const string ERROR_CODE_03 = "No three phase Service Location found in JSON for three phase Transformer:DAPHIE";
             public const string ERROR_CODE_04 = "No three phase Secondary Conductor found for three phase Transformer:TECHNICAL";
             public const string ERROR_CODE_05 = "No Secondary Conductors found in Trace table for Transformer:TECHNICAL";
             public const string ERROR_CODE_06 = "ABC phase found in secondary conductor of single/two phase transformer:TECHNICAL";
             public const string ERROR_CODE_07 = "Feature is having status as Field Verified.:TECHNICAL";
             public const string ERROR_CODE_08 = "Feature is having status as Office Verified.:TECHNICAL";
             public const string ERROR_CODE_09 = "Feature not found in Database:TECHNICAL";
             public const string ERROR_CODE_10 = "Phase information for this asset is not available in JSON, tried deriving within GIS but phase prediction was not available for immediate conductor.:DAPHIE";
             public const string ERROR_CODE_11 = "Feature is not updated as erroneous immediate upstream conductor record is coming from DAPHIE and Error = :DAPHIE";
             public const string ERROR_CODE_12 = "Source Conductor Phase is not recieved from JSON:DAPHIE";
             public const string ERROR_CODE_13 = "No Immediate Upstream Conductor Present for the Transformer Device:TECHNICAL";
             public const string ERROR_CODE_14 = "Phase Mismatch Error:DAPHIE";
             public const string ERROR_CODE_15 = "For Three Phase transformer - Service Location available in Trace Table but not found in JSON:DAPHIE";
             public const string ERROR_CODE_16 = "Feature is not updated as erroneous immediate downstream conductor record is coming from DAPHIE and Error = :DAPHIE";
             public const string ERROR_CODE_17 = "Phase information for this asset is not available in JSON, tried deriving from immediate upstream conductor but erroneous upstream conductor is coming from DAPHIE.:DAPHIE";
             public const string ERROR_CODE_18 = "DAPHIE Phase prediction did not match with GIS subtype.:DAPHIE";
             public const string ERROR_CODE_19 = "Phase conversion for transformer is not valid from three phase to other phase.:DAPHIE";
             public const string ERROR_CODE_20 = "Phase conversion for transformer is not valid from other phase to three phase.:DAPHIE";
            
         }



        public static string GetValue(string key)
        {
            string setting = null;
            try
            {
                setting = System.Configuration.ConfigurationManager.AppSettings[key];
            }
            catch (Exception exp)
            {
                (new Common())._log.Error("Exception occurred." + exp.Message.ToString());
            }
            return setting;
        }
        
        public struct NormalPosition
        {
            public const string Open = "0";
            public const string Close = "1";
            public const string NotApplicable = "2";
        }
        public struct PhasingStatus
        {
            public const string FieldVerified = "FIELDVERIFIED";
            public const string OfficeVerified = "OFFICEVERIFIED";
        }
        public struct ObjectClassType
        {
            public const string FeatureClassType = "FEATURECLASS";
            public const string TableType = "TABLE";
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
                catch (Exception exp)
                {
                    (new Common())._log.Error("Exception occurred." + exp.Message.ToString());
                }
            }
            return output;
        }
        //Fill AutoPhase AssignList
        private static IList<string> FillAutoPhaseAssignList()
        {
            IList<string> _retval = new List<string>();
            try
            {
                string GetValuefromConfig=GetValue("AUTOPHASEASSIGNCLASS").ToUpper();
                if(!string.IsNullOrEmpty(GetValuefromConfig))
                {
                    _retval =GetValuefromConfig.Split(',') ;
                }
            }
            catch (Exception exp)
            {
              (new Common())._log.Error("Exception occurred." + exp.Message.ToString());
            }

            return _retval;
        }

        
    }
}
