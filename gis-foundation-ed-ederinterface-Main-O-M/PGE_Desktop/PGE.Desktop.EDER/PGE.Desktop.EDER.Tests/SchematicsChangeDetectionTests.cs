using System;
using System.ComponentModel;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using PGE.Desktop.Tests.Application;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miner.Interop;
using PGE.Desktop.EDER.Model;
using PGE.Desktop.EDER.Tests.Common;

namespace PGE.Desktop.Tests
{
    [TestClass]
    public class SchematicsChangeDetectionTests
    {
        private readonly string SDE_CONN = @"C:\Users\a1nc\AppData\Roaming\ESRI\Desktop10.8\ArcCatalog\EDGISA1D_EDGIS.sde";
        private readonly string DELETE_POINT_TABLE = "EDGIS.PGE_VERSIONDELETEPOINT";
        private readonly string DELETE_LINE_TABLE = "EDGIS.PGE_VERSIONDELETELINE";
        private readonly string USER = "EDGIS";
        private readonly string VERSION = "schematicstest";


        [TestInitialize]
        public void StartTests()
        {
            Common.Common.InitializeLicenses();
        }

        [TestCleanup]
        public void FinishTests()
        {
            Common.Common.CloseLicenseObject();
        }

        [TestMethod]
        public void TestPointWithoutAUFramework()
        {
            Type type = Type.GetTypeFromProgID("mmGeoDatabase.MMAutoUpdater");
            IMMAutoUpdater auController = (IMMAutoUpdater)Activator.CreateInstance(type);
            mmAutoUpdaterMode prevAUMode = auController.AutoUpdaterMode;
            auController.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMArcMap;

            //Test insert
            VersionedEditor v = new VersionedEditor(SDE_CONN, VERSION, true);

            v.StartEditSession();

            IFeatureClass capFeatCl = v.OpenFeatureClass("CapacitorBank");
            IFeature capac = capFeatCl.CreateFeature();
            IPoint point = new PointClass();
            point.X = 1.0;
            point.Y = 2.0;
            capac.set_Value(capac.Fields.FindField("SHAPE"), point);

            RecordVersionChange.RecordInsertUpdate(capac);
            capac.Store();
            Assert.AreEqual<string>((string)capac.get_Value(capac.Fields.FindField("VERSIONNAME")), USER+"."+VERSION);

            //Test update
            //capac = capFeatCl.GetFeature(11);
            capac.set_Value(capac.Fields.FindField("OPERATINGNUMBER"), "1234");
            
            RecordVersionChange.RecordInsertUpdate(capac);
            capac.Store();
            Assert.AreEqual<string>((string)capac.get_Value(capac.Fields.FindField("VERSIONNAME")), USER + "." + VERSION);


            //Test delete
            IQueryFilter f = new QueryFilter();
            f.SubFields = "*";
            f.WhereClause = "";

            int deletepointCount = v.OpenTable(DELETE_POINT_TABLE).RowCount(f);
            RecordVersionChange.RecordDelete(capac);

            capac.Delete();

            Assert.AreEqual<int>(deletepointCount + 1, v.OpenTable(DELETE_POINT_TABLE).RowCount(f));

            v.FinishEditSession(true);
        }

        
        [TestMethod]
        public void TestLineWithoutAUFramework()
        {
            //Test insert
            VersionedEditor v = new VersionedEditor(SDE_CONN, VERSION, true);

            v.StartEditSession();

            IFeatureClass prioh = v.OpenFeatureClass("PriOHConductor");
            IFeature priohInstance = prioh.CreateFeature();
            IPointCollection p = new PolylineClass();
            IPoint from = new PointClass(), to = new PointClass();
            from.X = 1000.0;
            from.Y = 2000.0;
            to.X = 8000.0;
            to.Y = 9000.0;

            p.AddPoint(from);
            p.AddPoint(to);

            priohInstance.set_Value(priohInstance.Fields.FindField("SHAPE"), (IGeometry5) p);
            priohInstance.set_Value(priohInstance.Fields.FindField("SUBTYPECD"), 1);

            RecordVersionChange.RecordInsertUpdate(priohInstance);
            priohInstance.Store();
            Assert.AreEqual<string>((string)priohInstance.get_Value(priohInstance.Fields.FindField("VERSIONNAME")), USER + "." + VERSION);

            //Test update
            priohInstance.set_Value(priohInstance.Fields.FindField("CITY"), "Fresno");

            RecordVersionChange.RecordInsertUpdate(priohInstance);
            priohInstance.Store();
            Assert.AreEqual<string>((string)priohInstance.get_Value(priohInstance.Fields.FindField("VERSIONNAME")), USER + "." + VERSION);


            //Test delete
            IQueryFilter f = new QueryFilter();
            f.SubFields = "*";
            f.WhereClause = "";

            int deleteLineCount = v.OpenTable(DELETE_LINE_TABLE).RowCount(f);
            RecordVersionChange.RecordDelete(priohInstance);
            priohInstance.set_Value(priohInstance.Fields.FindField("CREATIONUSER"), "vs2010");
            priohInstance.set_Value(priohInstance.Fields.FindField("LASTUSER"), "vs2010");

            priohInstance.Delete();

            Assert.AreEqual<int>(deleteLineCount + 1, v.OpenTable(DELETE_LINE_TABLE).RowCount(f));

            v.FinishEditSession(true);
        }

        /// <summary>
        /// End to end test. DOES NOT WORK. Autoupdaters do not seem to fire when making 
        /// edits through Visual Studio
        /// </summary>
        /*
        [TestMethod]
        public void E2ETest()
        {

            Comm.InitializeESRILicense();
            Comm.InitializeArcFMLicense();

            Type type = Type.GetTypeFromProgID("mmGeoDatabase.MMAutoUpdater");
            IMMAutoUpdater auController = (IMMAutoUpdater)Activator.CreateInstance(type);
            mmAutoUpdaterMode prevAUMode = auController.AutoUpdaterMode;
            auController.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMArcMap;

            IWorkspace wksp = Comm.OpenWorkspaceFromSDEFile(SDE_CONN);
            VersionedEditor v = new VersionedEditor(SDE_CONN, VERSION);

            v.StartEditSession();

            IFeatureClass capFeatCl = v.OpenFeatureClass("CapacitorBank");
            IFeature capac = capFeatCl.CreateFeature();
            IPoint point = new PointClass();
            point.X = 1.0;
            point.Y = 2.0;
            capac.set_Value(capac.Fields.FindField("SHAPE"), point);

            //capac.set_Value(capac.Fields.FindField("VERSIONNAME"), "EDGIS.schematicstest");

            capac.Store();

            Assert.AreEqual<string>((string)capac.get_Value(capac.Fields.FindField("VERSIONNAME")), USER + "." + VERSION);

            IQueryFilter f = new QueryFilter();
            f.SubFields = "*";
            f.WhereClause = "";

            int deletepointCount = v.OpenTable(DELETE_POINT_TABLE).RowCount(f);
            capac.Delete();
            Assert.AreEqual<int>(deletepointCount + 1, v.OpenTable(DELETE_POINT_TABLE).RowCount(f));

            v.FinishEditSession(true);
        }*/
    }
}
