using ESRI.ArcGIS.Geodatabase;
using Miner.Geodatabase.GeodatabaseManager;
using Miner.Geodatabase.GeodatabaseManager.Serialization;
using Miner.Process.GeodatabaseManager;
using Miner.Process.GeodatabaseManager.ActionHandlers;
using System;

namespace PGE.Desktop.EDER.GDBM
{
    class PGE_SSD_DEVICE_AH : PxActionHandler

    {
        public PGE_SSD_DEVICE_AH()
        {
            this.Name = "PGE SSD and DeviceGroup CircuitID Updater";
            this.Description = "Updates the device group circuit ID values and source side device values for any features that may have been updated by feeder manager 2.0";
        
        }

        public override bool Enabled(ActionType actionType, Actions gdbmAction, PostVersionType versionType)
        {
            if (gdbmAction == Actions.BeforePost)
            {
               
                return true;
            }
            else
            {
                return false;
            }
        }
   
        protected override bool PxSubExecute(PxActionData actionData, GdbmParameter[] parameters)
        {
            bool success = false;bool success1 = false;bool success2 = false;
            try
            {
                if ((new CommonFunctions()).CheckUsage(actionData.Version, this.Log, ServiceConfiguration) == true)
                {
                    return true;
                }
                this.Log.Info(ServiceConfiguration.Name, "SSD Update Started");

                if ((new PGE_SourceSideDeviceAH(this.Log, ServiceConfiguration)).updateSSD(actionData.Version))
                {
                    success1 = true;
                    this.Log.Info(ServiceConfiguration.Name, "SSD Update Completed"); 
                }
                
                this.Log.Info(ServiceConfiguration.Name, "Device Update Started");
                if ((new PGE_DeviceGroupCircuitIDAH(this.Log, ServiceConfiguration)).UpdateDeviceGroupCircuitID(actionData.Version))
                {
                    success2 = true;
                    this.Log.Info(ServiceConfiguration.Name, "Device Update Completed");
                }
                if ((!success1) || (!success2))
                {

                    this.Log.Info(ServiceConfiguration.Name, "Change State from DP to Post Error");
                    if (!(ChangeState(parameters, actionData)))
                    {
                        this.Log.Info(ServiceConfiguration.Name, "State Change is not done.");
                    }
                    else
                    {
                        this.Log.Info(ServiceConfiguration.Name, "State has been changed from DP to Post Error");
                    }

                }
                else
                {
                    success = true;
                }

            }
            catch (Exception ex)
            {
              
                this.Log.Info(ServiceConfiguration.Name, ex.Message.ToString() + " Exception-  " + ex.Message.ToString());
                this.Log.Info(ServiceConfiguration.Name, "Change State from DP to Post Error");
                if (!(ChangeState(parameters, actionData)))
                {
                    this.Log.Info(ServiceConfiguration.Name, "State Change is not done successfully");
                }
                else
                {
                    this.Log.Info(ServiceConfiguration.Name, "State has been changed from DP to Post Error");
                }
                success= false;
            }
            return success;
            
        }

        private bool ChangeState(GdbmParameter[] parameters, PxActionData actionData)
        {
            bool success = false;
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

                
                    this.Log.Info(ServiceConfiguration.Name, "On Error going to change the state");
                   // (new PGE_QAQC_Ext(null, null)).DeleteOldExistingData(actionData.VersionName);

                    Type transitionType = Type.GetType("Miner.Process.GeodatabaseManager.ActionHandlers.TransitionPxNodeState, Miner.Process, Version=10.8.0.0, Culture=neutral, PublicKeyToken=196beceb052ed5dc");
                    PxActionHandler transitionHandler = Activator.CreateInstance(transitionType, true) as PxActionHandler;
                    transitionHandler.ServiceConfiguration = this.ServiceConfiguration;
                    transitionHandler.ActionName = this.ActionName;
                    transitionHandler.ActionType = this.ActionType;
                    transitionHandler.Execute(actionData, pParamNew);
                    this.Log.Info(ServiceConfiguration.Name, "Change state successfully");
                    success = true;
            }
            catch (Exception ex)
            {
                this.Log.Info(ServiceConfiguration.Name, ex.Message.ToString() + " Exception-  " + ex.Message.ToString());
            }
            return success;
        }
    }
}
