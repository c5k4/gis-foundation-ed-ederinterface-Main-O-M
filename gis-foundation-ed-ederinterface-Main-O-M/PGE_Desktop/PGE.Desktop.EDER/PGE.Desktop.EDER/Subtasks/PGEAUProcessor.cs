using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using PGE.Common.Delivery.Process.BaseClasses;
using Miner.Interop.Process;
using Miner.Geodatabase.Edit;
using ESRI.ArcGIS.Geodatabase;
using System.Text.RegularExpressions;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using Miner.ComCategories;

namespace PGE.Desktop.EDER.Subtasks
{
    [Guid("ebcf0968-b2d2-4041-a62c-67853c5d4b45")]
    [ProgId("PGE.Desktop.EDER.Subtasks.PGEAUProcessor")]
    [ComponentCategory(ComCategory.MMPxSubtasks)]
    public class PGEAUProcessor : BasePxSubtask
    {
        #region Com Reg Methods

        /// <summary>
        /// Registers the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComRegisterFunction]
        private static void Register(string regKey)
        {
            Miner.ComCategories.MMPxSubtasks.Register(regKey);
        }
        /// <summary>
        /// Uns the register.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComUnregisterFunction]
        private static void UnRegister(string regKey)
        {
            Miner.ComCategories.MMPxSubtasks.Unregister(regKey);
        }

        #endregion

        //Contains the object classID mapping to the mmEditEvent mapping for the list of AUs to execute. (i.e. For transformer on create execute AUs x,y, and z)
        private Dictionary<int, Dictionary<mmEditEvent, List<IMMSpecialAUStrategyEx>>> AUDictionaryMapping = null;
        private List<string> _processedList = new List<string>();
        private IMMSessionManager3 _sessionMgrExt;
        private string AUsToExecute = "";

        public PGEAUProcessor() : base("PGE Subtask AU Processor")
        {
            AddParameter("AutoUpdaters", "Specify what AUs to execute with this paramter. Specify the event, AU name, and class ID for each combination to execute" + 
                "Event(Create/Update/Delete):AUName(ProgID of AU):ClassIDs(Comma Separated ClassIDs of object class to execute against);");

            AddExtension(BasePxSubtask.SessionManagerExt);
        }

        /// <summary>
        /// Gets if the subtask is enabled for the specified px node.
        /// </summary>
        /// <param name="pPxNode">The px node.</param>
        /// <returns><c>true</c> if enabled; otherwise <c>false</c>.</returns>
        protected override bool InternalEnabled(IMMPxNode pPxNode)
        {
            //Verify incoming node is the expected session node type.
            if (pPxNode == null || pPxNode.NodeType != BasePxSubtask.SessionNodeType) { return false; }

            return true;

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

        protected override bool InternalExecute(Miner.Interop.Process.IMMPxNode pPxNode)
        {
            #region Parameter Initialization

            if (AUDictionaryMapping == null)
            {
                AUDictionaryMapping = new Dictionary<int, Dictionary<mmEditEvent, List<IMMSpecialAUStrategyEx>>>();
                AUsToExecute = GetParameter("AutoUpdaters").ToUpper();
                string[] AUsToExecuteList = Regex.Split(AUsToExecute, ";");
                foreach (string AU in AUsToExecuteList)
                {
                    string[] AUMappinglist = Regex.Split(AU, ":");
                    if (AUMappinglist[0].ToUpper() == "CREATE")
                    {
                        string[] classIDs = Regex.Split(AUMappinglist[2], ",");
                        foreach (string classIDString in classIDs)
                        {
                            if (string.IsNullOrEmpty(classIDString)) { continue; }
                            int classID = Int32.Parse(classIDString);
                            if (!AUDictionaryMapping.ContainsKey(classID))
                            {
                                Dictionary<mmEditEvent, List<IMMSpecialAUStrategyEx>> mmEditEventClassIDMap = new Dictionary<mmEditEvent, List<IMMSpecialAUStrategyEx>>();
                                mmEditEventClassIDMap.Add(mmEditEvent.mmEventFeatureCreate, new List<IMMSpecialAUStrategyEx>());
                                AUDictionaryMapping.Add(classID, mmEditEventClassIDMap);
                            }
                            else if (!AUDictionaryMapping[classID].ContainsKey(mmEditEvent.mmEventFeatureCreate))
                            {
                                AUDictionaryMapping[classID].Add(mmEditEvent.mmEventFeatureCreate, new List<IMMSpecialAUStrategyEx>());
                            }
                            //Add our AU
                            object au = Activator.CreateInstance(Type.GetTypeFromProgID(AUMappinglist[1]));
                            if (au != null && au is IMMSpecialAUStrategyEx)
                            {
                                AUDictionaryMapping[classID][mmEditEvent.mmEventFeatureCreate].Add(au as IMMSpecialAUStrategyEx);
                            }
                        }
                    }
                    else if (AUMappinglist[0].ToUpper() == "UPDATE")
                    {
                        string[] classIDs = Regex.Split(AUMappinglist[2], ",");
                        foreach (string classIDString in classIDs)
                        {
                            if (string.IsNullOrEmpty(classIDString)) { continue; }
                            int classID = Int32.Parse(classIDString);
                            if (!AUDictionaryMapping.ContainsKey(classID))
                            {
                                Dictionary<mmEditEvent, List<IMMSpecialAUStrategyEx>> mmEditEventClassIDMap = new Dictionary<mmEditEvent, List<IMMSpecialAUStrategyEx>>();
                                mmEditEventClassIDMap.Add(mmEditEvent.mmEventFeatureUpdate, new List<IMMSpecialAUStrategyEx>());
                                AUDictionaryMapping.Add(classID, mmEditEventClassIDMap);
                            }
                            else if (!AUDictionaryMapping[classID].ContainsKey(mmEditEvent.mmEventFeatureUpdate))
                            {
                                AUDictionaryMapping[classID].Add(mmEditEvent.mmEventFeatureUpdate, new List<IMMSpecialAUStrategyEx>());
                            }
                            //Add our AU
                            object au = Activator.CreateInstance(Type.GetTypeFromProgID(AUMappinglist[1]));
                            if (au != null && au is IMMSpecialAUStrategyEx)
                            {
                                AUDictionaryMapping[classID][mmEditEvent.mmEventFeatureUpdate].Add(au as IMMSpecialAUStrategyEx);
                            }
                        }
                    }
                    else if (AUMappinglist[0].ToUpper() == "DELETE")
                    {
                        string[] classIDs = Regex.Split(AUMappinglist[2], ",");
                        foreach (string classIDString in classIDs)
                        {
                            if (string.IsNullOrEmpty(classIDString)) { continue; }
                            int classID = Int32.Parse(classIDString);
                            if (!AUDictionaryMapping.ContainsKey(classID))
                            {
                                Dictionary<mmEditEvent, List<IMMSpecialAUStrategyEx>> mmEditEventClassIDMap = new Dictionary<mmEditEvent, List<IMMSpecialAUStrategyEx>>();
                                mmEditEventClassIDMap.Add(mmEditEvent.mmEventFeatureDelete, new List<IMMSpecialAUStrategyEx>());
                                AUDictionaryMapping.Add(classID, mmEditEventClassIDMap);
                            }
                            else if (!AUDictionaryMapping[classID].ContainsKey(mmEditEvent.mmEventFeatureDelete))
                            {
                                AUDictionaryMapping[classID].Add(mmEditEvent.mmEventFeatureDelete, new List<IMMSpecialAUStrategyEx>());
                            }
                            //Add our AU
                            object au = Activator.CreateInstance(Type.GetTypeFromProgID(AUMappinglist[1]));
                            if (au != null && au is IMMSpecialAUStrategyEx)
                            {
                                AUDictionaryMapping[classID][mmEditEvent.mmEventFeatureDelete].Add(au as IMMSpecialAUStrategyEx);
                            }
                        }
                    }
                }
            }
            #endregion

            if ((IWorkspaceEdit)Editor.EditWorkspace == null || !((IWorkspaceEdit)Editor.EditWorkspace).IsBeingEdited()) { return false; }
            
            _processedList.Clear();
            bool success = true;
            bool startedOperation = false;
            IWorkspaceEdit wkspcEdit = (IWorkspaceEdit)Editor.EditWorkspace;
            try
            {
                DesktopVersionDifference.ResetVersionDifference();
                ID8List versionDifference = DesktopVersionDifference.GetVersionDifference(Editor.EditWorkspace);
                startedOperation = true;
                wkspcEdit.StartEditOperation();
                ProcessD8List(versionDifference, mmEditEvent.mmEventFeatureUpdate);
            }
            catch (Exception e)
            {
                success = false;
                if (startedOperation) { wkspcEdit.AbortEditOperation(); }
                _logger.Error("Error during PGE Subtask AU Processor. Message: " + e.Message);
            }
            finally
            {
                wkspcEdit.StopEditOperation();
            }

            return success;
        }

        /// <summary>
        /// Loops through each element in the Passed in ID8List and if a ID8ListItem of type ID8GeoAssoc is found will get the field value from the model name and add the value to the Value to Persist List.
        /// </summary>
        /// <param name="d8List">Root D8List from version Difference</param>
        /// <param name="onlyFeatures">True will process only IFeature. False will process IFeature and IObject</param>
        private void ProcessD8List(ID8List d8List, mmEditEvent editEvent)
        {
            if (d8List is ID8GeoAssoc)
            {
                if (AlreadyProcessed(d8List))
                {
                    _logger.Debug("List with Display Name:" + ((ID8ListItem)d8List).DisplayName + " already processed. Skipping processing");
                    return;
                }
                else
                {
                    ProcessRow(d8List, editEvent);
                }
            }
            //We need not look at the related record. So Once a ID8GeoAssoc is found we need not loop thru' that.
            //QAQC returns the IObjects in the core list if they are edited. This is to improve Performance.
            else
            {
                if (d8List.HasChildren)
                {
                    d8List.Reset();
                    ID8ListItem listItem = null;
                    while ((listItem = d8List.Next()) != null)
                    {
                        _logger.Debug("Processing item with display name:" + listItem.DisplayName);
                        if (listItem.DisplayName == "Update, Update" || listItem.DisplayName == "Update, Delete" || listItem.DisplayName == "Update, No Change") { editEvent = mmEditEvent.mmEventFeatureUpdate; }
                        else if (listItem.DisplayName == "Insert") { editEvent = mmEditEvent.mmEventFeatureCreate; }
                        ProcessD8List((ID8List)listItem, editEvent);
                    }
                }
            }
        }

        private void ProcessRow(ID8List d8List, mmEditEvent editEvent)
        {
            if (!(d8List is ID8GeoAssoc)) { return; }

            ID8GeoAssoc geoAssoc = (ID8GeoAssoc)d8List;
            IRow row = geoAssoc.AssociatedGeoRow;
            int objectClassID = ((IObjectClass)row.Table).ObjectClassID;
            if (AUDictionaryMapping.ContainsKey(objectClassID))
            {
                Type auType = Type.GetTypeFromProgID("mmGeodatabase.MMAutoUpdater");
                object auObj = Activator.CreateInstance(auType);

                IMMAutoUpdater auFramework = auObj as IMMAutoUpdater;
                mmAutoUpdaterMode currentAUMode = auFramework.AutoUpdaterMode;
                auFramework.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMArcMap;
                if (AUDictionaryMapping[objectClassID].ContainsKey(editEvent))
                {
                    foreach (IMMSpecialAUStrategyEx AU in AUDictionaryMapping[objectClassID][editEvent])
                    {
                        AU.Execute(row as IObject, mmAutoUpdaterMode.mmAUMArcMap, editEvent);
                    }
                }

                auFramework.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;
                
                //Finally store the changes to the row.
                row.Store();

                //Resort our original AU mode
                auFramework.AutoUpdaterMode = currentAUMode;
            }
        }

        /// <summary>
        /// Will check if the passed in ID8GeoAssoc is already processed. If not will add to the Processed List and return False. Else returns true.
        /// </summary>
        /// <param name="d8List">ID8List which is a ID8GeoAssoc</param>
        /// <returns></returns>
        private bool AlreadyProcessed(ID8List d8List)
        {
            //if (_processedList == null) _processedList = new List<string>();
            bool retVal = true;
            //this should never happen but just for safety. This check is already made by the calling method.
            if (!(d8List is ID8GeoAssoc)) return true;
            ID8GeoAssoc geoAssoc = (ID8GeoAssoc)d8List;
            IRow row = geoAssoc.AssociatedGeoRow;
            string recordID = ((IObjectClass)row.Table).ObjectClassID + ":" + row.OID;
            if (!_processedList.Contains(recordID))
            {
                _logger.Debug("IRow with ClassID:RowID::" + recordID + " ready to process");
                _processedList.Add(recordID);
                retVal = false;
            }
            return retVal;
        }

    }
}
