using System;
using System.Configuration;
using System.Data;
using Oracle.DataAccess.Client;

namespace PGE.Interfaces.SAPRWNotification_Archive
{
    public class DataHelper : IDisposable
    {
        internal readonly string Connectionstring;
        private OracleConnection _oracleConnection;
        public bool Disposed { get; private set; }

        /// <summary>
        /// Default constructor. Loads configuration from the config file in the install directory.
        /// </summary>
        public DataHelper()
        {
            //Common.WriteToLog("Making database connetion", Common.LoggingLevel.Info);
            Connectionstring = ConfigurationManager.AppSettings["ConnectionString"];
            _oracleConnection = new OracleConnection(Connectionstring);
            _oracleConnection.Open();
            // Common.WriteToLog("connection is successfull", Common.LoggingLevel.Info);
        }

        // <summary>
        /// This methods returns data required for SAPRWNotification
        /// <returns></returns>
        public DataTable GetDataForSapRwnotification(string sql)
        {

            return GetData(sql);
        }

        public void DeleteData(string sql)
        {
            OracleTransaction dbTransaction = _oracleConnection.BeginTransaction(IsolationLevel.ReadCommitted);
            using (OracleCommand cmd = new OracleCommand(sql, _oracleConnection))
            {

                try
                {
                    cmd.CommandType = CommandType.Text;

                    //dbTransaction = _oracleConnection.BeginTransaction(IsolationLevel.ReadCommitted);
                    cmd.Transaction = dbTransaction;
                    cmd.ExecuteNonQuery();
                    dbTransaction.Commit();
                }

                catch (OracleException)
                {
                    dbTransaction.Rollback();
                }

            }
        }

        /// <summary>
        /// This method executes a SQL statement against the DB and returns a list of results
        /// </summary>
        /// <param name="sql">The SQL string to execute</param>
        /// <returns>List of rows from the database</returns>
        private DataTable GetData(string sql)
        {
            // Dictionary<string, string> retVal = new Dictionary<string, string>();
            DataTable dt = new DataTable();

            using (OracleCommand cmd = new OracleCommand(sql, _oracleConnection))
            {
                cmd.CommandType = CommandType.Text;

                using (OracleDataReader dataReader = cmd.ExecuteReader())
                {

                    // retVal.Add(CleanData(dataReader["ASSETID"]), CleanData(dataReader["FEATURECLASSNAME"]));
                    dt.Load(dataReader);

                }
                return dt;
            }
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
            if (Disposed == false)
            {
                if (disposing)
                {
                    // managed resources

                    if (_oracleConnection != null)
                    {
                        _oracleConnection.Close();
                        _oracleConnection.Dispose();
                    }
                }

                _oracleConnection = null;
                Disposed = true;
            }
        }
    }
}

