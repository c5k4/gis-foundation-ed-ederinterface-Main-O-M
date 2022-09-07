using ESRI.ArcGIS.Geodatabase;
using Miner.Geodatabase.GeodatabaseManager;
using Miner.Geodatabase.GeodatabaseManager.Serialization;
using Miner.Interop.Process;
using Miner.Process.GeodatabaseManager;
using Miner.Process.GeodatabaseManager.ActionHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGE.Desktop.EDER.GDBM
{
    class PGE_AfterRestrictReconcileActionhandler : PxActionHandler
    {
        IMMPxApplication _pxApplication = null;
        public PGE_AfterRestrictReconcileActionhandler()
        {
            this.Name = "PGE After Restrict Reconcile Process";
            this.Description = "Delete the entry from GDBM.GDBM_NO_RECONCILE_VERSIONS table to Restrict the reconcile if already done -once in 24 hours(configurable).";

        }
        public override bool Enabled(ActionType actionType, Actions gdbmAction, PostVersionType versionType)
        {
            if (gdbmAction == Actions.AfterReconcile)
            {

                return true;
            }
            return false;
        }

        protected override bool PxSubExecute(PxActionData actionData, GdbmParameter[] parameters)
        {
            #if DEBUG
                        System.Diagnostics.Debugger.Launch();
            #endif
            try
            {
                string sversionname = string.Empty;
                IWorkspace pworkspace = (IWorkspace )actionData.Version;
                string[] words = actionData.VersionName.Split('.');
                if (words.Length > 0)
                {
                    if (words.Length == 1)
                    {
                        sversionname = words[0];
                    }
                    else if(words.Length == 2)
                    {
                        sversionname = words[1];
                    }
                }
                
                string strquery= "delete from sde.GDBM_NO_RECONCILE_VERSIONS where NAME='" +  sversionname + "'";
                pworkspace.ExecuteSQL(strquery);
                this.Log.Info(ServiceConfiguration.Name, "Delete Query - " + strquery );

                this.Log.Info(ServiceConfiguration.Name, "Version name entry deleted from the table--" + sversionname);

            }
            catch (Exception ex)
            {
                this.Log.Info(ServiceConfiguration.Name, ex.Message.ToString());

            }
            return true;
        }

       
    }
}
