using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using ESRI.ArcGIS.Geodatabase;
using System.Reflection;
using PGE_DBPasswordManagement;
//using Telvent.Delivery.Diagnostics;

namespace PGE.BatchApplication.CWOSL
{
    public class SDEWorkspaceConnection
    {
        private static readonly Log4NetLoggerCWOSL _logger = new Log4NetLoggerCWOSL(MethodBase.GetCurrentMethod().DeclaringType, "CWOSLDefault.log4net.config");

        private IWorkspace _workspace = null;
        private IWorkspace _workspaceLandbase = null;
        public string WorkspaceConnectionFile { get; private set; }
        public string WorkspaceConnectionFileLandbase { get; private set; }

        public IWorkspace Workspace
        {
            get
            {
                _logger.Debug(MethodBase.GetCurrentMethod().Name);

                if (_workspace == null)
                {
                    _workspace = ArcSDEWorkspaceFromPath(WorkspaceConnectionFile);
                }
                return _workspace;
            }
        }
        public IWorkspace WorkspaceLandbase
        {
            get
            {
                _logger.Debug(MethodBase.GetCurrentMethod().Name);

                if (_workspaceLandbase == null)
                {
                    _workspaceLandbase = ArcSDEWorkspaceFromPath(WorkspaceConnectionFileLandbase);
                }
                return _workspaceLandbase;
            }
        }

        public static IWorkspace ArcSDEWorkspaceFromPath(string sFile)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name + " [ " + sFile + " ]");

            IWorkspace workspace = null;

            try
            {
                Type t = Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory");
                IWorkspaceFactory workspaceFactory = (IWorkspaceFactory)Activator.CreateInstance(t);
                workspace = workspaceFactory.OpenFromFile(sFile, 0);
                LogSdeConnectionProperties(workspace);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
                throw new ApplicationException("Workspace Can't be Opened at [ " + sFile + " ]");
            }

            return workspace;

        }

        public static void LogSdeConnectionProperties(IWorkspace sdeWorkspace)
        {
            object names = null, values = null;
            sdeWorkspace.ConnectionProperties.GetAllProperties(out names, out values);
            IDictionary<string, object> properties = new Dictionary<string, object>();
            string[] nameArray = (string[])names;
            object[] valueArray = (object[])values;
            for (int i = 0; i < nameArray.Length; i++)
            {
                properties.Add(nameArray[i], valueArray[i]);
            }
            string user = properties["USER"].ToString();
            string instance = properties["INSTANCE"].ToString();

            string version = (properties.ContainsKey("VERSION")) ? properties["VERSION"].ToString() : "NO VERSION";
            _logger.Debug(String.Format("ArcSDE [ {0} {1} {2} ]", instance, user, version));

        }

        public SDEWorkspaceConnection(string workspaceConnectionFile, string workspaceConnectionFileLandbase)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name + " [ " + workspaceConnectionFile + " ]" +
                                                               " [ " + workspaceConnectionFileLandbase + " ]");
            //m4jf edgisrearch 919
            //WorkspaceConnectionFile = workspaceConnectionFile;
            //WorkspaceConnectionFileLandbase = workspaceConnectionFileLandbase;
            WorkspaceConnectionFile = ReadEncryption.GetSDEPath(workspaceConnectionFile);
            WorkspaceConnectionFileLandbase = ReadEncryption.GetSDEPath(workspaceConnectionFileLandbase);
        }

        public void KillConnection()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            _workspace = null;
            _workspaceLandbase = null;
        }
        public void Disconnect()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            if (_workspace != null) Marshal.FinalReleaseComObject(_workspace);
            _workspace = null;

            if (_workspaceLandbase != null) Marshal.FinalReleaseComObject(_workspaceLandbase);
            _workspaceLandbase = null;
        }

    }

}
