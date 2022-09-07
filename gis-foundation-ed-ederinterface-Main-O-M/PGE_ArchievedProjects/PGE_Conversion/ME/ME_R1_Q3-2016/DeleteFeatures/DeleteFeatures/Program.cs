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
using Miner.Interop;
using System.Configuration;
using System.Collections.Generic;
using ESRI.ArcGIS.Geometry;
using Miner.Interop.Process;
using Miner.Interop;
using DeleteFeatures;

namespace DeleteFeatures
{
    class Program
    {
        private static LicenseInitializer m_AOLicenseInitializer = new DeleteFeatures.LicenseInitializer();
        private static string sPath = (String.IsNullOrEmpty(ConfigurationManager.AppSettings["LOGPATH"]) ? System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) : ConfigurationManager.AppSettings["LOGPATH"]) + "\\Logfile_" + DateTime.Now.Ticks.ToString() + ".log";
        private static StreamWriter pSWriter = default(StreamWriter);
        private static IMMAppInitialize arcFMAppInitialize = new MMAppInitializeClass();
        static void Main(string[] args)
        {
            (pSWriter = File.CreateText(sPath)).Close();
            List<string> issueList = new List<string>();

            Console.WriteLine("----Delte Features from FC's-----");

            //string issueNumber = Console.ReadLine(); // Read string from console                

            //Console.WriteLine("\n");
            Console.WriteLine("\n");

            string strEDGISSdeConn = ConfigurationManager.AppSettings["CONN_EDGIS_FILE"].ToString();
            string strSdeVerName = ConfigurationManager.AppSettings["SESSION_NAME"].ToString();
            Console.WriteLine("Sde Connection Parameter :" + strEDGISSdeConn);
            Console.WriteLine("Sde Version Name Parameter :" + strSdeVerName);

            Console.WriteLine("Please confirm all above important details, before proceed (Y/N):");
            string strConfirm = Console.ReadLine().ToUpper(); // Read string from console

            if (strConfirm == "N") // Try to parse the string as an integer
            {
                Console.Write("Change sde connection string in configuration file and try again");
                return;
            }
            else if (strConfirm != "N" && strConfirm != "Y") // Try to parse the string as an integer
            {
                Console.WriteLine("invalid input value, plz try again");
                return;

            }

            //ESRI License Initializer generated code.
            WriteLine(DateTime.Now.ToLongTimeString() + " -- Initializing Licence");
            m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeBasic, esriLicenseProductCode.esriLicenseProductCodeStandard, esriLicenseProductCode.esriLicenseProductCodeAdvanced },
            new esriLicenseExtensionCode[] { });
            mmLicenseStatus licenseStatus = CheckOutLicenses(mmLicensedProductCode.mmLPArcFM);
            if (RuntimeManager.ActiveRuntime == null)
                RuntimeManager.BindLicense(ProductCode.Desktop);

            //Process feature class wise

            List<string> lstFCNames = new System.Collections.Generic.List<string>();
            //ConfigurationManager.AppSettings["
            //Miner.Interop.Process.IMMSessionManager man=new            

            //ProcessSUPCorrection();
            //EDGIS.ElectricStitchPoint,EDGIS.CurcuitSource
            //Hashtable[] htTBLExcelData = ReadDataFromXls();
            //ProcessCircuitData(htTBLExcelData[0], "EDGIS.ElectricStitchPoint", true);
            //ProcessCircuitData(htTBLExcelData[1], "EDGIS.CircuitSource", false);

            DeleteFeatures();




        }



        private static void WriteLine(string sMsg)
        {
            pSWriter = File.AppendText(sPath);
            pSWriter.WriteLine(sMsg);
            //DrawProgressBar();
            Console.WriteLine(sMsg);
            pSWriter.Close();
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
        private static void DeleteFeatures()
        {
            IVersion pVersion = default(IVersion);
            IFeature pFeat = default(IFeature);
            IFeatureCursor pFCursor = default(IFeatureCursor);
            IQueryFilter pQFilter = default(IQueryFilter);
            IFeatureClass pFClass = default(IFeatureClass);
            ITable pTable = default(ITable);
            //IFeature pFeat = default(IFeature);
            IFeatureCursor pCursor = default(IFeatureCursor);
            
            //List<string> lstTargetFCNames = new System.Collections.Generic.List<string>();
            IMMAutoUpdater autoupdater = default(IMMAutoUpdater);
            mmAutoUpdaterMode oldMode;
            string strLastUser = string.Empty, strVersionName = string.Empty;

            try
            {
                WriteLine(DateTime.Now.ToLongTimeString() + " -- Creating Connection " + ConfigurationManager.AppSettings["CONN_EDGIS_FILE"] + " to provided Version " + ConfigurationManager.AppSettings["SESSION_NAME"]);
                IWorkspace objEDGISFW = ArcSdeWorkspaceFromFile(ConfigurationManager.AppSettings["CONN_EDGIS_FILE"]);
                pVersion = ((IVersionedWorkspace)objEDGISFW).FindVersion(ConfigurationManager.AppSettings["SESSION_NAME"]);
                string pFCName = ConfigurationManager.AppSettings["DELETE_FEATURE_CLASS"];             
                pFClass = (pVersion as IFeatureWorkspace).OpenFeatureClass(pFCName);
                //started processing FC's
                WriteLine(DateTime.Now.ToLongTimeString() + " Started processing " + pFCName);
                //To Write change records to GIS-SAP Staging table....
                
                string strGUIDValue = string.Empty;
                ((IWorkspaceEdit)pVersion).StartEditing(true);
                ((IWorkspaceEdit)pVersion).StartEditOperation();
                pCursor = pFClass.Update(null, false);
                int iCount = 0;
                if (DisableAutoUpdaterFramework(out autoupdater, out oldMode))
                {
                    while ((pFeat = pCursor.NextFeature()) != null)
                    {

                        WriteLine(DateTime.Now.ToLongTimeString() + " Processing GUID: " + pFeat.OID);
                        //pCursor.DeleteFeature();
                        pFeat.Delete();
                        iCount++;
                        //((IWorkspaceEdit)pVersion).
                        if (iCount % 5000 == 0)
                        {
                            ((IWorkspaceEdit)pVersion).StopEditOperation();
                            ((IWorkspaceEdit)pVersion).StopEditing(true);
                            ((IWorkspaceEdit)pVersion).StartEditing(true);
                            ((IWorkspaceEdit)pVersion).StartEditOperation();
                            pCursor = pFClass.Update(null, false);
                        }
                    }
                    ((IWorkspaceEdit)pVersion).StopEditOperation();
                    autoupdater.AutoUpdaterMode = oldMode;
                    ((IWorkspaceEdit)pVersion).StopEditing(true);
                    WriteLine(DateTime.Now.ToLongTimeString() + " Total " + pFCName + " features processed : " + iCount);
                }
            }
            catch (Exception ex)
            {
                WriteLine(DateTime.Now.ToLongTimeString() + " Error while Processing features " + ex.Message);

            }
            finally
            {
                if (pVersion != null) Marshal.ReleaseComObject(pVersion);
                if (pFeat != null) Marshal.ReleaseComObject(pFeat);
                if (pFCursor != null) Marshal.ReleaseComObject(pFCursor);
                if (pQFilter != null) Marshal.ReleaseComObject(pQFilter);
                if (pFClass != null) Marshal.ReleaseComObject(pFClass);
                if (pTable != null) Marshal.ReleaseComObject(pTable);
                //if (pRow != null) Marshal.ReleaseComObject(pRow);
                if (pCursor != null) Marshal.ReleaseComObject(pCursor);
                if (autoupdater != null) Marshal.ReleaseComObject(autoupdater);
                //if (oldMode != null) Marshal.ReleaseComObject(oldMode);
            }

        }
        private static IWorkspace ArcSdeWorkspaceFromFile(String connectionFile)
        {
            return ((IWorkspaceFactory)Activator.CreateInstance(Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory"))).
                OpenFromFile(connectionFile, 0);
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



    }


}
