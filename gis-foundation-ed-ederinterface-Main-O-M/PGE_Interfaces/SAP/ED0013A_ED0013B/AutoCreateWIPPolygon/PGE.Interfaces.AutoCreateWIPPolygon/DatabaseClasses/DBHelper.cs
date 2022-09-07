using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using System.Data.Common;
using Oracle.DataAccess.Client;


namespace PGE.Interfaces.AutoCreateWIPPolygon
{
       
        public   class DBHelper
        {
            
            private static OracleConnection conOraEDER = null;
            

            private   void OpenConnection(out OracleConnection objconOra)
            {
                objconOra = null;
                try
                {
                    if (conOraEDER == null)
                    {

                    // M4JF EDGISREARCH 919 
                    //string[] oracleConnectionInfo = ReadConfigurations.OracleConnString_WIP.Split(',');

                    //string oracleConnectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + oracleConnectionInfo[0] + ")(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" + oracleConnectionInfo[1] + "))); User Id=" + oracleConnectionInfo[2] + ";Password=" + oracleConnectionInfo[3] + ";Pooling=false";
                    string oracleConnectionString = ReadConfigurations.OracleConnString_WIP;
                    //Check whether the connection string present in the config file or not if not then 
                    //show the message to the user.
                    if (string.IsNullOrEmpty(oracleConnectionString))
                        {
                            Common._log.Error("Cannot read EDER Connection String path from configuration file");
                            
                        }

                        conOraEDER = new OracleConnection();
                        //make the database connection
                        conOraEDER.ConnectionString = oracleConnectionString;
                    }
                    else
                    {
                        objconOra = conOraEDER;
                    }
                    if (objconOra.State != ConnectionState.Open)
                    {
                        objconOra.Open();

                    }

                }
                catch (Exception ex)
                {
                    Common._log.Error("Unhandled exception encountered while Open the Oracle Connection : --" + ex.Message.ToString() + " at " + ex.StackTrace);
                }
            }

            private   void CloseConnection(OracleConnection objconOra, out OracleConnection objReleaseConn)
            {
                objReleaseConn = null;
                try
                {
                    //close the database connection
                    if (objconOra != null)
                    {
                        if (objconOra.State == ConnectionState.Open)
                            objconOra.Close();
                        objconOra.Dispose();
                        objconOra = null;
                        objReleaseConn = objconOra;

                    }
                }

                catch (Exception ex)
                {
                    objconOra = null;
                }

            }

            private void OpenConnectionforSetting(out OracleConnection conOraSetting)
            {
                conOraSetting = null;
                try
                {
                    if (conOraSetting == null)
                    {

                    // M4JF EDGISREARCH 919
                    //string[] oracleConnectionInfo = ReadConfigurations.OracleConnString_SettingDB.Split(',');

                    //string oracleConnectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + oracleConnectionInfo[0] + ")(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" + oracleConnectionInfo[1] + "))); User Id=" + oracleConnectionInfo[2] + ";Password=" + oracleConnectionInfo[3] + ";Pooling=false";
                    string oracleConnectionString = ReadConfigurations.OracleConnString_SettingDB;

                    //Check whether the connection string present in the config file or not if not then 
                    //show the message to the user.
                    if (string.IsNullOrEmpty(oracleConnectionString))
                        {
                            Common._log.Error("Cannot read EDER Connection String path from configuration file");

                        }

                        conOraSetting = new OracleConnection();
                        //make the database connection
                        conOraSetting.ConnectionString = oracleConnectionString;
                    }
                    if (conOraSetting.State != ConnectionState.Open)
                    {
                        conOraSetting.Open();

                    }

                }
                catch (Exception ex)
                {
                    Common._log.Error("Unhandled exception encountered while Open the Oracle Connection : --" + ex.Message.ToString() + " at " + ex.StackTrace);
                }
            }
            public   DataRow GetSingleDataRowByQuery(string strQuery)
            {
                DataRow dr = null;
                //Open Connection
                if (conOraEDER == null)
                    OpenConnection(out conOraEDER);
                DataSet dsData = new DataSet();
                DataTable DT = new DataTable() ;
                OracleCommand cmdExecuteSQL = null;
                OracleDataAdapter daOracle = null;
                try
                {
                    cmdExecuteSQL = new OracleCommand();
                    cmdExecuteSQL.CommandType = CommandType.Text;
                    cmdExecuteSQL.CommandTimeout = 0;
                    cmdExecuteSQL.CommandText = strQuery;
                    cmdExecuteSQL.Connection = conOraEDER;

                    daOracle = new OracleDataAdapter();
                    daOracle.SelectCommand = cmdExecuteSQL;
                    daOracle.Fill(DT);
                    if (DT != null)
                    {
                        if (DT.Rows.Count > 0)
                        {
                            dr = DT.Rows[0];
                        }
                    }
                }
                catch (Exception ex)
                {
                    Common._log.Error("Unhandled exception encountered while getting the data from query with querystring " + strQuery + " Exception is - " + ex.Message.ToString() + " at " + ex.StackTrace);
                }
                finally
                {
                    cmdExecuteSQL.Dispose();
                    daOracle.Dispose();
                    CloseConnection(conOraEDER, out conOraEDER);

                }
                return dr;
            }

            public    DataTable GetDataTable(string strQuery)
            {
                strQuery = strQuery.Trim();
                //Open Connection
                if (conOraEDER == null)
                    OpenConnection(out conOraEDER);
                DataSet dsData = new DataSet();
                DataTable DT = new DataTable();
                OracleCommand cmdExecuteSQL = null;
                OracleDataAdapter daOracle = null;
                try
                {
                    cmdExecuteSQL = new OracleCommand();
                    cmdExecuteSQL.CommandType = CommandType.Text;
                    cmdExecuteSQL.CommandTimeout = 0;
                    cmdExecuteSQL.CommandText = strQuery;
                    cmdExecuteSQL.Connection = conOraEDER;

                    daOracle = new OracleDataAdapter();
                    daOracle.SelectCommand = cmdExecuteSQL;
                    daOracle.Fill(dsData);
                    DT = dsData.Tables[0];
                    
                }
                catch (Exception ex)
                {
                    Common._log.Error("Unhandled exception encountered while getting the data from query with querystring " + strQuery + " Exception is - " + ex.Message.ToString() + " at " + ex.StackTrace);
                }
                finally
                {
                    cmdExecuteSQL.Dispose();
                    daOracle.Dispose();
                    CloseConnection(conOraEDER, out conOraEDER);

                }
                return DT;
            }


            public   bool BulkCopyDataFromDataTableToDatabaseTable(DataTable dt, string tableName)
            {
                bool retval = false;
                try
                {
                    if (conOraEDER == null)
                    {
                        OpenConnection(out conOraEDER);
                    }
                  
                    using (OracleBulkCopy bulkCopy = new OracleBulkCopy(conOraEDER))
                    {
                        bulkCopy.DestinationTableName = tableName;
                        bulkCopy.BulkCopyTimeout = 3000;
                        foreach (DataColumn column in dt.Columns)
                        {
                            switch (column.ColumnName)
                            {

                                case "XML_FILE_NAME":
                                     bulkCopy.ColumnMappings.Add(new OracleBulkCopyColumnMapping(column.ColumnName, ReadConfigurations.WIP_STAGING_TABLE_FIELDS.XML_FILE_NAME));
                                    break;
                                case "NOTIFICATION":
                                    bulkCopy.ColumnMappings.Add(new OracleBulkCopyColumnMapping(column.ColumnName, ReadConfigurations.WIP_STAGING_TABLE_FIELDS.NOTIFICATION_NUMBER));
                                    break;
                                case "NOTIFICATIONTYPE":
                                    bulkCopy.ColumnMappings.Add(new OracleBulkCopyColumnMapping(column.ColumnName, ReadConfigurations.WIP_STAGING_TABLE_FIELDS.NOTIFICATION_TYPE));
                                    break;
                                case "NOTIFDESCRIPTION":
                                    bulkCopy.ColumnMappings.Add(new OracleBulkCopyColumnMapping(column.ColumnName,ReadConfigurations.WIP_STAGING_TABLE_FIELDS.DESCRIPTION));
                                    break;
                                case "WORKTYPE":
                                    bulkCopy.ColumnMappings.Add(new OracleBulkCopyColumnMapping(column.ColumnName, ReadConfigurations.WIP_STAGING_TABLE_FIELDS.WORKTYPE));
                                    break;
                                case "PMORDERNUMBER":
                                    bulkCopy.ColumnMappings.Add(new OracleBulkCopyColumnMapping(column.ColumnName, ReadConfigurations.WIP_STAGING_TABLE_FIELDS.INSTALLJOBNUMBER));
                                    break;
                                case "ORDERTYPE":
                                    bulkCopy.ColumnMappings.Add(new OracleBulkCopyColumnMapping(column.ColumnName, ReadConfigurations.WIP_STAGING_TABLE_FIELDS.ORDERTYPE ));
                                    break;
                                case "ORDERCREATEDATE":
                                    bulkCopy.ColumnMappings.Add(new OracleBulkCopyColumnMapping(column.ColumnName,ReadConfigurations.WIP_STAGING_TABLE_FIELDS.ORDERCREATEDATE));
                                    break;
                                case "OBJECTCLASS":
                                    bulkCopy.ColumnMappings.Add(new OracleBulkCopyColumnMapping(column.ColumnName, ReadConfigurations.WIP_STAGING_TABLE_FIELDS.OBJECTCLASS));
                                    break;
                                case "SAPEQUIPMENTID":
                                    bulkCopy.ColumnMappings.Add(new OracleBulkCopyColumnMapping(column.ColumnName, ReadConfigurations.WIP_STAGING_TABLE_FIELDS.SAPEQUIPID));
                                    break;
                                case "LATITUDE":
                                    bulkCopy.ColumnMappings.Add(new OracleBulkCopyColumnMapping(column.ColumnName, ReadConfigurations.WIP_STAGING_TABLE_FIELDS.GPSLATITUDE));
                                    break;
                                case "LONGITUDE":
                                    bulkCopy.ColumnMappings.Add(new OracleBulkCopyColumnMapping(column.ColumnName,ReadConfigurations.WIP_STAGING_TABLE_FIELDS.GPSLONGITUDE));
                                    break;
                                case "PIN":
                                    bulkCopy.ColumnMappings.Add(new OracleBulkCopyColumnMapping(column.ColumnName,ReadConfigurations.WIP_STAGING_TABLE_FIELDS.PIN));
                                    break;
                                case "OIS":
                                    bulkCopy.ColumnMappings.Add(new OracleBulkCopyColumnMapping(column.ColumnName, ReadConfigurations.WIP_STAGING_TABLE_FIELDS.OIS));
                                    break;
                                case "SSD":
                                    bulkCopy.ColumnMappings.Add(new OracleBulkCopyColumnMapping(column.ColumnName, ReadConfigurations.WIP_STAGING_TABLE_FIELDS.SSD));
                                    break;
                                case "CIRCUIT":
                                    bulkCopy.ColumnMappings.Add(new OracleBulkCopyColumnMapping(column.ColumnName, ReadConfigurations.WIP_STAGING_TABLE_FIELDS.CIRCUIT));
                                    break;

                                case "PLATMAP":
                                    bulkCopy.ColumnMappings.Add(new OracleBulkCopyColumnMapping(column.ColumnName, ReadConfigurations.WIP_STAGING_TABLE_FIELDS.PLATMAP));
                                    break;
                                case "JOBOWNER":
                                    bulkCopy.ColumnMappings.Add(new OracleBulkCopyColumnMapping(column.ColumnName, ReadConfigurations.WIP_STAGING_TABLE_FIELDS.JOBOWNERLANID));
                                    break;
                                case "FILECREATIONDT":
                                    bulkCopy.ColumnMappings.Add(new OracleBulkCopyColumnMapping(column.ColumnName,ReadConfigurations.WIP_STAGING_TABLE_FIELDS.FILECREATIONDT));
                                    break;
                                case "BATCHID":
                                    bulkCopy.ColumnMappings.Add(new OracleBulkCopyColumnMapping(column.ColumnName, ReadConfigurations.WIP_STAGING_TABLE_FIELDS.BATCHID));
                                    break;
                                case "STREET":
                                    bulkCopy.ColumnMappings.Add(new OracleBulkCopyColumnMapping(column.ColumnName, ReadConfigurations.WIP_STAGING_TABLE_FIELDS.STREETADDRESS));
                                    break;
                                case "SEQUENCE":
                                    bulkCopy.ColumnMappings.Add(new OracleBulkCopyColumnMapping(column.ColumnName, ReadConfigurations.WIP_STAGING_TABLE_FIELDS.SEQUENCE));
                                    break;
                              
                            }
                        }
                        bulkCopy.WriteToServer(dt);
                        retval = true;
                    }
                    
                }
                catch (Exception ex)
                {
                    Common._log.Error("Exception encountered while BulkCopy the Data in Table " + tableName, ex);
                    retval = false;
                }
                finally
                {
                    CloseConnection(conOraEDER,out conOraEDER);
                }
                return retval;
            }
        
       
            public int GetSequence(string SequenceName)
            {
                Common CommonFuntions = new Common();
                int value = 0;
                try
                {
                    
                    if (conOraEDER == null)
                        OpenConnection(out conOraEDER);

                    OracleCommand cmdzs = new OracleCommand("select " + SequenceName + ".nextval FROM dual", conOraEDER);
                    cmdzs.CommandType = CommandType.Text;
                    var value_SP = cmdzs.ExecuteScalar();
                    if (value_SP.ToString() == "")
                    { value = 0; }
                    else
                    {
                        value = Convert.ToInt32(value_SP);
                    }

                }
                catch (Exception ex)
                {
                    Common._log.Error("Exception encountered while picking the sequence," + ex.Message.ToString() + " at " + ex.StackTrace);

                }
                finally
                {

                    CloseConnection(conOraEDER, out conOraEDER);

                }
                return value;
            }
            public int UpdateQuery(string strUpdateQry)
            {

                Common CommonFuntions = new Common();
                OpenConnection(out conOraEDER);

                int intUpdateResult = -1;
                OracleCommand cmdExecuteQuery = null;
                try
                {
                    cmdExecuteQuery = new OracleCommand(strUpdateQry, conOraEDER);
                    cmdExecuteQuery.CommandTimeout = 0;
                    intUpdateResult = cmdExecuteQuery.ExecuteNonQuery();
                }

                catch (Exception ex)
                {
                    Common._log.Error("Exception encountered while update Table with QueryString" + strUpdateQry + "," + ex.Message.ToString() + " at " + ex.StackTrace);
                }
                finally
                {
                    cmdExecuteQuery.Dispose();
                    CloseConnection(conOraEDER, out conOraEDER);
                }
                return intUpdateResult;
            }

            internal string GetCSDValueFromSettingDatabase(string strquery)
            {
                string sretval = string.Empty;
                OracleConnection conOraSetting = null;
                strquery = strquery.Trim();
                //Open Connection
                if (conOraSetting == null)
                    OpenConnectionforSetting(out conOraSetting);
                DataSet dsData = new DataSet();
                DataTable DT = new DataTable();
                OracleCommand cmdExecuteSQL = null;
                OracleDataAdapter daOracle = null;
                try
                {
                    cmdExecuteSQL = new OracleCommand();
                    cmdExecuteSQL.CommandType = CommandType.Text;
                    cmdExecuteSQL.CommandTimeout = 0;
                    cmdExecuteSQL.CommandText = strquery;
                    cmdExecuteSQL.Connection = conOraSetting;

                    daOracle = new OracleDataAdapter();
                    daOracle.SelectCommand = cmdExecuteSQL;
                    daOracle.Fill(dsData);
                    DT = dsData.Tables[0];
                    if((DT!=null)&&(DT.Rows.Count>0))
                    {
                        sretval = DT.Rows[0].ItemArray[0].ToString();
                    }
                }
                catch (Exception ex)
                {
                    Common._log.Error("Unhandled exception encountered while getting the data from query with querystring " + strquery + " Exception is - " + ex.Message.ToString() + " at " + ex.StackTrace);
                }
                finally
                {
                    cmdExecuteSQL.Dispose();
                    daOracle.Dispose();
                    CloseConnection(conOraSetting, out conOraSetting);

                }
                return sretval;
            }

           
        }

    
}
