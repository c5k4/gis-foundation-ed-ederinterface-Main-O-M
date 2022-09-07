using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Geodatabase;
using System.IO;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.GeoDatabaseDistributed;
using System.Reflection;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;
using System.Diagnostics;

namespace PGE.BatchApplication.FGDBExtraction
{
    public partial class ExtractDataForm : Form
    {
        public static ExtractDataForm Instance { get; set; }
        private static IWorkspace GISWorkspace = null;
        private static IWorkspace LandbaseWorkspace = null;
        private static IFeatureClass DivisionFeatClass = null;

        //private static string SearchPolygonFeatureClassName = "LBGIS.ElecDivision";
        private static string SearchPolygonFeatureClassName = "";
        private static string SearchPolygonFieldName = "";
        private StreamWriter writer = null;

        private static bool LicenseCheckedOut = false;

        public ExtractDataForm()
        {
            InitializeComponent();

            try
            {
                Instance = this;

                string assemblyLocation = Assembly.GetExecutingAssembly().Location;
                assemblyLocation = assemblyLocation.Substring(0, assemblyLocation.LastIndexOf("\\") + 1);
                writer = new StreamWriter(assemblyLocation + @"\" + DateTime.Now.ToString("yyyyMMdd_HH_mm") + "_Extract.Log");

                ImageList imageList = new ImageList();
                imageList.Images.Add(Image.FromFile("Images\\AnnoFeatureClass.png"));
                imageList.Images.Add(Image.FromFile("Images\\FeatureDataset.png"));
                imageList.Images.Add(Image.FromFile("Images\\LineFeatureClass.png"));
                imageList.Images.Add(Image.FromFile("Images\\PointFeatureClass.png"));
                imageList.Images.Add(Image.FromFile("Images\\PolygonFeatureClass.png"));
                imageList.Images.Add(Image.FromFile("Images\\Relationship.PNG"));
                imageList.Images.Add(Image.FromFile("Images\\Table.png"));

                treeDatabase.ImageList = imageList;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Failed to load images: " + ex.Message);
            }
            
            //lblPolygon.Text = SearchPolygonFeatureClassName;
        }

        private void btnConnectDatabase_Click(object sender, EventArgs e)
        {
            try
            {
                if (!LicenseCheckedOut) { LicenseCheckedOut = ModFunctions.InitializeLicense(); }

                if (LicenseCheckedOut)
                {
                    if (!txtSDEConnection.Text.Contains('\\'))
                    {
                        txtSDEConnection.Text = Directory.GetCurrentDirectory() + "\\" + txtSDEConnection.Text;
                    }
                    if (!txtLandbaseConnection.Text.Contains('\\'))
                    {
                        txtLandbaseConnection.Text = Directory.GetCurrentDirectory() + "\\" + txtLandbaseConnection.Text;
                    }
                    if (!txtFGDBLocation.Text.Contains('\\'))
                    {
                        txtFGDBLocation.Text = Directory.GetCurrentDirectory() + "\\" + txtFGDBLocation.Text;
                    }
                    if (!(File.Exists(txtSDEConnection.Text) || Directory.Exists(txtSDEConnection.Text)) || !(txtSDEConnection.Text.ToUpper().EndsWith(".SDE") || txtSDEConnection.Text.ToUpper().EndsWith(".GDB")))
                    {
                        MessageBox.Show("Please specify a valid sde/fgdb connection file for the GIS database");
                        return;
                    }
                    else if (!(File.Exists(txtLandbaseConnection.Text) || Directory.Exists(txtLandbaseConnection.Text)) || !(txtLandbaseConnection.Text.ToUpper().EndsWith(".SDE") || txtLandbaseConnection.Text.ToUpper().EndsWith(".GDB")))
                    {
                        MessageBox.Show("Please specify a valid sde.fgdb connection file for the landbase database");
                        return;
                    }
                    var wsFactory = txtSDEConnection.Text.ToUpper().EndsWith(".SDE") ? (IWorkspaceFactory)new SdeWorkspaceFactoryClass() : (IWorkspaceFactory)new FileGDBWorkspaceFactoryClass();
                    //SdeWorkspaceFactory wsFactory = new SdeWorkspaceFactory();
                    //FileGDBWorkspaceFactory wsFactory = new FileGDBWorkspaceFactoryClass();

                    if (GISWorkspace == null) { GISWorkspace = wsFactory.OpenFromFile(txtSDEConnection.Text, 0); }
                    if (LandbaseWorkspace == null) 
                    {
                        try {
                            wsFactory = txtLandbaseConnection.Text.ToUpper().EndsWith(".SDE") ? (IWorkspaceFactory)new SdeWorkspaceFactoryClass() : (IWorkspaceFactory)new FileGDBWorkspaceFactoryClass(); 
                            LandbaseWorkspace = wsFactory.OpenFromFile(txtLandbaseConnection.Text, 0);
                        }
                        catch { }
                    }

                    //Prepare the division domain drop down
                    SearchPolygonFeatureClassName = txtExtractionPolygon.Text;
                    SearchPolygonFieldName = txtExtractionField.Text;
                    try { DivisionFeatClass = ((IFeatureWorkspace)LandbaseWorkspace).OpenFeatureClass(SearchPolygonFeatureClassName); }
                    catch { }
                    if (DivisionFeatClass == null) 
                    {
                        DivisionFeatClass = ((IFeatureWorkspace)GISWorkspace).OpenFeatureClass(SearchPolygonFeatureClassName);
                        if (DivisionFeatClass == null) { throw new Exception("Unable to find devision feature class LBGIS.ElecDivision"); }
                    }

                    if (DivisionFeatClass != null)
                    {
                        List<string> divisions = new List<string>();
                        IQueryFilter qf = new QueryFilterClass();
                        qf.AddField(SearchPolygonFieldName);
                        int divisonFieldIdx = DivisionFeatClass.FindField(SearchPolygonFieldName);
                        if (divisonFieldIdx < 0) { throw new Exception("Unable to find specified extraction polygon field"); }

                        IFeatureCursor divisionCursor = DivisionFeatClass.Search(qf, false);
                        IFeature division = null;
                        while ((division = divisionCursor.NextFeature()) != null)
                        {
                            object divisionObj = division.get_Value(divisonFieldIdx);
                            if (divisionObj != null && !string.IsNullOrEmpty(divisionObj.ToString()))
                            {
                                divisions.Add(divisionObj.ToString());
                            }

                            if (division != null) { while (Marshal.ReleaseComObject(division) > 0) { } }
                        }

                        if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }
                        if (divisionCursor != null) { while (Marshal.ReleaseComObject(divisionCursor) > 0) { } }

                        divisions = divisions.Distinct().ToList();
                        divisions.Sort();

                        foreach (string div in divisions) { cboDivision.Items.Add(div); }
                        cboDivision.SelectedIndex = 0;
                    }

                    Dictionary<esriDatasetType, List<DatasetInfo>> sortedDatasets = new Dictionary<esriDatasetType, List<DatasetInfo>>();
                    //Add all of the database feature datasets, feature classes, and tables to our tree view for selection
                    treeDatabase.BeginUpdate();
                    IEnumDataset dsNames = GISWorkspace.get_Datasets(esriDatasetType.esriDTFeatureDataset);
                    dsNames.Reset();
                    IDataset dsName = null;
                    sortedDatasets.Add(esriDatasetType.esriDTFeatureDataset, new List<DatasetInfo>());
                    while ((dsName = dsNames.Next()) != null)
                    {
                        DatasetInfo dsInfo = new DatasetInfo();
                        dsInfo.datasetType = esriDatasetType.esriDTFeatureDataset;
                        dsInfo.name = dsName.Name;

                        IFeatureClassContainer featClassContainer = dsName as IFeatureClassContainer;
                        for (int i = 0; i < featClassContainer.ClassCount; i++)
                        {
                            IFeatureClass featClass = featClassContainer.get_Class(i);
                            DatasetInfo featClassInfo = new DatasetInfo();
                            featClassInfo.datasetType = esriDatasetType.esriDTFeatureClass;
                            featClassInfo.featureType = featClass.FeatureType;
                            featClassInfo.shapeType = featClass.ShapeType;
                            featClassInfo.name = ((IDataset)featClass).Name;
                            dsInfo.AddFeatureClass(featClassInfo);
                        }

                        sortedDatasets[esriDatasetType.esriDTFeatureDataset].Add(dsInfo);
                    }

                    dsNames = GISWorkspace.get_Datasets(esriDatasetType.esriDTFeatureClass);
                    sortedDatasets.Add(esriDatasetType.esriDTFeatureClass, new List<DatasetInfo>());
                    dsNames.Reset();
                    dsName = null;
                    while ((dsName = dsNames.Next()) != null)
                    {
                        try
                        {
                            IVersionedObject versionedObj = dsName as IVersionedObject;
                            if (((IObjectClass)dsName).ObjectClassID > 0 && versionedObj.IsRegisteredAsVersioned)
                            {
                                DatasetInfo dsInfo = new DatasetInfo();
                                dsInfo.datasetType = esriDatasetType.esriDTFeatureClass;
                                dsInfo.featureType = ((IFeatureClassName)dsName).FeatureType;
                                dsInfo.shapeType = ((IFeatureClassName)dsName).ShapeType;
                                dsInfo.name = dsName.Name;
                                sortedDatasets[esriDatasetType.esriDTFeatureClass].Add(dsInfo);
                            }
                        }
                        catch { }
                    }

                    dsNames = GISWorkspace.get_Datasets(esriDatasetType.esriDTTable);
                    sortedDatasets.Add(esriDatasetType.esriDTTable, new List<DatasetInfo>());
                    dsNames.Reset();
                    dsName = null;
                    while ((dsName = dsNames.Next()) != null)
                    {
                        try
                        {
                            IVersionedObject versionedObj = dsName as IVersionedObject;
                            if (((IObjectClass)dsName).ObjectClassID > 0 && versionedObj.IsRegisteredAsVersioned)
                            {
                                DatasetInfo dsInfo = new DatasetInfo();
                                dsInfo.datasetType = esriDatasetType.esriDTTable;
                                dsInfo.name = dsName.Name;
                                sortedDatasets[esriDatasetType.esriDTTable].Add(dsInfo);
                            }
                        }
                        catch { }
                    }

                    dsNames = GISWorkspace.get_Datasets(esriDatasetType.esriDTRelationshipClass);
                    sortedDatasets.Add(esriDatasetType.esriDTRelationshipClass, new List<DatasetInfo>());
                    dsNames.Reset();
                    dsName = null;
                    while ((dsName = dsNames.Next()) != null)
                    {
                        IRelationshipClass relClass = dsName as IRelationshipClass;
                        
                        DatasetInfo dsInfo = new DatasetInfo();
                        dsInfo.datasetType = esriDatasetType.esriDTRelationshipClass;
                        dsInfo.OriginClassName = ((IDataset)relClass.OriginClass).BrowseName;
                        dsInfo.DestinationClassName = ((IDataset)relClass.DestinationClass).BrowseName;
                        dsInfo.name = dsName.Name;
                        //sortedDatasets[esriDatasetType.esriDTRelationshipClass].Add(dsInfo);
                        //Add associated relationship classes to our table info objects so that they are automatically included
                        foreach (DatasetInfo tableInfo in sortedDatasets[esriDatasetType.esriDTTable])
                        {
                            if (tableInfo.name == dsInfo.OriginClassName || tableInfo.name == dsInfo.DestinationClassName)
                            {
                                tableInfo.AddRelationship(dsInfo);
                            }
                        }
                        while (Marshal.ReleaseComObject(relClass) > 0) { }
                    }

                    foreach (KeyValuePair<esriDatasetType, List<DatasetInfo>> kvp in sortedDatasets)
                    {
                        kvp.Value.Sort();
                        foreach (DatasetInfo dsInfo in kvp.Value)
                        {
                            TreeNode dsNode = new TreeNode(dsInfo.name);
                            dsNode.Tag = dsInfo;
                            if (dsInfo.datasetType == esriDatasetType.esriDTFeatureDataset) 
                            { 
                                dsNode.ImageIndex = 1;
                                dsNode.SelectedImageIndex = 1;
                            }
                            else if (dsInfo.datasetType == esriDatasetType.esriDTRelationshipClass)
                            {
                                dsNode.ImageIndex = 5;
                                dsNode.SelectedImageIndex = 5;
                            }
                            else if (dsInfo.datasetType == esriDatasetType.esriDTFeatureClass)
                            {
                                if (dsInfo.featureType == esriFeatureType.esriFTAnnotation) 
                                { 
                                    dsNode.ImageIndex = 0;
                                    dsNode.SelectedImageIndex = 0;
                                }
                                else if (dsInfo.shapeType == esriGeometryType.esriGeometryPoint) 
                                { 
                                    dsNode.ImageIndex = 3;
                                    dsNode.SelectedImageIndex = 3;
                                }
                                else if (dsInfo.shapeType == esriGeometryType.esriGeometryPolyline || dsInfo.shapeType == esriGeometryType.esriGeometryLine)
                                {
                                    dsNode.ImageIndex = 2;
                                    dsNode.SelectedImageIndex = 2;
                                }
                                else if (dsInfo.shapeType == esriGeometryType.esriGeometryPolygon)
                                {
                                    dsNode.ImageIndex = 4;
                                    dsNode.SelectedImageIndex = 4;
                                }
                            }
                            else if (dsInfo.datasetType == esriDatasetType.esriDTTable)
                            {
                                dsNode.ImageIndex = 6;
                                dsNode.SelectedImageIndex = 6;
                            }
                            dsNode.Checked = true;
                            treeDatabase.Nodes.Add(dsNode);
                        }
                    }

                    treeDatabase.EndUpdate();

                    //Enable our panel for extraction
                    pnlExtraction.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage(ex, "btnConnectDatabase_Click");
            }
        }


        private void AddOriginalFields()
        {
            LogMessage("Adding original GlobalID and OID fields");
            Dictionary<string, int> TableRegistrationIDs = new Dictionary<string, int>();
            IQueryFilter qf = new QueryFilterClass();
            qf.AddField("OWNER,TABLE_NAME,REGISTRATION_ID");

            ITable sdeRegistryTable = ((IFeatureWorkspace)GISWorkspace).OpenTable("SDE.TABLE_REGISTRY");
            ICursor cursor = sdeRegistryTable.Search(qf, false);
            int tableNameIdx = sdeRegistryTable.Fields.FindField("TABLE_NAME");
            int registrationIDIdx = sdeRegistryTable.Fields.FindField("REGISTRATION_ID");
            int ownerIDIdx = sdeRegistryTable.Fields.FindField("OWNER");

            IRow row = null;
            while ((row = cursor.NextRow()) != null)
            {
                try { TableRegistrationIDs.Add(row.get_Value(ownerIDIdx).ToString().ToUpper() + "." + row.get_Value(tableNameIdx).ToString().ToUpper(), (int)row.get_Value(registrationIDIdx)); }
                catch { }
            }

            List<string> tableNames = new List<string>();
            //Verify that the GLOBALID_O AND ORIGINAL_O exist in this database
            IEnumDatasetName datasets = GISWorkspace.get_DatasetNames(esriDatasetType.esriDTAny);
            IDatasetName DS = null;
            while ((DS = datasets.Next()) != null)
            {
                if (!DS.Name.ToUpper().StartsWith("EDGIS.")) { continue; }
                if (DS.Type == esriDatasetType.esriDTFeatureDataset)
                {
                    IFeatureDatasetName featClassContainer = DS as IFeatureDatasetName;
                    IEnumDatasetName featClassNames = featClassContainer.FeatureClassNames;
                    featClassNames.Reset();
                    IDatasetName featDS = null;
                    while((featDS = featClassNames.Next()) != null)
                    {
                        tableNames.Add(featDS.Name);
                    }
                }
                else if (DS.Type == esriDatasetType.esriDTFeatureClass || DS.Type == esriDatasetType.esriDTTable)
                {
                    tableNames.Add(DS.Name);
                }
            }

            tableNames.Sort();
            foreach (string TableName in tableNames)
            {
                try
                {
                    ITable table = ((IFeatureWorkspace)GISWorkspace).OpenTable(TableName);
                    AddFields(table, TableRegistrationIDs);
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void AddFields(ITable table, Dictionary<string, int> tableRegistryValues)
        {
            try
            {
                int registrationID = -1;

                try { registrationID = tableRegistryValues[((IDataset)table).BrowseName.ToUpper()]; }
                catch { }

                //Couldn't find a registration ID for this table.  Just return
                if (registrationID < 0) { return; }

                string originalGuidFieldName = "GLOBALID_O";
                string originalOIDFieldName = "OBJECTID_O";

                //Don't process these PGE views
                if (((IDataset)table).BrowseName.Contains("ZPGEVW_")) { return; }

                if (table.FindField(originalGuidFieldName) < 0 && table.FindField("GLOBALID") > -1)
                {
                    LogMessage(((IDataset)table).BrowseName + ": Adding " + originalGuidFieldName);

                    IFieldEdit2 field = new FieldClass() as IFieldEdit2;
                    field.Name_2 = originalGuidFieldName;
                    field.Type_2 = esriFieldType.esriFieldTypeGUID;

                    //Changes Done -2018_12_06
                    //ISchemaLock schemaLock = (ISchemaLock)table;
                    try
                    {
                        // Get an exclusive schema lock on the object class. 
                        //schemaLock.ChangeSchemaLock(esriSchemaLock.esriExclusiveSchemaLock);
                        table.AddField(field);
                    }
                    catch (Exception e)
                    {

                    }
                    finally
                    {                        
                        //schemaLock.ChangeSchemaLock(esriSchemaLock.esriSharedSchemaLock);
                    }
                    //Changes Done -2018_12_06

                    //table.AddField(field);


                    string updateSQL = "UPDATE A" + registrationID + " SET " + originalGuidFieldName + " = GLOBALID";
                    LogMessage(((IDataset)table).BrowseName + ": " + updateSQL);
                    GISWorkspace.ExecuteSQL(updateSQL);
                    updateSQL = "UPDATE " + ((IDataset)table).BrowseName + " SET " + originalGuidFieldName + " = GLOBALID";
                    LogMessage(((IDataset)table).BrowseName + ": " + updateSQL);
                    GISWorkspace.ExecuteSQL(updateSQL);
                }

                if (table.FindField(originalOIDFieldName) < 0 && !string.IsNullOrEmpty(table.OIDFieldName))
                {
                    LogMessage(((IDataset)table).BrowseName + ": Adding " + originalOIDFieldName);

                    IFieldEdit2 field = new FieldClass() as IFieldEdit2;
                    field.Name_2 = originalOIDFieldName;
                    field.Type_2 = esriFieldType.esriFieldTypeInteger;

                    //Changes Done -2018_12_06
                    ISchemaLock schemaLock = (ISchemaLock)table;
                    try
                    {
                        // Get an exclusive schema lock on the object class. 
                        schemaLock.ChangeSchemaLock(esriSchemaLock.esriExclusiveSchemaLock);
                        table.AddField(field);
                    }
                    catch (Exception e)
                    {

                    }
                    finally
                    {
                        schemaLock.ChangeSchemaLock(esriSchemaLock.esriSharedSchemaLock);
                    }
                    //Changes Done -2018_12_06

                    //table.AddField(field);

                    string updateSQL = "UPDATE A" + registrationID + " SET " + originalOIDFieldName + " = " + table.OIDFieldName;
                    LogMessage(((IDataset)table).BrowseName + ": " + updateSQL);
                    GISWorkspace.ExecuteSQL(updateSQL);
                    updateSQL = "UPDATE " + ((IDataset)table).BrowseName + " SET " + originalOIDFieldName + " = " + table.OIDFieldName;
                    LogMessage(((IDataset)table).BrowseName + ": " + updateSQL);
                    GISWorkspace.ExecuteSQL(updateSQL);
                }
            }
            catch (Exception ex)
            {
                LogMessage(((IDataset)table).BrowseName + ": Could not add original GUD and OID fields: " + ex.Message);
            }
        }

        public void ErrorMessage(Exception ex, string strFunctionName)
        {
            lblProcessLogging.Text += "------------------------------------------------------------------------------------------------\r\n";
            lblProcessLogging.Text += "[" + DateTime.Now + "] Error occured: Err Msg: " + ex.Message + ", (" + strFunctionName + ")\r\n";
            lblProcessLogging.Text += "------------------------------------------------------------------------------------------------\r\n";

            writer.WriteLine("------------------------------------------------------------------------------------------------");
            writer.WriteLine("[" + DateTime.Now + "] Error occured: Err Msg: " + ex.Message + ", (" + strFunctionName + ")");
            writer.WriteLine("------------------------------------------------------------------------------------------------");
            writer.Flush();
        }

        public void LogMessage(string strMessage)
        {
            lblProcessLogging.Text += DateTime.Now + ": " + strMessage + "\r\n";
            writer.WriteLine(DateTime.Now + ": " + strMessage);
            writer.Flush();
            Application.DoEvents();
        }

        private void txtLogs_TextChanged(object sender, EventArgs e)
        {
            // set the current caret position to the end
            lblProcessLogging.SelectionStart = lblProcessLogging.Text.Length;
            // scroll it automatically
            lblProcessLogging.ScrollToCaret();
        }

        private IGeometry GetDivisionPolygon()
        {
            IGeometry divisionPoly = null;
            string division = cboDivision.SelectedItem.ToString();

            IQueryFilter qf = new QueryFilterClass();
            qf.WhereClause = SearchPolygonFieldName + " = '" + division + "'";
            qf.SubFields = DivisionFeatClass.ShapeFieldName;
            IFeatureCursor cursor = DivisionFeatClass.Search(qf, false);
            IFeature divisionFeat = null;
            while ((divisionFeat = cursor.NextFeature()) != null)
            {
                divisionPoly = divisionFeat.ShapeCopy;
            }

            if (divisionFeat != null) { while (Marshal.ReleaseComObject(divisionPoly) > 0) { } }
            if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }
            if (cursor != null) { while (Marshal.ReleaseComObject(cursor) > 0) { } }

            return divisionPoly;
        }

        private void ExtractDataToFGDB(string division)
        {
            IGeoDataServer pParentGeoDataServer = null;
            IGeoDataServer pChildGeoDataServer = null;

            try
            {
                //**Create geodataservers for parent and child.                
                ExtractDataForm.Instance.LogMessage("     Initialise geo data server.");
                pParentGeoDataServer = InitGeoDataServer(null, true, GISWorkspace);

                string strPath = null;
                if (!GetEmptyFileGeodatabase(txtFGDBLocation.Text, division, out strPath))
                { throw new Exception("Failed to create file geodatabase. Process stopped."); }

                pChildGeoDataServer = InitGeoDataServer(strPath, false, null);

                Extract(pParentGeoDataServer, pChildGeoDataServer, "FAI_" + division + "_FGDB",
                    esriReplicaAccessType.esriReplicaAccessNone, GISWorkspace);

            }
            catch (Exception Ex)
            { ExtractDataForm.Instance.ErrorMessage(Ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); }
            finally
            {
                pParentGeoDataServer = null;
                pChildGeoDataServer = null;
            }
        }

        //**Extracts Data to FFDB.
        private ExtractionProgress extractFeatureProgress = null;
        private void Extract(IGeoDataServer parentGDS, IGeoDataServer
                                    childGDS, String replicaName, esriReplicaAccessType accessType, IWorkspace workspace)
        {
            IGPReplicaDataset pReplicaDataset = null;
            IGPReplicaDatasets pReplicaDatasets = null;
            IGPReplicaDescription pReplicaDesc = null;
            IReplicationAgent replicationAgent = null;

            try
            {
                IGeometry divisionPolygon = GetDivisionPolygon();

                //ExtractDataForm.Instance.LogMessage("Initiate Extract Process:");
                pReplicaDatasets = new GPReplicaDatasetsClass();
                List<string> classesAdded = new List<string>();

                foreach (TreeNode node in treeDatabase.Nodes)
                {
                    DatasetInfo dsInfo = (DatasetInfo)node.Tag;
                    if (dsInfo.datasetType == esriDatasetType.esriDTFeatureDataset && node.Checked)
                    {
                        ExtractDataForm.Instance.LogMessage("\t Dataset included: " + dsInfo.name);

                        //Process feature classes in this dataset
                        IFeatureDataset featDataset = ((IFeatureWorkspace)workspace).OpenFeatureDataset(dsInfo.name);
                        IFeatureClassContainer featureDatasetContainer = featDataset as IFeatureClassContainer;
                        for (int j = 0; j < featureDatasetContainer.ClassCount; j++)
                        {
                            ITable table = featureDatasetContainer.get_Class(j) as ITable;

                            pReplicaDataset = new GPReplicaDatasetClass();
                            pReplicaDataset.DatasetType = esriDatasetType.esriDTFeatureClass;
                            pReplicaDataset.Name = ((IDataset)table).BrowseName;// feature class;
                            pReplicaDataset.UseGeometry = false;
                            pReplicaDatasets.Add(pReplicaDataset);
                            ExtractDataForm.Instance.LogMessage("\t\t Feature Class included: " + ((IDataset)table).BrowseName);
                        }

                        //Process relationship classes in this dataset
                        IRelationshipClassContainer relClassContainer = featDataset as IRelationshipClassContainer;
                        IEnumRelationshipClass relClasses = relClassContainer.RelationshipClasses;
                        relClasses.Reset();
                        IRelationshipClass relClass = null;
                        while ((relClass = relClasses.Next()) != null)
                        {
                            string destinationClassName = ((IDataset)relClass.DestinationClass).BrowseName;
                            string originClassName = ((IDataset)relClass.OriginClass).BrowseName;
                            if (IsSelected(destinationClassName) && IsSelected(originClassName))
                            {
                                pReplicaDataset = new GPReplicaDatasetClass();
                                pReplicaDataset.DatasetType = esriDatasetType.esriDTRelationshipClass;
                                pReplicaDataset.Name = ((IDataset)relClass).BrowseName;// feature class;
                                pReplicaDataset.RelDestinationClass = ((IDataset)relClass.DestinationClass).BrowseName;
                                pReplicaDataset.RelOriginClass = ((IDataset)relClass.OriginClass).BrowseName;
                                pReplicaDataset.RelExtractDirection = esriRelExtractDirection.esriRelExtractDirectionNone;
                                pReplicaDataset.UseGeometry = false;
                                pReplicaDatasets.Add(pReplicaDataset);
                                ExtractDataForm.Instance.LogMessage("\t\t      Associated Relationsip included: " + ((IDataset)relClass).BrowseName);
                            }
                        }

                        INetworkCollection networkCollection = featDataset as INetworkCollection;
                        for (int j= 0; j < networkCollection.GeometricNetworkCount; j++)
                        {
                            IGeometricNetwork geomNetwork = networkCollection.get_GeometricNetwork(j);
                            pReplicaDataset = new GPReplicaDatasetClass();
                            pReplicaDataset.DatasetType = esriDatasetType.esriDTGeometricNetwork;
                            pReplicaDataset.Name = ((IDataset)geomNetwork).BrowseName;// feature class;
                            pReplicaDataset.UseGeometry = false;
                            pReplicaDatasets.Add(pReplicaDataset);
                            ExtractDataForm.Instance.LogMessage("\t\t Geometric Network included: " + ((IDataset)geomNetwork).BrowseName);
                        }

                    }
                    else if (dsInfo.datasetType == esriDatasetType.esriDTFeatureClass && node.Checked)
                    {
                        pReplicaDataset = new GPReplicaDatasetClass();
                        pReplicaDataset.DatasetType = esriDatasetType.esriDTFeatureClass;
                        pReplicaDataset.Name = node.Text;// feature class;
                        pReplicaDataset.UseGeometry = true;
                        pReplicaDatasets.Add(pReplicaDataset);
                        ExtractDataForm.Instance.LogMessage("\t Feature Class included: " + dsInfo.name);
                    }
                    else if (dsInfo.datasetType == esriDatasetType.esriDTTable && node.Checked)
                    {
                        pReplicaDataset = new GPReplicaDatasetClass();
                        pReplicaDataset.DatasetType = esriDatasetType.esriDTTable;
                        pReplicaDataset.Name = node.Text;// feature class;
                        pReplicaDataset.UseGeometry = false;
                        //pReplicaDataset.DefQuery = "1=0";
                        pReplicaDatasets.Add(pReplicaDataset);

                        ExtractDataForm.Instance.LogMessage("\t Table included: " + dsInfo.name);
                        foreach (DatasetInfo relInfo in dsInfo.AssociatedRelationships)
                        {
                            if (IsSelected(relInfo.DestinationClassName) && IsSelected(relInfo.OriginClassName))
                            {
                                pReplicaDataset = new GPReplicaDatasetClass();
                                pReplicaDataset.DatasetType = esriDatasetType.esriDTRelationshipClass;
                                pReplicaDataset.Name = relInfo.name;// feature class;
                                pReplicaDataset.RelDestinationClass = relInfo.DestinationClassName;
                                pReplicaDataset.RelOriginClass = relInfo.OriginClassName;
                                pReplicaDataset.RelExtractDirection = esriRelExtractDirection.esriRelExtractDirectionNone;
                                pReplicaDataset.UseGeometry = false;
                                pReplicaDatasets.Add(pReplicaDataset);
                                ExtractDataForm.Instance.LogMessage("\t      Associated Relationsip included: " + relInfo.name);
                            }
                        }
                    }
                }

                

                replicationAgent = new ReplicationAgentClass();
                //Set up information to track export progress
                IConnectionPointContainer connectionPointContainer = replicationAgent as IConnectionPointContainer;
                extractFeatureProgress = new ExtractionProgress(connectionPointContainer);
                ESRI.ArcGIS.GeoDatabaseDistributed.IFeatureProgress_Event extractionProgressEvent = replicationAgent as ESRI.ArcGIS.GeoDatabaseDistributed.IFeatureProgress_Event;
                extractionProgressEvent.Step += new ESRI.ArcGIS.GeoDatabaseDistributed.IFeatureProgress_StepEventHandler(extractionProgressEvent_Step);

                //Determine all of the counts of features that will be extracted
                extractFeatureProgress.DetermineToProcessCounts(workspace, divisionPolygon, pReplicaDatasets);

                // Set the replica description.
                pReplicaDesc = new GPReplicaDescriptionClass();
                pReplicaDesc.ReplicaDatasets = pReplicaDatasets;
                pReplicaDesc.ModelType = esriReplicaModelType.esriModelTypeFullGeodatabase;
                pReplicaDesc.SingleGeneration = (accessType == esriReplicaAccessType.esriReplicaAccessNone);
                pReplicaDesc.QueryGeometry = divisionPolygon;
                pReplicaDesc.SpatialRelation = esriSpatialRelEnum.esriSpatialRelIntersects;
                //pReplicaDesc.TransferRelatedObjects = true;

                //**Extract.
                //ExtractDataForm.Instance.LogMessage("Start Extract...");
                replicationAgent.ExtractData("", parentGDS, childGDS, pReplicaDesc);
                ExtractDataForm.Instance.LogMessage("\t Extract successful.");

                //Report on the extraction statistics
                ExtractDataForm.Instance.LogMessage("Extraction Results");
                foreach (KeyValuePair<ITable, int> ExpectedCount in extractFeatureProgress.TablesToProcess)
                {
                    string tableName = ((IDataset)ExpectedCount.Key).BrowseName;
                    try
                    {
                        int actualExtractedCount = 0;
                        if (extractFeatureProgress.TotalProcessedByTable.ContainsKey(ExpectedCount.Key))
                        {
                            actualExtractedCount = extractFeatureProgress.TotalProcessedByTable[ExpectedCount.Key];
                        }
                        ExtractDataForm.Instance.LogMessage(string.Format("\t {0}: Expected {1}, Actual {2}", ((IDataset)ExpectedCount.Key).BrowseName,
                            ExpectedCount.Value, actualExtractedCount));
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            catch (Exception Ex)
            { ExtractDataForm.Instance.ErrorMessage(Ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); }
            finally
            {
                pReplicaDataset = null;
                pReplicaDatasets = null;
                pReplicaDesc = null;
                replicationAgent = null;
            }
        }

        private bool IsSelected(string name)
        {
            bool isSelected = false;
            foreach (TreeNode node in treeDatabase.Nodes)
            {
                DatasetInfo dsInfo = (DatasetInfo)node.Tag;
                if (dsInfo.datasetType == esriDatasetType.esriDTFeatureDataset && node.Checked)
                {
                    if (dsInfo.name == name) { isSelected = true; }
                    else
                    {
                        foreach (DatasetInfo featClassInfo in dsInfo.ContainedFeatureClasses)
                        {
                            if (featClassInfo.name == name) { isSelected = true; }
                        }
                    }
                }
                else if (dsInfo.datasetType == esriDatasetType.esriDTFeatureClass && node.Checked && dsInfo.name == name) { isSelected = true; }
                else if (dsInfo.datasetType == esriDatasetType.esriDTTable && node.Checked && dsInfo.name == name) { return true; }                
            }
            return isSelected;
        }

        /// <summary>
        /// Updates progress within the console application.  If anything fails, it just catches and moves on.  This is for the case that UC4 fails updating the console
        /// window which has been seen in the past
        /// </summary>
        /// <param name="message"></param>
        /// <param name="currElementIndex"></param>
        /// <param name="totalElementCount"></param>
        public void ShowPrimaryPercentProgress(string message, double percent)
        {
            try
            {
                prgPrimaryProgress.Value = Int32.Parse(Math.Floor(percent).ToString());
            }
            catch (Exception ex)
            {

            }
            lblPrimaryProgress.Text = message;
            Application.DoEvents();
        }

        static void extractionProgressEvent_Step()
        {
            
        }

        /// <summary>
        /// Updates progress within the console application.  If anything fails, it just catches and moves on.  This is for the case that UC4 fails updating the console
        /// window which has been seen in the past
        /// </summary>
        /// <param name="message"></param>
        /// <param name="currElementIndex"></param>
        /// <param name="totalElementCount"></param>
        public void ShowSecondaryPercentProgress(string message, double percent)
        {
            try
            {
                prgSecondaryProgress.Value = Int32.Parse(Math.Floor(percent).ToString());
            }
            catch (Exception ex)
            {

            }

            lblSecondaryProgress.Text = message;
            Application.DoEvents();
        }

        private static IGeoDataServer InitGeoDataServer(String path, bool blnIsWorkspace, IWorkspace pWorkspace)
        {
            try
            {
                //Create the GeoDataServer and cast to the the IGeoDataServerInit interface.
                IGeoDataServer geoDataServer = new GeoDataServerClass();
                IGeoDataServerInit geoDataServerInit = (IGeoDataServerInit)geoDataServer;

                //Initialize the GeoDataServer and return it.
                if (blnIsWorkspace)
                    geoDataServerInit.InitWithWorkspace(pWorkspace);
                else
                {
                    //IWorkspaceFactory workspaceFactory = new FileGDBWorkspaceFactoryClass();
                    //IWorkspaceName workspaceName = workspaceFactory.Create(path.Replace(path.Split('\\')[path.Split('\\').Length - 1], string.Empty), path.Split('\\')[path.Split('\\').Length - 1], null, 0);
                    geoDataServerInit.InitFromFile(path);
                }
                return geoDataServer;
            }
            catch (Exception Ex)
            { ExtractDataForm.Instance.ErrorMessage(Ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); return null; }
        }


        private static bool GetEmptyFileGeodatabase(string gdbPath, string division, out string strPath)//string sourceDirectory, string targetDirectory)
        {
            //**This takes the empty file geodatbase and makes a copy of it, which then will be used to ingest extracted data.
            strPath = null;
            try
            {
                ExtractDataForm.Instance.LogMessage("     Create file geodatabase shell, to ingest data extracted");
                string assemblyLocation = Assembly.GetExecutingAssembly().Location;
                assemblyLocation = assemblyLocation.Substring(0, assemblyLocation.LastIndexOf("\\") + 1);
                string sourceDirectory = assemblyLocation + "EmptyFileGeodatabase.gdb";
                ExtractDataForm.Instance.LogMessage("     Shell FGDB: " + sourceDirectory);
                string targetDirectory = gdbPath + "\\" + DateTime.Now.ToString("yyyyMMdd") + "_" + division.Replace(' ', '_') + ".gdb";
                ExtractDataForm.Instance.LogMessage("     Target FGDB: " + targetDirectory);
                DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
                DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);
                strPath = targetDirectory;
                if (!CopyAll(diSource, diTarget)) return false;
                return true;
            }
            catch (Exception Ex)
            { ExtractDataForm.Instance.ErrorMessage(Ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); return false; }
        }


        private static bool CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            try
            {
                if (source == null)
                    throw new ArgumentNullException("sourceDirectory");

                if (source.Name.EndsWith(".mdb"))
                {
                    File.Copy(source.FullName, target.FullName);//.Substring(0, target.FullName.LastIndexOf(@"\")));
                    return true;
                }

                //Remove and re-add directory if it already exists
                if (Directory.Exists(target.FullName)) { Directory.Delete(target.FullName, true); }
                if (Directory.Exists(target.FullName) == false)
                {
                    Directory.CreateDirectory(target.FullName);
                }

                //Copy each file into it's new directory.
                foreach (FileInfo fi in source.GetFiles())
                {
                    //Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                    fi.CopyTo(System.IO.Path.Combine(target.ToString(), fi.Name), true);
                }

                //Copy each subdirectory using recursion.
                foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
                {
                    DirectoryInfo nextTargetSubDir =
                        target.CreateSubdirectory(diSourceSubDir.Name);
                    CopyAll(diSourceSubDir, nextTargetSubDir);
                }
                return true;
            }
            catch (Exception Ex)
            { ExtractDataForm.Instance.ErrorMessage(Ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); return false; }
        }

        private void btnExtract_Click(object sender, EventArgs e)
        {
            ExtractDataForm.Instance.LogMessage("Beginning extract process");
            Stopwatch SWExtract = new Stopwatch();
            SWExtract.Start();

            //First add necessary fields to maintain objectid and global ID mappings
            AddOriginalFields();

            ExtractDataToFGDB(cboDivision.SelectedItem.ToString());
            SWExtract.Stop();
            ExtractDataForm.Instance.LogMessage("Extract process completed");
            ExtractDataForm.Instance.LogMessage(string.Format("Total execution time: {0} hours {1} minutes {2} seconds", SWExtract.Elapsed.Hours,
                SWExtract.Elapsed.Minutes, SWExtract.Elapsed.Seconds));
        }

        private void txtExtractionPolygon_TextChanged(object sender, EventArgs e)
        {
            lblPolygon.Text = txtExtractionPolygon.Text;
            SearchPolygonFeatureClassName = txtExtractionPolygon.Text;
        }

        private void txtExtractionField_TextChanged(object sender, EventArgs e)
        {
            SearchPolygonFieldName = txtExtractionField.Text;
        }

    }

    public class DatasetInfo : IComparable<DatasetInfo>
    {
        public string name;
        public esriDatasetType datasetType;
        public esriFeatureType featureType;
        public esriGeometryType shapeType;
        public string OriginClassName;
        public string DestinationClassName;
        public List<DatasetInfo> AssociatedRelationships = new List<DatasetInfo>();
        public List<DatasetInfo> ContainedFeatureClasses = new List<DatasetInfo>();

        public void AddRelationship(DatasetInfo relInfo)
        {
            if (AssociatedRelationships == null) { AssociatedRelationships = new List<DatasetInfo>(); }
            AssociatedRelationships.Add(relInfo);
        }

        public void AddFeatureClass(DatasetInfo featClassInfo)
        {
            if (ContainedFeatureClasses == null) { ContainedFeatureClasses = new List<DatasetInfo>(); }
            ContainedFeatureClasses.Add(featClassInfo);
        }

        public int CompareTo(DatasetInfo other)
        {
            return this.name.CompareTo(other.name);
        }

        public override string ToString()
        {
            return name;
        }

        public override bool Equals(object obj)
        {
            if (obj is DatasetInfo)
            {
                return this.name.Equals(((DatasetInfo)obj).name);
            }
            else { return false; }
        }
    }
}
