using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;

namespace PGE.BatchApplication.AUConveyor.Processing
{
    /// <summary>
    /// Contains logic for managing GIS edit sessions.
    /// </summary>
    internal class GISEditor
    {
        private IVersion _version;
        private IWorkspaceEdit _editWorkspace;

        internal GISEditor(IWorkspaceEdit editWorkspace)
        {
            _editWorkspace = editWorkspace;
            _version = (IVersion)_editWorkspace;
        }

        internal void StartEditing()
        {
            _editWorkspace.StartEditing(false);
            _editWorkspace.StartEditOperation();
        }

        internal void StopEditing(bool saveEdits)
        {
            _editWorkspace.StopEditOperation();
            _editWorkspace.StopEditing(saveEdits);
        }

        internal void RestartEditing(bool saveEdits)
        {
            StopEditing(saveEdits);
            StartEditing();
        }

        internal void AbortEditing()
        {
            try { _editWorkspace.AbortEditOperation(); }
            catch { }
            try { _editWorkspace.StopEditing(false); }
            catch { }
        }

        internal bool Reconcile()
        {
            try
            {
                IVersionEdit4 versionEdit4 = _version as IVersionEdit4;
                bool conflictsFound = versionEdit4.Reconcile4(_version.VersionInfo.Parent.VersionName, false, false, false, false);
                return !conflictsFound;
            }
            catch (Exception)
            {
                return false;
            }
        }

        internal bool Post()
        {
            try
            {
                IVersionEdit4 versionEdit = _version as IVersionEdit4;
                if (versionEdit.CanPost())
                {
                    versionEdit.Post(_version.VersionInfo.Parent.VersionName);
                    return true;
                }
            }
            catch (Exception) { }

            return false;
        }
    }
}
