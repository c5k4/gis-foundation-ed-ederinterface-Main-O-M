using System;
using System.Collections.Generic;
using System.Linq;
using Miner.Process.GeodatabaseManager.ActionHandlers;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Framework.FeederManager;
using Miner.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using System.Runtime.InteropServices;
using PGE.Common.Delivery.Geodatabase.ChangeDetection;
using ESRI.ArcGIS.GeoDatabaseDistributed;

namespace PGE.Desktop.EDER.GDBM
{
    class PGE_CircuitIDCollectorForROBCCalculation : PxActionHandler
    {
        private ITable _XFRTable;
        private ITable _PriMeter;

        public PGE_CircuitIDCollectorForROBCCalculation()
        {
            this.Name = "PGE CircuitID Collector For ROBC calculation";
            this.Description = "Collects circuit IDs from the posted features to recalculate Established ROBC";
        }

        protected override bool PxSubExecute(Miner.Process.GeodatabaseManager.PxActionData actionData, Miner.Geodatabase.GeodatabaseManager.Serialization.GdbmParameter[] parameters)
        {
#if DEBUG
            System.Diagnostics.Debugger.Launch();
#endif
            this.Log.Info("PGE_CircuitIDCollectorForROBCCalculation: Collecting Circuit IDs to recalculate ROBC V1.0: " + actionData.Version.VersionName);

            try
            {
                if ((new CommonFunctions()).CheckUsage(actionData.Version, this.Log, ServiceConfiguration) == true)
                {
                    return true;
                }
                _XFRTable = null;
                _PriMeter = null;

                int inProcCount = 0;
                int totalCount = 0;
                int counter = 0;

                IWorkspace wSpace = (IWorkspace)actionData.Version;

                HashSet<string> htCircuitIds = new HashSet<string>();

                DifferenceDictionary insertededDictionary = null;
                DifferenceDictionary updatedDictionary = null;
                DifferenceDictionary deletedDictionary = null;

                IVersionedWorkspace versionWS = actionData.Version as IVersionedWorkspace;

                this.Log.Info("PGE_CircuitIDCollectorForROBCCalculation: Initializing DifferenceManager... ");
                DifferenceManager differenceManager = new DifferenceManager(versionWS.DefaultVersion, actionData.Version);
                differenceManager.Initialize();
                
                this.Log.Info("PGE_CircuitIDCollectorForROBCCalculation: Loading differences for insert, update and delete...");
                differenceManager.LoadDifferences(false, esriDataChangeType.esriDataChangeTypeInsert, esriDataChangeType.esriDataChangeTypeUpdate, esriDataChangeType.esriDataChangeTypeDelete);

                insertededDictionary = differenceManager.Inserts;
                updatedDictionary = differenceManager.Updates;
                deletedDictionary = differenceManager.Deletes;

                this.Log.Info("PGE_CircuitIDCollectorForROBCCalculation: Fetching circuit IDs for network inserts... ");
                Dictionary<string, DiffTable>.Enumerator inserts = insertededDictionary.GetEnumerator();
                while (inserts.MoveNext())
                {
                      DiffTable updateDiffTable = inserts.Current.Value;
                      if (updateDiffTable.TableName.ToUpper() != "EDGIS.SERVICEPOINT")
                      {
                         IFeatureWorkspace ifw = (IFeatureWorkspace)wSpace;
                         ITable tstTable = ifw.OpenTable(updateDiffTable.TableName);
                         if (CheckForCircuitIDFields(tstTable))
                         {
                             IEnumerable<IRow> insertedRows = differenceManager.GetInserts(inserts.Current.Key, "");
                             IEnumerator<IRow> insertsRowsEnum = insertedRows.GetEnumerator();

                             this.Log.Info("PGE_CircuitIDCollectorForROBCCalculation: Processing " + inserts.Current.Value.Count + " Inserts... ");

                             totalCount = inserts.Current.Value.Count;
                             inProcCount = 0;
                             counter = 0;

                             while (insertsRowsEnum.MoveNext())
                             {
                                 IRow row = insertsRowsEnum.Current;

                                 inProcCount++;
                                 counter++;

                                 if (counter == 100)
                                 {
                                     this.Log.Info("PGE_CircuitIDCollectorForROBCCalculation: Processing " + inProcCount + " of " + totalCount + " Inserts");
                                     counter = 0;
                                 }

                                 List<string> circuits = GetFM2CircuitIDs(wSpace, row); //using feeder manager 2 for inserted row as field may not have been set yet.
                                 foreach (string circuitID in circuits)
                                 {
                                     if (!htCircuitIds.Contains(circuitID))
                                     {
                                         htCircuitIds.Add(circuitID);
                                         this.Log.Info("Found CircuitID of " + circuitID);
                                     }
                                 }
                                 Marshal.FinalReleaseComObject(row);
                             }
                         }
                         else
                         {
                             this.Log.Info("PGE_CircuitIDCollectorForROBCCalculation: Table with Insert edits skipped as it does not have circuit IDS " + updateDiffTable.TableName + " ");
                         }
                         Marshal.FinalReleaseComObject(tstTable);
                      }
                }

                bool useCurrentWorkSpace;

                this.Log.Info("PGE_CircuitIDCollectorForROBCCalculation: Fetching circuit IDs for network updates... ");
                Dictionary<string, DiffTable>.Enumerator currentDiffTable = updatedDictionary.GetEnumerator();

                while (currentDiffTable.MoveNext())
                {
                    for (int i = 0; i < 2; i++)   // NEED TO GET CURRENT ROW (PRIMARY WORKSPACE) AND ORIGINAL ROW (SECONDARY ROW) SO BOTH SOURCE AND DESTINATION CIRCUITS ARE PROCESSED
                    {
                        if (i == 0)
                        {
                            useCurrentWorkSpace = true;
                        }
                        else
                        {
                            useCurrentWorkSpace = false;
                        }

                        DiffTable updateDiffTable = currentDiffTable.Current.Value;
                        if (updateDiffTable.TableName.ToUpper() != "EDGIS.SERVICEPOINT")
                        {
                            this.Log.Info("PGE_CircuitIDCollectorForROBCCalculation: Inside updates... ");
                            IFeatureWorkspace ifw = (IFeatureWorkspace)wSpace;
                            ITable tstTable = ifw.OpenTable(updateDiffTable.TableName);
                            if (CheckForCircuitIDFields(tstTable))
                            {
                                updatedDictionary.UsePrimaryWorkspace = useCurrentWorkSpace;
                                IEnumerator<DiffRow> updatesRowsEnum = currentDiffTable.Current.Value.GetEnumerator();

                                this.Log.Info("PGE_CircuitIDCollectorForROBCCalculation: Processing " + currentDiffTable.Current.Value.Count + " Updates... ");

                                totalCount = currentDiffTable.Current.Value.Count;
                                inProcCount = 0;
                                counter = 0;

                                while (updatesRowsEnum.MoveNext())
                                {
                                    IRow row = updatesRowsEnum.Current.GetIRow(updatedDictionary.GetWorkspace(DifferenceDictionaryWorkspace.Auto), currentDiffTable.Current.Value);                                    

                                    inProcCount++;
                                    counter++;

                                    if (counter == 100)
                                    {
                                        this.Log.Info("PGE_CircuitIDCollectorForROBCCalculation: Processing " + inProcCount + " of " + totalCount + " Updates");
                                        counter = 0;
                                    }

                                    List<string> circuits = new List<string>();
                                    if (useCurrentWorkSpace)
                                    {
                                        circuits = GetFM2CircuitIDs(wSpace, row);  //using feeder manager 2 for current row as field will not have been set yet.
                                    }
                                    else
                                    {
                                        circuits = GetCircuitIDs(wSpace, row);   //using read off row in memory for original row as field was set.
                                    }
                                    foreach (string circuitID in circuits)
                                    {
                                        if (!htCircuitIds.Contains(circuitID))
                                        {
                                            htCircuitIds.Add(circuitID);
                                            this.Log.Info("Found CircuitID of " + circuitID);
                                        }
                                    }
                                    Marshal.FinalReleaseComObject(row);
                                }
                            }
                            else
                            {
                                this.Log.Info("PGE_CircuitIDCollectorForROBCCalculation: Table with Update edits skipped as it does not have circuit IDS " + updateDiffTable.TableName + " ");
                            }
                            Marshal.FinalReleaseComObject(tstTable);
                        }
                    }
                }

                this.Log.Info("PGE_CircuitIDCollectorForROBCCalculation: Fetching circuit IDs for network deletes... ");
                Dictionary<string, DiffTable>.Enumerator deletes = deletedDictionary.GetEnumerator();
                while (deletes.MoveNext())
                {
                    DiffTable deleteDiffTable = deletes.Current.Value;
                     if (deleteDiffTable.TableName.ToUpper() != "EDGIS.SERVICEPOINT")
                     {
                         IFeatureWorkspace ifw = (IFeatureWorkspace)wSpace;
                         ITable tstTable = ifw.OpenTable(deleteDiffTable.TableName);
                         if (CheckForCircuitIDFields(tstTable))
                         {
                             IEnumerable<IRow> deletedRows = differenceManager.GetDeletes(deletes.Current.Key, "");
                             IEnumerator<IRow> deletesRowsEnum = deletedRows.GetEnumerator();

                             this.Log.Info("PGE_CircuitIDCollectorForROBCCalculation: Processing " + deletes.Current.Value.Count + " Deletes... ");

                             totalCount = deletes.Current.Value.Count;
                             inProcCount = 0;
                             counter = 0;

                             while (deletesRowsEnum.MoveNext())
                             {
                                 IRow row = deletesRowsEnum.Current;

                                 inProcCount++;
                                 counter++;

                                 if (counter == 100)
                                 {
                                     this.Log.Info("PGE_CircuitIDCollectorForROBCCalculation: Processing " + inProcCount + " of " + totalCount + " Deletes");
                                     counter = 0;
                                 }

                                 List<string> circuits = GetCircuitIDs(wSpace, row);
                                 foreach (string circuitID in circuits)
                                 {
                                     if (!htCircuitIds.Contains(circuitID))
                                     {
                                         htCircuitIds.Add(circuitID);
                                         this.Log.Info("Found CircuitID of " + circuitID);
                                     }
                                 }
                                 Marshal.FinalReleaseComObject(row);
                             }
                         }
                         else
                         {
                            this.Log.Info("PGE_CircuitIDCollectorForROBCCalculation: Table with Delete edits skipped as it does not have circuit IDS " + deleteDiffTable.TableName + " ");
                         }
                         Marshal.FinalReleaseComObject(tstTable);
                     }
                }

                List<string> svcPtCircuits = procSVCPTS(wSpace, insertededDictionary, updatedDictionary, deletedDictionary);

                foreach (string circuitID in svcPtCircuits)
                {
                    if (!htCircuitIds.Contains(circuitID))
                        htCircuitIds.Add(circuitID);
                }


                this.Log.Info("PGE_CircuitIDCollectorForROBCCalculation: Circuit count: " + htCircuitIds.Count);
                if (htCircuitIds.Count > 0)
                {
                    string[] circuitIDs = htCircuitIds.ToArray<string>();
                    actionData["ROBCCircuitIDs"] = string.Join(",", circuitIDs);
                }

                return true;
            }
            catch (Exception ex)
            {
                this.Log.Error(ServiceConfiguration.Name, "PGE_CircuitIDCollectorForROBCCalculation: Error during collecting Circuit IDs for ROBC processing: " + ex.Message + " StackTrace: " + ex.StackTrace);
                throw ex;
            }

            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="differenceManager"></param>
        /// <returns></returns>
        /// 

        private List<string> procSVCPTS(IWorkspace wSpace, DifferenceDictionary insertededDictionary, DifferenceDictionary updatedDictionary, DifferenceDictionary deletedDictionary)
        {
            List<string> circuitsIDs = new List<string>();
            List <int> SVCPTOIDsToProcess = new List<int>();
            List<string> xfmrGUIDs = new List<string>();
            List<string> priMeterGUIDs = new List<string>();            
            ITable tbl = null;
            //INSERTS            
            int tryTBL = 0;
            foreach (KeyValuePair<string, DiffTable> inserts in insertededDictionary)
            {
                DiffTable insertDiffTable = inserts.Value;                    
                if (insertDiffTable.TableName.ToUpper() == "EDGIS.SERVICEPOINT")
                {
                    this.Log.Info("PGE_CircuitIDCollectorForROBCCalculation: Processing Service Point Inserts... ");
                    IEnumerator<DiffRow> insertDiffRowEnum = insertDiffTable.GetEnumerator();
                    insertDiffRowEnum.Reset();
                    
                    while (insertDiffRowEnum.MoveNext())
                    {
                        if (tbl == null && tryTBL==0)
                        {
                            tryTBL = 1;
                            try
                            {
                                IFeatureWorkspace fws = (IFeatureWorkspace)insertededDictionary.GetWorkspace(DifferenceDictionaryWorkspace.Primary);
                                tbl = fws.OpenTable(insertDiffTable.TableName);
                                Miner.Interop.IMMModelNameManager iModelNameManager = Miner.Geodatabase.ModelNameManager.Instance; 
                                IEnumBSTR listTransformers = iModelNameManager.ClassNamesFromModelNameWS(insertededDictionary.GetWorkspace(DifferenceDictionaryWorkspace.Primary),SchemaInfo.Electric.ClassModelNames.PGETransformer);
                                listTransformers.Reset();
                                string sFCName = listTransformers.Next();
                                if (sFCName != null && sFCName != String.Empty)
                                {
                                    _XFRTable = fws.OpenTable(sFCName);
                                }
                                IEnumBSTR listPrimaryMeters = iModelNameManager.ClassNamesFromModelNameWS(insertededDictionary.GetWorkspace(DifferenceDictionaryWorkspace.Primary), SchemaInfo.Electric.ClassModelNames.PrimaryMeter);
                                listPrimaryMeters.Reset();
                                sFCName = String.Empty;
                                sFCName = listPrimaryMeters.Next();
                                if (sFCName != null && sFCName != String.Empty)
                                {
                                    _PriMeter = fws.OpenTable(sFCName);
                                }
                            }
                            catch
                            {
                                this.Log.Warn(ServiceConfiguration.Name, "PGE_CircuitIDCollectorForROBCCalculation: Error during collecting Circuit IDs for service point, unable to get Table from Inserts Dictionary");
                            }
                        }
                        SVCPTOIDsToProcess.Add(insertDiffRowEnum.Current.OID);                        
                    }
                }
            }

            tryTBL = 0;
            // UPDATES - CHILD VERSION
            foreach (KeyValuePair<string, DiffTable> cUpdates in updatedDictionary)
            {
                DiffTable cupdateDiffTable = cUpdates.Value;
                if (cupdateDiffTable.TableName.ToUpper() == "EDGIS.SERVICEPOINT")
                {
                    this.Log.Info("PGE_CircuitIDCollectorForROBCCalculation: Processing Service Point Updates... ");
                    IEnumerator<DiffRow> cUpdateDiffRowEnum = cupdateDiffTable.GetEnumerator();
                    cUpdateDiffRowEnum.Reset();

                    while (cUpdateDiffRowEnum.MoveNext())
                    {
                        if (tbl == null && tryTBL == 0)
                        {
                            tryTBL = 1;
                            try
                            {
                                IFeatureWorkspace fws = (IFeatureWorkspace)updatedDictionary.GetWorkspace(DifferenceDictionaryWorkspace.Primary);
                                tbl = fws.OpenTable(cupdateDiffTable.TableName);
                                Miner.Interop.IMMModelNameManager iModelNameManager = Miner.Geodatabase.ModelNameManager.Instance;
                                IEnumBSTR listTransformers = iModelNameManager.ClassNamesFromModelNameWS(updatedDictionary.GetWorkspace(DifferenceDictionaryWorkspace.Primary), SchemaInfo.Electric.ClassModelNames.PGETransformer);
                                listTransformers.Reset();
                                string sFCName = listTransformers.Next();
                                if (sFCName != null && sFCName != String.Empty)
                                {
                                    _XFRTable = fws.OpenTable(sFCName);
                                }
                                IEnumBSTR listPrimaryMeters = iModelNameManager.ClassNamesFromModelNameWS(updatedDictionary.GetWorkspace(DifferenceDictionaryWorkspace.Primary), SchemaInfo.Electric.ClassModelNames.PrimaryMeter);
                                listPrimaryMeters.Reset();
                                sFCName = String.Empty;
                                sFCName = listPrimaryMeters.Next();
                                if (sFCName != null && sFCName != String.Empty)
                                {
                                    _PriMeter = fws.OpenTable(sFCName);
                                }
                            }
                            catch
                            {
                                this.Log.Warn(ServiceConfiguration.Name, "PGE_CircuitIDCollectorForROBCCalculation: Error during collecting Circuit IDs for service point, unable to get Table from Updates Dictionary using the source version / primary workspace");
                            }
                        }                        
                        SVCPTOIDsToProcess.Add(cUpdateDiffRowEnum.Current.OID);
                    }
                }
            }


            if (tbl != null)
            {
                procSVCPTOXFMRIDs(tbl, SVCPTOIDsToProcess, ref xfmrGUIDs, ref priMeterGUIDs);
                getCIDsFromGuids(_XFRTable, xfmrGUIDs, ref circuitsIDs);
                getCIDsFromGuids(_PriMeter, priMeterGUIDs, ref circuitsIDs);
            }
            else
            {
                this.Log.Info(ServiceConfiguration.Name, "PGE_CircuitIDCollectorForROBCCalculation: no changes collected for inserts or updates using the source version / primary workspace");
            }
            SVCPTOIDsToProcess.Clear();

                // UPDATES - PARENT VERSION

            ITable parentVerTbl = null;
            tryTBL = 0;
            foreach (KeyValuePair<string, DiffTable> pUpdates in updatedDictionary)
            {
                DiffTable pUpdateDiffTable = pUpdates.Value;
                if (pUpdateDiffTable.TableName.ToUpper() == "EDGIS.SERVICEPOINT")
                {
                    IEnumerator<DiffRow> pUpdateDiffRowEnum = pUpdateDiffTable.GetEnumerator();
                    pUpdateDiffRowEnum.Reset();
                    while (pUpdateDiffRowEnum.MoveNext())
                    {
                        if (parentVerTbl == null && tryTBL == 0)
                        {
                            tryTBL = 1;
                            try
                            {
                                IFeatureWorkspace fws = (IFeatureWorkspace)updatedDictionary.GetWorkspace(DifferenceDictionaryWorkspace.Secondary);
                                parentVerTbl = fws.OpenTable(pUpdateDiffTable.TableName);
                                Miner.Interop.IMMModelNameManager iModelNameManager = Miner.Geodatabase.ModelNameManager.Instance;
                                IEnumBSTR listTransformers = iModelNameManager.ClassNamesFromModelNameWS(updatedDictionary.GetWorkspace(DifferenceDictionaryWorkspace.Secondary), SchemaInfo.Electric.ClassModelNames.PGETransformer);
                                listTransformers.Reset();
                                string sFCName = listTransformers.Next();
                                if (sFCName != null && sFCName != String.Empty)
                                {
                                    _XFRTable = fws.OpenTable(sFCName);
                                }
                                IEnumBSTR listPrimaryMeters = iModelNameManager.ClassNamesFromModelNameWS(updatedDictionary.GetWorkspace(DifferenceDictionaryWorkspace.Secondary), SchemaInfo.Electric.ClassModelNames.PrimaryMeter);
                                listPrimaryMeters.Reset();
                                sFCName = String.Empty;
                                sFCName = listPrimaryMeters.Next();
                                if (sFCName != null && sFCName != String.Empty)
                                {
                                    _PriMeter = fws.OpenTable(sFCName);
                                }
                            }
                            catch
                            {
                                this.Log.Warn(ServiceConfiguration.Name, "PGE_CircuitIDCollectorForROBCCalculation: Error during collecting Circuit IDs for service point, unable to get Table from Updates Dictionary using the target version / secondary workspace");
                            }
                        }

                        SVCPTOIDsToProcess.Add(pUpdateDiffRowEnum.Current.OID);
                    }
                }
            }
            


            // DELETES
            tryTBL = 0;
            foreach (KeyValuePair<string, DiffTable> deletes in deletedDictionary)
            {
                DiffTable deleteDiffTable = deletes.Value;
                if (deleteDiffTable.TableName.ToUpper() == "EDGIS.SERVICEPOINT")
                {
                    this.Log.Info("PGE_CircuitIDCollectorForROBCCalculation: Processing ServicePoint Deletes... ");
                    IEnumerator<DiffRow> deleteDiffRowEnum = deleteDiffTable.GetEnumerator();
                    deleteDiffRowEnum.Reset();

                    while (deleteDiffRowEnum.MoveNext())
                    {
                        if (parentVerTbl == null && tryTBL == 0)
                        {
                            tryTBL = 1;
                            try
                            {
                                IFeatureWorkspace fws = (IFeatureWorkspace)deletedDictionary.GetWorkspace(DifferenceDictionaryWorkspace.Auto);
                                parentVerTbl = fws.OpenTable(deleteDiffTable.TableName);
                                Miner.Interop.IMMModelNameManager iModelNameManager = Miner.Geodatabase.ModelNameManager.Instance;
                                IEnumBSTR listTransformers = iModelNameManager.ClassNamesFromModelNameWS(deletedDictionary.GetWorkspace(DifferenceDictionaryWorkspace.Auto), SchemaInfo.Electric.ClassModelNames.PGETransformer);
                                listTransformers.Reset();
                                string sFCName = listTransformers.Next();
                                if (sFCName != null && sFCName != String.Empty)
                                {
                                    _XFRTable = fws.OpenTable(sFCName);
                                }
                                IEnumBSTR listPrimaryMeters = iModelNameManager.ClassNamesFromModelNameWS(deletedDictionary.GetWorkspace(DifferenceDictionaryWorkspace.Auto), SchemaInfo.Electric.ClassModelNames.PrimaryMeter);
                                listPrimaryMeters.Reset();
                                sFCName = String.Empty;
                                sFCName = listPrimaryMeters.Next();
                                if (sFCName != null && sFCName != String.Empty)
                                {
                                    _PriMeter = fws.OpenTable(sFCName);
                                }
                            }
                            catch
                            {
                                this.Log.Warn(ServiceConfiguration.Name, "PGE_CircuitIDCollectorForROBCCalculation: Error during collecting Circuit IDs for service point, unable to get Table from Deletes Dictionary using the target version / secondary workspace");
                            }
                        }
                        SVCPTOIDsToProcess.Add(deleteDiffRowEnum.Current.OID);
                    }
                }
            }

            if (parentVerTbl != null)
            {
                procSVCPTOXFMRIDs(parentVerTbl, SVCPTOIDsToProcess, ref xfmrGUIDs, ref priMeterGUIDs);
            }
            else
            {
                this.Log.Info(ServiceConfiguration.Name, "PGE_CircuitIDCollectorForROBCCalculation: no changes collected for deletes or updates using the target version / secondary workspace");
            }
            getCIDsFromGuids(_XFRTable, xfmrGUIDs, ref circuitsIDs);
            getCIDsFromGuids(_PriMeter, priMeterGUIDs, ref circuitsIDs);
            return circuitsIDs;
        }



        //procSVCPTOXFMRIDs(parentVerTbl, SVCPTOIDsToProcess, ref XFMRobjectids, ref PriMeterobjectids);
        private void procSVCPTOXFMRIDs(ITable tbl, List<int> SVCPTOIDsToProcess, ref List<string> XFMRguids, ref List<string> PriMeterGUIDs)
        {
            this.Log.Info("PGE_CircuitIDCollectorForROBCCalculation: Processing Service Point OIDS to find Transformers and Primary Meters for essential customers only... ");
            string OIDList = string.Empty;
            int OIDListCounter = 0;
            int XFRGUIDIDX = tbl.Fields.FindField("TRANSFORMERGUID");
            int priMeterGUIDIDX = tbl.Fields.FindField("PRIMARYMETERGUID");
            
            List<string> SVCPTOIDWhereClause = new List<string>();

            for (int i = 0; i < SVCPTOIDsToProcess.Count; i++)
            {
                if (OIDListCounter == 0)
                {
                    OIDList = Convert.ToString(SVCPTOIDsToProcess[i]);
                    OIDListCounter++;
                }
                else
                {
                    OIDList += ", " + Convert.ToString(SVCPTOIDsToProcess[i]);
                    OIDListCounter++;

                    if (OIDListCounter == 800)
                    {
                        SVCPTOIDWhereClause.Add(OIDList);
                        OIDList = string.Empty;
                        OIDListCounter = 0;
                    }
                }
            }

            if (OIDList != string.Empty) { SVCPTOIDWhereClause.Add(OIDList); }

            string OIDWClauseList = string.Empty;

            IQueryFilter qFilter = null;
            ICursor cursor = null;

            try
            {
                for (int ii = 0; ii < SVCPTOIDWhereClause.Count; ii++)
                {
                    OIDWClauseList = Convert.ToString(SVCPTOIDWhereClause[ii]);

                    qFilter = new QueryFilterClass();
                    qFilter.WhereClause = "OBJECTID in (" + OIDWClauseList + ") and ESSENTIALCUSTOMERIDC = 'Y'";
                    this.Log.Info("PGE_CircuitIDCollectorForROBCCalculation: Processing ServicePoint OIDs in groups: " + OIDWClauseList);
                    cursor = null;
                    cursor = tbl.Search(qFilter, false);

                    IRow row = cursor.NextRow();

                    while (row != null)
                    {
                        object XFRGUIDObj = (object)row.get_Value(XFRGUIDIDX);

                        if ((XFRGUIDIDX != -1) && (XFRGUIDObj.ToString() != string.Empty))
                        {
                            if (!XFMRguids.Contains(XFRGUIDObj.ToString()))
                            {
                                XFMRguids.Add(XFRGUIDObj.ToString());
                                this.Log.Info("PGE_CircuitIDCollectorForROBCCalculation: XFMRGUID: " + XFRGUIDObj.ToString());
                            }
                        }

                        object PMTRGUIDObj = (object)row.get_Value(priMeterGUIDIDX);

                        if ((priMeterGUIDIDX != -1) && (PMTRGUIDObj.ToString() != string.Empty))
                        {
                            if(!PriMeterGUIDs.Contains(PMTRGUIDObj.ToString()))
                            {
                                PriMeterGUIDs.Add(PMTRGUIDObj.ToString());
                                this.Log.Info("PGE_CircuitIDCollectorForROBCCalculation: PrimaryMeterGUID: " + PMTRGUIDObj.ToString());
                            }
                        }
                        Marshal.ReleaseComObject(row);
                        row = cursor.NextRow();
                    }

                    if (cursor != null) { Marshal.ReleaseComObject(cursor); }
                }
            }
            finally
            {
                if (qFilter != null) { Marshal.ReleaseComObject(qFilter); }
                if (cursor != null) { Marshal.ReleaseComObject(cursor); }
            }
        }


        //private void procSVCPTOIDs(ITable tbl, List<int> SVCPTOIDsToProcess, ref List<IRow> SVCPTRowsToProcess)
        //{
        //    string OIDList = string.Empty;
        //    int OIDListCounter = 0;

        //    List <string> SVCPTOIDWhereClause = new List<string>();

        //    for (int i = 0; i < SVCPTOIDsToProcess.Count; i++)
        //    {
        //        if (OIDListCounter == 0)
        //        {
        //            OIDList = Convert.ToString(SVCPTOIDsToProcess[i]);
        //            OIDListCounter++;
        //        }
        //        else
        //        {
        //            OIDList += ", " + Convert.ToString(SVCPTOIDsToProcess[i]);
        //            OIDListCounter++;

        //            if (OIDListCounter == 800)
        //            {
        //                SVCPTOIDWhereClause.Add(OIDList);
        //                OIDList = string.Empty;
        //                OIDListCounter = 0;
        //            }
        //        }
        //    }

        //    if (OIDList != string.Empty) { SVCPTOIDWhereClause.Add(OIDList); }

        //    string OIDWClauseList = string.Empty;

        //    IQueryFilter qFilter = null;
        //    ICursor cursor = null;

        //    try
        //    {
        //        for (int ii = 0; ii < SVCPTOIDWhereClause.Count; ii++)
        //        {
        //            OIDWClauseList = Convert.ToString(SVCPTOIDWhereClause[ii]);

        //            qFilter = new QueryFilterClass();
        //            qFilter.WhereClause = "OBJECTID in (" + OIDWClauseList + ") and ESSENTIALCUSTOMERIDC = 'Y'";

        //            cursor = null;
        //            cursor = tbl.Search(qFilter, false);

        //            IRow row = cursor.NextRow();

        //            while (row != null)
        //            {
        //                if (!SVCPTRowsToProcess.Contains(row)) { SVCPTRowsToProcess.Add(row); };

        //                row = cursor.NextRow();
        //            }

        //            if (cursor != null) { Marshal.ReleaseComObject(cursor); }
        //        }
        //    }
        //    finally
        //    {
        //        if (qFilter != null) { Marshal.ReleaseComObject(qFilter); }
        //        if (cursor != null) { Marshal.ReleaseComObject(cursor); }
        //    }
        //}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentSVCPTCust"></param>
        /// <param name="childSVCPTCust"></param>
        /// <param name="SVCPTToProcess"></param>
        /// 

        private void getSVXPTToProcess(Dictionary<IRow, string> parentSVCPTCust, Dictionary<IRow, string> childSVCPTCust, ref List<IRow> SVCPTToProcess)
        {
            foreach (IRow SVCPTRow in childSVCPTCust.Keys)
            {
                string custValue = (string) SVCPTRow.get_Value(SVCPTRow.Fields.FindField("ESSENTIALCUSTOMERIDC"));
                if (custValue.ToUpper() != parentSVCPTCust[SVCPTRow].ToUpper()) { SVCPTToProcess.Add(SVCPTRow); }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        private List<string> calcSVCPTCircuitID(IWorkspace wSpace, IRow row)
        {
            IFeatureWorkspace fWSpace = (IFeatureWorkspace)wSpace;

            ITable tbl = row.Table;
            List<string> circuitsIDs = new List<string>();

            IDataset tableDS = (IDataset)tbl;

            if (tableDS.Name.ToUpper() == "EDGIS.SERVICEPOINT")
            {
                int XFRGUIDIDX = row.Fields.FindField("TRANSFORMERGUID");
                int priMeterGUIDIDX = row.Fields.FindField("PRIMARYMETERGUID");

                object XFRGUIDObj = (object) row.get_Value(XFRGUIDIDX);

                if ((XFRGUIDIDX != -1) && (XFRGUIDObj.ToString() != string.Empty))
                {
                    if (_XFRTable == null) { if(!getTables(fWSpace)) { return circuitsIDs; } }

                    getCIDs(_XFRTable, XFRGUIDIDX, row, ref circuitsIDs);
                }

                object PMTRGUIDObj = (object) row.get_Value(priMeterGUIDIDX);

                if ((priMeterGUIDIDX != -1) && (PMTRGUIDObj.ToString() != string.Empty))
                {
                    if (_PriMeter == null) { if(!getTables(fWSpace)) { return circuitsIDs; } }

                    getCIDs(_PriMeter, priMeterGUIDIDX, row, ref circuitsIDs);
                }
            }
     
            return circuitsIDs;
        }


        public override bool Enabled(Miner.Geodatabase.GeodatabaseManager.ActionType actionType, Miner.Geodatabase.GeodatabaseManager.Actions gdbmAction, Miner.Geodatabase.GeodatabaseManager.PostVersionType versionType)
        {
            if (actionType == Miner.Geodatabase.GeodatabaseManager.ActionType.Post && gdbmAction == Miner.Geodatabase.GeodatabaseManager.Actions.BeforePost)
            {
                return true;
            }
            else
                return false;
        }

        private List<string> GetCircuitIDs(IWorkspace wSpace, IRow row)
        {
            ITable tbl = row.Table;
            List<string> circuitsIDs = new List<string>();
            int circuitIDIndex = GetFieldIxFromModelName(tbl as IObjectClass, SchemaInfo.Electric.FieldModelNames.CircuitID);
            int circuitID2Index = GetFieldIxFromModelName(tbl as IObjectClass, SchemaInfo.Electric.FieldModelNames.CircuitID2);
            if (circuitIDIndex != -1)
            {
                object circuitID = row.get_Value(circuitIDIndex);
                if (circuitID != null && !string.IsNullOrEmpty(circuitID.ToString()))
                {
                    circuitsIDs.Add(circuitID.ToString());
                }
            }
            if (circuitID2Index != -1)
            {
                object circuitID = row.get_Value(circuitID2Index);
                if (circuitID != null && !string.IsNullOrEmpty(circuitID.ToString()))
                {
                    circuitsIDs.Add(circuitID.ToString());
                }
            }
            return circuitsIDs;
        }

        private List<string> GetFM2CircuitIDs(IWorkspace wSpace, IRow feat)
        {
            List<string> circuitIDs = new List<string>();
            string[] fmcircuitIDs = FeederManager2.GetCircuitIDs(feat);
            if (fmcircuitIDs != null)
            {
                foreach (string circuitID in fmcircuitIDs)
                {
                    if (!string.IsNullOrEmpty(circuitID))
                    {
                        circuitIDs.Add(circuitID);
                    }
                }
            }
            else
            {
                    try
                    {
                        this.Log.Warn(ServiceConfiguration.Name, "Unable to determine circuit ID for " + ((IDataset)(feat.Table)).BrowseName + " OID: " + feat.OID);
                    }
                    catch { }
            }
            return circuitIDs;
        }

        private bool CheckForCircuitIDFields(ITable tableToCheck)
        {
            bool returnVal = false;
            string tableName = " ";
            try
            {
                if (tableToCheck != null && tableToCheck is IObjectClass && tableToCheck is IDataset)
                {
                    tableName = ((IDataset)tableToCheck).Name;
                    int circuitIDIndex = GetFieldIxFromModelName(tableToCheck as IObjectClass, SchemaInfo.Electric.FieldModelNames.CircuitID);
                    int circuitID2Index = GetFieldIxFromModelName(tableToCheck as IObjectClass, SchemaInfo.Electric.FieldModelNames.CircuitID2);
                    if (circuitIDIndex != -1)
                    {
                        returnVal = true;
                    }
                    if (circuitID2Index != -1)
                    {
                        returnVal = true;
                    }
                }
            }
            catch
            {
                this.Log.Error(ServiceConfiguration.Name, "PGE_CircuitIDCollectorForROBCCalculation: error encountered trying to check a table for Circuit ID Fields:" + tableName);
            }
            return returnVal;
        }

        private void getCIDsFromGuids(ITable searchTable, List<string> guidList, ref List<string> circuitsIDs)
        {
            if (guidList == null || guidList.Count==0) { return; }
            ICursor cursor = null;
            IQueryFilter2 qFilter = null;

            try
            {
                int circuitIDIndex = GetFieldIxFromModelName(searchTable as IObjectClass, SchemaInfo.Electric.FieldModelNames.CircuitID);
                int circuitID2Index = GetFieldIxFromModelName(searchTable as IObjectClass, SchemaInfo.Electric.FieldModelNames.CircuitID2);
                if ((circuitIDIndex != -1) || (circuitID2Index != -1))
                {

                    List<string> whereClauses = new List<string>();
                    string whereClause = "";
                    int loopI = -1;
                    foreach (string globalID in guidList)
                    {
                        loopI = loopI + 1;
                        if (loopI == 0)
                        {
                            whereClause = "'" + globalID + "'";
                        }
                        else if (loopI == 992)
                        {
                            whereClause = whereClause + ",'" + globalID + "'";
                            loopI = -1;
                            whereClauses.Add(whereClause);
                            whereClause = "";
                        }
                        else
                        {
                            whereClause = whereClause + ",'" + globalID + "'";
                        }
                    }
                    if (loopI != -1)
                    {
                        loopI = -1;
                        whereClauses.Add(whereClause);
                        whereClause = "";
                    }
                    qFilter = new QueryFilterClass();
                    foreach (string globalidValues in whereClauses)
                    {
                        qFilter.WhereClause = " GLOBALID in (" + globalidValues + ")";
                        cursor = searchTable.Search(qFilter, false);
                        IRow searchRow = cursor.NextRow();
                        while (searchRow != null)
                        {
                                if (circuitIDIndex != -1)
                                {
                                    if (!DBNull.Value.Equals(searchRow.get_Value(circuitIDIndex)))
                                    {
                                        string circuitID = (string)searchRow.get_Value(circuitIDIndex);
                                        if (circuitID != null && !string.IsNullOrEmpty(circuitID.ToString()))
                                        {
                                            if (!circuitsIDs.Contains(circuitID.ToString()))
                                            {
                                                circuitsIDs.Add(circuitID.ToString());
                                            }
                                        }
                                    }
                                }
                                if (circuitID2Index != -1)
                                {
                                    if (!DBNull.Value.Equals(searchRow.get_Value(circuitID2Index)))
                                    {
                                        string circuitID = (string)searchRow.get_Value(circuitID2Index);
                                        if (circuitID != null && !string.IsNullOrEmpty(circuitID.ToString()))
                                        {
                                            if (!circuitsIDs.Contains(circuitID.ToString()))
                                            {
                                                circuitsIDs.Add(circuitID.ToString());
                                            }
                                        }
                                    }
                                }
                            Marshal.FinalReleaseComObject(searchRow);
                            searchRow = cursor.NextRow();
                        }
                    }
                }
            }
            finally
            {
                if (cursor != null) { Marshal.ReleaseComObject(cursor); }
                if (qFilter != null) { Marshal.ReleaseComObject(qFilter); }
            }
        }




        private void getCIDs(ITable searchTable, int guidIDX, IRow row, ref List<string> circuitsIDs)
        {
            object guid = (object)row.get_Value(guidIDX);
            if (guid.ToString() == string.Empty) { return; }

            string testGUID = (string)row.get_Value(guidIDX);
         
            ICursor cursor = null;
            IQueryFilter2 qFilter = null;

            try
            {
                qFilter = new QueryFilterClass();
                qFilter.WhereClause = "GLOBALID = '" + testGUID + "'";
                cursor = searchTable.Search(qFilter, false);

                IRow searchRow = cursor.NextRow();
                if (searchRow != null)
                {
                    int circuitIDIndex = GetFieldIxFromModelName(searchTable as IObjectClass, SchemaInfo.Electric.FieldModelNames.CircuitID);
                    int circuitID2Index = GetFieldIxFromModelName(searchTable as IObjectClass, SchemaInfo.Electric.FieldModelNames.CircuitID2);

                    if (circuitIDIndex != -1)
                    {
                        if (!DBNull.Value.Equals(searchRow.get_Value(circuitIDIndex)))
                        {
                            string circuitID = (string)searchRow.get_Value(circuitIDIndex);
                            if (circuitID != null && !string.IsNullOrEmpty(circuitID.ToString()))
                            {
                                circuitsIDs.Add(circuitID.ToString());
                            }
                        }
                    }
                    else if (circuitID2Index != -1)
                    {
                        if (!DBNull.Value.Equals(searchRow.get_Value(circuitIDIndex)))
                        {
                            string circuitID = (string)searchRow.get_Value(circuitID2Index);
                            if (circuitID != null && !string.IsNullOrEmpty(circuitID.ToString()))
                            {
                                circuitsIDs.Add(circuitID.ToString());
                            }
                        }
                    }
                }
            }
            finally
            {
                if (cursor != null) { Marshal.ReleaseComObject(cursor); }
                if (qFilter != null) { Marshal.ReleaseComObject(qFilter); }
            }

        }



        private bool getTables(IFeatureWorkspace fWSpace)
        {
            string errMsg = string.Empty;

            _XFRTable = fWSpace.OpenTable("EDGIS.TRANSFORMER");
            if (_XFRTable == null) { errMsg = "Unable to Open EDGIS.Transformer..."; }

            _PriMeter = fWSpace.OpenTable("EDGIS.PRIMARYMETER");
            if (_PriMeter == null) { errMsg = errMsg + "Unable to Open EDGIS.PrimaryMeter..."; }

            if (errMsg.Length > 0)
            {
                this.Log.Error(ServiceConfiguration.Name, "PGE_CircuitIDCollectorForROBCCalculation: Error during collecting Circuit IDs for ROBC processing: " + errMsg);
                return false;
            }

            return true;
        }

        private int GetFieldIxFromModelName(IObjectClass table, string fieldModelName)
        {
            int fieldIndex = -1;
            try
            {
                IField field = ModelNameManager.Instance.FieldFromModelName(table, fieldModelName);
                if (field != null)
                {
                    fieldIndex = table.FindField(field.Name);
                }
            }
            catch
            {
                fieldIndex = -1;
            }

            return fieldIndex;
        }
    }
}
