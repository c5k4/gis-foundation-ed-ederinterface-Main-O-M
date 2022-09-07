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
using ESRI.ArcGIS.Geometry;
using Miner.Interop.Process;
using Updatefeatures;

namespace Updatefeatures
{
    class Program
    {
        private static LicenseInitializer m_AOLicenseInitializer = new Updatefeatures.LicenseInitializer();
        private static string sPath = (String.IsNullOrEmpty(ConfigurationManager.AppSettings["LOGPATH"]) ? System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) : ConfigurationManager.AppSettings["LOGPATH"]) + "\\Logfile_" + DateTime.Now.Ticks.ToString() + ".log";
        private static StreamWriter pSWriter = default(StreamWriter);
        private static IMMAppInitialize arcFMAppInitialize = new MMAppInitializeClass();
        static void Main(string[] args)
        {
            (pSWriter = File.CreateText(sPath)).Close();
            List<string> issueList = new List<string>();

            Console.WriteLine("----Update Features from FC's-----");

            //string issueNumber = Console.ReadLine(); // Read string from console                

            //Console.WriteLine("\n");
            Console.WriteLine("\n");

            string strEDGISSdeConn = ConfigurationManager.AppSettings["CONN_EDGIS_FILE"].ToString();
            string strSdeVerName = ConfigurationManager.AppSettings["VERSON_NAME"].ToString();
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
            UpdateFeatures();

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
        private static void UpdateFeatures()
        {
            string strQueryFilter = string.Empty;
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

                strQueryFilter = ConfigurationManager.AppSettings["QUERY_FILTER"].ToString();
                pQFilter = new QueryFilterClass();
                pQFilter.WhereClause = strQueryFilter;
                WriteLine(DateTime.Now.ToLongTimeString() + " -- Creating Connection " + ConfigurationManager.AppSettings["CONN_EDGIS_FILE"] + " to provided Version " + ConfigurationManager.AppSettings["SESSION_NAME"]);
                IWorkspace objEDGISFW = ArcSdeWorkspaceFromFile(ConfigurationManager.AppSettings["CONN_EDGIS_FILE"]);
                pVersion = ((IVersionedWorkspace)objEDGISFW).FindVersion(ConfigurationManager.AppSettings["VERSON_NAME"]);
                

                string pFCName = ConfigurationManager.AppSettings["UPDATE_FEATURE_CLASS"];             
                pFClass = (pVersion as IFeatureWorkspace).OpenFeatureClass(pFCName);
                pTable = (pVersion as IFeatureWorkspace).OpenTable("EDGIS.SubController");
                //started processing FC's
                WriteLine(DateTime.Now.ToLongTimeString() + " Started processing " + pFCName);
                //To Write change records to GIS-SAP Staging table....
                string strLabeltxt = string.Empty;
                string strLabeltxt4 = string.Empty;
                string strGUIDValue = string.Empty;
                ((IWorkspaceEdit)pVersion).StartEditing(true);
                ((IWorkspaceEdit)pVersion).StartEditOperation();
                pCursor = pFClass.Update(pQFilter, false);
                int iCount = 0;
                if (DisableAutoUpdaterFramework(out autoupdater, out oldMode))
                {
                    while ((pFeat = pCursor.NextFeature()) != null)
                    {
                        WriteLine(DateTime.Now.ToLongTimeString() + " Processing GUID: " + pFeat.OID);

                        CreateRelatedObject(pFeat, pTable);
                        pFeat.Store();
                       
                        iCount++;
                        if (iCount % 5000 == 0)
                        {
                            ((IWorkspaceEdit)pVersion).StopEditOperation();
                            ((IWorkspaceEdit)pVersion).StopEditing(true);
                            ((IWorkspaceEdit)pVersion).StartEditing(true);
                            ((IWorkspaceEdit)pVersion).StartEditOperation();
                            pCursor = pFClass.Update(pQFilter, false);
                        }
                    }
                    ((IWorkspaceEdit)pVersion).StopEditOperation();
                    //autoupdater.AutoUpdaterMode = oldMode;
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
                //if (autoupdater != null) Marshal.ReleaseComObject(autoupdater);
                //if (oldMode != null) Marshal.ReleaseComObject(oldMode);
            }

        }

        public static void CreateRelatedObject(IFeature feature, ITable pTable)
        {
            //assumes that only one RelationshipClass exists for the Origin feature class
            IEnumRelationshipClass enumRelClass = feature.Class.get_RelationshipClasses(esriRelRole.esriRelRoleAny);

            IRelationshipClass rel = enumRelClass.Next();

            //if a feature with no Relationships established has been selected, exit
            if (rel == null)
            {
                return;
            }

            if ((IObject)feature != null)
            {
                ISet relObjs = rel.GetObjectsRelatedToObject((IObject)feature);
                for (IObject relObj = relObjs.Next() as IObject; relObj != null; relObj = relObjs.Next() as IObject)
                {
                    try
                    {
                        relObj.Delete();
                    }
                    catch (Exception)
                    {

                    }
                }
            }

            IRow pNewRowBuffer = pTable.CreateRow();
            //pNewRowBuffer.set_Value(pNewRowBuffer.Fields.FindField("SUBTYPECD"), "RecloserControl");
            //pNewRowBuffer.set_Value(pNewRowBuffer.Fields.FindField("STATUS"), "In Service");
            //pNewRowBuffer.set_Value(pNewRowBuffer.Fields.FindField("CONTROLLERTYPE"), "Form 6");

            pNewRowBuffer.set_Value(pNewRowBuffer.Fields.FindField("SUBTYPECD"), 1);
            pNewRowBuffer.set_Value(pNewRowBuffer.Fields.FindField("STATUS"), 5);
            pNewRowBuffer.set_Value(pNewRowBuffer.Fields.FindField("CONTROLLERTYPE"), 6);

            rel.CreateRelationship((IObject)pNewRowBuffer, (IObject)feature);
        }

        private static void IRelationship_Set(IFeature feature) 
        {
            //assumes that only one RelationshipClass exists for the Origin feature class
            IEnumRelationshipClass enumRelClass = feature.Class.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
            IRelationshipClass relClass = enumRelClass.Next();
            //if a feature with no Relationships established has been selected, exit
            if (relClass == null)
            {
                return;
            }
            ESRI.ArcGIS.esriSystem.ISet relSet = relClass.GetObjectsRelatedToObject((IObject)feature);
            relSet.Reset();
            //If an Attributed Relationship does not exist, exit
            if (relClass.IsAttributed != true)
            {
                return;
            }
            IFeature destinationFeature = (IFeature)relSet.Next();
            while (destinationFeature != null)
            {
                IRelationship relationship = relClass.GetRelationship((IObject)feature, (IObject)destinationFeature);
                IRow row = (IRow)relationship;
                object attributeValue;
                if (row.get_Value(0) == null)
                {
                    attributeValue = "0";
                }
                else
                {
                    attributeValue = row.get_Value(0);
                }
                Console.WriteLine("Destination OID: {0}  Origin OID:  {1}  Attribute value:  {2}", relationship.DestinationObject.get_Value(0), relationship.OriginObject.get_Value(0), attributeValue);
                destinationFeature = (IFeature)relSet.Next();
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
        // for posting cuurent session to default session added by amit 05122017
        public static bool ReconcileAndPostVersion(IVersion2 sapdailyinterfaceversion, IVersion2 defaultVersion)
        {
            bool bOpeartionSuccess = true;
            try
            {
                IMultiuserWorkspaceEdit muWorkspaceEdit = (IMultiuserWorkspaceEdit)sapdailyinterfaceversion;
                IWorkspaceEdit workspaceEdit = (IWorkspaceEdit2)sapdailyinterfaceversion;
                IVersionEdit4 versionEdit = (IVersionEdit4)workspaceEdit;

                if (muWorkspaceEdit.SupportsMultiuserEditSessionMode(esriMultiuserEditSessionMode.esriMESMVersioned))
                {
                    muWorkspaceEdit.StartMultiuserEditing(esriMultiuserEditSessionMode.esriMESMVersioned);
                    //Reconcile with the default version.
                    // Keeping conlict in favour of edit version
                    bool conflicts = versionEdit.Reconcile4(defaultVersion.VersionName, false, false, true, false);
                    if (conflicts)
                    {
                       
                        return bOpeartionSuccess;
                    }
                    workspaceEdit.StartEditOperation();
                    //Getting exception right now -- will be resolved later
                    if (versionEdit.CanPost())
                    {
                        try
                        {
                            versionEdit.Post(defaultVersion.VersionName);
                        }
                        catch
                        {
                           
                            bOpeartionSuccess = false;
                        }
                    }
                    workspaceEdit.StopEditOperation();
                    workspaceEdit.StopEditing(true);
                }
            }
            catch (Exception exp)
            {
               
                // throw;
                bOpeartionSuccess = false;
            }
            return bOpeartionSuccess;
        }


    }


}
