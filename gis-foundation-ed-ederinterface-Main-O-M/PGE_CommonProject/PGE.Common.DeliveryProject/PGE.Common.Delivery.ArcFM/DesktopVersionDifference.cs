using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.Interop;
using ESRI.ArcGIS.Geodatabase;
using log4net;
using System.Reflection;
using System.Runtime.InteropServices;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Common.Delivery.ArcFM
{
    /// <summary>
    /// Class that can be used to get version difference once and utilize it multiple times. 
    /// If the version difference should be obtained on a different version call the ResetVersionDifference method before calling GetVersionDifference method.
    /// This can only be used by Desktop components.
    /// </summary>
    public class DesktopVersionDifference
    {
        #region Private variables
        private static Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "");
        private static ID8List _versionDifference = null;
        private static IWorkspace _sourceWorkspace = null;
        private static IWorkspace _parentWorkspace = null;

        private static ID8List _insertsList = new D8ListClass();
        private static ID8List _updatesList = new D8ListClass();
        private static ID8List _deletesList = new D8ListClass();
        private static Dictionary<esriDifferenceType, ID8List> _versionDifferenceByType = new Dictionary<esriDifferenceType, ID8List>(); 
        #endregion

        #region Public Methods

        /// <summary>
        /// Given a version will get the version difference between the given version and the Default version.
        /// </summary>
        /// <param name="sourceWorkspace">Workspace that should be tested for version difference</param>
        /// <returns></returns>
        public static ID8List GetVersionDifference(IWorkspace sourceWorkspace)
        {
            if (_versionDifference == null)
            {
                _logger.Debug("Getting version difference for the first time");
                _logger.Debug("Setting up Parent and Source Versions");
                if (SetParentSource(sourceWorkspace))
                {
                    _logger.Debug("Getting version difference");
                    GetVersionDifference(_sourceWorkspace, _parentWorkspace);
                }
            }
            return _versionDifference;
        }
        /// <summary>
        /// Given a Source and Parent Wporkspace will check for Edits made in Source Workspace against teh Parent Workspace.
        /// </summary>
        /// <param name="sourceWorkspace">Workspace/Version for that should be compared to a base version</param>
        /// <param name="parentWorkspace">Version that should be used for comparing the sourceVersion</param>
        /// <returns>ID8List of Version Difference as Update-Update, Insert, Update-NoUpdate 
        /// The version Difference object persisted by this call does not contain any Deleted item in the Source version
        /// If deleted item is required call this method to get everything that has changed in the Source and call the GetVersionDifference method by passing in proper difference type.</returns>
        public static ID8List GetVersionDifference(IWorkspace sourceWorkspace, IWorkspace parentWorkspace)
        {
            if (_versionDifference == null)
            {
                _sourceWorkspace = sourceWorkspace;
                _parentWorkspace = parentWorkspace;
                _logger.Debug("Executing MMVersioningUtils.GetAllDifferencesInSource");
                IMMVersioningUtils versioningUtils = new MMVersioningUtilsClass();
                _versionDifference = versioningUtils.GetAllDifferencesInSource(_sourceWorkspace, ((IVersion)_sourceWorkspace).VersionName, ((IVersion)_parentWorkspace).VersionName, true, false);
                //Populate the Version Difference by Type
                PopulateVersionDifferenceByType((ID8ListItem)_versionDifference);
            }
            return _versionDifference;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceWorkspace"></param>
        /// <param name="differenceType"></param>
        /// <returns></returns>
        public static ID8List GetVersionDifference(IWorkspace sourceWorkspace, esriDifferenceType[] differenceType)
        {
            _logger.Debug("Getting version difference for the first time");
            _logger.Debug("Setting up Parent and Source Versions");
            ID8List d8List = new D8ListClass();
            if (SetParentSource(sourceWorkspace))
            {
                _logger.Debug("Getting version difference");
                IMMVersioningUtils versionUtils = new MMVersioningUtilsClass();
                foreach (esriDifferenceType diffType in differenceType)
                {
                    if (_versionDifferenceByType.ContainsKey(diffType))
                    {
                        d8List.Add(((ID8ListItem)_versionDifferenceByType[diffType]).Copy(true, true));
                    }
                    else
                    {
                        IMMVersionDiff versionDiff = versionUtils.GetDifferences(sourceWorkspace, ((IVersion)_sourceWorkspace).VersionName, ((IVersion)_parentWorkspace).VersionName, true, diffType);
                        if (versionDiff != null && ((ID8List)versionDiff).HasChildren)
                        {
                            d8List.Add((ID8ListItem)versionDiff);
                            //If this is the first time the version diff is called for the specified type add it to the 
                            _versionDifferenceByType.Add(diffType, (ID8List)versionDiff);
                        }
                    }
                }
            }
            return d8List;
        }
        /// <summary>
        /// Resets all the Static variable set by the GetVersionDifference
        /// </summary>
        public static void ResetVersionDifference()
        {
            _versionDifference = null;
            _sourceWorkspace = null;
            _parentWorkspace = null;
            _insertsList = null;
            _deletesList = null;
            _updatesList = null;
            foreach (KeyValuePair<esriDifferenceType, ID8List> kp in _versionDifferenceByType)
            {
                while (Marshal.ReleaseComObject(kp.Value) > 0) { }
            }
            _versionDifferenceByType = new Dictionary<esriDifferenceType, ID8List>();
        }
        /// <summary>
        /// Not Implemented
        /// </summary>
        /// <returns></returns>
        public static ID8List GetInserts()
        {
            return null;
        }
        /// <summary>
        /// Not Implemented
        /// </summary>
        /// <returns></returns>
        public static ID8List GetUpdates()
        {
            return null;
        }
        /// <summary>
        /// Not Implemented
        /// </summary>
        /// <returns></returns>
        public static ID8List GetDeletes()
        {
            return null;
        }
        #endregion

        private static bool SetParentSource(IWorkspace sourceWorkspace)
        {
            bool retVal = false;
            try
            {
                _sourceWorkspace = sourceWorkspace;
                if (((IVersion2)_sourceWorkspace).HasParent())
                {
                    _sourceWorkspace = sourceWorkspace;
                    IVersionInfo parentVersionInfo = ((IVersion2)_sourceWorkspace).VersionInfo.Parent;
                    IVersionedWorkspace versionedWorkspace = (IVersionedWorkspace)sourceWorkspace;
                    _logger.Debug("Getting Parent version:" + parentVersionInfo.VersionName);
                    _parentWorkspace = versionedWorkspace.FindVersion(parentVersionInfo.VersionName) as IWorkspace;
                    _logger.Debug("Parent workspace opened");
                    IVersion commonAncestor = ((IVersion2)_sourceWorkspace).GetCommonAncestor((IVersion)_parentWorkspace);
                    if (!string.IsNullOrEmpty(commonAncestor.VersionName))
                    {
                        _logger.Debug("Parent version is not 'DEFAULT'");
                        _parentWorkspace = (IWorkspace)commonAncestor;
                    }
                    _logger.Debug("Finished setting Parent and Source Versions");
                    retVal = true;
                }
            }
            catch
            {
                return false;
            }
            return retVal;
        }

        private static void PopulateVersionDifferenceByType(ID8ListItem d8ListItem)
        {
            if (d8ListItem is IMMVersionDiff)
            {
                IMMVersionDiff diff = (IMMVersionDiff)d8ListItem;
                if (_versionDifferenceByType.ContainsKey(diff.DifferenceType))
                {
                    _versionDifferenceByType[diff.DifferenceType] = (ID8List)d8ListItem.Copy(true, true);
                }
                else
                {
                    _versionDifferenceByType.Add(diff.DifferenceType, (ID8List)d8ListItem);
                }
            }
            else
            {
                if (((ID8List)d8ListItem).HasChildren)
                {
                    ID8List list = (ID8List)d8ListItem;
                    list.Reset();
                    ID8ListItem listItem = null;
                    while ((listItem = list.Next()) != null)
                    {
                        PopulateVersionDifferenceByType(listItem);
                    }

                }
            }
        }
    }
}
