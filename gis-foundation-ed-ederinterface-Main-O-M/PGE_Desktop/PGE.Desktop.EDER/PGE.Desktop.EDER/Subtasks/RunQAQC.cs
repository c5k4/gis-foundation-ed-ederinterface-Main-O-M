using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using PGE.Common.Delivery.Process.BaseClasses;
using Miner.Interop.Process;
using System.Runtime.InteropServices;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using Miner.Geodatabase.Edit;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;
using log4net;
using System.Reflection;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;
using System.Collections;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.GeoDatabaseDistributed;
using PGE.Desktop.EDER.ValidationRules;

namespace PGE.Desktop.EDER.Subtasks
{
    /// <summary>
    /// Subtask that runs the QA/QC of the version difference reported by the MMVersioningUtils.GetAllDifferencesInSource. 
    /// This runs QA only on features that are edited in this version.
    /// </summary>
    [ComVisible(true)]
    [Guid("80C70F56-F4BA-4332-A2A5-15D7484524BD")]
    [ProgId("PGE.Desktop.EDER.RunQAQC")]
    [ComponentCategory(ComCategory.MMPxSubtasks)]
    public class RunQAQC : BasePxSubtask
    {
        //private static Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        //private const string _severitiesToFailPost = "SEVERITIESTOFAILPOST";
        //private const string _runMaxDeleteCheckString = "CheckMaxDeletes";
        private IApplication _app;
        private IMMSessionManager3 _sessionMgrExt;

        /// <summary>
        /// Constructor
        /// </summary>
        public RunQAQC()
            : base("PGE Run QA/QC")
        {

        }

        //ArcMap application handle 
        protected IApplication Application
        {
            get
            {
                if (_app == null) _app = ApplicationFacade.Application;
                return _app;
            }
        }

        /// <summary>
        /// Initializes the subtask
        /// </summary>
        /// <param name="pPxApp">The IMMPxApplication.</param>
        //// <returns><c>true</c> if intialized; otherwise <c>false</c>.</returns>
        public override bool Initialize(IMMPxApplication pPxApp)
        {
            base.Initialize(pPxApp);

            //Check application and session manager
            if (base._PxApp == null) return false;

            _sessionMgrExt = _PxApp.FindPxExtensionByName(BasePxSubtask.SessionManagerExt) as IMMSessionManager3;
            if (_sessionMgrExt == null) return false;

            return true;
        }

        /// <summary>
        /// Gets if the subtask is enabled for the specified px node.
        /// </summary>
        /// <param name="pPxNode">The px node.</param>
        /// <returns><c>true</c> if enabled; otherwise <c>false</c>.</returns>
        protected override bool InternalEnabled(IMMPxNode pPxNode)
        {
            //Verify incoming node is the expected session node type.
            if (pPxNode == null) return false;
            if (pPxNode.NodeType != BasePxSubtask.SessionNodeType) return false;

            //Check if Px is in standalone mode
            if (_PxApp.StandAlone == true) return false;
            if (_sessionMgrExt.CurrentOpenSession == null) return false;

            //Verify that a session is open for editing
            IMMSessionEx sessionEx = _sessionMgrExt.CurrentOpenSession as IMMSessionEx;
            if (sessionEx.get_ViewOnly() == true) return false;

            return true;

        }

        /// <summary>
        /// Executes the Actual QA by obtaining the Version difference from the DesktopVersionDifference Processor.
        /// </summary>
        /// <param name="pPxNode">Session Node</param>
        /// <returns></returns>
        protected override bool InternalExecute(IMMPxNode pPxNode)
        {

            bool retVal = false;
            try
            {
                //Make sure the error form is not already open 
                ValidationEngine.Instance.Application = Application;

                //Fix for INC000003825091 - cannot allow the user to click cancel so 
                //we run our own custom VersionDifference and QAQC 

                //We want inserts and updates only for QAQC but we need deletes 
                //also for the maximum deletes check 
                esriDifferenceType[] pDiffTypes = new esriDifferenceType[6];
                pDiffTypes[0] = esriDifferenceType.esriDifferenceTypeInsert;
                pDiffTypes[1] = esriDifferenceType.esriDifferenceTypeUpdateDelete;
                pDiffTypes[2] = esriDifferenceType.esriDifferenceTypeUpdateNoChange;
                pDiffTypes[3] = esriDifferenceType.esriDifferenceTypeUpdateUpdate;
                pDiffTypes[4] = esriDifferenceType.esriDifferenceTypeDeleteNoChange;
                pDiffTypes[5] = esriDifferenceType.esriDifferenceTypeDeleteUpdate;

                ValidationEngine.Instance.Application = Application;
                Hashtable hshVersionDiffObjects = ValidationEngine.Instance.
                    GetVersionDifferences(
                        (IVersionedWorkspace)Editor.EditWorkspace,
                        pDiffTypes, false);

                _logger.Debug("Version Difference from 'DesktopVersionDifference' class successful");
                if (hshVersionDiffObjects.Count != 0)
                {
                    //Run the QAQC 
                    ValidationEngine.Instance.FilterType =
                            ValidationFilterType.valFilterTypeErrorOnly;
                    ValidationEngine.Instance.SelectionType =
                        ValidationSelectionType.ValidationSelectionTypeVersionDiff;

                    //******************************************************************
                    //Change for INC000003804430  
                    //MAX DELETE CHECK FOR SESSION 

                    //Perform the Maximum Delete Check to ensure the user cannot delete 
                    //huge amount of data 
                    //1. Check if the current task is 'Post Session' to determine whether check 
                    //   is run. If we are running the check complete the following steps.  
                    //2. Lookup the max deletes thresholds setting in the PGE_APP_SETTINGS table 
                    //3. Count the number of deletes in the version using the versionDifference 
                    //   ID8List 
                    //5. If the num deletes in version > max deletes threshold then give appropriate 
                    //   message and return false from subtask 
                    //6. If the num deletes in version > max deletes reporting threshold then record 
                    //   the delete in the PGE_SESSION_DELETES_LOG table 
                    //7. If the user has role PGE_BULK_DELETE_ADMIN allow the session after gaining 
                    //   confirmation of the desire to continue 

                    //If we are inside the 'Post Session' task then execute the mas deletes check 
                    IMMPxTask pxExecutingTask = base.ExecutingTask;
                    if (pxExecutingTask.Name == "Post Session")
                    {
                        _logger.Debug("Running Max Deletes check on the Version Difference");
                        //Set the threshold to default values initially
                        int deleteThresholdTotal = 1000;
                        int deleteThresholdForDataset = 100;
                        int deleteReportingThreshold = 200;
                        bool userHasMaxDeletesRole = UserHasMaxDeletesRole();
                        bool thresholdsExceeded = false;

                        //Count the number of deletes (excludes deletes of related anno) 
                        PopulateMaxDeleteThresholds(
                            Editor.EditWorkspace,
                            ref deleteThresholdTotal,
                            ref deleteThresholdForDataset,
                            ref deleteReportingThreshold);

                        Hashtable hshDeleteCount = new Hashtable();
                        CountDeletes(hshVersionDiffObjects, ref hshDeleteCount);

                        //Check for exceeding combined max deletes threshold 
                        int totalDeletes = 0;
                        foreach (Hashtable hshFC in hshDeleteCount.Values)
                        {
                            totalDeletes += hshFC.Count;
                        }
                        if (totalDeletes > deleteThresholdTotal)
                        {
                            if (!userHasMaxDeletesRole)
                            {
                                MessageBox.Show("You have deleted a total of: " + totalDeletes.ToString() +
                                    " records, which has exceeded the threshold of: " +
                                    deleteThresholdTotal.ToString() +
                                    ". To post this session successfully talk to your administrator.",
                                    "Maximum Total Session Deletes Exceeded",
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return false;
                            }
                            else
                                thresholdsExceeded = true;
                        }

                        //Check for exceeding individual objectclass threshold 
                        foreach (string dsName in hshDeleteCount.Keys)
                        {
                            Hashtable hshFC = (Hashtable)hshDeleteCount[dsName];
                            if (hshFC.Count > deleteThresholdForDataset)
                            {
                                if (!userHasMaxDeletesRole)
                                {
                                    MessageBox.Show("You have deleted : " + hshFC.Count.ToString() +
                                        " records in: " + dsName + ", which has exceeded the threshold of: " +
                                        deleteThresholdForDataset.ToString() +
                                        ". To post this session successfully talk to your administrator.",
                                        "Maximum Session Deletes for Individual Dataset",
                                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    return false;
                                }
                                else
                                {
                                    thresholdsExceeded = true;
                                    break;
                                }
                            }
                        }

                        //Give the bulk delete admin the opportunity to cancel 
                        if (userHasMaxDeletesRole && thresholdsExceeded)
                        {
                            DialogResult dr = MessageBox.Show(
                                "One or more of the Maximum Delete Thresholds were exceeded. Are you sure you wish to continue?",
                                "Maximum Thresholds Exceeded",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                            if (dr != DialogResult.Yes)
                                return false;
                        }

                        //Where there are more than the configured threshold of deletes
                        //in the session, log the deletes to a table 
                        if (totalDeletes > deleteReportingThreshold)
                            LogSessionDeletes(hshDeleteCount);

                    }
                    //END MAX DELETE CHECK FOR SESSION 
                    //******************************************************************                     
                }

                //Store the validation results for the use in the next subtask 
                //then clear them from memory when finished (after the Device Group 
                //subtask) 
                ValidationEngine.Instance.VersionDifferences = hshVersionDiffObjects;

                //Return true for the task 
                retVal = true;
            }
            catch (Exception ex)
            {
                _logger.Error("Failed processing the subtask:" + this._Name, ex);
            }
            finally
            {
                //Reset the QAQC filter back to valFilterTypeAll 
                ValidationEngine.Instance.FilterType = ValidationFilterType.valFilterTypeAll;
            }
            return retVal;
        }

        private bool UserHasMaxDeletesRole()
        {
            try
            {
                bool userHasRole = false;
                IMMPxIntegrationCache intcache = (IMMPxIntegrationCache)Application.
                    FindExtensionByName("Session Manager Integration Extension");
                IMMPxApplication pxApp = intcache.Application;
                IMMPxUser pPxUser = pxApp.User;

                //Loop through the roles for the user 
                IMMEnumPxRole pRoles = pPxUser.Roles;
                string role = pRoles.Next();
                while (!(role == null))
                {
                    if (role == "PGE_BULK_DELETE_ADMIN")
                    {
                        userHasRole = true;
                        break;
                    }
                    role = pRoles.Next();
                }

                return userHasRole;
            }
            catch (Exception ex)
            {
                throw new Exception("Error checking is user has max deletes role");
            }
        }


        /// <summary>
        /// Logs the count of deletes made by the user in the session both 
        /// for the individual dataset and for the entire session 
        /// </summary>
        /// <param name="hshDeletes"></param>
        private void LogSessionDeletes(Hashtable hshDeletes)
        {
            try
            {
                string maxDatasetDelName = "";
                int maxDatasetDelCount = 0;
                int datasetDelCount = 0;
                int sessionDelCount = 0;

                //Count up the deletes and find the dataset with the 
                //most deletes 
                foreach (string fc in hshDeletes.Keys)
                {
                    Hashtable hshFC = (Hashtable)hshDeletes[fc];
                    datasetDelCount = hshFC.Count;
                    sessionDelCount += datasetDelCount;
                    if (datasetDelCount > maxDatasetDelCount)
                    {
                        maxDatasetDelCount = datasetDelCount;
                        maxDatasetDelName = fc;
                    }
                }

                //Open the PGE_SESSION_DELETES_LOG table 
                ITable pSessionDeletesTable = (ITable)ModelNameFacade.ObjectClassByModelName(Editor.EditWorkspace,
                    SchemaInfo.Electric.ClassModelNames.PgeSessionDeletesLog);

                //Get the logged in user 
                IMMPxUser pUser = base._PxApp.User;
                IVersion pVersion = (IVersion)Editor.EditWorkspace;

                //Strip the sessionId from the version name  
                int sessionId = StripSessionIdFromVersionName(pVersion.VersionName);

                //Update the record 
                int fldIdxUserName = pSessionDeletesTable.Fields.FindField("USER_NAME");
                int fldIdxDateTime = pSessionDeletesTable.Fields.FindField("DATE_TIME");
                int fldIdxSessionId = pSessionDeletesTable.Fields.FindField("SESSION_ID");
                int fldIdxVersionName = pSessionDeletesTable.Fields.FindField("VERSION_NAME");
                int fldIdxSessionDelCount = pSessionDeletesTable.Fields.FindField("SESSION_DEL_COUNT");
                int fldIdxDatasetDelCount = pSessionDeletesTable.Fields.FindField("DATASET_DEL_COUNT");
                int fldIdxDatasetName = pSessionDeletesTable.Fields.FindField("DATASET_NAME");

                IRow pNewRow = pSessionDeletesTable.CreateRow();
                pNewRow.set_Value(fldIdxUserName, pUser.Name);
                pNewRow.set_Value(fldIdxDateTime, DateTime.Now);
                pNewRow.set_Value(fldIdxSessionId, sessionId);
                pNewRow.set_Value(fldIdxVersionName, pVersion.VersionName);
                pNewRow.set_Value(fldIdxSessionDelCount, sessionDelCount);
                pNewRow.set_Value(fldIdxDatasetDelCount, datasetDelCount);
                pNewRow.set_Value(fldIdxDatasetName, maxDatasetDelName);
                pNewRow.Store();

            }
            catch
            {
                throw new Exception("Error logging session deletes");
            }
        }

        /// <summary>
        /// Loads max delete configuration settings 
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <returns></returns>
        private void PopulateMaxDeleteThresholds(
            IWorkspace pWS,
            ref int maxDeleteThresholdTotal,
            ref int maxDeleteThresholdForDataset,
            ref int maxDeleteReportingThreshold)
        {
            try
            {
                ITable pAppSettingsTable = (ITable)ModelNameFacade.ObjectClassByModelName(pWS,
                    SchemaInfo.Electric.ClassModelNames.PgeAppsettings);
                int fldIdxName = pAppSettingsTable.Fields.FindField("SETTING_NAME");
                int fldIdxValue = pAppSettingsTable.Fields.FindField("SETTING_VALUE");
                _logger.Debug("Querying the PGE_APP_SETTINGS for severities");
                ICursor pCursor = pAppSettingsTable.Search(null, false);
                string settingName = "";
                string settingValue = "";
                IRow pRow = null;

                //check if received cursor is not empty
                while ((pRow = pCursor.NextRow()) != null)
                {
                    if (fldIdxName != -1)
                        settingName = pRow.get_Value(fldIdxName).ToString().ToLower();
                    if (fldIdxValue != -1)
                        settingValue = pRow.get_Value(fldIdxValue).ToString().ToLower();

                    //Populate the settings 
                    if (settingName.ToLower() == "MaxDeleteThresholdTotal".ToLower())
                        Int32.TryParse(settingValue, out maxDeleteThresholdTotal);
                    else if (settingName.ToLower() == "MaxDeleteThresholdForDataset".ToLower())
                        Int32.TryParse(settingValue, out maxDeleteThresholdForDataset);
                    else if (settingName.ToLower() == "MaxDeleteReportingThreshold".ToLower())
                        Int32.TryParse(settingValue, out maxDeleteReportingThreshold);
                }
                Marshal.FinalReleaseComObject(pCursor);
            }
            catch (Exception ex)
            {
                _logger.Debug("entering error handler for PopulateMaxDeleteThresholds: " + ex.Message);
                throw new Exception("Error populating max delete thresholds");
            }
        }

        //private void CountDeletes(ID8List versionDeletes, ref Hashtable hshDeleteCount)
        //{
        //    try
        //    {
        //        //Print out the deletes 
        //        bool reachedFeatureLevel = false;
        //        versionDeletes.Reset();
        //        ID8ListItem pListItem = versionDeletes.Next();

        //        while (pListItem != null)
        //        {
        //            //System.Diagnostics.Debug.Print("DisplayName:" + pListItem.DisplayName);
        //            if ((pListItem.ItemType == mmd8ItemType.mmd8itFeature) ||
        //                (pListItem.ItemType == mmd8ItemType.mmitAttrRelationshipInstance))
        //                reachedFeatureLevel = true;
        //            else
        //                reachedFeatureLevel = false;

        //            if (reachedFeatureLevel)
        //            {
        //                ID8GeoAssoc geo = (ID8GeoAssoc)pListItem;
        //                if (geo.AssociatedGeoRow is ESRI.ArcGIS.Geodatabase.IObject)
        //                {

        //                    IObject obj = (ESRI.ArcGIS.Geodatabase.IObject)geo.AssociatedGeoRow;
        //                    IDataset pDS = (ESRI.ArcGIS.Geodatabase.IDataset)obj.Class;

        //                    //Add the dataset name 
        //                    if (!hshDeleteCount.ContainsKey(pDS.Name))
        //                        hshDeleteCount.Add(pDS.Name, new Hashtable()); 

        //                    //Add the oid 
        //                    Hashtable hshOIds = (Hashtable)hshDeleteCount[pDS.Name];
        //                    if (!hshOIds.ContainsKey(obj.OID))
        //                        hshOIds.Add(obj.OID, 0);

        //                    //Change 02/13/2014 
        //                    //isAnnoFC = false;
        //                    //if (pDS is IFeatureClass)
        //                    //{
        //                    //    IFeatureClass pFC = (IFeatureClass)pDS;
        //                    //    if (((IFeatureClass)pDS).FeatureType == esriFeatureType.esriFTAnnotation)
        //                    //        isAnnoFC = true;
        //                    //}
        //                    //if (isAnnoFC == false)
        //                    //{
        //                    //    objectIdentifier = pDS.Name + ":" + obj.OID.ToString();
        //                    //    if (!hshDeleteCount.ContainsKey(objectIdentifier))
        //                    //    {
        //                    //        hshDeleteCount.Add(objectIdentifier, 0);
        //                    //        System.Diagnostics.Debug.Print("deleted++:" +
        //                    //            obj.Class.AliasName + ":" + obj.OID.ToString());
        //                    //    }
        //                    //}
        //                }
        //            }

        //            if ((pListItem.DisplayName == "Update, Update") ||
        //                (pListItem.DisplayName == "Update, Delete") ||
        //                (pListItem.DisplayName == "Update, No Change") ||
        //                (pListItem.DisplayName == "Insert"))
        //            {
        //                System.Diagnostics.Debug.Print("Not interested in Updates or Inserts");
        //            }
        //            else if (pListItem is ID8List)
        //            {
        //                if (!reachedFeatureLevel)
        //                {
        //                    //System.Diagnostics.Debug.Print(pListItem.DisplayName); 
        //                    ID8List pChildD8List = (ID8List)pListItem;
        //                    //Recursive Call here 
        //                    CountDeletes(pChildD8List, ref hshDeleteCount);
        //                }
        //            }
        //            pListItem = versionDeletes.Next();
        //        }
        //    }
        //    catch
        //    {
        //        throw new Exception("Error counting session deletes");
        //    }
        //}

        private void CountDeletes(Hashtable hshVersionDiffObjects, ref Hashtable hshDeleteCount)
        {
            try
            {
                foreach (object key in hshVersionDiffObjects.Keys)
                {
                    if (hshVersionDiffObjects[key] is DeletedObject)
                    {
                        DeletedObject pDeletedObj = (DeletedObject)hshVersionDiffObjects[key];

                        //Add the dataset name 
                        if (!hshDeleteCount.ContainsKey(pDeletedObj.DatasetName.ToUpper()))
                            hshDeleteCount.Add(pDeletedObj.DatasetName.ToUpper(), new Hashtable());

                        //Add the oid 
                        Hashtable hshOIds = (Hashtable)hshDeleteCount[pDeletedObj.DatasetName.ToUpper()];
                        if (!hshOIds.ContainsKey(pDeletedObj.OID))
                            hshOIds.Add(pDeletedObj.OID, 0);
                    }
                }
            }
            catch
            {
                throw new Exception("Error counting session deletes");
            }
        }

        private int StripSessionIdFromVersionName(string versionName)
        {
            try
            {
                string sessionIdString = "";
                int sessionId = 0;
                int posOfLastUnderscore = -1;

                posOfLastUnderscore = versionName.LastIndexOf("_");
                if (posOfLastUnderscore != -1)
                {
                    sessionIdString = versionName.Substring(
                        posOfLastUnderscore + 1, (versionName.Length - posOfLastUnderscore) - 1);
                }
                else
                {
                    sessionIdString = versionName;
                }

                //Parse to integer 
                Int32.TryParse(sessionIdString, out sessionId);
                return sessionId;
            }
            catch (Exception ex)
            {
                throw new Exception("Error returning sessionId from version name");
            }
        }


        //private bool FailPost(ID8ListItem qaqcTopLevel)
        //{
        //    bool retVal = true;
        //    string severitiesToFail = GetParameter(_severitiesToFailPost);
        //    //If severities are not configured fail as soon as an error is found
        //    if (string.IsNullOrEmpty(severitiesToFail))
        //    {
        //        return retVal;
        //    }
        //    //Get the Severities configured and split using "," and check if any of the Error message reported has the given severity
        //    else
        //    {
        //        string[] severities = severitiesToFail.Split(",".ToCharArray());
        //        retVal = ProcessD8List(qaqcTopLevel, severities);
        //    }
        //    return retVal;
        //} 

        //private bool ProcessD8List(ID8ListItem d8ListItem,string[] severities)
        //{
        //    bool retVal = false;
        //    if (d8ListItem is IMMValidationError)
        //    {
        //        retVal = severities.Contains(((IMMValidationError)d8ListItem).Severity.ToString());
        //    }
        //    else
        //    {
        //        if (d8ListItem is ID8List)
        //        {
        //            ID8List d8List = (ID8List)d8ListItem;
        //            System.Diagnostics.Debug.Print(d8ListItem.ViewImageID.ToString() + d8ListItem.DisplayName ); 
        //            if (d8List.HasChildren)
        //            {
        //                d8List.Reset();
        //                ID8ListItem listItem = null;
        //                while ((listItem = d8List.Next()) != null)
        //                {
        //                    _logger.Debug("Processing item with display name:" + listItem.DisplayName);
        //                    retVal=ProcessD8List(listItem,severities);
        //                    //Even if one of the errors is found return true and fail.
        //                    if (retVal) break;
        //                }
        //            }
        //        }
        //    }
        //    return retVal;
        //}        
    }
}





