using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGE.Common.Delivery.Diagnostics;
using Oracle.DataAccess.Client;
using System.Text.RegularExpressions;
using System.Data;
using PGE_DBPasswordManagement;

namespace PGE.BatchApplication.PGE_Tx_SL_Connectivity_Report
{
    public static class OracleConnectionControl
    {
        private static Log4NetLogger _logger = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        
        private static string CreateOracleConnectionString(string server, string sid, string user, string pass)
        {
            string connection = connection = String.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SID={1})));", server, sid);
            connection += String.Format("User Id={0};Password={1};", user, pass);
            return connection;
        }

        public static void BulkLoadData(DataSet set)
        {
            // string[] connSepeartedList = Regex.Split(Common.OracleConnection, ",");
            // string connection = CreateOracleConnectionString(connSepeartedList[0], connSepeartedList[1], connSepeartedList[2], connSepeartedList[3]);

            // m4jf edgisrearch 919 - get oracle connection string using Password Management tool
            string connection = ReadEncryption.GetConnectionStr(Common.OracleConnection);
            OracleConnection conn = null;

            try
            {
                conn = new OracleConnection(connection);
                conn.Open();
                using (var copy = new OracleBulkCopy(conn))
                {
                    //copy.DestinationTableName = tableName;
                    //copy.BulkCopyTimeout = DefaultTimeoutInSeconds;
                    //copy.BatchSize = BatchSize;
                    //copy.Insert(entities);
                    foreach (DataTable table in set.Tables)
                    {
                        //_log.Debug("Loading Table " + table.TableName);
                        copy.DestinationTableName = table.TableName;
                        copy.WriteToServer(table);
                    }
                    copy.Close();
                    copy.Dispose();
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Error Loading Data.  " + System.Environment.NewLine + ex.ToString());
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

        public static DataSet GetDataset()
        {
            // string[] connSepeartedList = Regex.Split(Common.OracleConnection, ",");
            //string connection = CreateOracleConnectionString(connSepeartedList[0], connSepeartedList[1], connSepeartedList[2], connSepeartedList[3]);

            // m4jf edgisrearch 919 - get oracle connection string using Password Management tool
            string connection = ReadEncryption.GetConnectionStr(Common.OracleConnection);
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
                    if (row["TABLE_NAME"].Equals("PGE_TX_SL_CONNECTIVITY"))
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
                _logger.Error("Error Getting Schema.  " + System.Environment.NewLine + ex.ToString());
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
    }
}
