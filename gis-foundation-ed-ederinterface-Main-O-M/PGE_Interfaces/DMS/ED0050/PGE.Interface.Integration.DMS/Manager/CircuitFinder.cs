using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGE.Interface.Integration.DMS.Common;
using ESRI.ArcGIS.Geodatabase;
using log4net;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesGDB;
using System.Runtime.InteropServices;

namespace PGE.Interface.Integration.DMS.Manager
{
    /// <summary>
    /// This class is used to determine what circuits to export either by processing the change tables or the CircuitSource table.
    /// It creates the list of circuits to export.
    /// </summary>
    public class CircuitFinder
    {
        /// <summary>
        /// Holds reference to the ITable instance
        /// </summary>
        protected ITable _table = null;
        /// <summary>
        /// Logger to log error / debug/ user information
        /// </summary>
        protected log4net.ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private List<long> _changedCircuitOIDs = null;
        private List<long> _changedSubstationOIDs = null;

        private IWorkspace _EDWorkSpace = null;
        private IWorkspace _SUBWorkSpace = null;
        /// <summary>
        /// Initialize the CircuitFinder
        /// </summary>
        /// <param name="workSpace">The workspace that contains the change tables and CircuitSource table</param>
        public CircuitFinder(IWorkspace EDWorkSpace, IWorkspace SUBWorkSpace)
        {
            _EDWorkSpace = EDWorkSpace;
            _SUBWorkSpace = SUBWorkSpace;
            //if (_workSpace is IVersion)
            //{
            //    _logger.Debug("Using version " + ((IVersion)_workSpace).VersionName);
            //}
        }
        /// <summary>
        /// Create a list of circuits based on the type of request
        /// </summary>
        /// <param name="msg">The message that has the type of request, Bulk or Changes</param>
        /// <returns>The list of circuits</returns>
        public List<String> FindCircuits(ExtractorMessage msg, ref int stitchPointsToProcess)
        {

            // #9
            if (msg.ExtractType == extracttype.Bulk)
            {
                return LoadCache(_EDWorkSpace, "EDGIS.CircuitSource", "CircuitID", "EDGIS.ELECTRICSTITCHPOINT", ref stitchPointsToProcess);
            }
            else
            {
                List<String> validStitchPoints = LoadCache(_EDWorkSpace, "EDGIS.CircuitSource", "CircuitID", "EDGIS.ELECTRICSTITCHPOINT", ref stitchPointsToProcess);
                return LoadCache(_EDWorkSpace, "EDGIS.PGE_CHANGED_CIRCUIT", "CircuitID", out _changedCircuitOIDs,
                    ref stitchPointsToProcess, validStitchPoints);
            }
            //return msg.IncludeCircuits;
        }

        //Used to be able to track how many circuits are associated with a particular substation.
        Dictionary<string, int> CircuitCountMapping = new Dictionary<string, int>();

        /// <summary>
        /// Create a list of substations based on the type of request
        /// </summary>
        /// <param name="msg">The message that has the type of request, Bulk or Changes</param>
        /// <returns>The list of substations</returns>
        public List<String> FindSubstations(ExtractorMessage msg, ref int stitchPointsToProcess)
        {
            // #6

            if (msg.ExtractType == extracttype.Bulk)
            {
                return LoadCache(_SUBWorkSpace, "EDGIS.CircuitSource", "SUBSTATIONID", "EDGIS.SUBELECTRICSTITCHPOINT", ref stitchPointsToProcess);
            }
            else
            {
                CircuitCountMapping.Clear();
                List<String> validSubstations = LoadCache(_SUBWorkSpace, "EDGIS.CircuitSource", "SUBSTATIONID", "EDGIS.SUBELECTRICSTITCHPOINT", ref stitchPointsToProcess);
                return LoadCache(_EDWorkSpace, "EDGIS.PGE_CHANGED_SUBSTATION", "SUBSTATIONID", out _changedSubstationOIDs, 
                    ref stitchPointsToProcess, validSubstations);
            }
            
        }

        /// <summary>
        /// Opens the table from the Database
        /// </summary>
        /// <param name="wSpace">Reference to the database</param>
        /// <param name="tableName">Name of the table of FeatureClass</param>
        /// <returns>Returns the reference to Table/FeatureClass. Returns Null if Table / Feature Class does not exist.</returns>
        protected ITable GetTable(IWorkspace wSpace, string tableName)
        {
            if (wSpace == null) return null;
            if (string.IsNullOrEmpty(tableName)) return null;
            IWorkspace2 wSpace2 = wSpace as IWorkspace2;
            ITable table = null;
            IFeatureWorkspace featureWSpace = wSpace as IFeatureWorkspace;
            //Checks whether table exists with the given name
            if (wSpace2.get_NameExists(esriDatasetType.esriDTTable, tableName))
            {
                table = featureWSpace.OpenTable(tableName);
            }
            //Checks whether FeatureClass exists with the given name
            else if (wSpace2.get_NameExists(esriDatasetType.esriDTFeatureClass, tableName))
            {
                table = featureWSpace.OpenFeatureClass(tableName) as ITable;
            }
            if (table == null)
            {
                _logger.Debug(tableName + " does not exist in " + wSpace.PathName);
            }
            return table;
        }




        /// <summary>
        /// Return the list of values for the defined fieldname of the given table
        /// </summary>
        private List<string> LoadCache(IWorkspace wsForLookup, string tableName, string fieldName, string stitchPointTableName, ref int stitchPointsToProcess)
        {
            // #7
            stitchPointsToProcess = 0;
            List<string> stitchPointGuids = new List<string>();
            ITable stitchPointTable = GetTable(wsForLookup, stitchPointTableName);
            IQueryFilter qf = new QueryFilterClass();
            ICursor stitchPointsCursor = stitchPointTable.Search(qf, true);
            int stitchGUIDIndex = stitchPointTable.Fields.FindField("GLOBALID");
            IRow stitchRow = null;
            while ((stitchRow = stitchPointsCursor.NextRow()) != null)
            {
                string value = stitchRow.get_Value(stitchGUIDIndex).ToString();
                if (!string.IsNullOrEmpty(value))
                {
                    stitchPointGuids.Add(value);
                }
            }

            List<string> IDs = new List<string>();
            _table = GetTable(wsForLookup, tableName);
            IQueryFilter filter = new QueryFilterClass();
            filter.WhereClause = fieldName + " is not null";
            int count = _table.RowCount(filter);
            ICursor cursor = _table.Search(filter, true);
            int IDIndex = _table.FindField(fieldName);
            int deviceGUIDFieldIndex = _table.Fields.FindField("DEVICEGUID");
            IRow row = null;
            while ((row = cursor.NextRow()) != null)
            {
                string val = row.get_Value(IDIndex).ToString();
                string deviceGUID = row.get_Value(deviceGUIDFieldIndex).ToString();
                if (!IDs.Contains(val) && stitchPointGuids.Contains(deviceGUID))
                {
                    IDs.Add(val);
                }
                if (stitchPointGuids.Contains(deviceGUID))
                {
                    stitchPointsToProcess++;
                    if (CircuitCountMapping.ContainsKey(val)) { CircuitCountMapping[val]++; }
                    else
                    {
                        CircuitCountMapping.Add(val, 1);
                    }
                }
            }
            Marshal.ReleaseComObject(stitchPointsCursor);
            Marshal.ReleaseComObject(cursor);
            return IDs;
        }

        /// <summary>
        /// Return the list of values for the defined fieldname of the given table.
        /// </summary>
        /// <param name="tableName">The name of the table to query</param>
        /// <param name="fieldName">The field to read values from</param>
        /// <param name="OIDs">An output list used to hold the OBJECTID of the rows returned by the Search.</param>
        /// <returns></returns>
        private List<string> LoadCache(IWorkspace wsForLookup, string tableName, string fieldName, out List<long> OIDs, 
            ref int stitchPointsToProcess, List<String> validStitchPoints)
        {

            // #8
            stitchPointsToProcess = 0;

            List<string> IDs = new List<string>();
            _table = GetTable(wsForLookup, tableName);
            IQueryFilter filter = new QueryFilterClass();
            filter.WhereClause = fieldName + " is not null";
            ICursor cursor = _table.Search(filter, true);
            int IDIndex = _table.FindField(fieldName);
            int oidIndex = _table.FindField(_table.OIDFieldName);
            OIDs = new List<long>();
            IRow row = null;
            while ((row = cursor.NextRow()) != null)
            {
                OIDs.Add(Convert.ToInt64(row.get_Value(oidIndex)));
                string val = row.get_Value(IDIndex).ToString();
                if (validStitchPoints.Contains(val))
                {
                    if (!IDs.Contains(val))
                    {
                        IDs.Add(val);

                        if (CircuitCountMapping.ContainsKey(val)) { stitchPointsToProcess += CircuitCountMapping[val]; }
                        else { stitchPointsToProcess++; }
                    }
                }
            }
            Marshal.ReleaseComObject(cursor);
            return IDs;
        }

        /// <summary>
        /// RBAE - 11/12/13 - Deprecated - use CleanChangeDetectionTables() instead
        /// Delete the circuits from the change table that were processed
        /// </summary>
        public void CleanChangeDetectionTables()
        {
            //RBAE - 11/12/13 - With Change Detection 2.0, the EDGIS.PGE_CHANGED_CIRCUIT and EDGIS.PGE_CHANGED_SUBSTATION
            //tables are unversioned.  The tables just need to be truncated instead of deleting rows from DEFAULT.
           // PGE.Interface.Integration.DMS.Common.Oracle.ExecuteProcedure(Configuration.CadopsConnection, "DMSSTAGING.CLEANUPDMSCHANGEDETECTION");
            PGE.Interface.Integration.DMS.Common.Oracle.ExecuteProcedure(Configuration.EDERConn, "DMSSTAGING.CLEANUPDMSCHANGEDETECTION");
        }

    }







}

