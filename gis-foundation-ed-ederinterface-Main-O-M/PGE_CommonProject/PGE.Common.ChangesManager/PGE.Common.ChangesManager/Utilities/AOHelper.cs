using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using PGE.Diagnostics;
using System.Reflection;

namespace PGE.ChangesManager.Utilities
{
    internal class AOHelper
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        //TODO: remove
        private const string _VERSION_TEMP_NAME = "Daily_Change_Temp";
        
        /// <summary>
        /// Opens an SDE connection file from a file name and loads it into an IWorkspace.
        /// </summary>
        /// <param name="sFile">The file path of the desired SDE connection file.</param>
        /// <returns>An IWorkspace object from the connection.</returns>
        public static ESRI.ArcGIS.Geodatabase.IWorkspace ArcSDEWorkspaceFromPath(string sFile)
        {
            IWorkspaceFactory workspaceFactory = new ESRI.ArcGIS.DataSourcesGDB.SdeWorkspaceFactory();
            return workspaceFactory.OpenFromFile(sFile, 0);
        }

        /// <summary>
        /// Intended to be run after all processing is complete. Rolls the versions forward by deleting the child version
        /// and renaming the temporary version to that of the child version.
        /// </summary>
        static public void RollVersions(IWorkspace workspaceChangeDetection, string versionDailyChangeName)
        {
            try
            {
                IVersionedWorkspace vWorkspace = (IVersionedWorkspace)workspaceChangeDetection;
                IVersion2 versDailyChange = (IVersion2)vWorkspace.FindVersion(versionDailyChangeName);
                _logger.Debug("Rolling Versions:");

                //Delete daily change version.
                string versionName = versDailyChange.VersionName;
                if (versionName.LastIndexOf('.') >= 0) versionName = versionName.Substring(versionName.LastIndexOf('.') + 1);
                versDailyChange.Delete();
                versDailyChange = null;
                _logger.Debug("  Deleted version (" + versionName + ").");

                //Rename the temporary version.
                IVersion2 versDailyChangeTemp = (IVersion2)vWorkspace.FindVersion(_VERSION_TEMP_NAME);
                versDailyChangeTemp.VersionName = versionName;
                _logger.Debug("  Renamed version " + _VERSION_TEMP_NAME + " to " + versionName + ".");
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Issue rolling versions forward: " + ex.Message, ex);
//                throw new ErrorCodeException(ErrorCode.VersionReset, "Issue rolling versions forward: " + ex.Message, ex);
            }
        }

        public static IWorkspace FileGdbWorkspaceFromPath(String path)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name + " for path [ " + path + " ]");

            Type factoryType = Type.GetTypeFromProgID(
                "esriDataSourcesGDB.FileGDBWorkspaceFactory");
            IWorkspaceFactory workspaceFactory = (IWorkspaceFactory)Activator.CreateInstance
                (factoryType);
            return workspaceFactory.OpenFromFile(path, 0);
        }

        /// <summary>
        /// Determines whether or not the version is a child of the specified potential parent version.
        /// </summary>
        /// <param name="version">The version to compare with a potential parent version.</param>
        /// <param name="potentialParent">The potential parent version.</param>
        /// <returns><c>true</c> if the version is a child of the parent, otherwise <c>false</c>.</returns>
        public static bool VersionIsChildOf(IVersion version, IVersion potentialParent)
        {
            if (!version.HasParent())
                return false;

            return version.VersionInfo.Parent.VersionName == potentialParent.VersionName;
        }

    }
}
