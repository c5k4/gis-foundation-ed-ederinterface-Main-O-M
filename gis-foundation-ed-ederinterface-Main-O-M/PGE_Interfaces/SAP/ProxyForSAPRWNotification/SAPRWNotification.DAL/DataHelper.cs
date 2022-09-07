using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;
using System.Configuration;
using System.Data;

namespace PGE.ProxyForSAPRWNotification.DAL
{
    public class DataHelper : IDisposable
    {
        private string _connectionstring;
        private OracleConnection _oracleConnection;
        private bool _disposed = false;

        /// <summary>
        /// Default constructor. Loads configuration from the config file in the install directory.
        /// </summary>
        public DataHelper()
        {
            try
            {
                _connectionstring = ConfigurationManager.AppSettings["ConnectionString"];
                _oracleConnection = new OracleConnection(_connectionstring);
                _oracleConnection.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // <summary>
        /// This methods returns data required for SAP
        /// </summary>
        /// <param name="globalID"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetDataForSAP()
        {
            string sql = "";

            sql = string.Format("SELECT ASSETID,FEATURECLASSNAME FROM {0}", "EDGIS.PGE_GISSAP_REPROCESSASSETSYNC");

            return GetData(sql);
        }

        public void InsertData(string sql)
        {
            using (OracleCommand cmd = new OracleCommand(sql, _oracleConnection))
            {
                cmd.CommandType = CommandType.Text;


                int recordinsert = cmd.ExecuteNonQuery();
                if (recordinsert == 0)
                    Common.WriteToLog("no record has inserted", Common.LoggingLevel.Error);
                else
                    Common.WriteToLog("record has inserted", Common.LoggingLevel.Info);
                
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
                        retVal.Add(cleanData(dataReader["ASSETID"]), cleanData(dataReader["FEATURECLASSNAME"]));
                    }
                }
                return retVal;
            }
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


        void Dispose(bool disposing)
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
