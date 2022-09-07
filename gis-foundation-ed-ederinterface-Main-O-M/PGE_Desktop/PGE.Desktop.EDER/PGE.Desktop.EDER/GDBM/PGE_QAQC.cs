using System;
using Miner.Process.GeodatabaseManager.ActionHandlers;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using System.Runtime.InteropServices;
using PGE.Desktop.EDER.ValidationRules;
using PGE.Common.Delivery.Framework;
using Miner.Geodatabase.GeodatabaseManager.Serialization;
using System.Collections.Generic;

namespace PGE.Desktop.EDER.GDBM
{
    /// <summary>
    /// This Action handler will be responsible for executing all of the QA/QC rules logic.  It will write any errors encountered to a PGE
    /// QAQC session errors table for review and fixes from ArcMap by a user
    /// </summary>
    public class PGE_QAQC : PxActionHandler
    {
        public static String WipUsername = null;
        public static String WipPassword = null;
        public static IDictionary<string, IFeatureClass> Featureclasslist = new Dictionary<string, IFeatureClass>();
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
            bool exception = false;
            IWorkspaceEdit wkspcEdit = null;
            try
            {
                
                PGE_QAQC_Ext pObjExt = new PGE_QAQC_Ext(this.Log, ServiceConfiguration);

                this.Log.Info(ServiceConfiguration.Name, "Beginning QA/QC Processing");

                wkspcEdit = (IWorkspaceEdit)actionData.Version;
                IWorkspace Editor = (IWorkspace)actionData.Version;
                IFeatureWorkspace featWorkspace = Editor as IFeatureWorkspace;
                
                // Check Session Delete
                this.Log.Info(ServiceConfiguration.Name, "Check Session Delete");
                if (wkspcEdit.IsBeingEdited())
                {
                    //Delete current QAQC table results
                    pObjExt.DeleteQAQCSessionRows(Editor);

                    if (Checksessiondelete(actionData.Version) == true)
                    {

                        ValidationEngine.QAQCWorkspace = Editor;
                        ModelNameFacade.ModelNameManager = Miner.Geodatabase.ModelNameManager.Instance;

                        //Run QAQC
                        success = pObjExt.RunQAQC(Editor);
                        //Save the version edits
                        wkspcEdit.StopEditing(true);
                        wkspcEdit.StartEditing(false);


                    }
                    else
                    {
                        this.Log.Info(ServiceConfiguration.Name.ToString(), "No need to run QAQC");
                        //Save the version edits
                        wkspcEdit.StopEditing(true);
                        wkspcEdit.StartEditing(false);
                        success = false;
                    }

                }
            }
            catch (Exception ex)
            {
                this.Log.Error(ServiceConfiguration.Name, "Error executing QA/QC: " + ex.Message + " StackTrace: " + ex.StackTrace);
                success = false;
                exception = true;
                //Update by YXA6(TCS) to implement requirementid- EDREARCH0026
                //To resolve the reconcilation error issue -Stop Editing if it is already started
                if (wkspcEdit != null)
                {
                    if (wkspcEdit.IsBeingEdited())
                    {
                        wkspcEdit.StopEditing(false);
                    }
                }
            }
          
            DateTime endTime = DateTime.Now;
            TimeSpan timeSpan = endTime - startTime;
            this.Log.Info(ServiceConfiguration.Name, "QAQC ran for " + timeSpan.Hours + ":" + timeSpan.Minutes + ":" + timeSpan.Seconds);

            try
            {
                GdbmParameter[] pParamNew = new GdbmParameter[2];

                int iParamNew = 0;
                for (int iParam = 0; iParam < parameters.Length; ++iParam)
                {


                    if (parameters[iParam].Name.ToUpper() == "FromState".ToUpper() || parameters[iParam].Name.ToUpper() == "ToState".ToUpper())
                    {

                        pParamNew[iParamNew] = new GdbmParameter();
                        pParamNew[iParamNew].Name = parameters[iParam].Name;
                        pParamNew[iParamNew].Value = parameters[iParam].Value;
                        pParamNew[iParamNew].Type = parameters[iParam].Type;

                        ++iParamNew;
                    }

                }

                if (!success)
                {
                    Type transitionType = Type.GetType("Miner.Process.GeodatabaseManager.ActionHandlers.TransitionPxNodeState, Miner.Process, Version=10.8.0.0, Culture=neutral, PublicKeyToken=196beceb052ed5dc");
                    PxActionHandler transitionHandler = Activator.CreateInstance(transitionType, true) as PxActionHandler;
                    transitionHandler.ServiceConfiguration = this.ServiceConfiguration;
                    transitionHandler.ActionName = this.ActionName;
                    transitionHandler.ActionType = this.ActionType;
                    transitionHandler.Execute(actionData, pParamNew);
                }
                if (exception)
                {
                    foreach (GdbmParameter parameter in parameters)
                    {
                        if (parameter.Name.ToUpper() == "RESTARTSERVICE")
                        {
                            this.Log.Error(ServiceConfiguration.Name, "Exceptionally Restarting service " + ServiceConfiguration.Name + " using file path : " + parameter.Value);
                            System.Diagnostics.Process.Start(parameter.Value);
                            System.Threading.Thread.Sleep(10000);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.Log.Error(ServiceConfiguration.Name, "Failed to transition the node for QA/QC errors " + ex.Message);
            }
            return success;
        }

        public bool Checksessiondelete(IVersion pVersion)
        {
            bool retVal = true;
            try
            {
                IVersion defaultVersion = ((IVersionedWorkspace)pVersion).DefaultVersion;
                try 
                {
                    ITable pAppSettingsTable = ((IFeatureWorkspace)pVersion).OpenTable("EDGIS.PGE_DELETEFEATURE_INFO");
                    if (pAppSettingsTable != null)
                    {
                        int fldIdxName = pAppSettingsTable.Fields.FindField("OBJECTCLASSNAME");
                        int fldIdxValue = pAppSettingsTable.Fields.FindField("MAXFEATURECOUNT");
                        //this.Log.Debug("Querying the EDGIS.PGE_DELETEFEATUREINFO for severities");
                        ICursor pCursor = pAppSettingsTable.Search(null, false);
                        string settingName = "";
                        string settingValue = "";
                        IRow pRow = null;

                        //check if received cursor is not empty
                        while ((pRow = pCursor.NextRow()) != null)
                        {
                            IDifferenceCursor differenceCursor = null;
                            int objectID = -1; IRow differenceRow = null;
                            //Find Table
                            if (fldIdxName != -1)
                                settingName = pRow.get_Value(fldIdxName).ToString().ToLower();

                            if (fldIdxValue != -1)
                                settingValue = pRow.get_Value(fldIdxValue).ToString().ToLower();

                            //Populate the settings 

                            ITable firsttbl = (pVersion as IFeatureWorkspace).OpenTable(settingName);
                            ITable commonAncestortbl = (defaultVersion as IFeatureWorkspace).OpenTable(settingName);
                            differenceCursor = (firsttbl as IVersionedTable).Differences
                                  (commonAncestortbl, esriDifferenceType.esriDifferenceTypeDeleteNoChange, null);
                            differenceCursor.Next(out objectID, out differenceRow);
                            int deletecount = 0;
                            while (objectID != -1)
                            {
                                deletecount = deletecount + 1;
                                differenceCursor.Next(out objectID, out differenceRow);
                            }
                            Marshal.ReleaseComObject(differenceCursor);
                            this.Log.Info(ServiceConfiguration.Name.ToString(), "Delete Feature Count=" + deletecount);
                            if (deletecount > Convert.ToInt32(settingValue))
                            {
                                this.Log.Info(ServiceConfiguration.Name.ToString(), "Open the PGE_QAQC_Error Table");
                                ITable QAQCTable = GetQAQCTable((IWorkspace)pVersion);
                                this.Log.Info(ServiceConfiguration.Name.ToString(), "PGE_QAQC_Error Table has been opened");
                                IRow newRow = QAQCTable.CreateRow();
                                int sessionNameIdx = newRow.Fields.FindField("SESSIONNAME");
                                int errorsIdx = newRow.Fields.FindField("ERRORSLIST");
                                newRow.set_Value(sessionNameIdx, (pVersion.VersionName));
                                newRow.set_Value(errorsIdx, "Deleted feature count is exceeded the threshold limit:" + pVersion.VersionName);
                                newRow.Store();
                                this.Log.Info(ServiceConfiguration.Name.ToString(), "Deleted Feature Count Error is stored in the PGE_QAQC Table");
                                this.Log.Warn("Deleted Feature Count is more than the limit for table = " + settingName + " and  - DeleteFeatureCount:ThresholdLimit =  " + deletecount.ToString() + ":" + settingValue);
                                return false;
                            }

                        }
                        Marshal.FinalReleaseComObject(pCursor);
                    }

                }
                catch
                {
                    //Message config table not present in database
                    this.Log.Warn("Config table is not present in database = " + " EDGIS.PGE_DELETEFEATURE_INFO");
                    return false;
                }




            }
            catch (Exception ex)
            {
                this.Log.Error("Failed processing the actionhandler:" + this.Name, ex);
                return false;
            }
            //finally
            //{
            //    //Reset the QAQC filter back to valFilterTypeAll 
            //    ValidationEngine.Instance.FilterType = ValidationFilterType.valFilterTypeAll;
            //}
            return retVal;

        }


        public override bool Enabled(Miner.Geodatabase.GeodatabaseManager.ActionType actionType, Miner.Geodatabase.GeodatabaseManager.Actions gdbmAction, Miner.Geodatabase.GeodatabaseManager.PostVersionType versionType)
        {
            if (actionType == Miner.Geodatabase.GeodatabaseManager.ActionType.Post && gdbmAction == Miner.Geodatabase.GeodatabaseManager.Actions.DataValidation)
            {

                return true;

            }
            else
            {

                return false;
            }
        }
      
        #endregion

        #region Private Methods
        //1368195 change the protection level

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
