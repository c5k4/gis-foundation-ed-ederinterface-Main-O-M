using System.IO;
using System.Runtime.InteropServices;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.Win32;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.GeoDatabaseDistributed;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.DataManagementTools;
using ESRI.ArcGIS.ADF;

namespace PGE.BatchApplication.FGDBExtraction
{
    class ModFunctions
    {
        private static StreamWriter mtxtStrLogFile;
        internal static string gstrMaster_Path = null;
        //public static string gstrUnderBinPath = null;
        internal static string gdtDateStamp = string.Empty;
        private const string cstSubDirectory = "/Logs/";
        internal static string gstrFgdbPath = string.Empty;
        internal static string gstrFgdbPrefix = string.Empty; 
        internal static string gstrAssemblyPath = null;
        private static InitLicenseManager m_AOLicenseInitializer = null;
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static string mSdeConnString = string.Empty;
        internal static IFeatureWorkspace gFeatWorkspace = null;
        private static IWorkspaceFactory mpWorFatry = null;
        internal static IFeatureClass gmAOILevelFC = null;
        private static bool mblnError = false;        

        public static bool InitializeLicense()
        {
            ExtractDataForm.Instance.LogMessage("Initialise license:");
            bool flag = false;
            m_AOLicenseInitializer = new InitLicenseManager();
            try
            {
                //ESRI License Initializer generated code.
                if (m_AOLicenseInitializer.Initialize(esriLicenseProductCode.esriLicenseProductCodeAdvanced, (Miner.Interop.mmLicensedProductCode)5))
                {
                    flag = true;
                    ExtractDataForm.Instance.LogMessage("SUCCESSFULLY GET LICENSE: " + flag);
                }

            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            finally
            {
                //_licenseManager.Shutdown();
            }
            return flag;
        }

        public static IFeatureWorkspace GetWorkspace(string connectionFile)
        {
            IWorkspace pWS = null;
            IFeatureWorkspace pFWS = null;

            try
            {
                // set feature workspace to the SDE connection                
                if (connectionFile.ToLower().EndsWith(".sde"))
                {
                    if (!File.Exists(connectionFile))
                        ExtractDataForm.Instance.LogMessage("The connection file: " + connectionFile + " does not exist on the file system!");
                    SdeWorkspaceFactory sdeWorkspaceFactory = (SdeWorkspaceFactory)new SdeWorkspaceFactory();
                    pWS = sdeWorkspaceFactory.OpenFromFile(connectionFile, 0);
                }
                else if (connectionFile.ToLower().EndsWith(".gdb"))
                {
                    if (!Directory.Exists(connectionFile))
                        ExtractDataForm.Instance.LogMessage("The file geodatabase: " + connectionFile + " does not exist on the file system!");

                    Type t = Type.GetTypeFromProgID("esriDataSourcesGDB.FileGDBWorkspaceFactory");
                    System.Object obj = Activator.CreateInstance(t);
                    IPropertySet propertySet = new PropertySetClass();
                    propertySet.SetProperty("DATABASE", @connectionFile);
                    IWorkspaceFactory workspaceFactory = (IWorkspaceFactory)obj;
                    pWS = workspaceFactory.OpenFromFile(@connectionFile, 0);
                }

                pFWS = (IFeatureWorkspace)pWS;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error opening SDE workspace.\r\n{0}", ex.Message));
            }
            return pFWS;
        }

        public static IPolygon GetDivisionsEnvelope(
            string landbaseConnectionFile,
            string divisions,
            double bufferDistance)
        {
            try
            {
                ExtractDataForm.Instance.LogMessage("Entering " + MethodBase.GetCurrentMethod());
                ExtractDataForm.Instance.LogMessage(" divisions: " + divisions);
                ExtractDataForm.Instance.LogMessage(" buffer: " + bufferDistance);

                IFeatureWorkspace pSourceFWS = GetWorkspace(landbaseConnectionFile);
                IFeatureClass pDivisionFC = pSourceFWS.OpenFeatureClass("LBGIS.ElecDivision");

                //You can use a spatial filter to create a subset of features to union together, 
                //in order to do that uncomment the next line, set the properties of the spatial filter here.
                //Also change the first parameter in the IFeatureCursor::Seach method.
                //ISpatialFilter queryFilter = new SpatialFilterClass();

                IGeometry geometryBag = new GeometryBagClass();
                IDataset pDS = null;
                IFeatureCursor featureCursor = null;
                IQueryFilter pQF = new QueryFilterClass();
                IFeature currentFeature = null;
                int divisionCount = 0;

                //Define the spatial reference of the bag before adding geometries to it.
                IGeoDataset pGDS = (IGeoDataset)pDivisionFC;
                geometryBag.SpatialReference = pGDS.SpatialReference;
                IGeometryCollection geometryCollection = geometryBag as IGeometryCollection;
                pQF.WhereClause = "DIVISION" + " IN(" + "'" + divisions + "')";
                ExtractDataForm.Instance.LogMessage("pQF.WhereClause: " + pQF.WhereClause);

                //Use a nonrecycling cursor so each returned geometry is a separate object.
                pDS = (IDataset)pDivisionFC;
                ExtractDataForm.Instance.LogMessage("Processing: " + pDS.BrowseName);
                featureCursor = pDivisionFC.Search(pQF, false);
                currentFeature = featureCursor.NextFeature();
                while (currentFeature != null)
                {
                    //Add a reference to this feature's geometry into the bag.
                    //You don't specify the before or after geometry (missing),
                    //so the currentFeature.Shape IGeometry is added to the end of the geometryCollection.
                    divisionCount++;
                    object missing = Type.Missing;
                    geometryCollection.AddGeometry(currentFeature.Shape, ref missing, ref missing);
                    currentFeature = featureCursor.NextFeature();
                }
                Marshal.FinalReleaseComObject(featureCursor);


                // Create the polygon that will be the union of the features returned from the search cursor.
                // The spatial reference of this feature does not need to be set ahead of time. The 
                // ConstructUnion method defines the constructed polygon's spatial reference to be the same as 
                // the input geometry bag.
                ITopologicalOperator unionedPolygon = new PolygonClass();
                unionedPolygon.ConstructUnion(geometryBag as IEnumGeometry);

                //Buffer the polyline by the buffer distance 
                IPolygon pExtractLimitsPolygon = (IPolygon)unionedPolygon.Buffer(bufferDistance);

                //Get the envelope of the buffered polygon 
                IPolygon pExtractLimits = new PolygonClass();
                pExtractLimits.SpatialReference = pGDS.SpatialReference;
                ISegmentCollection pSegColl = (ISegmentCollection)pExtractLimits;
                pSegColl.SetRectangle(pExtractLimitsPolygon.Envelope);

                //Check the area is not empty 
                IArea pArea = (IArea)pExtractLimits;
                ExtractDataForm.Instance.LogMessage("Area of extract envelope: " + pArea.Area.ToString());

                return pExtractLimits;
            }
            catch (Exception ex)
            {
                ExtractDataForm.Instance.LogMessage("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                return null;
            }
        }

        public static void Dispose()
        {
            try { ModFunctions.m_AOLicenseInitializer.Shutdown(); }
            catch (Exception Exf) { }
            try
            {
                ModFunctions.gmAOILevelFC = null;
                ModFunctions.gFeatWorkspace = null;
                ModFunctions.mpWorFatry = null;
                ModFunctions.mSdeConnString = null;
                ModFunctions.gstrFgdbPath = null;
            }
            catch (Exception Exf) { }
        }
        public static bool ErrorOccured
        {
            get { return ModFunctions.mblnError; }
            set { ModFunctions.mblnError = value; }
        }

        

    }
}
