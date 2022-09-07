using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Oracle.DataAccess.Client;
using System.Data;
using log4net;
using PGE_DBPasswordManagement;

namespace GISPLDBAssetSynchProcess.DAL
{
    public class DBHelper
    {
        OracleConnection conOra = null;
        OracleTransaction transOracle = null;
        private string _strDBCode = string.Empty;
      //  log4net.ILog objLogger = null;
        private log4net.ILog objLogger = null;
        public DBHelper(string strDatabaseCode)
        {
            _strDBCode = strDatabaseCode;
         //   InitializeLogger();
        }
        public DBHelper(string strDatabaseCode, log4net.ILog logger)
        {
            _strDBCode = strDatabaseCode;
            if (logger == null)
                InitializeLogger();
            else
                objLogger = logger;
        }

        public void OpenPGEDataConnection()
        {
            string strPassword = string.Empty;
            if (_strDBCode.Length == 0)
                throw new Exception("DBCode can not be null");
            try
            {
                conOra = new OracleConnection();
                string strConnectionString = DBConfiguration.GetPGEDataGetConnectionString();
                if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["Username_Instance"].ToString()))
                {
                    strPassword= ReadEncryption.GetPassword(System.Configuration.ConfigurationManager.AppSettings["Username_Instance"].ToString());
                    if (!string.IsNullOrEmpty(strPassword))
                    {
                        strConnectionString = strConnectionString.Replace("@Password", strPassword);
                    }

                    
                }

                

                conOra.ConnectionString = strConnectionString;
                conOra.Open();
            }
            catch (Exception ex)
            {
                objLogger.Error("OpenConnection():", ex);
                throw ex;
                //throw new DBException(ServerErrorMessage.GISServer_2005, "GISServer_2005", ex);
            }
        }
        /// <summary>
        /// Opens Oracle Database Connection
        /// </summary>
        public void OpenConnection()
        {
            string strPassword = string.Empty;
            if (_strDBCode.Length == 0)
                throw new Exception("DBCode can not be null");
            try
            {
                conOra = new OracleConnection();
                string strConnectionString = DBConfiguration.GetConnectionString(_strDBCode);
                if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["Username_Instance"].ToString()))
                {
                    strPassword = ReadEncryption.GetPassword(System.Configuration.ConfigurationManager.AppSettings["Username_Instance"].ToString());
                    if (!string.IsNullOrEmpty(strPassword))
                    {
                        strConnectionString = strConnectionString.Replace("@Password", strPassword);
                    }


                }


                conOra.ConnectionString = strConnectionString;
                conOra.Open();
            }
            catch (Exception ex)
            {
                objLogger.Error("OpenConnection():", ex);
                throw ex;
                //throw new DBException(ServerErrorMessage.GISServer_2005, "GISServer_2005", ex);
            }
        }
        /// <summary>
        /// Closes Oracle Database Connection
        /// </summary>
        public void CloseConnection()
        {
            if (conOra.State == ConnectionState.Open)
            {
                conOra.Close();
            }
            if (conOra != null)
                conOra.Dispose();
        }
        /// <summary>
        /// Begins Transaction
        /// </summary>
        public void BeginTransaction()
        {
            OpenConnection();
            transOracle = conOra.BeginTransaction();
        }
        /// <summary>
        /// Commits Transaction
        /// </summary>
        public void CommitTransaction()
        {
            transOracle.Commit();
            CloseConnection();
        }
        /// <summary>
        /// Rollbacks Transactions
        /// </summary>
        public void RollBackTransaction()
        {
            try
            {
                objLogger.Info("RollBackTransaction(): Begin Rollback");
                if (transOracle != null && transOracle.Connection != null)
                    transOracle.Rollback();
                CloseConnection();
                objLogger.Info("RollBackTransaction(): Rolledback");
            }
            catch (Exception ex)
            {

                objLogger.Error("RollBackTransaction():", ex);
                //throw new DBException(ServerErrorMessage.GISServer_2005, "GISServer_2005", ex);
            }

        }
        /// <summary>
        /// Update SQL Statement
        /// </summary>
        /// <param name="strUpdateQry">SQL Query</param>
        /// <returns></returns>
        public int UpdateQuery(string strUpdateQry)
        {
            OpenConnection();
            if (conOra == null)
                throw new Exception("Connection could not be made with Oracle DB Server");
            if (conOra.State != ConnectionState.Open)
                throw new Exception("Connection is not in Open State");
            int int_UpdateResult = 0;
            try
            {
                OracleCommand cmdUpdate;
                cmdUpdate = new OracleCommand(strUpdateQry, conOra);
                int_UpdateResult = cmdUpdate.ExecuteNonQuery();
                cmdUpdate.Dispose();

            }
            catch (Exception ex)
            {
                objLogger.Error("UpdateQuery():", ex);
                throw ex;
            }
            finally
            {
                CloseConnection();
            }
            return int_UpdateResult;
        }
        public void BulkInsert(DataTable DT, bool blTransactionBased,string TableName)
        {
            try
            {
                if (conOra == null)
                    OpenPGEDataConnection();
                if (conOra == null)
                    throw new Exception("Connection could not be made with Oracle DB Server");
                if (conOra.State != ConnectionState.Open)
                    throw new Exception("Connection is not in Open State");

                using (OracleBulkCopy bulkCopy = new OracleBulkCopy(conOra))
                {
                    bulkCopy.BulkCopyOptions = OracleBulkCopyOptions.UseInternalTransaction;
                    bulkCopy.BulkCopyTimeout = 60000;
                    bulkCopy.DestinationTableName = TableName;
                    bulkCopy.WriteToServer(DT);
                    Console.WriteLine(TableName +" Inserted Success!!");
                }
            }
            catch (Exception ex)
            {
                objLogger.Error("BulkInsert():", ex);
                throw ex;
            }
            finally
            {
                CloseConnection();
            }
           

        }
        public int UpdateSApEQUIDQuery(string strUpdateQry)
        {
            OpenPGEDataConnection();
            if (conOra == null)
                throw new Exception("Connection could not be made with Oracle DB Server");
            if (conOra.State != ConnectionState.Open)
                throw new Exception("Connection is not in Open State");
            int int_UpdateResult = 0;
            try
            {
                OracleCommand cmdUpdate;
                cmdUpdate = new OracleCommand(strUpdateQry, conOra);
                int_UpdateResult = cmdUpdate.ExecuteNonQuery();
                cmdUpdate.Dispose();

            }
            catch (Exception ex)
            {
                objLogger.Error("UpdateQuery():", ex);
                throw ex;
            }
            finally
            {
                CloseConnection();
            }
            return int_UpdateResult;
        }

        /// <summary>
        /// Performs DB Insert Operations
        /// </summary>
        /// <param name="strInsertQry"></param>
        /// <param name="blTransactionBased"></param>
        public void InsertQuery(string strInsertQry, bool blTransactionBased)
        {
            if (conOra == null)
                OpenConnection();
            if (conOra == null)
                throw new Exception("Connection could not be made with Oracle DB Server");
            if (conOra.State != ConnectionState.Open)
                throw new Exception("Connection is not in Open State");
            OracleCommand cmdInsert = null;
            try
            {
                cmdInsert = new OracleCommand(strInsertQry, conOra);
                cmdInsert.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                objLogger.Error("InsertQuery():", ex);
                throw ex;
            }
            finally
            {
                cmdInsert.Dispose();
                if (!blTransactionBased)
                    CloseConnection();

            }

        }

        /// <summary>
        /// Select query returns a DataTable of results
        /// </summary>
        /// <param name="strSelecteQry"></param>
        /// <param name="blTransactionBased"></param>
        /// <returns></returns>
        /// 
        public OracleDataReader FetchDetailsFromDB(string strSelecteQry, bool blTransactionBased)
        {
            if (conOra == null)
                OpenPGEDataConnection();
            OracleDataReader dataReader;
            if (conOra == null)
                throw new Exception("Connection could not be made with Oracle DB Server");
            if (conOra.State != ConnectionState.Open)
                throw new Exception("Connection is not in Open State");
            try
            {
                Oracle.DataAccess.Client.OracleCommand cmdSQL = new Oracle.DataAccess.Client.OracleCommand(strSelecteQry,conOra);
                dataReader = cmdSQL.ExecuteReader();
            }
            catch (Exception ex)
            {
                objLogger.Error("SelectQuery():", ex);
                throw ex;
            }
            finally
            {
                if (!blTransactionBased)
                    CloseConnection();
            }
            return dataReader;
            
        }
        public DataTable SelectQuery(string strSelecteQry, bool blTransactionBased)
        {
            if (conOra == null)
                OpenConnection();
            DataTable dtMapping = new DataTable();
            if (conOra == null)
                throw new Exception("Connection could not be made with Oracle DB Server");
            if (conOra.State != ConnectionState.Open)
                throw new Exception("Connection is not in Open State");

            try
            {
                DataSet dsMapping = new DataSet();
                OracleDataAdapter dAMapping = new OracleDataAdapter(strSelecteQry, conOra);
                dAMapping.Fill(dsMapping);
                dtMapping = dsMapping.Tables[0];
            }
            catch (Exception ex)
            {
                objLogger.Error("SelectQuery():", ex);
                throw ex;
            }
            finally
            {
                if (!blTransactionBased)
                    CloseConnection();
            }
            return dtMapping;
        }

        /// <summary>
        /// Executes Insert/Update/Delete StoredProcedure
        /// </summary>
        /// <param name="prmInOutCollection">Collection of input/output paramters</param>
        /// <param name="strStored_Proc_Name">Stored Procedure name</param>
        /// <param name="intOut_Params_Count">Output Parmater Count</param>
        /// <returns>Output Paramter Collection</returns>
        public OracleParameter[] ExecuteCommand(OracleParameter[] prmInOutCollection, string strStored_Proc_Name, int intOut_Params_Count, bool blTransactionBased)
        {
            //Open Connection
            OracleParameter[] prmOutputCollection = new OracleParameter[intOut_Params_Count];
            using (conOra)
            {
                if (conOra == null)
                    OpenConnection();

                int intUpdateResult = 0;
                OracleCommand cmdExecuteSP = null;
                try
                {
                    cmdExecuteSP = new OracleCommand();
                    cmdExecuteSP.CommandType = CommandType.StoredProcedure;
                    cmdExecuteSP.CommandTimeout = 0;
                    cmdExecuteSP.CommandText = strStored_Proc_Name;
                    cmdExecuteSP.Connection = conOra;

                    foreach (OracleParameter prmInOut in prmInOutCollection)
                    {
                        cmdExecuteSP.Parameters.Add(prmInOut);
                    }
                    intUpdateResult = cmdExecuteSP.ExecuteNonQuery();

                    //Check for output paramaters
                    if (intOut_Params_Count == 0)
                    {
                        cmdExecuteSP.Dispose();
                        return null;
                    }
                    //Get Output parameteres and add to collection
                    int intOutParamCount = 0;
                    foreach (OracleParameter prmOutput in cmdExecuteSP.Parameters)
                    {
                        if (prmOutput.Direction == ParameterDirection.Output || prmOutput.Direction == ParameterDirection.InputOutput)
                        {
                            if (intOutParamCount < intOut_Params_Count)
                            {
                                prmOutputCollection[intOutParamCount] = prmOutput;
                                intOutParamCount += 1;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //CloseConnection();
                    objLogger.Error("ExecuteCommand():", ex);
                    throw new DBException(ex.Message.ToString());
                }
                finally
                {
                    CloseConnection();
                    if (cmdExecuteSP != null)
                    {
                        cmdExecuteSP.Dispose();
                    }
                    if (!blTransactionBased)
                        CloseConnection();
                }
            }
            return prmOutputCollection;

        }
        public DataTable SelectPGEDataQuery(string strSelecteQry, bool blTransactionBased)
        {
            if (conOra == null)
                OpenPGEDataConnection();
            DataTable dtMapping = new DataTable();
            if (conOra == null)
                throw new Exception("Connection could not be made with Oracle DB Server");
            if (conOra.State != ConnectionState.Open)
                throw new Exception("Connection is not in Open State");

            try
            {
                DataSet dsMapping = new DataSet();
                OracleDataAdapter dAMapping = new OracleDataAdapter(strSelecteQry, conOra);
                dAMapping.Fill(dsMapping);
                dtMapping = dsMapping.Tables[0];
            }
            catch (Exception ex)
            {
                objLogger.Error("SelectQuery():", ex);
                throw ex;
            }
            finally
            {
                if (!blTransactionBased)
                    CloseConnection();
            }
            return dtMapping;
        }
        /// <summary>
        /// Executes Insert/Update/Delete StoredProcedure
        /// </summary>
        /// <param name="prmInOutCollection">Collection of input/output paramters</param>
        /// <param name="strStored_Proc_Name">Stored Procedure name</param>
        /// <param name="intOut_Params_Count">Output Parmater Count</param>
        /// <param name="logger">Write to  log file</param>
        /// <returns>Output Paramter Collection</returns>
        public OracleParameter[] ExecuteCommand(OracleParameter[] prmInOutCollection, string strStored_Proc_Name, int intOut_Params_Count,
            bool blTransactionBased, ILog objLogger)
        {

            //Open Connection
            if (conOra == null)
                OpenConnection();

            int intUpdateResult = 0;
            OracleParameter[] prmOutputCollection = new OracleParameter[intOut_Params_Count];
            OracleCommand cmdExecuteSP = null;
            try
            {
                cmdExecuteSP = new OracleCommand();
                cmdExecuteSP.CommandType = CommandType.StoredProcedure;
                cmdExecuteSP.CommandTimeout = 0;
                cmdExecuteSP.CommandText = strStored_Proc_Name;
                cmdExecuteSP.Connection = conOra;

                foreach (OracleParameter prmInOut in prmInOutCollection)
                {
                    cmdExecuteSP.Parameters.Add(prmInOut);
                }
                intUpdateResult = cmdExecuteSP.ExecuteNonQuery();

                //Check for output paramaters
                if (intOut_Params_Count == 0)
                {
                    cmdExecuteSP.Dispose();
                    return null;
                }
                //Get Output parameteres and add to collection
                int intOutParamCount = 0;
                foreach (OracleParameter prmOutput in cmdExecuteSP.Parameters)
                {
                    if (prmOutput.Direction == ParameterDirection.Output || prmOutput.Direction == ParameterDirection.InputOutput)
                    {
                        if (intOutParamCount < intOut_Params_Count)
                        {
                            prmOutputCollection[intOutParamCount] = prmOutput;
                            intOutParamCount += 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objLogger.Error("ExecuteCommand():", ex);
                throw ex;
            }

            finally
            {
                cmdExecuteSP.Dispose();
                if (!blTransactionBased)
                    CloseConnection();
            }
            return prmOutputCollection;
        }

        // calling Storeprocedure withou parameter 

        /// <summary>
        /// Executes Insert/Update/Delete StoredProcedure
        /// </summary>
        /// <param name="prmInOutCollection">Collection of input/output paramters</param>
        /// <param name="strStored_Proc_Name">Stored Procedure name</param>
        /// <param name="intOut_Params_Count">Output Parmater Count</param>
        /// <param name="logger">Write to  log file</param>
        /// <returns>Output Paramter Collection</returns>
        public int SPCallingWithoutParameter(string strStored_Proc_Name)
        {

            //Open Connection
            if (conOra == null)
                OpenConnection();

            int intUpdateResult = 0;
            OracleCommand cmdExecuteSP = null;
            try
            {
                cmdExecuteSP = new OracleCommand();
                cmdExecuteSP.CommandType = CommandType.StoredProcedure;
                cmdExecuteSP.CommandTimeout = 0;
                cmdExecuteSP.CommandText = strStored_Proc_Name;
                cmdExecuteSP.Connection = conOra;
                intUpdateResult = cmdExecuteSP.ExecuteNonQuery();
                               
            }
            catch (Exception ex)
            {
                objLogger.Error("ExecuteCommand():", ex);
                throw ex;
            }

            finally
            {
                cmdExecuteSP.Dispose();
                 CloseConnection();
            }
            return intUpdateResult;
        }

        // end code 
        public object ExecuteScalarPGEData(string strQuery)
        {
            OpenPGEDataConnection();
            object objSingleValue = null;
            OracleCommand cmdScalar = null;
            try
            {
                cmdScalar = new OracleCommand(strQuery, conOra);
                objSingleValue = cmdScalar.ExecuteScalar();
            }
            catch (Exception ex)
            {
                objLogger.Error("ExecuteScalar():", ex);
                throw ex;
            }

            finally
            {
                CloseConnection();
                cmdScalar.Dispose();
            }
            return objSingleValue;
        }
        /// <summary>
        /// Execute SQL Command to retriev Scalar Value
        /// </summary>
        /// <param name="strQuery">SQL Query</param>
        /// <returns>object</returns>
        public object ExecuteScalar(string strQuery)
        {
            OpenConnection();
            object objSingleValue = null;
            OracleCommand cmdScalar = null;
            try
            {
                cmdScalar = new OracleCommand(strQuery, conOra);
                objSingleValue = cmdScalar.ExecuteScalar();
            }
            catch (Exception ex)
            {
                objLogger.Error("ExecuteScalar():", ex);
                throw ex;
            }

            finally
            {
                CloseConnection();
                cmdScalar.Dispose();
            }
            return objSingleValue;
        }

        /// <summary>
        /// Extract the value of specified Paramter from the Oracle Parameter Collection
        /// </summary>
        /// <param name="prmCollection">OraclParameter Collection</param>
        /// <param name="strParam_Name">Parameter Name</param>
        /// <returns>object</returns>
        public object GetParameterValue(OracleParameter[] prmCollection, string strParam_Name)
        {
            object objValue = null;
            try
            {
                foreach (OracleParameter prmOracle in prmCollection)
                {
                    if (prmOracle.ParameterName.ToUpper() == strParam_Name.ToUpper())
                    {
                        objValue = prmOracle.Value;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return objValue;
        }

        /// <summary>
        /// This method returns dataset on the basis of provided SP name and parameters
        /// </summary>
        /// <param name="strStored_Proc_Name">Stored Procedure</param>
        /// <param name="prmInOutCollection">Input/Output Oracle Parameters</param>
        /// <returns>DataSet</returns>
        public DataSet GetDataSet(string strStored_Proc_Name, OracleParameter[] prmInOutCollection)
        {
            DataSet dsData = new DataSet();
            OracleCommand cmdSelect = null;
            OracleDataAdapter daOracle = null;
            OpenConnection();
            try
            {
                cmdSelect = new OracleCommand();
                cmdSelect.CommandType = CommandType.StoredProcedure;
                cmdSelect.CommandTimeout = 0;
                cmdSelect.CommandText = strStored_Proc_Name.ToUpper();
                cmdSelect.Connection = conOra;
                daOracle = new OracleDataAdapter();
                daOracle.SelectCommand = cmdSelect;
                foreach (OracleParameter prmInOut in prmInOutCollection)
                {
                    cmdSelect.Parameters.Add(prmInOut);
                }
                daOracle.Fill(dsData);
            }
            catch (OracleException oraException)
            {
                objLogger.Error("GetDataSet():", oraException);
                throw oraException;
            }
            catch (Exception ex)
            {
                objLogger.Error("GetDataSet():", ex);
                throw new DBException(ex.Message.ToString());
            }
            finally
            {
                CloseConnection();
                if (cmdSelect != null)
                {
                    cmdSelect.Dispose();
                }
                if (daOracle != null)
                {
                    daOracle.Dispose();
                }
            }
            return dsData;
        }
        /// <summary>
        /// This method returns dataset on the basis of provided SP name and parameters
        /// </summary>
        /// <param name="strStored_Proc_Name">Stored Procedure</param>
        /// <param name="prmInOutCollection">Input/Output Oracle Parameters</param>
        /// <returns>DataSet</returns>
        public DataSet GetDataSet(string strStored_Proc_Name, OracleParameter[] prmInOutCollection, bool transactionBased)
        {
            DataSet dsData = new DataSet();
            OracleCommand cmdSelect = null;
            OracleDataAdapter daOracle = null;
            OpenConnection();
            try
            {
                cmdSelect = new OracleCommand();
                cmdSelect.CommandType = CommandType.StoredProcedure;
                cmdSelect.CommandTimeout = 0;
                cmdSelect.CommandText = strStored_Proc_Name.ToUpper();
                cmdSelect.Connection = conOra;
                daOracle = new OracleDataAdapter();
                daOracle.SelectCommand = cmdSelect;
                foreach (OracleParameter prmInOut in prmInOutCollection)
                {
                    cmdSelect.Parameters.Add(prmInOut);
                }
                daOracle.Fill(dsData);
            }
            catch (OracleException oraException)
            {
                objLogger.Error("GetDataSet():", oraException);
                throw oraException;
            }
            catch (Exception ex)
            {
                objLogger.Error("GetDataSet():", ex);
                throw ex;
            }
            finally
            {
                daOracle.Dispose();
                cmdSelect.Dispose();
                if (!transactionBased)
                    CloseConnection();
            }
            return dsData;
        }

        /// <summary>
        /// InitializeLogger
        /// </summary>
        private void InitializeLogger()
        {
            try
            {
                //Get log file with complete path
                string logFile = GetLogfilepath();

                //log4net 
                log4net.GlobalContext.Properties["LogName"] = logFile;
                string executingAssemblyPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                executingAssemblyPath = (System.IO.Path.Combine(executingAssemblyPath, "logging.xml"));

                StreamWriter wr = new StreamWriter(executingAssemblyPath);
                wr.Write(Getlog4netConfig());
                wr.Close();
                FileInfo fi = new FileInfo(executingAssemblyPath);
                log4net.Config.XmlConfigurator.Configure(fi);

                //Initialize the logging 
               // objLogger = log4net.LogManager.GetLogger();
                //objLogger.Info("Logger Initialized");

                objLogger = log4net.LogManager.GetLogger(typeof(Program));
                objLogger.Info("Logger Initialized");
            }
            catch (Exception ex)
            {
                objLogger.Error(ex);
            }
        }
        /// <summary>
        /// GetLogfilepath gets local folder for log
        /// </summary>
        /// <returns></returns>
        /// 
        private string GetLogfilepath()
        {
            try
            {
                string executingAssemblyPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                //string logPath = (System.IO.Path.Combine(executingAssemblyPath, "C:\\Document\\Logs\\GIStoSAPLog.txt"));
                string path = System.Configuration.ConfigurationManager.AppSettings["LogFolder"];
                //string logPath = string.Format( path + "\\GIStoSAPLog_{0}.txt", DateTime.Now.ToShortDateString().Replace("/", "_"));
                return path;
            }
            catch (Exception ex)
            {
                objLogger.Error("Error in creating log file path" + ex.Message);
            }
            return "";
        }
        //private string GetLogfilepath()
        //{
        //    try
        //    {
        //        string executingAssemblyPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        //        //string logPath = (System.IO.Path.Combine(executingAssemblyPath, "C:\\Document\\Logs\\GIStoSAPLog.txt"));
        //        string path = System.Configuration.ConfigurationManager.AppSettings["LogFolder"];
        //        string logPath = string.Format(path + "\\GIStoPLDBLog_{0}.txt", DateTime.Now.ToShortDateString().Replace("/", "_"));
        //        return logPath;
        //    }
        //    catch (Exception ex)
        //    {
        //        objLogger.Info(ex.Message);
        //    }
        //    return "";
        //}
        /// <summary>
        /// Getlog4netConfig
        /// </summary>
        /// <returns></returns>
        private string Getlog4netConfig()
        {

            string Config = @"<?xml version=""1.0"" encoding=""utf-8"" ?> 
                                    <configuration> 
                                      <log4net debug=""False""> 
                                        <!-- Define some output appenders --> 
                                        <appender name=""RollingLogFileAppender"" type=""log4net.Appender.RollingFileAppender""> 
                                          <file type=""log4net.Util.PatternString"" value=""%property{LogName}"" /> 
                                          <appendToFile value=""true"" /> 
                                          <rollingStyle value=""Composite"" />                               
                                         <datePattern value=""ddMMyyyy"" />
                                          <maxSizeRollBackups value=""30"" /> 
                                          <maximumFileSize value=""1MB"" />                                                                                    
                                        <layout type=""log4net.Layout.PatternLayout"">
                                           <conversionPattern value=""%date [%thread] %level %logger - %message%newline""/>
                                        </layout>
                                        </appender> 
                 
                                   <!-- Setup the root category, add the appenders and set the default level --> 
                                   <root> 
                                   <level value=""ALL""/> 
                                   <appender-ref ref=""RollingLogFileAppender"" /> 
                                   </root> 
                                   </log4net> 
                                    </configuration>";
            return Config;
        }
    }              
}
