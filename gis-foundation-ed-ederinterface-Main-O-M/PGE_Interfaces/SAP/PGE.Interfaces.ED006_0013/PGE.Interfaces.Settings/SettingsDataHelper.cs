using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using System.Configuration;
using System.Data;
using PGE_DBPasswordManagement;

namespace PGE.Interfaces.Settings
{
    public class SettingsDataHelper : IDisposable
    {
        private const string _configFileName = "PGE.Interfaces.SAP.dll.config";
        private string _connectionstring;
        private OracleConnection _oracleConnection;
        private bool _disposed = false;
        /// <summary>
        /// Default constructor. Loads configuration from the config file in the install directory.
        /// </summary>
        public SettingsDataHelper()
        {
            try
            {
                // M4JF EDGISREARCH 9191 - get connection string using PGE_DBPasswordManagemenT
                // _connectionstring = ConfigurationManager.AppSettings["settingsConnectionString"];
                _connectionstring = ReadEncryption.GetConnectionStr(ConfigurationManager.AppSettings["EDER_ConnectionStr"].ToUpper());

                _oracleConnection = new OracleConnection(_connectionstring);
                _oracleConnection.Open();
            }
            catch (Exception ex)
            {
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
            Dictionary<string, string> retVal = new Dictionary<string, string>();

            using (OracleCommand cmd = new OracleCommand(sql, _oracleConnection))
            {
                cmd.CommandType = CommandType.Text;

                using (OracleDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        retVal.Add(cleanData(dataReader["GLOBAL_ID"]), cleanData(dataReader["FEATURE_CLASS_NAME"]));
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
        public Dictionary<string, string> GetDataForSAP(DateTime? date)
        {
            string sql = "";
            if(date.HasValue)
                sql = string.Format("SELECT GLOBAL_ID,FEATURE_CLASS_NAME FROM {0} WHERE DATE_MODIFIED >= To_Date('{1}','MM-DD-YYYY')", "PGEDATA_SM_SCADA_EAD", date.Value.ToString("MM-dd-yyyy"));
            else
                sql = string.Format("SELECT GLOBAL_ID,FEATURE_CLASS_NAME FROM {0}", "PGEDATA_SM_SCADA_EAD");

            return GetData(sql);
        }

        /// <summary>
        /// This methods returns last run date for this app
        /// </summary>
        /// <returns></returns>
        public DateTime? GetLastRunDate()
        {
            DateTime? retVal = null;
            string sql = string.Format("SELECT LASTDATE FROM PGEDATA_EXECUTED WHERE PROCESS_NAME='{0}'", "SAP_ETL");

            using (OracleCommand cmd = new OracleCommand(sql, _oracleConnection))
            {
                cmd.CommandType = CommandType.Text;

                using (OracleDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        if (dataReader["LASTDATE"] != null)
                            retVal = DateTime.Parse(dataReader["LASTDATE"].ToString());
                    }
                }

            }
            return retVal;
        }

        /// <summary>
        /// This methods sets last run date for this app
        /// </summary>
        /// <param name="processStartDateTime"></param>
       
        public void UpdateLastRunDate(DateTime processStartDateTime)
        {
            string sql = "";
            if(processExist())
                 sql = string.Format("UPDATE PGEDATA_EXECUTED SET LASTDATE=To_Date('{0}','MM-DD-YYYY HH24:MI:SS') WHERE PROCESS_NAME='{1}'", processStartDateTime.ToString("MM-dd-yyyy hh:mm:ss"), "SAP_ETL");
            else
                sql = string.Format("INSERT INTO PGEDATA_EXECUTED VALUES(To_Date('{0}','MM-DD-YYYY HH24:MI:SS'),'{1}')", processStartDateTime.ToString("MM-dd-yyyy hh:mm:ss"), "SAP_ETL");

            using (OracleCommand cmd = new OracleCommand(sql, _oracleConnection))
            {
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }
        }

        private bool processExist()
        {
            bool retVal = false;
            string sql = string.Format("SELECT * FROM PGEDATA_EXECUTED WHERE PROCESS_NAME='{0}'", "SAP_ETL");
            using (OracleCommand cmd = new OracleCommand(sql, _oracleConnection))
            {
                cmd.CommandType = CommandType.Text;
                using (OracleDataReader dataReader = cmd.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        retVal = true;
                        break;
                    }
                }
            }
            return retVal;
        }

        private string cleanData(object data)
        {
            if (data == null)
                return "";
            else
                return data.ToString();
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
