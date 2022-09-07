using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Oracle.DataAccess.Client;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Interface.Integration.DMS.Common
{
    public class Oracle
    {
        private static Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "ED50.log4net.config");
        public static string CreateOracleConnectionString(string server, string sid, string user, string pass)
        {
            string connection = "";
            if (Configuration.getBoolSetting("UseTNSConnection", false))
            {
                connection = String.Format("Data Source={0};", sid);
            }
            else
            {
                connection = String.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SID={1})));", server, sid);
            }
            connection += String.Format("User Id={0};Password={1};", user, pass);
            return connection;
        }
        public static void BulkLoadData(String file, string connection)
        {
            DataSet set = new DataSet();
            set.ReadXml(file);
            BulkLoadData(set, connection);
        }
        public static void LoadData(DataSet set, string connection)
        {
        }
        public static void BulkLoadData(DataSet set, string connection)
        {
            OracleConnection conn = null;

            try
            {               
                conn = new OracleConnection(connection);
                conn.Open();
                using (var copy = new OracleBulkCopy(conn))
                {
                    //copy.DestinationTableName = tableName;
                    copy.BulkCopyTimeout = Configuration.getIntSetting("BulkCopyTimeout",60);
                    //copy.BatchSize = BatchSize;
                    //copy.Insert(entities);
                    foreach (DataTable table in set.Tables)
                    {
                        _log.Debug("Writing Table to server: " + table.TableName);
                        copy.DestinationTableName = table.TableName;
                        copy.WriteToServer(table);
                    }
                    copy.Close();
                    copy.Dispose();
                }

            }
            catch (Exception ex)
            {
                _log.Error("Error Loading Data.  " + System.Environment.NewLine + ex.ToString());
                throw ex;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                    conn = null;
                }
            }
        }
        public static DataSet GetSchema(string connection, string schema)
        {
            OracleConnection conn = null;

            try
            {
                conn = new OracleConnection(connection);
                conn.Open();
                DataTable oleschema = conn.GetSchema("Tables");//This returns a table with a list of all the tables in the connected schema
                System.Data.Common.DbCommand GetTableCmd = conn.CreateCommand();
                System.Data.Common.DbDataAdapter ODA = null;

                ODA = new OracleDataAdapter() as System.Data.Common.DbDataAdapter;

                ODA.SelectCommand = GetTableCmd;
                DataSet set = new DataSet();
                foreach (DataRow row in oleschema.Rows)
                {
                    if (row["OWNER"].Equals(schema))
                    {
                        DataTable DBTable = new DataTable();
                        GetTableCmd.CommandText = "SELECT * FROM " + row["OWNER"] + "." + row["TABLE_NAME"]; ;
                        ODA.FillSchema(DBTable, SchemaType.Source);//This pulls down the schema for the given table
                        DBTable.TableName = row["OWNER"] + "." + DBTable.TableName;
                        set.Tables.Add(DBTable);
                        DBTable = null;
                    }
                }

                return set;

            }
            catch (Exception ex)
            {
                _log.Error("Error Getting Schema.  " + System.Environment.NewLine + ex.ToString());
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                    conn = null;
                }
            }
            return null;
        }
        public static object GetSingleValue(string sql, string connection)
        {
            object output = null;
            OracleConnection conn = null;
            if (connection != null)
            {
                try
                {
                    conn = new OracleConnection();
                    conn.ConnectionString = connection;
                    conn.Open();

                    OracleCommand cmd = new OracleCommand(sql, conn);
                    cmd.CommandType = CommandType.Text;
                    output = cmd.ExecuteScalar();
                    cmd.Dispose();
                }
                catch (Exception ex)
                {
                    _log.Error("Oracle Error getting single value. " + sql, ex);
                }
                finally
                {
                    if (conn != null)
                    {
                        conn.Close();
                        conn.Dispose();
                        conn = null;
                    }
                }


            }


            return output;
        }

        /// <summary>
        /// Execute an Oracle stored procedure
        /// RBAE - 11/12/13 - Moved here from ControlTable so that CircuitFinder could use it as well to truncate the
        /// Change Detection tables
        /// </summary>
        /// <param name="procedure">The name of the stored procedure to run</param>
        public static void ExecuteProcedure(string connection, string procedure)
        {
            int rowsChanged = 0;
            OracleConnection oracleConn = null; new OracleConnection(connection);
            try
            {
                oracleConn = new OracleConnection(connection);
                oracleConn.Open();
                using (var cmd = new OracleCommand(procedure, oracleConn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    _log.Debug("Executing Stored Procedure: " + procedure);
                    rowsChanged = (int)cmd.ExecuteNonQuery();
                    _log.Debug("Finished Executing Stored Procedure: " + procedure);
                    cmd.Dispose();
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error executing stored procedure: " + procedure, ex);
            }
            finally
            {
                if (oracleConn.State == ConnectionState.Open)
                    oracleConn.Close();
                oracleConn.Dispose();
                oracleConn = null;
            }
        }
    }
}
