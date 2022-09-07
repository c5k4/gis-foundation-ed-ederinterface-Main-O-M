using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;

using Telvent.PGE.Framework;
using Telvent.Delivery.Systems;
using Telvent.PGE.SAP;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.ADF.COMSupport;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS;
using Telvent.PGE.Framework.Exceptions;
using System.Xml;
using Telvent.PGE.Framework.FieldTransformers;
using ESRI.ArcGIS.Geometry;

using NUnit.Framework;
namespace Telvent.PGE.Test
{
    [TestFixture]
    public class SAPIntegrationTests
    {
        IWorkspace _workspace;

        public IWorkspace Workspace
        {
            get
            {
                if (_workspace == null)
                {                
                    try
                    {
                        RuntimeManager.Bind(ProductCode.Desktop);
                        IAoInitialize aoInit = new AoInitializeClass();
                        if (aoInit.IsProductCodeAvailable(esriLicenseProductCode.esriLicenseProductCodeArcInfo) == esriLicenseStatus.esriLicenseAvailable)
                        {
                            esriLicenseStatus stat = aoInit.Initialize(esriLicenseProductCode.esriLicenseProductCodeArcInfo);

                            //Assert.AreEqual(stat, esriLicenseStatus.esriLicenseCheckedOut);
                        }
                        SdeWorkspaceFactoryClass workspaceFac = new SdeWorkspaceFactoryClass();
                        IPropertySet propertySet = new PropertySetClass();
                        propertySet.SetProperty("SERVER", "");
                        propertySet.SetProperty("INSTANCE", "sde:oracle11g:\\;LOCAL=ORCI.ATHENA");
                        propertySet.SetProperty("DATABASE", "");
                        propertySet.SetProperty("USER", "pge");
                        propertySet.SetProperty("PASSWORD", "pge");
                        propertySet.SetProperty("VERSION", "SDE.DEFAULT");

                        _workspace = workspaceFac.Open(propertySet, 0);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                return _workspace;
            }
            set { _workspace = value; }
        }
        [Test]
        public void XYTransformerTests()
        {
            IWorkspace ws = Workspace;
            Assert.IsTrue(ws != null);
            IWorkspaceEdit wse = (IWorkspaceEdit)ws;
            wse.StartEditing(false);
            wse.StartEditOperation();
            IFeatureWorkspace fws = (IFeatureWorkspace)ws;
            IFeatureClass fc = fws.OpenFeatureClass("ElectricStitchPoint");
            IFeature feat = fc.CreateFeature();
            IPoint geom = new PointClass();
            geom.X = 2847235.087;
            geom.Y = 12877101.293;
            feat.Shape = geom;

            BaseFieldTransformer test = new XYTransformer();
            string xml = "<XYTransformer FieldName=\"X\"></XYTransformer>";
            XmlNode config = StaticUtilities.String2Node(xml);
            Assert.IsTrue(test.Initialize(config));
            Assert.AreEqual("-118.9502633095", test.GetValue<string>(feat));
            xml = "<XYTransformer FieldName=\"Y\"></XYTransformer>";
            config = StaticUtilities.String2Node(xml);
            Assert.IsTrue(test.Initialize(config));
            Assert.AreEqual("35.4000673486", test.GetValue<string>(feat));
            bool exception = false;
            try
            {
                //string x = test.GetValue<string>(transformerUnit1);
            }
            catch (InvalidConfigurationException ex)
            {
                exception = true;
            }
            //Assert.IsTrue(exception);
            wse.StopEditOperation();
            wse.StopEditing(false);
        }
        [Test]
        public void IntegratedTest()
        {
            //SystemRegistry sysReg = new SystemRegistry();
            //string passFile = Path.Combine(sysReg.Directory, "pass.txt");
            //StreamWriter streamWriter = new StreamWriter(passFile);
            //streamWriter.Write(EncryptionFacade.Encrypt("pge"));
            //streamWriter.Flush();
            //streamWriter.Close();

            //StreamReader streamReader = new StreamReader(passFile);
            //string pass = streamReader.ReadLine();
            //pass = EncryptionFacade.Decrypt(pass);

            //streamReader.Close();
            //return;

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            RuntimeManager.Bind(ProductCode.Desktop);
            IAoInitialize aoInit = new AoInitializeClass();
            if (aoInit.IsProductCodeAvailable(esriLicenseProductCode.esriLicenseProductCodeArcInfo) == esriLicenseStatus.esriLicenseAvailable)
            {
                esriLicenseStatus stat = aoInit.Initialize(esriLicenseProductCode.esriLicenseProductCodeArcInfo);

                //Assert.AreEqual(stat, esriLicenseStatus.esriLicenseCheckedOut);
            }

            try
            {
                SdeWorkspaceFactoryClass workspaceFac = new SdeWorkspaceFactoryClass();
                IPropertySet propertySet = new PropertySetClass();
                propertySet.SetProperty("SERVER", "");
                propertySet.SetProperty("INSTANCE", "sde:oracle11g:\\;LOCAL=ORCI.ATHENA");
                propertySet.SetProperty("DATABASE", "");
                propertySet.SetProperty("USER", "pge");
                propertySet.SetProperty("PASSWORD", "pge");
                propertySet.SetProperty("VERSION", "SDE.DEFAULT");

                // this temporarily switched by Junwei, it doesn't work the other way
                //IWorkspace workspace = workspaceFac.OpenFromFile(@"C:\Users\junwei\AppData\Roaming\ESRI\Desktop10.0\ArcCatalog\sapIntegrationTest_orci.athena.sde", 0);
                IWorkspace workspace = workspaceFac.Open(propertySet, 0);
                //IWorkspace editWorkspace = workspaceFac.OpenFromFile("", 0);

                Assert.IsNotNull(workspace);

                IVersionedWorkspace3 versionedWorkspace = (IVersionedWorkspace3)workspace;
                //IVersion editVersion = versionedWorkspace.FindVersion("Session1");
                //IVersion targetVersion = versionedWorkspace.FindVersion("SAPIntegrationTest");

                //IVersion editVersion = versionedWorkspace.FindVersion("PGE.JRMassUpdates");
                //IVersion targetVersion = versionedWorkspace.FindVersion("SDE.DEFAULT");

                IVersion editVersion = versionedWorkspace.FindVersion("PGE.CleanupEdits");
                IVersion targetVersion = versionedWorkspace.FindVersion("PGE.Cleanup");

                GISSAPIntegrator integrator = new GISSAPIntegrator(editVersion, targetVersion);
                bool initialized = integrator.Initialize();


                //Dictionary<string, Dictionary<string, IRowData>> obj = integrator.Process();

                Dictionary<string, Dictionary<string, IRowData>> assetIDandDataByClass = integrator.Process();

                string logFile = System.IO.Path.Combine(SAPComponentTests.GetPath(), "GisSap.txt");
                TextWriterTraceListener txtWriterTraceListener = new TextWriterTraceListener(logFile);
                Trace.Listeners.Add(txtWriterTraceListener);

                Dictionary<string, Dictionary<int, double>> assetOIDAndProcessingTimeByClass = integrator.AssetOIDAndProcessTimeByClass;
                foreach (KeyValuePair<string, Dictionary<int, double>> assetOIDTimeOneClass in assetOIDAndProcessingTimeByClass)
                {
                    string assetOutNameSourceName = assetOIDTimeOneClass.Key;
                    foreach (KeyValuePair<int, double> idTime in assetOIDTimeOneClass.Value)
                    {
                        string tmp = string.Empty;
                        tmp = assetOutNameSourceName + " " + idTime.Key + " : " + idTime.Value;
                        Trace.WriteLine(tmp);
                    }
                }

                Trace.Flush();
                txtWriterTraceListener.Flush();

                AssetProcessor dprow = new AssetProcessor();
                try
                {
                    foreach (Dictionary<string, IRowData> classAssetIDandData in assetIDandDataByClass.Values)
                    {
                        foreach (KeyValuePair<string, IRowData> row in classAssetIDandData)
                        {
                            dprow.Process(row);
                        }
                    }
                }
                finally
                {
                    dprow.Dispose();
                }

                //PxActionData actiondata = new PxActionData(null, null, string.Empty, null, false, false, string.Empty, string.Empty, 1);
                //Telvent.PGE.GDBM.ActionHandlers.DataPersistor dPersistor = new GDBM.ActionHandlers.DataPersistor();
                //actiondata.Add("SAPData", obj);
                //dPersistor.Execute(actiondata, null); 

                stopWatch.Stop();

                Trace.WriteLine(stopWatch.Elapsed.TotalSeconds);
                txtWriterTraceListener.Close();

                Assert.IsTrue(integrator.ErrorMessages().Count == 0);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false);
                Assert.Fail("Integration test failed" + e.InnerException);
            }
            finally
            {
                AOUninitialize.Shutdown();
                aoInit.Shutdown();
            }
        }
    }
}
