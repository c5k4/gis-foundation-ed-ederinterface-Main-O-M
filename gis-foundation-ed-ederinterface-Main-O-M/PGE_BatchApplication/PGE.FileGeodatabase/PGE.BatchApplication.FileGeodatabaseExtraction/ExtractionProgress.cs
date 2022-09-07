using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.GeoDatabaseDistributed;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using ESRI.ArcGIS.esriSystem;

namespace PGE.BatchApplication.FGDBExtraction
{
    public class ExtractionProgress : ESRI.ArcGIS.Geodatabase.IFeatureProgress
    {
        private static IWorkspace ExtractWorkspace = null;
        public Dictionary<ITable, int> TablesToProcess = new Dictionary<ITable, int>();
        public Dictionary<ITable, int> TotalProcessedByTable = new Dictionary<ITable, int>();
        List<string> TablesProcessed = new List<string>();
        public ExtractionProgress(IConnectionPointContainer connectionPointContainer)
        {
            // Get the event source's connection points.
            IEnumConnectionPoints enumConnectionPoints = null;
            connectionPointContainer.EnumConnectionPoints(out enumConnectionPoints);
            enumConnectionPoints.Reset();

            // Iterate through the connection points until one for IFeatureProgress is found.
            IConnectionPoint connectionPoint = null;
            Guid featureProgressGuid = typeof(IFeatureProgress).GUID;
            uint pcFetched = 0;
            enumConnectionPoints.RemoteNext(1, out connectionPoint, out pcFetched);
            while (connectionPoint != null)
            {
                Guid connectionInterfaceGuid;
                connectionPoint.GetConnectionInterface(out connectionInterfaceGuid);
                if (connectionInterfaceGuid == featureProgressGuid)
                {
                    break;
                }
                enumConnectionPoints.RemoteNext(1, out connectionPoint, out pcFetched);
            }

            // If IFeatureProgress wasn't found, throw an exception.
            if (connectionPoint == null)
            {
                throw new ArgumentException("An IFeatureProgress connection point could not be found.");
            }

            // Tie into the connection point.
            uint connectionPointCookie = 0;
            connectionPoint.Advise(this, out connectionPointCookie);
        }

        private bool DatasetIncludedInReplicaSet(IGPReplicaDatasets replicaDatasets, string tableName)
        {
            for (int i = 0; i < replicaDatasets.Count; i++)
            {
                IGPReplicaDataset replicaDataset = replicaDatasets.get_Element(i);
                if (replicaDataset.Name == tableName) { return true; }
            }

            return false;
        }

        /*
        private ISet GetFeatureSet(IFeatureClass featClass, IGeometry queryGeometry)
        {
            ISet featureSet = new SetClass();
            ISpatialFilter sf = new SpatialFilter();
            int guidFieldIdx = featClass.FindField("GLOBALID");
            sf.SubFields = featClass.OIDFieldName;
            if (guidFieldIdx > 0) { sf.SubFields += ",GLOBALID"; }
            sf.Geometry = queryGeometry;
            sf.GeometryField = featClass.ShapeFieldName;
            sf.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
            IFeatureCursor featCursor = featClass.Search(sf, false);
            IFeature feature = null;
            while ((feature = featCursor.NextFeature()) != null)
            {
                featureSet.Add(feature);
            }

            while ((Marshal.ReleaseComObject(sf) > 0)) { }
            while ((Marshal.ReleaseComObject(featCursor) > 0)) { }

            return featureSet;
        }
        */
        private void GetFeatureCount(IFeatureClass featClass, IGeometry queryGeometry, ref List<int> FeatureOIDs, 
            ref Dictionary<ITable, List<int>> relatedTableOIDs, IGPReplicaDatasets replicaDatasets)
        {
            FeatureOIDs = new List<int>();

            ISpatialFilter sf = new SpatialFilter();
            sf.SubFields = featClass.OIDFieldName;
            sf.Geometry = queryGeometry;
            sf.GeometryField = featClass.ShapeFieldName;
            sf.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
            
            IFeatureCursor featCursor = featClass.Search(sf, false);
            IFeature feature = null;
            while ((feature = featCursor.NextFeature()) != null)
            {
                FeatureOIDs.Add(feature.OID);
                while ((Marshal.ReleaseComObject(feature) > 0)) { }
            }

            while ((Marshal.ReleaseComObject(sf) > 0)) { }
            while ((Marshal.ReleaseComObject(featCursor) > 0)) { }

            //Now process related table information
            string tableName = ((IDataset)featClass).BrowseName;
            string shortTableName = tableName.Substring(tableName.LastIndexOf(".") + 1);
            List<int> relatedObjectOIDs = new List<int>();
            IEnumRelationshipClass relClasses = ((IObjectClass)featClass).get_RelationshipClasses(esriRelRole.esriRelRoleAny);
            relClasses.Reset();
            IRelationshipClass relClass = null;
            while ((relClass = relClasses.Next()) != null)
            {
                string relatedTableName = "";
                    string relatedTableShortName = "";
                string joinClause = "";
                ITable relatedTable = null;
                if (relClass.OriginClass == featClass) 
                { 
                    relatedTable = relClass.DestinationClass as ITable; 
                    relatedTableName = ((IDataset)relatedTable).BrowseName;
                    relatedTableShortName = relatedTableName.Substring(relatedTableName.LastIndexOf(".") + 1);
                    joinClause = shortTableName + "." + relClass.OriginPrimaryKey + " = " + relatedTableShortName + "." + relClass.OriginForeignKey;
                }
                else if (relClass.DestinationClass == featClass)
                {
                    relatedTable = relClass.OriginClass as ITable;
                    relatedTableName = ((IDataset)relatedTable).BrowseName;
                    relatedTableShortName = relatedTableName.Substring(relatedTableName.LastIndexOf(".") + 1);
                    joinClause = relatedTableShortName + "." + relClass.OriginPrimaryKey + " = " + shortTableName + "." + relClass.OriginForeignKey;
                }

                if (DatasetIncludedInReplicaSet(replicaDatasets, ((IDataset)relatedTable).BrowseName) && !(relatedTable is IFeatureClass))
                {
                    if (!relatedTableOIDs.ContainsKey(relatedTable)) { relatedTableOIDs.Add(relatedTable, new List<int>()); }
                    
                    IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)((IDataset)featClass).Workspace;
                    IQueryDef queryDef = featureWorkspace.CreateQueryDef();
                    queryDef.Tables = ((IDataset)featClass).BrowseName + "," + ((IDataset)relatedTable).BrowseName;
                    queryDef.SubFields = shortTableName + "." + featClass.OIDFieldName + "," + relatedTableShortName + "." + relatedTable.OIDFieldName;

                    queryDef.WhereClause = joinClause;
                    string OIDFieldName = shortTableName + "." + featClass.OIDFieldName;
                    ICursor rowCursor = queryDef.Evaluate();
                    IRow row = null;
                    while ((row = rowCursor.NextRow()) != null)
                    {
                        //Get the related table OID
                        int featureOID = (int)row.get_Value(0);
                        if (FeatureOIDs.Contains(featureOID))
                        {
                            int RelatedOID = (int)row.get_Value(1);
                            relatedTableOIDs[relatedTable].Add(RelatedOID);
                        }
                        while (Marshal.ReleaseComObject(row) > 0) { }
                    }
                    while (Marshal.ReleaseComObject(rowCursor) > 0) { }

                    /*
                    List<string> whereInClauses = GetWhereInClauses(FeatureOIDs);
                    foreach (string whereInClause in whereInClauses)
                    {
                        queryDef.WhereClause = shortTableName + "." + featClass.OIDFieldName + " in (" + whereInClause + ") AND " + joinClause;
                        ICursor rowCursor = queryDef.Evaluate();
                        IRow row = null;
                        while ((row = rowCursor.NextRow()) != null)
                        {
                            //Get the related table OID
                            int oid = (int)row.get_Value(1);
                            relatedTableOIDs[relatedTable].Add(oid);
                            while (Marshal.ReleaseComObject(row) > 0) { }
                        }
                        while (Marshal.ReleaseComObject(rowCursor) > 0) { }
                    }
                    */
                }
            }
        }

        /// <summary>
        /// Obtains a list of where in clauses for a list of guids
        /// </summary>
        /// <param name="guids"></param>
        /// <returns></returns>
        public static List<string> GetWhereInClauses(List<int> guids)
        {
            try
            {
                List<string> whereInClauses = new List<string>();
                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < guids.Count; i++)
                {
                    if ((((i % 999) == 0) && i != 0) || (guids.Count == i + 1))
                    {
                        builder.Append(guids[i]);
                        whereInClauses.Add(builder.ToString());
                        builder = new StringBuilder();
                    }
                    else { builder.Append(guids[i] + ","); }
                }
                return whereInClauses;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create guid where in clauses. Error: " + ex.Message);
            }
        }

        /*
        private int GetFeatureCount(IFeatureClass featClass, IGeometry queryGeometry, ref List<int> FeatureOIDs, ref List<string> FeatureGUIDs)
        {
            ISet featureSet = new SetClass();
            ISpatialFilter sf = new SpatialFilter();
            int guidFieldIdx = featClass.FindField("GLOBALID");
            sf.SubFields = featClass.OIDFieldName;
            if (guidFieldIdx > 0) { sf.SubFields += ",GLOBALID"; }
            sf.Geometry = queryGeometry;
            sf.GeometryField = featClass.ShapeFieldName;
            sf.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;

            int featureCount = featClass.FeatureCount(sf);

            while ((Marshal.ReleaseComObject(sf) > 0)) { }

            return featureCount;
        }
        */

        private void AddFeatureCount(ITable table, int count)
        {
            TablesToProcess.Add(table, count);
            ExtractDataForm.Instance.LogMessage("          " + ((IDataset)table).BrowseName + ": " + count + " rows found to replicate");
        }

        /*
        private List<int> GetRelatedTableCount(IGPReplicaDatasets replicaDatasets, ITable table, ISet setOfObjects, 
            ref Dictionary<ITable, List<int>> relatedTableOIDs)
        {
            List<int> relatedObjectOIDs = new List<int>();
            IEnumRelationshipClass relClasses = ((IObjectClass)table).get_RelationshipClasses(esriRelRole.esriRelRoleAny);
            relClasses.Reset();
            IRelationshipClass relClass = null;
            while ((relClass = relClasses.Next()) != null)
            {
                ITable relatedTable = null;
                if (relClass.OriginClass == table) { relatedTable = relClass.DestinationClass as ITable; }
                else if (relClass.DestinationClass == table) { relatedTable = relClass.OriginClass as ITable; }

                if (DatasetIncludedInReplicaSet(replicaDatasets, ((IDataset)relatedTable).BrowseName) && !(relatedTable is IFeatureClass))
                {
                    if (!relatedTableOIDs.ContainsKey(relatedTable)) { relatedTableOIDs.Add(relatedTable, new List<int>()); }

                    ISet relatedObjectsSet = relClass.GetObjectsRelatedToObjectSet(setOfObjects);
                    relatedObjectsSet.Reset();
                    IRow row = null;
                    while ((row = relatedObjectsSet.Next() as IRow) != null)
                    {
                        relatedObjectOIDs.Add(row.OID);
                        while (Marshal.ReleaseComObject(row) > 0) { }
                    }
                    while (Marshal.ReleaseComObject(relatedObjectsSet) > 0) { }
                    relatedTableOIDs[relatedTable].AddRange(relatedObjectOIDs);
                }
            }

            return relatedObjectOIDs;
        }
        */

        public void DetermineToProcessCounts(IWorkspace workspace, IGeometry queryGeometry, IGPReplicaDatasets replicaDatasets)
        {
            try
            {
                ExtractWorkspace = workspace;
                ExtractDataForm.Instance.LogMessage("     Determining tables to replicate");

                Dictionary<ITable, List<int>> relatedTableOIDsToProcessTemp = new Dictionary<ITable, List<int>>();
                for (int i = 0; i < replicaDatasets.Count; i++)
                {
                    IGPReplicaDataset replicaDataset = replicaDatasets.get_Element(i);
                    if (replicaDataset.DatasetType == esriDatasetType.esriDTFeatureClass)
                    {
                        ITable table = ((IFeatureWorkspace)workspace).OpenTable(replicaDataset.Name);
                        List<int> FeatureOIDs = new List<int>();
                        GetFeatureCount(table as IFeatureClass, queryGeometry, ref FeatureOIDs, ref relatedTableOIDsToProcessTemp, replicaDatasets);
                        
                        //Set up the OIDs to extract
                        ILongArray oidLongArray = new LongArrayClass();
                        foreach (int oid in FeatureOIDs) { oidLongArray.Add(oid); }
                        replicaDataset.SelectionIDs = oidLongArray;
                        if (FeatureOIDs.Count < 1) { replicaDataset.DefQuery = "1=0"; }

                        AddFeatureCount(table, FeatureOIDs.Count);
                    }
                    else if (replicaDataset.DatasetType == esriDatasetType.esriDTFeatureDataset)
                    {
                        /* Shouldn't have any datasets to process.  They should all be broken out as individual feature classes / relationships
                        IFeatureDataset featDataset = ((IFeatureWorkspace)workspace).OpenFeatureDataset(replicaDataset.Name);
                        IFeatureClassContainer featureDatasetContainer = featDataset as IFeatureClassContainer;
                        for (int j = 0; j < featureDatasetContainer.ClassCount; j++)
                        {
                            ITable table = featureDatasetContainer.get_Class(j) as ITable;
                            List<int> FeatureOIDs = new List<int>();
                            List<string> FeatureGUIDs = new List<string>();
                            GetFeatureCount(table as IFeatureClass, queryGeometry, ref FeatureOIDs, ref relatedTableOIDsToProcessTemp, replicaDatasets);
                            //ISet featClassSet = GetFeatureSet(table as IFeatureClass, queryGeometry);
                            //GetRelatedTableCount(replicaDatasets, table, featClassSet, ref relatedTableOIDsToProcess);
                            AddFeatureCount(table, FeatureOIDs.Count);
                        }
                        */
                    }
                }

                //Get the distinct list of OIDs
                Dictionary<ITable, List<int>> relatedTableOIDsToProcess = new Dictionary<ITable, List<int>>();
                foreach (KeyValuePair<ITable, List<int>> kvp in relatedTableOIDsToProcessTemp)
                {
                    relatedTableOIDsToProcess.Add(kvp.Key, kvp.Value.Distinct().ToList());
                }

                List<ITable> tablesProcessed = new List<ITable>();
                for (int i = 0; i < replicaDatasets.Count; i++)
                {
                    IGPReplicaDataset replicaDataset = replicaDatasets.get_Element(i);
                    if (replicaDataset.DatasetType == esriDatasetType.esriDTTable) 
                    {
                        ITable table = ((IFeatureWorkspace)workspace).OpenTable(replicaDataset.Name);
                        if (relatedTableOIDsToProcess.ContainsKey(table))
                        {
                            //We have records to process for this table
                            List<int> OIDsToExtract = relatedTableOIDsToProcess[table].Distinct().ToList();
                            ILongArray oidLongArray = new LongArrayClass();
                            foreach (int oid in OIDsToExtract) { oidLongArray.Add(oid); }
                            replicaDataset.SelectionIDs = oidLongArray;
                            if (OIDsToExtract.Count < 1) { replicaDataset.DefQuery = "1=0"; }

                            AddFeatureCount(table, oidLongArray.Count);
                        }
                        else
                        {
                            replicaDataset.DefQuery = "1 = 0";
                        }
                    }
                }

                foreach (KeyValuePair<ITable, int> kvp in TablesToProcess) { TotalFeaturesToProcess += kvp.Value; }
                ExtractDataForm.Instance.LogMessage("     There are " + TotalFeaturesToProcess + " rows to replicate");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int featureCount = 0;
        public int stepValue = 0;
        public string featureClassName = "";
        public int maxFeatures = 0;
        public int minFeatures = 0;
        public int position = 0;
        public bool cancelled = false;

        String IFeatureProgress.FeatureClassName
        {
            set
            {
                featureClassName = value;
                //Console.WriteLine("FeatureClassName: {0}", value);
            }
        }

        Boolean IFeatureProgress.IsCancelled
        {
            get { return cancelled; }
        }

        int IFeatureProgress.MaxFeatures
        {
            set
            {
                maxFeatures = value;
                //Console.WriteLine("MaxFeatures: {0}", value);
            }
        }

        int IFeatureProgress.MinFeatures
        {
            set
            {
                minFeatures = value;
                //Console.WriteLine("MinFeatures: {0}", value);
            }
        }

        int IFeatureProgress.Position
        {
            set
            {
                position = value;
                //Console.WriteLine("Position: {0}", value); 
            }
        }

        public Dictionary<string, ITable> TableList = new Dictionary<string, ITable>();
        public ITable GetTable(string tableName)
        {
            if (!TableList.ContainsKey(tableName))
            {
                ITable table = ((IFeatureWorkspace)ExtractWorkspace).OpenTable(tableName);
                TableList.Add(tableName, table);
            }
            return TableList[tableName];
        }

        public int TotalFeaturesToProcess = 0;
        private string CurrentFeatureClass = "";
        private string PreviousFeatureClass = "";
        private int featuresProcessed = 0;
        private int TotalProcessed = 0;
        
        void IFeatureProgress.Step()
        {
            try
            {
                CurrentFeatureClass = featureClassName;
                if (PreviousFeatureClass != CurrentFeatureClass && !string.IsNullOrEmpty(PreviousFeatureClass))
                {
                    TablesProcessed.Add(PreviousFeatureClass);
                    PreviousFeatureClass = CurrentFeatureClass;
                    ExtractDataForm.Instance.LogMessage("     Processing table " + CurrentFeatureClass);
                }
                else if (string.IsNullOrEmpty(PreviousFeatureClass))
                {
                    PreviousFeatureClass = CurrentFeatureClass;
                    ExtractDataForm.Instance.LogMessage("     Processing table " + CurrentFeatureClass);
                }

                ITable tableBeingProcessed = GetTable(CurrentFeatureClass);

                if (!TotalProcessedByTable.ContainsKey(tableBeingProcessed)) { TotalProcessedByTable.Add(tableBeingProcessed, 0); }
                if (TotalProcessedByTable[tableBeingProcessed] < maxFeatures) 
                {
                    // Increment the number of features replicated and display the progress.
                    TotalProcessed += stepValue;
                    TotalProcessedByTable[tableBeingProcessed] += stepValue; 
                }

                int processed = TablesProcessed.Count;
                int toProcess = TablesToProcess.Count;

                int totalToProcessForTable = TablesToProcess[tableBeingProcessed];
                int totalProcessedForTable = TotalProcessedByTable[tableBeingProcessed];

                double totalPercent = Math.Round((100.0 * ((double)TotalProcessed)) / (double)TotalFeaturesToProcess, 2);
                double totalSecondaryPercent = Math.Round((100.0 * ((double)totalProcessedForTable)) / (double)totalToProcessForTable, 2);

                ExtractDataForm.Instance.ShowPrimaryPercentProgress(string.Format("{0}% complete: {1} total rows processed of {2})", totalPercent, TotalProcessed,
                    TotalFeaturesToProcess), totalPercent);

                ExtractDataForm.Instance.ShowSecondaryPercentProgress(string.Format("{0}: {1}% complete ({2} total rows processed of {3})", CurrentFeatureClass,
                    totalSecondaryPercent, totalProcessedForTable, totalToProcessForTable), totalSecondaryPercent);

                //Console.Write("\r     {0}% complete ({1} features processed) {2} of {3} tables: {4} ({5} features processed): ", percent, TotalProcessed,
                //    processed, toProcess, featureClassName, featuresProcessed);
                //ExtractDataForm.Instance.ShowPercentProgress(string.Format("{0}% complete ({5} total rows processed)\r\n{1} of {2} tables processed\r\nCurrently processing: {3} ({4} rows)", percent, processed,
                //    toProcess, featureClassName, featuresProcessed, TotalProcessed), percent);
            }
            catch (Exception ex)
            {

            }

            //Console.WriteLine("{0} {1} features replicated.", ProcessedByFeatureClass[featureClassName], featureClassName);
        }

        int IFeatureProgress.StepValue
        {
            set { stepValue = value; }
        }
    }
}
