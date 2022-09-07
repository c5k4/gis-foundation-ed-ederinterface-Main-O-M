using System;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using IBM.PGE.ED.Desktop.Tests.Application;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Miner.Interop;
using Telvent.PGE.ED.Desktop;
using Telvent.PGE.ED.Desktop.GDBM;
using Telvent.PGE.ED.Desktop.Utility;

namespace IBM.PGE.ED.Desktop.Tests
{
    [TestClass]
    public class GDBMOpNumHandlerTest
    {
        private const string SdeConn = @"C:\Users\a1nc\AppData\Roaming\ESRI\Desktop10.2\ArcCatalog\PGE1_EDGIS.sde";
        private const string VERSION = "gdbm_test";
        private const string OpNum = "C98493";
        private const string JobNum = "948933";

        [TestInitialize]
        public void StartTests()
        {
            Common.Common.InitializeLicenses();

            IWorkspace wksp = Common.Common.OpenWorkspaceFromSDEFile(SdeConn);
            IVersionedWorkspace vw = (IVersionedWorkspace)wksp;

            try
            {
                IVersion v = vw.FindVersion(VERSION);

                if (v != null)
                {
                    v.Delete();
                    Marshal.FinalReleaseComObject(v);
                }

                v = null;
            }
            catch (Exception)
            {
            }
            
            

            Marshal.FinalReleaseComObject(wksp);
            Marshal.FinalReleaseComObject(vw);
            wksp = null;
            vw = null;
            

            GC.Collect();
            GC.WaitForFullGCComplete();

            Type type = Type.GetTypeFromProgID("mmGeoDatabase.MMAutoUpdater");
            IMMAutoUpdater auController = (IMMAutoUpdater)Activator.CreateInstance(type);
            mmAutoUpdaterMode prevAUMode = auController.AutoUpdaterMode;
            auController.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMArcMap;

            try
            {


                Telvent.PGE.ED.Desktop.OracleDbConnection c = new Telvent.PGE.ED.Desktop.OracleDbConnection("webr", "webr", WipDbApi.WipDbTns);

                c.ExecuteCommand("delete from " + Telvent.PGE.ED.Desktop.SchemaInfo.Wip.JETEquipmentTable +
                                 " where job_number='" + JobNum + "'");
                c.ExecuteCommand("delete from " + Telvent.PGE.ED.Desktop.SchemaInfo.Wip.JETJobsTable +
                                 " where job_number='" + JobNum + "'");

                c.ExecuteCommand("insert into " + Telvent.PGE.ED.Desktop.SchemaInfo.Wip.JETEquipmentTable +
                                 " (objectid, job_number, operating_number,equipment_type_id) values (r3183.nextval,'" + JobNum + "','" +
                                 OpNum + "','15')");

                c.ExecuteCommand("insert into " + SchemaInfo.Wip.JETJobsTable +
                                 "(objectid,job_number) values (r3182.nextval,'" + JobNum + "')");
            }
            catch (Exception e)
            {
                
            
            }
        }

        [TestMethod]
        public void TestInsertOpNum()
        {
            //Test insert
            VersionedEditor v = new VersionedEditor(SdeConn, VERSION, true);

            v.StartEditSession();

            IFeatureClass capFeatCl = v.OpenFeatureClass("CapacitorBank");
            IFeature capac = capFeatCl.CreateFeature();
            IPoint point = new PointClass();
            point.X = 1.0;
            point.Y = 2.0;
            capac.set_Value(capac.Fields.FindField("SHAPE"), point);
            capac.set_Value(capac.Fields.FindField("OPERATINGNUMBER"), OpNum);
            capac.set_Value(capac.Fields.FindField("INSTALLJOBNUMBER"), JobNum);
            capac.set_Value(capac.Fields.FindField("SUBTYPECD"), "2");
            capac.set_Value(capac.Fields.FindField("LOCALOFFICEID"), "1");
            
            capac.Store();

            v.FinishEditSession(true);

            PGEOperatingNumberHandler h = new PGEOperatingNumberHandler();
            PGEOperatingNumberHandler.ExecuteActionHandler(v.GetVersion(), "wip_rw", "wip_rw");
        }
    }
}
