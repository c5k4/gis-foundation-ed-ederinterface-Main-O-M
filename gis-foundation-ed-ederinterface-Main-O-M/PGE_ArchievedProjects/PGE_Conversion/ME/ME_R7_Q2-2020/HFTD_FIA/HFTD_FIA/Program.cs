#region reference
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using System.Collections.Generic;
//using System.Data.OracleClient;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using System.Collections.Generic;

using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

using System.Collections;

using System.Configuration;
using System.Collections.Generic;

using Miner.Interop.Process;
using System.Collections;

using System.Configuration;
using System.Collections.Generic;

using Miner.Interop.Process;

using DeleteFeatures;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geometry;
#endregion

namespace HFTD_FIA
{
    class Program
    {
        private static LicenseInitializer m_AOLicenseInitializer = new DeleteFeatures.LicenseInitializer();
        private static string sPath = (String.IsNullOrEmpty(ConfigurationManager.AppSettings["LOGPATH"]) ? System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) : ConfigurationManager.AppSettings["LOGPATH"]) + "\\Logfile_" + DateTime.Now.Ticks.ToString() + ".log";
        private static string updateFeature = ConfigurationManager.AppSettings["UPDATE_FEATURE_CLASS"].ToUpper().ToString();
        private static string HFTD_FIA_Table = ConfigurationManager.AppSettings["HFTD_FIA_TABLE"].ToUpper().ToString();
        private static string sPathScript = (String.IsNullOrEmpty(ConfigurationManager.AppSettings["SCRIPT_PATH"]) ? System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) : ConfigurationManager.AppSettings["SCRIPT_PATH"]) + "\\" + updateFeature.Substring(6) +  ".sql";

        private static StreamWriter pSWriter = default(StreamWriter);
        private static StreamWriter pSWriterScript = default(StreamWriter);
        private static IMMAppInitialize arcFMAppInitialize = new MMAppInitializeClass();
        private static string strEDGISSdeConn = null;
        private static string strLBGISSdeConn = null;
        private static string strSdeVerName = null;        
        
        private static IWorkspace objEDGISFW = null;
        private static IWorkspace objLBGISFW = null;

        private static IFeatureClass polyHFTD = null;
        private static IFeatureClass polyFIA = null;

        static void Main(string[] args)
        {
            (pSWriter = File.CreateText(sPath)).Close();
            List<string> issueList = new List<string>();

            (pSWriterScript = File.CreateText(sPathScript)).Close();

            //Console.WriteLine("----LiDAR Corrections Feature creation start-----");
            WriteLine(DateTime.Now.ToLongTimeString() + " ----Process started-----");
            Console.WriteLine("\n");

            strEDGISSdeConn = ConfigurationManager.AppSettings["CONN_EDGIS_FILE"].ToString();
            strLBGISSdeConn = ConfigurationManager.AppSettings["CONN_LBGIS_FILE"].ToString();
            //strSdeVerName = ConfigurationManager.AppSettings["VERSION_NAME"].ToString();
            //WriteLineScript("Insert stataement");

            WriteLine(DateTime.Now.ToLongTimeString() + " Sde Connection Parameter for EDGIS :" + strEDGISSdeConn);
            WriteLine(DateTime.Now.ToLongTimeString() + " Sde Connection Parameter for LBGIS :" + strLBGISSdeConn);
            //WriteLine(DateTime.Now.ToLongTimeString() + " Sde Version Name Parameter :" + strSdeVerName);
            //ESRI License Initializer generated code.
            WriteLine(DateTime.Now.ToLongTimeString() + " -- Initializing Licence");
            m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeBasic, esriLicenseProductCode.esriLicenseProductCodeStandard, esriLicenseProductCode.esriLicenseProductCodeAdvanced },
            new esriLicenseExtensionCode[] { });
            mmLicenseStatus licenseStatus = CheckOutLicenses(mmLicensedProductCode.mmLPArcFM);
            if (RuntimeManager.ActiveRuntime == null) { RuntimeManager.BindLicense(ProductCode.Desktop); }
            


            WriteLineScript(@"spool D:\Temp\InsertData_"+updateFeature.Substring(6)+".txt");
            WriteLineScript("SET SERVEROUTPUT ON;");
            WriteLineScript("SET DEFINE OFF;");
            WriteLineScript("");

            HFTD_FIA_Main();

            WriteLineScript("COMMIT;");
            WriteLineScript("/");
            WriteLineScript("SET DEFINE ON;");
            WriteLineScript("spool off;");

            WriteLine(DateTime.Now.ToLongTimeString() + " -------------Process completed-------- ");
            Console.ReadKey();
        }

        private static void HFTD_FIA_Main()
        {
            objEDGISFW = ArcSdeWorkspaceFromFile(ConfigurationManager.AppSettings["CONN_EDGIS_FILE"]);
            objLBGISFW = ArcSdeWorkspaceFromFile(ConfigurationManager.AppSettings["CONN_LBGIS_FILE"]);

            if (objLBGISFW != null)
            {
                WriteLine(DateTime.Now.ToLongTimeString() + " Getting HFTD and FIA feaure Class");
                polyHFTD = getFeatureClass(objLBGISFW, "LBGIS.HighFireThreatDist");
                polyFIA = getFeatureClass(objLBGISFW, "LBGIS.CalFireAdjIndex");
            }

            
            IFeatureClass featClass = (objEDGISFW as IFeatureWorkspace).OpenFeatureClass(updateFeature);
            ITable hftdFiatbl = (objEDGISFW as IFeatureWorkspace).OpenTable(HFTD_FIA_Table);

            IQueryFilter qf = new QueryFilter();
            qf.WhereClause = ConfigurationManager.AppSettings["HFTD_FIA_TABLE_WHERE_CLAUSE"];
            ICursor pCur = hftdFiatbl.Search(qf, false);

            IRow rw = null;
            int indexGlobalid = pCur.FindField("GLOBAL_ID"); 
            while ((rw = pCur.NextRow()) != null)
            {
                IQueryFilter pF = new QueryFilter();
                pF.WhereClause = "GLOBALID = '"+ rw.get_Value(indexGlobalid).ToString()+"'";
                IFeatureCursor fCur = featClass.Search(pF, false);

                IRow fRw = fCur.NextFeature();
                if (fRw != null)
                {
                    string HFTD = string.Empty;
                    string FIA = string.Empty;
                    string globalID = string.Empty;

                    HFTD = getHFTD(fRw, polyHFTD);
                    FIA = getFIA(fRw, polyFIA);
                    if (!(HFTD.Equals("Neither") && FIA.Equals("9")))
                    {
                        string datestring = DateTime.Now.ToString("dd-MMM-yy");
                        string sqlInsert = "Insert into " + HFTD_FIA_Table + " values ('" + rw.get_Value(indexGlobalid).ToString() + "','" + updateFeature + "','" + HFTD + "','" + FIA + "','" + datestring + "');";
                        WriteLineScript(sqlInsert);

                        WriteLine(DateTime.Now.ToLongTimeString() + " Record found to insert in table " + HFTD_FIA_Table + " for GLOBAL_ID " + rw.get_Value(indexGlobalid).ToString());
                    }
                    else
                    {
                        WriteLine(DateTime.Now.ToLongTimeString() + " As HFTD " + HFTD + " and FIA " + FIA + " found & both is default value for null value found so Skipping the Record to insert in table " + HFTD_FIA_Table + " for GLOBAL_ID " + rw.get_Value(indexGlobalid).ToString());
                    }
                }
                //Releasing the cursor
                if (fCur != null)
                {
                    Marshal.ReleaseComObject(fCur);                    
                }
            }

            //Releasing the cursor
            if (pCur != null)
            {
                Marshal.ReleaseComObject(pCur);
            }

            //IFeatureClass SuppStructFC = (cVer as IFeatureWorkspace).OpenFeatureClass(FCName); 
            
        }

        public static string getHFTD(IRow differenceRow, IFeatureClass FeatureClass)
        {
            //LBGIS.HighFireThreatDist  //LBGIS.PGE_LANDBASE   //CPUC_TIER  CPUC_FIRE_MAP
            //LBGIS.CalFireAdjIndex   //LBGIS.CA_LANDBASE  //NAME1
            string HFTD = string.Empty;

            IFeature feature = differenceRow as IFeature;
            Boolean isPointFeature = (feature.Shape.GeometryType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint);

            IFeatureClass polyHFTD = FeatureClass;

            if (isPointFeature)
            {
                WriteLine(DateTime.Now.ToLongTimeString() + " getting HFTD for Point feature : " + feature.Class.AliasName.ToString());

                ISpatialFilter pSFilter = new SpatialFilterClass();
                pSFilter.Geometry = feature.Shape;
                pSFilter.SubFields = "HFTD";
                pSFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                IFeatureCursor pFCursor = polyHFTD.Search(pSFilter, true);
                IFeature pPolyFeature = pFCursor.NextFeature();

                if (pPolyFeature != null)
                {
                    HFTD = pPolyFeature.get_Value(pPolyFeature.Fields.FindField("HFTD")).ToString();
                }
                //Releasing the cursor
                if (pFCursor != null)
                {
                    Marshal.ReleaseComObject(pFCursor);
                }

            }
            //Line Feature
            else
            {
                string hftdFrom = string.Empty;
                string hftdTo = string.Empty;
                string oidOH = feature.OID.ToString();
                IPolyline polyLine = feature.Shape as IPolyline;

                IPoint fromPT = polyLine.FromPoint as IPoint;
                IPoint toPT = polyLine.ToPoint as IPoint;
                WriteLine(DateTime.Now.ToLongTimeString() + " getting HFTD for Line feature : " + feature.Class.AliasName.ToString());

                //IFeature fromPTFeat = fromPT as IFeature;
                //IFeature ToPTFeat = toPT as IFeature;

                ISpatialFilter frompSFilter = new SpatialFilterClass();
                frompSFilter.Geometry = fromPT;
                frompSFilter.SubFields = "HFTD";
                frompSFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                IFeatureCursor FrompFCursor = polyHFTD.Search(frompSFilter, true);
                IFeature pPolyFeatureFrom = FrompFCursor.NextFeature();
                if (pPolyFeatureFrom != null)
                {
                    hftdFrom = pPolyFeatureFrom.get_Value(pPolyFeatureFrom.Fields.FindField("HFTD")).ToString();
                }
                //Releasing the cursor
                if (FrompFCursor != null)
                {
                    Marshal.ReleaseComObject(FrompFCursor);
                }

                ISpatialFilter topSFilter = new SpatialFilterClass();
                topSFilter.Geometry = toPT;
                topSFilter.SubFields = "HFTD";
                topSFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                IFeatureCursor TopFCursor = polyHFTD.Search(topSFilter, true);
                IFeature pPolyFeatureTo = TopFCursor.NextFeature();
                if (pPolyFeatureTo != null)
                {
                    hftdTo = pPolyFeatureTo.get_Value(pPolyFeatureTo.Fields.FindField("HFTD")).ToString();
                }
                //Releasing the cursor
                if (TopFCursor != null)
                {
                    Marshal.ReleaseComObject(TopFCursor);
                }

                HFTD = CompareHFTD(hftdFrom, hftdTo);

            }
            HFTD = getHFTDDescription(HFTD);
            return HFTD;
        }

        private static string CompareHFTD(string hftdFrom, string hftdTo)
        {
            string hftdvalue = string.Empty;
            int _hftdFrom = 0; int _hftdTo = 0;

            if (!string.IsNullOrEmpty(hftdFrom))
            {
                if (hftdFrom.Contains("0")) { _hftdFrom = 0; }
                if (hftdFrom.Contains("1")) { _hftdFrom = 1; }
                if (hftdFrom.Contains("2")) { _hftdFrom = 2; }
                if (hftdFrom.Contains("3")) { _hftdFrom = 3; }
            }
            if (!string.IsNullOrEmpty(hftdTo))
            {
                if (hftdTo.Contains("0")) { _hftdTo = 0; }
                if (hftdTo.Contains("1")) { _hftdTo = 1; }
                if (hftdTo.Contains("2")) { _hftdTo = 2; }
                if (hftdTo.Contains("3")) { _hftdTo = 3; }
            }

            if (string.IsNullOrEmpty(hftdFrom) && !string.IsNullOrEmpty(hftdTo)) { hftdvalue = _hftdTo.ToString(); }
            if (!string.IsNullOrEmpty(hftdTo) && string.IsNullOrEmpty(hftdTo)) { hftdvalue = _hftdFrom.ToString(); }
            if (!string.IsNullOrEmpty(hftdFrom) && !string.IsNullOrEmpty(hftdTo))
            {
                if (_hftdFrom == _hftdTo) { hftdvalue = _hftdFrom.ToString(); }
                if (_hftdFrom > _hftdTo) { hftdvalue = _hftdFrom.ToString(); }
                if (_hftdFrom < _hftdTo) { hftdvalue = _hftdTo.ToString(); }
            }

            if (string.IsNullOrEmpty(hftdTo) && string.IsNullOrEmpty(hftdTo)) { hftdvalue = "0"; }

            return hftdvalue;
        }
        private static string getHFTDDescription(string hftdvalue)
        {
            string hftdDesc = string.Empty;
            if (!string.IsNullOrEmpty(hftdvalue))
            {
                if (hftdvalue.Contains("0")) { hftdDesc = "Neither"; }
                if (hftdvalue.Contains("1")) { hftdDesc = "Zone 1"; }
                if (hftdvalue.Contains("2")) { hftdDesc = "Tier 2"; }
                if (hftdvalue.Contains("3")) { hftdDesc = "Tier 3"; }
            }
            else
            {
                //No HFTD found so setting HFTD as Neither 
                hftdDesc = "Neither";
            }
            return hftdDesc;
        }

        private static string getFIADescription(string FIAvalue)
        {
            string FIADesc = string.Empty;
            if (!string.IsNullOrEmpty(FIAvalue))
            {
                FIADesc = FIAvalue;
            }
            else
            {
                //No FIA found so setting FIA as 9 
                FIADesc = "9";
            }
            return FIADesc;
        }

        public static string getFIA(IRow differenceRow, IFeatureClass FeatureClass)
        {
            //LBGIS.HighFireThreatDist  //LBGIS.PGE_LANDBASE   //CPUC_TIER  CPUC_FIRE_MAP
            //LBGIS.CalFireAdjIndex   //LBGIS.CA_LANDBASE  //NAME1
            string FIA = string.Empty;

            IFeature feature = differenceRow as IFeature;
            Boolean isPointFeature = (feature.Shape.GeometryType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint);

            IFeatureClass polyFIA = FeatureClass;

            if (isPointFeature)
            {
                WriteLine(DateTime.Now.ToLongTimeString() + " getting FIA for Point feature : " + feature.Class.AliasName.ToString());

                ISpatialFilter pSFilter = new SpatialFilterClass();
                pSFilter.Geometry = feature.Shape;
                pSFilter.SubFields = "NAME1";
                pSFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                IFeatureCursor pFCursor = polyFIA.Search(pSFilter, true);
                IFeature pPolyFeature = pFCursor.NextFeature();
                if (pPolyFeature != null)
                {
                    FIA = pPolyFeature.get_Value(pPolyFeature.Fields.FindField("NAME1")).ToString();
                }
                //Relesing the cursor
                if (pFCursor != null)
                {
                    Marshal.ReleaseComObject(pFCursor);
                }



            }
            //Line Feature
            else
            {
                string FIAFrom = string.Empty;
                string FIATo = string.Empty;
                string oidOH = feature.OID.ToString();
                IPolyline polyLine = feature.Shape as IPolyline;
                IPoint fromPT = polyLine.FromPoint as IPoint;
                IPoint toPT = polyLine.ToPoint as IPoint;
                WriteLine(DateTime.Now.ToLongTimeString() + " getting FIA for Line feature : " + feature.Class.AliasName.ToString());

                ISpatialFilter frompSFilter = new SpatialFilterClass();
                frompSFilter.Geometry = fromPT;
                frompSFilter.SubFields = "NAME1";
                frompSFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                IFeatureCursor FrompFCursor = polyFIA.Search(frompSFilter, true);
                IFeature pPolyFeatureFrom = FrompFCursor.NextFeature();
                if (pPolyFeatureFrom != null)
                {
                    FIAFrom = pPolyFeatureFrom.get_Value(pPolyFeatureFrom.Fields.FindField("NAME1")).ToString();
                }
                //Releasing the cursor
                if (FrompFCursor != null)
                {
                    Marshal.ReleaseComObject(FrompFCursor);
                }

                ISpatialFilter topSFilter = new SpatialFilterClass();
                topSFilter.Geometry = toPT;
                topSFilter.SubFields = "NAME1";
                topSFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                IFeatureCursor TopFCursor = polyFIA.Search(topSFilter, true);
                IFeature pPolyFeatureTo = TopFCursor.NextFeature();
                if (pPolyFeatureTo != null)
                {
                    FIATo = pPolyFeatureTo.get_Value(pPolyFeatureTo.Fields.FindField("NAME1")).ToString();
                }
                //Releasing the cursor
                if (TopFCursor != null)
                {
                    Marshal.ReleaseComObject(TopFCursor);
                }

                FIA = CompareFIA(FIAFrom, FIATo);

            }
            FIA = getFIADescription(FIA);
            return FIA;
        }

        private static string CompareFIA(string FIAFrom, string FIATo)
        {
            string FIAvalue = string.Empty;
            int _FIAFrom = 0; int _FIATo = 0;
            if (!string.IsNullOrEmpty(FIAFrom)) { _FIAFrom = Convert.ToInt32(FIAFrom); }
            if (!string.IsNullOrEmpty(FIATo)) { _FIATo = Convert.ToInt32(FIATo); }

            if (string.IsNullOrEmpty(FIAFrom) && !string.IsNullOrEmpty(FIATo)) { FIAvalue = _FIATo.ToString(); }
            if (!string.IsNullOrEmpty(FIATo) && string.IsNullOrEmpty(FIATo)) { FIAvalue = _FIAFrom.ToString(); }
            if (!string.IsNullOrEmpty(FIAFrom) && !string.IsNullOrEmpty(FIATo))
            {
                if (_FIAFrom == _FIATo) { FIAvalue = _FIAFrom.ToString(); }
                if (_FIAFrom > _FIATo) { FIAvalue = _FIAFrom.ToString(); }
                if (_FIAFrom < _FIATo) { FIAvalue = _FIATo.ToString(); }
            }
            if (string.IsNullOrEmpty(FIATo) && string.IsNullOrEmpty(FIATo)) { FIAvalue = null; }

            return FIAvalue;
        }
       

        //get workspace from sde
        private static IWorkspace ArcSdeWorkspaceFromFile(String connectionFile)
        {
            return ((IWorkspaceFactory)Activator.CreateInstance(Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory"))).
                OpenFromFile(connectionFile, 0);
        }

        //get workspace from gdb
        public static IWorkspace WorkSpaceFromGDB(string dataPath)
        {
            IWorkspace workSpace = null;
            try
            {
                IWorkspaceFactory workspaceFactory = new ESRI.ArcGIS.DataSourcesGDB.FileGDBWorkspaceFactory();
                workSpace = workspaceFactory.OpenFromFile(dataPath, 0);
            }
            catch (Exception ex) { throw new Exception("Error returning raster workspace from shared path"); }
            return workSpace;
        }

        private static void WriteLine(string sMsg)
        {
            pSWriter = File.AppendText(sPath);
            pSWriter.WriteLine(sMsg);
            //DrawProgressBar();
            Console.WriteLine(sMsg);
            pSWriter.Close();
        }

        private static void WriteLineScript(string sMsg)
        {
            pSWriterScript = File.AppendText(sPathScript);
            pSWriterScript.WriteLine(sMsg);
            //DrawProgressBar();
            //Console.WriteLine(sMsg);
            pSWriterScript.Close();
        }

        //Get featureClass by Name
        public static IFeatureClass getFeatureClass(IWorkspace pWorkspace, string featureClassName)
        {
            //LBGIS.HighFireThreatDist  //LBGIS.PGE_LANDBASE   //CPUC_TIER  CPUC_FIRE_MAP
            //LBGIS.CalFireAdjIndex   //LBGIS.CA_LANDBASE  //NAME1

            IFeatureClass featureCls = null;
            WriteLine(DateTime.Now.ToLongTimeString() + " getting feature class for : " + featureClassName);
            try
            {//esriDTFeatureDataset
                IEnumDataset datasets = pWorkspace.get_Datasets(esriDatasetType.esriDTAny);
                datasets.Reset();
                IDataset dataset = null;
                while ((dataset = datasets.Next()) != null)
                {
                    if (dataset.Type == esriDatasetType.esriDTFeatureDataset)
                    {
                        IEnumDataset enumDataSet = null;
                        enumDataSet = dataset.Subsets;
                        enumDataSet.Reset();
                        IDataset ds = null;
                        while ((ds = enumDataSet.Next()) != null)
                        {
                            if (ds is IFeatureClass)
                            {
                                if ((ds.Name.ToUpper().Equals(featureClassName.ToUpper())))
                                {
                                    featureCls = ds as IFeatureClass;
                                    break;
                                }
                            }
                        }

                    }


                }
                // dataset = datasets.Next();

            }


            catch (Exception ex)
            {
                WriteLine(DateTime.Now.ToLongTimeString() + " Error in getting feature class : " + featureClassName + " , " + MethodBase.GetCurrentMethod() + " : " + ex.ToString());
            }
            finally
            {
                //Marshal.ReleaseComObject(pWorkspace);
            }
            return featureCls;

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
    }
}
