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
    public class MDBManagerTests : BaseTestFixture
    {
        MDBManager _mdbManager = null;

        private const string MDB_LOC = @"\\sfetgis-nas01\sfgispoc_data\ApplicationDevelopment\IBM_TeamMembers\PhilPenn\wip_fgdbs\wip.mdb";
        private const string MDB_FC = "EDGIS.WIP_VW";

        [TestInitialize]
        public void Initialize()
        {
            _mdbManager = _container.Resolve<MDBManager>();
            IExtractGDBManager extractGdbManager = _container.Resolve<IExtractGDBManager>();
        }

        [TestCleanup]
        public void TearDown()
        {
            Dispose();
        }

        [TestMethod]
        public void ShouldCreateInitial()
        {
            if (File.Exists(_mdbManager.BaseLocation))
            {
                File.Delete(_mdbManager.BaseLocation);
            }
            _mdbManager.ExportToBaseGDB();

            Assert.IsTrue(File.Exists(_mdbManager.BaseLocation));
        }
    }
}
