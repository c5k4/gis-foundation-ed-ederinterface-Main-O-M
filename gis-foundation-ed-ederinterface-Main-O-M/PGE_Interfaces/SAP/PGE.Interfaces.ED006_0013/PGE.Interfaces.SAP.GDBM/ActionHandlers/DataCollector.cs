using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;

using Miner.Geodatabase.GeodatabaseManager;
using Miner.Geodatabase.GeodatabaseManager.Serialization;
using Miner.Process.GeodatabaseManager;
using Miner.Process.GeodatabaseManager.ActionHandlers;

using SAP = PGE.Interfaces.SAP;
using PGE.Interfaces.Integration.Framework;
using Miner.Geodatabase.GeodatabaseManager.ActionHandlers;

namespace PGE.Interfaces.SAP.GDBM.ActionHandlers
{
    /// <summary>
    /// Collects the information required to be sent to SAP from GIS edits.
    /// </summary>
    public class DataCollector : ActionHandler
    {
        /// <summary>
        /// Constructor. ActionHandler's Name and Description are set.
        /// </summary>
        public DataCollector()
            : base()
        {
            this.Name = "SAP Data Collector";
            this.Description = "Collects the information required to be sent to SAP from GIS edits";
        }

        private void LogMessage(string message)
        {
            this.Log.Info(ServiceConfiguration.Name, message);
        }

        /// <summary>
        /// Collect data and pass via ActionData to next ActionHandler.
        /// </summary>
        /// <param name="actionData">Pass data between ActionHandlers</param>
        /// <param name="parameters"></param>
        /// <returns>true if executed successfully, or false</returns>
        protected override bool SubExecute(ActionData actionData, GdbmParameter[] parameters)
        {
#if DEBUG
            System.Diagnostics.Debugger.Launch();
#endif
            Log.Info(ServiceConfiguration.Name, "Collecting edits in version " + actionData.VersionName);
            bool successfulRun = false;
            try
            {
                //Check if the parameter is set to not use FM 2.0 data.  In this case, the CircuitID, and CircuitID2 fields will be used directly
                //The Feeder Sync action handler should be assigned prior to this 
                if (parameters != null)
                {
                    for (int i = 0; i < parameters.Count(); i++)
                    {
                        GdbmParameter parameter = parameters[i];
                        if (parameter.Name.ToUpper() == "ReadFM20Data".ToUpper())
                        {
                            if (parameter.Value.ToUpper() == "FALSE") { BaseRowTransformer.UseFeederManager20Fields = false; }
                        }
                    }
                }

                IVersion editVersion = actionData.Version;

                IVersionedWorkspace versionedWorkspace = (IVersionedWorkspace)editVersion;
                IVersion targetVersion = versionedWorkspace.FindVersion(actionData.TargetVersionName);

                PGE.Interfaces.SAP.GISSAPIntegrator integrator = new PGE.Interfaces.SAP.GISSAPIntegrator(editVersion, targetVersion, LogMessage);
                bool initSucceessful = integrator.Initialize();
                

                if (initSucceessful == false)
                {
                    Log.Error(ServiceConfiguration.Name, "Failed collecting edits from version " + actionData.VersionName + ". GISSAPIntegrator not initialized! ");
                   //return false;
                }
                else
                {
                    // pass data to DataPersistor
                    successfulRun = integrator.Process();
                }
            }
            catch(Exception ex)
            {
                Log.Error(ServiceConfiguration.Name, "Failed collecting edits from version " + actionData.VersionName+":"+ex.Message , ex);
                //return false;
            }

#if DEBUG
            //actionData["SAPData"] = null;
#endif

            if (!successfulRun)
            {
                Log.Error(ServiceConfiguration.Name, "Errors were encountered during SAP Data Collector action handler");
                actionData.DeletePostingPriorityRow = true;
                actionData.HasQAErrors = true;
                actionData.QAStopsPost = true;


                // false, if Data Collector is running under Before Post event; if Data Validation, return true and trigger Validation Error
                // this will depend on workflow.
                if (string.Compare(this.ActionName, "Before Post") == 0)
                {
                    try
                    {
                        //Transition to "Post Error"
                        Type transitionType = Type.GetType("Miner.Process.GeodatabaseManager.ActionHandlers.TransitionPxNodeState, Miner.Process, Version=10.1.0.0, Culture=neutral, PublicKeyToken=196beceb052ed5dc");
                        PxActionHandler transitionHandler = Activator.CreateInstance(transitionType, true) as PxActionHandler;
                        transitionHandler.ServiceConfiguration = this.ServiceConfiguration;
                        transitionHandler.ActionName = this.ActionName;
                        transitionHandler.ActionType = this.ActionType;

                        transitionHandler.Execute(actionData, parameters);
                    }
                    catch (Exception ex)
                    {
                        this.Log.Error(ServiceConfiguration.Name, "Failed to transition the node for SAP Data Collector errors");
                    }

                    return false;
                }
            }
            else
            {
                Log.Error(ServiceConfiguration.Name, "Successfully executed SAP Data Collector action handler");
            }

            return true;
        }


        /// <summary>
        /// Enable your action handler for certain situations, e.g. BeforePost.
        /// </summary>
        /// <param name="actionType"></param>
        /// <param name="gdbmAction"></param>
        /// <param name="versionType"></param>
        /// <returns></returns>
       

        public override bool Enabled(Miner.Geodatabase.GeodatabaseManager.ActionType actionType, Actions gdbmAction, PostVersionType versionType)
        {
            // only enabled for BeforePost
            bool enabled = false;
            if (gdbmAction == Actions.BeforePost || gdbmAction == Actions.DataValidation)
            {
                enabled = true;
            }
            return enabled;
        }
    }
}
