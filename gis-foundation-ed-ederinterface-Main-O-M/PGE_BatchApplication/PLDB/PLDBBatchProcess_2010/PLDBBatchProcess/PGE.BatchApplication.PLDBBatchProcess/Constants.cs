using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGE_DBPasswordManagement;
using System.Configuration;

namespace PGE.BatchApplication.PLDBBatchProcess
{
    public static class PLDBBatchConstants
    {
        public const string CONFIG_FILENAME = "PGE.BatchApplication.PLDBBatchProcess.Config.xml";
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
        public const string PLDB_INFO_QUERY_SQL = "SQLFile\\PLD_INFO_QUERY_New.sql";
        public const string CONFIG_LOG_FOLDER = "LOG_FOLDER";
        public const string CONFIG_LOG_FILE = "LOG_FILE";
        public const string CONFIG_POLE_FILTER = "POLE_FILTER";
        // m4jf edgisrearch 919
        // public const string CONFIG_EDGIS_SDE = "EDGIS_SDE";
        public const string CONFIG_EDGIS_SDE = "EDER_SDEConnection";

        public const string CONFIG_PLD_INFO_SEQUENCE = "PLD_INFO_OBJECTID_SEQUENCE";
        public const string CONFIG_PLD_INFO_SRID = "PLD_INFO_SRID";
        public const string CONFIG_SQL_SLEEP_INTERVAL = "SQL_SLEEP_INTERVAL";
        // m4jf - pldb update
        public const string CONFIG_SQL_CONN_TIMEOUT = "SQL_CONN_TIMEOUT";
        // public const string CONFIG_WIP_SDE = "WIP_SDE";
        public const string CONFIG_WIP_SDE = "WIP_SDEConnection";

        public const string CONFIG_PLDB_CONNECTION = "PLDB";
        public static string CONFIG_PLDB_CONNECTION_New = "Initial Catalog=PLDB;" + ReadEncryption.GetConnectionStr(ConfigurationManager.ConnectionStrings["PLDB"].ConnectionString.ToUpper());

        //public const string CONFIG_WIP_CONNECTION = "WIP";
        public static string CONFIG_WIP_CONNECTION = "Connection Timeout=3600;Incr Pool Size=5;Decr Pool Size=2;"+ReadEncryption.GetConnectionStr(ConfigurationManager.AppSettings["WIP_ConnectionStr"].ToUpper());



    }
}
