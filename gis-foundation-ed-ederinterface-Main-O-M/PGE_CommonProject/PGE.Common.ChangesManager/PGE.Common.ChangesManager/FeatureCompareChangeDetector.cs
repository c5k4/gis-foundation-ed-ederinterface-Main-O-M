using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.GeoDatabaseDistributed;
using System.Reflection;
using PGE.Common.ChangesManagerShared.Interfaces;
using PGE.Common.ChangesManagerShared;
using PGE.Common.Delivery.Diagnostics;
//V3SF - CD API (EDGISREARC-1452) - Added
using PGE.Common.ChangeDetectionAPI;

namespace PGE.Common.ChangesManager
{
    public class FeatureCompareChangeDetector : IChangeDetector
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "");

        private IFeatureClass _baseFC = null;
        private IFeatureClass _editsFC = null;
        private IExtractGDBManager _extractGdbManager;

        public bool DoNotRollExtractGDBs
        { get; set; }

        /// <summary>
        /// Dictionary mapping a list of features that have been deleted in the source version to the corresponding
        /// table name.
        /// </summary>
        private ChangeDictionary _deletes;
        public ChangeDictionary Deletes
        {
            get
            {
                return _deletes;
            }
            
        }

        /// <summary>
        /// Dictionary mapping a list of features that have been inserted in the source version to the corresponding
        /// table name.
        /// </summary>
        private ChangeDictionary _inserts;
        public ChangeDictionary Inserts
        {
            get
            {
                return _inserts;
            }
        }

        /// <summary>
        /// Dictionary mapping a list of features that have been updated in the source version to the corresponding
        /// table name.
        /// </summary>
        private ChangeDictionary _updates;
        public ChangeDictionary Updates
        {
            get
            {
                return _updates;
            }
        }

        public IFeatureClass BaseFC
        {
            get
            {
                if (_baseFC == null)
                {
                    _baseFC = _extractGdbManager.OpenBaseFeatureClass(); ;
                }
                return _baseFC;
            }
        }

        public IFeatureClass EditsFC
        {
            get
            {
                if (_editsFC == null)
                {
                    _editsFC = _extractGdbManager.OpenEditsFeatureClass();
                }
                return _editsFC;
            }
        }

        public FeatureCompareChangeDetector(IExtractGDBManager extractGdbManager)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            try
            {
                _extractGdbManager = extractGdbManager;
            }
            catch (Exception ex)
            {
                _logger.Debug(ex.ToString());
                throw ex;
            }
        }

        public void Read()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            _extractGdbManager.ExportToEditsGDB();

            LoadDifferences();

        }


        /// <summary>
        /// V3SF - CD API (EDGISREARC-1452) - Added [START]
        /// Added to maintain Structure in all Detectors
        /// </summary>
        /// <param name="toDate"></param>
        public void ReadCD(DateTime toDate, DateTime fromDate)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            _extractGdbManager.ExportToEditsGDB();

            LoadDifferences();

        }


        /// <summary>
        /// V3SF - CD API (EDGISREARC-1452) - Added [START]
        /// Added to maintain Structure in all Detectors
        /// </summary>
        /// <param name="toDate"></param>
        public void ReadCDBatch(DateTime toDate, DateTime fromDate,string TableName,string BatchID)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            _extractGdbManager.ExportToEditsGDB();

            LoadDifferences();

        }

        public void Cleanup()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            if (!DoNotRollExtractGDBs)
            {
                RollExtractGDBs();
            }
            _extractGdbManager.Dispose();
            _extractGdbManager = null;
        }

        /// <summary>
        /// Intended to be run after all processing is complete. Rolls the versions forward by deleting the child version
        /// and renaming the temporary version to that of the child version.
        /// </summary>
        private void RollExtractGDBs()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            _logger.Info("Rolling FGDB [ " + _extractGdbManager.EditsLocation + " ] over [ " + _extractGdbManager.BaseLocation + " ]");

            if (_inserts != null) _inserts.KillConnections();
            if (_updates != null) _updates.KillConnections();
            if (_deletes != null) _deletes.KillConnections();
            _extractGdbManager.KillConnections();
            if (_baseFC != null) Marshal.FinalReleaseComObject(_baseFC);
            if (_editsFC != null) Marshal.FinalReleaseComObject(_editsFC);
            _baseFC = null;
            _editsFC = null;
            _inserts.Clear();
            _inserts = null;
            _updates.Clear();
            _updates = null;
            _deletes.Clear();
            _deletes = null;
            // No way around this, sadly, or the FGDB remains locked
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Thread.Sleep(5000);
            _extractGdbManager.RolloverGDBs();
        }

        /// <summary>
        /// Loads the changes into the Inserts, Updates and Deletes properties using the specified types.
        /// </summary>
        /// <param name="differenceTypes">The difference types.</param>
        /// <returns>
        /// 	<c>true</c> if the changes are loaded; otherwise <c>false</c>.
        /// </returns>       
        public bool LoadDifferences(params esriDataChangeType[] differenceTypes)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            // Initialize the dictionaries.
            _inserts = new ChangeDictionary(_extractGdbManager.EditsLocation, esriDataChangeType.esriDataChangeTypeInsert);
            _updates = new ChangeDictionary(_extractGdbManager.EditsLocation, _extractGdbManager.BaseLocation, esriDataChangeType.esriDataChangeTypeUpdate);
            _deletes = new ChangeDictionary(_extractGdbManager.BaseLocation, esriDataChangeType.esriDataChangeTypeDelete);

            GetInsertsAndUpdates();
            GetDeletes();

            return true;
        }


        public void GetInsertsAndUpdates()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            IQueryFilter qf = null;
            IFeatureCursor editsRowsCursor = null;
            IFeature editsFeature = null;
            IFeature baseFeature = null;

            try
            {
                bool isFC = EditsFC is IFeatureClass;
                ChangeTable changeTableInserts = new ChangeTable(String.Copy(((IDataset)EditsFC).Name), isFC);
                ChangeTable changeTableUpdates = new ChangeTable(String.Copy(((IDataset)EditsFC).Name), isFC);
                this.Updates.Add(((IDataset)EditsFC).Name, changeTableUpdates);
                qf = new QueryFilterClass();
                qf.SubFields = "*";
                editsRowsCursor = _editsFC.Search(qf, false);
                while ((editsFeature = editsRowsCursor.NextFeature()) != null)
                {
                    try
                    {
                        baseFeature = BaseFC.GetFeature(editsFeature.OID);

                        if (!FeaturesAreEqual(baseFeature, editsFeature))
                        {
                            if (this.Updates.Count == 0)
                            {
                                this.Updates.Add(String.Copy(((IDataset)EditsFC).Name), changeTableUpdates);
                            }
                            changeTableUpdates.Add(new ChangeRow(editsFeature.OID, esriDataChangeType.esriDataChangeTypeUpdate));
                            _logger.Debug("Adding Edit [ " + editsFeature.OID.ToString() + " ]");
                        }
                    }
                    catch (COMException ex)
                    {
                        if (ex.ErrorCode == (int)fdoError.FDO_E_FEATURE_NOT_FOUND)
                        {
                            if (this.Inserts.Count == 0)
                            {
                                this.Inserts.Add(((IDataset)EditsFC).Name, changeTableInserts);
                            }
                            changeTableInserts.Add(new ChangeRow(editsFeature.OID, esriDataChangeType.esriDataChangeTypeInsert));
                            _logger.Debug("Adding Insert [ " + editsFeature.OID.ToString() + " ]");
                        }
                        else
                        {
                            _logger.Error("Error Getting Feature [ " + editsFeature.OID.ToString() + " ]", ex);

                            throw ex;
                        }
                    }
                }
            }
            finally
            {
                if (qf != null) Marshal.FinalReleaseComObject(qf);
                if (editsRowsCursor != null) Marshal.FinalReleaseComObject(editsRowsCursor);
                if (baseFeature != null) Marshal.FinalReleaseComObject(baseFeature);
                if (editsFeature != null) Marshal.FinalReleaseComObject(editsFeature);
            }
        }

        public bool FeaturesAreEqual(IRow baseRow, IRow editsRow)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name + " OID [ " + editsRow.OID.ToString() + " ]");

            bool rowsAreEquivalent = true;

            for (int i = 0; i < baseRow.Fields.FieldCount; i++)
            {
                object baseRowFieldValue = baseRow.get_Value(i);
                object editsRowFieldValue = editsRow.get_Value(i);

                if (((baseRowFieldValue == null || baseRowFieldValue is DBNull) &&
                    (editsRowFieldValue != null && !(editsRowFieldValue is DBNull))) ||
                    ((editsRowFieldValue == null || editsRowFieldValue is DBNull) &&
                    (baseRowFieldValue != null && !(baseRowFieldValue is DBNull))))
                {
                    return false;
                }

                if ((baseRowFieldValue == null || baseRowFieldValue is DBNull) &&
                    (editsRowFieldValue == null || editsRowFieldValue is DBNull))
                {
                    continue;
                }

                switch (baseRow.Fields.get_Field(i).Type)
                {
                    // Not going to compare the uncomparable
                    case esriFieldType.esriFieldTypeBlob:
                    case esriFieldType.esriFieldTypeGlobalID:
                    case esriFieldType.esriFieldTypeGUID:
                    case esriFieldType.esriFieldTypeOID:
                    case esriFieldType.esriFieldTypeRaster:
                    case esriFieldType.esriFieldTypeXML:
                        break;
                    case esriFieldType.esriFieldTypeDate:
                        if (Convert.ToDateTime(baseRowFieldValue) != Convert.ToDateTime(editsRowFieldValue))
                            return false;
                        break;
                    case esriFieldType.esriFieldTypeDouble:
                        if (Convert.ToDouble(baseRowFieldValue) != Convert.ToDouble(editsRowFieldValue))
                            return false;
                        break;
                    case esriFieldType.esriFieldTypeGeometry:
                        if (!((IRelationalOperator)baseRowFieldValue).Equals((IGeometry)editsRowFieldValue))
                            return false;
                        break;
                    case esriFieldType.esriFieldTypeInteger:
                        if (Convert.ToInt32(baseRowFieldValue) != Convert.ToInt32(editsRowFieldValue))
                            return false;
                        break;
                    case esriFieldType.esriFieldTypeSingle:
                        if (Convert.ToSingle(baseRowFieldValue) != Convert.ToSingle(editsRowFieldValue))
                            return false;
                        break;
                    case esriFieldType.esriFieldTypeSmallInteger:
                        if (Convert.ToInt32(baseRowFieldValue) != Convert.ToInt32(editsRowFieldValue))
                            return false;
                        break;
                    case esriFieldType.esriFieldTypeString:
                        if (Convert.ToString(baseRowFieldValue) != Convert.ToString(editsRowFieldValue))
                            return false;
                        break;
                    default:
                        break;
                }
            }

            return rowsAreEquivalent;
        }

        public void GetDeletes()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            IFeatureCursor baseRowsCursor = null;
            IQueryFilter qf = null;
            IFeature baseRow = null;
            IFeature editFeature = null;

            try
            {
                bool isFC = BaseFC is IFeatureClass;
                ChangeTable changeTableDeletes = new ChangeTable(String.Copy(((IDataset)BaseFC).Name), isFC);
                qf = new QueryFilterClass();
                qf.SubFields = "OBJECTID";
                baseRowsCursor = BaseFC.Search(qf, false);
                while ((baseRow = baseRowsCursor.NextFeature()) != null)
                {
                    try
                    {
                        editFeature = EditsFC.GetFeature(baseRow.OID);
                    }
                    catch (COMException ex)
                    {
                        if (ex.ErrorCode == (int)fdoError.FDO_E_FEATURE_NOT_FOUND)
                        {
                            if (this.Deletes.Count == 0)
                            {
                                this.Deletes.Add(String.Copy(((IDataset)BaseFC).Name), changeTableDeletes);
                            }
                            changeTableDeletes.Add(new ChangeRow(baseRow.OID, esriDataChangeType.esriDataChangeTypeDelete));
                            _logger.Debug("Added Delete [ " + baseRow.OID.ToString() + " ]");
                        }
                    }
                }
            }
            finally
            {
                if (qf != null) Marshal.FinalReleaseComObject(qf);
                if (baseRowsCursor != null) Marshal.FinalReleaseComObject(baseRowsCursor);
                if (baseRow != null) Marshal.FinalReleaseComObject(baseRow);
                if (editFeature != null) Marshal.FinalReleaseComObject(editFeature);
            }
        }

    }
}
