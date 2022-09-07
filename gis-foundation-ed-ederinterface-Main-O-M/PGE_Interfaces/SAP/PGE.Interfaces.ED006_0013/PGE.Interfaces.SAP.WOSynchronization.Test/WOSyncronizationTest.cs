using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml;
using NUnit.Framework;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace PGE.Interfaces.SAP.WOSynchronization
{
    [TestFixture]
    public class WOSyncronizationTest
    {
        #region Test Methods

        [SetUp]
        public void SetUp()
        {
            LicenseManager.ChecketOutLicenses();
        }

        [TearDown]
        public void TearDown()
        {
            LicenseManager.Shutdown();
        }

        /// <summary>
        /// Tests the readability of config file from custom location
        /// </summary>
        [Test]
        public void ReadConfigTest()
        {
            Assert.IsNotNull(Config.ReadConfigFromExeLocation());
        }

        /// <summary>
        /// Tests the existence of the WIPClouds for a given JobNumber and the number of the features with that JOBNUMBER
        /// </summary>
        [Test]
        public void SearchWIPCloudTest()
        {

            IWorkspace ws = GetSDEWorkSpace();
            Assert.IsNotNull(ws);

            //Create WIP Cloud table
            TableData wipCloud = new TableData(ws, "EDGIS.WIPCloud");
            Assert.IsNotNull(wipCloud);
            wipCloud.JobOrderNumberFieldName = ResourceConstants.FieldNames.WIPJOBORDER;
            //Get any record
            Assert.IsFalse(wipCloud.RecordExists(null));
            //Get List of all record
            IList<IRowData2> result = wipCloud.SearchTable(null, new string[] { ResourceConstants.FieldNames.WIPJOBORDER });
            Assert.IsNotNull(result);
            Assert.Greater(result.Count, 0);

            //Get List of all record
            result = null;
            string whereClause = "JOBNUMBER='000030008268'";
            //Get any record
            Assert.IsTrue(wipCloud.RecordExists("000030008268"));
            result = wipCloud.SearchTable(whereClause, new string[] { ResourceConstants.FieldNames.WIPJOBORDER });
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 2);


            //Get List of all record
            result = null;
            whereClause = "JOBNUMBER in ('000030004029','000030008268')";
            //Get any record
            Assert.IsTrue(wipCloud.RecordExists("000030008269"));
            result = wipCloud.SearchTable(whereClause, new string[] { ResourceConstants.FieldNames.WIPJOBORDER });
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 2);

            //Get List of all record
            result = null;
            whereClause = "JOBNUMBER='600030004069'";
            //Get any record
            Assert.IsFalse(wipCloud.RecordExists("600030004069"));
            result = wipCloud.SearchTable(whereClause, new string[] { ResourceConstants.FieldNames.WIPJOBORDER });
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 0);
        }
         
        /// <summary>
        /// Tests the Search/Insert/Update/Cleanup functionalities of PMorder
        /// </summary>
        [Test]
        public void PMOrderTest()
        {

            IWorkspace ws = GetSDEWorkSpace();
            Assert.IsNotNull(ws);
            (ws as IWorkspaceEdit).StartEditing(false);

            //Create PMOrder table data instance
            PMOrderTableData pmOrderTable = new PMOrderTableData(ws, "GULIVT.PMOrder");
            Assert.IsNotNull(pmOrderTable);
            pmOrderTable.JobOrderNumberFieldName = ResourceConstants.FieldNames.JOBORDER;
            //No records if JobNumber is null
            Assert.IsFalse(pmOrderTable.RecordExists(null));
            //Get List of all record
            IList<IRowData2> result = pmOrderTable.SearchTable(null, new string[] { ResourceConstants.FieldNames.JOBORDER });
            Assert.IsNotNull(result);
            Assert.Greater(result.Count, 0);

            //Get List of records
            result = null;
            string whereClause = "JOBNUMBER='200030011449'";
            //Get any record
            Assert.IsFalse(pmOrderTable.RecordExists("200030011449"));
            result = pmOrderTable.SearchTable(whereClause, new string[] { ResourceConstants.FieldNames.JOBORDER });
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 0);

            //create sample IRowData
            IRowData2 sampleData = CreateIRowData2_1();
            //Insert this record
            pmOrderTable.InsertRow(sampleData);
            //Checks whether record is added successfully
            Assert.IsTrue(pmOrderTable.RecordExists("200030011449"));
            //Verify record count
            result = pmOrderTable.SearchTable(whereClause, new string[] { ResourceConstants.FieldNames.JOBORDER, "JobType" });
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 1);
            //Verify one of the attribute
            Assert.AreEqual(result[0].FieldValues["JobType"], "E060");


            //Update this record
            IRowData2 updatedData = CreateIRowData2_2();
            pmOrderTable.UpdateRow(updatedData);
            //Checks whether record is still exists
            Assert.IsTrue(pmOrderTable.RecordExists("200030011449"));
            //Verify record count is still one
            result = pmOrderTable.SearchTable(whereClause, new string[] { ResourceConstants.FieldNames.JOBORDER, "JobType" });
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 1);
            //Verify updated attribute
            Assert.AreEqual(result[0].FieldValues["JobType"], "E760");

            //Verify record count is still one
            int beforeCount = pmOrderTable.SearchTable(whereClause, new string[] { ResourceConstants.FieldNames.JOBORDER }).Count;
            //Clean the newly inserted on updated record
            pmOrderTable.CleanRecords(whereClause);
            //Checks whether record is still exists
            Assert.IsFalse(pmOrderTable.RecordExists("200030011449"));
            int afterCount = pmOrderTable.SearchTable(whereClause, new string[] { ResourceConstants.FieldNames.JOBORDER }).Count;
            Assert.AreEqual(beforeCount - result.Count, afterCount);

            (ws as IWorkspaceEdit).StopEditing(true);

        }

        /// <summary>
        /// Test the export to CSV functionality of the data.
        /// </summary>
        [Test]
        public void ExportToCSVTest()
        {
            IList<IRowData2> data = new List<IRowData2>();
            data.Add(CreateIRowData2_1());
            data.Add(CreateIRowData2_2());
            data.Add(CreateIRowData2_3());
            Assert.AreEqual(data.Count, 3);
            string fileName = @"C:\Project\PGE\trunk\ED\Integration\SAP\PGE.Interfaces.SAP.WOSynchronization\bin\Debug\Test.csv";
            if (System.IO.File.Exists(fileName)) { System.IO.File.Delete(fileName); }
            Assert.IsFalse(System.IO.File.Exists(fileName));
            CSVFileWriter.Export(data, fileName);
            Assert.IsTrue(System.IO.File.Exists(fileName));
        }

        /// <summary>
        /// Tests the processing of some sample IRowData2 of PMorder items. This validates whether records were successfully inserted/updated.
        /// </summary>
        [Test]
        public void ProcessRows()
        {
            List<IRowData2> data = new List<IRowData2>();
            data.Add(CreateIRowData2_1());
            data.Add(CreateIRowData2_2());
            data.Add(CreateIRowData2_3());
            Assert.AreEqual(data.Count, 3);

            string whereClause = "JOBNUMBER='200030011449' or JOBNUMBER='000030011448'";

            //Clear the data
            IWorkspace ws = GetSDEWorkSpace();
            PMOrderTableData order = new PMOrderTableData(ws, "GULIVT.PMOrder");
            order.JobOrderNumberFieldName = ResourceConstants.FieldNames.JOBORDER;
            IList<IRowData2> result = order.SearchTable(null, new string[] { ResourceConstants.FieldNames.JOBORDER });
            int initialCount = result.Count;

            SAPPMOrderSync syncOrder = new SAPPMOrderSync();
            syncOrder.OutPutData(data);

            result = order.SearchTable(null, new string[] { ResourceConstants.FieldNames.JOBORDER });
            Assert.Greater(result.Count, initialCount);

            (ws as IWorkspaceEdit).StartEditing(false);
            //clean the newly inserted records
            order.CleanRecords(whereClause);
            (ws as IWorkspaceEdit).StopEditing(true);
            order.Dispose();

            order = new PMOrderTableData(ws, "GULIVT.PMOrder");
            order.JobOrderNumberFieldName = ResourceConstants.FieldNames.JOBORDER;
            result = order.SearchTable(null, new string[] { ResourceConstants.FieldNames.JOBORDER });
            Assert.AreEqual(result.Count, initialCount);


        }

        /// <summary>
        /// Test the conversion of PMOrder messages to list of IRowData2 by validating the message information/values
        /// </summary>
        [Test]
        public void ConvertToIRowDataTest()
        {
            string xmlContent = "<ORDERS><Table><Order>000030011443</Order><Type>E070</Type><OrderCreateDate>2010-10-27</OrderCreateDate><JobDesription>TESTINGUC3NOTIF1212</JobDesription><MainWorkCenter>SNFRAN</MainWorkCenter><JobOwner>KKD4</JobOwner><Location>ETS</Location><Equipment/><MaintActType>17B</MaintActType><WorkType>501</WorkType><LegacyMapNumber/><Region>R1-BA</Region><Division>D1</Division><Status>UNSE</Status></Table><Table><Order>000030011449</Order><Type>E060</Type><OrderCreateDate>2010-11-30</OrderCreateDate><JobDesription>EP852TIGERROUTESANFRANCISCOTEST1</JobDesription><MainWorkCenter>SNFRAN</MainWorkCenter><JobOwner>GXY5</JobOwner><Location>ED</Location><Equipment/><MaintActType>16G</MaintActType><WorkType>012</WorkType><LegacyMapNumber/><Region>R1-BA</Region><Division>D1</Division><Status>UNSE</Status></Table><Table><Order>000030011457</Order><Type>E060</Type><OrderCreateDate>2010-12-15</OrderCreateDate><JobDesription>GEPCREATEORDER:GENMAINTTASKLISTEST1</JobDesription><MainWorkCenter>SNFRAN</MainWorkCenter><JobOwner>GXY5</JobOwner><Location>ED</Location><Equipment/><MaintActType>10J</MaintActType><WorkType>203</WorkType><LegacyMapNumber/><Region>R1-BA</Region><Division>D1</Division><Status>UNSE</Status></Table></ORDERS>";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlContent);
            //Get the Orders Node
            XmlNode topNode = xmlDoc.FirstChild;
            XMLProcessor processor = new XMLProcessor();
            IList<IRowData2> convertedList = processor.ConvertToIRowData(topNode);
            Assert.AreEqual(topNode.ChildNodes.Count, convertedList.Count);
            //get any ORder message node
            XmlNode singleOrder = topNode.ChildNodes[0];
            //add this node at the end and convert to IRowData2 list
            topNode.AppendChild(singleOrder.Clone());
            Assert.AreEqual(topNode.ChildNodes.Count, 4);
            //Process the single node
            convertedList = processor.ConvertToIRowData(topNode);
            Assert.AreEqual(topNode.ChildNodes.Count, convertedList.Count);
            //Validate any XML node value
            Assert.AreEqual(convertedList[0].FieldValues["ORDER"], "000030011443");
            //Validate any XML node value
            Assert.AreEqual(convertedList[1].FieldValues["TYPE"], "E060");
            //Validate any XML node value
            Assert.AreEqual(convertedList[2].FieldValues["LegacyMapNumber".ToUpper()], string.Empty);
            //Validate any XML node value
            Assert.AreEqual(convertedList[3].FieldValues["ORDER"], "000030011443");
        }

        /// <summary>
        /// Tests the processing of XML files from scratch. This also creates XML files with some sample data and processes.
        /// </summary>
        [Test]
        public void ProcessFilesTest()
        {
            //Get the archive location directory
            string archiveDir = Config.ReadConfigFromExeLocation().AppSettings.Settings["ArchiveLocation"].Value;
            int initialArchiveFileCount = System.IO.Directory.GetFiles(archiveDir).Length;
            string fileDirectory = @"C:\Naidu\SAPGISReports\Test";
            string[] xmlFiles = CreateSampleXMLFiles(fileDirectory);
            //Get the Number of files

            int initialFileCount = System.IO.Directory.GetFiles(fileDirectory).Length;
            PMOrderFileProcessor fileProcessor = new PMOrderFileProcessor();
            fileProcessor.ProcessFiles(xmlFiles);
            int finalFileCount = System.IO.Directory.GetFiles(fileDirectory).Length;
            int finalArchiveFileCount = System.IO.Directory.GetFiles(archiveDir).Length;
            Assert.AreEqual(initialFileCount - xmlFiles.Length, finalFileCount);
            Assert.AreEqual(initialArchiveFileCount + xmlFiles.Length, finalArchiveFileCount);

        }

        #endregion Test Methods

        #region Helper Methods for test methods

        /// <summary>
        /// Retrieves the Database workspace by reading the database credentials from the config file.
        /// </summary>
        /// <returns>Returns the reference to the Database</returns>
        private IWorkspace GetSDEWorkSpace()
        {

            KeyValueConfigurationCollection appSettings = Config.ReadConfigFromExeLocation().AppSettings.Settings;
            IPropertySet propertySet = new PropertySetClass();
            propertySet.SetProperty("instance", appSettings["DBConnec"].Value);
            propertySet.SetProperty("User", appSettings["UserName"].Value);
            propertySet.SetProperty("Password", appSettings["Password"].Value);
            propertySet.SetProperty("version", appSettings["WIPCloudVersion"].Value);

            IWorkspaceFactory workspaceFactory = new SdeWorkspaceFactory();
            IWorkspace wspace = workspaceFactory.Open(propertySet, 0);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(workspaceFactory);
            return wspace;


        }

        /// <summary>
        /// Create sample data of IRowData2 type
        /// </summary>
        /// <returns></returns>
        private IRowData2 CreateIRowData2_1()
        {
            IRowData2 rowData = new RowData2();
            Dictionary<string, string> fieldValues = new Dictionary<string, string>();
            fieldValues.Add("Order".ToUpper(), "200030011449");
            fieldValues.Add("Type".ToUpper(), "E060");
            fieldValues.Add("Location".ToUpper(), "ED");
            fieldValues.Add("Status".ToUpper(), "UNSE");
            fieldValues.Add("MaintActType".ToUpper(), "16G");
            fieldValues.Add("JobOwner".ToUpper(), "GXY5");
            rowData.FieldValues = fieldValues;
            rowData.FacilityID = "200030011449";
            return rowData;
        }

        /// <summary>
        /// Create sample data of IRowData2 type
        /// </summary>
        /// <returns></returns>
        private IRowData2 CreateIRowData2_2()
        {
            IRowData2 rowData = new RowData2();
            Dictionary<string, string> fieldValues = new Dictionary<string, string>();
            fieldValues.Add("Order".ToUpper(), "200030011449");
            fieldValues.Add("Type".ToUpper(), "E760");//Modified value from that in 'CreateIRowData2_1' method
            fieldValues.Add("Location".ToUpper(), "ED");
            fieldValues.Add("Status".ToUpper(), "UNSE");
            fieldValues.Add("MaintActType".ToUpper(), "16G");
            fieldValues.Add("JobOwner".ToUpper(), "GXY5");
            rowData.FieldValues = fieldValues;
            rowData.FacilityID = "200030011449";
            return rowData;
        }

        /// <summary>
        /// Create sample data of IRowData2 type
        /// </summary>
        /// <returns></returns>
        private IRowData2 CreateIRowData2_3()
        {
            IRowData2 rowData = new RowData2();
            Dictionary<string, string> fieldValues = new Dictionary<string, string>();
            fieldValues.Add("Order".ToUpper(), "000030011448");//Modified value from that in 'CreateIRowData2_1' & 'CreateIRowData2_2' methods
            fieldValues.Add("Type".ToUpper(), "E760");//Modified value from that in 'CreateIRowData2_1' method but same as 'CreateIRowData2_2' method
            fieldValues.Add("Location".ToUpper(), "ED");
            fieldValues.Add("Status".ToUpper(), "UNSE");
            fieldValues.Add("MaintActType".ToUpper(), "16G");
            fieldValues.Add("JobOwner".ToUpper(), "GXY5");
            rowData.FieldValues = fieldValues;
            rowData.FacilityID = "000030011448";
            return rowData;
        }

        /// <summary>
        /// Creates XML files with some sample data jus' for testing purposes.
        /// </summary>
        /// <param name="directoryPath">Directory path to create sample XML files</param>
        /// <returns>Returns the file name array of the XML files created for testing purpose.</returns>
        private string[] CreateSampleXMLFiles(string directoryPath)
        {
            string[] fileNames = new string[3];
            if (!System.IO.Directory.Exists(directoryPath))
            {
                System.IO.Directory.CreateDirectory(directoryPath);
            }

            string xmlContent = "<ORDERS><Table><Order>000030011443</Order><Type>E070</Type><OrderCreateDate>2010-10-27</OrderCreateDate><JobDesription>TESTINGUC3NOTIF1212</JobDesription><MainWorkCenter>SNFRAN</MainWorkCenter><JobOwner>KKD4</JobOwner><Location>ETS</Location><Equipment/><MaintActType>17B</MaintActType><WorkType>501</WorkType><LegacyMapNumber/><Region>R1-BA</Region><Division>D1</Division><Status>UNSE</Status></Table><Table><Order>000030011449</Order><Type>E060</Type><OrderCreateDate>2010-11-30</OrderCreateDate><JobDesription>EP852TIGERROUTESANFRANCISCOTEST1</JobDesription><MainWorkCenter>SNFRAN</MainWorkCenter><JobOwner>GXY5</JobOwner><Location>ED</Location><Equipment/><MaintActType>16G</MaintActType><WorkType>012</WorkType><LegacyMapNumber/><Region>R1-BA</Region><Division>D1</Division><Status>UNSE</Status></Table><Table><Order>000030011457</Order><Type>E060</Type><OrderCreateDate>2010-12-15</OrderCreateDate><JobDesription>GEPCREATEORDER:GENMAINTTASKLISTEST1</JobDesription><MainWorkCenter>SNFRAN</MainWorkCenter><JobOwner>GXY5</JobOwner><Location>ED</Location><Equipment/><MaintActType>10J</MaintActType><WorkType>203</WorkType><LegacyMapNumber/><Region>R1-BA</Region><Division>D1</Division><Status>UNSE</Status></Table></ORDERS>";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlContent);
            string fileName = System.IO.Path.Combine(directoryPath, "test_1.xml");
            xmlDoc.Save(fileName);
            fileNames[0] = fileName;

            xmlContent = "<ORDERS><Table><Order>000030011441</Order><Type>E076</Type><OrderCreateDate>2010-10-27</OrderCreateDate><JobDesription>TESTINGUC3NOTIF1212</JobDesription><MainWorkCenter>SNFRAN</MainWorkCenter><JobOwner>KKD4</JobOwner><Location>ETS</Location><Equipment/><MaintActType>17B</MaintActType><WorkType>501</WorkType><LegacyMapNumber/><Region>R1-BA</Region><Division>D1</Division><Status>UNSE</Status></Table><Table><Order>000030011449</Order><Type>E060</Type><OrderCreateDate>2010-11-30</OrderCreateDate><JobDesription>EP852TIGERROUTESANFRANCISCOTEST1</JobDesription><MainWorkCenter>SNFRAN</MainWorkCenter><JobOwner>GXY5</JobOwner><Location>ED</Location><Equipment/><MaintActType>16G</MaintActType><WorkType>012</WorkType><LegacyMapNumber/><Region>R1-BA</Region><Division>D1</Division><Status>UNSE</Status></Table><Table><Order>000030011457</Order><Type>E060</Type><OrderCreateDate>2010-12-15</OrderCreateDate><JobDesription>GEPCREATEORDER:GENMAINTTASKLISTEST1</JobDesription><MainWorkCenter>SNFRAN</MainWorkCenter><JobOwner>GXY5</JobOwner><Location>ED</Location><Equipment/><MaintActType>10J</MaintActType><WorkType>203</WorkType><LegacyMapNumber/><Region>R1-BA</Region><Division>D1</Division><Status>UNSE</Status></Table></ORDERS>";
            xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlContent);
            fileName = System.IO.Path.Combine(directoryPath, "test_2.xml");
            xmlDoc.Save(fileName);
            fileNames[1] = fileName;

            xmlContent = "<ORDERS><Table><Order>000030011446</Order><Type>E078</Type><OrderCreateDate>2010-10-27</OrderCreateDate><JobDesription>TESTINGUC3NOTIF1212</JobDesription><MainWorkCenter>SNFRAN</MainWorkCenter><JobOwner>KKD4</JobOwner><Location>ETS</Location><Equipment/><MaintActType>17B</MaintActType><WorkType>501</WorkType><LegacyMapNumber/><Region>R1-BA</Region><Division>D1</Division><Status>UNSE</Status></Table><Table><Order>000030011449</Order><Type>E060</Type><OrderCreateDate>2010-11-30</OrderCreateDate><JobDesription>EP852TIGERROUTESANFRANCISCOTEST1</JobDesription><MainWorkCenter>SNFRAN</MainWorkCenter><JobOwner>GXY5</JobOwner><Location>ED</Location><Equipment/><MaintActType>16G</MaintActType><WorkType>012</WorkType><LegacyMapNumber/><Region>R1-BA</Region><Division>D1</Division><Status>UNSE</Status></Table><Table><Order>000030011457</Order><Type>E060</Type><OrderCreateDate>2010-12-15</OrderCreateDate><JobDesription>GEPCREATEORDER:GENMAINTTASKLISTEST1</JobDesription><MainWorkCenter>SNFRAN</MainWorkCenter><JobOwner>GXY5</JobOwner><Location>ED</Location><Equipment/><MaintActType>10J</MaintActType><WorkType>203</WorkType><LegacyMapNumber/><Region>R1-BA</Region><Division>D1</Division><Status>UNSE</Status></Table></ORDERS>";
            xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlContent);
            fileName = System.IO.Path.Combine(directoryPath, "test_3.xml");
            xmlDoc.Save(fileName);
            fileNames[2] = fileName;

            return fileNames;
        }

        #endregion Helper Methods for test methods
    }
}
