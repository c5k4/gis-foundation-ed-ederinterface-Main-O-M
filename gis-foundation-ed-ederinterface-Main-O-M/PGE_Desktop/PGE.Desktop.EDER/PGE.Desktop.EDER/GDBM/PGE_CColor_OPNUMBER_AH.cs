using Miner.Geodatabase.GeodatabaseManager;
using Miner.Geodatabase.GeodatabaseManager.Serialization;
using Miner.Process.GeodatabaseManager;
using Miner.Process.GeodatabaseManager.ActionHandlers;
using System;

namespace PGE.Desktop.EDER.GDBM
{
    class PGE_SSD_DEVICE_CColor_OPRTGNUM_AH : PxActionHandler

    {
        public PGE_SSD_DEVICE_CColor_OPRTGNUM_AH()
        {
            this.Name = "PGE CircuitColor,FeederType and JETOPNumber Updater";
            this.Description = "Updates the tables in the WIP database that track operating numbers for devices and job numbers ."
                + " Updates the circuit colors for any features with the PGE_CircuitColor class and field model names assigned."
                + " Also updates the Feeder Type field for feature classes with the PGE_FeederType class and field model names. ";
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

        [Obsolete]
        protected override bool PxSubExecute(PxActionData actionData, GdbmParameter[] parameters)
        {
            bool success1 = false;
            bool success2 = false;
            bool success = false;
            try
            {

                if ((new CommonFunctions()).CheckUsage(actionData.Version, this.Log, ServiceConfiguration) == true)
                {
                    return true;
                }
                this.Log.Info(ServiceConfiguration.Name, "Update Circuit Color AH Started");
                if ((new PGE_CircuitColorAH(this.Log, ServiceConfiguration)).UpdateCircuitColor(actionData.Version))
                {
                    this.Log.Info(ServiceConfiguration.Name, "Update Circuit Color AH Completed");

                    success1 = true;
                }
                
                this.Log.Info(ServiceConfiguration.Name, "Update Operating Number AH Started");
                if ((new PGE_OperatingNumberHandlerAH(this.Log, ServiceConfiguration)).UpdateOperatingNumber(actionData.Version))
                {
                    success2 = true;
                    this.Log.Info(ServiceConfiguration.Name, "Update Operating Number AH Completed");
                }
               
                if ((!success1) || (!success2))
                {

                    this.Log.Info(ServiceConfiguration.Name, "Change State from Post Queue to Post Error");
                    if (!(ChangeState(parameters, actionData)))
                    {
                        this.Log.Info(ServiceConfiguration.Name, "State Change is not done.");
                    }
                    else
                    {
                        this.Log.Info(ServiceConfiguration.Name, "State has been changed from Post Queue to Post Error");
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

                this.Log.Info(ServiceConfiguration.Name, "Change State from Post Queue to Post Error");
                if (!(ChangeState(parameters, actionData)))
                {
                    this.Log.Info(ServiceConfiguration.Name, "State Change is not done.");
                }
                else
                {
                    this.Log.Info(ServiceConfiguration.Name, "State has been changed from POst Queue to Post Error");
                }
                success = false;
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
