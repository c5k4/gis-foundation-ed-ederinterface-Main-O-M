using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.ChangesManagerShared;
using PGE.Common.ChangesManagerShared.Utilities;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.BatchApplication.ChangeDetection.ETL
{
    public class TablesTransferrer
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "ChangeDetectionDefault.log4net.config");

        private SDEWorkspaceConnection _sourceSdeWorkspaceConnection;
        private SDEWorkspaceConnection _destSdeWorkspaceConnection;

        public AdoOracleConnection SourceAdoOracleConnection { get; set; }
        public AdoOracleConnection DestAdoOracleConnection { get; set; }

        private IList<RowTransferrer> _rowTransferrers;

        public bool DeleteSourceRowsAfterTransfer { get; set; }

        public TablesTransferrer(SDEWorkspaceConnection sourceSdeWorkspaceConnection,
            SDEWorkspaceConnection destSdeWorkspaceConnection, IList<RowTransferrer> rowTransferrers)
        {
            _sourceSdeWorkspaceConnection = sourceSdeWorkspaceConnection;
            _destSdeWorkspaceConnection = destSdeWorkspaceConnection;
            _rowTransferrers = rowTransferrers;
        }
        public TablesTransferrer(IList<RowTransferrer> rowTransferrers)
        {
            _rowTransferrers = rowTransferrers;
        }

        private void TransferLogStart()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            if (_sourceSdeWorkspaceConnection != null)
            {
                _logger.Info("Source [ " + _sourceSdeWorkspaceConnection.WorkspaceConnectionFile + " ]");
                _logger.Info("Dest [ " + _destSdeWorkspaceConnection.WorkspaceConnectionFile + " ]");

                AOHelper.EnsureInEditSession(_sourceSdeWorkspaceConnection.NonVersionedEditsWorkspace);
                AOHelper.EnsureInEditSession(_destSdeWorkspaceConnection.NonVersionedEditsWorkspace);
            }
            else
            {
                _logger.Info("Source [ " + SourceAdoOracleConnection.ConnectionStringEncrypted + " ]");
                _logger.Info("Dest [ " + DestAdoOracleConnection.ConnectionStringEncrypted + " ]");

                DestAdoOracleConnection.BeginOracleTransaction();
            }
            
        }

        //There is no distributed transaction management. If the second one fails we could leave the database
        // in an inconsistent state. This will one day be done in Oracle distributed transaction, hopefully
        public void TransferTables()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            TransferLogStart();

            foreach (RowTransferrer rowTransferrer in _rowTransferrers)
            {
                if (_sourceSdeWorkspaceConnection != null)
                {
                    rowTransferrer.SourceSdeWorkspaceConnection = _sourceSdeWorkspaceConnection;
                    rowTransferrer.DestSdeWorkspaceConnection = _destSdeWorkspaceConnection;
                }
                else
                {
                    rowTransferrer.SourceAdoOracleConnection = SourceAdoOracleConnection;
                    rowTransferrer.DestAdoOracleConnection = DestAdoOracleConnection;
                }
                rowTransferrer.DeleteSourceRowsAfterTransfer = DeleteSourceRowsAfterTransfer;

                rowTransferrer.Transfer();
                rowTransferrer.Dispose();
            }

            _rowTransferrers.Clear();
            _rowTransferrers = null;
            TransferLogEnd();
        }

        private void TransferLogEnd()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            if (_sourceSdeWorkspaceConnection != null)
            {
                AOHelper.EnsureCloseEditSession(_sourceSdeWorkspaceConnection.NonVersionedEditsWorkspace);
                AOHelper.EnsureCloseEditSession(_destSdeWorkspaceConnection.NonVersionedEditsWorkspace);

                _destSdeWorkspaceConnection.DeleteTempVersion();
                _sourceSdeWorkspaceConnection.DeleteTempVersion();
            }
            else
            {
                DestAdoOracleConnection.Commit();
            }
        }

        public void Dispose()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

//            foreach (RowTransferrer rowTransferrer in _rowTransferrers)
//            {
////                rowTransferrer.Dispose();
//            }
            _rowTransferrers = null;
        }
    }
}
