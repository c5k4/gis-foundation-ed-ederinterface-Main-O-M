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
        private static Dictionary<object, object> _directionDict = new Dictionary<object, object>();
        private static string _objectIdInput = null;
        static void Main(string[] args)
        {
            _objectIdInput = args[0];
            //_objectIdInput = "0";
            //Console.WriteLine(_objectIdInput);
            (pSWriter = File.CreateText(sPath)).Close();
            List<string> issueList = new List<string>();

            Console.WriteLine("----Update Features from FC's-----");

            //string issueNumber = Console.ReadLine(); // Read string from console                

            //Console.WriteLine("\n");
            Console.WriteLine("\n");

            string strEDGISSdeConn = ConfigurationManager.AppSettings["CONN_EDGIS_FILE"].ToString();
            //string strSdeVerName = ConfigurationManager.AppSettings["VERSON_NAME"].ToString();
            Console.WriteLine("Sde Connection Parameter :" + strEDGISSdeConn);
           // Console.WriteLine("Sde Version Name Parameter :" + strSdeVerName);

            //Console.WriteLine("Please confirm all above important details, before proceed (Y/N):");
            //string strConfirm = Console.ReadLine().ToUpper(); // Read string from console

            //if (strConfirm == "N") // Try to parse the string as an integer
            //{
            //    Console.Write("Change sde connection string in configuration file and try again");
            //    return;
            //}
            //else if (strConfirm != "N" && strConfirm != "Y") // Try to parse the string as an integer
            //{
            //    Console.WriteLine("invalid input value, plz try again");
            //    return;

            //}

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
            IFeatureClass pFNewClass = default(IFeatureClass);
            ITable pTable = default(ITable);            
            IFeatureCursor pCursor = default(IFeatureCursor);
            
            IMMAutoUpdater autoupdater = default(IMMAutoUpdater);
            mmAutoUpdaterMode oldMode;
            string strLastUser = string.Empty, strVersionName = string.Empty;

            try
            {

                strQueryFilter = ConfigurationManager.AppSettings["QUERY_FILTER"].ToString();
                pQFilter = new QueryFilterClass();
                WriteLine(DateTime.Now.ToLongTimeString() + " -- Creating Connection " + ConfigurationManager.AppSettings["CONN_EDGIS_FILE"] + " to provided Version " + ConfigurationManager.AppSettings["SESSION_NAME"]);
                IWorkspace objEDGISFW = ArcSdeWorkspaceFromFile(ConfigurationManager.AppSettings["CONN_EDGIS_FILE"]);
                IFeatureWorkspace _featWksp = (IFeatureWorkspace)objEDGISFW;
                //List<string> _versionList = CreateVersion(_featWksp);
                string _VersionToBeCreated = CreateVersion(_featWksp, _objectIdInput);
                //for (int i = 0; i < _versionList.Count; i++)
                //{
                WriteLine(DateTime.Now.ToLongTimeString() + " Version Created: " + _VersionToBeCreated);
                pVersion = ((IVersionedWorkspace)objEDGISFW).FindVersion(_VersionToBeCreated);
                strQueryFilter = strQueryFilter + _objectIdInput + "'";
                string pFCName = ConfigurationManager.AppSettings["UPDATE_FEATURE_CLASS"];
                pFClass = (pVersion as IFeatureWorkspace).OpenFeatureClass(pFCName);
                pFNewClass = (pVersion as IFeatureWorkspace).OpenFeatureClass("EDGIS.Anchor");
                pTable = (pVersion as IFeatureWorkspace).OpenTable("EDGIS.Guy");
                //started processing FC's
                WriteLine(DateTime.Now.ToLongTimeString() + " Started processing " + "EDGIS.Anchor");

                string strLabeltxt = string.Empty;
                string strLabeltxt4 = string.Empty;
                string strGUIDValue = string.Empty;
                ((IWorkspaceEdit)pVersion).StartEditing(true);
                ((IWorkspaceEdit)pVersion).StartEditOperation();
                pQFilter.WhereClause = strQueryFilter;
                pCursor = pFClass.Search(pQFilter, false);
                Int64 iCount = 0;
                DirectionCollection();
                if (DisableAutoUpdaterFramework(out autoupdater, out oldMode))
                {
                    while ((pFeat = pCursor.NextFeature()) != null)
                    {
                        WriteLine(DateTime.Now.ToLongTimeString() + ": " + iCount.ToString() + " , Processing OBJECTID: " + pFeat.OID);

                        CreateRelatedObject(pFeat, pFNewClass, pTable);
                        //pFeat.Store();

                        iCount++;
                        if (iCount % 5000 == 0)
                        {
                            ((IWorkspaceEdit)pVersion).StopEditOperation();
                            ((IWorkspaceEdit)pVersion).StopEditing(true);
                            ((IWorkspaceEdit)pVersion).StartEditing(true);
                            ((IWorkspaceEdit)pVersion).StartEditOperation();
                            //pCursor = pFClass.Search(pQFilter, false);
                        }
                    }
                    ((IWorkspaceEdit)pVersion).StopEditOperation();
                    //autoupdater.AutoUpdaterMode = oldMode;
                    ((IWorkspaceEdit)pVersion).StopEditing(true);
                    WriteLine(DateTime.Now.ToLongTimeString() + " Total " + pFCName + " features processed : " + iCount);
                }
                //}
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

        public static void CreateRelatedObject(IFeature feature, IFeatureClass featureClass, ITable pTable)
        {
            string layerName = null;
            //Create feature in Anchor and Guy            
            IFeature featureCreated = featureClass.CreateFeature();
            IRow pNewRowBuffer = pTable.CreateRow(); 

            // Map Attributes in Anchor and Guy and create relationship
            MapandAssign(feature, featureCreated, pNewRowBuffer);

            #region iterrate all relationship for Anchor fearture class and create record wise relationship in right direction
            //assumes that only one RelationshipClass exists for the Origin feature class
            IEnumRelationshipClass enumRelClassAnchor = featureClass.get_RelationshipClasses(esriRelRole.esriRelRoleAny);

            IRelationshipClass relAnchor = enumRelClassAnchor.Next();
            //if a feature with no Relationships established has been selected, exit
            if (relAnchor == null)
            {
                return;
            }
            while (relAnchor != null)
            {
                layerName = relAnchor.DestinationClass.AliasName.ToUpper();

                if (layerName == "ANCHOR")
                {
                    GetOverlappRelatedObject(feature, featureCreated, relAnchor);                                        
                }
                else if (layerName == "JOINT OWNER")
                {
                    GetOverlappRelatedObject(feature, featureCreated, relAnchor);                    
                }
                else if (layerName == "GUY") 
                {
                    relAnchor.CreateRelationship((IObject)featureCreated, (IObject)pNewRowBuffer);
                }                
                relAnchor = enumRelClassAnchor.Next();
            }
            #endregion          
            
            try
            {
                featureCreated.Shape = feature.Shape;
                try
                {
                    featureCreated.Store();
                    pNewRowBuffer.Store();
                }
                catch (Exception ex)
                {
                    WriteLine(DateTime.Now.ToLongTimeString() + " Error in Storing Feature for Object ID: " + featureCreated.OID + ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void GetOverlappRelatedObject(IFeature feature, IFeature featureCreated, IRelationshipClass relAnchor)
        {
            //assumes that only one RelationshipClass exists for the Origin feature class
            IEnumRelationshipClass enumRelClass = feature.Class.get_RelationshipClasses(esriRelRole.esriRelRoleAny);

            IRelationshipClass rel = enumRelClass.Next();
            //if a feature with no Relationships established has been selected, exit
            if (rel == null)
            {
                return;
            }
            while (rel != null)
            {
                if ((IObject)feature != null)
                {
                    if (rel.DestinationClass.AliasName.ToUpper() == "ANCHOR GUY" && relAnchor.DestinationClass.AliasName.ToUpper() == "ANCHOR")
                    {
                        ISet relObjs = rel.GetObjectsRelatedToObject((IObject)feature);
                        for (IObject relObj = relObjs.Next() as IObject; relObj != null; relObj = relObjs.Next() as IObject)
                        {
                            try
                            {
                                relAnchor.CreateRelationship((IObject)relObj, featureCreated);
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }
                    else if (rel.DestinationClass.AliasName.ToUpper() == "JOINT OWNER" && relAnchor.DestinationClass.AliasName.ToUpper() == "JOINT OWNER")
                    {
                        ISet relObjs = rel.GetObjectsRelatedToObject((IObject)feature);
                        for (IObject relObj = relObjs.Next() as IObject; relObj != null; relObj = relObjs.Next() as IObject)
                        {
                            try
                            {
                                relAnchor.CreateRelationship((IObject)featureCreated, relObj);
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }
                }
                rel = enumRelClass.Next();
            }            
        }

        private static void MapandAssign(IFeature sourceFeature, IFeature destFeature, IRow destRow)
        {
            try
            {
                //populate feature
                try{
                    destFeature.set_Value(destFeature.Fields.FindField("ANCHORSIZE"), sourceFeature.get_Value(sourceFeature.Fields.FindField("ANCHORSIZE")));
                }
                catch (Exception ex) { WriteLine(DateTime.Now.ToLongTimeString() + " Error in assigning field values: " + ex.Message); }

                try{
                destFeature.set_Value(destFeature.Fields.FindField("ANCHORTYPE"), sourceFeature.get_Value(sourceFeature.Fields.FindField("ANCHORTYPE")));
                }
                catch (Exception ex) { WriteLine(DateTime.Now.ToLongTimeString() + " Error in assigning field values: " + ex.Message); }

                try{
                destFeature.set_Value(destFeature.Fields.FindField("CITY"), sourceFeature.get_Value(sourceFeature.Fields.FindField("CITY")));
                }
                catch (Exception ex) { WriteLine(DateTime.Now.ToLongTimeString() + " Error in assigning field values: " + ex.Message); }

                try{
                destFeature.set_Value(destFeature.Fields.FindField("COMMENTS"), sourceFeature.get_Value(sourceFeature.Fields.FindField("COMMENTS")));
                }
                catch (Exception ex) { WriteLine(DateTime.Now.ToLongTimeString() + " Error in assigning field values: " + ex.Message); }

                try{
                destFeature.set_Value(destFeature.Fields.FindField("CONVERSIONID"), sourceFeature.get_Value(sourceFeature.Fields.FindField("CONVERSIONID")));
                }
                catch (Exception ex) { WriteLine(DateTime.Now.ToLongTimeString() + " Error in assigning field values: " + ex.Message); }

                try{
                destFeature.set_Value(destFeature.Fields.FindField("CONVERSIONWORKPACKAGE"), sourceFeature.get_Value(sourceFeature.Fields.FindField("CONVERSIONWORKPACKAGE")));
                }
                catch (Exception ex) { WriteLine(DateTime.Now.ToLongTimeString() + " Error in assigning field values: " + ex.Message); }

                try{
                destFeature.set_Value(destFeature.Fields.FindField("COUNTY"), sourceFeature.get_Value(sourceFeature.Fields.FindField("COUNTY")));
                }
                catch (Exception ex) { WriteLine(DateTime.Now.ToLongTimeString() + " Error in assigning field values: " + ex.Message); }

                try{
                destFeature.set_Value(destFeature.Fields.FindField("CREATIONUSER"), sourceFeature.get_Value(sourceFeature.Fields.FindField("CREATIONUSER")));
                }
                catch (Exception ex) { WriteLine(DateTime.Now.ToLongTimeString() + " Error in assigning field values: " + ex.Message); }

                try{
                destFeature.set_Value(destFeature.Fields.FindField("CUSTOMEROWNED"), sourceFeature.get_Value(sourceFeature.Fields.FindField("CUSTOMEROWNED")));
                }
                catch (Exception ex) { WriteLine(DateTime.Now.ToLongTimeString() + " Error in assigning field values: " + ex.Message); }

                try{
                destFeature.set_Value(destFeature.Fields.FindField("DATECREATED"), sourceFeature.get_Value(sourceFeature.Fields.FindField("DATECREATED")));
                }
                catch (Exception ex) { WriteLine(DateTime.Now.ToLongTimeString() + " Error in assigning field values: " + ex.Message); }

                                // Write function to convert Direction (NE, SE) to Direction (0-360)
                try{
                    if (sourceFeature.get_Value(sourceFeature.Fields.FindField("DIRECTION")) == null || sourceFeature.get_Value(sourceFeature.Fields.FindField("DIRECTION")) == DBNull.Value)
                    {
                    
                    }
                    else
                    {
                        destFeature.set_Value(destFeature.Fields.FindField("DIRECTION"), GetDirectionInDegree(sourceFeature.get_Value(sourceFeature.Fields.FindField("DIRECTION"))));
                    }
                }
                catch (Exception ex) 
                { WriteLine(DateTime.Now.ToLongTimeString() + " Error in assigning field values: " + ex.Message); }

                try{
                destFeature.set_Value(destFeature.Fields.FindField("DISTRICT"), sourceFeature.get_Value(sourceFeature.Fields.FindField("DISTRICT")));
                }
                catch (Exception ex) { WriteLine(DateTime.Now.ToLongTimeString() + " Error in assigning field values: " + ex.Message); }

                try{
                destFeature.set_Value(destFeature.Fields.FindField("DIVISION"), sourceFeature.get_Value(sourceFeature.Fields.FindField("DIVISION")));
                }
                catch (Exception ex) { WriteLine(DateTime.Now.ToLongTimeString() + " Error in assigning field values: " + ex.Message); }

                try{
                destFeature.set_Value(destFeature.Fields.FindField("INSTALLATIONDATE"), sourceFeature.get_Value(sourceFeature.Fields.FindField("INSTALLATIONDATE")));
                }
                catch (Exception ex) { WriteLine(DateTime.Now.ToLongTimeString() + " Error in assigning field values: " + ex.Message); }
                
                try{
                destFeature.set_Value(destFeature.Fields.FindField("INSTALLJOBNUMBER"), sourceFeature.get_Value(sourceFeature.Fields.FindField("INSTALLJOBNUMBER")));
                }
                catch (Exception ex) { WriteLine(DateTime.Now.ToLongTimeString() + " Error in assigning field values: " + ex.Message); }

                try{
                destFeature.set_Value(destFeature.Fields.FindField("INSTALLJOBPREFIX"), sourceFeature.get_Value(sourceFeature.Fields.FindField("INSTALLJOBPREFIX")));
                }
                catch (Exception ex) { WriteLine(DateTime.Now.ToLongTimeString() + " Error in assigning field values: " + ex.Message); }
                
                try{
                destFeature.set_Value(destFeature.Fields.FindField("INSTALLJOBYEAR"), sourceFeature.get_Value(sourceFeature.Fields.FindField("INSTALLJOBYEAR")));
                }
                catch (Exception ex) { WriteLine(DateTime.Now.ToLongTimeString() + " Error in assigning field values: " + ex.Message); }

                try{
                destFeature.set_Value(destFeature.Fields.FindField("JOINTOWNED"), sourceFeature.get_Value(sourceFeature.Fields.FindField("JOINTOWNED")));
                }
                catch (Exception ex) { WriteLine(DateTime.Now.ToLongTimeString() + " Error in assigning field values: " + ex.Message); }

                try{
                destFeature.set_Value(destFeature.Fields.FindField("JPADATE"), sourceFeature.get_Value(sourceFeature.Fields.FindField("JPADATE")));
                }
                catch (Exception ex) { WriteLine(DateTime.Now.ToLongTimeString() + " Error in assigning field values: " + ex.Message); }

                try{
                destFeature.set_Value(destFeature.Fields.FindField("JPANUMBER"), sourceFeature.get_Value(sourceFeature.Fields.FindField("JPANUMBER")));
                }
                catch (Exception ex) { WriteLine(DateTime.Now.ToLongTimeString() + " Error in assigning field values: " + ex.Message); }

                try{
                destFeature.set_Value(destFeature.Fields.FindField("JPASEQUENCE"), sourceFeature.get_Value(sourceFeature.Fields.FindField("JPASEQUENCE")));
                }
                catch (Exception ex) { WriteLine(DateTime.Now.ToLongTimeString() + " Error in assigning field values: " + ex.Message); }

                try{
                destFeature.set_Value(destFeature.Fields.FindField("LABELTEXT"), sourceFeature.get_Value(sourceFeature.Fields.FindField("LABELTEXT")));
                }
                catch (Exception ex) { WriteLine(DateTime.Now.ToLongTimeString() + " Error in assigning field values: " + ex.Message); }

                try{
                destFeature.set_Value(destFeature.Fields.FindField("LASTUSER"), sourceFeature.get_Value(sourceFeature.Fields.FindField("LASTUSER")));
                }
                catch (Exception ex) { WriteLine(DateTime.Now.ToLongTimeString() + " Error in assigning field values: " + ex.Message); }

                try
                {
                    destFeature.set_Value(destFeature.Fields.FindField("LEAD"), sourceFeature.get_Value(sourceFeature.Fields.FindField("LEAD")));
                }
                catch (Exception ex) { WriteLine(DateTime.Now.ToLongTimeString() + " Error in assigning field values: " + ex.Message); }

                try{
                destFeature.set_Value(destFeature.Fields.FindField("LOCATIONDESCRIPTION"), sourceFeature.get_Value(sourceFeature.Fields.FindField("LOCATIONDESC")));
                }
                catch (Exception ex) { WriteLine(DateTime.Now.ToLongTimeString() + " Error in assigning field values: " + ex.Message); }

                try{
                destFeature.set_Value(destFeature.Fields.FindField("MANUFACTURER"), sourceFeature.get_Value(sourceFeature.Fields.FindField("MANUFACTURER")));
                }
                catch (Exception ex) { WriteLine(DateTime.Now.ToLongTimeString() + " Error in assigning field values: " + ex.Message); }

                try{
                destFeature.set_Value(destFeature.Fields.FindField("REGION"), sourceFeature.get_Value(sourceFeature.Fields.FindField("REGION")));
                }
                catch (Exception ex) { WriteLine(DateTime.Now.ToLongTimeString() + " Error in assigning field values: " + ex.Message); }

                try{
                destFeature.set_Value(destFeature.Fields.FindField("REPLACEGUID"), sourceFeature.get_Value(sourceFeature.Fields.FindField("REPLACEGUID")));
                }
                catch (Exception ex) { WriteLine(DateTime.Now.ToLongTimeString() + " Error in assigning field values: " + ex.Message); }

                try{
                destFeature.set_Value(destFeature.Fields.FindField("RETIREDATE"), sourceFeature.get_Value(sourceFeature.Fields.FindField("RETIREDATE")));
                }
                catch (Exception ex) { WriteLine(DateTime.Now.ToLongTimeString() + " Error in assigning field values: " + ex.Message); }

                try{
                destFeature.set_Value(destFeature.Fields.FindField("STATUS"), sourceFeature.get_Value(sourceFeature.Fields.FindField("STATUS")));
                }
                catch (Exception ex) { WriteLine(DateTime.Now.ToLongTimeString() + " Error in assigning field values: " + ex.Message); }

                try{
                destFeature.set_Value(destFeature.Fields.FindField("STRUCTURECONVID"), sourceFeature.get_Value(sourceFeature.Fields.FindField("STRUCTURECONVID")));
                }
                catch (Exception ex) { WriteLine(DateTime.Now.ToLongTimeString() + " Error in assigning field values: " + ex.Message); }

                try{
                destFeature.set_Value(destFeature.Fields.FindField("STRUCTUREGUID"), sourceFeature.get_Value(sourceFeature.Fields.FindField("STRUCTUREGUID")));
                }
                catch (Exception ex) { WriteLine(DateTime.Now.ToLongTimeString() + " Error in assigning field values: " + ex.Message); }

                try{
                destFeature.set_Value(destFeature.Fields.FindField("SUBTYPECD"), 1);
                }
                catch (Exception ex) { WriteLine(DateTime.Now.ToLongTimeString() + " Error in assigning field values: " + ex.Message); }

                try{
                destFeature.set_Value(destFeature.Fields.FindField("SYMBOLNUMBER"), sourceFeature.get_Value(sourceFeature.Fields.FindField("SYMBOLNUMBER")));
                }
                catch (Exception ex) { WriteLine(DateTime.Now.ToLongTimeString() + " Error in assigning field values: " + ex.Message); }

                try{
                destFeature.set_Value(destFeature.Fields.FindField("SYMBOLROTATION"), sourceFeature.get_Value(sourceFeature.Fields.FindField("SYMBOLROTATION")));
                }
                catch (Exception ex) { WriteLine(DateTime.Now.ToLongTimeString() + " Error in assigning field values: " + ex.Message); }

                try{
                destFeature.set_Value(destFeature.Fields.FindField("ZIP"), sourceFeature.get_Value(sourceFeature.Fields.FindField("ZIP")));
                }
                catch (Exception ex) { WriteLine(DateTime.Now.ToLongTimeString() + " Error in assigning field values: " + ex.Message); }

                //populate related table

                try{
                destRow.set_Value(destRow.Fields.FindField("CREATIONUSER"), sourceFeature.get_Value(sourceFeature.Fields.FindField("CREATIONUSER")));
                }
                catch (Exception ex) { WriteLine(DateTime.Now.ToLongTimeString() + " Error in assigning field values: " + ex.Message); }

                try{
                destRow.set_Value(destRow.Fields.FindField("CUSTOMEROWNED"), sourceFeature.get_Value(sourceFeature.Fields.FindField("CUSTOMEROWNED")));
                }
                catch (Exception ex) { WriteLine(DateTime.Now.ToLongTimeString() + " Error in assigning field values: " + ex.Message); }

                try{
                destRow.set_Value(destRow.Fields.FindField("DATECREATED"), DateTime.Now);
                }
                catch (Exception ex) { WriteLine(DateTime.Now.ToLongTimeString() + " Error in assigning field values: " + ex.Message); }

                try{
                destRow.set_Value(destRow.Fields.FindField("GUYCOUNT"), sourceFeature.get_Value(sourceFeature.Fields.FindField("GUYCOUNT")));
                }
                catch (Exception ex) { WriteLine(DateTime.Now.ToLongTimeString() + " Error in assigning field values: " + ex.Message); }

                try{
                destRow.set_Value(destRow.Fields.FindField("GUYTYPE"), sourceFeature.get_Value(sourceFeature.Fields.FindField("GUYTYPE")));
                }
                catch (Exception ex) { WriteLine(DateTime.Now.ToLongTimeString() + " Error in assigning field values: " + ex.Message); }

                try{
                destRow.set_Value(destRow.Fields.FindField("STATUS"), sourceFeature.get_Value(sourceFeature.Fields.FindField("STATUS")));
                }
                catch (Exception ex) { WriteLine(DateTime.Now.ToLongTimeString() + " Error in assigning field values: " + ex.Message); }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private static object GetDirectionInDegree(object p)
        {
            object _directionInDegree = null;
            try
            {
                _directionDict.TryGetValue(p, out _directionInDegree);
            }
            catch (Exception e)
            { throw e; }
            return _directionInDegree;
        }
        
        private static void DirectionCollection()
        {            
            _directionDict.Add("N", 0);
            _directionDict.Add("S", 180);
            _directionDict.Add("E", 90);
            _directionDict.Add("W", 270);
            _directionDict.Add("NW", 315);
            _directionDict.Add("NE", 45);
            _directionDict.Add("SW", 225);
            _directionDict.Add("SE", 135);        
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

        public List<string> GetObjectIDForProcessVersionWise(List<string> _versionList, Int64 totalRecord, Int64 minValue, Int64 maxValue)
        {
            List<string> _versionListOID = new List<string>();
            try
            {
                if (_versionList != null && _versionList.Count > 0)
                {
                    int recordCount = Convert.ToInt32(totalRecord / _versionList.Count);
                    for (int i = 0; i < _versionList.Count; i++)
                    { 
                    //_versionListOID.Add(_versionList[i]+","+minValue+","+
                    
                    }
                }
            }
            catch (Exception e)
            {
            
            }
            return _versionListOID;
        }

        private static string CreateVersion(IFeatureWorkspace _featWksp, string input)
        {
            string versionInitialName = ConfigurationManager.AppSettings["VERSION_INITIAL_NAME"];  
            IVersion _versionCreated= null;
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

        private static List<string> CreateVersion(IFeatureWorkspace _featWksp)
        {
            List<string> _versionList= new List<string>();
            int versionCount = Convert.ToInt16(ConfigurationManager.AppSettings["NO_OF_VERSION_TO_BE_CREATED"].ToString());
            string versionInitialName = "Anchor_";
            for (int i = 0; i < versionCount; i++)
            {
                try
                {
                    IVersion _versionCreated = CreateOrReCreateVersion(versionInitialName + i.ToString(), _featWksp);
                    if (_versionCreated == null)
                    {
                        throw new Exception("Unable to Create Version:" + versionInitialName + i.ToString());
                    }
                    else 
                    {
                        _versionList.Add(_versionCreated.VersionName);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            return _versionList;
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
