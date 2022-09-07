using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Framework;
using Miner.Interop;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Miner.ComCategories;

namespace PGE.Desktop.EDER.Subtasks
{
    /// <summary>
    /// Persist the changes to MapNumber polygon
    /// </summary>
    [ComVisible(true)]
    [Guid("E5F3CB6D-360B-45DC-A397-BC2AF6751341")]
    [ProgId("PGE.Desktop.EDER.RecordEditedMap")]
    [ComponentCategory(ComCategory.MMPxSubtasks)]
    public class RecordEditedMap : BaseChangeDetectionSubtask
    {
        #region Private Variables
        /// <summary>
        /// Stores the Map Production changes
        /// </summary>
        private IChange _change = null;
        #endregion Private Variables

        #region Constructor
        /// <summary>
        /// Initialises the new instance of <see cref="RecordDMSChanges"/> class
        /// </summary>
        public RecordEditedMap() : base("PGE Record Modified Maps")
        {
        }
        #endregion Constructor

        #region Protected Properties
        /// <summary>
        /// Change Detector to use to process changes
        /// </summary>
        protected override IChange Changedetector
        {
            get
            {
                if (_change == null) _change = new MapProductionChange();
                return _change;
            }
        }
        #endregion Protected Properties
    }

    /// <summary>
    /// Provides methods to read the changes and persist to the the CHANGE tables
    /// </summary>
    public class MapProductionChange : IChange
    {
        #region Private Variables and Constants

        /// <summary>
        /// Logs the messages
        /// </summary>
        private static Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        /// <summary>
        /// Refer to the database instance
        /// </summary>
        private IWorkspace _workspace = null;
        /// <summary>
        /// Holds a reference to the changes table
        /// </summary>
        private ITable _changeTable = null;
        /// <summary>
        /// Holds the collection of changes in Feeders and Substations
        /// </summary>
        List<string> _changes = new List<string>();
        /// <summary>
        /// Version Name field in Changes table
        /// </summary>
        private const string _versionNameField = "Version";
        /// <summary>
        /// Value Name field in Changes table
        /// </summary>
        private const string _fieldValueField = "Value";
        /// <summary>
        /// Modified Date field in Changes table
        /// </summary>
        private const string _modefiedDateField = "ModifiedDate";
        /// <summary>
        /// Table Namr storing the changes
        /// </summary>
        private const string _tableModelName = "PGE_MODIFIEDMAPS";
        /// <summary>
        /// Default Map number
        /// </summary>
        private const string NOMAPVALUE = "NOMAP";

        #endregion Private Variables and Constants

        #region Public Properties

        /// <summary>
        /// Workspace to be used for querying and persisting change data.
        /// </summary>
        public ESRI.ArcGIS.Geodatabase.IWorkspace Workspace
        {
            private get { return _workspace; }
            set
            {
                _workspace = value;
                _changeTable = null;//Reset the value to null, so that table is retrieved from new database instance
            }
        }

        /// <summary>
        /// Will process featureonly if this is set to true. That is in VersionDifference a Related object is found will not process those.
        /// For Map Production we need to change only those that are physically shown in the map so this returns true
        /// </summary>
        public bool FeatureOnly
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the table with modelname "PGE_MODIFIEDMAPS" from the workspace that is set.
        /// </summary>
        public ITable ChangeTable
        {
            get
            {
                if (_changeTable == null)
                {
                    _changeTable = ModelNameFacade.ObjectClassByModelName(_workspace, _tableModelName) as ITable;
                }
                return _changeTable;
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Adds the changed MapNumber:LocalOffice to the in-memory list by querying the Irow associated to the ID8List
        /// </summary>
        /// <param name="d8List">ID8List that was edited to record the changes</param>
        public void AddChange(ID8List d8List)
        {
            string mapNumber = string.Empty;
            string localOffice = string.Empty;
            ID8GeoAssoc geoAssoc = (ID8GeoAssoc)d8List;
            IRow row = geoAssoc.AssociatedGeoRow;
            if (FeatureOnly && !(row is IFeature))
            {
                return;
            }
            //For the Sake of performance save the MapNumber and LocalOffice combination which is unique and this should be translated to MapNumber:MapOffice in the Map Production interface.
            //This extends the time taken by the Interface but it is better than wasting user time in ArcMap

            //The Component Specification asks for "PGE_DISTRIBUTIONMAPNO" modelname and not "PGE_DISTRIBUTIONMAPNUMBER" so changing the code - Ravi 02-25-2013
            //int mapNumberFldIdx = ModelNameFacade.FieldIndexFromModelName(((IObject)row).Class, "PGE_DISTRIBUTIONMAPNUMBER");
            int mapNumberFldIdx = ModelNameFacade.FieldIndexFromModelName(((IObject)row).Class, "PGE_DISTRIBUTIONMAPNO");
            int localOfficeFldIdx = ModelNameFacade.FieldIndexFromModelName(((IObject)row).Class, "PGE_LOCALOFFICE");
            if (mapNumberFldIdx > -1 && localOfficeFldIdx > -1)
            {
                mapNumber = row.get_Value(mapNumberFldIdx).Convert(string.Empty);
                localOffice = row.get_Value(localOfficeFldIdx).Convert(string.Empty);
                if (string.IsNullOrEmpty(mapNumber) || mapNumber.ToUpper().Equals(NOMAPVALUE) || string.IsNullOrEmpty(localOffice))
                {
                    return;
                }
                if (!_changes.Contains(mapNumber + ":" + localOffice))
                {
                    _changes.Add(mapNumber + ":" + localOffice);
                }
            }
        }

        /// <summary>
        /// Adds a record to the Table with modelname "PGE_MODIFIEDMAPS" and persists he following information
        /// Version, a comma seperated string value of MAPNUMBER:MAPOFFICE
        /// If the Record for the version already exists then the changes are persisted to the same row.
        /// </summary>
        /// <returns></returns>
        public bool PersistChange()
        {
            bool retVal = true;
            if (ChangeTable != null)
            {
                //If there are no changes don't worry about changing anything - Ravi (02-25-2013)
                if (_changes.Count == 0)
                {
                    _logger.Debug("No map changes found for version:"+((IVersion)Workspace).VersionName); 
                    return true;
                }
                IRow rowToPersist = null;
                try
                {
                    //rowToPersist = ChangeTable.CreateRow();
                    rowToPersist = GetChangeDetectionRow(((IVersion)Workspace).VersionName);
                    _logger.Debug(string.Format("Created a new row with OID:{0} on table :{1}", rowToPersist.OID, ((IDataset)ChangeTable).Name));
                    _logger.Debug("setting version Name");
                    rowToPersist.set_Value(rowToPersist.Fields.FindField(_versionNameField), ((IVersion)Workspace).VersionName);
                    _logger.Debug("Setting field Value");
                    int valueFldIx = rowToPersist.Fields.FindField(_fieldValueField);
                    object fieldValue = rowToPersist.get_Value(valueFldIx);
                    string existingValue = (fieldValue == null||fieldValue==System.DBNull.Value) ? string.Empty : fieldValue.ToString();
                    if (existingValue != string.Empty) existingValue = existingValue + ",";
                    rowToPersist.set_Value(valueFldIx, existingValue + string.Join(",", _changes.ToArray()));
                    _logger.Debug("Setting date Value");
                    rowToPersist.set_Value(rowToPersist.Fields.FindField(_modefiedDateField), DateTime.Today.ToShortDateString());
                    rowToPersist.Store();
                    //Clear the changes
                    _changes.Clear();
                }
                catch (Exception ex)
                {
                    retVal = false;
                    _logger.Error("Failed persisting change to table with modelname: " + _tableModelName, ex);
                }
                finally
                {
                    if (rowToPersist != null)
                    {
                        while (Marshal.ReleaseComObject(rowToPersist) > 0) { }
                    }
                }
            }
            else
            {
                retVal = false;
                _logger.Error("Error getting change detecion table with modelname :" + _tableModelName);
            }
            return retVal;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Queries the changeDetection table using Version as the parameter if a row is found that will be returned and if a row is not found will create a new row for the given version and that is passed on to the caller.
        /// </summary>
        /// <param name="versionName">Name of the version</param>
        /// <returns>A New row if the Version does not exist in the Change Table else the table that has the given version name</returns>
        private IRow GetChangeDetectionRow(string versionName)
        {
            IRow retVal = null;
            IQueryFilter qf = new QueryFilterClass();
            qf.WhereClause = _versionNameField + "='" + versionName + "'";
            ICursor cursor = ChangeTable.Search(qf, false);
            retVal = cursor.NextRow();
            if (retVal == null)
            {
                retVal = ChangeTable.CreateRow();
            }
            //release the cursor
            while (Marshal.ReleaseComObject(cursor) > 0) { };
            return retVal;
        }

        #endregion Private Methods
    }
}
