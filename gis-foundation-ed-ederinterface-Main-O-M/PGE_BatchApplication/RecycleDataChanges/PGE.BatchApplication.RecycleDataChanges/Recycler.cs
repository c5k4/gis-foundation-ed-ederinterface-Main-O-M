using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using PGE.BatchApplication.RecycleDataChanges.Utilities;
using System.Diagnostics;
using PGE.Common.ChangeManager;

namespace PGE.BatchApplication.RecycleDataChanges
{
    public class Recycler
    {
        private IWorkspace _ws;

        private IVersion2 _versSource = null;
        private IVersion2 _versTarget = null;
        private IVersion2 _versLoad = null;

        public Recycler(IWorkspace workspace)
        {
            _ws = workspace;

            FindVersions();
        }

        /// <summary>
        /// Runs the logic to export the version differences to a file and load them into a target version.
        /// </summary>
        public void RecycleDifferences()
        {
            //Use the utility classes to export the version differences.
            Program.WriteStatus("");
            ChangeXmlConverter changeExporter = new ChangeXmlConverter(_versTarget, _versSource, _versLoad, ToolSettings.Instance.XmlFileLocation);
            if (ToolSettings.Instance.ExportFileEnabled)
            {
                Program.WriteStatus("Exporting differences...");
                changeExporter.ExportDifferences();
            }
            if (ToolSettings.Instance.ImportFileEnabled)
            {
                Program.WriteStatus("Importing differences...");
                changeExporter.ImportDifferences();
            }

            Program.WriteStatus("Changes recycled.");
        }

        #region Private Methods

        /// <summary>
        /// Use the class-level variables to load the versions into the class.
        /// </summary>
        private void FindVersions()
        {
            IVersionedWorkspace vWorkspace = (IVersionedWorkspace)_ws;

            Program.WriteStatus("Preparing versions:");
            try
            {
                //Find required versions
                if (ToolSettings.Instance.ExportFileEnabled)
                {
                    Program.WriteStatus("  Finding source version (" + ToolSettings.Instance.VersionSourceName + ")...");
                    _versSource = (IVersion2)vWorkspace.FindVersion(ToolSettings.Instance.VersionSourceName);
                    Program.WriteStatus("  Finding target version (" + ToolSettings.Instance.VersionTargetName + ")...");
                    _versTarget = (IVersion2)vWorkspace.FindVersion(ToolSettings.Instance.VersionTargetName);
                }
                if (ToolSettings.Instance.ImportFileEnabled)
                {
                    Program.WriteStatus("  Finding \"load to\" version (" + ToolSettings.Instance.VersionLoadName + ")...");
                    _versLoad = (IVersion2)vWorkspace.FindVersion(ToolSettings.Instance.VersionLoadName);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Issue finding versions: " + ex.Message, ex);
            }
            Program.WriteStatus("  Versions found.");

            if (ToolSettings.Instance.ExportFileEnabled)
            {
                //Validate the versions to make sure that the parent/child relationship is correct.
                if (!VersionIsChildOf(_versSource, _versTarget) && !VersionIsChildOf(_versTarget, _versSource))
                    throw new Exception("The versions configured to run did not have a proper parent/child relationship.");
            }
        }

        /// <summary>
        /// Determines whether or not the version is a child of the specified potential parent version.
        /// </summary>
        /// <param name="version">The version to compare with a potential parent version.</param>
        /// <param name="potentialParent">The potential parent version.</param>
        /// <returns><c>true</c> if the version is a child of the parent, otherwise <c>false</c>.</returns>
        private bool VersionIsChildOf(IVersion version, IVersion potentialParent)
        {
            if (!version.HasParent())
                return false;

            return version.VersionInfo.Parent.VersionName == potentialParent.VersionName;
        }

        #endregion
    }
}
