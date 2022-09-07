using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using System.Collections.Generic;
using System.Data.OracleClient;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using System.Collections;
using System.Configuration;
using Miner.Interop.Process;
using Smart_Network;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using Microsoft.VisualBasic.FileIO;
using ESRI.ArcGIS.Geometry;


namespace Smart_Network
{
    class Program 
    {
        private static VMS_GIS_STG.LicenseInitializer m_AOLicenseInitializer = new VMS_GIS_STG.LicenseInitializer();
        private static string sPath = (String.IsNullOrEmpty(ConfigurationManager.AppSettings["LOGPATH"]) ? System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) : ConfigurationManager.AppSettings["LOGPATH"]) + "\\Logfile_" + DateTime.Now.Ticks.ToString() + ".log";
        private static StreamWriter pSWriter = default(StreamWriter);
        private static IMMAppInitialize arcFMAppInitialize = new MMAppInitializeClass();
        private static Dictionary<object, object> _directionDict = new Dictionary<object, object>();
        private static string _objectIdInput = null;

        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                _objectIdInput = args[0];
                //Console.WriteLine(_objectIdInput);
                (pSWriter = File.CreateText(sPath)).Close();
                List<string> issueList = new List<string>();

                Console.WriteLine("----Update Features from FC's-----");
                Console.WriteLine("\n");

                string strEDGISSdeConn = ConfigurationManager.AppSettings["CONN_EDGIS_FILE"].ToString();

                Console.WriteLine("Sde Connection Parameter :" + strEDGISSdeConn);
                //ESRI License Initializer generated code.
                WriteLine(DateTime.Now.ToLongTimeString() + " -- Initializing Licence");
                m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeBasic, esriLicenseProductCode.esriLicenseProductCodeStandard, esriLicenseProductCode.esriLicenseProductCodeAdvanced },
                new esriLicenseExtensionCode[] { });
                Miner.Interop.mmLicenseStatus licenseStatus = CheckOutLicenses(mmLicensedProductCode.mmLPArcFM);
                if (RuntimeManager.ActiveRuntime == null)
                    RuntimeManager.BindLicense(ProductCode.Desktop);

                //Process feature class wise

                List<string> lstFCNames = new System.Collections.Generic.List<string>();
                UpdateFeatures();

            }

            catch (Exception ee)
            {
            }

        }

        private static mmLicenseStatus CheckOutLicenses(mmLicensedProductCode productCode)
        {
            mmLicenseStatus licenseStatus;
            licenseStatus = arcFMAppInitialize.IsProductCodeAvailable(productCode);
            if (licenseStatus == mmLicenseStatus.mmLicenseAvailable)
            {
                licenseStatus = arcFMAppInitialize.Initialize(productCode);
            }
            return licenseStatus;
        }

        private static void WriteLine(string sMsg)
        {
            pSWriter = File.AppendText(sPath);
            pSWriter.WriteLine(sMsg);
            //DrawProgressBar();
            Console.WriteLine(sMsg);
            pSWriter.Close();
        }

        private static void UpdateFeatures()
        {            
            IVersion pVersion = default(IVersion);
            IFeatureClass pFClass = default(IFeatureClass);
            //IFeature pfeat_New = default(IFeature);
            IFeature featureCreated = default(IFeature);
            IMMAutoUpdater autoupdater = default(IMMAutoUpdater);
            mmAutoUpdaterMode oldMode;
            //ESRI.ArcGIS.Geometry.IPoint point = new ESRI.ArcGIS.Geometry.Point();
            long sapequipid=0;
            int counts = 0;
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.ShowDialog();
                string pfile = ofd.FileName;
                Console.WriteLine("Path of the file:" + ofd.FileName);
                //DataTable tab = ImportExceltoDatatable(pfile);
                DataTable tab = ConvertCSVtoDataTable(pfile);
                WriteLine(DateTime.Now.ToLongTimeString() + " -- Creating Connection " + ConfigurationManager.AppSettings["CONN_EDGIS_FILE"] + " to provided Version " + ConfigurationManager.AppSettings["SESSION_NAME"]);
                IWorkspace objEDGISFW = ArcSdeWorkspaceFromFile(ConfigurationManager.AppSettings["CONN_EDGIS_FILE"]);
                IFeatureWorkspace _featWksp = (IFeatureWorkspace)objEDGISFW;
                IFeatureDataset feat_Dataset = _featWksp.OpenFeatureDataset("EDGIS.ElectricDataset");
                string _VersionToBeCreated = CreateVersion(_featWksp, _objectIdInput);
                WriteLine(DateTime.Now.ToLongTimeString() + " Version Created: " + _VersionToBeCreated);
                pVersion = ((IVersionedWorkspace)objEDGISFW).FindVersion(_VersionToBeCreated);
                string pFCName = ConfigurationManager.AppSettings["UPDATE_FEATURE_CLASS"];
                pFClass = (pVersion as IFeatureWorkspace).OpenFeatureClass(pFCName);

                //started processing FC's
                WriteLine(DateTime.Now.ToLongTimeString() + " Started processing " + "EDGIS.SmartMeterNetworkDevice");
                ((IWorkspaceEdit)pVersion).StartEditing(true);
                ((IWorkspaceEdit)pVersion).StartEditOperation();          
                if (DisableAutoUpdaterFramework(out autoupdater, out oldMode))
                {
                    for (counts = 0; counts < tab.Rows.Count; counts++)
                    {

                        featureCreated = pFClass.CreateFeature();
                        sapequipid = Convert.ToInt64(tab.Rows[counts]["SAPEQUIPID"]);
                        //Int64 sapequipid = Convert.ToInt64(dtRecord.Rows[i]["SAPEQUIPID"]);
                        double latValue = Convert.ToDouble(tab.Rows[counts]["LATITUDE"]);
                        double longValue = Convert.ToDouble(tab.Rows[counts]["LONGITUDE"]);
                        ISpatialReference fromSR = ((IGeoDataset)pFClass).SpatialReference;
                        //Create Spatial Reference Factory            
                        ISpatialReferenceFactory srFactory = new SpatialReferenceEnvironmentClass();
                        ISpatialReference sr1;
                        //GCS to project from              
                        IGeographicCoordinateSystem gcs = srFactory.CreateGeographicCoordinateSystem(4269);
                        sr1 = gcs;
                        //Projected Coordinate System to project into            

                        ISpatialReference sr2;
                        sr2 = fromSR;
                        //Point to project             
                        IPoint point = new PointClass() as IPoint;
                        point.PutCoords(longValue, latValue);
                        //Geometry Interface to do actual project             
                        IGeometry geometry;
                        geometry = point;
                        geometry.SpatialReference = sr1;
                        geometry.Project(sr2);
                        featureCreated.Shape = geometry;
                        point = geometry as IPoint;
                        double x;
                        double y;
                        point.QueryCoords(out x, out y);
                        //WriteLine(DateTime.Now.ToLongTimeString() + " Lat: " + x + ", Long: " + y);

                        featureCreated.set_Value(pFClass.FindField("LATITUDE"), latValue);
                        featureCreated.set_Value(pFClass.FindField("LONGITUDE"), longValue);
                        featureCreated.set_Value(pFClass.FindField("STATUS"), "0");
                        featureCreated.set_Value(pFClass.FindField("SAPEQUIPID"), sapequipid);
                        try
                        {
                            featureCreated.Store();
                            WriteLine(DateTime.Now.ToLongTimeString() + ": " + (counts + 1).ToString() + " , Created OBJECTID: " + featureCreated.OID);
                        }
                        catch (Exception ex)
                        {
                            WriteLine(DateTime.Now.ToLongTimeString() + " Error in Storing Feature for Object ID: " + featureCreated.OID + ex.Message);
                        }

                        
                    }  
             
                    ((IWorkspaceEdit)pVersion).StopEditOperation();
                    ((IWorkspaceEdit)pVersion).StopEditing(true);
                    WriteLine(DateTime.Now.ToLongTimeString() + " Total " + pFCName + " features processed : " + counts);
                }
            }
            catch (Exception ex)
            {
                WriteLine(DateTime.Now.ToLongTimeString() + " Error while Processing features " + ex.Message);

            }
            finally
            {
                if (pVersion != null) Marshal.ReleaseComObject(pVersion);
                if (featureCreated != null) Marshal.ReleaseComObject(featureCreated);
                if (pFClass != null) Marshal.ReleaseComObject(pFClass);
            }

        }

        private static IWorkspace ArcSdeWorkspaceFromFile(String connectionFile)
        {
            return ((IWorkspaceFactory)Activator.CreateInstance(Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory"))).
                OpenFromFile(connectionFile, 0);
        }

        private static string CreateVersion(IFeatureWorkspace _featWksp, string input)
        {
            string versionInitialName = ConfigurationManager.AppSettings["VERSION_INITIAL_NAME"];
            IVersion _versionCreated = null;
            try
            {
                _versionCreated = CreateOrReCreateVersion(versionInitialName + input, _featWksp);
                if (_versionCreated == null)
                {
                    throw new Exception("Unable to Create Version:" + versionInitialName + input);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return _versionCreated.VersionName;
        }

        private static IVersion CreateOrReCreateVersion(string vName, IFeatureWorkspace _featWksp)
        {
            IVersionedWorkspace vw = (IVersionedWorkspace)_featWksp;
            IVersion version = null;

            try
            {
                version = vw.FindVersion(vName);
                version.Delete();
                version = ((IVersionedWorkspace)_featWksp).DefaultVersion.CreateVersion(vName);
                version.Access = esriVersionAccess.esriVersionAccessPublic;
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not find version in instance. Trying to create.");
                try
                {
                    version = ((IVersionedWorkspace)_featWksp).DefaultVersion.CreateVersion(vName);
                    version.Access = esriVersionAccess.esriVersionAccessPublic;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Could not create version from default. Tried using " + vName + " as name.");
                    return null;
                }
            }
            return version;
        }

        private static bool DisableAutoUpdaterFramework(out IMMAutoUpdater autoupdater, out mmAutoUpdaterMode oldMode)
        {
            string strDisableAU = ConfigurationManager.AppSettings["DISABLE_AU_FRAMEWORK"].ToString();

            try
            {
                Type type = Type.GetTypeFromProgID("mmGeodatabase.MMAutoUpdater");
                object obj = Activator.CreateInstance(type);
                autoupdater = obj as IMMAutoUpdater;
                oldMode = autoupdater.AutoUpdaterMode;
                //disable all ArcFM Autoupdaters 
                if (Convert.ToBoolean(strDisableAU))
                {

                    autoupdater.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;
                    return true;
                }
                else
                {

                    return true;
                }
            }
            catch (Exception)
            {
                //WriteLine(DateTime.Now + "");
                //throw;
                WriteLine(DateTime.Now.ToLongTimeString() + " Error in disabling Auto Updaters. ");
            }
            autoupdater = null;
            oldMode = mmAutoUpdaterMode.mmAUMStandAlone;
            return false;
        }

        public static System.Data.DataTable ImportExceltoDatatable(string fileName)
        {
            string sheetName = "Sheet1";
            using (System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection())
            {
                DataTable dt = new DataTable();
                string fileExtension = System.IO.Path.GetExtension(fileName);
                if (fileExtension == ".xls")
                    conn.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName + ";" + "Extended Properties='Excel 8.0;HDR=YES;'";
                if (fileExtension == ".xlsx")
                    conn.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";" + "Extended Properties='Excel 12.0 Xml;HDR=YES;'";
                using (System.Data.OleDb.OleDbCommand comm = new System.Data.OleDb.OleDbCommand())
                {
                    comm.CommandText = "Select * from [" + sheetName + "$]";

                    comm.Connection = conn;

                    using (System.Data.OleDb.OleDbDataAdapter da = new System.Data.OleDb.OleDbDataAdapter())
                    {
                        da.SelectCommand = comm;
                        da.Fill(dt);
                        return dt;
                    }

                }
            }
        }


        public static DataTable ConvertCSVtoDataTable(string strFilePath)
        {
            DataTable dt=null;
            string header1 = string.Empty;
            string header2 = string.Empty;
            
            try
            {
                dt = new DataTable();
                using (StreamReader sr = new StreamReader(strFilePath))
                {
                    string[] headers = sr.ReadLine().Split(',');
                    foreach (string header in headers)
                    {
                        header1 = header.TrimStart('"');
                        header2 = header1.TrimEnd('"');
                        dt.Columns.Add(header2);
                    }
                    while (!sr.EndOfStream)
                    {
                        string[] rows = sr.ReadLine().Split(',');


                        DataRow dr = dt.NewRow();
                        for (int i = 0; i < headers.Length; i++)
                        {
                            string row=rows[i].TrimStart('"');
                            rows[i]=row.TrimEnd('"');
                            dr[i] = rows[i];
                        }
                        dt.Rows.Add(dr);
                    }

                }
            }
            catch (Exception ee)
            {
            }
            return dt;
        }
    



        
    }
}
