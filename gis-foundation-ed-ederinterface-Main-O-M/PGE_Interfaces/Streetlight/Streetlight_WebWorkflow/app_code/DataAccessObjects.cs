/**************************************************************************************************************
Project            :ETGISViewer Application
Module             :Database Connection 
File Name          :DataAccessObjects.cs
File Description   :This file is used for connecting and fetching data from Database
Author             :Tata Consultancy Services
Date of Creation   :26-06-2014
Company            :TCS
Version            :1.0.0
**************************************************************************************************************/
/*Update Log
Date Modified    Changed By        Description

**************************************************************************************************************/

using System.Data.OracleClient;
using System.Data;
using System;
using System.Collections.Generic;
using log4net;
using System.Runtime.CompilerServices;


namespace PGE.Streetlight
{
    /// <summary>
    /// DataAccessObjects class is defined for making a connection to database and retrieving values from database.
    /// </summary>
    public class DataAccessObjects
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(DataAccessObjects));
        OracleConnection connection = null;
        //Constructor of claas used for initializing the connection.
        public DataAccessObjects(string connectionString)
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
                    logger.Debug("Opening Connection");
                    connection.Open();

                }
                catch (OracleException excep)
                {
                    logger.Error("Error in  Connection", excep);
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


            //foreach (OracleParameter prmInOut in Values)
            //{
            //    command.Parameters.Add(prmInOut);
            //}

            return command;

        }
        /// <summary>
        /// This function is used for single record insert 
        /// </summary>
        /// <param name="query">string query</param>
        /// <param name="values">OleDbParameter[] values</param>
        /// <param name="type">CommandType type</param>
        /// <returns></returns>
        public bool InsertData(string query)
        {
            bool blnIsInserted = false;
            OracleCommand command = null;
            OracleParameter[] values = null;
            CommandType type = CommandType.Text;
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
                logger.Error(ex.Message, ex);
               // throw ex;
                blnIsInserted = false;
            }
            finally
            {
                // daPhoto.Dispose();
                //Releases data objects
                ReleaseDataObjects(null, command);
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
        public bool InsertData(List<string> queries)
        {
            bool blnIsInserted = false;
            OracleCommand command = null;
            OracleTransaction myTrans = null;
            OracleParameter[] values = null;
            CommandType type = CommandType.Text;
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

                logger.Error(ex.Message, ex);
                if (myTrans != null)
                {
                    try
                    {
                        myTrans.Rollback();
                    }
                    catch
                    {
                        logger.Error(ex.Message, ex);
                        //throw ex;
                    }
                }
               // throw ex;
                blnIsInserted = false;

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
                ReleaseDataObjects(null, command);

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
        public DataTable GetDataTableFromAdapter(string query)
        {
            DataTable _dt = new DataTable();
            OracleDataAdapter daPhoto = null;
            OracleCommand command = null;
            OracleParameter[] values = null;
            CommandType type = CommandType.Text;
            try
            {
                command = GetCommand(type, query, values);
                daPhoto = new OracleDataAdapter(command);
                //Fills datatable from data adapter
                daPhoto.Fill(_dt);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
               // throw ex;
            }
            finally
            {
                daPhoto.Dispose();
                //Releases data objects
                ReleaseDataObjects(null, command);
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
                    logger.Warn("Could not dispose the passed OleDBDataReader");
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
                    logger.Warn("Could not dispose the passed OleDBCommand");
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
                    logger.Warn("Could not close the passed OleDBConnection");
                }
            }
        }



    }
}
