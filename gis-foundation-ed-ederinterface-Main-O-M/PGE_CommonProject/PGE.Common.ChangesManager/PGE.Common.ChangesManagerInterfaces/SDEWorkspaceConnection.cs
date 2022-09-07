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
using PGE_DBPasswordManagement;

namespace PGE.Common.ChangesManagerShared
{
    public class SDEWorkspaceConnection
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "");

        private IWorkspace _workspace = null;
        private IWorkspace _tempWorkspace = null;

        public string WorkspaceConnectionFile { get; private set; }

        public string NonVersionedEditsVersionName { get; set; }

        public IWorkspace Workspace
        {
            get
            {
                _logger.Debug(MethodBase.GetCurrentMethod().Name);

                if (_workspace == null)
                {
                    _workspace = AOHelper.ArcSDEWorkspaceFromPath(WorkspaceConnectionFile);
                }
                return _workspace;
            }
        }

        public IWorkspace NonVersionedEditsWorkspace
        {
            get
            {
                if (_tempWorkspace == null)
                {
                    _tempWorkspace = GetNonVersionedEditsWorkspace();
                }
                return _tempWorkspace;
            }
        }


        private IWorkspace GetNonVersionedEditsWorkspace()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            if (!(Workspace is IVersionedWorkspace)) return null;

            IVersion childVersion = null;

            try
            {
                childVersion = ((IVersionedWorkspace)Workspace).FindVersion(NonVersionedEditsVersionName);
            }
            catch (COMException ex)
            {
                if (ex.ErrorCode != -2147215997)
                {
                    _logger.Error(ex.ToString());

                    throw ex;
                }
            }

            if (childVersion == null)
            {
                _logger.Info("Creating NonVersionedEditsVersion [ " + NonVersionedEditsVersionName + " ]");
                childVersion = ((IVersion)Workspace).CreateVersion(NonVersionedEditsVersionName);
                childVersion.Access = esriVersionAccess.esriVersionAccessPublic;
            }
            else
            {
                _logger.Info("Found NonVersionedEditsVersion [ " + NonVersionedEditsVersionName + " ]");
            }

            return (IWorkspace)childVersion;
        }

        public SDEWorkspaceConnection(string workspaceConnectionFile)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name + " [ " + workspaceConnectionFile + " ]");

            // m4jf edgisrearch 919
            // WorkspaceConnectionFile = workspaceConnectionFile;
            // for external databases
            if (workspaceConnectionFile.ToLower().EndsWith(".sde"))
            {
                WorkspaceConnectionFile = workspaceConnectionFile;
            }
            else
            {
                WorkspaceConnectionFile = ReadEncryption.GetSDEPath(workspaceConnectionFile);
            }
           
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

        public void DeleteTempVersion()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name + " [ " + NonVersionedEditsVersionName +" ]");

            if (_tempWorkspace != null && !String.IsNullOrEmpty(NonVersionedEditsVersionName))
            {
                // This is an extreme code smell, calling GC.Collect and then, the ultimate whiff, Thread.Sleep
                // I've checked and re-checked all references have been FinalReleaseComObject'd
                // Remove either and you'll get an Exception from ArcObjects claiming that
                // the Version is In Use
                // Furthermore, it's not a fatal error if the NonVersionedEditsVersionName cannot be deleted -- it will be cleaned up when the program is next started
                Marshal.FinalReleaseComObject(_tempWorkspace);
                _tempWorkspace = null;
                GC.Collect();
                IVersion tempVersion = ((IVersionedWorkspace)Workspace).FindVersion(NonVersionedEditsVersionName);
                _logger.Info("Deleting TempVersion [ " + NonVersionedEditsVersionName + " ]");
                GC.Collect();
                Thread.Sleep(50);
                try
                {
                    tempVersion.Delete();
                }
                catch (Exception ex)
                {
                    _logger.Info("Non fatal error -- cannot delete version [ " + NonVersionedEditsVersionName + " ] " + ex.ToString());
                }
                Marshal.FinalReleaseComObject(tempVersion);
            }
            
        }

    }
}
