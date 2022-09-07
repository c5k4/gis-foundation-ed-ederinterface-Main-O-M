using ArcFmWebConfigLimited;
using ArcFmWebConfigLimited.Core.Manager;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PGE.BatchApplication.ArcFmWebConfigLimited.Tests
{
    [TestClass]
    [DeploymentItem("pge.log4net.config")]
    public class ArcFmManagerTests
    {
        private const string SdeConn2 = @"C:\Users\a1nc\AppData\Roaming\ESRI\Desktop10.2\ArcCatalog\lbgiss2q_edgis.sde";
        private const string FileName2 = @"C:\temp\ArcFM_FieldProperties_lbgiss2q.csv";

        private const string SdeConn = @"C:\Users\a1nc\AppData\Roaming\ESRI\Desktop10.2\ArcCatalog\edgisi1d_a1nc.sde";
        private const string FileName = @"C:\temp\ArcFM_FieldProperties_i1d.csv";

        private const string CompareFileName = @"C:\temp\ArcFM_FieldProperties_compare.csv";
        

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
            ArcFmGeneralPropertyManager _m;

            _m = new ArcFmSimpleSettingManager(SdeConn2);
            _m.GdbToCsv(FileName2);
        }

        /*
        [TestMethod]
        public void Compare()
        {
            ArcFmGeneralPropertyManager _m, _m2;

            

            _m = new ArcFmSimpleSettingManager(SdeConn);
            _m.GdbToCsv(FileName);

            _m2 = new ArcFmSimpleSettingManager(SdeConn2);
            _m2.GdbToCsv(FileName2);

            StreamWriter file = new StreamWriter(CompareFileName);
            IDictionary<string, string[]> kvps1 = _m.CsvData.GetItems();
            IDictionary<string, string[]> kvps2 = _m2.CsvData.GetItems();

            file.WriteLine(_m.CsvData.GetHeaders()[0]+","+"Setting,Old Value,New Value");

            foreach (string key in kvps1.Keys)
            {
                string[] vals1 = kvps1[key];
                string[] vals2;

                if (kvps2.TryGetValue(key, out vals2))
                {
                    for (int i = 0; i < vals1.Count(); i++)
                    {
                        if (vals1[i] != null && vals2[i] != null && !vals1[i].Equals(vals2[i]))
                        {
                            file.WriteLine(key+","+_m.CsvData.GetHeaders()[i+1]+","+vals1[i]+","+vals2[i]);
                        }
                        else if ((vals1[i] == null && vals2[i] != null) || (vals1[i] != null && vals2[i] == null))
                        {
                            file.WriteLine(key + "," + _m.CsvData.GetHeaders()[i + 1] + "," + vals1[i] + "," + vals2[i]);
                        }
                    }
                    
                }
            }

            file.Close();

        }*/
    }
}