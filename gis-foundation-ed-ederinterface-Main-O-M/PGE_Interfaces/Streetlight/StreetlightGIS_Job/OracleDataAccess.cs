using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using System.Data;

namespace PGE.Interfaces.StreetlightGIS_Job
{
    class OracleDataAccess
    {
        OracleConnection connection = null;
        //Constructor of claas used for initializing the connection.
        public OracleDataAccess(string connectionString)
        {
            InitConnection(connectionString);
        }    

        /// <summary>
        /// Get Property for OleDbConnection
        /// </summary>
        public OracleConnection GetConnection
        {
            get { return connection; }
        }

        /// <summary>
        /// Initializes a connection
        /// </summary>
        /// <param name="ConnectionString">Connection string</param>

        private void InitConnection(string connectionString)
        {
            connection = new OracleConnection(connectionString);
            if (connection.State != ConnectionState.Open)
            {
                try
                {
                    //open a connection
                    connection.Open();
                }
                catch (OracleException excep)
                {
                    //logger.Error("Error in  Connection", excep);
                    throw excep;
                }
            }
        }

        /// <summary>
        /// Creates a command for query
        /// </summary>
        /// <param name="type">Type of command</param>
        /// <param name="query">Query assigned for command</param>
        /// <param name="Values">Parameters if any.</param>
        /// <returns><see cref="OleDbCOmmand"/> object</returns>
        private OracleCommand GetCommand(CommandType type, string query, OracleParameter[] Values)
        {
            OracleCommand command = new OracleCommand();
            command.Connection = connection;
            command.CommandText = query;
            command.CommandType = type;
            if (Values != null && Values.Length > 0)
                command.Parameters.AddRange(Values);
            return command;
        }
        /// <summary>
        /// This function is used for single record insert 
        /// </summary>
        /// <param name="query">string query</param>
        /// <param name="values">OleDbParameter[] values</param>
        /// <param name="type">CommandType type</param>
        /// <returns></returns>
        public bool InsertData(string query, OracleParameter[] values, CommandType type)
        {
            bool blnIsInserted = false;
            OracleCommand command = null;
            try
            {

                command = GetCommand(type, query, values);
                int iRowsAffected = command.ExecuteNonQuery();
                if (iRowsAffected > 0)
                {
                    blnIsInserted = true;
                }
            }
            catch (Exception ex)
            {
                //logger.Error(ex.Message, ex);
                throw ex;
            }
            finally
            {
                // daPhoto.Dispose();
                //Releases data objects
                //ReleaseDataObjects(null, command);
            }
            return blnIsInserted;
        }
        /// <summary>
        /// this function is used for inserting multiple record
        /// </summary>
        /// <param name="queries">List<string> queries</param>
        /// <param name="values">OleDbParameter[] values</param>
        /// <param name="type">CommandType type</param>
        /// <returns>bool isInserted</returns>
        public bool InsertData(List<string> queries, OracleParameter[] values, CommandType type)
        {
            bool blnIsInserted = false;
            OracleCommand command = null;
            OracleTransaction myTrans = null;
            try
            {                
                myTrans = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                int iRowsAffected = -1;
                for (int iQueryCnt = 0; iQueryCnt < queries.Count; iQueryCnt++)
                {
                    
                    command = GetCommand(type, queries[iQueryCnt], values);
                    command.Transaction = myTrans;
                    iRowsAffected = command.ExecuteNonQuery();
                    if (iRowsAffected > 0)
                    {
                        blnIsInserted = true;
                    }
                }
                myTrans.Commit();

            }
            catch (Exception ex)
            {
              
                //logger.Error(ex.Message, ex);
                if (myTrans != null)
                {
                    try
                    {
                        myTrans.Rollback();
                    }
                    catch
                    {
                        //logger.Error(ex.Message, ex);
                        throw ex;
                    }
                }
                throw ex;

            }
            finally
            {
                // daPhoto.Dispose();
                //Releases data objects
                if (myTrans != null)
                {
                    myTrans.Dispose();
                    myTrans = null;
                }
                //ReleaseDataObjects(null, command);
               
            }
            return blnIsInserted;
           
        }
        /// <summary>
        /// Fills datatable from Data Adapter based on the query
        /// </summary>
        /// <param name="query">Query to be executed</param>
        /// <param name="values">Parameters if any</param>
        /// <param name="type">Type of command</param>
        /// <returns>DataTable</returns>
        public DataTable GetDataTableFromAdapter(string query, OracleParameter[] values, CommandType type)
        {
            DataTable _dt = new DataTable();
            OracleDataAdapter daPhoto = null;
            OracleCommand command = null;
            try
            {
                command = GetCommand(type, query, values);
                daPhoto = new OracleDataAdapter(command);
                //Fills datatable from data adapter
                daPhoto.Fill(_dt);
            }
            catch (Exception ex)
            {
                //logger.Error(ex.Message, ex);
                throw ex;
            }
            finally
            {
                daPhoto.Dispose();
                //Releases data objects
                //ReleaseDataObjects(null, command);
            }
            return _dt;

        }

        /// <summary>
        /// Releases the OleDB data objects
        /// </summary>
        /// <param name="reader"><see cref="OleDBDataReader"/> object</param>
        /// <param name="command"><see cref="OleDbCommand"/> object</param>
        public void ReleaseDataObjects(OracleDataReader reader, OracleCommand command)
        {
            if (reader != null)
            {
                try
                {
                    reader.Close();
                    reader.Dispose();
                }
                catch (OracleException)
                {
                    //logger.Warn("Could not dispose the passed OleDBDataReader");
                }
            }
            if (command != null)
            {
                try
                {
                    command.Dispose();
                }
                catch (OracleException)
                {
                    //logger.Warn("Could not dispose the passed OleDBCommand");
                }
            }
            if (connection != null)
            {
                try
                {
                    // Close the connection
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
                catch (OracleException)
                {
                    //logger.Warn("Could not close the passed OleDBConnection");
                }
            }
        }

        public void ReleaseDataObjects()
        {
            if (connection != null)
            {
                try
                {
                    // Close the connection
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
                catch (OracleException)
                {
                    //logger.Warn("Could not close the passed OleDBConnection");
                }
            }
        }

        public Int32 ExecuteQuery(string strQuery,OracleParameter[] values, CommandType type)
        {
            Int32 intRowsAffected = -1;
            OracleCommand command = null;
            try
            {
                command = GetCommand(type, strQuery,values);
                intRowsAffected = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                command.Dispose();
                //Releases data objects
                //ReleaseDataObjects(null, command);
            }
            return intRowsAffected;
        }


        public List<string> ExecuteReader(string strQuery, OracleParameter[] values, CommandType type, int ColumnCount)
        {
            OracleCommand command = null;
            OracleDataReader _reader = null;
            List<string> lstValues = new List<string>();
            try
            {
                command = GetCommand(type, strQuery, values);
                _reader = command.ExecuteReader();
                while (_reader.Read())
                {
                    for (int iCnt = 0;iCnt<ColumnCount;iCnt++)
                    {
                        lstValues.Add(_reader.GetValue(iCnt).ToString());
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                command.Dispose();
                //Releases data objects
                //ReleaseDataObjects(null, command);
            }
            return lstValues;
        }

        

    }
}
