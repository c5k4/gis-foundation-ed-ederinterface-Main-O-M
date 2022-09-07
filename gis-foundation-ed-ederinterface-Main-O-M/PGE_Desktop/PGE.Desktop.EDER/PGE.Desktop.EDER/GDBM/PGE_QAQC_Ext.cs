using System;
using System.Collections.Generic;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using System.Runtime.InteropServices;
using PGE.Desktop.EDER.ValidationRules;
using System.Collections;
using System.Xml.Serialization;
using System.IO;
using Miner.Geodatabase.GeodatabaseManager.Serialization;
using Miner.Geodatabase.GeodatabaseManager.Logging;
using ADODB;
using System.Data;
using ESRI.ArcGIS.Geometry;
using PGE_DBPasswordManagement;

namespace PGE.Desktop.EDER.GDBM
{
    public class PGE_QAQC_Ext
    {
        private INameLog Log;
        private GdbmReconcilePostService ServiceConfiguration;
        private bool isGDBM = true;
        DBHelper clsdbhelper = new DBHelper();
        CommonFunctions cmnfun = new CommonFunctions();
        public PGE_QAQC_Ext()
        {
            isGDBM = false;
        }
        public PGE_QAQC_Ext(INameLog iNameLog, GdbmReconcilePostService ServiceConfiguration)
        {
            // TODO: Complete member initialization
            this.Log = iNameLog;
            this.ServiceConfiguration = ServiceConfiguration;
            isGDBM = true;

        }


        public bool RunQAQC(IWorkspace ws)
        {
            try
            {            
                
               
                if (isGDBM)
                    this.Log.Debug(ServiceConfiguration.Name, "Getting the version differences");
                IVersionedWorkspace defaultworkspace = (IVersionedWorkspace)(ws);
                IVersion pdefaultversion = defaultworkspace.DefaultVersion;
                CommonFunctions.gConnectionstring = ReadEncryption.GetConnectionStr("EDGIS@EDER");
                //We want inserts/updates and feeder manager only for QAQC but we need deletes 
                //also for the maximum deletes check 
                esriDifferenceType[] pDiffTypes = new esriDifferenceType[6];
                pDiffTypes[0] = esriDifferenceType.esriDifferenceTypeInsert;
                pDiffTypes[1] = esriDifferenceType.esriDifferenceTypeUpdateDelete;
                pDiffTypes[2] = esriDifferenceType.esriDifferenceTypeUpdateNoChange;
                pDiffTypes[3] = esriDifferenceType.esriDifferenceTypeUpdateUpdate;
                pDiffTypes[4] = esriDifferenceType.esriDifferenceTypeDeleteNoChange;
                pDiffTypes[5] = esriDifferenceType.esriDifferenceTypeDeleteUpdate;

                if (ValidationEngine.Instance.VersionDifferences != null) { ValidationEngine.Instance.VersionDifferences.Clear(); }

                Hashtable hshVersionDiffObjects;
                if (ValidationEngine.Instance.Application != null)
                {
                    hshVersionDiffObjects = ValidationEngine.Instance.
                        GetVersionDifferences(
                            (IVersionedWorkspace)ws,
                            pDiffTypes, false); 
                    hshVersionDiffObjects = isGDBM ? ValidationEngine.Instance.
                        GetVersionDifferences(
                            (IVersionedWorkspace)ws,
                            pDiffTypes, false, true, this.Log, ServiceConfiguration.Name) :
                            ValidationEngine.Instance.
                        GetVersionDifferences(
                            (IVersionedWorkspace)ws,
                            pDiffTypes, false);
                }
                else
                {
                    // when it is called from cwosl, application will be null
                    hshVersionDiffObjects = isGDBM ? ValidationEngine.Instance.
                       GetVersionDifferences(
                           (IVersionedWorkspace)ws,
                           pDiffTypes, false, true, this.Log, ServiceConfiguration.Name) :
                           ValidationEngine.Instance.
                       GetVersionDifferences_CWOSL(
                           (IVersionedWorkspace)ws,
                           pDiffTypes, false);
                }


                if (isGDBM)
                    this.Log.Debug(ServiceConfiguration.Name, "Obtained version differences successfully");
                if (hshVersionDiffObjects.Count != 0)
                {
                    //Get list of FC names, and for those FCs QA/QC errors are displayed as 'ERRORS' and not as 'WARNINGS'. SubPNode requirement. April-16th-2019
                    //this.Log.Info("[Substation Requirement]: Get list of feature class to display 'WARNINGS' as 'ERRORS'...");
                    if (isGDBM)
                        PGEError.FeatureClassList_ForceErrors(ws, this.Log);
                    else
                        PGEError.FeatureClassList_ForceErrors(ws);

                    if (isGDBM)
                    {
                        this.Log.Debug(ServiceConfiguration.Name, "Running QA QC on the Version Differences");
                        this.Log.Info(ServiceConfiguration.Name, "Found: " + hshVersionDiffObjects.Count.ToString() + " version difference objects");
                    }

                    #region Getting Version Difference 
                    ValidationEngine.Instance.FilterType =
                            ValidationFilterType.valFilterTypeErrorOnly;
                    ValidationEngine.Instance.SelectionType =
                        ValidationSelectionType.ValidationSelectionTypeVersionDiff;
                    Hashtable hshFullErrorsList = new Hashtable();
                    if (isGDBM)
                    {
                        ValidationEngine.Instance.RunQAQCCustomised(
                            hshVersionDiffObjects, ref hshFullErrorsList, this.Log, ServiceConfiguration.Name);
                    }
                    else if (ValidationEngine.Instance.Application == null)
                    {
                        ValidationEngine.Instance.RunQAQCCustomised_CWOSL(
                            hshVersionDiffObjects, ref hshFullErrorsList);
                    }
                    #endregion
                   

                    //Determine the validation errors 
                    if (hshFullErrorsList.Count != 0)
                    {
                        #region Determine the validation errors
                        if (isGDBM)
                            this.Log.Info(ServiceConfiguration.Name, "QA/QC Errors were found. Writing errors to session errors table.");
                        //Need to serialize our results and write to our errors table.
                        List<PGEError> hashAsList = new List<PGEError>();
                        IDictionaryEnumerator errorListEnum = hshFullErrorsList.GetEnumerator();
                        errorListEnum.Reset();
                        while (errorListEnum.MoveNext())
                        {
                            //KeyValuePair<int, PGEError> kvp = new KeyValuePair<int, PGEError>((int)errorListEnum.Key, (PGEError)errorListEnum.Value);
                            hashAsList.Add((PGEError)errorListEnum.Value);
                        }
                        XmlSerializer QAQCResultsSerializer = new XmlSerializer(typeof(List<PGEError>));
                        StringWriter stringWriter = new StringWriter();
                        QAQCResultsSerializer.Serialize(stringWriter, hashAsList);

                        //Write the QA/QC serialized list to the database.
                        ITable QAQCTable = GetQAQCTable(ws);
                        IRow newRow = QAQCTable.CreateRow();
                        int sessionNameIdx = newRow.Fields.FindField("SESSIONNAME");
                        int errorsIdx = newRow.Fields.FindField("ERRORSLIST");
                        newRow.set_Value(sessionNameIdx, ((IVersion)ws).VersionName);
                        newRow.set_Value(errorsIdx, stringWriter.ToString());
                        newRow.Store();
                        return false;
                        #endregion
                    }
                    else
                    {
                        if (isGDBM)
                        {
                            this.Log.Debug(ServiceConfiguration.Name, "Update Staging Data Started");
                            UpdateStagingData(hshVersionDiffObjects, ws, defaultworkspace, pdefaultversion);
                            this.Log.Debug(ServiceConfiguration.Name, "Update Staging Data Completed");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DeleteOldExistingData((ws as IVersion).VersionName);
                throw ex;
            }
            finally
            {
            }

            return true;
        }
        private void UpdateStagingData(Hashtable hshVersionDiffObjects,IWorkspace ws, IVersionedWorkspace defaultworkspace, IVersion pdefaultversion)
        {
#if DEBUG
            System.Diagnostics.Debugger.Launch();
#endif
            try
            {
               
                IDictionary<string, IList<int>> Stageddata = new Dictionary<string, IList<int>>();
                //DataTable DTStagingTOTAL = new DataTable();
                //DTStagingTOTAL = cmnfun.CreateDTStaging();
                DataTable DTStaging = new DataTable();
                DTStaging = cmnfun.CreateDTStaging();
                #region //JeeraId-382-YXA6 Update Staging Data for Change Detectection API
                string sAction = string.Empty;
                
                string soldGlobalID = string.Empty, strQuery = string.Empty,
                CLASSNAME = string.Empty, sGlobalID = string.Empty;
                string sUsageName = string.Empty;
                string SsessionName = cmnfun.GetSessionNamefromVersionName(ws as IVersion);
                string sVersionName = (ws as IVersion).VersionName.ToString();
                this.Log.Debug(ServiceConfiguration.Name, "Getting session name - " + SsessionName);
                //Check whether session entry is present in the table or not if present then delete
                DeleteOldExistingData(sVersionName);

                #region Get max ObjectID
                long objID = -1;
                string sQuery = "select max(OBJECTID) OBJECTID from " + SchemaInfo.General.pAHInfoTableName;
                DataRow DR = clsdbhelper.GetSingleDataRowByQuery(sQuery);

                if (!string.IsNullOrEmpty(DR[0].ToString()))
                {
                    try { objID = Convert.ToInt32(DR[0].ToString()); } catch { }

                }
                #endregion
                this.Log.Debug(ServiceConfiguration.Name, "Getting Maximum Objectid - " + objID.ToString());
                VersionDiffObject pQAQCSrc = null;
                
                string UpdateQuery = string.Empty;
                DeletedObject pDeletedObject = null;
                //Get Usage  String 
                DataTable UsagesDR = GetUsage();
                //Check Usage

                sUsageName = cmnfun.CheckUsagewithUsageString(UsagesDR, this.Log, this.ServiceConfiguration, SsessionName );

                this.Log.Debug(ServiceConfiguration.Name, "Update data in " + SchemaInfo.General.pAHInfoTableName + " table Started, Total Feature Count - " + hshVersionDiffObjects.Count.ToString());
                for (int i = 0; i < hshVersionDiffObjects.Count; i++)
                {
                    try
                    {
                        sAction = string.Empty;
                        strQuery = string.Empty;
                       // sUsageName = string.Empty;
                        pDeletedObject = null;
                        pQAQCSrc = null;
                        #region Getting Difference Information
                        IObject pTargetObject = null;
                        FeederManagerDiffObject feederDiffObject = null;


                        if (hshVersionDiffObjects[i] is VersionDiffObject)
                        {

                            pQAQCSrc = (VersionDiffObject)
                            hshVersionDiffObjects[i];
                            pTargetObject = pQAQCSrc.Object;
                            if (pQAQCSrc.DifferenceType == esriDifferenceType.esriDifferenceTypeInsert)
                            {
                                sAction = "I";
                            }
                            else 
                            {
                                sAction = "U";
                            }
                            
                        }
                        else if (hshVersionDiffObjects[i] is FeederManagerDiffObject)
                        {
                            feederDiffObject = hshVersionDiffObjects[i] as FeederManagerDiffObject;
                            pTargetObject = feederDiffObject.Object;
                           
                            sAction = "U";

                        }
                        else
                        {
                            //Must be a deleted object so QAQC is not necessary 

                            pTargetObject = null;
                            sAction = "D";
                        }
                        #endregion
                       


                        #region Update/Insert/Delete Case in Staging table

                        sGlobalID = string.Empty;
                       

                        if (pTargetObject != null)
                        {


                                string featclassname = string.Empty;
                                if (pQAQCSrc != null)
                                {
                                    featclassname = pQAQCSrc.DatasetName.ToString();
                                }
                                else if(feederDiffObject !=null)
                                {
                                    featclassname = feederDiffObject.DatasetName;
                                }

                                string Feate_OID = pTargetObject.OID.ToString();
                               
                                objID = objID + 1;
                            // cmnfun.UpdateBulkData(ref DTStagingTOTAL,objID, pTargetObject, Feate_OID, featclassname, sVersionName, SsessionName, sAction, sUsageName, ref DTStaging,ServiceConfiguration.Name,this.Log);
                                cmnfun.UpdateBulkData( ref Stageddata,objID, pTargetObject, Feate_OID, featclassname, sVersionName, SsessionName, sAction, sUsageName, ref DTStaging, ServiceConfiguration.Name, this.Log);

                            // this.Log.Debug(ServiceConfiguration.Name, " FEAT OBJECT ID - " + pTargetObject.OID.ToString() + "  Inserted successfull with Action - " + sAction);

                        }
                        else
                        {
                            pDeletedObject = hshVersionDiffObjects[i] as DeletedObject;
                               
                             if (pDeletedObject != null)
                             {
                                    objID = objID + 1;
                                //cmnfun.UpdateBulkData(ref DTStagingTOTAL,objID, null, pDeletedObject.OID.ToString(), pDeletedObject.DatasetName.ToString(), sVersionName, SsessionName, "D", sUsageName
                                //  , ref DTStaging, ServiceConfiguration.Name,this.Log);

                                cmnfun.UpdateBulkData(ref Stageddata,objID, null, pDeletedObject.OID.ToString(), pDeletedObject.DatasetName.ToString(), sVersionName, SsessionName, "D", sUsageName
                                , ref DTStaging, ServiceConfiguration.Name,this.Log);

                                //  this.Log.Debug(ServiceConfiguration.Name, " FEAT OBJECT ID - " + pDeletedObject.OID.ToString() + "  Inserted successfull with Action - D");

                            }

                        }


                        #endregion


                    }
                    catch (Exception ex)
                    {
                            this.Log.Debug(ServiceConfiguration.Name, "Exception while storing the staging data:  " + ex.Message + " StackTrace: " + ex.StackTrace);
                          
                            throw ex;
                        
                    }
                }
             
                if (DTStaging != null)
                {

                    if (DTStaging.Rows.Count > 0)
                    {
                        clsdbhelper.BulkCopyDataFromDataTable(DTStaging, SchemaInfo.General.pAHInfoTableName);
                        this.Log.Debug(ServiceConfiguration.Name, "Records inserted in " + SchemaInfo.General.pAHInfoTableName + " - " + DTStaging.Rows.Count.ToString());
                    }

                }
             
                #endregion
            }
            catch (Exception ex)
            {
                this.Log.Debug(ServiceConfiguration.Name, "Exception Occurred:  " + ex.Message + " StackTrace: " + ex.StackTrace);

                DeleteOldExistingData((ws as IVersion).VersionName);
                throw ex;
            }
        }

        private bool RowExistInDatatable(string soid, VersionDiffObject pQAQCSrc,DataTable DTStaging)
        {
            bool rowexist = false;
            DataRow[] drd = null;
            try
            {
                if (pQAQCSrc == null)
                {
                    drd = DTStaging.Select("FEAT_OID = " + soid );
                   
                }
                else
                {
                     
                     drd = DTStaging.Select("FEAT_OID = " + soid + "  and FEAT_CLASSNAME='" + pQAQCSrc.DatasetName.ToUpper() + "'");
                   
                }
                if (drd.Length > 0)
                {
                    rowexist = true;
                }
            }

            catch(Exception ex)
            {
                throw ex;
            }
            return rowexist;
        }

        private DataTable GetUsage()
        {
            DataTable DT = null;
            try
            {
                string strQuery = "SELECT key,value from " + SchemaInfo.General.pEDERCONFIGTABLE + " where key like '%GDBMBYPASS_%'";
                DT = clsdbhelper.GetDataTable(strQuery);
                
            }
            catch { }

            return DT;
        }
    
        public void DeleteOldExistingData(string sSessionName)
        {
            try
            {
                string strquery = "delete from " + SchemaInfo.General.pAHInfoTableName + " where VERSIONNAME='" + sSessionName + "'";
                clsdbhelper.UpdateQuery(strquery);
                this.Log.Info("Delete GDBM Info Data for sessionname = "  + sSessionName);
            }

            catch (Exception ex)
            {
                this.Log.Error(ServiceConfiguration.Name, ex.Message.ToString() + " Exception-  " + ex.Message.ToString());
            }
        }

        private void checkdt(DataTable dt)
        {
            try 
            {
                foreach (DataRow dr in dt.Rows)
                {
                    for (int i = 0; i < dr.ItemArray.Length; i++)
                    {
                        string svalue = dr[i].ToString();
                        if (svalue.Contains("'"))
                        {

                            svalue =svalue.Replace("'","\\");
                            dr[i] = svalue;
                            dr.AcceptChanges();
                        }
                        else if (svalue.Contains("&"))
                        {

                            svalue = svalue.Replace("&", "\\");
                            dr[i] = svalue;
                            dr.AcceptChanges();
                        }
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Getting Feature
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="pFWSpace"></param>
        /// <returns></returns>
        public IFeature GetFeature(string FeatureClassName, IFeatureWorkspace pFWSpace, int lOID)
        {
            IFeatureClass pFeatureclass = null;
            IFeature ReturnFeature = null;
            try
            {

                PGE_QAQC.Featureclasslist.TryGetValue(FeatureClassName, out pFeatureclass);
                if (pFeatureclass is null)
                {
                    pFeatureclass = pFWSpace.OpenFeatureClass(FeatureClassName);
                }
                if (pFeatureclass != null)
                {
                    ReturnFeature = pFeatureclass.GetFeature(lOID);
                    if (!PGE_QAQC.Featureclasslist.ContainsKey(FeatureClassName))
                    {
                        PGE_QAQC.Featureclasslist.Add(FeatureClassName, pFeatureclass);
                    }
                }
            }
            catch (Exception ex)
            {

                
            }
            return ReturnFeature;
        }



        public static string geometryToString(ESRI.ArcGIS.Geometry.IGeometry geometry, out int esriGeomType)
        {
            string retStr = string.Empty;
            try
            {
                esriGeomType = (int)geometry.GeometryType;
                ESRI.ArcGIS.Geometry.IWkb wkb = geometry as ESRI.ArcGIS.Geometry.IWkb;
                byte[] byt = new byte[wkb.WkbSize];
                int size = wkb.WkbSize;
                wkb.ExportToWkb(ref size, out byt[0]); retStr = string.Join(", ", byt);
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Error:: " + ex.Message + " at " + ex.StackTrace);
                throw ex;
            }
            return retStr;
        }
      

        /// <summary>
        /// Delete QAQC Session Rows
        /// </summary>
        /// <param name="ws"></param>
        public void DeleteQAQCSessionRows(IWorkspace ws)
        {
            try
            {
                if (isGDBM)
                    this.Log.Info(ServiceConfiguration.Name, "Clearing out current QAQC results");
                //First delete all information from the session errors table so they don't get posted to default.

                ITable QAQCTable = GetQAQCTable(ws);
                IQueryFilter qf = new QueryFilterClass();
                QAQCTable.DeleteSearchedRows(qf);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private ITable GetQAQCTable(IWorkspace ws)
        {
            ITable QAQCTable = null;
            try
            {
                IMMEnumTable QAQCTableEnum = Miner.Geodatabase.ModelNameManager.Instance.TablesFromModelNameWS(ws, SchemaInfo.Electric.ClassModelNames.SessionQAQC);
                QAQCTableEnum.Reset();
               
                QAQCTable = QAQCTableEnum.Next();
                if (QAQCTable == null)
                {
                    throw new Exception("Unable to find table with model name: " + SchemaInfo.Electric.ClassModelNames.SessionQAQC);
                }
            }
            catch(Exception ex) 
            {
                throw ex;
            }
            return QAQCTable;
        }
    }
}
