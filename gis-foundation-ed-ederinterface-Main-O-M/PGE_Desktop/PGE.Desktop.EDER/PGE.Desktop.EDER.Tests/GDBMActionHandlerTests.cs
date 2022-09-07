using System;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Carto;
using PGE.Desktop.Tests.Application;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ESRI.ArcGIS.Geodatabase;
using PGE.Desktop.EDER.GDBM.CircuitProcessing;
using Comm = PGE.Desktop.Tests.Common.Common;
using Miner.Interop;

namespace PGE.Desktop.Tests
{
    [TestClass]
    public class GdbmActionHandlerTests
    {
        private const string SdeConn = @"C:\Users\a1nc\AppData\Roaming\ESRI\Desktop10.8\ArcCatalog\EDGISA1D_EDGIS.sde";
        private const string VERSION = "gdbm_test";
        

        [TestInitialize]
        public void StartTests()
        {
            Common.Common.InitializeLicenses();

            IWorkspace wksp = Common.Common.OpenWorkspaceFromSDEFile(SdeConn);
            IVersionedWorkspace vw = (IVersionedWorkspace)wksp;

            IVersion v = vw.FindVersion(VERSION);
            if (v != null)
            {
                v.Delete();
                Marshal.FinalReleaseComObject(v);
            }

            Marshal.FinalReleaseComObject(wksp);
            Marshal.FinalReleaseComObject(vw);
            wksp = null;
            vw = null;
            v = null;

            GC.Collect();
            GC.WaitForFullGCComplete();

            Type type = Type.GetTypeFromProgID("mmGeoDatabase.MMAutoUpdater");
            IMMAutoUpdater auController = (IMMAutoUpdater)Activator.CreateInstance(type);
            mmAutoUpdaterMode prevAUMode = auController.AutoUpdaterMode;
            auController.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMArcMap;
        }

        [TestCleanup]
        public void FinishTests()
        {
            Common.Common.CloseLicenseObject();
        }

        [TestMethod]
        public void TestCircuitSourceFieldChange()
        {
            VersionedEditor editor = new VersionedEditor(SdeConn, VERSION, true);
            IVersion myV = editor.GetVersion();
            IQueryFilter qf;
            IRow row;

            editor.StartEditSession();

            ITable circuitSourceTable = editor.OpenTable("EDGIS.CIRCUITSOURCE");

            qf = new QueryFilterClass();
            qf.WhereClause = "objectid=324409";

            ICursor resultsCursor = circuitSourceTable.Search(qf, false);
            row = resultsCursor.NextRow();

            row.Value[row.Fields.FindField("FEEDERTYPE")] = 1;
            row.Value[row.Fields.FindField("CIRCUITCOLOR")] = "Green";
            row.Store();

            PGE_CircuitColor c = new PGE_CircuitColor();
            c.ExecuteActionHandler(myV);

            editor.FinishEditSession(true);

            myV.RefreshVersion();
            IFeatureWorkspace wksp = (IFeatureWorkspace) myV;

            IFeatureClass openPt = wksp.OpenFeatureClass("EDGIS.PRIUGCONDUCTOR");

            //downstream from circuit point
            qf = new QueryFilterClass {WhereClause = "objectid=6661555"};

            IFeatureCursor featureResultsCursor = openPt.Search(qf, false);
            row = featureResultsCursor.NextFeature();

            Assert.IsTrue(row.Value[row.Fields.FindField("FEEDERTYPE")].ToString() == "1");
            Assert.IsTrue(row.Value[row.Fields.FindField("CIRCUITCOLOR")].ToString() == "Green");
        }
    }
}
