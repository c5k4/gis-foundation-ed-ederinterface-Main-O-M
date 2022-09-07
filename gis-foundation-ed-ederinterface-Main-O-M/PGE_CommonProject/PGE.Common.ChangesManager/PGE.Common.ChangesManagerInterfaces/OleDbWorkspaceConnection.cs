using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.ChangesManagerShared.Utilities;
using PGE.Common.ChangesManagerShared;
using System.Reflection;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Common.ChangesManagerShared
{
    public class OleDbWorkspaceConnection
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "");

        private IWorkspace _workspace = null;
        private IWorkspace _tempWorkspace = null;

        public string WorkspaceConnectionFile { get; private set; }

        public IWorkspace Workspace
        {
            get
            {

                if (_workspace == null)
                {
                    _workspace = AOHelper.OleDbWorkspaceFromPath(WorkspaceConnectionFile);
                }
                return _workspace;
            }
        }

        public OleDbWorkspaceConnection(string workspaceConnectionFile)
        {
            //_logger.Debug(MethodBase.GetCurrentMethod().Name + " [ " + workspaceConnectionFile + " ]");

            WorkspaceConnectionFile = workspaceConnectionFile;
        }
        public void KillConnection()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            _workspace = null;
            _tempWorkspace = null;
        }
        public void Disconnect()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            if (_workspace != null)     Marshal.FinalReleaseComObject(_workspace);
            _workspace = null;
            if (_tempWorkspace != null) Marshal.FinalReleaseComObject(_tempWorkspace);
            _tempWorkspace = null;
        }

    }
}
