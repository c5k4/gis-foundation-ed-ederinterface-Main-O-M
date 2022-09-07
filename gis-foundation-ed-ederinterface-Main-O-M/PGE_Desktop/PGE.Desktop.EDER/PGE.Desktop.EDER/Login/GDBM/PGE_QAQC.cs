using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.Process.GeodatabaseManager.ActionHandlers;
using Telvent.Delivery.Diagnostics;
using System.Reflection;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using System.Runtime.InteropServices;
using Telvent.PGE.ED.Desktop.ValidationRules;
using System.Collections;
using System.Xml.Serialization;
using System.IO;
using Telvent.Delivery.Framework;
using log4net.Core;
using log4net;
using log4net.Appender;

namespace Telvent.PGE.ED.Desktop.GDBM
{
    /// <summary>
    /// This Action handler will be responsible for executing all of the QA/QC rules logic.  It will write any errors encountered to a PGE
    /// QAQC session errors table for review and fixes from ArcMap by a user
    /// </summary>
    public class PGE_QAQC : PxActionHandler
    {
        public static String WipUsername = null;
        public static String WipPassword = null;

        public PGE_QAQC()
        {
            this.Name = "PGE QAQC";
            this.Description = "Executes QAQC on the session and writes any errors encountered to the session errors table";
        }

        #region Overidden methods

        protected override bool PxSubExecute(Miner.Process.GeodatabaseManager.PxActionData actionData, Miner.Geodatabase.GeodatabaseManager.Serialization.GdbmParameter[] parameters)
        {

#if DEBUG
            System.Diagnostics.Debugger.Launch();
#endif
            DateTime startTime = DateTime.Now;
            bool success = true;
            try
            {
                this.Log.Info(ServiceConfiguration.Name, "Beginning QA/QC Processing");

                IWorkspace Editor = (IWorkspace)actionData.Version;
                IFeatureWorkspace featWorkspace = Editor as IFeatureWorkspace;
                IWorkspaceEdit wkspcEdit = (IWorkspaceEdit)actionData.Version;

                if (wkspcEdit.IsBeingEdited())
                {
                    ValidationEngine.QAQCWorkspace = Editor;
                    ModelNameFacade.ModelNameManager = Miner.Geodatabase.ModelNameManager.Instance;

                    //Delete current QAQC table results
                    DeleteQAQCSessionRows(Editor);

                    //Run QAQC
                    success = RunQAQC(Editor);

                    //Save the version edits
                    wkspcEdit.StopEditing(true);
                    wkspcEdit.StartEditing(false);
                }
            }
            catch (Exception ex)
            {
                this.Log.Error(ServiceConfiguration.Name, "Error executing QA/QC: " + ex.Message + " StackTrace: " + ex.StackTrace);
                success = false;
            }
            finally
            {

            }

            DateTime endTime = DateTime.Now;
            TimeSpan timeSpan = endTime - startTime;
            this.Log.Info(ServiceConfiguration.Name, "QAQC ran for " + timeSpan.Hours + ":" + timeSpan.Minutes + ":" + timeSpan.Seconds);

            try
            {
                if (!success)
                {
                    Type transitionType = Type.GetType("Miner.Process.GeodatabaseManager.ActionHandlers.TransitionPxNodeState, Miner.Process, Version=10.1.0.0, Culture=neutral, PublicKeyToken=196beceb052ed5dc");
                    PxActionHandler transitionHandler = Activator.CreateInstance(transitionType, true) as PxActionHandler;
                    transitionHandler.ServiceConfiguration = this.ServiceConfiguration;
                    transitionHandler.ActionName = this.ActionName;
                    transitionHandler.ActionType = this.ActionType;
                    transitionHandler.Execute(actionData, parameters);
                }
            }
            catch(Exception ex)
            {
                this.Log.Error(ServiceConfiguration.Name, "Failed to transition the node for QA/QC errors");
            }
            return success;
        }

        public override bool Enabled(Miner.Geodatabase.GeodatabaseManager.ActionType actionType, Miner.Geodatabase.GeodatabaseManager.Actions gdbmAction, Miner.Geodatabase.GeodatabaseManager.PostVersionType versionType)
        {
            if (actionType == Miner.Geodatabase.GeodatabaseManager.ActionType.Post && gdbmAction == Miner.Geodatabase.GeodatabaseManager.Actions.DataValidation)
            {
                return true;
            }

            return false;
        }

        #endregion

        #region Private Methods

        private bool RunQAQC(IWorkspace ws)
        {
            try
            {
                this.Log.Debug(ServiceConfiguration.Name, "Getting the version differences");

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
                Hashtable hshVersionDiffObjects = ValidationEngine.Instance.
                    GetVersionDifferences(
                        (IVersionedWorkspace)ws,
                        pDiffTypes, false, true, this.Log, ServiceConfiguration.Name);

                this.Log.Debug(ServiceConfiguration.Name, "Obtained version differences successfully");
                if (hshVersionDiffObjects.Count != 0)
                {
                    this.Log.Debug(ServiceConfiguration.Name, "Running QA QC on the Version Differences");
                    this.Log.Info(ServiceConfiguration.Name, "Found: " + hshVersionDiffObjects.Count.ToString() + " version difference objects");

                    //Run the QAQC 
                    ValidationEngine.Instance.FilterType =
                            ValidationFilterType.valFilterTypeErrorOnly;
                    ValidationEngine.Instance.SelectionType =
                        ValidationSelectionType.ValidationSelectionTypeVersionDiff;
                    Hashtable hshFullErrorsList = new Hashtable();
                    ValidationEngine.Instance.RunQAQCCustomised(
                        hshVersionDiffObjects, ref hshFullErrorsList, this.Log, ServiceConfiguration.Name);
                    
                    //Determine the validation errors 
                    if (hshFullErrorsList.Count != 0)
                    {
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
                    }                
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
            }

            return true;
        }

        private void DeleteQAQCSessionRows(IWorkspace ws)
        {
            this.Log.Info(ServiceConfiguration.Name, "Clearing out current QAQC results");
            //First delete all information from the session errors table so they don't get posted to default.

            ITable QAQCTable = GetQAQCTable(ws);
            IQueryFilter qf = new QueryFilterClass();
            QAQCTable.DeleteSearchedRows(qf); 
        }

        private ITable GetQAQCTable(IWorkspace ws)
        {
            IMMEnumTable QAQCTableEnum = Miner.Geodatabase.ModelNameManager.Instance.TablesFromModelNameWS(ws, SchemaInfo.Electric.ClassModelNames.SessionQAQC);
            QAQCTableEnum.Reset();
            ITable QAQCTable = null;
            QAQCTable = QAQCTableEnum.Next();
            if (QAQCTable == null)
            {
                throw new Exception("Unable to find table with model name: " + SchemaInfo.Electric.ClassModelNames.SessionQAQC);
            }
            return QAQCTable;
        }

        #endregion
    }
}
