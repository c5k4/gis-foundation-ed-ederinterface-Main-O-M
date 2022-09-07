using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PGE.Common.ChangesManager;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.ChangesManagerShared;
using System.Runtime.InteropServices;

namespace PGE.BatchApplication.ChangeManager.Tests
{
    [TestClass]
    public class FGDBManagerTests : BaseTestFixture
    {
        FGDBManager _fgdbManager = null;

        private const string FGDB_LOC = @"\\sfetgis-nas01\sfgispoc_data\ApplicationDevelopment\IBM_TeamMembers\PhilPenn\wip_fgdbs\wip.gdb";
        private const string GDB_FC = "EDGIS.WIP_VW";

        [TestInitialize]
        public void InitializeFGDBManager()
        {
            _fgdbManager = new FGDBManager(SDEWorkspace, GDB_FC, FGDB_LOC);
        }

        [TestCleanup]
        public void TearDown()
        {
            Dispose();
        }

        [TestMethod]
        public void ShouldCreateFGDB()
        {
            if (Directory.Exists(FGDB_LOC))
            {
                Directory.Delete(FGDB_LOC, true);
            }
            _fgdbManager.CreateGDB(FGDB_LOC);
            Assert.IsTrue(Directory.Exists(FGDB_LOC));
        }
        [TestMethod]
        public void ShouldDelete()
        {
            if (Directory.Exists(_fgdbManager.BaseLocation))
            {
                Directory.Delete(_fgdbManager.BaseLocation, true);
            }
            _fgdbManager.ExportToBaseGDB();
            IFeatureClass fc = _fgdbManager.OpenBaseFeatureClass();
            _fgdbManager.KillConnections();
//            _fgdbManager = null;
            fc = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Directory.Delete(_fgdbManager.BaseLocation, true);

        }
        [TestMethod]
        public void ShouldExportToBaseFGDB()
        {
            if (Directory.Exists(FGDB_LOC))
            {
                Directory.Delete(FGDB_LOC, true);
            }
            _fgdbManager.ExportToBaseGDB();

            Assert.IsTrue(VerifyFCExists(FGDB_LOC, GDB_FC.Substring(GDB_FC.LastIndexOf(".") + 1)));
        }

        [TestMethod]
        public void ShouldExportToEditsFGDB()
        {
            if (Directory.Exists(_fgdbManager.EditsLocation))
            {
                Directory.Delete(_fgdbManager.EditsLocation, true);
            }
            _fgdbManager.ExportToEditsGDB();

            Assert.IsTrue(VerifyFCExists(_fgdbManager.EditsLocation, GDB_FC.Substring(GDB_FC.LastIndexOf(".") + 1)));
        }

        [TestMethod]
        public void ShouldThrowException()
        {
            FGDBManager fgdbManager = new FGDBManager(SDEWorkspace, GDB_FC, FGDB_LOC.Replace("CD", "DC"));

            bool exceptionThrown = false;
            try
            {
                fgdbManager.ExportToEditsGDB();
            }
            catch (ErrorCodeException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown);
        }

        [TestMethod]
        public void ShouldRelease()
        {
            if (Directory.Exists(_fgdbManager.BaseLocation))
            {
                Directory.Delete(_fgdbManager.BaseLocation, true);
            }
            _fgdbManager.ExportToBaseGDB();
            string loc = _fgdbManager.BaseLocation;
            _fgdbManager = null;
            Directory.Delete(loc, true);
        }
        [TestMethod]
        public void ShouldRollover()
        {
            if (Directory.Exists(_fgdbManager.BaseLocation))
            {
                Directory.Delete(_fgdbManager.BaseLocation, true);
            }
            if (Directory.Exists(_fgdbManager.EditsLocation))
            {
                Directory.Delete(_fgdbManager.EditsLocation, true);
            }
            _fgdbManager.ExportToBaseGDB();
            _fgdbManager.ExportToEditsGDB(); 
            IFeatureClass baseFC = _fgdbManager.OpenBaseFeatureClass();
            IFeatureClass editsFC = _fgdbManager.OpenEditsFeatureClass();
            baseFC = null;
            editsFC = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();

            _fgdbManager.RolloverGDBs();

            Assert.IsTrue(Directory.Exists(_fgdbManager.BaseLocation));
            Assert.IsTrue(!Directory.Exists(_fgdbManager.EditsLocation));

        }

        private bool VerifyFCExists(string gdbLocation, string fcName)
        {
            IWorkspace gdbWorkspace = FGDBManager.FileGdbWorkspaceFromPath(gdbLocation);
            IFeatureClass fc = ((IFeatureWorkspace)gdbWorkspace).OpenFeatureClass(fcName);
            
            return (fc != null);
        }

    }
}
