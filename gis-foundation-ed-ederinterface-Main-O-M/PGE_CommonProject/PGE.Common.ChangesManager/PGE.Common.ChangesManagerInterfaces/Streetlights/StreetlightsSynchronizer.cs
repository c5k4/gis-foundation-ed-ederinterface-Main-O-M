using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;
using PGE.Common.ChangeDetectionAPI;
using PGE.Common.Delivery.Diagnostics;
using PGE.Desktop.EDER.AutoUpdaters.Special;
using PGE.Desktop.EDER.AutoUpdaters.Special.RegionalAttributesAU;

namespace PGE.Common.ChangesManagerShared.Streetlights
{
    public class StreetlightSynchronizer
    {
        #region Member vars

        // For logging
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "ChangeDetectionDefault.log4net.config");

        // For managing configurable settings
        private Configuration _config = null;

        // For managing sessions
        private MinerSession _minerSession;
        private IWorkspace _workspace;
        private IWorkspace _nonVersionedWorkspace;
        private SDEWorkspaceConnection _sdeWorkspace;
        private SDEWorkspaceConnection _landbaseWorkspace;

        // For interacting with systems
        private StreetlightCcb _slCcb = null;
        private StreetlightGis _slGis = null;
        private StreetlightInv _slInv = null;
        private StreetlightFieldPts _slFieldPoints = null; 
        private StreetlightInvStage _slInvStage = null;

        #endregion

        #region Properties

        public SDEWorkspaceConnection SdeWorkspace
        {
            set { _sdeWorkspace = value; }
        }

        public SDEWorkspaceConnection LandbaseWorkspace
        {
            set { _landbaseWorkspace = value; }
        }

        public StreetlightCcb StreetlightCCB
        {
            set
            {
                _slCcb = value;
                if (GetSetting("ccbPath") != "")
                {
                    _slCcb.ReportPath = GetSetting("ccbPath");
                }
                if (GetSetting("ccbInsertsFile") != "")
                {
                    _slCcb.ReportFile = GetSetting("ccbInsertsFile");
                }
            }
        }

        #endregion

        #region Constructor

        // CCB->GIS
        public StreetlightSynchronizer(string configFile)
        {
            InitConfig(configFile);
        }

        // Conflation
        public StreetlightSynchronizer(MinerSession minerSession, string configFile)
        {
            InitConfig(configFile);
            _minerSession = minerSession;
        }

        #endregion

        #region Public CD sync methods

        public void SyncGISAdd(IRow newSL, IRow parentRow = null)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            try
            {
                _logger.Info("Synchronizing new streetlight from GIS with OID of: " + newSL.OID.ToString());

                // Check for In Service status
                bool wasPlacedInService = false;
                bool wasAlreadyInService = false;
                _slGis.WasPlacedInService(newSL as IFeature, parentRow as IFeature, out wasPlacedInService, out wasAlreadyInService);

                // Check for valid SPID assignment
                bool wasAssignedSPID = false;
                bool wasAlreadyAssignedSPID = false;
                _slGis.WasAssignedSPID(newSL as IFeature, parentRow as IFeature, out wasAssignedSPID, out wasAlreadyAssignedSPID);

                // Check for existing GIS ID value, if it has one... it must have come from INV table
                bool hasGisId = _slGis.HasGisID(newSL as IFeature);
                
                // If it was placed in service and was assigned SPID.. or one of those things happened and the other was already true
                if ((wasPlacedInService == true && wasAssignedSPID == true && hasGisId == false) ||
                    (wasPlacedInService == true && wasAssignedSPID == true && hasGisId == true && parentRow != null) || 
                   (wasPlacedInService == true && wasAlreadyAssignedSPID == true) || 
                   (wasAssignedSPID == true && wasAlreadyInService))
                {

                    //ME Q3 ITEM 2018: DATA FROM GIS WILL BE SENT AS UPDATE ONLY IF DATA EXISTS IN SLCDX DATA
                    bool IsSPIDInSlcdx ;
                    _slGis.IsSPIDInSLCDXData(newSL as IFeature,out IsSPIDInSlcdx);
                    if (IsSPIDInSlcdx)
                    {

                        // Ensure lookup tables are added
                        IDictionary<string, string> ccbItemTypeCodeLookup = _slCcb.GetLookupTableValues("CCBTOGIS_ITEMTYPECODE_LOOKUP");
                        _slGis.AddToDomainMap("CCBTOGIS_ITEMTYPECODE_LOOKUP", ccbItemTypeCodeLookup);
                        IDictionary<string, string> ccbOpScheduleCodeLookup = _slCcb.GetLookupTableValues("CCBTOGIS_OPSCHEDULE_LOOKUP");
                        _slGis.AddToDomainMap("CCBTOGIS_OPSCHEDULE_LOOKUP", ccbOpScheduleCodeLookup);

                        if (hasGisId == false)
                        {
                            // Update the new SL with a GIS ID
                            string newGisID = _slCcb.GetNextGisID();
                            if (newGisID != string.Empty)
                            {
                                _logger.Info("Updating SL: " + newSL.OID.ToString() + " with GISID of " + newGisID);
                                _slGis.UpdateGisID(newSL, newGisID);
                            }

                            // Submit the new SL to the Inventory table
                            _logger.Info("Inserting SL:" + newSL.OID.ToString() + " into Inventory table");
                            IList<string> invValues = _slGis.GetValues(newSL, StreetlightGis.INV_FIELDS, StreetlightGis.FetchType.FetchTypeInv);
                            _slInv.Insert((newSL as IFeature).ShapeCopy, invValues);

                            // Submit the new SL to the FieldPts table
                            _logger.Info("Inserting SL:" + newSL.OID.ToString() + " into FieldPoints table");
                            IList<string> fieldPtsValues = _slGis.GetValues(newSL, StreetlightGis.FIELDPTS_FIELDS, StreetlightGis.FetchType.FetchTypeFieldPts);
                            _slFieldPoints.Insert((newSL as IFeature).ShapeCopy, fieldPtsValues);
                        }

                        // Submit the new SL to CCB
                        _logger.Info("Sending SL: " + newSL.OID.ToString() + " to CCB");
                        //IList<string> ccbValues = _slGis.GetValues(newSL, StreetlightGis.CCB_EXPORT_FIELDS, StreetlightGis.FetchType.FetchTypeCcb);
                        IList<string> ccbValues = _slGis.GetValues(newSL, StreetlightGis.CCB_EXPORT_FIELDS_FIELDPTS, StreetlightGis.FetchType.FetchTypeCcb);
                        _slCcb.WriteCCB(ccbValues);
                    }
                    else
                    {
                        _logger.Info("This SL: " + newSL.OID.ToString() + " has not been sent to CCB as SPID not found in SLCDX_DATA");
                    }
                    
                }
                else
                {
                    if (wasPlacedInService == true && wasAssignedSPID == true && hasGisId == true)
                    {
                        _logger.Info("New In Service/non-zero SPID streetlight entered with GIS ID. Assuming came from Inventory and ignoring");
                    }
                    else
                    {
                        _logger.Info("Streetlight is not In Service and/or has no SP ID so not sending to Inventory or CCB");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to synchronize streetlight add: " + ex.ToString());
            }
        }

        public void CDSyncGISAdd(IRow newSL, UpdateFeat parentRow = null)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            try
            {
                _logger.Info("Synchronizing new streetlight from GIS with OID of: " + newSL.OID.ToString());

                // Check for In Service status
                bool wasPlacedInService = false;
                bool wasAlreadyInService = false;
                _slGis.CDWasPlacedInService(newSL as IFeature, parentRow, out wasPlacedInService, out wasAlreadyInService);

                // Check for valid SPID assignment
                bool wasAssignedSPID = false;
                bool wasAlreadyAssignedSPID = false;
                _slGis.CDWasAssignedSPID(newSL as IFeature, parentRow, out wasAssignedSPID, out wasAlreadyAssignedSPID);

                // Check for existing GIS ID value, if it has one... it must have come from INV table
                bool hasGisId = _slGis.HasGisID(newSL as IFeature);

                // If it was placed in service and was assigned SPID.. or one of those things happened and the other was already true
                if ((wasPlacedInService == true && wasAssignedSPID == true && hasGisId == false) ||
                    (wasPlacedInService == true && wasAssignedSPID == true && hasGisId == true && parentRow != null) ||
                   (wasPlacedInService == true && wasAlreadyAssignedSPID == true) ||
                   (wasAssignedSPID == true && wasAlreadyInService))
                {

                    //ME Q3 ITEM 2018: DATA FROM GIS WILL BE SENT AS UPDATE ONLY IF DATA EXISTS IN SLCDX DATA
                    bool IsSPIDInSlcdx;
                    _slGis.IsSPIDInSLCDXData(newSL as IFeature, out IsSPIDInSlcdx);
                    if (IsSPIDInSlcdx)
                    {

                        // Ensure lookup tables are added
                        IDictionary<string, string> ccbItemTypeCodeLookup = _slCcb.GetLookupTableValues("CCBTOGIS_ITEMTYPECODE_LOOKUP");
                        _slGis.AddToDomainMap("CCBTOGIS_ITEMTYPECODE_LOOKUP", ccbItemTypeCodeLookup);
                        IDictionary<string, string> ccbOpScheduleCodeLookup = _slCcb.GetLookupTableValues("CCBTOGIS_OPSCHEDULE_LOOKUP");
                        _slGis.AddToDomainMap("CCBTOGIS_OPSCHEDULE_LOOKUP", ccbOpScheduleCodeLookup);

                        if (hasGisId == false)
                        {
                            // Update the new SL with a GIS ID
                            string newGisID = _slCcb.GetNextGisID();
                            if (newGisID != string.Empty)
                            {
                                _logger.Info("Updating SL: " + newSL.OID.ToString() + " with GISID of " + newGisID);
                                _slGis.UpdateGisID(newSL, newGisID);
                            }

                            // Submit the new SL to the Inventory table
                            _logger.Info("Inserting SL:" + newSL.OID.ToString() + " into Inventory table");
                            IList<string> invValues = _slGis.GetValues(newSL, StreetlightGis.INV_FIELDS, StreetlightGis.FetchType.FetchTypeInv);
                            _slInv.Insert((newSL as IFeature).ShapeCopy, invValues);

                            // Submit the new SL to the FieldPts table
                            _logger.Info("Inserting SL:" + newSL.OID.ToString() + " into FieldPoints table");
                            IList<string> fieldPtsValues = _slGis.GetValues(newSL, StreetlightGis.FIELDPTS_FIELDS, StreetlightGis.FetchType.FetchTypeFieldPts);
                            _slFieldPoints.Insert((newSL as IFeature).ShapeCopy, fieldPtsValues);
                        }

                        // Submit the new SL to CCB
                        _logger.Info("Sending SL: " + newSL.OID.ToString() + " to CCB");
                        //IList<string> ccbValues = _slGis.GetValues(newSL, StreetlightGis.CCB_EXPORT_FIELDS, StreetlightGis.FetchType.FetchTypeCcb);
                        IList<string> ccbValues = _slGis.GetValues(newSL, StreetlightGis.CCB_EXPORT_FIELDS_FIELDPTS, StreetlightGis.FetchType.FetchTypeCcb);
                        _slCcb.WriteCCB(ccbValues);
                    }
                    else
                    {
                        _logger.Info("This SL: " + newSL.OID.ToString() + " has not been sent to CCB as SPID not found in SLCDX_DATA");
                    }

                }
                else
                {
                    if (wasPlacedInService == true && wasAssignedSPID == true && hasGisId == true)
                    {
                        _logger.Info("New In Service/non-zero SPID streetlight entered with GIS ID. Assuming came from Inventory and ignoring");
                    }
                    else
                    {
                        _logger.Info("Streetlight is not In Service and/or has no SP ID so not sending to Inventory or CCB");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to synchronize streetlight add: " + ex.ToString());
            }
        }

        public void SyncGISDelete(IRow deletedSL)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            try
            {
                _logger.Info("Synchronizing deleted streetlight from GIS with OID of: " + deletedSL.OID.ToString());

                // Mark the streetlight as inactive in the inventory table
                _slInv.Delete(deletedSL);
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to synchronize streetlight delete: " + ex.ToString());
            }
        }

        public void CDSyncGISDelete(DeleteFeat deletedSL)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            try
            {
                _logger.Info("Synchronizing deleted streetlight from GIS with OID of: " + deletedSL.OID.ToString());

                // Mark the streetlight as inactive in the inventory table
                _slInv.CDDelete(deletedSL);
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to synchronize streetlight delete: " + ex.ToString());
            }
        }

        #endregion

        #region Public ETL methods

        public void SyncCCBEdits()
        {
            // Log entry
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            try
            {
                // Notify GIS of settings
                bool sendToSAP = true;
                if (GetSetting("sendToSAP") != string.Empty)
                {
                    bool.TryParse(GetSetting("sendToSAP"), out sendToSAP);
                }

                // Instruct the CCB class to update the GIS
                _slCcb.UpdateGisFromCcb(sendToSAP);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SyncInvEdits()
        {
            // Log entry
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            
            try
            {
                // Notify GIS of settings
                if (GetSetting("reportPath") != string.Empty)
                {
                    _slGis.ReportPath = GetSetting("reportPath");
                }
                int commitInterval = 100;
                if (GetSetting("commitInterval") != string.Empty)
                {
                    commitInterval = int.Parse(GetSetting("commitInterval"));
                }
                _logger.Info("Commit Interval is: " + commitInterval.ToString());
                string whereClause = string.Empty;
                if (GetSetting("whereClause") != string.Empty)
                {
                    whereClause = GetSetting("whereClause");
                    _logger.Info("Running with Where clause of: " + whereClause);
                }

                // Create some log files
                _slGis.CreateSuspectSlFile();
                _slGis.CreateBadDomainValueFile();

                // Ensure lookup tables are added
                IDictionary<string, string> ccbItemTypeCodeLookup = _slCcb.GetLookupTableValues("CCBTOGIS_ITEMTYPECODE_LOOKUP");
                _slGis.AddToDomainMap("Street Light Item Type", ccbItemTypeCodeLookup);
                IDictionary<string, string> ccbOpScheduleCodeLookup = _slCcb.GetLookupTableValues("CCBTOGIS_OPSCHEDULE_LOOKUP");
                _slGis.AddToDomainMap("Street Light Operating Schedule", ccbOpScheduleCodeLookup);

                // Disable model name lookup on the local office AU and force the map number AU to fire
                PopulateLocalOfficeAU.LookupLocalOfficeByModelName = false;
                PopulateDistMapNoAU.AllowExecutionOutsideOfArcMap = true;
                InheritRegionalAttributesAU.AllowExecutionOutsideOfArcMap(_landbaseWorkspace.Workspace);

                // For each new streetlight in the staging table...
                _logger.Info("Processing INV staging table adds into GIS...");
                int counter = 0;
                IFeatureCursor stagingAdds = _slInvStage.FetchAdds(whereClause);
                IFeature stagingAdd = stagingAdds.NextFeature();
                while (stagingAdd != null)
                {
                    counter++;

                    try
                    {
                        // Add it to the GIS
                        _slGis.AddSL(stagingAdd);

                        // Delete it from the staging table
                        stagingAdds.DeleteFeature();
                    }
                    catch (Exception ex)
                    {
                        // Log and keep on ticking
                        _logger.Error("Failed to add streetlight: " + ex.ToString());
                        // Ignore certain types of error
                        //bool bContinue = false;
                        //if (ex.Message.Contains("Zero-length polylines not allowed") == true)
                        //{
                        //    bContinue = true;
                        //}
                        //if (bContinue == false)
                        //{
                        //    throw ex;
                        //}
                    }

                    if (counter % 10 == 0)
                    {
                        _logger.Info("Processed " + counter.ToString() + " streetlight adds...");

                        // Save every once in a while in case things bomb
                        if (counter % commitInterval == 0)
                        {
                            _logger.Info("Commit interval hit, checking for edits...");
                            IWorkspaceEdit wsEdit = _workspace as IWorkspaceEdit;
                            bool hasEdits = false;
                            wsEdit.HasEdits(ref hasEdits);
                            if (hasEdits == true)
                            {
                                _logger.Info("Edits found. Saving...");
                                wsEdit.StopEditing(true);
                                wsEdit.StartEditing(false);
                                if (((IWorkspaceEdit)_nonVersionedWorkspace).IsBeingEdited())
                                {
                                    ((IWorkspaceEdit)_nonVersionedWorkspace).StopEditing(true);
                                    IMultiuserWorkspaceEdit muWorkspaceEdit = _nonVersionedWorkspace as IMultiuserWorkspaceEdit;
                                    muWorkspaceEdit.StartMultiuserEditing(esriMultiuserEditSessionMode.esriMESMNonVersioned);
                                }
                                _logger.Info("Save complete. Re-fetching staging records...");
                                if (stagingAdds != null) Marshal.FinalReleaseComObject(stagingAdds);
                                stagingAdds = _slInvStage.FetchAdds(whereClause);
                                _logger.Info("Fetch complete. Resuming Processing...");
                            }
                            else
                            {
                                _logger.Info("No edits found. Resuming Processing...");
                            }
                        }
                    }

                    // Get the next one
                    stagingAdd = stagingAdds.NextFeature();
                }
                _logger.Info("Adds complete. Saving...");
                ((IWorkspaceEdit)_minerSession.Workspace).StopEditing(true);
                ((IWorkspaceEdit)_minerSession.Workspace).StartEditing(false);
                if (((IWorkspaceEdit)_nonVersionedWorkspace).IsBeingEdited())
                {
                    ((IWorkspaceEdit)_nonVersionedWorkspace).StopEditing(true);
                    IMultiuserWorkspaceEdit muWorkspaceEdit = _nonVersionedWorkspace as IMultiuserWorkspaceEdit;
                    muWorkspaceEdit.StartMultiuserEditing(esriMultiuserEditSessionMode.esriMESMNonVersioned);
                }
                _logger.Info("Save complete");

                // For each deleted streetlight in the staging table...
                _logger.Info("Processing INV staging table deletes into GIS...");
                counter = 0;
                IFeatureCursor stagingDeletes = _slInvStage.FetchDeletes();
                IFeature stagingDelete = stagingDeletes.NextFeature();
                while (stagingDelete != null)
                {
                    counter++;
                    if (counter % 10 == 0)
                    {
                        _logger.Info("Processed " + counter.ToString() + " streetlight deletes...");
                    }

                    // Add it to the GIS
                    _slGis.DeleteSL(stagingDelete);
                    
                    // Delete it from the staging table
                    //_slInvStage.Delete(stagingDelete);
                    stagingDeletes.DeleteFeature();

                    // Get the next one
                    stagingDelete = stagingDeletes.NextFeature();
                }

                _logger.Info("Deletes complete. Saving...");
                ((IWorkspaceEdit)_minerSession.Workspace).StopEditing(true);
                ((IWorkspaceEdit)_nonVersionedWorkspace).StopEditing(true);
                _logger.Info("Save complete");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Conflate()
        {
            // Log entry
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            IFeatureCursor slInvCur = null;
            bool postOnComplete = true;
            bool copyUnmatchedToStaging = true;

            try
            {
                // Notify GIS of settings
                if (GetSetting("distanceThreshold") != string.Empty)
                {
                    _slGis.MisplacedSlDistanceThreshold = double.Parse(GetSetting("distanceThreshold"));
                }
                if (GetSetting("reportPath") != string.Empty)
                {
                    _slGis.ReportPath = GetSetting("reportPath");
                }
                int startObjectId = 1;
                if (GetSetting("startObjectId") != string.Empty)
                {
                    startObjectId = int.Parse(GetSetting("startObjectId"));
                }
                int commitInterval = 100;
                if (GetSetting("commitInterval") != string.Empty)
                {
                    commitInterval = int.Parse(GetSetting("commitInterval"));
                }
                int rowsToProcess = 999999999;
                if (GetSetting("rowsToProcess") != string.Empty)
                {
                    rowsToProcess = int.Parse(GetSetting("rowsToProcess"));
                }
                string whereClause = string.Empty;
                if (GetSetting("whereClause") != string.Empty)
                {
                    whereClause = GetSetting("whereClause");
                    _logger.Info("Running with Where clause of: " + whereClause);
                }
                if (GetSetting("copyUnmatchedToStaging") != string.Empty)
                {
                    bool.TryParse(GetSetting("copyUnmatchedToStaging"), out copyUnmatchedToStaging);
                }
                if (copyUnmatchedToStaging == true)
                {
                    _logger.Info("Unmatched rows will be placed onto the staging table");
                }
                else
                {
                    _logger.Info("Unmatched rows will NOT be placed onto the staging table");
                }
                if (GetSetting("submitToPostOnCompletion") != string.Empty)
                {
                    bool.TryParse(GetSetting("submitToPostOnCompletion"), out postOnComplete);
                }

                // Delete invalid streetlights from the GIS
                if (GetSetting("reportInvalidSL").ToUpper() == "TRUE")
                {
                    bool delete = GetSetting("deleteInvalidSL").ToUpper() == "TRUE";
                    _slGis.DetectInvalid(delete);
                    if (delete == true)
                    {
                        _logger.Info("Deleted invalid SL's. Saving...");
                        ((IWorkspaceEdit)_minerSession.Workspace).StopEditing(true);
                        ((IWorkspaceEdit)_minerSession.Workspace).StartEditing(false);
                        _logger.Info("Save complete");
                    }

                }
                _slGis.CreateMisplacedSlFile();
                _slGis.CreateCannotConflateFile();

                // Get all the existing rows from the inv table
                int currFeatureCount = 0;
                IQueryFilter filter = new QueryFilterClass();
                int featureCount = _slInv.StreetlightInvFc.FeatureCount(filter);
                slInvCur = _slInv.Fetch(startObjectId, whereClause);
                IFeature slInvFeature = slInvCur.NextFeature();
                while (slInvFeature != null && currFeatureCount < rowsToProcess)
                {
                    // Match on GIS_ID first
                    bool match = _slGis.ConflateByGisId(slInvFeature, StreetlightGis.INV_FIELD_GISID, StreetlightGis.GIS_FIELD_GISID, true, true);

                    // Otherwise try SP_ID
                    if (match == false)
                    {
                        match = _slGis.ConflateBySpId(slInvFeature, StreetlightGis.INV_FIELD_SPID, StreetlightGis.GIS_FIELD_SPID, true, true);
                    }

                    // Otherwise try combo of badge number, local office and fixture code
                    if (match == false)
                    {
                        match = _slGis.ConflateByBadge(slInvFeature);
                    }

                    // Otherwise write row to the streetlight_stage table as a 'new' row
                    if (match == false)
                    {
                        if (copyUnmatchedToStaging == true)
                        {
                            _slInvStage.Insert(slInvFeature);
                        }
                        else
                        {
                            _logger.Info("No match found for OID: " + slInvFeature.OID.ToString());
                        }
                    }

                    // Lets keep track for the sake of sanity (and twitchy ctrl-c types)
                    currFeatureCount++;
                    if (currFeatureCount % 10 == 0)
                    {
                        // Write something suitable somewhere
                        string progress = "Processed " + currFeatureCount.ToString() + " features "; // of " + featureCount.ToString() + ".";
                        _logger.Info(progress);

                        // Save every once in a while in case things bomb
                        if (currFeatureCount % commitInterval == 0)
                        {
                            _logger.Info("Commit interval hit, checking for edits...");
                            IWorkspaceEdit wsEdit = _workspace as IWorkspaceEdit;
                            bool hasEdits = false;
                            wsEdit.HasEdits(ref hasEdits);
                            if (hasEdits == true)
                            {
                                _logger.Info("Edits found. Saving...");
                                int lastObjId = slInvFeature.OID;
                                wsEdit.StopEditing(true);
                                wsEdit.StartEditing(false);
                                if (((IWorkspaceEdit)_nonVersionedWorkspace).IsBeingEdited())
                                {
                                    ((IWorkspaceEdit)_nonVersionedWorkspace).StopEditing(true);
                                    IMultiuserWorkspaceEdit muWorkspaceEdit = _nonVersionedWorkspace as IMultiuserWorkspaceEdit;
                                    muWorkspaceEdit.StartMultiuserEditing(esriMultiuserEditSessionMode.esriMESMNonVersioned);
                                }
                                _logger.Info("Save complete. Re-fetching inventory records...");
                                if (slInvCur != null) Marshal.FinalReleaseComObject(slInvCur);
                                slInvCur = _slInv.Fetch(lastObjId + 1, whereClause);
                                _logger.Info("Fetch complete. Resuming Processing...");
                            }
                            else
                            {
                                _logger.Info("No edits found. Resuming Processing...");
                            }
                        }
                    }

                    // Get the next feature
                    slInvFeature = slInvCur.NextFeature();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (slInvCur != null) Marshal.FinalReleaseComObject(slInvCur);
            }

            return postOnComplete;
        }

        #endregion

        #region Common public methods

        public void CreateWorkspace(string versionName)
        {
            // Log entry
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            // Create a session
            _minerSession.CreateMMSessionVersion(versionName);
            _logger.Info("Using session: " + _minerSession.SessionName + " in version: " + _minerSession.Version.VersionName);
            _workspace = _minerSession.Workspace;
            ((IWorkspaceEdit)_workspace).StartEditing(false);

            // Edit the non-versioned, er, version
            _nonVersionedWorkspace = _sdeWorkspace.NonVersionedEditsWorkspace;
            IMultiuserWorkspaceEdit muWorkspaceEdit = _nonVersionedWorkspace as IMultiuserWorkspaceEdit;

            // Make sure that non-versioned editing is supported. If not, throw an exception.
            if (!muWorkspaceEdit.SupportsMultiuserEditSessionMode
                (esriMultiuserEditSessionMode.esriMESMNonVersioned))
            {
                throw new ArgumentException(
                    "The workspace does not support non-versioned editing.");
            }            
            muWorkspaceEdit.StartMultiuserEditing(esriMultiuserEditSessionMode.esriMESMNonVersioned);

            // Create child classes
            _slGis = new StreetlightGis(_workspace, _nonVersionedWorkspace);
            _slInv = new StreetlightInv(_nonVersionedWorkspace);
            _slInvStage = new StreetlightInvStage(_nonVersionedWorkspace);
            _slFieldPoints = new StreetlightFieldPts(_nonVersionedWorkspace);
        }

        public void PostWorkspace(bool post = true)
        {
            // Log entry
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            try
            {
                // Save and post versioned edits
                _logger.Info("Saving Outstanding Edits...");
                if (((IWorkspaceEdit)_workspace).IsBeingEdited())
                {
                    ((IWorkspaceEdit)_workspace).StopEditing(true);
                }

                if (post == true)
                {
                    _logger.Info("Submitting session to post queue...");
                    const int POSTING_STATE_POST_QUEUE = 5;
                    _minerSession.SubmitDirectly_To_Post_InGDBM(POSTING_STATE_POST_QUEUE);
                }

                // Save and post non-versioned edits
                if (((IWorkspaceEdit)_nonVersionedWorkspace).IsBeingEdited())
                {
                    ((IWorkspaceEdit)_nonVersionedWorkspace).StopEditing(true);
                }
            }
            catch (Exception ex)
            {
                // Rollback
                _logger.Info("Error saving or submitting: " + ex.ToString());
                if (((IWorkspaceEdit)_workspace).IsBeingEdited())
                {
                    ((IWorkspaceEdit)_workspace).StopEditing(false);
                }
                if (((IWorkspaceEdit)_nonVersionedWorkspace).IsBeingEdited())
                {
                    ((IWorkspaceEdit)_nonVersionedWorkspace).StopEditing(false);
                }

                // Throw error
                throw ex;
            }
        }

        #endregion
  
        #region Common private methods

        private void InitConfig(string configFile)
        {
            try
            {
                ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
                fileMap.ExeConfigFilename = configFile;
                _config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            }
            catch (Exception ex)
            {
                _logger.Error("Streetlight Synchronizer unable to load settings file: " + ex.ToString());
                throw;
            }
        }

        private string GetSetting(string settingName, string defaultValue = "")
        {
            // Log entry
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            // Assume we'll use default value
            string settingValue = defaultValue;

            // Get settings value
            if (_config.AppSettings.Settings[settingName] != null)
            {
                settingValue = _config.AppSettings.Settings[settingName].Value;
            }

            // Return the result
            return settingValue;
        }


        #endregion
    }
}
