using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.Interface.ENOS.Batch
{
    public enum ENOSExitCodes
    {
        success = 0, 
        failure = 1 
    }

    public enum SourceType
    {
        sourceTypeENOS_Stage = 0,
        sourceTypeENOS_Error = 1
    }

    public enum EquipmentType
    {
        equipTypePV = 0,
        equipTypeWindTurbine = 1,
        equipTypePVWithInverter = 2, 
        equipTypeWindTurbineWithInverter = 3, 
        equipTypeInverter = 4
    }

    public enum GenType
    {
        genTypeNone = 0,
        genTypeDC = 1,
        genTypeMixed = 2, 
        genTypeSync = 3, 
        genTypeInduc = 4
    }

    class ENOSCommon
    {
        //TABLES / FEATURECLASSES 
        public const string GENERATION_TBL = "EDGIS.GENERATION";
        public const string SERVICEPOINT_TBL = "EDGIS.SERVICEPOINT";
        public const string TRANSFORMER_FC = "EDGIS.TRANSFORMER";
        public const string SERVICELOCATION_FC = "EDGIS.SERVICELOCATION";
        public const string ENOS_STAGE_TBL = "PGEDATA.ENOS_STAGE";
        public const string ENOS_ERROR_TBL = "PGEDATA.ENOS_ERROR";
        public const string ENOS_ARCHIVE_TBL = "PGEDATA.ENOS_ARCHIVE";
        public const string CEDSA_METER_TBL = "CEDSADATA.METER";

        //GENERATION TABLE FIELDS 
        public const string SERVICEPOINTID_FLD = "servicepointid"; 
        public const string ENOS_REF_ID_FLD = "enos_ref_id"; 
        public const string GENTYPE_FLD = "gentype"; 
        public const string NOTES_FLD = "notes";
        public const string ENOS_EQP_ID_FLD = "enos_eqp_id";
        public const string INV_INVERTERID_FLD = "inv_inverterid";
        public const string MANF_CD_FLD = "manf_cd";
        public const string MODEL_CD_FLD = "model_cd";
        public const string DC_RATING_FLD = "dc_rating";
        public const string QUANTITY_FLD = "quantity";
        public const string SERVICEPOINTGUID_FLD = "servicepointguid";
        public const string POWER_SOURCE_CD_FLD = "power_source_cd";
        public const string STATUS_CD_FLD = "status_cd";
        public const string KW_OUT_FLD = "kw_out";
        public const string NP_KVA_FLD = "np_kva";
        public const string LASTUSER_FLD = "lastuser";
        public const string DATECREATED_FLD = "datecreated";

        //ENOS FIELDS (in ENOS_STAGE, ENOS_ARCHIVE, ENOS_ERROR tables) 
        public const string ET_ENOS_REF_ID_FLD = "ENOS_REF_ID"; 
        public const string ET_ENOS_STATUS_FLD = "ENOS_STATUS";
        public const string ET_EQUIPMENT_ID_FLD = "EQUIPMENT_ID";
        public const string ET_EQUIPMENT_TYPE_FLD = "EQUIPMENT_TYPE";
        public const string ET_GENERATION_STATUS_FLD = "GENERATION_STATUS";
        public const string ET_INVERTERID_FLD = "INVERTERID";
        public const string ET_MANUFACTURER_FLD = "MANUFACTURER";
        public const string ET_MODEL_FLD = "MODEL";
        public const string ET_POWER_SOURCE_FLD = "POWER_SOURCE";
        public const string ET_QUANTITY_FLD = "QUANTITY";
        public const string ET_RATING_FLD = "RATING";
        public const string ET_SERVICE_POINT_ID_FLD = "SERVICE_POINT_ID";
        public const string ET_STATUS_FLD = "STATUS";
        public const string ET_ERROR_DESCRIPTION_FLD = "ERROR_DESCRIPTION"; 
        public const string ET_ERROR_DATE_FLD = "ERROR_DATE";
        public const string ET_LOCAL_OFFICE_FLD = "LOCAL_OFFICE";
        public const string ET_ARCHIVE_DATE_FLD = "ARCHIVE_DATE";

        //OTHER CONSTANTS 
        public const int PARENT_VERISON_INDEX = 99;
        public const string STATUS_CD_VALUE = "ON";  
        public const string ORACLE_SYSDATE = "SYSDATE";
        public const string DEFAULT_VERSION = "SDE.DEFAULT"; 
    }
}
