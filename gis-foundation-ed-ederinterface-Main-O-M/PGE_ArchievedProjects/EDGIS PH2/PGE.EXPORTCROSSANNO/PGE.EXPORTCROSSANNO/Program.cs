using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using System.Collections.Generic;
using System.Data.OracleClient;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using System.Collections;
using System.Configuration;
using System.Collections.Generic;
using ESRI.ArcGIS.Geometry;
using DeleteFeatures;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using Microsoft.VisualBasic;

namespace PGE.EXPORTCROSSANNO
{
    class Program
    {
        private static LicenseInitializer m_AOLicenseInitializer = new DeleteFeatures.LicenseInitializer();
        private static string sPath = (String.IsNullOrEmpty(ConfigurationManager.AppSettings["LOGPATH"]) ? System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) : ConfigurationManager.AppSettings["LOGPATH"]) + "\\Logfile_" + DateTime.Now.Ticks.ToString() + ".log";
        private static StreamWriter pSWriter = default(StreamWriter);
        //private static IMMAppInitialize arcFMAppInitialize = new MMAppInitializeClass();
        static void Main(string[] args)
        {
            try
            {
                (pSWriter = File.CreateText(sPath)).Close();
                List<string> issueList = new List<string>();

                string TargetFGBDPath = args[0].ToString();
                string ExportType = args[1].ToString();
                string extent = args[2].ToString();
                string FacilityID = args[3].ToString();
                string UUID = args[4].ToString();

                WriteLine(TargetFGBDPath);
                WriteLine(DateTime.Now.ToLongTimeString() + " -- Initializing Licence");
                //m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeBasic, esriLicenseProductCode.esriLicenseProductCodeStandard, esriLicenseProductCode.esriLicenseProductCodeAdvanced },
                //new esriLicenseExtensionCode[] { });
                //mmLicenseStatus licenseStatus = CheckOutLicenses(mmLicensedProductCode.mmLPArcFM);
                if (RuntimeManager.ActiveRuntime == null)
                    RuntimeManager.BindLicense(ProductCode.Server);
                WriteLine(DateTime.Now.ToLongTimeString() + " -- Initializing Licence is completed");
                //Process feature class wise     
                //string TEMPLATE_FGDB_NAME = 

                WriteLine(TargetFGBDPath);                

                CopyFGDBtoTarget(TargetFGBDPath, UUID);

                WriteLine(TargetFGBDPath);
                WriteLine(DateTime.Now.ToLongTimeString() + " -- Creating Connection " + ConfigurationManager.AppSettings["CONN_EDGIS_FILE"] + " to provided Version " + ConfigurationManager.AppSettings["SESSION_NAME"]);
                IWorkspace objEDGISFW = ArcSdeWorkspaceFromFile(ConfigurationManager.AppSettings["CONN_EDGIS_FILE"]);
                //pVersion = ((IVersionedWorkspace)objEDGISFW).FindVersion(ConfigurationManager.AppSettings["SESSION_NAME"]);
                string pFCName = string.Empty;

                TargetFGBDPath = TargetFGBDPath + "\\" + UUID +"\\" +ConfigurationSettings.AppSettings["TEMPLATE_FGDB_NAME"].ToString();
                IWorkspace objFGDB = FGDBWorkspaceFromFile(TargetFGBDPath);

                //pFGDBWorkSpace = objFGDB as IFeatureWorkspace;
                bool status1=false,status2=false;
                if (ExportType.Contains("DUCTVIEW"))
                    status1 = ExportAnnoFeatures(TargetFGBDPath, extent, "DUCTVIEW", FacilityID, objFGDB, pFCName, objEDGISFW);
                if (ExportType.Contains("BUTTERFLY") && FacilityID !="-1")
                    status2 = ExportAnnoFeatures(TargetFGBDPath, extent, "BUTTERFLY", FacilityID, objFGDB, pFCName, objEDGISFW);
                
                //CreateStatusFile(TargetFGBDPath,status1 || status2);

            }
            catch (Exception ex)
            {
                
                WriteLine(ex.Message+ex.StackTrace+ex.InnerException);
            }
            
        }

        private static void CreateStatusFile(string TargetFGBDPath, bool status)
        {
            try
            {
                StreamWriter pSWriterLog = default(StreamWriter);
                string sPath=string.Empty;
                if(status)
                    sPath = TargetFGBDPath + "\\COMPLETE_CROSS_ANNO_EXTRACTION" + ".log";
                else
                    sPath = TargetFGBDPath + "\\FAIL_CROSS_ANNO_EXTRACTION" + ".log";
                
                pSWriterLog = File.CreateText(sPath);
                pSWriterLog.WriteLine("CROSS SECTION EXPORT PROCESS COMPLETED SUCCESSFULLY ");                                
                pSWriterLog.Close();                                
            }
            catch (Exception)
            {
                WriteLine("Error in copying fileGDB to target folder " + TargetFGBDPath);        
            }
        }

        private static void WriteLine(string sMsg)
        {
            pSWriter = File.AppendText(sPath);
            pSWriter.WriteLine(sMsg);
            //DrawProgressBar();
            Console.WriteLine(sMsg);
            pSWriter.Close();
        }
        //private static mmLicenseStatus CheckOutLicenses(mmLicensedProductCode productCode)
        //{
        //    mmLicenseStatus licenseStatus;
        //    licenseStatus = arcFMAppInitialize.IsProductCodeAvailable(productCode);
        //    if (licenseStatus == mmLicenseStatus.mmLicenseAvailable)
        //    {
        //        licenseStatus = arcFMAppInitialize.Initialize(productCode);
        //    }
        //    return licenseStatus;
        //}
        
        //private static void DeleteFeatures()
        //{
        //    IVersion pVersion = default(IVersion);
        //    IFeature pFeat = default(IFeature);
        //    IFeatureCursor pFCursor = default(IFeatureCursor);
        //    IQueryFilter pQFilter = default(IQueryFilter);
        //    IFeatureClass pFClass = default(IFeatureClass);
        //    ITable pTable = default(ITable);
        //    //IFeature pFeat = default(IFeature);
        //    IFeatureCursor pCursor = default(IFeatureCursor);
            
        //    //List<string> lstTargetFCNames = new System.Collections.Generic.List<string>();
        //    //IMMAutoUpdater autoupdater = default(IMMAutoUpdater);
        //    //mmAutoUpdaterMode oldMode;
        //    string strLastUser = string.Empty, strVersionName = string.Empty;

        //    try
        //    {
        //        WriteLine(DateTime.Now.ToLongTimeString() + " -- Creating Connection " + ConfigurationManager.AppSettings["CONN_EDGIS_FILE"] + " to provided Version " + ConfigurationManager.AppSettings["SESSION_NAME"]);
        //        IWorkspace objEDGISFW = ArcSdeWorkspaceFromFile(ConfigurationManager.AppSettings["CONN_EDGIS_FILE"]);
        //        pVersion = ((IVersionedWorkspace)objEDGISFW).FindVersion(ConfigurationManager.AppSettings["SESSION_NAME"]);
        //        string pFCName = ConfigurationManager.AppSettings["DELETE_FEATURE_CLASS"];             
        //        pFClass = (pVersion as IFeatureWorkspace).OpenFeatureClass(pFCName);
        //        //started processing FC's
        //        WriteLine(DateTime.Now.ToLongTimeString() + " Started processing " + pFCName);
        //        //To Write change records to GIS-SAP Staging table....
                
        //        string strGUIDValue = string.Empty;
        //        ((IWorkspaceEdit)pVersion).StartEditing(true);
        //        ((IWorkspaceEdit)pVersion).StartEditOperation();
        //        pCursor = pFClass.Update(null, false);
        //        int iCount = 0;
        //        if (DisableAutoUpdaterFramework(out autoupdater, out oldMode))
        //        {
        //            while ((pFeat = pCursor.NextFeature()) != null)
        //            {

        //                WriteLine(DateTime.Now.ToLongTimeString() + " Processing GUID: " + pFeat.OID);
        //                //pCursor.DeleteFeature();
        //                pFeat.Delete();
        //                iCount++;
        //                //((IWorkspaceEdit)pVersion).
        //                if (iCount % 5000 == 0)
        //                {
        //                    ((IWorkspaceEdit)pVersion).StopEditOperation();
        //                    ((IWorkspaceEdit)pVersion).StopEditing(true);
        //                    ((IWorkspaceEdit)pVersion).StartEditing(true);
        //                    ((IWorkspaceEdit)pVersion).StartEditOperation();
        //                    pCursor = pFClass.Update(null, false);
        //                }
        //            }
        //            ((IWorkspaceEdit)pVersion).StopEditOperation();
        //            autoupdater.AutoUpdaterMode = oldMode;
        //            ((IWorkspaceEdit)pVersion).StopEditing(true);
        //            WriteLine(DateTime.Now.ToLongTimeString() + " Total " + pFCName + " features processed : " + iCount);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLine(DateTime.Now.ToLongTimeString() + " Error while Processing features " + ex.Message);

        //    }
        //    finally
        //    {
        //        if (pVersion != null) Marshal.ReleaseComObject(pVersion);
        //        if (pFeat != null) Marshal.ReleaseComObject(pFeat);
        //        if (pFCursor != null) Marshal.ReleaseComObject(pFCursor);
        //        if (pQFilter != null) Marshal.ReleaseComObject(pQFilter);
        //        if (pFClass != null) Marshal.ReleaseComObject(pFClass);
        //        if (pTable != null) Marshal.ReleaseComObject(pTable);
        //        //if (pRow != null) Marshal.ReleaseComObject(pRow);
        //        if (pCursor != null) Marshal.ReleaseComObject(pCursor);
        //        if (autoupdater != null) Marshal.ReleaseComObject(autoupdater);
        //        //if (oldMode != null) Marshal.ReleaseComObject(oldMode);
        //    }

        //}

        private static bool ExportAnnoFeatures(string TargetFGBDPath, string extent, string ExportType, string FacilityID, IWorkspace objFGDB, string pFCName, IWorkspace pGDBWorkspace)
        {
            IVersion pVersion = default(IVersion);
            IFeature pFeat = default(IFeature);
            IFeatureCursor pFCursor = default(IFeatureCursor);
            IQueryFilter pQFilter = default(IQueryFilter);
            IFeatureClass pFClass = default(IFeatureClass);
            ITable pTable = default(ITable);           
            IFeatureCursor pCursor = default(IFeatureCursor);                        
            IFeatureClass pGDBClass = default(IFeatureClass);
            IFeatureClass pFGDBClass = default(IFeatureClass);
            IFeatureClass pFGDBPolyClass = default(IFeatureClass);
            IFeatureWorkspace pFGDBWorkSpace = default(IFeatureWorkspace);
            string strLastUser = string.Empty, strVersionName = string.Empty;
            IFeature pGDBFeature = default(IFeature);
            IAnnotationFeature pAnnoFeature = default(IAnnotationFeature);
            IFeatureCursor pFeatCursor = default(IFeatureCursor);
            IElement pEle = default(IElement);
            IGroupElement GPEle = default(IGroupElement);
            IEnumElement pEnum = default(IEnumElement);
            IElement pElement = default(IElement);
            IFeature pFGDBFeature = default(IFeature);
            try
            {
                

                if (ExportType.Contains("DUCTVIEW"))
                {
                    pFGDBClass = (objFGDB as IFeatureWorkspace).OpenFeatureClass(ConfigurationManager.AppSettings["CROSS_50ANNO_FGDB_CLASS"]);
                    pFGDBPolyClass = (objFGDB as IFeatureWorkspace).OpenFeatureClass(ConfigurationManager.AppSettings["CROSS_50POLY_FGDB_CLASS"]);
                    pFCName = ConfigurationManager.AppSettings["CROSS_50ANNO_CLASS"];
                }
                if (ExportType.Contains("BUTTERFLY"))
                {
                    pFGDBClass = (objFGDB as IFeatureWorkspace).OpenFeatureClass(ConfigurationManager.AppSettings["CROSS_10ANNO_FGDB_CLASS"]);
                    pFGDBPolyClass = (objFGDB as IFeatureWorkspace).OpenFeatureClass(ConfigurationManager.AppSettings["CROSS_10POLY_FGDB_CLASS"]);
                    pFCName = ConfigurationManager.AppSettings["CROSS_10ANNO_CLASS"];
                }

                pGDBClass = (pGDBWorkspace as IFeatureWorkspace).OpenFeatureClass(pFCName);
                WriteLine(DateTime.Now.ToLongTimeString() + " Started processing " + pFCName);
                pFeatCursor = QueryFeatures(ExportType, extent, pGDBClass, FacilityID, (pGDBClass as IGeoDataset).SpatialReference);

                (objFGDB as IWorkspaceEdit).StartEditing(true);
                
                pGDBFeature = pFeatCursor.NextFeature();
                while (pGDBFeature!=null)
                {
                    pAnnoFeature = pGDBFeature as IAnnotationFeature;
                    pEle = pAnnoFeature.Annotation;
                    if (pEle is IGroupElement)
                    {
                        GPEle = pEle as IGroupElement;
                        pEnum = GPEle.Elements;
                        pElement = pEnum.Next();


                        while (pElement != null)
                        {
                            if (pElement is ITextElement)
                            {
                                if ((pElement as ITextElement).Text != string.Empty)
                                {
                                    pFGDBFeature = pFGDBClass.CreateFeature();                                    

                                    (pFGDBFeature as IAnnotationFeature).Annotation = pElement;

                                    //((pFGDBFeature as IAnnotationFeature).Annotation as ITextElement).Text = (pElement as ITextElement).Text.Replace("\n", "^").Replace("\r", "");
                                    pFGDBFeature.Store();

                                }
                            }
                            else if (pElement is IPolygonElement)
                            {
                                pFGDBFeature = pFGDBPolyClass.CreateFeature();
                                IGeometry pGeo = pElement.Geometry;
                                pFGDBFeature.Shape = pGeo;
                                if ((pElement as IFillShapeElement).Symbol.Color.CMYK != 0)
                                    pFGDBFeature.set_Value(pFGDBFeature.Fields.FindField("FILLED"), "1");
                                else if ((pElement as IFillShapeElement).Symbol.Color.CMYK == 0)
                                    pFGDBFeature.set_Value(pFGDBFeature.Fields.FindField("FILLED"), "0");
                                pFGDBFeature.Store();
                            }

                            else if (pElement is IGroupElement)
                            {
                                IGroupElement pIngGroup = pElement as IGroupElement;
                                IEnumElement pInEnum = pIngGroup.Elements;
                                IElement pInelement = pInEnum.Next();
                                while (pInelement != null)
                                {
                                    if (pInelement is ITextElement)
                                    {
                                        if ((pInelement as ITextElement).Text != string.Empty)
                                        {
                                            pFGDBFeature = pFGDBClass.CreateFeature();
                                            (pFGDBFeature as IAnnotationFeature).Annotation = pInelement;
                                            pFGDBFeature.Store();

                                        }
                                    }
                                    else if (pInelement is IPolygonElement)
                                    {
                                        pFGDBFeature = pFGDBPolyClass.CreateFeature();
                                        IGeometry pGeo = pInelement.Geometry;
                                        pFGDBFeature.Shape = pGeo;
                                        if ((pInelement as IFillShapeElement).Symbol.Color.CMYK != 0)
                                            pFGDBFeature.set_Value(pFGDBFeature.Fields.FindField("FILLED"), "1");
                                        else if ((pInelement as IFillShapeElement).Symbol.Color.CMYK == 0)
                                            pFGDBFeature.set_Value(pFGDBFeature.Fields.FindField("FILLED"), "0");
                                        pFGDBFeature.Store();
                                    }
                                    pInelement = pInEnum.Next();
                                }
                            }

                            pElement = pEnum.Next();

                        }
                    }
                    else if (pElement is ITextElement)
                    {
                        if ((pElement as ITextElement).Text != string.Empty)
                        {
                            pFGDBFeature = pFGDBClass.CreateFeature();
                            (pFGDBFeature as IAnnotationFeature).Annotation = pElement;
                            pFGDBFeature.Store();

                        }
                    }
                    pGDBFeature = pFeatCursor.NextFeature();
                    
                }
                

                (objFGDB as IWorkspaceEdit).StopEditing(true);
                WriteLine("Completed");
                return true;
                
            }
            catch (Exception ex)
            {
                WriteLine(DateTime.Now.ToLongTimeString() + " Error while Processing features " + ex.Message);
                return false;

            }
            finally
            {
                if (pVersion != null) Marshal.ReleaseComObject(pVersion);
                if (pFeat != null) Marshal.ReleaseComObject(pFeat);
                if (pFCursor != null) Marshal.ReleaseComObject(pFCursor);
                if (pQFilter != null) Marshal.ReleaseComObject(pQFilter);
                if (pFClass != null) Marshal.ReleaseComObject(pFClass);
                if (pTable != null) Marshal.ReleaseComObject(pTable);
                if (pCursor != null) Marshal.ReleaseComObject(pCursor);
                if (pGDBClass != null) Marshal.ReleaseComObject(pGDBClass);
                if (pFGDBClass != null) Marshal.ReleaseComObject(pFGDBClass);
                if (pFGDBPolyClass != null) Marshal.ReleaseComObject(pFGDBPolyClass);
                if (pFGDBWorkSpace != null) Marshal.ReleaseComObject(pFGDBWorkSpace);
                if (pGDBFeature != null) Marshal.ReleaseComObject(pGDBFeature);
                if (pAnnoFeature != null) Marshal.ReleaseComObject(pAnnoFeature);
                if (pFeatCursor != null) Marshal.ReleaseComObject(pFeatCursor);
                if (pEle != null) Marshal.ReleaseComObject(pEle);
                if (GPEle != null) Marshal.ReleaseComObject(GPEle);
                if (pEnum != null) Marshal.ReleaseComObject(pEnum);
                if (pElement != null) Marshal.ReleaseComObject(pElement);
                if (pFGDBFeature != null) Marshal.ReleaseComObject(pFGDBFeature);         
            }

        }

        private static IWorkspace ArcSdeWorkspaceFromFile(String connectionFile)
        {
            return ((IWorkspaceFactory)Activator.CreateInstance(Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory"))).
                OpenFromFile(connectionFile, 0);
        }

        private static IWorkspace FGDBWorkspaceFromFile(String connectionFile)
        {
            return ((IWorkspaceFactory)Activator.CreateInstance(Type.GetTypeFromProgID("esriDataSourcesGDB.FileGDBWorkspaceFactory"))).OpenFromFile(connectionFile, 0);
        }

        //private static bool DisableAutoUpdaterFramework(out IMMAutoUpdater autoupdater, out mmAutoUpdaterMode oldMode)
        //{
        //    string strDisableAU = ConfigurationManager.AppSettings["DISABLE_AU_FRAMEWORK"].ToString();

        //    try
        //    {
        //        Type type = Type.GetTypeFromProgID("mmGeodatabase.MMAutoUpdater");
        //        object obj = Activator.CreateInstance(type);
        //        autoupdater = obj as IMMAutoUpdater;
        //        oldMode = autoupdater.AutoUpdaterMode;
        //        //disable all ArcFM Autoupdaters 
        //        if (Convert.ToBoolean(strDisableAU))
        //        {

        //            autoupdater.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;
        //            return true;
        //        }
        //        else
        //        {

        //            return true;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        //WriteLine(DateTime.Now + "");
        //        //throw;
        //        WriteLine(DateTime.Now.ToLongTimeString() + " Error in disabling Auto Updaters. ");
        //    }
        //    autoupdater = null;
        //    oldMode = mmAutoUpdaterMode.mmAUMStandAlone;
        //    return false;
        //}

        private static void CopyFGDBtoTarget(string TargetFGBDPath, string UUID)
        {
            try
            {
                string SourcePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\" + ConfigurationSettings.AppSettings["TEMPLATE_FGDB_NAME"].ToString();

                if (Directory.Exists(SourcePath) && Directory.Exists(TargetFGBDPath))
                {
                    DirectoryInfo SourceDir = new DirectoryInfo(SourcePath);
                    DirectoryInfo DestDir = new DirectoryInfo(TargetFGBDPath);
                    //Directory.CreateDirectory(UUID);
                    DirectoryInfo UUIDFolder = DestDir.CreateSubdirectory(UUID);
                    DirectoryInfo FGDBFolder = UUIDFolder.CreateSubdirectory(ConfigurationSettings.AppSettings["TEMPLATE_FGDB_NAME"].ToString());

                    //File.Copy(SourcePath, "@" + TargetFGBDPath + "\\" + ConfigurationSettings.AppSettings["TEMPLATE_FGDB_NAME"].ToString(), true);
                    FileInfo[] files = SourceDir.GetFiles();
                    foreach (FileInfo file in files)
                    {
                        string temppath = System.IO.Path.Combine(FGDBFolder.FullName, file.Name);
                        //temppath = System.IO.Path.Combine(temppath, UUID);
                        WriteLine(temppath);
                        file.CopyTo(temppath, true);

                    }
                }

                else
                {
                    WriteLine("Source path or Destination path is  not accessible SourcePath : " + SourcePath + " DestinationPath " + TargetFGBDPath);
                    throw (new Exception());
                }
            }
            catch (Exception ex)
            {

                WriteLine("Source path or Destination path is  not accessible "+ex.Message + ex.StackTrace+ex.InnerException);
                //throw ex;
            }

        }
        private static IFeatureCursor QueryFeatures(string ExportType, string strExtent, IFeatureClass pCrossAnnoClass,string strFacilityID,ISpatialReference pSpatialReference)
        {
            IGeometry pSourceGeo = default(IGeometry);
            //1814745 13722979 1814745 13723123 1814938 13723123 1814938 13722979 1814745 13722979 
            ISpatialFilter pSpatialFilter = default(ISpatialFilter);
            IQueryFilter pQueryFilter = default(IQueryFilter);
            IFeatureCursor pFeatureCursor = default(IFeatureCursor);
            try
            {
                

                if (ExportType == "BUTTERFLY")
                {
                    pQueryFilter = new QueryFilterClass();
                    pQueryFilter.WhereClause = "FACILITYID='" + strFacilityID + "'";
                    pFeatureCursor = pCrossAnnoClass.Search(pQueryFilter, false);
                }
                else if (ExportType == "DUCTVIEW")
                {
                    pSpatialFilter = new SpatialFilterClass();
                    pSpatialFilter.Geometry = ConstructGeometry(strExtent, pSpatialReference);
                    pSpatialFilter.GeometryField = pCrossAnnoClass.ShapeFieldName;
                    pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;
                    pFeatureCursor = pCrossAnnoClass.Search(pSpatialFilter, false);
                }
                return pFeatureCursor;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if(pSpatialFilter!=null) Marshal.ReleaseComObject(pSpatialFilter);
                if (pQueryFilter != null) Marshal.ReleaseComObject(pQueryFilter);
            }


        }

        private static IGeometry ConstructGeometry(string strExtent,ISpatialReference pSpatialRef)
        {
             IPointCollection4 pPointCol = new PolygonClass();
             IPoint pPoint = default(IPoint);
             IPolygon pPolygon = default(IPolygon);
            try
            {

                string[] Coordinates = strExtent.Split(' ');


                for (int i = 0; i < Coordinates.Length; i = i + 2)
                {
                    pPoint = new PointClass();
                    pPoint.SpatialReference = pSpatialRef;
                    pPoint.X = Convert.ToDouble(Coordinates[i]);
                    pPoint.Y = Convert.ToDouble(Coordinates[i + 1]);
                    pPointCol.AddPoint(pPoint, Type.Missing, Type.Missing);

                    pPoint = null;

                }
                //pPolygon = new PolygonClass();

                IGeometry pGeo = (IPolygon)pPointCol;
                pGeo.SpatialReference = pSpatialRef;
                //pPolygon = pPointCol as IPolygon;
                //pPolygon.SpatialReference = pSpatialRef;
                return pGeo as IGeometry;             



            }
            catch (Exception ex)
            {
                WriteLine("Error in constructing geometry " + ex.Message);
                throw ex;
            }
            finally
            {
               // Marshal.ReleaseComObject(pPointCol);
                if (pPoint!=null) Marshal.ReleaseComObject(pPoint);
                //Marshal.ReleaseComObject(pPolygon);
            }

        }


    }


}

