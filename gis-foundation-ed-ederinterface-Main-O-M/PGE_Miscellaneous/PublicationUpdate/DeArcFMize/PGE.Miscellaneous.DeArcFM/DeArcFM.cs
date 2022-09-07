// ========================================================================
// Copyright © 2021 PGE.
// <history>
// De ArcFMize SDE Database
// TCS V3SF (EDGISREARC-767) 04/20/2021               Created
// </history>
// All rights reserved.
// ========================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Configuration;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using System.Reflection;
using System.Text;
using PGE_DBPasswordManagement;

namespace PGE.Miscellaneous.DeArcFM
{
    /// <summary>
    /// **De-ArcFM SDE Connection
    /// </summary>
    class DeArcFM
    {
        #region Data Members
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

        private static Configuration pConfiguration = default(Configuration);
        #endregion

        #region Member Functions

        /// <summary>
        /// Main Function
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static int Main(string[] args)
        {
            #region Data Members
            Stopwatch stpWatch = default;
            IFeatureCursor pFeatCur = default;
            bool blnProcessFailed = default;
            StringBuilder logPath = default;
            StringBuilder strSourcePath = default;
            TimeSpan span = default;
            StringBuilder strTmElapsed = default;
            ExeConfigurationFileMap objExeConfigMap = default;
            #endregion

            try
            {

                if(string.IsNullOrWhiteSpace(args[0]))
                {
                    throw new Exception("Null parameter passed for Config Name. Exiting");
                }

                objExeConfigMap = new ExeConfigurationFileMap();
                objExeConfigMap.ExeConfigFilename = args[0];
                //objExeConfigMap.ExeConfigFilename = @"PGE.Miscellaneous.DeArcFM.exe.config";
                pConfiguration = ConfigurationManager.OpenMappedExeConfiguration(objExeConfigMap, ConfigurationUserLevel.None);
                
                if (!pConfiguration.HasFile)
                {
                    throw new Exception("Config File not found. Exiting");
                }

                stpWatch = new Stopwatch();

                //Start Timer 
                stpWatch.Start();

                blnProcessFailed = false;

                //**Log Path from App Config.
                logPath = new StringBuilder(String.IsNullOrEmpty(pConfiguration.AppSettings.Settings["LOG_Path"].Value) ? System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) : pConfiguration.AppSettings.Settings["LOG_Path"].Value);
                ModCls.strLogPath = Convert.ToString(logPath);
                if (String.IsNullOrEmpty(ModCls.strLogPath))
                    throw new Exception("Unable to get LOG path from config file. Exiting");
                ModCls.LogMessage("Log Path: " + logPath);

                //**Initialization.
                ModCls.InitMasterPath();
                if (ModCls.InitProcess() == false)
                    throw new Exception("Failed to initialise 'Service' procees. Please check error log for details.");
                ModCls.LogMessage("Initialised 'Service' procees");

                //**SDE Connection
                if (Convert.ToString(pConfiguration.AppSettings.Settings["SDE_CONNECTION"].Value).ToUpper().Contains(".SDE"))
                {
                    strSourcePath = new StringBuilder(Convert.ToString(pConfiguration.AppSettings.Settings["SDE_CONNECTION"].Value));
                }
                else
                {
                    strSourcePath = new StringBuilder(ReadEncryption.GetSDEPath(pConfiguration.AppSettings.Settings["SDE_CONNECTION"].Value));
                }
                if (string.IsNullOrEmpty(Convert.ToString(strSourcePath)))
                    throw new Exception("Failed to get SDE geodatabase-path from config file.");
                if (!ModCls.FileGdbWorkspaceFromPath(Convert.ToString(strSourcePath)))
                    throw new Exception("Failed to open SDE connection.");

                //**De-ArcFM the database.
                DeArcFMDatabase();
                if (blnProcessFailed)
                    throw new Exception("De-ArcFm failed.");

                //**Time Elapsed.
                stpWatch.Stop();
                span = stpWatch.Elapsed;
                strTmElapsed = new StringBuilder();

                if (span.Days > 0)
                    strTmElapsed.AppendFormat("{0} days, {1} hours, {2} minutes", span.Days, span.Hours, span.Minutes);
                else if (span.Hours > 0)
                    strTmElapsed.AppendFormat("{0} hours, {1} minutes", span.Hours, span.Minutes);
                else
                    strTmElapsed.AppendFormat("{0} minutes", span.Minutes);

                ModCls.LogMessage("Time to complete the process: " + strTmElapsed);

                //**One or more process failed, return non-zero error code.
                if (blnProcessFailed)
                    return (int)ExitCodes.Error;
                else
                    return (int)ExitCodes.Success;
            }

            catch (Exception Ex)
            {
                ModCls.ErrorMessage(Ex, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                ModCls.LogMessage("De-ArcFm Process failed ");
                return (int)ExitCodes.Error;
            }
            finally
            {
                try
                {   //**Release memory.
                    if (pFeatCur != null)
                    {
                        while (Marshal.ReleaseComObject(pFeatCur) != 0) { };
                    }
                    pFeatCur = null;
                    ModCls.Dispose();
                }
                catch (Exception Exf)
                {
                    ModCls.ErrorMessage(Exf, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                    ModCls.LogMessage("Release memory Process failed ");
                }
            }
        }

        /// <summary>
        /// **De-ArcFm the database.
        /// </summary>
        /// <returns></returns>
        public static bool DeArcFMDatabase()
        {
            #region Data members
            IMMObjectClassConversionTool pOjbClsConv = default;
            IWorkspace pWorkspace = default;
            IFeatureWorkspace featWS = default;
            List<IDataset> pDeleteList = default;
            IEnumDataset datasets = default;
            IDataset pDs = default;
            IDataset ds = default;
            IFeatureDataset fDS = default;
            IFeatureClassContainer fClassContainer = default;
            IEnumFeatureClass fclasses = default;
            IFeatureClass fc = default;
            string[] aryObjectClassesToExclude = default;
            string strDsName = default;
            string[] schemaOwnerToProcess = default;
            bool blnRemoved = false;
            int intcount = default;
            int intRemoveExt = default;
            int intExtNull = default;
            int intExtExcluded = default;
            int intAnnoDeArcFM = default;
            int intEsrified = default;
            int intSkipped = default;
            #endregion

            try
            {
                pOjbClsConv = Activator.CreateInstance(Type.GetTypeFromProgID("MMGXTools.MMObjectClassConversionTool")) as IMMObjectClassConversionTool;

                aryObjectClassesToExclude = new string[0];
                GetDataInArrayUpper(out aryObjectClassesToExclude, "OBJECT_CLASSES_EXCLUDED");
                pDeleteList = new List<IDataset>();
                //schemaOwnerToProcess = pConfiguration.AppSettings.Settings["schemaOwnerToProcess"].Value;
                GetDataInArrayUpper(out schemaOwnerToProcess, "schemaOwnerToProcess");
                pWorkspace = ModCls.gpFeatWorkspace as IWorkspace;

                if (pWorkspace == null)
                    return false;

                featWS = (IFeatureWorkspace)pWorkspace;
                datasets = pWorkspace.get_Datasets(esriDatasetType.esriDTAny);

                blnRemoved = false;
                intcount = 0;
                intRemoveExt = 0;
                intExtNull = 0;
                intExtExcluded = 0;
                intAnnoDeArcFM = 0;
                intEsrified = 0;
                intSkipped = 0;

                for (ds = datasets.Next(); ds != null; ds = datasets.Next())
                {
                    ModCls.LogMessage("Dataset:" + ds.BrowseName);

                    if (schemaOwnerToProcess != null && schemaOwnerToProcess.Length != 0)
                    {
                        if (!schemaOwnerToProcess.Contains(ds.BrowseName.Split('.')[0].ToUpper()))
                            continue;
                    }

                    blnRemoved = false;
                    if (ds is IObjectClass)
                    {
                        ModCls.LogMessage("Dataset (IObjectClass):" + ds.BrowseName);
                        intcount++;
                        try
                        {
                            pDs = ds as IDataset;
                            strDsName = pDs.Name;
                            if (aryObjectClassesToExclude.Contains(strDsName.ToUpper()))
                            {
                                pDeleteList.Add(pDs);
                                ModCls.LogMessage(pDs.Name + " Added to delete List");
                                blnRemoved = true;
                            }
                        }
                        catch (Exception exDs)
                        {
                            ModCls.ErrorMessage(exDs, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                            ModCls.LogMessage(pDs.Name + " Error adding to delete List");
                        }
                        if (!blnRemoved)
                        {
                            if(RemoveArcFM((IObjectClass)ds, ref intRemoveExt, ref intExtNull, ref intExtExcluded, ref intAnnoDeArcFM))
                                ConvertToESRI((IObjectClass)ds, pOjbClsConv, ref intEsrified, ref intSkipped);  
                        }
                    }
                    else if (ds is IFeatureDataset)
                    {
                        ModCls.LogMessage("Dataset (IFeatureDataset):" + ds.BrowseName);

                        fDS = (IFeatureDataset)ds;
                        fClassContainer = (IFeatureClassContainer)fDS;
                        fclasses = fClassContainer.Classes;
                        fclasses.Reset();
                        fc = fclasses.Next();

                        while (fc != null)
                        {
                            if (schemaOwnerToProcess != null && schemaOwnerToProcess.Length != 0)
                            {
                                if (!schemaOwnerToProcess.Contains(((IDataset)fc).BrowseName.Split('.')[0].ToUpper()))
                                {
                                    try
                                    {
                                        fc = fclasses.Next();
                                    }
                                    catch (Exception Ex)
                                    {
                                        ModCls.ErrorMessage(Ex, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                                        ModCls.LogMessage(" Error enumirating to next Feature Class in DS :" + pDs.Name);

                                        fc = null;
                                    }

                                    continue;
                                }
                            }

                            ModCls.LogMessage("Dataset (IFeatureClass):" + fc.AliasName);
                            blnRemoved = false;
                            intcount++;
                            try
                            {
                                pDs = fc as IDataset;
                                strDsName = pDs.Name;
                                if (aryObjectClassesToExclude.Contains(strDsName.ToUpper()))
                                {
                                    pDeleteList.Add(pDs);
                                    ModCls.LogMessage(pDs.Name + " Added to delete List");
                                    blnRemoved = true;
                                }
                            }
                            catch (Exception exDs)
                            {
                                ModCls.ErrorMessage(exDs, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                                ModCls.LogMessage(pDs.Name + " Error adding to delete List");
                            };
                            if (!blnRemoved)
                            {
                                if(RemoveArcFM((IObjectClass)fc, ref intRemoveExt, ref intExtNull, ref intExtExcluded, ref intAnnoDeArcFM))
                                    ConvertToESRI((IObjectClass)fc, pOjbClsConv, ref intEsrified, ref intSkipped);
                            }
                            try
                            {
                                fc = fclasses.Next();
                            }
                            catch (Exception Ex)
                            {
                                ModCls.ErrorMessage(Ex, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                                ModCls.LogMessage(" Error enumirating to next Feature Class in DS :" + pDs.Name);

                                fc = null;
                            }
                        }
                        Release(fDS as object);
                        Release(fClassContainer as object);
                        Release(fclasses as object);
                        Release(fc as object);
                    }
                }

                try
                {
                    ModCls.LogMessage("Deleting  Datasets");

                    for (int i = 0; i < pDeleteList.Count; i++)
                    {
                        try
                        {
                            strDsName = pDeleteList[i].Name;
                            pDeleteList[i].Delete();
                            ModCls.LogMessage("OBJECT CLASS REMOVED: " + strDsName);
                        }
                        catch (Exception Ex1)
                        {
                            ModCls.ErrorMessage(Ex1, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                            ModCls.LogMessage("Error Deleting  " + strDsName);
                        }
                    }
                }
                catch (Exception Ex0)
                {
                    ModCls.ErrorMessage(Ex0, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                    ModCls.LogMessage("Error Deleting  Datasets");
                }

                for (int i = 0; i < pDeleteList.Count; i++)
                {
                    try
                    {
                        Release(pDeleteList[i] as object);
                    }
                    catch (Exception Ex1)
                    {
                        ModCls.ErrorMessage(Ex1, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                        ModCls.LogMessage("Error Releasing Delete Datasets List");
                    }
                }

                ModCls.LogMessage("Classes Esrified: " + intEsrified);
                ModCls.LogMessage("Classes Skipped Esrify process: " + intSkipped);

                return true;
            }
            catch (Exception Ex)
            {
                ModCls.ErrorMessage(Ex, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                return false;
            }
            finally
            {
                Release(pDs as object);
                Release(datasets as object);
                Release(featWS as object);
                Release(pWorkspace as object);
                Release(pOjbClsConv as object);

                ModCls.LogMessage("GC Clean start...");
                GC.Collect();
                GC.WaitForPendingFinalizers();
                ModCls.LogMessage("GC Clean complete.");
            }
        }

        /// <summary>
        /// **Release Com Objects
        /// </summary>
        /// <param name="comObj"></param>
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
            catch (Exception ExRel)
            {
                ModCls.ErrorMessage(ExRel, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            }
        }

        /// <summary>
        /// **Return List of Datasets to Delete from SDE
        /// </summary>
        /// <param name="aryDataSetsToExclude"></param>
        /// <param name="strPropName"></param>
        /// <returns></returns>
        private static bool GetDataInArray(out string[] aryDataSetsToExclude, string strPropName)
        {
            string strDataSetsToExclude = default;
            aryDataSetsToExclude = default;
            try
            {
                aryDataSetsToExclude = new string[0];
                strDataSetsToExclude = pConfiguration.AppSettings.Settings[strPropName].Value;
                aryDataSetsToExclude = strDataSetsToExclude.Split(',');
                return true;
            }
            catch (Exception Ex)
            {
                ModCls.ErrorMessage(Ex, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                return false;
            }
        }

        /// <summary>
        /// **Return List of Datasets to Delete from SDE
        /// </summary>
        /// <param name="aryDataSetsToExclude"></param>
        /// <param name="strPropName"></param>
        /// <returns></returns>
        private static bool GetDataInArrayUpper(out string[] aryDataSetsToExclude, string strPropName)
        {
            string strDataSetsToExclude = default;
            aryDataSetsToExclude = default;
            try
            {
                aryDataSetsToExclude = new string[0];
                strDataSetsToExclude = pConfiguration.AppSettings.Settings[strPropName].Value.ToUpper();
                aryDataSetsToExclude = strDataSetsToExclude.Split(',');
                return true;
            }
            catch (Exception Ex)
            {
                ModCls.ErrorMessage(Ex, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                return false;
            }
        }

        /// <summary>
        /// **Removes ArcFM Components
        /// </summary>
        /// <param name="oc"></param>
        /// <param name="intRemoveExt"></param>
        /// <param name="intExtNull"></param>
        /// <param name="intExtExcluded"></param>
        /// <param name="intAnnoDeArcFM"></param>
        private static bool RemoveArcFM(IObjectClass oc, ref int intRemoveExt, ref int intExtNull, ref int intExtExcluded, ref int intAnnoDeArcFM)
        {
            #region Data Members
            bool retValue = true;
            IClassSchemaEdit cse = default;
            ESRI.ArcGIS.esriSystem.UIDClass pUidCls = default;
            #endregion

            try
            {
                ModCls.LogMessage("RemoveArcFM Start :" + oc.AliasName);
                if (oc.EXTCLSID != null)
                {
                    if (BadCLSIDs.Contains(oc.EXTCLSID.Value))
                    {
                        intRemoveExt++;
                        cse = (IClassSchemaEdit)oc;
                        ModCls.LogMessage(intRemoveExt + "." + oc.AliasName + ": \t " + oc.EXTCLSID.Value.ToString());
                        cse.AlterClassExtensionCLSID(null, null);
                    }
                    else
                    {
                        cse = (IClassSchemaEdit)oc;
                        if (oc.CLSID.Value.ToString() == cstMMArcFMAnnoFeatCLSID)
                        {
                            //**For annotations, to de-ArcFM they need to have their CLSID modifed from ArcFM to ESRI.
                            intAnnoDeArcFM++;
                            ModCls.LogMessage("\t\t " + intExtExcluded + ". EXTCLSID IDENTIFIED: " + oc.AliasName + ": \t " + oc.EXTCLSID.Value.ToString());
                            ModCls.LogMessage("\t\t " + intAnnoDeArcFM + ". CLSID MODIFIED: " + oc.AliasName + ": \t " + oc.CLSID.Value.ToString());
                            pUidCls = new ESRI.ArcGIS.esriSystem.UIDClass();
                            pUidCls.Value = cstESRI_CartoAnnoFeatCLSID;
                            cse.AlterInstanceCLSID(pUidCls);
                            retValue = false;
                        }
                        else
                        {
                            intExtExcluded++;
                            ModCls.LogMessage("\t\t " + intExtExcluded + ". EXTCLSID EXCLUDED: " + oc.AliasName + ": \t " + oc.EXTCLSID.Value.ToString());
                            ModCls.LogMessage("\t\t " + intAnnoDeArcFM + ". CLSID EXCLUDED   : " + oc.AliasName + ": \t " + oc.CLSID.Value.ToString());
                        }
                    }
                }
                else
                {
                    intExtNull++;
                    ModCls.LogMessage("\t\t " + intExtNull + ". IS NULL: " + oc.AliasName);
                    retValue = false;
                }
                ModCls.LogMessage("RemoveArcFM End :" + oc.AliasName);
            }
            catch (Exception Ex)
            {
                ModCls.ErrorMessage(Ex, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                retValue = false;
            }
            
            return retValue;
        }

        /// <summary>
        /// **Converts ArcFM Datasets to ESRI Datasets
        /// </summary>
        /// <param name="oc"></param>
        /// <param name="pOjbClsConv"></param>
        /// <param name="intEsrified"></param>
        /// <param name="intSkipped"></param>
        private static void ConvertToESRI(IObjectClass oc, IMMObjectClassConversionTool pOjbClsConv, ref int intEsrified, ref int intSkipped)
        {
            #region Data Members
            IClass pClass = default;
            bool blnTest = default;
            #endregion

            try
            {
                ModCls.LogMessage("ConvertToESRI Start :" + oc.AliasName);
                if (oc is IClass)
                {
                    pClass = oc as IClass;
                    ModCls.LogMessage(oc.AliasName + ": Convert to ESRI...");
                    blnTest = pOjbClsConv.ConvertClass(pClass, mmObjectClassConversionOption.mmConvertToESRI, false);

                    if (blnTest)
                        ModCls.LogMessage(oc.AliasName + ": \t Converted Successfully.");
                    else
                        ModCls.LogMessage(oc.AliasName + ": \t Conversion to ESRI object failed.");
                    intEsrified++;
                }
                else
                {
                    ModCls.LogMessage(oc.AliasName + ": Skipped...");
                    intSkipped++;
                }
                ModCls.LogMessage("ConvertToESRI End :" + oc.AliasName);
            }
            catch (Exception Ex)
            {
                ModCls.ErrorMessage(Ex, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                ModCls.LogMessage(oc.AliasName + ": \t Error....................................");
            }
            finally
            {
                Release(pClass as object);
            }
        }

        /// <summary>
        /// **Exit Return Codes
        /// </summary>
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

        #endregion

    }
}
