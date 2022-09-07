using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;

using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;

using PGE.Common.Delivery.Framework;
using PGE.Interfaces.Integration.Framework;
using PGE.Common.Delivery.Systems.Configuration;
using PGE_DBPasswordManagement;

namespace PGE.Interfaces.SAP
{
    /// <summary>
    /// This class facilitates interaction with the Database
    /// </summary>
    public class SettingDataHelper : IDisposable
    {
        private const string _configFileName = "PGE.Interfaces.SAP.dll.config";
        private string _connectionstring;
        private OracleConnection _oracleConnection;
        private bool _disposed = false;
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "");
        /// <summary>
        /// Default constructor. Loads configuration from the config file in the install directory.
        /// </summary>
        public SettingDataHelper()
        {
            try
            {
                SystemRegistry sysRegistry = new SystemRegistry("PGESAP");
                //string PGEFolder = sysRegistry.Directory;
                //string installationFolder = sysRegistry.GetSetting<string>("SAPDirectory", PGEFolder);

                string installationFolder = sysRegistry.Directory;

                string assemblyLocation = Assembly.GetExecutingAssembly().Location;
                //installationFolder = Path.GetDirectoryName(assemblyLocation);
                //string configPath = Path.Combine(installationFolder, "Config");

                //ME Q3-19 CHANGE
                //string configPath = Path.Combine(installationFolder, "SAP Asset Synch");//read from registry
                string configPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\SAP Asset Synch";
                if (!Directory.Exists(configPath))
                {
                    configPath = @"D:\Program Files (x86)\Miner and Miner\PG&E Custom Components\SAP Asset Synch";
                    if (!Directory.Exists(configPath))
                        configPath = @"C:\Program Files (x86)\Miner and Miner\PG&E Custom Components\SAP Asset Synch";
                }
                string configFile = Path.Combine(configPath, _configFileName);
                
                ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap(); //Path to your config file
                fileMap.ExeConfigFilename = configFile;
                Configuration _config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
                // M4JF EDGISREARCH 919 - GET CONNECTION STRING USING PASSWORDMANAGEMENT TOOL
                //_connectionstring = _config.AppSettings.Settings["settingsConnectionString"].Value;
                _connectionstring = ReadEncryption.GetConnectionStr(_config.AppSettings.Settings["EDER_ConnectionStr_Pgedata"].Value.ToUpper());
                _logger.Debug("connection:" + _connectionstring);
                _oracleConnection = new OracleConnection(_connectionstring);
                _oracleConnection.Open();
            }
            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); throw oex; }
            catch (Exception ex)
            {
                _logger.Debug("Exception in SettingDataHelper Constructor: " + ex);
                throw ex;
            }
        }


        /// <summary>
        /// This method executes a SQL statement against the DB and returns a list of results
        /// </summary>
        /// <param name="sql">The SQL string to execute</param>
        /// <returns>List of rows from the database</returns>
        private Dictionary<string, string> GetData(string sql)
        {
            Dictionary<string,string> retVal = new Dictionary<string,string>();

            using (OracleCommand cmd = new OracleCommand(sql, _oracleConnection))
            {
                cmd.CommandType = CommandType.Text;

                using (OracleDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        retVal.Add("SettingsRadioManufacturer", (dataReader["RADIO_MANF_CD"] == null ? "":dataReader["RADIO_MANF_CD"].ToString()));
                        retVal.Add("SettingsRadioModelNumber", (dataReader["RADIO_MODEL_NUM"] == null ? "" : dataReader["RADIO_MODEL_NUM"].ToString()));
                        retVal.Add("SettingsRadioSerialNumber", (dataReader["RADIO_SERIAL_NUM"] == null ? "" : dataReader["RADIO_SERIAL_NUM"].ToString()));
                        retVal.Add("SettingsNotes", (dataReader["SPECIAL_CONDITIONS"] == null ? "" : dataReader["SPECIAL_CONDITIONS"].ToString()));
                        retVal.Add("ControllerType", (dataReader["CONTROL_TYPE"] == null ? "" : dataReader["CONTROL_TYPE"].ToString()));
                        retVal.Add("ControllerSerialNumber", (dataReader["CONTROL_SERIAL_NUM"] == null ? "" : dataReader["CONTROL_SERIAL_NUM"].ToString()));
                        
                        break;
                    }
                }
                return retVal;
            }
        }

        /// <summary>
        /// This methods returns data required for SAP
        /// </summary>
        /// <param name="globalID"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetDataForSAP(string globalID, string tableName)
        {
            string sql = "";
            //if(deviceType.ToUpper() == "SM_SWITCH")
            //    sql = string.Format("SELECT RADIO_MANF_CD, RADIO_MODEL_NUM, RADIO_SERIAL_NUM, NOTES,CONTROL_UNIT_TYPE,CONTROL_SERIAL_NUM  FROM {0} WHERE CURRENT_FUTURE = 'C' AND GLOBAL_ID='{1}'", deviceType, globalID);
            //else
            //    sql = string.Format("SELECT RADIO_MANF_CD, RADIO_MODEL_NUM, RADIO_SERIAL_NUM, NOTES,CONTROL_TYPE,CONTROL_SERIAL_NUM  FROM {0} WHERE CURRENT_FUTURE = 'C' AND GLOBAL_ID='{1}'", deviceType, globalID);
            sql = string.Format("SELECT RADIO_MANF_CD, RADIO_MODEL_NUM, RADIO_SERIAL_NUM, SPECIAL_CONDITIONS ,CONTROL_TYPE,CONTROL_SERIAL_NUM FROM {0} WHERE GLOBAL_ID='{1}'", tableName, globalID);


            return GetData(sql);
        }

        /// <summary>
        /// This methods returns data required for reverse synch
        /// </summary>
        /// <param name="globalID"></param>
        /// <param name="reverseSyncedClass"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetDataForReverseSynch(string globalID, ReverseSyncedClass reverseSyncedClass)
        {
            Dictionary<string, string> retVal = new Dictionary<string, string>();
            string sqlString = reverseSyncedClass.GetSQLString(globalID);

            if (!string.IsNullOrEmpty(sqlString))
            {
                using (OracleCommand cmd = new OracleCommand(sqlString, _oracleConnection))
                {
                    try
                    {
                        cmd.CommandType = CommandType.Text;
                        using (OracleDataReader dataReader = cmd.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                foreach (ReverseSyncedField fld in reverseSyncedClass.ReverseSyncedFields)
                                {
                                    string val = (dataReader[fld.SettingsFieldName] == null ? "" : dataReader[fld.SettingsFieldName].ToString());
                                    retVal.Add(fld.OutName, val);
                                }
                                break;
                            }
                        }
                    }
                    catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode))) { throw oex; } }
                    catch (Exception ex)
                    {
                        // log error 
                    }
                }
            }

            return retVal;
        }


        /// <summary>
        /// Disposes oracle connection object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (this._disposed == false)
            {
                if (disposing)
                {
                    // managed resources

                    if (this._oracleConnection != null)
                    {
                        this._oracleConnection.Close();
                        this._oracleConnection.Dispose();
                    }
                }

                this._oracleConnection = null;
                this._disposed = true;
            }
        }
    }
}

