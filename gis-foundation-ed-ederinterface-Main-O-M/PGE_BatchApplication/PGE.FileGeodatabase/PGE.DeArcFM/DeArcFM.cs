using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using System.Configuration;
using System.IO;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.GeoDatabaseDistributed;
//using ESRI.ArcGIS.Geoprocessor;
//using ESRI.ArcGIS.Geoprocessing;
//using ESRI.ArcGIS.DataManagementTools;
using Miner.Interop;
//using Ionic.Zip;

namespace PGE.DeArcFM
{
    class DeArcFM
    {

        private static string[] BadCLSIDs = new string[] { "{27E88E1C-9598-49C3-9B48-08FB5F5836B2}", "{91BC9A23-B210-4EE5-B524-93BCD640E58D}",
                                                           "{BF77404C-E8B3-4EE8-9456-BCA121416675}", "{D94429F6-466F-4DF9-8262-DE969EF4491C}",
                                                           "{E0EC09F6-0588-4882-B12C-DE0306550FD6}"};//,
        //"{EA831E01-7D3D-11D4-9A1B-0001031AE963}", "{EA831E02-7D3D-11D4-9A1B-0001031AE963}",
        //"{EA831E05-7D3D-11D4-9A1B-0001031AE963}", "{EA831E06-7D3D-11D4-9A1B-0001031AE963}",
        //"{EA831E03-7D3D-11D4-9A1B-0001031AE963}", "{1CBACE68-7E30-46EF-89F6-486082380E16}"};

        private const string cstMMArcFMAnnoFeatCLSID = "{1CBACE68-7E30-46EF-89F6-486082380E16}";    //mmGeodatabase.mmArcFMAnnotationFeature
        private const string cstESRI_CartoAnnoFeatCLSID = "{E3676993-C682-11D2-8A2A-006097AFF44E}"; //esriCarto.AnnotationFeature

        //MMClassExtensions (EXTCLSID)
        //{E0EC09F6-0588-4882-B12C-DE0306550FD6}	mmGeoDatabase.MMSimpleEdgeFeatureExtension
        //{91BC9A23-B210-4EE5-B524-93BCD640E58D}	mmGeoDatabase.MMSimpleJunctionFeatureExtension
        //{BF77404C-E8B3-4EE8-9456-BCA121416675}	mmGeoDatabase.MMFeatureExtension
        //{D94429F6-466F-4DF9-8262-DE969EF4491C}	mmGeoDatabase.MMObjectExtension
        //{27E88E1C-9598-49C3-9B48-08FB5F5836B2}	mmGeoDatabase.MMComplexEdgeFeatureExtension

        //ArcFM CGOs (CLSID)
        //{EA831E01-7D3D-11D4-9A1B-0001031AE963}	mmGeodatabase.MMArcFMObject
        //{EA831E02-7D3D-11D4-9A1B-0001031AE963}	mmGeodatabase.MMArcFMFeature
        //{EA831E05-7D3D-11D4-9A1B-0001031AE963}	mmGeodatabase.MMArcFMSimpleEdgeFeature
        //{EA831E06-7D3D-11D4-9A1B-0001031AE963}	mmGeodatabase.MMArcFMComplexEdgeFeature
        //{EA831E03-7D3D-11D4-9A1B-0001031AE963}	mmGeodatabase.MMArcFMSimpleJunctionFeature
        //{1CBACE68-7E30-46EF-89F6-486082380E16}	mmGeodatabase.mmArcFMAnnotationFeature
        private static string mstrFGDBNamePath = null;
        static int Main(string[] args)
        {
            //**This connects to Division FC, calls file geodatabase Extraction.exe for each division feature.
            Stopwatch stpWatch = new Stopwatch();
            stpWatch.Start();
            IFeatureCursor pFeatCur = null;
            ModCls.InitMasterPath();
            bool blnProcessFailed = false;
            try
            {
                ModCls.gstrFgdbPath = ConfigurationManager.AppSettings["FGDB_PATH"];
                if (String.IsNullOrEmpty(ModCls.gstrFgdbPath))
                    throw new Exception("Unable to get FGDB path from config file. Exiting");

                if (ModCls.InitProcess() == false)
                    throw new Exception("Failed to initialise 'Service' procees. Please check error log for details.");

                ModCls.LogMessage("testing...");
                //**Copy the Database.
                string strSourcePath = ConfigurationManager.AppSettings["FGDB_CONNECTION"];
                if (string.IsNullOrEmpty(strSourcePath))
                    throw new Exception("Failed to get file geodatabase-path from config file.");
                string sourceDirectory =strSourcePath;
                string targetDirectory = ConfigurationManager.AppSettings["TARGET_Local_Path"];
                if (string.IsNullOrEmpty(targetDirectory))
                    throw new Exception("Failed to get target-path from config file.");

                ////***
                ////**Move file to network path.
                //string targetNetworkPath = ConfigurationManager.AppSettings["Network_Path"];
                //if (string.IsNullOrEmpty(targetNetworkPath))
                //    throw new Exception("Failed to get target-network-path from config file.");
                //string strSourceDir = targetDirectory + ".zip";
                //string strFileName = strSourceDir.Substring(strSourceDir.LastIndexOf(@"\")+1);
                //return 1;
                ////***
                try
                { 
                    Directory.Delete(targetDirectory);
                    Directory.Delete(targetDirectory + ".zip");
                }catch (Exception exIgnore) { }//**Delete the old version.
                if (!Copy(sourceDirectory, targetDirectory))
                    throw new Exception("Failed copy process.");

                mstrFGDBNamePath = targetDirectory;
                if(!ModCls.FileGdbWorkspaceFromPath(mstrFGDBNamePath))
                    throw new Exception("Failed to open FGDB connection.");

                //**De-ArcFM the database.
                DeArcFMDatabase();
                if (blnProcessFailed)
                    throw new Exception("De-ArcFm failed.");
                //if (!ZipFGDB2())
                //    throw new Exception("Failed zip process.");
                
                //**Move file to network path.
                string targetNetworkPath = ConfigurationManager.AppSettings["Network_Path"];
                if (string.IsNullOrEmpty(targetNetworkPath))
                    throw new Exception("Failed to get target-network-path from config file.");
                string strSourceDir = targetDirectory + ".zip";
                string strFileName = strSourceDir.Substring(strSourceDir.LastIndexOf(@"\") + 1);
                MoveFGDB(strSourceDir, targetNetworkPath + "\\"+ strFileName);// "\\Pathfinder.gdb.zip");

                //TimeSpan tmSpan = stpWatch.Elapsed;
                TimeSpan span = stpWatch.Elapsed;
                string strTmElapsed = null;
                if (span.Days > 0)
                    strTmElapsed = string.Format("{0} days, {1} hours, {2} minutes", span.Days, span.Hours, span.Minutes);
                else if (span.Hours > 0)
                    strTmElapsed = string.Format("{0} hours, {1} minutes", span.Hours, span.Minutes);
                else
                    strTmElapsed = string.Format("{0} minutes", span.Minutes);
                ModCls.LogMessage("Time to complete the process: " + strTmElapsed);
                if (blnProcessFailed)//**One or more process failed, return non-zero error code.
                    return (int)ExitCodes.Error;
                else
                    return (int)ExitCodes.Success;
            }

            catch (Exception Ex)
            { ModCls.ErrorMessage(Ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); return (int)ExitCodes.Error; ; }
            finally
            {
                try
                {   //**Release memory.
                    while (Marshal.ReleaseComObject(pFeatCur) != 0) { };
                    pFeatCur = null;
                    ModCls.Dispose();
                }
                catch (Exception Exf) { }
            }
        }

        //**De-ArcFm the database.
        public static bool DeArcFMDatabase()
        {
            try
            {
                IMMObjectClassConversionTool pOjbClsConv = Activator.CreateInstance(Type.GetTypeFromProgID("MMGXTools.MMObjectClassConversionTool")) as IMMObjectClassConversionTool;

                string[] aryObjectClassesToExclude = new string[0];
                GetDataInArray(out aryObjectClassesToExclude, "OBJECT_CLASSES_EXCLUDED");
                List<IDataset> pDeleteList = new List<IDataset>();

                IWorkspace pWorkspace = ModCls.gpFeatWorkspace as IWorkspace;// FileGdbWorkspaceFromPath(mstrFGDBNamePath);// for testing : disable
                //IWorkspace pWorkspace = FileGdbWorkspaceFromPath(@"C:\Source\Offline Data\ExtractedFileGeodatabases\PENINSULA-Sat-Apr-04-2015-6-31-AM.gdb");
                //IWorkspace pWorkspace = FileGdbWorkspaceFromPath(@"C:\Source\Offline Data\ExtractedFileGeodatabases\Copy-Los-Padres-Fri-Dec-12-2014-9-48-AM.gdb");
                if (pWorkspace == null) return false;
                IFeatureWorkspace featWS = (IFeatureWorkspace)pWorkspace;
                IEnumDataset datasets = pWorkspace.get_Datasets(esriDatasetType.esriDTAny);
                IDataset pDs = null;
                string strDsName = null;
                bool blnRemoved = false;
                int intcount = 0;
                int intRemoveExt = 0;
                int intExtNull = 0;
                int intExtExcluded = 0;
                int intAnnoDeArcFM = 0;
                int intEsrified = 0;
                int intSkipped = 0;

                for (IDataset ds = datasets.Next(); ds != null; ds = datasets.Next())
                {
                    blnRemoved = false;
                    if (ds is IObjectClass)
                    {
                        intcount++;
                        try
                        {
                            pDs = ds as IDataset;
                            strDsName = pDs.Name;
                            if (aryObjectClassesToExclude.Contains(strDsName))
                            {
                                pDeleteList.Add(pDs);
                                //pDs.Delete();
                                //ModCls.LogMessage("OBJECT CLASS REMOVED: " + strDsName);
                                blnRemoved = true;
                            }
                        }
                        catch (Exception exDs) { };
                        if (!blnRemoved)
                        {
                            RemoveArcFM((IObjectClass)ds, ref intRemoveExt, ref intExtNull, ref intExtExcluded, ref intAnnoDeArcFM);
                            ConvertToESRI((IObjectClass)ds, pOjbClsConv, ref intEsrified, ref intSkipped);
                        }
                    }
                    else if (ds is IFeatureDataset)
                    {

                        IFeatureDataset fDS = (IFeatureDataset)ds;
                        IFeatureClassContainer fClassContainer = (IFeatureClassContainer)fDS;
                        IEnumFeatureClass fclasses = fClassContainer.Classes;
                        fclasses.Reset();
                        IFeatureClass fc = fclasses.Next();
                        ////***
                        ////if (pDeleteList.Count < 1)
                        ////{
                        //IEnumDataset penumds = fDS.Subsets;
                        //IDataset pds1 = penumds.Next();

                        ////**Only required for GasDataset.
                        //while (pds1 != null)
                        //{
                        //    if (pds1 is IGeometricNetwork)
                        //    {
                        //        //pDeleteList.Add(pds1);
                        //        string strGeoName = pds1.Name;
                        //        ModCls.LogMessage("Deleting: " + strGeoName);
                        //        pds1.Delete();
                        //        ModCls.LogMessage("Successfully deleted: " + strGeoName);

                        //        break;
                        //    }
                        //    pds1 = penumds.Next();
                        //}
                        //Release(pds1 as object);
                        //Release(penumds as object);
                        ////}
                        ////***

                        while (fc != null)
                        {
                            blnRemoved = false;
                            intcount++;
                            try
                            {
                                pDs = fc as IDataset;
                                strDsName = pDs.Name;
                                if (aryObjectClassesToExclude.Contains(strDsName))
                                {
                                    pDeleteList.Add(pDs);
                                    blnRemoved = true;
                                }
                            }
                            catch (Exception exDs) { };
                            if (!blnRemoved)
                            {
                                RemoveArcFM((IObjectClass)fc, ref intRemoveExt, ref intExtNull, ref intExtExcluded, ref intAnnoDeArcFM);
                                ConvertToESRI((IObjectClass)fc, pOjbClsConv, ref intEsrified, ref intSkipped);
                            }
                            try
                            {
                                fc = fclasses.Next();
                            }
                            catch (Exception Ex) { fc = null; }
                        }
                        Release(fDS as object);
                        Release(fClassContainer as object);
                        Release(fclasses as object);
                        Release(fc as object);
                    }
                }
                try
                {
                    for (int i = 0; i < pDeleteList.Count; i++)
                    {
                        try
                        {
                            strDsName = pDeleteList[i].Name;
                            pDeleteList[i].Delete();
                            ModCls.LogMessage("OBJECT CLASS REMOVED: " + strDsName);
                        }
                        catch (Exception Ex1) { }
                    }
                }
                catch (Exception Ex0) { }
                for (int i = 0; i < pDeleteList.Count; i++)
                {
                    try
                    {
                        Release(pDeleteList[i] as object);
                    }
                    catch (Exception Ex1) { }
                }
                Release(pDs as object);
                Release(datasets as object);
                Release(featWS as object);
                Release(pWorkspace as object);
                Release(pOjbClsConv as object);
                //ModCls.LogMessage("Pause for lock releases.");
                //Thread.Sleep((10000 * 6) * 5);
                //ModCls.LogMessage("Pause complete.");
                ModCls.LogMessage("GC Clean start...");
                GC.Collect();
                GC.WaitForPendingFinalizers();
                ModCls.LogMessage("GC Clean complete.");
                //ModCls.LogMessage("Extension Null: " + intExtNull);
                //ModCls.LogMessage("Extension Excluded: " + intExtExcluded);
                //ModCls.LogMessage("Annotation Modified: " + intAnnoDeArcFM);
                //ModCls.LogMessage("Extension Removed: " + intRemoveExt); 
                ModCls.LogMessage("Classes Esrified: " + intEsrified);
                ModCls.LogMessage("Classes Skipped Esrify process: " + intSkipped);

                return true;
            }
            catch (Exception Ex)
            { ModCls.ErrorMessage(Ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); return false; }
        }
        public static bool DeleteGeometricNetwork()
        {
            try
            {
                List<IDataset> pDeleteList = new List<IDataset>();
                IWorkspace pWorkspace = FileGdbWorkspaceFromPath(mstrFGDBNamePath);
                if (pWorkspace == null) return false;
                IFeatureWorkspace featWS = (IFeatureWorkspace)pWorkspace;
                IEnumDataset datasets = pWorkspace.get_Datasets(esriDatasetType.esriDTAny);
                IDataset pDs = null;
                string strDsName = null;

                for (IDataset ds = datasets.Next(); ds != null; ds = datasets.Next())
                {
                    if (ds is IFeatureDataset)
                    {
                        IFeatureDataset fDS = (IFeatureDataset)ds;
                        //***
                        IEnumDataset penumds = fDS.Subsets;
                        IDataset pds1 = penumds.Next();

                        //**Only required for GasDataset.
                        while (pds1 != null)
                        {
                            if (pds1 is IGeometricNetwork)
                            {
                                pDeleteList.Add(pds1);
                                //string strGeoName = pds1.Name;
                                //ModCls.LogMessage("Deleting: " + strGeoName);
                                //pds1.Delete();
                                //ModCls.LogMessage("Successfully deleted: " + strGeoName);
                                //break;
                            }
                            pds1 = penumds.Next();
                        }
                        Release(pds1 as object);
                        Release(penumds as object);
                        Release(fDS as object);
                    }
                    //***
                }
                try
                {
                    for (int i = 0; i < pDeleteList.Count; i++)
                    {
                        try
                        {
                            strDsName = pDeleteList[i].Name;
                            ModCls.LogMessage("Deleting Geometric Network: " + strDsName);
                            pDeleteList[i].Delete();
                            ModCls.LogMessage("Geometric Network Deleted Successfully: " + strDsName);
                        }
                        catch (Exception Ex1) { }
                    }
                }
                catch (Exception Ex0) { }
                for (int i = 0; i < pDeleteList.Count; i++)
                {
                    try
                    {
                        Release(pDeleteList[i] as object);
                    }
                    catch (Exception Ex1) { }
                }
                Release(pDs as object);
                Release(datasets as object);
                Release(featWS as object);
                Release(pWorkspace as object);
                //ModCls.LogMessage("Pause for lock releases.");
                //Thread.Sleep((10000 * 6) * 5);
                //ModCls.LogMessage("Pause complete.");
                ModCls.LogMessage("GC Clean start...");
                GC.Collect();
                GC.WaitForPendingFinalizers();
                ModCls.LogMessage("GC Clean complete.");
                return true;
            }
            catch (Exception Ex)
            { ModCls.ErrorMessage(Ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); return false; }
        }
        static void Release(object comObj)
        {
            try
            {
                int refsLeft = 0;
                do
                {
                    refsLeft = Marshal.ReleaseComObject(comObj);
                }
                while (refsLeft > 0);
                //if (comObj != null)
                //    while (Marshal.ReleaseComObject(comObj) != 0) { };
                comObj = null;
            }
            catch (Exception ExRel) { }
        }
        public static IWorkspace FileGdbWorkspaceFromPath(String path)
        {
            try
            {
                Type factoryType = Type.GetTypeFromProgID("esriDataSourcesGDB.FileGDBWorkspaceFactory");
                IWorkspaceFactory workspaceFactory = (IWorkspaceFactory)Activator.CreateInstance(factoryType);
                return workspaceFactory.OpenFromFile(path, 0);
            }
            catch (Exception Ex)
            { ModCls.ErrorMessage(Ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); return null; }
        }
        private static bool GetDataInArray(out string[] aryDataSetsToInclude, string strPropName)
        {
            aryDataSetsToInclude = new string[0];
            try
            {
                //string strDataSetsToInclude = ConfigurationManager.AppSettings["DATA_SETS_INCLUDED"];
                string strDataSetsToInclude = ConfigurationManager.AppSettings[strPropName];
                aryDataSetsToInclude = strDataSetsToInclude.Split(',');
                return true;
            }
            catch (Exception Ex)
            { ModCls.ErrorMessage(Ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); return false; }
        }        

        private static void RemoveArcFM(IObjectClass oc, ref int intRemoveExt, ref int intExtNull, ref int intExtExcluded, ref int intAnnoDeArcFM)
        {
            try
            {
                if (oc.EXTCLSID != null)
                {
                    if (BadCLSIDs.Contains(oc.EXTCLSID.Value))
                    {
                        intRemoveExt++;
                        IClassSchemaEdit cse = (IClassSchemaEdit)oc;
                        ModCls.LogMessage(intRemoveExt + "." + oc.AliasName + ": \t " + oc.EXTCLSID.Value.ToString());
                        cse.AlterClassExtensionCLSID(null, null);
                    }
                    else
                    {

                        IClassSchemaEdit cse = (IClassSchemaEdit)oc;
                        //cse.AlterClassExtensionCLSID(null, null);
                        if (oc.CLSID.Value.ToString() == cstMMArcFMAnnoFeatCLSID)
                        {
                            //**For annotations, to de-ArcFM they need to have their CLSID modifed from ArcFM to ESRI.
                            intAnnoDeArcFM++;
                            ModCls.LogMessage("\t\t " + intExtExcluded + ". EXTCLSID IDENTIFIED: " + oc.AliasName + ": \t " + oc.EXTCLSID.Value.ToString());
                            ModCls.LogMessage("\t\t " + intAnnoDeArcFM + ". CLSID MODIFIED: " + oc.AliasName + ": \t " + oc.CLSID.Value.ToString());
                            UIDClass pUidCls = new UIDClass();
                            pUidCls.Value = cstESRI_CartoAnnoFeatCLSID;
                            cse.AlterInstanceCLSID(pUidCls);
                        }
                        else
                        {
                            intExtExcluded++;
                            ModCls.LogMessage("\t\t " + intExtExcluded + ". EXTCLSID EXCLUDED: " + oc.AliasName + ": \t " + oc.EXTCLSID.Value.ToString());
                            ModCls.LogMessage("\t\t " + intAnnoDeArcFM + ". CLSID EXCLUDED   : " + oc.AliasName + ": \t " + oc.CLSID.Value.ToString());
                            //cse.AlterClassExtensionCLSID(null, null);
                        }
                    }
                }
                else
                {
                    intExtNull++;
                    ModCls.LogMessage("\t\t " + intExtNull + ". IS NULL: " + oc.AliasName);
                }
            }
            catch (Exception Ex)
            { ModCls.ErrorMessage(Ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); }
        }
        private static void ConvertToESRI(IObjectClass oc, IMMObjectClassConversionTool pOjbClsConv, ref int intEsrified, ref int intSkipped)
        {
            IClass pClass = null;
            try
            {
                if (oc is IClass)
                {
                    pClass = oc as IClass;
                    ModCls.LogMessage(oc.AliasName + ": Convert to ESRI...");
                    bool blnTest = pOjbClsConv.ConvertClass(pClass, mmObjectClassConversionOption.mmConvertToESRI, false);
                    if (blnTest)
                        ModCls.LogMessage(oc.AliasName + ": \t Converted Successfully.");
                    //else
                    //    ModCls.LogMessage(oc.AliasName + ": \t Conversion to ESRI object failed.");
                    intEsrified++;
                }
                else
                {
                    ModCls.LogMessage(oc.AliasName + ": Skipped...");
                    intSkipped++;
                }
            }
            catch (Exception Ex)
            {
                ModCls.ErrorMessage(Ex, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                //ModCls.LogMessage(oc.AliasName + ": \t Error....................................");
            }
            finally { Release(pClass as object); }
        }

        //string sourceDirectory = @"c:\sourceDirectory";
        //string targetDirectory = @"c:\targetDirectory";
        //Copy(sourceDirectory, targetDirectory);
        public static bool Copy(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);
            return CopyAll(diSource, diTarget);
        }
        public static bool CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            try
            {
                ModCls.LogMessage("Make a local copy of FGDB...");
                Directory.CreateDirectory(target.FullName);
                // Copy each file into the new directory.
                foreach (FileInfo fi in source.GetFiles())
                {
                    //Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                    fi.CopyTo(System.IO.Path.Combine(target.FullName, fi.Name), true);
                }

                // Copy each subdirectory using recursion.
                foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
                {
                    DirectoryInfo nextTargetSubDir =
                        target.CreateSubdirectory(diSourceSubDir.Name);
                    CopyAll(diSourceSubDir, nextTargetSubDir);
                }
                ModCls.LogMessage("Completed successfully.");
                return true;
            }
            catch (Exception Ex)
            { ModCls.ErrorMessage(Ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); return false; }
        }

        //private static bool ZipFGDB2()
        //{
        //    try
        //    {
        //        //**Zip the folder if process successfull.     
        //        ModCls.LogMessage("Start zipping process...");
        //        using (var zip = new ZipFile())
        //        {
        //            //zip.AddDirectory(@"C:\Pathfinder\Mobile\DivisionsData\" + strDivsionFGDBName);
        //            zip.UseZip64WhenSaving = Zip64Option.Always;
        //            zip.AddDirectory(mstrFGDBNamePath);
        //            zip.Save(mstrFGDBNamePath + ".zip");
        //        }
        //        ModCls.LogMessage("Completed successfully.");
        //        return true;
        //    }
        //    catch (Exception Ex)
        //    { ModCls.ErrorMessage(Ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); return false; }
        //}
        public static bool MoveFGDB(string sourceDir, string destinationDir)
        {
            try
            {
                //string sourceDir = @"C:\Source\Offline Data\File Geodatabases For Mobile\Empty File Geodatabase.gdb";
                //string destinationDir = @"C:\Source\Offline Data\File Geodatabases For Mobile\Test.gdb";
                if (File.Exists(sourceDir))
                {
                    //File.Move(sourceDir, destinationDir);
                    ModCls.LogMessage("Start FGDB Copy to location process...");
                    ModCls.LogMessage("\t From: " + sourceDir);
                    ModCls.LogMessage("\t To:   " + destinationDir);
                    File.Copy(sourceDir, destinationDir, true);
                    ModCls.LogMessage("File successfully copied");
                }
                return true;
            }
            catch (Exception Ex)
            { ModCls.ErrorMessage(Ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); return false; }
        }
    
        enum ExitCodes : int
        {
            Success = 0,
            Error = 1,
            //SignToolNotInPath = 1,
            //AssemblyDirectoryBad = 2,
            //PFXFilePathBad = 4,
            //PasswordMissing = 8,
            //SignFailed = 16,
            //UnknownError = 32
        }
    }
}
