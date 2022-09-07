using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGE.Interface.Integration.DMS.Manager;
using Oracle.DataAccess.Client;
using System.Data;
using PGE.Common.Delivery.Diagnostics;
using PGE_DBPasswordManagement;

namespace PGE.Interface.Integration.DMS.Common
{
    /// <summary>
    /// Class with methods for reading and parsing data out of the App.config file. It also contains some common
    /// config settings.
    /// </summary>
    public class Configuration
    {
        private static Log4NetLogger _log4 = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "ED50.log4net.config");

        private static Dictionary<string, int> _fcids;
        private static Dictionary<string, int> _subfcids;
        private static Dictionary<string, int> _nonfcids;
        private static Dictionary<int, string> _EDFcidToSchemFcidMap;
        private static string _schemDiagramClassIDTable;
        private static string _cadopsConnection;
        private static string _EDERConn;
        private static string _EDConnection;
        private static string _SUBConnection;
        private static string _schemOracleConnection;
        private static List<String> _tables;
        private static string _path;
        private static string _archivepath;
        private static string _triggerfile;
        private static int _buffer;

        //Changes for ENOS to SAP migration - DMS .. Start

        private static string _ServiceLocationTableName;
        /// <summary>
        /// The value of the ServiceLocationTableName.
        /// </summary>
        public static string ServiceLocationTableName
        {
            get
            {
                if (_ServiceLocationTableName == null)
                {
                    _ServiceLocationTableName = getStringSetting("ServiceLocationTableName", "");
                }

                return _ServiceLocationTableName;
            }

        }


        private static string _GenCategoryFieldName;
        /// <summary>
        /// The value of the GenCategoryFieldName.
        /// </summary>
        public static string GenCategoryFieldName
        {
            get
            {
                if (_GenCategoryFieldName == null)
                {
                    _GenCategoryFieldName = getStringSetting("GenCategoryFieldName", "");
                }
                return _GenCategoryFieldName;
            }
        }

        private static string _GenSymbologyFieldName;
        /// <summary>
        /// The value of the GenSymbologyFieldName.
        /// </summary>
        public static string GenSymbologyFieldName
        {
            get
            {
                if (_GenSymbologyFieldName == null)
                {
                    _GenSymbologyFieldName = getStringSetting("GenSymbologyFieldName", "");
                }

                return _GenSymbologyFieldName;
            }
        }


        private static string _GenCategoryDomainName;
        /// <summary>
        /// The value of the GenCategoryDomainName.
        /// </summary>
        public static string GenCategoryDomainName
        {
            get
            {
                if (_GenCategoryDomainName == null)
                {
                    _GenCategoryDomainName = getStringSetting("GenCategoryDomainName", "");
                }

                return _GenCategoryDomainName;
            }

        }


        private static string _GenCategoryValueForPrimary;
        /// <summary>
        /// The value of the GenCategoryValueForPrimary.
        /// </summary>
        public static string GenCategoryValueForPrimary
        {
            get
            {
                if (_GenCategoryValueForPrimary == null)
                {
                    _GenCategoryValueForPrimary = getStringSetting("GenCategoryValueForPrimary", "");
                }

                return _GenCategoryValueForPrimary;
            }

        }

        //Changes for ENOS to SAP migration - DMS .. End

        /// <summary>
        /// The name of the trigger file to create after CSVs have been exported.
        /// </summary>
        public static string TriggerFile
        {
            get
            {
                if (_triggerfile == null)
                {
                    _triggerfile = getStringSetting("TriggerFile", "trigger.txt");
                }

                return _triggerfile;
            }

        }
        /// <summary>
        /// The directory where the exported CSVs will be archived.
        /// </summary>
        public static string ArchivePath
        {
            get
            {
                if (_archivepath == null)
                {
                    _archivepath = getStringSetting("ArchivePath", "").Replace(@"/",@"\");
                    if (!_archivepath.EndsWith(@"\"))
                        _archivepath += @"\";

                }

                return _archivepath;
            }
            set
            {
                _archivepath = value;
            }
        }
        /// <summary>
        /// The directory to export the CSVs to.
        /// </summary>
        public static string Path
        {
            get
            {
                if (_path == null)
                {
                    _path = getStringSetting("ExportPath", "").Replace(@"/", @"\");
                    if (!_path.EndsWith(@"\"))
                        _path += @"\";
                }

                return _path;
            }
            set
            {
                _path = value;
            }
        }
        /// <summary>
        /// For a dynamically created site, the number of feet to add to the height and width.
        /// </summary>
		public static int Buffer
        {
            get
            {
                if (_buffer == 0)
                {
                    _buffer = getIntSetting("SiteBufferFeet", 2) * 100;
                }
                return _buffer;
            }
        }
        /// <summary>
        /// A list of the tables to export to CSV from the staging database.
        /// </summary>
        public static List<String> ExportTables
        {
            get
            {
                if (_tables == null)
                {
                    _tables = getCommaSeparatedList("ExportTables", new List<String>());
                }
                return _tables;
            }
        }
        /// <summary>
        /// The Oracle connection string for the staging database.
        /// </summary>
        public static string CadopsConnection
        {
            get {
                if (_cadopsConnection == null)
                {
                    // m4jf - edgisrearch 388 Commented below line of code - updated oracle connection string without server name
                    //string[] con = getCommaSeparatedList("StagingConnection", new string[4]);
                    //_cadopsConnection = Oracle.CreateOracleConnectionString(con[0], con[1], con[2], con[3]);

                    // string[] con = getCommaSeparatedList("StagingConnection", new string[3]);
                    //_cadopsConnection = Oracle.CreateOracleConnString(con[0], con[1], con[2]);
                    // m4jf edgisrearch 919
                    string[] userInst = System.Configuration.ConfigurationManager.AppSettings["EDGMC_ConnectionStr_dmsstaging"].Split('@');
                    //string password = ReadEncryption.GetPassword(System.Configuration.ConfigurationManager.AppSettings["EDGMC_ConnectionStr_dmsstaging"].ToUpper());
                    _cadopsConnection = ReadEncryption.GetConnectionStr(System.Configuration.ConfigurationManager.AppSettings["EDGMC_ConnectionStr_dmsstaging"].ToUpper());
                }
                
                return Configuration._cadopsConnection; }
            set { Configuration._cadopsConnection = value; }
        }

        /// <summary>
        /// The Oracle connection string for the staging database.
        /// </summary>
        public static string EDERConn
        {
            get
            {
                if (_EDERConn == null)
                {
            
                    
                    _EDERConn = ReadEncryption.GetConnectionStr(System.Configuration.ConfigurationManager.AppSettings["EDGMC_ConnectionStr_dmsstaging"].ToUpper());
                }

                return Configuration._cadopsConnection;
            }
            set { Configuration._cadopsConnection = value; }
        }
        /// <summary>
        /// The Oracle connection string for the staging database.
        /// </summary>
        public static string EDConnection
        {
            get
            {
                if (_EDConnection == null)
                {
                    // m4jf - edgisrearch 388 Commented below line of code - updated oracle connection string without server name
                    //string[] con = getCommaSeparatedList("EDConnection", new string[4]);
                    //_EDConnection = Oracle.CreateOracleConnectionString(con[0], con[1], con[2], con[3]);

                    //string[] con = getCommaSeparatedList("EDConnection", new string[3]);
                    // m4jf edgisrearch 919
                    string[] userInst = System.Configuration.ConfigurationManager.AppSettings["EDGMC_ConnectionStr"].Split('@');
                    //string password = ReadEncryption.GetPassword(System.Configuration.ConfigurationManager.AppSettings["EDGMC_ConnectionStr"].ToUpper());
                    // _EDConnection = Oracle.CreateOracleConnString(con[0], con[1], con[2]);
                    _EDConnection = ReadEncryption.GetConnectionStr(System.Configuration.ConfigurationManager.AppSettings["EDGMC_ConnectionStr"]);
                }

                return Configuration._EDConnection;
            }
            set { Configuration._EDConnection = value; }
        }
        /// <summary>
        /// The Oracle connection string for the staging database.
        /// </summary>
        public static string SUBConnection
        {
            get
            {
                if (_SUBConnection == null)
                {
                    // m4jf - edgisrearch 388 Commented below line of code - updated oracle connection string without server name
                    //string[] con = getCommaSeparatedList("SUBConnection", new string[4]);
                    //_SUBConnection = Oracle.CreateOracleConnectionString(con[0], con[1], con[2], con[3]);

                    // string[] con = getCommaSeparatedList("SUBConnection", new string[3]);
                    // _SUBConnection = Oracle.CreateOracleConnString(con[0], con[1], con[2]);
                    // m4jf edgisrearch 919
                    string[] userInst = System.Configuration.ConfigurationManager.AppSettings["EDSUBGMC_ConnectionStr"].Split('@');
                    //string password = ReadEncryption.GetPassword(System.Configuration.ConfigurationManager.AppSettings["EDSUBGMC_ConnectionStr"].ToUpper());
                    _SUBConnection = ReadEncryption.GetConnectionStr(System.Configuration.ConfigurationManager.AppSettings["EDSUBGMC_ConnectionStr"].ToUpper());
                }

                return Configuration._SUBConnection;
            }
            set { Configuration._SUBConnection = value; }
        }
        /// <summary>
        /// The Oracle connection string for the staging database.
        /// </summary>
        public static string SchemOracleConnection
        {
            get
            {
                if (_schemOracleConnection == null)
                {
                    // m4jf - edgisrearch 388 Commented below line of code - updated oracle connection string without server name
                    //string[] con = getCommaSeparatedList("SCHEMConnection", new string[4]);
                    //_schemOracleConnection = Oracle.CreateOracleConnectionString(con[0], con[1], con[2], con[3]);

                    // string[] con = getCommaSeparatedList("SCHEMConnection", new string[3]);
                    // _schemOracleConnection = Oracle.CreateOracleConnString(con[0], con[1], con[2]);

                    // m4jf edgisrearch 919
                    string[] userInst = System.Configuration.ConfigurationManager.AppSettings["EDSCHM_ConnectionStr"].Split('@');
                   // string password = ReadEncryption.GetPassword(System.Configuration.ConfigurationManager.AppSettings["EDSCHM_ConnectionStr"].ToUpper());
                    _schemOracleConnection = ReadEncryption.GetConnectionStr(System.Configuration.ConfigurationManager.AppSettings["EDSCHM_ConnectionStr"].ToUpper());
                }

                return Configuration._schemOracleConnection;
            }
            set { Configuration._schemOracleConnection = value; }
        }
        /// <summary>
        /// ED FCIDS loaded from the config file: code,fcid
        /// Codes JU:ElectricDistNetwork_Junctions, PO:PriOHConductor, PU:PriUGConductor, DI:DistBusBar, ST:ElectricStitchPoint, DV:DeviceGroup, TR:Transformer, CP:CapacitorBank, SW:Switch, SE:StepDown, FU:Fuse, PM:PrimaryMeter, DP:DynamicProtectiveDevice, VR:VoltageRegulator, OP:OpenPoint, TI:Tie, PG:PrimaryGeneration, FI:FaultIndicator, OI:PriOHConductorInfo, UI:PriUGConductorInfo, CO:Controller, TU:TransformerUnit, VU:VoltageRegulatorUnit, RI:PrimaryRiser
        /// </summary>
        public static Dictionary<string, int> FCIDS
        {
            get
            {

                if (_fcids == null)
                {
                    ControlTable controlTable = new ControlTable(Configuration.EDConnection);
                    //configuration changes to provide the name of the feature classes which we can do a lookup query for the
                    //actual object class ID
                    Dictionary<string, string> nonFCIDNameMap = getStringMap("FCIDMap", new Dictionary<string, string>());
                    _fcids = new Dictionary<string, int>();
                    foreach (KeyValuePair<string, string> KeyPhysicalNameKVP in nonFCIDNameMap)
                    {
                        int classID = controlTable.GetObjectClassID(KeyPhysicalNameKVP.Value);
                        if (classID > 0)
                        {
                            _fcids.Add(KeyPhysicalNameKVP.Key, classID);
                        }
                    }
                    controlTable = null;
                }
                return Configuration._fcids;
            }
        }
        /// <summary>
        /// Substation FCIDS loaded from the config file: code,fcid
        /// Codes JU	SubGeometricNetwork_Junctions, BU	SUBBusBar, UG	SUBUGConductor, OH	SUBOHConductor, SW	SUBSwitch, RI	SUBRiser, PT	SUBPotentialTransformer, FU	SUBFuse, CT	SUBCurrentTransformer, TR	SUBTransformerBank, ST	SUBStationTransformer, SI	SUBElectricStitchPoint, CA	SUBCapacitorBank, CE	SUBCell, RE	SUBReactor, TI	SUBTie, VO	SUBVoltageRegulator, ID	SUBInterruptingDevice, MT	SUBMTU, CS	CircuitSource, UI	SUBUGConductorInfo, OI	SUBOHConductorInfo, LI	SUBLink, EX	SUBExtraWinding, TU	SUBTransformerUnit, TA	SUBTransformerRating, LO	SUBLoadTapChanger, VU	SUBVoltageRegulatorUnit, 
        /// </summary>
        public static Dictionary<string, int> SUBFCIDS
        {
            get
            {

                if (_subfcids == null)
                {
                    ControlTable controlTable = new ControlTable(Configuration.SUBConnection);
                    //configuration changes to provide the name of the feature classes which we can do a lookup query for the
                    //actual object class ID
                    Dictionary<string, string> subFCIDNameMap = getStringMap("SUBFCID", new Dictionary<string, string>());
                    _subfcids = new Dictionary<string, int>();
                    foreach (KeyValuePair<string, string> KeyPhysicalNameKVP in subFCIDNameMap)
                    {
                        int classID = controlTable.GetObjectClassID(KeyPhysicalNameKVP.Value);
                        if (classID > 0)
                        {
                            _subfcids.Add(KeyPhysicalNameKVP.Key, classID);
                        }
                    }
                    controlTable = null;
                }
                return Configuration._subfcids;
            }
        }
        /// <summary>
        /// FCIDS of secondary features loaded from the config file: code,fcid
        /// Codes DC:DCRectifier, DE:DeliveryPoint, SG:SecondaryGeneration, SL:ServiceLocation, SM:SmartMeterNetworkDevice, ST:StreetLight, SD:DCConductor, SO:SecOHConductor, SU:SecUGConductor, TL:TransformerLead, 
        /// </summary>
        public static Dictionary<string, int> NonFCIDS
        {
            get
            {

                if (_nonfcids == null)
                {
                    ControlTable controlTable = new ControlTable(Configuration.CadopsConnection);
                    //configuration changes to provide the name of the feature classes which we can do a lookup query for the
                    //actual object class ID
                    Dictionary<string, string> nonFCIDNameMap = getStringMap("NONFCID", new Dictionary<string, string>());
                    _nonfcids = new Dictionary<string, int>();
                    foreach (KeyValuePair<string, string> KeyPhysicalNameKVP in nonFCIDNameMap)
                    {
                        int classID = controlTable.GetObjectClassID(KeyPhysicalNameKVP.Value);
                        _nonfcids.Add(KeyPhysicalNameKVP.Key, classID);
                    }
                    controlTable = null;
                }
                return Configuration._nonfcids;
            }
        }
        /// <summary>
        /// ED FCID to schem FCID map loaded from the config file: EDFcid,SchemFcid
        /// </summary>
        public static Dictionary<int, string> EDFcidToSchemFcidMap
        {
            get
            {

                if (_EDFcidToSchemFcidMap == null)
                {
                    Dictionary<string, string> featClassNameMapping = GetSchematicsFeatureClassNameMap();

                    ControlTable controlTable = new ControlTable(Configuration.CadopsConnection);
                    _EDFcidToSchemFcidMap = new Dictionary<int, string>();
                    foreach (KeyValuePair<string, string> KeyPhysicalNameKVP in featClassNameMapping)
                    {
                        int classID = controlTable.GetObjectClassID(KeyPhysicalNameKVP.Key);
                        if (classID > 0)
                        {
                            _EDFcidToSchemFcidMap.Add(classID, KeyPhysicalNameKVP.Value);
                        }
                        else
                        {
                            _log4.Info("Schematics mapping in ELTCLASS table is incorrect and the GIS feature class could not be found: "
                                    + "GIS Feature Class: " + KeyPhysicalNameKVP.Key + " Schematics Feature Class: " + KeyPhysicalNameKVP.Value);
                        }
                    }
                }
                return Configuration._EDFcidToSchemFcidMap;
            }
        }

        private static int SchematicsID = -1;
        private static string SchematicsOwner = "";
        public static Dictionary<string, string> GetSchematicsFeatureClassNameMap()
        {
            Dictionary<string, string> EDFeatClassNameToSchemFeatClassName = new Dictionary<string, string>();

            OracleConnection oraConnection = null;
            //First we need to determine the owner and the current schematics ID for our tables by querying
            //the sde.sch_dataset
            string sql = "select OWNER,ID from sde.sch_dataset ORDER BY ID";
            try
            {
                oraConnection = new OracleConnection(SchemOracleConnection);
                oraConnection.Open();
                using (var cmd = new OracleCommand(sql, oraConnection))
                {
                    cmd.CommandType = CommandType.Text;
                    using (OracleDataReader dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            object SchemOwnerObject = dataReader["OWNER"];
                            object SchemIDObject = dataReader["ID"];
                            if ((SchemIDObject != null && !(SchemIDObject is DBNull)) && 
                                (SchemOwnerObject != null && !(SchemOwnerObject is DBNull)))
                            {
                                SchematicsID = Int32.Parse(SchemIDObject.ToString());
                                SchematicsOwner = SchemOwnerObject.ToString();
                            }
                        }
                        dataReader.Close();
                        dataReader.Dispose();
                    }
                }

                //Now we can query the ELTClass table for the feature class name mapping
                sql = "select UPPER(CREATIONNAME) CREATIONNAME,UPPER(NAME) NAME from " + SchematicsOwner + ".SCH" + SchematicsID + "_ELTCLASS";
                using (var cmd = new OracleCommand(sql, oraConnection))
                {
                    cmd.CommandType = CommandType.Text;
                    using (OracleDataReader dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            object CreationNameObject = dataReader["CREATIONNAME"];
                            object NameObject = dataReader["NAME"];
                            if ((CreationNameObject != null && !(CreationNameObject is DBNull)) &&
                                (NameObject != null && !(NameObject is DBNull)))
                            {
                                EDFeatClassNameToSchemFeatClassName.Add(SchematicsOwner + "." + NameObject.ToString(), SchematicsOwner + ".SCH" + SchematicsID + "E_" + CreationNameObject.ToString());
                            }
                        }
                        dataReader.Close();
                        dataReader.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                _log4.Error("Error obtaining schematics Feature Class name mapping. SQL : " + sql, ex);
            }
            finally
            {
                oraConnection.Close();
                oraConnection.Dispose();
                oraConnection = null;
            }

            return EDFeatClassNameToSchemFeatClassName;
        }

        /// <summary>
        /// Obtains the schematics diagram class ID table
        /// </summary>
        public static string SchemDiagramClassIDTable
        {
            get
            {
                if (string.IsNullOrEmpty(_schemDiagramClassIDTable))
                {
                    _schemDiagramClassIDTable = SchematicsOwner + ".SCH" + SchematicsID + "_DIACLASS";
                }
                return _schemDiagramClassIDTable;
            }
        }
        /// <summary>
        /// Get a setting from the AppSettings
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>The setting value</returns>
        public static string getSetting(string key)
        {
            string setting = null;
            try
            {
                setting = System.Configuration.ConfigurationManager.AppSettings[key];
            }
            catch { }
            return setting;
        }
        /// <summary>
        /// Get a string setting from the config file
        /// </summary>
        /// <param name="Key">The name of the setting</param>
        /// <param name="Default">What to return if the setting is null</param>
        /// <returns>The setting value or Default if the value is null</returns>
        public static string getStringSetting(string Key, string Default)
        {
            string output = Default;
            string value = getSetting(Key);
            if (value != null)
            {
                output = value;
            }
            return output;
        }
        /// <summary>
        /// Get a boolean setting from the config file
        /// </summary>
        /// <param name="Key">The name of the setting</param>
        /// <param name="Default">What to return if the setting is null</param>
        /// <returns>The setting value or Default if the value is null</returns>
        public static bool getBoolSetting(string Key, bool Default)
        {
            bool output = Default;
            string value = getSetting(Key);
            if (value != null)
            {
                try
                {
                    output = Convert.ToBoolean(value);
                }
                catch { }
            }
            return output;
        }
        /// <summary>
        /// Get a integer setting from the config file
        /// </summary>
        /// <param name="Key">The name of the setting</param>
        /// <param name="Default">What to return if the setting is null</param>
        /// <returns>The setting value or Default if the value is null</returns>
        public static int getIntSetting(string Key, int Default)
        {
            // #2
            int output = Default;
            string value = getSetting(Key);
            if (value != null)
            {
                try
                {
                    output = Convert.ToInt32(value);
                }
                catch { }
            }
            return output;
        }
        /// <summary>
        /// Get a double setting from the config file
        /// </summary>
        /// <param name="Key">The name of the setting</param>
        /// <param name="Default">What to return if the setting is null</param>
        /// <returns>The setting value or Default if the value is null</returns>
        public static double getDoubleSetting(string Key, double Default)
        {
            double output = Default;
            string value = getSetting(Key);
            if (value != null)
            {
                try
                {
                    output = Convert.ToDouble(value);
                }
                catch { }
            }
            return output;
        }
        /// <summary>
        /// Get a string array from a comma separated list in the config file
        /// </summary>
        /// <param name="Key">The name of the setting</param>
        /// <param name="Default">What to return if the setting is null</param>
        /// <returns>The string array or Default if the value is null</returns>
        public static string[] getCommaSeparatedList(string Key, string[] Default)
        {
            string[] output = Default;
            string value = getSetting(Key);
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
        /// <summary>
        /// Get a string dictionary from a comma separated list in the config file
        /// </summary>
        /// <param name="Key">The name of the setting</param>
        /// <param name="Default">What to return if the setting is null</param>
        /// <returns>The dictionary or Default if the value is null</returns>
        public static Dictionary<string, bool> getCommaSeparatedList(string Key, Dictionary<string, bool> Default)
        {
            Dictionary<string, bool> output = Default;
            string[] list = getCommaSeparatedList(Key, new string[0]);
            if (list.Length > 0)
            {
                output = new Dictionary<string, bool>();
                foreach (string s in list)
                {
                    if (!output.ContainsKey(s))
                    {
                        output.Add(s, true);
                    }
                }
            }
            return output;
        }
        /// <summary>
        /// Get a list of strings from a comma separated list in the config file
        /// </summary>
        /// <param name="Key">The name of the setting</param>
        /// <param name="Default">What to return if the setting is null</param>
        /// <returns>The list or Default if the value is null</returns>
        public static List<String> getCommaSeparatedList(string Key, List<String> Default)
        {
            List<String> output = Default;
            string[] list = getCommaSeparatedList(Key, new string[0]);
            if (list.Length > 0)
            {
                output = new List<String>();
                foreach (string s in list)
                {
                        output.Add(s);
                }
            }
            return output;
        }
        /// <summary>
        /// Get a string array from a semi colon separated list in the config file
        /// </summary>
        /// <param name="Key">The name of the setting</param>
        /// <param name="Default">What to return if the setting is null</param>
        /// <returns>The string array or Default if the value is null</returns>
        public static string[] getSemmiSeparatedList(string Key, string[] Default)
        {
            string[] output = Default;
            string value = getSetting(Key);
            if (value != null)
            {
                try
                {
                    string[] temp = value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
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
        /// <summary>
        /// Get a string dictionary from a semi colon separated list in the config file
        /// </summary>
        /// <param name="Key">The name of the setting</param>
        /// <param name="Default">What to return if the setting is null</param>
        /// <returns>The dictionary or Default if the value is null</returns>
        public static Dictionary<string, bool> getSemmiSeparatedList(string Key, Dictionary<string, bool> Default)
        {
            Dictionary<string, bool> output = Default;
            string[] list = getSemmiSeparatedList(Key, new string[0]);
            if (list.Length > 0)
            {
                output = new Dictionary<string, bool>();
                foreach (string s in list)
                {
                    if (!output.ContainsKey(s))
                    {
                        output.Add(s, true);
                    }
                }
            }
            return output;
        }
        /// <summary>
        /// Get a dictionary from a comma and semi colon separated list in the config file: key,value;key,value
        /// </summary>
        /// <param name="Key">The name of the setting</param>
        /// <param name="Default">What to return if the setting is null</param>
        /// <returns>The dictionary or Default if the value is null</returns>
        public static Dictionary<string, string> getStringMap(string Key, Dictionary<string, string> Default)
        {
            Dictionary<string, string> output = Default;
            string[] list = getSemmiSeparatedList(Key, new string[0]);
            if (list.Length > 0)
            {
                output = new Dictionary<string, string>();
                foreach (string s in list)
                {
                    string[] map = s.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (map.Length == 2)
                    {
                        if (!output.ContainsKey(map[0]))
                        {
                            if (!String.IsNullOrEmpty(map[1]))
                            {
                                output.Add(map[0], map[1]);
                            }
                        }
                    }
                }
            }
            return output;
        }
        /// <summary>
        /// Get a dictionary from a comma and semi colon separated list in an input string: key,value;key,value
        /// </summary>
        /// <param name="Value">The input string</param>
        /// <param name="Default">The value to use if the input is null</param>
        /// <returns>The dictionary or Default if the value is null</returns>
        public static Dictionary<string, string> getStringMapValue(string Value, Dictionary<string, string> Default)
        {
            Dictionary<string, string> output = Default;
            string[] list = getSemmiSeparatedListValue(Value, new string[0]);
            if (list.Length > 0)
            {
                output = new Dictionary<string, string>();
                foreach (string s in list)
                {
                    string[] map = s.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (map.Length == 2)
                    {
                        if (!output.ContainsKey(map[0]))
                        {
                            if (!String.IsNullOrEmpty(map[1]))
                            {
                                output.Add(map[0], map[1]);
                            }
                        }
                    }
                }
            }
            return output;
        }
        /// <summary>
        /// Get a string array from a semi colon separated input string
        /// </summary>
        /// <param name="value">The string with the values</param>
        /// <param name="Default">What to return if the input is null</param>
        /// <returns>The string array or Default is the input is null</returns>
        public static string[] getSemmiSeparatedListValue(string value, string[] Default)
        {
            string[] output = Default;
            if (value != null)
            {
                try
                {
                    string[] temp = value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    output = temp;                    
                }
                catch { }
            }
            return output;
        }
        /// <summary>
        /// Parse a config setting into a string(key),int(value) dictionary. Config setting should be in the format key,value;key,value
        /// </summary>
        /// <param name="Key">The name of the setting</param>
        /// <param name="Default">What to return if the setting doesn't exist or is null</param>
        /// <returns>A dictionary of the key value pairs</returns>
        public static Dictionary<string, int> getIntMap(string Key, Dictionary<string, int> Default)
        {
            Dictionary<string, int> output = Default;
            string[] list = getSemmiSeparatedList(Key, new string[0]);
            if (list.Length > 0)
            {
                output = new Dictionary<string, int>();
                foreach (string s in list)
                {
                    string[] map = s.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (map.Length == 2)
                    {
                        if (!output.ContainsKey(map[0]))
                        {
                            int value = 0;
                            if (int.TryParse(map[1], out value))
                            {
                                output.Add(map[0], value);
                            }
                        }
                    }
                }
            }
            return output;
        }
        /// <summary>
        /// Parse a config setting into a string(key),int(value) dictionary. Config setting should be in the format key,value;key,value
        /// </summary>
        /// <param name="Key">The name of the setting</param>
        /// <param name="Default">What to return if the setting doesn't exist or is null</param>
        /// <returns>A dictionary of the key value pairs</returns>
        public static Dictionary<int, string> getIntMap(string Key, Dictionary<int, string> Default)
        {
            Dictionary<int, string> output = Default;
            string[] list = getSemmiSeparatedList(Key, new string[0]);
            if (list.Length > 0)
            {
                output = new Dictionary<int, string>();
                foreach (string s in list)
                {
                    string[] map = s.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (map.Length == 2)
                    {
                        if (!output.ContainsKey(Int32.Parse(map[0])))
                        {
                            output.Add(Int32.Parse(map[0]), map[1]);
                        }
                    }
                }
            }
            return output;
        }
        /// <summary>
        /// Parse a config setting into a int(key),int(value) dictionary. Config setting should be in the format key,value;key,value
        /// </summary>
        /// <param name="Key">The name of the setting</param>
        /// <param name="Default">What to return if the setting doesn't exist or is null</param>
        /// <returns>A dictionary of the key value pairs</returns>
        public static Dictionary<int, int> getIntIntMap(string Key, Dictionary<int, int> Default)
        {
            Dictionary<int, int> output = Default;
            string[] list = getSemmiSeparatedList(Key, new string[0]);
            if (list.Length > 0)
            {
                output = new Dictionary<int, int>();
                foreach (string s in list)
                {
                    string[] map = s.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (map.Length == 2)
                    {
                        int key = 0;
                        if (int.TryParse(map[0], out key))
                        {
                            if (!output.ContainsKey(key))
                            {
                                int value = 0;
                                if (int.TryParse(map[1], out value))
                                {
                                    output.Add(key, value);
                                }
                            }
                        }
                    }
                }
            }
            return output;
        }
        /// <summary>
        /// Parse a config setting into a int(key),List[int](value) dictionary. Config setting should be in the format key,value;key,value
        /// </summary>
        /// <param name="Key">The name of the setting</param>
        /// <param name="Default">What to return if the setting doesn't exist or is null</param>
        /// <returns>A dictionary of the key value pairs</returns>
        public static Dictionary<int, List<int>> getIntIntListMap(string Key, Dictionary<int, List<int>> Default)
        {
            Dictionary<int, List<int>> output = Default;
            string[] list = getSemmiSeparatedList(Key, new string[0]);
            if (list.Length > 0)
            {
                output = new Dictionary<int, List<int>>();
                foreach (string s in list)
                {
                    string[] map = s.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (map.Length == 2)
                    {
                        int key = 0;
                        if (int.TryParse(map[0], out key))
                        {
                            if (!output.ContainsKey(key))
                            {
                                int value = 0;
                                if (int.TryParse(map[1], out value))
                                {
                                    List<int> ilist = new List<int>();
                                    ilist.Add(value);
                                    output.Add(key, ilist);
                                }
                            }
                            else
                            {
                                int value = 0;
                                if (int.TryParse(map[1], out value))
                                {
                                    output[key].Add(value);
                                }
                            }
                        }
                    }
                }
            }
            return output;
        }
        /// <summary>
        /// Parse a config setting into a dictionary[int(key),dictionary[int(key2),string(value)]]. 
        /// Config setting should be in the format key,key2,value;key,key2,value
        /// </summary>
        /// <param name="Key">The name of the setting</param>
        /// <param name="Default">What to return if the setting doesn't exist or is null</param>
        /// <returns>A dictionary of the key value pairs</returns>
        public static Dictionary<int, Dictionary<int, string>> getIntIntStringMap(string Key, Dictionary<int, Dictionary<int, string>> Default)
        {
            Dictionary<int, Dictionary<int, string>> output = Default;
            string[] list = getSemmiSeparatedList(Key, new string[0]);
            if (list.Length > 0)
            {
                output = new Dictionary<int, Dictionary<int, string>>();
                foreach (string s in list)
                {
                    string[] map = s.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (map.Length == 3)
                    {
                        int key = 0;
                        if (int.TryParse(map[0], out key))
                        {
                            if (!output.ContainsKey(key))
                            {
                                output.Add(key, new Dictionary<int, string>());
                            }
                            Dictionary<int, string> output2 = output[key];
                            int value = 0;
                            if (int.TryParse(map[1], out value))
                            {
                                if (!output2.ContainsKey(value))
                                {
                                    output2.Add(value, map[2]);
                                }
                            }
                        }
                    }
                }
            }
            return output;
        }
    }
}
