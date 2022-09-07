using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.ChangesManagerShared;
using PGE.Common.ChangesManagerShared.Interfaces;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.BatchApplication.ChangeDetection.ETL
{
    public class TlmToEderEtl : IExtractTransformLoad
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "ChangeDetectionDefault.log4net.config");
        
        private SDEWorkspaceConnection _sdeWorkspaceConnectionDestination;
        private RowTransferrer _rowTransferrer;

        private string _storedProc;

        public TlmToEderEtl(OleDbWorkspaceConnection oleDbWorkspaceConnectionSource, 
            SDEWorkspaceConnection sdeWorkspaceConnectionDestination, string storedProc, 
            RowTransferrer rowTransferrer)
        {
            _sdeWorkspaceConnectionDestination = sdeWorkspaceConnectionDestination;
            _rowTransferrer = rowTransferrer;
            _rowTransferrer.SourceOleDbWorkspaceConnection = oleDbWorkspaceConnectionSource;
            _rowTransferrer.DestSdeWorkspaceConnection = sdeWorkspaceConnectionDestination;
            // This should not be necessary but for some reason the Workspace is being copied to Source & Dest in ioc
            _rowTransferrer.DestOleDbWorkspaceConnection = null;
            _rowTransferrer.SourceSdeWorkspaceConnection = null;
            _rowTransferrer.UseGPTool = false;
            _storedProc = storedProc;
        }

        public void Transfer()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            try
            {
                // Let the RowTransferrer do its thing
                _rowTransferrer.Transfer();
                // Reset the last date processed field
                UpdateBaseAndATables();
            }
            catch (Exception exception)
            {
                _logger.Error(exception.ToString());                
                throw;
            }
        }

        private void UpdateBaseAndATables()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            string sql = "BEGIN " + _storedProc + "(); END;";
            _logger.Debug(sql);
            IWorkspace workspace = _sdeWorkspaceConnectionDestination.Workspace;
            _logger.Debug("ExecuteSQL [ " + sql + " ]");
            workspace.ExecuteSQL(sql);
        }

        public void Dispose()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
        }
    }
}
