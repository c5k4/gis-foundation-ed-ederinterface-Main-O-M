using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using System.Runtime.InteropServices;

namespace PGE.Common.Delivery.Geodatabase
{
    /// <summary>
    /// Helper class to work with Geodatabase Versions
    /// </summary>
    public class VersionFacade
    {
        //IWorkspace _Workspace = null;
        //public VersionFacade(IWorkspace workspace)
        //{
        //    _Workspace = workspace;
        //}

        #region Old Version Methods
        ///// <summary>
        ///// Given a Workspace, version name and parent Version name creates the version in the geodtabase.
        ///// </summary>
        ///// <param name="versionName">Name to be used for the new version</param>
        ///// <param name="parentVersionName">Parentversion name is the name of the version that should be used as the parent version.
        ///// If this parameter is null or empty string or could not finda version with the given name, will create the version from the version of the current workspace.</param>
        ///// <returns>IVersion</returns>
        ///// <exception cref="System.NotSupportedException">Throws NotSupportedException when the passed in workspace does not support versioning</exception>
        ///// <exception cref="System.DuplicateWaitObjectException">Throws DuplicateWaitObjectException when a version with the passed versionname already exists</exception>
        //public IVersion CreateVersion(string versionName, string parentVersionName)
        //{
        //    if (!(_Workspace is IVersionedWorkspace)) throw new NotSupportedException("The passed in workspace does not support versioning");
        //    IVersionedWorkspace versionWksp = (IVersionedWorkspace)_Workspace;
        //    if (versionWksp.FindVersion(versionName) != null) throw new DuplicateWaitObjectException("Version with the given version name:" + versionName + " already exists");
        //    IVersion baseVer = null;
        //    if (string.IsNullOrEmpty(parentVersionName)) baseVer = (IVersion)_Workspace;
        //    else
        //    {
        //        baseVer = versionWksp.FindVersion(parentVersionName);
        //        if (baseVer == null) baseVer = (IVersion)_Workspace;
        //    }
        //    return baseVer.CreateVersion(versionName);
        //}
        //public void DeleteVersion(string versionName)
        //{
        //    if (!(_Workspace is IVersionedWorkspace)) throw new NotSupportedException("The passed in workspace does not support versioning");
        //    IVersionedWorkspace versionWksp = (IVersionedWorkspace)_Workspace;
        //    if (versionWksp.FindVersion(versionName) != null) throw new DuplicateWaitObjectException("Version with the given version name:" + versionName + " already exists");
        //    IVersion2 versionToDelete = (IVersion2)versionWksp.FindVersion(versionName);
        //    IEnumLockInfo lockInfo = versionToDelete.VersionLocks;
        //    lockInfo.Reset();
        //    if (lockInfo.Next() != null) throw new AccessViolationException("Cannot delete the version. It is in use by another application or process.");
        //    else versionToDelete.Delete();
        //}

#endregion
        #region Version Methods

        /// <summary>
        /// Creates a new version in the database and returns a workspace containing the version.
        /// </summary>
        /// <param name="parentWorkspace">The workspace that creates the version.</param>
        /// <param name="name">The name of the version.</param>
        /// <param name="access">The access level of the version.</param>
        /// <param name="description">The description of the version.</param>
        /// <param name="deleteExisting">Flag to delete version if it exists</param>
        /// <returns>The workspace that contains the newly created version.</returns>
        public static IWorkspace CreateVersionAndGetWorkspace(IWorkspace parentWorkspace, string name, esriVersionAccess access, string description, bool deleteExisting)
        {
            IWorkspace workspace = null;
            IVersion childversion = null;
            IVersion parentVersion = (IVersion)parentWorkspace;
            IVersionedWorkspace parentVersionedWorkspace = (IVersionedWorkspace)parentWorkspace;

            try
            {
                // try and create the version
                parentVersion.CreateVersion(name);
                childversion = parentVersionedWorkspace.FindVersion(name);
                childversion.Access = access;
                childversion.Description = description;
                childversion.RefreshVersion();
                workspace = (IWorkspace)childversion;
            }
            catch (COMException comEx)
            {
                // if the version already exists and the delete existing version flag is 
                // set to true then go ahead and and delete the version and try creating it again.
                if (comEx.ErrorCode == (int)fdoError.FDO_E_SE_VERSION_EXISTS)
                {
                    if (deleteExisting)
                    {
                        IVersion tempVersion = parentVersionedWorkspace.FindVersion(name);
                        tempVersion.Delete();
                        return CreateVersionAndGetWorkspace(parentWorkspace, name, access, description, false);
                    }
                    else
                    {
                        return workspace;
                    }
                }
                else
                {
                    throw;
                }
            }

            return workspace;
        }

        /// <summary>
        /// Creates a new version in the database.
        /// </summary>
        /// <param name="parentWorkspace">The workspace that creates the version.</param>
        /// <param name="name">The name of the version.</param>
        /// <param name="access">The access level of the version.</param>
        /// <param name="description">The description of the version.</param>
        /// <param name="deleteExisting">Flag to delete version if it exists</param>
        /// <returns><c>true</c> if the version was created and <c>false</c> if it was not.</returns>
        public static bool CreateVersion(IWorkspace parentWorkspace, string name, esriVersionAccess access, string description, bool deleteExisting)
        {
            IVersion childversion = null;
            IVersion parentVersion = (IVersion)parentWorkspace;
            IVersionedWorkspace parentVersionedWorkspace = (IVersionedWorkspace)parentWorkspace;

            try
            {
                // try and create the version
                parentVersion.CreateVersion(name);
                childversion = parentVersionedWorkspace.FindVersion(name);
                childversion.Access = access;
                childversion.Description = description;
                childversion.RefreshVersion();
            }
            catch (COMException comEx)
            {
                // if the version already exists and the delete existing version flag is 
                // set to true then go ahead and and delete the version and try creating it again.
                if (comEx.ErrorCode == (int)fdoError.FDO_E_SE_VERSION_EXISTS)
                {
                    if (deleteExisting)
                    {
                        DeleteVersion(parentWorkspace, name);
                        return CreateVersion(parentWorkspace, name, access, description, false);
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    throw;
                }
            }

            return true;
        }

        /// <summary>
        /// Deletes a version from the database.
        /// </summary>
        /// <param name="parentWorkspace">The workspace connection to the database.</param>
        /// <param name="name">The name of the version to delete.</param>
        public static void DeleteVersion(IWorkspace parentWorkspace, string name)
        {
            IVersion version = null;

            try
            {
                // get the version and delete it
                IVersionedWorkspace versionedWorkspace = (IVersionedWorkspace)parentWorkspace;
                version = versionedWorkspace.FindVersion(name);
                version.Delete();
            }
            catch (COMException comEx)
            {
                // if the version is already deleted then ignore.
                if (comEx.ErrorCode != (int)fdoError.FDO_E_OBJECT_IS_DELETED)
                    throw;
            }
        }

        /// <summary>
        /// Reconciles and posts the version back to the target version.
        /// </summary>
        /// <param name="version">The version to be reconciled and posted.</param>
        /// <param name="acquireLock">Whether to aquire a lock or not.</param>
        /// <param name="abortIfConflicts">Whether to abort if conflicts exist.</param>
        /// <param name="childWins">Whether conflicts are resolved in favor of the source version.</param>
        /// <param name="columnLevel">Whether to reconcile at the column level.</param>
        /// <remarks>
        /// The target version is the parent version.
        /// </remarks>
        public static void ReconcileAndPost(IVersion version, bool acquireLock, bool abortIfConflicts, bool childWins, bool columnLevel)
        {
            // setup the versioning variables to prepare for the reconcile and post
            IWorkspaceEdit workspaceEdit = (IWorkspaceEdit2)version;
            IVersionEdit4 versionEdit = (IVersionEdit4)workspaceEdit;

            //reconcile against the default version    
            if (version.HasParent() != false)
            {
                workspaceEdit.StartEditing(false);
                workspaceEdit.StartEditOperation();
                Boolean conflictsDetected = versionEdit.Reconcile4(version.VersionInfo.Parent.VersionName, acquireLock, abortIfConflicts, childWins, columnLevel);

                //no conflicts detected so post can be performed-        
                if (conflictsDetected != true && versionEdit.CanPost())
                {
                    versionEdit.Post(version.VersionInfo.Parent.VersionName);
                }

                // Commit the edit operation and edit session.
                workspaceEdit.StopEditOperation();
                workspaceEdit.StopEditing(true);
            }
        }

        /// <summary>
        /// Checks to see if there are any locks currently associated with the version.
        /// </summary>
        /// <param name="version">The version to check for locks.</param>
        /// <returns><c>true</c> if there are locks and <c>false</c> if there are no locks.</returns>
        public static bool CheckForVersionLocks(IVersion version)
        {
            //checking the locks on the version    
            IEnumLockInfo enumLockInfo = version.VersionLocks;
            ILockInfo lockinfo = enumLockInfo.Next();
            if (lockinfo != null)
                return true;

            return false;
        }

        /// <summary>
        /// Gets the version based on the name.
        /// </summary>
        /// <param name="workspace">The connection to the database that contains the version.</param>
        /// <param name="name">The name of the version to get.</param>
        /// <returns>The version.</returns>
        /// <remarks>The name must include the owner i.e. to get the DEFAULT version you would pass is SDE.DEFAULT.</remarks>
        public static IVersion GetVersion(IWorkspace workspace, string name)
        {
            IVersion version = null;
            IVersionedWorkspace versionedWorkspace = (IVersionedWorkspace)workspace;
            version = versionedWorkspace.FindVersion(name);

            return version;
        }

        #endregion
    }
}
