using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PGE.Interfaces.Integration.Framework;
using System.Diagnostics;
using PGE.Interfaces.SAP;
using ESRI.ArcGIS.Geodatabase;
using System.IO;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS;
using System.Reflection;
using ESRI.ArcGIS.ADF.COMSupport;

namespace PGE.Interfaces.SAP.TestApp
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void cmdExe_Click(object sender, EventArgs e)
        {
            IAoInitialize aoInit = null;
            
            try
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                RuntimeManager.Bind(ProductCode.Desktop);
                aoInit = new AoInitializeClass();
                if (aoInit.IsProductCodeAvailable(esriLicenseProductCode.esriLicenseProductCodeAdvanced) == esriLicenseStatus.esriLicenseAvailable)
                {
                    esriLicenseStatus stat = aoInit.Initialize(esriLicenseProductCode.esriLicenseProductCodeAdvanced);

                    //Assert.AreEqual(stat, esriLicenseStatus.esriLicenseCheckedOut);
                }

                SdeWorkspaceFactoryClass workspaceFac = new SdeWorkspaceFactoryClass();
                IPropertySet propertySet = new PropertySetClass();
                propertySet.SetProperty("SERVER", "");
                propertySet.SetProperty("INSTANCE", txtInstance.Text);
                propertySet.SetProperty("DATABASE", "");
                propertySet.SetProperty("USER", txtUser.Text);
                propertySet.SetProperty("PASSWORD", txtPass.Text);
                propertySet.SetProperty("VERSION", "SDE.DEFAULT");

                IWorkspace workspace = workspaceFac.Open(propertySet, 0);
                IVersionedWorkspace3 versionedWorkspace = (IVersionedWorkspace3)workspace;
                IVersion editVersion = versionedWorkspace.FindVersion(txtSource.Text);
                IVersion targetVersion = versionedWorkspace.FindVersion(txtTarget.Text);

                GISSAPIntegrator integrator = new GISSAPIntegrator(editVersion, targetVersion);
                bool initialized = integrator.Initialize();

                //Dictionary<string, Dictionary<string, IRowData>> assetIDandDataByClass = integrator.Process();

                string logFile = Path.Combine(GetPath(), "GisSap.txt");
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

                SAP.AssetProcessor dprow = new SAP.AssetProcessor();
                try
                {
                    //foreach (Dictionary<string, IRowData> classAssetIDandData in assetIDandDataByClass.Values)
                    //{
                    //    foreach (KeyValuePair<string, IRowData> row in classAssetIDandData)
                    //    {
                    //        dprow.Process(row);
                    //    }
                    //}
                }
                finally
                {
                    //dprow.Dispose();
                }


                if (chkCSV.Checked)
                {
                    PGE.Interfaces.SAP.Batch.Program.Process(null);
                }
                stopWatch.Stop();
                MessageBox.Show("Finished. Total Time: " + stopWatch.Elapsed.ToString());
                Trace.WriteLine(stopWatch.Elapsed.TotalSeconds);
                txtWriterTraceListener.Close();

                foreach (Exception wex in integrator.ErrorMessages())
                {
                    txtErrors.Text += wex.ToString() + System.Environment.NewLine;
                }
            }
            catch (Exception ex)
            {
                txtErrors.Text += ex.ToString() + System.Environment.NewLine;
                MessageBox.Show(ex.StackTrace);
            }
            finally
            {
                AOUninitialize.Shutdown();
                if (aoInit != null)
                {
                    aoInit.Shutdown();
                }
            }
        }
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
