using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using Telvent.Delivery.Framework;
using Telvent.PGE.ED;
using System.Runtime.InteropServices;

namespace Telvent.PGE.MapProduction.PreProcessing
{
    /// <summary>
    /// Implementation of IMPChangeDetector for PG&E
    /// </summary>
    public class PGEMPChangeDetector : IMPChangeDetector
    {
        #region private members
        /// <summary>
        /// Change Detection table name
        /// </summary>
        private const string _changeDetectionTableMN = "PGE_MODIFIEDMAPS";
        /// <summary>
        /// Field Name storing the modified MapNumber:MapOffice information
        /// </summary>
        private const string _valueField = "MODIFIEDMAPS_VALUE";
        /// <summary>
        /// Instance of the Change Detection table
        /// </summary>
        private IObjectClass _changedetectionTable = null;
        /// <summary>
        /// Database instance in use
        /// </summary>
        private IWorkspace _workspace = null;
        /// <summary>
        /// GeoDatabaseManager instance
        /// </summary>
        private IMPGDBDataManager _gdbDataManager = null;
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor gets the Workspace to use for look ing at the Change detection table
        /// </summary>
        /// <param name="workspace"></param>
        public PGEMPChangeDetector(IWorkspace workspace, IMPGDBDataManager gdbDataManager)
        {
            _workspace = workspace;
            _gdbDataManager = gdbDataManager;
        }

        #endregion Constructor

        #region Overridden Properties

        /// <summary>
        /// ModelName to use to get the ChangeDetection table from the given workspace 
        /// </summary>
        protected virtual string ChangeDetectionTableModelName
        {
            get { return _changeDetectionTableMN; }
        }

        /// <summary>
        /// ChangeDetection table got using teh ModelName from the given workspace
        /// </summary>
        protected virtual IObjectClass ChangeDetectionTable
        {
            get
            {
                if (_changedetectionTable == null && !string.IsNullOrEmpty(ChangeDetectionTableModelName))
                {
                    _changedetectionTable = ModelNameFacade.ObjectClassByModelName(_workspace, ChangeDetectionTableModelName);
                }
                return _changedetectionTable;
            }
        }

        #endregion Overridden Properties

        #region Public and Protected Methods

        /// <summary>
        /// Get a list of Unique keys from the ChangeDetection table.
        /// It is expected that the map change detection process will store a combination of MapNumber:MapOffice field values.
        /// At PGE the map numbers are not unique across the organization. The combination of MapOffice and MapNumber makes it Unique
        /// For PGE the value in the ChangeDetection table will be of the format MapNumber:MapOffice,MapNumber1:MapOffice...
        /// </summary>
        /// <returns></returns>
        public virtual List<string> GetChangedKey()
        {
            List<string> retVal = new List<string>();
            ICursor cursor = null;
            try
            {
                if (_workspace != null && ChangeDetectionTable != null)
                {
                    cursor = ((ITable)ChangeDetectionTable).Search(null, true);
                    IRow row = null;
                    while ((row = cursor.NextRow()) != null)
                    {
                        retVal.AddRange(GetValueFromRow(row));
                    }
                    if (cursor != null)
                    {
                        while (Marshal.ReleaseComObject(cursor) > 0) { }
                        cursor = null;
                    }

                    if (retVal.Count > 0)
                    {
                        retVal = retVal.Distinct().ToList();
                        //This is not required any more as the Desktop Change Detection is now modified to store the MapNumber:MapOffice combination instead of MapNumber:LocalOffice value
                        //retVal = GetChangedKeysAfterMapping(retVal);
                    }
                }
            }
            finally
            {
                if (cursor != null)
                {
                    while (Marshal.ReleaseComObject(cursor) > 0) { }
                }
            }
            return retVal;
        }

        /// <summary>
        /// Deletes all records from the ChangeDetection table
        /// </summary>
        /// <returns></returns>
        public virtual bool ClearAllChanges()
        {
            bool retVal = true;
            IWorkspaceEdit wse = (IWorkspaceEdit)_workspace;
            try
            {
                wse.StartEditing(true);
                wse.StartEditOperation();
                ((ITable)ChangeDetectionTable).DeleteSearchedRows(null);
                wse.StopEditOperation();
                wse.StopEditing(true);
            }
            finally
            {
                if (wse.IsBeingEdited())
                {
                    if (((IWorkspaceEdit2)wse).IsInEditOperation)
                    {
                        wse.StopEditOperation();
                    }
                    wse.StopEditing(false);
                    retVal = false;
                }
            }
            return retVal;
        }

        /// <summary>
        /// Adds a given record to Change Detection table
        /// </summary>
        /// <param name="gdbData"></param>
        /// <returns></returns>
        public virtual bool MarkAsChanged(IMPGDBData gdbData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Given a IRow from the ChangeDetection table will get the Value from the Value column in the ChangeDetectionTable
        /// At PGE The value in the ChangeDetection table will be of the format MapNumber:MapOffice,MapNumber1:MapOffice...
        /// </summary>
        /// <param name="row">IRow object obtained fro mthe ChangeDetection table.</param>
        /// <returns></returns>
        protected virtual List<string> GetValueFromRow(IRow row)
        {
            List<string> retVal = new List<string>();
            if (row != null)
            {
                string value = ((IObject)row).GetFieldValue(_valueField).Convert<string>(string.Empty);
                if (!string.IsNullOrEmpty(value))
                {
                    retVal = new List<string>(value.Split(",".ToCharArray()));
                }
            }
            return retVal;
        }

        protected virtual List<string> GetChangedKeysAfterMapping(List<string> changedKeys)
        {
            List<string> retVal = new List<string>();
            IFeatureCursor cursor = null;
            if (_gdbDataManager == null) return changedKeys;
            //Get the Map Polygon feature class
            IFeatureClass mapPolygonFeatureClass = _gdbDataManager.MapPolygonFeatureClass;
            IFeature mapFeature;
            string mapNumberFieldName = ModelNameFacade.FieldNameFromModelName(mapPolygonFeatureClass, PGEGDBFieldLookUp.PGEGDBFields.MapNumberMN);
            string localOfficeFieldName = ModelNameFacade.FieldNameFromModelName(mapPolygonFeatureClass, PGEGDBFieldLookUp.PGEGDBFields.LocalOfficeMN);
            string whereClause = mapNumberFieldName + "='{0}' AND " + localOfficeFieldName + "='{1}'";

            string mapOfficeFiedName = ModelNameFacade.FieldNameFromModelName(mapPolygonFeatureClass, PGEGDBFieldLookUp.PGEGDBFields.MapOfficeMN);
            int mapOfficeFiedIndex = ModelNameFacade.FieldIndexFromModelName(mapPolygonFeatureClass, PGEGDBFieldLookUp.PGEGDBFields.MapOfficeMN);
            string mapNumber, localOffice;

            try
            {
                //Set the filter criteria
                IQueryFilter filter = new QueryFilterClass();
                //filter.SubFields = mapOfficeFiedName;

                //Loop through each changed key
                foreach (string key in changedKeys)
                {
                    mapNumber = key.Split(":".ToArray())[0];
                    localOffice = key.Split(":".ToArray())[1];
                    filter.WhereClause = string.Format(whereClause, mapNumber, localOffice);

                    //Fire query on feature class
                    cursor = mapPolygonFeatureClass.Search(filter, true);
                    mapFeature = cursor.NextFeature();
                    if (mapFeature != null)
                    {
                        if (mapFeature.get_Value(mapOfficeFiedIndex) != null)
                        {
                            retVal.Add(mapNumber + ":" + mapFeature.get_Value(mapOfficeFiedIndex).ToString());
                        }
                        Marshal.ReleaseComObject(mapFeature);
                    }
                    //Release the cursor
                    Marshal.ReleaseComObject(cursor);

                }

            }
            finally
            {
                if (cursor != null)
                {
                    while (Marshal.ReleaseComObject(cursor) > 0) { }
                }
            }
            return retVal.Count > 0 ? retVal.Distinct().ToList() : retVal;
        }

        #endregion Public and Protected Methods

        #region Protected Properties

        protected IMPGDBDataManager GDBDataManager
        {
            get { return _gdbDataManager; }           
        }

        #endregion
    }
}
