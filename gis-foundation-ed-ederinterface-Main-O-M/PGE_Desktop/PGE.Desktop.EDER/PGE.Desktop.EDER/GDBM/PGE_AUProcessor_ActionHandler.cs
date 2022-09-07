using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.Process.GeodatabaseManager.ActionHandlers;
using Miner.Process.GeodatabaseManager;
using Miner.Geodatabase.GeodatabaseManager;
using Miner.Geodatabase.GeodatabaseManager.Serialization;
using Miner.Interop;
using System.Text.RegularExpressions;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.ArcFM;

namespace PGE.Desktop.EDER.GDBM
{
    public class PGE_AUProcessor_ActionHandler : PxActionHandler
    {
        //Contains the object classID mapping to the mmEditEvent mapping for the list of AUs to execute. (i.e. For transformer on create execute AUs x,y, and z)
        private Dictionary<int, Dictionary<mmEditEvent, List<IMMSpecialAUStrategyEx>>> AUDictionaryMapping = null;
        private List<string> _processedList = new List<string>();

        public PGE_AUProcessor_ActionHandler()
        {
            this.Name = "PGE AU Processor";
            this.Description = "Executes autoupdaters against the version edits as configured. Name: ProgID of the autoupdater to execute.  Value: " + 
                "Comma separated object class IDs for the tables to execute the autoupdater against.  Type: Edit event (Create, Update, Delete)";
        }
        /*
        /// <summary>
        /// Populates an array list of deletable work function values from the configured parameters.
        /// </summary>
        /// <param name="parameters">Parameters set from configuration.</param>
        /// <returns>Array list of deletable work function values.</returns>
        private ArrayList ParseConfiguredWorkFunctionValues(GdbmParameter[] parameters)
        {
            ArrayList workFunctionValues = new ArrayList();
            foreach (GdbmParameter parameter in parameters)
            {
                if (parameter.Name.Equals(WorkFunction, StringComparison.OrdinalIgnoreCase))
                {
                    int workFunction = Convert.ToInt32(parameter.Value);
                    if (!workFunctionValues.Contains(workFunction))
                    {
                        workFunctionValues.Add(workFunction);
                    }
                }
            }
            
            return workFunctionValues;
        }
        */
        protected override bool PxSubExecute(PxActionData actionData, GdbmParameter[] parameters)
        {
            bool success = true;
            try
            {
                if ((new CommonFunctions()).CheckUsage(actionData.Version, this.Log, ServiceConfiguration) == true)
                {
                    return true;
                }

                #region Parameter initialization
                if (AUDictionaryMapping == null)
                {
                    AUDictionaryMapping = new Dictionary<int, Dictionary<mmEditEvent, List<IMMSpecialAUStrategyEx>>>();
                    foreach (GdbmParameter param in parameters)
                    {
                        string AU = param.Name;
                        if (param.Type.ToUpper() == "CREATE")
                        {
                            string[] classIDs = Regex.Split(param.Value, ",");
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
                                object au = Activator.CreateInstance(Type.GetTypeFromProgID(AU));
                                if (au != null && au is IMMSpecialAUStrategyEx)
                                {
                                    AUDictionaryMapping[classID][mmEditEvent.mmEventFeatureCreate].Add(au as IMMSpecialAUStrategyEx);
                                }
                            }
                        }
                        else if (param.Type.ToUpper() == "UPDATE")
                        {
                            string[] classIDs = Regex.Split(param.Value, ",");
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
                                object au = Activator.CreateInstance(Type.GetTypeFromProgID(AU));
                                if (au != null && au is IMMSpecialAUStrategyEx)
                                {
                                    AUDictionaryMapping[classID][mmEditEvent.mmEventFeatureUpdate].Add(au as IMMSpecialAUStrategyEx);
                                }
                            }
                        }
                        else if (param.Type.ToUpper() == "DELETE")
                        {
                            string[] classIDs = Regex.Split(param.Value, ",");
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
                                object au = Activator.CreateInstance(Type.GetTypeFromProgID(AU));
                                if (au != null && au is IMMSpecialAUStrategyEx)
                                {
                                    AUDictionaryMapping[classID][mmEditEvent.mmEventFeatureDelete].Add(au as IMMSpecialAUStrategyEx);
                                }
                            }
                        }
                    }
                }
                #endregion

                IWorkspace Editor = (IWorkspace)actionData.Version;
                IWorkspaceEdit wkspcEdit = (IWorkspaceEdit)actionData.Version;
                bool startedEditing = false;

                if (!wkspcEdit.IsBeingEdited())
                {
                    wkspcEdit.StartEditing(false);
                    startedEditing = true;
                    this.Log.Debug(ServiceConfiguration.Name, "Starting editing of workspace");
                }
                else
                {
                    this.Log.Debug(ServiceConfiguration.Name, "Editing already in progress.");
                }

                _processedList.Clear();
                bool startedOperation = false;

                try
                {
                    DesktopVersionDifference.ResetVersionDifference();
                    ID8List versionDifference = DesktopVersionDifference.GetVersionDifference(Editor);
                    startedOperation = true;
                    wkspcEdit.StartEditOperation();
                    ProcessD8List(versionDifference, mmEditEvent.mmEventFeatureUpdate);
                }
                catch (Exception e)
                {
                    success = false;
                    if (startedOperation) { wkspcEdit.AbortEditOperation(); }
                    if (startedEditing && wkspcEdit.IsBeingEdited())
                    {
                        wkspcEdit.StopEditing(false);
                    }
                    this.Log.Error(ServiceConfiguration.Name, "Error during PGE AU Processor. Message: " + e.Message);
                }
                finally
                {
                    if (startedOperation && wkspcEdit.IsBeingEdited()) { wkspcEdit.StopEditOperation(); }

                    if (startedEditing && wkspcEdit.IsBeingEdited())
                    {
                        wkspcEdit.StopEditing(true);
                    }
                }
            }
            catch (Exception e)
            {
                success = false;
            }
            finally
            {
                
            }
            return success;
        }

        public override bool Enabled(ActionType actionType, Actions gdbmAction, PostVersionType versionType)
        {
            bool enabled = false;

            if (gdbmAction == Actions.BeforeReconcile || gdbmAction == Actions.Reconcile || gdbmAction == Actions.Post)
            {
                enabled = true;
            }

            return enabled;
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
                    this.Log.Debug(ServiceConfiguration.Name, "List with Display Name:" + ((ID8ListItem)d8List).DisplayName + " already processed. Skipping processing");
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
                        this.Log.Debug(ServiceConfiguration.Name, "Processing item with display name:" + listItem.DisplayName);
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
                Type type = Type.GetTypeFromProgID("mmGeodatabase.MMAutoUpdater");
                object obj = Activator.CreateInstance(type);
                IMMAutoUpdater auFramework = obj as IMMAutoUpdater;
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
                this.Log.Debug(ServiceConfiguration.Name, "IRow with ClassID:RowID::" + recordID + " ready to process");
                _processedList.Add(recordID);
                retVal = false;
            }
            return retVal;
        }
    }
}
