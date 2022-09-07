using System;
using System.Collections.Generic;
using System.Data;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.ChangesManagerShared;
using PGE.Common.ChangesManagerShared.Interfaces;
using PGE.Common.ChangesManagerShared.Utilities;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.BatchApplication.ChangeDetection.ETL
{
    public class SettingsToEDERETL : IExtractTransformLoad
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "ChangeDetectionDefault.log4net.config");

        private AdoOracleConnection _adoOracleConnectionSource;
        private AdoOracleConnection _adoOracleConnectionDestination;

        private IList<RowTransferrer> _rowTransferrers;

        private string _lastExecutedTable;
        private string _storedProc;

        private DateTime _executedTime;

        private const string FIELD_LASTDATE = "LASTDATE";
        private const string FIELD_PROCESS_NAME = "PROCESS_NAME";
        private const string BEGINNING_OF_TIME = "2000-01-01 12:00:00";
        private string PROCESS_NAME = "SETTINGS_ETL";

        public SettingsToEDERETL()
        {

        }
        /// <summary>
        /// Added new argument PROCESSNAME because previously it was working only for setting now it will work for setting_ADMS also
        /// </summary>
        /// <param name="adoOracleConnectionSource"></param>
        /// <param name="adoOracleConnectionDestination"></param>
        /// <param name="storedProc"></param>
        /// <param name="lastExecutedTable"></param>
        /// <param name="rowTransferrers"></param>
        /// <param name="PROCESSNAME"></param>
        public SettingsToEDERETL(AdoOracleConnection adoOracleConnectionSource,
            AdoOracleConnection adoOracleConnectionDestination, string storedProc, string lastExecutedTable,
            IList<RowTransferrer> rowTransferrers, string PROCESSNAME)
        {
            _adoOracleConnectionSource = adoOracleConnectionSource;
            _adoOracleConnectionDestination = adoOracleConnectionDestination;
            _lastExecutedTable = lastExecutedTable;
            _rowTransferrers = rowTransferrers;
            foreach (RowTransferrer rowTransferrer in rowTransferrers)
            {
                rowTransferrer.SourceAdoOracleConnection = adoOracleConnectionSource;
                rowTransferrer.DestAdoOracleConnection = adoOracleConnectionDestination;

                // NB this seems to be a bug with Windsor setting this property erroneously 
                rowTransferrer.DestOleDbWorkspaceConnection = null;
            }
            //Added this if condition because previously it was running for only settings 
            //data but now it is running for ADMS Settings also.
            if (PROCESSNAME != null)
            {
                PROCESS_NAME = PROCESSNAME;
            }
            _storedProc = storedProc;

        }

        public void Transfer()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            try
            {
                _adoOracleConnectionSource.BeginOracleTransaction();

                // Initialize
                _executedTime = DateTime.Now;
                string sql = "EXECUTE " + _storedProc + "(" + GetFromDate() + ", " + GetToDate() + ");";
                _logger.Debug("ExecuteSQL [ " + sql + " ]");
                OracleCommand command = new OracleCommand(_storedProc, _adoOracleConnectionSource.OracleConnection);
                command.Transaction = _adoOracleConnectionSource.DbTransaction;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("I_From", OracleDbType.Date).Direction = ParameterDirection.Input;
                command.Parameters["I_From"].Value = GetFromDate();
                command.Parameters.Add("I_To", OracleDbType.Date).Direction = ParameterDirection.Input;
                command.Parameters["I_To"].Value = GetToDate();

                command.ExecuteNonQuery();

                // Let the RowTransferrers do their thing
                foreach (RowTransferrer rowTransferrer in _rowTransferrers)
                {
                    rowTransferrer.Transfer();
                }
                // Reset the last date processed field
                ResetLastDateProcessed();

                _adoOracleConnectionDestination.Commit();
            }
            catch (Exception exception)
            {
                _logger.Error(exception.ToString());
                _adoOracleConnectionDestination.Rollback();
                throw;
            }

        }

        private void ResetLastDateProcessed()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            string sql = "UPDATE " + _lastExecutedTable + " SET " + FIELD_LASTDATE + "=" + GetOracleDateString(GetToDate()) +
                " WHERE " + FIELD_PROCESS_NAME + "='" + PROCESS_NAME + "'";
            _logger.Debug("ExecuteSQL [ " + sql + " ]");
            OracleCommand updateCommand = new OracleCommand(sql, _adoOracleConnectionDestination.OracleConnection);
            updateCommand.Transaction = _adoOracleConnectionDestination.DbTransaction;
            updateCommand.ExecuteNonQuery();
        }

        private DateTime GetFromDate()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            DateTime fromDate = DateTime.MinValue;

            try
            {
                string sql = "SELECT LASTDATE FROM " + _lastExecutedTable + " WHERE " + FIELD_PROCESS_NAME + " ='" + PROCESS_NAME + "'";

                OracleCommand queryCommand = new OracleCommand(sql, _adoOracleConnectionDestination.OracleConnection);
                queryCommand.CommandType = CommandType.Text;
                queryCommand.Transaction = _adoOracleConnectionDestination.DbTransaction;
                object lastDate = queryCommand.ExecuteScalar();

                if (lastDate == null || lastDate == DBNull.Value)
                {
                    string insertSql = "INSERT INTO " + _lastExecutedTable + " (" + FIELD_LASTDATE + "," + FIELD_PROCESS_NAME +
                        ") VALUES(" +
                        GetOracleDateString(Convert.ToDateTime(BEGINNING_OF_TIME)) + ",'" + PROCESS_NAME + "')";
                    _logger.Debug("ExecuteSQL [ " + insertSql + " ]");
                    OracleCommand insertCommand = new OracleCommand(insertSql, _adoOracleConnectionDestination.OracleConnection);
                    insertCommand.CommandType = CommandType.Text;
                    insertCommand.Transaction = _adoOracleConnectionDestination.DbTransaction;
                    int rows = insertCommand.ExecuteNonQuery();
                }
                else
                {
                    fromDate = Convert.ToDateTime(lastDate);
                }
            }
            catch (Exception exception)
            {
                _logger.Warn(exception.ToString());
            }

            if (fromDate == DateTime.MinValue)
            {
                fromDate = Convert.ToDateTime(BEGINNING_OF_TIME);
            }

            return fromDate;
        }

        //private string GetFromDate2()
        //{
        //    _logger.Debug(MethodBase.GetCurrentMethod().Name);

        //    DateTime fromDate = DateTime.MinValue;
        //    ITable lastExecutedTable = null;
        //    IQueryFilter queryFilter = null;
        //    ICursor cursor = null;
        //    IRow row = null;

        //    try
        //    {
        //        // Read from the table
        //        lastExecutedTable =
        //            ((IFeatureWorkspace)_oleDbWorkspaceConnectionDestination.Workspace).OpenTable(_lastExecutedTable);
        //        queryFilter = new QueryFilterClass();
        //        queryFilter.WhereClause = FIELD_PROCESS_NAME + " ='" + PROCESS_NAME + "'";
        //        cursor = lastExecutedTable.Search(queryFilter, false);

        //        // There should be only one or zero features (first run)
        //        row = cursor.NextRow();
        //        if (row == null)
        //        {
        //            string sql = "INSERT INTO " + _lastExecutedTable + " (" + FIELD_LASTDATE + "," + FIELD_PROCESS_NAME +
        //                ") VALUES(" +
        //                GetOracleDateString(Convert.ToDateTime(BEGINNING_OF_TIME)) + ",'" + PROCESS_NAME + "')";
        //            IWorkspace workspace = _oleDbWorkspaceConnectionDestination.Workspace;
        //            _logger.Debug("ExecuteSQL [ " + sql + " ]");
        //            workspace.ExecuteSQL(sql);
        //        }
        //        else
        //        {
        //            fromDate = Convert.ToDateTime(row.get_Value(row.Fields.FindField(FIELD_LASTDATE)));
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        _logger.Warn(exception.ToString());
        //    }
        //    finally
        //    {
        //        if (row != null) Marshal.FinalReleaseComObject(row);
        //        if (cursor != null) Marshal.FinalReleaseComObject(cursor);
        //        if (lastExecutedTable != null) Marshal.FinalReleaseComObject(lastExecutedTable);
        //        if (queryFilter != null) Marshal.FinalReleaseComObject(queryFilter);
        //    }

        //    if (fromDate == DateTime.MinValue)
        //    {
        //        fromDate = Convert.ToDateTime(BEGINNING_OF_TIME);
        //    }

        //    return GetOracleDateString(fromDate);
        //}


        private DateTime GetToDate()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            return _executedTime;
        }

        private string GetOracleDateString(DateTime dateTime)
        {

            return "to_date('" + dateTime.ToString("yyyy-MM-dd HH:mm:ss") +
                               "','YYYY-MM-DD HH24:MI:SS')";
        }

        public void Dispose()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
        }
    }
}
