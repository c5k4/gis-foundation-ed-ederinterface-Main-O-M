using ArcFmWebConfigLimited;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PGE.BatchApplication.ArcFmWebConfigLimited.Core.Enumerator;
using PGE.BatchApplication.ArcFmWebConfigLimited.Core.Manager;

namespace PGE.BatchApplication.ArcFmWebConfigLimited.Tests
{
    [TestClass]
    [DeploymentItem("pge.log4net.config")]
    public class ArcFmManagerTests
    {
        private const string SdeConn = @"C:\Users\a1nc\AppData\Roaming\ESRI\Desktop10.0\ArcCatalog\lbgisq3q_edgis.sde";
        private const string SdeConn2 = @"C:\Users\a1nc\AppData\Roaming\ESRI\Desktop10.0\ArcCatalog\lbgiss1q_edgis.sde";
        private const string fileName = @"C:\temp\ArcFM_FieldProperties_webr_elec.xlsx";
        private const string fileName2 = @"C:\temp\ArcFM_FieldProperties_webr_sub.xlsx";
        private ArcFmGeneralPropertyManager _m;

        [TestInitialize]
        public void Init()
        {
            Program.CheckOutLicenses();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Program.CheckInLicenses();
        }

        [TestMethod]
        public void TestReadWrite()
        {
            _m = new ArcFmSimpleSettingManager(SdeConn);
            _m.XlsxToGdb(fileName);

            PropertyEnumerator.ResetCompiledObjectClasses();

            _m = new ArcFmSimpleSettingManager(SdeConn2);
            _m.XlsxToGdb(fileName2);
        }
    }
}