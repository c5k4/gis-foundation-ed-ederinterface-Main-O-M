using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Telvent.PGE.Framework.FieldTransformers;
using Telvent.PGE.Test.Data;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;

using ESRI.ArcGIS;
using ESRI.ArcGIS.ADF.COMSupport;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;

using Telvent.PGE.SAP.RowTransformers;
using Telvent.PGE.Framework;
using Telvent.PGE.Framework.Data;
using Telvent.PGE.Framework.Utilities;
using Telvent.PGE.Framework.Exceptions;
using Miner.Geodatabase.GeodatabaseManager;
using Miner.Process.GeodatabaseManager;
using Telvent.PGE.SAP;
using Telvent.PGE.GDBM;
using Telvent.Delivery.Diagnostics;
using Telvent.Delivery.Systems;



namespace Telvent.PGE.Test
{
    [TestFixture]
    public class SAPComponentTests
    {
        private TestFeature transformer;
        private TestFeature transformerUnit1;
        private TestFeature ugstruct;
        protected static Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public SAPComponentTests()
        {

        }
        [SetUp]
        protected void SetUp()
        {
            transformer = new TestFeature(
                new string[] { "OperatingVoltage", "CircuitID", "Status", "LastModified", "CreationDate", "LastUser", "CreationUser", "AssetID" },
                new object[] { 4, "129981", "1", new DateTime(2012, 4, 25, 12, 30, 0, 0), null, "John", "{000-000-000}" },
                "Transformer");
            transformer.SetXY(41.113, 121.112);
            TestFeature relatedRow = new TestFeature(
                new string[] { "field1", "DeviceGroupNumber", "field3" },
                new object[] { "a", "x", "c" },
                "TestTable");
            transformer.AddRelatedRow(relatedRow, "DeviceGroup_Transformer");
            transformerUnit1 = new TestFeature(
                new string[] { "TransformerType", "UnitID", "field3", "Status" },
                new object[] { "3", "1234", "c", "5" },
                "TransformerUnit");
            transformerUnit1.SubtypeCode = 1;
            transformer.AddRelatedRow(transformerUnit1, "Transformer_TransformerUnit");
            transformerUnit1.AddRelatedRow(transformer, "Transformer_TransformerUnit");
            ugstruct = new TestFeature(
                new string[] { "field1", "DeviceGroupNumber", "StructureNumber", "BoostPercent", "BuckPercent" },
                new object[] { "a", "x", "c", null, 12.5M },
                "TestTable");
            relatedRow.AddRelatedRow(ugstruct, "DeviceGroup_SubsurfaceStructure");

            string domainsXmlFile = Path.Combine(GetPath(), @"Output\Debug\Config\Domains.xml");

            XmlDocument document = new XmlDocument();

            // this temporarily switched by Junwei, it doesn't work the other way
            document.Load(domainsXmlFile);
            //document.Load(@"C:\code\Telvent.PGE\Telvent.PGE.SAP\XMLConfig\Domains.xml");
            DomainManager.Instance.Initialize(document.FirstChild, "SAP");
        }
        [Test]
        public void LoggingTest()
        {
            Log4NetLogger.ConfigureLog("C:\\");
            Log4NetLogger log = new Log4NetLogger("TestLogger");
            log.Debug("test");
            log.Error("test", new InvalidConfigurationException("test"));
            _log.Error("test 2");
        }
       
        [Test]
        public void RelationshipFieldTransformerTests()
        {
            BaseFieldTransformer test = new RelationshipFieldTransformer();
            string xml = "<RelationshipFieldTransformer RelationshipName=\"DeviceGroup_Transformer\">" +
              "<FieldMapper TransformerType=\"Telvent.PGE.Framework.FieldTransformers.FieldValueTransformer, Telvent.PGE.Framework\">" +
              "<FieldValueTransformer FieldName=\"DeviceGroupNumber\"/></FieldMapper></RelationshipFieldTransformer>";
            XmlNode config = StaticUtilities.String2Node(xml);
            Assert.IsTrue(test.Initialize(config));
            Assert.AreEqual("x", test.GetValue<string>(transformer));
        }
        [Test]
        public void MultiRelationshipTransformerTests()
        {
            BaseFieldTransformer test = new MultiRelationshipFieldTransformer();
            string xml = "<MultiRelationshipFieldTransformer>" +
    "<Relationships>" +
        "<MultiRelationshipFieldTransformer RelationshipName=\"SubsurfaceStructure_Transformer\" >" +
            "<FieldMapper TransformerType=\"Telvent.PGE.Framework.FieldTransformers.FieldValueTransformer, Telvent.PGE.Framework\">" +
                "<FieldValueTransformer FieldName=\"StructureNumber\" />" +
            "</FieldMapper>" +
        "</MultiRelationshipFieldTransformer>" +
        "<MultiRelationshipFieldTransformer RelationshipName=\"DeviceGroup_Transformer\" >" +
            "<Relationships>" +
                "<MultiRelationshipFieldTransformer RelationshipName=\"DeviceGroup_SubsurfaceStructure\" >" +
                    "<FieldMapper TransformerType=\"Telvent.PGE.Framework.FieldTransformers.FieldValueTransformer, Telvent.PGE.Framework\">" +
                        "<FieldValueTransformer FieldName=\"StructureNumber\" />" +
                    "</FieldMapper>" +
                "</MultiRelationshipFieldTransformer>" +
            "</Relationships>" +
        "</MultiRelationshipFieldTransformer>" +
    "</Relationships>" +
"</MultiRelationshipFieldTransformer>";
            XmlNode config = StaticUtilities.String2Node(xml);
            Assert.IsTrue(test.Initialize(config));
            Assert.AreEqual("c", test.GetValue<string>(transformer));
        }
        [Test]
        public void AlternateFieldTransformerTests()
        {
            BaseFieldTransformer test = new AlternateFieldTransformer();
            string xml = "<AlternateFieldTransformer FieldName=\"LastUser\" AlternateField=\"CreationUser\"></AlternateFieldTransformer>";
            XmlNode config = StaticUtilities.String2Node(xml);
            Assert.IsTrue(test.Initialize(config));
            Assert.AreEqual("John", test.GetValue<string>(transformer));
        }
        [Test]
        public void AlternateDateTimeTransformerTests()
        {
            BaseFieldTransformer test = new AlternateDateTimeTransformer();
            string xml = "<AlternateDateTimeTransformer FieldName=\"LastModified\" AlternateField=\"CreationDate\" DateFormat=\"yyyyMMdd\"></AlternateDateTimeTransformer>";
            XmlNode config = StaticUtilities.String2Node(xml);
            Assert.IsTrue(test.Initialize(config));
            Assert.AreEqual("20120425", test.GetValue<string>(transformer));
        }
        [Test]
        public void AlternateNumberTransformerTests()
        {
            BaseFieldTransformer test = new AlternateNumberTransformer();
            string xml = "<AlternateNumberTransformer FieldName=\"BoostPercent\" AlternateField=\"BuckPercent\" FieldValue=\"BOOST\" AlternateValue=\"BUCK\" UnspecifiedValue=\"UNSP\"/>";
            XmlNode config = StaticUtilities.String2Node(xml);
            Assert.IsTrue(test.Initialize(config));
            Assert.AreEqual("BUCK", test.GetValue<string>(ugstruct));
            xml = "<AlternateNumberTransformer FieldName=\"BoostPercent\" AlternateField=\"BuckPercent\" UnspecifiedValue=\"UNSP\"/>";
            config = StaticUtilities.String2Node(xml);
            Assert.IsTrue(test.Initialize(config));
            Assert.AreEqual("12.5", test.GetValue<string>(ugstruct));
        }
        [Test]
        public void DateTimeTransformerTests()
        {
            BaseFieldTransformer test = new DateTimeTransformer();
            string xml = "<DateTimeTransformer FieldName=\"LastModified\" DateFormat=\"yyyyMMdd\"></DateTimeTransformer>";
            XmlNode config = StaticUtilities.String2Node(xml);
            Assert.IsTrue(test.Initialize(config));
            Assert.AreEqual("20120425", test.GetValue<string>(transformer));
        }
        [Test]
        public void DomainTransformerTests()
        {
            BaseFieldTransformer test = new DomainTransformer();
            string xml = "<DomainTransformer FieldName=\"OperatingVoltage\" DomainName=\"Primary Voltage\"></DomainTransformer>";
            XmlNode config = StaticUtilities.String2Node(xml);
            Assert.IsTrue(test.Initialize(config));
            Assert.AreEqual("4", test.GetValue<string>(transformer));
        }
        [Test]
        public void SubtypeDomainTransformerTests()
        {
            BaseFieldTransformer test = new SubtypeDomainTransformer();
            string xml = "<SubtypeDomainTransformer FieldName=\"TransformerType\" >" +
                "<Subtypes><SubtypeDomainMap Subtype=\"1\" Domain=\"Transformer Unit Type Overhead\"/> <SubtypeDomainMap Subtype=\"2\" Domain=\"Transformer Unit Type SubSurface\"/></Subtypes>" +
                "</SubtypeDomainTransformer>";
            XmlNode config = StaticUtilities.String2Node(xml);
            Assert.IsTrue(test.Initialize(config));
            Assert.AreEqual("03", test.GetValue<string>(transformerUnit1));
        }
        
        [Test]
        public void BaseFieldTransformerTests()
        {
            TestFeature row = new TestFeature(
                new string[] { "field1", "field2", "field3" },
                new string[] { "a", "b", "c" },
                "TestTable");
            BaseFieldTransformer test = new FieldValueTransformer();
            //This should be changed to handle the FieldValueTransformer
            string xml = "<FieldValueTransformer FieldName=\"field2\"></FieldValueTransformer>";
            XmlNode config = StaticUtilities.String2Node(xml);
            Assert.IsTrue(test.Initialize(config));
            Assert.AreEqual("b", test.GetValue<string>(row));
        }
        [Test]
        public void BaseRowTransformerTests()
        {

            XmlDocument document = new XmlDocument();
            document.Load(Path.Combine(GetPath(), @"Telvent.PGE.SAP\Config\TestConfig.xml"));
            SystemMapper mapper = XmlFacade.DeserializeFromXml<SystemMapper>(document);
            mapper.Initialize();
            List<IRowData> data = mapper.TrackedClasses[1].RowTransformer.ProcessRow(transformer, null, ChangeType.Insert);
            Telvent.PGE.SAP.Data.SAPRowData test = new Telvent.PGE.SAP.Data.SAPRowData(new int[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }, new string[] { "99", "x", "129981", "4", "41.113", "121.112", "John", "20120425", "20120425", "{000-000-000}" });
            Assert.AreEqual(test, data[0]);
            Assert.AreEqual("{000-000-000}", data[0].AssetID);
        }
        [Test]
        public void RelationshipRowTransformerTests()
        {
            //GISSAPIntegrator integrator = new GISSAPIntegrator(null, null);
            //integrator.Initialize();

            XmlDocument document = new XmlDocument();
            document.Load(Path.Combine(GetPath(), @"Telvent.PGE.Test\TestXml.xml"));
            //document.Load(@"C:\Code\Telvent.PGE\Telvent.PGE.Test\TestXml.xml");
            SystemMapper mapper = XmlFacade.DeserializeFromXml<SystemMapper>(document);
            mapper.Initialize();
            //integrator._systemMapper = mapper;

            TestFeature regulator = new TestFeature(
                new string[] { "OperatingVoltage", "CircuitID", "Status", "LastModified", "CreationDate", "LastUser", "CreationUser", "GlobalID", "StructureGUID" },
                new object[] { 4, "129981", "1", null, new DateTime(2012, 4, 25, 12, 30, 0, 0), null, "John", "{000-000-000}", "{000-000-123}" },
                "VoltageRegulator");
            regulator.SetXY(41.113, 121.112);

            TestFeature deviceGroupRow = new TestFeature(
                new string[] { "field1", "DeviceGroupNumber", "field3" },
                new object[] { "a", "x", "c" },
                "TestTable");
            regulator.AddRelatedRow(deviceGroupRow, "DeviceGroup_VoltageRegulator");

            TestFeature regulatorUnit1 = new TestFeature(
                new string[] { "field1", "UnitID", "field3", "Status" },
                new object[] { "a", "1234", "c", "5" },
                "VoltageRegulatorUnit");
            regulator.AddRelatedRow(regulatorUnit1, "VoltageReg_VoltageRegUnit");
            regulatorUnit1.AddRelatedRow(regulator, "VoltageReg_VoltageRegUnit");

            TestFeature regualtorUnit2 = new TestFeature(
                new string[] { "field1", "UnitID", "field3", "Status" },
                new object[] { "a", "1235", "c", "5" },
                "VoltageRegulatorUnit");
            regulator.AddRelatedRow(regualtorUnit2, "VoltageReg_VoltageRegUnit");

            TestFeature controllerRow = new TestFeature(
                new string[] { "SerialNumber", "UnitID", "field3", "Status" },
                new object[] { "9999", "1235", "c", "5" },
                "Controller");
            regulator.AddRelatedRow(controllerRow, "VoltageRegulator_Controller");

            List<IRowData> data = mapper.TrackedClasses[0].RowTransformer.ProcessRow(regulatorUnit1, null, ChangeType.Insert);

            AssetProcessor dataPersistRowProcessor = new AssetProcessor();

            try
            {
                Dictionary<string, IRowData> assetIdRowData = new Dictionary<string, IRowData>();
                assetIdRowData.Add(data[0].AssetID, data[0]);
                foreach (KeyValuePair<string, IRowData> pair in assetIdRowData)
                {
                    dataPersistRowProcessor.Process(pair);
                }
            }
            finally
            {
                dataPersistRowProcessor.Dispose();
            }

            Telvent.PGE.SAP.Data.SAPRowData test = new Telvent.PGE.SAP.Data.SAPRowData(new int[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 }, new string[] { "99", "x", "129981", "4", "41.113", "121.112", "John", "20120425", "20120425", "{000-000-000}", "1234" });
            Assert.AreEqual(test, data[0]);
            Assert.AreEqual("1234", data[0].AssetID);
        }
        [Test]
        public void SerializeTest()
        {
            List<object> testdata = new List<object>();
            testdata.Add("Test");
            TestFeature row = new TestFeature(testdata);
            BaseFieldTransformer test = new FieldValueTransformer();
            //This should be changed to handle the FieldValueTransformer
            string xml = "<FieldValueTransformer FieldName=\"Test\"></FieldValueTransformer>";
            XmlNode config = StaticUtilities.String2Node(xml);
            XmlDocument document = new XmlDocument();
            document.Load(Path.Combine(GetPath(), @"Telvent.PGE.SAP\Config\SampleConfig.xml"));
            SystemMapper mapper = XmlFacade.DeserializeFromXml<SystemMapper>(document);
            mapper.Initialize();
            //mapper.TrackedClasses[0].Fields[0].FieldMapper.GetValue<string>(row);
            IRowTransformer<IRow> srt = new SAPRowTransformer();
            List<IRowData> rowData = srt.ProcessRow(row, row, ChangeType.Insert);
            Assert.AreEqual(rowData[0].GetType(), typeof(Telvent.PGE.SAP.Data.SAPRowData));
        }

        [Test]
        public void ValidateTest()
        {
            string path = GetPath();
            string xsdFileName = Path.Combine(path, @"Telvent.PGE.SAP\Config\SAPIntegrationConfig.xsd");
            XmlDocument document = new XmlDocument();
            document.Load(Path.Combine(path, @"Telvent.PGE.SAP\Config\GISSAP_AssetSynch_Config.xml"));
            Assert.IsTrue(XmlFacade.ValidateXML(document.OuterXml, xsdFileName, false));
        }

        #region Domain Helper Tests
        [Test]
        public void DomainHelperInitialize()
        {
            string xml = "<DomainMappings> <DomainMapping name=\"SAP\"> <Domain name=\"test\"> <add fromValue=\"1\" toValue=\"x\" /> <add fromValue=\"2\" toValue=\"y\" /> </Domain> <Domain name=\"test2\"> <add fromValue=\"1\" toValue=\"y\" /> <add fromValue=\"2\" toValue=\"z\" /> </Domain> </DomainMapping> <DomainMapping name=\"OMS\"> <Domain name=\"test\"> <add fromValue=\"1\" toValue=\"a\" /> <add fromValue=\"2\" toValue=\"b\" /> </Domain> </DomainMapping> </DomainMappings> ";
            XmlNode config = StaticUtilities.String2Node(xml);
            DomainManager.Instance.Initialize(config, "SAP");
            Assert.AreEqual("x", DomainManager.Instance.GetValue("test", "1"));
        }
        #endregion end Domain Helper Tests


        public static string GetPath()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            path = Path.GetDirectoryName(path);
            path = Directory.GetParent(path).FullName;
            path = Directory.GetParent(path).FullName;
            return path;
        }
    }
}
