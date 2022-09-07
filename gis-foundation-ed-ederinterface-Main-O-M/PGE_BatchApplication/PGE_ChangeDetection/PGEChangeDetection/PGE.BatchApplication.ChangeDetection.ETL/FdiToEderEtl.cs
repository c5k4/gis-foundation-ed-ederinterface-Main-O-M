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
    public class FdiToEderEtl : IExtractTransformLoad
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "ChangeDetectionDefault.log4net.config");

        private AdoOracleConnection _adoOracleConnectionDestination;

        private IList<RowTransferrer> _rowTransferrers;

        private string _fdiIbalTable;
        private string _fdiIbalView;


        public FdiToEderEtl()
        {
            
        }

        public FdiToEderEtl(SDEWorkspaceConnection sdeWorkspaceConnectionSource, 
                            SDEWorkspaceConnection sdeWorkspaceConnectionDestination,
                            AdoOracleConnection adoOracleConnectionDestination,
                            string fdiIbalTable,
                            string fdiIbalView,
                            IList<RowTransferrer> rowTransferrers)
        {
            _adoOracleConnectionDestination = adoOracleConnectionDestination;
            _fdiIbalTable = fdiIbalTable;
            _fdiIbalView = fdiIbalView;
            _rowTransferrers = rowTransferrers;
            foreach (RowTransferrer rowTransferrer in rowTransferrers)
            {
                rowTransferrer.SourceSdeWorkspaceConnection = sdeWorkspaceConnectionSource;
                rowTransferrer.DestSdeWorkspaceConnection = sdeWorkspaceConnectionDestination;

                // NB this seems to be a bug with Windsor setting this property erroneously 
                rowTransferrer.SourceAdoOracleConnection = null;
                rowTransferrer.DestAdoOracleConnection = null;
            }

        }

        public void Transfer()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            try
            {

                // Let the RowTransferrers do their thing, no need to be in a transaction as this is only a staging table scenario
                foreach (RowTransferrer rowTransferrer in _rowTransferrers)
                {
                    rowTransferrer.Transfer();
                }

                _adoOracleConnectionDestination.BeginOracleTransaction();

                // Initialize
                string sql = "DELETE FROM " + _fdiIbalTable;
                _logger.Debug("ExecuteSQL [ " + sql + " ]");
                OracleCommand command = new OracleCommand(sql, _adoOracleConnectionDestination.OracleConnection);
                command.Transaction = _adoOracleConnectionDestination.DbTransaction;
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();

                string schemaName = _fdiIbalTable.Substring(0, _fdiIbalTable.LastIndexOf("."));
                string fdiIbalTableName = _fdiIbalTable.Substring(_fdiIbalTable.LastIndexOf(".") + 1);
                sql = "select registration_id from sde.table_registry where table_name='" + fdiIbalTableName + "'";
                command.CommandText = sql;
                _logger.Debug("ExecuteScalar [ " + sql + " ]");
                int registrationId = Convert.ToInt32(command.ExecuteScalar());
                _logger.Debug("RegistrationId [ " + registrationId + " ]");


                sql = "INSERT INTO " + _fdiIbalTable + "(OBJECTID,GUID,MAXASYMSUM,MAXASYMWIN,MAXSYMSUM,MAXSYMWIN,LINETOLINESUM,LINETOLINEWIN,LINETOLINETOGROUNDSUM,LINETOLINETOGROUNDWIN,LINETOGROUNDSUM,LINETOGROUNDWIN,PEAKLOADSUM,PEAKLOADWIN) SELECT SDE.VERSION_USER_DDL.NEXT_ROW_ID('" + schemaName + "'," + registrationId + "),GUID,MAXASYMSUM,MAXASYMWIN,MAXSYMSUM,MAXSYMWIN,LINETOLINESUM,LINETOLINEWIN,LINETOLINETOGROUNDSUM,LINETOLINETOGROUNDWIN,LINETOGROUNDSUM,LINETOGROUNDWIN,PEAKLOADSUM,PEAKLOADWIN FROM " + _fdiIbalView;
                _logger.Debug("ExecuteSQL [ " + sql + " ]");
                command.CommandText = sql;
                command.ExecuteNonQuery();

                _adoOracleConnectionDestination.Commit();
                command.Dispose();
            }
            catch (Exception exception)
            {
                _logger.Error(exception.ToString()); 
                _adoOracleConnectionDestination.Rollback();
                throw;
            }

        }



        public void Dispose()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

        }
    }
}
