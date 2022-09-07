using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

using ESRI.ArcGIS.DataSourcesGDB;
//using ESRI.ArcGIS.DataManagementTools;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
//using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
//using ESRI.ArcGIS.GeoDatabaseDistributed;

namespace PoleLoadingStats
{
    public class ExtractUtils
    {
        private FileInfo _logfileInfo = null;
        //ISpatialReference _toSpatialRef = null;
        //ITransformation _transformation;

        public enum SynchType
        {
            SynchTypeInsert = 0,
            SynchTypeUpdate = 1,
            SynchTypeDelete = 2
        }

        //public ExtractUtils(FileInfo logfileInfo)
        //{
        //    _logfileInfo = logfileInfo;
        //    //_toSpatialRef = toSpatialRef;
        //    //_transformation = transformation;
        //}

        //public ISpatialReference SpatialRef
        //{
        //    get { return _toSpatialRef; }
        //    set { _toSpatialRef = value; }
        //}

        //public ITransformation Transformation
        //{
        //    get { return _transformation; }
        //    set { _transformation = value; }
        //}

        public ExtractUtils(FileInfo logfileInfo)
        {
            _logfileInfo = logfileInfo;
        }

        public ExtractUtils()
        {

        }

        public void WriteToLogfile(string msg)
        {
            try
            {
                if (_logfileInfo == null)
                    return;

                //fInfo.Create();
                using (StreamWriter sWriter = _logfileInfo.AppendText())
                {
                    sWriter.WriteLine(DateTime.Now.ToLongTimeString() + " " + msg);
                    sWriter.Close();
                }
            }
            catch
            {
                //Do nothing 
            }
        }

        /// <summary>
        /// Creates a fuile GDB at the specified path 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private IWorkspace CreateFileGDB(string path)
        {
            try
            {
                // Instantiate a file geodatabase workspace factory and create a file geodatabase.
                // The Create method returns a workspace name object.
                Type factoryType = Type.GetTypeFromProgID(
                    "esriDataSourcesGDB.FileGDBWorkspaceFactory");
                IWorkspaceFactory workspaceFactory = (IWorkspaceFactory)Activator.CreateInstance
                    (factoryType);
                IWorkspaceName workspaceName = workspaceFactory.Create(path, "Sample.gdb", null,
                    0);

                // Cast the workspace name object to the IName interface and open the workspace.
                IName name = (IName)workspaceName;
                IWorkspace workspace = (IWorkspace)name.Open();
                return workspace;
            }
            catch (Exception ex)
            {
                WriteToLogfile("Error creating File GDB: " + ex.Message);
                throw new Exception("Error creating File GDB");
            }
        }

        /// <summary>
        /// Check if the fileGDB exists but look for the stop file not just the 
        /// existance of the folder - this will ensure that there is no problem 
        /// where it is only partially copied/created 
        /// </summary>
        /// <param name="mapGeo"></param>
        /// <param name="requiresExtraction"></param>
        /// 
        public bool GeodatabaseExistsInFolder(string folder, string mapGeo)
        {
            try
            {
                bool gdbExists = false;
                string stopFile = System.IO.Path.Combine(folder, mapGeo.ToUpper() + ".txt");
                if (File.Exists(stopFile))
                    gdbExists = true;
                return gdbExists;
            }
            catch (Exception ex)
            {
                WriteToLogfile("Error in GeodatabaseExistsInFolder stack trace: " + ex.StackTrace);
                throw new Exception("An error occurred in GeodatabaseExistsInFolder: " +
                    ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IFeatureWorkspace GetWorkspace(string connectionFile)
        {
            IWorkspace pWS = null;
            IFeatureWorkspace pFWS = null;

            try
            {
                // set feature workspace to the SDE connection                
                if (connectionFile.ToLower().EndsWith(".sde"))
                {
                    if (!File.Exists(connectionFile))
                        WriteToLogfile("The connection file: " + connectionFile + " does not exist on the file system!");
                    SdeWorkspaceFactory sdeWorkspaceFactory = (SdeWorkspaceFactory)new SdeWorkspaceFactory();
                    pWS = sdeWorkspaceFactory.OpenFromFile(connectionFile, 0);
                }
                else if (connectionFile.ToLower().EndsWith(".gdb"))
                {
                    if (!Directory.Exists(connectionFile))
                        WriteToLogfile("The file geodatabase: " + connectionFile + " does not exist on the file system!");

                    Type t = Type.GetTypeFromProgID("esriDataSourcesGDB.FileGDBWorkspaceFactory");
                    System.Object obj = Activator.CreateInstance(t);
                    IPropertySet propertySet = new PropertySetClass();
                    propertySet.SetProperty("DATABASE", @connectionFile);
                    IWorkspaceFactory workspaceFactory = (IWorkspaceFactory)obj;
                    pWS = workspaceFactory.OpenFromFile(@connectionFile, 0);
                }

                pFWS = (IFeatureWorkspace)pWS;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error opening SDE workspace.\r\n{0}", ex.Message));
            }
            return pFWS;
        }

        /// <summary>
        /// Compacts the File Geodatabase - I have found that this can vastly 
        /// improve the performance
        /// </summary>
        public void CompactGeodatabase(IFeatureWorkspace pFWS)
        {
            try
            {
                //Load up the workspaces
                WriteToLogfile("Entering CompactGeodatabase");
                IDatabaseCompact pDBCompact = (IDatabaseCompact)pFWS;
                if (pDBCompact.CanCompact())
                {
                    WriteToLogfile("CanCompact returned true");
                    pDBCompact.Compact();
                    WriteToLogfile("Compact completed successfully");
                }
                else
                {
                    WriteToLogfile("CanCompact returned false");
                    WriteToLogfile("File GeoDatabase was not compacted");
                }
                WriteToLogfile("Leaving CompactGeodatabase");
            }
            catch (Exception ex)
            {
                //Do nothing - does not matter if dataset is not within a certain 
                //featuredataset - map production can still function ok 
                WriteToLogfile("Error in CompactGeodatabase: " + ex.Message);
            }
        }

        /// <summary>
        /// Copies a file GDB to the passed folder 
        /// </summary>
        /// <param name="fromGDBFolder"></param>
        /// <param name="toGDBFolder"></param>
        /// <param name="fromGDBName"></param>
        /// <param name="toGDBName"></param>
        //public void CopyGeodatabaseToFolder(
        //    string fromGDBFolder,
        //    string toGDBFolder,
        //    string fromGDBName,
        //    string toGDBName)
        //{
        //    try
        //    {
        //        //_pLogger.Log("Entering CopyGeodatabaseToFolder");
        //        //_pLogger.Log("    fromGDBFolder: " + fromGDBFolder);
        //        //_pLogger.Log("    toGDBFolder: " + toGDBFolder);
        //        //_pLogger.Log("    fromGDBName: " + fromGDBName);
        //        //_pLogger.Log("    toGDBName: " + toGDBName);

        //        string fromGDBFile = System.IO.Path.Combine(fromGDBFolder, fromGDBName + ".gdb");
        //        string toGDBFile = System.IO.Path.Combine(toGDBFolder, toGDBName + ".gdb");
        //        string stopFile = System.IO.Path.Combine(toGDBFolder, toGDBName.ToLower() + ".txt");

        //        //First have to delete the existing target folder
        //        GC.Collect();
        //        if (File.Exists(stopFile))
        //            File.Delete(stopFile);
        //        //_pLogger.Log("Deleted stop file");

        //        //Delete the actual GDB Folder 
        //        DirectoryInfo dirOldTarget = new DirectoryInfo(toGDBFile);
        //        if (dirOldTarget.Exists)
        //            dirOldTarget.Delete(true);
        //        //_pLogger.Log("Deleted old gdb: " + toGDBFile);

        //        //Copy the GDB to the toGDBFolder
        //        //_pLogger.Log("Executing Copy geoprocessor tool");
        //        var geoprocessor = new Geoprocessor();
        //        geoprocessor.Execute(new ESRI.ArcGIS.DataManagementTools.Copy(fromGDBFile, toGDBFile), null);
        //        System.IO.FileStream fs = File.Create(stopFile);
        //        fs.Close();
        //        //_pLogger.Log("Copy finished successfully");
        //    }
        //    catch (Exception ex)
        //    {
        //        //_pLogger.Log("Entering error for CopyGeodatabaseToFolder: " + ex.Message);
        //        throw new Exception("Error copying geodatabase to folder");
        //    }
        //}

        /// <summary>
        /// Copies a dataset from one FileGDB to another 
        /// </summary>
        /// <param name="fromGDBFolder"></param>
        /// <param name="toGDBFolder"></param>
        /// <param name="fromGDBName"></param>
        /// <param name="toGDBName"></param>
        //public bool CopyDataset(
        //    string sourcegdbFolder,
        //    string targetgdbFolder,
        //    string fromFCName)
        //{
        //    try
        //    {
        //        WriteToLogfile("Entering CopyDataset");
        //        WriteToLogfile("    fromFCName: " + fromFCName);

        //        // Create workspace name objects.
        //        IWorkspaceName sourceWorkspaceName = new WorkspaceNameClass();
        //        IWorkspaceName targetWorkspaceName = new WorkspaceNameClass();
        //        IName targetName = (IName)targetWorkspaceName;

        //        // Set the workspace name properties.
        //        sourceWorkspaceName.PathName = sourcegdbFolder;
        //        sourceWorkspaceName.WorkspaceFactoryProgID =
        //            "esriDataSourcesGDB.FileGDBWorkspaceFactory";
        //        targetWorkspaceName.PathName = targetgdbFolder;
        //        targetWorkspaceName.WorkspaceFactoryProgID =
        //            "esriDataSourcesGDB.FileGDBWorkspaceFactory";

        //        // Create a name object for the source feature class.
        //        IFeatureClassName featureClassName = new FeatureClassNameClass();

        //        // Set the featureClassName properties.
        //        IDatasetName sourceDatasetName = (IDatasetName)featureClassName;
        //        sourceDatasetName.WorkspaceName = sourceWorkspaceName;
        //        sourceDatasetName.Name = fromFCName;
        //        IName sourceName = (IName)sourceDatasetName;

        //        // Create an enumerator for source datasets.
        //        IEnumName sourceEnumName = new NamesEnumeratorClass();
        //        IEnumNameEdit sourceEnumNameEdit = (IEnumNameEdit)sourceEnumName;

        //        // Add the name object for the source class to the enumerator.
        //        sourceEnumNameEdit.Add(sourceName);

        //        // Create a GeoDBDataTransfer object and a null name mapping enumerator.
        //        IGeoDBDataTransfer geoDBDataTransfer = new GeoDBDataTransferClass();
        //        IEnumNameMapping enumNameMapping = null;

        //        // Use the data transfer object to create a name mapping enumerator.
        //        //targetName.NameString = sourceDatasetName.Name + "_copy";
        //        WriteToLogfile("GenerateNameMapping");
        //        Boolean conflictsFound = geoDBDataTransfer.GenerateNameMapping(sourceEnumName,
        //            targetName, out enumNameMapping);
        //        enumNameMapping.Reset();
        //        WriteToLogfile("Checking for name conflicts");

        //        // Check for conflicts.
        //        if (conflictsFound)
        //        {
        //            WriteToLogfile("Name conflicts found");

        //            //We cannot have name conflicts as this will 
        //            //break MXDs - so throw error for this and 
        //            //return false from this function 
        //            throw new Exception("Name conflicts found");

        //            // Iterate through each name mapping.
        //            //INameMapping nameMapping = null;
        //            //while ((nameMapping = enumNameMapping.Next()) != null)
        //            //{
        //            //    // Resolve the mapping's conflict (if there is one).
        //            //    if (nameMapping.NameConflicts)
        //            //    {
        //            //        nameMapping.TargetName = nameMapping.GetSuggestedName(targetName);
        //            //    }

        //            //    // See if the mapping's children have conflicts.
        //            //    IEnumNameMapping childEnumNameMapping = nameMapping.Children;
        //            //    if (childEnumNameMapping != null)
        //            //    {
        //            //        childEnumNameMapping.Reset();

        //            //        // Iterate through each child mapping.
        //            //        INameMapping childNameMapping = null;
        //            //        while ((childNameMapping = childEnumNameMapping.Next()) != null)
        //            //        {
        //            //            if (childNameMapping.NameConflicts)
        //            //            {
        //            //                childNameMapping.TargetName = childNameMapping.GetSuggestedName
        //            //                    (targetName);
        //            //            }
        //            //        }
        //            //    }
        //            //}
        //        }

        //        // Start the transfer.
        //        geoDBDataTransfer.Transfer(enumNameMapping, targetName);

        //        WriteToLogfile("Copy finished successfully");
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteToLogfile("Entering error for CopyDataset: " + ex.Message);
        //        return false;
        //    }
        //}

        /// <summary>
        /// Passing zero values for all three double parameters recalculates 
        /// the spatial index with acceptable (but not necessarily optimal) values
        /// </summary>
        /// <param name="featureClass"></param>
        /// <param name="gridOneSize"></param>
        /// <param name="gridTwoSize"></param>
        /// <param name="gridThreeSize"></param>
        public void RebuildSpatialIndex(IFeatureClass pFC)
        {
            try
            {
                //Set grid sizes to zero 
                Double gridOneSize = 0;
                Double gridTwoSize = 0;
                Double gridThreeSize = 0;

                // Get an enumerator for indexes based on the shape field.
                IIndexes indexes = pFC.Indexes;
                String shapeFieldName = pFC.ShapeFieldName;
                IEnumIndex enumIndex = indexes.FindIndexesByFieldName(shapeFieldName);
                enumIndex.Reset();

                // Get the index based on the shape field (should only be one) and delete it.
                IIndex index = enumIndex.Next();
                if (index != null)
                {
                    pFC.DeleteIndex(index);
                }

                // Clone the shape field from the feature class.
                int shapeFieldIndex = pFC.FindField(shapeFieldName);
                IFields fields = pFC.Fields;
                IField sourceField = fields.get_Field(shapeFieldIndex);
                IClone sourceFieldClone = (IClone)sourceField;
                IClone targetFieldClone = sourceFieldClone.Clone();
                IField targetField = (IField)targetFieldClone;

                // Open the geometry definition from the cloned field and modify it.
                IGeometryDef geometryDef = targetField.GeometryDef;
                IGeometryDefEdit geometryDefEdit = (IGeometryDefEdit)geometryDef;
                geometryDefEdit.GridCount_2 = 3;
                geometryDefEdit.set_GridSize(0, gridOneSize);
                geometryDefEdit.set_GridSize(1, gridTwoSize);
                geometryDefEdit.set_GridSize(2, gridThreeSize);

                // Create a spatial index and set the required attributes.
                IIndex newIndex = new IndexClass();
                IIndexEdit newIndexEdit = (IIndexEdit)newIndex;
                newIndexEdit.Name_2 = String.Concat(shapeFieldName, "_Index");
                newIndexEdit.IsAscending_2 = true;
                newIndexEdit.IsUnique_2 = false;

                // Create a fields collection and assign it to the new index.
                IFields newIndexFields = new FieldsClass();
                IFieldsEdit newIndexFieldsEdit = (IFieldsEdit)newIndexFields;
                newIndexFieldsEdit.AddField(targetField);
                newIndexEdit.Fields_2 = newIndexFields;

                // Add the spatial index back into the feature class.
                pFC.AddIndex(newIndex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error rebuilding the spatial index");
            }
        }

        /// <summary>
        /// Runs a geoprocessor tool to project a dataset into the 
        /// desired stateplane coordinate system 
        /// </summary>
        //public void ProjectDataset(
        //    string datasetName,
        //    string workingGDBFolderPath,
        //    string workingGDBName,
        //    string outputCoordSystem)
        //{
        //    try
        //    {
        //        //Project the dataset using the projection tool
        //        WriteToLogfile("Projecting " + datasetName + "...");
        //        var geoprocessor = new Geoprocessor();
        //        ESRI.ArcGIS.DataManagementTools.Project projTool = new ESRI.ArcGIS.DataManagementTools.Project();
        //        projTool.in_dataset = workingGDBFolderPath + "\\" + workingGDBName + ".gdb" + "\\" + datasetName + "_unproj";
        //        WriteToLogfile("projTool.in_dataset: " + projTool.in_dataset.ToString());
        //        projTool.out_dataset = workingGDBFolderPath + "\\" + workingGDBName + ".gdb" + "\\" + datasetName;
        //        WriteToLogfile("projTool.out_dataset: " + projTool.out_dataset.ToString());
        //        projTool.out_coor_system = outputCoordSystem;
        //        WriteToLogfile("projTool.out_coor_system: " + projTool.out_coor_system.ToString());
        //        projTool.transform_method = "NAD_1927_To_NAD_1983_NADCON";
        //        WriteToLogfile("projTool.transform_method: " + projTool.transform_method.ToString());
        //        geoprocessor.Execute(projTool, null);
        //        WriteToLogfile("Projected " + datasetName);
        //    }
        //    catch (Exception ex)
        //    {
        //        //This is a 'show stopper' throw an error 
        //        WriteToLogfile("Error projecting dataset: " + ex.Message);
        //        throw new Exception("Error in ProjectDataset: " + ex.Message);
        //    }
        //}

        /// <summary>
        /// Deletes a dataset by calling IDataset::Delete()  
        /// </summary>
        public void DeleteDataset(IFeatureWorkspace pFWS, string datasetName)
        {
            try
            {
                WriteToLogfile("Deleting " + datasetName + "...");
                IFeatureClass pFC = pFWS.OpenFeatureClass(datasetName);
                IDataset pDS = (IDataset)pFC;
                if (pDS.CanDelete())
                {
                    pDS.Delete();
                    WriteToLogfile("Deleted " + datasetName);
                }
                else
                    WriteToLogfile("Unable to delete dataset " + datasetName +
                        " since CanDelete() returned false");
            }
            catch (Exception ex)
            {
                //This is not fatal since mapping will not be affected so just 
                //log the problem 
                WriteToLogfile("Error deleting dataset: " + datasetName + ex.Message);
            }
        }

        //public void DeleteDataset(
        //    string datasetName,
        //    string workingGDBFolderPath,
        //    string workingGDBName)
        //{
        //    try
        //    {
        //        //Project the dataset using the projection tool
        //        WriteToLogfile("Projecting " + datasetName + "...");
        //        var geoprocessor = new Geoprocessor();
        //        ESRI.ArcGIS.DataManagementTools.Delete delTool = new ESRI.ArcGIS.DataManagementTools.Delete();
        //        delTool.in_data = workingGDBFolderPath + "\\" + workingGDBName + ".gdb" + "\\" + datasetName;
        //        WriteToLogfile("delTool.in_dataset: " + delTool.in_data.ToString());
        //        geoprocessor.Execute(delTool, null);
        //        WriteToLogfile("Projected " + datasetName);
        //    }
        //    catch (Exception ex)
        //    {
        //        //This is a 'show stopper' throw an error 
        //        WriteToLogfile("Error projecting dataset: " + ex.Message);
        //        throw new Exception("Error in ProjectDataset: " + ex.Message);
        //    }
        //}
        /// <summary>
        /// Exports the featureclass load only and includes 
        /// joined fields from another table 
        /// </summary>
        /// <param name="msgFC"></param>
        /// <param name="useClip"></param>
        /// <param name="projectFeatureData"></param>
        /// <param name="pSourceFC"></param>
        /// <param name="pDestFC"></param>
        /// <param name="pSourceJoinTable"></param>
        /// <param name="pClipPolygon"></param>
        /// <param name="joinWhereClause"></param>
        public void ExportFeatureClassWithJoin(
            string msgFC,
            bool useClip,
            bool projectFeatureData,
            IFeatureClass pSourceFC,
            IFeatureClass pDestFC,
            ITable pSourceJoinTable,
            IPolygon pClipPolygon,
            string joinWhereClause)
        {

            IWorkspaceEdit pWSE = null;
            IFeatureClassLoad featureClassLoad = null;

            try
            {
                WriteToLogfile("Entering ExportFeatureClassWithJoin");
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                IDataset pSrcDS = (IDataset)pSourceFC;
                IDataset pDestDS = (IDataset)pDestFC;
                IDataset pSrcJoinTblDS = (IDataset)pSourceJoinTable;

                //Get names of source datasets 
                string srcFeatureclassName = GetShortDatasetName(pSrcDS.Name);
                string srcJoinTableName = GetShortDatasetName(pSrcJoinTblDS.Name);
                string browseName = pDestDS.BrowseName;

                //Create a hashtable of oids of all features within the clip polygon 
                Hashtable hshOIdsWithinClip = new Hashtable();
                ISpatialFilter pSF = new SpatialFilterClass();
                pSF.Geometry = pClipPolygon;
                pSF.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                IFeatureCursor pFCursor = pSourceFC.Search(pSF, false);
                IFeature pFeature = pFCursor.NextFeature();
                while (pFeature != null)
                {
                    hshOIdsWithinClip.Add(pFeature.OID, 0);
                    pFeature = pFCursor.NextFeature();
                }
                Marshal.FinalReleaseComObject(pFCursor);

                Hashtable hshJoinFldIndexes = new Hashtable();
                Hashtable hshJoinedAttribs = GetJoinedAttributes(
                    pSourceFC,
                    pDestFC,
                    pSourceJoinTable,
                    hshOIdsWithinClip,
                    ref hshJoinFldIndexes,
                    joinWhereClause);

                //Get the list of required fields 
                string destFieldName = "";
                string sourceFieldName = "";
                int srcFieldIndex = -1;
                int destFieldIndex = -1;
                string fieldList = "";
                Hashtable hshFields = new Hashtable();
                Hashtable hshJoinFields = new Hashtable();
                for (int i = 0; i < pDestFC.Fields.FieldCount; i++)
                {
                    destFieldName = pDestFC.Fields.get_Field(i).Name.ToLower();
                    if (pDestFC.Fields.get_Field(i).Editable == true)
                    {
                        if (destFieldName == "globalid_old")
                        {
                            WriteToLogfile(pDestDS.Name + " has field globalid_old");
                            sourceFieldName = "globalid";
                            srcFieldIndex = pSourceFC.Fields.FindField(sourceFieldName);
                        }
                        else if (destFieldName == "objectid_old")
                        {
                            WriteToLogfile(pDestDS.Name + " has field objectid_old");
                            sourceFieldName = pSourceFC.OIDFieldName;
                            srcFieldIndex = pSourceFC.Fields.FindField(sourceFieldName);
                        }
                        else if (destFieldName.StartsWith("join_"))
                        {
                            srcFieldIndex = -1;
                            hshJoinFields.Add(destFieldName, i);
                        }
                        else
                        {
                            sourceFieldName = destFieldName;
                            srcFieldIndex = pSourceFC.Fields.FindField(destFieldName);
                        }

                        destFieldIndex = i;
                        if ((srcFieldIndex != -1) && (destFieldIndex != -1))
                        {
                            int[] indexes = { srcFieldIndex, i };
                            if (hshFields.ContainsKey(destFieldName) == false)
                                hshFields.Add(destFieldName, indexes);

                            if (fieldList == "")
                                fieldList = sourceFieldName;
                            else
                                fieldList = fieldList + "," + sourceFieldName;
                        }
                    }
                }
                IDictionaryEnumerator enumFields = hshFields.GetEnumerator();
                IDictionaryEnumerator enumJoinFields = hshJoinFields.GetEnumerator();
                WriteToLogfile("Field count for: " + pDestDS.Name + " : " + hshFields.Count.ToString());

                //Setup the spatial filter 
                ITopologicalOperator pTopo =
                    (ITopologicalOperator)pClipPolygon;
                pSF = new SpatialFilterClass();
                pSF.SubFields = fieldList;
                if (useClip)
                {
                    pSF.SpatialRel = esriSpatialRelEnum.
                    esriSpatialRelIntersects;
                    pSF.Geometry = pClipPolygon;
                }
                int recCount = 0;
                int newFeatureCount = 0;
                int editCount = 0;
                int srcShapeFldIdx = pSourceFC.Fields.FindField(pSourceFC.ShapeFieldName);
                //bool projectFeatureData = false;
                //if (_mapProdVersion == 1)
                //    projectFeatureData = true;
                WriteToLogfile("projectFeatureData: " + projectFeatureData.ToString());

                //Look for the features (can use recycle cursor) 
                pFCursor = pSourceFC.Search(pSF, true);
                IFeature pSourceFeature = pFCursor.NextFeature();

                if (pSourceFeature != null)
                {
                    //Start an edit operation 
                    pWSE = (IWorkspaceEdit)pDestDS.Workspace;
                    pWSE.StartEditOperation();

                    // Cast the feature class to the IFeatureClassLoad interface.
                    featureClassLoad = (IFeatureClassLoad)pDestFC;

                    // Enable load-only mode on the feature class (much faster) 
                    featureClassLoad.LoadOnlyMode = true;

                    //Create an insert featurecusor                     
                    IFeatureCursor pInsertCursor = pDestFC.Insert(true);
                    IFeatureBuffer pInsertFB = pDestFC.CreateFeatureBuffer();

                    while (pSourceFeature != null)
                    {
                        //Loop through the features to clip   
                        recCount++;

                        //copy all the attributes 
                        enumFields.Reset();
                        while (enumFields.MoveNext())
                        {
                            destFieldName = enumFields.Key.ToString().
                                ToLower();
                            int[] indexes = (int[])enumFields.Value;
                            srcFieldIndex = indexes[0];
                            destFieldIndex = indexes[1];

                            if (srcFieldIndex == -1)
                            {
                                Debug.Print("problem is here");
                            }

                            if (srcShapeFldIdx == srcFieldIndex)
                            {
                                //Copy the shape - projecting if required                                 
                                //if (projectFeatureData)
                                //    pInsertFB.Shape = Project(
                                //        pSourceFeature.ShapeCopy,
                                //        _toSpatialRef,
                                //        _transformation);
                                //else
                                    pInsertFB.Shape = pSourceFeature.ShapeCopy;
                            }
                            else
                            {
                                //Just copy the field value accross 
                                //Debug.Print("comment to trouble shoot");
                                object theValue = pSourceFeature.
                                    get_Value(srcFieldIndex);

                                if (destFieldIndex == 86)
                                    Debug.Print("this is it");

                                pInsertFB.set_Value(destFieldIndex,
                                    theValue);
                            }
                        }

                        //Populate the joined attributes 
                        if (hshJoinedAttribs.ContainsKey(pSourceFeature.OID))
                        {
                            object[] joinedAttsArr = (object[])hshJoinedAttribs[pSourceFeature.OID];
                            enumJoinFields.Reset();
                            while (enumJoinFields.MoveNext())
                            {
                                destFieldName = enumJoinFields.Key.ToString().ToLower();
                                destFieldIndex = (int)enumJoinFields.Value;
                                pInsertFB.set_Value(destFieldIndex,
                                    joinedAttsArr[(int)hshJoinFldIndexes[destFieldName]]);
                            }
                        }
                        else
                        {
                            enumJoinFields.Reset();
                            while (enumJoinFields.MoveNext())
                            {
                                destFieldName = enumJoinFields.Key.ToString().ToLower();
                                destFieldIndex = (int)enumJoinFields.Value;
                                pInsertFB.set_Value(destFieldIndex,
                                    DBNull.Value);
                            }
                        }


                        //Insert the new feature 
                        newFeatureCount++;
                        editCount++;
                        try
                        {
                            pInsertCursor.InsertFeature(pInsertFB);
                        }
                        catch
                        {
                            WriteToLogfile("Failed to insert featurebuffer on: " +
                                pDestDS.Name + " oid: " + pSourceFeature.OID.ToString());
                            throw new Exception("Unable to insert feature");
                        }

                        //Flush every record 
                        if (newFeatureCount == 5000)
                        {
                            WriteToLogfile("Written: " + editCount.ToString() + " " + pDestDS.Name + " features");
                            try
                            { pInsertCursor.Flush(); }
                            catch
                            { throw new Exception("Unable to flush feature buffer"); }
                            newFeatureCount = 0;
                        }
                        pSourceFeature = pFCursor.NextFeature();
                    }

                    //Flush if necessary 
                    if (newFeatureCount != 0)
                        pInsertCursor.Flush();

                    //Stop the edit operation 
                    pWSE.StopEditOperation();

                    //Release the cursor and buffer 
                    Marshal.FinalReleaseComObject(pInsertCursor);
                    Marshal.FinalReleaseComObject(pInsertFB);
                }
                //Release the cursor 
                Marshal.FinalReleaseComObject(pFCursor);

                //Write message to logfile 
                WriteToLogfile(browseName + " successfully exported: " + recCount.ToString() +
                    " features with join to destination geodatabase");
                WriteToLogfile(browseName + " completed in : " + stopWatch.ElapsedMilliseconds.ToString() + " milliseconds");
                stopWatch.Stop();
            }
            catch (Exception ex)
            {
                //Just throw the error 
                WriteToLogfile("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message);
                if (pWSE != null)
                {
                    pWSE.AbortEditOperation();
                }
                throw ex;
            }
            finally
            {
                // Disable load-only mode on the feature class.
                if (featureClassLoad != null)
                    featureClassLoad.LoadOnlyMode = false;
            }
        }

        private Hashtable GetJoinedAttributes(
            IFeatureClass pSourceFC,
            IFeatureClass pDestFC,
            ITable pSourceJoinTable,
            Hashtable hshOIdsInClip,
            ref Hashtable hshJoinFldIndexes,
            string joinWhereClause)
        {

            try
            {
                WriteToLogfile("Entering GetJoinedAttributes");
                Hashtable hshJoinedAttributes = new Hashtable();
                IDataset pSrcDS = (IDataset)pSourceFC;
                IDataset pDestDS = (IDataset)pDestFC;
                IDataset pSrcJoinTblDS = (IDataset)pSourceJoinTable;

                //Get names of source datasets 
                //string srcFeatureclassName = GetShortDatasetName(pSrcDS.Name);
                //string srcJoinTableName = GetShortDatasetName(pSrcJoinTblDS.Name);

                //Create a queryDef (does an inner join) 
                IFeatureWorkspace pSourceFWS = (IFeatureWorkspace)pSrcDS.Workspace;
                IQueryDef queryDef = pSourceFWS.CreateQueryDef();

                //Take all the field from the join table which have a corresponding 
                //field of "join_" + field_name in the dest featureclass

                //Get the list of required fields 
                string destFieldName = "";
                string sourceFieldName = "";
                string destFieldNameFull = "";
                string sourceFieldNameFull = "";
                int srcFieldIndex = -1;
                int destFieldIndex = -1;
                int joinedFldIdx = 0;
                string fieldList = "";

                //First get the joined attributes into a hashtable keyed 
                //off the objectid and with the joined attributes as an 
                //array of objects 
                Hashtable hshFields = new Hashtable();
                Hashtable hshFieldNames = new Hashtable();
                hshJoinFldIndexes.Clear();

                //Add the OId to the field list 
                for (int i = 0; i < pDestFC.Fields.FieldCount; i++)
                {
                    destFieldName = pDestFC.Fields.get_Field(i).Name.ToLower();
                    destFieldNameFull = pDestDS.Name.ToLower() + "." + destFieldName;
                    sourceFieldName = "";
                    sourceFieldNameFull = "";

                    if (pDestFC.Fields.get_Field(i).Editable == true)
                    {
                        if (destFieldName.StartsWith("join_"))
                        {
                            sourceFieldName = destFieldName.Replace("join_", "");
                            if (pSourceJoinTable.Fields.FindField(sourceFieldName) != -1)
                                sourceFieldNameFull = pSrcJoinTblDS.Name + "." + sourceFieldName;
                        }

                        if (sourceFieldNameFull != string.Empty)
                        {
                            if (!hshFieldNames.ContainsKey(destFieldNameFull))
                            {
                                hshFieldNames.Add(destFieldNameFull, sourceFieldNameFull);
                                hshJoinFldIndexes.Add(destFieldName, joinedFldIdx);
                                if (fieldList == string.Empty)
                                    fieldList = sourceFieldNameFull;
                                else
                                    fieldList += "," + sourceFieldNameFull;
                                joinedFldIdx++;
                            }
                        }
                    }
                }

                //Exit out if no joined attibutes  
                if (hshFieldNames.Count == 0)
                    return hshJoinedAttributes;

                //Figure this out from the fields on the destination dataset - where 
                //prefixed with "JOIN_" this is a joined field from the pSourceJoinTable
                fieldList += "," + pSrcDS.Name + "." + pSourceFC.OIDFieldName;
                queryDef.SubFields = fieldList;
                queryDef.Tables = pSrcDS.Name + "," + pSrcJoinTblDS.Name;
                queryDef.WhereClause = joinWhereClause;
                ICursor pCursor = queryDef.Evaluate();

                //Determine the field indexes 
                foreach (string field in hshFieldNames.Keys)
                {
                    destFieldNameFull = field;
                    destFieldName = GetShortDatasetName(destFieldNameFull);
                    sourceFieldNameFull = hshFieldNames[field].ToString();
                    destFieldIndex = pDestFC.Fields.FindField(destFieldName);
                    srcFieldIndex = pCursor.Fields.FindField(sourceFieldNameFull);
                    int[] indexes = { srcFieldIndex, destFieldIndex };
                    hshFields.Add(destFieldName, indexes);
                }
                IDictionaryEnumerator enumFields = hshFields.GetEnumerator();
                WriteToLogfile("Field count for: " + pDestDS.Name + " : " + hshFields.Count.ToString());

                //Loop through the joined results 
                IRow pSourceRow = pCursor.NextRow();
                int joinCount = 0;
                int oidFldIdx = pCursor.Fields.FindField(pSrcDS.Name + "." + pSourceFC.OIDFieldName);
                bool extractRow = false;
                int oid = -1;
                //int srcShapeFldIdx = pCursor.Fields.FindField(pSrcDS.Name + "." + pSourceFC.ShapeFieldName);
                object[] pJoinedAtts;

                while (pSourceRow != null)
                {


                    extractRow = false;
                    oid = Convert.ToInt32(pSourceRow.get_Value(oidFldIdx));
                    if (hshOIdsInClip.ContainsKey(oid))
                        extractRow = true;

                    if (extractRow)
                    {
                        //Loop through the features to clip   
                        joinCount++;

                        pJoinedAtts = new object[hshFieldNames.Count];

                        //copy all the attributes 
                        enumFields.Reset();
                        while (enumFields.MoveNext())
                        {
                            destFieldName = enumFields.Key.ToString().
                                ToLower();
                            int[] indexes = (int[])enumFields.Value;
                            srcFieldIndex = indexes[0];
                            destFieldIndex = indexes[1];

                            //Store the joined attribute 
                            pJoinedAtts[(int)hshJoinFldIndexes[destFieldName]] =
                                pSourceRow.get_Value(srcFieldIndex);
                        }

                        //Add the attributes to the hashtable of attributes
                        if (!hshJoinedAttributes.ContainsKey(oid))
                            hshJoinedAttributes.Add(oid, pJoinedAtts);
                        else
                            Debug.Print("oid not unique");

                        //Get the next joined feature  
                        joinCount++;
                    }
                    pSourceRow = pCursor.NextRow();
                }

                //Release the cursor 
                Marshal.FinalReleaseComObject(pCursor);

                //Return the joined attributes 
                return hshJoinedAttributes;
            }
            catch (Exception ex)
            {
                //Just throw the error 
                WriteToLogfile("An error occurred in: " + "GetJoinedAttributes: " + ex.Message);
                throw ex;
            }
        }


        /// <summary>
        /// Exports an Anno featureclass MUCH faster than ExportFeatureClassAnno
        /// routine but it does not set the classid - so I have created a dummy 
        /// classid which can be used by map layers to filter what is displayed 
        /// to achieve the same result
        /// </summary>
        /// <param name="msgFC"></param>
        /// <param name="filter"></param>
        /// <param name="pSourceFC"></param>
        /// <param name="pDestFC"></param>
        /// <param name="pClipPolygon"></param>
        public void ExportAnnoFeature(
            string msgFC,
            string filter,
            string sourceFeatureFilter,
            bool useClip,
            bool projectFeatureData,
            IFeatureClass pSourceFC,
            IFeatureClass pDestFC,
            IPolygon pClipPolygon)
        {
            ITransactions pTransactions = null;

            try
            {
                WriteToLogfile("Entering ExportAnnoFeature");
                WriteToLogfile("    filter: " + filter.ToString());
                WriteToLogfile("    useClip: " + useClip.ToString());
                WriteToLogfile("    sourceFeatureFilter: " + sourceFeatureFilter.ToString());

                //Setup the stopwatch 
                Stopwatch pStopWatch = new Stopwatch();
                pStopWatch.Start();

                //set up fields for mapping
                IFields pOFlds = pDestFC.Fields;
                IFields pIFlds = pSourceFC.Fields;
                int featureIdFldIdx = pSourceFC.Fields.FindField("featureid");

                //First just get the mapped field count 
                int fldCount = 0;
                for (int i = 0; i < pIFlds.FieldCount; i++)
                {
                    IField pFld = pIFlds.get_Field(i);
                    if ((pFld.Type != esriFieldType.esriFieldTypeOID) &&
                        (pFld.Type != esriFieldType.esriFieldTypeGeometry) &&
                        (pFld.Name.ToUpper() != "ELEMENT") &&
                        (pFld.Name.ToUpper() != "ZORDER") &&
                        (pFld.Editable))
                    {
                        Debug.Print(pFld.Name);
                        fldCount++;
                    }
                }

                //create arrays for mapped field indexes 
                int[] lSrcFlds = new int[fldCount];
                int[] lTrgFlds = new int[fldCount];
                string targetFieldName = string.Empty;
                fldCount = 0;
                for (int i = 0; i < pIFlds.FieldCount; i++)
                {
                    IField pFld = pIFlds.get_Field(i);
                    if ((pFld.Type != esriFieldType.esriFieldTypeOID) &&
                        (pFld.Type != esriFieldType.esriFieldTypeGeometry) &&
                        (pFld.Name.ToUpper() != "ELEMENT") &&
                        (pFld.Name.ToUpper() != "ZORDER") &&
                        (pFld.Editable))
                    {
                        //Generate the field mapping 
                        if (pFld.Name.ToUpper() == "ANNOTATIONCLASSID")
                            targetFieldName = "ANNOTATIONCLASSID_OLD";
                        else
                            targetFieldName = pFld.Name;

                        lSrcFlds[fldCount] = i;
                        lTrgFlds[fldCount] = pOFlds.FindField(targetFieldName);
                        fldCount++;
                    }
                }

                //Setup the spatial filter 
                IDataset pDestDS = (IDataset)pDestFC;
                ISpatialFilter pSF = new SpatialFilterClass();
                bool applyFilter = !(sourceFeatureFilter == string.Empty);
                bool includeFeature = false;
                Hashtable hshOIds = null;

                if (applyFilter)
                    hshOIds = GetRelatedFeatureOIds(pSourceFC, pDestDS.Name.ToLower(), sourceFeatureFilter);

                if (useClip)
                {
                    pSF.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                    pSF.Geometry = pClipPolygon;
                }

                //Add additional def query if passed to function 
                if (filter != string.Empty)
                {
                    pSF.WhereClause = filter;
                    WriteToLogfile("Applied whereclause " + pSF.WhereClause);
                }

                //Start transaction for performance                  
                pTransactions = (ITransactions)pDestDS.Workspace;
                pTransactions.StartTransaction();
                int newFeatureCount = 0;
                int editCount = 0;
                //bool projectFeatureData = false;
                //if (_mapProdVersion == 1)
                //    projectFeatureData = true;
                WriteToLogfile("projectFeatureData: " + projectFeatureData.ToString());

                //Search the source anno class 
                IFeatureCursor pICursor = pSourceFC.Search(pSF, true);
                IFeature pSourceFeature = pICursor.NextFeature();
                IFDOGraphicsLayerFactory pGLF = new FDOGraphicsLayerFactoryClass();
                IFDOGraphicsLayer pFDOGLayer = (IFDOGraphicsLayer)pGLF.OpenGraphicsLayer(
                    (IFeatureWorkspace)pDestDS.Workspace,
                    pDestFC.FeatureDataset,
                    pDestDS.Name);
                IFDOAttributeConversion pFDOACon = (IFDOAttributeConversion)pFDOGLayer;
                IClone pAClone = null;
                IAnnotationFeature pAnnoFeature = null;
                IElement pGSElement = null;

                //Add the anno features as FDO elements 
                IElementCollection pElementColl = new ElementCollectionClass();
                pFDOGLayer.BeginAddElements();
                pFDOACon.SetupAttributeConversion2(fldCount, lSrcFlds, lTrgFlds);
                while (pSourceFeature != null)
                {
                    //Determine if we want to include this feature  
                    includeFeature = true;
                    pAnnoFeature = (IAnnotationFeature)pSourceFeature;

                    if (applyFilter)
                    {
                        if (!hshOIds.ContainsKey(pAnnoFeature.LinkedFeatureID))
                            includeFeature = false;
                    }

                    if (includeFeature)
                    {
                        newFeatureCount++;
                        editCount++;

                        if (pAnnoFeature.Annotation != null)
                        {
                            pAClone = (IClone)pAnnoFeature.Annotation;
                            pGSElement = (IElement)pAClone.Clone();
                            pGSElement.Geometry.SpatialReference =
                                pAnnoFeature.Annotation.Geometry.SpatialReference;

                            //Project if required                                 
                            //if (projectFeatureData)
                            //    pGSElement.Geometry = Project(
                            //        pGSElement.Geometry,
                            //        _toSpatialRef,
                            //        _transformation);
                            pFDOGLayer.DoAddFeature(pSourceFeature, pGSElement, 0);
                        }

                        if (newFeatureCount == 5000)
                        {
                            WriteToLogfile("Written: " + editCount.ToString() + " " + pDestDS.Name + " features");
                            newFeatureCount = 0;
                            pTransactions.CommitTransaction();
                            pTransactions.StartTransaction();
                            GC.Collect();
                        }
                    }
                    pSourceFeature = pICursor.NextFeature();
                }

                Marshal.FinalReleaseComObject(pICursor);

                //End adding elements and commit 
                pFDOGLayer.EndAddElements();
                pTransactions.CommitTransaction();

                //Write message to logfile 
                WriteToLogfile(pDestDS.BrowseName + " successfully exported: " + editCount.ToString() +
                    " anno features to destination geodatabase");
                WriteToLogfile(pDestDS.BrowseName + " completed in : " + pStopWatch.ElapsedMilliseconds.ToString() + " milliseconds");
                pStopWatch.Stop();

            }
            catch (Exception ex)
            {
                //Just throw the error 
                WriteToLogfile("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message);
                if (pTransactions != null)
                    pTransactions.AbortTransaction();
                throw ex;
            }
        }

        public void ExportTable(
            string msgTable,
            ITable pSourceTable,
            ITable pDestTable,
            string filter)
        {

            IWorkspaceEdit pWSE = null;

            try
            {
                WriteToLogfile("Entering ExportTable");
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                IDataset pSrcDS = (IDataset)pSourceTable;
                IDataset pDestDS = (IDataset)pDestTable;
                string browseName = pDestDS.BrowseName;
                WriteToLogfile(msgTable);

                //Get the list of required fields 
                string destFieldName = "";
                string sourceFieldName = "";
                int srcFieldIndex = -1;
                int destFieldIndex = -1;
                string fieldList = "";
                Hashtable hshFields = new Hashtable();
                for (int i = 0; i < pDestTable.Fields.FieldCount; i++)
                {
                    destFieldName = pDestTable.Fields.get_Field(i).Name.ToLower();
                    if (pDestTable.Fields.get_Field(i).Editable == true)
                    {
                        if (destFieldName == "globalid_old")
                        {
                            WriteToLogfile(pDestDS.Name + " has field globalid_old");
                            sourceFieldName = "globalid";
                            srcFieldIndex = pSourceTable.Fields.FindField(sourceFieldName);
                        }
                        else if (destFieldName == "objectid_old")
                        {
                            WriteToLogfile(pDestDS.Name + " has field objectid_old");
                            sourceFieldName = pSourceTable.OIDFieldName;
                            srcFieldIndex = pSourceTable.Fields.FindField(sourceFieldName);
                        }
                        else
                        {
                            sourceFieldName = destFieldName;
                            srcFieldIndex = pSourceTable.Fields.FindField(destFieldName);
                        }

                        destFieldIndex = i;
                        if ((srcFieldIndex != -1) && (destFieldIndex != -1))
                        {
                            int[] indexes = { srcFieldIndex, i };
                            if (hshFields.ContainsKey(destFieldName) == false)
                                hshFields.Add(destFieldName, indexes);

                            if (fieldList == "")
                                fieldList = sourceFieldName;
                            else
                                fieldList = fieldList + "," + sourceFieldName;
                        }
                    }
                }
                IDictionaryEnumerator enumFields = hshFields.
                    GetEnumerator();
                WriteToLogfile("Field count for: " + pDestDS.Name + " : " + hshFields.Count.ToString());

                //Setup the query filter 
                IQueryFilter pQF = null;
                pQF = new QueryFilterClass();
                pQF.SubFields = fieldList;
                if (filter != string.Empty)
                {
                    pQF.WhereClause = filter;
                    if (pQF.WhereClause.Trim() != "")
                        WriteToLogfile("Applied whereclause " + pQF.WhereClause);
                }
                int recCount = 0;
                int newRowCount = 0;
                int editCount = 0;

                //Look for the rows (can use recycle sursor) 
                ICursor pTableCursor = pSourceTable.Search(pQF, true);
                IRow pSourceRow = pTableCursor.NextRow();

                if (pSourceRow != null)
                {
                    //Start an edit operation 
                    pWSE = (IWorkspaceEdit)pDestDS.Workspace;
                    pWSE.StartEditOperation();

                    //Create an insert featurecusor                     
                    ICursor pInsertCursor = pDestTable.Insert(true);
                    IRowBuffer pInsertRB = pDestTable.CreateRowBuffer();

                    while (pSourceRow != null)
                    {
                        //Loop through the features to clip   
                        recCount++;

                        //copy all the attributes 
                        enumFields.Reset();
                        while (enumFields.MoveNext())
                        {
                            destFieldName = enumFields.Key.ToString().
                                ToLower();
                            int[] indexes = (int[])enumFields.Value;
                            srcFieldIndex = indexes[0];
                            destFieldIndex = indexes[1];

                            //Just copy the field value accross 
                            object theValue = pSourceRow.
                                get_Value(srcFieldIndex);
                            pInsertRB.set_Value(destFieldIndex,
                                theValue);

                        }

                        //Insert the new feature 
                        newRowCount++;
                        editCount++;
                        try
                        {
                            pInsertCursor.InsertRow(pInsertRB);
                        }
                        catch
                        {
                            WriteToLogfile("Failed to insert featurebuffer on: " +
                                pDestDS.Name + " oid: " + pSourceRow.OID.ToString());
                            throw new Exception("Unable to insert feature");
                        }

                        //Flush every record 
                        if (newRowCount == 5000)
                        {
                            WriteToLogfile("Written: " + editCount.ToString() + " " + pDestDS.Name + " rows");
                            try
                            { pInsertCursor.Flush(); }
                            catch
                            { throw new Exception("Unable to flush row buffer"); }
                            newRowCount = 0;
                        }
                        pSourceRow = pTableCursor.NextRow();
                    }

                    //Flush if necessary 
                    if (newRowCount != 0)
                        pInsertCursor.Flush();

                    //Stop the edit operation 
                    pWSE.StopEditOperation();

                    //Release the cursor and buffer 
                    Marshal.FinalReleaseComObject(pInsertCursor);
                    Marshal.FinalReleaseComObject(pInsertRB);
                }
                //Release the cursor 
                Marshal.FinalReleaseComObject(pTableCursor);

                //Write message to logfile 
                WriteToLogfile(browseName + " successfully exported: " + recCount.ToString() +
                    " rows to destination geodatabase");
                WriteToLogfile(browseName + " completed in : " + stopWatch.ElapsedMilliseconds.ToString() + " milliseconds");
                stopWatch.Stop();
            }
            catch (Exception ex)
            {
                //Just throw the error 
                WriteToLogfile("An error occurred in: " + "ExportTable: " + ex.Message);
                if (pWSE != null)
                {
                    pWSE.AbortEditOperation();
                }
                throw ex;
            }
        }

        private Hashtable GetRelatedFeatureOIds(
            IFeatureClass pAnnoFC,
            string outputAnnoDSName,
            string sourceFeatureFilter)
        {
            try
            {
                WriteToLogfile("Entering GetRelatedFeatureOIds");
                //applyFilter = true; 
                IEnumRelationshipClass pEnumRel = pAnnoFC.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
                IRelationshipClass pRelCls = pEnumRel.Next();
                IDataset pDS = null;
                string originDSName = "";
                //string defQuery = "";
                ITable pSourceFC = null;
                Hashtable hshOIds = new Hashtable();

                if (pRelCls != null)
                {
                    pDS = (IDataset)pRelCls.OriginClass;
                    pSourceFC = (ITable)pRelCls.OriginClass;
                    originDSName = pDS.Name.ToLower();
                    originDSName = GetShortDatasetName(originDSName);
                    WriteToLogfile("originDSName: " + originDSName);
                }

                //If there is no definition query do not apply the filter 
                if (sourceFeatureFilter == "")
                    return hshOIds;

                WriteToLogfile("Applying whereclause: " + sourceFeatureFilter);
                IQueryFilter pQF = new QueryFilterClass();
                pQF.WhereClause = sourceFeatureFilter;
                pQF.SubFields = pSourceFC.OIDFieldName;
                ICursor pCursor = pSourceFC.Search(pQF, false);
                IRow pRow = pCursor.NextRow();

                //if (isOdd)
                //{
                //    while (pRow != null)
                //    {
                //        if (pRow.OID % 2 == 1)
                //            hshOIds.Add(pRow.OID, 0);
                //        pRow = pCursor.NextRow(); 
                //    }
                //}
                //else if (isEven)
                //{
                //    while (pRow != null)
                //    {
                //        if (pRow.OID % 2 == 0)
                //            hshOIds.Add(pRow.OID, 0);
                //        pRow = pCursor.NextRow();
                //    }
                //}
                //else 
                //{
                while (pRow != null)
                {
                    hshOIds.Add(pRow.OID, 0);
                    pRow = pCursor.NextRow();
                }
                //}

                Marshal.FinalReleaseComObject(pCursor);

                //return the definition query 
                return hshOIds;
            }
            catch (Exception ex)
            {
                WriteToLogfile("An error occurred in: " + "GetRelatedFeatureOIds: " + ex.Message);
                throw new Exception("Error returning the OIds for source annotation featureclass");
            }
        }

        /// <summary>
        /// Exports an Anno feattureclass MUCH faster than ExportFeatureClassAnno
        /// routine but it does not set any attributes just the shape (fastest anno export) 
        /// </summary>
        /// <param name="msgFC"></param>
        /// <param name="filter"></param>
        /// <param name="pSourceFC"></param>
        /// <param name="pDestFC"></param>
        /// <param name="pClipPolygon"></param>
        /// 
        public void ExportAnnoGraphic(
            string msgFC,
            string filter,
            string sourceFeatureFilter,
            string mapGeo,
            bool useClip,
            bool projectFeatureData,
            IFeatureClass pSourceFC,
            IFeatureClass pDestFC,
            IPolygon pClipPolygon)
        {
            ITransactions pTransactions = null;

            try
            {
                WriteToLogfile("Entering ExportAnnoGraphic");
                WriteToLogfile("    filter: " + filter.ToString());
                WriteToLogfile("    useClip: " + useClip.ToString());
                WriteToLogfile("    sourceFeatureFilter: " + sourceFeatureFilter);

                //Setup the stopwatch 
                Stopwatch pStopWatch = new Stopwatch();
                pStopWatch.Start();

                //set up fields for mapping
                IFields pOFlds = pDestFC.Fields;
                IFields pIFlds = pSourceFC.Fields;
                int featureIdFldIdx = pSourceFC.Fields.FindField("featureid");

                //Setup the spatial filter 
                IDataset pDestDS = (IDataset)pDestFC;
                IDataset pSourceDS = (IDataset)pSourceFC;
                ISpatialFilter pSF = new SpatialFilterClass();
                if (useClip)
                {
                    pSF.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                    pSF.Geometry = pClipPolygon;
                }

                //Add additional def query if passed to function 
                if (filter != string.Empty)
                {
                    pSF.WhereClause = filter;
                    WriteToLogfile("Applied whereclause " + pSF.WhereClause);
                }

                bool includeFeature = false;
                bool applyFilter = !(sourceFeatureFilter == string.Empty);
                Hashtable hshOIds = null;
                if (sourceFeatureFilter != string.Empty)
                    hshOIds = GetRelatedFeatureOIds(pSourceFC, pDestDS.Name.ToLower(), sourceFeatureFilter);

                //Start transaction for performance                  
                pTransactions = (ITransactions)pDestDS.Workspace;
                pTransactions.StartTransaction();
                int newFeatureCount = 0;
                int editCount = 0;
                WriteToLogfile("projectFeatureData: " + projectFeatureData.ToString());

                //Search the source anno class 
                IFeatureCursor pICursor = pSourceFC.Search(pSF, true);
                IFeature pSourceFeature = pICursor.NextFeature();
                IFDOGraphicsLayerFactory pGLF = new FDOGraphicsLayerFactoryClass();
                IFDOGraphicsLayer2 pFDOGLayer = (IFDOGraphicsLayer2)pGLF.OpenGraphicsLayer(
                    (IFeatureWorkspace)pDestDS.Workspace,
                    pDestFC.FeatureDataset,
                    pDestDS.Name);
                IClone pAClone = null;
                IAnnotationFeature pAnnoFeature = null;
                IElement pGSElement = null;

                //Add the anno features as FDO elements 
                IElementCollection pElementColl = new ElementCollectionClass();
                pFDOGLayer.BeginAddElements();

                while (pSourceFeature != null)
                {
                    //Determine if we want to include this feature  
                    includeFeature = true;
                    if (applyFilter)
                    {
                        pAnnoFeature = (IAnnotationFeature)pSourceFeature;
                        if (!hshOIds.ContainsKey(pAnnoFeature.LinkedFeatureID))
                            includeFeature = false;
                    }

                    if (includeFeature)
                    {

                        //Determine if we want to include this feature  
                        pAnnoFeature = (IAnnotationFeature)pSourceFeature;
                        newFeatureCount++;
                        editCount++;

                        if (pAnnoFeature.Annotation != null)
                        {
                            pAClone = (IClone)pAnnoFeature.Annotation;
                            pGSElement = (IElement)pAClone.Clone();
                            pGSElement.Geometry.SpatialReference =
                                pAnnoFeature.Annotation.Geometry.SpatialReference;

                            //Project if required                                 
                            //if (projectFeatureData)
                            //    pGSElement.Geometry = Project(
                            //        pGSElement.Geometry,
                            //        _toSpatialRef,
                            //        _transformation);
                            pElementColl.Add(pGSElement, 0);
                        }

                        if (newFeatureCount == 5000)
                        {
                            WriteToLogfile("Written: " + editCount.ToString() + " " + pDestDS.Name + " features");
                            pFDOGLayer.DoAddElements(pElementColl, 0);
                            pElementColl.Clear();
                            pTransactions.CommitTransaction();
                            pTransactions.StartTransaction();
                            newFeatureCount = 0;
                            GC.Collect();
                        }
                    }
                    pSourceFeature = pICursor.NextFeature();
                }
                Marshal.FinalReleaseComObject(pICursor);

                //Commit any left over elements
                if (pElementColl.Count > 0)
                {
                    pFDOGLayer.DoAddElements(pElementColl, 0);
                    pElementColl.Clear();
                }

                //End adding elements and commit 
                pFDOGLayer.EndAddElements();
                pTransactions.CommitTransaction();

                //Write message to logfile 
                WriteToLogfile(pDestDS.BrowseName + " successfully exported: " + editCount.ToString() +
                    " anno features to destination geodatabase");
                WriteToLogfile(pDestDS.BrowseName + " completed in : " + pStopWatch.ElapsedMilliseconds.ToString() + " milliseconds");
                pStopWatch.Stop();
            }
            catch (Exception ex)
            {
                //Just throw the error 
                WriteToLogfile("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message);
                if (pTransactions != null)
                    pTransactions.AbortTransaction();
                throw ex;
            }
        }

        /// <summary>
        /// Exports an Anno feattureclass MUCH faster than ExportFeatureClassAnno
        /// routine but it does not set any attributes just the shape (fastest anno export) 
        /// </summary>
        /// <param name="msgFC"></param>
        /// <param name="filter"></param>
        /// <param name="pSourceFC"></param>
        /// <param name="pDestFC"></param>
        /// <param name="pClipPolygon"></param>
        /// 
        public void ExportAnnoGraphicWithSourceRelCondition(
            string msgFC,
            string filter,
            string sourceFeatureFilter,
            string mapGeo,
            string relatedFC,
            bool mustHaveRel,
            bool useClip,
            bool projectFeatureData,
            IFeatureClass pSourceAnnoFC,
            IFeatureClass pDestAnnoFC,
            IPolygon pClipPolygon)
        {
            ITransactions pTransactions = null;

            try
            {
                WriteToLogfile("Entering ExportAnnoGraphicWithSourceRelCondition");
                WriteToLogfile("    filter: " + filter.ToString());
                WriteToLogfile("    useClip: " + useClip.ToString());
                WriteToLogfile("    sourceFeatureFilter: " + sourceFeatureFilter);
                WriteToLogfile("    relatedFC: " + relatedFC);
                WriteToLogfile("    mustHaveRel: " + mustHaveRel.ToString());

                //Setup the stopwatch 
                Stopwatch pStopWatch = new Stopwatch();
                pStopWatch.Start();

                //Step 1. 
                //Get the source featureclass  
                IEnumRelationshipClass pEnumRel = pSourceAnnoFC.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
                IRelationshipClass pAnnoRelClass = pEnumRel.Next();
                IDataset pDS = null;
                string sourceFCName = "";
                IFeatureClass pSourceFC = null;
                if (pAnnoRelClass != null)
                {
                    pSourceFC = (IFeatureClass)pAnnoRelClass.OriginClass;
                    pDS = (IDataset)pAnnoRelClass.OriginClass;
                    sourceFCName = pDS.Name.ToLower();
                    WriteToLogfile("source featureclass is: " + sourceFCName);
                }
                else
                {
                    WriteToLogfile("Error - unable to find the source featureclass!");
                    return;
                }

                //Step 2. 
                //Search the source featureclass using the 
                //sourceFeatureFilter and spatial filter and 
                //store in an ISet - call this: SetA 
                Hashtable hshSetA_OIds = new Hashtable();
                ISet pSetA = new SetClass();
                IDataset pSourceDS = (IDataset)pSourceFC;
                ISpatialFilter pSF = new SpatialFilterClass();
                if (useClip)
                {
                    pSF.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                    pSF.Geometry = pClipPolygon;
                }
                if (sourceFeatureFilter != string.Empty)
                {
                    pSF.WhereClause = sourceFeatureFilter;
                    WriteToLogfile("Applied whereclause " + pSF.WhereClause);
                }
                IFeatureCursor pSourceFCursor = pSourceFC.Search((IQueryFilter)pSF, false);
                IFeature pSourceFeature = pSourceFCursor.NextFeature();
                while (pSourceFeature != null)
                {
                    if (!hshSetA_OIds.ContainsKey(pSourceFeature.OID))
                    {
                        hshSetA_OIds.Add(pSourceFeature.OID, 0);
                        pSetA.Add(pSourceFeature);
                    }
                    pSourceFeature = pSourceFCursor.NextFeature();
                }
                Marshal.FinalReleaseComObject(pSourceFCursor);
                WriteToLogfile("SetA count: " + hshSetA_OIds.Count.ToString());
                if (hshSetA_OIds.Count == 0)
                {
                    WriteToLogfile("Exiting extraction for dataset since no features in SetA");
                    return;
                }

                //Step 3 
                //Get the relationship class to the relatedFC
                //(to the vault for example)
                IRelationshipClass pTargetRelClass = null;
                pEnumRel = pSourceFC.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
                IRelationshipClass pRelCls = pEnumRel.Next();
                while (pRelCls != null)
                {
                    if ((((IDataset)pRelCls.OriginClass).Name.ToLower() == relatedFC.ToLower()) ||
                        (((IDataset)pRelCls.DestinationClass).Name.ToLower() == relatedFC.ToLower()))
                    {
                        //this is the relationshipclass 
                        pTargetRelClass = pRelCls;
                        break;
                    }
                    pRelCls = pEnumRel.Next();
                }
                if (pTargetRelClass != null)
                {
                    pDS = (IDataset)pTargetRelClass;
                    WriteToLogfile("target relationship class is: " + pDS.Name.ToLower());
                }
                else
                {
                    WriteToLogfile("Error - unable to find the target relationship class!");
                    return;
                }

                //Step 4 
                //Use GetObjectsMatchingObjectSet to get the matching 
                //subsurfacestructures and vaults (with a relationship) 
                //call this: SetB 
                ISet pSetC = new SetClass();
                Hashtable hshSetC_OIds = new Hashtable();
                Hashtable hshSetB_OIds = new Hashtable();
                pSetA.Reset();
                IRelClassEnumRowPairs pRowPairs = pTargetRelClass.GetObjectsMatchingObjectSet(pSetA);

                //Step 5. 
                bool destRowIsTarget = false;
                IRow pMatchSourceRow = null;
                IRow pMatchDestRow = null;
                IRow pMatchTargetRow = null;
                int counter = 0;
                pRowPairs.Reset();
                pRowPairs.Next(out pMatchSourceRow, out pMatchDestRow);
                while ((pMatchDestRow != null) && (pMatchSourceRow != null))
                {

                    if (counter == 0)
                    {
                        if (((IDataset)pMatchDestRow.Table).Name == ((IDataset)pSourceFC).Name)
                        {
                            destRowIsTarget = true;
                        }
                        else if (((IDataset)pMatchSourceRow.Table).Name == ((IDataset)pSourceFC).Name)
                        {
                            destRowIsTarget = false;
                        }
                        else
                        {
                            throw new Exception("Error with relationshipclass");
                        }
                    }
                    counter++;

                    if (destRowIsTarget)
                        pMatchTargetRow = pMatchDestRow;
                    else
                        pMatchTargetRow = pMatchSourceRow;

                    if (!hshSetB_OIds.ContainsKey(pMatchTargetRow.OID))
                        hshSetB_OIds.Add(pMatchTargetRow.OID, 0);
                    if (mustHaveRel)
                    {
                        if (!hshSetC_OIds.ContainsKey(pMatchTargetRow.OID))
                        {
                            hshSetC_OIds.Add(pMatchTargetRow.OID, 0);
                            pSetC.Add(pMatchTargetRow);
                        }
                    }
                    pRowPairs.Next(out pMatchSourceRow, out pMatchDestRow);
                }
                WriteToLogfile("SetB count: " + hshSetB_OIds.Count.ToString());

                //Where mustHaveRel = false we want SetA - SetB 
                pSetA.Reset();
                IFeature pFeature = (IFeature)pSetA.Next();
                if (!mustHaveRel)
                {
                    while (pFeature != null)
                    {
                        if (!hshSetB_OIds.ContainsKey(pFeature.OID))
                        {
                            if (!hshSetC_OIds.ContainsKey(pFeature.OID))
                            {
                                hshSetC_OIds.Add(pFeature.OID, 0);
                                pSetC.Add(pFeature);
                            }
                        }
                        pFeature = (IFeature)pSetA.Next();
                    }
                }
                WriteToLogfile("SetC count: " + hshSetC_OIds.Count.ToString());


                //Step 6. Take SetC and use GetObjectsMatchingObjectSet against 
                //the anno relationshipclass to get the related source anno 
                //features
                ISet pSetD = new SetClass();
                Hashtable hshSetD_OIds = new Hashtable();
                if (hshSetC_OIds.Count != 0)
                {
                    pRowPairs = pAnnoRelClass.GetObjectsMatchingObjectSet(pSetC);
                    pRowPairs.Reset();
                    pRowPairs.Next(out pMatchSourceRow, out pMatchDestRow);
                    while ((pMatchDestRow != null) && (pMatchSourceRow != null))
                    {
                        //The anno is always the destination class 
                        pMatchTargetRow = pMatchDestRow;
                        //Debug.Print(((IDataset)pMatchTargetRow.Table).Name); 

                        if (!hshSetD_OIds.ContainsKey(pMatchTargetRow.OID))
                        {
                            hshSetD_OIds.Add(pMatchTargetRow.OID, 0);
                            pSetD.Add(pMatchTargetRow);
                        }

                        pRowPairs.Next(out pMatchSourceRow, out pMatchDestRow);
                    }
                }
                Marshal.FinalReleaseComObject(pSetA);
                Marshal.FinalReleaseComObject(pSetC);
                WriteToLogfile("SetD count: " + hshSetD_OIds.Count.ToString());
                if (hshSetD_OIds.Count == 0)
                {
                    WriteToLogfile("Exiting extraction for dataset since no features in SetD");
                    return;
                }

                //Step 7. Load the data 
                WriteToLogfile("Loading the data...");

                //set up fields for mapping
                IFields pOFlds = pDestAnnoFC.Fields;
                IFields pIFlds = pSourceAnnoFC.Fields;
                int featureIdFldIdx = pSourceAnnoFC.Fields.FindField("featureid");
                bool includeFeature = false;
                bool applyFilter = !(filter == string.Empty);
                Hashtable hshClasses = new Hashtable();
                if (applyFilter)
                {
                    //parse out the filter as a list of possible classes 
                    int posOfIn = filter.ToUpper().IndexOf("IN(");
                    int posOfEquals = filter.ToUpper().IndexOf("=");
                    int classInt = -1;
                    string classesString = string.Empty;
                    if (posOfIn != -1)
                        classesString = filter.Substring(posOfIn + 3, filter.Length - (posOfIn + 4));
                    else if (posOfEquals != -1)
                        classesString = filter.Substring(posOfEquals + 1, filter.Length - (posOfEquals + 1));
                    else
                    {
                        throw new Exception("Unsupported definition query");
                    }
                    string[] classes = classesString.Split(",".ToCharArray());
                    foreach (string classString in classes)
                    {
                        if (int.TryParse(classString, out classInt))
                        {
                            if (!hshClasses.ContainsKey(classInt))
                                hshClasses.Add(classInt, 0);
                        }
                    }
                }
                IDataset pDestDS = (IDataset)pDestAnnoFC;
                pSetD.Reset();
                IFeature pSourceAnnoFeature = (IFeature)pSetD.Next();

                //Start transaction for performance                  
                pTransactions = (ITransactions)(((IDataset)pDestAnnoFC).Workspace);
                pTransactions.StartTransaction();
                int newFeatureCount = 0;
                int editCount = 0;
                int fldIdx = -1;
                WriteToLogfile("projectFeatureData: " + projectFeatureData.ToString());

                //Search the source anno class 
                IFDOGraphicsLayerFactory pGLF = new FDOGraphicsLayerFactoryClass();
                IFDOGraphicsLayer2 pFDOGLayer = (IFDOGraphicsLayer2)pGLF.OpenGraphicsLayer(
                    (IFeatureWorkspace)pDestDS.Workspace,
                    pDestAnnoFC.FeatureDataset,
                    pDestDS.Name);
                IClone pAClone = null;
                IAnnotationFeature pAnnoFeature = null;
                IElement pGSElement = null;
                int classId = -1;

                //Add the anno features as FDO elements 
                IElementCollection pElementColl = new ElementCollectionClass();
                pFDOGLayer.BeginAddElements();

                while (pSourceAnnoFeature != null)
                {
                    //Determine if we want to include this feature  
                    includeFeature = true;
                    if (applyFilter)
                    {
                        //Filter according to which classes we are displaying 
                        includeFeature = false;
                        if (fldIdx == -1)
                            fldIdx = pSourceAnnoFeature.Fields.FindField("annotationclassid");
                        if (pSourceAnnoFeature.get_Value(fldIdx) != DBNull.Value)
                        {
                            classId = Convert.ToInt32(pSourceAnnoFeature.get_Value(fldIdx));
                            if (hshClasses.ContainsKey(classId))
                                includeFeature = true;
                        }
                    }

                    if (includeFeature)
                    {
                        pAnnoFeature = (IAnnotationFeature)pSourceAnnoFeature;
                        newFeatureCount++;
                        editCount++;

                        if (pAnnoFeature.Annotation != null)
                        {
                            pAClone = (IClone)pAnnoFeature.Annotation;
                            pGSElement = (IElement)pAClone.Clone();
                            pGSElement.Geometry.SpatialReference =
                                pAnnoFeature.Annotation.Geometry.SpatialReference;

                            //Project if required                                 
                            //if (projectFeatureData)
                            //    pGSElement.Geometry = Project(
                            //        pGSElement.Geometry,
                            //        _toSpatialRef,
                            //        _transformation);
                            pElementColl.Add(pGSElement, 0);
                        }

                        if (newFeatureCount == 5000)
                        {
                            WriteToLogfile("Written: " + editCount.ToString() + " " + pDestDS.Name + " features");
                            pFDOGLayer.DoAddElements(pElementColl, 0);
                            pElementColl.Clear();
                            pTransactions.CommitTransaction();
                            pTransactions.StartTransaction();
                            newFeatureCount = 0;
                            GC.Collect();
                        }
                    }
                    pSourceAnnoFeature = (IFeature)pSetD.Next();
                }
                Marshal.FinalReleaseComObject(pSetD);

                //Commit any left over elements
                if (pElementColl.Count > 0)
                {
                    pFDOGLayer.DoAddElements(pElementColl, 0);
                    pElementColl.Clear();
                }

                //End adding elements and commit 
                pFDOGLayer.EndAddElements();
                pTransactions.CommitTransaction();

                //Write message to logfile 
                WriteToLogfile(pDestDS.BrowseName + " successfully exported: " + editCount.ToString() +
                    " anno features to destination geodatabase");
                WriteToLogfile(pDestDS.BrowseName + " completed in : " + pStopWatch.ElapsedMilliseconds.ToString() + " milliseconds");
                pStopWatch.Stop();
            }
            catch (Exception ex)
            {
                //Just throw the error 
                WriteToLogfile("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message);
                if (pTransactions != null)
                    pTransactions.AbortTransaction();
                throw ex;
            }
        }


        /// <summary>
        /// Function to remove the owner name from the fully qualified 
        /// dataset name 
        /// </summary>
        /// <param name="datasetName"></param>
        /// <returns>The dataset name without the owner prefix</returns>
        public string GetShortDatasetName(string datasetName)
        {
            try
            {
                string shortDatasetName = "";
                int posOfLastPeriod = -1;

                posOfLastPeriod = datasetName.LastIndexOf(".");
                if (posOfLastPeriod != -1)
                {
                    shortDatasetName = datasetName.Substring(
                        posOfLastPeriod + 1, (datasetName.Length - posOfLastPeriod) - 1);
                }
                else
                {
                    shortDatasetName = datasetName;
                }

                return shortDatasetName;
            }
            catch (Exception ex)
            {
                throw new Exception("Error returning the shortened dataset name");
            }
        }

        public void ExportFeatureClassLO(
            string msgFC,
            bool useClip,
            bool projectFeatureData,
            IFeatureClass pSourceFC,
            IFeatureClass pDestFC,
            IPolygon pClipPolygon,
            string filter)
        {

            IWorkspaceEdit pWSE = null;
            IFeatureClassLoad featureClassLoad = null;

            try
            {
                WriteToLogfile("Entering ExportFeatureClassLO");
                WriteToLogfile("    filter: " + filter.ToString());
                WriteToLogfile("    useClip: " + useClip.ToString());

                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                IDataset pSrcDS = (IDataset)pSourceFC;
                IDataset pDestDS = (IDataset)pDestFC;
                string browseName = pDestDS.BrowseName;
                WriteToLogfile(msgFC);

                //Get the list of required fields 
                string destFieldName = "";
                string sourceFieldName = "";
                int srcFieldIndex = -1;
                int destFieldIndex = -1;
                string fieldList = "";
                Hashtable hshFields = new Hashtable();
                for (int i = 0; i < pDestFC.Fields.FieldCount; i++)
                {
                    destFieldName = pDestFC.Fields.get_Field(i).Name.ToLower();
                    if (pDestFC.Fields.get_Field(i).Editable == true)
                    {
                        if (destFieldName == "globalid_old")
                        {
                            WriteToLogfile(pDestDS.Name + " has field globalid_old");
                            sourceFieldName = "globalid";
                            srcFieldIndex = pSourceFC.Fields.FindField(sourceFieldName);
                        }
                        else if (destFieldName == "objectid_old")
                        {
                            WriteToLogfile(pDestDS.Name + " has field objectid_old");
                            sourceFieldName = pSourceFC.OIDFieldName;
                            srcFieldIndex = pSourceFC.Fields.FindField(sourceFieldName);
                        }
                        else
                        {
                            sourceFieldName = destFieldName;
                            srcFieldIndex = pSourceFC.Fields.FindField(destFieldName);
                        }

                        destFieldIndex = i;
                        if ((srcFieldIndex != -1) && (destFieldIndex != -1))
                        {
                            int[] indexes = { srcFieldIndex, i };
                            if (hshFields.ContainsKey(destFieldName) == false)
                                hshFields.Add(destFieldName, indexes);

                            if (fieldList == "")
                                fieldList = sourceFieldName;
                            else
                                fieldList = fieldList + "," + sourceFieldName;
                        }
                    }
                }
                IDictionaryEnumerator enumFields = hshFields.
                    GetEnumerator();
                WriteToLogfile("Field count for: " + pDestDS.Name + " : " + hshFields.Count.ToString());

                //Setup the spatial filter 
                ITopologicalOperator pTopo =
                    (ITopologicalOperator)pClipPolygon;
                ISpatialFilter pSF = new SpatialFilterClass();
                pSF.SubFields = fieldList;
                if (filter != string.Empty)
                {
                    pSF.WhereClause = filter;
                    if (pSF.WhereClause.Trim() != "")
                        WriteToLogfile("Applied whereclause " + pSF.WhereClause);
                }
                if (useClip)
                {
                    pSF.SpatialRel = esriSpatialRelEnum.
                    esriSpatialRelIntersects;
                    pSF.Geometry = pClipPolygon;
                }
                int recCount = 0;
                int newFeatureCount = 0;
                int editCount = 0;
                //bool projectFeatureData = false;
                //if (_mapProdVersion == 1)
                //    projectFeatureData = true;
                int srcShapeFldIdx = pSourceFC.Fields.FindField(pSourceFC.ShapeFieldName);
                WriteToLogfile("projectFeatureData: " + projectFeatureData.ToString());

                //Look for the features (can use recycle cursor) 
                IFeatureCursor pFCursor = pSourceFC.Search(pSF, true);
                IFeature pSourceFeature = pFCursor.NextFeature();

                if (pSourceFeature != null)
                {
                    //Start an edit operation 
                    pWSE = (IWorkspaceEdit)pDestDS.Workspace;
                    pWSE.StartEditOperation();

                    // Cast the feature class to the IFeatureClassLoad interface.
                    featureClassLoad = (IFeatureClassLoad)pDestFC;

                    // Enable load-only mode on the feature class (much faster) 
                    featureClassLoad.LoadOnlyMode = true;

                    //Create an insert featurecusor                     
                    IFeatureCursor pInsertCursor = pDestFC.Insert(true);
                    IFeatureBuffer pInsertFB = pDestFC.CreateFeatureBuffer();

                    while (pSourceFeature != null)
                    {
                        //Loop through the features to clip   
                        recCount++;

                        //copy all the attributes 
                        enumFields.Reset();
                        while (enumFields.MoveNext())
                        {
                            destFieldName = enumFields.Key.ToString().
                                ToLower();
                            int[] indexes = (int[])enumFields.Value;
                            srcFieldIndex = indexes[0];
                            destFieldIndex = indexes[1];

                            if ((srcFieldIndex == -1) || (destFieldIndex == -1))
                                Debug.Print("found problem");

                            if (srcShapeFldIdx == srcFieldIndex)
                            {
                                ////Copy the shape - projecting if required                                 
                                //if (projectFeatureData)
                                //    pInsertFB.Shape = Project(
                                //        pSourceFeature.ShapeCopy,
                                //        _toSpatialRef,
                                //        _transformation);
                                //else
                                    pInsertFB.Shape = pSourceFeature.ShapeCopy;

                            }
                            else
                            {
                                //Just copy the field value accross 
                                object theValue = pSourceFeature.
                                    get_Value(srcFieldIndex);
                                pInsertFB.set_Value(destFieldIndex,
                                    theValue);
                            }
                        }

                        //Insert the new feature 
                        newFeatureCount++;
                        editCount++;
                        try
                        {
                            pInsertCursor.InsertFeature(pInsertFB);
                        }
                        catch (Exception ex)
                        {
                            WriteToLogfile("Failed to insert featurebuffer on: " +
                                pDestDS.Name + " oid: " + pSourceFeature.OID.ToString() +
                                " error: " + ex.Message);
                            throw new Exception("Unable to insert feature");
                        }

                        //Flush every record 
                        if (newFeatureCount == 5000)
                        {
                            WriteToLogfile("Written: " + editCount.ToString() + " " + pDestDS.Name + " features");
                            try
                            { pInsertCursor.Flush(); }
                            catch
                            { throw new Exception("Unable to flush feature buffer"); }
                            newFeatureCount = 0;
                        }
                        pSourceFeature = pFCursor.NextFeature();
                    }

                    //Flush if necessary 
                    if (newFeatureCount != 0)
                        pInsertCursor.Flush();

                    //Stop the edit operation 
                    pWSE.StopEditOperation();

                    //Release the cursor and buffer 
                    Marshal.FinalReleaseComObject(pInsertCursor);
                    Marshal.FinalReleaseComObject(pInsertFB);
                }
                //Release the cursor 
                Marshal.FinalReleaseComObject(pFCursor);

                //Write message to logfile 
                WriteToLogfile(browseName + " successfully exported: " + recCount.ToString() +
                    " features to destination geodatabase");
                WriteToLogfile(browseName + " completed in : " + stopWatch.ElapsedMilliseconds.ToString() + " milliseconds");
                stopWatch.Stop();
            }
            catch (Exception ex)
            {
                //Just throw the error 
                WriteToLogfile("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message);
                if (pWSE != null)
                {
                    pWSE.AbortEditOperation();
                }
                throw ex;
            }
            finally
            {
                // Disable load-only mode on the feature class.
                if (featureClassLoad != null)
                    featureClassLoad.LoadOnlyMode = false;
            }
        }

        public void ExportMPLabels(
            IFeatureClass pSourceFC,
            IFeatureClass pDestFC)
        {

            IWorkspaceEdit pWSE = null;
            IFeatureClassLoad featureClassLoad = null;

            try
            {
                WriteToLogfile("Entering ExportMPLabels");

                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                IDataset pSrcDS = (IDataset)pSourceFC;
                IDataset pDestDS = (IDataset)pDestFC;
                string browseName = pDestDS.BrowseName;
                IArea pArea = null;

                //Get the list of required fields 
                string destFieldName = "";
                string sourceFieldName = "";
                int srcFieldIndex = -1;
                int destFieldIndex = -1;
                string fieldList = "";
                Hashtable hshFields = new Hashtable();
                for (int i = 0; i < pDestFC.Fields.FieldCount; i++)
                {
                    destFieldName = pDestFC.Fields.get_Field(i).Name.ToLower();
                    if (pDestFC.Fields.get_Field(i).Editable == true)
                    {
                        if (destFieldName == "globalid_old")
                        {
                            WriteToLogfile(pDestDS.Name + " has field globalid_old");
                            sourceFieldName = "globalid";
                            srcFieldIndex = pSourceFC.Fields.FindField(sourceFieldName);
                        }
                        else if (destFieldName == "objectid_old")
                        {
                            WriteToLogfile(pDestDS.Name + " has field objectid_old");
                            sourceFieldName = pSourceFC.OIDFieldName;
                            srcFieldIndex = pSourceFC.Fields.FindField(sourceFieldName);
                        }
                        else
                        {
                            sourceFieldName = destFieldName;
                            srcFieldIndex = pSourceFC.Fields.FindField(destFieldName);
                        }

                        destFieldIndex = i;
                        if ((srcFieldIndex != -1) && (destFieldIndex != -1))
                        {
                            int[] indexes = { srcFieldIndex, i };
                            if (hshFields.ContainsKey(destFieldName) == false)
                                hshFields.Add(destFieldName, indexes);

                            if (fieldList == "")
                                fieldList = sourceFieldName;
                            else
                                fieldList = fieldList + "," + sourceFieldName;
                        }
                    }
                }
                IDictionaryEnumerator enumFields = hshFields.
                    GetEnumerator();
                WriteToLogfile("Field count for: " + pDestDS.Name + " : " + hshFields.Count.ToString());

                //Setup the spatial filter 
                ISpatialFilter pSF = new SpatialFilterClass();
                pSF.SubFields = fieldList;
                int recCount = 0;
                int newFeatureCount = 0;
                int editCount = 0;
                int srcShapeFldIdx = pSourceFC.Fields.FindField(pSourceFC.ShapeFieldName);

                //Look for the features (can use recycle cursor) 
                IFeatureCursor pFCursor = pSourceFC.Search(null, false);
                IFeature pSourceFeature = pFCursor.NextFeature();

                if (pSourceFeature != null)
                {
                    //Start an edit operation 
                    pWSE = (IWorkspaceEdit)pDestDS.Workspace;
                    pWSE.StartEditOperation();

                    // Cast the feature class to the IFeatureClassLoad interface.
                    featureClassLoad = (IFeatureClassLoad)pDestFC;

                    // Enable load-only mode on the feature class (much faster) 
                    featureClassLoad.LoadOnlyMode = true;

                    //Create an insert featurecusor                     
                    IFeatureCursor pInsertCursor = pDestFC.Insert(true);
                    IFeatureBuffer pInsertFB = pDestFC.CreateFeatureBuffer();

                    while (pSourceFeature != null)
                    {
                        //Loop through the features to clip   
                        recCount++;

                        //copy all the attributes 
                        enumFields.Reset();
                        while (enumFields.MoveNext())
                        {
                            destFieldName = enumFields.Key.ToString().
                                ToLower();
                            int[] indexes = (int[])enumFields.Value;
                            srcFieldIndex = indexes[0];
                            destFieldIndex = indexes[1];

                            if ((srcFieldIndex == -1) || (destFieldIndex == -1))
                                Debug.Print("found problem");

                            if (srcShapeFldIdx == srcFieldIndex)
                            {
                                //Use the centroid for the label 
                                pArea = (IArea)pSourceFeature.ShapeCopy;
                                pInsertFB.Shape = pSourceFeature.ShapeCopy; 
                                //pInsertFB.Shape = Project(
                                //        pArea.Centroid,
                                //        _toSpatialRef,
                                //        _transformation);
                            }
                            else
                            {
                                //Just copy the field value accross 
                                object theValue = pSourceFeature.
                                    get_Value(srcFieldIndex);
                                pInsertFB.set_Value(destFieldIndex,
                                    theValue);
                            }
                        }

                        //Insert the new feature 
                        newFeatureCount++;
                        editCount++;
                        try
                        {
                            pInsertCursor.InsertFeature(pInsertFB);
                        }
                        catch (Exception ex)
                        {
                            WriteToLogfile("Failed to insert featurebuffer on: " +
                                pDestDS.Name + " oid: " + pSourceFeature.OID.ToString() +
                                " error: " + ex.Message);
                            throw new Exception("Unable to insert feature");
                        }

                        //Flush every record 
                        if (newFeatureCount == 5000)
                        {
                            WriteToLogfile("Written: " + editCount.ToString() + " " + pDestDS.Name + " features");
                            try
                            { pInsertCursor.Flush(); }
                            catch
                            { throw new Exception("Unable to flush feature buffer"); }
                            newFeatureCount = 0;
                        }
                        pSourceFeature = pFCursor.NextFeature();
                    }

                    //Flush if necessary 
                    if (newFeatureCount != 0)
                        pInsertCursor.Flush();

                    //Stop the edit operation 
                    pWSE.StopEditOperation();

                    //Release the cursor and buffer 
                    Marshal.FinalReleaseComObject(pInsertCursor);
                    Marshal.FinalReleaseComObject(pInsertFB);
                }
                //Release the cursor 
                Marshal.FinalReleaseComObject(pFCursor);

                //Write message to logfile 
                WriteToLogfile(browseName + " successfully exported: " + recCount.ToString() +
                    " features to destination geodatabase");
                WriteToLogfile(browseName + " completed in : " + stopWatch.ElapsedMilliseconds.ToString() + " milliseconds");
                stopWatch.Stop();
            }
            catch (Exception ex)
            {
                //Just throw the error 
                WriteToLogfile("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message);
                if (pWSE != null)
                {
                    pWSE.AbortEditOperation();
                }
                throw ex;
            }
            finally
            {
                // Disable load-only mode on the feature class.
                if (featureClassLoad != null)
                    featureClassLoad.LoadOnlyMode = false;
            }
        }


        public IGeometry Project(IGeometry geometryToProject, ISpatialReference toSpatialReference, ITransformation transformation)
        {
            try
            {
                if (geometryToProject == null)
                    return null;
                IGeometry2 geom2 = (IGeometry2)geometryToProject;
                geom2.ProjectEx(toSpatialReference, esriTransformDirection.esriTransformReverse, transformation as IGeoTransformation, false, 0, 0);
                return (IGeometry)geom2;
            }
            catch (Exception ex)
            {
                throw new Exception("Error projecting shape");
            }
        }


        //********************************************************************

        //    public void SynchFileGeodatabase(
        //IWorkspace pSourceWS,
        //IWorkspace pDestWS)
        //    {
        //        IWorkspaceEdit pWSE = null;
        //        Hashtable hshClassesExported = new Hashtable();

        //        try
        //        {
        //            //Set the mouse cursor 
        //            //pMouseCursor.SetCursor(2);
        //            Stopwatch stopWatch = new Stopwatch();
        //            stopWatch.Start();

        //            //Have to get the daily diff version 
        //            WriteToLogfile("Getting version: SDE.Daily_Change_Detection_Sync");
        //            IVersion pDailyDiffVersion = GetVersionByName(
        //                (IVersionedWorkspace)pSourceWS, "SDE.Daily_Change_Detection_Sync");

        //            //Get a full list of featureclasses
        //            WriteToLogfile("Getting list of featureclasses");
        //            Hashtable hshAllFCs = new Hashtable();

        //            //First determine which classes are anno classes and which are 
        //            //normal featureclasses
        //            IFeatureWorkspace pSourceFWS = (IFeatureWorkspace)pSourceWS;
        //            foreach (string fc in hshAllFCs.Keys)
        //            {
        //                IFeatureClass pSourceFC = pSourceFWS.OpenFeatureClass(fc);
        //                if (pSourceFC.FeatureType != ESRI.ArcGIS.Geodatabase.esriFeatureType.esriFTAnnotation)
        //                {
        //                    if (!hshAllFCs.ContainsValue(fc.ToLower()))
        //                        hshAllFCs.Add(hshAllFCs.Count + 1, fc.ToLower());
        //                }
        //            }

        //            //Add the anno featureclasses next 
        //            foreach (string fc in hshAllFCs.Keys)
        //            {
        //                if (!hshAllFCs.ContainsValue(fc.ToLower()))
        //                    hshAllFCs.Add(hshAllFCs.Count + 1, fc.ToLower());
        //            }

        //            //Get the valid construction statuses 
        //            //*** these should be looked up from config file 
        //            Hashtable hshValidConStatuses = new Hashtable();
        //            hshValidConStatuses.Add(1, 0);
        //            hshValidConStatuses.Add(2, 0);
        //            hshValidConStatuses.Add(3, 0);
        //            hshValidConStatuses.Add(5, 0);
        //            hshValidConStatuses.Add(30, 0);

        //            //Get the version differences 
        //            SynchVersionDifferences(
        //                    (IVersionedWorkspace)pDailyDiffVersion,
        //                    pDestWS,
        //                    SynchType.SynchTypeUpdate,
        //                    hshAllFCs,
        //                    hshValidConStatuses);

        //            //Stop editing 
        //            //pWSE.StopEditing(true);
        //            WriteToLogfile("File Geodatabase synch completed successfully");
        //            WriteToLogfile("Finished in: " + stopWatch.ElapsedMilliseconds + " milliseconds");
        //            stopWatch.Stop();

        //        }
        //        catch (Exception ex)
        //        {
        //            //pStatusBar.HideProgressBar();
        //            if (pWSE != null)
        //            {
        //                pWSE.StopEditing(false);
        //            }
        //            WriteToLogfile("An error occurred in ExportGeodatabase_GOLD " +
        //                "edits have been aborted. Error message: " +
        //                ex.Message);
        //        }
        //    }

        private IVersion GetVersionByName(IVersionedWorkspace pVWS, string targetVersionName)
        {
            try
            {
                IVersion pTargetVersion = pVWS.FindVersion(targetVersionName);
                return pTargetVersion;
            }
            catch
            {
                WriteToLogfile("Error returning version: " + targetVersionName);
                throw new Exception("Error returning version: " + targetVersionName);
            }
        }

        //    public void SynchVersionDifferences(
        //        IVersionedWorkspace pVWS,
        //        IWorkspace pDestWS,
        //        SynchType pSynchType,
        //        Hashtable hshAllFCs,
        //        Hashtable hshValidConStatuses)
        //    {

        //        IWorkspaceEdit pWSE = (IWorkspaceEdit)pDestWS;

        //        try
        //        {
        //            //Get the edit version 
        //            //sProgressor.Message = "Performing Version Difference";
        //            IVersion pDefaultVersion = pVWS.DefaultVersion;
        //            IVersion pDailySyncVersion = (IVersion)pVWS;
        //            Hashtable hshUpdatedObjects = new Hashtable();
        //            Hashtable hshInsertedObjects = new Hashtable();
        //            Hashtable hshDeletedObjects = new Hashtable();
        //            esriDifferenceType[] pDiffTypes;
        //            IFeatureWorkspace pSourceFWS = (IFeatureWorkspace)pVWS;
        //            IFeatureWorkspace pDestFWS = (IFeatureWorkspace)pDestWS;
        //            IFeatureClass pSourceFC = null;
        //            IFeatureClass pDestFC = null;
        //            string className = "";

        //            if ((pDefaultVersion != null) && (pDailySyncVersion != null))
        //            {
        //                //Find the common ancestor version 
        //                IVersion2 pDailySyncVersion2 = (IVersion2)pDailySyncVersion;
        //                IVersion pCommonAncestorVersion = pDailySyncVersion2.
        //                    GetCommonAncestor(pDefaultVersion);

        //                //Find the featureclasses edited in the version 
        //                Hashtable hshEditedClasses = GetClassesEditedInVersion(
        //                    pDailySyncVersion, pDefaultVersion);
        //                WriteToLogfile("Found: " + hshEditedClasses.Count.ToString() + " edited classes...");

        //                //Set up step progressor
        //                if (hshEditedClasses.Count != 0)
        //                {
        //                    //Start Editing 
        //                    pWSE.StartEditing(false);

        //                    //Loop through each of the modified classes
        //                    int fcCounter = 0;

        //                    for (int i = 1; i <= hshAllFCs.Count; i++)
        //                    {

        //                        fcCounter++;
        //                        className = ((string)hshAllFCs[i]).ToUpper();

        //                        //Check if the featureclass is in the map prod list 
        //                        if (hshEditedClasses.ContainsKey(className))
        //                        {
        //                            WriteToLogfile("======================================================");
        //                            WriteToLogfile("Synchronizing changes for class: " +
        //                                fcCounter.ToString() + " of: " + hshEditedClasses.Count.ToString() +
        //                                " : " + className);

        //                            //Clear the edited lists 
        //                            hshInsertedObjects.Clear();
        //                            hshUpdatedObjects.Clear();
        //                            hshDeletedObjects.Clear();

        //                            //Open the source, dest featureclass 
        //                            pSourceFC = pSourceFWS.OpenFeatureClass(className);
        //                            pDestFC = pDestFWS.OpenFeatureClass(GetShortDatasetName(className));

        //                            //Process the updates 
        //                            WriteToLogfile("Processing updates...");
        //                            pDiffTypes = new esriDifferenceType[1];
        //                            pDiffTypes[0] = esriDifferenceType.esriDifferenceTypeUpdateNoChange;
        //                            FindVersionDifferences(
        //                                pDefaultVersion,
        //                                pDailySyncVersion,
        //                                pCommonAncestorVersion,
        //                                className,
        //                                ref hshUpdatedObjects,
        //                                pDiffTypes);
        //                            WriteToLogfile("Found: " + hshUpdatedObjects.Count.ToString() + " updates...");
        //                            ProcessUpdates(
        //                                hshUpdatedObjects,
        //                                hshValidConStatuses,
        //                                ref hshInsertedObjects,
        //                                ref hshDeletedObjects,
        //                                pDestWS,
        //                                pSourceFC,
        //                                pDestFC);

        //                            //Get the version differences - inserts 
        //                            WriteToLogfile("Processing inserts...");
        //                            pDiffTypes = new esriDifferenceType[1];
        //                            pDiffTypes[0] = esriDifferenceType.esriDifferenceTypeInsert;
        //                            FindVersionDifferences(
        //                                pDefaultVersion,
        //                                pDailySyncVersion,
        //                                pCommonAncestorVersion,
        //                                className,
        //                                ref hshInsertedObjects,
        //                                pDiffTypes);
        //                            WriteToLogfile("Found: " + hshInsertedObjects.Count.ToString() + " inserts...");
        //                            ProcessInserts(
        //                                hshInsertedObjects,
        //                                pDestWS,
        //                                pSourceFC,
        //                                pDestFC);

        //                            //Process the deletes 
        //                            WriteToLogfile("Found: " + hshDeletedObjects.Count.ToString() + " deletes...");
        //                            WriteToLogfile("Processing deletes...");
        //                            pDiffTypes = new esriDifferenceType[1];
        //                            pDiffTypes[0] = esriDifferenceType.esriDifferenceTypeDeleteNoChange;
        //                            FindVersionDifferences(
        //                                pDefaultVersion,
        //                                pDailySyncVersion,
        //                                pCommonAncestorVersion,
        //                                className,
        //                                ref hshDeletedObjects,
        //                                pDiffTypes);
        //                            ProcessDeletes(
        //                                hshDeletedObjects,
        //                                pDestWS,
        //                                pSourceFC,
        //                                pDestFC);
        //                        }
        //                    }

        //                    //Stop Editing saving edits 
        //                    pWSE.StopEditing(true);

        //                }
        //            }
        //            else
        //            {
        //                WriteToLogfile("Error occurred in SynchVersionDifferences unable to find synch version");
        //                throw new Exception("Error returning version difference - version not found");
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            if (pWSE != null)
        //                pWSE.StopEditing(false);
        //            WriteToLogfile("Error occurred in SynchVersionDifferences: " + ex.Message);
        //            throw new Exception("Error returning version differences");
        //        }
        //    }

        //    public void FindVersionDifferences(
        //        IVersion pDefaultVersion,
        //        IVersion pDailySyncVersion,
        //        IVersion pCommonAncestorVersion,
        //        string tableName,
        //        ref Hashtable hshDiffObjects,
        //        esriDifferenceType[] pDiffTypes)
        //    {
        //        try
        //        {
        //            WriteToLogfile("Entering FindVersionDifferences " + DateTime.Now.ToString());
        //            WriteToLogfile("    Table: " + tableName);
        //            WriteToLogfile("    Default Version: " + pDefaultVersion.VersionName);
        //            WriteToLogfile("    Daily Sync Version: " + pDailySyncVersion.VersionName);
        //            hshDiffObjects.Clear();

        //            // Cast the child version to IFeatureWorkspace and open the table.
        //            Debug.Print("Processing featureclass: " + tableName);
        //            IFeatureWorkspace pDefaultFWS = (IFeatureWorkspace)pDefaultVersion;
        //            IFeatureWorkspace pDailySyncFWS = (IFeatureWorkspace)pDailySyncVersion;
        //            ITable pDefaultTable = pDefaultFWS.OpenTable(tableName);
        //            IDataset pChildDS = (IDataset)pDefaultTable;

        //            //Get the globalid field index 
        //            string globalId = "";
        //            int globalIdFieldIdx = pDefaultTable.Fields.FindField("globalid");
        //            if (globalIdFieldIdx == -1)
        //                Debug.Print("globalid field is not present for: " + tableName);

        //            // Cast the common ancestor version to IFeatureWorkspace and open the table.
        //            IFeatureWorkspace commonAncestorFWS = (IFeatureWorkspace)pCommonAncestorVersion;
        //            ITable commonAncestorTable = commonAncestorFWS.OpenTable(tableName);

        //            // Cast to the IVersionedTable interface to create a difference cursor.
        //            IVersionedTable versionedTable = (IVersionedTable)pDefaultTable;

        //            for (int i = 0; i < pDiffTypes.Length; i++)
        //            {
        //                WriteToLogfile("Looking for differences of type: " + pDiffTypes[i].ToString());
        //                IDifferenceCursor differenceCursor = versionedTable.Differences
        //                    (commonAncestorTable, pDiffTypes[i], null);

        //                // Step through the cursor, storing OId of each row
        //                IRow differenceRow = null;
        //                int objectID = -1;
        //                differenceCursor.Next(out objectID, out differenceRow);

        //                while (objectID != -1)
        //                {
        //                    //Add the object
        //                    if ((pDiffTypes[i] != ESRI.ArcGIS.Geodatabase.esriDifferenceType.esriDifferenceTypeDeleteNoChange) &&
        //                        (pDiffTypes[i] != ESRI.ArcGIS.Geodatabase.esriDifferenceType.esriDifferenceTypeDeleteUpdate))
        //                    {
        //                        globalId = differenceRow.get_Value(globalIdFieldIdx).ToString();
        //                    }
        //                    else
        //                    {
        //                        //has been deleted so get globalid from the non-default version 
        //                        globalId = commonAncestorTable.GetRow(objectID).get_Value(globalIdFieldIdx).ToString();
        //                    }

        //                    //Add the row keyed from the objectid 
        //                    if (!hshDiffObjects.ContainsKey(globalId))
        //                        hshDiffObjects.Add(globalId, differenceRow);

        //                    differenceCursor.Next(out objectID, out differenceRow);
        //                }

        //                Marshal.FinalReleaseComObject(differenceCursor);
        //            }
        //            WriteToLogfile("    Found following count of differences: " + hshDiffObjects.Count.ToString());
        //        }
        //        catch (Exception ex)
        //        {
        //            WriteToLogfile("Entering FindVersionDifferences Error Handler: " + ex.Message);
        //            throw ex;
        //        }
        //    }

        //    private void ProcessInserts(
        //        Hashtable hshInsertedObjects,
        //        IWorkspace pDestWS,
        //        IFeatureClass pSourceFC,
        //        IFeatureClass pDestFC)
        //    {

        //        IWorkspaceEdit pWSE = null;
        //        IFeatureClassLoad featureClassLoad = null;

        //        try
        //        {
        //            Stopwatch stopWatch = new Stopwatch();
        //            stopWatch.Start();

        //            IDataset pSrcDS = (IDataset)pSourceFC;
        //            IDataset pDestDS = (IDataset)pDestFC;
        //            IFeature pSourceFeature = null;
        //            string browseName = pDestDS.BrowseName;
        //            WriteToLogfile("======================================================");

        //            //Get the list of required fields 
        //            string destFieldName = "";
        //            string sourceFieldName = "";
        //            int srcFieldIndex = -1;
        //            int destFieldIndex = -1;
        //            string fieldList = "";
        //            Hashtable hshFields = new Hashtable();
        //            for (int i = 0; i < pDestFC.Fields.FieldCount; i++)
        //            {
        //                destFieldName = pDestFC.Fields.get_Field(i).Name.ToLower();
        //                if (pDestFC.Fields.get_Field(i).Editable == true)
        //                {
        //                    if (destFieldName == "globalid_old")
        //                    {
        //                        WriteToLogfile(pDestDS.Name + " has field globalid_old");
        //                        sourceFieldName = "globalid";
        //                        srcFieldIndex = pSourceFC.Fields.FindField(sourceFieldName);
        //                    }
        //                    else if (destFieldName == "objectid_old")
        //                    {
        //                        WriteToLogfile(pDestDS.Name + " has field objectid_old");
        //                        sourceFieldName = pSourceFC.OIDFieldName;
        //                        srcFieldIndex = pSourceFC.Fields.FindField(sourceFieldName);
        //                    }
        //                    else
        //                    {
        //                        sourceFieldName = destFieldName;
        //                        srcFieldIndex = pSourceFC.Fields.FindField(destFieldName);
        //                    }

        //                    destFieldIndex = i;
        //                    if ((srcFieldIndex != -1) && (destFieldIndex != -1))
        //                    {
        //                        int[] indexes = { srcFieldIndex, i };
        //                        if (hshFields.ContainsKey(destFieldName) == false)
        //                            hshFields.Add(destFieldName, indexes);

        //                        if (fieldList == "")
        //                            fieldList = sourceFieldName;
        //                        else
        //                            fieldList = fieldList + "," + sourceFieldName;
        //                    }
        //                }
        //            }
        //            IDictionaryEnumerator enumFields = hshFields.
        //                GetEnumerator();
        //            WriteToLogfile("Field count for: " + pDestDS.Name + " : " + hshFields.Count.ToString());

        //            int recCount = 0;
        //            int newFeatureCount = 0;
        //            int editCount = 0;
        //            int srcShapeFldIdx = pSourceFC.Fields.FindField(pSourceFC.ShapeFieldName);

        //            //Look for the features (can use recycle sursor) 
        //            //IFeatureCursor pFCursor = pSourceFC.Search(null, true);
        //            int recTotal = hshInsertedObjects.Count;
        //            WriteToLogfile("recTotal: " + recTotal.ToString());

        //            if (recTotal != 0)
        //            {
        //                //Look for 
        //s 
        //                IQueryFilter pQF = new QueryFilterClass();
        //                pQF.WhereClause = "globalid_old" + " IN(" + GetCommaSeparatedList(hshInsertedObjects) + ")";
        //                int duplicateCount = pDestFC.FeatureCount(pQF);
        //                Hashtable hshDuplicates = new Hashtable();
        //                if (duplicateCount != 0)
        //                {
        //                    WriteToLogfile("duplicate count for: " + pDestDS.Name + " is: " + duplicateCount.ToString());
        //                    IFeatureCursor pDupCursor = pDestFC.Search(pQF, false);
        //                    IFeature pDupFeature = pDupCursor.NextFeature();
        //                    while (pDupFeature != null)
        //                    {
        //                        string globId = pDupFeature.get_Value(pDupFeature.Fields.FindField("globalid_old")).ToString();
        //                        if (!hshDuplicates.ContainsKey(globId))
        //                            hshDuplicates.Add(globId, 0);
        //                        pDupFeature = pDupCursor.NextFeature();
        //                    }
        //                    Marshal.FinalReleaseComObject(pDupCursor);
        //                }


        //                //Start an edit operation 
        //                pWSE = (IWorkspaceEdit)pDestDS.Workspace;
        //                pWSE.StartEditOperation();

        //                // Cast the feature class to the IFeatureClassLoad interface.
        //                featureClassLoad = (IFeatureClassLoad)pDestFC;

        //                // Enable load-only mode on the feature class (much faster) 
        //                featureClassLoad.LoadOnlyMode = true;

        //                //Create an insert featurecusor                     
        //                IFeatureCursor pInsertCursor = pDestFC.Insert(true);
        //                IFeatureBuffer pInsertFB = pDestFC.CreateFeatureBuffer();

        //                foreach (string globalId in hshInsertedObjects.Keys)
        //                {
        //                    //Loop through the features to clip   
        //                    recCount++;
        //                    pSourceFeature = (IFeature)hshInsertedObjects[globalId];
        //                    string msgCur = "Processing " + recCount + " of: " +
        //                        recTotal;

        //                    //Do not insert if it is a duplicate 
        //                    if (!hshDuplicates.ContainsKey(globalId))
        //                    {
        //                        //copy all the attributes 
        //                        enumFields.Reset();
        //                        while (enumFields.MoveNext())
        //                        {
        //                            destFieldName = enumFields.Key.ToString().
        //                                ToLower();
        //                            int[] indexes = (int[])enumFields.Value;
        //                            srcFieldIndex = indexes[0];
        //                            destFieldIndex = indexes[1];

        //                            if (srcShapeFldIdx == srcFieldIndex)
        //                            {
        //                                //Copy the shape 
        //                                pInsertFB.Shape = pSourceFeature.ShapeCopy;
        //                            }
        //                            else
        //                            {
        //                                //Just copy the field value accross 
        //                                //Debug.Print("comment to trouble shoot");
        //                                object theValue = pSourceFeature.
        //                                    get_Value(srcFieldIndex);
        //                                pInsertFB.set_Value(destFieldIndex,
        //                                    theValue);
        //                            }
        //                        }

        //                        //Insert the new feature 
        //                        newFeatureCount++;
        //                        editCount++;
        //                        try
        //                        {
        //                            pInsertCursor.InsertFeature(pInsertFB);
        //                        }
        //                        catch
        //                        {
        //                            WriteToLogfile("Failed to insert featurebuffer on: " +
        //                                pDestDS.Name + " oid: " + pSourceFeature.OID.ToString());
        //                            throw new Exception("Unable to insert feature");
        //                        }

        //                        //Flush every record 
        //                        if (newFeatureCount == 1000)
        //                        {
        //                            WriteToLogfile("Written: " + editCount.ToString() + " features");
        //                            try
        //                            { pInsertCursor.Flush(); }
        //                            catch
        //                            { throw new Exception("Unable to flush feature buffer"); }
        //                            newFeatureCount = 0;
        //                        }
        //                    }
        //                }

        //                //Flush if necessary 
        //                if (newFeatureCount != 0)
        //                    pInsertCursor.Flush();

        //                //Stop the edit operation 
        //                pWSE.StopEditOperation();

        //                //Release the cursor and buffer 
        //                Marshal.FinalReleaseComObject(pInsertCursor);
        //                Marshal.FinalReleaseComObject(pInsertFB);
        //            }

        //            //Write message to logfile 
        //            WriteToLogfile(browseName + " successfully processed " + editCount +
        //                " inserted features to destination geodatabase");
        //            WriteToLogfile(browseName + " completed in : " + stopWatch.ElapsedMilliseconds.ToString() + " milliseconds");
        //            stopWatch.Stop();
        //            WriteToLogfile("======================================================");

        //        }
        //        catch (Exception ex)
        //        {
        //            //Just throw the error 
        //            WriteToLogfile("An error occurred in: " +
        //                "ExportFeatureclass: " + ex.Message);
        //            if (pWSE != null)
        //            {
        //                pWSE.AbortEditOperation();
        //            }
        //            throw ex;
        //        }
        //        finally
        //        {
        //            // Disable load-only mode on the feature class.
        //            if (featureClassLoad != null)
        //                featureClassLoad.LoadOnlyMode = false;
        //        }
        //    }

        //    private void ProcessUpdates(
        //        Hashtable hshEditedObjects,
        //        Hashtable hshValidConStatuses,
        //        ref Hashtable hshAdditionalDeletes,
        //        ref Hashtable hshAdditionalInserts,
        //        IWorkspace pDestWS,
        //        IFeatureClass pSourceFC,
        //        IFeatureClass pDestFC)
        //    {
        //        IWorkspaceEdit pWSE = null;

        //        try
        //        {
        //            Stopwatch stopWatch = new Stopwatch();
        //            stopWatch.Start();

        //            IDataset pSrcDS = (IDataset)pSourceFC;
        //            IDataset pDestDS = (IDataset)pDestFC;
        //            string browseName = pDestDS.BrowseName;
        //            WriteToLogfile("======================================================");

        //            //Get the list of required fields 
        //            string destFieldName = "";
        //            string sourceFieldName = "";
        //            IFeature pEditedFeature = null;
        //            int srcFieldIndex = -1;
        //            int destFieldIndex = -1;
        //            int conStatusFieldIdx = GetConstructionStatusFieldIndex(pSourceFC);
        //            bool hasValidConStatus = false;
        //            int destGlobalIdFieldIdx = pDestFC.Fields.FindField("globalid_old");
        //            string destGlobalId = "";
        //            int conStatus = -1;
        //            string fieldList = "";
        //            Hashtable hshFields = new Hashtable();

        //            for (int i = 0; i < pDestFC.Fields.FieldCount; i++)
        //            {
        //                destFieldName = pDestFC.Fields.get_Field(i).Name.ToLower();
        //                if (pDestFC.Fields.get_Field(i).Editable == true)
        //                {
        //                    if (destFieldName == "globalid_old")
        //                    {
        //                        WriteToLogfile(pDestDS.Name + " has field globalid_old");
        //                        sourceFieldName = "globalid";
        //                        srcFieldIndex = pSourceFC.Fields.FindField(sourceFieldName);
        //                    }
        //                    else if (destFieldName == "objectid_old")
        //                    {
        //                        WriteToLogfile(pDestDS.Name + " has field objectid_old");
        //                        sourceFieldName = pSourceFC.OIDFieldName;
        //                        srcFieldIndex = pSourceFC.Fields.FindField(sourceFieldName);
        //                    }
        //                    else
        //                    {
        //                        sourceFieldName = destFieldName;
        //                        srcFieldIndex = pSourceFC.Fields.FindField(destFieldName);
        //                    }

        //                    destFieldIndex = i;
        //                    if ((srcFieldIndex != -1) && (destFieldIndex != -1))
        //                    {
        //                        int[] indexes = { srcFieldIndex, i };
        //                        if (hshFields.ContainsKey(destFieldName) == false)
        //                            hshFields.Add(destFieldName, indexes);

        //                        if (fieldList == "")
        //                            fieldList = sourceFieldName;
        //                        else
        //                            fieldList = fieldList + "," + sourceFieldName;
        //                    }
        //                }
        //            }
        //            IDictionaryEnumerator enumFields = hshFields.
        //                GetEnumerator();
        //            WriteToLogfile("Field count for: " + pDestDS.Name + " : " + hshFields.Count.ToString());

        //            Hashtable hshEditsNotFound = new Hashtable();
        //            foreach (string globId in hshEditedObjects.Keys)
        //            {
        //                hshEditsNotFound.Add(globId, 0);
        //            }
        //            int recCount = 0;
        //            int newFeatureCount = 0;
        //            int editCount = 0;
        //            int srcShapeFldIdx = pSourceFC.Fields.FindField(pSourceFC.ShapeFieldName);

        //            //Look for the features (can use recycle sursor) 
        //            //IFeatureCursor pFCursor = pSourceFC.Search(null, true);
        //            int recTotal = hshEditedObjects.Count;
        //            WriteToLogfile("recTotal: " + recTotal.ToString());

        //            if (hshEditedObjects.Count != 0)
        //            {
        //                //Start an edit operation 
        //                pWSE = (IWorkspaceEdit)pDestDS.Workspace;
        //                pWSE.StartEditOperation();

        //                //Create an update featurecusor  
        //                IQueryFilter pQF = new QueryFilterClass();
        //                pQF.WhereClause = "globalid_old" + " IN(" + GetCommaSeparatedList(hshEditedObjects) + ")";
        //                IFeatureCursor pUpdateCursor = pDestFC.Update(pQF, false);
        //                IFeature pDestFeature = pUpdateCursor.NextFeature();

        //                while (pDestFeature != null)
        //                {
        //                    //Find the globalid 
        //                    destGlobalId = "";
        //                    if (destGlobalIdFieldIdx != -1)
        //                        destGlobalId = pDestFeature.get_Value(destGlobalIdFieldIdx).ToString();

        //                    //Check if this feature is edited 
        //                    if (hshEditedObjects.ContainsKey(destGlobalId))
        //                    {
        //                        recCount++;
        //                        hasValidConStatus = false;

        //                        //Get the edited feature 
        //                        pEditedFeature = (IFeature)hshEditedObjects[destGlobalId];
        //                        if (hshEditsNotFound.ContainsKey(destGlobalId))
        //                            hshEditsNotFound.Remove(destGlobalId);

        //                        //If the featureclass has construction status field make 
        //                        //sure that it has the necessary status (not proposed)
        //                        if (conStatusFieldIdx != -1)
        //                        {
        //                            conStatus = Convert.ToInt32(pEditedFeature.get_Value(conStatusFieldIdx));
        //                            if (hshValidConStatuses.ContainsKey(conStatus))
        //                                hasValidConStatus = true;
        //                        }

        //                        if (hasValidConStatus)
        //                        {
        //                            //copy all the attributes 
        //                            enumFields.Reset();
        //                            while (enumFields.MoveNext())
        //                            {
        //                                destFieldName = enumFields.Key.ToString().
        //                                    ToLower();
        //                                int[] indexes = (int[])enumFields.Value;
        //                                srcFieldIndex = indexes[0];
        //                                destFieldIndex = indexes[1];

        //                                if (srcShapeFldIdx == srcFieldIndex)
        //                                {
        //                                    //Copy the shape 
        //                                    pDestFeature.Shape = pEditedFeature.ShapeCopy;
        //                                }
        //                                else
        //                                {
        //                                    //Just copy the field value accross 
        //                                    //Debug.Print("comment to trouble shoot");
        //                                    object theValue = pEditedFeature.
        //                                        get_Value(srcFieldIndex);
        //                                    pDestFeature.set_Value(destFieldIndex,
        //                                        theValue);
        //                                }
        //                            }

        //                            //Insert the new feature 
        //                            newFeatureCount++;
        //                            editCount++;
        //                            try
        //                            {
        //                                pUpdateCursor.UpdateFeature(pDestFeature);
        //                            }
        //                            catch
        //                            {
        //                                WriteToLogfile("Failed to update feature on: " +
        //                                    pDestDS.Name + " oid: " + pDestFeature.OID.ToString());
        //                                throw new Exception("Unable to update feature");
        //                            }
        //                        }
        //                        else
        //                        {
        //                            //the feature now does not have a valid con status and 
        //                            //should be deleted 
        //                            if (!hshAdditionalDeletes.ContainsKey(destGlobalId))
        //                                hshAdditionalDeletes.Add(destGlobalId, hshEditedObjects[destGlobalId]);
        //                        }
        //                    }
        //                    pDestFeature = pUpdateCursor.NextFeature();
        //                }

        //                //Flush if necessary 
        //                pUpdateCursor.Flush();

        //                //Stop the edit operation 
        //                pWSE.StopEditOperation();

        //                //Release the cursor and buffer 
        //                Marshal.FinalReleaseComObject(pUpdateCursor);
        //            }

        //            //Any that were not found need to be inserted - this might happen when 
        //            //a feature of proposed status is changed to in-service  
        //            if (hshEditsNotFound.Count != 0)
        //            {
        //                foreach (string globId in hshEditsNotFound.Keys)
        //                    hshAdditionalInserts.Add(globId, hshEditedObjects[globId]);
        //            }

        //            //Write message to logfile 
        //            WriteToLogfile(browseName + " successfully updated " + recCount +
        //                " features in destination geodatabase");
        //            WriteToLogfile(browseName + " completed in : " + stopWatch.ElapsedMilliseconds.ToString() + " milliseconds");
        //            stopWatch.Stop();
        //            WriteToLogfile("======================================================");

        //        }
        //        catch (Exception ex)
        //        {
        //            //Just throw the error 
        //            WriteToLogfile("An error occurred in: " +
        //                "ExportFeatureclass: " + ex.Message);
        //            if (pWSE != null)
        //            {
        //                pWSE.AbortEditOperation();
        //            }
        //            throw ex;
        //        }
        //    }

        //    private void ProcessDeletes(
        //        Hashtable hshEditedObjects,
        //        IWorkspace pDestWS,
        //        IFeatureClass pSourceFC,
        //        IFeatureClass pDestFC)
        //    {

        //        IWorkspaceEdit pWSE = null;
        //        //IFeatureClassLoad featureClassLoad = null;

        //        try
        //        {
        //            Stopwatch stopWatch = new Stopwatch();
        //            stopWatch.Start();

        //            IDataset pSrcDS = (IDataset)pSourceFC;
        //            IDataset pDestDS = (IDataset)pDestFC;
        //            string browseName = pDestDS.BrowseName;
        //            WriteToLogfile("======================================================");

        //            Hashtable hshFoundOIds = new Hashtable();
        //            int recCount = 0;
        //            int newFeatureCount = 0;
        //            int editCount = 0;
        //            int srcShapeFldIdx = pSourceFC.Fields.FindField(pSourceFC.ShapeFieldName);
        //            int destGlobalIdFieldIdx = pDestFC.Fields.FindField("globalid");
        //            string destGlobalId = "";

        //            //Look for the features (can use recycle sursor) 
        //            //IFeatureCursor pFCursor = pSourceFC.Search(null, true);
        //            int recTotal = hshEditedObjects.Count;
        //            WriteToLogfile("recTotal: " + recTotal.ToString());

        //            if (hshEditedObjects.Count != 0)
        //            {
        //                //Start an edit operation 
        //                pWSE = (IWorkspaceEdit)pDestDS.Workspace;
        //                pWSE.StartEditOperation();

        //                //Create an update featurecusor   
        //                IQueryFilter pQF = new QueryFilterClass();
        //                pQF.WhereClause = "globalid_old" + " IN(" + GetCommaSeparatedList(hshEditedObjects) + ")";
        //                IFeatureCursor pUpdateCursor = pDestFC.Update(pQF, false);
        //                IFeature pDestFeature = pUpdateCursor.NextFeature();

        //                while (pDestFeature != null)
        //                {
        //                    //Find the globalid 
        //                    destGlobalId = "";
        //                    if (destGlobalIdFieldIdx != -1)
        //                        destGlobalId = pDestFeature.get_Value(destGlobalIdFieldIdx).ToString();

        //                    //Check if this feature is edited 
        //                    if (hshEditedObjects.ContainsKey(destGlobalId))
        //                    {
        //                        recCount++;
        //                        hshFoundOIds.Add(destGlobalId, 0);

        //                        //Insert the new feature 
        //                        newFeatureCount++;
        //                        editCount++;
        //                        try
        //                        {
        //                            pUpdateCursor.DeleteFeature();
        //                        }
        //                        catch
        //                        {
        //                            WriteToLogfile("Failed to delete feature on: " +
        //                                pDestDS.Name + " oid: " + pDestFeature.OID.ToString());
        //                            throw new Exception("Unable to delete feature");
        //                        }
        //                    }

        //                    pDestFeature = pUpdateCursor.NextFeature();
        //                }

        //                //Stop the edit operation 
        //                pWSE.StopEditOperation();

        //                //Release the cursor and buffer 
        //                Marshal.FinalReleaseComObject(pUpdateCursor);
        //            }

        //            //Write message to logfile 
        //            WriteToLogfile(browseName + " successfully deleted " + recCount +
        //                " features in destination geodatabase");
        //            WriteToLogfile(browseName + " completed in : " + stopWatch.ElapsedMilliseconds.ToString() + " milliseconds");
        //            stopWatch.Stop();
        //            WriteToLogfile("======================================================");

        //        }
        //        catch (Exception ex)
        //        {
        //            //Just throw the error 
        //            WriteToLogfile("An error occurred in: " +
        //                "ExportFeatureclass: " + ex.Message);
        //            if (pWSE != null)
        //            {
        //                pWSE.AbortEditOperation();
        //            }
        //            throw ex;
        //        }
        //    }

        //    private Hashtable GetClassesEditedInVersion(
        //IVersion pDesignVersion,
        //IVersion pDefaultVersion)
        //    {
        //        try
        //        {
        //            Hashtable hshEditedClasses = new Hashtable();
        //            IDataChanges diffs = null;
        //            IEnumModifiedClassInfo modifiedClasses = null;
        //            IVersionDataChangesInit versionDiffsInit = (IVersionDataChangesInit)new VersionDataChangesClass();
        //            IWorkspaceName pWSNDesign = (IWorkspaceName)(pDesignVersion as IDataset).FullName;
        //            IWorkspaceName pWSNDefault = (IWorkspaceName)(pDefaultVersion as IDataset).FullName;

        //            //Initialize the IVersionDataChangesInit 
        //            //versionDiffsInit.Init(pWSNDesign, pWSNDefault);
        //            versionDiffsInit.Init(pWSNDefault, pWSNDesign);

        //            diffs = (IDataChanges)versionDiffsInit;
        //            modifiedClasses = diffs.GetModifiedClassesInfo();

        //            IModifiedClassInfo oneModifiedClass = null;
        //            string sName = null;

        //            //Find the modified classes that we are interested in                 
        //            oneModifiedClass = modifiedClasses.Next();
        //            while (oneModifiedClass != null)
        //            {
        //                sName = oneModifiedClass.ChildClassName.ToUpper();
        //                if (!hshEditedClasses.ContainsKey(sName))
        //                    hshEditedClasses.Add(sName, 0);
        //                oneModifiedClass = modifiedClasses.Next();
        //            }
        //            return hshEditedClasses;
        //        }
        //        catch (Exception ex)
        //        {
        //            WriteToLogfile("Failed returning version difference:" + ex.Message);
        //            throw ex;
        //        }
        //    }

        //    private int GetConstructionStatusFieldIndex(IFeatureClass pFC)
        //    {
        //        try
        //        {
        //            int fldIdx = -1;
        //            for (int i = 0; i < pFC.Fields.FieldCount; i++)
        //            {
        //                if (pFC.Fields.get_Field(i).Domain != null)
        //                {
        //                    IDomain pDomain = (IDomain)pFC.Fields.get_Field(i).Domain;
        //                    if (pDomain.Name.ToLower() == "construction status")
        //                    {
        //                        fldIdx = i;
        //                        break;
        //                    }

        //                }
        //            }
        //            return fldIdx;
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception("Error finding construction status field index");
        //        }
        //    }

        private string GetCommaSeparatedList(Hashtable hshKeys)
        {
            try
            {
                StringBuilder commaSepList = new StringBuilder();
                foreach (string guid in hshKeys.Keys)
                {
                    if (commaSepList.Length == 0)
                    {
                        commaSepList.Append("'" + guid + "'");
                    }
                    else
                    {
                        commaSepList.Append("," + "'" + guid + "'");
                    }
                }
                return commaSepList.ToString();
            }
            catch
            {
                throw new Exception("Error returning comma separated list");
            }
        }

        /// <summary>
        /// Takes a Dataset by reference and sets the spatial reference to the 
        /// passed target spatial reference 
        /// </summary>
        /// <param name="pGeodataset"></param>
        /// <param name="pTargetSR"></param>
        public void SetDatasetSpatialReference(ref IGeoDataset pGeodataset, ISpatialReference pTargetSR)
        {
            try
            {

                //set the to the desired spatial reference 
                IGeoDatasetSchemaEdit pGeoDatasetEdit = (IGeoDatasetSchemaEdit)pGeodataset;
                if (pGeoDatasetEdit.CanAlterSpatialReference)
                {
                    ISpatialReferenceFactory pSpatialRefFact = new SpatialReferenceEnvironmentClass();
                    IProjectedCoordinateSystem pProjCoordSys = (IProjectedCoordinateSystem)pTargetSR;
                    pGeoDatasetEdit.AlterSpatialReference(pProjCoordSys);
                }

                if (pGeodataset.SpatialReference.Name != pTargetSR.Name)
                    throw new Exception("Error changing dataset spatial reference");

            }
            catch (Exception ex)
            {
                throw new Exception("Error changing dataset spatial reference");
            }
        }

        //public void ProcessSynch()
        //{
        //    try
        //    {
        //        //Connect to the necessary workspaces 
        //        WriteToLogfile("connecting to workspaces");
        //        WriteToLogfile("======================================================");
        //        WriteToLogfile("connecting to source electric workspace");
        //        _pSourceElectricWorkspace = (IWorkspace)GetWorkspace(_sourceElectricWorkspace);
        //        WriteToLogfile("connecting to destination electric workspace");
        //        _pDestElectricWorkspace = (IWorkspace)GetWorkspace(_destElectricWorkspace);
        //        WriteToLogfile("connecting to source wip workspace");
        //        _pSourceWIPWorkspace = (IWorkspace)GetWorkspace(_sourceWIPWorkspace);
        //        WriteToLogfile("connecting to destination wip workspace");
        //        _pDestWIPWorkspace = (IWorkspace)GetWorkspace(_destWIPWorkspace);
        //        WriteToLogfile("connecting to source landbase workspace");
        //        _pSourceLandbaseWorkspace = (IWorkspace)GetWorkspace(_sourceLandbaseWorkspace);
        //        WriteToLogfile("connecting to destination landbase workspace");
        //        _pDestLandbaseWorkspace = (IWorkspace)GetWorkspace(_destLandbaseWorkspace);

        //        if (_includeElectric)
        //        {
        //            if (_pSourceElectricWorkspace == null)
        //            {
        //                WriteToLogfile("You must first define the source electric workspace");
        //                return;
        //            }
        //            if (_pDestElectricWorkspace == null)
        //            {
        //                WriteToLogfile("You must first define the destination electric workspace");
        //                return;
        //            }
        //        }
        //        WriteToLogfile("======================================================");

        //        if (_includeElectric)
        //            SynchFileGeodatabase(
        //                _pSourceElectricWorkspace,
        //                _pDestElectricWorkspace);

        //        WriteToLogfile("ProcessExtract completed");
        //        return;

        //    }
        //    catch (Exception ex)
        //    {
        //        WriteToLogfile("An error occurred in ProcessExtract: " +
        //            ex.Message);
        //    }
        //}

    }
}
