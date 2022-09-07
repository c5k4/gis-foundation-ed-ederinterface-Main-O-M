using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
//using System.Drawing;
using System.Linq;
using System.Text;
using Miner.Interop;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.EditorExt;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.NetworkAnalysis;
using Miner.Interop.Process;
using ESRI.ArcGIS.Framework;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using System.Reflection;
using PGE.BatchApplication.IGPPhaseUpdate.Processing_Logic_Classes;
using Telvent.Delivery.Framework.FeederManager;
using ESRI.ArcGIS.GeoDatabaseDistributed;
using PGE.BatchApplication.IGPPhaseUpdate.Validation_Rules;

namespace PGE.BatchApplication.IGPPhaseUpdate.Processing_Logic_Classes
{
    public class RunValidationRule
    {
        DBHelper m_DBHelperClass = new DBHelper();
        private Hashtable m_pHT_Severities;
        Common CommonFuntions = new Common();

        internal bool RUNQAQC(IWorkspace pWorkspace, string sCircuitID, ref bool blIfExceededError,ref bool blIfNotInGDBMLimit)
        {
            Hashtable pHT_Severities = new Hashtable();
            bool _retval = false;          
            try
            {
                LoadSeverities();

                esriDifferenceType[] pDiffTypes = new esriDifferenceType[1];
                pDiffTypes[0] = esriDifferenceType.esriDifferenceTypeUpdateNoChange;              
                IVersionedWorkspace pVWS = (IVersionedWorkspace)pWorkspace;   
                
                //Get Version Diffenrece list
                Hashtable hshVersionDiffObjects = GetVersionDifferences(pVWS,pDiffTypes);
                if (hshVersionDiffObjects.Count != 0)
                {
                    if (hshVersionDiffObjects.Count > Convert.ToInt32(ReadConfigurations.GDBMLimit))
                    {
                        blIfNotInGDBMLimit = true;
                        string UpdateQuery = "Update " + ReadConfigurations.DAPHIEFileDetTableName + " set Update_Count = " + hshVersionDiffObjects.Count + " where batchid =  '" + MainClass.Batch_Number + "'";
                        m_DBHelperClass.UpdateQuery(UpdateQuery);
                    }
                    else
                    {
                        CommonFuntions.WriteLine_Info("Running QA QC on the Version Difference" + " at," + DateTime.Now);
                        CommonFuntions.WriteLine_Info("Found: " + hshVersionDiffObjects.Count.ToString() + " version difference objects");

                        //Run the QAQC and generate the error list                         
                        RunQAQCCustomised(hshVersionDiffObjects, sCircuitID, pWorkspace, ref blIfExceededError);
                    }
                    _retval = true;
                }
                else
                {
                    //Check if old phase and calculated phase is same for all the features
                    if (CheckUnProcessedTable() == true)
                    {
                        _retval = true;
                    }
                }
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error(ex.Message + " at " + ex.StackTrace);
            }
            return _retval;
        }

        private bool CheckUnProcessedTable()
        {
            String sQuery = null;
            DataTable pDTCheck = new DataTable();
            try
            {
                sQuery = "SELECT COUNT(*) FROM " + ReadConfigurations.UnprocessedTableName + "  WHERE " + ReadConfigurations.UnprocessedTablesFields.BatchId_FieldName + " = '" + 
                    MainClass.Batch_Number + "' AND " + ReadConfigurations.UnprocessedTablesFields.Processed_FieldName + " = 'P' AND OLD_PHASE <> VALUE";
                pDTCheck = (new DBHelper()).GetDataTable(sQuery);
                if ((pDTCheck != null) && (pDTCheck.Rows.Count > 0))
                {
                    int retcount = Convert.ToInt32(pDTCheck.Rows[0].ItemArray[0].ToString());
                    if (retcount == 0)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error(ex.Message + "   " + ex.StackTrace);
                return false;
            }
        }

        public void RunQAQCCustomised(Hashtable hshVersionDiffObjects, string sCircuitID, IWorkspace pWorkspace, ref bool blIfExceededError)
        {

            string sQuery = null;
            string[] sEnabledClasses = null;
            try
            {
                CommonFuntions.WriteLine_Info("Total Version Difference Objects found," + hshVersionDiffObjects.Count.ToString());

                //Check if there is anything to process 
                if (hshVersionDiffObjects.Count == 0)
                    return;

                //Set up step progressor 
                IDataset pDS = null;
              
                int qaqcCounter = 0;
                IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)pWorkspace;
                string LOOP_FEATURE_QAQC_LIST = "EDGIS.LOOP_FEATURE_QAQC_LIST";
                 
                ITable LOOP_FEATURE_QAQC_LIST_table = featureWorkspace.OpenTable(LOOP_FEATURE_QAQC_LIST);
                //Run QAQC on each object 
                if (hshVersionDiffObjects.Count != 0)
                {
                    //Update FeatureCount in DAPHIE DET
                    string UpdateQuery = "Update " + ReadConfigurations.DAPHIEFileDetTableName + " set Update_Count = " + hshVersionDiffObjects.Count + " where batchid =  '" + MainClass.Batch_Number + "'";
                    m_DBHelperClass.UpdateQuery(UpdateQuery);
                    // Start processing the version difference object
                    for (int i = 0; i <= hshVersionDiffObjects.Count - 1; i++)
                    {
                        IObject pTargetObject = null;
                        if (hshVersionDiffObjects[i] is VersionDiffObject)
                        {
                            //Running the QAQC on for version differences 
                            VersionDiffObject pQAQCSrc = (VersionDiffObject)
                                hshVersionDiffObjects[i];
                            pTargetObject = pQAQCSrc.Object;
                            //pPGEDiffType = GetPGEDifferenceType(pQAQCSrc.DifferenceType);
                        }
                        else if (hshVersionDiffObjects[i] is FeederManagerDiffObject)
                        {
                            FeederManagerDiffObject feederDiffObject = hshVersionDiffObjects[i] as FeederManagerDiffObject;
                            pTargetObject = feederDiffObject.Object;
                             
                        }
                        else if (hshVersionDiffObjects[i] is IObject)
                        {
                            //Running the QAQC for Map Selection 
                            pTargetObject = (IObject)hshVersionDiffObjects[i];
                             
                        }
                        else
                        {
                            //Must be a deleted object so QAQC is not necessary 
                            pTargetObject = null;
                        }

                        if (pTargetObject != null)
                        {
                            pDS = (IDataset)pTargetObject.Class;
                            //sProgressor.Message = "Running PGE QAQC on: " +
                                 
                            RunQAQCOnObject(pTargetObject, sCircuitID, LOOP_FEATURE_QAQC_LIST_table);
                            qaqcCounter++;
                        }

                        if (i % 1000 == 0)
                        {
                            CommonFuntions.WriteLine_Info("Objects Completed" + i.ToString() + " at," + DateTime.Now);
                        }
                    }
                    #region Update the QAQC_log for those features which are not the part of phase update 
                    CommonFuntions.WriteLine_Info("Feature De-Energization started at," + DateTime.Now);

                    sEnabledClasses = GetSourceConnectivityClasses();

                    if (sEnabledClasses != null)
                    {
                        IFeatureClass pFeatClass = null;
                        DataTable UnProcessedRecord = GetNotupdatedRecordsFromTraceTable(sCircuitID, MainClass.Batch_Number);
                        int icounter = 0;
                        foreach (DataRow DR in UnProcessedRecord.Rows)
                        {
                            icounter = icounter + 1;
                            PGE_Validate_Source_Connectivity checkvalidity = new PGE_Validate_Source_Connectivity();
                            IFeature pFeature = null;
                            string sFeatClsName = DR["physicalName"].ToString();
                            int iOid = Convert.ToInt32(DR["TO_FEATURE_OID"].ToString());
                       
                            if (sEnabledClasses.Contains(sFeatClsName) == false)
                            {
                                continue;
                            }

                            if (QAQC_Vaildation.m_pHT_All_FeatCls.Contains(sFeatClsName) == false)
                            {
                                pFeatClass = ((IFeatureWorkspace)pWorkspace).OpenFeatureClass(sFeatClsName);
                                QAQC_Vaildation.m_pHT_All_FeatCls.Add(sFeatClsName, pFeatClass);
                            }
                            else
                            {
                                pFeatClass = (IFeatureClass)QAQC_Vaildation.m_pHT_All_FeatCls[sFeatClsName];
                            }
                            try
                            {
                                pFeature = pFeatClass.GetFeature(iOid);
                            }
                            catch { }

                            if (pFeature != null)
                            {
                                checkvalidity.InternalIsValid(pFeature as IRow, sCircuitID, true);
                            }
                        }
                    }
                    CommonFuntions.WriteLine_Info("Feature De-Energization completed at," + DateTime.Now);
                    #endregion
                    CommonFuntions.WriteLine_Info("QAQC was run on: " + qaqcCounter.ToString() + " rows");
                    if (MainClass.g_DT_QAQC.Rows.Count == 0)
                    {
                        sQuery = "DELETE FROM " + ReadConfigurations.QAQCTableName + " WHERE BATCHID = '" + MainClass.Batch_Number + "'";
                        m_DBHelperClass.UpdateQuery(sQuery);
                        CommonFuntions.WriteLine_Info("No error found to Export to Excel for circuitid," + sCircuitID);
                    }
                    else
                    {
                        //CommonFuntions.WriteLine_Info("Export to Excel started for circuitid," + sCircuitID);
                        //ReadConfigurations.sQAQCErrorFile = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\QAQCLOG_" + sCircuitID + "_" + MainClass.Batch_Number + "_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm") + ".xls";

                        CommonFuntions.WriteLine_Info("Delete earlier records of same batchid started for circuitid," + sCircuitID + " at," + DateTime.Now);
                        sQuery = "DELETE FROM " + ReadConfigurations.QAQCTableName + " WHERE BATCHID = '" + MainClass.Batch_Number + "'";
                        m_DBHelperClass.UpdateQuery(sQuery);
                        CommonFuntions.WriteLine_Info("Delete earlier records of same batchid completed for circuitid," + sCircuitID + " at," + DateTime.Now);

                        CommonFuntions.WriteLine_Info("Export to QC Table started without Comments for circuitid," + sCircuitID + " at," + DateTime.Now);
                        if (m_DBHelperClass.BulkCopyDataFromDataTable(MainClass.g_DT_QAQC, ReadConfigurations.QAQCTableName) == false)
                        {
                            CommonFuntions.WriteLine_Info("Exception occurred  without Comments while copying the bulk data in " + ReadConfigurations.QAQCTableName);
                        }
                        else
                        {
                            CommonFuntions.WriteLine_Info("Export to QC Table completed without Comments for circuitid," + sCircuitID + " at," + DateTime.Now);
                        }


                        //Check if Primary network Features count get exceeded as per config value
                        if (CheckIfExceedingLimit() == true)
                        {
                            blIfExceededError = true;
                        }
                        else
                        {
                            if (ConfigurationManager.AppSettings["QAQC_VALIDATION_REQUIRED"].ToString().ToUpper() == ("true").ToUpper())
                            {
                                CommonFuntions.WriteLine_Info("Export to QAQC started for circuitid," + sCircuitID + " at," + DateTime.Now);
                                QAQC_Vaildation pQAQC_Validation = new QAQC_Vaildation();
                                pQAQC_Validation.Update_QAQC_Validation(featureWorkspace);
                                CommonFuntions.WriteLine_Info("Export to QAQC completed for circuitid," + sCircuitID + " at," + DateTime.Now);


                                CommonFuntions.WriteLine_Info("Delete without comments started for circuitid," + sCircuitID + " at," + DateTime.Now);
                                sQuery = "DELETE FROM " + ReadConfigurations.QAQCTableName + " WHERE BATCHID = '" + MainClass.Batch_Number + "'";
                                m_DBHelperClass.UpdateQuery(sQuery);
                                CommonFuntions.WriteLine_Info("Delete without comments completed for circuitid," + sCircuitID + " at," + DateTime.Now);

                                CommonFuntions.WriteLine_Info("Export to QC Table(comments) started for circuitid," + sCircuitID + " at," + DateTime.Now);
                                if (m_DBHelperClass.BulkCopyDataFromDataTable(MainClass.g_DT_QAQC, ReadConfigurations.QAQCTableName) == false)
                                {
                                   CommonFuntions.WriteLine_Info("Exception occurred while copying the bulk data in " + ReadConfigurations.QAQCTableName);
                                }
                                else
                                {
                                    CommonFuntions.WriteLine_Info("Export to QC Table(comments) completed for circuitid," + sCircuitID + " at," + DateTime.Now);
                                }
                            }

                            
                        }
                        CommonFuntions.WriteLine_Info("Export to Excel started for circuitid," + sCircuitID + " at," + DateTime.Now);                     
                        ReadConfigurations.sQAQCErrorFile = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\LOG\\QAQCLOG_" + sCircuitID + "_" + MainClass.Batch_Number + "_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm") + ".xls";
                        //ReadConfigurations.sQAQCErrorFile = sLogFilePath + "\\QAQCLOG_" + sCircuitID + "_" + MainClass.Batch_Number + "_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm") + ".xls";
                        
                        Common pCommonClass = new Common();
                        pCommonClass.ExportToExcel(MainClass.g_DT_QAQC, ReadConfigurations.sQAQCErrorFile, "QAQCERRORS");
                        CommonFuntions.WriteLine_Info("Export to Excel completed for circuitid," + sCircuitID + " at," + DateTime.Now);

                    }
                  
                }
                
            }
            catch (Exception ex)
            {
                //_logger.Error("Failed in RunQAQCCustomised: " + ex.Message);
                CommonFuntions.WriteLine_Error(ex.Message + " at " + ex.StackTrace);
                throw new Exception("Error in RunQAQCCustomised");
            }
            finally
            {
                GC.Collect();
            }
        }

        private bool CheckIfExceedingLimit()
        {
            DataRow[] pRows = null;
            string[] sAll_PrimaryClasses = null;
            string sWhereClause = null;
            int iSkippingCount = 0;
            try
            {
                iSkippingCount = Convert.ToInt32(ConfigurationManager.AppSettings["QAQC_SKIPPING_PRIMARY_COUNT"].ToString());
                sAll_PrimaryClasses = ReadConfigurations.PrimaryFeatureClasses.Split(',');

                for (int i = 0; i < sAll_PrimaryClasses.Length; ++i)
                {
                    if (i == 0)
                    {
                        sWhereClause = "'" + sAll_PrimaryClasses[i].ToString() + "'"; 
                    }
                    else
                    {
                        sWhereClause = sWhereClause + ",'" + sAll_PrimaryClasses[i].ToString() + "'"; 
                    }
                }

                sWhereClause = sWhereClause + ",'" + ReadConfigurations.Devices.OPENPOINTClassName + "','" + ReadConfigurations.Devices.TieClassName + "'";

                pRows = MainClass.g_DT_QAQC.Select("NAME" + " IN (" + sWhereClause + ")");
                if (pRows.Count() > iSkippingCount)
                {
                    return true;
                }
                
                return false;
            }
            catch(Exception ex)
            {
                CommonFuntions.WriteLine_Error(ex.Message + " at " + ex.StackTrace);
                return false;
            }
        }

        private DataTable GetNotupdatedRecordsFromTraceTable(string sCircuitID, string BatchNumber)
        {
            DataTable returndatatable = null;
            try
            {
                string strQuery = "select a.TO_FEATURE_OID,b.physicalName from " + ReadConfigurations.FeederNetworkTraceTableName + " a left join " + ReadConfigurations.GDBITEMSTableName + " b " +
                " on a.to_feature_fcid = b.objectid where (a.FEEDERID = '" + sCircuitID + "' OR a.FEEDERFEDBY = '" + sCircuitID + "')" +
                " and to_feature_fcid not in('19469','1015') " +
                " and (a.TO_FEATURE_GLOBALID not in (select feature_guid from " + ReadConfigurations.UnprocessedTableName + " where batchid='" + BatchNumber + "' and REC_TYPE<>'TABLE' and processed in('P'))) " +  
                " order by a.order_num desc";

                returndatatable = m_DBHelperClass.GetDataTable(strQuery);
            }
            catch(Exception ex)
            {
                CommonFuntions.WriteLine_Error(ex.Message + " at " + ex.StackTrace);
                
            }
            return returndatatable;
        }


        public Hashtable GetVersionDifferences(IVersionedWorkspace pVWS,esriDifferenceType[] pDiffTypes)
        {
            List<IGeometricNetwork> editedNetworks = new List<IGeometricNetwork>();
            try
            {
                //Get the edit version 
                IVersion pDefaultVersion = pVWS.DefaultVersion;
                IVersion pDesignVersion = (IVersion)pVWS;
                Hashtable hshEditedObjects = new Hashtable();

                if ((pDefaultVersion != null) && (pDesignVersion != null))
                {
                    //Find the common ancestor version 
                    IVersion2 pDesignVersion2 = (IVersion2)pDesignVersion;
                    IVersion pCommonAncestorVersion = pDesignVersion2.
                        GetCommonAncestor(pDefaultVersion);

                    //Find the featureclasses edited in the version 
                    Hashtable hshEditedClasses = GetClassesEditedInVersion(pDesignVersion, pDefaultVersion);

                    //Set up step progressor
                    if (hshEditedClasses.Count != 0)
                    {
                        //Loop through each of the modified classes
                        int fcCounter = 0;
                        foreach (string className in hshEditedClasses.Keys)
                        {
                            fcCounter++;

                            //Get the version differences cursor 
                            IGeometricNetwork editedNetwork = FindVersionDifferences(pDesignVersion,pDefaultVersion,pCommonAncestorVersion,className,ref hshEditedObjects,pDiffTypes);

                        }
                    }
                }
                else
                {
                    throw new Exception("Error returning version difference - version not found");
                }
                return hshEditedObjects;
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error(ex.Message + " at " + ex.StackTrace);
                throw new Exception("Error returning version differences");
                
            }
            finally
            {
            }
        }

        private Hashtable GetClassesEditedInVersion(IVersion pDesignVersion,IVersion pDefaultVersion)
        {
            string ExcludedClass = System.Configuration.ConfigurationManager.AppSettings["FEATURECLASS_EXCLUDED_FROM_VERDIFF"];
            string[] ExcludedClasses = ExcludedClass.Split(',');
            Hashtable hshEditedClasses = new Hashtable();
            IDataChanges diffs = null;
            IEnumModifiedClassInfo modifiedClasses = null;
            IVersionDataChangesInit versionDiffsInit = (IVersionDataChangesInit)new VersionDataChangesClass();
            try
            {
                IWorkspaceName pWSNDesign = (IWorkspaceName)(pDesignVersion as IDataset).FullName;
                IWorkspaceName pWSNDefault = (IWorkspaceName)(pDefaultVersion as IDataset).FullName;

                //Initialize the IVersionDataChangesInit 
                versionDiffsInit.Init(pWSNDesign, pWSNDefault);

                diffs = (IDataChanges)versionDiffsInit;
                modifiedClasses = diffs.GetModifiedClassesInfo();

                IModifiedClassInfo oneModifiedClass = null;
                string sName = null;

                //Find the modified classes that we are interested in
                
                oneModifiedClass = modifiedClasses.Next();
                while (oneModifiedClass != null)
                {
                    sName = oneModifiedClass.ChildClassName.ToUpper();
                    if (ExcludedClasses.Contains(sName) == false)
                    {
                        if (!hshEditedClasses.ContainsKey(sName))
                            hshEditedClasses.Add(sName, 0);
                    }
                   
                    oneModifiedClass = modifiedClasses.Next();
                }
                return hshEditedClasses;
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error(ex.Message + " at " + ex.StackTrace);
                
                throw ex;
            }
        }

        public IGeometricNetwork FindVersionDifferences(
           IVersion pEditVersion,
           IVersion pDefaultVersion,
           IVersion pCommonAncestorVersion,
           string tableName,
           ref Hashtable hshDiffObjects,
           esriDifferenceType[] pDiffTypes)
        {
            IGeometricNetwork editedNetwork = null;
            try
            {
                // Cast the child version to IFeatureWorkspace and open the table.
                Debug.Print("Processing featureclass: " + tableName);
                IFeatureWorkspace pChildFWS = (IFeatureWorkspace)pEditVersion;
                IFeatureWorkspace pDefaultFWS = (IFeatureWorkspace)pDefaultVersion;
                ITable pChildTable = pChildFWS.OpenTable(tableName);
                if (pChildTable is INetworkClass)
                {
                    INetworkClass networkClass = pChildTable as INetworkClass;
                    editedNetwork = networkClass.GeometricNetwork;
                }

                IDataset pChildDS = (IDataset)pChildTable;

                // Cast the common ancestor version to IFeatureWorkspace and open the table.
                IFeatureWorkspace commonAncestorFWS = (IFeatureWorkspace)pCommonAncestorVersion;
                ITable commonAncestorTable = commonAncestorFWS.OpenTable(tableName);

                // Cast to the IVersionedTable interface to create a difference cursor.
                IVersionedTable versionedTable = (IVersionedTable)pChildTable;

                for (int i = 0; i < pDiffTypes.Length; i++)
                {
                    //_logger.Debug("Looking for differences of type: " + pDiffTypes[i].ToString());
                    IDifferenceCursor differenceCursor = versionedTable.Differences
                        (commonAncestorTable, pDiffTypes[i], null);

                    // Step through the cursor, storing OId of each row
                    IRow differenceRow = null;
                    int objectID = -1;
                    differenceCursor.Next(out objectID, out differenceRow);
                    List<int> VersionDiffOIDs = new List<int>();

                    while (objectID != -1)
                    {
                        if ((pDiffTypes[i] == esriDifferenceType.esriDifferenceTypeDeleteNoChange) ||
                            (pDiffTypes[i] == esriDifferenceType.esriDifferenceTypeDeleteUpdate))
                        {
                            //Create a DeletedObject
                            DeletedObject pDelObj = new DeletedObject(
                                tableName.ToUpper(), objectID);
                            hshDiffObjects.Add(hshDiffObjects.Count, pDelObj);
                        }
                        else
                        {
                            VersionDiffOIDs.Add(objectID);
                        }
                        differenceCursor.Next(out objectID, out differenceRow);
                    }

                    //Version information is not associated with the difference row, so we need to get the objects directly from the child
                    //table once we know what has changed.

                    try
                    {
                        int[] oidsToObtain = VersionDiffOIDs.ToArray();
                        ICursor differenceRowsToAdd = pChildTable.GetRows(oidsToObtain, false);
                        IRow row = null;
                        while ((row = differenceRowsToAdd.NextRow()) != null)
                        {
                            IDataset ds = row.Table as IDataset;
                            string currentVersion = ds.Workspace.ConnectionProperties.GetProperty("VERSION").ToString();
                            //Create a VersionDiffObject 
                            VersionDiffObject pQAQCSrc = new VersionDiffObject(
                            pDiffTypes[i], (IObject)row, tableName, row.OID);
                            hshDiffObjects.Add(hshDiffObjects.Count, pQAQCSrc);
                        }

                        if (differenceRowsToAdd != null) { while (Marshal.ReleaseComObject(differenceRowsToAdd) > 0) { } }
                    }//App crash issue resolution
                    catch (Exception ex)
                    {
                        CommonFuntions.WriteLine_Error(ex.Message + " at " + ex.StackTrace);
                    }
                    finally
                    {
                        //App crash issue resolution
                        if (differenceCursor != null)
                        {
                            Marshal.FinalReleaseComObject(differenceCursor);
                            differenceCursor = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error(ex.Message + " at " + ex.StackTrace);
                 
                throw ex;
            }
            return editedNetwork;
        }


        private void RunQAQCOnObject(IObject pObject, string sCircuitID, ITable LOOP_FEATURE_QAQC_LIST_table)
        {
            string sDatasetName = null;
            string sRuleName = null;
            IDataset pDS = null;
            //string sSeverity = null;
            try
            {
                //Check the feature has not already been run 
                pDS = (IDataset)pObject.Class;
                sDatasetName = pDS.BrowseName.ToUpper();

                foreach (object pObj in m_pHT_Severities.Keys)
                {
                    sRuleName = pObj.ToString();
                    //sSeverity = m_pHT_Severities[sRuleName].ToString();

                    if (IsQAQCRuleEnabled(sRuleName, sDatasetName) == false)
                    {
                        continue;
                    }

                    if ("PGE Validate Loop Feature" == sRuleName)
                    {
                        PGE_Validate_Loop_Feature pValidate_Loop_Features = new PGE_Validate_Loop_Feature();
                        if (pValidate_Loop_Features.InternalIsValid((IRow)pObject, sCircuitID, LOOP_FEATURE_QAQC_LIST_table) == true)
                        {
                             
                        }
                    }
                    else if ("PGE Validate Phase Designation" == sRuleName)
                    {
                        PGE_Validate_Phase_Designation pValidate_Phase_Designation = new PGE_Validate_Phase_Designation();
                        if (pValidate_Phase_Designation.InternalIsValid((IRow)pObject, sCircuitID) == true)
                        {

                        }
                    }
                    else if ("PGE Validate Source Connectivity" == sRuleName)
                    {
                        PGE_Validate_Source_Connectivity pValidate_Source_Connectivity = new PGE_Validate_Source_Connectivity();
                        if (pValidate_Source_Connectivity.InternalIsValid((IRow)pObject, sCircuitID,true) == true)
                        {

                        }
                    }
                    else if ("PGE Validate Multi-feed Feature" == sRuleName)
                    {
                        PGE_Validate_Multi_feed_Feature pValidate_Multi_feed_Feature = new PGE_Validate_Multi_feed_Feature();
                        if (pValidate_Multi_feed_Feature.InternalIsValid((IRow)pObject, sCircuitID) == true)
                        {

                        }
                    }
                    else if ("PGE Validate ServicePoint Relationships" == sRuleName)
                    {
                        PGE_Validate_ServicePoint_Relationships pValidate_Service_Relation = new PGE_Validate_ServicePoint_Relationships();
                        if (pValidate_Service_Relation.InternalIsValid((IRow)pObject, sCircuitID) == true)
                        {

                        }
                    }
                    else if ("PGE Validate Unique CGC Number" == sRuleName)
                    {
                        PGE_Validate_Unique_CGC_Number pValidate_Unique_CGC = new PGE_Validate_Unique_CGC_Number();
                        if (pValidate_Unique_CGC.InternalIsValid((IRow)pObject, sCircuitID) == true)
                        {

                        }
                    }
                }


            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error(ex.Message + " at " + ex.StackTrace);
                 
                throw ex;
            }
        }


        private void LoadSeverities()
        {
            DataTable pDT = null;
            string sQuery = null;
            string sName = null;
            string sSeverity = null;
            string sClassesNames = null;
            try
            {
                m_pHT_Severities = new Hashtable();
                pDT = new DataTable();
                sQuery = "SELECT " + ReadConfigurations.ConfigTablesFields.Validation_Name_FieldName + "," + ReadConfigurations.ConfigTablesFields.Validation_Severity_FieldName + "," + 
                    ReadConfigurations.ConfigTablesFields.Validation_EnabledClasses_FieldName + " FROM " + ReadConfigurations.ValidationSeverityMapTableName;
                pDT = m_DBHelperClass.GetDataTable(sQuery);
                if((pDT==null) ||(pDT.Rows.Count==0))
                {
                      throw new Exception("Severity table not found in database,please check with administrator. Table Name is -- " +  ReadConfigurations.ValidationSeverityMapTableName);                   
                }
                for (int i = 0; i < pDT.Rows.Count; ++i)
                {
                    sName = pDT.Rows[i][ReadConfigurations.ConfigTablesFields.Validation_Name_FieldName].ToString();
                    sSeverity = pDT.Rows[i][ReadConfigurations.ConfigTablesFields.Validation_Severity_FieldName].ToString();
                    sClassesNames = pDT.Rows[i][ReadConfigurations.ConfigTablesFields.Validation_EnabledClasses_FieldName].ToString();
                    if (m_pHT_Severities.Contains(sName) == false)
                    {
                        m_pHT_Severities.Add(sName, sSeverity + ":" + sClassesNames);
                    }
                }

            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error(ex.Message + " at " + ex.StackTrace);
                throw ex;
            }
            finally
            {
               
            }
        }

        private string[] GetSourceConnectivityClasses()
        {
            DataTable pDT = null;
            string sQuery = null;
            string sClassesNames = null;
            string[] sAllClasses = null;
            try
            {
                pDT = new DataTable();
                sQuery = "SELECT " + ReadConfigurations.ConfigTablesFields.Validation_EnabledClasses_FieldName + " FROM " +
                    ReadConfigurations.ValidationSeverityMapTableName + " WHERE " + ReadConfigurations.ConfigTablesFields.Validation_Name_FieldName + " = 'PGE Validate Source Connectivity'";
                pDT = m_DBHelperClass.GetDataTable(sQuery);
                if ((pDT == null) || (pDT.Rows.Count == 0))
                {
                    CommonFuntions.WriteLine_Error("Severity table not found in database,please check with administrator. Table Name is -- " + ReadConfigurations.ValidationSeverityMapTableName);
                    return null;
                }
                for (int i = 0; i < pDT.Rows.Count; ++i)
                {
                    sClassesNames = pDT.Rows[i][ReadConfigurations.ConfigTablesFields.Validation_EnabledClasses_FieldName].ToString();
                    sAllClasses = sClassesNames.Split(',');
                    return sAllClasses;
                }
                return sAllClasses;
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error(ex.Message + " at " + ex.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Pass the function what kind of filter is being applied to the validation and 
        /// the name of the validation rule to be run and the function will return a boolean
        /// to indicate whether the rule should be enabled 
        /// </summary>
        /// <param name="name">The name.</param>
        public bool IsQAQCRuleEnabled(string sRuleName,string sInputClassName)
        {
            string sSeverityAndClass = null;
            string sSeverity = null;
            string sClassesName = null;
            string[] sArrayOfClasses = null;
            try
            {
                if (m_pHT_Severities.ContainsKey(sRuleName))
                {
                    sSeverityAndClass = m_pHT_Severities[sRuleName].ToString();
                    sSeverity = sSeverityAndClass.Split(':')[0];
                    sClassesName = sSeverityAndClass.Split(':')[1];

                    sArrayOfClasses = sClassesName.Split(',');

                    if (sArrayOfClasses.Contains(sInputClassName) == true)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                //_logger.Debug("entering error handler for IsQAQCRuleEnabled: " + ex.Message);
                //assume the error should be run (to be safe) - reversed in phase case
                CommonFuntions.WriteLine_Error(ex.Message + " at " + ex.StackTrace);
                return false;
            }
        }
 


        public class DeletedObject
        {
            //Property storage variables 
            private string m_datasetName;
            private int m_oid;

            public DeletedObject(
                string datasetName,
                int oid)
            {
                m_datasetName = datasetName;
                m_oid = oid;
            }

            public string DatasetName
            {
                get { return m_datasetName; }
            }
            public int OID
            {
                get { return m_oid; }
            }
        }

        public class VersionDiffObject
        {
            //Property storage variables 
            private esriDifferenceType m_diffType;
            private IObject m_Object;
            private string m_datasetName;
            private int m_oid;

            public VersionDiffObject(
                esriDifferenceType diffType,
                IObject pObject,
                string datasetName,
                int oid)
            {
                m_diffType = diffType;
                m_Object = pObject;
                m_datasetName = datasetName;
                m_oid = oid;
            }

            public esriDifferenceType DifferenceType
            {
                get { return m_diffType; }
            }
            public IObject Object
            {
                get { return m_Object; }
                set { m_Object = value; }
            }
            public string DatasetName
            {
                get { return m_datasetName; }
                set { m_datasetName = value; }
            }
            public int OID
            {
                get { return m_oid; }
                set { m_oid = value; }
            }
        }

        public class FeederManagerDiffObject
        {
            //Property storage variables 
            private IObject m_Object;
            private string m_datasetName;
            private int m_oid;

            public FeederManagerDiffObject(
                IObject pObject,
                string datasetName,
                int oid)
            {
                m_Object = pObject;
                m_datasetName = datasetName;
                m_oid = oid;
            }

            public IObject Object
            {
                get { return m_Object; }
                set { m_Object = value; }
            }
            public string DatasetName
            {
                get { return m_datasetName; }
                set { m_datasetName = value; }
            }
            public int OID
            {
                get { return m_oid; }
                set { m_oid = value; }
            }
        }

        

    }
}
