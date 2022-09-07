using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.Interop;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Geodatabase.ChangeDetection;
using ESRI.ArcGIS.GeoDatabaseDistributed;
using System.Runtime.InteropServices;
using PGE.Common.Delivery.Framework;
using log4net;
using System.Reflection;

namespace PGE.Common.Delivery.Geodatabase
{
    /// <summary>
    /// Class ot use for obtaining the VersionDifference between 2 versions when a hook to IApplication cannot be obtained.
    /// </summary>
    public class VersionDifferenceProcessor
    {
        #region Private variables
        private static ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static ID8List _versionDifference=null;
        private static IWorkspace _sourceWorkspace = null; 
        private static IWorkspace _parentWorkspace=null;

        private static ID8List _insertsList = new D8ListClass();
        private static ID8List _updatesList = new D8ListClass();
        private static ID8List _deletesList = new D8ListClass();
        #endregion

        #region Public Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceWorkspace"></param>
        /// <returns></returns>
        public static ID8List GetVersionDifference(bool loadFeederManager20Updates, IWorkspace sourceWorkspace)
        {
            if (_versionDifference == null)
            {
                if (((IVersion2)_sourceWorkspace).HasParent())
                {
                    _sourceWorkspace = sourceWorkspace;
                    IVersionInfo parentVersionInfo = ((IVersion)_sourceWorkspace).VersionInfo.Parent;
                    IVersionedWorkspace versionedWorkspace = (IVersionedWorkspace)sourceWorkspace;
                    _parentWorkspace = versionedWorkspace.FindVersion(parentVersionInfo.VersionName) as IWorkspace;
                    GetVersionDifference(loadFeederManager20Updates, _sourceWorkspace, _parentWorkspace);
                }
           }
            return _versionDifference;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceWorkspace"></param>
        /// <param name="parentWorkspace"></param>
        /// <returns></returns>
        public static ID8List GetVersionDifference(bool loadFeederManager20Updates, IWorkspace sourceWorkspace, IWorkspace parentWorkspace)
        {
            if (_versionDifference == null)
            {
                ProcessVersionDifference(loadFeederManager20Updates);
            }
            return _versionDifference;
        }
        /// <summary>
        /// 
        /// </summary>
        public static void ResetVersionDifference()
        {
            _versionDifference= null;
            _sourceWorkspace = null;
            _parentWorkspace = null;
            _insertsList = null;
            _deletesList = null; 
            _updatesList = null;
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


        #region Private Members
        /// <summary>
        /// Gets the Version Difference for each DataChangeType Insert,Update and Delete
        /// </summary>
        private static void ProcessVersionDifference(bool loadFeederManager20Updates)
        {
            //Create Difference Manager
            DifferenceManager diffManager = new DifferenceManager((IVersion)_sourceWorkspace, (IVersion)_parentWorkspace);
            //Initialize Difference Manager
            diffManager.Initialize();
            if (diffManager.LoadDifferences(loadFeederManager20Updates, esriDataChangeType.esriDataChangeTypeInsert, esriDataChangeType.esriDataChangeTypeUpdate, esriDataChangeType.esriDataChangeTypeDelete))
            {
                //Create D8List for each of the Diff dictionary
                foreach (string modifiedClass in diffManager.ModifiedClassNames)
                {
                    ID8List tableList = GetTableD8List(modifiedClass);
                    ID8List insertsList = GetD8List(diffManager.Inserts[modifiedClass],esriDataChangeType.esriDataChangeTypeInsert);
                    //AddToDifferenceType(insertsList, esriDataChangeType.esriDataChangeTypeInsert, modifiedClass);
                    ID8List updatesList = GetD8List(diffManager.Updates[modifiedClass], esriDataChangeType.esriDataChangeTypeUpdate);
                    //AddToDifferenceType(updatesList, esriDataChangeType.esriDataChangeTypeUpdate, modifiedClass);
                    ID8List deletesList = GetD8List(diffManager.Deletes[modifiedClass], esriDataChangeType.esriDataChangeTypeDelete);
                    //AddToDifferenceType(deletesList, esriDataChangeType.esriDataChangeTypeDelete, modifiedClass);
                    tableList.Add((ID8ListItem)insertsList);
                    tableList.Add((ID8ListItem)updatesList);
                    tableList.Add((ID8ListItem)deletesList);
                    _versionDifference.Add((ID8ListItem)tableList); 
                }
            }
            
            
        }

        private static ID8List GetD8List(DiffTable difftable,esriDataChangeType changeType)
        {
            ID8List returnList = new VersionDiffD8List();
            ((IVersionDiff)returnList).DataChangeType = changeType;
            IWorkspace workspacetoUse= changeType==esriDataChangeType.esriDataChangeTypeDelete ? _parentWorkspace : _sourceWorkspace;
            IRow row = null;
            try
            {
                foreach (DiffRow diffRow in difftable)
                {
                    row = difftable.GetIRow(workspacetoUse, diffRow);
                    ID8Feature listtoAdd = new D8FeatureClass();
                    ((ID8GeoAssoc)listtoAdd).AssociatedGeoRow = row;
                    returnList.Add((ID8ListItem)listtoAdd);
                }
            }
            finally
            {
                if (row != null)
                {
                    while (Marshal.ReleaseComObject(row) > 0) { } 
                }
            }
            return (ID8List)returnList;
        }

        private static bool AddToDifferenceType(ID8List d8List,esriDataChangeType changeType,string tableName)
        {
            ID8List tableList = GetTableD8List(tableName);
            tableList.Add(((ID8ListItem)d8List).Copy(true, true));

            if (changeType == esriDataChangeType.esriDataChangeTypeInsert)
            {
                _insertsList.Add((ID8ListItem)tableList); 
            }
            if (changeType == esriDataChangeType.esriDataChangeTypeUpdate)
            {
                _updatesList.Add((ID8ListItem)tableList);
            }
            if (changeType == esriDataChangeType.esriDataChangeTypeDelete)
            {
                _deletesList.Add((ID8ListItem)tableList);
            }

            return true;
        }
        private static ID8List GetTableD8List(string tableName)
        {
            ID8Table d8Table = new D8TableClass();
            d8Table.Name = tableName;
            return (ID8List)d8Table;
        }
        #endregion
    }
}
