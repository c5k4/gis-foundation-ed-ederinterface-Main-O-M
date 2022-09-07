using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PLDBBatchProcess
{
    public static class PLDBBatchConstants
    {
        public const string CONFIG_FILENAME = "PLDBBatchProcess.Config.xml";
        public const string PLDB_SQL_FILE_PREFIX = "PLDBBatchSQLfile"; 
        public const string PLD_INFO_FC_NAME = "WEBR.PLD_INFO";
        public const string HEIGHTS_LAYER_NAME = "Heights"; 
        public const double COINCIDENCE_THRESHOLD = 0.001;
        public const string CONSTRUCTION_TYPE_DEFAULT = "HORIZONTAL";
        public const string SUPPORT_STRUCTURE_FC = "edgis.supportstructure";
        public const string EDGIS_POLES_TABLE = "SYNC_EDGIS_POLES";
        public const string PLDB_POLES_TABLE = "SYNC_PLDB_POLES";
        public const string PLD_INFO_TEMP_TABLE = "WEBR.PLD_INFO_TEMP"; 
        public const string LAT_LONG_UPDATES_TABLE = "SYNC_LAT_LONG_UPDATES";
        public const string OCALCPROANALYSIS_TABLE = "OCalcProAnalysis";
        public const string OCALCPROANALYSISROOT_TABLE = "OCalcProAnalysisRoot";
        public const string UPDATE_LAT_LONG_STORED_PROC = "dbo.spsync_UpdateLatLong";        
        public const string OPA_LAT_FIELD = "Latitude";
        public const string OPA_LONG_FIELD = "Longitude";
        public const string OPAR_LAT_FIELD = "Latitude";
        public const string OPAR_LONG_FIELD = "Longitude";        
        public const string EDGIS_POLES_FIELDS = "PLDBID,PGE_GLOBALID,PGE_REPLACEGUID,PGE_SAPEQUIPID,Latitude,Longitude";
        public const string LAT_LONG_UPDATE_FIELDS = "PLDBID,EDGIS_GLOBALID,Elevation,Latitude,Longitude";
        public const int INSERTS_PER_FILE = 25000;
        public const int UPDATES_PER_FILE = 500;        
        public const string MAPDOCUMENT_FILENAME = "ElevationMap.mxd";
        //public const string BAT_FILENAME = "SQL_RUNNER.bat";
        public const string PLDB_INFO_QUERY_SQL = "PLD_INFO_QUERY_New.sql";
        public const string CREATE_PLD_INFO_TEMP_TABLE_SQL = "CREATE_PLD_INFO_TEMP.sql"; 
        public const string CREATE_PLDB_POLES_TABLE_SQL = "CREATE_PLDB_POLES_TABLE.sql";
        public const string CREATE_EDGIS_POLES_TABLE_SQL = "CREATE_EDGIS_POLES_TABLE.sql";
        public const string CREATE_LAT_LONG_UPDATES_TABLE_SQL = "CREATE_LAT_LONG_ UPDATE_TABLE.sql";
        public const string LAT_LONG_DISCREPANCY_QUERY_SQL = "LAT_LONG_DISCREPANCY_QUERY.sql";
        public const string ID_DISCREPANCY_QUERY_SQL = "ID_DISCREPANCY_QUERY.sql";
        public const string DECOMM_DISCREPANCY_QUERY_SQL = "DECOMM_DISCREPANCY_QUERY.sql";
        public const string CONFIG_LOG_FOLDER = "LOG_FOLDER";
        public const string CONFIG_LOG_FILE = "LOG_FILE";
        public const string CONFIG_POLE_FILTER = "POLE_FILTER";
        public const string CONFIG_EDGIS_SDE = "EDGIS_SDE";
        public const string CONFIG_PLD_INFO_SEQUENCE = "PLD_INFO_OBJECTID_SEQUENCE";
        public const string CONFIG_PLD_INFO_SRID = "PLD_INFO_SRID";
        public const string CONFIG_SQL_SLEEP_INTERVAL = "SQL_SLEEP_INTERVAL";
        public const string CONFIG_WIP_SDE = "WIP_SDE";
        public const string CONFIG_DECOMM_WHERECLAUSE = "DECOMM_WHERECLAUSE";
        public const string CONFIG_PLDB_CONNECTION = "PLDB";
        public const string CONFIG_WIP_CONNECTION = "WIP";
        public const string CONFIG_ENABLE_POP_EDGIS_POLES = "ENABLE_ENABLE_POP_EDGIS_POLES"; 
        public const string CONFIG_ENABLE_POP_PLDB_POLES = "ENABLE_ENABLE_POP_PLDB_POLES"; 
        public const string CONFIG_ENABLE_UPDATE_LOCATION = "ENABLE_UPDATE_LOCATION";
        public const string CONFIG_ENABLE_DECOMMISSION = "ENABLE_DECOMMISSION";
        public const string CONFIG_ENABLE_UPDATE_IDS = "ENABLE_UPDATE_IDS";
        

    }
}
