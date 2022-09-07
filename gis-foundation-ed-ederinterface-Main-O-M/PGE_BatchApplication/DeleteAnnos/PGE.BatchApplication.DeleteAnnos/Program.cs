using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Diagnostics;
using System.Threading;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using System.Configuration;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.Carto;
using Miner.Interop;


namespace PGE.BatchApplication.DeleteAnnos
{
    class Program
    {
        private static esriLicenseProductCode[] _prodCodes = { esriLicenseProductCode.esriLicenseProductCodeAdvanced };
        private static LicenseInitializer _licenceManager = new LicenseInitializer();
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "PGE.BatchApplication.DeleteAnnos.log4net.config");
        private static List<Process> _childProcessList;
        private static List<string> _50AnnoClassNames = new List<string>();
        private static List<string> _100AnnoClassNames = new List<string>();
        private static int _totalAnnoClassCount;

        static void Main(string[] args)
        {
            Console.WriteLine("Checking licenses...");
            if (CheckLicense(_licenceManager))
            {
                try
                {
                    Console.WriteLine("Opening workspace...");
                    string connectionFileName = PGE_DBPasswordManagement.ReadEncryption.GetSDEPath(ConfigurationManager.AppSettings["SDEConnectionFile"]);
                    Console.WriteLine("Conn file name: " + connectionFileName);
                    IWorkspace workSpace = Common.OpenWorkspace();
                    string boundaryFeatureClassName = ConfigurationManager.AppSettings["BoundaryFeatureClass"];

                    _childProcessList = new List<Process>();
                    Console.WriteLine("Deleting annos...");
                    _logger.Info("Start time: " + DateTime.Now.ToLocalTime());
                    Console.WriteLine("Start time: " + DateTime.Now.ToLocalTime());

                    IFeatureClass annoClass;
                    string modelName = "";
                    string versionName = "";

                    //50Sclae Anno
                    if (args[0].Equals("50", StringComparison.CurrentCultureIgnoreCase))
                    {
                        versionName = string.Format("{0}_{1}", ConfigurationManager.AppSettings["VersionName50Scale"], DateTime.Now.ToString());
                        modelName = ConfigurationManager.AppSettings["50ScaleAnnoModelName"];
                        IEnumFeatureClass annos_50Scale = Common.FeatureClassesByModelName(workSpace, modelName);
                        annos_50Scale.Reset();

                        while ((annoClass = annos_50Scale.Next()) != null)
                        {
                            IDataset dataset = annoClass as IDataset;
                            string name = (dataset.FullName as IDatasetName).Name;

                            _50AnnoClassNames.Add(name);



                        }
                        _totalAnnoClassCount = _50AnnoClassNames.Count;

                    }

                    if (args[0].Equals("100", StringComparison.CurrentCultureIgnoreCase))
                    {
                        versionName = string.Format("{0}_{1}", ConfigurationManager.AppSettings["VersionName100Scale"], DateTime.Now.ToString());
                        //100 Scale Anno
                        modelName = ConfigurationManager.AppSettings["100ScaleAnnoModelName"];
                        IEnumFeatureClass annos_100Scale = Common.FeatureClassesByModelName(workSpace, modelName);
                        annos_100Scale.Reset();

                        while ((annoClass = annos_100Scale.Next()) != null)
                        {
                            IDataset dataset = annoClass as IDataset;
                            string name = (dataset.FullName as IDatasetName).Name;
                            _100AnnoClassNames.Add(name);
                        }
                        _totalAnnoClassCount = _100AnnoClassNames.Count;

                    }






                    bool annosDeleted;




                    Console.WriteLine(string.Format("Creating version: {0}", versionName));
                    _logger.Info(string.Format("Creating version: {0}", versionName));
                    IVersion editVersion = ((IVersion2)workSpace).CreateChild(versionName, (IVersion2)workSpace);
                    IWorkspaceEdit workspaceEdit = (IWorkspaceEdit2)editVersion;
                    IVersionEdit4 versionEdit = (IVersionEdit4)workspaceEdit;
                    IMultiuserWorkspaceEdit muWorkspaceEdit = (IMultiuserWorkspaceEdit)editVersion;
                    try
                    {
                        if (muWorkspaceEdit.SupportsMultiuserEditSessionMode(esriMultiuserEditSessionMode.esriMESMVersioned))
                        {

                            muWorkspaceEdit.StartMultiuserEditing(esriMultiuserEditSessionMode.esriMESMVersioned);
                            workspaceEdit.StartEditOperation();

                            //Disable AUs
                            _logger.Debug("Disabling AUs...");
                            Type type = Type.GetTypeFromProgID("mmGeodatabase.MMAutoUpdater");
                            object obj = Activator.CreateInstance(type);
                            IMMAutoUpdater pAutoupdater = obj as IMMAutoUpdater;
                            pAutoupdater.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;

                            //50 scale
                            if (args[0].Equals("50", StringComparison.CurrentCultureIgnoreCase))
                            {

                                Console.WriteLine("Deleting 50 scale annos that lie outside the 50 scale boundaries");
                                _logger.Info("Deleting 50 scale annos that lie outside the 50 scale boundaries");

                                foreach (string fName in _50AnnoClassNames)
                                {
                                    _logger.Info(string.Format("Deleting 50 scale {0} annos...", fName));
                                    Console.WriteLine(string.Format("Deleting 50 scale {0} annos...", fName));
                                    try
                                    {
                                        annosDeleted = Delete50Annos(editVersion, boundaryFeatureClassName, fName);
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.Info(string.Format("Error in Deleting 50 scale {0} annos...", fName));
                                    }


                                }
                            }

                            //100 scale
                            if (args[0].Equals("100", StringComparison.CurrentCultureIgnoreCase))
                            {

                                Console.WriteLine();
                                Console.WriteLine("Deleting 100 scale annos that lie inside the 50 scale boundaries");
                                _logger.Info("Deleting 100 scale annos that lie inside the 50 scale boundaries");

                                foreach (string fName in _100AnnoClassNames)
                                {
                                    _logger.Info(string.Format("Deleting 100 scale {0} annos...", fName));
                                    Console.WriteLine(string.Format("Deleting 100 scale {0} annos...", fName));
                                    try
                                    {
                                        annosDeleted = Delete100Annos(editVersion, boundaryFeatureClassName, fName);

                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.Info(string.Format("Error in Deleting 100 scale {0} annos...", fName));
                                    }



                                }


                            }

                            workspaceEdit.StopEditOperation();

                            //reconcile
                            bool conflictsDetected = versionEdit.Reconcile4(editVersion.VersionInfo.Parent.VersionName, true, false, false, false);

                            workspaceEdit.StartEditOperation();
                            //no conflicts detected so post can be performed        
                            if (!conflictsDetected && versionEdit.CanPost())
                            {
                                Console.WriteLine(string.Format("Posting version: {0} to SDE...", editVersion.VersionInfo.VersionName));
                                _logger.Info(string.Format("Posting version: {0} to SDE...", editVersion.VersionInfo.VersionName));
                                versionEdit.Post(editVersion.VersionInfo.Parent.VersionName);
                                Console.WriteLine(string.Format("version: {0} has been successfully posted...", editVersion.VersionInfo.VersionName));
                                _logger.Info(string.Format("version:{0} has been successfully posted...", editVersion.VersionInfo.VersionName));

                            }




                            workspaceEdit.StopEditOperation();
                            workspaceEdit.StopEditing(true);

                            //Delete the  version ...
                            try
                            {
                                IVersion delVersion = ((IVersionedWorkspace)workSpace).FindVersion(versionName);
                                if (delVersion != null)
                                    Console.WriteLine(string.Format("Deleting  version: {0} ", versionName));
                                _logger.Info(string.Format("Deleting  version: {0} ...", versionName));
                                delVersion.Delete();
                                _logger.Info(string.Format(" version: {0} successfully deleted", versionName));
                                Console.WriteLine(string.Format("version:{ 0}  successfully deleted", versionName));
                            }
                            catch
                            {

                            }

                        }
                        _licenceManager.ReleaseArcFMLicense();
                        _licenceManager.ShutdownApplication();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("Error: " + ex.Message);
                        workspaceEdit.AbortEditOperation();
                        //throw the exception so that outer catch block logs the error.
                       // throw;
                    }

                    Console.WriteLine("Done!");
                    _logger.Info("Done!");
                    _logger.Info("End time: " + DateTime.Now.ToLocalTime());
                    Console.WriteLine("End time: " + DateTime.Now.ToLocalTime());

                    Environment.Exit(0);
                }
                catch (Exception ex)
                {
                    _logger.Error("Error: " + ex.Message); 
                    Environment.Exit(1);
                }
            }
        }

        /// <summary>
        /// Initializes ArcGIS license
        /// </summary>
        /// <param name="licenceManager"></param>
        /// <returns></returns>
        private static bool CheckLicense(LicenseInitializer licenceManager)
        {
            bool isOk = false;

            try
            {
                isOk = licenceManager.InitializeApplication(_prodCodes);
                isOk = licenceManager.GetArcFMLicense(Miner.Interop.mmLicensedProductCode.mmLPArcFM);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to initialize license : " + ex.Message);
                _logger.Error("Failed to initialize license : " + ex.Message);
            }

            return isOk;
        }



        private static bool Delete50Annos(IVersion version, string boundaryFeatureClassName, string featureClassName)
        {
            IFeatureWorkspace fWorkspace = version as IFeatureWorkspace;
            object missingType = Type.Missing;
            IGeometryBag geometryBag = new GeometryBagClass();
            IGeometryCollection geometryCollection = geometryBag as IGeometryCollection;
            ISpatialFilter spatialFilter = new SpatialFilterClass();
            spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects; //esriSpatialRelRelation; //
            //spatialFilter.SpatialRelDescription = "FF*FF****";  //The geometries must be completely disjointed.

            IFeatureClass boundaryFC = fWorkspace.OpenFeatureClass(boundaryFeatureClassName);

            using (ComReleaser comReleaser = new ComReleaser())
            {
                IFeatureCursor fCursor = boundaryFC.Search(null, false);
                comReleaser.ManageLifetime(fCursor);

                List<IFeature> boundaryList = new List<IFeature>();
                IFeature boundaryFeature;
                while ((boundaryFeature = fCursor.NextFeature()) != null)
                {
                    geometryCollection.AddGeometry(boundaryFeature.Shape, ref missingType, missingType);

                    boundaryList.Add(boundaryFeature);

                }

                IGeoDataset geoDataset = (IGeoDataset)boundaryFC;
                ISpatialReference spatialReference = geoDataset.SpatialReference;
                geometryBag.SpatialReference = spatialReference;

                // Cast the geometry bag to the ISpatialIndex interface and call the Invalidate method
                // to generate a new spatial index.
                ISpatialIndex spatialIndex = (ISpatialIndex)geometryBag;
                spatialIndex.AllowIndexing = true;
                spatialIndex.Invalidate();

                spatialFilter.Geometry = geometryBag;

                IFeatureClass annoClass = (version as IFeatureWorkspace).OpenFeatureClass(featureClassName);

                List<IFeature> annosToBeDeleted = new List<IFeature>();
                HashSet<int> annoOIDsInside50ScalePolygons = new HashSet<int>();

                IFeatureCursor cursor = null;

                try
                {
                    cursor = annoClass.Search(spatialFilter, true);
                    IFeature annoFeature;
                    while ((annoFeature = cursor.NextFeature()) != null)
                    {
                        annoOIDsInside50ScalePolygons.Add(annoFeature.OID);
                    }
                }
                finally
                {
                    if (cursor != null) { while (Marshal.ReleaseComObject(cursor) > 0) ; }
                    cursor = null;
                }

                int deleteCount = 0;
                int totalCount = 0;
                try
                {
                    cursor = annoClass.Search(null, false);
                    IFeature annoFeature;


                    while ((annoFeature = cursor.NextFeature()) != null)
                    {
                        totalCount++;
                        if (!annoOIDsInside50ScalePolygons.Contains(annoFeature.OID))
                        {
                            try
                            {
                                IFeature feat = annoFeature;
                                feat.Delete();
                                deleteCount++;
                            }
                            catch
                            {
                                throw;
                            }
                        }
                    }
                }
                finally
                {
                    if (cursor != null) { while (Marshal.ReleaseComObject(cursor) > 0) ; }
                    cursor = null;
                }

                _logger.Info(string.Format("{0} features of {1} deleted.", deleteCount, annoClass.AliasName));
                Console.WriteLine(string.Format("{0} features of {1} deleted.", deleteCount, annoClass.AliasName));

                //set and exclusive lock on the class
                //ISchemaLock schemaLock = (ISchemaLock)anno;
                //schemaLock.ChangeSchemaLock(esriSchemaLock.esriExclusiveSchemaLock);
            }

            return true;
        }

        private static bool Delete100Annos(IVersion version, string boundaryFeatureClassName, string featureClassName)
        {
            IFeatureWorkspace fWorkspace = version as IFeatureWorkspace;
            object missingType = Type.Missing;
            IGeometryBag geometryBag = new GeometryBagClass();
            IGeometryCollection geometryCollection = geometryBag as IGeometryCollection;
            ISpatialFilter spatialFilter = new SpatialFilterClass();
            spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;

            IFeatureClass boundaryFC = fWorkspace.OpenFeatureClass(boundaryFeatureClassName);

            using (ComReleaser comReleaser = new ComReleaser())
            {
                IFeatureCursor fCursor = boundaryFC.Search(null, false);
                comReleaser.ManageLifetime(fCursor);

                IFeature boundaryFeature;
                while ((boundaryFeature = fCursor.NextFeature()) != null)
                {
                    geometryCollection.AddGeometry(boundaryFeature.Shape, ref missingType, missingType);
                }

                IGeoDataset geoDataset = (IGeoDataset)boundaryFC;
                ISpatialReference spatialReference = geoDataset.SpatialReference;
                geometryBag.SpatialReference = spatialReference;

                // Cast the geometry bag to the ISpatialIndex interface and call the Invalidate method
                // to generate a new spatial index.
                ISpatialIndex spatialIndex = (ISpatialIndex)geometryBag;
                spatialIndex.AllowIndexing = true;
                spatialIndex.Invalidate();

                spatialFilter.Geometry = geometryBag;

                IFeatureClass annoClass = (version as IFeatureWorkspace).OpenFeatureClass(featureClassName);

                int deleteCount = 0;
                List<IFeature> annosToBeDeleted = new List<IFeature>();

                IFeatureCursor cursor = null;
                try
                {
                    cursor = annoClass.Search(spatialFilter, false);
                    IFeature annoFeature;

                    while ((annoFeature = cursor.NextFeature()) != null)
                    {
                        IFeature feat = annoFeature;
                        feat.Delete();
                        deleteCount++;
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    if (cursor != null) { while (Marshal.ReleaseComObject(cursor) > 0) ; }
                    cursor = null;
                }

                _logger.Info(string.Format("{0} features of {1} deleted.", deleteCount, featureClassName));
                Console.WriteLine(string.Format("{0} features of {1} deleted.", deleteCount, featureClassName));
            }

            return true;
        }

    }
}
